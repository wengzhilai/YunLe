/**
 * Created by Administrator on 2016/8/17.
 */
mainController.controller('editMaintainOrderCtr', function ($scope, toPost, common, $stateParams, $state, orderFac, $ionicLoading, storageUserFac) {
  var garage = {};
  var user = storageUserFac.getUser();
  $scope.changeAddress = function (o) {
    $scope.order.bean.ADDRESS = o.ADDRESS;
    $scope.order.bean.LANG = o.LANG;
    $scope.order.bean.LAT = o.LAT;
  };
  $scope.order = {
    bean: {
      addressBean: {},
      ORDER_NO: '',
      CLIENT_ID: user.ID,
      REACH_TYPE: '上门接车',
      CLIENT_NAME: user.NAME,
      CLIENT_PHONE: user.LOGIN_NAME,
      PLATE_NUMBER: user.NowCar == null ? '' : user.NowCar.PLATE_NUMBER,
      CAR_TYPE: user.NowCar == null ? '' : user.NowCar.MODEL,
      BRAND: user.NowCar == null ? '' : user.NowCar.BRAND,
      MODEL: user.NowCar == null ? '' : user.NowCar.FRAME_NUMBER,
      CAR_ID: user.NowCar == null ? '' : user.NowCar.ID
    },
    reLoad: function () {
      garage = $stateParams.garage;
      if ($stateParams.id != null && $stateParams.id != 0) {
        this.bean.ID = $stateParams.id;
        toPost.single("RescueSingle", $stateParams.id, function (response) {
          $scope.helpOrder.bean = response;
          for (var i = 0; i < $scope.helpOrder.bean.AllFiles.length; i++) {
            $scope.helpOrder.bean.AllFiles[i].indexNo = i;
          }
          garage = {
            garageId: $scope.helpOrder.bean.GARAGE_ID,
            garageName: $scope.helpOrder.bean.GarageName
          };
        });
      }
      if (garage != null) {
        this.bean.GARAGE_ID = garage.garageId;
        this.bean.GarageName = garage.garageName;
      }
    },
    selectLat: function () {
      var location = {"lng": $scope.order.bean.LANG, "lat": $scope.order.bean.LAT};
      $state.go('selectMyLat', {
        location: location,
        url: "editMaintainOrder",
        garage: garage,
        id: $stateParams.id,
        bean: this.bean,
        reload: true
      });  //路由跳转
    },
    save: function () {
      toPost.saveOrUpdate('RescueSave', this.bean, function (response) {
        if (response.IsError) {
          common.hint(response.Message);
        } else {
          $scope.order.bean = response;
          $ionicLoading.show({
            noBackdrop: true,
            template: "您的" + $scope.order.bean.ORDER_TYPE + "已经提交成功！",
            duration: 5000
          });
          $state.go('orderList', {orderType:'维保',reload: true});  //路由跳转
        }
      });
    },
    getMyAddress: function () {
      if (this.bean.REACH_TYPE == "上门接车") {
        toPost.list("AddressList", {}, function (response) {
          $scope.order.addressOptions = response.data;
          console.log(response);
        });
      }
    },
//根据不同的送修方式，显示相关字段
    showField: function () {
      if (this.bean.REACH_TYPE == "上门接车") {
        $("#reachDiv").show();
        $("#ReachTimeDiv").hide();
      } else if (this.bean.REACH_TYPE == "自行送店") {
        $("#ReachTimeDiv").show();
        $("#reachDiv").hide();
      }
    }
  };

  if ($stateParams.bean) {
    $scope.order.bean = $stateParams.bean;
  }

  if ($stateParams.location) {
    console.log($stateParams.location)
    $scope.order.bean.ADDRESS = $stateParams.location.address;
    $scope.order.bean.LANG = $stateParams.location.lng;
    $scope.order.bean.LAT = $stateParams.location.lat;

  }
})
;
