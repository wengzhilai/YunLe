/**
 * Created by Administrator on 2016/8/9.
 */
mainController.controller('editCarCtr', function ($scope, $ionicLoading, $ionicPopup, carFac, toPost, $state, $stateParams, $ionicActionSheet, $cordovaCamera, fileUpFac, fileUpDel, $cordovaImagePicker, storageUserFac) {
  $scope.tmpNo = {
    no1: '川',
    no2: ''
  };
  $scope.chkchange = function (chk) {
    chk = "DDD";
  }
  if ($stateParams.id) {
    toPost.single("CarSingle", $stateParams.id, function (ent) {
      $scope.car.bean = ent;
      $scope.tmpNo = {
        no1: ent.PLATE_NUMBER.substr(0, 1),
        no2: ent.PLATE_NUMBER.substr(1)
      };
    })
  }

  $scope.car = {
    bigImage: false,
    bigSrc: '',
    showBigImage: function (ent) {
      var url = $(ent.target).attr("src");
      if (url == null || url == '') {
        //选择上传图片
        this.upImg(ent);
        return;
      }
      fileUpFac.FullScreenImage(url,$scope);
    },
    currEnt: {},
    bean: {},
    save: function () {
      if( $scope.tmpNo.no2!=null && $scope.tmpNo.no2!='')
      {
        $scope.car.bean.PLATE_NUMBER = $scope.tmpNo.no1 + $scope.tmpNo.no2;
      }
      toPost.saveOrUpdate("CarSave", this.bean, function (currMsg) {
        console.log(currMsg);
        if (currMsg.IsError) {
          $ionicLoading.show({
            noBackdrop: true,
            template: currMsg.Message,
            duration: 5000
          });
        }
        else
        {
          $ionicLoading.show({
            noBackdrop: true,
            template: '车辆保存成功！',
            duration: 5000
          });
          $scope.car.bean.ID = currMsg.ID;
          var user = storageUserFac.getUser();
          user.NowCar = currMsg;
          storageUserFac.setUser(user);
          $state.go('myCarList', {}, {reload: true});  //路由跳转
        }

      });
    },
    upImg: function () {
      var thisImgEvent = $("#carImg");
      fileUpFac.upImg(thisImgEvent, $scope.car.bean.DRIVING_PIC_ID, function (currMsg) {
        $scope.car.bean.DRIVING_PIC_ID = currMsg.ID;
        $scope.car.bean.DrivingPicUrl = currMsg.URL;
      },$scope);
    }
  };
});
