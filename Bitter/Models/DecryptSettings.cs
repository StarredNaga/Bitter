namespace Bitter.Models;

/// <summary>
/// Settings for Decrypt operation
/// </summary>
/// <param name="Key">Decrypt key</param>
/// <param name="InputPath">Input path to decrypt</param>
/// <param name="OutputPath">Output file to write decrypted data</param>
public record class DecryptSettings(byte[] Key, string InputPath, string OutputPath);
