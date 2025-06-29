using Bitter.Interfaces;

namespace Bitter.Services.Ciphers;

/// <summary>
/// Async xor cipher
/// </summary>
public class AsyncXorCipher : IAsyncCipher
{
    private byte[] _key;

    public AsyncXorCipher(byte[] key)
    {
        if (key.Length == 0 || key == null)
            throw new ArgumentNullException("key");

        _key = key;
    }

    public void SetKey(byte[] key)
    {
        if (key.Length == 0 || key == null)
            throw new ArgumentNullException("key");

        _key = key;
    }

    public async Task<byte[]> EncryptAsync(byte[] input)
    {
        if (input == null || input.Length == 0)
            throw new ArgumentNullException("input");

        var result = new byte[input.Length];

        await Task.Run(() =>
        {
            for (var i = 0; i < input.Length; i++)
            {
                var keyByte = _key[i % _key.Length];

                result[i] = (byte)(input[i] ^ keyByte);
            }
        });

        return result;
    }

    public async Task<byte[]> DecryptAsync(byte[] input)
    {
        if (input == null || input.Length == 0)
            throw new ArgumentNullException("input");

        var result = new byte[input.Length];

        await Task.Run(() =>
        {
            for (var i = 0; i < input.Length; i++)
            {
                var keyByte = _key[i % _key.Length];

                result[i] = (byte)(input[i] ^ keyByte);
            }
        });

        return result;
    }

    public void Dispose()
    {
        Array.Clear(_key, 0, _key.Length);
    }
}
