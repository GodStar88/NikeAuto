using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NikeAuto.Engine
{
    class Log
    {
        private MainForm _MainForm;
        public delegate void FormController(string message);

        public Log(MainForm mainForm)
        {
            _MainForm = mainForm;
        }

        public void PrintTaskStatus(int pThreadIndex, string status)
        {
            _MainForm.TaskGridView.Rows[pThreadIndex].Cells["STATUS"].Value = status;
        }

        public void UpdateInputFile(string fileName, string FolderName, List<string> updateList)
        {
            try
            {
                TextWriter wirter = new StreamWriter(@Program.basePath + "\\" + FolderName + "\\" + fileName + ".txt");
                for (int i = 0; i < updateList.Count; i++)
                {
                    wirter.WriteLine(updateList[i]);
                }
                wirter.Close();
                Console.WriteLine("\tRemove Suspended Account In Account File");
            }
            catch (Exception e)
            {
                Console.WriteLine("Input file update exception: " + e.Message);
            }
        }

        public void SaveLog(string logFileName, string logFolderName, string logInfo)
        {
            try
            {
                if (Directory.Exists(@Program.basePath + "\\" + logFolderName))
                {
                    if (!File.Exists(@Program.basePath + "\\" + logFolderName + "\\" + logFileName + ".txt"))
                    {
                        File.Create(@Program.basePath + "\\" + logFolderName + "\\" + logFileName + ".txt");
                        TextWriter tw = new StreamWriter(@Program.basePath + "\\" + logFolderName + "\\" + logFileName + ".txt");
                        tw.WriteLine(logInfo, Encoding.Default);
                        tw.Close();
                    }
                    else if (File.Exists(@Program.basePath + "\\" + logFolderName + "\\" + logFileName + ".txt"))
                    {
                        using (var tw = new StreamWriter(@Program.basePath + "\\" + logFolderName + "\\" + logFileName + ".txt", true))
                        {
                            tw.WriteLine(logInfo, Encoding.Default);
                        }
                    }
                }
                else
                {
                    DirectoryInfo di = Directory.CreateDirectory(@Program.basePath + "\\log");
                    Console.WriteLine("The directory was created successfully at {0}.", Directory.GetCreationTime(@Program.basePath));
                    File.Create(@Program.basePath + "\\" + logFolderName + "\\" + logFileName + ".txt");
                    TextWriter tw = new StreamWriter(@Program.basePath + "\\" + logFolderName + "\\" + logFileName + ".txt");
                    tw.WriteLine(logInfo, Encoding.Default);
                    tw.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Log file creation and save log exception :" + e.Message);
            }
        }
    }
}
