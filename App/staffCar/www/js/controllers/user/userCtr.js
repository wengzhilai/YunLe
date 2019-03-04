/**
 * Created by wengzhilai on 2016/9/19.
 */
mainController.controller('userCtr', function (common,$scope, CarIn, $ionicPopup, $state, storageUserFac,fileUpFac,toPost) {
  $scope.user = {
    bean: storageUserFac.getUser(),
    toShare: function () {
      var user_id = storageUserFac.getUserId();
      var baseUrl = CarIn.baseUrl;
      var qrcode = baseUrl + '/File/QrCode/salesman_' + user_id + '.jpg';
      var qrcodeWeiXin = baseUrl + '/File/QrCode/salesman_weixin_' + user_id + '.jpg';
      if (navigator.userAgent.toLowerCase().indexOf('micromessenger') > -1) {
        wx.previewImage({
          current: qrcodeWeiXin, // 当前显示图片的http链接
          urls: [qrcode, qrcodeWeiXin] // 需要预览的图片http链接列表
        });
        return;
      }
      else {
        fileUpFac.FullScreenImage(qrcodeWeiXin,$scope)
        //window.plugins.socialsharing.share("乐享", "subject", qrcodeWeiXin, qrcodeWeiXin);
      }
    },
    weChat: function () {
      $("#mcover").css("display", "none");  // 点击弹出层，弹出层消失
    },
    watting: function () {
      common.hint("后续开放，敬请期待");
    },
    sendFre: function () {
      common.hint("发送给朋友");
    }
  };
  $scope.outLogin = function () {
    var confirmPopup = $ionicPopup.confirm({
      title: '确认注销',
      template: '是否退出登录?',
      okText: '注销',
      cancelText: '取消'
    });
    confirmPopup.then(function (res) {
      if (res) {
        toPost.single("LoginOut",0,function (errobj) {
          if (errobj.IsError) {
            $ionicPopup.alert({
              title: '提示',
              template: "退出错误"
            });
          }
          storageUserFac.clearAll();
          $state.go('login', {reload: true});  //路由跳转
        });

      } else {
        console.log('You are not sure');
      }
    });
  };

})
