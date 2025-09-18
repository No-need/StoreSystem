using System.Security.Cryptography;
using System.Text;

namespace StoreSystem.Helper
{
    public static class HashHelper
    {
        //MD5雜湊
        public static string ToMD5(string strs)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] bytes = Encoding.Default.GetBytes(strs);//將要加密的字串轉換為位元組陣列
            byte[] encryptdata = md5.ComputeHash(bytes);//將字串加密後也轉換為字元陣列
            return Convert.ToBase64String(encryptdata);//將加密後的位元組陣列轉換為加密字串
        }
    }
}
