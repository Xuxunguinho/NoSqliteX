using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime;

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

    }
}