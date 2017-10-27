﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.BusinessObject.Dto;
using FMS.BusinessObject;

namespace FMS.Contract.BLL
{
    public interface IDelegationBLL
    {
        List<DelegationDto> GetDelegation();
        DelegationDto GetDelegationById(int Id);
        void Save(DelegationDto DelegationDto);
    }
}