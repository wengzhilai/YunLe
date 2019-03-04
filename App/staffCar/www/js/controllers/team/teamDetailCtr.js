/**
 * Created by wengzhilai on 2016/11/4.
 */

mainController.controller('teamDetailCtr', function (Storage, toPost, fileUpFac, storageUserFac, CarIn, $scope, $ionicLoading, $state, $stateParams, $ionicPopup) {
  $scope.Url=""//当前显示的图片地址
  $scope.team = {
    nowTabsIndex:1,//当前列表
    scrollWidth:0,
    scrollHeight:0,
    lists:[],
    bean: {
      userId: 0,
      authToken: '',
      pageSize: 0,
      id: 0,
      currentPage: 0,
      searchKey: [],
      orderBy: []
    },
    init:function () {
      this.scrollHeight=$(document).height()-300;
      this.scrollWidth=$(document).width()-30;
      toPost.list("TeamMyAll", $scope.team.bean, function (currMsg) {
        if (currMsg.IsError == true) {
          $ionicLoading.show({
            noBackdrop: true,
            template: currMsg.Message,
            duration: 2000
          });
        } else {
          $scope.team.lists = currMsg;
        }
        $scope.$broadcast('scroll.refreshComplete');
        $scope.$broadcast('scroll.infiniteScrollComplete');
      });
    },
    onTab:function(obj,index){
      $(".tab-item").removeClass("active")
      $(obj.target).addClass("active");
      this.nowTabsIndex=index;
    },
    showBigImage: function (ent) {
      var url = ent.target.getAttribute("src");
      if (url == null || url == '') {
        return;
      }
      fileUpFac.FullScreenImage(url,$scope);
    }
  };
})
