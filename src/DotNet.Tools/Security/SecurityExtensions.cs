using System.Security.Cryptography;
using YanAn.Extensions;
using YanAn.Logging;
namespace YanAn.Security
{
    public static class SecurityExtensions
    {
        /// <summary>
        /// 加密-MD5
        /// </summary>
        /// <param name="str">需加密字符串</param>
        /// <param name="encoding">字符串编码格式,默认UTF-8</param>
        /// <param name="lower">加密字符串大小写,默认小写</param>
        /// <returns></returns>
        public static string ToMD5(this string str, Encoding? encoding = null, bool lower = true)
        {
            string md5Str = string.Empty;
            if (str.IsNullOrEmpty())
                return md5Str;
            if (encoding == null)
                encoding = Encoding.UTF8;
            MD5 md5 = MD5.Create();
            byte[] buffer = encoding.GetBytes(str);
            byte[] bytes = md5.ComputeHash(buffer);
            md5Str = bytes.Aggregate("", (current, b) => current + b.ToString($"{(lower ? "x" : "X")}2"));
            return md5Str;
        }
        /// <summary>
        /// 加密-base64
        /// </summary>
        /// <param name="str">需加密字符串</param>
        /// <param name="encoding">字符串编码格式,默认UTF-8</param>
        /// <returns></returns>
        public static string ToBase64(this string str, Encoding? encoding = null)
        {
            if (encoding == null)
                encoding = Encoding.UTF8;
            byte[] encbuff = encoding.GetBytes(str);
            return Convert.ToBase64String(encbuff);
        }
        /// <summary>
        /// 解密-base64
        /// </summary>
        /// <param name="encryptStr"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string DecryptBase64(this string encryptStr, Encoding? encoding = null)
        {
            try
            {
                if (encoding == null)
                    encoding = Encoding.UTF8;
                byte[] decbuff = Convert.FromBase64String(encryptStr);
                return encoding.GetString(decbuff);
            }
            catch (Exception ex)
            {
                LogManager.Error(ex);
                return string.Empty;
            }
        }
        /// <summary>
        /// 加密-SHA256
        /// </summary>
        /// <param name="str">需加密字符串</param>
        /// <param name="encoding">字符串编码格式,默认UTF-8</param>
        /// <returns></returns>
        public static string ToSHA256(this string str, Encoding? encoding = null)
        {
            if (encoding == null)
                encoding = Encoding.UTF8;
            using var sha256 = SHA256.Create();
            byte[] encbuff = encoding.GetBytes(str);
            byte[] result = sha256.ComputeHash(encbuff);
            return Convert.ToBase64String(result);
        }
    }
}
