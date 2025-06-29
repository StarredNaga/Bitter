using System.IO;
using System.Text;
using Bitter.Interfaces;
using Bitter.Services.Ciphers;
using Bitter.Services.FileRecognizers;
using Bitter.Services.FileService;
using Bitter.Services.KeyGenerators;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Bitter.Services;

/// <summary>
/// Configuration class
/// </summary>
public class Configurations
{
    private IServiceCollection _services;

    private IConfiguration _configuration;

    public Configurations()
    {
        _services = new ServiceCollection();

        var filePath = Directory.GetCurrentDirectory();

        // Add appsettings.json file to configurations
        var builder = new ConfigurationBuilder()
            .SetBasePath(filePath)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        _configuration = builder.Build();

        _services.AddSingleton(_configuration);
    }

    // Adding services
    public IServiceProvider ConfigureServices()
    {
        var key = _configuration["Encryption:Key"];

        if (string.IsNullOrEmpty(key))
            throw new Exception("Encryption Key is missing");

        _services
            .AddSingleton(Encoding.UTF8.GetBytes(key)) // Encryption/Decryption key
            .AddSingleton<IKeyGenerator, BaseKeyGenerator>() // Key generator
            .AddSingleton<IAsyncCipher, AsyncXorCipher>() // Async cipher
            .AddSingleton<ICipher, AesCipher>() // Sync cipher
            .AddSingleton<IFileService, StreamFileService>() // Sync file service
            .AddSingleton<IAsyncFileService, AsyncStreamFileService>() // Async file service
            .AddSingleton<IFileTypeRecognizer, BaseFileTypeRecognizer>() // File recognizer
            .AddSingleton<BitTool>();

        return _services.BuildServiceProvider();
    }
}
