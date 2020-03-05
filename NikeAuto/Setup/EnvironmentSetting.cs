using Microsoft.Win32;
using NikeAuto.Engine;
using System;
using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;

namespace NikeAuto.Setup
{
    public class EnvironmentSetting
    {
        private static RegistryKey envKey;
        private static string pcUserName = Environment.UserName;
        private static string javaHomeValue;
        private static string androidHomeValue;
        private const int HWND_BROADCAST = 0xffff;
        private const uint WM_SETTINGCHANGE = 0x001a;

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern bool SendNotifyMessage(IntPtr hWnd, uint Msg,
            UIntPtr wParam, string lParam);

        public EnvironmentSetting()
        {
            if (pcUserName == "LionelMessi")
            {
                androidHomeValue = "E:\\Work\\MobileDev\\AndroidSDK";
            }
            else
            {
                androidHomeValue = "C:\\Users\\" + pcUserName + "\\Desktop\\Environment\\AndroidSDK";
            }
            javaHomeValue = "C:\\Program Files\\Java\\jdk1.8.0_181";
        }

        public void SetEnviornment()
        {
            try
            {
                using (envKey = Registry.CurrentUser.OpenSubKey(@"Environment", true))
                {
                    Contract.Assert(envKey != null, @"registry key is missing!");
                    envKey.SetValue("ANDROID_HOME", androidHomeValue);
                    SendNotifyMessage((IntPtr)HWND_BROADCAST, WM_SETTINGCHANGE,
                        (UIntPtr)0, "Environment");
                }

                using (envKey = Registry.LocalMachine.OpenSubKey(
                    @"SYSTEM\CurrentControlSet\Control\Session Manager\Environment",
                    true))
                {
                    Contract.Assert(envKey != null, @"registry key is missing!");
                    envKey.SetValue("JAVA_HOME", javaHomeValue);
                    Object o = envKey.GetValue("Path");
                    string currentSystemPathValue = o.ToString();

                    if (currentSystemPathValue.Contains("AndroidSDK") == false)
                    {
                        string updateSystemPathValue = currentSystemPathValue + ";" + androidHomeValue + "\\build-tools\\23.0.1;" + androidHomeValue + "\\platform-tools;" + androidHomeValue + "\\emulator;" + androidHomeValue + "\\tools;";
                        envKey.SetValue("Path", updateSystemPathValue);
                    }

                    SendNotifyMessage((IntPtr)HWND_BROADCAST, WM_SETTINGCHANGE,
                        (UIntPtr)0, "Environment");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("System environment variables setting Eexeception: " + e.Message);
            }
        }
    }
}
