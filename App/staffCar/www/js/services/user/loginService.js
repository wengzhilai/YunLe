/**
 * Created by wengzhilai on 2016/8/7.
 */
mainService.factory('userFac', function ($rootScope, CarIn, common) {
    var loginUrl = CarIn.api + CarIn.SLogin,
        findPwdUrl = CarIn.api + CarIn.findPwd,
        reUrl = CarIn.api + CarIn.LoginReg,
        msg = {};
    return {
        login: function (obj) {
            common.showLoading();
            common.clearAll();
            return $.post(loginUrl, obj,
                //回调函数
                function (response) {
                    console.log(response);
                    if (response.IsError) {
                        common.showError(response);
                    } else {
                        msg = response;
                        $rootScope.$broadcast('User.loginUpdated');
                    }
                },
                //返回类型
                "json").error(function (err) {
                    msg = {IsError: true, Message: '登录失败'};
                    console.log(msg);
                });
        },
        findPwd: function (obj) {
            common.showLoading();
            return $.post(findPwdUrl, obj,
                //回调函数
                function (response) {
                    console.log(response);
                    msg = response;
                    $rootScope.$broadcast('findPwd.Update');
                },
                //返回类型
                "json").error(function (err) {
                    msg = {IsError: true, Message: '登录失败'};
                    $rootScope.$broadcast('findPwd.Update');
                });
        },
        register: function (obj) {
            common.showLoading();
            return $.post(reUrl, obj,
                //回调函数
                function (response) {
                    console.log(response);
                    if (response.IsError) {
                        common.showError(response);
                    } else {
                        msg = response;
                        $rootScope.$broadcast('User.loginReg');
                    }
                },
                //返回类型
                "json").error(function (err) {
                    msg = {IsError: true, Message: '登录失败'};
                    common.showError(msg);
                });
        },
        getCurrentMes: function () {
            common.hideLoading();
            return msg;
        }
    }
});