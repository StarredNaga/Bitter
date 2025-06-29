using System.IO;
using Bitter.Interfaces;

namespace Bitter;

/// <summary>
///  Class for encrypting/decrypting files
/// </summary>
public class BitTool
{
    // Services for encryption
    private readonly ICipher _cipher;
    private readonly IAsyncCipher _asyncCipher;

    // Services for reading/writing file data
    private readonly IFileService _fileService;
    private readonly IAsyncFileService _asyncFileService;

    // Service for get/add extension to file data
    private readonly IFileTypeRecognizer _typeRecognizer;

    public BitTool(
        ICipher cipher,
        IFileService fileService,
        IFileTypeRecognizer typeRecognizer,
        IAsyncCipher asyncCipher,
        IAsyncFileService asyncFileService
    )
    {
        _cipher = cipher;
        _fileService = fileService;
        _typeRecognizer = typeRecognizer;
        _asyncCipher = asyncCipher;
        _asyncFileService = asyncFileService;
    }

    /// <summary>
    /// Set key to all services
    /// </summary>
    /// <param name="key">New key for encryption</param>
    /// <exception cref="ArgumentNullException">Key is null or empty</exception>
    public void SetKey(byte[] key)
    {
        if (key == null || key.Length == 0)
            throw new ArgumentNullException("key");

        _cipher.SetKey(key);
    }

    /// <summary>
    ///  Encrypting file
    /// </summary>
    /// <param name="inputFile">File with data to encrypt</param>
    /// <param name="outputFile">File to write encrypted data</param>
    /// <param name="progress">Progress to show in UI</param>
    /// <exception cref="ArgumentException">If input/output file path is null or empty</exception>
    /// <exception cref="Exception">Encryption error</exception>
    public void EncryptFile(string inputFile, string outputFile, IProgress<int> progress)
    {
        if (string.IsNullOrEmpty(inputFile))
            throw new ArgumentException("Input file cannot be null or empty.");

        if (string.IsNullOrEmpty(outputFile))
            throw new ArgumentException("Output file cannot be null or empty.");

        // Reading data from input file
        var fileData = _fileService.Read(inputFile);

        progress.Report(25);

        // Get input file extension
        var extension = Path.GetExtension(inputFile);

        // Adding extension to data
        var dataToEncrypt = _typeRecognizer.AddExtensionPrefix(fileData, extension);

        progress.Report(25);

        using (_cipher)
        {
            // Encrypt data
            var encryptedData = _cipher.Encrypt(dataToEncrypt);

            progress.Report(25);

            if (encryptedData.Length == 0)
                throw new Exception("Encrypted Data cannot be empty.");

            // Write data to output file
            _fileService.Write(outputFile, encryptedData);

            progress.Report(25);
        }
    }

    /// <summary>
    ///  Decrypting file
    /// </summary>
    /// <param name="inputFile">File with data to decrypt</param>
    /// <param name="outputFile">File to write decrypted data</param>
    /// <param name="progress">Progress to show in UI</param>
    /// <exception cref="ArgumentException">If input/output file path is null or empty</exception>
    /// <exception cref="Exception">Encryption error or can't get extension from decrypted data</exception>
    public void DecryptFile(string inputFile, string outputFile, IProgress<int> progress)
    {
        if (string.IsNullOrEmpty(inputFile))
            throw new ArgumentException("Input file cannot be null or empty.");

        if (string.IsNullOrEmpty(outputFile))
            throw new ArgumentException("Output file cannot be null or empty.");

        // Read data from encrypted file
        var encryptedData = _fileService.Read(inputFile);

        progress.Report(25);

        if (encryptedData.Length == 0)
            throw new Exception("Encrypted data reading error.");

        using (_cipher)
        {
            // Decrypt data
            var fileData = _cipher.Decrypt(encryptedData);

            progress.Report(25);

            // Get file extension from decrypted data
            var extension = _typeRecognizer.GetExtension(fileData);

            progress.Report(25);

            if (string.IsNullOrEmpty(extension))
                throw new Exception("Extension reading error.");

            // Get data without file extension from decrypted data
            var data = _typeRecognizer.GetPureData(fileData);

            if (data.Length == 0)
                throw new Exception("Data decryption error.");

            // Write decrypted data to file with right extension
            _fileService.Write(Path.ChangeExtension(outputFile, extension), data);

            progress.Report(25);
        }
    }

    /// <summary>
    ///  Async file encrypting
    /// </summary>
    /// <param name="inputFile">File with data to encrypt</param>
    /// <param name="outputFile">File to write encrypted data</param>
    /// <param name="progress">Progress to show in UI</param>
    /// <exception cref="ArgumentException">If input/output file path is null or empty</exception>
    /// <exception cref="Exception">Encryption error</exception>
    public async Task EncryptFileAsync(string inputFile, string outputFile, IProgress<int> progress)
    {
        // Look in EncryptFile method
        if (string.IsNullOrEmpty(inputFile))
            throw new ArgumentException("Input file cannot be null or empty.");

        if (string.IsNullOrEmpty(outputFile))
            throw new ArgumentException("Output file cannot be null or empty.");

        var fileData = await _asyncFileService.ReadAsync(inputFile);

        progress.Report(25);

        var extension = Path.GetExtension(inputFile);

        var dataToEncrypt = _typeRecognizer.AddExtensionPrefix(fileData, extension);

        progress.Report(25);

        using (_cipher)
        {
            var encryptedData = await _asyncCipher.EncryptAsync(dataToEncrypt);

            progress.Report(25);

            if (encryptedData.Length == 0)
                throw new Exception("Encrypted Data cannot be empty.");

            await _asyncFileService.WriteAsync(outputFile, encryptedData);

            progress.Report(25);
        }
    }

    /// <summary>
    ///  Async file decrypting
    /// </summary>
    /// <param name="inputFile">File with data to decrypt</param>
    /// <param name="outputFile">File to write decrypted data</param>
    /// <param name="progress">Progress to show in UI</param>
    /// <exception cref="ArgumentException">If input/output file path is null or empty</exception>
    /// <exception cref="Exception">Encryption error or can't get extension from decrypted data</exception>
    public async Task DecryptFileAsync(string inputFile, string outputFile, IProgress<int> progress)
    {
        // Look in DecryptFile method
        if (string.IsNullOrEmpty(inputFile))
            throw new ArgumentException("Input file cannot be null or empty.");

        if (string.IsNullOrEmpty(outputFile))
            throw new ArgumentException("Output file cannot be null or empty.");

        var encryptedData = await _asyncFileService.ReadAsync(inputFile);

        progress.Report(25);

        if (encryptedData.Length == 0)
            throw new Exception("Encrypted data reading error.");

        using (_cipher)
        {
            var fileData = await _asyncCipher.DecryptAsync(encryptedData);

            progress.Report(25);

            var extension = _typeRecognizer.GetExtension(fileData);

            progress.Report(25);

            if (string.IsNullOrEmpty(extension))
                throw new Exception("Extension reading error.");

            var data = _typeRecognizer.GetPureData(fileData);

            if (data.Length == 0)
                throw new Exception("Data decryption error.");

            await _asyncFileService.WriteAsync(Path.ChangeExtension(outputFile, extension), data);

            progress.Report(25);
        }
    }
}
