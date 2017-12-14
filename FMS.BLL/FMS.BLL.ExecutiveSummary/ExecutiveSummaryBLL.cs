﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FMS.BusinessObject.Dto;
using FMS.BusinessObject.Inputs;
using FMS.Contract;
using FMS.Contract.BLL;
using FMS.Contract.Service;
using FMS.DAL.Services;

namespace FMS.BLL.ExecutiveSummary
{
    public class ExecutiveSummaryBLL : IExecutiveSummaryBLL
    {
        private IExecutiveSummaryService _ExecSummService;
        private IUnitOfWork _uow;

        public ExecutiveSummaryBLL(IUnitOfWork uow)
        {
            _uow = uow;
            _ExecSummService = new ExecutiveSummaryService(_uow);
        }

        public List<NoVehicleDto> GetNoOfVehicleData(VehicleGetByParamInput filter)
        {
            var data = _ExecSummService.GetAllNoVehicle(filter);

            return Mapper.Map<List<NoVehicleDto>>(data);
        }

        public List<NoVehicleWtcDto> GetNoOfVehicleWtcData(VehicleWtcGetByParamInput filter)
        {
            var data = _ExecSummService.GetAllNoVehicleWtc(filter);

            return Mapper.Map<List<NoVehicleWtcDto>>(data);
        }

        public List<NoVehicleMakeDto> GetNoOfVehicleMakeData(VehicleMakeGetByParamInput filter)
        {
            var data = _ExecSummService.GetAllNoVehicleMake(filter);

            return Mapper.Map<List<NoVehicleMakeDto>>(data);
        }

        public List<OdometerDto> GetOdometerData(OdometerGetByParamInput filter)
        {
            var data = _ExecSummService.GetAllOdometer(filter);

            return Mapper.Map<List<OdometerDto>>(data);
        }

        public List<LiterByFunctionDto> GetLiterByFunctionData(LiterFuncGetByParamInput filter)
        {
            var data = _ExecSummService.GetAllLiterByFunction(filter);

            return Mapper.Map<List<LiterByFunctionDto>>(data);
        }

        public List<FuelCostByFunctionDto> GetFuelCostByFunctionData(FuelCostFuncGetByParamInput filter)
        {
            var data = _ExecSummService.GetAllFuelCostByFunction(filter);

            return Mapper.Map<List<FuelCostByFunctionDto>>(data);
        }

        public List<LeaseCostByFunctionDto> GetLeaseCostByFunctionData(LeaseCostFuncGetByParamInput filter)
        {
            var data = _ExecSummService.GetAllLeaseCostByFunction(filter);

            return Mapper.Map<List<LeaseCostByFunctionDto>>(data);
        }

        public List<SalesByRegionDto> GetSalesByRegionData(SalesRegionGetByParamInput filter)
        {
            var data = _ExecSummService.GetAllSalesByRegion(filter);

            return Mapper.Map<List<SalesByRegionDto>>(data);
        }

        public List<AccidentDto> GetAccidentData(AccidentGetByParamInput filter)
        {
            var data = _ExecSummService.GetAllAccident(filter);

            return Mapper.Map<List<AccidentDto>>(data);
        }

        public List<AcVsObDto> GetAcVsObData(AcVsObGetByParamInput filter)
        {
            var data = _ExecSummService.GetAllAcVsOb(filter);

            return Mapper.Map<List<AcVsObDto>>(data);
        }
    }
}
