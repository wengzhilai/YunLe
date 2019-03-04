/**
 * Created by wengzhilai on 2016/8/12.
 */
mainService.factory('orderFac', function (CarIn, $rootScope, storageUserFac, common) {
    var url = CarIn.api + CarIn.OrderList,
        rescueSingleUrl = CarIn.api + CarIn.rescueSingle,
        OrderSingleUrl=CarIn.api + CarIn.OrderSingle,
        msg = {};
    return {
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
                    msg = {IsError: true, Message: '获取订单失败'};
                    common.showError(msg);
                });
        },
        getOrderList: function (obj) {
            common.showLoading();
            obj.authToken = storageUserFac.getUserAuthToken();
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
                        $rootScope.$broadcast('order.SingleUpdate');
                    }
                },
                //返回类型
                "json").error(function (err) {
                    msg = {IsError: true, Message: '查询订单出错！'};
                    common.showError(msg);
                });
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