using Microsoft.WindowsAzure.Storage.Table;
using System.Threading.Tasks;
using core.Models;

namespace core.Services
{
    public class UserService: AzureTableService<User>
    {
        public UserService(CloudTable userTable) : base(userTable)
        {
        }
    }
}