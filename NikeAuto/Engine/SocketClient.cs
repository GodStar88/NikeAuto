using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using NikeAuto.Define;

namespace NikeAuto.Engine
{
    public class SocketClient
    {
        private Socket sckClient;
        private const int BUFFER_SIZE = 1024;
        private static string ENDSYMBOL = "<==End==>";
        private StringBuilder receivedData = new StringBuilder();

        public delegate void ConnectedHandler();
        public event ConnectedHandler OnConnectHandler;

        public delegate void SendHandler();
        public event SendHandler OnSendHandler;

        public delegate void ReceiveHandler(string msg);
        public event ReceiveHandler OnReceiveHandler;

        public delegate void DisconnectCallBack();
        public event DisconnectCallBack OnDisconnectHandler;

        public void Start(string serverIP, int port)
        {
            try
            {
                sckClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                sckClient.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, true);
                IPEndPoint ipep = new IPEndPoint(IPAddress.Parse(serverIP), port);

                SocketAsyncEventArgs args = new SocketAsyncEventArgs();
                args.RemoteEndPoint = ipep;
                args.Completed += new EventHandler<SocketAsyncEventArgs>(Connect_Completed);

                sckClient.ConnectAsync(args);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("서버접속실패: " + ex.Message);
            }
        }

        private void Connect_Completed(object sender, SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
            {
                if (this.OnConnectHandler != null)
                {
                    this.OnConnectHandler();
                }

                BeginReceive();
            }
            else
            {
                Debug.WriteLine(DateTime.Now.ToString("HH:mm:ss.fff") + " Socket: Connect Fail: {0}", e.SocketError);
            }
        }

        private void BeginReceive()
        {
            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.Completed += new EventHandler<SocketAsyncEventArgs>(Process_Receive);
            //receive_event_arg.UserToken = token;
            args.SetBuffer(new byte[BUFFER_SIZE], 0, BUFFER_SIZE);

            sckClient.ReceiveAsync(args); 
        }

        private void Process_Receive(object sender, SocketAsyncEventArgs e)
        {
            if (e.LastOperation == SocketAsyncOperation.Receive)
            {
                if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
                {
                    receivedData.Append(Encoding.UTF8.GetString(e.Buffer, 0, e.BytesTransferred));

                    if (receivedData.ToString().Contains(ENDSYMBOL))
                    {
                        if (this.OnReceiveHandler != null)
                        {
                            this.OnReceiveHandler(receivedData.ToString().Replace(ENDSYMBOL, ""));
                        }

                        receivedData.Clear();
                    }

                    sckClient.ReceiveAsync(e);
                }
                else
                {
                    Debug.WriteLine(DateTime.Now.ToString("HH:mm:ss.fff") + " Socket: Process_Receive Fail: {0}, {1}", e.BytesTransferred, e.SocketError);

                    if (this.OnDisconnectHandler != null)
                    {
                        this.OnDisconnectHandler();
                    }
                }
            }
        }

        public void SendMessage(SocketPacket packet)
        {
            try
            {
                SocketAsyncEventArgs args = new SocketAsyncEventArgs();
                args.Completed += new EventHandler<SocketAsyncEventArgs>(Process_Send);
                //send_event_arg.UserToken = token;
                string msg = JsonConvert.SerializeObject(packet);
                byte[] szData = Encoding.UTF8.GetBytes(msg + ENDSYMBOL);
                args.SetBuffer(szData, 0, szData.Length);

                //if (isConnected)
                {
                    sckClient.SendAsync(args);
                }
            }
            catch (Exception e)
            {
                if (sckClient == null)
                {
                    //close();
                    return;
                }

                Debug.WriteLine(DateTime.Now.ToString("HH:mm:ss.fff") + " Socket: SendMessage Fail: {0}", e.ToString());
            }
        }

        private void Process_Send(object sender, SocketAsyncEventArgs e)
        {
            if (e.LastOperation == SocketAsyncOperation.Send)
            {
                if (e.BytesTransferred <= 0 || e.SocketError != SocketError.Success)
                {
                    // 연결이 끊겨서 이미 소켓이 종료된 경우일 것이다.
                    Debug.WriteLine(DateTime.Now.ToString("HH:mm:ss.fff") + " Socket: Process_Send Fail: {0}, {1}", e.SocketError, e.BytesTransferred);
                    return;
                }

                if (this.OnSendHandler != null)
                {
                    this.OnSendHandler();
                }
            }
        }

        public void ReleaseSocket()
        {
            if (sckClient != null)
            {
                sckClient.Shutdown(SocketShutdown.Both);
                sckClient.Close();
            }
        }
    }
}
