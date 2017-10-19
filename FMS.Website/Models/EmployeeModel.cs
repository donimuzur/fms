﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FMS.Website.Models
{
    public class EmployeeModel : BaseModel
    {
        public EmployeeModel()
        {
            Details = new List<EmployeeItem>();
        }

        public List<EmployeeItem> Details { get; set; }
    }

    public class EmployeeItem : BaseModel
    {
        public string EMPLOYEE_ID { get; set; }
        public string FORMAL_NAME { get; set; }
        public string POSITION_TITLE { get; set; }
        public string DIVISON { get; set; }
        public string DIRECTORATE { get; set; }
        public string ADDRESS { get; set; }
        public string CITY { get; set; }
        public string BASETOWN { get; set; }
        public string COMPANY { get; set; }
        public string COST_CENTER { get; set; }
        public int GROUP_LEVEL { get; set; }
        public string EMAIL_ADDRESS { get; set; }
        public Nullable<int> FLEX_POINT { get; set; }
        public string CREATED_BY { get; set; }
        public System.DateTime CREATED_DATE { get; set; }
        public string MODIFIED_BY { get; set; }
        public DateTime? MODIFIED_DATE { get; set; }
        public bool IS_ACTIVE { get; set; }
    }


    public class EmployeeUploadItem : BaseModel
    {
        public string EMPLOYEE_ID { get; set; }
        public string FORMAL_NAME { get; set; }
        public string POSITION_TITLE { get; set; }
        public string DIVISON { get; set; }
        public string DIRECTORATE { get; set; }
        public string ADDRESS { get; set; }
        public string CITY { get; set; }
        public string BASETOWN { get; set; }
        public string COMPANY { get; set; }
        public string COST_CENTER { get; set; }
        public Nullable<int> GROUP_LEVEL { get; set; }
        public string EMAIL_ADDRESS { get; set; }
        public Nullable<int> FLEX_POINT { get; set; }
        public string CREATED_BY { get; set; }
        public System.DateTime CREATED_DATE { get; set; }
        public string MODIFIED_BY { get; set; }
        public DateTime? MODIFIED_DATE { get; set; }
        public bool IS_ACTIVE { get; set; }
    }
}