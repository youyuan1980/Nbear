using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.IO.Compression;

namespace NBear.Common
{
    /// <summary>
    /// Compress Manager
    /// </summary>
    public sealed class CompressionManager
    {
        private CompressionManager()
        {
        }

        /// <summary>
        /// Compresses the specified STR.
        /// </summary>
        /// <param name="str">The STR.</param>
        /// <returns></returns>
        public static string Compress(string str)
        {
            byte[] buffer = UTF8Encoding.Unicode.GetBytes(str);
            MemoryStream ms = new MemoryStream();
            using (GZipStream gzip = new GZipStream(ms, CompressionMode.Compress))
            {
                gzip.Write(buffer, 0, buffer.Length);
            }
            return Convert.ToBase64String(ms.GetBuffer()).TrimEnd('\0');
        }

        /// <summary>
        /// Decompress the specified STR.
        /// </summary>
        /// <param name="str">The STR.</param>
        /// <returns></returns>
        public static string Decompress(string str)
        {
            byte[] buffer = Convert.FromBase64String(str);
            MemoryStream ms = new MemoryStream();
            MemoryStream msOut = new MemoryStream();
            ms.Write(buffer, 0, buffer.Length);
            ms.Position = 0;
            byte[] writeData = new byte[4096];
            using (GZipStream gzip = new GZipStream(ms, CompressionMode.Decompress))
            {
                int n;
                while ((n = gzip.Read(writeData, 0, writeData.Length)) != 0)
                {
                    msOut.Write(writeData, 0, n);
                }
            }
            return UTF8Encoding.Unicode.GetString(msOut.GetBuffer()).TrimEnd('\0');
        }

        /// <summary>
        /// 7Zip Compress the str.
        /// </summary>
        /// <param name="str">The STR.</param>
        /// <returns></returns>
        public static string Compress7Zip(string str)
        {
                byte[] inbyt = UTF8Encoding.Unicode.GetBytes(str);
                byte[] b = SevenZip.Compression.LZMA.SevenZipHelper.Compress(inbyt);
                return Convert.ToBase64String(b);
        }

        /// <summary>
        /// 7Zip Decompress the str.
        /// </summary>
        /// <param name="str">The STR.</param>
        /// <returns></returns>
        public static string Decompress7Zip(string str)
        {
            byte[] inbyt = Convert.FromBase64String(str);
            byte[] b = SevenZip.Compression.LZMA.SevenZipHelper.Decompress(inbyt);
            return UTF8Encoding.Unicode.GetString(b);
        }
    }
}
