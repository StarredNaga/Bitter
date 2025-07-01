using System.IO;
using System.Security.Cryptography;
using Bitter.Interfaces;
using Bitter.Models;

namespace Bitter.Services.Ciphers;

/// <summary>
///  Sync Aes cipher
/// </summary>
public class AesCipher : ICipher
{
    private byte[] _key;

    private byte[] _iv;
    private byte[] _aesKey;

    public AesCipher(EncryptionOptions options)
    {
        SetKey(options.Key);
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
        _iv = MD5.HashData(_key);

        var deriveBytes = new PasswordDeriveBytes(_key, _iv);

        _aesKey = deriveBytes.CryptDeriveKey("TripleDES", "SHA256", 192, _iv.Take(8).ToArray());
    }

    private void UpdateIv()
    {
        _iv = MD5.HashData(_key);
    }

    public byte[] Encrypt(byte[] input)
    {
        if (input == null || input.Length == 0)
            throw new ArgumentNullException("input", "Content can't be null or empty.");

        UpdateIv();

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

        cryptoStream.Write(input, 0, input.Length);
        cryptoStream.FlushFinalBlock();

        return memoryStream.ToArray();
    }

    public byte[] Decrypt(byte[] input)
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
        cryptoStream.CopyTo(outputStream);

        return outputStream.ToArray();
    }

    public void Dispose()
    {
        Array.Clear(_key, 0, _key.Length);
        Array.Clear(_iv, 0, _iv.Length);
        Array.Clear(_aesKey, 0, _aesKey.Length);
    }
}
