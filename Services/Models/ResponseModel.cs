using System;
using System.IO;
using System.Diagnostics;
using Lockstep;
using MikudosLockStepGameService.Services.MessageHandlers;

namespace MikudosLockStepGameService.Services.Models
{
    [DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
    public class ResponseModel
    {
        public long PlayerId;
        public MStepRes Message;

        public ResponseModel()
        {
        }

        public override string ToString()
        {
            return String.Format("stepMessage from: {0}, type: {1}", PlayerId, Message.MsgType);
        }
        private string GetDebuggerDisplay()
        {
            return ToString();
        }

        public byte[] SerializeMessage()
        {
            var mmstream = new MemoryStream();
            var stream = new Google.Protobuf.CodedOutputStream(mmstream);
            this.Message.WriteTo(stream);
            stream.Dispose();
            mmstream.Seek(0, SeekOrigin.Begin);
            var byteArray = new byte[mmstream.Length];
            var count = mmstream.Read(byteArray, 0, 20);
            // Read the remaining bytes, byte by byte.
            while (count < mmstream.Length)
            {
                byteArray[count++] = Convert.ToByte(mmstream.ReadByte());
            }
            // return byteArr;
            return byteArray;
        }

        public void SetMessageFromDeserialize(byte[] data)
        {
            this.Message = MStepRes.Parser.ParseFrom(data);
        }
    }
}