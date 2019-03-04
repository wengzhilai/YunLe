

mainController.controller('homeCtr', function ($scope, $state, storageUserFac,toPost,$ionicPopup,common) {
  $scope.user = storageUserFac.getUser();
  $scope.toHelp = function (id) {
    $state.go('helpList', {reload: true});
  }
  $scope.toMessageList = function () {
    $state.go('messageList', {reload: true});
  }
  $scope.alert = function (str) {
    common.hint(str);
  }
  $scope.home = {
    init: function () {
      $scope.user = storageUserFac.getUser();
      console.log($scope.user)
      if($scope.user==null || $scope.user.authToken==null || $scope.user.authToken==''){
        $state.go('login', {reload: true});  //路由跳转
      }
      else {
        toPost.single("SalesmanSingle",$scope.user.ID,function (currMsg) {
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
    },
    toTeam: function () {
      if (storageUserFac.getTeamId) {
        common.hint("您已经加入团队，如需更新团队请联系管理员");
      } else {
        cordova.plugins.barcodeScanner.scan(
          function (result) {
            salesmanFac.salesmanToTeam(result.text);
          },
          function (error) {
            common.hint("Scanning failed: " + error);
          }
        );

      }
    }
  };

})
