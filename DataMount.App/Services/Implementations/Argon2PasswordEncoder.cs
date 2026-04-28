using Isopoh.Cryptography.Argon2;

namespace DataMount.App.Services.Implementations;

public class Argon2PasswordEncoder : IPasswordEncoder
{
    public bool Verify(string plain, string hash)
    {
        return Argon2.Verify(hash, plain);
    }

    public string Hash(string plain)
    {
        return Argon2.Hash(plain);
    }
}