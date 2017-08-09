using System.Threading.Tasks;
using core.Services;

namespace core.Interfaces
{
    public interface IUserServiceFactory
    {
        UserService GetUserService();
    }
}