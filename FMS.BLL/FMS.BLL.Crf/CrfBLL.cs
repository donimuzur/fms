﻿using System.Security.Policy;
using AutoMapper;
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
using FMS.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace FMS.BLL.Crf
{
    public class CrfBLL : ITraCrfBLL
    {
        private ICRFService _CrfService;
        private IEpafService _epafService;
        private IDocumentNumberService _docNumberService;
        private IEmployeeService _employeeService;
        private IFleetService _fleetService;
        private IVendorService _vendorService;
        private IWorkflowHistoryService _workflowService;
        private IMessageService _messageService;
        private ISettingService _settingService;
        private ITemporaryService _temporaryService;

        private IUnitOfWork _uow;
        public CrfBLL(IUnitOfWork uow)
        {
            _uow = uow;
            _CrfService = new CrfService(_uow);
            _epafService = new EpafService(_uow);
            _employeeService = new EmployeeService(_uow);
            _docNumberService = new DocumentNumberService(_uow);
            _workflowService = new WorkflowHistoryService(_uow);
            _vendorService = new VendorService(_uow);
            _fleetService = new FleetService(_uow);
            _messageService = new MessageService(_uow);
            _settingService = new SettingService(_uow);
            _temporaryService = new TemporaryService(_uow);
        }


        public List<TraCrfDto> GetList(Login currentUser)
        {
            var data = _CrfService.GetList().Where(x => x.DOCUMENT_STATUS != (int)Enums.DocumentStatus.Completed
                && x.DOCUMENT_STATUS != (int)Enums.DocumentStatus.Cancelled
                //&& x.DOCUMENT_STATUS != (int)Enums.DocumentStatus.Draft
                //|| (x.DOCUMENT_STATUS != (int)Enums.DocumentStatus.Draft 
                //&& x.DOCUMENT_STATUS != (int)Enums.DocumentStatus.Cancelled
                //&& x.DOCUMENT_STATUS != (int)Enums.DocumentStatus.Completed
                //&& x.CREATED_BY == currentUser.USER_ID)
                ).ToList();
            List<TRA_CRF> crfList = new List<TRA_CRF>();
            if (currentUser.UserRole == Enums.UserRole.User || currentUser.UserRole == 0)
            {
                crfList.AddRange(data.Where(x => x.EMPLOYEE_ID == currentUser.EMPLOYEE_ID).ToList());
                
            }

            if (currentUser.UserRole == Enums.UserRole.Fleet || currentUser.UserRole == Enums.UserRole.HR)
            {
                data = data.Where(x => x.EMPLOYEE_ID != currentUser.EMPLOYEE_ID).ToList();
                if (currentUser.UserRole == Enums.UserRole.Fleet)
                {
                    crfList.AddRange(data.Where(x => x.VEHICLE_TYPE == "WTC" 
                        || x.DOCUMENT_STATUS == (int) Enums.DocumentStatus.WaitingFleetApproval 
                        || x.DOCUMENT_STATUS == (int) Enums.DocumentStatus.InProgress));
                    
                }

                if (currentUser.UserRole == Enums.UserRole.HR )
                {
                    crfList.AddRange(data.Where(x => x.VEHICLE_TYPE == "BENEFIT"));
                }
                    
                
            }

            if (currentUser.UserRole == Enums.UserRole.Viewer
                    || currentUser.UserRole == Enums.UserRole.Administrator)
            {
                crfList.AddRange(data);
            }


            
            return Mapper.Map<List<TraCrfDto>>(crfList);
        }

        public List<TraCrfDto> GetList()
        {
            var data = _CrfService.GetList().Where(x=> x.IS_ACTIVE);


            return Mapper.Map<List<TraCrfDto>>(data);
        }


        public List<TraCrfDto> GetCompleted()
        {
            var data = _CrfService.GetList().Where(x => x.DOCUMENT_STATUS == (int)Enums.DocumentStatus.Completed
                || x.DOCUMENT_STATUS == (int)Enums.DocumentStatus.Cancelled);


            return Mapper.Map<List<TraCrfDto>>(data);
        }

        public TraCrfDto GetDataById(long id)
        {
            var data = _CrfService.GetById((int)id);

            return Mapper.Map<TraCrfDto>(data);
        }

        

        

        

        public TraCrfDto AssignCrfFromEpaf(long epafId, Login CurrentUser)
        {

            var epafData = _epafService.GetEpafById(epafId);
            
            if (epafData != null)
            {
                var existingData = _CrfService.GetByEpafId(epafId);
                if (existingData != null)
                {
                    throw new Exception("Epaf Already asigned.");
                }
                
                TraCrfDto item = new TraCrfDto();
                var employeeData = _employeeService.GetEmployeeById(epafData.EMPLOYEE_ID);
                var fleetData = _fleetService.GetFleetByParam(new FleetParamInput()
                {
                    EmployeeId = employeeData.EMPLOYEE_ID,
                    VehicleType = "BENEFIT"

                }).FirstOrDefault();
                var fleetDo = Mapper.Map<FleetDto>(fleetData);
                var employeeDto = Mapper.Map<EmployeeDto>(employeeData);
                employeeDto.EmployeeVehicle = fleetDo;
                item = Mapper.Map<TraCrfDto>(epafData);
                if (CurrentUser.UserRole == Enums.UserRole.HR)
                {
                    item.VEHICLE_TYPE = "BENEFIT";
                    item.COST_CENTER = employeeDto.COST_CENTER;
                    
                }
                else if (CurrentUser.UserRole == Enums.UserRole.Fleet)
                {
                    item.VEHICLE_TYPE = "WTC";
                }

                if (employeeData == null)
                {
                    throw new Exception(string.Format("Employee Data {0} not found.", epafData.EMPLOYEE_ID));
                }

                item.LOCATION_CITY = employeeData.CITY;
                item.LOCATION_OFFICE = employeeData.BASETOWN;

                item.CREATED_BY = CurrentUser.USER_ID;
                item.CREATED_DATE = DateTime.Now;
                item.DOCUMENT_STATUS = (int) Enums.DocumentStatus.AssignedForUser;
                item.IS_ACTIVE = true;

                var vehicleData = _fleetService.GetFleetByParam(new FleetParamInput()
                {
                    EmployeeId = epafData.EMPLOYEE_ID,
                    VehicleType = item.VEHICLE_TYPE,

                }).FirstOrDefault();

                if (vehicleData != null)
                {
                    var vendorData =
                        _vendorService.GetVendor().FirstOrDefault(x => x.SHORT_NAME == vehicleData.VENDOR_NAME);
                    item.POLICE_NUMBER = vehicleData.POLICE_NUMBER;
                    item.MANUFACTURER = vehicleData.MANUFACTURER;
                    item.MODEL = vehicleData.MODEL;
                    item.SERIES = vehicleData.SERIES;
                    item.Body = vehicleData.BODY_TYPE;
                    item.VENDOR_NAME = vendorData != null ? vendorData.SHORT_NAME : null;
                    item.VENDOR_ID = vendorData != null ? vendorData.MST_VENDOR_ID : (int?) null;
                    item.START_PERIOD = vehicleData.START_CONTRACT;
                    item.END_PERIOD = vehicleData.END_CONTRACT;
                    item.VEHICLE_TYPE = vehicleData.VEHICLE_TYPE;
                    item.VEHICLE_USAGE = vehicleData.VEHICLE_USAGE;
                    item.MST_FLEET_ID = vehicleData.MST_FLEET_ID;

                    item.WITHD_CITY = vehicleData.CITY;
                    item.WITHD_ADDRESS = vehicleData.ADDRESS;
                }
                else
                {
                    throw new Exception("Vehicle for this employee not found on FMS.");
                }

                item.DELIV_CITY = epafData.CITY;
                item.DELIV_ADDRESS = epafData.BASE_TOWN;

                

                var returnData = this.SaveCrf(item, CurrentUser);

                AddWorkflowHistory(returnData,CurrentUser,Enums.ActionType.Submit, null);

                return returnData;
            }
            else
            {
                throw new Exception("Please select Epaf document first.");
            }
        }

        public TraCrfDto SaveCrf(TraCrfDto data,Login userLogin)
        {
            var remark = data.REMARK_ID;
            data.REMARK_ID = null;
            var datatosave = Mapper.Map<TRA_CRF>(data);
            datatosave.BODY_TYPE = data.Body;
            datatosave.MODIFIED_BY = userLogin.USER_ID;

            var dataFleet = _fleetService.GetFleetByParam(new FleetParamInput()
            {
                EmployeeId = datatosave.EMPLOYEE_ID,
                VehicleType = datatosave.VEHICLE_TYPE
            }).FirstOrDefault();

            if (datatosave.TRA_CRF_ID > 0)
            {
                
            }
            else
            {
                //datatosave.role_type
                    

                datatosave.DOCUMENT_NUMBER = _docNumberService.GenerateNumber(new GenerateDocNumberInput() { 
                    Month = DateTime.Now.Month,
                    Year = DateTime.Now.Year,
                    DocType = (int) Enums.DocumentType.CRF
                });

                //datatosave.DELIV_CITY = datatosave.LOCATION_CITY;
                //datatosave.DELIV_ADDRESS = datatosave.LOCATION_OFFICE;

                //datatosave.WITHD_CITY = dataFleet != null ? dataFleet.CITY : null;
                //datatosave.WITHD_ADDRESS = dataFleet != null ? dataFleet.ADDRESS : null;
            }

            if (datatosave.VEHICLE_TYPE == "WTC" || (datatosave.VEHICLE_TYPE == "BENEFIT" && datatosave.VEHICLE_USAGE=="COP"))
            {
                datatosave.RELOCATION_TYPE = "RELOCATE_UNIT";
                
            }
            if (data.DOCUMENT_STATUS != (int) Enums.DocumentStatus.Cancelled)
            {
                datatosave.MST_REMARK = null;
                datatosave.REMARK = null;
            }
            else
            {
                datatosave.REMARK = remark;
                _CrfService.SaveCrf(datatosave, userLogin);
                return data;
                //AddWorkflowHistory(data, userLogin, Enums.ActionType.Created, null);
            }
            
            
            datatosave.BODY_TYPE = dataFleet != null ? dataFleet.BODY_TYPE : null;
            datatosave.MST_FLEET_ID = dataFleet != null ? dataFleet.MST_FLEET_ID : (long?) null;
            var isCompleted = false;
            if (data.DOCUMENT_STATUS == (int)Enums.DocumentStatus.InProgress
                && DateTime.Now >= data.EFFECTIVE_DATE)
            {
                if (data.PRICE != null)
                {
                    data.DOCUMENT_STATUS = (int)Enums.DocumentStatus.Completed;
                    isCompleted = CompleteDocument(data.TRA_CRF_ID,userLogin);
                    if (isCompleted)
                    {
                        datatosave.DOCUMENT_STATUS = data.DOCUMENT_STATUS;
                    }
                }
                
                
            }
            if (!data.IsSend)
            {
                data.TRA_CRF_ID = _CrfService.SaveCrf(datatosave, userLogin);
                if (data.DOCUMENT_STATUS == (int) Enums.DocumentStatus.Draft)
                {
                    AddWorkflowHistory(data, userLogin, Enums.ActionType.Created, null);
                }
            }
            else
            {
                data.TRA_CRF_ID = _CrfService.SaveCrf(datatosave, userLogin);
                
                //AddWorkflowHistory(data, userLogin, Enums.ActionType.Submit, null);
                _uow.SaveChanges();

                data.DOCUMENT_NUMBER = datatosave.DOCUMENT_NUMBER;
    
                SubmitCrf(data,userLogin);
            }
            
            
            
            
            return data;
            
            
            
        }

        private bool CompleteDocument(long traCrfId,Login fleetUser)
        {
            bool success = true;
            var data = GetDataById(traCrfId);
            success = UpdateFleet(data, fleetUser);
            SendEmailWorkflow(data,Enums.ActionType.Completed);
            return success;
        }

        public List<string> CompleteAllDocument()
        {
            List<string> message = new List<string>();
            var dataToComplete = _CrfService.GetList(new TraCrfEpafParamInput()
            {
                EffectiveDateComplete = DateTime.Today
            });
            var dtoList = Mapper.Map<List<TraCrfDto>>(dataToComplete.Where(x => !string.IsNullOrEmpty(x.PO_NUMBER)));
            foreach (var data in dtoList)
            {
                try
                {
                    UpdateFleet(data, new Login()
                    {
                        USER_ID = "SYSTEM"
                    });

                    var datatosave = Mapper.Map<TRA_CRF>(data);
                    datatosave.DOCUMENT_STATUS = (int)Enums.DocumentStatus.Completed;
                    datatosave.MODIFIED_BY = "SYSTEM";
                    datatosave.MODIFIED_DATE = DateTime.Now;

                    _CrfService.SaveCrf(datatosave, null);

                    SendEmailWorkflow(data,Enums.ActionType.Completed);

                    _uow.SaveChanges();
                }
                catch (Exception ex)
                {
                    message.Add(ex.Message);
                }
            }

            return message;
        }

        private bool UpdateFleet(TraCrfDto data,Login loginFleet)
        {
            var dataFleet = _fleetService.GetFleetByParam(new FleetParamInput()
            {
                EmployeeId = data.EMPLOYEE_ID,
                VehicleType = data.VEHICLE_TYPE,
                VehicleUsage = data.VEHICLE_USAGE,
                
                PoliceNumber = data.POLICE_NUMBER
            }).FirstOrDefault();
            if(data.MST_FLEET_ID.HasValue)
            {
                dataFleet = _fleetService.GetFleetById((int)data.MST_FLEET_ID);
            }

            if (dataFleet == null)
            {
                return false;
            }
            else
            {
                dataFleet.IS_ACTIVE = false;
                                
                try
                {
                    if ((data.VEHICLE_USAGE == null ? "" : data.VEHICLE_USAGE.ToUpper()) == "CFM" && (data.RelocationType == null ? "" : data.RelocationType.ToUpper()) == "CHANGE_UNIT")
                    {
                        var IdleVehicle = dataFleet;
                        IdleVehicle.MST_FLEET_ID = 0;
                        IdleVehicle.IS_ACTIVE = true;
                        IdleVehicle.EMPLOYEE_ID = null;
                        IdleVehicle.EMPLOYEE_NAME = null;
                        IdleVehicle.ASSIGNED_TO = null;
                        IdleVehicle.START_DATE = DateTime.Now;
                        IdleVehicle.END_DATE = null;
                        IdleVehicle.VEHICLE_STATUS = "LIVE";
                        IdleVehicle.VEHICLE_USAGE = "CFM IDLE";
                        IdleVehicle.CREATED_BY = "SYSTEM";
                        IdleVehicle.CREATED_DATE = DateTime.Now;
                        IdleVehicle.MODIFIED_BY = null;
                        IdleVehicle.MODIFIED_DATE = null;
                        IdleVehicle.DOCUMENT_NUMBER = data.DOCUMENT_NUMBER; 

                        _fleetService.save(IdleVehicle);
                    }

                    _fleetService.save(dataFleet);
                 
                }
                catch (Exception)
                {
                    return false;
                }
            }

            var dataToSave = Mapper.Map<FleetDto>(dataFleet);

            if (data.CHANGE_POLICE_NUMBER.HasValue && data.CHANGE_POLICE_NUMBER.Value)
            {
                dataToSave.PoliceNumber = data.NEW_POLICE_NUMBER;
            }
            
            dataToSave.City = data.LOCATION_CITY_NEW;
            dataToSave.ModifiedBy = loginFleet.USER_ID;
            dataToSave.ModifiedDate = DateTime.Now;
            dataToSave.CostCenter = data.COST_CENTER_NEW;
            dataToSave.IsActive = true;
            dataToSave.MstFleetId = 0;
            dataToSave.DocumentNumber = data.DOCUMENT_NUMBER;
            try
            {
                var dataSaveFromDto = Mapper.Map<MST_FLEET>(dataToSave);
                _fleetService.save(dataSaveFromDto);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
            
        }

        public bool IsAllowedEdit(Login currentUser, TraCrfDto data)
        {
            
            if (currentUser.EMPLOYEE_ID == data.EMPLOYEE_ID 
                && data.DOCUMENT_STATUS == (int) Enums.DocumentStatus.AssignedForUser) 
                return true;

            if (currentUser.UserRole == Enums.UserRole.HR
                && (data.DOCUMENT_STATUS == (int)Enums.DocumentStatus.Draft || data.DOCUMENT_STATUS == (int)Enums.DocumentStatus.AssignedForUser))
                return true;
            if (currentUser.UserRole == Enums.UserRole.Fleet 
                && data.DOCUMENT_STATUS == (int)Enums.DocumentStatus.InProgress)
                return true;
            if (currentUser.UserRole == Enums.UserRole.Fleet
                && (data.DOCUMENT_STATUS == (int)Enums.DocumentStatus.Draft || data.DOCUMENT_STATUS == (int)Enums.DocumentStatus.AssignedForUser)
                && data.VEHICLE_TYPE == "WTC")
                return true;
            return false;
        }

        public bool IsAllowedApprove(Login currentUser, TraCrfDto data)
        {
            bool isAllowed = false;

            if (currentUser.UserRole == Enums.UserRole.HR
                && data.DOCUMENT_STATUS == (int)Enums.DocumentStatus.WaitingHRApproval)
                return true;
            if (currentUser.UserRole == Enums.UserRole.Fleet
                && data.DOCUMENT_STATUS == (int)Enums.DocumentStatus.WaitingFleetApproval)
                return true;

            return isAllowed;
        }

        public void Approve(long TraCrfId,Login currentUser)
        {
            var data = _CrfService.GetById((int) TraCrfId);
            var dataToSave = Mapper.Map<TraCrfDto>(data);
            if (data.VEHICLE_TYPE == "BENEFIT")
            {
                if (currentUser.UserRole == Enums.UserRole.HR)
                {
                    data.DOCUMENT_STATUS = (int) Enums.DocumentStatus.WaitingFleetApproval;
                    dataToSave.DOCUMENT_STATUS = data.DOCUMENT_STATUS;
                    AddWorkflowHistory(dataToSave, currentUser, Enums.ActionType.Approve, null);

                }

                if (currentUser.UserRole == Enums.UserRole.Fleet)
                {
                    data.DOCUMENT_STATUS = (int)Enums.DocumentStatus.InProgress;
                    dataToSave.DOCUMENT_STATUS = data.DOCUMENT_STATUS;
                    AddWorkflowHistory(dataToSave, currentUser, Enums.ActionType.Approve, null);

                }
            }
            else
            {
                if (currentUser.UserRole == Enums.UserRole.Fleet)
                {
                    data.DOCUMENT_STATUS = (int)Enums.DocumentStatus.InProgress;
                    dataToSave.DOCUMENT_STATUS = data.DOCUMENT_STATUS;
                    AddWorkflowHistory(dataToSave, currentUser, Enums.ActionType.Approve, null);

                }
            }

            data.MODIFIED_BY = currentUser.USER_ID;
            data.MODIFIED_DATE = DateTime.Now;

            _uow.SaveChanges();
        }

        public void Reject(long TraCrfId, int? remark, Login currentUser)
        {
            var data = _CrfService.GetById((int)TraCrfId);
            var dataToSave = Mapper.Map<TraCrfDto>(data);
            if (data.VEHICLE_TYPE == "BENEFIT")
            {
                if (currentUser.UserRole == Enums.UserRole.HR)
                {
                    data.DOCUMENT_STATUS = (int) Enums.DocumentStatus.AssignedForUser;
                    AddWorkflowHistory(dataToSave, currentUser, Enums.ActionType.Reject, remark);

                }

                if (currentUser.UserRole == Enums.UserRole.Fleet)
                {
                    data.DOCUMENT_STATUS = (int) Enums.DocumentStatus.WaitingHRApproval;
                    AddWorkflowHistory(dataToSave, currentUser, Enums.ActionType.Reject, remark);

                }
            }
            else
            {
                if (currentUser.UserRole == Enums.UserRole.Fleet)
                {
                    data.DOCUMENT_STATUS = (int)Enums.DocumentStatus.AssignedForUser;
                    AddWorkflowHistory(dataToSave, currentUser, Enums.ActionType.Reject, remark);

                }    
            }
            

            

            _uow.SaveChanges();
        }

        public void SubmitCrf(TraCrfDto dataSubmit,Login currentUser)
        {
            var data = Mapper.Map<TRA_CRF>(dataSubmit);
            var currentDocStatus = data.DOCUMENT_STATUS;
            if (currentUser.UserRole == Enums.UserRole.HR && data.VEHICLE_TYPE.ToUpper() == "BENEFIT")
            {
                data.DOCUMENT_STATUS = (int) Enums.DocumentStatus.AssignedForUser;

            }
            
            if (currentUser.UserRole == Enums.UserRole.Fleet 
                && data.VEHICLE_TYPE.ToUpper() == "WTC" 
                && data.DOCUMENT_STATUS == (int)Enums.DocumentStatus.Draft)
            {
                data.DOCUMENT_STATUS = (int) Enums.DocumentStatus.AssignedForUser;
            }

            var dataFleet = _fleetService.GetFleetByParam(new FleetParamInput()
            {
                EmployeeId = data.EMPLOYEE_ID,
                VehicleType = data.VEHICLE_TYPE
            }).FirstOrDefault();

            if (currentUser.EMPLOYEE_ID == data.EMPLOYEE_ID
                && data.DOCUMENT_STATUS == (int) Enums.DocumentStatus.AssignedForUser)
            {
                data.DELIV_ADDRESS = dataSubmit.DELIV_ADDRESS;
                data.DELIV_CITY = dataSubmit.DELIV_CITY;
                data.DELIV_PHONE = dataSubmit.DELIV_PHONE;
                data.DELIV_PIC = dataSubmit.DELIV_PIC;
                data.WITHD_ADDRESS = dataSubmit.WITHD_ADDRESS;
                data.WITHD_CITY = dataSubmit.WITHD_CITY;
                data.WITHD_DATETIME = dataSubmit.WITHD_DATETIME;
                data.WITHD_PHONE = dataSubmit.WITHD_PHONE;
                data.WITHD_PIC = dataSubmit.WITHD_PIC;
                data.DOCUMENT_STATUS = (int) (data.VEHICLE_TYPE.ToUpper() == "WTC"
                    ? Enums.DocumentStatus.WaitingFleetApproval
                    : Enums.DocumentStatus.WaitingHRApproval);
            }
            
            

            
            data.IS_ACTIVE = true;
            
            data.BODY_TYPE = dataFleet != null ? dataFleet.BODY_TYPE : null;
            if (dataSubmit.MST_FLEET_ID == null)
            {
                data.MST_FLEET_ID = dataFleet != null ? dataFleet.MST_FLEET_ID : (long?)null;    
            }
            
            if (dataSubmit.DOCUMENT_STATUS == (int) Enums.DocumentStatus.InProgress)
            {
                if (data.EXPECTED_DATE < data.WITHD_DATETIME)
                {
                    data.DOCUMENT_STATUS = (int) Enums.DocumentStatus.InProgress;
                }
            }
            data.MODIFIED_BY = currentUser.USER_ID;
            _CrfService.SaveCrf(data, currentUser);
            var crfDto = Mapper.Map<TraCrfDto>(data);

            if (currentDocStatus != data.DOCUMENT_STATUS)
            {
                AddWorkflowHistory(crfDto, currentUser, Enums.ActionType.Submit, null);
                _uow.SaveChanges();
            }
            
            
        }

        public List<TraCrfDto> GetCrfByParam(TraCrfEpafParamInput input)
        {
            var data = _CrfService.GetList(input);

            return Mapper.Map<List<TraCrfDto>>(data);
        }

        

        private void AddWorkflowHistory(TraCrfDto input,Login currentUserLogin,Enums.ActionType action, int? RemarkId)
        {
            var dbData = new WorkflowHistoryDto();
            dbData.ACTION_BY = currentUserLogin.USER_ID;
            dbData.FORM_ID = input.TRA_CRF_ID;
            dbData.MODUL_ID = Enums.MenuList.TraCrf;
            dbData.REMARK_ID = RemarkId;
            dbData.ACTION_DATE = DateTime.Now;
            dbData.ACTION = action;
            dbData.REMARK_ID = RemarkId;

            switch (input.DOCUMENT_STATUS)
            {
                case (int)Enums.DocumentStatus.AssignedForUser:
                    SendEmailWorkflow(input, Enums.ActionType.Submit);
                    break;
                case (int)Enums.DocumentStatus.WaitingHRApproval:
                    if(action == Enums.ActionType.Approve) SendEmailWorkflow(input, Enums.ActionType.Approve);
                    else if (action == Enums.ActionType.Submit) SendEmailWorkflow(input, Enums.ActionType.Approve);
                    else SendEmailWorkflow(input, Enums.ActionType.Reject);

                    break;
                case (int)Enums.DocumentStatus.WaitingFleetApproval:
                    if (action == Enums.ActionType.Approve) SendEmailWorkflow(input, Enums.ActionType.Approve);
                    else if (action == Enums.ActionType.Submit) SendEmailWorkflow(input, Enums.ActionType.Approve);
                    else SendEmailWorkflow(input, Enums.ActionType.Reject);
                    break;
                case (int)Enums.DocumentStatus.InProgress:
                    SendEmailWorkflow(input, Enums.ActionType.Approve);
                    break;
            }

            _workflowService.Save(dbData);
            _uow.SaveChanges();

        }

        private void SendEmailWorkflow(TraCrfDto crfData,Enums.ActionType action)
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

        private FMSMailNotification ProsesMailNotificationBody(TraCrfDto crfData, Enums.ActionType action)
        {
            

            var bodyMail = new StringBuilder();
            var rc = new FMSMailNotification();

            //var vehTypeBenefit = _settingService.GetSetting().Where(x => x.SETTING_GROUP == "VEHICLE_TYPE" && x.SETTING_NAME == "BENEFIT").FirstOrDefault().MST_SETTING_ID;

            var isBenefit = crfData.VEHICLE_TYPE.ToUpper().Contains("BENEFIT");
            string creatorDataEmail = "";
            var creatorName = string.Empty;
            var webRootUrl = ConfigurationManager.AppSettings["WebRootUrl"];
            var typeEnv = ConfigurationManager.AppSettings["Environment"];
            var serverIntranet = ConfigurationManager.AppSettings["ServerIntranet"];
            var employeeData = _employeeService.GetEmployeeById(crfData.EMPLOYEE_ID);

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

            var hrQueryEmail = "SELECT EMAIL FROM " + serverIntranet + ".[dbo].[tbl_ADSI_User] WHERE FULL_NAME IN (" + hrList + ")";
            var fleetQueryEmail = "SELECT EMAIL FROM " + serverIntranet + ".[dbo].[tbl_ADSI_User] WHERE FULL_NAME IN (" + fleetList + ")";
            var creatorQuery = "SELECT EMAIL, INTERNAL_EMAIL from " + serverIntranet + ".[dbo].[tbl_ADSI_User] where FULL_NAME like 'PMI\\" +
                crfData.CREATED_BY + "'";
            if (typeEnv == "VTI")
            {
                hrQueryEmail = "SELECT EMAIL FROM EMAIL_FOR_VTI WHERE FULL_NAME IN (" + hrList + ")";
                fleetQueryEmail = "SELECT EMAIL FROM EMAIL_FOR_VTI WHERE FULL_NAME IN (" + fleetList + ")";
                creatorQuery = "SELECT EMAIL, DISPLAY_NAME FROM LOGIN_FOR_VTI WHERE LOGIN like '" + crfData.CREATED_BY + "'";
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

            query = new SqlCommand(creatorQuery, con);
            reader = query.ExecuteReader();
            while (reader.Read())
            {
                creatorDataEmail = reader[0].ToString();
                creatorName = reader[1].ToString();
            }

            reader.Close();
            con.Close();

            var receiver = "";
            var sender = "";

            switch (action)
            {
                case Enums.ActionType.Submit:
                    //if submit from HR to EMPLOYEE
                    if (isBenefit)
                    {
                        rc.Subject = crfData.DOCUMENT_NUMBER + " - Benefit Car Relocation";

                        bodyMail.Append("Dear " + crfData.EMPLOYEE_NAME + ",<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Kindly be advised due to your relocation, you are entitled to move your " + crfData.VEHICLE_USAGE + ".<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Please confirm the relocation details, and fill in the information for the withdrawal and delivery <a href='" + webRootUrl + "/TraCrf/Edit/" + crfData.TRA_CRF_ID + "?isPersonalDashboard=True'>HERE.</a><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("For any assistance please contact " + creatorName + "<br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Thanks<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Regards,<br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("HR Team");
                        bodyMail.AppendLine();

                        rc.To.Add(employeeData.EMAIL_ADDRESS);
                        if (crfData.VEHICLE_TYPE == "BENEFIT")
                        {
                            foreach (var item in hrEmailList)
                            {
                                rc.CC.Add(item);
                            }
                        }
                        else
                        {
                            foreach (var item in fleetEmailList)
                            {
                                rc.CC.Add(item);
                            }
                        }
                        
                    }
                    //if submit from FLEET to EMPLOYEE
                    else if (!isBenefit)
                    {
                        rc.Subject = crfData.DOCUMENT_NUMBER + " - Operational Vehicle Relocation";

                        bodyMail.Append("Dear " + crfData.EMPLOYEE_NAME + ",<br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("new operational car has been recorded as " + crfData.DOCUMENT_NUMBER + "<br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Please submit detail vehicle information <a href='" + webRootUrl + "/TraCrf/Edit/" + crfData.TRA_CRF_ID + "?isPersonalDashboard=True'>HERE.</a><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("We kindly ask you to complete the form back to within 7 calendar days<br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("For any assistance please contact Fleet Name<br />");
                        bodyMail.AppendLine();

                        rc.To.Add(employeeData.EMAIL_ADDRESS);

                        foreach (var item in fleetEmailList)
                        {
                            rc.CC.Add(item);
                        }
                    }
                    break;
                case Enums.ActionType.Approve:

                    if (crfData.DOCUMENT_STATUS != (int) Enums.DocumentStatus.InProgress)
                    {
                        
                        rc.Subject = "CRF - Request Approval";
                        if (crfData.VEHICLE_TYPE == "BENEFIT" &&
                            crfData.DOCUMENT_STATUS == (int) Enums.DocumentStatus.WaitingHRApproval)
                        {
                            receiver = crfData.CREATED_BY;
                            sender = employeeData.FORMAL_NAME;
                        }
                        else if (crfData.VEHICLE_TYPE == "BENEFIT" &&
                                 crfData.DOCUMENT_STATUS == (int) Enums.DocumentStatus.WaitingFleetApproval)
                        {
                            receiver = "Fleet Team";
                            sender = "HR Team";
                        }
                        else
                        {
                            receiver = "Fleet Team";
                            sender = employeeData.FORMAL_NAME;
                        }

                        bodyMail.Append("Dear " + receiver + ",<br /><br />");
                        
                        bodyMail.AppendLine();
                        bodyMail.Append("You have received new car relocation request<br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Send confirmation by clicking <a href='" + webRootUrl + "/TraCrf/Edit/" + crfData.TRA_CRF_ID + "?isPersonalDashboard=True'>Here</a>.<br />");
                        bodyMail.AppendLine();
                        //bodyMail.Append("<a href='" + webRootUrl + "/TraCrf/Edit/" + crfData.TRA_CRF_ID + "?isPersonalDashboard=True'>" +
                        //                crfData.DOCUMENT_NUMBER + "</a> requested by " + crfData.EMPLOYEE_NAME +
                        //                "<br /><br />");
                        //bodyMail.AppendLine();
                        bodyMail.Append("Thanks<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Regards,<br />");
                        bodyMail.AppendLine();
                        bodyMail.Append(sender);
                        bodyMail.AppendLine();



                        if (crfData.VEHICLE_TYPE == "BENEFIT" &&
                            crfData.DOCUMENT_STATUS == (int) Enums.DocumentStatus.WaitingHRApproval)
                        {
                            rc.To.Add(creatorDataEmail);
                            rc.CC.Add(employeeData.EMAIL_ADDRESS);
                        }
                        else
                        {
                            foreach (var item in fleetEmailList)
                            {
                                rc.To.Add(item);
                            }
                            rc.CC.Add(creatorDataEmail);
                            rc.CC.Add(employeeData.EMAIL_ADDRESS);
                        }
                    }
                    else
                    {
                        var vendorData = _vendorService.GetByShortName(crfData.VENDOR_NAME);

                        rc.Subject = "CRF - Relocation Request, need confirmation.";

                        bodyMail.Append("Dear " + vendorData.VENDOR_NAME + ",<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Below are the details of vehicle relocation request :<br />");
                        bodyMail.AppendLine();

                        if (crfData.CHANGE_POLICE_NUMBER.HasValue && crfData.CHANGE_POLICE_NUMBER.Value)
                        {
                            
                            bodyMail.Append(string.Format("Old Police number : {0} <br />", crfData.POLICE_NUMBER));
                            bodyMail.Append(string.Format("New Police number : {0} <br />", crfData.NEW_POLICE_NUMBER));
                        }
                        else
                        {
                            bodyMail.Append(string.Format("Police number : {0} <br />", crfData.POLICE_NUMBER));
    
                        }

                        
                        bodyMail.AppendLine();
                        bodyMail.Append(string.Format("Current Location : {0} - {1} <br />", crfData.LOCATION_CITY, crfData.LOCATION_OFFICE));
                        bodyMail.AppendLine();
                        bodyMail.Append(string.Format("Destination Location : {0} - {1} <br />", crfData.LOCATION_CITY_NEW, crfData.LOCATION_OFFICE_NEW));
                        bodyMail.Append(string.Format("Withdrawal Date : {0} <br />", crfData.WITHD_DATETIME));
                        bodyMail.Append(string.Format("Expected Delivery Date : {0} <br />", crfData.EFFECTIVE_DATE));
                        bodyMail.AppendLine();
                        bodyMail.Append("Reply this email for your confirmation.");
                        bodyMail.AppendLine();
                        bodyMail.Append("Thanks<br /><br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Regards,<br />");
                        bodyMail.AppendLine();
                        bodyMail.Append("Fleet Team");
                        bodyMail.AppendLine();


                        foreach (var item in fleetEmailList)
                        {
                            rc.CC.Add(item);
                        }
                        rc.CC.Add(creatorDataEmail);
                        rc.To.Add(vendorData.EMAIL_ADDRESS);
                    }
                    
                    

                        
                    
                    
                    break;
                case Enums.ActionType.Reject:

                    if (crfData.DOCUMENT_STATUS == (int) Enums.DocumentStatus.WaitingHRApproval)
                    {
                        sender = "HR Team";
                        receiver = employeeData.FORMAL_NAME;
                        rc.To.Add(employeeData.EMAIL_ADDRESS);
                        rc.CC.Add(creatorDataEmail);
                    }
                    
                    if (crfData.DOCUMENT_STATUS == (int)Enums.DocumentStatus.WaitingFleetApproval 
                        && crfData.VEHICLE_TYPE == "WTC"
                        )
                    {
                        //foreach (var item in fleetList)
                        //{
                        //    rc.CC.Add(item);
                        //}
                        sender = "Fleet Team";
                        receiver = employeeData.FORMAL_NAME;
                        rc.To.Add(employeeData.EMAIL_ADDRESS);
                        rc.CC.Add(creatorDataEmail);
                    }

                    if (crfData.DOCUMENT_STATUS == (int)Enums.DocumentStatus.WaitingFleetApproval
                        && crfData.VEHICLE_TYPE == "BENEFIT"
                        )
                    {
                        foreach (var item in fleetEmailList)
                        {
                            rc.CC.Add(item);
                        }
                        rc.To.Add(creatorDataEmail);
                        sender = "Fleet Team";
                        receiver = "HR Team";
                    }

                    rc.Subject = "CRF - Request Rejected";

                    bodyMail.Append("Dear " + receiver + ",<br /><br />");
                    bodyMail.AppendLine();
                    bodyMail.Append("Your car relocation request has been rejected.<br />");
                    bodyMail.AppendLine();
                    bodyMail.Append("Please fix your data and resubmit by clicking <a href='" + webRootUrl + "/TraCrf/Edit/" + crfData.TRA_CRF_ID + "?isPersonalDashboard=True'>Here</a>.<br />");
                    bodyMail.AppendLine();
                    //bodyMail.Append("<a href='" + webRootUrl + "/TraCrf/Edit/" + crfData.TRA_CRF_ID + "?isPersonalDashboard=True'>Here</a> requested by " + crfData.EMPLOYEE_NAME + "<br /><br />");
                    //bodyMail.AppendLine();
                    bodyMail.Append("Thanks<br /><br />");
                    bodyMail.AppendLine();
                    bodyMail.Append("Regards,<br />");
                    bodyMail.AppendLine();
                    bodyMail.Append(sender);
                    bodyMail.AppendLine();
                        
                    

                    

                    break;
                case Enums.ActionType.Completed:
                    rc.Subject = "CRF - Request Completed";

                    bodyMail.Append("Dear " + employeeData.FORMAL_NAME + ",<br /><br />");
                    bodyMail.AppendLine();
                    bodyMail.Append("Your car relocation request has been completed.<br />");
                    bodyMail.AppendLine();
                    bodyMail.Append("If you need to see your request reference click <a href='" + webRootUrl + "/TraCrf/Edit/" + crfData.TRA_CRF_ID + "?isPersonalDashboard=True'>Here</a>.<br />");
                    bodyMail.AppendLine();
                    //bodyMail.Append("<a href='" + webRootUrl + "/TraCrf/Details/" + crfData.TRA_CRF_ID + "?isPersonalDashboard=True'>Here</a> requested by " + crfData.EMPLOYEE_NAME + "<br /><br />");
                    //bodyMail.AppendLine();
                    bodyMail.Append("Thanks<br /><br />");
                    bodyMail.AppendLine();
                    bodyMail.Append("Regards,<br />");
                    bodyMail.AppendLine();
                    bodyMail.Append("Fleet Team");
                    bodyMail.AppendLine();

                    rc.To.Add(employeeData.EMAIL_ADDRESS);
                    rc.CC.Add(creatorDataEmail);

                    foreach (var item in fleetEmailList)
                    {
                        rc.CC.Add(item);
                    }

                    break;
            }

            rc.Body = bodyMail.ToString();

            if (rc.CC.Count > 0)
            {
                rc.IsCCExist = true;
            }

            return rc;
        }

        public List<EpafDto> GetCrfEpaf(bool isActive = true)
        {
            var data = _epafService.GetEpafByDocumentType(Enums.DocumentType.CRF);
            var dataEpaf = Mapper.Map<List<EpafDto>>(data);

            

            var dataCrf = GetList();

            var dataJoin = (from ep in dataEpaf
                             join crf in dataCrf on ep.MstEpafId equals crf.EPAF_ID
                             select new EpafDto() {
                                 CrfNumber = crf.DOCUMENT_NUMBER,
                                MstEpafId = ep.MstEpafId,
                                 CrfStatus = Utils.EnumHelper.GetDescription((Enums.DocumentStatus)crf.DOCUMENT_STATUS),
                                EmployeeId = crf.EMPLOYEE_ID,
                                EmployeeName = crf.EMPLOYEE_NAME,
                                CityNew = ep.City,
                                BaseTownNew = ep.BaseTown,
                                BaseTown = crf.LOCATION_OFFICE,
                                City = crf.LOCATION_CITY,
                                CrfId = crf.TRA_CRF_ID
                             }).ToList();

            var dataEmployee = _employeeService.GetEmployee();
            

            foreach (var dtEp in dataEpaf)
            {
                var dataCrfJoin = dataJoin.Where(x=> x.MstEpafId == dtEp.MstEpafId).FirstOrDefault();
                if (dataCrfJoin != null)
                {
                    dtEp.CrfId = dataCrfJoin.CrfId;
                    dtEp.CrfNumber = dataCrfJoin.CrfNumber;
                    dtEp.CrfStatus = dataCrfJoin.CrfStatus;
                    dtEp.BaseTownNew = dataCrfJoin.BaseTownNew;
                    dtEp.CityNew = dataCrfJoin.CityNew;
                    dtEp.City = dataCrfJoin.City;
                    dtEp.BaseTown = dataCrfJoin.BaseTown;
                }
                else
                {
                    var employee = dataEmployee.Where(x=>x.EMPLOYEE_ID == dtEp.EmployeeId).FirstOrDefault();
                    var baseTownNew = dtEp.BaseTown;
                    var cityNew = dtEp.City;
                    dtEp.BaseTown = employee.BASETOWN;
                    dtEp.City = employee.CITY;
                    dtEp.BaseTownNew = baseTownNew;
                    dtEp.CityNew = cityNew;
                }
                
            }

            return dataEpaf;
        }


        public List<TraCrfDto> GetCrfPersonal(Login CurrentUser)
        {
            var allData = this.GetList();
            var data = allData.Where(x=> x.IS_ACTIVE 
                && (x.EMPLOYEE_ID == CurrentUser.EMPLOYEE_ID 
                || x.CREATED_BY == CurrentUser.USER_ID)).ToList();
            
            var dataIds = data.Select(x => x.TRA_CRF_ID).ToList();
            var dataWorkflow = _workflowService.GetWorkflowHistoryByUser((int) Enums.DocumentType.CRF, CurrentUser.USER_ID);
            var formIdList = dataWorkflow.Where(x=> x.FORM_ID != null && !dataIds.Contains(x.FORM_ID.Value)).GroupBy(x=> x.FORM_ID).Select(x=> x.Key).ToList();


            var myWorkflowData =  allData.Where(x => formIdList.Contains(x.TRA_CRF_ID)).ToList();
            data.AddRange(myWorkflowData);
            return data;
        }


        public TemporaryDto SaveTemp(TemporaryDto item,DateTime expectedDate, Login CurrentUser)
        {
            TRA_TEMPORARY model;
            if (item == null)
            {
                throw new Exception("Invalid Data Entry");
            }

            try
            {
                if (item.TRA_TEMPORARY_ID > 0)
                {
                    //update
                    model = _temporaryService.GetTemporaryById(item.TRA_TEMPORARY_ID);

                    if (model == null)
                        throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

                    Mapper.Map<TemporaryDto, TRA_TEMPORARY>(item, model);
                }
                else
                {
                    var inputDoc = new GenerateDocNumberInput();
                    inputDoc.Month = DateTime.Now.Month;
                    inputDoc.Year = DateTime.Now.Year;
                    inputDoc.DocType = (int)Enums.DocumentType.TMP;

                    item.DOCUMENT_NUMBER_TEMP = _docNumberService.GenerateNumber(inputDoc);
                    item.IS_ACTIVE = true;

                    model = Mapper.Map<TRA_TEMPORARY>(item);
                }

                _temporaryService.saveTemporary(model, CurrentUser);
                var data = _CrfService.GetByNumber(item.DOCUMENT_NUMBER_RELATED);
                data.EXPECTED_DATE = expectedDate;
                _CrfService.SaveCrf(data,null);
                _uow.SaveChanges();
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return Mapper.Map<TemporaryDto>(model);
        }


        public List<TemporaryDto> GetTempByCsf(string docNumber)
        {
            var tempData = _temporaryService.GetAllTemp().Where(x => x.DOCUMENT_NUMBER_RELATED == docNumber).ToList();

            return Mapper.Map<List<TemporaryDto>>(tempData);
        }

        #region ----------- Batch Email -----------
        public bool BatchEmailCrf(List<TraCrfDto> ListCrf, string Vendor, string AttachmentWtc, string AttachmentBenefit)
        {

            var rc = new FMSMailNotification();
            var bodyMail = new StringBuilder();
            var CC = ConfigurationManager.AppSettings["CC_MAIL"];
            var AttchmentList = new List<string>();

            var GetVendor = _vendorService.GetVendor().Where(x => (x.VENDOR_NAME == null ? "" : x.VENDOR_NAME.ToUpper()) == (Vendor == null ? "" : Vendor.ToUpper()) && x.IS_ACTIVE).FirstOrDefault();
            var EmailVendor = (GetVendor == null ? "" : GetVendor.EMAIL_ADDRESS);
            bool isSend = false;
            rc.Subject = "CRF " + DateTime.Now.ToString("dd-MMM-yyyy HH:mm");

            bodyMail.Append("Dear Vendor " + Vendor + ",<br /><br />");
            bodyMail.AppendLine();
            bodyMail.Append("Bellow are list of CRF Requests<br />");
            bodyMail.AppendLine();
            bodyMail.Append("Please find the detail in attached document<br />");
            bodyMail.AppendLine();
            bodyMail.Append("<table>");
            bodyMail.AppendLine();
            bodyMail.Append("<tr><td style = 'border: 1px solid black; padding : 5px' >Doc No</td><td style = 'border: 1px solid black; padding : 5px' >Effective Date</td><td style = 'border: 1px solid black; padding : 5px' >Police Number</td><td style = 'border: 1px solid black; padding : 5px' >Employee Name</td><td style = 'border: 1px solid black; padding : 5px' >Current Basetown</td><td style = 'border: 1px solid black; padding : 5px' >Vehicle Type</td></tr>");
            bodyMail.AppendLine();
            foreach (var CtfDoc in ListCrf)
            {
                bodyMail.Append("<tr><td style = 'border: 1px solid black; padding : 5px' >" + CtfDoc.DOCUMENT_NUMBER + "</td><td style = 'border: 1px solid black; padding : 5px' >" + (CtfDoc.EFFECTIVE_DATE == null ? "" : CtfDoc.EFFECTIVE_DATE.Value.ToString("dd-MMM-yyyy")) + "</td><td style = 'border: 1px solid black; padding : 5px' >" + CtfDoc.POLICE_NUMBER + "</td><td style = 'border: 1px solid black; padding : 5px' >" + CtfDoc.EMPLOYEE_NAME + "</td><td style = 'border: 1px solid black; padding : 5px' >" + CtfDoc.LOCATION_OFFICE+ "</td><td style = 'border: 1px solid black; padding : 5px' >" + CtfDoc.VEHICLE_TYPE + "</td></tr>");
                bodyMail.AppendLine();
            }
            bodyMail.Append("</table>");
            bodyMail.AppendLine();
            bodyMail.Append("<br /><br />Thank you <br />");
            bodyMail.AppendLine();
            bodyMail.Append("Best Regards,<br />");
            bodyMail.AppendLine();
            bodyMail.Append("Fleet Team");
            bodyMail.AppendLine();

            rc.IsCCExist = false;
            rc.Body = bodyMail.ToString();

            rc.To.Add(EmailVendor);
            rc.CC.Add(CC);

            if (rc.CC.Count > 0) rc.IsCCExist = true;

            if (AttachmentWtc != null)
            {
                AttchmentList.Add(AttachmentWtc);
            }

            if (AttachmentBenefit != null)
            {
                AttchmentList.Add(AttachmentBenefit);
            }

            if (rc.IsCCExist)
                //Send email with CC
                isSend = _messageService.SendEmailToListWithCC(rc.To, rc.CC, rc.Subject, rc.Body, true, AttchmentList);
            else
                isSend = _messageService.SendEmailToList(rc.To, rc.Subject, rc.Body, true);

            return isSend;
        }
        #endregion  
    }
}
