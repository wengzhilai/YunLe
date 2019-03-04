/**
 * Created by wengzhilai on 2016/8/11.
 */
mainController.controller('helpOrderListCtr', function (common,toPost, $scope,storageUserFac, Storage, $ionicPopup, $state, $ionicLoading, $ionicActionSheet) {
    $scope.orderList = {
        bean: {
            searchKey : [
                {K: 'CLIENT_ID', V: storageUserFac.getUserId(), T: '=='},
                {
                    K: 'YL_ORDER_RESCUE.GARAGE_TYPE', V: '救援', T: '=='
                }
            ]
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
        reLoad: function () {
            if (this.bean.currentPage == null || this.bean.currentPage == 0) {
                this.loadMore();
            }
        },
        doRefresh: function () {
            console.log("下拉刷新");
            this.bean.currentPage = 1;
            toPost.list("OrderRescueList", this.bean, this.getList);
        },
        getList: function (response) {
            $scope.orderList.lists = response;
            console.log(response);
            $scope.$broadcast('scroll.refreshComplete');
            $scope.$broadcast('scroll.infiniteScrollComplete');
        },
        loadMore: function () {
            if (this.bean.currentPage == null || this.bean.currentPage == 0) {
                this.bean.currentPage = 1;
            } else {
                this.bean.currentPage++;
            }
            console.log("加载更多");
            toPost.list("OrderRescueList", this.bean, this.getList);
        },
        /*操作救援定单*/
        showSFD: function (o) {
            $ionicActionSheet.show({
                buttons: [
                    {text: '跟踪订单'},
                    {text: '删除订单'}
                ],
                titleText: '选择操作',
                cancel: function () {
                    // add cancel code..
                },
                buttonClicked: function (index) {
                    if (index == 0) {
                        $state.go('followHelpOrder', {id: o.ID, reload: true});  //路由跳转
                    } else if (index == 1) {
                        if (o.STATUS == '已付款') {
                          common.hint('付款订单无法删除！');
                        } else {
                          common.hint("不支持删除订单");
                            // orderFac.delOrder(o.id);
                        }
                    }
                }
            });
        }
    };
});
