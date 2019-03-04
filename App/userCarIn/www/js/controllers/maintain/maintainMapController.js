/**
 * Created by Administrator on 2016/8/17.
 */
mainController.controller('maintainMapCtr', function ($scope,$timeout, common,$ionicModal, $ionicPopover,storageUserFac, $stateParams, $ionicPopup, $state, toPost, baiduMap) {
  $scope.popup=null;
  $scope.nowGarage=null
  $ionicModal.fromTemplateUrl('templates/FlowAdd.html', {
    scope: $scope,
    animation: 'slide-in-up'
  }).then(function (modal) {
    $scope.modal = modal;
  });

  $scope.openPopover = function($event) {
    console.log($scope.popup)
    $scope.popup.close()
    $scope.modal.show();
  };

  var map = null;
  $scope.maintain = {
    myMark: null,
    counter: null,
    reLoad: function () {
      if (storageUserFac.getUser().ID == null) {
        $state.go('login', {reload: true});
      }

      // 定义高度
      document.getElementById('allmap').style.height = $(document).height() - 44 + "px";
      map = new BMap.Map("allmap");
      map.centerAndZoom(new BMap.Point(104.072366, 30.66367), 11);
      var postBean = {};
      toPost.list("RescueQuery", postBean, this.garageCallback);
      this.getNewMark();
    },
    getNewMark: function () {
      map.removeOverlay($scope.maintain.myMark);
      baiduMap.getGeo(function (location) {
        var point = new BMap.Point(location.lng, location.lat);
        var myIcon = new BMap.Icon("http://api.map.baidu.com/img/markers.png", new BMap.Size(23, 25), {
          offset: new BMap.Size(10, 25), // 指定定位位置
          imageOffset: new BMap.Size(0, 0 - 10 * 25) // 设置图片偏移
        });
        $scope.maintain.myMark = new BMap.Marker(point, {icon: myIcon});  // 创建标注
        map.addOverlay($scope.maintain.myMark);              // 将标注添加到地图中
        map.panTo(point);
      }, {'isLoading': false});
    },
    garageCallback: function (currMsg) {
      if (currMsg.IsError) {
        common.hint(currMsg.Message);
        $state.go('login', {reload: true});  //路由跳转
        return;
      }
      for (var i = 0; i < currMsg.data.length; i++) {
        var new_point = new BMap.Point(currMsg.data[i].LANG, currMsg.data[i].LAT);
        var myIcon = new BMap.Icon("http://api.map.baidu.com/img/markers.png", new BMap.Size(23, 25), {
          offset: new BMap.Size(10, 25), // 指定定位位置
          imageOffset: new BMap.Size(0, 0 - 11 * 25) // 设置图片偏移
        });
        var marker = new BMap.Marker(new_point, {icon: myIcon});  // 创建标注
        marker.data = currMsg.data[i];
        map.addOverlay(marker);              // 将标注添加到地图中
        marker.addEventListener("click", function (e) {
          this.setAnimation(BMAP_ANIMATION_BOUNCE); //跳动的动画

          $scope.nowGarage=e.target.data;
          var myPopup=$ionicPopup.confirm({
            scope: $scope,
            title: e.target.data.NAME,
            subTitle: '地址：' + e.target.data.ADDRESS,
            template: '<p>座机：' + e.target.data.PHONE + '</p><p>手机：' + e.target.data.REMARK + '</p>',
            cancelText: '取消',
            okText: '确定',
            buttons: [
              { text: '取消' },
              { text: '查看详情',
                type: 'button-positive',
                onTap: function(e) {
                  $state.go('garageInfo', {
                    id:  $scope.nowGarage.ID,
                    reload: true
                  });
                }
              },
              {
                text: '<b>确定</b>',
                type: 'button-positive',
                  onTap: function(e) {
                  var garage = {
                    garageId: $scope.nowGarage.ID,
                    garageName: $scope.nowGarage.NAME
                  };
                  common.tempData = garage;
                  $state.go('editHelpOrder', {
                    garage: garage,
                    id: $stateParams.id,
                    orderType:'维修',
                    reload: true
                  });
                }
              },
            ]
          }).then(function (res) {
            console.log(res);
            if (res) {

            } else {
              console.log('取消了"' + e.target.data.NAME + '"');
            }
          });
          myPopup.then(function(res) {
            console.log('Tapped!', res);
          });
        });
      }
    }
  };


});
