﻿using System;
using System.Security.Cryptography;

namespace HorseSpot.Infrastructure.Utils
{
    public class Helper
    {
        /// <summary>
        /// Get Hash From Tokens
        /// </summary>
        /// <param name="input">Token Input</param>
        /// <returns>Hash String</returns>
        public static string GetHash(string input)
        {
            HashAlgorithm hashAlgorithm = new SHA256CryptoServiceProvider();

            byte[] byteValue = System.Text.Encoding.UTF8.GetBytes(input);

            byte[] byteHash = hashAlgorithm.ComputeHash(byteValue);

            return Convert.ToBase64String(byteHash);
        }
    }
}
