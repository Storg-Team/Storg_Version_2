using System;
using System.Security.Cryptography;
using System.Text;

namespace StorgLibs;

public static class CryptoHelper
{
    public static string Hash(string password)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] hashed = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return BitConverter.ToString(hashed).Replace("-", "").ToLowerInvariant();
        }
    }
}
