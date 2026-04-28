namespace DataMount.App.Services;

public interface IPasswordEncoder
{
    bool Verify(string plain, string hash);
    string Hash(string plain);
}