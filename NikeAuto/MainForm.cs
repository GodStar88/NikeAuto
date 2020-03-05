using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using NikeAuto.Engine;
using NikeAuto.Define;
using System.Data;
using System.Windows.Forms;

namespace NikeAuto
{
    public partial class MainForm : Form
    {
        public NikeCore _NikeCore;
        public UpcomingProduct _UpcomingProduct;

        private string settingFilePath = Program.basePath + @"\config.ini";
        private StringBuilder settingBuilder = new StringBuilder(100);
        [DllImport("kernel32")]
        public static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32")]
        public static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        private List<bool> isMenuClicked = Enumerable.Repeat(false, 6).ToList();
        private bool isDragging = false;
        private string cardYear, cardMonth;

        private Point startPoint = new Point(0, 0);
        private CheckBox TaskGridHeaderCheckbox = null;
        private static bool IsUpdateProfile = false;
        private static int ProfileId = 0;

        public MainForm()
        {
            InitializeComponent();
            _NikeCore = new NikeCore(this);
            _UpcomingProduct = new UpcomingProduct(this);
            LoadIniFile();
        }

        public void LoadIniFile()
        {
            if (File.Exists(settingFilePath))
            {
                //GetPrivateProfileString("Settings", "RootPath", "", settingBuilder, 1000, settingFilePath);
                //Program.basePath = settingBuilder.ToString();
                GetPrivateProfileString("Settings", "SessionDelay", "", settingBuilder, 100, settingFilePath);
                NikeCore.sessionDelay = Convert.ToInt32(settingBuilder.ToString());
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            isMenuClicked[0] = true;
            TasksMenu.ForeColor = Color.White;
            TasksContainer.Dock = DockStyle.Fill;
            AddTaskgridHeaderCheckbox();
            TaskGridHeaderCheckbox.MouseClick += new MouseEventHandler(TaskGridHeaderCheckbox_MouseClick);
        }

        public void ShowProxyAmount(string proxyAmount)
        {
            ProxyAmountLabel.Text = proxyAmount;
        }

        public void ShowAccountAmount(string accountAmount)
        {
            AccountAmountLabel.Text = accountAmount;
        }

        private void StartAllButton_Click(object sender, EventArgs e)
        {
            _NikeCore.selectedTaskIndex.Clear();
            for (int i = 0; i < TaskGridView.RowCount; i++)
            {
                try
                {
                    if (!String.IsNullOrEmpty(TaskGridView.Rows[i].Cells["taskId"].Value.ToString()))
                    {
                        _NikeCore.selectedTaskIndex.Add(i);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("There are not tasks that it is possible to run yet: " + ex.Message);
                }
            }
            if (!_NikeCore.isTaskRun)
            {
                _NikeCore.workerThreadPool = new Thread(new ThreadStart(_NikeCore.ThreadPool));
                _NikeCore.workerThreadPool.Start();
            }
            else
            {
                MessageBox.Show("There are already running all tasks");
            }
        }

        private void StopAllButton_Click(object sender, EventArgs e)
        {
            _NikeCore.StopTask();
        }

        private void DeleteAllButton_Click(object sender, EventArgs e)
        {
            if (!_NikeCore.isTaskRun)
            {
                _NikeCore.DeleteAllTask();
            }
            else
            {
                MessageBox.Show("It is impossible to delete all tasks because of running them");
            }
        }

        private void AddTaskgridHeaderCheckbox()
        {
            TaskGridHeaderCheckbox = new CheckBox();
            TaskGridHeaderCheckbox.Size = new Size(15, 15);
            this.TaskGridView.Controls.Add(TaskGridHeaderCheckbox);
        }

        private void TaskGridHeaderCheckboxClick(CheckBox pCheckbox)
        {
            foreach (DataGridViewRow row in TaskGridView.Rows)
            {
                ((DataGridViewCheckBoxCell)row.Cells["checkbox"]).Value = pCheckbox.Checked;
            }
            TaskGridView.RefreshEdit();
        }

        private void TaskGridHeaderCheckbox_MouseClick(object sender, MouseEventArgs e)
        {
            TaskGridHeaderCheckboxClick((CheckBox)sender);
        }

        private void TaskGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                DataGridViewCheckBoxCell chk = (DataGridViewCheckBoxCell)TaskGridView.Rows[e.RowIndex].Cells["checkbox"];
                if (chk.Value == chk.TrueValue)
                {
                    chk.Value = chk.FalseValue;
                }
                else
                {
                    chk.Value = chk.TrueValue;
                }
            }
        }

        private void StartSelectedButton_Click(object sender, EventArgs e)
        {
            _NikeCore.selectedTaskIndex.Clear();
            for (int i = 0; i < TaskGridView.RowCount; i++)
            {
                try
                {
                    if (!String.IsNullOrEmpty(TaskGridView.Rows[i].Cells["taskId"].Value.ToString()))
                    {
                        if (TaskGridView.Rows[i].Cells["checkbox"].Value.ToString() == "true")
                        {
                            _NikeCore.selectedTaskIndex.Add(i);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("There are not tasks that it is possible to run yet: " + ex.Message);
                }
            }
            if (!_NikeCore.isTaskRun)
            {
                _NikeCore.workerThreadPool = new Thread(new ThreadStart(_NikeCore.ThreadPool));
                _NikeCore.workerThreadPool.Start();
            }
            else
            {
                MessageBox.Show("There are already runnnign selected task");
            }
        }

        private void StopSelectedButton_Click(object sender, EventArgs e)
        {
            _NikeCore.StopTask();
        }

        private void DeleteSelectedButton_Click(object sender, EventArgs e)
        {
            _NikeCore.selectedTaskId.Clear();
            _NikeCore.selectedTaskIndex.Clear();
            for (int i = 0; i < TaskGridView.RowCount; i++)
            {
                if (Convert.ToBoolean(TaskGridView.Rows[i].Cells["checkbox"].Value) == true)
                {
                    _NikeCore.selectedTaskId.Add(Convert.ToInt32(TaskGridView.Rows[i].Cells["taskId"].Value));
                    _NikeCore.selectedTaskIndex.Add(i);
                }
            }
            if (!_NikeCore.isTaskRun)
            {
                _NikeCore.DeleteSelectedTask();
            }
            else
            {
                MessageBox.Show("It is impossible to delete the selected task becasue of running them");
            }
        }

        private void UpcomingDateTimePicker_ValueChanged(object sender, EventArgs e)
        {
            SKUComboBox.Items.Clear(); // Added by Vidit on 14th Feb 2020
            string upcomingDate = UpcomingDateTimePicker.Value.ToString().Split(' ')[0];
            _NikeCore.LoadUpcomingProduct(upcomingDate);
        }

        private void CreateTaskButton_Click(object sender, EventArgs e)
        {
            _NikeCore.CreateTask();
        }

        private void UpdateUpcomingProductButton_Click(object sender, EventArgs e)
        {
            _NikeCore.FetchUpcommingProduct();
        }

        private void AddProfileButton_Click(object sender, EventArgs e)
        {
            TasksContainer.Visible = false;
            CreateTasksContainer.Visible = false;
            ProxiesContainer.Visible = false;
            AccountsContainer.Visible = false;
            SettingsContainer.Visible = false;
            ProfilesContainer.Visible = false;
            CreateProfileContainer.Visible = true;
            CreateProfileContainer.Dock = DockStyle.Fill;
            ProfileId = 0;
        }

        private void DeleteAllProfileButton_Click(object sender, EventArgs e)
        {
            var confirmResult = MessageBox.Show("Are you sure you want to delete all the records??", "Confirm Delete!!", MessageBoxButtons.YesNo);
            if (confirmResult == DialogResult.Yes)
            {
                _NikeCore.DeleteAllProfiles();
            }
        }

        private void ImportProfileButton_Click(object sender, EventArgs e)
        {
            _NikeCore.ImportProfileInfo();
        }

        private void ExportProfileButton_Click(object sender, EventArgs e)
        {
            DataTable dt = _NikeCore.ExportProfile();

            try
            {
                string FileName = "";
                FileName = "Profile";

                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "Excel Documents (*.xls)|*.xls";
                sfd.FileName = FileName + ".xls";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    ToCsV(dt, sfd.FileName);
                }

                ////////string filename = OpenSavefileDialog();
                ////////ToCSVNew(dt,filename);
            }
            catch (Exception ex)
            {
                //////wl.LogWrite(ex.Message);
                //////MessageBox.Show("An Error has occured. Kindly check Logs.");
            }
        }

        private void ToCSVNew(DataTable dtDataTable, string strFilePath)
        {
            StreamWriter sw = new StreamWriter(strFilePath, false);
            //headers  
            for (int i = 0; i < dtDataTable.Columns.Count; i++)
            {
                sw.Write(dtDataTable.Columns[i]);
                if (i < dtDataTable.Columns.Count - 1)
                {
                    sw.Write(",");
                }
            }
            sw.Write(sw.NewLine);
            foreach (DataRow dr in dtDataTable.Rows)
            {
                for (int i = 0; i < dtDataTable.Columns.Count; i++)
                {
                    if (!Convert.IsDBNull(dr[i]))
                    {
                        string value = dr[i].ToString();
                        if (value.Contains(','))
                        {
                            value = String.Format("\"{0}\"", value);
                            sw.Write(value);
                        }
                        else
                        {
                            sw.Write(dr[i].ToString());
                        }
                    }
                    if (i < dtDataTable.Columns.Count - 1)
                    {
                        sw.Write(",");
                    }
                }
                sw.Write(sw.NewLine);
            }
            sw.Close();
        }

        private void ToCsV(DataTable dGV, string filename)
        {
            try
            {
                string stOutput = "";
                // Export titles:
                string sHeaders = "";

                for (int j = 0; j < dGV.Columns.Count; j++)
                    sHeaders = sHeaders.ToString() + Convert.ToString(dGV.Columns[j].ColumnName) + "\t";
                stOutput += sHeaders + "\r\n";
                // Export data.
                for (int i = 0; i < dGV.Rows.Count; i++)
                {
                    //////string stLine = "";
                    //////for (int j = 0; j < dGV.Rows[i].Count; j++)
                    //////    stLine = stLine.ToString() + Convert.ToString(dGV.Rows[i].Cells[j].Value) + "\t";
                    //////stOutput += stLine + "\r\n";

                    foreach (DataRow dr in dGV.Rows)
                    {
                        string stLine = "";
                        for (int j = 0; j < dGV.Columns.Count; j++)
                        {
                            if (!Convert.IsDBNull(dr[j]))
                            {
                                string value = dr[j].ToString();
                                stLine = stLine.ToString() + Convert.ToString(value) + "\t";
                                stOutput += stLine + "\r\n";
                            }
                        }
                    }
                }
                Encoding utf16 = Encoding.GetEncoding(1254);
                byte[] output = utf16.GetBytes(stOutput);
                FileStream fs = new FileStream(filename, FileMode.Create);
                BinaryWriter bw = new BinaryWriter(fs);
                bw.Write(output, 0, output.Length); //write the encoded file
                bw.Flush();
                bw.Close();
                fs.Close();
            }
            catch (Exception ex)
            {
                //////wl.LogWrite(ex.Message);
                //////MessageBox.Show("An Error has occured. Kindly check Logs.");
            }
        }

        private void SameCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (SameCheckbox.Checked)
            {
                _NikeCore.isSameShipping = true;

                string FirstName = SFirstnameInput.Text.ToString();
                string LastName = SLastnameInput.Text.ToString();
                string Address = SAddressInput.Text.ToString();
                string APTInput = SAPTInput.Text.ToString();

                string ZipCode = SZIPInput.Text.ToString();
                string City = SCityInput.Text.ToString();
                string State = SStateInput.Text.ToString();
                string Phone = SPhoneInput.Text.ToString();

                BFirstnameInput.Text = FirstName;
                BLastnameInput.Text = LastName;
                BAddressInput.Text = Address;
                BAPTInput.Text = APTInput;

                BZIPInput.Text = ZipCode;
                BCityInput.Text = City;
                BStateInput.Text = State;
                BPhoneInput.Text = Phone;
            }
            else
            {
                _NikeCore.isSameShipping = false;

                BFirstnameInput.Text = "";
                BLastnameInput.Text = "";
                BAddressInput.Text = "";
                BAPTInput.Text = "";

                BZIPInput.Text = "";
                BCityInput.Text = "";
                BStateInput.Text = "";
                BPhoneInput.Text = "";
            }
        }

        private void ClearControls(string Section)
        {
            if (Section == "Profile")
            {
                BFirstnameInput.Text = "";
                BLastnameInput.Text = "";
                BAddressInput.Text = "";
                BAPTInput.Text = "";

                BZIPInput.Text = "";
                BCityInput.Text = "";
                BStateInput.Text = "";
                BPhoneInput.Text = "";

                SFirstnameInput.Text = "";
                SLastnameInput.Text = "";
                SAddressInput.Text = "";
                SAPTInput.Text = "";

                SZIPInput.Text = "";
                SCityInput.Text = "";
                SStateInput.Text = "";
                SPhoneInput.Text = "";

                CreditCardNumberInput.Text = "";
                CVVInput.Text = "";
                CreditCardYearSelect.SelectedIndex = 0;
                CreditCardMonthSelect.SelectedIndex = 0;

                IsUpdateProfile = false;
                ProfileId = 0;
            }
        }

        private void CreditCardMonthSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CreditCardMonthSelect.SelectedItem.ToString() != "")
            {
                if (CreditCardMonthSelect.SelectedItem.ToString().Length == 1)
                {
                    cardMonth = "0" + CreditCardMonthSelect.SelectedItem.ToString();
                }
                else
                {
                    cardMonth = CreditCardMonthSelect.SelectedItem.ToString();
                }
            }
        }

        private void CreditCardYearSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CreditCardYearSelect.SelectedItem.ToString() != "")
            {
                cardYear = CreditCardYearSelect.SelectedItem.ToString().Substring(2, 2);
            }
        }

        private void SaveProfileButton_Click(object sender, EventArgs e)
        {
            Profile profile = new Profile();
            if (BFirstnameInput.Text != "")
            {
                profile.BFirstName = BFirstnameInput.Text;
                BFirstnameInput.Clear();
                BFirstnameInput.WaterMarkForeColor = Color.Gray;
                BFirstnameInput.WaterMark = "FirstName";
            }
            else
            {
                BFirstnameInput.WaterMarkForeColor = Color.Red;
                BFirstnameInput.WaterMark = "Please enter FirstName";
            }
            if (BLastnameInput.Text != "")
            {
                profile.BLastName = BLastnameInput.Text;
                BLastnameInput.Clear();
                BLastnameInput.WaterMarkForeColor = Color.Gray;
                BLastnameInput.WaterMark = "LastName";
            }
            else
            {
                BLastnameInput.WaterMarkForeColor = Color.Red;
                BLastnameInput.WaterMark = "Please enter LastName";
            }
            if (BAddressInput.Text != "")
            {
                profile.BAddress = BAddressInput.Text;
                BAddressInput.Clear();
                BAddressInput.WaterMarkForeColor = Color.Gray;
                BAddressInput.WaterMark = "Address";
            }
            else
            {
                BAddressInput.WaterMarkForeColor = Color.Red;
                BAddressInput.WaterMark = "Please enter address";
            }
            if (BAPTInput.Text != "")
            {
                profile.BAPT = BAPTInput.Text;
                BAPTInput.Clear();
            }
            if (BZIPInput.Text != "")
            {
                profile.BZip = BZIPInput.Text;
                BZIPInput.Clear();
                BZIPInput.WaterMarkForeColor = Color.Gray;
                BZIPInput.WaterMark = "ZIP";
            }
            else
            {
                BZIPInput.WaterMarkForeColor = Color.Red;
                BZIPInput.WaterMark = "Please enter ZIP code";
            }
            if (BCityInput.Text != "")
            {
                profile.BCity = BCityInput.Text;
                BCityInput.Clear();
            }
            if (BStateInput.Text != "")
            {
                profile.BState = BStateInput.Text;
                BStateInput.Clear();
            }
            if (BPhoneInput.Text != "")
            {
                profile.BPhone = BPhoneInput.Text;
                BPhoneInput.Clear();
                BPhoneInput.WaterMarkForeColor = Color.Gray;
                BPhoneInput.WaterMark = "Phone";
            }
            else
            {
                BPhoneInput.WaterMarkForeColor = Color.Red;
                BPhoneInput.WaterMark = "Please enter phone number";
            }
            if (CreditCardNumberInput.Text != "")
            {
                profile.CardNumber = CreditCardNumberInput.Text;
                CreditCardNumberInput.Clear();
                CreditCardNumberInput.WaterMarkForeColor = Color.Gray;
                CreditCardNumberInput.WaterMark = "Card Number";
            }
            else
            {
                CreditCardNumberInput.WaterMarkForeColor = Color.Red;
                CreditCardNumberInput.WaterMark = "Please enter credit card number";
            }
            if (cardYear != null)
            {
                profile.CardYear = cardYear;
                cardYear = null;
                CreditCardYearSelect.ForeColor = Color.Gray;
                CreditCardYearSelect.Text = "Year";
            }
            else
            {
                CreditCardYearSelect.ForeColor = Color.Red;
                CreditCardYearSelect.Text = "Please select card year";
            }
            if (cardMonth != null)
            {
                profile.CardMonth = cardMonth;
                cardMonth = null;
                CreditCardMonthSelect.ForeColor = Color.Gray;
                CreditCardMonthSelect.Text = "Month";
            }
            else
            {
                CreditCardMonthSelect.ForeColor = Color.Red;
                CreditCardMonthSelect.Text = "Please select card month";
            }
            if (CVVInput.Text != "")
            {
                profile.CardSecurity = CVVInput.Text;
                CVVInput.Clear();
                CVVInput.WaterMarkForeColor = Color.Gray;
                CVVInput.WaterMark = "CVV";
            }
            else
            {
                CVVInput.WaterMarkForeColor = Color.Red;
                CVVInput.WaterMark = "Please enter CVV";
            }
            if (!_NikeCore.isSameShipping)
            {
                if (SFirstnameInput.Text != "")
                {
                    profile.SFirstName = SFirstnameInput.Text;
                    SFirstnameInput.Clear();
                    SFirstnameInput.WaterMarkForeColor = Color.Gray;
                    SFirstnameInput.WaterMark = "FirstName";
                }
                else
                {
                    SFirstnameInput.WaterMarkForeColor = Color.Red;
                    SFirstnameInput.WaterMark = "Please enter FirstName";
                }
                if (SLastnameInput.Text != "")
                {
                    profile.SLastName = SLastnameInput.Text;
                    SLastnameInput.Clear();
                    SLastnameInput.WaterMarkForeColor = Color.Gray;
                    SLastnameInput.WaterMark = "LastName";
                }
                else
                {
                    SLastnameInput.WaterMarkForeColor = Color.Red;
                    SLastnameInput.WaterMark = "Please enter LastName";
                }
                if (SAddressInput.Text != "")
                {
                    profile.SAddress = SAddressInput.Text;
                    SAddressInput.Clear();
                    SAddressInput.WaterMarkForeColor = Color.Gray;
                    SAddressInput.WaterMark = "Address";
                }
                else
                {
                    SAddressInput.WaterMarkForeColor = Color.Red;
                    SAddressInput.WaterMark = "Please enter address";
                }
                if (SAPTInput.Text != "")
                {
                    profile.SAPT = SAPTInput.Text;
                    SAPTInput.Clear();
                }
                if (SZIPInput.Text != "")
                {
                    profile.SZip = SZIPInput.Text;
                    SZIPInput.Clear();
                    SZIPInput.WaterMarkForeColor = Color.Gray;
                    SZIPInput.WaterMark = "ZIP";
                }
                else
                {
                    SZIPInput.WaterMarkForeColor = Color.Red;
                    SZIPInput.WaterMark = "Please enter ZIP code";
                }
                if (SCityInput.Text != "")
                {
                    profile.SCity = SCityInput.Text;
                    SCityInput.Clear();
                }
                if (SStateInput.Text != "")
                {
                    profile.SState = SStateInput.Text;
                    SStateInput.Clear();
                }
                if (SPhoneInput.Text != "")
                {
                    profile.SPhone = SPhoneInput.Text;
                    SPhoneInput.Clear();
                    SPhoneInput.WaterMarkForeColor = Color.Gray;
                    SPhoneInput.WaterMark = "Phone";
                }
                else
                {
                    SPhoneInput.WaterMarkForeColor = Color.Red;
                    SPhoneInput.WaterMark = "Please enter phone number";
                }
                if (profile.BFirstName != null && profile.BLastName != null && profile.BAddress != null && profile.BZip != null && profile.BPhone != null && profile.CardNumber != null && profile.CardYear != null && profile.CardMonth != null && profile.CardSecurity != null && profile.SFirstName != null && profile.SLastName != null && profile.SAddress != null && profile.SZip != null && profile.SPhone != null)
                {
                    if (IsUpdateProfile == true)
                    {
                        profile.Id = ProfileId;
                        _NikeCore.UpdateProfile(profile);
                        ClearControls("Profile");
                        MessageBox.Show("Successfully Updated!");
                    }
                    else
                    {
                        _NikeCore.CreateProfile(profile);
                        ClearControls("Profile");
                        MessageBox.Show("Successfully Created!");
                    }
                }
            }
            else
            {
                SFirstnameInput.WaterMarkForeColor = Color.Gray;
                SFirstnameInput.WaterMark = "FirstName";
                SLastnameInput.WaterMarkForeColor = Color.Gray;
                SLastnameInput.WaterMark = "LastName";
                SAddressInput.WaterMarkForeColor = Color.Gray;
                SAddressInput.WaterMark = "Address";
                SZIPInput.WaterMarkForeColor = Color.Gray;
                SZIPInput.WaterMark = "ZIP";
                SPhoneInput.WaterMarkForeColor = Color.Gray;
                SPhoneInput.WaterMark = "Phone";
                profile.SFirstName = profile.BFirstName;
                profile.SLastName = profile.BLastName;
                profile.SAddress = profile.BAddress;
                profile.SAPT = profile.BAPT;
                profile.SZip = profile.BZip;
                profile.SCity = profile.BCity;
                profile.SState = profile.BState;
                profile.SPhone = profile.BPhone;

                ////////////profile.BFirstName = profile.SFirstName;
                ////////////profile.BLastName = profile.SLastName;
                ////////////profile.BAddress = profile.SAddress;
                ////////////profile.BAPT = profile.SAPT;
                ////////////profile.BZip = profile.SZip;
                ////////////profile.BCity = profile.SCity;
                ////////////profile.BState = profile.SState;
                ////////////profile.BPhone = profile.SPhone;

                ////////if (profile.BFirstName != null && profile.BLastName != null && profile.BAddress != null && profile.BZip != null && profile.BPhone != null && profile.CardNumber != null && profile.CardYear != null && profile.CardMonth != null && profile.CardSecurity != null)
                if (profile.SFirstName != null && profile.SLastName != null && profile.SAddress != null && profile.SZip != null && profile.SPhone != null && profile.CardNumber != null && profile.CardYear != null && profile.CardMonth != null && profile.CardSecurity != null)
                {
                    if (IsUpdateProfile == true)
                    {
                        profile.Id = ProfileId;
                        _NikeCore.UpdateProfile(profile);
                        ClearControls("Profile");
                        MessageBox.Show("Successfully Updated!");
                    }
                    else
                    {
                        _NikeCore.CreateProfile(profile);
                        ClearControls("Profile");
                        MessageBox.Show("Successfully Created!");
                    }
                }
            }
        }

        private void ProxySaveButton_Click(object sender, EventArgs e)
        {
            int currentProxyCount = Convert.ToInt32(ProxyAmountLabel.Text);
            for (int i = currentProxyCount; i < ProxyTextBox.Lines.Length; i++)
            {
                if (ProxyTextBox.Lines[i] != "")
                {
                    Proxy proxy = new Proxy();
                    string[] proxyInfoArray = ProxyTextBox.Lines[i].Split(':');
                    proxy.Ip = proxyInfoArray[0];
                    proxy.Port = proxyInfoArray[1];
                    proxy.UserId = proxyInfoArray[2];
                    proxy.Password = proxyInfoArray[3];
                    _NikeCore.SaveProxy(proxy);
                }
            }
            ProxyTextBox.Refresh();
            ProxyAmountLabel.Text = ProxyTextBox.Lines.Length.ToString();
        }

        private void ProxyClearAllButton_Click(object sender, EventArgs e)
        {
            _NikeCore.DeleteAllProxy();
        }

        private void AccountSaveButton_Click(object sender, EventArgs e)
        {
            int currentAccountCount = Convert.ToInt32(AccountAmountLabel.Text);
            for (int i = currentAccountCount; i < AccountTextBox.Lines.Length; i++)
            {
                if (AccountTextBox.Lines[i] != "")
                {
                    Account account = new Account();
                    string[] accountInfoArray = AccountTextBox.Lines[i].Split(':');
                    account.Email = accountInfoArray[0];
                    account.Password = accountInfoArray[1];
                    _NikeCore.SaveAccount(account);
                }
            }
            AccountTextBox.Refresh();
            AccountAmountLabel.Text = AccountTextBox.Lines.Length.ToString();
        }

        private void AccountClearButton_Click(object sender, EventArgs e)
        {
            _NikeCore.DeleteAllAccount();
        }

        private void HeaderPanel_MouseDown(object sender, MouseEventArgs e)
        {
            isDragging = true;
            startPoint = new Point(e.X, e.Y);
        }

        private void HeaderPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                Point p = PointToScreen(e.Location);
                Location = new Point(p.X - startPoint.X, p.Y - startPoint.Y);
            }
        }

        private void HeaderPanel_MouseUp(object sender, MouseEventArgs e)
        {
            isDragging = false;
        }

        private void TasksMenu_Click(object sender, EventArgs e)
        {
            isMenuClicked = Enumerable.Repeat(false, 6).ToList();
            isMenuClicked[0] = true;
            if (isMenuClicked[0])
            {
                TasksMenu.ForeColor = Color.White;
                CreateTasksMenu.ForeColor = Color.FromArgb(42, 90, 157);
                ProfilesMenu.ForeColor = Color.FromArgb(42, 90, 157);
                ProxiesMenu.ForeColor = Color.FromArgb(42, 90, 157);
                AccountsMenu.ForeColor = Color.FromArgb(42, 90, 157);
                SettingsMenu.ForeColor = Color.FromArgb(42, 90, 157);
            }
            TasksContainer.Visible = true;
            TasksContainer.Dock = DockStyle.Fill;
            CreateTasksContainer.Visible = false;
            ProfilesContainer.Visible = false;
            ProxiesContainer.Visible = false;
            AccountsContainer.Visible = false;
            SettingsContainer.Visible = false;
            CreateProfileContainer.Visible = false;
        }

        private void TasksMenu_MouseHover(object sender, EventArgs e)
        {
            TasksMenu.ForeColor = Color.White;
        }

        private void TasksMenu_MouseLeave(object sender, EventArgs e)
        {
            if (!isMenuClicked[0])
            {
                TasksMenu.ForeColor = Color.FromArgb(42, 90, 157);
            }
        }

        private void CreateTasksMenu_Click(object sender, EventArgs e)
        {
            isMenuClicked = Enumerable.Repeat(false, 6).ToList();
            isMenuClicked[1] = true;
            if (isMenuClicked[1])
            {
                TasksMenu.ForeColor = Color.FromArgb(42, 90, 157);
                CreateTasksMenu.ForeColor = Color.White;
                ProfilesMenu.ForeColor = Color.FromArgb(42, 90, 157);
                ProxiesMenu.ForeColor = Color.FromArgb(42, 90, 157);
                AccountsMenu.ForeColor = Color.FromArgb(42, 90, 157);
                SettingsMenu.ForeColor = Color.FromArgb(42, 90, 157);
            }
            CreateTasksContainer.Visible = true;
            CreateTasksContainer.Dock = DockStyle.Fill;
            TasksContainer.Visible = false;
            ProfilesContainer.Visible = false;
            ProxiesContainer.Visible = false;
            AccountsContainer.Visible = false;
            SettingsContainer.Visible = false;
            CreateProfileContainer.Visible = false;
        }

        private void CreateTasksMenu_MouseHover(object sender, EventArgs e)
        {
            CreateTasksMenu.ForeColor = Color.White;
        }

        private void CreateTasksMenu_MouseLeave(object sender, EventArgs e)
        {
            if (!isMenuClicked[1])
            {
                CreateTasksMenu.ForeColor = Color.FromArgb(42, 90, 157);
            }
        }

        private void ProfilesMenu_Click(object sender, EventArgs e)
        {
            isMenuClicked = Enumerable.Repeat(false, 6).ToList();
            isMenuClicked[2] = true;
            if (isMenuClicked[2])
            {
                ProfilesMenu.ForeColor = Color.White;
                TasksMenu.ForeColor = Color.FromArgb(42, 90, 157);
                CreateTasksMenu.ForeColor = Color.FromArgb(42, 90, 157);
                ProxiesMenu.ForeColor = Color.FromArgb(42, 90, 157);
                AccountsMenu.ForeColor = Color.FromArgb(42, 90, 157);
                SettingsMenu.ForeColor = Color.FromArgb(42, 90, 157);
            }
            ProfilesContainer.Visible = true;
            ProfilesContainer.Dock = DockStyle.Fill;
            CreateTasksContainer.Visible = false;
            TasksContainer.Visible = false;
            ProxiesContainer.Visible = false;
            AccountsContainer.Visible = false;
            SettingsContainer.Visible = false;
            CreateProfileContainer.Visible = false;
        }

        private void ProfilesMenu_MouseHover(object sender, EventArgs e)
        {
            ProfilesMenu.ForeColor = Color.White;
        }

        private void ProfilesMenu_MouseLeave(object sender, EventArgs e)
        {
            if (!isMenuClicked[2])
            {
                ProfilesMenu.ForeColor = Color.FromArgb(42, 90, 157);
            }
        }

        private void ProxiesMenu_Click(object sender, EventArgs e)
        {
            isMenuClicked = Enumerable.Repeat(false, 6).ToList();
            isMenuClicked[3] = true;
            if (isMenuClicked[3])
            {
                ProxiesMenu.ForeColor = Color.White;
                TasksMenu.ForeColor = Color.FromArgb(42, 90, 157);
                CreateTasksMenu.ForeColor = Color.FromArgb(42, 90, 157);
                ProfilesMenu.ForeColor = Color.FromArgb(42, 90, 157);
                AccountsMenu.ForeColor = Color.FromArgb(42, 90, 157);
                SettingsMenu.ForeColor = Color.FromArgb(42, 90, 157);
            }
            ProxiesContainer.Visible = true;
            ProxiesMenu.ForeColor = Color.White;
            ProxiesContainer.Dock = DockStyle.Fill;
            TasksContainer.Visible = false;
            CreateTasksContainer.Visible = false;
            ProfilesContainer.Visible = false;
            AccountsContainer.Visible = false;
            SettingsContainer.Visible = false;
            CreateProfileContainer.Visible = false;
        }

        private void ProxiesMenu_MouseHover(object sender, EventArgs e)
        {
            ProxiesMenu.ForeColor = Color.White;
        }

        private void ProxiesMenu_MouseLeave(object sender, EventArgs e)
        {
            if (!isMenuClicked[3])
            {
                ProxiesMenu.ForeColor = Color.FromArgb(42, 90, 157);
            }
        }

        private void AccountsMenu_Click(object sender, EventArgs e)
        {
            isMenuClicked = Enumerable.Repeat(false, 6).ToList();
            isMenuClicked[4] = true;
            if (isMenuClicked[4])
            {
                AccountsMenu.ForeColor = Color.White;
                TasksMenu.ForeColor = Color.FromArgb(42, 90, 157);
                CreateTasksMenu.ForeColor = Color.FromArgb(42, 90, 157);
                ProfilesMenu.ForeColor = Color.FromArgb(42, 90, 157);
                ProxiesMenu.ForeColor = Color.FromArgb(42, 90, 157);
                SettingsMenu.ForeColor = Color.FromArgb(42, 90, 157);
            }
            AccountsContainer.Visible = true;
            AccountsContainer.Dock = DockStyle.Fill;
            TasksContainer.Visible = false;
            CreateTasksContainer.Visible = false;
            ProfilesContainer.Visible = false;
            ProxiesContainer.Visible = false;
            SettingsContainer.Visible = false;
            CreateProfileContainer.Visible = false;
        }

        private void AccountsMenu_MouseHover(object sender, EventArgs e)
        {
            AccountsMenu.ForeColor = Color.White;
        }

        private void AccountsMenu_MouseLeave(object sender, EventArgs e)
        {
            if (!isMenuClicked[4])
            {
                AccountsMenu.ForeColor = Color.FromArgb(42, 90, 157);
            }
        }

        private void SettingsMenu_Click(object sender, EventArgs e)
        {
            isMenuClicked = Enumerable.Repeat(false, 6).ToList();
            isMenuClicked[5] = true;
            if (isMenuClicked[5])
            {
                SettingsMenu.ForeColor = Color.White;
                TasksMenu.ForeColor = Color.FromArgb(42, 90, 157);
                CreateTasksMenu.ForeColor = Color.FromArgb(42, 90, 157);
                ProfilesMenu.ForeColor = Color.FromArgb(42, 90, 157);
                ProxiesMenu.ForeColor = Color.FromArgb(42, 90, 157);
                AccountsMenu.ForeColor = Color.FromArgb(42, 90, 157);
            }
            SettingsMenu.ForeColor = Color.White;
            SettingsContainer.Visible = true;
            SettingsContainer.Dock = DockStyle.Fill;
            TasksContainer.Visible = false;
            CreateTasksContainer.Visible = false;
            ProfilesContainer.Visible = false;
            ProxiesContainer.Visible = false;
            AccountsContainer.Visible = false;
            CreateProfileContainer.Visible = false;
        }

        private void SettingsMenu_MouseHover(object sender, EventArgs e)
        {
            SettingsMenu.ForeColor = Color.White;
        }

        private void SettingsMenu_MouseLeave(object sender, EventArgs e)
        {
            if (!isMenuClicked[5])
            {
                SettingsMenu.ForeColor = Color.FromArgb(42, 90, 157);
            }
        }

        private void MinimizeButtonLabel_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void ProfileGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 3)
            {
                int ProfileID = Convert.ToInt32(ProfileGridView.Rows[e.RowIndex].Cells[0].Value);
                //////string BIBNO = ProfileGridView.Rows[e.RowIndex].Cells[0].Value.ToString();
                //////int RaceID = Convert.ToInt32(ProfileGridView.Rows[e.RowIndex].Cells[2].Value);
                ////DisplayUserInvalidDataDetails(BIBNO, RaceID);
                DisplayProfileData(ProfileID);
                IsUpdateProfile = true;
            }
        }

        private void DisplayProfileData(int ProfileID)
        {
            TasksContainer.Visible = false;
            CreateTasksContainer.Visible = false;
            ProxiesContainer.Visible = false;
            AccountsContainer.Visible = false;
            SettingsContainer.Visible = false;
            ProfilesContainer.Visible = false;
            CreateProfileContainer.Visible = true;
            CreateProfileContainer.Dock = DockStyle.Fill;

            List<Profile> profiledata = _NikeCore.LoadProfileData(ProfileID);

            if (profiledata.Count > 0)
            {
                BFirstnameInput.Text = profiledata[0].BFirstName;
                BLastnameInput.Text = profiledata[0].BLastName;
                BAddressInput.Text = profiledata[0].BAddress;
                BAPTInput.Text = profiledata[0].BAPT;

                BZIPInput.Text = profiledata[0].BZip;
                BCityInput.Text = profiledata[0].BCity;
                BStateInput.Text = profiledata[0].BState;
                BPhoneInput.Text = profiledata[0].BPhone;

                SFirstnameInput.Text = profiledata[0].SFirstName;
                SLastnameInput.Text = profiledata[0].SLastName;
                SAddressInput.Text = profiledata[0].SAddress;
                SAPTInput.Text = profiledata[0].SAPT;

                SZIPInput.Text = profiledata[0].SZip;
                SCityInput.Text = profiledata[0].SCity;
                SStateInput.Text = profiledata[0].SState;
                SPhoneInput.Text = profiledata[0].SPhone;

                CreditCardNumberInput.Text = profiledata[0].CardNumber;
                CVVInput.Text = profiledata[0].CardSecurity;
                ////CreditCardYearSelect.SelectedIndex = 0;
                ////CreditCardMonthSelect.SelectedIndex = 0;

                ////////CreditCardYearSelect.SelectedItem = profiledata[0].CardYear;
                ////////CreditCardMonthSelect.SelectedItem = profiledata[0].CardMonth;

                CreditCardYearSelect.SelectedIndex = CreditCardYearSelect.FindStringExact("20" + Convert.ToInt32(profiledata[0].CardYear).ToString());
                CreditCardMonthSelect.SelectedIndex = CreditCardMonthSelect.FindStringExact(Convert.ToInt32(profiledata[0].CardMonth).ToString());

                ProfileId = ProfileID;
            }
        }

        private void btnCancel_Profile_Click(object sender, EventArgs e)
        {
            ClearControls("Profile");
            ProfilesContainer.Visible = true;
            ProfilesContainer.Dock = DockStyle.Fill;
            CreateTasksContainer.Visible = false;
            TasksContainer.Visible = false;
            ProxiesContainer.Visible = false;
            AccountsContainer.Visible = false;
            SettingsContainer.Visible = false;
            CreateProfileContainer.Visible = false;
        }

        private void ExitButtonLabel_Click(object sender, EventArgs e)
        {
            DialogResult dialog = MessageBox.Show("Do you really want to close the program?", "Exit", MessageBoxButtons.YesNo);
            if (dialog == DialogResult.Yes)
            {
                foreach (var process in Process.GetProcessesByName("chromedriver"))
                {
                    process.Kill();
                }
                Application.ExitThread();
                //Environment.Exit(Environment.ExitCode);
                Application.Exit();
            }
            else
            {

            }
        }

        private void btnDeleteSelectedProfile_Click(object sender, EventArgs e)
        {
            var confirmResult = MessageBox.Show("Are you sure you want to delete this record??", "Confirm Delete!!", MessageBoxButtons.YesNo);
            if (confirmResult == DialogResult.Yes)
            {
                _NikeCore.DeleteProfile(ProfileId);
                ProfileId = 0;
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Environment.Exit(0);
        }

        private void ProfileGridView_RowHeaderMouseClick(object sender, System.Windows.Forms.DataGridViewCellMouseEventArgs e)
        {
            ProfileId = Convert.ToInt32(ProfileGridView.Rows[e.RowIndex].Cells[0].Value.ToString());
        }
    }
}