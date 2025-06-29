namespace Bitter.Interfaces;

public interface IFileTypeRecognizer
{
    string GetExtension(byte[] content);
    
    byte[] GetPureData(byte[] content);

    byte[] AddExtensionPrefix(byte[] fileData, string extension);
}