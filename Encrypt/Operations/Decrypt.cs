using System.Security.Cryptography;
using System.Text;

namespace Encrypt;

public class Decrypt
{
    public static void Decryption()
    {
        Console.Write("Çözülecek dosyanın adresini gir (.encrypted): ");
        string encryptedFilePath = Console.ReadLine();
        if (!File.Exists(encryptedFilePath))
        {
            Console.WriteLine("Dosya bulunamadı.");
            return;
        }
        if (!encryptedFilePath.EndsWith(".encrypted"))
        {
            Console.WriteLine("Bu dosya encrypted değil.");
            return;
        }
        Console.Write("Key gir (32 karakter): ");
        string keyInput = Console.ReadLine();
        if (keyInput.Length != 32)
        {
            keyInput = keyInput.PadLeft(32, '0');
        }
        byte[] key = Encoding.UTF8.GetBytes(keyInput);
        try
        {
           
            string base64EncryptedName = Path.GetFileNameWithoutExtension(encryptedFilePath);
            byte[] encryptedNameBytes = Convert.FromBase64String(base64EncryptedName);

            using (FileStream fsInput = new FileStream(encryptedFilePath, FileMode.Open, FileAccess.Read))
            {
                byte[] iv = new byte[16];
                fsInput.Read(iv, 0, 16); // IV başta

                string originalFileName = DecryptStringFromBytes(encryptedNameBytes, key, iv);
                string decryptFolder = Path.Combine(Path.GetDirectoryName(encryptedFilePath), "Decrypt");
                Directory.CreateDirectory(decryptFolder);
                string outputPath = Path.Combine(decryptFolder, originalFileName);

                using (Aes aes = Aes.Create())
                {
                    aes.Key = key;
                    aes.IV = iv;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;

                    using (CryptoStream cryptoStream = new CryptoStream(fsInput, aes.CreateDecryptor(), CryptoStreamMode.Read))
                    using (FileStream fsOutput = new FileStream(outputPath, FileMode.Create, FileAccess.Write))
                    {
                        cryptoStream.CopyTo(fsOutput);
                    }
                }

                Console.WriteLine("Dosya çözüldü. Adresi : " + outputPath);
            }
        }
        catch (FormatException)
        {
            Console.WriteLine("Format hatası");
        }
        catch (CryptographicException)
        {
            Console.WriteLine("Anahtar yanlış ya da dosya bozuk.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Hata: " + ex.Message);
        }
    }

    static string DecryptStringFromBytes(byte[] cipherText, byte[] key, byte[] iv)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = key;
            aes.IV = iv;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            using (var ms = new MemoryStream(cipherText))
            using (var cryptoStream = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read))
            using (var reader = new StreamReader(cryptoStream, Encoding.UTF8))
            {
                return reader.ReadToEnd();
            }
        }
    }
    }
