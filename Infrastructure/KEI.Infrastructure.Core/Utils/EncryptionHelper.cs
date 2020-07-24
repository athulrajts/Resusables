﻿using System;
using System.Text;
using System.Security.Cryptography;

namespace KEI.Infrastructure.Helpers
{
    public static class EncryptionHelper
    {
        private static byte[] sm_PassKey = null;
        private static byte[] PassKey
        {
            get
            {
                if (sm_PassKey == null)
                {
                    HashAlgorithm algorithm = new MD5CryptoServiceProvider();
                    sm_PassKey = algorithm.ComputeHash(Encoding.UTF8.GetBytes("kei_pw_key"));
                }

                return sm_PassKey;
            }
        }

        public static string Encrypt(string p_Value)
        {
            if (string.IsNullOrEmpty(p_Value))
                return string.Empty;

            var TDES = new TripleDESCryptoServiceProvider();

            TDES.Key = PassKey;
            TDES.Mode = CipherMode.ECB;
            TDES.Padding = PaddingMode.PKCS7;

            byte[] passToEncrypt = Encoding.UTF8.GetBytes(p_Value);

            byte[] byteRes = null;
            try
            {
                byteRes = TDES.CreateEncryptor().TransformFinalBlock(passToEncrypt, 0, passToEncrypt.Length);
            }
            finally
            {
                TDES.Clear();
            }

            return Convert.ToBase64String(byteRes);
        }

        public static string Decrypt(string p_Value)
        {
            var TDES = new TripleDESCryptoServiceProvider();

            TDES.Key = PassKey;
            TDES.Mode = CipherMode.ECB;
            TDES.Padding = PaddingMode.PKCS7;

            byte[] passToDecrypt = Convert.FromBase64String(p_Value);

            byte[] byteRes = null;
            try
            {
                byteRes = TDES.CreateDecryptor().TransformFinalBlock(passToDecrypt, 0, passToDecrypt.Length);
            }
            finally
            {
                TDES.Clear();
            }

            return Encoding.UTF8.GetString(byteRes);
        }
    }
}
