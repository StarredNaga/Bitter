using System.Text;
using Bitter.Interfaces;
using Org.BouncyCastle.Security;

namespace Bitter.Services.KeyGenerators;

/// <summary>
/// Base Encryption/Decryption key generator
/// </summary>
public class BaseKeyGenerator : IKeyGenerator
{
    private const string ValidCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

    public byte[] GenerateKey(int length)
    {
        var random = new SecureRandom();
        var builder = new StringBuilder();

        for (var i = 0; i < length; i++)
        {
            builder.Append(ValidCharacters[random.Next(0, ValidCharacters.Length)]);
        }

        return Encoding.UTF8.GetBytes(builder.ToString());
    }
}
