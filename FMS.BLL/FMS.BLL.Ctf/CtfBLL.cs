﻿using AutoMapper;
using FMS.BusinessObject;
using FMS.BusinessObject.Business;
using FMS.BusinessObject.Dto;
using FMS.BusinessObject.Inputs;
using FMS.Contract;
using FMS.Contract.BLL;
using FMS.Contract.Service;
using FMS.Core;
using FMS.Core.Exceptions;
using FMS.DAL.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Data.Entity.Core.EntityClient;
using System.Threading.Tasks;
using FMS.Utils;

namespace FMS.BLL.Ctf
{
    public class CtfBLL : ITraCtfBLL
    {
        private IUnitOfWork _uow;
        private ICtfService _ctfService;
        private IDocumentNumberService _docNumberService;
        private IWorkflowHistoryService _workflowService;
        private ISettingService _settingService;
        private IReasonService _reasonService;
        private IPenaltyLogicService _penaltyLogicService;
        private IPriceListService _pricelistService;
        private IFleetService _fleetService;
        private IRemarkService _remarkService;
        private IMessageService _messageService;
        private IEmployeeService _employeeService;
        private IVendorService _vendorService;

        public CtfBLL(IUnitOfWork uow)
        {
            _uow = uow;
            _ctfService = new CtfService(uow);
            _docNumberService = new DocumentNumberService(uow);
            _workflowService = new WorkflowHistoryService(uow);
            _settingService = new SettingService(uow);
            _reasonService = new ReasonService(uow);
            _remarkService = new RemarkService(uow);
            _penaltyLogicService = new PenaltyLogicService(uow);
            _pricelistService = new PriceListService(uow);
            _fleetService = new FleetService(uow);
            _messageService = new MessageService(_uow);
            _employeeService = new EmployeeService(_uow);
            _vendorService = new VendorService(_uow);
        }

        public List<TraCtfDto> GetCtf()
        {
            var data = _ctfService.GetCtf();
            var redata = Mapper.Map<List<TraCtfDto>>(data);
            return redata;
        }
        public List<TraCtfDto> GetCtfDashboard(Login userLogin, bool isCompleted)
        {
            var settingData = _settingService.GetSetting().Where(x => x.SETTING_GROUP == EnumHelper.GetDescription(Enums.SettingGroup.VehicleType));
            var benefitType = settingData.Where(x => x.SETTING_NAME.ToUpper() == "BENEFIT").FirstOrDefault().SETTING_NAME;
            var wtcType = settingData.Where(x => x.SETTING_NAME.ToUpper() == "WTC").FirstOrDefault().SETTING_NAME;

            var data = _ctfService.GetCtfDashboard(userLogin, isCompleted, benefitType, wtcType);
            var retData = Mapper.Map<List<TraCtfDto>>(data);
            return retData;
        }
        public List<TraCtfDto> GetCtfPersonal(Login userLogin)
        {
            var data = _ctfService.GetCtf().Where(x => ((x.EMPLOYEE_ID == userLogin.EMPLOYEE_ID && x.DOCUMENT_STATUS != Enums.DocumentStatus.Draft && x.DOCUMENT_STATUS != Enums.DocumentStatus.Completed && x.DOCUMENT_STATUS != Enums.DocumentStatus.Cancelled)
                                                                || x.EMPLOYEE_ID_CREATOR == userLogin.USER_ID || x.EMPLOYEE_ID_FLEET_APPROVAL == userLogin.EMPLOYEE_ID)).ToList();
            var retData = Mapper.Map<List<TraCtfDto>>(data);
            return retData;
        }
        public TraCtfDto Save(TraCtfDto Dto, Login userLogin)
        {
            TRA_CTF dbTraCtf;
            if (Dto == null)
            {
                throw new Exception("Invalid Data Entry");
            }

            try
            {
                bool changed = false;

                if (Dto.TraCtfId> 0)
                {
                    //update
                   var Exist = _ctfService.GetCtf().Where(c => c.TRA_CTF_ID == Dto.TraCtfId).FirstOrDefault();

                    if (Exist== null)
                        throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

                    //changed = SetChangesHistory(model, item, userId);
                    dbTraCtf =Mapper.Map<TRA_CTF>(Dto);
                    _ctfService.Save(dbTraCtf, userLogin);
                }
                else
                {
                    var inputDoc = new GenerateDocNumberInput();
                    inputDoc.Month = DateTime.Now.Month;
                    inputDoc.Year = DateTime.Now.Year;
                    inputDoc.DocType = (int)Enums.DocumentType.CTF;

                    Dto.DocumentNumber = _docNumberService.GenerateNumber(inputDoc);

                   dbTraCtf= Mapper.Map<TRA_CTF>(Dto);
                    _ctfService.Save(dbTraCtf, userLogin);

                }
                var input = new CtfWorkflowDocumentInput()
                {
                    DocumentId = dbTraCtf.TRA_CTF_ID,
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
            var data = _ctfService.GetCtf().Where(x => x.DOCUMENT_NUMBER == Dto.DocumentNumber).FirstOrDefault();
            Dto = Mapper.Map<TraCtfDto>(data);
            return Dto;
        }

        public decimal? PenaltyCost (TraCtfDto CtfDto)
        {
         
            if (CtfDto == null)
               return null;
            var reason = _reasonService.GetReasonById(CtfDto.Reason.Value);
            if (reason.IS_PENALTY)
            {
                var rumus = _penaltyLogicService.GetPenaltyLogic();
                CtfDto.Penalty = 10000;
                return 0;
            }

            return null;
        }
        
        public decimal? RefundCost(TraCtfDto CtfDto)
        {
            var fleet = _fleetService.GetFleet().Where(x => x.EMPLOYEE_ID == CtfDto.EmployeeId && x.POLICE_NUMBER == CtfDto.PoliceNumber && x.IS_ACTIVE).FirstOrDefault();
            if (fleet == null) return null;

            var installmentEmp = _pricelistService.GetPriceList().Where(x => x.MANUFACTURER == fleet.MANUFACTURER && x.MODEL == fleet.MODEL && x.SERIES == fleet.SERIES && x.IS_ACTIVE == true).FirstOrDefault().INSTALLMEN_EMP;
            if (installmentEmp == null) return null;


            var rentMonth = ((fleet.END_CONTRACT.Value.Year - fleet.START_CONTRACT.Value.Year) * 12) + fleet.END_CONTRACT.Value.Month - fleet.START_CONTRACT.Value.Month;
                
            if(_reasonService.GetReasonById(CtfDto.Reason.Value).REASON.ToLower() == "RESIGN")
            {
                var cost1 = (rentMonth * installmentEmp);
                return cost1;
            }

            var cost2 = (rentMonth * installmentEmp);
            return cost2;

        }


        public void CtfWorkflow(CtfWorkflowDocumentInput input)
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
            }
            //todo sent mail
            if (isNeedSendNotif)SendEmailWorkflow(input);

            _uow.SaveChanges();
        }
        private void SendEmailWorkflow(CtfWorkflowDocumentInput input)
        {
            var ctfData = Mapper.Map<TraCtfDto>(_ctfService.GetCtfById(input.DocumentId));

            var mailProcess = ProsesMailNotificationBody(ctfData, input);

            //distinct double To email
            List<string> ListTo = mailProcess.To.Distinct().ToList();

            if (mailProcess.IsCCExist)
                //Send email with CC
                _messageService.SendEmailToListWithCC(ListTo, mailProcess.CC, mailProcess.Subject, mailProcess.Body, true);
            else
                _messageService.SendEmailToList(ListTo, mailProcess.Subject, mailProcess.Body, true);

        }

        private class CtfMailNotification
        {
            public CtfMailNotification()
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

        private CtfMailNotification ProsesMailNotificationBody(TraCtfDto ctfData, CtfWorkflowDocumentInput input)
        {
            var bodyMail = new StringBuilder();
            var rc = new CtfMailNotification();

            var fleetdata = _fleetService.GetFleet().Where(x => x.POLICE_NUMBER == ctfData.PoliceNumber && x.IS_ACTIVE).FirstOrDefault();
            var vendor = _vendorService.GetVendor().Where(x => x.VENDOR_NAME == fleetdata.VENDOR_NAME).FirstOrDefault();
            var vehTypeBenefit = _settingService.GetSetting().Where(x => x.SETTING_GROUP == "VEHICLE_TYPE" && x.SETTING_NAME == "BENEFIT").FirstOrDefault().SETTING_NAME;

            var isBenefit = ctfData.VehicleType == vehTypeBenefit.ToString() ? true : false;

            var webRootUrl = ConfigurationManager.AppSettings["WebRootUrl"];
            var typeEnv = ConfigurationManager.AppSettings["Environment"];
            var employeeData = _employeeService.GetEmployeeById(ctfData.EmployeeId);
            var creatorData = _employeeService.GetEmployeeById(ctfData.EmployeeIdCreator);

            var fleetApprovalData = _employeeService.GetEmployeeById(ctfData.EmployeeIdFleetApproval);

            var employeeDataEmail = employeeData == null ? string.Empty : employeeData.EMAIL_ADDRESS;
            var creatorDataEmail = creatorData == null ? string.Empty : creatorData.EMAIL_ADDRESS;
            var vendorDataEmail = vendor == null ? string.Empty : vendor.EMAIL_ADDRESS;

            var employeeDataName = employeeData == null ? string.Empty : employeeData.FORMAL_NAME;
            var creatorDataName = creatorData == null ? string.Empty : creatorData.FORMAL_NAME;
            var fleetApprovalDataName = fleetApprovalData == null ? string.Empty : fleetApprovalData.FORMAL_NAME;
            var vendorDataName = vendor == null ? string.Empty : vendor.VENDOR_NAME;

            var hrList = new List<string>();
            var fleetList = new List<string>();

            var hrRole = _settingService.GetSetting().Where(x => x.SETTING_GROUP == EnumHelper.GetDescription(Enums.SettingGroup.UserRole)
                                                                && x.SETTING_VALUE.Contains("HR")).FirstOrDefault().SETTING_VALUE;
            var fleetRole = _settingService.GetSetting().Where(x => x.SETTING_GROUP == EnumHelper.GetDescription(Enums.SettingGroup.UserRole)
                                                                && x.SETTING_VALUE.Contains("FLEET")).FirstOrDefault().SETTING_VALUE;

            var hrQuery = "SELECT employeeID FROM OPENQUERY(ADSI, 'SELECT employeeID, sAMAccountName, displayName, name, givenName, whenCreated, whenChanged, SN, manager, distinguishedName, info FROM ''LDAP://DC=PMINTL,DC=NET'' WHERE memberOf = ''CN = " + hrRole + ", OU = ID, OU = Security, OU = IMDL Managed Groups, OU = Global, OU = Users & Workstations, DC = PMINTL, DC = NET''') ";
            var fleetQuery = "SELECT employeeID FROM OPENQUERY(ADSI, 'SELECT employeeID, sAMAccountName, displayName, name, givenName, whenCreated, whenChanged, SN, manager, distinguishedName, info FROM ''LDAP://DC=PMINTL,DC=NET'' WHERE memberOf = ''CN = " + fleetRole + ", OU = ID, OU = Security, OU = IMDL Managed Groups, OU = Global, OU = Users & Workstations, DC = PMINTL, DC = NET''') ";

            if (typeEnv == "VTI")
            {
                hrQuery = "SELECT EMPLOYEE_ID FROM LOGIN_FOR_VTI WHERE AD_GROUP = '" + hrRole + "'";
                fleetQuery = "SELECT EMPLOYEE_ID FROM LOGIN_FOR_VTI WHERE AD_GROUP = '" + fleetRole + "'";
            }

            EntityConnectionStringBuilder e = new EntityConnectionStringBuilder(ConfigurationManager.ConnectionStrings["FMSEntities"].ConnectionString);
            string connectionString = e.ProviderConnectionString;
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            SqlCommand query = new SqlCommand(hrQuery, con);
            SqlDataReader reader = query.ExecuteReader();
            while (reader.Read())
            {
                var hrEmail = _employeeService.GetEmployeeById(ctfData.EmployeeId);
                var hrEmailData = hrEmail == null ? string.Empty : hrEmail.EMAIL_ADDRESS;
                hrList.Add(hrEmailData);
            }

            query = new SqlCommand(fleetQuery, con);
            reader = query.ExecuteReader();
            while (reader.Read())
            {
                var fleetEmail = _employeeService.GetEmployeeById(ctfData.EmployeeId);
                var fleetEmailData = fleetEmail == null ? string.Empty : fleetEmail.EMAIL_ADDRESS;
                fleetList.Add(fleetEmailData);
            }

            reader.Close();
            con.Close();

            switch (input.ActionType)
            {
                case Enums.ActionType.Submit:
                    //if submit from HR to EMPLOYEE
                    if (ctfData.EmployeeIdCreator == input.EmployeeId && isBenefit)
                    {
                        rc.Subject = ctfData.DocumentNumber + " - Car Termination";

                        bodyMail.Append("Dear " + ctfData.EmployeeName + ",<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Here is vehicle data which terminated for the below reason<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("'" + ctfData.ReasonS + "'<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Please confirm for the vehicle, and fill the information for Withdrawal <a href='" + webRootUrl + "/TraCtf/EditForEmployeeBenefit?TraCtfId=" + ctfData.TraCtfId + "&isPersonalDashboard=True" + "'>HERE</a><br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("For any assistance please contact " + ctfData.CreatedBy + " <br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Thanks<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Regards,<br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("HR Team <br /><br />");
                        bodyMail.AppendLine();
                        rc.To.Add(employeeDataEmail);

                        foreach (var item in hrList)
                        {
                            rc.CC.Add(item);
                        }
                    }
                    //if submit from FLEET to EMPLOYEE
                    else if (ctfData.EmployeeIdCreator == input.EmployeeId && !isBenefit && !input.EndRent.Value)
                    {

                        rc.Subject = ctfData.DocumentNumber + " - Car Termination";

                        bodyMail.Append("Dear " + ctfData.EmployeeName + ",<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Here is vehicle data which terminated for the below reason<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("'" + ctfData.ReasonS + "'<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Please confirm for the vehicle, and fill the information for Withdrawal <a href='" + webRootUrl + "/TraCtf/EditForEmployee?TraCtfId=" + ctfData.TraCtfId + "&isPersonalDashboard=True" + "'>HERE</a><br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("For any assistance please contact " + ctfData.CreatedBy + " <br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Thanks<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Regards,<br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Fleet Team <br /><br />");
                        bodyMail.AppendLine();
                        rc.To.Add(employeeDataEmail);

                        foreach (var item in fleetList)
                        {
                            rc.CC.Add(item);
                        }
                    }
                    else if (ctfData.EmployeeIdCreator == input.EmployeeId && !isBenefit && input.EndRent.Value)
                    {
                        rc.Subject = "CTF -  Car Termination";

                        bodyMail.Append("Dear " + creatorDataName + ",<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("You have received new Car Termination Form<br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Send confirmation by clicking below CTF number:<br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("<a href='" + webRootUrl + "/TraCtf/Edit?TraCtfId=" + ctfData.TraCtfId + "&isPersonalDashboard=False" + "'>" + ctfData.DocumentNumber + "</a> requested by " + ctfData.EmployeeName + "<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Thanks<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Regards,<br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Fleet Team");
                        bodyMail.AppendLine();

                        rc.To.Add(creatorDataEmail);

                        foreach (var item in fleetList)
                        {
                            rc.CC.Add(item);
                        }
                    }
                    //if submit from EMPLOYEE to Fleet
                    else if (ctfData.EmployeeId == input.EmployeeId)
                    {
                        rc.Subject = "CTF -  Car Termination";

                        bodyMail.Append("Dear " + creatorDataName + ",<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("You have received new Car Termination Form<br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Send confirmation by clicking below CTF number:<br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("<a href='" + webRootUrl + "/TraCtf/Edit?TraCtfId=" + ctfData.TraCtfId + "&isPersonalDashboard=False" + "'>" + ctfData.DocumentNumber + "</a> requested by " + ctfData.EmployeeName + "<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Thanks<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Regards,<br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Fleet Team");
                        bodyMail.AppendLine();

                        rc.To.Add(creatorDataEmail);

                        foreach (var item in fleetList)
                        {
                            rc.CC.Add(item);
                        }
                    }

                    break;
                case Enums.ActionType.Approve:

                    //if Fleet Approve for benefit
                    if (input.UserRole == Enums.UserRole.Fleet && isBenefit)
                    {
                        rc.Subject = ctfData.DocumentNumber + " - Car Termination";

                        bodyMail.Append("Dear " + creatorDataName + ",<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Your Car Termination Form " + ctfData.DocumentNumber + " has been approved by " + fleetApprovalDataName + "<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Click <a href='" + webRootUrl + "/TraCtf/DetailsBenefit?TraCtfId=" + ctfData.TraCtfId + "&isPersonalDashboard=False" + "'>HERE</a> to monitor your request<br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Thanks<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Regards,<br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Fleet Team");
                        bodyMail.AppendLine();

                        rc.To.Add(creatorDataEmail);

                        foreach (var item in fleetList)
                        {
                            rc.CC.Add(item);
                        }
                    }
                    //if Fleet Approve for wtc
                    else if (input.UserRole == Enums.UserRole.Fleet && !isBenefit)
                    {
                        rc.Subject = ctfData.DocumentNumber + " - Car Termination";

                        bodyMail.Append("Dear " + creatorDataName + ",<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Your Car Termination Form " + ctfData.DocumentNumber + " has been approved by " + fleetApprovalDataName + "<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Click <a href='" + webRootUrl + "/TraCtf/DetailsWTC?TraCtfId=" + ctfData.TraCtfId + "&isPersonalDashboard=False" + "'>HERE</a> to monitor your request<br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Thanks<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Regards,<br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Fleet Team");
                        bodyMail.AppendLine();

                        rc.To.Add(creatorDataEmail);

                        foreach (var item in fleetList)
                        {
                            rc.CC.Add(item);
                        }
                    }
                    break;
                case Enums.ActionType.Reject:

                    if (input.UserRole == Enums.UserRole.Fleet && isBenefit)
                    {
                        rc.Subject = ctfData.DocumentNumber + " - Car Termination";

                        bodyMail.Append("Dear " + ctfData.EmployeeName + ",<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Your Document " + ctfData.DocumentNumber + " has been rejected by " + fleetApprovalDataName + " for below reason : " + _remarkService.GetRemarkById(input.Comment.Value).REMARK + "<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Please revised and re-submit your request <a href='" + webRootUrl + "/TraCtf/EditForEmployeeBenefit?TraCtfId=" + ctfData.TraCtfId + "&isPersonalDashboard=True" + "'>HERE</a><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Thanks<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Regards,<br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Fleet Team");
                        bodyMail.AppendLine();

                        rc.To.Add(employeeDataEmail);

                        foreach (var item in fleetList)
                        {
                            rc.CC.Add(item);
                        }
                    }
                    //if Fleet Reject Benefit
                    else if (input.UserRole == Enums.UserRole.Fleet && !isBenefit)
                    {
                        rc.Subject = ctfData.DocumentNumber + "- Car Termination";

                        bodyMail.Append("Dear " + ctfData.EmployeeName + ",<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Your Document " + ctfData.DocumentNumber + " has been rejected by " + fleetApprovalDataName + " for below reason : " + _remarkService.GetRemarkById(input.Comment.Value).REMARK + "<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Please revised and re-submit your request <a href='" + webRootUrl + "/TraCtf/EditForEmployeeWTC?TraCtfId=" + ctfData.TraCtfId + "&isPersonalDashboard=True" + "'>HERE</a><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Thanks<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Regards,<br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Fleet Team");
                        bodyMail.AppendLine();

                        rc.To.Add(employeeDataEmail);

                        foreach (var item in fleetList)
                        {
                            rc.CC.Add(item);
                        }
                    }
                    break;
            }
                    rc.Body = bodyMail.ToString();
                    return rc;
        }

        private void CreateDocument(CtfWorkflowDocumentInput input)
        {
            var dbData = _ctfService.GetCtf().Where(x => x.TRA_CTF_ID== input.DocumentId).FirstOrDefault();

            if (dbData == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            input.DocumentNumber = dbData.DOCUMENT_NUMBER;

            AddWorkflowHistory(input);
        }

        private void AddWorkflowHistory(CtfWorkflowDocumentInput input)
        {
            var dbData = Mapper.Map<WorkflowHistoryDto>(input);

            dbData.ACTION_DATE = DateTime.Now;
            dbData.MODUL_ID = Enums.MenuList.TraCtf;
            dbData.ACTION = input.ActionType;
            dbData.REMARK_ID = null;


            _workflowService.Save(dbData);

        }

        public void CancelCtf(long id, int Remark, string user)
        {
            _ctfService.CancelCtf(id, Remark, user);
        }

        private void SubmitDocument(CtfWorkflowDocumentInput input)
        {
            var dbData = _ctfService.GetCtfById(input.DocumentId);

            if (dbData == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            if (input.EndRent.Value)
            {
                dbData.DOCUMENT_STATUS = Enums.DocumentStatus.InProgress;
            }
            else if (dbData.DOCUMENT_STATUS == Enums.DocumentStatus.Draft)
            {
                dbData.DOCUMENT_STATUS = Enums.DocumentStatus.AssignedForUser;
            }
            else if (dbData.DOCUMENT_STATUS == Enums.DocumentStatus.AssignedForUser)
            {
               dbData.DOCUMENT_STATUS = Enums.DocumentStatus.WaitingFleetApproval;
             
            }

            input.DocumentNumber = dbData.DOCUMENT_NUMBER;

            AddWorkflowHistory(input);

        }


        public TraCtfDto GetCtfById(long id)
        {
            var data = _ctfService.GetCtfById(id);
            var retData = Mapper.Map<TraCtfDto>(data);
            return retData;
        }
        private void ApproveDocument(CtfWorkflowDocumentInput input)
        {
            var dbData = _ctfService.GetCtfById(input.DocumentId);

            if (dbData == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);
        
            if (dbData.DOCUMENT_STATUS == Enums.DocumentStatus.WaitingFleetApproval && dbData.EFFECTIVE_DATE == DateTime.Today)
            {
                dbData.DOCUMENT_STATUS = Enums.DocumentStatus.InProgress;
            }
            else if (dbData.DOCUMENT_STATUS == Enums.DocumentStatus.WaitingFleetApproval && dbData.EFFECTIVE_DATE == DateTime.Today )
            {
                dbData.DOCUMENT_STATUS = Enums.DocumentStatus.Completed;
            }
            input.DocumentNumber = dbData.DOCUMENT_NUMBER;

            AddWorkflowHistory(input);

        }

        private void RejectDocument(CtfWorkflowDocumentInput input)
        {
            var dbData = _ctfService.GetCtfById(input.DocumentId);

            if (dbData == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

           if (dbData.DOCUMENT_STATUS == Enums.DocumentStatus.WaitingFleetApproval)
            {
                dbData.DOCUMENT_STATUS = Enums.DocumentStatus.AssignedForUser;
            }

            input.DocumentNumber = dbData.DOCUMENT_NUMBER;

            AddWorkflowHistory(input);

        }
    }
}

