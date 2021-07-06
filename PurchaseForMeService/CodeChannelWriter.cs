using System;
using System.IO;
using System.Text;
using Akka.Event;
using PurchaseForMe.Core.TaskMonitoring;

namespace PurchaseForMe
{
    class CodeChannelWriter : TextWriter
    {
        private readonly EventStream _eventStream;
        private readonly Guid _codeGuid;

        public CodeChannelWriter(Guid codeGuid, EventStream eventStream)
        {
            _eventStream = eventStream;
            _codeGuid = codeGuid;
        }
        public override Encoding Encoding => Encoding.UTF8;
        public override void WriteLine(string? value)
        {
            Write(value + "\r\n");
        }

        public override void Write(string? value)
        {
            _eventStream.Publish(new CodeChannelWriteMessage(_codeGuid, value));
        }
    }
}