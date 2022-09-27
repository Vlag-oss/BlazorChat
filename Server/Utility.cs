using System.Security.Cryptography;
using System.Text;

namespace BlazorChat.Server;

public class Utility
{
    public static string Encrypt(string password)
    {
        var provider = MD5.Create();
        var salt = "S0m3R@nd0mSalt";
        byte[] bytes = provider.ComputeHash(Encoding.UTF32.GetBytes(salt + password));
        return BitConverter.ToString(bytes).Replace("-", "").ToLower();
    }
}