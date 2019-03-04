/**
 * Created by wengzhilai on 2016/10/12.
 */
mainController.controller('editCustomerCtr', function ($scope, fileUpFac, toPost, $location, $ionicLoading, $state, $stateParams) {
  $scope.customer = {
    bean: {
      ID: 0,
      NAME: '',
      SEX: '',
      ADDRESS: '',
      LOGIN_NAME: '',
      phone: '',
      STATUS: '',
      REMARK: '',
      SALESMAN_ID: '',
      ID_NO: '',
      ID_NO_PIC_ID: '',
      DRIVER_PIC_ID: '',
      NowCar: {
        MODEL: '',
        PRICE: '',
        BRAND: '',
        FRAME_NUMBER: '',
        ENGINE_NUMBER: '',
        PLATE_NUMBER: ''
      }

    },
    currEnt: {},
    upImg: function (obj, OutFileId) {
      $scope.customer.currEnt = $(obj.target);
      if (OutFileId == null) {
        OutFileId=$scope.customer.currEnt.attr("data-id");
      }
      fileUpFac.upImg($scope.customer.currEnt, OutFileId, $scope.customer.upCallback,$scope);
    },
    upCallback: function (result) {
      var name = $scope.customer.currEnt.attr("name");
      switch (name) {
        case "idNoUrl":
          $scope.customer.bean.ID_NO_PIC_ID = result.ID;
          $scope.customer.bean.idNoUrl = result.URL;
          break;
        case "idNoUrl1":
          $scope.customer.bean.ID_NO_PIC_ID1 = result.ID;
          $scope.customer.bean.idNoUrl1 = result.URL;
          break;
        case "driverPicUrl":
          $scope.customer.bean.DRIVER_PIC_ID = result.ID;
          $scope.customer.bean.driverPicUrl = result.URL;
          break;
        case "driverPicUrl1":
          $scope.customer.bean.DRIVER_PIC_ID1 = result.ID;
          $scope.customer.bean.driverPicUrl1 = result.URL;
          break;
        case "carDrivingPicUrl":
          $scope.customer.bean.NowCar.DRIVING_PIC_ID = result.ID;
          $scope.customer.bean.NowCar.DrivingPicUrl = result.URL;
          break;
        case "carDrivingPicUrl1":
          $scope.customer.bean.NowCar.DRIVING_PIC_ID1 = result.ID;
          $scope.customer.bean.NowCar.DrivingPicUrl1 = result.URL;
          break;
        case "carIconURL":
          $scope.customer.bean.NowCar.ID_NO_PIC_ID = result.ID;
          $scope.customer.bean.NowCar.idNoUrl = result.URL;
          break;
        case "carIconURL1":
          $scope.customer.bean.NowCar.ID_NO_PIC_ID1 = result.ID;
          $scope.customer.bean.NowCar.idNoUrl1 = result.URL;
          break;
      }

    },
    showBigImage: function (ent) {
      var url = $(ent.target).attr("src");
      if (url == null || url == '') {
        //选择上传图片
        this.upImg(ent);
        return;
      }
      fileUpFac.FullScreenImage(url);
    },
    save: function () {
      this.bean.LOGIN_NAME = this.bean.phone;
      toPost.saveOrUpdate("SalesmanClientAdd", this.bean, this.saveCallBack)
    },
    saveCallBack: function (currMsg) {
      if (currMsg.IsError == true) {
        $ionicLoading.show({
          noBackdrop: true,
          template: currMsg.Message,
          duration: 2000
        });
      } else {
        $ionicLoading.show({
          noBackdrop: true,
          template: '客户保存成功！',
          duration: 2000
        });
        $state.go('customerList', {reload: true});  //路由跳转
      }
    }
  };

  $scope.singleCallBack = function (currMsg) {
    if (currMsg.IsError == true) {
      $ionicLoading.show({
        noBackdrop: true,
        template: currMsg.Message,
        duration: 2000
      });

    } else {
      $scope.customer.bean = currMsg;
    }
  };

  if ($stateParams.id) {
    toPost.single("ClientSingle", $stateParams.id, $scope.singleCallBack)
  }

})
