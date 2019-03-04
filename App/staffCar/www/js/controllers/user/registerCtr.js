/**
 * Created by wengzhilai on 2016/10/23.
 */
mainController.controller('registerCtr', function ($scope, toPost, Storage, userFac, CarIn, $stateParams, $ionicPopup, $state, $ionicLoading, $interval) {

  var opendId = Storage.get("openid", '');
  if (opendId != null && opendId != '') {
    $scope.initBack = function (ent) {
      var code = ent.SCENE_STR;
      var type = code.substr(0, code.lastIndexOf("_"));
      if (type == "qrscene_team") {
        code = code.substr(code.lastIndexOf("_") + 1);
        $scope.register.bean.pollCode = code;
      }
      else {
        common.hint('你不是服务商，请关闭后，选择用户入口');
        window.close();
      }
    }
    var postBean = {
      authToken: Storage.get("openid", '')
    }
    toPost.Post("WeixinGetUser", postBean, $scope.initBack);
  }

  $scope.register = {
    bean: {
      loginName: '',
      password: '',
      version: CarIn.version,
      code: '',
      type: '1',
      pollCode: $stateParams.pollCode
    },
    init: function () {

    },
    onSubmit: function () {
      var rpw = document.getElementById('rpw').value;
      if (this.bean.password == rpw) {
        toPost.Post("LoginReg", this.bean, $scope.register.regCallback);
      } else {
        var alertPopup = $ionicPopup.alert({
          title: '警告',
          template: '两次密码的值不一致！',
          okText: '确认'
        });
        alertPopup.then(function (res) {
          console.log('显示警告');
        });
      }
    },
    regCallback: function (currMsg) {
      console.log(currMsg);
      if (currMsg.IsError == true) {
        $ionicLoading.show({
          noBackdrop: true,
          template: currMsg.Message,
          duration: 2000
        });
      } else {
        $ionicLoading.show({
          noBackdrop: true,
          template: "业务员注册成功",
          duration: 2000
        });
        $state.go('login', {reload: true});  //路由跳转
      }
    }
  };

  $scope.paracont = "获取验证码";
  $scope.paraclass = "badge assertive-bg light button";
  $scope.paraevent = true;
  $scope.SendCode = function () {
    if(!$scope.paraevent)return;
    toPost.Post("SendCode", {phone: $scope.register.bean.loginName}, function (currMsg) {
      console.log(currMsg);
      if (currMsg.IsError == true) {
        $ionicLoading.show({
          noBackdrop: true,
          template: currMsg.Message,
          duration: 2000
        });
      } else {
        var second = 60,
          timePromise = undefined;
        timePromise = $interval(function () {
          if (second <= 0) {
            $interval.cancel(timePromise);
            timePromise = undefined;
            second = 60;
            $scope.paracont = "重发验证码";
            $scope.paraclass = "badge assertive-bg light button";
            $scope.paraevent = true;
          } else {
            $scope.paracont = second + "秒后重发";
            $scope.paraclass = "badge button";
            $scope.paraevent = false;
            second--;
          }
        }, 1000, 100);
      }
    });
  };

})
