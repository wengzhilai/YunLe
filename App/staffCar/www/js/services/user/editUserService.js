/**
 * Created by wengzhilai on 2016/8/7.
 */
mainService.factory('salesmanFac', function (CarIn, $rootScope, storageUserFac, common) {
    var msg = {},
        salesmanToTeamUrl = CarIn.api + CarIn.salesmanToTeam,
        saveUrl = CarIn.api + CarIn.SalesmanSave,
        upPasswordUrl = CarIn.api + CarIn.upPassword;

    return {
        upPassword: function (ent) {
            common.showLoading();
            var obj = {
                authToken: storageUserFac.getUserAuthToken(),
                userId: 0,
                entity: ent
            };
            console.log(obj);
            return $.post(upPasswordUrl, obj,
                //回调函数
                function (response) {
                    console.log(response);
                    if (response.IsError) {
                        common.showError(response);
                    } else {
                        msg = response;
                        $rootScope.$broadcast('salesmanUpPassword.Update');
                    }
                },
                //返回类型
                "json").error(function (err) {
                    common.showError(err);
                });
        },
        save: function (ent) {
            common.showLoading();
            //alert( window.JSON.stringify(ent));
            var obj = {
                authToken: storageUserFac.getUserAuthToken(),
                userId: 0,
                saveKeys: 'NAME,SEX,phone,ID_NO,ICON_FILES_ID,ID_NO_PIC',
                entity: ent
            };
            console.log(obj);
            return $.post(saveUrl, obj,
                //回调函数
                function (response) {
                    console.log(response);
                    if (response.IsError) {
                        common.showError(response);
                    } else {
                        msg = response;
                        $rootScope.$broadcast('salesmanSave.Update');
                    }
                },
                //返回类型
                "json").error(function (err) {
                    common.showError(err);
                });
        },
        salesmanToTeam: function (entity) {
            common.showLoading();
            var obj = {
                authToken: storageUserFac.getUserAuthToken(),
                userId: 0,
                entity: entity
            };
            console.log(obj);
            return $.post(salesmanToTeamUrl, obj,
                //回调函数
                function (response) {
                    console.log(response);
                    if (response.IsError) {
                        common.showError(response);
                    } else {
                        msg = response;
                        $rootScope.$broadcast('salesmanToTeam.Update');
                    }
                },
                //返回类型
                "json").error(function (err) {
                    common.showError(err);
                });

        },
        getCurrentMes: function () {
            common.hideLoading();
            return msg;
        }
    }
})