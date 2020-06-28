using System;
using Lockstep;
namespace MikudosLockStepGameService.Services.Models
{
    public class MultiFrames
    {
        public int StartTick;
        public ServerFrame[] frames;

        public MultiFrames()
        {
        }

        public MMultiFrames TransformToMMultiFrames()
        {
            MMultiFrames multiFrames = new MMultiFrames() { StartTick = this.StartTick };
            foreach (var frame in this.frames)
            {
                multiFrames.ServerFrames.Add(frame.TransformToMServerFrame());
            }
            return multiFrames;
        }
    }
}
