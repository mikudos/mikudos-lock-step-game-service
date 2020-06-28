using System;
using System.Diagnostics;
using System.IO;
using Lockstep;
using MikudosLockStepGameService.Services.MessageHandlers;

namespace MikudosLockStepGameService.Services.Models
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
                    _handler = new ReqMissFrameHandler(_service._configuration);
                    break;
                case EMessageType.ReqMissFrameAck:
                    break;
                case EMessageType.HashCode:
                    break;
                case EMessageType.LoadingProgress:
                    _handler = new LoadingProgressHandler(_service._configuration);
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

        public byte[] SerializeMessage()
        {
            var mmstream = new MemoryStream(this.Message.CalculateSize());
            var stream = new Google.Protobuf.CodedOutputStream(mmstream);
            this.Message.WriteTo(stream);
            System.Console.WriteLine($"len: {mmstream.CanRead}, {mmstream.CanWrite}, {mmstream.Capacity}, {this.Message.CalculateSize()}, {mmstream.GetBuffer()}");
            // read from stream
            var byteArray = mmstream.GetBuffer();
            // dispose after use
            stream.Dispose();
            // return byteArr;
            return byteArray;
        }

        public void SetMessageFromDeserialize(byte[] data)
        {
            this.Message = MStepReq.Parser.ParseFrom(data);
        }
    }
}