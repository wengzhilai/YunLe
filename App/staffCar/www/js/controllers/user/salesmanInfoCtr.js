/**
 * Created by wengzhilai on 2016/11/7.
 */

mainController.controller('salesmanInfoCtr', function (Storage, toPost, fileUpFac, storageUserFac, CarIn, $scope, $ionicLoading, $state, $stateParams, $ionicPopup) {
  $scope.Url=""//当前显示的图片地址
  $scope.salesman = {
    nowTabsIndex:1,//当前列表
    scrollWidth:0,
    scrollHeight:0,
    bean: {

    },
    init:function () {
      this.scrollHeight=$(document).height()-300;
      this.scrollWidth=$(document).width()-30;
      if($stateParams.id!=null)
      {
        toPost.single("SalesmanSingle", $stateParams.id, function (currMsg) {
          if (currMsg.IsError == true) {
            $ionicLoading.show({
              noBackdrop: true,
              template: currMsg.Message,
              duration: 2000
            });
          } else {
            $scope.salesman.bean = currMsg;
          }
        });
      }
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
