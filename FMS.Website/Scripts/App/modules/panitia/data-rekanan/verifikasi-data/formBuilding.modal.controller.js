(function () {
    'use strict';

    angular.module("app")
    .controller("FormBuildingController", ctrl);
    
    ctrl.$inject = ['$scope', '$http', '$uibModalInstance', '$cookieStore', '$state', '$stateParams', '$rootScope','VerifikasiDataService',
        '$uibModal', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'item',
        'ProvinsiService', 'UIControlService'];
    /* @ngInject */
    function ctrl($scope, $http, $uibModalInstance, $cookieStore, $state, $stateParams, $rootScope,VerifikasiDataService,
        $uibModal, $translate, $translatePartialLoader, $location, SocketService, item,
        ProvinsiService, UIControlService) {
        var vm = this;
        vm.dataBuilding = {};
        vm.isAdd = item.isForm;
        vm.tipeform = item.type;        
        vm.title = "Data Bangunan dan Infrastruktur";
        vm.FullAddress;
        //console.info("modal: " + JSON.stringify(item));

        vm.init = init;
        function init() {
            if (vm.isAdd === true) {
                vm.dataBuilding = new BuildingField(0, 0, 0, "", 0);
            }
            else {
                vm.ID = item.data.ID;
                vm.VendorID = item.data.VendorID;
                vm.dataBuilding = new BuildingField(item.data.VendorID, item.data.category, item.data.BuildingArea,
                    item.data.address.AddressInfo, item.data.ownershipStatus);
                vm.FullAddress = item.data.address.AddressInfo + " , " + item.data.address.AddressDetail
            }
            getProvinsi();
            loadCategory();
            loadKepemilikan();
        }

        vm.listCategory = [];
        vm.selectedCategory;
        function loadCategory() {
            UIControlService.loadLoading("Loading Data Kategori");
            VerifikasiDataService.getCategoryBuilding(
            function (reply) {
                UIControlService.unloadLoading();
                vm.listCategory = reply.data.List;
                if (vm.isAdd === false) {
                    for (var i = 0; i < vm.listCategory.length; i++) {
                        if (item.data.Category === vm.listCategory[i].RefID) {
                            vm.selectedCategory = vm.listCategory[i];
                            break;
                        }
                    }
                }
            }, function (err) {
                UIControlService.msg_growl("error", "MESSAGE.API");
                UIControlService.unloadLoading();
            });
        }

        vm.listKepemilikan = [];
        vm.selectedKepemilikan;
        function loadKepemilikan() {
            UIControlService.loadLoading("Loading Data Kepemilikan");
            VerifikasiDataService.getOwnership(
            function (reply) {
                UIControlService.unloadLoading();
                vm.listKepemilikan = reply.data.List;
                if (vm.isAdd === false) {
                    for (var i = 0; i < vm.listKepemilikan.length; i++) {
                        if (item.data.OwnershipStatus === vm.listKepemilikan[i].RefID) {
                            vm.selectedKepemilikan = vm.listKepemilikan[i];
                            break;
                        }
                    }
                }
            }, function (err) {
                UIControlService.msg_growl("error", "MESSAGE.API");
                UIControlService.unloadLoading();
            });
        }

        /* begin provinsi, kabupaten, kecamatan */
        vm.getProvinsi = getProvinsi;
        vm.selectedProvince;
        function getProvinsi() {
            ProvinsiService.getStates(360,
               function (response) {
                   vm.listProvinsi = response.data;
                   //console.info(">> " + JSON.stringify(vm.listProvinsi));
                   if (vm.isAdd === false) {
                       for (var i = 0; i < vm.listProvinsi.length; i++) {
                           if (item.data.address.StateID === vm.listProvinsi[i].StateID) {
                               vm.selectedProvince = vm.listProvinsi[i];
                               vm.changeProvince();
                               break;
                           }
                       }
                   }

               },
           function (response) {
               UIControlService.unloadLoading();
               UIControlService.msg_growl("error", "MESSAGE.API");;
           });
        }

        vm.changeProvince = changeProvince;
        vm.selectedKabupaten;
        function changeProvince() {
            ProvinsiService.getCities(vm.selectedProvince.StateID,
               function (response) {
                   vm.listKabupaten = response.data;
                   if (vm.isAdd === false) {
                       for (var i = 0; i < vm.listKabupaten.length; i++) {
                           if (item.data.address.CityID === vm.listKabupaten[i].CityID) {
                               vm.selectedKabupaten = vm.listKabupaten[i];
                               vm.changeCities();
                               break;
                           }
                       }
                   }

               },
           function (response) {
               UIControlService.unloadLoading();
               UIControlService.msg_growl("error", "MESSAGE.API");
           });

        }

        vm.changeCities = changeCities;
        vm.selectedKecamatan;
        function changeCities() {
            ProvinsiService.getDistrict(vm.selectedKabupaten.CityID,
               function (response) {
                   vm.listKecamatan = response.data;
                   if (vm.isAdd === false) {
                       for (var i = 0; i < vm.listKecamatan.length; i++) {
                           if (item.data.address.DistrictID === vm.listKecamatan[i].DistrictID) {
                               vm.selectedKecamatan = vm.listKecamatan[i];
                               break;
                           }
                       }
                   }
               },
           function (response) {
               UIControlService.unloadLoading();
               UIControlService.msg_growl("error", "MESSAGE.API");               
           });

        }
        /* end provinsi, kabupaten, kecamatan */

        vm.batal = batal;
        function batal() {
            $uibModalInstance.dismiss('cancel');
        }

        vm.saveData = saveData;
        function saveData() {
            if (vm.selectedCategory === undefined) {
                UIControlService.msg_growl("warning", "MESSAGE.NO_CATEGORY");
                return;
            }
            if (vm.selectedKepemilikan === undefined) {
                UIControlService.msg_growl("error", "MESSAGE.NO_OWNERSHIP");
                return;
            }
            if (vm.selectedKabupaten === undefined) {
                UIControlService.msg_growl("error", "MESSAGE.NO_CITY");
                return;
            }
            if (vm.selectedProvince === undefined) {
                UIControlService.msg_growl("error", "MESSAGE.NO_PROVINCE");
                return;
            }
            if (vm.selectedKecamatan === undefined) {
                UIControlService.msg_growl("error", "MESSAGE.NO_DISTRICT");
                return;
            }
            
            var addr_arr = [];
            var addr = {
                AddressDetail: vm.selectedKecamatan.Name + " , "+ vm.selectedKabupaten.Name + " , "+vm.selectedProvince.Name,
                AddressInfo: vm.dataBuilding.Address,
                StateID: vm.selectedProvince.StateID,
                CityID: vm.selectedKabupaten.CityID,
                DistrictID: vm.selectedKecamatan.DistrictID 
            }
            addr_arr.push(addr);
            if (vm.isAdd === true) {
                DataPerlengkapanService.insertBuilding({
                    addresses: addr_arr,
                    BuildingArea: vm.dataBuilding.BuildingArea,
                    Category: vm.selectedCategory.RefID,
                    OwnershipStatus: vm.selectedKepemilikan.RefID
                }, function (reply) {
                    if (reply.status === 200) {
                        UIControlService.msg_growl("success", "MESSAGE.SUCCES_SAVE_BUILDING");
                        $uibModalInstance.close();
                    }
                }, function (err) {
                    UIControlService.unloadLoading();
                    UIControlService.msg_growl("error", "MESSAGE.API");
                });
            } else {
                VerifikasiDataService.updateBuilding({
                    ID: vm.ID,
                    IsActive: true,
                    VendorID: vm.VendorID,
                    addresses: addr_arr,
                    BuildingArea: vm.dataBuilding.BuildingArea,
                    Category: vm.selectedCategory.RefID,
                    OwnershipStatus: vm.selectedKepemilikan.RefID
                }, function (reply) {
                    if (reply.status === 200) {
                        UIControlService.msg_growl("success", "MESSAGE.SUCCES_UPDATE_BUILDING");
                        $uibModalInstance.close();
                    }
                }, function (err) {
                    UIControlService.unloadLoading();
                    UIControlService.msg_growl("error", "MESSAGE.API");
                });
            }
            
        }

        function BuildingField(VendorID, Category, BuildingArea, Address, OwnershipStatus) {
            var self = this;
            self.VendorID = VendorID;
            self.Category = Category;
            self.BuildingArea = BuildingArea;
            self.Address = Address;
            self.OwnershipStatus = OwnershipStatus;
        }

    };
})();