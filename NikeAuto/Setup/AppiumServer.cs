using System;
using NikeAuto.Engine;
using OpenQA.Selenium.Appium.Service;
using OpenQA.Selenium.Appium.Service.Options;

namespace NikeAuto.Setup
{
    class AppiumServer
    {
        private AppiumLocalService service;
        public int appiumServerPort = 4723;
        public int bootstrapPort = 4724;
        public int selendroidPort = 8080;
        private Log _Log;

        public AppiumServer(MainForm mainForm)
        {
            _Log = new Log(mainForm);
        }

        public void AppiumServerRun(int pThreadIndex)
        {
            try
            {
                OptionCollector args = new OptionCollector().AddArguments(GeneralOptionList.OverrideSession());
                args.AddArguments(AndroidOptionList.BootstrapPort((bootstrapPort + pThreadIndex * 2).ToString()));
                args.AddArguments(AndroidOptionList.SelendroidPort((selendroidPort + pThreadIndex * 2).ToString()));
                service = new AppiumServiceBuilder().UsingPort(appiumServerPort + pThreadIndex * 2).WithArguments(args).Build();
                service.Start();
            }
            catch (Exception e)
            {
                _Log.PrintTaskStatus(pThreadIndex, "\tCreate the Appium Server Exception : " + e.Message);
            }
        }
    }
}
