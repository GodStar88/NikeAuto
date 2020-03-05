using System.Diagnostics;

namespace NikeAuto.Utils
{
    class EmulatorUtils
    {
        public string PlayerName { get; set; }
        Process process;
        ProcessStartInfo startInfo;

        public EmulatorUtils(string noxConsolePath)
        {
            process = new Process();
            startInfo = new ProcessStartInfo
            {
                FileName = noxConsolePath,
                WindowStyle = ProcessWindowStyle.Hidden
            };
        }
        public void Add()
        {
            startInfo.Arguments = "add -name:" + PlayerName + " -systemtype:5";
            process.StartInfo = startInfo;
            process.Start();
        }

        public void Remove()
        {
            startInfo.Arguments = "remove -name:" + PlayerName;
            process.StartInfo = startInfo;
            process.Start();
        }

        public void Launch()
        {
            startInfo.Arguments = "launch -name:" + PlayerName;
            process.StartInfo = startInfo;
            process.Start();
        }

        public void ModifySetting()
        {
            startInfo.Arguments = "modify -name:" + PlayerName + " -resolution:400,711,200 -imei:auto -androidid:auto -imsi:auto -mac:auto";
            process.StartInfo = startInfo;
            process.Start();
        }

        public void Quit()
        {
            startInfo.Arguments = "quit -name:" + PlayerName;
            process.StartInfo = startInfo;
            process.Start();
        }
    }
}
