/**
 * Created by wengzhilai on 2016/8/9.
 */
mainController.controller('homeCtr', function (common,$scope, $rootScope, $interval, storageUserFac, CarIn, toPost, $state, $ionicPopup, $ionicTabsDelegate, $cordovaLocalNotification) {
  $scope.user = storageUserFac.getUser();
  $scope.home = {
    init: function () {
      console.log($scope.user)
      if ($scope.user == null || $scope.user.authToken == null || $scope.user.authToken == '') {
        console.log('没登录')
        $state.go('login', {reload: true});  //路由跳转
      }
      else {
        console.log('服务器验证')
        toPost.single("ClientSingle",$scope.user.ID,function (currMsg) {
          console.log(currMsg)
          if (currMsg.IsError) {
            common.hint(currMsg.Message);
            $state.go('login', {reload: true});  //路由跳转
          } else {
            $scope.user=currMsg;
            storageUserFac.setUser(currMsg);
          }
        })
      }
    }
  }
  $scope.select = function (index) {
    $ionicTabsDelegate.select(index);
//        if (index == 1) {
//            $cordovaLocalNotification.isScheduled("1234").then(function (isScheduled) {
//                alert("Notification 1234 Scheduled: " + isScheduled);
//            });
//        }
  };
  if ($scope.user != null && $scope.user.NowCar != null) {
    $scope.NowCar = $scope.user.NowCar;
  }
  $scope.alert = function (str) {
    common.hint(str)
  };

  $scope.toEditCar = function (obj) {
    $state.go('editCar', {"id": obj.ID});  //路由跳转
  }
  if (ionic.Platform.isIOS() || ionic.Platform.isAndroid()) {
    //获取消息提醒
    var timer = $interval(function () {
      var postBean = {
        authToken: storageUserFac.getUserAuthToken(),
        userId: 0,
        pageSize: CarIn.pageSize,
        postBean: currentPage = 1,
        searchKey: [
          {"STATUS": "等待"}
        ]
      };
      $.post(CarIn.api + "MessageGetAll", postBean,
        //回调函数
        function (response) {
          if (!ent.IsError) {
            for (var i = 0; i < ent.data.length; i++) {
              $cordovaLocalNotification.add({
                id: ent.ID,
                date: new Date(),
                message: ent.CONTENT,
                title: ent.TITLE,
                autoCancel: true,
                sound: null
              }).then(function () {
                cordova.plugins.notification.local.on("click", function (notification, state) {
                  //alert('courses');
                }, this)
              });
            }
          }
        },
        //返回类型
        "json").error(function (err) {
      });
    }, 1000 * 60);
  }

});
