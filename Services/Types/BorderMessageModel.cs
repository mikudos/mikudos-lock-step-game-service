using System;
using System.Diagnostics;
using Lockstep;
using MikudosLockStepGameService.Services.MessageHandlers;

namespace MikudosLockStepGameService.Types
{
    [DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
    public class BorderMessageModel
    {
        public long? PlayerId;
        public HelloRequest Message;

        public BorderMessageModel()
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
    }
}