namespace Bitter.Interfaces;

public interface IFileService
{
    byte[] Read(string path);
    
    void Write(string path, byte[] content);
}