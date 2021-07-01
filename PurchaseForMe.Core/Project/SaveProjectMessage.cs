﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PurchaseForMe.Core.Project
{
    public class SaveProjectMessage
    {
        public Guid ProjectGuid { get; }
        public string UserId { get; }

        public SaveProjectMessage(Guid projectGuid, string userId = "")
        {
            UserId = userId;
            ProjectGuid = projectGuid;
        }
    }
}