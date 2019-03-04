/**
 * Created by wengzhilai on 2016/9/20.
 */
mainService.factory('baiduMap', function ($rootScope, $cordovaGeolocation, Storage, CarIn, common) {
  var posOptions = {timeout: 10000, enableHighAccuracy: false},
    msg = {},
    reLocation = {},
    storageKey = 'user',
    user = Storage.get(storageKey);

  return {
    //在地图中查找关键字位置
    searBaidu: function (map, value) {
      common.showLoading();
      var options = {
        onSearchComplete: function (results) {
          if (local.getStatus() == BMAP_STATUS_SUCCESS) {
            map.clearOverlays();
            msg = results;
            $rootScope.$broadcast('baiduMap.Search');
          } else {
            msg = {IsError: true, Message: '搜索位置失败请使用准确的关键字！'};
            common.showError(msg);
          }

        }
      }
      var local = new BMap.LocalSearch(map, options);
      local.search(value);
    },
    //获取经纬度和地址
    getGeo: function (callBack, para) {
      var getAddressFun = this.getAddress;
      var option = {maximumAge: 3000, timeout: 10000, enableHighAccuracy: true};
      //是微信
      if (navigator.userAgent.toLowerCase().indexOf('micromessenger') > -1) {
        if (para == null || para.isLoading) {
          common.showLoading();
        }
        wx.getLocation({
          type: 'wgs84', // 默认为wgs84的gps坐标，如果要返回直接给openLocation用的火星坐标，可传入'gcj02'
          success: function (res) {
            var latitude = res.latitude; // 纬度，浮点数，范围为90 ~ -90
            var longitude = res.longitude; // 经度，浮点数，范围为180 ~ -180。
            var speed = res.speed; // 速度，以米/每秒计
            var accuracy = res.accuracy; // 位置精度
            reLocation = {"lng": longitude, "lat": latitude}
            BMap.Convertor.translate(reLocation, 0, function (location) {
              reLocation = location;
              common.hideLoading();
              getAddressFun(location, function (address) {
                reLocation.address = address;
                if (callBack) {
                  callBack(reLocation);
                }
              });
            });
          }
        });
      }
      else if(ionic.Platform.isAndroid() || ionic.Platform.isIOS()) {
        if (para == null || para.isLoading) {
          common.showLoading();
        }
        $cordovaGeolocation.getCurrentPosition(option).then(
          function (position) {
            //alert('纬度: '          + position.coords.latitude          + '\n' +
            //  '经度: '         + position.coords.longitude         + '\n' +
            //  '海拔: '          + position.coords.altitude          + '\n' +
            //  '水平精度: '          + position.coords.accuracy          + '\n' +
            //  '垂直精度: ' + position.coords.altitudeAccuracy  + '\n' +
            //  '方向: '           + position.coords.heading           + '\n' +
            //  '速度: '             + position.coords.speed             + '\n' +
            //  '时间戳: '         + position.timestamp                + '\n');

            reLocation = {"lng": position.coords.longitude, "lat": position.coords.latitude}
            BMap.Convertor.translate(reLocation, 0, function (location) {
              reLocation = location;
              common.hideLoading();
              getAddressFun(location, function (address) {
                reLocation.address = address;
                if (callBack) {
                  callBack(reLocation);
                }
              });
            });
          },
          function (err) {
            var errStr = "";
            switch (err.code) {
              case 1 :
                errStr = ("用户选了不允许");//用户选了不允许
                break;
              case 2:
                errStr = ("连不上GPS卫星，或者网络断了");
                //
                break;
              case 3:
                errStr = ("超时了");//超时了
                break;
              default:
                errStr = ("未知错误");//未知错误，其实是err.code==0的时候
                break;
            }
            common.showError(errStr);
          });
      }

    },
    //获取地址
    getAddress: function (location, callBack) {
      common.showLoading();
      var url = "http://api.map.baidu.com/geocoder/v2/?ak=xpiepfbv6zvlVSbLGfMg5sr1&callback=renderReverse&location=" + location.lat + "," + location.lng + "&output=json&pois=0";
      $.ajax({
        url: url,
        type: 'GET',
        dataType: 'JSONP',
        success: function (jsonObj) {
          if (callBack) {
            callBack(jsonObj.result.formatted_address);
          }
          common.hideLoading();
        }
      });
    },
    getCurrentMes: function () {
      common.hideLoading();
      return msg;
    }
  }

})
