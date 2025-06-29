using System.IO;
using Bitter.Interfaces;

namespace Bitter.Services.FileService;

/// <summary>
/// File service with using file stream
/// </summary>
public class StreamFileService : IFileService
{
    private const int BufferSize = 4096;

    public byte[] Read(string path)
    {
        if (string.IsNullOrEmpty(path))
            throw new ArgumentNullException(nameof(path));

        using var fs = new FileStream(
            path,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            BufferSize,
            FileOptions.SequentialScan
        );

        using var ms = new MemoryStream();

        fs.CopyTo(ms);

        return ms.ToArray();
    }

    public void Write(string path, byte[] content)
    {
        if (string.IsNullOrEmpty(path))
            throw new ArgumentNullException(nameof(path));

        if (content == null || content.Length == 0)
            throw new ArgumentNullException(nameof(content));

        using var fs = new FileStream(
            path,
            FileMode.Create,
            FileAccess.Write,
            FileShare.None,
            BufferSize
        );

        fs.Write(content, 0, content.Length);
    }
}
