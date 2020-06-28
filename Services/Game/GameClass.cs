using System;
using System.IO;
using Lockstep;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using MikudosLockStepGameService.Rx;
using MikudosLockStepGameService.Util;
using MikudosLockStepGameService.Services.Models;
using MikudosLockStepGameService.Services.Exceptions;
using MikudosLockStepGameService.Services.Logger;

namespace MikudosLockStepGameService.Services.Game
{
    public class GameClass : BaseLogger, IGameClass
    {
        public static Dictionary<ushort, IGameClass> AllGames { get; } = new Dictionary<ushort, IGameClass>();
        public static Dictionary<long, ushort> PlayerGameMap = new Dictionary<long, ushort>();
        public static CommonObservable<BorderMessageModel> borderMessageO { get; private set; } = new CommonObservable<BorderMessageModel>();
        private IConfiguration _configuration;
        public ushort GameId { get; private set; }
        public int MapId { get; set; }
        public int GameType { get; set; }
        public string GameHash { get; set; }
        public string Name { get; set; }
        public int Tick { get; private set; } = 0;
        public int Seed { get; set; }
        public EGameState State { get; private set; } = EGameState.Idle;
        private Dictionary<long, byte> _userId2LocalId = new Dictionary<long, byte>();
        public PlayerModel[] Players { get; private set; }
        private int MaxPlayerCount;
        private float _timeSinceLoaded;
        private float _firstFrameTimeStamp = 0;
        private float _waitTimer = 0;
        public long _gameStartTimestampMs = -1;
        public int _ServerTickDealy = 0;
        public int _tickSinceGameStart =>
            (int)((LTime.realtimeSinceStartupMS - _gameStartTimestampMs) / _configuration.GetValue<int>("frame_interval", 100) / 1000.0f);
        private List<ServerFrame> _allHistoryFrames = new List<ServerFrame>(); //所有的历史帧
        public GameClass(IConfiguration configuration)
        {
            this._configuration = configuration;
            MaxPlayerCount = configuration.GetValue("max_player_count", 100);
        }

        public static IGameClass GetGame(ushort key)
        {
            if (AllGames[key] == null)
            {
                throw new SceneNotExistsException();
            }
            return AllGames[key];
        }

        public IGameClass DoCreate(IConfiguration configuration)
        {
            var _key = AllGames.Count;
            if (_key >= _configuration.GetValue<int>("max_scene_count", ushort.MaxValue))
            {
                throw new GameOverloadException();
            }
            ushort key = (ushort)_key;
            AllGames[key] = new GameClass(configuration) { GameId = key };
            return AllGames[key];
        }

        public void DoStart(int gameType, int mapId, PlayerModel[] playerInfos, string gameHash)
        {
            State = EGameState.Loading;
            Seed = LRandom.Range(1, 100000);
            _timeSinceLoaded = 0;
            _firstFrameTimeStamp = 0;
            GameType = gameType;
            GameHash = gameHash;
            Name = GameId.ToString();
            MapId = mapId;
            _userId2LocalId.Clear();
            foreach (var player in playerInfos)
            {
                _userId2LocalId.Add(player.UserId, player.LocalId);
            }
        }

        public void DoUpdate(float deltaTime)
        {
            _timeSinceLoaded += deltaTime;
            _waitTimer += deltaTime;
            if (State != EGameState.Playing) return;
            if (_gameStartTimestampMs <= 0) return;
            while (Tick < _tickSinceGameStart)
            {
                _CheckBorderServerFrame(true);
            }
        }

        public void DoDestroy()
        {
            Log($"Room {GameId} Destroy");
            DumpGameFrames();
        }

        private void DumpGameFrames()
        {
            var msg = new MultiFrames();
            int count = System.Math.Min((Tick - 1), _allHistoryFrames.Count);
            if (count <= 0) return;
            var frames = new ServerFrame[count];
            for (int i = 0; i < count; i++)
            {
                frames[i] = _allHistoryFrames[i];
                Logger.Debug.Assert(frames[i] != null, "!!!!!!!!!!!!!!!!!");
            }

            msg.startTick = frames[0].tick;
            msg.frames = frames;
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                "../Record/" + System.DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + GameType + "_" + GameId +
                ".record");
            var dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            Log("Create Record " + path);
            //File.WriteAllBytes(path, bytes);
        }

        private bool _CheckBorderServerFrame(bool isForce = false)
        {
            if (State != EGameState.Playing) return false;
            var frame = GetOrCreateFrame(Tick);
            var inputs = frame.Inputs;
            if (!isForce)
            {
                //是否所有的输入  都已经等到
                for (int i = 0; i < inputs.Length; i++)
                {
                    if (inputs[i] == null)
                    {
                        return false;
                    }
                }
            }

            //将所有未到的包 给予默认的输入
            for (int i = 0; i < inputs.Length; i++)
            {
                if (inputs[i] == null)
                {
                    inputs[i] = new Msg_PlayerInput(Tick, (byte)i) { IsMiss = true };
                }
            }

            //Debug.Log($" Border input {Tick} isUpdate:{isForce} _tickSinceGameStart:{_tickSinceGameStart}");
            var msg = new MultiFrames();
            int count = Tick < 2 ? Tick + 1 : 3;
            var frames = new ServerFrame[count];
            for (int i = 0; i < count; i++)
            {
                frames[count - i - 1] = _allHistoryFrames[Tick - i];
            }

            msg.startTick = frames[0].tick;
            msg.frames = frames;
            // FIXME: set response, message should with ServerFrames or PlayerGameInputs
            borderMessageO.Notify(new BorderMessageModel() { GameId = this.GameId, Message = new MStepRes() { MsgType = EResType.StepResponse } });
            if (_firstFrameTimeStamp <= 0)
            {
                _firstFrameTimeStamp = _timeSinceLoaded;
            }

            if (_gameStartTimestampMs < 0)
            {
                _gameStartTimestampMs =
                    LTime.realtimeSinceStartupMS + _configuration.GetValue<int>("frame_interval", 100) * _ServerTickDealy;
            }

            Tick++;
            return true;
        }

        ServerFrame GetOrCreateFrame(int tick)
        {
            //扩充帧队列
            var frameCount = _allHistoryFrames.Count;
            if (frameCount <= tick)
            {
                var count = tick - _allHistoryFrames.Count + 1;
                for (int i = 0; i < count; i++)
                {
                    _allHistoryFrames.Add(null);
                }
            }

            if (_allHistoryFrames[tick] == null)
            {
                _allHistoryFrames[tick] = new ServerFrame() { tick = tick };
            }

            var frame = _allHistoryFrames[tick];
            if (frame.Inputs == null || frame.Inputs.Length != MaxPlayerCount)
            {
                frame.Inputs = new Msg_PlayerInput[MaxPlayerCount];
            }

            return frame;
        }

        public ushort GetGameIdWithPlayerId(long playerId)
        {
            return PlayerGameMap[playerId];
        }
    }
}
