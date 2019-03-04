/**
 * Created by wengzhilai on 2016/10/13.
 */

mainController.controller('orderRuningListCtr', function ($scope,storageUserFac, toPost, $ionicLoading, $state, $stateParams, $ionicPopup) {
    $scope.orderList = {
        bean: {
            userId: 0,
            authToken: '',
            pageSize: 0,
            id: 0,
            currentPage: 0,
            searchKey: [
                {K: 'CLIENT_ID', V: storageUserFac.getUserId(), T: '=='},
                {K: "YL_ORDER_RESCUE", V: "null", T: '!='},
                {K: "YL_ORDER_RESCUE.STATUS", V: "完成", T: '!='},
                {K: "YL_ORDER_RESCUE.GARAGE_ID", V: "null", T: '!='}
            ],
            orderBy: []
        },
        toFollowOrder: function (id, type) {
            if (type == "救援") {
                $state.go('followHelpOrder', {id: id, reload: true});  //路由跳转
            } else if (type == "审车") {
                $state.go('followHelpOrder', {id: id, reload: true});  //路由跳转
            } else if (type == "维修" || type == "保养") {
                $state.go('followHelpOrder', {id: id, reload: true});  //路由跳转
            } else if (type == "投保") {
                $state.go('followInsure', {id: id, reload: true});  //路由跳转
            }
        },
        lists: {},
        hasNextPage: function () {
            if (this.lists.totalPage == null || this.lists.totalPage == 0) {
                return false;
            }
            else if (this.lists.totalPage <= this.lists.currentPage) {
                return false;
            }
            return true;
        },
        doRefresh: function () {
            console.log("下拉刷新");
            this.bean.currentPage = 1;

            var buB = $("a[class='button button-balanced']").text();
            switch (buB)
            {
                case "救援":
                    this.bean.searchKey=[{K: "ORDER_TYPE", V: buB, T: '=='}];
                    break;
                case "审车":
                    this.bean.searchKey=[{K: "ORDER_TYPE", V: buB, T: '=='}];
                    break;
                case "维保":
                    this.bean.searchKey=[{K: "ORDER_TYPE", V: buB, T: '=='}];
                    break;
                case "保单":
                    this.bean.searchKey=[{K: "ORDER_TYPE", V: "投保", T: '=='}];
                    break;
            }
            toPost.list("OrderList", this.bean, this.callListReturn);
        },
        callListReturn:function(currMsg){
            if (currMsg.IsError == true) {
                $ionicLoading.show({
                    noBackdrop: true,
                    template: currMsg.Message,
                    duration: 2000
                });
            } else {
                $scope.orderList.lists = currMsg;
            }
            $scope.$broadcast('scroll.refreshComplete');
            $scope.$broadcast('scroll.infiniteScrollComplete');
        },
        loadMore: function () {
            if (this.bean.currentPage == 0) {
                this.bean.currentPage = 1;
            } else {
                this.bean.currentPage++;
            }
            console.log("加载更多");
            toPost.list("OrderList", this.bean, this.callListReturn);
        },
        filterKey: function (obj) {
            if ($("a[class='button button-balanced']").length != 0) {
                $("a[class='button button-balanced']").each(function () {
                    $(this).attr("class", "button");
                });
            }
            obj.target.setAttribute("class", "button button-balanced");
            this.doRefresh();
        },
        showOrderPopup: function (obj) {
            // 自定义弹窗
            var myPopup = $ionicPopup.show({
                template: '<div  class="item item-input item-select"><div class="input-label">排序字段</div><select id="orderF"><option value="">--请选择--</option><option value="CREATE_TIME">下单日期</option><option value="COST">订单金额</option><option value="ORDER_NO">订单号</option></select></div>' +
                    '<div class="list"><div class="item">升序<input type="radio" name="order" data-text="升序" value="asc" checked="checked"/></div>' +
                    '<div class="item">降序<input type="radio" data-text="降序" name="order" value="desc"/> </div>',
                title: '选择排序方式',
                scope: $scope,
                buttons: [
                    {
                        text: '<b>清除</b>',
                        type: 'button-positive',
                        onTap: function () {
                            myPopup.close();
                            obj.target.innerHTML = '排序';
                            $scope.orderList.bean.orderBy = [];
                        }
                    },
                    {
                        text: '<b>确定</b>',
                        type: 'button-positive',
                        onTap: function (e) {
                            //$("input[name='order']:checked").attr("date-text")
                            var order = $('input[name="order"]:checked');
                            var orderF = $('#orderF option:selected');

                            if (orderF.val() != '') {
                                $scope.orderList.bean.orderBy = [
                                    {K: orderF.val(), V: order.val(), T: ''}
                                ];

                                obj.target.innerHTML = orderF.text() + ' ' + order.attr("data-text");
                            } else {

                                obj.target.innerHTML = '排序';
                            }
                        }
                    }
                ]
            });
        }
    };
})
