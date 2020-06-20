using System;
namespace MikudosLockStepGameService.Services.Game
{
    [System.Serializable]
    public partial class ServerFrame
    {
        public byte[] inputDatas; //包含玩家的输入& 游戏输入
        public int tick;
        public Msg_PlayerInput[] _inputs;

        public Msg_PlayerInput[] Inputs
        {
            get { return _inputs; }
            set
            {
                _inputs = value;
                inputDatas = null;
            }
        }

        private byte[] _serverInputs;

        public byte[] ServerInputs
        {//服务器输入 如掉落等
            get { return _serverInputs; }
            set
            {
                _serverInputs = value;
                inputDatas = null;
            }
        }

        public override string ToString()
        {
            var count = (inputDatas == null) ? 0 : inputDatas.Length;
            return
                $"t:{tick} " +
                $"inputNum:{count}";
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
