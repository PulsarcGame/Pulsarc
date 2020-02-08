using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Pulsarc.Utils.Update
{
    internal enum HashType
    {
        MD5,
        SHA1,
        SHA512,
    }

    internal static class Hasher
    {
        public static string HashFile(string filePath, HashType algorithm)
        {
            switch (algorithm)
            {
                case HashType.MD5:
                    return MakeHashString(MD5.Create().ComputeHash(
                        new FileStream(filePath, FileMode.Open)));
                case HashType.SHA1:
                    return MakeHashString(SHA1.Create().ComputeHash(
                        new FileStream(filePath, FileMode.Open)));
                case HashType.SHA512:
                    return MakeHashString(SHA512.Create().ComputeHash(
                        new FileStream(filePath, FileMode.Open)));
                default:
                    return "";
            }
        }

        private static string MakeHashString(byte[] hash)
        {
            StringBuilder builder = new StringBuilder(hash.Length * 2);

            for (int i = 0; i < hash.Length; i++)
            {
                builder.Append(hash[i].ToString("X2").ToLower());
            }

            return builder.ToString();
        }
    }
}
