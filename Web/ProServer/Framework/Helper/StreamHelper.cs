using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace ProServer.Helper
{
    /// <summary>
    /// 内在流助手
    /// </summary>
    public static class StreamHelper
    {
        /// <summary>
        /// 将文件转成byte[]
        /// </summary>
        public static byte[] ReadFile(string fileName)
        {
            string err;
            return ReadFile(fileName, out err);
        }

        /// <summary>
        /// 将文件转成byte[]
        /// </summary>
        public static byte[] ReadFile(string fileName, out string err)
        {
            FileStream pFileStream = null;
            byte[] pReadByte = new byte[0];

            try
            {
                pFileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                BinaryReader r = new BinaryReader(pFileStream);
                r.BaseStream.Seek(0, SeekOrigin.Begin);    //将文件指针设置到文件开
                pReadByte = r.ReadBytes((int)r.BaseStream.Length);
                err = null;
                return pReadByte;
            }
            catch (Exception e)
            {
                err = e.Message;
                return pReadByte;
            }

            finally
            {
                if (pFileStream != null)
                    pFileStream.Close();
            }
        }

        /// <summary>
        /// 将byte[]保存为文件
        /// </summary>
        public static bool WriteFile(byte[] pReadByte, string fileName)
        {
            FileStream pFileStream = null;

            try
            {
                pFileStream = new FileStream(fileName, FileMode.OpenOrCreate);
                pFileStream.Write(pReadByte, 0, pReadByte.Length);
            }
            catch
            {
                return false;
            }
            finally
            {
                if (pFileStream != null)
                    pFileStream.Close();
            }

            return true;

        }

        /// <summary>
        /// 将流转成二进制数组
        /// </summary>
        /// <param name="stream">文件流</param>
        public static byte[] StreamToBytes(Stream stream)
        {
            byte[] result = null;
            if (stream != null && stream.Length > 0)
            {
                result = new byte[stream.Length];
                stream.Seek(0, SeekOrigin.Begin);
                stream.Read(result, 0, (int)stream.Length);
            }
            return result;
        }

        /// <summary>
        /// 取指定二进制的MD5码(32位string)
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string MD5Bytes(byte[] bytes)
        {
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] retVal = md5.ComputeHash(bytes);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < retVal.Length; i++)
                sb.Append(retVal[i].ToString("x2"));
            return sb.ToString();
        }
        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="pToDecrypt"></param>
        /// <param name="sKey"></param>
        /// <returns></returns>
        public static string Decrypt(string pToDecrypt, string sKey = "12345678")
        {
            byte[] inputByteArray = Convert.FromBase64String(pToDecrypt);
            using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
            {
                des.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
                des.Mode = CipherMode.CBC;
                des.IV = ASCIIEncoding.ASCII.GetBytes(sKey);
                des.Padding = PaddingMode.PKCS7;
                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                using (CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(inputByteArray, 0, inputByteArray.Length);
                    cs.FlushFinalBlock();
                    cs.Close();
                }
                string str = Encoding.Default.GetString(ms.ToArray());
                ms.Close();
                return str;
            }


        }
        /// <summary>
        /// AES解密
        /// </summary>
        /// <param name="inputdata">输入的数据</param>
        /// <param name="iv">向量128</param>
        /// <param name="strKey">key</param>
        /// <returns></returns>
        public static string AESDecrypt(string inputdata, string iv, string strKey)
        {
            string result = "";
            SymmetricAlgorithm des = Rijndael.Create();
            byte[] encryptedData = Convert.FromBase64String(inputdata);
            des.Key = Encoding.UTF8.GetBytes(strKey.Substring(0, 16));
            des.IV = Encoding.UTF8.GetBytes(iv.Substring(0, 16));
            byte[] decryptBytes = new byte[encryptedData.Length];
            using (MemoryStream ms = new MemoryStream(encryptedData))
            {
                using (CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Read))
                {
                    cs.Read(decryptBytes, 0, decryptBytes.Length);
                    cs.Close();
                    ms.Close();
                }
            }
            result = Encoding.UTF8.GetString(decryptBytes);
            result = result.Replace("\0", "").Trim();
            return result;

        }
    }
}
