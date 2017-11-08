﻿using FMS.Contract.BLL;
using FMS.Core;
using FMS.Website.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FMS.Website.Controllers
{
    public class TraCtfController : BaseController
    {
        private IEpafBLL _epafBLL;
        private ITraCtfBLL _ctfBLL;
        private IRemarkBLL _remarkBLL;
        private IFleetBLL _fleetBLL;
        private IEmployeeBLL _employeeBLL;
        private IReasonBLL _reasonBLL;
        private Enums.MenuList _mainMenu;
        private IPageBLL _pageBLL;
        public TraCtfController(IPageBLL pageBll, IEpafBLL epafBll, ITraCtfBLL ctfBll, IRemarkBLL RemarkBLL, 
                                IEmployeeBLL  EmployeeBLL, IReasonBLL ReasonBLL, IFleetBLL FleetBLL): base(pageBll, Core.Enums.MenuList.TraCtf)
        {
            _epafBLL = epafBll;
            _ctfBLL = ctfBll;
            _employeeBLL = EmployeeBLL;
            _pageBLL = pageBll;
            _remarkBLL = RemarkBLL;
            _reasonBLL = ReasonBLL;
            _fleetBLL = FleetBLL;
            _mainMenu = Enums.MenuList.Transaction;
        }
        public ActionResult Index()
        {
            var model = new CtfModel();
            //model.TitleForm = "CSF Open Document";
            //model.EpafList = Mapper.Map<List<EpafData>>(data);
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            return View(model);
        }

        public ActionResult CreateFormWtc()
        {
            var model = new CtfItem();
            var EmployeeList = _employeeBLL.GetEmployee().Where(x => x.IS_ACTIVE == true).Select(x => new { x.EMPLOYEE_ID, employee = x.EMPLOYEE_ID + " == " + x.FORMAL_NAME }).Distinct().ToList();
            model.EmployeeIdList = new SelectList(EmployeeList, "EMPLOYEE_ID", "employee");
            var ReasonList = _reasonBLL.GetReason().Where(x => x.IsActive == true && x.DocumentType == 6).ToList();
            model.ReasonList = new SelectList(ReasonList, "MstReasonId", "Reason");
            model.CreatedDate = DateTime.Now;
            model.CreatedDateS = model.CreatedDate.ToString("dd-MMM-yyyy");
            model.DocumentStatus = Enums.DocumentStatus.Draft.GetHashCode();
            model.DocumentStatusS = Enums.DocumentStatus.Draft.ToString();
            var PoliceNumberList = _fleetBLL.GetFleet().Where(x => x.VehicleType == "Benefit" && x.IsActive == true && x.VehicleUsage == "CFM").ToList();
            model.PoliceNumberList = new SelectList(PoliceNumberList, "PoliceNumber", "PoliceNumber");
            model.CreatedBy = CurrentUser.USERNAME;
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            return View(model);
        }
        [HttpPost]
        public ActionResult CreateFormWtc(CtfItem Model)
        {
            return RedirectToAction("Index", "TraCtf");
        }
        public ActionResult CreateFormBenefit()
        {
            var model = new CtfItem();
            var EmployeeList= _employeeBLL.GetEmployee().Where(x => x.IS_ACTIVE == true).Select(x => new { x.EMPLOYEE_ID , employee=x.EMPLOYEE_ID+" == "+ x.FORMAL_NAME }).Distinct().ToList();
            model.EmployeeIdList = new SelectList(EmployeeList, "EMPLOYEE_ID", "employee");
            var ReasonList = _reasonBLL.GetReason().Where(x => x.IsActive == true && x.DocumentType == 6 ).ToList();
            model.ReasonList = new SelectList(ReasonList, "MstReasonId", "Reason");
            model.CreatedDate = DateTime.Now;
            model.CreatedDateS = model.CreatedDate.ToString("dd-MMM-yyyy");
            model.DocumentStatus = Enums.DocumentStatus.Draft.GetHashCode();
            model.DocumentStatusS = Enums.DocumentStatus.Draft.ToString();
            var PoliceNumberList = _fleetBLL.GetFleet().Where(x => x.VehicleType == "Benefit" && x.IsActive == true && x.VehicleUsage=="CFM").ToList();
            model.PoliceNumberList = new SelectList(PoliceNumberList, "PoliceNumber", "PoliceNumber");
            var ExtendList = new Dictionary<bool, string>
                                    { { false, "No" }, { true, "Yes" }};
            model.ExtendList= new SelectList(ExtendList, "Key", "Value");
            var UserDecisionList = new Dictionary<int, string>
                                    { { 1, "Buy" }, { 2, "Refund" }};
            model.UserDecisionList = new SelectList(ExtendList, "Key", "Value");
            model.CreatedBy = CurrentUser.USERNAME;
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            return View(model);
        }

        
        [HttpPost]
        public ActionResult CreateFormBenefit(CtfItem Model)
        {
            return RedirectToAction("Index","TraCtf");
        }
        [HttpPost]
        public JsonResult GetEmployee(string Id)
        {
            var model = _employeeBLL.GetByID(Id);
            return Json(model);
        }
        [HttpPost]
        public JsonResult SetExtendVehicle()
        {
            var model = "";
            return Json(model);
        }
        [HttpPost]
        public JsonResult GetVehicle(string Id)
        {
            var model = _fleetBLL.GetFleet().Where(x=>x.PoliceNumber==Id).FirstOrDefault();
            return Json(model);
        }

        public ActionResult DashboardEpaf()
        {
            var EpafData = _epafBLL.GetEpafByDocType(Enums.DocumentType.CTF).ToList();
            var RemarkList = _remarkBLL.GetRemark().Where(x => x.RoleType == CurrentUser.UserRole.ToString()).ToList();
            
            var model = new CtfModel();
            model.RemarkList = new SelectList(RemarkList, "MstRemarkId", "Remark");
            foreach (var data in EpafData)
            {
                var item = new CtfItem();
                item.EPafData = data;
                model.Details.Add(item);
            }
            model.TitleForm = "CTF Dashboard"; 
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            return View(model);
        }

        public ActionResult Completed()
        {
            var data = _epafBLL.GetEpaf().Where(x => x.DocumentType == 1);
            var model = new CsfIndexModel();
            //model.TitleForm = "CSF Completed Document";
            //model.EpafList = Mapper.Map<List<EpafData>>(data);
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            return View("Index", model);
        }

        public ActionResult CloseEpaf(int MstEpafId, int RemarkId)
        {
            
            if(ModelState.IsValid)
            {
                try
                {
                    _epafBLL.DeactivateEpaf(MstEpafId, RemarkId, CurrentUser.USERNAME);
                }
                catch (Exception)
                {
                    
                }
                
            }
            return RedirectToAction("DashboardEpaf", "TraCtf");
        }

    }
}
