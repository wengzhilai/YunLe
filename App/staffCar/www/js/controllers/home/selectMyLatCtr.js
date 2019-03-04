/**
 * Created by Administrator on 2016/8/9.
 */
mainController.controller('selectMyLatCtr', function ($scope, $timeout, common, baiduMap, storageUserFac, $stateParams, $ionicPopup, $state) {
  console.log($stateParams)
  var garage = null;
  var map = null;
  var myLocation = null;
  var url = "editHelpOrder";
  if ($stateParams.url != null) {
    url = $stateParams.url;
  }
  $scope.Lat = {
    searchKey: "",
    allPlace: [],
    ShowList: true,
    reLoad: function () {
      console.log($stateParams)
      myLocation = $stateParams.location;
      console.log(myLocation);
      document.getElementById('maplocatin').style.height = $(document).height() - 44 + "px";
      map = new BMap.Map("maplocatin");
      map.addEventListener("click", $scope.Lat.createMark, true);
      map.centerAndZoom(new BMap.Point(104.072366, 30.66367), 11);
      map.enableScrollWheelZoom();   //启用滚轮放大缩小，默认禁用
      map.enableContinuousZoom();    //启用地图惯性拖拽，默认禁用
      map.closeInfoWindow();
      map.disableDoubleClickZoom();
      if ($stateParams.location != null && $stateParams.location.lng != null) {
        $scope.Lat.createMark({"title": "", "point": {"lng": myLocation.lng, "lat": myLocation.lat}});
      }
    },
    ShowName: function (poi) {
      if (poi == null) {
        $scope.Lat.ShowList = !$scope.Lat.ShowList;
      }
      else {
        $scope.Lat.ShowList = false;

        $scope.Lat.createMark(poi);
      }
    },
    SearchName: function () {
      common.showLoading();

      var city = new BMap.LocalSearch(map, {renderoptions: {map: map, autoviewport: true}});
      map.clearOverlays(); //清除地图上所有标记
      city.search("成都市");//查找城市
      var ls = new BMap.LocalSearch($scope.Lat.searchKey);
      ls.search($scope.Lat.searchKey);
      var i = 1;
      //$scope.Lat.allPlace=[{"title":$scope.Lat.searchKey,"point":{"lng":"1","lat":"2"}}];
      ls.setSearchCompleteCallback(function (rs) {
        $scope.Lat.allPlace = [];
        if (ls.getStatus() == BMAP_STATUS_SUCCESS) {
          for (j = 0; j < rs.getCurrentNumPois(); j++) {
            var poi = rs.getPoi(j);
            $scope.Lat.allPlace[$scope.Lat.allPlace.length] = {
              "title": poi.title,
              "point": {"lng": poi.point.lng, "lat": poi.point.lat}
            };
          }
        }
        common.hideLoading();

      });
      $scope.Lat.ShowList = true;

    },
    toEditHelp: function () {
      $state.go(url, {location: myLocation, garage: $stateParams.garage, id: $stateParams.id, reload: true});  //路由跳转
    },
    createMark: function (e) {
      //alert(e.point.lng + "," + e.point.lat);
      map.clearOverlays(); //清除地图上所有标记

      var new_point = new BMap.Point(e.point.lng, e.point.lat);
      map.setCenter(new_point);
      map.panTo(new_point);
      var marker = new BMap.Marker(new_point);  // 创建标注

      map.addOverlay(marker);              // 将标注添加到地图中
      marker.enableDragging();
      myLocation = {lng: e.point.lng, lat: e.point.lat}
      // map.removeEventListener("click", $scope.Lat.createMark, true);
      marker.addEventListener("click", $scope.Lat.selectMark, true);
    },
    createPointMark: function (lng, lat) {
      //alert(e.point.lng + "," + e.point.lat);
      var new_point = new BMap.Point(lng, lat);
      var marker = new BMap.Marker(new_point);  // 创建标注
      map.addOverlay(marker);              // 将标注添加到地图中
      map.panTo(new_point);
      marker.enableDragging();
      marker.addEventListener("click", $scope.Lat.selectMark, true);
    },
    selectMark: function (e) {
      //this.disableDragging();
      // var marketpoint = this.getPosition();
      // myLocation = {lng: marketpoint.lng, lat: marketpoint.lat};
      baiduMap.getAddress(myLocation, function (address) {
        myLocation.address = address;
        $state.go(url, {
          location: myLocation,
          garage: $stateParams.garage,
          id: $stateParams.id,
          bean: $stateParams.bean,
          reload: true
        });
      });


//            var myIcon = new BMap.Icon("http://developer.baidu.com/map/jsdemo/img/fox.gif", new BMap.Size(300, 157));
//            this.setIcon(myIcon);
//
//            this.removeEventListener("click", $scope.Lat.selectMark, true);
//            this.addEventListener("click", $scope.Lat.reSelectMark, true);
      //this.setAnimation(BMAP_ANIMATION_BOUNCE); //跳动的动画
    },
    reSelectMark: function (e) {
      var marketpoint = this.getPosition();
      var confirmPopup = $ionicPopup.confirm({
        title: '选好位置',
        template: '是否现在确认当前定位?',
        okText: '确认',
        cancelText: '取消选择'
      });
      confirmPopup.then(function (res) {
        if (res) {
          myLocation = {lng: marketpoint.lng, lat: marketpoint.lat};
          $state.go('editHelpOrder', {
            location: myLocation,
            garage: $stateParams.garage,
            id: $stateParams.id,
            reload: true
          });  //路由跳转
        } else {
          map.clearOverlays();
          map.addEventListener("click", $scope.Lat.createMark, true);
        }
      });
    }

  };
});
