using System.Collections.Generic;

namespace Nop.Plugin.Admin.OrderManagementList.Services
{
    public interface IFTPService
    {
        void SendFile(string fullFilePath, string remoteFolder);

        void Initialize(string host, string username, string password);

        string GetTrackingNumber(string localFolderPath, string remotePath, int orderId);

        void CleanupGLSStatusFiles(string remotePath, int daysToKeep);
    }
}