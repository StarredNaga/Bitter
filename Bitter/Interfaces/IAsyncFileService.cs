namespace Bitter.Interfaces;

public interface IAsyncFileService
{
    Task WriteAsync(string path, byte[] content);

    Task<byte[]> ReadAsync(string path);
}
