namespace Infrastructure.Auth
{
    public interface IJWTAuthenticationManager
    {
        string Authenticate(string username = null, string password = null);
    }
}