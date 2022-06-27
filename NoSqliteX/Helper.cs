using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime;
using System.Security.Cryptography;
using System.Text;

namespace NoSqliteX
{

    internal static class TokensHelper
    {
        /// <summary>
        ///  Metodod Simples para Gerar Token keys
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string TokenGenerator(int length)
        {
            var random = new Random();

            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static string GenerateId()
        {
            var blockSize = 4;
            var blocks = 4;
            var separador = '-';
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var str = string.Empty;
            var random = new Random();
            for (var i = 0; i < blocks; i++)
            {
                str += new string(Enumerable.Repeat(chars, blockSize)
                    .Select(s => s[random.Next(s.Length)]).ToArray());
                if (i < blocks - 1)
                    str += separador;
            }

            return str;
        }
        private static object CareNullValues(Type fieldType)
        {

            var @switch = new Dictionary<Type, dynamic> {
                { typeof(bool), false },
                { typeof(float), 0 },
                { typeof(int), 0 },
                { typeof(double), 0 },
                { typeof(string), string.Empty},
                { typeof(DateTime), DateTime.Now},
                { typeof(TimeSpan), TimeSpan.Zero},
                { typeof(byte[]), new Byte[]{}},

            };
            var valueR = @switch[fieldType];
            return valueR;
        }
        
        internal static T CareNull <T>(this T inst)
        {
            var props = TypeDescriptor.GetProperties(inst);

            foreach (PropertyDescriptor x in props)
            {
                var value = x.GetValue(inst);
                if(value == null)
                    x.SetValue(inst, CareNullValues(x.PropertyType));
            }
            return inst;
        }
        internal static string EncryptPlainTextToCipherText(this string plainText)
        {
            // Getting the bytes of Input String.
            var toEncryptedArray = Encoding.UTF8.GetBytes(plainText);

            var objMd5CryptoService = new MD5CryptoServiceProvider();
            //Gettting the bytes from the Security Key and Passing it to compute the Corresponding Hash Value.
            var securityKeyArray = objMd5CryptoService.ComputeHash(Encoding.UTF8.GetBytes(Shareds.CriptoPassword));
            //De-allocatinng the memory after doing the Job.
            objMd5CryptoService.Clear();

            var objTripleDesCryptoService = new TripleDESCryptoServiceProvider();
            //Assigning the Security key to the TripleDES Service Provider.
            objTripleDesCryptoService.Key = securityKeyArray;
            //Mode of the Crypto service is Electronic Code Book.
            objTripleDesCryptoService.Mode = CipherMode.ECB;
            //Padding Mode is PKCS7 if there is any extra byte is added.
            objTripleDesCryptoService.Padding = PaddingMode.PKCS7;


            var objCrytpoTransform = objTripleDesCryptoService.CreateEncryptor();
            //Transform the bytes array to resultArray
            var resultArray = objCrytpoTransform.TransformFinalBlock(toEncryptedArray, 0, toEncryptedArray.Length);
            objTripleDesCryptoService.Clear();
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        //This method is used to convert the Encrypted/Un-Readable Text back to readable  format.
        internal static string DecryptCipherTextToPlainText(this string cipherText)
        {
            var toEncryptArray = Convert.FromBase64String(cipherText);
            var objMd5CryptoService = new MD5CryptoServiceProvider();

            //Gettting the bytes from the Security Key and Passing it to compute the Corresponding Hash Value.
            var securityKeyArray = objMd5CryptoService.ComputeHash(Encoding.UTF8.GetBytes(Shareds.CriptoPassword));
            objMd5CryptoService.Clear();

            var objTripleDesCryptoService = new TripleDESCryptoServiceProvider();
            //Assigning the Security key to the TripleDES Service Provider.
            objTripleDesCryptoService.Key = securityKeyArray;
            //Mode of the Crypto service is Electronic Code Book.
            objTripleDesCryptoService.Mode = CipherMode.ECB;
            //Padding Mode is PKCS7 if there is any extra byte is added.
            objTripleDesCryptoService.Padding = PaddingMode.PKCS7;

            var objCrytpoTransform = objTripleDesCryptoService.CreateDecryptor();
            //Transform the bytes array to resultArray
            var resultArray = objCrytpoTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            objTripleDesCryptoService.Clear();

            //Convert and return the decrypted data/byte into string format.
            return Encoding.UTF8.GetString(resultArray);
        }
    }
}