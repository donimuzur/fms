﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.Core;
using FMS.Core.Exceptions;
using FMS.Contract.BLL;
using FMS.Contract.Service;
using FMS.BusinessObject;
using FMS.BusinessObject.Business;
using FMS.BusinessObject.Dto;
using FMS.BusinessObject.Inputs;
using FMS.Contract;
using FMS.DAL.Services;
using FMS.Utils;
using AutoMapper;
using System.Data.SqlClient;
using System.Data.Entity.Core.EntityClient;

namespace FMS.BLL.Temporary
{
    public class TemporaryBLL : ITraTemporaryBLL
    {
        private ITemporaryService _TemporaryService;
        private IUnitOfWork _uow;

        private ISettingService _settingService;
        private IDocumentNumberService _docNumberService;
        private IWorkflowHistoryService _workflowService;
        private IMessageService _messageService;
        private IEmployeeService _employeeService;
        private IRemarkService _remarkService;
        private ILocationMappingService _locationMappingService;
        private IFleetService _fleetService;
        private IPriceListService _priceListService;

        public TemporaryBLL(IUnitOfWork uow)
        {
            _uow = uow;
            _TemporaryService = new TemporaryService(_uow);

            _settingService = new SettingService(_uow);
            _docNumberService = new DocumentNumberService(_uow);
            _workflowService = new WorkflowHistoryService(_uow);
            _messageService = new MessageService(_uow);
            _employeeService = new EmployeeService(_uow);
            _remarkService = new RemarkService(_uow);
            _locationMappingService = new LocationMappingService(_uow);
            _fleetService = new FleetService(_uow);
            _priceListService = new PriceListService(_uow);
        }

        public List<TemporaryDto> GetTemporary(Login userLogin, bool isCompleted)
        {
            var settingData = _settingService.GetSetting().Where(x => x.SETTING_GROUP == EnumHelper.GetDescription(Enums.SettingGroup.VehicleType));
            var benefitType = settingData.Where(x => x.SETTING_NAME.ToUpper() == "BENEFIT").FirstOrDefault().MST_SETTING_ID.ToString();
            var wtcType = settingData.Where(x => x.SETTING_NAME.ToUpper() == "WTC").FirstOrDefault().MST_SETTING_ID.ToString();

            var locationMapping = _locationMappingService.GetLocationMapping().Where(x => x.IS_ACTIVE).OrderByDescending(x => x.VALIDITY_FROM).ToList();

            var data = _TemporaryService.GetTemp(userLogin, isCompleted, benefitType, wtcType);
            var retData = Mapper.Map<List<TemporaryDto>>(data);

            foreach (var item in retData)
            {
                var region = locationMapping.Where(x => x.LOCATION.ToUpper() == item.LOCATION_CITY.ToUpper()).FirstOrDefault();

                item.REGIONAL = region == null ? string.Empty : region.REGION;

                item.VEHICLE_TYPE_NAME = "BENEFIT";

                if (item.VEHICLE_TYPE == wtcType)
                {
                    item.VEHICLE_TYPE_NAME = "WTC";
                }
            }

            return retData;
        }

        public List<TemporaryDto> GetTempPersonal(Login userLogin)
        {
            var data = _TemporaryService.GetAllTemp().Where(x => (x.EMPLOYEE_ID == userLogin.EMPLOYEE_ID && x.DOCUMENT_STATUS != Enums.DocumentStatus.Draft)
                                                                || x.CREATED_BY == userLogin.USER_ID
                                                                || x.APPROVED_FLEET == userLogin.USER_ID).ToList();
            var retData = Mapper.Map<List<TemporaryDto>>(data);

            var settingData = _settingService.GetSetting().Where(x => x.SETTING_GROUP == EnumHelper.GetDescription(Enums.SettingGroup.VehicleType));
            var benefitType = settingData.Where(x => x.SETTING_NAME.ToUpper() == "BENEFIT").FirstOrDefault().MST_SETTING_ID.ToString();
            var wtcType = settingData.Where(x => x.SETTING_NAME.ToUpper() == "WTC").FirstOrDefault().MST_SETTING_ID.ToString();

            var locationMapping = _locationMappingService.GetLocationMapping().Where(x => x.IS_ACTIVE).OrderByDescending(x => x.VALIDITY_FROM).ToList();

            foreach (var item in retData)
            {
                var region = locationMapping.Where(x => x.LOCATION.ToUpper() == item.LOCATION_CITY.ToUpper()).FirstOrDefault();

                item.REGIONAL = region == null ? string.Empty : region.REGION;

                item.VEHICLE_TYPE_NAME = "BENEFIT";
                if (item.VEHICLE_TYPE == wtcType)
                {
                    item.VEHICLE_TYPE_NAME = "WTC";
                }
            }

            return retData;
        }

        public TemporaryDto Save(TemporaryDto item, Login userLogin)
        {
            TRA_TEMPORARY model;
            if (item == null)
            {
                throw new Exception("Invalid Data Entry");
            }

            try
            {
                bool changed = false;

                if (item.TRA_TEMPORARY_ID > 0)
                {
                    //update
                    model = _TemporaryService.GetTemporaryById(item.TRA_TEMPORARY_ID);

                    if (model == null)
                        throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

                    Mapper.Map<TemporaryDto, TRA_TEMPORARY>(item, model);

                    changed = true;
                }
                else
                {
                    var inputDoc = new GenerateDocNumberInput();
                    inputDoc.Month = DateTime.Now.Month;
                    inputDoc.Year = DateTime.Now.Year;
                    inputDoc.DocType = (int)Enums.DocumentType.TMP;

                    item.DOCUMENT_NUMBER_TEMP = _docNumberService.GenerateNumber(inputDoc);
                    item.IS_ACTIVE = true;
                    item.EMPLOYEE_ID_CREATOR = userLogin.EMPLOYEE_ID;

                    model = Mapper.Map<TRA_TEMPORARY>(item);
                }

                _TemporaryService.saveTemporary(model, userLogin);
                _uow.SaveChanges();

                //set workflow history
                var input = new TempWorkflowDocumentInput()
                {
                    DocumentId = model.TRA_TEMPORARY_ID,
                    ActionType = Enums.ActionType.Modified,
                    UserId = userLogin.USER_ID
                };

                if (changed)
                {
                    AddWorkflowHistory(input);
                }
                _uow.SaveChanges();
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return Mapper.Map<TemporaryDto>(model);
        }

        private void AddWorkflowHistory(TempWorkflowDocumentInput input)
        {
            var dbData = Mapper.Map<WorkflowHistoryDto>(input);

            dbData.ACTION_DATE = DateTime.Now;
            dbData.MODUL_ID = Enums.MenuList.TraTmp;
            dbData.REMARK_ID = null;

            _workflowService.Save(dbData);

        }

        public void TempWorkflow(TempWorkflowDocumentInput input)
        {
            var isNeedSendNotif = true;
            switch (input.ActionType)
            {
                case Enums.ActionType.Created:
                    CreateDocument(input);
                    isNeedSendNotif = false;
                    break;
                case Enums.ActionType.Submit:
                    SubmitDocument(input);
                    break;
                case Enums.ActionType.Approve:
                    ApproveDocument(input);
                    break;
                case Enums.ActionType.Reject:
                    RejectDocument(input);
                    break;
                case Enums.ActionType.Completed:
                    CompleteDocument(input);
                    break;
            }

            //todo sent mail
            if (isNeedSendNotif) SendEmailWorkflow(input);

            _uow.SaveChanges();
        }

        private void CompleteDocument(TempWorkflowDocumentInput input)
        {
            var dbData = _TemporaryService.GetTemporaryById(input.DocumentId);

            dbData.MODIFIED_BY = input.UserId;
            dbData.MODIFIED_DATE = DateTime.Now;

            if (dbData == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            dbData.DOCUMENT_STATUS = Enums.DocumentStatus.Completed;
            input.DocumentNumber = dbData.DOCUMENT_NUMBER;

            AddWorkflowHistory(input);

        }

        private void CreateDocument(TempWorkflowDocumentInput input)
        {
            var dbData = _TemporaryService.GetTemporaryById(input.DocumentId);

            if (dbData == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            input.DocumentNumber = dbData.DOCUMENT_NUMBER;

            AddWorkflowHistory(input);
        }

        private void SubmitDocument(TempWorkflowDocumentInput input)
        {
            var dbData = _TemporaryService.GetTemporaryById(input.DocumentId);

            dbData.MODIFIED_BY = input.UserId;
            dbData.MODIFIED_DATE = DateTime.Now;

            if (dbData == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            var vehTypeBenefit = _settingService.GetSetting().Where(x => x.SETTING_GROUP == "VEHICLE_TYPE" && x.SETTING_NAME == "BENEFIT").FirstOrDefault().MST_SETTING_ID;

            var isBenefit = dbData.VEHICLE_TYPE == vehTypeBenefit.ToString() ? true : false;

            dbData.DOCUMENT_STATUS = Enums.DocumentStatus.WaitingFleetApproval;

            if (!isBenefit)
            {
                dbData.DOCUMENT_STATUS = Enums.DocumentStatus.InProgress;
            }

            input.DocumentNumber = dbData.DOCUMENT_NUMBER;

            AddWorkflowHistory(input);

        }

        private void ApproveDocument(TempWorkflowDocumentInput input)
        {
            var dbData = _TemporaryService.GetTemporaryById(input.DocumentId);

            dbData.MODIFIED_BY = input.UserId;
            dbData.MODIFIED_DATE = DateTime.Now;

            if (dbData == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            dbData.DOCUMENT_STATUS = Enums.DocumentStatus.InProgress;
            dbData.APPROVED_FLEET = input.UserId;
            dbData.APPROVED_FLEET_DATE = DateTime.Now;
            dbData.EMPLOYEE_ID_FLEET_APPROVAL = input.EmployeeId;

            _uow.SaveChanges();

            input.DocumentNumber = dbData.DOCUMENT_NUMBER;

            AddWorkflowHistory(input);

        }

        private void RejectDocument(TempWorkflowDocumentInput input)
        {
            var dbData = _TemporaryService.GetTemporaryById(input.DocumentId);

            dbData.MODIFIED_BY = input.UserId;
            dbData.MODIFIED_DATE = DateTime.Now;

            if (dbData == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            dbData.DOCUMENT_STATUS = Enums.DocumentStatus.Draft;

            input.DocumentNumber = dbData.DOCUMENT_NUMBER;

            AddWorkflowHistory(input);

        }

        private void SendEmailWorkflow(TempWorkflowDocumentInput input)
        {
            var tempData = Mapper.Map<TemporaryDto>(_TemporaryService.GetTemporaryById(input.DocumentId));

            var mailProcess = ProsesMailNotificationBody(tempData, input);

            //distinct double To email
            List<string> ListTo = mailProcess.To.Distinct().ToList();

            if (mailProcess.IsCCExist)
                //Send email with CC
                _messageService.SendEmailToListWithCC(ListTo, mailProcess.CC, mailProcess.Subject, mailProcess.Body, true);
            else
                _messageService.SendEmailToList(ListTo, mailProcess.Subject, mailProcess.Body, true);

        }

        private class TempMailNotification
        {
            public TempMailNotification()
            {
                To = new List<string>();
                CC = new List<string>();
                IsCCExist = false;
            }
            public string Subject { get; set; }
            public string Body { get; set; }
            public List<string> To { get; set; }
            public List<string> CC { get; set; }
            public bool IsCCExist { get; set; }
        }

        private TempMailNotification ProsesMailNotificationBody(TemporaryDto tempData, TempWorkflowDocumentInput input)
        {
            var bodyMail = new StringBuilder();
            var rc = new TempMailNotification();

            var vehTypeBenefit = _settingService.GetSetting().Where(x => x.SETTING_GROUP == "VEHICLE_TYPE" && x.SETTING_NAME == "BENEFIT").FirstOrDefault().MST_SETTING_ID;

            var isBenefit = tempData.VEHICLE_TYPE == vehTypeBenefit.ToString() ? true : false;

            var webRootUrl = ConfigurationManager.AppSettings["WebRootUrl"];
            var typeEnv = ConfigurationManager.AppSettings["Environment"];
            var employeeData = _employeeService.GetEmployeeById(tempData.EMPLOYEE_ID);
            var creatorData = _employeeService.GetEmployeeById(tempData.EMPLOYEE_ID_CREATOR);
            var fleetApprovalData = _employeeService.GetEmployeeById(tempData.EMPLOYEE_ID_FLEET_APPROVAL);

            var employeeDataEmail = employeeData == null ? string.Empty : employeeData.EMAIL_ADDRESS;
            var creatorDataEmail = creatorData == null ? string.Empty : creatorData.EMAIL_ADDRESS;
            var fleetApprovalDataEmail = fleetApprovalData == null ? string.Empty : fleetApprovalData.EMAIL_ADDRESS;

            var employeeDataName = employeeData == null ? string.Empty : employeeData.FORMAL_NAME;
            var creatorDataName = creatorData == null ? string.Empty : creatorData.FORMAL_NAME;
            var fleetApprovalDataName = fleetApprovalData == null ? string.Empty : fleetApprovalData.FORMAL_NAME;

            var hrList = string.Empty;
            var fleetList = string.Empty;

            var hrEmailList = new List<string>();
            var fleetEmailList = new List<string>();

            var hrRole = _settingService.GetSetting().Where(x => x.SETTING_GROUP == EnumHelper.GetDescription(Enums.SettingGroup.UserRole)
                                                                && x.SETTING_VALUE.Contains("HR")).FirstOrDefault().SETTING_VALUE;
            var fleetRole = _settingService.GetSetting().Where(x => x.SETTING_GROUP == EnumHelper.GetDescription(Enums.SettingGroup.UserRole)
                                                                && x.SETTING_VALUE.Contains("FLEET")).FirstOrDefault().SETTING_VALUE;

            var hrQuery = "SELECT 'PMI\\' + sAMAccountName AS sAMAccountName FROM OPENQUERY(ADSI, 'SELECT employeeID, sAMAccountName, displayName, name, givenName, whenCreated, whenChanged, SN, manager, distinguishedName, info FROM ''LDAP://DC=PMINTL,DC=NET'' WHERE memberOf = ''CN = " + hrRole + ", OU = ID, OU = Security, OU = IMDL Managed Groups, OU = Global, OU = Users & Workstations, DC = PMINTL, DC = NET''') ";
            var fleetQuery = "SELECT 'PMI\\' + sAMAccountName AS sAMAccountName FROM OPENQUERY(ADSI, 'SELECT employeeID, sAMAccountName, displayName, name, givenName, whenCreated, whenChanged, SN, manager, distinguishedName, info FROM ''LDAP://DC=PMINTL,DC=NET'' WHERE memberOf = ''CN = " + fleetRole + ", OU = ID, OU = Security, OU = IMDL Managed Groups, OU = Global, OU = Users & Workstations, DC = PMINTL, DC = NET''') ";

            if (typeEnv == "VTI")
            {
                hrQuery = "SELECT 'PMI\\' + LOGIN AS LOGIN FROM LOGIN_FOR_VTI WHERE AD_GROUP = '" + hrRole + "'";
                fleetQuery = "SELECT 'PMI\\' + LOGIN AS LOGIN FROM LOGIN_FOR_VTI WHERE AD_GROUP = '" + fleetRole + "'";
            }

            EntityConnectionStringBuilder e = new EntityConnectionStringBuilder(ConfigurationManager.ConnectionStrings["FMSEntities"].ConnectionString);
            string connectionString = e.ProviderConnectionString;
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            SqlCommand query = new SqlCommand(hrQuery, con);
            SqlDataReader reader = query.ExecuteReader();
            while (reader.Read())
            {
                var hrLogin = "'" + reader[0].ToString() + "',";
                hrList += hrLogin;
            }

            hrList = hrList.TrimEnd(',');

            query = new SqlCommand(fleetQuery, con);
            reader = query.ExecuteReader();
            while (reader.Read())
            {
                var fleetLogin = "'" + reader[0].ToString() + "',";
                fleetList += fleetLogin;
            }

            fleetList = fleetList.TrimEnd(',');

            var hrQueryEmail = "SELECT EMAIL FROM [HMSSQLFWOPRD.ID.PMI\\PRD03].[db_Intranet_HRDV2].[dbo].[tbl_ADSI_User] WHERE FULL_NAME IN (" + hrList + ")";
            var fleetQueryEmail = "SELECT EMAIL FROM [HMSSQLFWOPRD.ID.PMI\\PRD03].[db_Intranet_HRDV2].[dbo].[tbl_ADSI_User] WHERE FULL_NAME IN (" + fleetList + ")";

            if (typeEnv == "VTI")
            {
                hrQueryEmail = "SELECT EMAIL FROM EMAIL_FOR_VTI WHERE FULL_NAME IN (" + hrList + ")";
                fleetQueryEmail = "SELECT EMAIL FROM EMAIL_FOR_VTI WHERE FULL_NAME IN (" + fleetList + ")";
            }

            query = new SqlCommand(hrQueryEmail, con);
            reader = query.ExecuteReader();
            while (reader.Read())
            {
                hrEmailList.Add(reader[0].ToString());
            }

            query = new SqlCommand(fleetQueryEmail, con);
            reader = query.ExecuteReader();
            while (reader.Read())
            {
                fleetEmailList.Add(reader[0].ToString());
            }

            reader.Close();
            con.Close();

            switch (input.ActionType)
            {
                case Enums.ActionType.Submit:
                    //if submit from FLEET for wtc
                    if (tempData.CREATED_BY == input.UserId && !isBenefit)
                    {
                        rc.Subject = tempData.DOCUMENT_NUMBER_TEMP + " - Temporary Car Request";

                        bodyMail.Append("Dear " + tempData.EMPLOYEE_NAME + ",<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("new temporary car has been recorded as " + tempData.DOCUMENT_NUMBER_TEMP + "<br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Click <a href='" + webRootUrl + "/TraTemporary/Detail/" + tempData.TRA_TEMPORARY_ID + "?isPersonalDashboard=True" + "'>HERE</a> to monitor your request<br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("For any assistance please contact " + creatorDataName + "<br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Thanks<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Regards,<br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Fleet Team");
                        bodyMail.AppendLine();

                        foreach (var item in fleetEmailList)
                        {
                            rc.To.Add(item);
                        }

                        rc.CC.Add(employeeDataEmail);
                    }
                    //if submit from HR for benefit
                    if (tempData.CREATED_BY == input.UserId && isBenefit)
                    {
                        rc.Subject = tempData.DOCUMENT_NUMBER_TEMP + " - Temporary Car Request";

                        bodyMail.Append("Dear Fleet Team,<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("new temporary car has been recorded as " + tempData.DOCUMENT_NUMBER_TEMP + "<br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Please submit detail vehicle information <a href='" + webRootUrl + "/TraTemporary/Edit/" + tempData.TRA_TEMPORARY_ID + "?isPersonalDashboard=False" + "'>HERE</a><br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("For any assistance please contact " + creatorDataName + "<br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Thanks<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Regards,<br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("HR Team");
                        bodyMail.AppendLine();

                        foreach (var item in fleetEmailList)
                        {
                            rc.To.Add(item);
                        }

                        foreach (var item in hrEmailList)
                        {
                            rc.To.Add(item);
                        }

                        rc.CC.Add(employeeDataEmail);
                    }
                    rc.IsCCExist = true;
                    break;
                case Enums.ActionType.Approve:
                    //if Fleet Approve for benefit
                    if (input.UserRole == Enums.UserRole.Fleet && isBenefit)
                    {
                        rc.Subject = tempData.DOCUMENT_NUMBER_TEMP + " - Temporary Car Request";

                        bodyMail.Append("Dear " + creatorDataName + ",<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Your temporary car request " + tempData.DOCUMENT_NUMBER_TEMP + " has been approved by " + fleetApprovalDataName + "<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Click <a href='" + webRootUrl + "/TraTemporary/Detail/" + tempData.TRA_TEMPORARY_ID + "?isPersonalDashboard=False" + "'>HERE</a> to monitor your request<br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Thanks<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Regards,<br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Fleet Team");
                        bodyMail.AppendLine();

                        rc.To.Add(creatorDataEmail);

                        foreach (var item in fleetEmailList)
                        {
                            rc.CC.Add(item);
                        }

                        rc.CC.Add(employeeDataEmail);
                    }
                    rc.IsCCExist = true;
                    break;
                case Enums.ActionType.Reject:
                    //if Fleet Reject Benefit
                    if (input.UserRole == Enums.UserRole.Fleet && isBenefit)
                    {
                        rc.Subject = tempData.DOCUMENT_NUMBER_TEMP + " - Temporary Car Request";

                        bodyMail.Append("Dear " + creatorDataName + ",<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Your temporary car request " + tempData.DOCUMENT_NUMBER_TEMP + " has been rejected by " + fleetApprovalDataName + " for below reason : " + _remarkService.GetRemarkById(input.Comment.Value).REMARK + "<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Please revised and re-submit your request <a href='" + webRootUrl + "/TraTemporary/Edit/" + tempData.TRA_TEMPORARY_ID + "?isPersonalDashboard=False" + "'>HERE</a><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Thanks<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Regards,<br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Fleet Team");
                        bodyMail.AppendLine();

                        rc.To.Add(creatorDataEmail);

                        foreach (var item in fleetEmailList)
                        {
                            rc.CC.Add(item);
                        }

                        rc.CC.Add(employeeDataEmail);
                    }
                    rc.IsCCExist = true;
                    break;
                case Enums.ActionType.Completed:
                    rc.Subject = tempData.DOCUMENT_NUMBER_TEMP + " - Completed Document";

                    bodyMail.Append("Dear " + creatorDataName + ",<br /><br />");
                    bodyMail.AppendLine();
                    bodyMail.Append("Your temporary car request " + tempData.DOCUMENT_NUMBER_TEMP + " has been completed by system<br /><br />");
                    bodyMail.AppendLine();
                    bodyMail.Append("Click <a href='" + webRootUrl + "/TraTemporary/Detail/" + tempData.TRA_TEMPORARY_ID + "?isPersonalDashboard=True" + "'>HERE</a> to monitor your request<br />");
                    bodyMail.AppendLine();
                    bodyMail.Append("Thanks<br /><br />");
                    bodyMail.AppendLine();
                    bodyMail.Append("Regards,<br />");
                    bodyMail.AppendLine();
                    bodyMail.Append("Fleet Team");
                    bodyMail.AppendLine();

                    rc.To.Add(creatorDataEmail);
                    rc.CC.Add(employeeDataEmail);
                    rc.CC.Add(fleetApprovalDataEmail);
                    rc.IsCCExist = true;
                    break;
                case Enums.ActionType.Cancel:
                    rc.Subject = tempData.DOCUMENT_NUMBER_TEMP + " - Cancelled Document";

                    bodyMail.Append("Dear " + employeeDataName + ",<br /><br />");
                    bodyMail.AppendLine();
                    bodyMail.Append("Your temporary car request " + tempData.DOCUMENT_NUMBER_TEMP + " has been cancelled by " + creatorDataName + "<br /><br />");
                    bodyMail.AppendLine();
                    bodyMail.Append("Click <a href='" + webRootUrl + "/TraTemporary/Detail/" + tempData.TRA_TEMPORARY_ID + "?isPersonalDashboard=True" + "'>HERE</a> to monitor your request<br />");
                    bodyMail.AppendLine();
                    bodyMail.Append("Thanks<br /><br />");
                    bodyMail.AppendLine();
                    bodyMail.Append("Regards,<br />");
                    bodyMail.AppendLine();
                    bodyMail.Append("Fleet Team");
                    bodyMail.AppendLine();

                    rc.To.Add(employeeDataEmail);
                    rc.CC.Add(creatorDataEmail);

                    foreach (var item in fleetEmailList)
                    {
                        rc.CC.Add(item);
                    }

                    rc.IsCCExist = true;
                    break;
            }

            rc.Body = bodyMail.ToString();
            return rc;
        }

        public TemporaryDto GetTempById(long id)
        {
            var data = _TemporaryService.GetTemporaryById(id);
            var retData = Mapper.Map<TemporaryDto>(data);
            return retData;
        }

        public List<VehicleFromVendorUpload> ValidationUploadDocumentProcess(List<VehicleFromVendorUpload> inputs, int id)
        {
            var messageList = new List<string>();
            var messageListStopper = new List<string>();
            var outputList = new List<VehicleFromVendorUpload>();

            var dataTemp = _TemporaryService.GetTemporaryById(id);

            foreach (var inputItem in inputs)
            {
                messageList.Clear();

                //check temp number
                if (dataTemp.DOCUMENT_NUMBER.ToLower() != inputItem.CsfNumber.ToLower())
                {
                    messageList.Add("Temporary Number not valid");
                    messageListStopper.Add("Temporary Number not valid");
                }

                //check employee name
                if (dataTemp.EMPLOYEE_NAME.ToLower() != inputItem.EmployeeName.ToLower())
                {
                    messageList.Add("Employee name not same as employee name request");
                    messageListStopper.Add("Employee name not same as employee name request");
                }

                //check manufacturer
                if (dataTemp.MANUFACTURER.ToLower() != inputItem.Manufacturer.ToLower())
                {
                    messageList.Add("Manufacturer not same as employee request");
                }

                //check models
                if (dataTemp.MODEL.ToLower() != inputItem.Models.ToLower())
                {
                    messageList.Add("Models not same as employee request");
                }

                //check series
                if (dataTemp.SERIES.ToLower() != inputItem.Series.ToLower())
                {
                    messageList.Add("Series not same as employee request");
                }

                //check body type
                if (dataTemp.BODY_TYPE.ToLower() != inputItem.BodyType.ToLower())
                {
                    messageList.Add("Body Type not same as employee request");
                }

                //check color
                if (dataTemp.COLOR.ToLower() != inputItem.Color.ToLower())
                {
                    messageList.Add("Colour not same as employee request");
                }

                #region -------------- Set Message Info if exists ---------------

                if (messageList.Count > 0)
                {
                    inputItem.MessageError = "";
                    foreach (var message in messageList)
                    {
                        inputItem.MessageError += message + ";";
                    }
                }

                else
                {
                    inputItem.MessageError = string.Empty;
                }

                #endregion

                #region -------------- Set Message Stopper Info if exists ---------------

                if (messageListStopper.Count > 0)
                {
                    inputItem.MessageErrorStopper = "";
                    foreach (var message in messageListStopper)
                    {
                        inputItem.MessageErrorStopper += message + ";";
                    }
                }

                else
                {
                    inputItem.MessageErrorStopper = string.Empty;
                }

                #endregion

                outputList.Add(inputItem);
            }

            return outputList;
        }

        public void CheckTempInProgress()
        {
            var dateMinus1 = DateTime.Now.AddDays(-1);

            var allTemp = _TemporaryService.GetAllTemp().Where(x => x.VENDOR_CONTRACT_START_DATE != null).ToList();

            var listTempInProgress = allTemp.Where(x => x.DOCUMENT_STATUS == Enums.DocumentStatus.InProgress
                                                                        && x.VENDOR_CONTRACT_START_DATE.Value.Day == dateMinus1.Day
                                                                        && x.VENDOR_CONTRACT_START_DATE.Value.Month == dateMinus1.Month
                                                                        && x.VENDOR_CONTRACT_START_DATE.Value.Year == dateMinus1.Year
                                                                        && !string.IsNullOrEmpty(x.VENDOR_PO_NUMBER)).ToList();

            foreach (var item in listTempInProgress)
            {
                //change status completed
                var input = new TempWorkflowDocumentInput();
                input.ActionType = Enums.ActionType.Completed;
                input.UserId = "SYSTEM";
                input.DocumentId = item.TRA_TEMPORARY_ID;
                input.DocumentNumber = item.DOCUMENT_NUMBER;

                TempWorkflow(input);

                //add new master fleet
                MST_FLEET dbFleet;

                var getZonePriceList = _locationMappingService.GetLocationMapping().Where(x => x.LOCATION == item.LOCATION_CITY
                                                                                                 && x.IS_ACTIVE).FirstOrDefault();

                var zonePrice = getZonePriceList == null ? "" : getZonePriceList.ZONE_PRICE_LIST;

                var priceList = _priceListService.GetPriceList().Where(x => x.YEAR == item.CREATED_DATE.Year
                                                                        && x.MANUFACTURER == item.VENDOR_MANUFACTURER
                                                                        && x.MODEL == item.VENDOR_MODEL
                                                                        && x.SERIES == item.VENDOR_SERIES
                                                                        && x.IS_ACTIVE
                                                                        && x.ZONE_PRICE_LIST == zonePrice).FirstOrDefault();

                dbFleet = Mapper.Map<MST_FLEET>(item);
                dbFleet.IS_ACTIVE = true;
                dbFleet.CREATED_DATE = DateTime.Now;
                dbFleet.VEHICLE_TYPE = _settingService.GetSettingById(Convert.ToInt32(item.VEHICLE_TYPE)).SETTING_VALUE.ToUpper();
                dbFleet.VEHICLE_USAGE = string.Empty;
                dbFleet.SUPPLY_METHOD = "TEMPORARY";
                dbFleet.MONTHLY_HMS_INSTALLMENT = priceList == null ? 0 : priceList.PRICE;
                dbFleet.FUEL_TYPE = string.Empty;

                _fleetService.save(dbFleet);

                _uow.SaveChanges();
            }
        }


        public bool CheckTempExistsInFleet(TemporaryDto item)
        {
            var isExist = false;

            var vehicleType = _settingService.GetSettingById(Convert.ToInt32(item.VEHICLE_TYPE)).SETTING_VALUE.ToUpper();

            var existData = _fleetService.GetFleet().Where(x => x.IS_ACTIVE && x.EMPLOYEE_ID == item.EMPLOYEE_ID
                                                                && x.VEHICLE_TYPE == vehicleType
                                                                && item.START_DATE <= x.END_CONTRACT
                                                                && x.SUPPLY_METHOD == "TEMPORARY").ToList();

            if (existData.Count > 0)
            {
                isExist = true;
            }

            return isExist;
        }


        public bool CheckTempOpenExists(TemporaryDto item)
        {
            var isExist = false;

            var benefitType = _settingService.GetSetting().Where(x => x.SETTING_NAME.ToUpper() == "BENEFIT").FirstOrDefault().MST_SETTING_ID.ToString();

            var existDataOpen = _TemporaryService.GetAllTemp().Where(x => x.EMPLOYEE_ID == item.EMPLOYEE_ID && x.DOCUMENT_STATUS != Enums.DocumentStatus.Cancelled
                                                                       && x.DOCUMENT_STATUS != Enums.DocumentStatus.Completed
                                                                       && x.VEHICLE_TYPE == benefitType).ToList();

            if (existDataOpen.Count > 0)
            {
                isExist = true;
            }

            return isExist;
        }


        public List<TemporaryDto> GetList()
        {
            var data = _TemporaryService.GetAllTemp();

            return Mapper.Map<List<TemporaryDto>>(data);
        }
    }
}
