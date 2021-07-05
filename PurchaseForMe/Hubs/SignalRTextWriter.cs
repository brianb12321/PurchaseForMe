using System.IO;
using System.Text;
using Microsoft.AspNetCore.SignalR;

namespace PurchaseForMe.Hubs
{
    class SignalRTextWriter : TextWriter
    {
        private readonly IClientProxy _client;

        public SignalRTextWriter(IClientProxy client)
        {
            _client = client;
        }
        public override Encoding Encoding => Encoding.UTF8;
        public override void WriteLine(string? value)
        {
            Write(value + "\n");
        }

        public override void Write(string? value)
        {
            _client.SendAsync("Console", value).GetAwaiter().GetResult();
        }
    }
}