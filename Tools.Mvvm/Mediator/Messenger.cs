using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools.Mvvm.Mediator
{
    public sealed class Messenger<TMessage>
    {
        private static Messenger<TMessage>? _instance;

        public static Messenger<TMessage> Instance
        {
            get { return _instance ??= new Messenger<TMessage>(); }
        }

        private Action<TMessage>? _broadcast;

        private Messenger()
        {

        }

        public void Register(Action<TMessage> action)
        {
            _broadcast += action;
        }

        public void Send(TMessage message)
        {
            Action<TMessage>? broadcast = _broadcast;
            broadcast?.Invoke(message);
        }
    }
}
