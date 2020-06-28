using System;
using MikudosLockStepGameService.Services.Models;
namespace MikudosLockStepGameService.Services.Game
{
    [System.Serializable]
    public partial class ServerFrame
    {
        public int tick;
        private Msg_PlayerInput[] _inputs;

        public Msg_PlayerInput[] Inputs
        {
            get { return _inputs; }
            set
            {
                _inputs = value;
            }
        }

        private byte[] _serverInputs;

        public byte[] ServerInputs
        {//服务器输入 如掉落等
            get { return _serverInputs; }
            set
            {
                _serverInputs = value;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var frame = obj as ServerFrame;
            return Equals(frame);
        }

        public override int GetHashCode()
        {
            return tick;
        }

        public bool Equals(ServerFrame frame)
        {
            if (frame == null) return false;
            if (tick != frame.tick) return false;
            for (int i = _inputs.Length - 1; i >= 0; i--)
            {
                if (_inputs[i].Equals(frame.Inputs[i]))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
