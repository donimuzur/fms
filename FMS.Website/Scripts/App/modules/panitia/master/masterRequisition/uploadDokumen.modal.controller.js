﻿(function () {
    'use strict';

    angular.module("app").controller("UploadDokumenCtrl", ctrl);

    ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService', 
        'RFQVHSService', '$state', 'UIControlService', 'UploadFileConfigService',
        'UploaderService', 'item', '$uibModalInstance', 'GlobalConstantService'];
    function ctrl($http, $translate, $translatePartialLoader, $location, SocketService, 
        RFQVHSService, $state, UIControlService, UploadFileConfigService, 
        UploaderService, item, $uibModalInstance, GlobalConstantService) {
        var vm = this;
        //console.info("item:" + JSON.stringify(item));
        vm.fileUpload;
        vm.data = item.data;
        vm.allowEdit = item.data.StatusName == 'RFQ_DRAFT';
        vm.DocName = "";
        vm.listDoc = [];
        vm.folderFile = GlobalConstantService.getConstant('api') + "/";

        vm.init = init;
        function init() {
            loadData();
        }

        function loadData() {
            UIControlService.loadLoadingModal("MESSAGE.LOADING");
            RFQVHSService.selectDoc({ID: vm.data.ID}, function (reply) {
                UIControlService.unloadLoadingModal();
                if (reply.status === 200) {
                    vm.listDoc = reply.data;
                }
                loadTypeSizeFile();
            }, function (err) {
                UIControlService.msg_growl("error", "MESSAGE.ERR_API");
                UIControlService.unloadLoadingModal();
            });
        }

        function loadTypeSizeFile() {
            UIControlService.loadLoadingModal("MESSAGE.LOADING");
            //get tipe dan max.size file - 1
            UploadFileConfigService.getByPageName("PAGE.ADMIN.RFQVHS", function (response) {
                UIControlService.unloadLoadingModal();
                if (response.status == 200) {
                    vm.idUploadConfigs = response.data;
                    vm.idFileTypes = UIControlService.generateFilterStrings(response.data);
                    vm.idFileSize = vm.idUploadConfigs[0];

                } else {
                    UIControlService.msg_growl("error", ".MESSAGE.ERR_TYPEFILE");
                    return;
                }
            }, function (err) {
                UIControlService.msg_growl("error", "MESSAGE.API");
                UIControlService.unloadLoadingModal();
                return;
            });
        }
        /*proses upload file*/
        vm.uploadFile = uploadFile;
        function uploadFile() {
            if (validateFileType(vm.fileUpload, vm.idUploadConfigs)) {
                upload(vm.fileUpload, vm.idFileSize, vm.idFileTypes);
            }

        }

        function upload(file, config, types) {
            var size = config.Size;
            var unit = config.SizeUnitName;
            if (unit == 'SIZE_UNIT_KB') {
                size *= 1024;
            }

            if (unit == 'SIZE_UNIT_MB') {
                size *= (1024 * 1024);
            }

            UIControlService.loadLoadingModal("LOADERS.LOADING_UPLOAD_FILE");
            UploaderService.uploadSingleFileRFQVHS(vm.data.ID, file, size, types,
                function (response) {
                    UIControlService.unloadLoadingModal();
                    if (response.status == 200) {
                        var url = response.data.Url;
                        var size = response.data.FileLength;
                        //vm.dataExp.DocumentURL = url;
                        //vm.pathFile = vm.folderFile + url;
                        UIControlService.msg_growl("success", "FORM.MSG_SUC_UPLOAD");
                        saveProcess(url,size);

                    } else {
                        UIControlService.msg_growl("error", "MESSAGE.ERR_UPLOAD");
                        return;
                    }
                },
                function (response) {
                    UIControlService.msg_growl("error", "MESSAGE.API")
                    UIControlService.unloadLoadingModal();
                });

        }

        function validateFileType(file, allowedFileTypes) {
            //console.info(JSON.stringify(allowedFileTypes));
            if (!file || file.length == 0) {
                UIControlService.msg_growl("error", "MESSAGE.ERR_NOFILE");
                return false;
            }

            /*
            var selectedFileType = file[0].type;
            selectedFileType = selectedFileType.substring(selectedFileType.lastIndexOf('/') + 1);
            if (selectedFileType === "vnd.ms-excel") {
                selectedFileType = "xls";
            }
            else if (selectedFileType === "vnd.openxmlformats-officedocument.spreadsheetml.sheet") {
                selectedFileType = "xlsx";
            } else if (selectedFileType === "application/msword") {
                selectedFileType = "doc";
            }
            else if (selectedFileType === "application/vnd.openxmlformats-officedocument.wordprocessingml.document" || selectedFileType == "vnd.openxmlformats-officedocument.wordprocessingml.document") {
                selectedFileType = "docx";
            }
            else {
                selectedFileType = selectedFileType;
            }
            var allowed = false;
            for (var i = 0; i < allowedFileTypes.length; i++) {
                if (allowedFileTypes[i].Name == selectedFileType) {
                    allowed = true;
                    return allowed;
                }
            }

            if (!allowed) {
                UIControlService.msg_growl("warning", "MESSAGE.ERR_INVALID_FILETYPE");
                return false;
            }
            */
            return true;
        }
        /* end proses upload*/

        function saveProcess(docurl,docsize) {
            var senddata = {
                RFQVHSID: vm.data.ID,
                DocName: vm.DocName,
                DocUrl: docurl,
                Size: docsize
            }
            RFQVHSService.createDoc(senddata, function (reply) {
                UIControlService.unloadLoadingModal();
                if (reply.status === 200) {
                    UIControlService.msg_growl("success", "FORM.MSG_SUC_SAVE");
                    loadData();
                    //$uibModalInstance.close();
                }
                else {
                    UIControlService.msg_growl("error", "FORM.MSG_ERR_SAVE");
                    return;
                }
            }, function (err) {
                UIControlService.msg_growl("error", "MESSAGE.ERR_API");
                UIControlService.unloadLoadingModal();
            });
        }

        vm.delDoc = delDoc;
        function delDoc(data) {
            RFQVHSService.deleteDoc({ID: data.ID}, function (reply) {
                UIControlService.unloadLoadingModal();
                if (reply.status === 200) {
                    UIControlService.msg_growl("success", "Berhasil Hapus Dokumen");
                    loadData();
                    //$uibModalInstance.close();
                }
                else {
                    UIControlService.msg_growl("error", "FORM.MSG_ERR_SAVE");
                    return;
                }
            }, function (err) {
                UIControlService.msg_growl("error", "MESSAGE.ERR_API");
                UIControlService.unloadLoadingModal();
            });
        }

        vm.batal = batal;
        function batal() {
            $uibModalInstance.dismiss('cancel');
        };
    }
})();