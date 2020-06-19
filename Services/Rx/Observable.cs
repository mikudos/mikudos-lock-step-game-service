using System;
using System.Collections.Generic;
using MikudosLockStepGameService.Services.Exceptions;

namespace MikudosLockStepGameService.Rx
{
    public class CommonObservable<T> : IObservable<T>
    {
        private List<IObserver<T>> observers;

        public CommonObservable()
        {
            observers = new List<IObserver<T>>();
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            if (!observers.Contains(observer))
                observers.Add(observer);
            return new Unsubscriber(observers, observer);
        }

        public void Notify(T obj)
        {
            foreach (var observer in observers)
            {
                if (EqualityComparer<T>.Default.Equals(obj, default(T)))
                    observer.OnError(new UnknownException());
                else
                    observer.OnNext(obj);
            }
        }

        private class Unsubscriber : IDisposable
        {
            private List<IObserver<T>> _observers;
            private IObserver<T> _observer;

            public Unsubscriber(List<IObserver<T>> observers, IObserver<T> observer)
            {
                this._observers = observers;
                this._observer = observer;
            }

            public void Dispose()
            {
                if (_observer != null && _observers.Contains(_observer))
                    _observers.Remove(_observer);
            }
        }
    }
}