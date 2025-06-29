namespace Bitter.Models;

/// <summary>
/// Base settings
/// </summary>
/// <param name="Key">Key for Encrypt/Decrypt</param>
public record class Settings(byte[] Key);
