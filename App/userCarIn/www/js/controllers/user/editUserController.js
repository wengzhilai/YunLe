/**
 * Created by wengzhilai on 2016/8/7.
 */
mainController.controller('editUserCtr', function ($scope, common, $ionicPopup, $state, storageUserFac, fileUpDel, fileUpFac, $cordovaCamera, $cordovaImagePicker, $ionicLoading, toPost, $ionicActionSheet) {
  $scope.ctrlBar = true;
  $scope.toggleFullScreen = function () {
    var fullElem = document.getElementById("fullScreen");
    if (document.webkitFullscreenElement) {
      document.webkitCancelFullScreen();
      $scope.ctrlBar = true;
    } else {
      fullElem.webkitRequestFullscreen();
      $scope.toggleVideo = function () {
        if ($scope.ctrlBar) {
          $timeout.cancel($scope.ctrlBar);
        }
        $scope.ctrlBar = $timeout(
          function () {
            $scope.ctrlBar = false;
          },
          5000);
      }
    }
  }

  $scope.user = {
    bean: storageUserFac.getUser(),
    currEnt: {},
    upImg: function (obj, OutFileId) {
      $scope.user.currEnt = $(obj.target);
      fileUpFac.upImg($scope.user.currEnt, OutFileId, $scope.user.upCallback,$scope);
    },
    upCallback: function (result) {
      var name = $scope.user.currEnt.attr("name");
      switch (name) {
        case "iconURL":
          $scope.user.bean.ICON_FILES_ID = result.ID;
          $scope.user.bean.iconURL = result.URL;
          break;
        case "idNoUrl":
          $scope.user.bean.ID_NO_PIC_ID = result.ID;
          $scope.user.bean.idNoUrl = result.URL;
          break;
        case "driverPicUrl":
          $scope.user.bean.DRIVER_PIC_ID = result.ID;
          $scope.user.bean.driverPicUrl = result.URL;
          break;
      }

    },
    showBigImage: function (ent) {
      var url = ent.target.getAttribute("src");
      if (url == null || url == '') {
        //选择上传图片
        this.upImg(ent);
        return;
      }
      fileUpFac.FullScreenImage(url, $scope);
    },
    save: function () {
      var obj = {
        authToken: storageUserFac.getUserAuthToken(),
        userId: 0,
        saveKeys: 'NAME,SEX,ADDRESS,ID_NO,ID_NO_PIC_ID,ICON_FILES_ID,DRIVER_PIC_ID',
        entity: this.bean
      };

      if($scope.user.bean.ID_NO!='')

      if ($scope.user.bean.ID_NO!='' || common.regExpIdNo($scope.user.bean.ID_NO, function (msg) {
          $ionicLoading.show({
            noBackdrop: true,
            template: msg,
            duration: 2000
          });
        })) {
        toPost.Post("ClientSave", obj, $scope.user.saveCallback);
      }

    },
    saveCallback: function (result) {
      $ionicLoading.show({
        noBackdrop: true,
        template: "个人信息保存成功！",
        duration: 2000
      });
      storageUserFac.setUser($scope.user.bean);
      $state.go('user', {reload: true});  //路由跳转
    },
    updatePassword: function () {
      var myPopup = $ionicPopup.show({
        template: '' +
        '<input type="password"  placeholder="输入原密码"  ng-model="user.bean.password" style="margin-bottom: 2px">' +
        '<input type="password"  placeholder="输入新密码"  ng-model="user.bean.password1" style="margin-bottom: 2px">' +
        '<input type="password"  placeholder="重复新密码"  ng-model="user.bean.password2" style="margin-bottom: 2px">',
        title: '修改密码',
        scope: $scope,
        buttons: [
          {text: '取消'},
          {
            text: '<b>保存</b>',
            type: 'button-positive',
            onTap: function (e) {
              if (!$scope.user.bean.password) {
                common.hint("输入新密码才可以保存");
              } else {
                var obj = {
                  authToken: storageUserFac.getUserAuthToken(),
                  userId: 0,
                  entity: $scope.user.bean.password1,
                  para: [{
                    K: 'oldPwd', V: $scope.user.bean.password
                  }]
                };
                if ($scope.user.bean.password1 == $scope.user.bean.password2) {
                  toPost.Post("UserEditPwd", obj, $scope.user.upPasswordCallback)
                }
                else {
                  common.hint("两次密码不一致");
                }
              }
            }
          },
        ]
      });
    },
    upPasswordCallback: function (result) {
      $ionicLoading.show({
        noBackdrop: true,
        template: "密码更新成功！",
        duration: 2000
      });
    }
  };
})
