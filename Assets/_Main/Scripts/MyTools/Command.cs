using System;

namespace _Main.Scripts.MyTools
{
    public interface ICommand
    {
        void Execute();
    }
    
    public abstract class Command : ICommand , IDisposable
    {
        
        public abstract void Execute();

        public virtual void Dispose() { }

        ~Command()
        {
            Dispose();
        }
    }
}