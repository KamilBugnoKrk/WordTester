// Copyright (C) 2021  Kamil Bugno
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
using Microsoft.Extensions.Logging;
using MyBlazorApp.Server.Data;
using MyBlazorApp.Server.Data.Audio;
using MyBlazorApp.Server.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyBlazorApp.Server.BackgroundServices
{
    public class Worker : BackgroundService
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IConfiguration _configuration;
        private readonly IAudioService _audioService;
        public Worker(IServiceProvider serviceProvider, IConfiguration configuration, IAudioService audioService)
        {
            _applicationDbContext = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<ApplicationDbContext>();
            _configuration = configuration;
            _audioService = audioService;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var words = _applicationDbContext
                        .Words
                        .Include(w => w.Course)
                        .Where(w => !w.HasAudioGenerated )
                        .Select(s => new { 
                            WordId = s.Id, 
                            ExampleUse = s.ExampleUse, 
                            OriginalWord = s.OriginalWord, 
                            LanguageName = s.Course.Language.VoiceName 
                        });

                    var serviceKey = _configuration.GetSection("SpeechServiceKey").Value;
                    var config = SpeechConfig.FromSubscription(serviceKey, "eastus");
                    config.SpeechSynthesisVoiceName = "en-GB-LibbyNeural";

                    foreach (var word in words.ToList())
                    {
                        try
                        {
                            var texts = ExtractTextToAutio(word.WordId, word.ExampleUse, word.OriginalWord);

                            foreach (var item in texts)
                            {
                                await GenerateAudioToTmp(config, item.Value);
                                _audioService.UploadAudio(word.WordId, item.Key);

                                var toUpdate = _applicationDbContext
                                    .Words
                                    .FirstOrDefault(w => w.Id == word.WordId);

                                if (toUpdate != null)
                                {
                                    toUpdate.HasAudioGenerated = true;                                   
                                }                                
                            }
                            _applicationDbContext.SaveChanges();
                        }
                        catch (Exception e)
                        {

                        }                       
                    }
                    await Task.Delay(100000, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    return;
                }
            }
        }

        private static Dictionary<WordType, string> ExtractTextToAutio(int wordId, string exampleUse, string originalWord)
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

        private static async Task GenerateAudioToTmp(SpeechConfig config, string toAudio)
        {
            ResultReason result = ResultReason.NoMatch;
            var i = 0;
            do
            {
                using var audioConfigBlankExampleUse = AudioConfig.FromWavFileOutput("./tmp.wav");
                using var synthesizerBlankExampleUse = new SpeechSynthesizer(config, audioConfigBlankExampleUse);
                result = (await synthesizerBlankExampleUse.SpeakTextAsync(toAudio)).Reason;

                await Task.Delay(1000);
                audioConfigBlankExampleUse.Dispose();
                synthesizerBlankExampleUse.Dispose();
                i++;

            } while (result != ResultReason.SynthesizingAudioCompleted & i < 5);
        }
    }

}
