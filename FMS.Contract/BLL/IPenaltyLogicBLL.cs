﻿using FMS.BusinessObject.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.Contract.BLL
{
    public interface IPenaltyLogicBLL
    {
        PenaltyLogicDto GetPenaltyLogicById(int MstPealtyLogicById);
        List<PenaltyLogicDto> GetPenaltyLogic();
        void Save(PenaltyLogicDto Dto);
    }
}
