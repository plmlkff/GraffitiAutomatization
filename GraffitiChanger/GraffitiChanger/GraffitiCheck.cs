using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;


namespace GraffitiChanger
{
    class GraffitiCheck
    {
        public static List<string> _clientWhiteList = new List<string>();//Xlient's graffities that are now free
        public static List<string> _projectWhiteList = new List<string>();//Projet's graffities that are now free
        public static List<string> _graffitiList = new List<string>()
        {
            "https://testnet.ironfish.network/users/24219", //MOHITO
            "https://testnet.ironfish.network/users/24216", //ALBERTICH
            "https://testnet.ironfish.network/users/24220", //tvhome
            "https://testnet.ironfish.network/users/24052" //KOLA

        };
        public static void _startChecking()
        {
            ChromeDriverService service = ChromeDriverService.CreateDefaultService();
            service.EnableVerboseLogging = false;
            service.SuppressInitialDiagnosticInformation = true;
            service.HideCommandPromptWindow = true;

            ChromeOptions chromeOptions = new ChromeOptions();
            chromeOptions.PageLoadStrategy = PageLoadStrategy.Eager;
            chromeOptions.AddArgument("--headless"); //Disable the graphical shell of the window
            List<string> toChange = new List<string>();
            Terminal.labelOutput("Graffiti verification process started");
            IWebDriver driver = new ChromeDriver(chromeOptions);

            foreach (string address in _graffitiList)
            {
                string name = _getGraffitiNameOrAddress(address);
                driver.Navigate().GoToUrl(address);
                string element = "";
                while (driver.FindElements(By.ClassName("text-5xl")).Count == 0) { }
                element = driver.FindElements(By.ClassName("text-5xl"))[0].Text;
                if (element == "0")
                {
                    _sendToWhiteList(address);

                }

                if (element != "0" && (_clientWhiteList.Contains(_getGraffitiNameOrAddress(address)) || _projectWhiteList.Contains(_getGraffitiNameOrAddress(address))))
                {
                    if (_clientWhiteList.Contains(_getGraffitiNameOrAddress(address)))
                    {
                        _clientWhiteList.Remove(_getGraffitiNameOrAddress(address));
                    }
                    else
                    {
                        _projectWhiteList.Remove(_getGraffitiNameOrAddress(address));
                    }
                }

                if (element == "1 000")
                {
                    toChange.Add(name);
                }
            }
            driver.Quit();

            foreach (string name in toChange)
            {
                ConnectionLogic._startGraffitiChanging(name);
            }
            toChange = new List<string>();
            if (App.isWorking)
            {
                App.timer.Start();
            }
        }

        public static void _sendToWhiteList(string address)
        {
            string name = _getGraffitiNameOrAddress(address);
            if (ExcelLogic.data.Count(x => x!=null && x[2] == name) == 0)//If graffiti already has servers - graffiti is not added to the whitelist
            {
                switch (name)
                {
                    case "MOHITO":
                        _clientWhiteList.Add("MOHITO");
                        break;
                    case "KOLA":
                        _projectWhiteList.Add("KOLA");
                        break;
                    case "ALBERTICH":
                        _clientWhiteList.Add("ALBERTICH");
                        break;
                    case "tvhome":
                        _clientWhiteList.Add("tvhome");
                        break;


                }
            }
        }

        public static bool _singleCheckByName(string address) //Checking graffiti by name
        {
            ChromeOptions chromeOptions = new ChromeOptions();
            chromeOptions.AddArgument("--headless"); //Disable the graphical shell of the window
            chromeOptions.AddArgument("log-level=3");
            IWebDriver driver = new ChromeDriver(chromeOptions);
            driver.Navigate().GoToUrl(address);
            string elements = driver.FindElements(By.ClassName("text-5xl"))[0].Text;
            return false;
        }

        public static string _getGraffitiNameOrAddress(string name)
        {
            if (_graffitiList.Contains(name))
            {
                switch (_graffitiList.IndexOf(name))
                {
                    case 0:
                        return "MOHITO";
                    case 1:
                        return "ALBERTICH";
                    case 2:
                        return "tvhome";
                    case 3:
                        return "KOLA";
                    default:
                        return "";
                }
            }
            else
            {
                switch (name)
                {
                    case "MOHITO":
                        return _graffitiList[0];
                    case "KOLA":
                        return _graffitiList[3];
                    case "ALBERTICH":
                        return _graffitiList[1];
                    case "tvhome":
                        return _graffitiList[2];
                    default:
                        return "";
                }
            }
        }
    }
}
