namespace PurchaseForMe.Core.User
{
    public record GetLoggedInUserMessage();

    public record GetUserByIdMessage(string UserId);

    public record GetUserResponseMessage(PurchaseForMeUser User);
}