# Bitter - File Encryption Tool

![WPF Application](https://img.shields.io/badge/.NET%20WPF-Application-blueviolet)
![Platform](https://img.shields.io/badge/Platform-Windows-informational)

![Encryption page](https://github.com/StarredNaga/Bitter/blob/master/Bitter/Images/EncryptPage.bmp)
![Decryption page](https://github.com/StarredNaga/Bitter/blob/master/Bitter/Images/DecryptPage.bmp)
![Settings page](https://github.com/StarredNaga/Bitter/blob/master/Bitter/Images/SettingsPage.bmp)

**Bitter** is a reliable desktop application developed in C# and WPF designed for encrypting and decrypting files using modern cryptographic algorithms. Ensure the security of your data with an easy-to-use interface for Windows.

---

## ✨ Main Features
- **Support for multiple encryption algorithms**: AES-256 (industry standard) and XOR cipher
- **Password protection**: key generation using PBKDF2 and SHA-256
- **Flexible file processing**: synchronous and asynchronous operation
- **Modular architecture**: easily extendable via dependency injection
- **File type detection**: preserves original extensions during encryption
- **Intuitive interface**: minimalist design in white tones

---

## 🚀 Getting Started

### Requirements
- Windows 7 or newer
- [.NET 6 Desktop Runtime](https://dotnet.microsoft.com/download/dotnet/6.0)

### How to use
1. Launch the application.
2. Select a file for encryption or decryption.
3. Enter your password in settings or use default.
4. Click "Encrypt" or "Decrypt".
5. Specify the save location.

```mermaid
graph LR
A[Launch Bitter] --> B[Select file]
B --> C[Enter password]
C --> D[Choose operation: encrypt/decrypt]
D --> E[Save result]
```

---

## 🧠 Architecture Overview

### Configuration

- Uses Dependency Injection configured via `appsettings.json`
- Modular structure for easy addition of new components
- Simple replacement of services and algorithms

### Encryption

#### Algorithm interfaces:
- `ICipher` — synchronous encryption/decryption operations
- `IAsyncCipher` — asynchronous versions of the same operations

#### Implemented algorithms:
- AES-256 (CBC mode with PKCS7 padding)
- XOR cipher

#### Extensibility:
Add custom algorithms by implementing the relevant interfaces.

### File Handling

#### Processing modes:
- Synchronous (`IFileService`)
- Asynchronous (`IAsyncFileService`)

#### Stream processing:
Uses stream reading/writing for efficiency and to preserve file extensions during encryption.

### Key Generation

- Interface `IKeyGenerator` for creating cryptographic keys
- Implementation of PBKDF2 with configurable iterations
- Ability to add custom key generators

### File Type Detection

- Interface `IFileTypeRecognizer` preserves original file extensions
- Automatically assigns extensions upon decryption
- Built-in support without additional configuration

---

## ➕ Extending Functionality

To add your own components:

1. Implement the required interface (`ICipher`, `IFileService`, etc.)
2. Register them in the application's configuration, e.g.:

```csharp
services.AddSingleton<ICipher, YourCustomCipher>();
services.AddTransient<IFileService, YourFileService>();
```
