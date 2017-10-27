﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FMS.Website.Models
{
    public class PenaltyModel : BaseModel
    {
        public PenaltyModel()
        {
            Details = new List<PenaltyItem>();
        }

        public List<PenaltyItem> Details { get; set; }
    }

    public class PenaltyItem : BaseModel
    {
        public int MstPenaltyId { get; set; }
        public string Manufacturer { get; set; }
        public string Models { get; set; }
        public string Series { get; set; }
        public int Year { get; set; }
        public int MonthStart { get; set; }
        public int MonthEnd { get; set; }
        public string VehicleType { get; set; }
        public int Penalty { get; set; }
        public bool Restitution { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsActive { get; set; }
    }
}