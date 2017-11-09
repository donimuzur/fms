﻿using FMS.BusinessObject.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.Contract.BLL
{
    public interface ITraCtfBLL
    {
        List<TraCtfDto> GetCtf();
        void Save(TraCtfDto Dto, string userId);
    }
}
