﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.BusinessObject.Dto;
using FMS.BusinessObject.Inputs;

namespace FMS.Contract.BLL
{
    public interface IExecutiveSummaryBLL
    {
        List<NoVehicleDto> GetNoOfVehicleData(VehicleGetByParamInput filter);
        List<NoVehicleWtcDto> GetNoOfVehicleWtcData(VehicleWtcGetByParamInput filter);
        List<NoVehicleMakeDto> GetNoOfVehicleMakeData(VehicleMakeGetByParamInput filter);
        List<OdometerDto> GetOdometerData(OdometerGetByParamInput filter);
        List<LiterByFunctionDto> GetLiterByFunctionData(LiterFuncGetByParamInput filter);
    }
}
