/**
 * Created by Administrator on 2016/8/8.
 */
mainController.controller('selectGarageCtr', function ($ionicPlatform, $scope, common, storageUserFac, $stateParams, $ionicPopup, $state, toPost, baiduMap, $location) {
  var map = null;
  $scope.help = {
    myMark: null,
    counter: null,
    goToHome: function () {
      $state.go('home', {reload: true});
    },
    goTohelpOrderList: function () {
      $state.go('helpOrderList', {reload: true});
    },
    init: function () {
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
      map.removeOverlay(this.myMark);
      baiduMap.getGeo(function (location) {
        var point = new BMap.Point(location.lng, location.lat);
        var myIcon = new BMap.Icon("http://api.map.baidu.com/img/markers.png", new BMap.Size(23, 25), {
          offset: new BMap.Size(10, 25), // 指定定位位置
          imageOffset: new BMap.Size(0, 0 - 10 * 25) // 设置图片偏移
        });
        this.myMark = new BMap.Marker(point, {icon: myIcon});  // 创建标注
        map.addOverlay(this.myMark);              // 将标注添加到地图中
        map.panTo(point);
      }, {'isLoading': false});
    },
    garageCallback: function (currMsg) {
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
          $ionicPopup.confirm({
            title: e.target.data.NAME,
            subTitle: '地址：' + e.target.data.ADDRESS,
            template: '<p>座机：' + e.target.data.PHONE + '</p><p>手机：' + e.target.data.REMARK + '</p>',
            cancelText: '取消',
            okText: '确定'
          }).then(function (res) {
            if (res) {
              var garage = {
                garageId: e.target.data.ID,
                garageName: e.target.data.NAME
              };
              common.tempData = garage;
              $state.go('editHelpOrder', {
                location: $stateParams.location,
                bean:$stateParams.bean,
                garage: garage,
                id: $stateParams.id,
                reload: true
              });
            } else {
              console.log('取消了"' + e.target.data.NAME + '"');
            }
          });

        });
      }
    }
  };


});
