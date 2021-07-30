using System;

namespace PurchaseForMe.Core.TaskMonitoring
{
    public record ClientConnectedMessage(Guid TaskGuid, string ClientId);

    public record ClientDisconnectedMessage(string ClientId);

    public record CodeChannelWriteMessage(Guid CodeGuid, string Message);
}