using FluentFTP;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Nop.Plugin.Admin.OrderManagementList.Services
{
    public class FTPService : IFTPService
    {
        private FtpClient _client;

        public void Initialize(string host, string username, string password)
        {
            _client = new FtpClient(host);

            // if you don't specify login credentials, we use the "anonymous" user account
            _client.Credentials = new NetworkCredential(username, password);
        }

        public void SendFile(string fullFilePath, string remoteFolder)
        {
            // begin connecting to the server
            _client.Connect();

            // upload a file
            _client.UploadFile(fullFilePath, remoteFolder);
        }
    }
}
