﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IodemBot.Core.UserManagement;

namespace IodemBot.Modules.GoldenSunMechanics
{
    class ReviveRequirement : IRequirement
    {
        public int apply(UserAccount user)
        {
            return user.revives >= 200 ? 2 : user.revives >= 120 ? 1 : 0;
        }
    }
}
