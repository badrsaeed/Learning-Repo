using System.Security.Cryptography;
using System.Text;

internal class Program
{
    //static void Main()
    //{
    //    // نفس الـ PublicKey اللى بيرجعه Extension.GetPublicKey()
    //    string passphrase = "!yekgnolsetyb02742875764287a!yekgnolsetyb02742875764287a";

    //    // JSON تجريبى (غيره برحتك)
    //    List<string> json = new List<string>()
    //    {
    //        "smtp.gmail.com",
    //        "badrsaeed85@gmail.com",
    //        "bnxeswstvapdhcvj",
    //        "userName",
    //        "password",
    //        "LDAP://server/OU=IDMS,OU=Groups,DC=domain,DC=com",
    //        "server"
    //    };
    //    foreach (string plainJson in json)
    //    {
    //        Console.WriteLine("🟢 Original JSON : " + plainJson);

    //        var mvcEnc = new MvcAes();
    //        string encrypted = mvcEnc.Encrypt(plainJson, passphrase);
    //        Console.WriteLine("\n🔒 Encrypted (MVC) : " + encrypted);

    //        var apiEnc = new ApiAes();
    //        string decrypted = apiEnc.Decrypt(encrypted, passphrase);
    //        Console.WriteLine("\n🔑 Decrypted (API) : " + decrypted);

    //        Console.WriteLine("\n✅ Match? " + (plainJson == decrypted));
    //    }
    //}
    static void Main()
    {
        while (true)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            // Same passphrase used for both encryption and decryption
            string passphrase = "!yekgnolsetyb02742875764287a!yekgnolsetyb02742875764287a";

            while (true) // Infinite loop to keep the app running
            {
                Console.WriteLine("\n🔐 Do you want to Encrypt (e) or Decrypt (d)?");
                Console.Write("Your choice (e/d): ");
                string choice = Console.ReadLine()?.Trim().ToLower();

                if (choice != "e" && choice != "d")
                {
                    Console.WriteLine("❌ Invalid choice. Please enter 'e' for encryption or 'd' for decryption.");
                    continue; // back to the beginning of the loop
                }

                Console.Write("📥 Enter the text: ");
                string input = Console.ReadLine();

                if (string.IsNullOrEmpty(input))
                {
                    Console.WriteLine("⚠️ No input provided. Please try again.");
                    continue; // restart from the beginning
                }

                if (choice == "e")
                {
                    var mvcEnc = new MvcAes();
                    string encrypted = mvcEnc.Encrypt(input, passphrase);
                    Console.WriteLine("\n🔒 Encrypted text: " + encrypted);
                }
                else if (choice == "d")
                {
                    var apiEnc = new ApiAes();
                    try
                    {
                        string decrypted = apiEnc.Decrypt(input, passphrase);
                        Console.WriteLine("\n🔓 Decrypted text: " + decrypted);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("❌ Error during decryption: " + ex.Message);
                    }
                }

                Console.WriteLine("\n🔁 Restarting...\n");
            }
        }
    }

    public class MvcAes
    {
        public string Encrypt(string plainText, string passphrase)
        {
            byte[] salt = new byte[8];
            using (var rng = RandomNumberGenerator.Create())
                rng.GetNonZeroBytes(salt);

            DeriveKeyAndIV(passphrase, salt, out var key, out var iv);

            byte[] cipherBytes = EncryptStringToBytesAes(plainText, key, iv);

            byte[] salted = new byte[8 + 8 + cipherBytes.Length];
            Buffer.BlockCopy(Encoding.ASCII.GetBytes("Salted__"), 0, salted, 0, 8);
            Buffer.BlockCopy(salt, 0, salted, 8, 8);
            Buffer.BlockCopy(cipherBytes, 0, salted, 16, cipherBytes.Length);

            return Convert.ToBase64String(salted)
                          .Replace('+', '-').Replace('/', '_').TrimEnd('=');
        }

        // ---------------------------------------------------------------------------------
        private static void DeriveKeyAndIV(string pass, byte[] salt, out byte[] key, out byte[] iv)
        {
            List<byte> hashes = new(48);
            byte[] pwd = Encoding.UTF8.GetBytes(pass);
            byte[] cur = Array.Empty<byte>();
            using var md5 = MD5.Create();

            while (hashes.Count < 48)
            {
                byte[] pre = new byte[cur.Length + pwd.Length + salt.Length];
                Buffer.BlockCopy(cur, 0, pre, 0, cur.Length);
                Buffer.BlockCopy(pwd, 0, pre, cur.Length, pwd.Length);
                Buffer.BlockCopy(salt, 0, pre, cur.Length + pwd.Length, salt.Length);
                cur = md5.ComputeHash(pre);
                hashes.AddRange(cur);
            }
            key = hashes.Take(32).ToArray();
            iv = hashes.Skip(32).Take(16).ToArray();
        }

        private static byte[] EncryptStringToBytesAes(string plain, byte[] key, byte[] iv)
        {
            using var aes = new RijndaelManaged { Mode = CipherMode.CBC, KeySize = 256, BlockSize = 128, Key = key, IV = iv };
            using var ms = new MemoryStream();
            using (var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
            using (var sw = new StreamWriter(cs)) { sw.Write(plain); }
            return ms.ToArray();
        }
    }

    public class ApiAes
    {
        public string Decrypt(string encrypted, string passphrase)
        {
            string b64 = encrypted.Replace('-', '+').Replace('_', '/');
            b64 = b64.PadRight(b64.Length + (4 - b64.Length % 4) % 4, '=');

            byte[] allBytes = Convert.FromBase64String(b64);

            string header = Encoding.ASCII.GetString(allBytes, 0, 8);
            if (header != "Salted__")
                throw new Exception("Header != \"Salted__\"");

            byte[] salt = allBytes.Skip(8).Take(8).ToArray();
            byte[] cipher = allBytes.Skip(16).ToArray();

            DeriveKeyAndIV(passphrase, salt, out var key, out var iv);

            byte[] plainBytes = DecryptStringFromBytesAes(cipher, key, iv);
            return Encoding.UTF8.GetString(plainBytes);
        }

        private static void DeriveKeyAndIV(string pass, byte[] salt, out byte[] key, out byte[] iv)
        {
            List<byte> hashes = new(48);
            byte[] pwd = Encoding.UTF8.GetBytes(pass);
            byte[] cur = Array.Empty<byte>();
            using var md5 = MD5.Create();

            while (hashes.Count < 48)
            {
                byte[] pre = new byte[cur.Length + pwd.Length + salt.Length];
                Buffer.BlockCopy(cur, 0, pre, 0, cur.Length);
                Buffer.BlockCopy(pwd, 0, pre, cur.Length, pwd.Length);
                Buffer.BlockCopy(salt, 0, pre, cur.Length + pwd.Length, salt.Length);
                cur = md5.ComputeHash(pre);
                hashes.AddRange(cur);
            }
            key = hashes.Take(32).ToArray();
            iv = hashes.Skip(32).Take(16).ToArray();
        }

        private static byte[] DecryptStringFromBytesAes(byte[] cipher, byte[] key, byte[] iv)
        {
            using var aes = new RijndaelManaged { Mode = CipherMode.CBC, KeySize = 256, BlockSize = 128, Key = key, IV = iv };
            using var ms = new MemoryStream(cipher);
            using var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read);
            using var dst = new MemoryStream();
            cs.CopyTo(dst);
            return dst.ToArray();
        }
    }
}
