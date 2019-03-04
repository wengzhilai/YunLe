// Ionic Starter App

// angular.module is a global place for creating, registering and retrieving Angular modules
// 'starter' is the name of this angular module example (also set in a <body> attribute in index.html)
// the 2nd parameter is an array of 'requires'
// 'starter.services' is found in services.js
// 'starter.controllers' is found in controllers.js
angular.module('starter', ['ionic', 'starter.controllers', 'starter.services', 'config', 'filters', 'ngCordova', 'ngIOS9UIWebViewPatch','CoderYuan'])
  .run(function ($ionicPlatform, CarIn, Storage, toPost, common, storageUserFac, $state, $rootScope, $ionicActionSheet, $timeout, $interval, $location, $cordovaAppVersion, $ionicPopup, $ionicLoading, $cordovaFileTransfer, $cordovaFile, $cordovaFileOpener2) {
    $ionicPlatform.ready(function () {
      // Hide the accessory bar by default (remove this to show the accessory bar above the keyboard
      // for form inputs)
      console.log(new Date().toLocaleTimeString()+"1");

      if (window.cordova && window.cordova.plugins && window.cordova.plugins.Keyboard) {
        cordova.plugins.Keyboard.hideKeyboardAccessoryBar(true);
        cordova.plugins.Keyboard.disableScroll(true);
      }
      if (window.StatusBar) {
        // org.apache.cordova.statusbar required
        StatusBar.styleDefault();
      }
      //是微信
      if (navigator.userAgent.toLowerCase().indexOf('micromessenger') > -1) {
        var openid = common.getQueryString("openid");
        var code = common.getQueryString("code");
        var state = common.getQueryString("state");
        if (openid == null && code == null && state == null) {
          var tmp = window.location.href;
          var redUrl = tmp.substr(0, tmp.indexOf("#"));
          state = tmp.substr(tmp.indexOf("#") + 2);
          if (state.indexOf('register') > -1) //如果是注册就不自动登录
          {
            return;
          }
          var strUrl = "https://open.weixin.qq.com/connect/oauth2/authorize?appid=" + CarIn.weixinAppId + "&redirect_uri=" + redUrl + "&response_type=code&scope=snsapi_base&state=" + state + "#wechat_redirect"
          window.location = strUrl;
          return;
        }
        else if (openid != null) {
          var postBean = {
            "openid": openid
          }
          Storage.set("openid", openid);

          toPost.Post("SalesmanLogin", postBean, function (currMsg) {
            if (currMsg.IsError) {
              $state.go('login', {reload: true});  //路由跳转
            }
            else {
              storageUserFac.setUser(currMsg);
              toPost.Post("WeiXinJSSDKSign", {"authToken": location.href.split('#')[0]}, function (reBean) {
                console.log(reBean);
                wx.config({
                  debug: false, // 开启调试模式,调用的所有api的返回值会在客户端alert出来，若要查看传入的参数，可以在pc端打开，参数信息会通过log打出，仅在pc端时才会打印。
                  appId: reBean.appId, // 必填，公众号的唯一标识
                  timestamp: reBean.timestamp, // 必填，生成签名的时间戳
                  nonceStr: reBean.nonceStr, // 必填，生成签名的随机串
                  signature: reBean.signature,// 必填，签名，见附录1
                  jsApiList: [
                    // 所有要调用的 API 都要加到这个列表中
                    'chooseImage',
                    'previewImage',
                    'uploadImage',
                    'downloadImage'
                  ] // 必填，需要使用的JS接口列表，所有JS接口列表见附录2
                });
                wx.ready(function () {
                  console.log('成功');
                  //wx.hideOptionMenu();
                });
                wx.error(function (res) {
                  console.log('失败');
                });

              });

            }
          })
          return;
        }
        else if (code != null && state != null) {
          var postBean = {
            "openid": "code|" + code + "|" + state
          }
          //alert(postBean.openid)
          toPost.Post("SalesmanLogin", postBean, function (currMsg) {
            if (currMsg.IsError) {
              var tmp = window.location.href;
              var redUrl = tmp.substr(0, tmp.indexOf("?"));
              state = "home";
              if (state.indexOf('register') > -1) //如果是注册就不自动登录
              {
                return;
              }
              var strUrl = "https://open.weixin.qq.com/connect/oauth2/authorize?appid=" + CarIn.weixinAppId + "&redirect_uri=" + redUrl + "&response_type=code&scope=snsapi_base&state=" + state + "#wechat_redirect"
              window.location = strUrl;
              console.log('You are sure');
            }
            else {
              if (currMsg.ID == 0) {
                var tmp = window.location.href;
                var newUrl = tmp.substr(0, tmp.indexOf("?"));
                newUrl = newUrl + "?openid=" + currMsg.OpenId + "#login";
                Storage.set("openid", currMsg.OpenId);
                window.location = newUrl;
              }
              else {
                storageUserFac.setUser(currMsg);
                toPost.Post("WeiXinJSSDKSign", {"authToken": location.href.split('#')[0]}, function (reBean) {
                  console.log(reBean);
                  wx.config({
                    debug: false, // 开启调试模式,调用的所有api的返回值会在客户端alert出来，若要查看传入的参数，可以在pc端打开，参数信息会通过log打出，仅在pc端时才会打印。
                    appId: reBean.appId, // 必填，公众号的唯一标识
                    timestamp: reBean.timestamp, // 必填，生成签名的时间戳
                    nonceStr: reBean.nonceStr, // 必填，生成签名的随机串
                    signature: reBean.signature,// 必填，签名，见附录1
                    jsApiList: [
                      // 所有要调用的 API 都要加到这个列表中
                      'chooseImage',
                      'previewImage',
                      'uploadImage',
                      'downloadImage'
                    ] // 必填，需要使用的JS接口列表，所有JS接口列表见附录2
                  });
                  wx.ready(function () {
                    console.log('成功');
                    //wx.hideOptionMenu();
                  });
                  wx.error(function (res) {
                    console.log('失败');
                  });

                });

                $state.go(state, {reload: true});  //路由跳转
              }
            }
          })
          return;
        }
        console.log("是微信");
        return;
      }
      else if (ionic.Platform.isAndroid()) {
        //升级系统
        $cordovaAppVersion.getVersionCode().then(function (version) {
          var nowversionNum = version
          //获取服务器上版本
          var postBean = {
            userId: 0,
            id: nowversionNum,
            para: [
              {"K": "type", "V": "StaffApk"}
            ]
          };
          toPost.Post("CheckUpdate", postBean, function (ent) {
            if (ent.IsError) {
              alert("获取版本错误：" + ent.Message);
            }
            else {
              if (nowversionNum != ent.ID) {
                var confirmPopup = $ionicPopup.confirm({
                  title: '版本升级',
                  template: ent.REMARK, //从服务端获取更新的内容
                  cancelText: '取消',
                  okText: '升级'
                });
                confirmPopup.then(function (res) {
                  if (res) {
                    $ionicLoading.show({
                      template: "已经下载：0%"
                    });
                    var url = ent.UPDATE_URL;
                    var apkName = url.substr(url.lastIndexOf("/") + 1);
                    var targetPath = cordova.file.externalRootDirectory + apkName;
                    var trustHosts = true
                    var options = {};
                    $cordovaFileTransfer.download(url, targetPath, options, trustHosts).then(function (result) {
                      $cordovaFileOpener2.open(targetPath, 'application/vnd.android.package-archive'
                      ).then(function () {
                      }, function (err) {
                        alert('安装失败:' + err);
                      });
                      $ionicLoading.hide();
                    }, function (err) {
                      alert('下载失败:' + err);
                    }, function (progress) {
                      $timeout(function () {
                        var downloadProgress = (progress.loaded / progress.total) * 100;
                        $ionicLoading.show({
                          template: "已经下载：" + Math.floor(downloadProgress) + "%"
                        });
                        if (downloadProgress > 99) {
                          $ionicLoading.hide();
                        }
                      })
                    });
                  }
                });

              }
            }
          })


        });
      }
      else if (navigator.userAgent.toLowerCase().indexOf('mozilla') > -1) {
        console.log("浏览器");
      }

      if (storageUserFac.getUserId() == null) {
        $state.go('login', {reload: true});  //路由跳转
      }
    });
  })
  .config(function ($stateProvider, $urlRouterProvider, $ionicConfigProvider) {
    $ionicConfigProvider.platform.ios.tabs.position('bottom');

    // Ionic uses AngularUI Router which uses the concept of states
    // Learn more here: https://github.com/angular-ui/ui-router
    // Set up the various states which the app can be in.
    // Each state's controller can be found in controllers.js
    $stateProvider

    //车险路由
    //登录
      .state('login', {
        url: '/login',
        templateUrl: 'templates/user/login.html',
        controller: 'loginCtr'
      })
      .state('register', {
        url: '/register/:pollCode',
        cache: 'false',
        templateUrl: 'templates/user/register.html',
        controller: 'registerCtr'
      })
      .state('selectMyLat', {
        url: '/selectMyLat',
        cache: 'false',
        templateUrl: 'templates/home/selectMyLat.html',
        controller: 'selectMyLatCtr',
        params: {
          garage: null,
          location: null,
          url: null,
          bean: null,
          id: 0
        }
      })
      .state('selectGarage', {
        url: '/selectGarage',
        cache: 'false',
        templateUrl: 'templates/home/selectGarage.html',
        controller: 'selectGarageCtr',
        params: {
          garage: null,
          location: null,
          url: null,
          bean: null,
          id: 0
        }
      })
      .state('editUser', {
        url: '/editUser',
        templateUrl: 'templates/user/editUser.html',
        controller: 'editUserCtr'
      })
      .state('customerInfo', {
        url: '/customerInfo/:id',
        cache: 'false',
        templateUrl: 'templates/service/customerInfo.html',
        controller: 'customerInfoCtr',
        params: {
          customer: null
        }
      })
      .state('teamDetail', {
        url: '/teamDetail',
        cache: 'false',
        templateUrl: 'templates/team/teamDetail.html',
        controller: 'teamDetailCtr'
      })

      .state('followHelpOrder', {
        url: '/followHelpOrder/:id',
        cache: 'false',
        templateUrl: 'templates/order/followHelpOrder.html',
        controller: 'followHelpOrderCtr'
      })
      .state('followMaintainOrder', {
        url: '/followMaintainOrder/:id',
        templateUrl: 'templates/order/followMaintainOrder.html',
        controller: 'followMaintainOrderCtr'
      })
      .state('followTrialOrder', {
        url: '/followTrialOrder/:id',
        templateUrl: 'templates/order/followTrialOrder.html',
        controller: 'followTrialOrderCtr'
      })
      .state('followInsure', {
        url: '/followInsure/:id',
        cache: 'false',
        templateUrl: 'templates/insure/followInsure.html',
        controller: 'followInsureCtr',
        params: {
          InsureOder: null
        }
      })
      .state('editCustomer', {
        url: '/editCustomer/:id',
        cache: 'false',
        templateUrl: 'templates/service/editCustomer.html',
        controller: 'editCustomerCtr'
      })


      .state('editInsure', {
        url: '/editInsure/:id',
        templateUrl: 'templates/insure/editInsure.html',
        controller: 'editInsureCtr',
        params: {
          customer: null
        }
      })
      .state('editInsureFast', {
        url: '/editInsureFast/:id',
        cache: 'false',
        templateUrl: 'templates/insure/editInsureFast.html',
        controller: 'editInsureFastCtr',
        params: {
          customer: null
        }
      })
      .state('insureList', {
        url: '/insureList',
        cache: 'false',
        templateUrl: 'templates/insure/insureList.html',
        controller: 'insureListCtr',
        params: {
          clientId: null,
          orderType:null
        }
      })
      .state('findPwd', {
        url: '/findPwd',
        templateUrl: 'templates/user/findPwd.html',
        controller: 'findPwdCtr'
      })
      .state('carTab', {
        url: '/carTab',
        abstract: true,
        templateUrl: 'templates/public/tabs.html'
      })


      .state('home', {
        url: '/home',
        cache: 'false',
        templateUrl: 'templates/home/home.html',
        controller: 'homeCtr'
      })
      .state('carTab.home', {
        url: '/home',
        views: {
          'tab-home': {
            templateUrl: 'templates/home/home.html',
            controller: 'homeCtr'
          }
        }

      })

      .state('user', {
        url: '/user',
        templateUrl: 'templates/user/user.html',
        controller: 'userCtr'
      })
      .state('carTab.user', {
        url: '/user',
        views: {
          'tab-user': {
            templateUrl: 'templates/user/user.html',
            controller: 'userCtr'
          }
        }
      })
      .state('orderList', {
        url: '/orderList',
        cache: 'false',
        templateUrl: 'templates/order/orderList.html',
        controller: 'orderListCtr',
        params: {
          clientId: null,
          orderType: null,
          title: null
        }
      })
      .state('expireInsure', {
        url: '/expireInsure',
        cache: 'false',
        templateUrl: 'templates/insure/expireInsure.html',
        controller: 'expireInsureCtr',
        params: {
          expireInsureList: null
        }
      })
      .state('orderGrabList', {
        url: '/orderGrabList',
        cache: 'false',
        templateUrl: 'templates/order/orderGrabList.html',
        controller: 'orderGrabListCtr'
      })
      .state('orderRuningList', {
        url: '/orderRuningList',
        cache: 'false',
        templateUrl: 'templates/order/orderRuningList.html',
        controller: 'orderRuningListCtr'
      })
      .state('editHelpOrder', {
        url: '/editHelpOrder',
        cache: 'false',
        templateUrl: 'templates/order/editHelpOrder.html',
        controller: 'editHelpOrderCtr',
        params: {
          garage: null,
          location: null,
          id: 0,
          bean: null,
          user: null
        }
      })



      .state('customerList', {
        url: '/customerList',
        cache: 'false',
        templateUrl: 'templates/service/customerList.html',
        controller: 'customerListCtr'
      })

      .state('bbsList', {
        url: '/bbsList',
        templateUrl: 'templates/bbs/bbsList.html',
        controller: 'bbsListCtr'
      })
      .state('salesmanInfo', {
        url: '/salesmanInfo/:id',
        templateUrl: 'templates/user/salesmanInfo.html',
        controller: 'salesmanInfoCtr'
      })
      .state('Task/Handle', {
        url: '/Task/Handle/',
        templateUrl: 'templates/Task/Handle.html',
        controller: 'TaskHandleCtr',
        params: {
          taskId:null,
          orderId:null,
          butName:null,
          returnUrl:null,
          taskFlowId:null
        }
      })
    ;

    // if none of the above states are matched, use this as the fallback
    if (navigator.userAgent.toLowerCase().indexOf('micromessenger') > -1) {
      return;
    }
    $urlRouterProvider.otherwise('/home');

  })
  .directive('capitalize', function () {
    return {
      require: 'ngModel',
      link: function (scope, element, attrs, modelCtrl) {
        var capitalize = function (inputValue) {
          if (inputValue == undefined) inputValue = '';
          var capitalized = inputValue.toUpperCase();
          if (capitalized !== inputValue) {
            modelCtrl.$setViewValue(capitalized);
            modelCtrl.$render();
          }
          return capitalized;
        }
        modelCtrl.$parsers.push(capitalize);
        capitalize(scope[attrs.ngModel]); // capitalize initial value
      }
    };
  });
