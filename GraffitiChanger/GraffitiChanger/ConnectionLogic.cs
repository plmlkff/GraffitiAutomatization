using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading;

namespace GraffitiChanger
{
    class ServerData
    {
        public string ip { get; set; }
        public string pass { get; set; }
    }
    class ConnectionLogic
    {
        public static void _startGraffitiChanging(string graffitiName)//Starts the process of changing graffiti
        {
            string newGraff = _chooseNewGraff();//Choose a free graff
            List<ServerData> servers = (List<ServerData>)_getIpAndPassByGraff(graffitiName, newGraff);//Getting information about the servers that need to be redirected
            if (newGraff!=null)
            {
                Process cmd;
                App.labelOutput($"Redirect process started from {graffitiName} to {newGraff}");
                foreach (var server in servers)
                {
                    cmd = new Process();
                    cmd.StartInfo = new ProcessStartInfo(@"cmd.exe")
                    {
                        RedirectStandardInput = true,
                        RedirectStandardOutput = true,
                        UseShellExecute = false
                    };
                    cmd.Start();


                    cmd.StandardInput.WriteLine(@$"ssh -tt root@{server.ip}");//Create connection
                    if (cmd.StandardOutput.ReadLine().IndexOf("Are you sure you want to") != -1)
                    {
                        cmd.StandardInput.WriteLine("yes");
                    }

                    cmd.StandardInput.WriteLine(server.pass);

                    while (cmd.StandardOutput.ReadLine().IndexOf("Last login:") == -1) { Thread.Sleep(300); }

                    string command = $"ironfish config:set nodeName {newGraff} \nironfish config:set blockGraffiti {newGraff} \nironfish accounts:create {newGraff} \nironfish accounts:use {newGraff}";

                    cmd.StandardInput.WriteLine(command);

                    Thread.Sleep(200);

                    App.labelOutput($"ip: {server.ip} redirected to graffiti: {newGraff}");
                    ExcelLogic._commitingChanges(graffitiName, newGraff);
                }
                App.labelOutput("Redirect process completed");
            }
        }

        private static string _chooseNewGraff()
        {
            if (GraffitiCheck._clientWhiteList.Count!=0)
            {
                string output = GraffitiCheck._clientWhiteList[0];
                GraffitiCheck._clientWhiteList.RemoveAt(0);
                return output;
            }
            if (GraffitiCheck._projectWhiteList.Count != 0)
            {
                string output = GraffitiCheck._projectWhiteList[0];
                GraffitiCheck._projectWhiteList.RemoveAt(0);
                return output;
            }
            App.labelOutput("No free graffiti");
            return "";
        }

        public static IEnumerable<ServerData> _getIpAndPassByGraff(string oldGraff, string newGraff)
        {
            string[][] data = ExcelLogic.data;
            for (int i = 0; i < data.GetLength(0); i++)
            {
                if (data[i][2]==oldGraff)
                {
                    ServerData server = new ServerData()
                    {
                        ip = data[i][0],
                        pass = data[i][1]
                    };
                    data[i][2] = newGraff;
                    yield return server;
                }
            }
        }
    }
}
