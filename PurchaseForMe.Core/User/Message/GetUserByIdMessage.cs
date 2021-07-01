using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PurchaseForMe.Core.User.Message
{
    public class GetUserByIdMessage
    {
        public string UserId { get; }

        public GetUserByIdMessage(string userId)
        {
            UserId = userId;
        }
    }
}