using System;
using System.IO;
using System.Windows.Forms;
using Shell32;

namespace NikeAuto
{
    static class Program
    {
        public static string basePath = Path.GetDirectoryName(Application.ExecutablePath);
        public static string noxExePath;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //////string noxShortcutPath = @"C:\\Users\\" + Environment.UserName + "\\Desktop\\Nox.lnk";
            string noxShortcutPath = @"C:\\Users\\Public\\Desktop\\BlueStacks.lnk";
            noxExePath = GetShortcutTargetFile(noxShortcutPath);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new LoginForm());
        }

        public static string GetShortcutTargetFile(string shortcutFilename)
        {
            string pathOnly = Path.GetDirectoryName(shortcutFilename);
            string filenameOnly = Path.GetFileName(shortcutFilename);

            Shell shell = new Shell();
            Folder folder = shell.NameSpace(pathOnly);
            FolderItem folderItem = folder.ParseName(filenameOnly);
            if (folderItem != null)
            {
                ShellLinkObject link = (ShellLinkObject)folderItem.GetLink;
                return link.Path;
            }

            return string.Empty;
        }
    }
}
