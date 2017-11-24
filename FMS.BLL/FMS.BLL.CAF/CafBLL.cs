﻿using System.Configuration;
using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;
using AutoMapper;
using FMS.BusinessObject;
using FMS.BusinessObject.Business;
using FMS.BusinessObject.Dto;
using FMS.BusinessObject.Inputs;
using FMS.Contract;
using FMS.Contract.BLL;
using FMS.Contract.Service;
using FMS.Core;
using FMS.DAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.Utils;

namespace FMS.BLL.CAF
{
    public class CafBLL : ICafBLL
    {
        private ICAFService _CafService;
        private IUnitOfWork _uow;

        private IDocumentNumberService _docNumberService;
        private IWorkflowHistoryService _workflowService;
        private ISettingService _settingService;
        private IMessageService _messageService;
        private IEmployeeService _employeeService;
        private IEpafService _epafService;
        private IRemarkService _remarkService;
        private ITemporaryService _temporaryService;
        private IVendorService _vendorService;

        public CafBLL(IUnitOfWork uow)
        {
            _uow = uow;
            _CafService = new CafService(_uow);

            _docNumberService = new DocumentNumberService(_uow);
            _workflowService = new WorkflowHistoryService(_uow);
            _settingService = new SettingService(_uow);
            _messageService = new MessageService(_uow);
            _employeeService = new EmployeeService(_uow);
            _epafService = new EpafService(_uow);
            _remarkService = new RemarkService(_uow);
            _temporaryService = new TemporaryService(_uow);
            _vendorService = new VendorService(_uow);
        }

        public void Save(BusinessObject.Dto.TraCafDto data, BusinessObject.Business.Login user)
        {
            throw new NotImplementedException();
        }

        public List<BusinessObject.Dto.TraCafDto> GetCaf()
        {
            var data = _CafService.GetList();

            return Mapper.Map<List<TraCafDto>>(data);
        }

        public BusinessObject.Dto.TraCafDto GetById(long id)
        {
            var data = _CafService.GetCafById(id);

            return Mapper.Map<TraCafDto>(data);
        }

        public void SaveList(List<TraCafDto> data, BusinessObject.Business.Login CurrentUser)
        {
            
            var datatoSave = Mapper.Map<List<TRA_CAF>>(data);
            foreach (var caf in datatoSave)
            {
                TRA_CAF dataCaf = _CafService.GetCafByNumber(caf.SIRS_NUMBER);
                if (!string.IsNullOrEmpty(caf.VENDOR_NAME))
                {
                    var vendorData = _vendorService.GetByShortName(caf.VENDOR_NAME);
                    caf.VENDOR_ID = vendorData.MST_VENDOR_ID;
                }
                else
                {
                    caf.VENDOR_ID = null;
                }
                if (dataCaf == null)
                {
                    caf.REMARK = null;
                    caf.IS_ACTIVE = true;
                    caf.DOCUMENT_NUMBER = _docNumberService.GenerateNumber(new GenerateDocNumberInput()
                    {
                        DocType = (int) Enums.DocumentType.CAF,
                        Month = DateTime.Now.Month,
                        Year = DateTime.Now.Year

                    });
                    caf.DOCUMENT_STATUS = (int) Enums.DocumentStatus.Draft;
                    _CafService.Save(caf, CurrentUser);
                }
                else
                {
                    dataCaf.IS_ACTIVE = true;
                    dataCaf.REMARK = null;
                }
                
                
            }
            _uow.SaveChanges();
        }


        public void ValidateCaf(TraCafDto dataTovalidate, out string message)
        {
            var dbData = _CafService.GetCafByNumber(dataTovalidate.SirsNumber);
            message = "";
            if (dbData != null)
            {
                message += "Sirs Number already registered in FMS.";
            }
        }


        public TraCafDto GetCafBySirs(string sirsNumber)
        {
            var data = _CafService.GetCafByNumber(sirsNumber);
            return Mapper.Map<TraCafDto>(data);
        }


        public int SaveProgress(TraCafProgressDto traCafProgressDto,string sirsNumber, BusinessObject.Business.Login CurrentUser)
        {
            var data = Mapper.Map<TRA_CAF_PROGRESS>(traCafProgressDto);

            data.CREATED_BY = CurrentUser.USER_ID;
            data.CREATED_DATE = DateTime.Now;
            var caf = _CafService.GetCafByNumber(sirsNumber);
            var lastStatus = caf.TRA_CAF_PROGRESS.OrderByDescending(x => x.STATUS_ID).Select(x => x.STATUS_ID).FirstOrDefault();
            _CafService.SaveProgress(data,sirsNumber,CurrentUser);
            
            if (lastStatus != data.STATUS_ID)
            {
                _workflowService.Save(new WorkflowHistoryDto()
                {
                    ACTION = Enums.ActionType.Modified,
                    ACTION_DATE = DateTime.Now,
                    ACTION_BY = CurrentUser.USER_ID,
                    FORM_ID = caf.TRA_CAF_ID,
                    MODUL_ID = Enums.MenuList.TraCaf
                    
                });
                _uow.SaveChanges();
                caf.DOCUMENT_STATUS = lastStatus.Value;
                var dataCaf = Mapper.Map<TraCafDto>(caf);
                SendEmailWorkflow(dataCaf, Enums.ActionType.Submit);
            }
            return  lastStatus.HasValue ? lastStatus.Value : 0;
        }


        private void SendEmailWorkflow(TraCafDto crfData, Enums.ActionType action)
        {
            //var csfData = Mapper.Map<TraCsfDto>(_CsfService.GetCsfById(input.DocumentId));

            var mailProcess = ProsesMailNotificationBody(crfData, action);

            //distinct double To email
            List<string> ListTo = mailProcess.To.Distinct().ToList();

            if (mailProcess.IsCCExist)
                //Send email with CC
                _messageService.SendEmailToListWithCC(ListTo, mailProcess.CC, mailProcess.Subject, mailProcess.Body, true);
            else
                _messageService.SendEmailToList(ListTo, mailProcess.Subject, mailProcess.Body, true);

        }

        private FMSMailNotification ProsesMailNotificationBody(TraCafDto crfData, Enums.ActionType action)
        {
            var bodyMail = new StringBuilder();
            var rc = new FMSMailNotification();

            //var vehTypeBenefit = _settingService.GetSetting().Where(x => x.SETTING_GROUP == "VEHICLE_TYPE" && x.SETTING_NAME == "BENEFIT").FirstOrDefault().MST_SETTING_ID;

            
            string creatorDataEmail = "";
            var webRootUrl = ConfigurationManager.AppSettings["WebRootUrl"];
            var typeEnv = ConfigurationManager.AppSettings["Environment"];
            var employeeData = _employeeService.GetEmployeeById(crfData.EmployeeId);

            var hrList = new List<string>();
            var fleetList = new List<string>();

            var hrRole = _settingService.GetSetting().Where(x => x.SETTING_GROUP == EnumHelper.GetDescription(Enums.SettingGroup.UserRole)
                                                                && x.SETTING_VALUE.Contains("HR")).FirstOrDefault().SETTING_VALUE;
            var fleetRole = _settingService.GetSetting().Where(x => x.SETTING_GROUP == EnumHelper.GetDescription(Enums.SettingGroup.UserRole)
                                                                && x.SETTING_VALUE.Contains("FLEET")).FirstOrDefault().SETTING_VALUE;

            var hrQuery = "SELECT employeeID FROM OPENQUERY(ADSI, 'SELECT employeeID, sAMAccountName, displayName, name, givenName, whenCreated, whenChanged, SN, manager, distinguishedName, info FROM ''LDAP://DC=PMINTL,DC=NET'' WHERE memberOf = ''CN = " + hrRole + ", OU = ID, OU = Security, OU = IMDL Managed Groups, OU = Global, OU = Users & Workstations, DC = PMINTL, DC = NET''') ";
            var fleetQuery = "SELECT employeeID FROM OPENQUERY(ADSI, 'SELECT employeeID, sAMAccountName, displayName, name, givenName, whenCreated, whenChanged, SN, manager, distinguishedName, info FROM ''LDAP://DC=PMINTL,DC=NET'' WHERE memberOf = ''CN = " + fleetRole + ", OU = ID, OU = Security, OU = IMDL Managed Groups, OU = Global, OU = Users & Workstations, DC = PMINTL, DC = NET''') ";
            var creatorQuery =
                "SELECT EMAIL from [HMSSQLFWOPRD.ID.PMI\\PRD03].[db_Intranet_HRDV2].[dbo].[tbl_ADSI_User] where FULL_NAME like 'PMI\\" +
                crfData.CreatedBy + "'";
            if (typeEnv == "VTI")
            {
                hrQuery = "SELECT EMPLOYEE_ID FROM LOGIN_FOR_VTI WHERE AD_GROUP = '" + hrRole + "'";
                fleetQuery = "SELECT EMPLOYEE_ID FROM LOGIN_FOR_VTI WHERE AD_GROUP = '" + fleetRole + "'";
                creatorQuery = "SELECT EMAIL FROM LOGIN_FOR_VTI WHERE LOGIN like '" + crfData.CreatedBy + "'";
            }

            EntityConnectionStringBuilder e = new EntityConnectionStringBuilder(ConfigurationManager.ConnectionStrings["FMSEntities"].ConnectionString);
            string connectionString = e.ProviderConnectionString;
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            SqlCommand query = new SqlCommand(hrQuery, con);
            SqlDataReader reader = query.ExecuteReader();
            while (reader.Read())
            {
                var hrEmail = _employeeService.GetEmployeeById(crfData.EmployeeId).EMAIL_ADDRESS;
                hrList.Add(hrEmail);
            }

            query = new SqlCommand(fleetQuery, con);
            reader = query.ExecuteReader();
            while (reader.Read())
            {
                var fleetEmail = _employeeService.GetEmployeeById(crfData.EmployeeId).EMAIL_ADDRESS;
                fleetList.Add(fleetEmail);
            }

            query = new SqlCommand(creatorQuery, con);
            reader = query.ExecuteReader();
            while (reader.Read())
            {
                creatorDataEmail = reader["EMAIL"].ToString();
            }

            reader.Close();
            con.Close();

            rc.Subject = "CAF - Car Accident Report Progress";

            bodyMail.Append("Dear " + crfData.EmployeeId + ",<br /><br />");
            bodyMail.AppendLine();
            bodyMail.Append("Your filed Car accident report has  updated.<br />");
            bodyMail.AppendLine();
            
            bodyMail.AppendLine();
            bodyMail.Append("SIRS Number : " + crfData.SirsNumber + "<br />");
            bodyMail.AppendLine();
            bodyMail.Append("Current status of your report : "+ crfData.DocumentStatusString +"<br />");
            bodyMail.AppendLine();
            bodyMail.Append("Send confirmation by clicking below CAF number:<br />");
            bodyMail.AppendLine();
            bodyMail.Append("<a href='" + webRootUrl + "/TraCaf/Edit/" + crfData.TraCafId + "'>" +
                            "CAF Number : "+ crfData.DocumentNumber + "</a> requested by " + crfData.EmployeeName +
                            "<br /><br />");
            bodyMail.AppendLine();
            bodyMail.Append("Thanks<br /><br />");
            bodyMail.AppendLine();
            bodyMail.Append("Regards,<br />");
            bodyMail.AppendLine();
            bodyMail.Append("Fleet Team");
            bodyMail.AppendLine();



            
            rc.To.Add(employeeData.EMAIL_ADDRESS);
            rc.CC.Add(creatorDataEmail);
            foreach (var item in fleetList)
            {
                rc.CC.Add(item);
            }
            
            

            rc.Body = bodyMail.ToString();

            if (rc.CC.Count > 0)
            {
                rc.IsCCExist = true;
            }

            return rc;
        }
    }
}
