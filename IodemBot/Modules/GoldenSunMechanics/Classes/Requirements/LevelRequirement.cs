﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IodemBot.Core.UserManagement;

namespace IodemBot.Modules.GoldenSunMechanics
{
    class LevelRequirement : IRequirement
    {
        public int apply(UserAccount user)
        {
            return (int)(user.LevelNumber / 10);
        }
    }
}
