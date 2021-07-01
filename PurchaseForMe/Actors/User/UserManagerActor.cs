using Akka.Actor;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using PurchaseForMe.Core.User;
using PurchaseForMe.Core.User.Message;

namespace PurchaseForMe.Actors.User
{
    public delegate IActorRef UserManagerActorFactory();
    public class UserManagerActor : ReceiveActor
    {
        private readonly UserManager<PurchaseForMeUser> _userManager;
        private IHttpContextAccessor _httpContext;
        public UserManagerActor(UserManager<PurchaseForMeUser> userManager, IHttpContextAccessor httpContext)
        {
            _userManager = userManager;
            _httpContext = httpContext;
            ReceiveAsync<GetLoggedInUserMessage>(async message =>
            {
                PurchaseForMeUser user = await _userManager.GetUserAsync(_httpContext.HttpContext.User);
                Sender.Tell(new GetUserResponseMessage(user));
            });
            ReceiveAsync<GetUserByIdMessage>(async message =>
            {
                PurchaseForMeUser user = await _userManager.FindByIdAsync(message.UserId);
                Sender.Tell(new GetUserResponseMessage(user));
            });
        }
    }
}