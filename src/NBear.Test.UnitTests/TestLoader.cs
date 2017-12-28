using System;
using System.Threading;

namespace NBear.Test.UnitTests
{
    public delegate object TestHandler1();
    public delegate void TestHandler2(object obj);

	/// <summary>
	/// Timeout Test
	/// </summary>
	public class TestLoader
	{
        private TestLoader(int time, TestHandler1 handler)
        {
            _Time = time;
            _Handler = handler;
            _Spend = 0;
        }

        private TestLoader(int time, TestHandler1 handler, TestHandler2 handler2)
        {
            _Time = time;
            _Handler = handler;
            _Handler2 = handler2;
            _Spend = 0;
        }

        private int _Time;
        private TestHandler1 _Handler;
        private TestHandler2 _Handler2 = null;
        private int _Spend;
		private int _ThreadCount;

		private static readonly TimeSpan [] _Timeouts = new TimeSpan[] {
			new TimeSpan(14563), new TimeSpan(43523), new TimeSpan(9812), new TimeSpan(45234), new TimeSpan(20234)};

		private void RunMultiThreadTest()
		{
			while (_Time > 0)
			{
				if (_ThreadCount < _Timeouts.Length)
				{
					Thread thread = new Thread(new ThreadStart(this.MultiThreadWorking));
					Interlocked.Increment(ref _ThreadCount);
					thread.Start();
					_Time --;
				}
				else
				{
					Thread.Sleep(0);
				}
			}
		}

		private void AddSpend(int spend)
		{
			lock(this)
			{
				_Spend += spend;
			}
		}

		private void MultiThreadWorking()
		{
			long x0 = DateTime.Now.Ticks;
            object x = _Handler();
			x0 = DateTime.Now.Ticks - x0; 
			AddSpend((int) x0);
			TimeSpan ts = _Timeouts[_Time % _Timeouts.Length];
			AutoResetEvent are = new AutoResetEvent(false);
			are.WaitOne(ts, false);
            if (this._Handler2 != null)
            {
                _Handler2(x);
            }
			are.Close();
			Interlocked.Decrement(ref _ThreadCount);
			x = null;
		}

        private void SingleThreadWorking()
        {
            long x0 = DateTime.Now.Ticks;
            for (int i = 0; i < _Time; i++)
            {
                object x = _Handler();
                if (_Handler2 != null)
                {
                    _Handler2(x);
                }
                x = null;
            }
            x0 = DateTime.Now.Ticks - x0;
            AddSpend((int)x0);
        }

        public static TimeSpan GetSingleThreadSpend(int time, TestHandler1 handler)
        {
            TestLoader tests = new TestLoader(time, handler);
            tests.SingleThreadWorking();
            return new TimeSpan(tests._Spend);
        }

        public static TimeSpan GetSingleThreadSpend(int time, TestHandler1 handler, TestHandler2 handler2)
        {
            TestLoader tests = new TestLoader(time, handler, handler2);
            tests.SingleThreadWorking();
            return new TimeSpan(tests._Spend);
        }

        public static TimeSpan GetMultiThreadSpend(int time, TestHandler1 handler)
		{
            TestLoader tests = new TestLoader(time, handler);
			tests.RunMultiThreadTest();
			return new TimeSpan(tests._Spend);
		}

        public static TimeSpan GetMultiThreadSpend(int time, TestHandler1 handler, TestHandler2 handler2)
        {
            TestLoader tests = new TestLoader(time, handler, handler2);
            tests.RunMultiThreadTest();
            return new TimeSpan(tests._Spend);
        }
	}
}
