// Copyright (C) 2022  Kamil Bugno
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MyBlazorApp.Server.Data;
using MyBlazorApp.Server.Data.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MyBlazorApp.Server.BackgroundServices
{
    public class Worker : BackgroundService
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly string _serviceKey;
        private readonly IAudioService _audioService;
        private const int _numberOfTriesToGenerateAudio = 5;
        private const int _numberOfHoursInDay = 24;

        //We need to verify how many characters are sent to 
        //Azure Text-to-Speech API, because we have only
        //0.5M of free usage, and we don't want to run over it. 
        private const long _numberOfCharactersPerDay = 5000;
        public Worker(IServiceProvider serviceProvider, IConfiguration configuration, IAudioService audioService)
        {
            _applicationDbContext = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<ApplicationDbContext>();
            _serviceKey = configuration.GetSection("SpeechServiceKey").Value;
            _audioService = audioService;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var config = SpeechConfig.FromSubscription(_serviceKey, "eastus");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var languages = _applicationDbContext.Languages.Select(l => l).ToList();

                    foreach (var language in languages)
                    {  
                        config.SpeechSynthesisVoiceName = language.VoiceName;
                        await GenerateAudioForThisLanguage(config, language);
                    }
                }
                catch (OperationCanceledException)
                {
                    return;
                }
                await Task.Delay(10000, stoppingToken);
            }
        }

        private async Task GenerateAudioForThisLanguage(SpeechConfig config, Language language)
        {
            var words = RetrieveAllWords(language);
            foreach (var word in words.ToList())
            {
                try
                {
                    await GenerateAudioForThisWord(config, word);
                }
                catch
                {
                    //It can be ignored
                }
            }
        }

        private async Task GenerateAudioForThisWord(SpeechConfig config, (int WordId, string ExampleUse, string OriginalWord, string VoiceName) word)
        {
            var texts = ExtractTextToAutio(word.ExampleUse, word.OriginalWord);

            foreach (var text in texts)
            {
                var isOk = await GenerateAudioToTmp(config, text.Value);
                if (!isOk)
                {
                    throw new Exception();
                }
                _audioService.UploadAudio(word.WordId, text.Key);
            }
            SaveAudioGeneratedAsTrue(word);
        }

        private void SaveAudioGeneratedAsTrue((int WordId, string ExampleUse, string OriginalWord, string VoiceName) word)
        {
            var toUpdate = _applicationDbContext
                    .Words
                    .FirstOrDefault(w => w.Id == word.WordId);

            if (toUpdate != null)
            {
                toUpdate.HasAudioGenerated = true;
                _applicationDbContext.SaveChanges();
            }            
        }

        private IEnumerable<(int WordId, string ExampleUse, string OriginalWord, string VoiceName)> RetrieveAllWords(Language language) =>
            _applicationDbContext
                .Words
                .Include(w => w.Course)
                .Where(w => !w.HasAudioGenerated && w.Course.LanguageId == language.Id).ToList()
                .Select(s => 
                (
                    s.Id,
                    s.ExampleUse,
                    s.OriginalWord,
                    s.Course.Language.VoiceName
                ));
        

        private static Dictionary<WordType, string> ExtractTextToAutio(string exampleUse, string originalWord)
        {
            var dict = new Dictionary<WordType, string>();
            
            var start = exampleUse.IndexOf('*');
            var end = exampleUse.LastIndexOf('*');
            var blankExampleUse = exampleUse.Substring(0, start)
                + "......"
                + exampleUse.Substring(end + 1);

            dict.Add(WordType.BlankExampleUse, blankExampleUse);
            dict.Add(WordType.FullExampleUse, exampleUse.Replace("*", ""));
            dict.Add(WordType.OriginalWord, originalWord);

            return dict;
        }

        private async Task<bool> GenerateAudioToTmp(SpeechConfig config, string toAudio)
        {
            var i = 0;
            ResultReason result;
            do
            {
                result = await TryToGenerateAudio(config, toAudio);
                i++;
            } while (result != ResultReason.SynthesizingAudioCompleted && i < _numberOfTriesToGenerateAudio);

            return result == ResultReason.SynthesizingAudioCompleted;
        }

        private async Task<ResultReason> TryToGenerateAudio(SpeechConfig config, string toAudio)
        {
            AudioConfig audioConfigBlankExampleUse;
            SpeechSynthesizer synthesizerBlankExampleUse;
            var audioStats = _applicationDbContext.AudioStats.FirstOrDefault(a => a.Date.Date == DateTime.UtcNow.Date);
            var isNewEntry = false;

            if (audioStats == null)
            {
                audioStats = new AudioStats();
                isNewEntry = true;
            }

            if (audioStats.NumberOfCharacters + toAudio.Length > _numberOfCharactersPerDay)
            {
                await Task.Delay(TimeSpan.FromHours(_numberOfHoursInDay - DateTime.UtcNow.Hour));
                return ResultReason.Canceled;
            }

            audioStats.Date = DateTime.UtcNow.Date;
            audioStats.NumberOfCharacters += toAudio.Length;

            AddOrUpdateAudioStats(audioStats, isNewEntry);

            audioConfigBlankExampleUse = AudioConfig.FromWavFileOutput("./tmp.wav");
            synthesizerBlankExampleUse = new SpeechSynthesizer(config, audioConfigBlankExampleUse);
            var result = (await synthesizerBlankExampleUse.SpeakTextAsync(toAudio)).Reason;

            await Task.Delay(1000);
            audioConfigBlankExampleUse.Dispose();
            synthesizerBlankExampleUse.Dispose();
            return result;
        }

        private void AddOrUpdateAudioStats(AudioStats audioStats, bool isNewEntry)
        {
            if (isNewEntry)
            {
                _applicationDbContext.AudioStats.Add(audioStats);
            }
            else
            {
                _applicationDbContext.AudioStats.Update(audioStats);
            }
            _applicationDbContext.SaveChanges();
        }
    }

}
