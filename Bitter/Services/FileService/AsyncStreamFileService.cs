using System.IO;
using Bitter.Interfaces;

namespace Bitter.Services.FileService;

/// <summary>
/// Async file service with file stream using
/// </summary>
public class AsyncStreamFileService : IAsyncFileService
{
    private const int BufferSize = 4096;

    public async Task WriteAsync(string path, byte[] content)
    {
        if (string.IsNullOrEmpty(path))
            throw new ArgumentNullException(nameof(path));

        using var fs = new FileStream(
            path,
            FileMode.Create,
            FileAccess.Write,
            FileShare.None,
            BufferSize,
            FileOptions.Asynchronous
        );

        await fs.WriteAsync(content, 0, content.Length);
    }

    public async Task<byte[]> ReadAsync(string path)
    {
        if (string.IsNullOrEmpty(path))
            throw new ArgumentNullException(nameof(path));

        using var fs = new FileStream(
            path,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            BufferSize,
            FileOptions.Asynchronous
        );

        using var ms = new MemoryStream();

        await fs.CopyToAsync(ms);

        return ms.ToArray();
    }
}
