using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Util.Quee
{
    public class ResponseMessage
    {
        public string SerializableResponse { get; set; }
        public ResponseMessage()
        {
           
        }
    }
    public interface INotification { }
    public abstract class Message
    {
        public string MessageType { get; protected set; }
        public Guid AggregateId { get; protected set; }
        protected Message()
        {
            MessageType = GetType().Name;
        }
    }
    public abstract class Event : Message, INotification
    {
        public DateTime Timestamp { get; private set; }

        protected Event()
        {
            Timestamp = DateTime.Now;
        }
    }
    public enum PositionEvent
    {

        Start = 1,
        Proccess = 2,
        End = 3,

    }

    public enum TypeProcess
    {
        IncluirUsuario = 1,
        DeletarUsuario = 2

    }
    public abstract class IntegrationEvent : Event
    {

        public int Order { get; set; }
        public Guid UserId { get; set; }
        public Guid ProcessoId { get; set; }
        public IntegrationEvent() : base()
        {

        }

    }
    public class PropsMessageQueeDto
    {
        public string Queue { get; set; } = "";
        public bool Durable { get; set; } = true;
        public bool Exclusive { get; set; } = false;
        public bool AutoDelete { get; set; } = false;
        public IDictionary<string, object> Arguments { get; set; } = new Dictionary<string, object>();
        /*5 min*/
        public int TimeoutMensagem { get; set; } = 300000;
    }
}
