/**
 * Created by wengzhilai on 2016/10/12.
 */
mainService.factory('customerFac', function (CarIn, $rootScope, storageUserFac, common) {
    var msg = {},
    //保存客户url
        saveUrl = CarIn.api + CarIn.SalesmanClientAdd,
    //根据id查客户url
        getIdUrl = CarIn.api + CarIn.ClientSingle,
    //查找业务员的所有客户
        getUserUrl = CarIn.api + CarIn.getUserUrl,
    //修改用户密码
        rePasUrl = CarIn.api + CarIn.UserEditPwd;
    return {
        getCustomerList: function (obj) {
            common.showLoading();
            obj.authToken = storageUserFac.getUserAuthToken();
            obj.pageSize = CarIn.pageSize;
            console.log(obj);
            return $.post(getUserUrl, obj,
                //回调函数
                function (response) {
                    console.log(response);
                    if (response.IsError) {
                        common.showError(response);
                    } else {
                        msg = response;
                        $rootScope.$broadcast('CustomerList.Update');
                    }
                },
                //返回类型
                "json").error(function (err) {
                    common.showError(err);
                });

        },
        resetUserPas: function (id) {
            common.showLoading();
            var obj = {
                userId: id,
                authToken: storageUserFac.getUserAuthToken(),
                entity: CarIn.initPwd
            };
            return $.post(rePasUrl, obj,
                //回调函数
                function (response) {
                    console.log(response);
                    if (response.IsError) {
                        common.showError(response);
                    } else {
                        msg = response;
                        common.showError("密码重置成功为" + CarIn.initPwd);
                    }
                },
                //返回类型
                "json").error(function (err) {
                    common.showError(err);
                });
        },
        getCustomerById: function (id) {
            common.showLoading();
            var obj = {
                userId: 0,
                authToken: storageUserFac.getUserAuthToken(),
                id: id
            };
            return $.post(getIdUrl, obj,
                //回调函数
                function (response) {
                    console.log(response);
                    if (response.IsError) {
                        common.showError(response);
                    } else {
                        msg = response;
                        $rootScope.$broadcast('Customer.GetIdUpdate');
                    }
                },
                //返回类型
                "json").error(function (err) {
                    common.showError(err);
                });
        },
        getCustomerByUser: function () {
            common.showLoading();
            var obj = {
                authToken: storageUserFac.getUserAuthToken()
            }

            return $.post(getUserUrl, obj,
                //回调函数
                function (response) {
                    console.log(response);
                    if (response.IsError) {
                        common.showError(response);
                    } else {
                        msg = response;
                        $rootScope.$broadcast('Customer.GetUserUpdate');
                    }
                },
                //返回类型
                "json").error(function (err) {
                    common.showError(err);
                });
        },
        hasNextPage: function () {
            if (msg.totalPage <= msg.currentPage) {
                return false;
            }
            if (msg.IsError == true) {
                return false;
            } else {
                return true;
            }
        },
        save: function (ent) {
            common.showLoading();
            var obj = {
                userId: 0,
                authToken: storageUserFac.getUserAuthToken(),
                saveKeys: 'ID,NAME,SEX,phone,ADDRESS,NowCar,LOGIN_NAME',
                entity: ent
            };
            return $.post(saveUrl, obj,
                //回调函数
                function (response) {
                    console.log(response);
                    if (response.IsError) {
                        common.showError(response);
                    } else {
                        msg = response;
                        $rootScope.$broadcast('Customer.Update');
                    }
                },
                //返回类型
                "json").error(function (err) {
                    common.showError(err);
                });
        }, getCurrentMes: function () {
            common.hideLoading();
            return msg;
        }

    };
})