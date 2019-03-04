/**
 * Created by Administrator on 2016/8/9.
 */
mainController.controller('myCarListCtr', function ($scope, $ionicLoading, $stateParams, carFac, $state, Storage, toPost, $ionicActionSheet, storageUserFac) {
  var storageKey = 'car';
  $scope.carList = {
    bean: {
      userId: 0,
      authToken: storageUserFac.getUserAuthToken(),
      pageSize: 0,
      id: 0,
      currentPage: 0,
      searchKey: [],
      orderBy: []
    },
    lists: {},
    reLoad: function () {
      if (storageUserFac.getUser() == null) {
        $state.go('login', {reload: true});
      }
      if (this.bean.currentPage == null || this.bean.currentPage == 0) {
        this.loadMore();
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
      toPost.list("CarList", this.bean, $scope.carList.reFreshBack);
      //carFac.getCarList(this.bean);
    },
    reFreshBack: function (currMsg) {
      if (currMsg.IsError == true) {
        $ionicLoading.show({
          noBackdrop: true,
          template: currMsg.Message,
          duration: 2000
        });
      } else {
        $scope.carList.lists = currMsg;
        console.log("返回数据");
        console.log(currMsg.data);
      }
      $scope.$broadcast('scroll.refreshComplete');
      $scope.$broadcast('scroll.infiniteScrollComplete');
    },
    loadMore: function () {
      console.log("加载更多");
      toPost.list("CarList", this.bean, $scope.carList.reFreshBack);
    },
    addCar: function () {
      Storage.remove(storageKey);
      $state.go('editCar', {reload: true});  //路由跳转
    },
    toEditCar: function (obj) {
      Storage.set(storageKey, obj);
      $state.go('editCar', {"id": obj.ID});  //路由跳转
    },
    showSDD: function (id) {
      $ionicActionSheet.show({
        buttons: [
          {text: '设为默认车辆'},
          {text: '删除车辆信息'}
        ],
        titleText: '选择操作',
        cancel: function () {
          // add cancel code..
        },
        buttonClicked: function (index) {
          var obj = {
            userId: 0,
            authToken: storageUserFac.getUserAuthToken(),
            id: id
          }
          if (index == 0) {
            //设为默认车辆
            toPost.Post("CarSetDefault", obj, $scope.carList.setDefaultCallBack)
          } else if (index == 1) {
            //删除车辆
            toPost.Post("CarDelete", obj, $scope.carList.deleteCallBack)
          }
        }
      });
    },
    setDefaultCallBack: function (currMsg) {
      if (currMsg.IsError == true) {
        $ionicLoading.show({
          noBackdrop: true,
          template: currMsg.Message,
          duration: 2000
        });
      } else {
        $ionicLoading.show({
          noBackdrop: true,
          template: '设为默认车辆成功',
          duration: 2000
        });
        $scope.carList.bean.currentPage = 0;
        console.log(currMsg);
        var user = storageUserFac.getUser();
        user.NowCar = currMsg;
        storageUserFac.setUser(user);
        $state.go('myCarList', {}, {reload: true});  //路由跳转
      }
      $scope.$broadcast('scroll.refreshComplete');
      $scope.$broadcast('scroll.infiniteScrollComplete');
    },
    deleteCallBack: function (currMsg) {
      $ionicLoading.show({
        noBackdrop: true,
        template: '删除成功',
        duration: 2000
      });
      $scope.carList.bean.currentPage = 0;
      $scope.carList.loadMore();
      console.log(currMsg);
      $scope.$broadcast('scroll.refreshComplete');
      $scope.$broadcast('scroll.infiniteScrollComplete');
    }
  };
})
