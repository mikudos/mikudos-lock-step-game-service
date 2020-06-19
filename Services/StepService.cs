using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Lockstep;
using Microsoft.Extensions.Configuration;
using MikudosLockStepGameService.Types;
using MikudosLockStepGameService.Rx;
using MikudosLockStepGameService.Services.MessageHandlers;

namespace MikudosLockStepGameService
{
    public class StepService
    {
        private LockStepImpl _lockStepService;
        private IConfiguration _configuration;
        private CommonObserver<StepMessageModel> stepperObserver;
        private const double UpdateInterval = 100 / 1000.0f; //frame rate = 10
        private DateTime _lastUpdateTimeStamp;
        private DateTime _startUpTimeStamp;
        private double _deltaTime;
        private double _timeSinceStartUp;
        public StepService(LockStepImpl lockStepService)
        {
            this._lockStepService = lockStepService;
            this._configuration = lockStepService._configuration;
            stepperObserver = new CommonObserver<StepMessageModel>("stepper", StepMessageHandler);
            this._lockStepService.requestO.Subscribe(stepperObserver);
        }

        public async void StepMessageHandler(StepMessageModel stepMessage)
        {
            System.Console.WriteLine($"on subscribe stepMessage: {stepMessage}");
            var playerId = stepMessage.PlayerId;
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