/**
 * Created by wengzhilai on 2016/8/7.
 */
mainService.factory('insureFac', function (CarIn, $rootScope, storageUserFac, common) {
    var searCarUrl = CarIn.api + CarIn.QueryCar,
        searInsureUrl = CarIn.api + CarIn.QueryInsure,
        OrderInsureSingleUrl = CarIn.api + CarIn.OrderInsureSingle,
        OrderInsureSaveUrl = CarIn.api + CarIn.OrderInsureSave,
        getOrderListUrl = CarIn.api + CarIn.OrderList,
        OrderInsureListUrl = CarIn.api + CarIn.OrderInsureList,
        OrderSingleUrl=CarIn.api + CarIn.OrderSingle
        msg = {};
    return {
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
        getOrderList: function (obj) {
            common.showLoading();
            obj.authToken = storageUserFac.getUserAuthToken(),
                obj.pageSize = CarIn.pageSize;
            console.log(obj);
            return $.post(getOrderListUrl, obj,
                //回调函数
                function (response) {
                    console.log(response);
                    if (response.IsError) {
                        common.showError(response);
                    } else {
                        msg = response;
                        $rootScope.$broadcast('insureList.Update');
                    }
                },
                //返回类型
                "json").error(function (err) {
                    msg = {IsError: true, Message: '获取投保订单列表失败'};
                    common.showError(msg);
                });

        },
        getOrderInsureList: function (obj) {
            common.showLoading();
            obj.authToken = storageUserFac.getUserAuthToken();
            obj.pageSize = CarIn.pageSize;
            console.log(obj);
            return $.post(OrderInsureListUrl, obj,
                //回调函数
                function (response) {
                    console.log(response);
                    if (response.IsError) {
                        common.showError(response);
                    } else {
                        msg = response;
                        $rootScope.$broadcast('insureList.Update');
                    }
                },
                //返回类型
                "json").error(function (err) {
                    msg = {IsError: true, Message: '获取投保订单列表失败'};
                    common.showError(msg);
                });

        },
        OrderInsureSave: function (ent) {
            common.showLoading();
            var obj = {
                userId: 0,
                authToken: storageUserFac.getUserAuthToken(),
                saveKeys: 'CAR_ID,CLIENT_ID,INSURER_ID,DELIVERY,SaveProductId',
                entity: ent
            };
            return $.post(OrderInsureSaveUrl, obj,
                //回调函数
                function (response) {
                    console.log(response);
                    if (response.IsError) {
                        common.showError(response);
                    } else {
                        msg = response;
                        $rootScope.$broadcast('Insure.OrderInsureSave');
                    }
                },
                //返回类型
                "json").error(function (err) {
                    msg = {IsError: true, Message: '保存保险订单出错！'};
                    common.showError(msg);
                });
        },
        OrderInsureSingle: function (id) {
            common.showLoading();
            var obj = {
                userId: 0,
                authToken: storageUserFac.getUserAuthToken(),
                id: id
            };
            return $.post(OrderInsureSingleUrl, obj,
                //回调函数
                function (response) {
                    console.log(response);
                    if (response.IsError) {
                        common.showError(response);
                    } else {
                        msg = response;
                        $rootScope.$broadcast('Insure.OrderSingle');
                    }
                },
                //返回类型
                "json").error(function (err) {
                    msg = {IsError: true, Message: '查询保险订单出错！'};
                    common.showError(msg);
                });
        },
        getInsureByCar: function (searObj) {
            common.showLoading();
            var obj = {
                userId: 0,
                authToken: storageUserFac.getUserAuthToken(),
                entity: searObj
            };
            return $.post(searInsureUrl, obj,
                //回调函数
                function (response) {
                    console.log(response);
                    if (response.IsError) {
                        common.showError(response);
                    } else {
                        msg = response;
                        $rootScope.$broadcast('Insure.SearInsureByCarUpdate');
                    }
                },
                //返回类型
                "json").error(function (err) {
                    msg = {IsError: true, Message: '查询保险公司出错！'};
                    common.showError(msg);
                });
        },
        carSearch: function (searObj) {
            common.showLoading();
            var obj = {
                userId: 0,
                authToken: user.authToken,
                id: 0,
                para: [
                    {
                        K: 'lateNumber',
                        V: searObj.lateNumber,
                        T: '=='
                    },
                    {
                        K: 'userName',
                        V: searObj.userName,
                        T: '=='
                    }
                ]
            };
            return $.post(searCarUrl, obj,
                //回调函数
                function (response) {
                    console.log(response);
                    if (response.IsError) {
                        common.showError(response);
                    } else {
                        msg = response;
                        $rootScope.$broadcast('Insure.SearUpdate');
                    }
                },
                //返回类型
                "json").error(function (err) {
                    msg = {IsError: true, Message: '查询车辆信息出错！'};
                    common.showError(msg);
                });

        },
        getCurrentMes: function () {
            common.hideLoading();
            return msg;
        }
    }
})