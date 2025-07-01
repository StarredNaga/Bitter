using Bitter.Interfaces;
using Bitter.Models;

namespace Bitter.Services.Ciphers;

/// <summary>
/// Sync xor cipher
/// </summary>
public class XorCipher : ICipher
{
    private byte[] _key;

    public XorCipher(EncryptionOptions options)
    {
        SetKey(options.Key);
    }

    public void SetKey(byte[] key)
    {
        if (key.Length == 0 || key == null)
            throw new ArgumentNullException("key");

        _key = key;
    }

    public byte[] Encrypt(byte[] input)
    {
        if (input == null || input.Length == 0)
            throw new ArgumentNullException("input");

        var result = new byte[input.Length];

        for (var i = 0; i < input.Length; i++)
        {
            var keyByte = _key[i % _key.Length];

            result[i] = (byte)(input[i] ^ keyByte);
        }

        return result;
    }

    public byte[] Decrypt(byte[] input)
    {
        if (input == null || input.Length == 0)
            throw new ArgumentNullException("input");

        var result = new byte[input.Length];

        for (var i = 0; i < input.Length; i++)
        {
            var keyByte = _key[i % _key.Length];

            result[i] = (byte)(input[i] ^ keyByte);
        }

        return result;
    }

    public void Dispose()
    {
        Array.Clear(_key, 0, _key.Length);
    }
}
