﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using AutoMapper;
using FMS.AutoMapperExtensions;
using FMS.BusinessObject;
using FMS.BusinessObject.Dto;
using FMS.BusinessObject.Inputs;
using FMS.Website.Models;

namespace FMS.Website.Code
{
    public partial class FMSWebsiteMapper
    {
        public static void InitializeExecutiveSummary()
        {
            Mapper.CreateMap<NoVehicleDto, NoVehicleData>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ID))
                .ForMember(dest => dest.VehicleType, opt => opt.MapFrom(src => src.VEHICLE_TYPE))
                .ForMember(dest => dest.SupplyMethod, opt => opt.MapFrom(src => src.SUPPLY_METHOD))
                .ForMember(dest => dest.Function, opt => opt.MapFrom(src => src.FUNCTION))
                .ForMember(dest => dest.NoOfVehicle, opt => opt.MapFrom(src => src.NO_OF_VEHICLE))
                .ForMember(dest => dest.ReportMonth, opt => opt.MapFrom(src => src.REPORT_MONTH))
                .ForMember(dest => dest.Month, opt => opt.MapFrom(src => CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(src.REPORT_MONTH.Value)))
                .ForMember(dest => dest.ReportYear, opt => opt.MapFrom(src => src.REPORT_YEAR))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CREATED_DATE))
                ;

            Mapper.CreateMap<NoVehicleData, NoVehicleDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.VEHICLE_TYPE, opt => opt.MapFrom(src => src.VehicleType))
                .ForMember(dest => dest.SUPPLY_METHOD, opt => opt.MapFrom(src => src.SupplyMethod))
                .ForMember(dest => dest.FUNCTION, opt => opt.MapFrom(src => src.Function))
                .ForMember(dest => dest.NO_OF_VEHICLE, opt => opt.MapFrom(src => src.NoOfVehicle))
                .ForMember(dest => dest.REPORT_MONTH, opt => opt.MapFrom(src => src.ReportMonth))
                .ForMember(dest => dest.REPORT_YEAR, opt => opt.MapFrom(src => src.ReportYear))
                .ForMember(dest => dest.CREATED_DATE, opt => opt.MapFrom(src => src.CreatedDate))
                ;

            Mapper.CreateMap<NoVehicleWtcDto, NoVehicleWtcData>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ID))
                .ForMember(dest => dest.Regional, opt => opt.MapFrom(src => src.REGIONAL))
                .ForMember(dest => dest.Function, opt => opt.MapFrom(src => src.FUNCTION))
                .ForMember(dest => dest.NoOfVehicle, opt => opt.MapFrom(src => src.NO_OF_VEHICLE))
                .ForMember(dest => dest.ReportMonth, opt => opt.MapFrom(src => src.REPORT_MONTH))
                .ForMember(dest => dest.Month, opt => opt.MapFrom(src => CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(src.REPORT_MONTH.Value)))
                .ForMember(dest => dest.ReportYear, opt => opt.MapFrom(src => src.REPORT_YEAR))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CREATED_DATE))
                ;

            Mapper.CreateMap<NoVehicleWtcData, NoVehicleWtcDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.REGIONAL, opt => opt.MapFrom(src => src.Regional))
                .ForMember(dest => dest.FUNCTION, opt => opt.MapFrom(src => src.Function))
                .ForMember(dest => dest.NO_OF_VEHICLE, opt => opt.MapFrom(src => src.NoOfVehicle))
                .ForMember(dest => dest.REPORT_MONTH, opt => opt.MapFrom(src => src.ReportMonth))
                .ForMember(dest => dest.REPORT_YEAR, opt => opt.MapFrom(src => src.ReportYear))
                .ForMember(dest => dest.CREATED_DATE, opt => opt.MapFrom(src => src.CreatedDate))
                ;

            Mapper.CreateMap<VehicleSearchView, VehicleGetByParamInput>().IgnoreAllNonExisting();
            Mapper.CreateMap<VehicleSearchViewExport, VehicleGetByParamInput>().IgnoreAllNonExisting();

            Mapper.CreateMap<VehicleSearchViewWtc, VehicleWtcGetByParamInput>().IgnoreAllNonExisting();
            Mapper.CreateMap<VehicleSearchViewExportWtc, VehicleWtcGetByParamInput>().IgnoreAllNonExisting();
        }
    }
}