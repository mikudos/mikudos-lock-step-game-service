﻿using System;
using Lockstep;
using System.Collections.Generic;
using MikudosLockStepGameService.Services.Models;

namespace MikudosLockStepGameService.Services.Models
{
    [System.Serializable]
    public partial class Msg_PlayerInput
    {
        public bool IsMiss;
        public byte ActorId;
        public int Tick;
        public PlayerInput[] Commands;
#if DEBUG_FRAME_DELAY
        public float timeSinceStartUp;
#endif

        public Msg_PlayerInput(int tick, byte actorID, List<PlayerInput> inputs)
        {
            this.Tick = tick;
            this.ActorId = actorID;
            if (inputs != null && inputs.Count > 0)
            {
                this.Commands = inputs.ToArray();
            }
        }
        public Msg_PlayerInput(int tick, byte actorID, PlayerInput[] inputs = null)
        {
            this.Tick = tick;
            this.ActorId = actorID;
            if (inputs != null && inputs.Length > 0)
            {
                this.Commands = inputs;
            }
        }

        public Msg_PlayerInput() { }

        public override bool Equals(object obj)
        {
            return Equals(obj as Msg_PlayerInput);
        }

        public bool Equals(Msg_PlayerInput other)
        {
            if (other == null) return false;
            if (Tick != other.Tick) return false;
            for (int i = Commands.Length - 1; i >= 0; i--)
            {
                if (Commands[i].Equals(other.Commands[i]))
                {
                    return false;
                }
            }
            return true;
        }

        public override int GetHashCode()
        {
            return (int)(ActorId << 24 & Tick);
        }

        public MPlayerGameInput TransformToPlayerGameInput()
        {
            var playerGameInput = new MPlayerGameInput() { ActorId = this.ActorId, Tick = this.Tick, IsMiss = this.IsMiss };
            foreach (var c in this.Commands)
            {
                playerGameInput.Commands.Add(c.GetGameInput());
            }
            return playerGameInput;
        }
    }
}
