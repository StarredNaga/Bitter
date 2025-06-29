namespace Bitter.Models;

/// <summary>
/// Settings for Encrypt operation
/// </summary>
/// <param name="Key">Encrypt key</param>
/// <param name="InputPath">Input path to encrypt</param>
/// <param name="OutputPath">Output file to write encrypted data</param>
public record EncryptSettings(byte[] Key, string InputPath, string OutputPath);
