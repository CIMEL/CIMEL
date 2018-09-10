using CIMEL.RSA;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSAKeyGen
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("*************************");
            int keySize = 1024;
            var keys = Encryptor.Singleton.GenerateKeys(keySize);

            Console.WriteLine("key size: {0}", keySize);

            Console.WriteLine("public key:");
            Console.WriteLine(keys.PublicKey);
            string publicKey = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "rsa.pub");
            Console.WriteLine("public key: {0}", publicKey);
            File.WriteAllText(publicKey, keys.PublicKey, Encoding.UTF8);

            Console.WriteLine("private key:");
            Console.WriteLine(keys.PrivateKey);
            string privateKey = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "rsa");
            Console.WriteLine("private key: {0}", privateKey);
            File.WriteAllText(privateKey, keys.PrivateKey, Encoding.UTF8);

            //string text = "text for encryption";

            //string encrypted = Encryptor.EncryptText(text, keys.PublicKey);
            //string decrypted = Encryptor.DecryptText(encrypted, keys.PrivateKey);

            //Console.WriteLine(encrypted);
            //Console.WriteLine(decrypted);

            Console.WriteLine("*************************");
            Console.ReadKey();
        }
    }
}
