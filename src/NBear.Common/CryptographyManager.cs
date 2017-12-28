using System;
using System.Data;
using System.IO;
using System.Text;
using System.Configuration;
using System.Security;
using System.Security.Cryptography;

namespace NBear.Common
{
    /// <summary>
    /// Common CryptographyManager
    /// </summary>
    public class CryptographyManager
    {
        private string _defaultLegalIV = "E4ghj*Ghg7!rNIfb&95GUY86GfghUb#er57HBh(u%g6HJ($jhWk7&!hg4ui%$hjk";

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CryptographyManager"/> class.
        /// </summary>
        public CryptographyManager()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CryptographyManager"/> class.
        /// </summary>
        /// <param name="legalIVKey">The legal IV key.</param>
        public CryptographyManager(string legalIVKey)
        {
            Check.Require(!string.IsNullOrEmpty(legalIVKey), "legalIVKey could not be null or empty.");

            _defaultLegalIV = legalIVKey;
        }

        #endregion

        #region Private Members

        /// <summary>
        /// The default encrypt key.
        /// </summary>
        public const string DEFAULT_KEY = "aslkjkljlsajsuaggasfklrjuisdhaie";

        private byte[] GetLegalKey(SymmetricAlgorithm mobjCryptoService, string key)
        {
            string sTemp = key;
            mobjCryptoService.GenerateKey();
            byte[] bytTemp = mobjCryptoService.Key;
            int KeyLength = bytTemp.Length;
            if (sTemp.Length > KeyLength)
                sTemp = sTemp.Substring(0, KeyLength);
            else if (sTemp.Length < KeyLength)
                sTemp = sTemp.PadRight(KeyLength, ' ');
            return ASCIIEncoding.ASCII.GetBytes(sTemp);
        }

        private byte[] GetLegalIV(SymmetricAlgorithm mobjCryptoService)
        {
            string sTemp = _defaultLegalIV;
            mobjCryptoService.GenerateIV();
            byte[] bytTemp = mobjCryptoService.IV;
            int IVLength = bytTemp.Length;
            if (sTemp.Length > IVLength)
                sTemp = sTemp.Substring(0, IVLength);
            else if (sTemp.Length < IVLength)
                sTemp = sTemp.PadRight(IVLength, ' ');
            return ASCIIEncoding.ASCII.GetBytes(sTemp);
        }

        #endregion

        #region public Members

        /// <summary>
        /// Symmetrics encrpyt.
        /// </summary>
        /// <param name="str">The STR to encrpyt.</param>
        /// <param name="mobjCryptoService">A concrete symmetric algorithm.</param>
        /// <param name="key">The key.</param>
        /// <returns>The encrpyt str.</returns>
        public string SymmetricEncrpyt(string str, SymmetricAlgorithm mobjCryptoService, string key)
        {
            Check.Require(str != null, "Arguments error.", new ArgumentNullException("str"));
            Check.Require(mobjCryptoService != null, "Arguments error.", new ArgumentNullException("mobjCryptoService"));

            byte[] bytIn = UTF8Encoding.UTF8.GetBytes(str);
            MemoryStream ms = new MemoryStream();
            mobjCryptoService.Key = GetLegalKey(mobjCryptoService, key);
            mobjCryptoService.IV = GetLegalIV(mobjCryptoService);
            ICryptoTransform encrypto = mobjCryptoService.CreateEncryptor();
            CryptoStream cs = new CryptoStream(ms, encrypto, CryptoStreamMode.Write);
            cs.Write(bytIn, 0, bytIn.Length);
            cs.FlushFinalBlock();
            ms.Close();
            byte[] bytOut = ms.ToArray();
            return Convert.ToBase64String(bytOut);
        }

        /// <summary>
        /// Symmetrics decrpyt.
        /// </summary>
        /// <param name="str">The STR to decrpyt.</param>
        /// <param name="mobjCryptoService">A concrete symmetric algorithm.</param>
        /// <param name="key">The key.</param>
        /// <returns>The decrpyted str.</returns>
        public string SymmetricDecrpyt(string str, SymmetricAlgorithm mobjCryptoService, string key)
        {
            Check.Require(str != null, "Arguments error.", new ArgumentNullException("str"));
            Check.Require(mobjCryptoService != null, "Arguments error.", new ArgumentNullException("mobjCryptoService"));

            byte[] bytIn = Convert.FromBase64String(str);
            MemoryStream ms = new MemoryStream(bytIn, 0, bytIn.Length);
            mobjCryptoService.Key = GetLegalKey(mobjCryptoService, key);
            mobjCryptoService.IV = GetLegalIV(mobjCryptoService);
            ICryptoTransform encrypto = mobjCryptoService.CreateDecryptor();
            CryptoStream cs = new CryptoStream(ms, encrypto, CryptoStreamMode.Read);
            StreamReader sr = new StreamReader(cs);
            return sr.ReadToEnd();
        }

        /// <summary>
        /// Computes the hash.
        /// </summary>
        /// <param name="str">The STR to compute hash value.</param>
        /// <returns>The hash value.</returns>
        public string ComputeHash(string str)
        {
            Check.Require(str != null, "Arguments error.", new ArgumentNullException("str"));

            byte[] bytIn = UTF8Encoding.UTF8.GetBytes(str);
            HashAlgorithm sha = new SHA1CryptoServiceProvider();
            byte[] bytOut = sha.ComputeHash(bytIn);
            return Convert.ToBase64String(bytOut);
        }

        #endregion
    }
}