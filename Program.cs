using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DecryptorEcryptor
{
    class Program
    {
        static void Main(string[] args)
        {            
            Console.WriteLine("Podaj nazwe pliku do wczytania");
            var fileName = Console.ReadLine();
            try
            {
                var tmpText = "text";
                using (StreamReader sr = new StreamReader(fileName))
                {
                    var line = sr.ReadToEnd();
                    Console.WriteLine($"Zawartość pliku to: {line} \n   Chcesz plik 1. Zaszyfrowac? 2. Odszyfrowac?");
                    if (Console.ReadKey().Key == ConsoleKey.D1)
                    {
                        tmpText = Encrypt(line);                       
                    }
                    if (Console.ReadKey().Key == ConsoleKey.D2)
                    {
                        tmpText = Decrypt(line);                        
                    }
                    Console.WriteLine($"Operacja się powiodła. Tekst znajdujacy sie w pliku to: {tmpText}");
                    Console.ReadLine();
                }
                using (var fs = new FileStream(fileName, FileMode.Truncate))
                {
                }

                using (StreamWriter sw = new StreamWriter(fileName))
                {
                    sw.Write(tmpText);
                }
            }

            catch (IOException e)
            {
                Console.WriteLine("Nie mozna odczytac pliku:");
                Console.WriteLine(e.Message);
            }
            Console.ReadKey();
        }

        public static string Encrypt(string line)
        {
            string EncryptionKey = "abc123";
            byte[] clearBytes = Encoding.Unicode.GetBytes(line);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    line = Convert.ToBase64String(ms.ToArray());
                }
            }
            return line;
        }

        public static string Decrypt(string line)
        {
            string EncryptionKey = "abc123";
            line = line.Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(line);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    line = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return line;
        }
    }
}
