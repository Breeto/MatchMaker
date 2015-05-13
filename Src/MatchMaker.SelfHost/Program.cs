using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
// using Nancy.Hosting.Self;
using MatchMaker.Web;
using Fos;
using Fos.Owin;
using Nancy;
using Nancy.Owin;
using Owin;

namespace MatchMaker.SelfHost
{
// 	public class Program : Nancy.NancyModule
// 	{
//         static void Main(string[] args)
//         {
//             var uri = "http://192.168.22.10:8888";
//             Console.WriteLine(uri);
//             // initialize an instance of NancyHost (found in the Nancy.Hosting.Self package)
//             var host = new NancyHost(new Bootstrapper(), new Uri(uri));
//             host.Start();  // start hosting
// 
//             //Under mono if you daemonize a process a Console.ReadLine will cause an EOF 
//             //so we need to block another way
//             if (args.Any(s => s.Equals("-d", StringComparison.CurrentCultureIgnoreCase)))
//             {
//                 Thread.Sleep(Timeout.Infinite);
//             }
//             else
//             {
//                 Console.ReadKey();
//             }
// 
//             host.Stop();  // stop hosting
//         }
// 	}

    public static class OwinAppSetup
    {
        private static void ConfigureOwin(IAppBuilder builder) {
            builder.UseNancy();
        }

        public static void Main(string[] args) {
            using (var fosServer = new FosSelfHost(ConfigureOwin))
            {
                fosServer.Bind(System.Net.IPAddress.Loopback, 9000);
                fosServer.Start(false);
            }
        }
    }

}
