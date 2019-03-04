/**
 * Created by wengzhilai on 2016/10/13.
 */

mainController.controller('orderGrabListCtr', function ($scope, toPost, $ionicLoading, $state, $stateParams, $ionicPopup) {
    $scope.orderList = {
        bean: {
            userId: 0,
            authToken: '',
            pageSize: 0,
            id: 0,
            currentPage: 0,
            searchKey: [],
            orderBy: []
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
            toPost.list("OrderGrabList", this.bean, this.callListReturn);
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
            toPost.list("OrderGrabList", this.bean, this.callListReturn);
        }
    };
})