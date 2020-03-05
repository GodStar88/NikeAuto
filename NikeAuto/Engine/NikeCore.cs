using System;
using System.Collections.Generic;
using System.Threading;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Appium.Enums;
using OpenQA.Selenium.Appium.Android.Enums;
using OpenQA.Selenium.Appium.Android;
using NikeAuto.Define;
using System.Diagnostics;
using NikeAuto.Setup;
using System.Windows.Forms;
using NikeAuto.Utils;
using System.IO;
using System.Text;
using System.Drawing;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NikeAuto.Engine
{
    public class NikeCore
    {
        public Thread workerThreadPool;
        public List<Thread> nikeWorkerList = new List<Thread>();
        private Random random = new Random();
        DateTime myDateTime = DateTime.Now;

        private List<Task> taskList = new List<Task>();
        private List<Profile> profileList = new List<Profile>();
        private List<Proxy> proxyList = new List<Proxy>();
        private List<Account> accountList = new List<Account>();
        private List<Product> productList = new List<Product>();
        private List<EmulatorUtils> emulatorList = new List<EmulatorUtils>();

        private string noxExePath = Program.noxExePath;
        public string upcomingDate;

        public static int upcomingProductId;
        public static int sessionDelay { get; set; }
        public int systemPort = 8200;
        private int noxPort = 62001;
        public List<int> selectedTaskIndex = new List<int>();
        public List<int> selectedTaskId = new List<int>();

        public bool isTaskRun = false, isSameShipping = false;
        OpenFileDialog file = new OpenFileDialog();
        public delegate void FormController(string message);

        private EnvironmentSetting _EnvironmentSetting;
        private AppiumServer _LocalServer;
        private Log _Log;
        private EmulatorUtils _EmulatorUtils;
        private MainForm _MainForm;
        private Scraper _Scraper;

        public NikeCore(MainForm mainForm)
        {
            if (mainForm != null)
            {
                _MainForm = mainForm;
                _EnvironmentSetting = new EnvironmentSetting();
                _LocalServer = new AppiumServer(mainForm);
                _Log = new Log(mainForm);
                _Scraper = new Scraper();
                LoadUpcomingProduct();
                LoadProfile();
                LoadProxy();
                LoadAccount();
                LoadTask();
            }
        }

        public void CreateTask()
        {
            try
            {
                Task task = new Task();
                task.Sneaker = upcomingProductId;
                string sku = _MainForm.SKUComboBox.SelectedItem.ToString();
                task.Size = _MainForm.SizeComboBox.SelectedItem.ToString();
                task.Quantity = Convert.ToInt32(_MainForm.QuantityComboBox.SelectedItem);
                task.Profile = Convert.ToInt32((_MainForm.ProfilecomboBox.SelectedItem as ComboboxItem).Value);
                task.Shipping = 0;
                //////task.Proxy = 1;
                //////task.Account = 1;
                task.Proxy = FetchTaskProxy();
                task.Account = FetchTaskAccount();
                task.Status = "Idle";
                string mode = _MainForm.ModeComboBox.SelectedItem.ToString();
                if (mode == "Mobile")
                {
                    task.Mode = 0;
                }
                else
                {
                    task.Mode = 1;
                }
                SqliteDataAccess.SaveTask(task);
                MessageBox.Show("Successfully Created!");
                LoadTask();
            }
            catch (Exception e)
            {
                MessageBox.Show("Create a task exception: " + e.Message);
            }
        }

        public int FetchTaskProxy()
        {
            int ProxyID = 0, ProxyCount = 0;
            //////foreach (var proxyRow in proxyList)
            //////{
            //////    ProxyID = proxyRow.Id;
            //////}

            for (int i = 0; i < proxyList.Count; i++)
            {
                ProxyID = proxyList[i].Id;
                ProxyCount = GetProxyCount_Task(ProxyID);

                if (i == proxyList.Count - 1)
                {
                    if (ProxyCount == 0)
                    {
                        ProxyID = proxyList[i].Id;
                        return ProxyID;
                    }
                    else
                    {
                        ProxyID = proxyList[0].Id;
                        return ProxyID;
                    }
                }
                else
                {
                    if (ProxyCount == 0)
                    {
                        return ProxyID;
                    }
                }
            }
            return ProxyID;
        }

        public int FetchTaskAccount()
        {
            int AccountID = 0, AccountCount = 0;
            //////////foreach (var accountRow in accountList)
            //////////{
            //////////    AccountID = accountRow.Id;
            //////////}

            for (int i = 0; i < accountList.Count; i++)
            {
                AccountID = accountList[i].Id;
                AccountCount = GetAccountCount_Task(AccountID);

                if (i == accountList.Count - 1)
                {
                    if (AccountCount == 0)
                    {
                        AccountID = accountList[i].Id;
                        return AccountID;
                    }
                    else
                    {
                        AccountID = accountList[0].Id;
                        return AccountID;
                    }
                }
                else
                {
                    if (AccountCount == 0)
                    {
                        return AccountID;
                    }
                }
            }

            return AccountID;
        }

        public int GetProxyCount_Task(int ProxyId)
        {
            List<Task> ProxyCountList = new List<Task>();
            try
            {
                ProxyCountList = SqliteDataAccess.GetProxyCount_Task(ProxyId);
            }
            catch (Exception e)
            {
                MessageBox.Show("Load stored tasks exception: " + e.Message);
            }

            return ProxyCountList.Count();
        }

        public int GetAccountCount_Task(int AccountId)
        {
            List<Task> AccountCountList = new List<Task>();
            try
            {
                AccountCountList = SqliteDataAccess.GetAccountCount_Task(AccountId);
            }
            catch (Exception e)
            {
                MessageBox.Show("Load stored tasks exception: " + e.Message);
            }

            return AccountCountList.Count();
        }

        public void LoadUpcomingProduct(string upcomingDate = null)
        {
            try
            {
                int index = 0;
                if (upcomingDate != null)
                {
                    DateTime dt = new DateTime();
                    dt = Convert.ToDateTime(upcomingDate);
                    upcomingDate = dt.ToString("yyyy-MM-dd").ToString();
                }
                productList = SqliteDataAccess.LoadProduct(upcomingDate);
                _MainForm.UpcomingListPanel.Controls.Clear();
                foreach (var row in productList)
                {
                    if (row.Date == upcomingDate)
                    {
                        var upcomingProductInstance = new UpcomingProduct(null);
                        upcomingProductInstance.productId = Convert.ToInt32(row.Id);
                        ////////upcomingProductInstance.UpcomingProductImage.Width = upcomingProductInstance.UpcomingProductImage.Width - 31;
                        upcomingProductInstance.UpcomingProductImage.Size = new Size(upcomingProductInstance.UpcomingProductImage.Width, upcomingProductInstance.UpcomingProductImage.Height);
                        ////////upcomingProductInstance.UpcomingProductImage.Size = new Size(upcomingProductInstance.UpcomingProductImage.Width, 90);
                        upcomingProductInstance.UpcomingProductImage.Image = Image.FromFile(@Program.basePath + "\\image\\" + row.Image_Name.ToString());
                        upcomingProductInstance.UpcomingProductImage.SizeMode = PictureBoxSizeMode.StretchImage;
                        upcomingProductInstance.ProductNameLabel.Text = row.Model + "'" + row.Color + "'";
                        upcomingProductInstance.ProductUpcomingDateLabel.Text = row.Date + "," + row.Time;
                        upcomingProductInstance.ProductPriceLabel.Text = row.Price + "$";
                        upcomingProductInstance.Top = index * (upcomingProductInstance.Height + 10);
                        _MainForm.UpcomingListPanel.Controls.Add(upcomingProductInstance);
                        index += 1;
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Load stored upcoming products exception: " + e.Message);
            }
        }

        public List<Product> LoadProductSKU(int ProductID)
        {
            var productListSKU = SqliteDataAccess.LoadProductSKU(ProductID);

            return productListSKU;
        }

        public void FetchUpcommingProduct()
        {
            _Scraper.upcommingProductFetcher = new Thread(new ThreadStart(_Scraper.FetchUpcommingProduct));
            _Scraper.upcommingProductFetcher.Start();
        }

        public List<Profile> LoadProfileData(int ProfileID)
        {
            profileList = SqliteDataAccess.LoadProfileData(ProfileID);

            return profileList;
        }

        public void LoadProfile()
        {
            try
            {
                int index = 0;
                _MainForm.ProfilecomboBox.Items.Clear();
                _MainForm.ProfileGridView.Rows.Clear();
                profileList = SqliteDataAccess.LoadProfile();

                if (_MainForm.ProfileGridView.Columns.Contains("btn"))
                {
                    _MainForm.ProfileGridView.Columns.Remove("btn");
                }

                var btn = new DataGridViewButtonColumn();
                _MainForm.ProfileGridView.Columns.Add(btn);
                btn.HeaderText = "Action";
                btn.Text = "Edit Details";
                btn.Name = "btn";
                btn.UseColumnTextForButtonValue = true;

                foreach (var row in profileList)
                {
                    _MainForm.ProfileGridView.Rows.Add();
                    _MainForm.ProfileGridView.Rows[index].Cells["ProfileID"].Value = row.Id;
                    _MainForm.ProfileGridView.Rows[index].Cells["NAME"].Value = row.BFirstName + " " + row.BLastName;
                    _MainForm.ProfileGridView.Rows[index].Cells["DETAILS"].Value = row.BAddress;

                    ComboboxItem item = new ComboboxItem();
                    item.Text = row.BFirstName + " " + row.BLastName;
                    item.Value = row.Id;
                    _MainForm.ProfilecomboBox.Items.Add(item);
                    _MainForm.ProfilecomboBox.SelectedIndex = 0;
                    index += 1;
                }

                _MainForm.ProfileGridView.Columns["ProfileID"].Visible = false;
                _MainForm.ProfileGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            catch (Exception e)
            {
                MessageBox.Show("Load stored profiles exception: " + e.Message);
            }
        }

        private void LoadProxy()
        {
            try
            {
                proxyList = SqliteDataAccess.LoadProxy();
                foreach (var row in proxyList)
                {
                    _MainForm.ProxyTextBox.AppendText(row.Ip + ":" + row.Port + ":" + row.UserId + ":" + row.Password);
                    _MainForm.ProxyTextBox.AppendText(Environment.NewLine);
                }
                _MainForm.ShowProxyAmount(proxyList.Count.ToString());
            }
            catch (Exception e)
            {
                MessageBox.Show("Load stored proxies exception: " + e.Message);
            }
        }

        private void LoadAccount()
        {
            try
            {
                accountList = SqliteDataAccess.LoadAccount();
                foreach (var row in accountList)
                {
                    _MainForm.AccountTextBox.AppendText(row.Email + ":" + row.Password);
                    _MainForm.AccountTextBox.AppendText(Environment.NewLine);
                }
                _MainForm.ShowAccountAmount(accountList.Count.ToString());
            }
            catch (Exception e)
            {
                MessageBox.Show("Load stored account exception: " + e.Message);
            }
        }

        public void LoadTask()
        {
            try
            {
                taskList = SqliteDataAccess.LoadTask();
                LoadTaskGridView();
            }
            catch (Exception e)
            {
                MessageBox.Show("Load stored tasks exception: " + e.Message);
            }
        }

        public void LoadTaskGridView()
        {
            try
            {
                int index = 0;
                _MainForm.TaskGridView.Rows.Clear();
                foreach (var row in taskList)
                {
                    _MainForm.TaskGridView.Rows.Add();
                    _MainForm.TaskGridView.Rows[index].Cells["taskId"].Value = row.Id;
                    _MainForm.TaskGridView.Rows[index].Cells["ID"].Value = index + 1;
                    foreach (var productRow in productList)
                    {
                        if (productRow.Id == row.Sneaker)
                        {
                            _MainForm.TaskGridView.Rows[index].Cells["SNEAKER"].Value = productRow.Model + "'" + productRow.Color + "'";
                            _MainForm.TaskGridView.Rows[index].Cells["SKU"].Value = productRow.SKU;
                        }
                    }
                    foreach (var proxyRow in proxyList)
                    {
                        if (proxyRow.Id == row.Proxy)
                        {
                            _MainForm.TaskGridView.Rows[index].Cells["PROXY"].Value = proxyRow.Ip;
                        }
                    }
                    _MainForm.TaskGridView.Rows[index].Cells["SIZE"].Value = row.Size;
                    foreach (var accountRow in accountList)
                    {
                        if (accountRow.Id == row.Account)
                        {
                            _MainForm.TaskGridView.Rows[index].Cells["ACCOUNT"].Value = accountRow.Email;
                        }
                    }
                    foreach (var profileRow in profileList)
                    {
                        if (profileRow.Id == row.Profile)
                        {
                            _MainForm.TaskGridView.Rows[index].Cells["PROFILE"].Value = profileRow.BFirstName + " " + profileRow.BLastName;
                        }
                    }
                    _MainForm.TaskGridView.Rows[index].Cells["STATUS"].Value = "Idle";
                    index += 1;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Load task gridview exception: " + e.Message);
            }
        }

        public void StopTask()
        {
            try
            {
                if (isTaskRun)
                {
                    if (selectedTaskIndex.Count > 0)
                    {
                        for (int i = 0; i < selectedTaskIndex.Count; i++)
                        {
                            nikeWorkerList[i].Abort();
                            _MainForm.TaskGridView.Rows[selectedTaskIndex[i]].Cells["STATUS"].Value = "Stoped";
                            if (emulatorList.Count > 0)
                            {
                                foreach (EmulatorUtils row in emulatorList)
                                {
                                    row.Quit();
                                }
                            }
                        }
                        nikeWorkerList.Clear();
                        isTaskRun = false;
                    }
                    else
                    {
                        MessageBox.Show("There are not selected tasks yet.");
                    }
                }
                else
                {
                    MessageBox.Show("There are not running task yet.");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Stop task exception: " + e.Message);
            }
        }

        public void DeleteAllTask()
        {
            try
            {
                SqliteDataAccess.DeleteAllTask();
                taskList.Clear();
                foreach (var row in profileList)
                {
                    if (row.isUse)
                        row.isUse = false;
                }
                foreach (var row in proxyList)
                {
                    if (row.isUse)
                        row.isUse = false;
                }
                foreach (var row in accountList)
                {
                    if (row.isUse)
                        row.isUse = false;
                }
                _MainForm.TaskGridView.Rows.Clear();
                _MainForm.TaskGridView.RowCount = 0;
                _MainForm.TaskGridView.Refresh();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Delete all tasks exception: " + ex.Message);
            }
        }

        public void DeleteSelectedTask()
        {
            try
            {
                if (selectedTaskId.Count > 0)
                {
                    string sqlDelete = "DELETE FROM Task WHERE Id IN (";
                    for (int j = 0; j < selectedTaskId.Count; j++)
                    {
                        sqlDelete += selectedTaskId[j] + ",";
                    }
                    sqlDelete = sqlDelete.Remove(sqlDelete.Length - 1);
                    sqlDelete += ")";
                    SqliteDataAccess.DeleteTask(sqlDelete);
                    for (int i = 0; i < selectedTaskIndex.Count; i++)
                    {
                        foreach (var row in profileList)
                        {
                            if (row.Id == taskList[selectedTaskIndex[i]].Profile)
                            {
                                row.isUse = false;
                            }
                        }
                        foreach (var row in proxyList)
                        {
                            if (row.Id == taskList[selectedTaskIndex[i]].Proxy)
                            {
                                row.isUse = false;
                            }
                        }
                        foreach (var row in accountList)
                        {
                            if (row.Id == taskList[selectedTaskIndex[i]].Account)
                            {
                                row.isUse = false;
                            }
                        }
                    }
                    for (int j = 0; j < selectedTaskIndex.Count; j++)
                    {
                        taskList.RemoveAt(selectedTaskIndex[j] - j);
                    }
                    _MainForm.TaskGridView.Rows.Clear();
                    LoadTaskGridView();
                }
                else
                {
                    MessageBox.Show("There are not selected task to delete yet");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Delete selected tasks exception: " + ex.Message);
            }
        }

        public void DeleteAllProfiles()
        {
            try
            {
                SqliteDataAccess.DeleteAllProfile();
                _MainForm.ProfileGridView.Rows.Clear();
                _MainForm.ProfileGridView.Refresh();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Delete all profiles exception: " + ex.Message);
            }
        }

        public void DeleteProfile(int ProfileID)
        {
            try
            {
                SqliteDataAccess.DeleteProfile(ProfileID);
                LoadProfile();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Delete all profiles exception: " + ex.Message);
            }
        }

        public void ImportProfileInfo()
        {
            try
            {
                List<string> profileInfoList;
                file.Filter = "TEXT|*.txt";
                if (file.ShowDialog() == DialogResult.OK)
                {
                    profileInfoList = new List<string>(File.ReadAllLines(@file.FileName, Encoding.Default));
                    for (int i = 0; i < profileInfoList.Count; i++)
                    {
                        Profile profile = new Profile();
                        string[] profileInfoArray = profileInfoList[i].Split('-');
                        profile.CardNumber = profileInfoArray[0];
                        profile.CardYear = profileInfoArray[1].Split('/')[1];
                        profile.CardMonth = profileInfoArray[1].Split('/')[0];
                        profile.CardSecurity = profileInfoArray[2];
                        profile.BFirstName = profileInfoArray[3];
                        profile.BLastName = profileInfoArray[4];
                        profile.BAddress = profileInfoArray[5];
                        profile.BZip = profileInfoArray[6];
                        profile.BPhone = profileInfoArray[7];
                        profile.SFirstName = profileInfoArray[8];
                        profile.SLastName = profileInfoArray[9];
                        profile.SAddress = profileInfoArray[10];
                        profile.SZip = profileInfoArray[11];
                        profile.SPhone = profileInfoArray[12];
                        SqliteDataAccess.SaveProfile(profile);
                    }
                    LoadProfile();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Import the profile informations exception: " + ex.Message);
            }
        }

        public void CreateProfile(Profile profile)
        {
            SqliteDataAccess.SaveProfile(profile);
            LoadProfile();
        }

        public void UpdateProfile(Profile profile)
        {
            SqliteDataAccess.UpdateProfile(profile);
            LoadProfile();
        }

        public DataTable ExportProfile()
        {
            profileList = SqliteDataAccess.LoadProfile();
            DataTable dt = new DataTable();
            dt = ConvertListToDataTable(profileList);

            return dt;
        }

        static DataTable ConvertListToDataTable(List<Profile> items)
        {
            DataTable dataTable = new DataTable(typeof(Profile).Name);

            //Get all the properties

            PropertyInfo[] Props = typeof(Profile).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo prop in Props)

            {
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name);

            }

            foreach (Profile item in items)

            {

                var values = new object[Props.Length];

                for (int i = 0; i < Props.Length; i++)

                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }

            //put a breakpoint here and check datatable

            return dataTable;
        }

        public void SaveProxy(Proxy proxy)
        {
            SqliteDataAccess.SaveProxy(proxy);
        }

        public void DeleteAllProxy()
        {
            SqliteDataAccess.DeleteAllProxy();
        }

        public void SaveAccount(Account account)
        {
            SqliteDataAccess.SaveAccount(account);
        }

        public void DeleteAllAccount()
        {
            SqliteDataAccess.DeleteAllAccount();
        }

        public AndroidDriver<AndroidElement> BeforeAll(int pThreadIndex, string appName)
        {
            AndroidDriver<AndroidElement> driver;
            try
            {
                DesiredCapabilities capabilities = new DesiredCapabilities();
                capabilities.SetCapability(MobileCapabilityType.DeviceName, "Motorola AOSP On Shama");
                if (pThreadIndex == 0)
                {
                    capabilities.SetCapability(MobileCapabilityType.Udid, "127.0.0.1:" + noxPort.ToString());
                }
                else
                {
                    capabilities.SetCapability(MobileCapabilityType.Udid, "127.0.0.1:" + (noxPort + 23 + pThreadIndex).ToString());
                }
                capabilities.SetCapability(MobileCapabilityType.PlatformName, App.AndroidDeviceName());
                capabilities.SetCapability(MobileCapabilityType.PlatformVersion, App.AndroidPlatformVersion());
                capabilities.SetCapability(MobileCapabilityType.AutomationName, "UIAutomator2");
                capabilities.SetCapability(MobileCapabilityType.NewCommandTimeout, 5 * 60 * 1000);
                capabilities.SetCapability("systemPort", systemPort + pThreadIndex);

                if (appName == "SNKRS")
                {
                    //capabilities.SetCapability("appPackage", "com.nike.snkrs");
                    //capabilities.SetCapability("appActivity", "com.nike.snkrs.main.activities.SnkrsActivity");
                    capabilities.SetCapability(MobileCapabilityType.App, App.AndroidApp());
                }
                else if (appName == "Drony")
                {
                    capabilities.SetCapability(MobileCapabilityType.App, App.AndroidDronyApp());
                }

                driver = new AndroidDriver<AndroidElement>(Env.ServerUri((_LocalServer.appiumServerPort + pThreadIndex * 2).ToString()), capabilities, Env.INIT_TIMEOUT_SEC);
                driver.Manage().Timeouts().ImplicitWait = Env.IMPLICIT_TIMEOUT_SEC;
                _Log.PrintTaskStatus(pThreadIndex, "driver" + pThreadIndex + " successfully created!");
                return driver;
            }
            catch (Exception e)
            {
                _Log.PrintTaskStatus(pThreadIndex, "driver creation exception: " + e.Message);
                return null;
            }
        }

        public void KeyInput(AndroidDriver<AndroidElement> pDriver, int pThreadIndex, string inputString)
        {
            try
            {
                Thread.Sleep(1000);
                for (int k = 0; k < inputString.Length; k++)
                {
                    if (Keyboard.keyDict.TryGetValue(inputString[k].ToString(), out int dicVal))
                    {
                        if (dicVal > 900)
                        {
                            pDriver.PressKeyCode(dicVal - 900, AndroidKeyMetastate.Meta_Caps_Lock_On);
                        }
                        else
                        {
                            pDriver.PressKeyCode(dicVal);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _Log.PrintTaskStatus(pThreadIndex, "Keyboard Input Exception : " + e.Message);
            }
        }

        public void KillProcess(int pid)
        {
            Process process = Process.GetProcessById(pid);
            process.Kill();
        }

        public void SetProxy(AndroidDriver<AndroidElement> pDriver, int pThreadIndex)
        {
            try
            {
                foreach (var row in proxyList)
                {
                    if (row.Id == taskList[pThreadIndex].Proxy)
                    {
                        while (true)
                        {
                            try
                            {
                                pDriver.Swipe(390, 200, 100, 200, 1000);
                                pDriver.FindElementByXPath("//android.widget.TextView[contains(@text,'Wi-Fi')]").Click();
                                break;
                            }
                            catch (Exception e)
                            {
                                _Log.PrintTaskStatus(pThreadIndex, "Retry to swipe the screen for drony setting: " + e.Message);
                                Thread.Sleep(1000);
                            }
                        }

                        pDriver.FindElementById("org.sandroproxy.drony:id/network_list_item_id_name_value").Click();

                        pDriver.FindElementByXPath("//android.widget.TextView[contains(@text,'Hostname')]").Click();
                        pDriver.FindElementByClassName("android.widget.EditText").SendKeys(row.Ip);
                        pDriver.FindElementById("android:id/button1").Click();

                        pDriver.FindElementByXPath("//android.widget.TextView[contains(@text,'Port')]").Click();
                        pDriver.FindElementByClassName("android.widget.EditText").Clear();
                        pDriver.FindElementByClassName("android.widget.EditText").SendKeys(row.Port);
                        pDriver.FindElementById("android:id/button1").Click();

                        pDriver.FindElementByXPath("//android.widget.TextView[contains(@text,'Username')]").Click();
                        pDriver.FindElementByClassName("android.widget.EditText").SendKeys(row.UserId);
                        pDriver.FindElementById("android:id/button1").Click();

                        pDriver.FindElementByXPath("//android.widget.TextView[contains(@text,'Password')]").Click();
                        pDriver.FindElementByClassName("android.widget.EditText").SendKeys(row.Password);
                        pDriver.FindElementById("android:id/button1").Click();

                        pDriver.FindElementByAccessibilityId("Navigate up").Click();
                        pDriver.FindElementByAccessibilityId("Navigate up").Click();

                        while (true)
                        {
                            try
                            {
                                pDriver.Swipe(100, 200, 390, 200, 1000);
                                pDriver.FindElementById("org.sandroproxy.drony:id/toggleButtonOnOff").Click();
                                break;
                            }
                            catch (Exception e)
                            {
                                _Log.PrintTaskStatus(pThreadIndex, "Retry to swipe the screen for clicking on/off button: " + e.Message);
                                Thread.Sleep(1000);
                            }
                        }

                        try
                        {
                            if (pDriver.FindElementById("android:id/button1").Enabled)
                            {
                                pDriver.FindElementById("android:id/button1").Click();
                            }
                        }
                        catch
                        {
                            _Log.PrintTaskStatus(pThreadIndex, "Already confirmed to click the on/off button on modal");
                        }
                        _Log.PrintTaskStatus(pThreadIndex, "Proxy setting successfully");
                    }
                }
            }
            catch (Exception e)
            {
                _Log.PrintTaskStatus(pThreadIndex, "Proxy Setting Exception: " + e.Message);
            }
        }

        private void NikeLogin(AndroidDriver<AndroidElement> pDriver, int pThreadIndex)
        {
            foreach (var row in accountList)
            {
                if (row.Id == taskList[pThreadIndex].Account)
                {
                    while (true)
                    {
                        try
                        {
                            try
                            {
                                if (pDriver.FindElementById("com.nike.snkrs:id/buttonDefaultPositive").Enabled)
                                {
                                    pDriver.FindElementById("com.nike.snkrs:id/buttonDefaultPositive").Click();
                                    break;
                                }
                            }
                            catch (Exception e)
                            {
                                _Log.PrintTaskStatus(pThreadIndex, "Nike login modal is not seen: " + e.Message);
                            }

                            try
                            {
                                if (pDriver.FindElementById("android:id/button2").Enabled)
                                {
                                    pDriver.FindElementById("android:id/button2").Click();
                                }
                            }
                            catch (Exception e)
                            {
                                _Log.PrintTaskStatus(pThreadIndex, "Lack modal is not seen: " + e.Message);
                            }

                            try
                            {
                                if (pDriver.FindElementById("com.nike.snkrs:id/loginButton").Enabled)
                                {
                                    pDriver.FindElementById("com.nike.snkrs:id/loginButton").Click();
                                    break;
                                }
                            }
                            catch (Exception e)
                            {
                                _Log.PrintTaskStatus(pThreadIndex, "Login button is not seen yet: " + e.Message);
                            }
                        }
                        catch (Exception e)
                        {
                            _Log.PrintTaskStatus(pThreadIndex, "Retry to pass the nike app login splash: " + e.Message);
                            Thread.Sleep(1000);
                        }
                    }

                    while (true)
                    {
                        try
                        {
                            IList<AndroidElement> loginInputList = pDriver.FindElementsByClassName("android.widget.EditText");
                            loginInputList[0].SendKeys(row.Email);
                            loginInputList[1].SendKeys(row.Password);
                            Thread.Sleep(1000);
                            pDriver.FindElementByClassName("android.widget.Button").Click();
                            break;
                        }
                        catch (Exception e)
                        {
                            _Log.PrintTaskStatus(pThreadIndex, "Retry to login with nike account info: " + e.Message);
                            Thread.Sleep(1000);
                        }
                    }
                    _Log.PrintTaskStatus(pThreadIndex, "Successfully logined to the nike app");
                }
            }
        }

        private void EditPaymentSetting(AndroidDriver<AndroidElement> pDriver, int pThreadIndex)
        {
            while (true)
            {
                try
                {
                    pDriver.FindElementById("com.nike.snkrs:id/nav_profile").Click();
                    pDriver.FindElementById("com.nike.snkrs:id/action_settings").Click();
                    break;
                }
                catch (Exception e)
                {
                    _Log.PrintTaskStatus(pThreadIndex, "Retry finding the profile button: " + e.Message);
                    Thread.Sleep(1000);
                }
            }

            while (true)
            {
                try
                {
                    pDriver.Swipe(180, 590, 180, 100, 1000);
                    pDriver.FindElementByXPath("//android.widget.TextView[contains(@text,'Payment Information')]").Click();
                    break;
                }
                catch (Exception e)
                {
                    _Log.PrintTaskStatus(pThreadIndex, "Retry to swipe for finding payment information button: " + e.Message);
                    Thread.Sleep(1000);
                }
            }
        }

        private void AddPaymentInfo(AndroidDriver<AndroidElement> pDriver, int pThreadIndex)
        {
            try
            {
                foreach (var row in profileList)
                {
                    if (row.Id == taskList[pThreadIndex].Profile)
                    {
                        _Log.PrintTaskStatus(pThreadIndex, "Starting to edit payment setting");
                        EditPaymentSetting(pDriver, pThreadIndex);
                        _Log.PrintTaskStatus(pThreadIndex, "Starting to delete already registered payment info");
                        DeletePaymentInfo(pDriver, pThreadIndex);

                        while (true)
                        {
                            try
                            {
                                pDriver.FindElementByXPath("//android.widget.TextView[contains(@text,'Add Credit Card')]").Click();
                                break;
                            }
                            catch (Exception e)
                            {
                                _Log.PrintTaskStatus(pThreadIndex, "Retry to find the add credit card nav button: " + e.Message);
                                Thread.Sleep(1000);
                            }
                        }

                        IList<AndroidElement> inputList = pDriver.FindElementsByClassName("android.widget.EditText");
                        inputList[0].SendKeys(row.CardNumber);
                        string expirationDate = row.CardMonth + row.CardYear; //Regex.Replace(paymentInfo[1], "[^0-9]+", string.Empty);
                        KeyInput(pDriver, pThreadIndex, expirationDate);
                        KeyInput(pDriver, pThreadIndex, row.CardSecurity);
                        inputList[1].Clear();
                        inputList[1].SendKeys(row.BFirstName);
                        inputList[2].Clear();
                        inputList[2].SendKeys(row.BLastName);
                        inputList[3].Clear();
                        inputList[3].SendKeys(row.BAddress);
                        inputList[5].Clear();
                        inputList[5].SendKeys(row.BZip);
                        inputList[7].Clear();
                        inputList[7].SendKeys(row.BPhone);

                        while (true)
                        {
                            try
                            {
                                pDriver.Swipe(180, 590, 180, 100, 1000);
                                pDriver.FindElementByClassName("android.widget.Button").Click();
                                break;
                            }
                            catch (Exception e)
                            {
                                _Log.PrintTaskStatus(pThreadIndex, "Retry to find the add credit card button: " + e.Message);
                                Thread.Sleep(1000);
                            }
                        }

                        pDriver.FindElementByAccessibilityId("Navigate up").Click();
                        pDriver.FindElementByXPath("//android.widget.TextView[contains(@text,'Shipping Information')]").Click();
                        Thread.Sleep(1000);
                        _Log.PrintTaskStatus(pThreadIndex, "Starting to delete already registered shipping info");
                        DeleteShippingInfo(pDriver, pThreadIndex);
                        pDriver.FindElementByXPath("//android.widget.TextView[contains(@text,'Add Shipping Address')]").Click();
                        IList<AndroidElement> shippingInputList = pDriver.FindElementsByClassName("android.widget.EditText");
                        if (row.SFirstName != null)
                        {
                            shippingInputList[0].Clear();
                            shippingInputList[0].SendKeys(row.BFirstName);
                            shippingInputList[1].Clear();
                            shippingInputList[1].SendKeys(row.BLastName);
                            shippingInputList[2].Clear();
                            shippingInputList[2].SendKeys(row.BAddress);
                            shippingInputList[4].Clear();
                            shippingInputList[4].SendKeys(row.BZip);
                            shippingInputList[6].Clear();
                            shippingInputList[6].SendKeys(row.BPhone);
                        }
                        else
                        {
                            shippingInputList[0].Clear();
                            shippingInputList[0].SendKeys(row.SFirstName);
                            shippingInputList[1].Clear();
                            shippingInputList[1].SendKeys(row.SLastName);
                            shippingInputList[2].Clear();
                            shippingInputList[2].SendKeys(row.SAddress);
                            shippingInputList[4].Clear();
                            shippingInputList[4].SendKeys(row.SZip);
                            shippingInputList[6].Clear();
                            shippingInputList[6].SendKeys(row.SPhone);
                        }
                        pDriver.FindElementByClassName("android.widget.Button").Click();

                        pDriver.FindElementByAccessibilityId("Navigate up").Click();
                        pDriver.FindElementByAccessibilityId("Navigate up").Click();
                        _Log.PrintTaskStatus(pThreadIndex, "Successfully edition account's payment infomation");
                    }
                }
            }
            catch (Exception e)
            {
                _Log.PrintTaskStatus(pThreadIndex, "Edit the payment infomation exception: " + e.Message);
            }
        }

        private void DeletePaymentInfo(AndroidDriver<AndroidElement> pDriver, int pThreadIndex)
        {
            while (true)
            {
                try
                {
                    AndroidElement addGiftCardButton = pDriver.FindElementByXPath("//android.widget.TextView[contains(@text,'Add Credit Card')]");
                    IList<AndroidElement> currentCreditCardList = pDriver.FindElementsByXPath("//android.widget.TextView[contains(@text,'Delete')]");
                    if (currentCreditCardList.Count > 0)
                    {
                        for (int i = 0; i < currentCreditCardList.Count; i++)
                        {
                            while (true)
                            {
                                try
                                {
                                    if (currentCreditCardList[i].Enabled)
                                    {
                                        currentCreditCardList[i].Click();
                                        Thread.Sleep(1000);
                                        pDriver.FindElementById("com.nike.snkrs:id/buttonDefaultPositive").Click();
                                        break;
                                    }
                                }
                                catch (Exception e)
                                {
                                    _Log.PrintTaskStatus(pThreadIndex, "Retry to find the current registered credit card list: " + e.Message);
                                    Thread.Sleep(1000);
                                }
                            }
                        }
                    }
                    else
                    {
                        _Log.PrintTaskStatus(pThreadIndex, "There is no current registered credit card");
                        break;
                    }
                }
                catch (Exception e)
                {
                    _Log.PrintTaskStatus(pThreadIndex, "Retry to find the add gift card button for finding registered credit-card list: " + e.Message);
                    Thread.Sleep(1000);
                }
            }
        }

        private void DeleteShippingInfo(AndroidDriver<AndroidElement> pDriver, int pThreadIndex)
        {
            try
            {
                int textEditListCount = pDriver.FindElementsByClassName("android.widget.TextView").Count;
                if (textEditListCount > 2)
                {
                    for (int i = 1; i < textEditListCount - 1; i++)
                    {
                        IList<AndroidElement> textEditList = pDriver.FindElementsByClassName("android.widget.TextView");
                        textEditList[1].Click();
                        IList<AndroidElement> buttonList = pDriver.FindElementsByClassName("android.widget.Button");
                        buttonList[1].Click();
                        pDriver.FindElementById("com.nike.snkrs:id/buttonDefaultPositive").Click();
                        Thread.Sleep(1000);
                    }
                }
            }
            catch (Exception e)
            {
                _Log.PrintTaskStatus(pThreadIndex, "There is no current registered shipping info list: " + e.Message);
            }
        }

        private void NikeBuyProduct(AndroidDriver<AndroidElement> pDriver, int pThreadIndex)
        {
            try
            {
                string cvv = "", accountPassword = "";
                foreach (var profileRow in profileList)
                {
                    if (profileRow.Id == taskList[pThreadIndex].Profile)
                    {
                        cvv = profileRow.CardSecurity;
                    }
                }
                foreach (var accountRow in accountList)
                {
                    if (accountRow.Id == taskList[pThreadIndex].Account)
                    {
                        accountPassword = accountRow.Password;
                    }
                }

                pDriver.FindElementByXPath("//android.widget.TextView[contains(@text,'PURCHASES')]").Click();
                pDriver.FindElementById("com.nike.snkrs:id/nav_home").Click();
                //pDriver.FindElementByXPath("//android.widget.TextView[contains(@text,'IN STOCK')]").Click();
                //while (true)
                //{
                //    try
                //    {
                //        IList<AndroidElement> productList = pDriver.FindElementsById("com.nike.snkrs:id/item_discover_grid_cell_flat_relative_layout");
                //        productList[0].Click();
                //        break;
                //    }
                //    catch (Exception e)
                //    {
                //        _Log.PrintTaskStatus(pThreadIndex, "Retry to find the nike instock products list:" + e.Message);
                //    }
                //}

                pDriver.FindElementByXPath("//android.widget.TextView[contains(@text,'UPCOMING')]").Click();
                while (true)
                {
                    try
                    {
                        IList<AndroidElement> productList = pDriver.FindElementsById("com.nike.snkrs:id/item_upcoming_image");
                        productList[0].Click();
                        break;
                    }
                    catch (Exception e)
                    {
                        _Log.PrintTaskStatus(pThreadIndex, "Retry to find the nike upcomming products list:" + e.Message);
                    }
                }

                string buyButtonText = pDriver.FindElementById("com.nike.snkrs:id/view_cta_button").GetAttribute("text");
                if (buyButtonText == "NOTIFY ME")
                {
                    _Log.PrintTaskStatus(pThreadIndex, "This upcomming product is not ready to buy yet, must wait");
                    while (true)
                    {
                        if (!pDriver.FindElementById("com.nike.snkrs:id/view_cta_button").GetAttribute("text").Contains("NOTIFY ME"))
                        {
                            break;
                        }
                        else
                        {
                            _Log.PrintTaskStatus(pThreadIndex, "Retry to wait until to change the Notify me to price button");
                            Thread.Sleep(1000);
                        }
                    }
                }
                else
                {
                    _Log.PrintTaskStatus(pThreadIndex, "This upcomming product is ready to buy yet, Starting to buy now");
                }

                pDriver.FindElementById("com.nike.snkrs:id/view_cta_button").Click();
                IList<AndroidElement> sizeOptionList = pDriver.FindElementsById("com.nike.snkrs:id/shoeSizeTextView");
                for (int i = 0; i < sizeOptionList.Count; i++)
                {
                    sizeOptionList[i].Click();
                    pDriver.FindElementByXPath("//android.widget.Button[contains(@text,'OK')]").Click();
                    Thread.Sleep(1000);
                    try
                    {
                        if (pDriver.FindElementByXPath("//android.widget.Button[contains(@text,'OK')]").Enabled)
                        {
                            _Log.PrintTaskStatus(pThreadIndex, "This size is not able to select, try next size option");
                            continue;
                        }
                    }
                    catch (Exception e)
                    {
                        _Log.PrintTaskStatus(pThreadIndex, "This size is able to select: " + e.Message);
                        break;
                    }
                }

                while (true)
                {
                    try
                    {
                        pDriver.FindElementById("com.nike.snkrs:id/fragment_prereceipt_buy_button").Click();
                        try
                        {
                            if (pDriver.FindElementById("com.nike.snkrs:id/cvv_reentry_entered_cvv").Enabled)
                            {
                                pDriver.FindElementById("com.nike.snkrs:id/cvv_reentry_entered_cvv").SendKeys(cvv);
                                pDriver.FindElementById("com.nike.snkrs:id/buttonDefaultPositive").Click();
                                break;
                            }
                        }
                        catch (Exception e)
                        {
                            _Log.PrintTaskStatus(pThreadIndex, "CVV input modal is not seen: " + e.Message);
                            break;
                        }
                    }
                    catch (Exception e)
                    {
                        _Log.PrintTaskStatus(pThreadIndex, "Retry to find the buy now button: " + e.Message);
                        Thread.Sleep(1000);
                    }
                }

                while (true)
                {
                    try
                    {
                        pDriver.FindElementById("com.nike.snkrs:id/fragment_password_entry_password_edit_text").SendKeys(accountPassword);
                        pDriver.FindElementById("com.nike.snkrs:id/buttonDefaultPositive").Click();
                        break;
                    }
                    catch (Exception e)
                    {
                        _Log.PrintTaskStatus(pThreadIndex, "Retry to find the password input: " + e.Message);
                        Thread.Sleep(1000);
                    }
                }
            }
            catch (Exception e)
            {
                _Log.PrintTaskStatus(pThreadIndex, "Nike product buying exception: " + e.Message);
            }
        }

        private void NikeBot(AndroidDriver<AndroidElement> pDriver, int pThreadIndex)
        {
            if (pDriver.IsAppInstalled("com.nike.snkrs"))
            {
                _Log.PrintTaskStatus(pThreadIndex, "Uninstalling the nike app");
                pDriver.RemoveApp("com.nike.snkrs");
            }
            _Log.PrintTaskStatus(pThreadIndex, "Installing the nike app");
            pDriver.InstallApp(App.AndroidApp());

            _Log.PrintTaskStatus(pThreadIndex, "Launch the nike app");
            //pDriver.LaunchApp();
            pDriver.StartActivity("com.nike.snkrs", "com.nike.snkrs.main.activities.SnkrsActivity");

            _Log.PrintTaskStatus(pThreadIndex, "Starting to login the nike app");
            NikeLogin(pDriver, pThreadIndex);

            _Log.PrintTaskStatus(pThreadIndex, "Starting to add the payment infos");
            AddPaymentInfo(pDriver, pThreadIndex);

            _Log.PrintTaskStatus(pThreadIndex, "Starting to buy the nike product");
            NikeBuyProduct(pDriver, pThreadIndex);
        }

        public void NikeWorker(int threadIndex)
        {
            string noxName;
            AndroidDriver<AndroidElement> driver = null;

            if (threadIndex == 0)
            {
                noxName = "NoxPlayer";
            }
            else
            {
                noxName = "NoxPlayer(" + threadIndex + ")";
            }

            if (!String.IsNullOrEmpty(noxExePath))
            {
                if (noxExePath.Contains("Bluestacks.exe"))
                {
                    ////string noxConsolePath = noxExePath.Replace("Bluestacks.exe", "HD-MultiInstanceManager.exe");
                    string noxConsolePath = noxExePath;
                    _EmulatorUtils = new EmulatorUtils(noxConsolePath);
                    _EmulatorUtils.PlayerName = noxName;
                    emulatorList.Add(_EmulatorUtils);
                    _Log.PrintTaskStatus(threadIndex, noxName + " starting");
                    _EmulatorUtils.ModifySetting();
                    Thread.Sleep(2000);
                    _EmulatorUtils.Launch();
                    Thread.Sleep(30000);
                }
            }
            _EnvironmentSetting.SetEnviornment();
            _LocalServer.AppiumServerRun(threadIndex);

            while (driver == null)
            {
                _Log.PrintTaskStatus(threadIndex, noxName + " driver creation attempting");
                driver = BeforeAll(threadIndex, "Drony");
            }

            //driver.InstallApp(App.AndroidDronyApp());
            //driver.StartActivity("org.sandroproxy.drony", "org.sandroproxy.drony.DronyMainActivity");
            SetProxy(driver, 0);
            _Log.PrintTaskStatus(threadIndex, "Starting nike app bot");
            NikeBot(driver, threadIndex);
        }

        public void ThreadPool()
        {
            if (selectedTaskIndex.Count > 0)
            {
                isTaskRun = true;
                for (int i = 0; i < selectedTaskIndex.Count; i++)
                {
                    var j = i;
                    Thread nikeWorker = new Thread(() => NikeWorker(selectedTaskIndex[j]));
                    nikeWorkerList.Add(nikeWorker);
                }
                for (int j = 0; j < nikeWorkerList.Count; j++)
                {
                    nikeWorkerList[j].Start();
                    Thread.Sleep(sessionDelay * 1000);
                }
            }
            else
            {
                MessageBox.Show("There are not selected tasks yet");
            }
        }
    }
}