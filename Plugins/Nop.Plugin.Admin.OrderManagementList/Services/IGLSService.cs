using System.Threading.Tasks;
using GLSReference;

namespace Nop.Plugin.Admin.OrderManagementList.Services
{
    public interface IGLSService
    {
        PakkeshopData GetParcelShopData(string parcelNumber);
    }
}