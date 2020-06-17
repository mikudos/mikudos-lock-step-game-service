using System;
using System.Collections.Generic;

namespace MikudosLockStepGameService.Rx
{
    public class CommonObserver<T> : IObserver<T>
    {
        private IDisposable unsubscriber;
        private string Name;
        private Action<T> fn;
        public CommonObserver(string name, Action<T> fn)
        {
            this.Name = name;
            this.fn = fn;
        }

        public virtual void OnCompleted()
        {
            this.Unsubscribe();
        }

        public virtual void OnError(Exception e)
        {
            Console.WriteLine(string.Format("The location cannot be determined."));
        }

        public virtual void OnNext(T obj) // 需要被重写
        {
            this.fn(obj);
        }


        public virtual void Subscribe(IObservable<T> provider)
        {
            if (provider != null)
                unsubscriber = provider.Subscribe(this);
        }

        public virtual void Unsubscribe()
        {
            unsubscriber.Dispose();
        }
    }

}
