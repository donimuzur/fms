﻿using FMS.BusinessObject;
using FMS.BusinessObject.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.Contract.Service
{
    public interface IArchTraCrfService
    {
        void Save(ARCH_TRA_CRF db,Login Login);
        List<ARCH_TRA_CRF> GetList();
        ARCH_TRA_CRF GetById(int id);
    }
}