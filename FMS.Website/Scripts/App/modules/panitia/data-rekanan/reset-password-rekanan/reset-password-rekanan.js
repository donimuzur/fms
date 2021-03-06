angular.module('eprocAppPanitia')
.controller( 'resetPasswordRekananCtrl', function( $scope, $http, $cookieStore, $state, $rootScope ){
    $scope.totalRecords = 0;
    $scope.currentPage = 1;
    $scope.fullSize = 10;

    $scope.data = [];
    $scope.menuhome = 0;

    $scope.initilize = function(){
        $scope.menuhome = $rootScope.menuhome;
        $rootScope.getSession().then(function(result){
            $rootScope.userSession = result.data.data;
            $rootScope.userLogin = $rootScope.userSession.username;
            $rootScope.authorize(loadAwal());
        });
    }; // end initilize
    
    function loadAwal(){
        /*dikomen dulu nunggu API
         * eb.send('itp.rekanan.getresetcountuser', '', function(rpl) {
            if (rpl.status === 'ok') {
                $scope.totalRecords = rpl.result[0].jumlah;
                $scope.$apply();
            }
        });

        eb.send('itp.rekanan.getresetalluser', {
            limit: $scope.fullSize,
            offset: 0
        }, function(rpl) {
            if (rpl.status === 'ok') {
                $scope.data = rpl.result;
                $scope.$apply();
                 $rootScope.unloadLoading();
            }
            else $rootScope.unloadLoading();
        });
        */
    }

    $scope.ubah = function(obj) {
        bootbox.confirm("Yakin mereset password ?", function(res) {
            if (res) {
                var gntpswd = $.md5('rekanan12345');
                eb.send('itp.rekanan.setnewdefaultpswd', {
                    username: obj.nama_user,
                    pswd: gntpswd,
                    id_reset: obj.id_reset
                }, function(rpl) {
                    if (rpl.status === 'ok') {
                        eb.send('itp.rekanan.getEmail', {
                            username: obj.nama_user
                        }, function(getemail) {
                            if (getemail.status === 'ok') {
                                $scope.email_pribadi = getemail.result[0].email_pribadi;

                                $.growl.notice({title: "[INFO]", message: "Reset password berhasil"});
                                var variables = [];

                                eb.send("itp.mailconfig.getMailContent", // SEND EMAIL
                                        {
                                            id_konten_email: 7,
                                            variables: variables
                                        }, function(reply) {
                                    var mailBody = "";
                                    var mailSubject = "";
                                    if (reply.status === 'ok') {
                                        mailBody = reply.result.mailBody;
                                        mailSubject = reply.result.mailSubject;
                                    }
                                    eb.send('itp.rekanan.sendNotification', {
                                        from: $rootScope.email,
                                        to: $scope.email_pribadi,
                                        subject: mailSubject,
                                        body: mailBody
                                    }, function(adm) {
                                        if (adm.status === 'ok') {
                                            $.growl.notice({title: "[INFO]", message: "Email notifikasi berhasil terkirim"});
                                            $scope.initilize();
                                        }
                                        else $.growl.error({title: "[PERINGATAN]", message: "Email notifikasi gagal terkirim"});
                                    });
                                });
                            }
                            else $.growl.error({title: "[PERINGATAN]", message: "Email tidak ditemukan di username apapun"});
                        });
                    }
                    else $.growl.error({title: "[PERINGATAN]", message: "Reset password gagal"});
                });
            }
        });
    };

    $scope.hapusrequest = function(obj) {
        bootbox.confirm("Anda yakin menghapus permintaan reset password ini?", function(rmv) {
            if (rmv) {
                var reset_id = obj.id_reset;
                eb.send('itp.rekanan.deleterequest', {
                    reset: reset_id
                }, function(reply) {
                    if (reply.status === 'ok') {
                        $.growl.notice({title: "[INFO]", message: "Permintaan Reset Berhasil Dihapus"});
                        $scope.initilize();
                    }
                    else $.growl.error({title: "[PERINGATAN]", message: "Data Gagal Dihapus"});
                });
            }
        });
    };

    $scope.changepage = function(page) {
        $scope.currentPage = page;
        $scope.data = [];
         $rootScope.loadLoading("Silahkan Tunggu...");
        eb.send('itp.rekanan.getresetalluser', {
            limit: $scope.fullSize,
            offset: ($scope.currentPage * $scope.fullSize) - $scope.fullSize
        }, function(rpl) {
            if (rpl.status === 'ok') {
                $scope.data = rpl.result;
                $scope.$apply();
                 $rootScope.unloadLoading();
            }
            else $rootScope.unloadLoading();
        });
    };
}); // end resetPasswordRekananCtrl