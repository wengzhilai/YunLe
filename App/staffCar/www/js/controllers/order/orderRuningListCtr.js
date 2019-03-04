/**
 * Created by wengzhilai on 2016/10/13.
 */

mainController.controller('orderRuningListCtr', function ($scope, toPost, $ionicLoading, $state, $stateParams, $ionicPopup,storageUserFac) {
  $scope.orderList = {
    bean: {
      userId: 0,
      authToken: '',
      pageSize: 0,
      id: 0,
      currentPage: 0,
      searchKey: [
        {K: "YL_ORDER_RESCUE", V: "null", T: '!='},
        {K: "YL_ORDER_RESCUE.STATUS", V: "完成", T: '!='},
        {K: "YL_ORDER_RESCUE.GARAGE_ID", V: "null", T: '!='}
      ],
      orderBy: []
    },
    lists: {},
    toFollowOrder: function (id, type) {
      if (type == "救援") {
        $state.go('followHelpOrder', {id: id, reload: true});  //路由跳转
      } else if (type == "审车") {
        $state.go('followTrialOrder', {id: id, reload: true});  //路由跳转
      } else if (type == "维修" || type == "保养") {
        $state.go('followHelpOrder', {id: id, reload: true});  //路由跳转
      } else if (type == "投保") {
        $state.go('followInsure', {id: id, reload: true});  //路由跳转
      }
    },
    init: function () {
      var allData=storageUserFac.getUser().allOrder;
      console.log('参数');
      console.log(allData);
      $scope.orderList.lists.data =allData ;
    }
  };
})
