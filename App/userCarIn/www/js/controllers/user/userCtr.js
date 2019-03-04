/**
 * Created by wengzhilai on 2016/8/22.
 */
mainController.controller('userCtr', function ($scope, CarIn, storageUserFac, $ionicPopup, $state,$ionicLoading) {
  $scope.user = {
    bean: storageUserFac.getUser(),
    toShare: function () {
      var salesman_id = storageUserFac.getUser().SALESMAN_ID;
      var baseUrl = CarIn.baseUrl;
      var qrcode = baseUrl + '/File/QrCode/salesman_' + salesman_id + '.jpg';
      var qrcodeWeiXin = baseUrl + '/File/QrCode/salesman_weixin_' + salesman_id + '.jpg';
      console.log(qrcodeWeiXin);
      if (navigator.userAgent.toLowerCase().indexOf('micromessenger') > -1) {
        wx.previewImage({
          current: qrcodeWeiXin, // 当前显示图片的http链接
          urls: [qrcode, qrcodeWeiXin] // 需要预览的图片http链接列表
        });
        return;
      }
      window.plugins.socialsharing.share("乐享", "subject", qrcode, qrcode);
    },
    alert: function (str) {
      $ionicLoading.show({
        noBackdrop: true,
        template: str,
        duration: 2000
      });
    }
  };

  $scope.$on('userSave.Update', function () {
    var currMsg = userFac.getCurrentMes();
    $ionicLoading.show({
      noBackdrop: true,
      template: currMsg.Message,
      duration: 2000
    });
  });
  $scope.outLogin = function () {
    var confirmPopup = $ionicPopup.confirm({
      title: '确认注销',
      template: '是否退出登录?',
      okText: '注销',
      cancelText: '取消'
    });
    confirmPopup.then(function (res) {
      if (res) {
        storageUserFac.remove();
        $state.go('login', {reload: true});  //路由跳转
      } else {
        console.log('You are not sure');
      }
    });
  };

})
