﻿(function () {
    'use strict';

    angular.module("app")
    .controller("selectEmployeeModal", ctrl);
    
    ctrl.$inject = ['$state', '$scope', '$http', '$filter', '$uibModalInstance', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'DataContractRequisitionService', 'UIControlService'];
    /* @ngInject */
    function ctrl($state, $scope, $http, $filter, $uibModalInstance, $translate, $translatePartialLoader, $location, SocketService, DataContractRequisitionService, UIControlService) {

        var vm = this;
        var loadmsg = "";
        
        vm.count;
        vm.pageNumber = 1;
        vm.pageSize = 10;
        vm.searchText = "";
        vm.pegawai = [];

        vm.onBatalClick = function () {
            $uibModalInstance.dismiss('cancel');
        };

        vm.onSearchSubmit = function (searchText) {
            vm.searchText = searchText;
            vm.loadData();
        };

        vm.init = init;
        function init() {
            $translatePartialLoader.addPart('data-contract-requisition');
            $translate.refresh().then(function () {
                loadmsg = $filter('translate')('MESSAGE.LOADING');
            });
            vm.loadData();
        };

        vm.loadData = loadData; 
        function loadData() {
            UIControlService.loadLoadingModal(loadmsg);
            DataContractRequisitionService.SelectEmployee({
                Keyword: vm.searchText,
                Limit: vm.pageSize,
                Offset: (vm.pageNumber - 1) * vm.pageSize,
                column: 1,
            }, function (reply) {
                if (reply.status === 200) {
                    vm.pegawai = reply.data;
                    DataContractRequisitionService.CountEmployee({
                        Keyword: vm.searchText,
                        column: 1
                    }, function (reply) {
                        if (reply.status === 200) {
                            vm.count = reply.data;
                        } else {
                            UIControlService.msg_growl('error', $filter('translate')('MESSAGE.ERR_LOAD_EMPLOYEE'));
                        }
                        UIControlService.unloadLoadingModal();
                    }, function (error) {
                        UIControlService.msg_growl('error', $filter('translate')('MESSAGE.ERR_LOAD_EMPLOYEE'));
                        UIControlService.unloadLoadingModal();
                    });
                } else {
                    UIControlService.msg_growl('error', $filter('translate')('MESSAGE.ERR_LOAD_EMPLOYEE'));
                    UIControlService.unloadLoadingModal();
                }
            }, function (error) {
                UIControlService.msg_growl('error', $filter('translate')('MESSAGE.ERR_LOAD_EMPLOYEE'));
                UIControlService.unloadLoadingModal();
            });
        };

        vm.onSelectClick = onSelectClick;
        function onSelectClick(owner) {
            $uibModalInstance.close(owner);
        };
    }
})();