using FluentFTP;
using System.IO;
using System.Net;

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

        public void SendFile(string fullFilePath, string remotePath)
        {            
            _client.Connect();            
            _client.UploadFile(fullFilePath, remotePath);
            _client.Disconnect();
        }

        public string GetTrackingNumber(string localFolderPath, string remotePath, int orderId)
        {
            _client.Connect();
            FtpListItem[] statusFiles = _client.GetListing(remotePath);
            for (int i = 0; i < statusFiles.Length; i++)
            {
                _client.DownloadFile(localFolderPath + "\\" + statusFiles[i].Name, statusFiles[i].FullName);
            }
            _client.Disconnect();

            foreach (string fileName in Directory.GetFiles(localFolderPath))
            {
                if (!fileName.StartsWith(localFolderPath + "\\stat_"))
                    continue;

                string[] lines = File.ReadAllLines(fileName);
                foreach (var line in lines)
                {
                    string[] trackId = new string[3];
                    trackId[0] = line.Substring(0, line.IndexOf(" ")).Trim();
                    trackId[1] = line.Substring(line.IndexOf(" ")).Trim();
                    trackId[2] = fileName;

                    if(trackId[1] == orderId.ToString())
                    {
                        return trackId[0];
                    }
                }
            }
            
            return string.Empty;
        }       
    }
}
