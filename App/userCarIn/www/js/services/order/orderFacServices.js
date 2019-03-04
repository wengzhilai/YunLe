/**
 * Created by wengzhilai on 2016/8/11.
 */
mainService.factory('orderFac', function (CarIn, $rootScope, storageUserFac, common) {
    var url = CarIn.api + CarIn.OrderList,
        orderRescueListUrl = CarIn.api + CarIn.OrderRescueList,
        orderSaveUrl = CarIn.api + CarIn.orderSave,
        helpOrderSaveUrl = CarIn.api + CarIn.toHelpUrl,
        maintainOrderSaveUrl = CarIn.api + CarIn.toHelpUrl,
        rescueSingleUrl = CarIn.api + CarIn.rescueSingle,
        trialOrderSaveUrl = CarIn.api + CarIn.orderSave,
        OrderSingleUrl=CarIn.api + CarIn.OrderSingle,
    msg = {};
    return {
        finish: function () {
            var obj = {
                userId: 0,
                authToken: storageUserFac.getUserAuthToken(),
                saveKeys: 'STATUS',
                entity: {
                    STATUS: '完成'
                }
            };
            return $.post(maintainOrderSaveUrl, obj,
                //回调函数
                function (response) {
                    console.log(response);
                    if (response.IsError) {
                        common.showError(response);
                    } else {
                        msg = response;
                        $rootScope.$broadcast('finish.Update');
                    }
                },
                //返回类型
                "json").error(function (err) {
                    common.showError(err);
                });
        },
        trialOrderSave: function (ent) {
            common.showLoading();
            var obj = {
                userId: 0,
                authToken: storageUserFac.getUserAuthToken(),
                saveKeys: 'HITCH_TYPE,REACH_TYPE,CLIENT_NAME,CLIENT_PHONE,GARAGE_TYPE,ORDER_TYPE,REACH_TIME,ADDRESS',
                entity: ent
            };
            return $.post(trialOrderSaveUrl, obj,
                //回调函数
                function (response) {
                    console.log(response);
                    if (response.IsError) {
                        common.showError(response);
                    } else {
                        msg = response;
                        $rootScope.$broadcast('trialOrderSave.Update');
                    }
                },
                //返回类型
                "json").error(function (err) {
                    common.showError(err);
                });
        },
        maintainOrderSave: function (ent) {
            common.showLoading();
            var obj = {
                userId: 0,
                authToken: storageUserFac.getUserAuthToken(),
                saveKeys: 'GARAGE_ID,HITCH_TYPE,REACH_TYPE,CLIENT_NAME,CLIENT_PHONE,PLATE_NUMBER,CAR_TYPE,REMARK,GARAGE_TYPE,ORDER_TYPE,REACH_TIME,ADDRESS',
                entity: ent
            };
            return $.post(maintainOrderSaveUrl, obj,
                //回调函数
                function (response) {
                    console.log(response);
                    if (response.IsError) {
                        common.showError(response);
                    } else {
                        msg = response;
                        $rootScope.$broadcast('maintainOrderSave.Update');
                    }
                },
                //返回类型
                "json").error(function (err) {
                    common.showError(err);
                });
        },
        helpOrderSave: function (ent) {
            common.showLoading();
            var obj = {
                userId: 0,
                authToken: storageUserFac.getUserAuthToken(),
                saveKeys: 'GARAGE_ID,HITCH_TYPE,REACH_TYPE,CLIENT_NAME,CLIENT_PHONE,PLATE_NUMBER,CAR_TYPE,REMARK,GARAGE_TYPE,ORDER_TYPE,REACH_TIME,AllFiles',
                entity: ent
            };
            //alert(window.JSON.stringify(obj));
            return $.post(helpOrderSaveUrl, obj,
                //回调函数
                function (response) {
                    console.log(response);
                    if (response.IsError) {
                        common.showError(response);
                    } else {
                        msg = response;
                        $rootScope.$broadcast('helpOrderSave.Update');
                    }
                },
                //返回类型
                "json").error(function (err) {
                    msg = {IsError: true, Message: '发起救援失败！'};
                    common.showError(msg);
                });
        },
        /*获取救援单信息*/
        rescueSingle: function (id) {
            common.showLoading();
            var obj = {
                userId: 0,
                authToken: storageUserFac.getUserAuthToken(),
                id: id
            };
            return $.post(rescueSingleUrl, obj,
                //回调函数
                function (response) {
                    console.log(response);
                    if (response.IsError) {
                        common.showError(response);
                    } else {
                        msg = response;
                        $rootScope.$broadcast('order.rescueSingleUpdate');
                    }
                },
                //返回类型
                "json").error(function (err) {
                    msg = {IsError: true, Message: '获取跟踪失败'};
                    common.showError(msg);
                });
        },
        getOrderList: function (obj) {
            common.showLoading();
            obj.authToken = storageUserFac.getUserAuthToken(),
                obj.pageSize = CarIn.pageSize;
            console.log(obj);
            return $.post(url, obj,
                //回调函数
                function (response) {
                    console.log(response);
                    if (response.IsError) {
                        common.showError(response);
                    } else {
                        msg = response;
                        $rootScope.$broadcast('orderListFac.Update');
                    }
                },
                //返回类型
                "json").error(function (err) {
                    msg = {IsError: true, Message: '获取订单列表失败'};
                    common.showError(msg);
                });

        },
        OrderSingle: function (id) {
            common.showLoading();
            var obj = {
                userId: 0,
                authToken: storageUserFac.getUserAuthToken(),
                id: id
            };
            return $.post(OrderSingleUrl, obj,
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
                    msg = {IsError: true, Message: '查询订单出错！'};
                    common.showError(msg);
                });
        },
        /* 获取救援列表*/
        getOrderRescueList: function (obj) {
            common.showLoading();
            obj.authToken = storageUserFac.getUserAuthToken(),
                obj.pageSize = CarIn.pageSize;
            console.log(obj);
            return $.post(orderRescueListUrl, obj,
                //回调函数
                function (response) {
                    console.log(response);
                    if (response.IsError) {
                        common.showError(response);
                    } else {
                        msg = response;
                        $rootScope.$broadcast('orderRescueListFac.Update');
                    }
                },
                //返回类型
                "json").error(function (err) {
                    msg = {IsError: true, Message: '获取订单列表失败'};
                    common.showError(msg);
                });

        },
        delOrder: function (id) {

        },
        getCurrentMes: function () {
            common.hideLoading();
            return msg;
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

        }
    }
})