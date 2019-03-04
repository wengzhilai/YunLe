/**
 * Created by wengzhilai on 2016/8/18.
 */
mainController.controller('addressListCtr', function ($scope,$state,storageUserFac, $ionicLoading, toPost) {
    var storageKey = 'address';

    $scope.addressList = {
        showMsg:function()
        {
            $ionicLoading.show({
                noBackdrop: true,
                template: '为方便为您提供上门接车、礼品派送等服务，请准确填写您的地址！',
                duration: 2000
            });
        },
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
        reLoad: function () {
            if (storageUserFac.getUser() == null) {
                $state.go('login', {reload: true });
            }
            if (this.bean.currentPage == null || this.bean.currentPage == 0) {
                toPost.list("AddressList", this.bean,this.reFreshBack);
                //this.loadMore();
            }
        },
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
            toPost.list("AddressList", this.bean,this.reFreshBack);
        },
        reFreshBack: function (currMsg) {
            if (currMsg.IsError == true) {
                $ionicLoading.show({
                    noBackdrop: true,
                    template: currMsg.Message,
                    duration: 2000
                });
            } else {
                $scope.addressList.lists = currMsg;
                console.log("返回数据");
                console.log(currMsg.data);
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
            toPost.list("AddressList", this.bean, this.reFreshBack);
        }
    };

})
