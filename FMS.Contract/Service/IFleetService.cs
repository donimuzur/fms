﻿using FMS.BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.Contract.Service
{
    public interface IFleetService
    {
        List<MST_FLEET> GetFleet();
        MST_FLEET GetFleetById(int MstFleetId);
        void save(MST_FLEET  dbFleet);
    }
}
