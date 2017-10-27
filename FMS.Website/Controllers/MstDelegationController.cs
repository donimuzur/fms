﻿using AutoMapper;
using FMS.BusinessObject;
using FMS.Contract.BLL;
using FMS.BusinessObject.Dto;
using FMS.Core;
using FMS.Website.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using SpreadsheetLight;
using DocumentFormat.OpenXml.Spreadsheet;
using FMS.Website.Utility;

namespace FMS.Website.Controllers
{
    public class MstDelegationController : BaseController
    {
        private IDelegationBLL _DelegationBLL;
        private Enums.MenuList _mainMenu;
        private IEmployeeBLL _employeeBLL;

        public MstDelegationController(IPageBLL PageBll, IDelegationBLL DelegationBLL, IEmployeeBLL EmployeeBLL) : base(PageBll, Enums.MenuList.MasterDelegation)
        {
            _DelegationBLL = DelegationBLL;
            _employeeBLL = EmployeeBLL;
            _mainMenu = Enums.MenuList.MasterData;
        }

        //
        // GET: /MstDelegation/

        public ActionResult Index()
        {
            var data = _DelegationBLL.GetDelegation();
            var model = new DelegationModel();
            model.Details = Mapper.Map<List<DelegationItem>>(data);
            model.MainMenu = _mainMenu;
            return View(model);
        }

        public ActionResult Create()
        {
            var model = new DelegationItem();
            var list = _employeeBLL.GetEmployee().Select(x => new { x.EMPLOYEE_ID, x.FORMAL_NAME }).ToList().OrderBy(x => x.FORMAL_NAME);
            model.EmployeeListFrom = new SelectList(list, "EMPLOYEE_ID", "FORMAL_NAME");
            model.EmployeeListTo = new SelectList(list, "EMPLOYEE_ID", "FORMAL_NAME");
            model.MainMenu = _mainMenu;
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(DelegationItem model, HttpPostedFileBase Attachment)
        {
            if (ModelState.IsValid)
            {
                var data = Mapper.Map<DelegationDto>(model);
                data.CreatedBy = "User";
                data.CreatedDate = DateTime.Today;
                data.ModifiedDate = null;
                if (Attachment != null)
                {
                    string filename = System.IO.Path.GetFileName(Attachment.FileName);
                    Attachment.SaveAs(Server.MapPath("~/files_upload/" + filename));
                    string filepathtosave = "files_upload" + filename;
                    data.Attachment = filename;
                }
                _DelegationBLL.Save(data);
            }
            return RedirectToAction("Index", "MstDelegation");
        }

        public ActionResult Edit(int MstDelegationId)
        {
            var data = _DelegationBLL.GetDelegationById(MstDelegationId);
            var model = new DelegationItem();
            model = Mapper.Map<DelegationItem>(data);
            var list = _employeeBLL.GetEmployee().Select(x => new {x.EMPLOYEE_ID, x.FORMAL_NAME }).ToList().OrderBy(x => x.FORMAL_NAME);
            model.EmployeeListFrom = new SelectList(list, "EMPLOYEE_ID", "FORMAL_NAME");
            model.EmployeeListTo = new SelectList(list, "EMPLOYEE_ID", "FORMAL_NAME");
            model.MainMenu = _mainMenu;
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(DelegationItem model, HttpPostedFileBase Attachment)
        {
            if (ModelState.IsValid)
            {
                var data = Mapper.Map<DelegationDto>(model);
                data.ModifiedDate = DateTime.Now;
                data.ModifiedBy = "User";
                if (Attachment != null)
                {
                    string filename = System.IO.Path.GetFileName(Attachment.FileName);
                    Attachment.SaveAs(Server.MapPath("~/files_upload/" + filename));
                    string filepathtosave = "files_upload" + filename;
                    data.Attachment = filename;
                }
                _DelegationBLL.Save(data);
            }
            return RedirectToAction("Index", "MstDelegation");
        }

        #region export xls
        public void ExportMasterDelegation()
        {
            string pathFile = "";

            pathFile = CreateXlsMasterDelegation();

            var newFile = new FileInfo(pathFile);

            var fileName = Path.GetFileName(pathFile);

            string attachment = string.Format("attachment; filename={0}", fileName);
            Response.Clear();
            Response.AddHeader("content-disposition", attachment);
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.WriteFile(newFile.FullName);
            Response.Flush();
            newFile.Delete();
            Response.End();
        }
        private string CreateXlsMasterDelegation()
        {
            //get data
            List<DelegationDto> penalty = _DelegationBLL.GetDelegation();
            var listData = Mapper.Map<List<DelegationItem>>(penalty);

            var slDocument = new SLDocument();

            //title
            slDocument.SetCellValue(1, 1, "Master Delegation");
            slDocument.MergeWorksheetCells(1, 1, 1, 11);
            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.SetHorizontalAlignment(HorizontalAlignmentValues.Center);
            valueStyle.Font.Bold = true;
            valueStyle.Font.FontSize = 18;
            slDocument.SetCellStyle(1, 1, valueStyle);

            //create header
            slDocument = CreateHeaderExcelMasterDelegation(slDocument);

            //create data
            slDocument = CreateDataExcelMasterDelegation(slDocument, listData);

            var fileName = "Master Data Delegation" + DateTime.Now.ToString(" yyyyMMddHHmmss") + ".xlsx";
            var path = Path.Combine(Server.MapPath(Constans.UploadPath), fileName);

            slDocument.SaveAs(path);

            return path;

        }

        private SLDocument CreateHeaderExcelMasterDelegation(SLDocument slDocument)
        {
            int iRow = 2;

            slDocument.SetCellValue(iRow, 1, "Mst Delegation ID");
            slDocument.SetCellValue(iRow, 2, "Employee From");
            slDocument.SetCellValue(iRow, 3, "Employee To");
            slDocument.SetCellValue(iRow, 4, "Date From");
            slDocument.SetCellValue(iRow, 5, "Date To");
            slDocument.SetCellValue(iRow, 6, "Is Complaint Form");
            slDocument.SetCellValue(iRow, 7, "Created By");
            slDocument.SetCellValue(iRow, 8, "Created Date");
            slDocument.SetCellValue(iRow, 9, "Modified By");
            slDocument.SetCellValue(iRow, 10, "Modified Date");
            slDocument.SetCellValue(iRow, 11, "Status");

            SLStyle headerStyle = slDocument.CreateStyle();
            headerStyle.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            headerStyle.Font.Bold = true;
            headerStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            slDocument.SetCellStyle(iRow, 1, iRow, 11, headerStyle);

            return slDocument;

        }

        private SLDocument CreateDataExcelMasterDelegation(SLDocument slDocument, List<DelegationItem> listData)
        {
            int iRow = 3; //starting row data

            foreach (var data in listData)
            {
                slDocument.SetCellValue(iRow, 1, data.MstDelegationID);
                slDocument.SetCellValue(iRow, 2, data.EmployeeFrom);
                slDocument.SetCellValue(iRow, 3, data.EmployeeTo);
                slDocument.SetCellValue(iRow, 4, data.DateFrom.ToString("dd/MM/yyyy"));
                slDocument.SetCellValue(iRow, 5, data.DateTo.ToString("dd/MM/yyyy"));
                slDocument.SetCellValue(iRow, 6, data.IsComplaintFrom);
                slDocument.SetCellValue(iRow, 7, data.CreatedBy);
                slDocument.SetCellValue(iRow, 8, data.CreatedDate.ToString("dd/MM/yyyy hh:mm"));
                slDocument.SetCellValue(iRow, 9, data.ModifiedBy);
                slDocument.SetCellValue(iRow, 10, data.ModifiedDate.Value.ToString("dd/MM/yyyy hh:mm"));
                if (data.IsActive)
                {
                    slDocument.SetCellValue(iRow, 11, "Active");
                }
                else
                {
                    slDocument.SetCellValue(iRow, 11, "InActive");
                }

                iRow++;
            }

            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;

            slDocument.AutoFitColumn(1, 11);
            slDocument.SetCellStyle(3, 1, iRow - 1, 11, valueStyle);

            return slDocument;
        }

        #endregion

        #region Upload
        public ActionResult Upload()
        {
            var model = new DelegationModel();
            model.MainMenu = _mainMenu;
            return View(model);
        }


        [HttpPost]
        public ActionResult Upload(DelegationModel Model)
        {
            if (ModelState.IsValid)
            {
                foreach (DelegationItem data in Model.Details)
                {
                    try
                    {
                        data.CreatedDate = DateTime.Now;
                        data.CreatedBy = "User";
                        data.ModifiedDate = null;
                        data.IsActive = true;

                        var dto = Mapper.Map<DelegationDto>(data);
                        _DelegationBLL.Save(dto);
                        AddMessageInfo(Constans.SubmitMessage.Saved, Enums.MessageInfoType.Success);
                    }
                    catch (Exception exception)
                    {
                        AddMessageInfo(exception.Message, Enums.MessageInfoType.Error);
                        return View(Model);
                    }
                }
            }
            return RedirectToAction("Index", "MstDelegation");
        }

        [HttpPost]
        public JsonResult UploadFile(HttpPostedFileBase upload)
        {
            var qtyPacked = string.Empty;
            var qty = string.Empty;

            var data = (new ExcelReader()).ReadExcel(upload);
            var model = new List<DelegationUploadItem>();
            if (data != null)
            {
                foreach (var dataRow in data.DataRows)
                {
                    if (dataRow[0] == "")
                    {
                        continue;
                    }
                    var item = new DelegationUploadItem();
                    item.EmployeeFrom = dataRow[0].ToString();
                    item.EmployeeTo = dataRow[1].ToString();
                    item.DateFrom = dataRow[2].ToString();
                    item.DateTo = dataRow[3].ToString();
                    item.IsComplaintForm = dataRow[4].ToString();
                    model.Add(item);
                }
            }
            return Json(model);
        }
        #endregion
    }
}