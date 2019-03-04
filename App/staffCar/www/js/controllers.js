/**
 * Created by Administrator on 2016/8/7.
 */
var mainController=angular.module('starter.controllers', []);

/*处理IOS9 $digest()时，检测超限报错BUG*/
angular.module('ngIOS9UIWebViewPatch', ['ng']).config(['$provide', function ($provide) {
  'use strict';

  $provide.decorator('$browser', ['$delegate', '$window', function ($delegate, $window) {

    if (isIOS9UIWebView($window.navigator.userAgent)) {
      return applyIOS9Shim($delegate);
    }

    return $delegate;

    function isIOS9UIWebView(userAgent) {
      return /(iPhone|iPad|iPod).* OS 9_\d/.test(userAgent) && !/Version\/9\./.test(userAgent);
    }

    function applyIOS9Shim(browser) {
      var pendingLocationUrl = null;
      var originalUrlFn = browser.url;

      browser.url = function () {
        if (arguments.length) {
          pendingLocationUrl = arguments[0];
          return originalUrlFn.apply(browser, arguments);
        }

        return pendingLocationUrl || originalUrlFn.apply(browser, arguments);
      };

      window.addEventListener('popstate', clearPendingLocationUrl, false);
      window.addEventListener('hashchange', clearPendingLocationUrl, false);

      function clearPendingLocationUrl() {
        pendingLocationUrl = null;
      }

      return browser;
    }
  }]);
}]);



/**
 * Created by wengzhilai on 2016/11/6.
 */
mainController.controller('bbsListCtr', ["$ionicPopup", "common", "$scope", "toPost", "$ionicLoading", "$ionicModal", "fileUpFac", "storageUserFac", function ($ionicPopup,common,$scope, toPost, $ionicLoading, $ionicModal,fileUpFac,storageUserFac) {
  $scope.callPhone = function (mobilePhone) {
    common.callPhone(mobilePhone);
  };
  $scope.user=storageUserFac.getUser();
  $ionicModal.fromTemplateUrl('templates/bbs/bbsAdd.html', {
    scope: $scope
  }).then(function(modal) {
    $scope.modal = modal;
  });
  $scope.AllMessageType=[];
  $scope.addBean={
    MESSAGE_TYPE_ID:1,
    CONTENT:'',
    AllFiles:[],
    fileIdStr:''
  }
  $scope.func={
    upImg: function (obj) {
      $scope.bbsList.currEnt = $(obj.target);
      var indexNo = $scope.bbsList.currEnt.attr("data-indexNo");
      fileUpFac.upImg($scope.bbsList.currEnt, $scope.addBean.AllFiles[indexNo].ID, $scope.func.upCallback,$scope);
    },
    upCallback: function (result) {
      var indexNo = $scope.bbsList.currEnt.attr("data-indexNo");
      $scope.addBean.AllFiles[indexNo].ID = result.ID;
      $scope.addBean.AllFiles[indexNo].URL = result.URL;
    },
    AddImg:function(){
      $scope.addBean.AllFiles[$scope.addBean.AllFiles.length] = {"indexNo": $scope.addBean.AllFiles.length};
    },
    createEnt:function(){
      for(var i=0;i<$scope.addBean.AllFiles.length;i++)
      {
        if($scope.addBean.AllFiles[i].ID!=null)
        {
          $scope.addBean.fileIdStr+=","+$scope.addBean.AllFiles[i].ID;
        }
      }
      toPost.saveOrUpdate("BbsSave",$scope.addBean,function(currMsg){
        if (!currMsg.IsError){
          $ionicLoading.show({
            noBackdrop: true,
            template: "保存成功",
            duration: 2000
          });
          $scope.bbsList.doRefresh();
        }
        else {
        }
        $scope.modal.hide();
      });
    }
  }


  $scope.bbsList = {
    nowTabsIndex:0,
    bbsReplay:'',
    currEnt:null,
    bean: {
      userId: 0,
      authToken: '',
      pageSize: 0,
      id: 0,
      currentPage: 0,
      searchKey: [],
      para:[],
      orderBy: []
    },
    lists: {},
    init:function () {
      toPost.Post("MessageTypeAll",{authToken : storageUserFac.getUserAuthToken()},function (mageType) {
        $scope.AllMessageType=mageType;
      })
      this.loadMore();
    },
    onTab:function(obj,index){
      $(".tab-item").removeClass("active")
      $(obj.target).addClass("active");
      this.nowTabsIndex=index;
      this.doRefresh();
    },
    hasNextPage: function () {
      if (this.lists.totalPage == null || this.lists.totalPage == 0) {
        return false;
      }
      else if (this.lists.totalPage <= this.lists.currentPage) {
        return false;
      }
      return true;
    },
    doRefresh: function () {
      console.log("下拉刷新");
      this.bean.currentPage = 1;

      var buB = $("a[class='button button-balanced']").text();
      switch (buB)
      {
        case "经验交流":
          this.bean.searchKey=[{K: "MESSAGE_TYPE_ID", V: 3, T: '=='}];
          break;
        case "问题咨询":
          this.bean.searchKey=[{K: "MESSAGE_TYPE_ID", V: 1, T: '=='}];
          break;
        case "修改意见":
          this.bean.searchKey=[{K: "MESSAGE_TYPE_ID", V: 2, T: '=='}];
          break;
      }
      this.bean.para=[{K: "type", V: this.nowTabsIndex, T: '=='}];
      toPost.list("bbsList", this.bean, this.callListReturn);
    },
    callListReturn:function(currMsg){
      if (currMsg.IsError == true) {
        $ionicLoading.show({
          noBackdrop: true,
          template: currMsg.Message,
          duration: 2000
        });
      } else {
        $scope.bbsList.lists = currMsg;
      }
      $scope.$broadcast('scroll.refreshComplete');
      $scope.$broadcast('scroll.infiniteScrollComplete');
    },
    loadMore: function () {
      if (this.bean.currentPage == 0) {
        this.bean.currentPage = 1;
      } else {
        this.bean.currentPage++;
      }
      console.log("加载更多");
      toPost.list("bbsList", this.bean, this.callListReturn);
    },
    filterKey: function (obj) {
      if ($("a[class='button button-balanced']").length != 0) {
        $("a[class='button button-balanced']").each(function () {
          $(this).attr("class", "button");
        });
      }
      obj.target.setAttribute("class", "button button-balanced");
      this.doRefresh();
    },
    toFollowOrder:function (bbsBean) {
      $scope.bbsList.bbsReplay="";
      // 自定义弹窗
      var myPopup = $ionicPopup.show({
        template: '<input type="text" ng-model="bbsList.bbsReplay">',
        title: '请输入回内容',
        subTitle:bbsBean.CONTENT ,
        scope: $scope,
        buttons: [
          { text: '取消' },
          {
            text: '<b>回复</b>',
            type: 'button-positive',
            onTap: function(e) {
              if ($scope.bbsList.bbsReplay=="") {

                common.hint( '回复信息必填');
              } else {
                var postBean = {
                  CONTENT:$scope.bbsList.bbsReplay,
                  PARENT_ID:bbsBean.ID
                };
                toPost.saveOrUpdate("BbsSave",postBean,function (currMsg) {
                  if (!currMsg.IsError){
                    $ionicLoading.show({
                      noBackdrop: true,
                      template: "保存成功",
                      duration: 2000
                    });
                    $scope.bbsList.doRefresh();
                  }

                })
                return true;
              }
            }
          },
        ]
      });
    }
  };
}])



mainController.controller('homeCtr', ["$scope", "$state", "storageUserFac", "toPost", "$ionicPopup", "common", function ($scope, $state, storageUserFac,toPost,$ionicPopup,common) {
  $scope.user = storageUserFac.getUser();
  $scope.toHelp = function (id) {
    $state.go('helpList', {reload: true});
  }
  $scope.toMessageList = function () {
    $state.go('messageList', {reload: true});
  }
  $scope.alert = function (str) {
    common.hint(str);
  }
  $scope.home = {
    init: function () {
      $scope.user = storageUserFac.getUser();
      console.log($scope.user)
      if($scope.user==null || $scope.user.authToken==null || $scope.user.authToken==''){
        $state.go('login', {reload: true});  //路由跳转
      }
      else {
        toPost.single("SalesmanSingle",$scope.user.ID,function (currMsg) {
          console.log(currMsg)
          if (currMsg.IsError) {
            common.hint(currMsg.Message);
            $state.go('login', {reload: true});  //路由跳转
          } else {
            $scope.user=currMsg;
            storageUserFac.setUser(currMsg);
          }
        })
      }
    },
    toTeam: function () {
      if (storageUserFac.getTeamId) {
        common.hint("您已经加入团队，如需更新团队请联系管理员");
      } else {
        cordova.plugins.barcodeScanner.scan(
          function (result) {
            salesmanFac.salesmanToTeam(result.text);
          },
          function (error) {
            common.hint("Scanning failed: " + error);
          }
        );

      }
    }
  };

}])

/**
 * Created by Administrator on 2016/8/8.
 */
mainController.controller('selectGarageCtr', ["$ionicPlatform", "$scope", "common", "storageUserFac", "$stateParams", "$ionicPopup", "$state", "toPost", "baiduMap", "$location", function ($ionicPlatform, $scope, common, storageUserFac, $stateParams, $ionicPopup, $state, toPost, baiduMap, $location) {
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


}]);

/**
 * Created by Administrator on 2016/8/9.
 */
mainController.controller('selectMyLatCtr', ["$scope", "$timeout", "common", "baiduMap", "storageUserFac", "$stateParams", "$ionicPopup", "$state", function ($scope, $timeout, common, baiduMap, storageUserFac, $stateParams, $ionicPopup, $state) {
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
}]);

/**
 * Created by wengzhilai on 2016/8/7.
 */
mainController.controller('editInsureCtr', ["$scope", "toPost", "fileUpFac", "$ionicPopup", "$ionicLoading", "Storage", "carFac", "$cordovaCamera", "$cordovaImagePicker", "$stateParams", "fileUpFac", "insureFac", "$state", "$ionicActionSheet", function ($scope,toPost, fileUpFac,$ionicPopup, $ionicLoading, Storage, carFac,$cordovaCamera,$cordovaImagePicker, $stateParams,fileUpFac, insureFac, $state, $ionicActionSheet) {


    $scope.OrderInsureSingleCallBack=function (currMsg) {
        $scope.insure.bean = currMsg;
    };
    if ($stateParams.id) {
        toPost.single("OrderInsureSingle",$stateParams.id,$scope.OrderInsureSingleCallBack)
    }

    $scope.insure = {
        bean: {
            CAR_ID: 0,
            CLIENT_ID: '',
            INSURER_ID: 0,
            CAR_USERNAME: '',
            ID_NO: '',
            CAR_OWNER: '',
            SaveProductId: [],
            ID_NO_PIC_ID: '',
            DRIVING_PIC_ID:'',
            DRIVER_PIC_ID:'',
            AllInsurePrice:[]
        },
        maxMoney:300000,
        userInfo: '',
        carInfo: '',
        carsOptions: [],
        insurerOptions: [],
        insurerInfo: {},
        currEnt:{},
        removeProduct:function(productId){
            console.log($scope.insure.insurerInfo);
            for(var i=0;i<$scope.insure.insurerInfo.AllProductPrice.length;i++)
            {
                if($scope.insure.insurerInfo.AllProductPrice[i].ID==productId && $scope.insure.insurerInfo.AllProductPrice[i].IS_MUST==0)
                {
                    $scope.insure.insurerInfo.AllProductPrice.splice(i,1);
                }
            }

        },
        showBigImage: function (ent) {
            var url =$(ent.target).attr("src");
            if (url == null || url == '') {
                //选择上传图片
                this.upImg(ent);
                return;
            }
            fileUpFac.FullScreenImage(url,$scope);
        },
        showCarInfo: function () {
            this.carsOptions.forEach(function (i) {
                if (i.ID == $scope.insure.bean.CAR_ID) {
                    $scope.insure.carInfo = i;
                    $scope.insure.bean.DRIVING_PIC_ID = i.DRIVING_PIC_ID;
                    insureFac.getInsureByCar($scope.insure.carInfo);
                    return;
                }
            });
        },
        showInsurerInfo: function () {
            this.insurerOptions.forEach(function (i) {
                if (i.ID == $scope.insure.bean.INSURER_ID) {
                    $scope.insure.insurerInfo = i;
                    return;
                }
            });
        },
        hideOrShowCarInfo: function () {
            var temp = $("#carInfoDiv").is(":hidden");
            if (temp) {
                $("#carInfoDiv").show();
            } else {
                $("#carInfoDiv").hide();
            }
        },
        hideOrShowInsureInfo: function () {
            var temp = $("#insureInfoDiv").is(":hidden");
            if (temp) {
                $("#insureInfoDiv").show();
            } else {
                $("#insureInfoDiv").hide();
            }
        },
        updateProduct: function (obj) {
            this.insurerInfo.AllProductPrice.forEach(function (i) {
                if (i.ID == obj.target.value) {
                    if (obj.target.checked) {
                        $scope.insure.bean.SaveProductId.push({PRODUCT_ID: i.ID});
                    } else {
                        $scope.insure.bean.SaveProductId.remove({PRODUCT_ID: i.ID});
                    }
                    return;
                }
            });
        },
        save: function () {
            console.log(this.bean);
            this.bean.SaveProductId = [];
            for (var i = 0; i < $scope.insure.insurerInfo.AllProductPrice.length; i++) {
                var tmp = $scope.insure.insurerInfo.AllProductPrice[i];
                if(tmp.ID==4)
                {
                    $scope.insure.bean.SaveProductId[i] = {PRODUCT_ID: tmp.ID,MAX_PAY:$scope.insure.maxMoney};
                }
                else
                {
                    $scope.insure.bean.SaveProductId[i] = {PRODUCT_ID: tmp.ID};
                }


            }
            insureFac.OrderInsureSave(this.bean);

        },
        upImg: function (obj, OutFileId) {
            $scope.insure.currEnt = $(obj.target);
            fileUpFac.upImg($scope.insure.currEnt, OutFileId, $scope.insure.upCallback,$scope);
        },
        upCallback: function (result) {
            var name = $scope.insure.currEnt.attr("name");
            if (name == "idNoImg") {
                $scope.insure.bean.ID_NO_PIC_ID = result.ID;
                $scope.insure.userInfo.idNoUrl=result.URL;
            } else if (name == "DrivingPicImg") {
                $scope.insure.bean.DRIVING_PIC_ID = result.ID;
                $scope.insure.userInfo.DrivingPicUrl=result.URL;
            }else if(name == "driverPicImg"){
                $scope.insure.bean.DRIVER_PIC_ID =result.ID;
                $scope.insure.userInfo.driverPicUrl=result.URL;
            }
        }
    };
    var user = $stateParams.customer;
    if(user!=null) {
        $scope.insure.userInfo = user;
        $scope.insure.carInfo = user.NowCar;
        $scope.insure.carsOptions = [user.NowCar];
        $scope.insure.bean.CLIENT_ID = user.ID;
        $scope.insure.bean.CAR_USERNAME = user.NAME;
        $scope.insure.bean.ID_NO = user.ID_NO;
        $scope.insure.bean.CAR_OWNER = user.phone;
        $scope.insure.bean.ID_NO_PIC_ID = user.ID_NO_PIC_ID;
        $scope.insure.bean.DRIVER_PIC_ID = user.DRIVER_PIC_ID;
        if (user.NowCar != null) {
            $scope.insure.bean.DRIVING_PIC_ID = user.NowCar.DRIVING_PIC_ID;
        }
    }



    $scope.$on('fileUp.Update', function () {
        var currMsg = fileUpFac.getCurrentMes();


    });
    $scope.$on('Insure.OrderInsureSave', function () {
        var currMsg = insureFac.getCurrentMes();
        $ionicLoading.show({
            noBackdrop: true,
            template: "投保成功",
            duration: 2000
        });
    });

    $scope.$on('CarList.Update', function () {
        var currMsg = carFac.getCurrentMes();
        $scope.insure.carsOptions = currMsg.data;
    });
    $scope.$on('Insure.SearInsureByCarUpdate', function () {
        var currMsg = insureFac.getCurrentMes();
        $scope.insure.insurerOptions = currMsg;
    });

}])

/**
 * Created by wengzhilai on 2016/8/7.
 */
mainController.controller('editInsureFastCtr', ["$timeout", "$scope", "toPost", "storageUserFac", "common", "fileUpFac", "$ionicPopup", "$ionicLoading", "Storage", "carFac", "$cordovaCamera", "$cordovaImagePicker", "$stateParams", "fileUpFac", "insureFac", "$state", "$ionicActionSheet", function ($timeout, $scope, toPost, storageUserFac, common, fileUpFac, $ionicPopup, $ionicLoading, Storage, carFac, $cordovaCamera, $cordovaImagePicker, $stateParams, fileUpFac, insureFac, $state, $ionicActionSheet) {

  $scope.insure = {
    bean: {
      Car:{},
      Client: {},
      SaveProductId: [],
      AllInsurePrice: [],
      CLIENT_ID: 0,
      AllFiles: []
    },
    init: function () {
      console.log("传入的参数");
      console.log($stateParams);

      toPost.single("QueryInsure", {authToken: storageUserFac.getUserAuthToken()}, function (currMsg) {
        $scope.insure.bean.AllInsurePrice = currMsg;
        $scope.insure.bean.INSURER_ID = 1;
        for (var i = 0; i < $scope.insure.bean.AllInsurePrice.length; i++) {
          for (var a = 0; a < $scope.insure.bean.AllInsurePrice[i].AllProductPrice.length; a++) {
            $scope.insure.bean.AllInsurePrice[i].AllProductPrice[a].isCheck = ($scope.insure.bean.AllInsurePrice[i].AllProductPrice[a].IS_MUST == 1)
          }
        }

      })

      if ($stateParams.id && $stateParams.id != '') {
        toPost.single("OrderInsureSingle", $stateParams.id, function (currMsg) {
          $scope.insure.bean = currMsg;
        })
      }

      var user = $stateParams.customer;
      console.log('user');
      console.log(user);
      var nowUser = storageUserFac.getUser();
      var isStaff = false
      if(user!=null) {
        for (var i = 0; i < user.RoleAllID.length; i++) {
          if (user.RoleAllID[i] == 3) {
            isStaff = true;
          }
        }
      }

      console.log('nowUser');
      console.log($stateParams.customer != null || !isStaff);

      if ($stateParams.customer != null || !isStaff) {
        console.log('有客户');

        if (user == null) {
          user = nowUser;
        }
        $scope.insure.bean.Client = user;
        $scope.insure.bean.Car = user.AllCar[0];
        $scope.insure.bean.CLIENT_ID = user.ID;
        $scope.insure.bean.CAR_USERNAME=$scope.insure.bean.Client.NAME
      }
    },
    maxMoney: 300000,
    userInfo: '',
    carInfo: '',
    carsOptions: [],
    purchaseWayArr: ['续保', '三责20万', '三责50万', '三责100万'],
    insurerInfo: {},
    currEnt: null,
    changepurchaseWay: function () {
      var purchaseWay = $scope.insure.bean.PURCHASE_WAY;
      for (var i = 0; i < $scope.insure.bean.AllInsurePrice.length; i++) {
        switch (purchaseWay) {
          case "三责20万":
            $scope.insure.bean.AllInsurePrice[i].AllProductPrice[3].maxPay = "20万";
            $scope.insure.bean.AllInsurePrice[i].AllProductPrice[4].maxPay = "10万";
            $scope.insure.bean.AllInsurePrice[i].AllProductPrice[5].maxPay = "10万";
            $scope.insure.bean.AllInsurePrice[i].AllProductPrice[8].maxPay = "国产";
            // $scope.insure.bean.AllInsurePrice[i].AllProductPrice[10].maxPay = "0.2万";
            break;
          case "三责50万":
            $scope.insure.bean.AllInsurePrice[i].AllProductPrice[3].maxPay = "50万";
            $scope.insure.bean.AllInsurePrice[i].AllProductPrice[4].maxPay = "2万";
            $scope.insure.bean.AllInsurePrice[i].AllProductPrice[5].maxPay = "2万";
            $scope.insure.bean.AllInsurePrice[i].AllProductPrice[8].maxPay = "国产";
            // $scope.insure.bean.AllInsurePrice[i].AllProductPrice[10].maxPay = "0.2万";
            break;
          case "三责100万":
            $scope.insure.bean.AllInsurePrice[i].AllProductPrice[3].maxPay = "100万";
            $scope.insure.bean.AllInsurePrice[i].AllProductPrice[4].maxPay = "5万";
            $scope.insure.bean.AllInsurePrice[i].AllProductPrice[5].maxPay = "5万";
            $scope.insure.bean.AllInsurePrice[i].AllProductPrice[8].maxPay = "进口";
            // $scope.insure.bean.AllInsurePrice[i].AllProductPrice[10].maxPay = "0.5万";
            break;
        }

        for (var a = 0; a < $scope.insure.bean.AllInsurePrice[i].AllProductPrice.length; a++) {
          $scope.insure.bean.AllInsurePrice[i].AllProductPrice[a].isCheck = ($scope.insure.bean.AllInsurePrice[i].AllProductPrice[a].IS_MUST == 1)
        }
      }
    },
    showBigImage: function (ent) {
      var url = $(ent.target).attr("src");
      if (url == null || url == '') {
        //选择上传图片
        this.upImg(ent);
        return;
      }
      fileUpFac.FullScreenImage(url, $scope);
    },
    save: function () {
      console.log(this.bean);
      $scope.insure.bean.SaveProductId = [];
      for (var i = 0; i < $scope.insure.bean.AllInsurePrice.length; i++) {
        if ($scope.insure.bean.AllInsurePrice[i].ID == $scope.insure.bean.INSURER_ID) {
          for (var a = 0; a < $scope.insure.bean.AllInsurePrice[i].AllProductPrice.length; a++) {
            if ($scope.insure.bean.AllInsurePrice[i].AllProductPrice[a].isCheck) {
              $scope.insure.bean.SaveProductId[$scope.insure.bean.SaveProductId.length] = {
                PRODUCT_ID: $scope.insure.bean.AllInsurePrice[i].AllProductPrice[a].ID,
                MAX_PAY: $scope.insure.bean.AllInsurePrice[i].AllProductPrice[a].maxPay
              };

              for (var a1 = 0; a1 < this.bean.AllInsurePrice[i].AllProductPrice[a].ChildItem.length; a1++) {
                if (this.bean.AllInsurePrice[i].AllProductPrice[a].ChildItem[a1].isCheck) {
                  this.bean.SaveProductId[this.bean.SaveProductId.length] = {
                    PRODUCT_ID: this.bean.AllInsurePrice[i].AllProductPrice[a].ChildItem[a1].ID,
                    MAX_PAY: this.bean.AllInsurePrice[i].AllProductPrice[a].ChildItem[a1].maxPay
                  };
                }
              }
            }
          }
        }
      }

      toPost.saveOrUpdate("OrderInsureSave", this.bean, function (currMsg) {
        if (currMsg.IsError) {
          common.hint(currMsg.Message);
        } else {
          common.hint("资料已上传，等待业务员与您联系");
          $state.go('insureList', {orderType: '保单', reload: true});  //路由跳转
        }
      })

    },
    AddImg: function () {
      var indexNo = $scope.insure.bean.AllFiles.length;
      $scope.insure.bean.AllFiles[indexNo] = {"indexNo": $scope.insure.bean.AllFiles.length};

      var jqObj=$("img[name='allFile_"+indexNo+"']");
      var timer = $timeout(
        function() {
          var jqObj=$("img[name='allFile_"+indexNo+"']");
          $scope.insure.currEnt = $(jqObj[0]);
          fileUpFac.upImg($scope.insure.currEnt, $scope.insure.bean.AllFiles[indexNo].ID, $scope.insure.upImgByNameCallBack,$scope);
        },
        100
      )
    },
    upImgByName: function (obj, OutFileId) {
      $scope.insure.currEnt = $(obj.target);
      fileUpFac.upImg($scope.insure.currEnt, OutFileId, $scope.insure.upImgByNameCallBack, $scope);
    },
    upImgByNameCallBack:function (result) {
      var indexNo = $scope.insure.currEnt.attr("data-indexNo");
      $scope.insure.bean.AllFiles[indexNo].ID = result.ID;
      $scope.insure.bean.AllFiles[indexNo].URL = result.URL;
    },
    upImg: function (obj, OutFileId) {
      $scope.insure.currEnt = $(obj.target);
      fileUpFac.upImg($scope.insure.currEnt, OutFileId, $scope.insure.upCallback, $scope);
    },
    upCallback: function (result) {
      console.log($scope.insure.bean);
      var name = $scope.insure.currEnt.attr("name");
      switch (name) {
        case "idNoImg":
          $scope.insure.bean.Car.ID_NO_PIC_ID = result.ID;
          $scope.insure.bean.Car.idNoUrl = result.URL;
          break;
        case "idNoImg1":
          $scope.insure.bean.Car.ID_NO_PIC_ID1 = result.ID;
          $scope.insure.bean.Car.idNoUrl1 = result.URL;
          break;
        case "drivingPicImg":
          $scope.insure.bean.Car.DRIVING_PIC_ID = result.ID;
          $scope.insure.bean.Car.DrivingPicUrl = result.URL;
          break;
        case "drivingPicImg1":
          $scope.insure.bean.Car.DRIVING_PIC_ID1 = result.ID;
          $scope.insure.bean.Car.DrivingPicUrl1 = result.URL;
          break;
        case "driverPicImg":
          $scope.insure.bean.DRIVER_PIC_ID = result.ID;
          $scope.insure.bean.driverPicUrl = result.URL;
          break;
        case "driverPicImg1":
          $scope.insure.bean.DRIVER_PIC_ID1 = result.ID;
          $scope.insure.bean.driverPicUrl1 = result.URL;
          break;
        case "recognizeePicImg":
          $scope.insure.bean.RECOGNIZEE_PIC_ID = result.ID;
          $scope.insure.bean.recognizeePicUrl = result.URL;
          break;
        case "recognizeePicImg1":
          $scope.insure.bean.RECOGNIZEE_PIC_ID1 = result.ID;
          $scope.insure.bean.recognizeePicUrl1 = result.URL;
          break;
        case "billUrl":
          $scope.insure.bean.Car.BILL_PIC_ID = result.ID;
          $scope.insure.bean.Car.billUrl = result.URL;
          break;
        case "certificatePicUrl":
          $scope.insure.bean.Car.CERTIFICATE_PIC_ID = result.ID;
          $scope.insure.bean.Car.certificatePicUrl = result.URL;
          break;
      }
    }
  };

}])

/**
 * Created by wengzhilai on 2016/11/14.
 */
mainController.controller('expireInsureCtr', ["common", "$scope", "toPost", "fileUpFac", "$ionicPopup", "$ionicLoading", "Storage", "$cordovaCamera", "$cordovaImagePicker", "$stateParams", "fileUpFac", "insureFac", "$state", "$ionicActionSheet", function (common,$scope,toPost, fileUpFac,$ionicPopup, $ionicLoading, Storage,$cordovaCamera,$cordovaImagePicker, $stateParams,fileUpFac, insureFac, $state, $ionicActionSheet) {
  console.log($stateParams)
  if($stateParams.expireInsureList==null)
  {
    common.hint('参数有误');
  }
  else {
    $scope.lists=$stateParams.expireInsureList;
  }
}]);

/**
 * Created by wengzhilai on 2016/10/13.
 */
mainController.controller('followInsureCtr', ["$scope", "common", "orderPay", "$cordovaFileTransfer", "toPost", "fileUpFac", "$ionicModal", "$stateParams", "$state", "$ionicLoading", "storageUserFac", function ($scope, common, orderPay, $cordovaFileTransfer, toPost, fileUpFac, $ionicModal, $stateParams, $state, $ionicLoading, storageUserFac) {
  $scope.callPhone = function (mobilePhone) {
    common.callPhone(mobilePhone);
  };
  $scope.user = storageUserFac.getUser();
  $ionicModal.fromTemplateUrl('templates/FlowAdd.html', {
    scope: $scope
  }).then(function (modal) {
    $scope.modal = modal;
  });

  $scope.Order = {
    Url: "",
    vitalCheck:false,
    bean: {
      VITAL: 1
    },
    pushNotificationChange :function() {
      this.bean.VITAL=(this.vitalCheck)?1:0;
      var postBean = {
        userId: 0,
        authToken: storageUserFac.getUserAuthToken(),
        para:[{K:'VITAL',V:this.bean.VITAL}],
        id: this.bean.ID
      };
      toPost.Post("OrderSaveVital",postBean,function (currMsg) {
        $ionicLoading.show({
          noBackdrop: true,
          template: currMsg.Message,
          duration: 2000
        });
      })
      console.log(this.bean.VITAL)
    },
    showBigImage: function (ent) {
      var url = ent.target.getAttribute("src");
      if (url == null || url == '') {
        return;
      }
      fileUpFac.FullScreenImage(url, $scope);
    },
    Pay: function () {
      orderPay.pay($scope.Order.bean.ID, function (ent) {
        common.hint(ent.Message);
      })
    },
    editHelp: function () {
      $state.go('editOrder', {
        id: $stateParams.id
      });
    },
    showHelpSingle: function () {
      var temp = $("#helpSingleDiv").is(":hidden");
      if (temp) {
        $("#helpSingleDiv").show();
      } else {
        $("#helpSingleDiv").hide();
      }

    },
    save: function () {
      var postBean = {
        userId: 0,
        authToken: storageUserFac.getUserAuthToken(),
        saveKeys: 'STATUS',
        entity: {
          STATUS: '完成'
        }
      };
      toPost.Post("RescueSave", postBean, $scope.saveCallBack);
    }
  };
  /*添加流程*/
  $scope.bean = {
    orderId: $stateParams.id,
    cost: 0,
    body: "",
    fileIdStr: "",
    stauts: $scope.Order.bean.NextButton,
    AllFiles: []
  };
  /*修改流程*/
  $scope.ShowAddFlow = function () {
    $scope.bean = {
      orderId: $stateParams.id,
      cost: 0,
      body: "",
      fileIdStr: "",
      stauts: $scope.Order.bean.NextButton,
      AllFiles: []
    };
    $scope.modal.show();
  };
  $scope.func = {
    upImg: function (obj) {
      $scope.Order.currEnt =  $(obj.target);
      var indexNo = $scope.Order.currEnt.attr("data-indexNo");
      fileUpFac.upImg($scope.Order.currEnt, $scope.bean.AllFiles[indexNo].ID, $scope.func.upCallback,$scope);
    },
    upCallback: function (result) {
      var indexNo = $scope.Order.currEnt.attr("data-indexNo");
      $scope.bean.AllFiles[indexNo].ID = result.ID;
      $scope.bean.AllFiles[indexNo].URL = result.URL;
    },
    AddImg: function () {
      $scope.bean.AllFiles[$scope.bean.AllFiles.length] = {"indexNo": $scope.bean.AllFiles.length};
    },
    createFlow: function (nowstatu) {
      for (var i = 0; i < $scope.bean.AllFiles.length; i++) {
        if ($scope.bean.AllFiles[i].ID != null) {
          $scope.bean.fileIdStr += "," + $scope.bean.AllFiles[i].ID;
        }
      }
      var postBean = {
        authToken: storageUserFac.getUserAuthToken(),
        para: [
          {K: "orderId", V: $scope.bean.orderId},
          {K: "taskID", V: $scope.Order.bean.TaskId},
          {K: "taskFlowId", V: $scope.Order.bean.TaskFlowId},
          {K: "cost", V: $scope.bean.cost},
          {K: "body", V: $scope.bean.body},
          {K: "fileIdStr", V: $scope.bean.fileIdStr},
          {K: "stauts", V:nowstatu}
        ]
      };
      toPost.Post("OrderSaveStatus", postBean, $scope.func.createFlowCallBack);
    },
    createFlowCallBack: function (currMsg) {
      if (!currMsg.IsError) {
        $ionicLoading.show({
          noBackdrop: true,
          template: "保存成功",
          duration: 2000
        });
      }
      $scope.modal.hide();
      toPost.single("OrderInsureSingle", $stateParams.id, $scope.SingleCallBack)
    }
  }
  $scope.saveCallBack = function (currMsg) {
    if (currMsg.IsError) {
      $ionicLoading.show({
        noBackdrop: true,
        template: currMsg.Message,
        duration: 2000
      });
    } else {
      $ionicLoading.show({
        noBackdrop: true,
        template: "救援已经确认",
        duration: 2000
      });
    }
  }

  $scope.SingleCallBack = function (currMsg) {
    if (currMsg.IsError) {
      $ionicLoading.show({
        noBackdrop: true,
        template: currMsg.Message,
        duration: 2000
      });
    } else {
      $scope.Order.bean = currMsg;
      $scope.Order.vitalCheck=(currMsg.VITAL==1)
      console.log($scope.Order.vitalCheck)
      $scope.bean.stauts = currMsg.NextButton
    }
  }

  if ($stateParams.id) {
    toPost.single("OrderInsureSingle", $stateParams.id, $scope.SingleCallBack)
  } else {
    common.hint("无订单号无法跟踪");
  }
}])

/**
 * Created by wengzhilai on 2016/8/12.
 */
mainController.controller('insureListCtr', ["$scope", "toPost", "$ionicLoading", "$state", "$stateParams", "$ionicPopup", function ($scope, toPost, $ionicLoading, $state, $stateParams, $ionicPopup) {
  console.log($stateParams);
  $scope.orderList = {
    orderType:'救援',
    bean: {
      userId: 0,
      authToken: '',
      pageSize: 0,
      id: 0,
      currentPage: 0,
      searchKey: [],
      orderBy: []
    },
    addOrder:function () {
      $state.go("editInsureFast",{})
    },
    toFollowOrder: function (id, type) {
      if (type == "救援") {
        $state.go('followHelpOrder', {id: id, reload: true});  //路由跳转
      } else if (type == "审车") {
        $state.go('followTrialOrder', {id: id, reload: true});  //路由跳转
      } else if (type == "维修" || type == "保养") {
        $state.go('followHelpOrder', {id: id, reload: true});  //路由跳转
//                $state.go('followMaintainOrder', {id: id, reload: true});  //路由跳转
      } else if (type == "投保") {
        $state.go('followInsure', {id: id, reload: true});  //路由跳转
      }
    },
    lists: {},
    init: function () {
      console.log('参数');
      console.log($stateParams);
      $scope.title=$stateParams.title;

      if($stateParams.orderType!=null) {
        $scope.orderList.orderType = $stateParams.orderType;
        var htmlObj = $("a[data-value='" + $stateParams.orderType + "']");
        htmlObj.attr("class", "button button-balanced");
      }
      this.doRefresh();
    },
    hasNextPage: function () {
      if (this.lists.totalPage == null || this.lists.totalPage == 0 || this.lists.totalPage <= this.lists.currentPage) {
        return false;
      }
      return true;
    },
    doRefresh: function () {
      console.log("下拉刷新");
      this.bean.currentPage = 1;
      this.bean.searchKey = [{K: "ORDER_TYPE", V: "投保", T: '=='}];
      if($stateParams.clientId!=null){
        this.bean.searchKey[this.bean.searchKey.length]={K:"CLIENT_ID",T:'==',V:$stateParams.clientId};
      }
      toPost.list("OrderList", this.bean, this.callListReturn);
    },
    callListReturn: function (currMsg) {
      if (currMsg.IsError == true) {
        $ionicLoading.show({
          noBackdrop: true,
          template: currMsg.Message,
          duration: 2000
        });
      } else {
        $scope.orderList.lists = currMsg;
      }
      $scope.$broadcast('scroll.refreshComplete');
      $scope.$broadcast('scroll.infiniteScrollComplete');
    },
    loadMore: function () {
      if (this.bean.currentPage == 0) {
        this.bean.currentPage = 1;
      } else {
        this.bean.currentPage++;
      }
      console.log("加载更多");
      toPost.list("OrderList", this.bean, this.callListReturn);
    },
    filterKey: function (obj) {
      if ($("a[class='button button-balanced']").length != 0) {
        $("a[class='button button-balanced']").each(function () {
          $(this).attr("class", "button");
        });
      }
      obj.target.setAttribute("class", "button button-balanced");
      this.doRefresh();
    },
    showOrderPopup: function (obj) {
      // 自定义弹窗
      var myPopup = $ionicPopup.show({
        template: '<div  class="item item-input item-select"><div class="input-label">排序字段</div><select id="orderF"><option value="">--请选择--</option><option value="CREATE_TIME">下单日期</option><option value="COST">订单金额</option><option value="ORDER_NO">订单号</option></select></div>' +
        '<div class="list"><div class="item">升序<input type="radio" name="order" data-text="升序" value="asc" checked="checked"/></div>' +
        '<div class="item">降序<input type="radio" data-text="降序" name="order" value="desc"/> </div>',
        title: '选择排序方式',
        scope: $scope,
        buttons: [
          {
            text: '<b>清除</b>',
            type: 'button-positive',
            onTap: function () {
              myPopup.close();
              obj.target.innerHTML = '排序';
              $scope.orderList.bean.orderBy = [];
            }
          },
          {
            text: '<b>确定</b>',
            type: 'button-positive',
            onTap: function (e) {
              //$("input[name='order']:checked").attr("date-text")
              var order = $('input[name="order"]:checked');
              var orderF = $('#orderF option:selected');

              if (orderF.val() != '') {
                $scope.orderList.bean.orderBy = [
                  {K: orderF.val(), V: order.val(), T: ''}
                ];

                obj.target.innerHTML = orderF.text() + ' ' + order.attr("data-text");
              } else {

                obj.target.innerHTML = '排序';
              }
            }
          }
        ]
      });
    }
  };
}])

/**
 * Created by Administrator on 2016/8/8.
 */
mainController.controller('editHelpOrderCtr', ["$scope", "$timeout", "fileUpDel", "baiduMap", "common", "$ionicLoading", "$stateParams", "$state", "$cordovaCamera", "$ionicPopup", "fileUpFac", "$cordovaImagePicker", "toPost", "storageUserFac", "$ionicActionSheet", "$cordovaGeolocation", function ($scope,$timeout, fileUpDel, baiduMap, common, $ionicLoading, $stateParams, $state, $cordovaCamera, $ionicPopup, fileUpFac, $cordovaImagePicker, toPost, storageUserFac, $ionicActionSheet, $cordovaGeolocation) {
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
}]);

/**
 * Created by wengzhilai on 2016/10/13.
 */
mainController.controller('followHelpOrderCtr', ["$scope", "common", "$cordovaFileTransfer", "orderPay", "toPost", "fileUpFac", "$ionicModal", "$stateParams", "$state", "$ionicLoading", "storageUserFac", function ($scope, common, $cordovaFileTransfer,orderPay , toPost, fileUpFac, $ionicModal, $stateParams, $state, $ionicLoading, storageUserFac) {
  $scope.callPhone = function (mobilePhone) {
    common.callPhone(mobilePhone);
  };
  $scope.user = storageUserFac.getUser();
  $ionicModal.fromTemplateUrl('templates/FlowAdd.html', {
    scope: $scope,
    animation: 'slide-in-up'
  }).then(function (modal) {
    $scope.modal = modal;
  });

  $scope.Order = {
    bean: {},
    showBigImage: function (ent) {
      var url = ent.target.getAttribute("src");
      if (url == null || url == '') {
        return;
      }
      fileUpFac.FullScreenImage(url,$scope);
    },
    Pay: function () {
      orderPay.pay($scope.Order.bean.ID, function (ent) {
        if (ent.IsError) {
          common.hint(ent.Message);
        }
        else {
          common.hint(ent.Message);
        }
      })
    },
    editHelp: function () {
      $state.go('editHelpOrder', {
        id: $stateParams.id
      });
    },
    goOrderList: function () {
      $state.go('orderList', {
        orderType: $scope.Order.bean.ORDER_TYPE,
        reload: true
      });
    },
    showHelpSingle: function () {
      var temp = $("#helpSingleDiv").is(":hidden");
      if (temp) {
        $("#helpSingleDiv").show();
      } else {
        $("#helpSingleDiv").hide();
      }

    },
    save: function () {
      var postBean = {
        userId: 0,
        authToken: storageUserFac.getUserAuthToken(),
        saveKeys: 'STATUS',
        entity: {
          STATUS: '完成'
        }
      };
      toPost.Post("RescueSave", postBean, $scope.saveCallBack);
    }
  };
  /*添加流程*/
  $scope.bean = {
    orderId: $stateParams.id,
    cost: 0,
    body: "",
    fileIdStr: "",
    stauts: $scope.Order.bean.NextButton,
    AllFiles: []
  };
  /*修改流程*/
  $scope.ShowAddFlow = function () {

    if ($scope.Order.bean.GARAGE_ID == null) {
      toPost.single("OrderGrab", $scope.Order.bean.ID, function (ent) {
        common.hint("抢单成功");
        $state.go('orderRuningList');
      });
      return;
    }
    $scope.bean = {
      orderId: $stateParams.id,
      cost: 0,
      body: "",
      body1:'',
      fileIdStr: "",
      stauts: $scope.Order.bean.NextButton,
      AllFiles: []
    };
    $scope.modal.show();
  };
  $scope.func = {
    upImg: function (obj) {
      $scope.Order.currEnt = $(obj.target);
      var indexNo = $scope.Order.currEnt.attr("data-indexNo");
      fileUpFac.upImg($scope.Order.currEnt, $scope.bean.AllFiles[indexNo].ID, $scope.func.upCallback,$scope);
    },
    upCallback: function (result) {
      var indexNo = $scope.Order.currEnt.attr("data-indexNo");
      $scope.bean.AllFiles[indexNo].ID = result.ID;
      $scope.bean.AllFiles[indexNo].URL = result.URL;
    },
    AddImg: function () {
      $scope.bean.AllFiles[$scope.bean.AllFiles.length] = {"indexNo": $scope.bean.AllFiles.length};
    },
    createFlow: function (nowbean,status) {
      if(status!=null)
      {
        $scope.Order.bean.NextButton=status
      }
      for (var i = 0; i < $scope.bean.AllFiles.length; i++) {
        if ($scope.bean.AllFiles[i].ID != null) {
          $scope.bean.fileIdStr += "," + $scope.bean.AllFiles[i].ID;
        }
      }
      if($scope.bean.body1!=null && $scope.bean.body1!='')
      {
        $scope.bean.body+="预计到达时间："+$scope.bean.body1;
      }
      var postBean = {
        authToken: storageUserFac.getUserAuthToken(),
        para: [
          {K: "orderId", V: $scope.bean.orderId},
          {K: "cost", V: $scope.bean.cost},
          {K: "body", V: $scope.bean.body},
          {K: "fileIdStr", V: $scope.bean.fileIdStr},
          {K: "stauts", V: $scope.Order.bean.NextButton}
        ]
      };

      toPost.Post("OrderSaveStatus", postBean, function (currMsg) {
        if (!currMsg.IsError) {
          $ionicLoading.show({
            noBackdrop: true,
            template: "保存成功",
            duration: 2000
          });
        }
        $scope.modal.hide();
        toPost.single("RescueSingle", $stateParams.id, $scope.SingleCallBack)
      });
    }
  }
  $scope.saveCallBack = function (currMsg) {
    if (currMsg.IsError) {
      $ionicLoading.show({
        noBackdrop: true,
        template: currMsg.Message,
        duration: 2000
      });
    } else {
      $ionicLoading.show({
        noBackdrop: true,
        template: "救援已经确认",
        duration: 2000
      });
    }
  }

  $scope.SingleCallBack = function (currMsg) {
    if (currMsg.IsError) {
      $ionicLoading.show({
        noBackdrop: true,
        template: currMsg.Message,
        duration: 2000
      });
    } else {
      $scope.Order.bean = currMsg;
      $scope.bean.stauts = currMsg.NextButton
    }
  }

  if ($stateParams.id) {
    toPost.single("RescueSingle", $stateParams.id, $scope.SingleCallBack)
  } else {
    common.hint("无订单号无法跟踪");
  }
}])

/**
 * Created by wengzhilai on 2016/8/12.
 */
mainController.controller('followMaintainOrderCtr', ["common", "$scope", "orderFac", "$ionicLoading", "$stateParams", "$state", "$ionicActionSheet", function (common,$scope, orderFac, $ionicLoading, $stateParams, $state, $ionicActionSheet) {
    if ($stateParams.id) {
        orderFac.rescueSingle($stateParams.id);
    } else {
      common.hint("无订单号无法跟踪");
    }
    $scope.followOrder = {
        bean: {},
        showHelpSingle: function () {
            var temp = $("#singleDiv").is(":hidden");

            if (temp) {
                $("#singleDiv").show();
            } else {
                $("#singleDiv").hide();
            }

        }
    };
    $scope.$on('order.rescueSingleUpdate', function () {
        var currMsg = orderFac.getCurrentMes();
        if (currMsg.IsError) {
            $ionicLoading.show({
                noBackdrop: true,
                template: currMsg.Message,
                duration: 2000
            });
        } else {
            $scope.followOrder.bean = currMsg;
        }

    });

}])

/**
 * Created by wengzhilai on 2016/8/13.
 */
mainController.controller('followTrialOrderCtr', ["$scope", "orderFac", "$ionicLoading", "$stateParams", "$state", "$ionicActionSheet", function ($scope, orderFac, $ionicLoading, $stateParams, $state, $ionicActionSheet) {
    if ($stateParams.id) {
        orderFac.OrderSingle($stateParams.id);
    } else {
      common.hint("无订单号无法跟踪");
    }
    $scope.followOrder = {
        bean: {},
        showTrialSingle: function () {
            var temp = $("#trialSingleDiv").is(":hidden");

            if (temp) {
                $("#trialSingleDiv").show();
            } else {
                $("#trialSingleDiv").hide();
            }

        }
    };
    $scope.$on('order.SingleUpdate', function () {
        var currMsg = orderFac.getCurrentMes();

        $scope.followOrder.bean = currMsg;

    });

}])

/**
 * Created by wengzhilai on 2016/10/13.
 */

mainController.controller('orderGrabListCtr', ["$scope", "toPost", "$ionicLoading", "$state", "$stateParams", "$ionicPopup", function ($scope, toPost, $ionicLoading, $state, $stateParams, $ionicPopup) {
    $scope.orderList = {
        bean: {
            userId: 0,
            authToken: '',
            pageSize: 0,
            id: 0,
            currentPage: 0,
            searchKey: [],
            orderBy: []
        },
        lists: {},
        hasNextPage: function () {
            if (this.lists.totalPage == null || this.lists.totalPage == 0) {
                return false;
            }
            else if (this.lists.totalPage <= this.lists.currentPage) {
                return false;
            }
            return true;
        },
        doRefresh: function () {
            console.log("下拉刷新");
            this.bean.currentPage = 1;
            toPost.list("OrderGrabList", this.bean, this.callListReturn);
        },
        callListReturn:function(currMsg){
            if (currMsg.IsError == true) {
                $ionicLoading.show({
                    noBackdrop: true,
                    template: currMsg.Message,
                    duration: 2000
                });
            } else {
                $scope.orderList.lists = currMsg;
            }
            $scope.$broadcast('scroll.refreshComplete');
            $scope.$broadcast('scroll.infiniteScrollComplete');
        },
        loadMore: function () {
            if (this.bean.currentPage == 0) {
                this.bean.currentPage = 1;
            } else {
                this.bean.currentPage++;
            }
            console.log("加载更多");
            toPost.list("OrderGrabList", this.bean, this.callListReturn);
        }
    };
}])
/**
 * Created by wengzhilai on 2016/8/12.
 */
mainController.controller('orderListCtr', ["$scope", "toPost", "$ionicLoading", "$state", "$stateParams", "$ionicPopup", function ($scope, toPost, $ionicLoading, $state, $stateParams, $ionicPopup) {
  console.log($stateParams);
  $scope.orderList = {
    orderType:'救援',
    bean: {
      userId: 0,
      authToken: '',
      pageSize: 0,
      id: 0,
      currentPage: 0,
      searchKey: [],
      orderBy: []
    },
    addOrder:function () {
      var buB = $("a[class='button button-balanced']").text();
      switch (buB) {
        case "救援":
          $state.go("editHelpOrder",{})
          break;
        case "审车":
          $state.go("editHelpOrder",{})
          break;
        case "维保":
          $state.go("maintainMap",{})
          break;
        default:
          $state.go("editHelpOrder",{})
          break;
      }
    },
    toFollowOrder: function (id, type) {
      if (type == "救援") {
        $state.go('followHelpOrder', {id: id, reload: true});  //路由跳转
      } else if (type == "审车") {
        $state.go('followTrialOrder', {id: id, reload: true});  //路由跳转
      } else if (type == "维修" || type == "保养") {
        $state.go('followHelpOrder', {id: id, reload: true});  //路由跳转
//                $state.go('followMaintainOrder', {id: id, reload: true});  //路由跳转
      } else if (type == "投保") {
        $state.go('followInsure', {id: id, reload: true});  //路由跳转
      }
    },
    lists: {},
    init: function () {
      console.log('参数');
      console.log($stateParams);
      $scope.title=$stateParams.title;

      if($stateParams.orderType!=null) {
        $scope.orderList.orderType = $stateParams.orderType;
      }
      else {
        $scope.orderList.orderType='救援';
      }
      var htmlObj = $("a[data-value='" + $scope.orderList.orderType + "']");
      htmlObj.attr("class", "button button-balanced");
      this.doRefresh();
    },
    hasNextPage: function () {
      if (this.lists.totalPage == null || this.lists.totalPage == 0 || this.lists.totalPage <= this.lists.currentPage) {
        return false;
      }
      return true;
    },
    doRefresh: function () {
      console.log("下拉刷新");
      this.bean.currentPage = 1;
      var buB = $("a[class='button button-balanced']").text();
      buB=$scope.orderList.orderType;
      switch (buB) {
        case "救援":
          this.bean.searchKey = [{K: "ORDER_TYPE", V: buB, T: '=='}];
          break;
        case "审车":
          this.bean.searchKey = [{K: "ORDER_TYPE", V: buB, T: '=='}];
          break;
        case "维保":
          this.bean.searchKey = [{K: "ORDER_TYPE", V: buB, T: '=='}];
          break;
      }
      if($stateParams.clientId!=null){
        this.bean.searchKey[this.bean.searchKey.length]={K:"CLIENT_ID",T:'==',V:$stateParams.clientId};
      }
      toPost.list("OrderList", this.bean, this.callListReturn);
    },
    callListReturn: function (currMsg) {
      if (currMsg.IsError == true) {
        $ionicLoading.show({
          noBackdrop: true,
          template: currMsg.Message,
          duration: 2000
        });
      } else {
        $scope.orderList.lists = currMsg;
      }
      $scope.$broadcast('scroll.refreshComplete');
      $scope.$broadcast('scroll.infiniteScrollComplete');
    },
    loadMore: function () {
      if (this.bean.currentPage == 0) {
        this.bean.currentPage = 1;
      } else {
        this.bean.currentPage++;
      }
      console.log("加载更多");
      toPost.list("OrderList", this.bean, this.callListReturn);
    },
    filterKey: function (obj) {
      if ($("a[class='button button-balanced']").length != 0) {
        $("a[class='button button-balanced']").each(function () {
          $(this).attr("class", "button");
        });
      }
      obj.target.setAttribute("class", "button button-balanced");
      $scope.orderList.orderType = $(obj.target).text();
      this.doRefresh();
    },
    showOrderPopup: function (obj) {
      // 自定义弹窗
      var myPopup = $ionicPopup.show({
        template: '<div  class="item item-input item-select"><div class="input-label">排序字段</div><select id="orderF"><option value="">--请选择--</option><option value="CREATE_TIME">下单日期</option><option value="COST">订单金额</option><option value="ORDER_NO">订单号</option></select></div>' +
        '<div class="list"><div class="item">升序<input type="radio" name="order" data-text="升序" value="asc" checked="checked"/></div>' +
        '<div class="item">降序<input type="radio" data-text="降序" name="order" value="desc"/> </div>',
        title: '选择排序方式',
        scope: $scope,
        buttons: [
          {
            text: '<b>清除</b>',
            type: 'button-positive',
            onTap: function () {
              myPopup.close();
              obj.target.innerHTML = '排序';
              $scope.orderList.bean.orderBy = [];
            }
          },
          {
            text: '<b>确定</b>',
            type: 'button-positive',
            onTap: function (e) {
              //$("input[name='order']:checked").attr("date-text")
              var order = $('input[name="order"]:checked');
              var orderF = $('#orderF option:selected');

              if (orderF.val() != '') {
                $scope.orderList.bean.orderBy = [
                  {K: orderF.val(), V: order.val(), T: ''}
                ];

                obj.target.innerHTML = orderF.text() + ' ' + order.attr("data-text");
              } else {

                obj.target.innerHTML = '排序';
              }
            }
          }
        ]
      });
    }
  };
}])

/**
 * Created by wengzhilai on 2016/10/13.
 */

mainController.controller('orderRuningListCtr', ["$scope", "toPost", "$ionicLoading", "$state", "$stateParams", "$ionicPopup", "storageUserFac", function ($scope, toPost, $ionicLoading, $state, $stateParams, $ionicPopup,storageUserFac) {
  $scope.orderList = {
    bean: {
      userId: 0,
      authToken: '',
      pageSize: 0,
      id: 0,
      currentPage: 0,
      searchKey: [
        {K: "YL_ORDER_RESCUE", V: "null", T: '!='},
        {K: "YL_ORDER_RESCUE.STATUS", V: "完成", T: '!='},
        {K: "YL_ORDER_RESCUE.GARAGE_ID", V: "null", T: '!='}
      ],
      orderBy: []
    },
    lists: {},
    toFollowOrder: function (id, type) {
      if (type == "救援") {
        $state.go('followHelpOrder', {id: id, reload: true});  //路由跳转
      } else if (type == "审车") {
        $state.go('followTrialOrder', {id: id, reload: true});  //路由跳转
      } else if (type == "维修" || type == "保养") {
        $state.go('followHelpOrder', {id: id, reload: true});  //路由跳转
      } else if (type == "投保") {
        $state.go('followInsure', {id: id, reload: true});  //路由跳转
      }
    },
    init: function () {
      var allData=storageUserFac.getUser().allOrder;
      console.log('参数');
      console.log(allData);
      $scope.orderList.lists.data =allData ;
    }
  };
}])

/**
 * Created by wengzhilai on 2016/10/12.
 */
mainController.controller('customerInfoCtr', ["common", "Storage", "toPost", "fileUpFac", "storageUserFac", "CarIn", "$scope", "$ionicLoading", "$state", "$stateParams", "$ionicPopup", "$window", function (common,Storage, toPost, fileUpFac, storageUserFac, CarIn, $scope, $ionicLoading, $state, $stateParams, $ionicPopup,$window) {
  $scope.callPhone = function (mobilePhone) {
    common.callPhone(mobilePhone);
  };
  $scope.customer = {
    bean: {
      ID: 0,
      NAME: '测试名',
      SEX: '',
      ADDRESS: '',
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
        ENGINE_NUMBER: ''
      },
      authToken: '',
      iconURL: '',
      idNoUrl: '',
      driverPicUrl: '',
      distictName: '',
      allCost: '',
      email: '',
      phone: ''
    },
    editCustomer: function () {
      //alert("修改功能开发中！")
      $state.go('editCustomer', {id: this.bean.ID, reload: true});
    },
    resetUserPas: function () {
      //  confirm 对话框
      var confirmPopup = $ionicPopup.confirm({
        title: '密码重置',
        template: '是否确认要重置密码?',
        okText: '确认',
        cancelText: '取消'
      });
      confirmPopup.then(function (res) {
        if (res) {
          var postBean = {
            userId: storageUserFac.getUserId(),
            authToken: storageUserFac.getUserAuthToken(),
            entity: $scope.customer.bean.ID
          };
          toPost.saveOrUpdate("SalesmanClientRestPwd", postBean, function (obj) {
          })
        } else {
          console.log('You are not sure');
        }
      });


    },
    editInsureFast: function () {
      if (this.bean.NowCar == null) {
        common.hint("用户未设置默认车辆不能投保");
      }
      else {
        $state.go('editInsureFast', {customer: this.bean, reload: true});  //路由跳转
      }
    },
    showBigImage: function (ent) {
      var url = ent.target.getAttribute("src");
      if (url == null || url == '') {
        return;
      }
      fileUpFac.FullScreenImage(url,$scope);
    }
  };
  if ($stateParams.customer) {
    $scope.customer.bean = $stateParams.customer;
  }
  else if ($stateParams.id) {
    $scope.customer.bean.ID = $stateParams.id;
    var postBean = {
      userId: 0,
      authToken: storageUserFac.getUserAuthToken(),
      id: $stateParams.id
    };
    toPost.Post("ClientSingle", postBean, function (currMsg) {
      if (currMsg.IsError == true) {
        $ionicLoading.show({
          noBackdrop: true,
          template: currMsg.Message,
          duration: 2000
        });
      } else {
        $scope.customer.bean = currMsg;
      }
    });
  }


}])

/**
 * Created by wengzhilai on 2016/9/30.
 */

mainController.controller('customerListCtr', ["Storage", "$ionicLoading", "toPost", "$ionicActionSheet", "$scope", "$ionicPopup", "$cordovaImagePicker", "$cordovaCamera", "fileUpFac", "$state", function (Storage, $ionicLoading,toPost, $ionicActionSheet, $scope, $ionicPopup, $cordovaImagePicker, $cordovaCamera, fileUpFac, $state) {
    $scope.customerList = {
        bean: {
            userId: 0,
            authToken: '',
            pageSize: 0,
            id: 0,
            currentPage: 0,
            searchKey: [],
            orderBy: []
        },
        lists: {},
        bigImage: false,
        bigSrc: '',
        showBigImage: function (ent) {
            var src =$(ent.target).attr("src");
            if (src == null || src == '') {
                //选择上传图片
                this.upImg(ent);
                return;
            }
            this.bigSrc = src;
            this.bigImage = true;                   //显示大图
        },
        hideBigImage: function () {
            this.bigImage = false;
        },
        toTel: function (obj) {
            window.plugins.CallNumber.callNumber(function (result) {
            }, function (result) {
            }, phone, true);
        },
        addCustomer: function () {
            $state.go('editCustomer', {reload: true});  //路由跳转
        },
        hasNextPage: function () {
            if (this.lists.totalPage == null || this.lists.totalPage == 0) {
                return false;
            }
            else if (this.lists.totalPage <= this.lists.currentPage) {
                return false;
            }
            return true;
        },
        doRefresh: function () {
            console.log("下拉刷新");
            this.bean.currentPage = 1;
            if ($('#searchKey').val() != '') {
                this.bean.searchKey = [
                    {K: 'searchName', V: $('#searchKey').val(), T: 'like'}
                ];
            }
            toPost.list("SalesmanClientList", this.bean, $scope.callListReturn);
        },
        loadMore: function () {
            if (this.bean.currentPage == 0) {
                this.bean.currentPage = 1;
            } else {
                this.bean.currentPage++;
            }
            console.log("加载更多");
            if ($('#searchKey').val() != '') {
                this.bean.searchKey = [
                    {K: 'searchName', V: $('#searchKey').val(), T: 'like'}
                ];
            }
            toPost.list("SalesmanClientList", this.bean, $scope.callListReturn);
        }

    };

    $scope.callListReturn = function (currMsg) {
        if (currMsg.IsError == true) {
            $ionicLoading.show({
                noBackdrop: true,
                template: currMsg.Message,
                duration: 2000
            });

        } else {
            $scope.customerList.lists = currMsg;
        }
        $scope.$broadcast('scroll.refreshComplete');
        $scope.$broadcast('scroll.infiniteScrollComplete');
    };

    /**
     $scope.startVib=function(){
            // 震动 1000ms
            $cordovaVibration.vibrate(1000);

        };
     **/


    $scope.showOrderPopup = function (obj) {
        // 自定义弹窗
        var myPopup = $ionicPopup.show({
            template: '<div  class="item item-input item-select"><div class="input-label">排序字段</div><select id="orderF"><option value="">--请选择--</option><option value="ID">ID</option></select></div>' +
                '<div class="list"><div class="item">升序<input type="radio" name="order" value="asc" checked="checked"/></div>' +
                '<div class="item">降序<input type="radio" name="order" value="desc"/> </div>',
            title: '选择排序方式',
            buttons: [
                {
                    text: '<b>取消</b>',
                    type: 'button-positive',
                    onTap: function () {
                        myPopup.close();
                    }
                },
                {
                    text: '<b>确定</b>',
                    type: 'button-positive',
                    onTap: function (e) {
                        var order = $('input[name="order"]:checked').val();
                        var orderF = $('#orderF option:selected').val();
                        if (orderF != '') {
                            $scope.customerList.bean.orderBy = [
                                {K: orderF, V: order, T: ''}
                            ];

                            obj.target.innerHTML = orderF + ' ' + order;
                        } else {

                            obj.target.innerHTML = '排序';
                        }
                    }
                }
            ]
        });
    };
    $scope.showUserPopup = function (obj) {
        // 自定义弹窗
        var myPopup = $ionicPopup.show({
            template: '<div  class="item item-input item-select"><div class="input-label">订单类型</div><select id="OrderType"><option value="" selected="selected">--请选择--</option><option value="救援">救援</option><option value="维护">维护</option><option value="保养">保养</option><option value="投保">投保</option><option value="审车">审车</option></select></div>',

            title: '选择订单类型',
            scope: $scope,
            buttons: [
                {
                    text: '<b>取消</b>',
                    type: 'button-positive',
                    onTap: function () {
                        myPopup.close();
                    }
                },
                {
                    text: '<b>确定</b>',
                    type: 'button-positive',
                    onTap: function (e) {
                        var OrderType = $('#OrderType option:selected').val();
                        if (OrderType != '') {
                            if ($('#searchKey').val() != '') {
                                $scope.bean.searchKey = [
                                    {K: 'searchName', V: $('#searchKey').val(), T: 'like'},
                                    {
                                        K: 'OrderType',
                                        V: OrderType,
                                        T: '=='
                                    }
                                ];
                            } else {
                                $scope.customerList.bean.searchKey = [
                                    {K: 'OrderType', V: OrderType, T: '=='}
                                ];
                            }

                            obj.target.innerHTML = OrderType;
                        } else {
                            obj.target.innerHTML = '全部客户';

                        }
                    }
                }
            ]
        });
    };
 }])

/**
 * Created by wengzhilai on 2016/10/12.
 */
mainController.controller('editCustomerCtr', ["$scope", "fileUpFac", "toPost", "$location", "$ionicLoading", "$state", "$stateParams", function ($scope, fileUpFac, toPost, $location, $ionicLoading, $state, $stateParams) {
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

}])

/**
 * Created by wengzhilai on 2016/12/12.
 */
mainController.controller('TaskHandleCtr', ["Storage", "$timeout", "toPost", "fileUpFac", "storageUserFac", "CarIn", "$scope", "$ionicLoading", "$state", "$stateParams", "$ionicPopup", function (Storage,$timeout, toPost, fileUpFac, storageUserFac, CarIn, $scope, $ionicLoading, $state, $stateParams, $ionicPopup) {
  $scope.init=function () {
    console.log($stateParams);
    if ($stateParams.taskId) $scope.taskFlow.bean.taskID = $stateParams.taskId;
    if ($stateParams.taskFlowId) $scope.taskFlow.bean.taskFlowId = $stateParams.taskFlowId;
    if ($stateParams.orderId) $scope.taskFlow.bean.orderId = $stateParams.orderId;
    if ($stateParams.butName) $scope.taskFlow.bean.stauts = $stateParams.butName;
    if ($stateParams.returnUrl) $scope.taskFlow.returnUrl = $stateParams.returnUrl;
  }

  $scope.taskFlow={
    returnUrl:'',
    currEnt:null,
    AllFiles:[],

    bean:{
      orderId:'',
      taskID:'',
      taskFlowId:'',
      cost:'',
      body:'',
      fileIdStr:'',
      stauts:'',
      PickTime:''
    },
    upImg: function (obj) {
      $scope.taskFlow.currEnt =$(obj.target);
      var indexNo =  $scope.taskFlow.currEnt.attr("data-indexNo");
      fileUpFac.upImg( $scope.taskFlow.currEnt, $scope.taskFlow.AllFiles[indexNo].ID, $scope.taskFlow.upCallback,$scope);
    },
    upCallback: function (result) {
      var indexNo = $scope.taskFlow.currEnt.attr("data-indexNo");
      $scope.taskFlow.AllFiles[indexNo].ID = result.ID;
      $scope.taskFlow.AllFiles[indexNo].URL = result.URL;
    },
    AddImg: function () {
      var indexNo =$scope.taskFlow.AllFiles.length;
      $scope.taskFlow.AllFiles[indexNo] = {"indexNo": $scope.taskFlow.AllFiles.length};

      var jqObj=$("img[data-indexNo='"+indexNo+"']");
      var timer = $timeout(
        function() {
          var jqObj=$("img[data-indexNo='"+indexNo+"']");
          $scope.taskFlow.currEnt = $(jqObj[0]);
          fileUpFac.upImg($scope.taskFlow.currEnt, $scope.taskFlow.AllFiles[indexNo].ID, $scope.taskFlow.upCallback,$scope);
        },
        100
      );
    },
    createFlow: function () {
      for (var i = 0; i < $scope.taskFlow.AllFiles.length; i++) {
        if ($scope.taskFlow.AllFiles[i].ID != null) {
          $scope.taskFlow.bean.fileIdStr += "," + $scope.taskFlow.AllFiles[i].ID;
        }
      }

      if($scope.taskFlow.bean.cost!=null && $scope.taskFlow.bean.cost!='')
      {
        $scope.taskFlow.bean.body+="费用："+$scope.taskFlow.bean.cost+'\r\n';
      }
      if($scope.taskFlow.bean.PickTime!=null && $scope.taskFlow.bean.PickTime!='')
      {
        $scope.taskFlow.bean.body+="接车时间："+$scope.taskFlow.bean.PickTime+'\r\n';
      }

      var postBean = {
        authToken: storageUserFac.getUserAuthToken(),
        para: [
          {K: "orderId", V: $scope.taskFlow.bean.orderId},
          {K: "taskID", V: $scope.taskFlow.bean.taskID},
          {K: "taskFlowId", V:  $scope.taskFlow.bean.taskFlowId},
          {K: "cost", V:  $scope.taskFlow.bean.cost},
          {K: "body", V:  $scope.taskFlow.bean.body},
          {K: "fileIdStr", V:  $scope.taskFlow.bean.fileIdStr},
          {K: "stauts", V: $scope.taskFlow.bean.stauts}
        ]
      };
      toPost.Post("OrderSaveStatus", postBean,function (currMsg) {
        if (currMsg.IsError) {
          $ionicLoading.show({
            noBackdrop: true,
            template: currMsg.Message,
            duration: 2000
          });
        }
        else {
          $ionicLoading.show({
            noBackdrop: true,
            template: "提交成功",
            duration: 2000
          });
          $state.go($scope.taskFlow.returnUrl,{id:$scope.taskFlow.bean.orderId});
        }
      });
    },
  }
}])

/**
 * Created by wengzhilai on 2016/11/4.
 */

mainController.controller('teamDetailCtr', ["Storage", "toPost", "fileUpFac", "storageUserFac", "CarIn", "$scope", "$ionicLoading", "$state", "$stateParams", "$ionicPopup", function (Storage, toPost, fileUpFac, storageUserFac, CarIn, $scope, $ionicLoading, $state, $stateParams, $ionicPopup) {
  $scope.Url=""//当前显示的图片地址
  $scope.team = {
    nowTabsIndex:1,//当前列表
    scrollWidth:0,
    scrollHeight:0,
    lists:[],
    bean: {
      userId: 0,
      authToken: '',
      pageSize: 0,
      id: 0,
      currentPage: 0,
      searchKey: [],
      orderBy: []
    },
    init:function () {
      this.scrollHeight=$(document).height()-300;
      this.scrollWidth=$(document).width()-30;
      toPost.list("TeamMyAll", $scope.team.bean, function (currMsg) {
        if (currMsg.IsError == true) {
          $ionicLoading.show({
            noBackdrop: true,
            template: currMsg.Message,
            duration: 2000
          });
        } else {
          $scope.team.lists = currMsg;
        }
        $scope.$broadcast('scroll.refreshComplete');
        $scope.$broadcast('scroll.infiniteScrollComplete');
      });
    },
    onTab:function(obj,index){
      $(".tab-item").removeClass("active")
      $(obj.target).addClass("active");
      this.nowTabsIndex=index;
    },
    showBigImage: function (ent) {
      var url = ent.target.getAttribute("src");
      if (url == null || url == '') {
        return;
      }
      fileUpFac.FullScreenImage(url,$scope);
    }
  };
}])

/**
 * Created by wengzhilai on 2016/8/7.
 */
mainController.controller('editUserCtr', ["common", "$scope", "$ionicPopup", "$state", "toPost", "storageUserFac", "$cordovaCamera", "$cordovaImagePicker", "$ionicLoading", "fileUpFac", "$ionicActionSheet", function (common,$scope, $ionicPopup, $state, toPost, storageUserFac, $cordovaCamera, $cordovaImagePicker, $ionicLoading, fileUpFac, $ionicActionSheet) {
    $scope.user = {
        bean:  storageUserFac.getUser(),
        currEnt: {},
        upImg: function (obj, OutFileId) {
            $scope.user.currEnt = $(obj.target);
            fileUpFac.upImg($scope.user.currEnt, OutFileId, $scope.user.upCallback,$scope);
        },
        upCallback: function (result) {
            var name = $scope.user.currEnt.attr("name");
            switch(name)
            {
                case "iconURL":
                    $scope.user.bean.ICON_FILES_ID = result.ID;
                    $scope.user.bean.iconURL = result.URL;
                    break;
                case "idNoUrl":
                    $scope.user.bean.ID_NO_PIC = result.ID;
                    $scope.user.bean.idNoUrl = result.URL;
                    break;
                case "driverPicUrl":
                    $scope.user.bean.DRIVER_PIC_ID = result.ID;
                    $scope.user.bean.driverPicUrl =result.URL;
                    break;
            }
        },
        showBigImage: function (ent) {
            var url =$(ent.target).attr("src");
            if (url == null || url == '') {
                //选择上传图片
                this.upImg(ent);
                return;
            }
            fileUpFac.FullScreenImage(url,$scope);
        },
        save: function () {
            toPost.saveOrUpdate("SalesmanSave",this.bean,this.saveBack);
        },
        saveBack: function (currMsg) {
            if(!currMsg.IsError) {
                $ionicLoading.show({
                    noBackdrop: true,
                    template: "个人信息保存成功！",
                    duration: 2000
                });
                storageUserFac.setUser($scope.user.bean);
                $state.go('user', {reload: true});  //路由跳转
            }
            else
            {
                $ionicLoading.show({
                    noBackdrop: true,
                    template:currMsg.Message,
                    duration: 2000
                });
            }
        },
        updatePassword: function () {
            var myPopup = $ionicPopup.show({
                template: '<input type="password" ng-model="user.bean.password">',
                title: '请输入你要修改的新密码',
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
                                var subBean = {
                                    authToken: storageUserFac.getUserAuthToken(),
                                    userId: 0,
                                    entity: $scope.user.bean.password
                                };
                                toPost.Post("UserEditPwd",subBean,$scope.user.updatePasswordBack);
                            }
                        }
                    }
                ]
            });
        },
        updatePasswordBack: function (currMsg){
            $ionicLoading.show({
                noBackdrop: true,
                template: "密码更新成功！",
                duration: 2000
            });
        }
    };
}])

/**
 * Created by wengzhilai on 2016/9/18.
 */
mainController.controller('findPwdCtr', ["common", "$scope", "$ionicLoading", "sendCodeFac", "toPost", "$state", function (common,$scope, $ionicLoading, sendCodeFac, toPost, $state) {
    $scope.findPwd = {
        bean: {
            userId: 0,
            authToken: '',
            id: 0,
            para: [{
                K: 'VerifyCode',
                V: '',
                T: 'string'
            }, {
                K: 'LoginName',
                V: '',
                T: 'string'
            }, {
                K: 'NewPwd',
                V: '',
                T: 'string'
            }]
        },
        onSubmit: function () {
            console.log(this.bean);
            toPost.Post("ResetPassword",this.bean,this.onSubmitBack)
            userFac.findPwd(this.bean);
        },
        onSubmitBack: function (currMsg) {
            console.log(currMsg);
            if (currMsg.IsError == true) {
                $ionicLoading.show({
                    noBackdrop: true,
                    template: currMsg.Message,
                    duration: 2000
                });
            } else {
              common.hint("重设密码成功");
                $state.go('login', {reload: true});  //路由跳转
            }
        }
    };

    $scope.SendCode = function () {
        $("#sendCode").text('发送中...');
        toPost.Post("SendCode",{phone: $scope.findPwd.bean.para[1].V},$scope.SendCodeBack)
    };
    $scope.SendCodeBack = function (currMsg) {
        console.log(currMsg);
        if (currMsg.IsError == true) {
            $ionicLoading.show({
                noBackdrop: true,
                template: currMsg.Message,
                duration: 2000
            });
        } else {
            $("#sendCode").text('获取成功');
        }
    };

}])

/**
 * Created by wengzhilai on 2016/8/7.
 */
mainController.controller('loginCtr', ["$scope", "toPost", "$state", "common", "CarIn", "storageUserFac", "Storage", "$ionicLoading", function ($scope, toPost, $state,common, CarIn,storageUserFac,Storage, $ionicLoading) {
    //Storage.set("loginName",'18180770313');
    $scope.login = {
        rememberPwd:(Storage.get("loginPwd")==null)?false:true,
        bean: {
            loginName: Storage.get("loginName"),
            password: Storage.get("loginPwd"),
            version: CarIn.version,
            type: '1',
            openid:common.getQueryString("openid")
        },
        user: {},
        reLoadJS: function () {
            location.reload();
        },
        submit: function () {
            //alert(document.loginForm.loginName.checkValidity()===false);
            if (document.loginForm.checkValidity() === false) {
                if (document.loginForm.loginName.checkValidity() === false) {
                  common.hint("登录名不能为空");
                } else if (document.loginForm.password.checkValidity() === false) {
                  common.hint("密码不能为空");
                }
            } else {
                Storage.set("loginName", $scope.login.bean.loginName);
                if($scope.login.rememberPwd)
                {
                    Storage.set("loginPwd", $scope.login.bean.password);
                }else
                {
                    Storage.remove("loginPwd");
                }
                if(navigator.userAgent.toLowerCase().indexOf('micromessenger')>-1) {
                    $scope.login.openid=common.getQueryString("openid");
                    if($scope.login.openid==null) {
                      common.hint(window.location.href);
                      common.hint("未获取获取到openid");
                      return;
                    }
                }
                toPost.Post("SalesmanLogin",this.bean,this.onSubmitBack)
            }
        },
        onSubmitBack: function (currMsg) {
            if (currMsg.IsError) {
                $ionicLoading.show({
                    noBackdrop: true,
                    template: currMsg.Message,
                    duration: 2000
                });
            } else {
                storageUserFac.setUser(currMsg);
                $state.go('home', {reload: true});  //路由跳转
            }
        }
    };
}])

/**
 * Created by wengzhilai on 2016/10/23.
 */
mainController.controller('registerCtr', ["$scope", "toPost", "Storage", "userFac", "CarIn", "$stateParams", "$ionicPopup", "$state", "$ionicLoading", "$interval", function ($scope, toPost, Storage, userFac, CarIn, $stateParams, $ionicPopup, $state, $ionicLoading, $interval) {

  var opendId = Storage.get("openid", '');
  if (opendId != null && opendId != '') {
    $scope.initBack = function (ent) {
      var code = ent.SCENE_STR;
      var type = code.substr(0, code.lastIndexOf("_"));
      if (type == "qrscene_team") {
        code = code.substr(code.lastIndexOf("_") + 1);
        $scope.register.bean.pollCode = code;
      }
      else {
        common.hint('你不是服务商，请关闭后，选择用户入口');
        window.close();
      }
    }
    var postBean = {
      authToken: Storage.get("openid", '')
    }
    toPost.Post("WeixinGetUser", postBean, $scope.initBack);
  }

  $scope.register = {
    bean: {
      loginName: '',
      password: '',
      version: CarIn.version,
      code: '',
      type: '1',
      pollCode: $stateParams.pollCode
    },
    init: function () {

    },
    onSubmit: function () {
      var rpw = document.getElementById('rpw').value;
      if (this.bean.password == rpw) {
        toPost.Post("LoginReg", this.bean, $scope.register.regCallback);
      } else {
        var alertPopup = $ionicPopup.alert({
          title: '警告',
          template: '两次密码的值不一致！',
          okText: '确认'
        });
        alertPopup.then(function (res) {
          console.log('显示警告');
        });
      }
    },
    regCallback: function (currMsg) {
      console.log(currMsg);
      if (currMsg.IsError == true) {
        $ionicLoading.show({
          noBackdrop: true,
          template: currMsg.Message,
          duration: 2000
        });
      } else {
        $ionicLoading.show({
          noBackdrop: true,
          template: "业务员注册成功",
          duration: 2000
        });
        $state.go('login', {reload: true});  //路由跳转
      }
    }
  };

  $scope.paracont = "获取验证码";
  $scope.paraclass = "badge assertive-bg light button";
  $scope.paraevent = true;
  $scope.SendCode = function () {
    if(!$scope.paraevent)return;
    toPost.Post("SendCode", {phone: $scope.register.bean.loginName}, function (currMsg) {
      console.log(currMsg);
      if (currMsg.IsError == true) {
        $ionicLoading.show({
          noBackdrop: true,
          template: currMsg.Message,
          duration: 2000
        });
      } else {
        var second = 60,
          timePromise = undefined;
        timePromise = $interval(function () {
          if (second <= 0) {
            $interval.cancel(timePromise);
            timePromise = undefined;
            second = 60;
            $scope.paracont = "重发验证码";
            $scope.paraclass = "badge assertive-bg light button";
            $scope.paraevent = true;
          } else {
            $scope.paracont = second + "秒后重发";
            $scope.paraclass = "badge button";
            $scope.paraevent = false;
            second--;
          }
        }, 1000, 100);
      }
    });
  };

}])

/**
 * Created by wengzhilai on 2016/11/7.
 */

mainController.controller('salesmanInfoCtr', ["Storage", "toPost", "fileUpFac", "storageUserFac", "CarIn", "$scope", "$ionicLoading", "$state", "$stateParams", "$ionicPopup", function (Storage, toPost, fileUpFac, storageUserFac, CarIn, $scope, $ionicLoading, $state, $stateParams, $ionicPopup) {
  $scope.Url=""//当前显示的图片地址
  $scope.salesman = {
    nowTabsIndex:1,//当前列表
    scrollWidth:0,
    scrollHeight:0,
    bean: {

    },
    init:function () {
      this.scrollHeight=$(document).height()-300;
      this.scrollWidth=$(document).width()-30;
      if($stateParams.id!=null)
      {
        toPost.single("SalesmanSingle", $stateParams.id, function (currMsg) {
          if (currMsg.IsError == true) {
            $ionicLoading.show({
              noBackdrop: true,
              template: currMsg.Message,
              duration: 2000
            });
          } else {
            $scope.salesman.bean = currMsg;
          }
        });
      }
    },
    onTab:function(obj,index){
      $(".tab-item").removeClass("active")
      $(obj.target).addClass("active");
      this.nowTabsIndex=index;
    },
    showBigImage: function (ent) {
      var url = ent.target.getAttribute("src");
      if (url == null || url == '') {
        return;
      }
      fileUpFac.FullScreenImage(url,$scope);
    }
  };
}])

/**
 * Created by wengzhilai on 2016/9/19.
 */
mainController.controller('userCtr', ["common", "$scope", "CarIn", "$ionicPopup", "$state", "storageUserFac", "fileUpFac", "toPost", function (common,$scope, CarIn, $ionicPopup, $state, storageUserFac,fileUpFac,toPost) {
  $scope.user = {
    bean: storageUserFac.getUser(),
    toShare: function () {
      var user_id = storageUserFac.getUserId();
      var baseUrl = CarIn.baseUrl;
      var qrcode = baseUrl + '/File/QrCode/salesman_' + user_id + '.jpg';
      var qrcodeWeiXin = baseUrl + '/File/QrCode/salesman_weixin_' + user_id + '.jpg';
      if (navigator.userAgent.toLowerCase().indexOf('micromessenger') > -1) {
        wx.previewImage({
          current: qrcodeWeiXin, // 当前显示图片的http链接
          urls: [qrcode, qrcodeWeiXin] // 需要预览的图片http链接列表
        });
        return;
      }
      else {
        fileUpFac.FullScreenImage(qrcodeWeiXin,$scope)
        //window.plugins.socialsharing.share("乐享", "subject", qrcodeWeiXin, qrcodeWeiXin);
      }
    },
    weChat: function () {
      $("#mcover").css("display", "none");  // 点击弹出层，弹出层消失
    },
    watting: function () {
      common.hint("后续开放，敬请期待");
    },
    sendFre: function () {
      common.hint("发送给朋友");
    }
  };
  $scope.outLogin = function () {
    var confirmPopup = $ionicPopup.confirm({
      title: '确认注销',
      template: '是否退出登录?',
      okText: '注销',
      cancelText: '取消'
    });
    confirmPopup.then(function (res) {
      if (res) {
        toPost.single("LoginOut",0,function (errobj) {
          if (errobj.IsError) {
            $ionicPopup.alert({
              title: '提示',
              template: "退出错误"
            });
          }
          storageUserFac.clearAll();
          $state.go('login', {reload: true});  //路由跳转
        });

      } else {
        console.log('You are not sure');
      }
    });
  };

}])
