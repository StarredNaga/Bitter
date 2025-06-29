using System.IO;
using Bitter.Interfaces;

namespace Bitter.Services.FileService;

/// <summary>
/// Base file service without using file stream
/// </summary>
public class BaseFileService : IFileService
{
    public byte[] Read(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("Path cannot be empty");

        if (!File.Exists(path))
            throw new FileNotFoundException($"File not found: {path}");

        return File.ReadAllBytes(path);
    }

    public void Write(string path, byte[] content)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("Path cannot be empty");

        if (content == null)
            throw new ArgumentNullException(nameof(content));

        var directory = Path.GetDirectoryName(path);

        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        File.WriteAllBytes(path, content);
    }
}
