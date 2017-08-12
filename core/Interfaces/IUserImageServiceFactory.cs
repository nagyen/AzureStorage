using core.Services;

namespace core.Interfaces
{
    public interface IUserImageServiceFactory
    {
        UserImageService GetUserImageService();
    }
}