using System;

namespace NikeAuto.Define
{
    public enum SocketID
    {
        Connected = 0,
        ReqLogin,
        Blocked,
        WrongPwd,
        Expired,
        LoginSuccess,
        NoExist,
        OtherMac,
    }

    [Serializable]
    public class SocketPacket
    {
        public SocketID socketID;
    }

    [Serializable]
    public class LoginInfo : SocketPacket
    {
        public string nickName;
        public string pwd;
        public string macAddr;
        public string ipAddr;
    }
}
