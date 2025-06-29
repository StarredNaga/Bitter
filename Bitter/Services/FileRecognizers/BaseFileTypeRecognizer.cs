using System.Text;
using Bitter.Interfaces;

namespace Bitter.Services.FileRecognizers;

/// <summary>
/// Class for adding file extension to Encrypted data and get this extension from decrypted data
/// </summary>
public class BaseFileTypeRecognizer : IFileTypeRecognizer
{
    public string GetExtension(byte[] content)
    {
        if (content == null || content.Length == 0)
            throw new Exception("Content cannot be null or empty.");

        var separatorIndex = GetSeparatorIndex(content);

        if (separatorIndex < 0)
            throw new Exception("Content doesn't contain separator.");

        if (separatorIndex == 0)
            throw new Exception("Extension is empty.");

        var extension = content[..separatorIndex];

        return Encoding.UTF8.GetString(extension.ToArray());
    }

    private int GetSeparatorIndex(byte[] content)
    {
        for (var i = 0; i < content.Length; i++)
        {
            if (content[i] == 0x00)
                return i;
        }

        return -1;
    }

    public byte[] GetPureData(byte[] content)
    {
        if (content == null || content.Length == 0)
            throw new Exception("Content cannot be null or empty.");

        var separatorIndex = GetSeparatorIndex(content);

        if (separatorIndex < 0)
            throw new Exception("Content doesn't contain separator.");

        if (separatorIndex == 0)
            throw new Exception("Extension is empty.");

        if (separatorIndex >= content.Length - 1)
            return [];

        return content[(separatorIndex + 1)..];
    }

    public byte[] AddExtensionPrefix(byte[] fileData, string extension)
    {
        if (string.IsNullOrEmpty(extension))
            throw new Exception("Extension cannot be null or empty.");

        if (fileData == null || fileData.Length == 0)
            throw new Exception("Content cannot be null or empty.");

        var extensionBytes = Encoding.UTF8.GetBytes(extension);

        var result = new byte[fileData.Length + extensionBytes.Length + 1];

        Array.Copy(extensionBytes, 0, result, 0, extensionBytes.Length);

        result[extensionBytes.Length] = 0x00;

        Array.Copy(fileData, 0, result, extensionBytes.Length + 1, fileData.Length);

        return result;
    }
}
