/* 
 * Author Julian Yue
 * 
 * StepService Handle all client Message, and reply the result.
 * With Public Task Start method, StepService starting iterator. 
 * 
 * Update will check UpdateInterval, every interval arival, StepService will inform Game service to do update
 * The Server Frame will be update with Player input from lastest update.
 * 
 * Every Server Frame updated, StepService will response then.
 * 
 */
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Lockstep;
using Microsoft.Extensions.Configuration;
using MikudosLockStepGameService.Types;
using MikudosLockStepGameService.Rx;
using MikudosLockStepGameService.Services.MessageHandlers;
using MikudosLockStepGameService.Services.Game;
using System.IO;

namespace MikudosLockStepGameService
{
    public sealed class StepService
    {
        private ILockStepImpl _lockStepService;
        private IConfiguration _configuration;
        private CommonObserver<StepMessageModel> stepperObserver;
        private CommonObserver<BorderMessageModel> gameFrameObserver;
        private CommonObserver<ResponseModel> responseObserver;
        private double UpdateInterval;
        private DateTime _lastUpdateTimeStamp;
        private DateTime _startUpTimeStamp;
        private double _deltaTime;
        private double _timeSinceStartUp;
        private IGameClass _game;
        public StepService(ILockStepImpl lockStepService)
        {
            this._lockStepService = lockStepService;
            this._configuration = lockStepService._configuration;
            UpdateInterval = _configuration.GetValue<int>("frame_interval", 100) / 1000.0f; // default frame_interval = 100
            stepperObserver = new CommonObserver<StepMessageModel>("stepper", StepMessageHandler);
            this._lockStepService.requestO.Subscribe(stepperObserver);
            gameFrameObserver = new CommonObserver<BorderMessageModel>("border", BorderMessageHandler);
            GameClass.borderMessageO.Subscribe(gameFrameObserver);
            responseObserver = new CommonObserver<ResponseModel>("response", ResponseMessageHandler);
            GameClass.responseMessageO.Subscribe(responseObserver);
        }

        private async void ResponseMessageHandler(ResponseModel responseMessage)
        {
            foreach (var (pid, stream) in _lockStepService.PlayerStreams)
            {
                if (pid != responseMessage.PlayerId)
                    continue;
                await stream.WriteAsync(responseMessage.Message);
            }
        }

        private async void BorderMessageHandler(BorderMessageModel borderMessage)
        {
            foreach (var (playerId, stream) in _lockStepService.PlayerStreams)
            {
                if (GameClass.PlayerGameMap[playerId] != borderMessage.GameId)
                    continue;
                await stream.WriteAsync(borderMessage.Message);
            }
        }

        private async void StepMessageHandler(StepMessageModel stepMessage)
        {
            byte[] byteArr = new byte[Google.Protobuf.CodedOutputStream.DefaultBufferSize];
            var stream = new Google.Protobuf.CodedOutputStream(byteArr);
            stepMessage.Message.WriteTo(stream);
            stream.Dispose();

            System.Console.WriteLine($"on subscribe stepMessage: {stepMessage}");
            long playerId = stepMessage.PlayerId;
            var reply = stepMessage.Handle();
            if (reply == null)
            {
                return;
            }
            await _lockStepService.PlayerStreams[playerId].WriteAsync(reply);
        }

        private void Update()
        {
            var now = DateTime.Now;
            _deltaTime = (now - _lastUpdateTimeStamp).TotalSeconds;
            if (_deltaTime > UpdateInterval)
            {
                _lastUpdateTimeStamp = now;
                _timeSinceStartUp = (now - _startUpTimeStamp).TotalSeconds;
                DoUpdate();
            }
        }

        private void DoUpdate()
        {
            //check frame inputs
            var fDeltaTime = (float)_deltaTime;
            var fTimeSinceStartUp = (float)_timeSinceStartUp;
            // System.Console.WriteLine("deltatime over, do update");
            // _game?.DoUpdate(fDeltaTime);
        }

        public async Task Start()
        {
            await Task.Run(() =>
            {
                while (true)
                {
                    try
                    {
                        Thread.Sleep(3);
                        Update();
                    }
                    catch (ThreadAbortException e)
                    {
                        return;
                    }
                    catch (Exception e)
                    {
                        System.Console.WriteLine($"error: {e}");
                    }
                }
            });
        }
    }
}