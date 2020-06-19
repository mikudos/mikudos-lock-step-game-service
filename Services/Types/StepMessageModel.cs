using System;
using System.Diagnostics;
using Lockstep;
using MikudosLockStepGameService.Services.MessageHandlers;

namespace MikudosLockStepGameService.Types
{
    [DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
    public class StepMessageModel
    {
        public long PlayerId;
        public HelloRequest Message;
        private LockStepImpl _service;
        private IHandler _handler;

        public StepMessageModel(LockStepImpl service)
        {
            _service = service;
        }

        public override string ToString()
        {
            return String.Format("stepMessage from: {0}, type: {1}", PlayerId, Message.MsgType);
        }
        private string GetDebuggerDisplay()
        {
            return ToString();
        }

        public void GenHandler()
        {
            switch (Message.MsgType)
            {
                case MessageType.Ping:
                    _handler = new PingHandler();
                    break;
                case MessageType.Pong:
                    break;
                case MessageType.ReqMissFrame:
                    break;
                case MessageType.ReqMissFrameAck:
                    break;
                case MessageType.RepMissFrame:
                    break;
                case MessageType.HashCode:
                    break;
                case MessageType.FrameData:
                    break;
                case MessageType.PlayerInput:
                    _handler = new InputHandler(_service._configuration);
                    break;
            }
        }

        public HelloReply Handle()
        {
            GenHandler();
            if (_handler == null)
            {
                return null;
            }
            return _handler.Handle(PlayerId, Message);
        }
    }
}