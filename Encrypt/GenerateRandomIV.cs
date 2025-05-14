using System.Security.Cryptography;

namespace Encrypt;

internal static class GenerateRandomIV
{
    internal static byte[] Generate()
    {
        using (var rng = RandomNumberGenerator.Create())
        {
            byte[] iv = new byte[16];
            rng.GetBytes(iv);
            return iv;
        }
    }
}