using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
namespace NBear.Web
{
    public class WebHelper
    {
        private static readonly object SyncObject = new object();

        private static long ID=-1;

        public static long GetUniqueID()
        {
            long num;
            DateTime time;
            lock (SyncObject)
            {
                do
                {
                    Thread.Sleep(10);
                    time = new DateTime(0x7d6, 11, 1, 0, 0, 0, 0);
                    num = Convert.ToInt64(DateTime.Now.Subtract(time).TotalMilliseconds);
                }
                while (num == ID);

                ID = num;

            }
            return Math.Abs(num);
        }
    }
}
