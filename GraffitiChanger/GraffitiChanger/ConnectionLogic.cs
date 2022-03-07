using System;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using Renci.SshNet;

namespace GraffitiChanger
{
    class Client
    {
        public SshClient currentClient { get; set; }
        public ShellStream currentStream { get; set; }
    }
    class ServerData
    {
        public string ip { get; set; }
        public string pass { get; set; }
    }
    class ConnectionLogic
    {
        public static void _startGraffitiChanging(string graffitiName)//Starts the process of changing graffiti
        {
            List<Client> clients = new List<Client>();
            string newGraff = _chooseNewGraff();//Choose a free graff
            List<ServerData> servers = _getIpAndPassByGraff(graffitiName, newGraff);//Getting information about the servers that need to be redirected
            if (servers.Count!=0)
            {
                _removeFromWhiteList();
            }
            if (newGraff!=null)
            {
                SshClient client;

                foreach (var server in servers)
                {
                    try
                    {
                        client = new SshClient(server.ip, "root", server.pass);
                        string cmd = new String($"ironfish config:set nodeName {newGraff};ironfish config:set blockGraffiti {newGraff};ironfish accounts:create {newGraff};ironfish accounts:use {newGraff}");
                        client.Connect();
                        if (client.IsConnected)
                        {
                            ShellStream stream = client.CreateShellStream("input", 0, 0, 0, 0, 1000000);
                            stream.WriteLine(cmd);
                            clients.Add(new Client()
                            {
                                currentClient = client,
                                currentStream = stream
                            });
                        }
                        else
                        {
                            Terminal.labelOutput($"Server connection error: {server.ip}");
                        }
                    }
                    catch (Exception)
                    {
                        Terminal.labelOutput($"Server connection error: {server.ip}");
                    }
                }
                ExcelLogic._commitingChanges(graffitiName, newGraff);
                if (clients.Count!=0)
                {
                    foreach (Client cl in clients)
                    {
                        while (cl.currentStream.Read() == "") { Thread.Sleep(100); }
                        while (!cl.currentStream.Read().Contains("The default account is now:")) { Thread.Sleep(300); }
                        cl.currentClient.Disconnect();
                        Terminal.labelOutput($"IP: {cl.currentClient.ConnectionInfo.Host}  redirected form {graffitiName} to {newGraff}");
                    }
                    Terminal.labelOutput($"Redirect process from {graffitiName} to {newGraff} completed");
                }
                clients = new List<Client>();
            }
        }

        private static void _removeFromWhiteList()
        {
            if (GraffitiCheck._clientWhiteList.Count != 0)
            {
                GraffitiCheck._clientWhiteList.RemoveAt(0);
            }
            if (GraffitiCheck._projectWhiteList.Count != 0)
            {
                GraffitiCheck._projectWhiteList.RemoveAt(0);
            }
        }

        private static string _chooseNewGraff()
        {
            if (GraffitiCheck._clientWhiteList.Count!=0)
            {
                string output = GraffitiCheck._clientWhiteList[0];
                return output;
            }
            if (GraffitiCheck._projectWhiteList.Count != 0)
            {
                string output = GraffitiCheck._projectWhiteList[0];
                return output;
            }
            Terminal.labelOutput("No free graffiti");
            return "";
        }

        private static List<ServerData> _getIpAndPassByGraff(string oldGraff, string newGraff)
        {
            List<ServerData> servers = new List<ServerData>();
            string[][] data = ExcelLogic.data;
            for (int i = 1; i < ExcelLogic.lastRow; i++)
            {
                if (data[i][2]==oldGraff)
                {
                    ServerData server = new ServerData()
                    {
                        ip = data[i][0],
                        pass = data[i][1]
                    };
                    data[i][2] = newGraff;
                    servers.Add(server);
                }
            }
            ExcelLogic.data = data;
            return servers;
        }
    }
}
