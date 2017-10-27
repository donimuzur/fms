﻿using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using FMS.BLL.Reason;
using FMS.BusinessObject.Dto;
using FMS.Contract.BLL;
using FMS.Core;
using FMS.Website.Models;
using FMS.Website.Utility;
using SpreadsheetLight;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FMS.Website.Controllers
{
    public class MstReasonController : BaseController
    {
        private IPageBLL _pageBLL;
        private IReasonBLL _rasonBLL;
        private IDocumentTypeBLL _documentTypeBLL;
        private Enums.MenuList _mainMenu;

        // GET: /MstRemark/
        public MstReasonController(IPageBLL PageBll, IReasonBLL ReasonBLL, IDocumentTypeBLL DocTypeBLL) : base(PageBll, Enums.MenuList.MasterVendor)
        {
            _pageBLL = PageBll;
            _rasonBLL = ReasonBLL;
            _documentTypeBLL = DocTypeBLL;
            _mainMenu = Enums.MenuList.MasterData;
        }

        public ActionResult Index()
        {
            var data = _rasonBLL.GetReason();
            var model = new ReasonModel();
            model.Details = Mapper.Map<List<ReasonItem>>(data);
            model.MainMenu = _mainMenu;

            return View(model);
        }

        public ActionResult Create()
        {
            var model = new ReasonItem();
            var list1 = _documentTypeBLL.GetDocumentType();

            model.DocumentTypeList = new SelectList(list1, "MstDocumentTypeId", "DocumentType");
            model.MainMenu = _mainMenu;
           
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(ReasonItem model)
        {
            if (ModelState.IsValid)
            {
                var dto = Mapper.Map<ReasonDto>(model);
                dto.CreatedBy = "Doni";
                dto.CreatedDate = DateTime.Now;
                dto.IsActive = true;
                try
                {
                    _rasonBLL.save(dto);
                }
                catch (Exception ex)
                {
                    var msg = ex.Message;
                }
            }

            return RedirectToAction("Index", "MstReason");
        }
    
        public ActionResult Edit(int MstReasonId)
        {
            var data = _rasonBLL.GetReasonById(MstReasonId);
            var model = Mapper.Map<ReasonItem>(data);


            var list1 = _documentTypeBLL.GetDocumentType();

            model.DocumentTypeList = new SelectList(list1, "MstDocumentTypeId", "DocumentType");
            model.MainMenu = _mainMenu;
         
            return View(model);

        }

        [HttpPost]
        public ActionResult Edit(ReasonItem model)
        {
            if (ModelState.IsValid)
            {
                var dto = Mapper.Map<ReasonDto>(model);
                dto.ModifiedBy = "User";
                dto.ModifiedDate = DateTime.Now;
                try
                {
                    _rasonBLL.save(dto);
                }
                catch (Exception ex)
                {
                    var msg = ex.Message;
                }
            }

            return RedirectToAction("Index", "MstReason");
        }

        public ActionResult Upload()
        {
            var model = new ReasonModel();
            model.MainMenu = _mainMenu;
            return View(model);
        }

        [HttpPost]
        public ActionResult Upload(ReasonModel Model)
        {
            if (ModelState.IsValid)
            {
                foreach (var data in Model.Details)
                {
                    try
                    {
                        data.CreatedDate = DateTime.Now;
                        data.CreatedBy = "doni";
                        data.ModifiedDate = null;
                        data.IsActive = true;
                        if (data.ErrorMessage == "" | data.ErrorMessage == null)
                        {
                            var dto = Mapper.Map<ReasonDto>(data);

                            _rasonBLL.save(dto);
                        }

                        AddMessageInfo(Constans.SubmitMessage.Saved, Enums.MessageInfoType.Success);
                    }
                    catch (Exception exception)
                    {
                        AddMessageInfo(exception.Message, Enums.MessageInfoType.Error);
                        return View(Model);
                    }
                }
            }
            return RedirectToAction("Index", "MstReason");
        }

        [HttpPost]
        public JsonResult UploadFile(HttpPostedFileBase upload)
        {
            var qtyPacked = string.Empty;
            var qty = string.Empty;

            var data = (new ExcelReader()).ReadExcel(upload);
            var model = new List<ReasonItem>();
            if (data != null)
            {
                foreach (var dataRow in data.DataRows)
                {
                    if (dataRow[0] == "")
                    {
                        continue;
                    }
                    var item = new ReasonItem();
                    try
                    {
                        var getdto = _documentTypeBLL.GetDocumentType().Where(x => x.DocumentType == dataRow[0]).FirstOrDefault();
                        item.MstDocumentType = dataRow[0];
                        if (getdto == null)
                        {
                            item.ErrorMessage = "Document " + dataRow[0] + " tidak ada di database";
                        }
                        else if (getdto != null)
                        {
                            item.DocumentType = getdto.MstDocumentTypeId;
                            item.ErrorMessage = "";
                        }

                        item.Reason = dataRow[1].ToString();
                        if (dataRow[2].ToString() == "Yes" | dataRow[2].ToString() == "YES" | dataRow[2].ToString() == "true" | dataRow[2].ToString() == "TRUE" | dataRow[2].ToString() == "1")
                        {
                            item.IsPenalty = true;
                        }
                        else if (dataRow[2].ToString() == "No" | dataRow[2].ToString() == "NO" | dataRow[2].ToString() == "False" | dataRow[2].ToString() == "FALSE" | dataRow[2].ToString() == "0")
                        {
                            item.IsPenalty = false;
                        }
                        
                        model.Add(item);
                    }
                    catch (Exception ex)
                    {
                        var a = ex.Message;
                    }
                }
            }
            return Json(model);
        }


        #region export xls
        public void ExportMasterReason()
        {
            string pathFile = "";

            pathFile = CreateXlsMasterReason();

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

        private string CreateXlsMasterReason()
        {
            //get data
            List<ReasonDto> Reason = _rasonBLL.GetReason();
            var listData = Mapper.Map<List<ReasonItem>>(Reason);

            var slDocument = new SLDocument();

            //title
            slDocument.SetCellValue(1, 1, "Master Reason");
            slDocument.MergeWorksheetCells(1, 1, 1, 8);
            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.SetHorizontalAlignment(HorizontalAlignmentValues.Center);
            valueStyle.Font.Bold = true;
            valueStyle.Font.FontSize = 18;
            slDocument.SetCellStyle(1, 1, valueStyle);

            //create header
            slDocument = CreateHeaderExcelMasterReason(slDocument);

            //create data
            slDocument = CreateDataExcelMasterReason(slDocument, listData);

            var fileName = "Master_Data_Reason" + DateTime.Now.ToString("_yyyyMMddHHmmss") + ".xlsx";
            var path = Path.Combine(Server.MapPath(Constans.UploadPath), fileName);

            slDocument.SaveAs(path);

            return path;

        }

        private SLDocument CreateHeaderExcelMasterReason(SLDocument slDocument)
        {
            int iRow = 2;

            slDocument.SetCellValue(iRow, 1, "Document Type");
            slDocument.SetCellValue(iRow, 2, "Reason");
            slDocument.SetCellValue(iRow, 3, "Penalty");
            slDocument.SetCellValue(iRow, 4, "Created Date");
            slDocument.SetCellValue(iRow, 5, "Created By");
            slDocument.SetCellValue(iRow, 6, "Modified Date");
            slDocument.SetCellValue(iRow, 7, "Modified By");
            slDocument.SetCellValue(iRow, 8, "Status");

            SLStyle headerStyle = slDocument.CreateStyle();
            headerStyle.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            headerStyle.Font.Bold = true;
            headerStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            slDocument.SetCellStyle(iRow, 1, iRow, 8, headerStyle);

            return slDocument;

        }

        private SLDocument CreateDataExcelMasterReason(SLDocument slDocument, List<ReasonItem> listData)
        {
            int iRow = 3; //starting row data

            foreach (var data in listData)
            {
                slDocument.SetCellValue(iRow, 1, data.MstDocumentType);
                slDocument.SetCellValue(iRow, 2, data.Reason);
                slDocument.SetCellValue(iRow, 3, data.IsPenalty == true ? "Yes" : "No");
                slDocument.SetCellValue(iRow, 4, data.CreatedDate.ToString("dd - MM - yyyy hh: mm"));
                slDocument.SetCellValue(iRow, 5, data.CreatedBy);
                slDocument.SetCellValue(iRow, 6, data.ModifiedDate == null ? "" : data.ModifiedDate.Value.ToString("dd - MM - yyyy hh: mm"));
                slDocument.SetCellValue(iRow, 7, data.ModifiedBy);
                if (data.IsActive)
                {
                    slDocument.SetCellValue(iRow, 8, "Active");
                }
                else
                {
                    slDocument.SetCellValue(iRow, 8, "InActive");
                }

                iRow++;
            }

            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;

            slDocument.AutoFitColumn(1, 8);
            slDocument.SetCellStyle(3, 1, iRow - 1, 8, valueStyle);

            return slDocument;
        }

        #endregion

    }
}