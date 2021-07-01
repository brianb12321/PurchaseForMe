using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PurchaseForMe.Core.User.Message
{
    public class GetUserResponseMessage
    {
        public PurchaseForMeUser User { get; }

        public GetUserResponseMessage(PurchaseForMeUser user)
        {
            User = user;
        }
    }
}