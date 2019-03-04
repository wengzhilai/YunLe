/**
 * Created by Administrator on 2016/8/8.
 */
mainController.controller('editHelpOrderCtr', function ($scope,$timeout, fileUpDel, baiduMap, common, $ionicLoading, $stateParams, $state, $cordovaCamera, $ionicPopup, fileUpFac, $cordovaImagePicker, toPost, storageUserFac, $ionicActionSheet, $cordovaGeolocation) {
  $scope.user = storageUserFac.getUser();
  //当前位置
  var location = null;
  //维修站
  var garage = null;

  //定义接收选择结果变量
  $scope.data = {result: ""};

  $scope.helpOrder = {
    reLoad: function () {
      console.log('传入参数');
      console.log($stateParams);
      if($stateParams.orderType){
        this.bean.ORDER_TYPE=$stateParams.orderType;
      }
      if ($scope.user.garage != null) {
        garage = {garageId: $scope.user.garage.ID, garageName: $scope.user.garage.NAME}
        this.bean.GARAGE_ID = garage.garageId;
        this.bean.GarageName = garage.garageName;
      }
      if ($stateParams.user) {
        var thisClient = $stateParams.user;
        this.bean.CLIENT_NAME = thisClient.NAME;
        this.bean.CLIENT_PHONE = thisClient.LOGIN_NAME;
        this.bean.PLATE_NUMBER = thisClient.NowCar == null ? '' : thisClient.NowCar.PLATE_NUMBER;
        this.bean.CAR_TYPE = thisClient.NowCar == null ? '' : thisClient.NowCar.MODEL;
        this.bean.BRAND = thisClient.NowCar == null ? '' : thisClient.NowCar.BRAND;
        this.bean.MODEL = thisClient.NowCar == null ? '' : thisClient.NowCar.FRAME_NUMBER;
        this.bean.CAR_ID = thisClient.NowCar == null ? '' : thisClient.NowCar.ID;
        this.bean.CLIENT_ID = thisClient.ID;
      }
      else if($scope.user.SALESMAN_ID!=null){
        var thisClient = $scope.user;
        this.bean.CLIENT_NAME = thisClient.NAME;
        this.bean.CLIENT_PHONE = thisClient.LOGIN_NAME;
        this.bean.PLATE_NUMBER = thisClient.NowCar == null ? '' : thisClient.NowCar.PLATE_NUMBER;
        this.bean.CAR_TYPE = thisClient.NowCar == null ? '' : thisClient.NowCar.MODEL;
        this.bean.BRAND = thisClient.NowCar == null ? '' : thisClient.NowCar.BRAND;
        this.bean.MODEL = thisClient.NowCar == null ? '' : thisClient.NowCar.FRAME_NUMBER;
        this.bean.CAR_ID = thisClient.NowCar == null ? '' : thisClient.NowCar.ID;
        this.bean.CLIENT_ID = thisClient.ID;
      }
      if ($stateParams.bean) {
        this.bean = $stateParams.bean;
      }
      if ($stateParams.id != 0 && $stateParams.id != null) {
        $scope.helpOrder.bean.ID = $stateParams.id;
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
      if ($stateParams.garage) {
        garage = $stateParams.garage;
        this.bean.GARAGE_ID = garage.garageId;
        this.bean.GarageName = garage.garageName;
      }
      if ($stateParams.location) {
        location = $stateParams.location;
        $scope.helpOrder.bean.LANG = location.lng;
        $scope.helpOrder.bean.LAT = location.lat;
        baiduMap.getAddress(location, function (address) {
          $scope.helpOrder.bean.ADDRESS = address;
        })
      }
      if (location == null) {
        baiduMap.getGeo(function (myLocation) {
          location = {"lng": myLocation.lng, "lat": myLocation.lat}
          $scope.helpOrder.bean.LANG = myLocation.lng;
          $scope.helpOrder.bean.LAT = myLocation.lat;
          $scope.helpOrder.bean.ADDRESS = myLocation.address;
        });
      }
      console.log(garage);
    },
    currEnt: {},
    bean: {
      ID: 0,
      ORDER_NO: '',
      REACH_TYPE: '接车',
      ORDER_TYPE: '救援',
      REMARK: '',
      HITCH_TYPE: '',
      REACH_TIME: '',
      AllFiles: [],
      AllFlow: [],
      CLIENT_NAME: $scope.user.NAME,
      CLIENT_PHONE:$scope.user.LOGIN_NAME
    },
    selectLat: function () {
      $scope.myPopup = $ionicPopup.show({
        scope: $scope,
        title: '定位方式选择',
        buttons: [
          { //Array[Object] (可选)。放在弹窗footer内的按钮。
            text: '手动定位',
            type: 'button-positive',
            onTap: function (e) {
              console.log('手动定位');
              $state.go('selectMyLat', {
                location: location,
                garage: garage,
                id: $stateParams.id,
                bean: $scope.helpOrder.bean,
                reload: true
              });  //路由跳转
            }
          },
          {
            text: '自动定位',
            type: 'button-positive',
            onTap: function (e) {
              console.log('自动定位');
              baiduMap.getGeo(function (myLocation) {
                location = {"lng": myLocation.lng, "lat": myLocation.lat}
                $scope.helpOrder.bean.LANG = myLocation.lng;
                $scope.helpOrder.bean.LAT = myLocation.lat;
                $scope.helpOrder.bean.ADDRESS = myLocation.address;
              });
            }
          },
          {
            text: '关闭'
          }
        ]
      }).then(function (res) {
        if (res) {

        } else {
        }
      });
    },
    selectGarage: function () {
      $state.go('selectGarage', {
        location: location,
        garage: garage,
        id: $stateParams.id,
        bean: this.bean,
        reload: true
      });  //路由跳转
    },
    save: function () {
      if ($scope.helpOrder.bean.LANG == null || $scope.helpOrder.bean.LAT == null) {
        common.hint("车辆位置，还未设置");
        return;
      }
      toPost.saveOrUpdate("RescueSave", this.bean, function (response) {
        if (response.IsError) {
          common.hint(response.Message);
        } else {
          common.hint("您的订单已经提交成功");
          $state.go('orderList', {orderType:$scope.helpOrder.bean.ORDER_TYPE,reload: true});  //路由跳转
        }
      });
    },
    AddImg: function () {
      var indexNo =$scope.helpOrder.bean.AllFiles.length;
      $scope.helpOrder.bean.AllFiles[indexNo] = {"indexNo": $scope.helpOrder.bean.AllFiles.length};
      var jqObj=$("img[data-indexNo='"+indexNo+"']");
      var timer = $timeout(
        function() {
          var jqObj=$("img[data-indexNo='"+indexNo+"']");
          $scope.helpOrder.currEnt = $(jqObj[0]);
          fileUpFac.upImg($scope.helpOrder.currEnt, $scope.helpOrder.bean.AllFiles[indexNo].ID, $scope.helpOrder.upCallback,$scope);
        },
        100
      );

    },
    upImg: function (obj) {
      $scope.helpOrder.currEnt = $(obj.target);
      var indexNo = $scope.helpOrder.currEnt.attr("data-indexNo");
      fileUpFac.upImg( $scope.helpOrder.currEnt, $scope.helpOrder.bean.AllFiles[indexNo].ID, $scope.helpOrder.upCallback,$scope);
    },
    upCallback: function (result) {
      //alert( $scope.car.bean.DRIVING_PIC_ID+"前："+ window.JSON.stringify($scope.car.bean));
      //$scope.helpOrder.currEnt.target.setAttribute("data-id", window.JSON.parse(result.response).indexNo);
      var indexNo = $scope.helpOrder.currEnt.attr("data-indexNo");
      $scope.helpOrder.bean.AllFiles[indexNo].ID = result.ID;
      $scope.helpOrder.bean.AllFiles[indexNo].URL = result.URL;
    },
    showBigImage: function (ent) {
      var url = ent.target.getAttribute("src");
      if (url == null || url == '') {
        return;
      }
      fileUpFac.FullScreenImage(url);
    }
  };
});
