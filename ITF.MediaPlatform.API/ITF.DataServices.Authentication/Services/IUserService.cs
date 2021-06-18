namespace ITF.DataServices.Authentication.Services
{
    public interface IUserService
    {
        int Authenticate(string userName, string password);
    }
}
