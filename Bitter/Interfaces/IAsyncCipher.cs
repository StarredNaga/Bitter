namespace Bitter.Interfaces;

public interface IAsyncCipher : IDisposable
{
    void SetKey(byte[] key);

    Task<byte[]> EncryptAsync(byte[] input);

    Task<byte[]> DecryptAsync(byte[] input);
}
