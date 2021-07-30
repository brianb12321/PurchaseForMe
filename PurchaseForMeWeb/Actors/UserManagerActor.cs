using Akka.Actor;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using PurchaseForMe.Core.User;

namespace PurchaseForMeWeb.Actors
{
    public class UserManagerActor : ReceiveActor
    {
        private IHttpContextAccessor _httpContext;
        private readonly UserManager<PurchaseForMeUser> _userManager;
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