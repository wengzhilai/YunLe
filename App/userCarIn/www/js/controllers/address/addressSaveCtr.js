/**
 * Created by wengzhilai on 2016/8/18.
 */
/**
 * Created by Administrator on 2016/8/9.
 */
mainController.controller('addressSaveCtr', function ($scope, $ionicLoading, $ionicPopup, toPost, baiduMap, storageUserFac, $state, $stateParams, $cordovaGeolocation) {
    $scope.address = {
      currEnt: {},
      selectLat: function () {
        var location = {"lng": $scope.address.bean.LANG, "lat": $scope.address.bean.LAT};
        $scope.myPopup = $ionicPopup.show({
          scope: $scope,
          title: '定位方式选择',
          buttons: [
            { //Array[Object] (可选)。放在弹窗footer内的按钮。
              text: '手动定位',
              type: 'button-positive',
              onTap: function (e) {
                console.log('手动定位');
                $state.go('selectMyLat', {location: location, url: "addressSave", id: $stateParams.id, reload: true});  //路由跳转
              }
            },
            {
              text: '自动定位',
              type: 'button-positive',
              onTap: function (e) {
                console.log('自动定位');
                baiduMap.getGeo(function (thisLocation) {
                  $scope.address.bean.LANG = thisLocation.lng;
                  $scope.address.bean.LAT = thisLocation.lat;
                  $scope.address.bean.ADDRESS = thisLocation.address;
                })
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
      bean: {
        NAME: storageUserFac.getUserName(),
        PHONE: storageUserFac.getUserPhone()
      },
      save: function () {
        toPost.saveOrUpdate("AddressSave", this.bean, saveReturn);
      }
    };
    var saveReturn = function (currMsg) {
      console.log(currMsg);
      if (currMsg.IsError) {
        $ionicLoading.show({
          noBackdrop: true,
          template: currMsg.Message,
          duration: 5000
        });
      }
      else {
        $ionicLoading.show({
          noBackdrop: true,
          template: '保存成功！',
          duration: 2000
        });
        $scope.address.bean.ID = currMsg.ID;
        $state.go('addressList', {reload: true});  //路由跳转
      }
    };

    var SingleReturn = function (currMsg) {
      console.log(currMsg);
      $scope.address.bean = currMsg;
      if ($stateParams.location) {
        $scope.address.bean.LANG = $stateParams.location.lng;
        $scope.address.bean.LAT = $stateParams.location.lat;
      }
    };

    if ($stateParams.id) {
      toPost.single("AddressSingle", $stateParams.id, SingleReturn);
    }

    if ($stateParams.location) {
      $scope.address.bean.LANG = $stateParams.location.lng;
      $scope.address.bean.LAT = $stateParams.location.lat;
      $scope.address.bean.ADDRESS = $stateParams.location.address;
    }
  }
)
;
