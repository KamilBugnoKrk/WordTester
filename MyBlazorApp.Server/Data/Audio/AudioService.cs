
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

using Microsoft.Extensions.Configuration;
using System;
using System.Net;

namespace MyBlazorApp.Server.Data.Audio
{

    public class AudioService : IAudioService
    {
        private readonly IConfiguration _configuration;

        public AudioService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string RetrieveAudio(int wordId, WordType wordType)
        {
            using var client = new WebClient();
            client.Credentials = new NetworkCredential(_configuration.GetSection("FTPName").Value,
               _configuration.GetSection("FTPPassword").Value);
            var bytes = client.DownloadData($"{_configuration.GetSection("FTPURL").Value}{wordId}-{wordType}.wav");
            string base64String = Convert.ToBase64String(bytes);
            
            return base64String;
        }

        public void UploadAudio(int wordId, WordType wordType)
        {
            using var client = new WebClient();

            client.Credentials = new NetworkCredential(_configuration.GetSection("FTPName").Value,
                _configuration.GetSection("FTPPassword").Value);
            client.UploadFile($"{_configuration.GetSection("FTPURL").Value}{wordId}-{wordType}.wav",
                WebRequestMethods.Ftp.UploadFile, "./tmp.wav");
        }

        public void DeleteAudio(int wordId)
        {
            RemoveFromFTP(wordId, WordType.FullExampleUse);
            RemoveFromFTP(wordId, WordType.OriginalWord);
            RemoveFromFTP(wordId, WordType.BlankExampleUse);
        }

        private void RemoveFromFTP(int wordId, WordType wordType)
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest
                            .Create($"{_configuration.GetSection("FTPURL").Value}{wordId}-{wordType}.wav");
            request.Method = WebRequestMethods.Ftp.DeleteFile;
            request.Credentials = new NetworkCredential(
                _configuration.GetSection("FTPName").Value, 
                _configuration.GetSection("FTPPassword").Value);

            using FtpWebResponse response = (FtpWebResponse)request.GetResponse();
        }
    }
}
