﻿using System;
using System.Collections.Generic;
using System.IO;

namespace MikudosLockStepGameService.Services.Game
{
    //#if DEBUG_SHOW_INPUT
    public partial class PlayerInput
    {
        public FixVector2 mousePos;
        public FixVector2 inputUV;
        public bool isInputFire;
        public int skillId;
        public bool isSpeedUp;

        //public override void Serialize(Serializer writer){
        //    writer.Write(mousePos);
        //    writer.Write(inputUV);
        //    writer.Write(isInputFire);
        //    writer.Write(skillId);
        //    writer.Write(isSpeedUp);
        //}

        public void Reset()
        {
            mousePos = FixVector2.Zero;
            inputUV = FixVector2.Zero;
            isInputFire = false;
            skillId = 0;
            isSpeedUp = false;
        }

        //public override void Deserialize(Deserializer reader){
        //    mousePos = reader.ReadLVector2();
        //    inputUV = reader.ReadLVector2();
        //    isInputFire = reader.ReadBoolean();
        //    skillId = reader.ReadInt32();
        //    isSpeedUp = reader.ReadBoolean();
        //}

        public static PlayerInput Empty = new PlayerInput();

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            var other = obj as PlayerInput;
            return Equals(other);
        }

        public bool Equals(PlayerInput other)
        {
            if (other == null) return false;
            if (mousePos != other.mousePos) return false;
            if (inputUV != other.inputUV) return false;
            if (isInputFire != other.isInputFire) return false;
            if (skillId != other.skillId) return false;
            if (isSpeedUp != other.isSpeedUp) return false;
            return true;
        }

        public override int GetHashCode()
        {
            return 0;
        }

        public PlayerInput Clone()
        {
            var tThis = this;
            return new PlayerInput()
            {
                mousePos = tThis.mousePos,
                inputUV = tThis.inputUV,
                isInputFire = tThis.isInputFire,
                skillId = tThis.skillId,
                isSpeedUp = tThis.isSpeedUp,
            };
        }
    }
    //#endif
}
