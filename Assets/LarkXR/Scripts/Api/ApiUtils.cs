using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;



namespace LarkXR
{
    public class ApiUtils
    {
        private readonly static long Jan1st1970Ms = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc).Ticks;
        /// <summary>
        /// 返回当前时间的毫秒数, 这个毫秒其实就是自1970年1月1日0时起的毫秒数
        /// </summary>
        public static long CurrentTimeMillis()
        {
            return (System.DateTime.UtcNow.Ticks - Jan1st1970Ms) / 10000;

        }

        public static string GetSignature(string key, string secret, string timestamp)
        {
            // appKey、appSecret、timestamp三个参数进行字典序排序
            // 或者adminKey、adminSecret、timestamp三个参数进行字典序排序
            // Sort a string
            string[] arr = { key, secret, timestamp };
            Array.Sort(arr);
            string substr = "";
            foreach (string i in arr)
            {
                substr += i;
            }

            string signature = SHA1Hash(substr);

            return signature;
        }

        public static string SHA1Hash(string input)
        {
            var hash = new SHA1Managed().ComputeHash(Encoding.UTF8.GetBytes(input));
            return string.Concat(hash.Select(b => b.ToString("x2")));
        }

    }
}
