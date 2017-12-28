using System;
using System.Collections.Generic;
using System.Text;

using NBear.Common;
using NBear.Net.Remoting;
using NBear.IoC.Service;
using NBear.IoC.Service.Configuration;

namespace NBear.IoC.Hosts.ServiceHost
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceFactoryConfigurationSection config = ServiceFactory.LoadServiceFactoryConfiguration();
            LogHandler logger = (config.Debug ? new LogHandler(Console.WriteLine) : null);
            ServiceFactory.Create().OnLog += logger;

            Console.WriteLine("Service host started...");
            Console.WriteLine("Logger Status: " + (config.Debug ? "On" : "Off"));
            Console.WriteLine("Press any key to exit and stop host...");
            Console.ReadLine();
        }
    }
}
