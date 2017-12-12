﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using FMS.BusinessObject;
using FMS.BusinessObject.Inputs;
using FMS.Contract;
using FMS.Contract.Service;
using FMS.Utils;

namespace FMS.DAL.Services
{
    public class ExecutiveSummaryService : IExecutiveSummaryService
    {
        private IUnitOfWork _uow;

        private IGenericRepository<NO_OF_VEHICLE_REPORT_DATA> _noVehRepository;
        private IGenericRepository<NO_OF_WTC_VEHICLE_REPORT_DATA> _noVehWtcRepository;
        private IGenericRepository<NO_OF_VEHICLE_MAKE_TYPE_REPORT_DATA> _noVehMakeRepository;
        private IGenericRepository<ODOMETER_REPORT_DATA> _odometerRepository;
        private IGenericRepository<LITER_BY_FUNC_REPORT_DATA> _literByFuncRepository;
        private IGenericRepository<FUEL_COST_BY_FUNC_REPORT_DATA> _fuelCostByFuncRepository;

        public ExecutiveSummaryService(IUnitOfWork uow)
        {
            _uow = uow;
            _noVehRepository = _uow.GetGenericRepository<NO_OF_VEHICLE_REPORT_DATA>();
            _noVehWtcRepository = _uow.GetGenericRepository<NO_OF_WTC_VEHICLE_REPORT_DATA>();
            _noVehMakeRepository = _uow.GetGenericRepository<NO_OF_VEHICLE_MAKE_TYPE_REPORT_DATA>();
            _odometerRepository = _uow.GetGenericRepository<ODOMETER_REPORT_DATA>();
            _literByFuncRepository = _uow.GetGenericRepository<LITER_BY_FUNC_REPORT_DATA>();
            _fuelCostByFuncRepository = _uow.GetGenericRepository<FUEL_COST_BY_FUNC_REPORT_DATA>();
        }

        public List<NO_OF_VEHICLE_REPORT_DATA> GetAllNoVehicle(VehicleGetByParamInput filter)
        {
            Expression<Func<NO_OF_VEHICLE_REPORT_DATA, bool>> queryFilter = PredicateHelper.True<NO_OF_VEHICLE_REPORT_DATA>();

            if (filter != null) {
                if (filter.MonthFrom > 0)
                {
                    queryFilter = queryFilter.And(c => c.REPORT_MONTH >= filter.MonthFrom);
                }
                if (filter.MonthTo > 0)
                {
                    queryFilter = queryFilter.And(c => c.REPORT_MONTH <= filter.MonthTo);
                }
                if (filter.YearFrom > 0)
                {
                    queryFilter = queryFilter.And(c => c.REPORT_YEAR >= filter.YearFrom);
                }
                if (filter.YearTo > 0)
                {
                    queryFilter = queryFilter.And(c => c.REPORT_YEAR <= filter.YearTo);
                }
                if (!string.IsNullOrEmpty(filter.VehicleType))
                {
                    queryFilter = queryFilter.And(c => c.VEHICLE_TYPE.ToUpper() == filter.VehicleType.ToUpper());
                }
                if (!string.IsNullOrEmpty(filter.SupplyMethod))
                {
                    queryFilter = queryFilter.And(c => c.SUPPLY_METHOD.ToUpper() == filter.SupplyMethod.ToUpper());
                }
                if (!string.IsNullOrEmpty(filter.Regional))
                {
                    queryFilter = queryFilter.And(c => c.REGION.ToUpper() == filter.Regional.ToUpper());
                }
                if (!string.IsNullOrEmpty(filter.Function))
                {
                    queryFilter = queryFilter.And(c => c.FUNCTION.ToUpper() == filter.Function.ToUpper());
                }
            }

            return _noVehRepository.Get(queryFilter, null, "").ToList();
        }

        public List<NO_OF_WTC_VEHICLE_REPORT_DATA> GetAllNoVehicleWtc(VehicleWtcGetByParamInput filter)
        {
            Expression<Func<NO_OF_WTC_VEHICLE_REPORT_DATA, bool>> queryFilter = PredicateHelper.True<NO_OF_WTC_VEHICLE_REPORT_DATA>();

            if (filter != null)
            {
                if (filter.MonthFrom > 0)
                {
                    queryFilter = queryFilter.And(c => c.REPORT_MONTH >= filter.MonthFrom);
                }
                if (filter.MonthTo > 0)
                {
                    queryFilter = queryFilter.And(c => c.REPORT_MONTH <= filter.MonthTo);
                }
                if (filter.YearFrom > 0)
                {
                    queryFilter = queryFilter.And(c => c.REPORT_YEAR >= filter.YearFrom);
                }
                if (filter.YearTo > 0)
                {
                    queryFilter = queryFilter.And(c => c.REPORT_YEAR <= filter.YearTo);
                }
                if (!string.IsNullOrEmpty(filter.Regional))
                {
                    queryFilter = queryFilter.And(c => c.REGIONAL.ToUpper() == filter.Regional.ToUpper());
                }
                if (!string.IsNullOrEmpty(filter.Function))
                {
                    queryFilter = queryFilter.And(c => c.FUNCTION.ToUpper() == filter.Function.ToUpper());
                }
            }

            return _noVehWtcRepository.Get(queryFilter, null, "").ToList();
        }

        public List<NO_OF_VEHICLE_MAKE_TYPE_REPORT_DATA> GetAllNoVehicleMake(VehicleMakeGetByParamInput filter)
        {
            Expression<Func<NO_OF_VEHICLE_MAKE_TYPE_REPORT_DATA, bool>> queryFilter = PredicateHelper.True<NO_OF_VEHICLE_MAKE_TYPE_REPORT_DATA>();

            if (filter != null)
            {
                if (filter.MonthFrom > 0)
                {
                    queryFilter = queryFilter.And(c => c.REPORT_MONTH >= filter.MonthFrom);
                }
                if (filter.MonthTo > 0)
                {
                    queryFilter = queryFilter.And(c => c.REPORT_MONTH <= filter.MonthTo);
                }
                if (filter.YearFrom > 0)
                {
                    queryFilter = queryFilter.And(c => c.REPORT_YEAR >= filter.YearFrom);
                }
                if (filter.YearTo > 0)
                {
                    queryFilter = queryFilter.And(c => c.REPORT_YEAR <= filter.YearTo);
                }
                if (!string.IsNullOrEmpty(filter.Manufacturer))
                {
                    queryFilter = queryFilter.And(c => c.MANUFACTURER.Contains(filter.Manufacturer));
                }
                if (!string.IsNullOrEmpty(filter.BodyType))
                {
                    queryFilter = queryFilter.And(c => c.BODY_TYPE.Contains(filter.BodyType));
                }
            }

            return _noVehMakeRepository.Get(queryFilter, null, "").ToList();
        }

        public List<ODOMETER_REPORT_DATA> GetAllOdometer(OdometerGetByParamInput filter)
        {
            Expression<Func<ODOMETER_REPORT_DATA, bool>> queryFilter = PredicateHelper.True<ODOMETER_REPORT_DATA>();

            if (filter != null)
            {
                if (filter.MonthFrom > 0)
                {
                    queryFilter = queryFilter.And(c => c.REPORT_MONTH >= filter.MonthFrom);
                }
                if (filter.MonthTo > 0)
                {
                    queryFilter = queryFilter.And(c => c.REPORT_MONTH <= filter.MonthTo);
                }
                if (filter.YearFrom > 0)
                {
                    queryFilter = queryFilter.And(c => c.REPORT_YEAR >= filter.YearFrom);
                }
                if (filter.YearTo > 0)
                {
                    queryFilter = queryFilter.And(c => c.REPORT_YEAR <= filter.YearTo);
                }
                if (!string.IsNullOrEmpty(filter.Region))
                {
                    queryFilter = queryFilter.And(c => c.REGION.ToUpper() == filter.Region.ToUpper());
                }
                if (!string.IsNullOrEmpty(filter.Function))
                {
                    queryFilter = queryFilter.And(c => c.FUNCTION.ToUpper() == filter.Function.ToUpper());
                }
                if (!string.IsNullOrEmpty(filter.VehicleType))
                {
                    queryFilter = queryFilter.And(c => c.VEHCILE_TYPE.ToUpper() == filter.VehicleType.ToUpper());
                }
            }

            return _odometerRepository.Get(queryFilter, null, "").ToList();
        }

        public List<LITER_BY_FUNC_REPORT_DATA> GetAllLiterByFunction(LiterFuncGetByParamInput filter)
        {
            Expression<Func<LITER_BY_FUNC_REPORT_DATA, bool>> queryFilter = PredicateHelper.True<LITER_BY_FUNC_REPORT_DATA>();

            if (filter != null)
            {
                if (filter.MonthFrom > 0)
                {
                    queryFilter = queryFilter.And(c => c.REPORT_MONTH >= filter.MonthFrom);
                }
                if (filter.MonthTo > 0)
                {
                    queryFilter = queryFilter.And(c => c.REPORT_MONTH <= filter.MonthTo);
                }
                if (filter.YearFrom > 0)
                {
                    queryFilter = queryFilter.And(c => c.REPORT_YEAR >= filter.YearFrom);
                }
                if (filter.YearTo > 0)
                {
                    queryFilter = queryFilter.And(c => c.REPORT_YEAR <= filter.YearTo);
                }
                if (!string.IsNullOrEmpty(filter.Region))
                {
                    queryFilter = queryFilter.And(c => c.REGION.ToUpper() == filter.Region.ToUpper());
                }
                if (!string.IsNullOrEmpty(filter.Function))
                {
                    queryFilter = queryFilter.And(c => c.FUNCTION.ToUpper() == filter.Function.ToUpper());
                }
                if (!string.IsNullOrEmpty(filter.VehicleType))
                {
                    queryFilter = queryFilter.And(c => c.VEHICLE_TYPE.ToUpper() == filter.VehicleType.ToUpper());
                }
            }

            return _literByFuncRepository.Get(queryFilter, null, "").ToList();
        }

        public List<FUEL_COST_BY_FUNC_REPORT_DATA> GetAllFuelCostByFunction(FuelCostFuncGetByParamInput filter)
        {
            Expression<Func<FUEL_COST_BY_FUNC_REPORT_DATA, bool>> queryFilter = PredicateHelper.True<FUEL_COST_BY_FUNC_REPORT_DATA>();

            if (filter != null)
            {
                if (filter.MonthFrom > 0)
                {
                    queryFilter = queryFilter.And(c => c.REPORT_MONTH >= filter.MonthFrom);
                }
                if (filter.MonthTo > 0)
                {
                    queryFilter = queryFilter.And(c => c.REPORT_MONTH <= filter.MonthTo);
                }
                if (filter.YearFrom > 0)
                {
                    queryFilter = queryFilter.And(c => c.REPORT_YEAR >= filter.YearFrom);
                }
                if (filter.YearTo > 0)
                {
                    queryFilter = queryFilter.And(c => c.REPORT_YEAR <= filter.YearTo);
                }
                if (!string.IsNullOrEmpty(filter.Region))
                {
                    queryFilter = queryFilter.And(c => c.REGION.ToUpper() == filter.Region.ToUpper());
                }
                if (!string.IsNullOrEmpty(filter.Function))
                {
                    queryFilter = queryFilter.And(c => c.FUNCTION.ToUpper() == filter.Function.ToUpper());
                }
                if (!string.IsNullOrEmpty(filter.VehicleType))
                {
                    queryFilter = queryFilter.And(c => c.VEHICLE_TYPE.ToUpper() == filter.VehicleType.ToUpper());
                }
            }

            return _fuelCostByFuncRepository.Get(queryFilter, null, "").ToList();
        }
    }
}
