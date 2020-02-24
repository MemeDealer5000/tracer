using System;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;

namespace racert
{
    class Program
    {
        public static void Main()
        {
            Console.Write("Enter ip to trace here:");
            String ip = Console.ReadLine();
            var result  = Traceroute(ip);
        }
        #region Trasert Plugin

        public static string Traceroute(string ipAddressOrHostName)//, PingerDevice device)
        {

            PingReply pingReply = null;


            StringBuilder traceResults = new StringBuilder();
            using (Ping pingSender = new Ping())
            {
                PingOptions pingOptions = new PingOptions();
                Stopwatch stopWatch = new Stopwatch();
                byte[] bytes = new byte[32];
                pingOptions.DontFragment = true;
                pingOptions.Ttl = 1;
                int maxHops = 30;
                //IPAddress ipAddress = Dns.GetHostEntry(ipAddressOrHostName).AddressList[0];
                traceResults.AppendLine(string.Format("Tracing route to {0} over a maximum of {1} hops:", ipAddressOrHostName, maxHops));
                traceResults.AppendLine("Trace may take some time...");
                traceResults.AppendLine();


                for (int i = 1; i < maxHops + 1; i++)
                {
                    stopWatch.Reset();
                    stopWatch.Start();
                    pingReply = pingSender.Send(ipAddressOrHostName, 1000, new byte[32], pingOptions);
                    stopWatch.Stop();

                    if (pingReply.Status != IPStatus.TtlExpired && pingReply.Status != IPStatus.Success)
                    {
                        traceResults.AppendLine(string.Format("{0} \t{1}", i, pingReply.Status.ToString()));
                    }

                    else
                    {
                        IPHostEntry @ipHost = Dns.Resolve(pingReply.Address.ToString());

                        if (ipHost.HostName != pingReply.Address.ToString())
                        {
                            traceResults.AppendLine(string.Format("{0}\t{1} ms\t{2} \t[{3}]", i, stopWatch.ElapsedMilliseconds, pingReply.Address, ipHost.HostName));
                        }
                        else
                        {
                            traceResults.AppendLine(string.Format("{0}\t{1} ms\t{2}", i, stopWatch.ElapsedMilliseconds, pingReply.Address));
                        }
                    }

                    if (pingReply.Status == IPStatus.Success)
                    {
                        traceResults.AppendLine();
                        traceResults.AppendLine("Trace complete.");
                        Console.WriteLine("{0}", traceResults.ToString());
                        //device.SendMessage(string.Format(ISPluginResources.msgTracetr, traceResults.ToString()));
                        break;

                    }
                    pingOptions.Ttl++;
                }
            }

            return traceResults.ToString();
        }
        #endregion
    }
}
