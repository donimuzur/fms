﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.BusinessObject.Dto
{
    public class TraCsfDto
    {
        public long TRA_CSF_ID { get; set; }
        public string DOCUMENT_NUMBER { get; set; }
        public int DOCUMENT_STATUS { get; set; }
        public Nullable<long> EPAF_ID { get; set; }
        public int REASON_ID { get; set; }
        public System.DateTime EFFECTIVE_DATE { get; set; }
        public string EMPLOYEE_ID { get; set; }
        public string EMPLOYEE_NAME { get; set; }
        public string COST_CENTER { get; set; }
        public Nullable<int> GROUP_LEVEL { get; set; }
        public string VEHICLE_TYPE { get; set; }
        public string VEHICLE_CATEGORY { get; set; }
        public string VEHICLE_USAGE { get; set; }
        public string LOCATION_CITY { get; set; }
        public string LOCATION_ADDRESS { get; set; }
        public string POLICE_NUMBER { get; set; }
        public string MANUFACTURER { get; set; }
        public string MODEL { get; set; }
        public string SERIES { get; set; }
        public string BODY_TYPE { get; set; }
        public string COLOUR { get; set; }
        public Nullable<System.DateTime> START_PERIOD { get; set; }
        public Nullable<System.DateTime> END_PERIOD { get; set; }
        public Nullable<int> VENDOR { get; set; }
        public Nullable<System.DateTime> EXPECTED_DATE { get; set; }
        public Nullable<System.DateTime> END_RENT_DATE { get; set; }
        public string SUPPLY_METHOD { get; set; }
        public Nullable<bool> IS_PROJECT { get; set; }
        public string PROJECT_NAME { get; set; }
        public string PO_NUMBER { get; set; }
        public Nullable<int> REMARK_ID { get; set; }
        public string CREATED_BY { get; set; }
        public System.DateTime CREATED_DATE { get; set; }
        public string MODIFIED_BY { get; set; }
        public Nullable<System.DateTime> MODIFIED_DATE { get; set; }
        public bool IS_ACTIVE { get; set; }
        public Nullable<System.DateTime> START_RENT_DATE { get; set; }
    }
}
