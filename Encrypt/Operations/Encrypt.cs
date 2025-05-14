using System.Security.Cryptography;
using System.Text;
namespace Encrypt;

public class Encrypt
{
    public static void Encryption()
    {
        Console.Write("Dosya adını gir: ");
        string inputFile = Console.ReadLine();
        Console.WriteLine("Dosya aranıyor...");
        if (!File.Exists(inputFile))
        {
            Console.WriteLine("Dosya bulunamadı");
            return;
        }
        Console.WriteLine("Dosya bulundu.");
        Console.Write("Key giriniz : ");
        string keyInput = Console.ReadLine();

        if (keyInput.Length != 32)
        { 
            keyInput = keyInput.PadLeft(32, '0');
        }
        byte[] key = Encoding.UTF8.GetBytes(keyInput);
        byte[] iv = GenerateRandomIV.Generate();
        string originalName = Path.GetFileName(inputFile); // notlar.txt gibi
        byte[] encryptedNameBytes = EncryptStringToBytes(originalName, key, iv);
        string encryptedFileNameBase64 = Convert.ToBase64String(encryptedNameBytes);
        string outputFileName = encryptedFileNameBase64 + ".encrypted";
        string outputFile = Path.Combine(Path.GetDirectoryName(inputFile),
            outputFileName);
        try
        {
            Console.WriteLine("Dosya şifreleniyor.");
            using (FileStream fsInput = new FileStream(inputFile, FileMode.Open, FileAccess.Read))
            using (FileStream fsOutput = new FileStream(outputFile, FileMode.Create, FileAccess.Write))
            {
                fsOutput.Write(iv, 0, iv.Length);

                using (Aes aes = Aes.Create())
                {
                    aes.Key = key;
                    aes.IV = iv;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;

                    using (CryptoStream cryptoStream = new CryptoStream(fsOutput, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        fsInput.CopyTo(cryptoStream);
                    }
                }
            }

            Console.WriteLine("Şifreleme başarılı. dosya :  " + outputFile);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Bir hata oluştu: " + ex.Message);
        }

    }
    static byte[] EncryptStringToBytes(string plainText, byte[] key, byte[] iv)
    { Console.WriteLine("Dosya adı şifreleniyor..");
        using (Aes aes = Aes.Create())
        {
            aes.Key = key;
            aes.IV = iv;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            using (var ms = new MemoryStream())
            using (var cryptoStream = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(plainText);
                cryptoStream.Write(inputBytes, 0, inputBytes.Length);
                cryptoStream.FlushFinalBlock();
                return ms.ToArray();
            }
        }
    }
}