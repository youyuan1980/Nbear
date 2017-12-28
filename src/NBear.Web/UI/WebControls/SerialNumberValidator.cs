//------------------------------
//Copyright (c) 2006 JianHan Fan	
//Mail:henryfan@msn.com
//All rights reserved.
//------------------------------
using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Drawing;
using System.Security.Cryptography;
using System.IO;
using System.Text;

namespace WebValidates
{
    /// <summary>
    /// The SerialNumberValidator control.
    /// </summary>
	public class SerialNumberValidator: Control
	{
        /// <summary>
        /// The serial num type.
        /// </summary>
        public enum SerialNumberType
        {
            /// <summary>
            /// NumberOnly
            /// </summary>
            NumberOnly,
            /// <summary>
            /// AlphabetOnly
            /// </summary>
            AlphabetOnly,
            /// <summary>
            /// NumberAndAlphabet
            /// </summary>
            NumberAndAlphabet
        }

        /// <summary>
        /// Gets or sets the type of the char.
        /// </summary>
        /// <value>The type of the char.</value>
        [Bindable(true)]
        [Category("Behavior")]
        [DefaultValue(SerialNumberValidator.SerialNumberType.NumberAndAlphabet)]
        [Localizable(true)]
        public SerialNumberType CharType
        {
            get
            {
                return ((ViewState["CharType"] == null) ? SerialNumberType.NumberAndAlphabet : (SerialNumberType)ViewState["CharType"]);
            }

            set
            {
                ViewState["CharType"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the min char count.
        /// </summary>
        /// <value>The min char count.</value>
        [Bindable(true)]
        [Category("Behavior")]
        [DefaultValue(4)]
        [Localizable(true)]
        public int MinCharCount
        {
            get
            {
                return ((ViewState["MinCharCount"] == null) ? 4 : (int)ViewState["MinCharCount"]);
            }

            set
            {
                ViewState["MinCharCount"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the max char count.
        /// </summary>
        /// <value>The max char count.</value>
        [Bindable(true)]
        [Category("Behavior")]
        [DefaultValue(4)]
        [Localizable(true)]
        public int MaxCharCount
        {
            get
            {
                return ((ViewState["MaxCharCount"] == null) ? 4 : (int)ViewState["MaxCharCount"]);
            }

            set
            {
                ViewState["MaxCharCount"] = value;
            }
        }

        private static System.Collections.IDictionary mImages = null;
        /// <summary>
        /// Gets the images.
        /// </summary>
        /// <value>The images.</value>
		protected static System.Collections.IDictionary Images
		{
			get
			{
				lock(typeof(SerialNumberValidator))
				{
					if(mImages == null)
					{
						LoadImages();	
					}
					return mImages;
				}
			}
		}
		private static void LoadImages()
		{
			mImages = new System.Collections.Hashtable();
			string imagename;
			System.Drawing.Image img = null;
            foreach (Char item in _Seed)
			{
                imagename = "NBear.Web.EmbeddedImages." + item + ".gif";
				img = System.Drawing.Image.FromStream(typeof(SerialNumberValidator).Assembly.GetManifestResourceStream(imagename));
				mImages.Add(item.ToString(),img);

			}
		}
        /// <summary>
        /// Renders the specified output.
        /// </summary>
        /// <param name="output">The output.</param>
		protected override void Render(HtmlTextWriter output)
		{	
			if(this.Site!=null && this.Site.DesignMode)
			{
				output.Write("<br>SerialNumberValidator<br>");
			}
			else
			{
				if(mSN !="")
				{
                    Crypto a = new Crypto();
                    a.CryptText = SN;
                    a.CryptIV = IV;
                    a.CryptKey = Key;
                    string _name = a.Encrypt();
					output.Write("<img border=\"0\" src=\"{0}\">",this.Page.Request.Path+"?"+ _ImageTag + "=" +
                        System.Web.HttpContext.Current.Server.UrlEncode(_name));
				}
				else
				{
					output.Write("<br>SerialNumberValidator<br>");
				}
				
			}
		}
		private const string _ImageTag="_ImageTag";
		private string mSN="";
        /// <summary>
        /// Gets the SN.
        /// </summary>
        /// <value>The SN.</value>
		protected string SN
		{
			get
			{
				return mSN;
			}
		}
        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"></see> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"></see> object that contains the event data.</param>
		protected override void OnInit(EventArgs e)
		{
			base.OnInit (e);
			string sn = Page.Request.QueryString[_ImageTag];
			
			if(sn != null)
			{
				Byte[] bytes =null;
				try
				{
					Crypto a = new Crypto();
					a.CryptText =sn;
					a.CryptIV = IV;
					a.CryptKey = Key;
					sn = a.Decrypt();
					System.Collections.ArrayList imgs = new System.Collections.ArrayList();
					int width =0;
					System.Drawing.Image img = null;
					foreach(Char item in sn)
					{
						img = (System.Drawing.Image)Images[item.ToString()];
						width += img.Width;
						imgs.Add(img);
					}

					Bitmap bmp = new Bitmap(width,37);
					Graphics grap = Graphics.FromImage(bmp);
					int left =0;
					foreach(System.Drawing.Image  item in imgs)
					{
						
						grap.DrawImage(item,left,0);
						left += item.Width;
					}
					imgs.Clear();
					grap.Flush();
					grap.Dispose();
					System.IO.MemoryStream stream = new System.IO.MemoryStream();
					bmp.Save(stream,System.Drawing.Imaging.ImageFormat.Gif);
					bmp.Dispose();
					bytes=new byte[stream.Length];
					stream.Position =0;
					stream.Read(bytes,0,bytes.Length);
					stream.Close();
				}
				catch(Exception e_)
				{
					string str = e_.Message;
				}
				Page.Response.Clear();
				Page.Response.BinaryWrite(bytes);
				Page.Response.Flush();
				Page.Response.End();
			}
		}

        /// <summary>
        /// Creates this instance.
        /// </summary>
		public void Create()
		{
			OnCreate();
		}

        /// <summary>
        /// Checks the SN.
        /// </summary>
        /// <param name="sn">The sn.</param>
        /// <returns>Whether sn is correct.</returns>
		public bool CheckSN(string sn)
		{
			return sn.ToUpper() == mSN;
		}

		private void OnCreate()
		{
			mSN = "";
			Random ran = new Random();
			int length = ran.Next(MinCharCount, MaxCharCount + 1);
			int unit =0;
			for(int i =0;i<length;i++)
			{
				unit = ran.Next(0,Seed.Length - 1);
                mSN += Seed.Substring(unit, 1);
			}
		}

        private const string _Seed = "1234567890ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string _SeedNumberOnly = "1234567890";
        private const string _SeedAlphabetOnly = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        /// <summary>
        /// Gets the seed.
        /// </summary>
        /// <value>The seed.</value>
        public string Seed
        {
            get
            {
                if (CharType == SerialNumberType.AlphabetOnly)
                {
                    return _SeedAlphabetOnly;
                }
                else if (CharType == SerialNumberType.NumberOnly)
                {
                    return _SeedNumberOnly;
                }
                else
                {
                    return _Seed;
                }
            }
        }

        /// <summary>
        /// Saves any server control view-state changes that have occurred since the time the page was posted back to the server.
        /// </summary>
        /// <returns>
        /// Returns the server control's current view state. If there is no view state associated with the control, this method returns null.
        /// </returns>
        protected override object SaveViewState()
		{
			Crypto a = new Crypto();
			a.CryptText =mSN;
			a.CryptIV = IV;
			a.CryptKey = Key;
			
			return new object[]{base.SaveViewState (),a.Encrypt()};
		}

        /// <summary>
        /// Restores view-state information from a previous page request that was saved by the <see cref="M:System.Web.UI.Control.SaveViewState"></see> method.
        /// </summary>
        /// <param name="savedState">An <see cref="T:System.Object"></see> that represents the control state to be restored.</param>
		protected override void LoadViewState(object savedState)
		{
			Object[] objs = (Object[])savedState;
			base.LoadViewState (objs[0]);
			Crypto a = new Crypto();
			a.CryptText =(string)objs[1];
			a.CryptIV = IV;
			a.CryptKey = Key;
			mSN = a.Decrypt();
		}

		static byte[] Key = {0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16};
		static byte[] IV = {0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16};
	}

    /// <summary>
    /// Crypto helper class.
    /// </summary>
  public class Crypto
    {
        private string _CryptText;
        private byte[] _CryptKey;
        private byte[] _CryptIV;

        /// <summary>
        /// Gets or sets the crypt text.
        /// </summary>
        /// <value>The crypt text.</value>
        public string CryptText
        {
            set
            {
                _CryptText = value;
            }
            get
            {
                return _CryptText;
            }
        }

        /// <summary>
        /// Gets or sets the crypt key.
        /// </summary>
        /// <value>The crypt key.</value>
        public byte[] CryptKey
        {
            set
            {
                _CryptKey = value;
            }
            get
            {
                return _CryptKey;
            }
        }

        /// <summary>
        /// Gets or sets the crypt IV.
        /// </summary>
        /// <value>The crypt IV.</value>
        public byte[] CryptIV
        {
            set
            {
                _CryptIV = value;
            }
            get
            {
                return _CryptIV;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Crypto"/> class.
        /// </summary>
        public Crypto()
        {
        }

        /// <summary>
        /// Encrypts this instance.
        /// </summary>
        /// <returns>The encrypted str.</returns>
        public string Encrypt()
        {
            string strEnText = CryptText;
            byte[] EnKey = CryptKey;
            byte[] EnIV = CryptIV;

            byte[] inputByteArray = System.Text.Encoding.UTF8.GetBytes(strEnText);

            RijndaelManaged RMCrypto = new RijndaelManaged();

            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, RMCrypto.CreateEncryptor(EnKey, EnIV), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();

            return Convert.ToBase64String(ms.ToArray());
        }

        /// <summary>
        /// Decrypts this instance.
        /// </summary>
        /// <returns>The decrypt string.</returns>
        public string Decrypt()
        {
            string strDeText = CryptText;
            byte[] DeKey = CryptKey;
            byte[] DeIV = CryptIV;

            byte[] inputByteArray = Convert.FromBase64String(strDeText);

            RijndaelManaged RMCrypto = new RijndaelManaged();

            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, RMCrypto.CreateDecryptor(DeKey, DeIV), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();

            return System.Text.Encoding.UTF8.GetString(ms.ToArray());
        }
    }
}
