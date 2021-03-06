using System;
using System.Diagnostics;
using Lockstep;
using MikudosLockStepGameService.Services.MessageHandlers;

namespace MikudosLockStepGameService.Services.Models
{
    [DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
    public class BorderMessageModel
    {
        public ushort GameId;
        public MStepRes Message;

        public BorderMessageModel()
        {
        }

        public override string ToString()
        {
            return String.Format("stepMessage of game: {0}, type: {1}", GameId, Message.MsgType);
        }
        private string GetDebuggerDisplay()
        {
            return ToString();
        }
    }
}