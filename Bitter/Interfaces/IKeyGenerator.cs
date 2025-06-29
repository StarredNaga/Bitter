namespace Bitter.Interfaces;

public interface IKeyGenerator
{
    byte[] GenerateKey(int length);
}
