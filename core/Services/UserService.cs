using Microsoft.WindowsAzure.Storage.Table;
using System.Threading.Tasks;
using core.Models;

namespace core.Services
{
    // service to handle CRUD operations to User table
    public class UserService: AzureTableService<User>
    {
        public UserService(CloudTable userTable) : base(userTable)
        {
        }
    }
}