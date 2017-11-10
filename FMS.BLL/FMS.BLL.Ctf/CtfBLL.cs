﻿using AutoMapper;
using FMS.BusinessObject;
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.BLL.Ctf
{
    public class CtfBLL : ITraCtfBLL
    {
        private IUnitOfWork _uow;
        private ICtfService _ctfService;
        private IDocumentNumberService _docNumberService;

        public CtfBLL(IUnitOfWork uow)
        {
            _uow = uow;
            _ctfService = new CtfService(uow);
            _docNumberService = new DocumentNumberService(uow);
        }

        public List<TraCtfDto> GetCtf()
        {
            var data = _ctfService.GetCtf();
            var redata = Mapper.Map<List<TraCtfDto>>(data);
            return redata;
        }

        public TraCtfDto Save(TraCtfDto Dto, string userId)
        {
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
                    var dbTraCtf =Mapper.Map<TRA_CTF>(Dto);
                    _ctfService.Save(dbTraCtf);
                }
                else
                {
                    var inputDoc = new GenerateDocNumberInput();
                    inputDoc.Month = DateTime.Now.Month;
                    inputDoc.Year = DateTime.Now.Year;
                    inputDoc.DocType = (int)Enums.DocumentType.CTF;

                    Dto.DocumentNumber = _docNumberService.GenerateNumber(inputDoc);

                    var dbTraCtf= Mapper.Map<TRA_CTF>(Dto);
                    _ctfService.Save(dbTraCtf);
                    //_repository.InsertOrUpdate(model);

                }

              

                ////set workflow history
                //var getUserRole = _poabll.GetUserRole(userId);
                //var input = new Ck4cWorkflowDocumentInput()
                //{
                //    DocumentId = model.CK4C_ID,
                //    DocumentNumber = model.NUMBER,
                //    ActionType = Enums.ActionType.Modified,
                //    UserId = userId,
                //    UserRole = getUserRole
                //};

                if (changed)
                {
                    //AddWorkflowHistory(input);
                }
                _uow.SaveChanges();
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return Dto;
        }
        public void CtfWorkflow(CtfWorkflowDocumentInput input)
        {
            //var isNeedSendNotif = true;
            switch (input.ActionType)
            {
                case Enums.ActionType.Created:
                    CreateDocument(input);
                    //isNeedSendNotif = false;
                    break;
                    //case Enums.ActionType.Submit:
                    //    SubmitDocument(input);
                    //    break;
                    //case Enums.ActionType.Approve:
                    //    ApproveDocument(input);
                    //    break;
                    //case Enums.ActionType.Reject:
                    //    RejectDocument(input);
                    //    break;
            }

            //todo sent mail
            //if (isNeedSendNotif) SendEmailWorkflow(input);

            _uow.SaveChanges();
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

            //_workflowHistoryBll.Save(dbData);

        }
    }
}
