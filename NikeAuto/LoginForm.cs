using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Win32;
using Newtonsoft.Json;
using NikeAuto.Define;
using NikeAuto.Engine;

namespace NikeAuto
{
    public partial class LoginForm : Form
    {
        //public SocketClient client = new SocketClient();
        //private string serverIP = "54.180.121.117";
        //private int port = 9005;
        //private Thread thread;
        //private string myIPAddr;
        //private string myMacAddr;
        private RegistryKey reg;
        private string userId = "Diego";
        private string userPassword = "NikeKiller";

        public LoginForm()
        {
            InitializeComponent();
        }

        // login form
        private void Form1_Load(object sender, EventArgs e)
        {
            //client.OnConnectHandler += connect;
            //client.OnDisconnectHandler += disconnect;
            //client.OnReceiveHandler += msg_received;

            //myIPAddr = GetExternalIPAddress();
            //myMacAddr = GstMacAddress();

            bool isRegistered = ReadRegistry(ref userId, ref userPassword);
            if (isRegistered == true)
            {
                MainForm mainForm = new MainForm();
                mainForm.Show();
                this.Opacity = 0;
                this.ShowInTaskbar = false;
                this.Hide();
            }
            else
            {
                this.Opacity = 1;
                this.ShowInTaskbar = true;
                this.Show();
            }
        }

        // button
        private void button_ok_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(input_id.Text.Trim()))
            {
                MessageBox.Show("Enter the User ID.");
                input_id.Focus();
                return;
            }

            if (string.IsNullOrEmpty(input_password.Text.Trim()))
            {
                MessageBox.Show("Enter the User Password.");
                input_password.Focus();
                return;
            }

            string userId = this.input_id.Text;
            string password = this.input_password.Text;
            if (userId == this.userId && password == this.userPassword)
            {
                WriteRegistry(userId, password);
                MainForm mainForm = new MainForm();
                mainForm.Show();
                this.Hide();
            }

            //if (thread.IsAlive)
            //{
            //    thread = new Thread(new ThreadStart(() => client.Start(serverIP, port)));
            //    thread.IsBackground = true;
            //    thread.Start();
            //}
            //else
            //{
            //    LoginInfo pckLogin = new LoginInfo();
            //    pckLogin.socketID = SocketID.ReqLogin;
            //    pckLogin.nickName = input_id.Text;
            //    pckLogin.pwd = input_password.Text;
            //    pckLogin.ipAddr = myIPAddr;
            //    pckLogin.macAddr = myMacAddr;
            //    client.SendMessage(pckLogin);
            //}
        }

        private void button_join_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void LoginForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            //client.ReleaseSocket();

            //client.OnConnectHandler -= connect;
            //client.OnDisconnectHandler -= disconnect;
            //client.OnReceiveHandler -= msg_received;
        }

        private void connect()
        {

        }

        //private void disconnect()
        //{
        //    if (thread.IsAlive)
        //    {
        //        thread.Interrupt();
        //        thread.Abort();
        //    }
        //}

        //private void msg_received(string msg)
        //{
        //    SocketPacket packet = JsonConvert.DeserializeObject<SocketPacket>(msg);

        //    switch (packet.socketID)
        //    {
        //        case SocketID.Connected:
        //            LoginInfo pckLogin = new LoginInfo();
        //            pckLogin.socketID = SocketID.ReqLogin;
        //            pckLogin.nickName = input_id.Text;
        //            pckLogin.pwd = input_password.Text;
        //            pckLogin.ipAddr = myIPAddr;
        //            pckLogin.macAddr = myMacAddr;
        //            client.SendMessage(pckLogin);
        //            break;

        //        case SocketID.Blocked:
        //            MessageBox.Show("Unfortunaley You are blocked by Manager. Please ask the support.");
        //            break;

        //        case SocketID.WrongPwd:
        //            MessageBox.Show("Incorrect Password. Please try again.");
        //            break;

        //        case SocketID.Expired:
        //            MessageBox.Show("It's past its expiration date. Please ask the support.");
        //            break;

        //        case SocketID.NoExist:
        //            MessageBox.Show("Invalid Account. Please ask support.");
        //            break;

        //        case SocketID.OtherMac:
        //            MessageBox.Show("We found the login attempt on other device, Please ask the support");
        //            break;

        //        case SocketID.LoginSuccess:
        //            this.Invoke(new MethodInvoker(delegate ()
        //            {
        //                MainForm mainForm = new MainForm();
        //                this.Hide();
        //                mainForm.ShowDialog();
        //            }));
        //            break;
        //    }
        //}

        //private string GetExternalIPAddress()
        //{
        //    string externalip = new WebClient().DownloadString("http://icanhazip.com").Trim(); //http://ipinfo.io/ip

        //    if (String.IsNullOrWhiteSpace(externalip))
        //    {
        //        externalip = "xxx.xxx.xx.xxx";
        //    }

        //    return externalip;
        //}

        //private string GstMacAddress()
        //{
        //    string mac = NetworkInterface.GetAllNetworkInterfaces()[0].GetPhysicalAddress().ToString();
        //    return mac;
        //}

        public bool ReadRegistry(ref string id, ref string pw)
        {
            reg = Registry.CurrentUser.CreateSubKey("SoftWare").CreateSubKey("NikeAuto");
            id = reg.GetValue("ID", "").ToString();
            pw = reg.GetValue("PW", "").ToString();

            if (id == this.userId && pw == this.userPassword)
                return true;
            else
                return false;
        }

        public void WriteRegistry(string id, string pw)
        {
            reg = Registry.CurrentUser.CreateSubKey("SoftWare").CreateSubKey("NikeAuto");
            reg.SetValue("ID", id);
            reg.SetValue("PW", pw);
        }
    }
}
