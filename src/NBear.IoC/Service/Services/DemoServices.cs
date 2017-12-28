using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace NBear.IoC.Service.Services.DemoServices
{
    /// <summary>
    /// sample math service interface.
    /// </summary>
    public interface IMathService : IServiceInterface
    {
        /// <summary>
        /// Gets the result.
        /// </summary>
        /// <param name="op">The op.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns>The result.</returns>
        int GetResult(char op, int x, int y);
    }

    /// <summary>
    /// sample math service impl.
    /// </summary>
    [Serializable]
    public class MathService : BaseAutoService, IMathService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MathService"/> class.
        /// </summary>
        /// <param name="mq">The mq.</param>
        public MathService(IServiceMQ mq)
            : base("demo.math", mq)
        {
        }

        /// <summary>
        /// Gets the result.
        /// </summary>
        /// <param name="op">The op.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns>The result.</returns>
        public int GetResult(char op, int x, int y)
        {
            int rt = 0;
            switch (op)
            {
                case '+':
                    rt = x + y;
                    break;
                case '-':
                    rt = x - y;
                    break;
                case '*':
                    rt = x * y;
                    break;
                case '/':
                    rt = x / y;
                    break;

            }
            return rt;
        }

        /// <summary>
        /// Runs the specified MSG.
        /// </summary>
        /// <param name="msg">The MSG.</param>
        /// <returns>The msg.</returns>
        protected override ResponseMessage Run(RequestMessage msg)
        {
            int rt = GetResult(msg.Parameters["op"][0], int.Parse(msg.Parameters["x"]), int.Parse(msg.Parameters["y"]));
            ResponseMessage retMsg = new ResponseMessage();
            retMsg.ServiceName = msg.ServiceName;
            retMsg.Parameters["Result"] = rt.ToString();
            retMsg.MessageId = Guid.NewGuid();
            retMsg.TransactionId = msg.TransactionId;
            retMsg.Request = msg;
            retMsg.Timestamp = DateTime.Now;
            retMsg.Expiration = DateTime.Now.AddDays(1);

            return retMsg;
        }
    }
}
