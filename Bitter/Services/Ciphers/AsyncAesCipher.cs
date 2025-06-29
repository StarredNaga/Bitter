using System.IO;
using System.Security.Cryptography;
using Bitter.Interfaces;

namespace Bitter.Services.Ciphers;

/// <summary>
/// Async Aes cipher
/// </summary>
public class AsyncAesCipher : IAsyncCipher
{
    private byte[] _key;

    private byte[] _iv;
    private byte[] _aesKey;

    public AsyncAesCipher(byte[] key)
    {
        SetKey(key);
    }

    public void SetKey(byte[] key)
    {
        if (key.Length == 0 || key == null)
            throw new ArgumentNullException("key");

        _key = key;

        DeriveKeyAndIv();
    }

    private void DeriveKeyAndIv()
    {
        _aesKey = SHA256.HashData(_key);
        _iv = MD5.HashData(_key);
    }

    public async Task<byte[]> EncryptAsync(byte[] input)
    {
        if (input == null || input.Length == 0)
            throw new ArgumentNullException("input", "Content can't be null or empty.");

        using var aes = Aes.Create();

        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;
        aes.Key = _aesKey;
        aes.IV = _iv;

        using var memoryStream = new MemoryStream();
        using var cryptoStream = new CryptoStream(
            memoryStream,
            aes.CreateEncryptor(),
            CryptoStreamMode.Write
        );

        await cryptoStream.WriteAsync(input, 0, input.Length);
        await cryptoStream.FlushFinalBlockAsync();

        return memoryStream.ToArray();
    }

    public async Task<byte[]> DecryptAsync(byte[] input)
    {
        if (input == null || input.Length == 0)
            throw new ArgumentNullException("input", "Content can't be null or empty.");

        using var aes = Aes.Create();

        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;
        aes.Key = _aesKey;
        aes.IV = _iv;

        using var memoryStream = new MemoryStream(input);
        using var cryptoStream = new CryptoStream(
            memoryStream,
            aes.CreateDecryptor(),
            CryptoStreamMode.Read
        );

        using var outputStream = new MemoryStream();
        await cryptoStream.CopyToAsync(outputStream);

        return outputStream.ToArray();
    }

    public void Dispose()
    {
        Array.Clear(_key, 0, _key.Length);
        Array.Clear(_iv, 0, _iv.Length);
        Array.Clear(_aesKey, 0, _aesKey.Length);
    }
}
