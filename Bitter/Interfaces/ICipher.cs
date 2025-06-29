namespace Bitter.Interfaces;

public interface ICipher : IDisposable
{
    void SetKey(byte[] key);

    byte[] Encrypt(byte[] input);

    byte[] Decrypt(byte[] input);
}
