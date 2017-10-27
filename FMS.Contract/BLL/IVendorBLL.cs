﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.BusinessObject.Dto;
using FMS.BusinessObject;

namespace FMS.Contract.BLL
{
    public interface IVendorBLL
    {
        List<VendorDto> GetVendor();
        VendorDto GetExist(string VendorName);
        VendorDto GetByID(int Id);
        void Save(VendorDto VendorDto);
    }
}