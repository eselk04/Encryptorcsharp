using Encrypt;
while (true)
{
    Console.WriteLine("Yapmak istediğiniz işlemi seçiniz. \n 1=> Encryption \n 2=> Decryption");
    string operation = Console.ReadLine();
    switch (operation){
        case "1":
            Encrypt.Encrypt.Encryption();
            break;
        case "2":
            Decrypt.Decryption();
            break;
        default: Console.WriteLine("Hata.");
            break;
    }
}



