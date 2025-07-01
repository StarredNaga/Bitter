using System.Text;
using System.Windows;
using Bitter.Interfaces;
using Bitter.Models;
using Bitter.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using Path = System.IO.Path;

namespace Bitter;

public partial class MainWindow : Window
{
    private BitTool _bitTool;

    private IKeyGenerator _keyGenerator;

    private Configurations _configurations;

    private EncryptSettings _encryptSettings;
    private DecryptSettings _decryptSettings;
    private Settings _settings;

    public MainWindow()
    {
        InitializeComponent();

        _configurations = new Configurations();

        InitializeServices();
        SetupInitialSettings();
    }

    // Initializing services
    private void InitializeServices()
    {
        var services = _configurations.ConfigureServices();

        _bitTool = services.GetRequiredService<BitTool>();
        _keyGenerator = services.GetRequiredService<IKeyGenerator>();
    }

    // Set encryption key in ui and settings
    private void SetupInitialSettings()
    {
        var key = _configurations.ConfigureServices().GetRequiredService<EncryptionOptions>().Key;

        _settings = new Settings(key);
        _encryptSettings = new EncryptSettings(key, string.Empty, string.Empty);
        _decryptSettings = new DecryptSettings(key, string.Empty, string.Empty);

        KeyTextBox.Text = Encoding.UTF8.GetString(key);
    }

    /// <summary>
    /// Validate input data for encryption/decryption
    /// </summary>
    /// <param name="key">Encryption key</param>
    /// <param name="inputPath">Path to file with input data</param>
    /// <param name="outputPath">Path to file to write encrypted/decrypted data</param>
    /// <param name="operationType"></param>
    /// <returns>If an operation is valid</returns>
    private bool ValidateOperation(
        byte[] key,
        string inputPath,
        string outputPath,
        string operationType
    )
    {
        if (key == null || key.Length == 0)
        {
            ShowError($"{operationType} key is empty");
            return false;
        }

        if (string.IsNullOrWhiteSpace(inputPath))
        {
            ShowError("Input file is not selected");
            return false;
        }

        if (string.IsNullOrWhiteSpace(outputPath))
        {
            ShowError("Output file is not selected");
            return false;
        }

        return true;
    }

    /// <summary>
    ///  Shows error
    /// </summary>
    /// <param name="message">Error message that will be shown</param>
    private void ShowError(string message) =>
        MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);

    /// <summary>
    ///  Shows success
    /// </summary>
    /// <param name="message">Message that will be shown</param>
    private void ShowSuccess(string message) =>
        MessageBox.Show(message, "Success", MessageBoxButton.OK, MessageBoxImage.Information);

    private void EncryptInputFileButton_OnClick(object sender, RoutedEventArgs e)
    {
        // Get input file for encrypt
        var filePath = AskForFile(false);

        if (string.IsNullOrEmpty(filePath))
            return;

        // Set file path in UI
        EncryptInputFileBox.Text = filePath;

        // Set settings
        _encryptSettings = _encryptSettings with
        {
            InputPath = filePath,
        };
    }

    private void EncryptOutputFileButton_OnClick(object sender, RoutedEventArgs e)
    {
        // Get output file for Encrypt
        var filePath = AskForFile(true, defaultExtension: ".enc");

        if (string.IsNullOrEmpty(filePath))
            return;

        // Get extension
        var extension = Path.GetExtension(filePath);

        // Validate extension
        if (string.IsNullOrEmpty(extension) || extension != ".enc")
        {
            ShowError("Invalid file extension");

            return;
        }

        // Set file path in UI
        EncryptOutputFileBox.Text = filePath;

        // Set settings
        _encryptSettings = _encryptSettings with
        {
            OutputPath = filePath,
        };
    }

    private async void EncryptButton_OnClick(object sender, RoutedEventArgs e)
    {
        // Get data from settings
        var key = _encryptSettings.Key;
        var input = _encryptSettings.InputPath;
        var output = _encryptSettings.OutputPath;

        // Validate data
        if (!ValidateOperation(key, input, output, operationType: "Encryption"))
            return;

        // Set progress for progress bar
        var progress = new Progress<int>(x => EncryptProgressBar.Value = x);

        // Show log in UI
        EncryptLogBox.Text = "Encryption started.\n";

        try
        {
            // Encrypt file
            await _bitTool.EncryptFileAsync(input, output, progress);

            // Show logs in UI
            EncryptLogBox.Text += $"Progress - {EncryptProgressBar.Value}%\n";

            EncryptProgressBar.Value = 100;

            EncryptLogBox.Text += $"Progress - {EncryptProgressBar.Value}%\n";

            EncryptLogBox.Text += "Success Encryption";
        }
        catch (Exception ex)
        {
            // Log error in UI
            EncryptLogBox.Text += ex.Message;
        }
        finally
        {
            EncryptProgressBar.Value = 100;
        }
    }

    private void DecryptInputFileButton_OnClick(object sender, RoutedEventArgs e)
    {
        // Get input file for Decrypt
        var filePath = AskForFile(false, defaultExtension: ".enc");

        if (string.IsNullOrEmpty(filePath))
            return;

        // Get file extension
        var extension = Path.GetExtension(filePath);

        // Validate extension
        if (string.IsNullOrEmpty(extension) || extension != ".enc")
        {
            ShowError("Invalid file extension");

            return;
        }

        // Set file path in UI
        DecryptInputFileBox.Text = filePath;

        // Set settings
        _decryptSettings = _decryptSettings with
        {
            InputPath = filePath,
        };
    }

    private void DecryptOutputFileButton_OnClick(object sender, RoutedEventArgs e)
    {
        // Get output file for decrypt
        var filePath = AskForFile(true);

        if (string.IsNullOrEmpty(filePath))
            return;

        // Show file path int UI
        DecryptOutputFileBox.Text = filePath;

        // Set settings
        _decryptSettings = _decryptSettings with
        {
            OutputPath = filePath,
        };
    }

    private async void DecryptButton_OnClick(object sender, RoutedEventArgs e)
    {
        // Get data from settings
        var key = _decryptSettings.Key;
        var input = _decryptSettings.InputPath;
        var output = _decryptSettings.OutputPath;

        // Validate data
        if (!ValidateOperation(key, input, output, operationType: "Decryption"))
            return;

        // Progress for progress bar
        var progress = new Progress<int>(x => DecryptProgressBar.Value = x);

        // Log message in UI
        DecryptLogBox.Text = "Decryption started.\n";

        try
        {
            // Decrypt file
            await _bitTool.DecryptFileAsync(input, output, progress);

            // Show progress in UI
            DecryptLogBox.Text += $"Progress - {DecryptProgressBar.Value}%\n";

            DecryptProgressBar.Value = 100;

            DecryptLogBox.Text += $"Progress - {DecryptProgressBar.Value}%\n";

            DecryptLogBox.Text += "Decryption completed.\n";
        }
        catch (Exception ex)
        {
            // Show error in UI
            DecryptLogBox.Text += ex.Message + "\n";
        }
        finally
        {
            DecryptProgressBar.Value = 100;
        }
    }

    private void GenerateKeyButton_OnClick(object sender, RoutedEventArgs e)
    {
        // Generate new key
        var newKey = _keyGenerator.GenerateKey(20);

        // Show new key in UI
        KeyTextBox.Text = Encoding.UTF8.GetString(newKey);
    }

    private void ApplySettingsButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(KeyTextBox.Text))
        {
            ShowError("Please enter a key first.");
            return;
        }

        // Get new key
        var newKey = Encoding.UTF8.GetBytes(KeyTextBox.Text);

        // Set key
        _encryptSettings = _encryptSettings with
        {
            Key = newKey,
        };
        _decryptSettings = _decryptSettings with { Key = newKey };
        _settings = new Settings(newKey);

        _bitTool.SetKey(newKey);

        // Show success
        ShowSuccess("Settings have been applied.");
    }

    /// <summary>
    ///  Shows open file dialog/save file dialog
    /// </summary>
    /// <param name="isSaveDialog">If u want to shown save file dialog</param>
    /// <param name="defaultExtension">Default file extension for dialog</param>
    /// <returns>File path that user pick or empty string if user canceled</returns>
    private string AskForFile(bool isSaveDialog, string? defaultExtension = null)
    {
        FileDialog fileDialog = isSaveDialog ? new SaveFileDialog() : new OpenFileDialog();

        if (!string.IsNullOrEmpty(defaultExtension))
            fileDialog.DefaultExt = defaultExtension;

        var result = fileDialog.ShowDialog();

        if (!result.HasValue || !result.Value)
            return string.Empty;

        return fileDialog.FileName;
    }
}
