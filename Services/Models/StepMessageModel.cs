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
        public MStepReq Message;
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
                case EMessageType.Ping:
                    _handler = new PingHandler();
                    break;
                case EMessageType.ReqMissFrame:
                    _handler = new PingHandler();
                    break;
                case EMessageType.ReqMissFrameAck:
                    break;
                case EMessageType.HashCode:
                    break;
                case EMessageType.PlayerInput:
                    _handler = new InputHandler(_service._configuration);
                    break;
                case EMessageType.Begin:
                    _handler = new BeginHandler(_service._configuration);
                    break;
            }
        }

        public MStepRes Handle()
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