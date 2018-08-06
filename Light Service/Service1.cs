using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace Light_Service
{
    public class Outlet
    {
        internal static void Turn(string state)
        {
            RegistryKey options = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\Workbench Light");
            string token = options.GetValue("token").ToString();
            string account = options.GetValue("accountid").ToString();
            string outlet = options.GetValue("outletid").ToString();

            string url = $"https://smartapi.vesync.com/v1/wifi-switch-1.3/{outlet}/status/{state}";
            
            using (var client = new System.Net.WebClient())
            {
                client.Headers.Add("tk", token);
                client.Headers.Add("accountid", account);

                bool success = false;
                int tries = 0;
                while (success == false && tries < 10)
                {
                    try
                    {
                        success = true;
                        client.UploadData(url, "PUT", Encoding.UTF8.GetBytes("{}"));
                        tries++;
                    }
                    catch (System.Net.WebException)
                    {
                        success = false;
                    }
                    System.Threading.Thread.Sleep(3000);
                }
            }
        }
    }

    public partial class Service1 : ServiceBase
    {
        public Service1()
        {
            InitializeComponent();
            this.CanHandlePowerEvent = true;
        }

        protected override void OnStart(string[] args)
        {
            //Outlet.Turn("on");
        }

        protected override bool OnPowerEvent(PowerBroadcastStatus powerStatus)
        {
            if (powerStatus.HasFlag(PowerBroadcastStatus.QuerySuspend))
            {
                Outlet.Turn("off");
            }

            if (powerStatus.HasFlag(PowerBroadcastStatus.ResumeSuspend))
            {
                Outlet.Turn("on");
            }
            return base.OnPowerEvent(powerStatus);
        }

        protected override void OnStop()
        {
            //Outlet.Turn("off");
        }
    }
}
