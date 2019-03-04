var mainController=angular.module('starter.controllers', [])
  .controller('followOrderCtr', ["common", "$scope", "orderFac", "$ionicLoading", "$stateParams", "$state", "$ionicActionSheet", function (common,$scope, orderFac, $ionicLoading, $stateParams, $state, $ionicActionSheet) {
    if ($stateParams.id) {
      orderFac.rescueSingle($stateParams.id);
    } else {
      common.hint("无订单号无法跟踪")
    }

    $scope.$on('order.rescueSingleUpdate', function () {
      var currMsg = orderFac.getCurrentMes();

      $scope.followOrder = currMsg;

    });

  }])
  .controller('editCarCtr', ["$scope", "$ionicLoading", "$ionicPopup", "carFac", "$state", "$stateParams", "$ionicActionSheet", "$cordovaCamera", "fileUpFac", "$cordovaImagePicker", "Storage", function ($scope, $ionicLoading, $ionicPopup, carFac, $state, $stateParams, $ionicActionSheet, $cordovaCamera, fileUpFac, $cordovaImagePicker, Storage) {
    $scope.tmpNo = {
      no1: '川',
      no2: ''
    };

    $scope.car = {
      currEnt: {},
      bean: {},
      save: function () {
        $scope.car.bean.PLATE_NUMBER = $scope.tmpNo.no1 + $scope.tmpNo.no2;
        carFac.save(this.bean);
      },
      upImg: function (ent) {
        var src = ent.target.getAttribute("src");
        $scope.car.currEnt = ent;

        if (src != '') {
          var confirmPopup = $ionicPopup.confirm({
            title: '提示',
            template: '图片已经存在，是否要删除重新上传?',
            okText: '确认',
            cancelText: '取消'
          });
          confirmPopup.then(function (res) {
            if (res) {
              fileUpFac.fileDel(id);
              return;
            } else {
              console.log('You are not sure');
            }
          });
        } else {
          //选择上传图片
          selectImage();
        }

      }
    };

    var storageKey = 'car';
    var car = Storage.get(storageKey);
    if (car != null) {
      $scope.car.bean = car;
    }
    //选择图片形式
    var selectImage = function () {
      $ionicActionSheet.show({
        buttons: [
          {text: '相机 '},
          {text: '图库'}
        ],
        titleText: '选择图片来源',
        cancelText: '关闭',
        cancel: function () {
        },
        buttonClicked: function (index) {
          switch (index) {
            case 0:
              appendByCamera();
              return;
            case 1:
              pickImage();
              break;
            default:
              break;
          }
        }
      });
    };

    //调用相机
    var appendByCamera = function () {
      var options = {
        destinationType: Camera.DestinationType.FILE_URI,
        sourceType: Camera.PictureSourceType.CAMERA,
        quality: 50  //图片压缩，值为1-100
      };
      $cordovaCamera.getPicture(options).then(function (imageURI) {
        $("#carImg").attr('src', imageURI);
        $scope.car.bean.DrivingPicUrl = imageURI;
        fileUpFac.saveFile(imageURI);
      }, function (err) {
        // error
      });
    };

    //调用相册
    var pickImage = function () {
      var options = {
        maximumImagesCount: 1,
        width: 800,
        height: 800,
        quality: 50
      };
      $cordovaImagePicker.getPictures(options)
        .then(function (results) {
          for (var i = 0; i < results.length; i++) {
            $("#carImg").attr('src', results[i]);
            $scope.car.bean.DrivingPicUrl = results[i];
            fileUpFac.saveFile(results[i]);
          }
        }, function (error) {
          // error getting photos
        });
    };

    $scope.$on('fileUp.Update', function () {
      var currMsg = fileUpFac.getCurrentMes();
      //alert( $scope.car.bean.DRIVING_PIC_ID+"前："+ window.JSON.stringify($scope.car.bean));
      $scope.car.bean.DRIVING_PIC_ID = window.JSON.parse(currMsg.response).ID;
      $scope.car.bean.DrivingPicUrl = window.JSON.parse(currMsg.response).URL;
      // alert( window.JSON.stringify(currMsg)+"｜currMsg.response.ID 上传后的PIC_ID："+$scope.car.bean.DRIVING_PIC_ID);
      // alert("后:"+ window.JSON.stringify($scope.car.bean));
    });

    $scope.$on('Car.Update', function () {
      var currMsg = carFac.getCurrentMes();
      console.log(currMsg);
      $ionicLoading.show({
        noBackdrop: true,
        template: '车辆保存成功！',
        duration: 2000
      });
      $scope.car.bean.ID = currMsg.ID;

    });
    $scope.$on('fileDel.Update', function () {
      var currMsg = fileUpFac.getCurrentMes();
      $ionicLoading.show({
        noBackdrop: true,
        template: "删除图片成功！",
        duration: 2000
      });
      var ent = $scope.car.currEnt;
      ent.target.setAttribute("src", "");
      ent.target.setAttribute("data-id", "");
//选择上传图片
      selectImage();
    });

  }])

  .controller('vehListCtr', ["$scope", "$ionicLoading", "$state", "Storage", "$ionicActionSheet", function ($scope, $ionicLoading, $state, Storage, $ionicActionSheet) {
    var vehKey = "veh";
    $scope.vehList = {
      lists: [],
      goQuery: function () {
        Storage.remove(vehKey);
        $state.go('queryVeh', {reload: true});  //路由跳转
      }
    };

    if (Storage.get(vehKey) != null) {
      $scope.vehList.lists = Storage.get(vehKey);
    }

  }])
  .controller('myCarListCtr', ["$scope", "$ionicLoading", "carFac", "$state", "Storage", "$ionicActionSheet", function ($scope, $ionicLoading, carFac, $state, Storage, $ionicActionSheet) {
    var storageKey = 'car';
    $scope.carList = {
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
        return carFac.hasNextPage();
      },
      doRefresh: function () {
        console.log("下拉刷新");
        this.bean.currentPage = 1;

        carFac.getCarList(this.bean);
      },
      loadMore: function () {
        if (this.bean.currentPage == 0) {
          this.bean.currentPage = 1;
        } else {
          this.bean.currentPage++;

        }
        console.log("加载更多");

        carFac.getCarList(this.bean);

      },
      addCar: function () {
        Storage.remove(storageKey);
        $state.go('editCar', {reload: true});  //路由跳转
      },
      toEditCar: function (obj) {
        Storage.set(storageKey, obj);
        $state.go('editCar', {reload: true});  //路由跳转
      },
      showSDD: function (id) {
        $ionicActionSheet.show({
          buttons: [
            {text: '设为默认车辆'},
            {text: '删除车辆信息'}
          ],
          titleText: '选择操作',
          cancel: function () {
            // add cancel code..
          },
          buttonClicked: function (index) {
            if (index == 0) {
              //设为默认车辆
              carFac.setDefault(id);
            } else if (index == 1) {
              //删除车辆
              carFac.delCar(id);
            }
          }
        });
      }
    };

    $scope.$on('CarDel.Update', function () {
      var currMsg = carFac.getCurrentMes();
      if (currMsg.IsError == true) {
        $ionicLoading.show({
          noBackdrop: true,
          template: currMsg.Message,
          duration: 2000
        });

      } else {
        $ionicLoading.show({
          noBackdrop: true,
          template: '删除成功',
          duration: 2000
        });

        $scope.carList.doRefresh();
        console.log(currMsg);
      }

      $scope.$broadcast('scroll.refreshComplete');
      $scope.$broadcast('scroll.infiniteScrollComplete');
    });
    $scope.$on('CarSetDefault.Update', function () {
      var currMsg = carFac.getCurrentMes();
      if (currMsg.IsError == true) {
        $ionicLoading.show({
          noBackdrop: true,
          template: currMsg.Message,
          duration: 2000
        });

      } else {
        $ionicLoading.show({
          noBackdrop: true,
          template: '设为默认车辆成功',
          duration: 2000
        });

        $scope.carList.doRefresh();
        console.log(currMsg);
      }

      $scope.$broadcast('scroll.refreshComplete');
      $scope.$broadcast('scroll.infiniteScrollComplete');
    });

    $scope.$on('CarList.Update', function () {
      var currMsg = carFac.getCurrentMes();

      if (currMsg.IsError == true) {
        $ionicLoading.show({
          noBackdrop: true,
          template: currMsg.Message,
          duration: 2000
        });

      } else {
        $scope.carList.lists = currMsg;

        console.log(currMsg.data);
      }

      $scope.$broadcast('scroll.refreshComplete');
      $scope.$broadcast('scroll.infiniteScrollComplete');
    });


  }])

  .controller('carInfoCtr', ["$scope", "Storage", "insureFac", "$state", function ($scope, Storage, insureFac, $state) {
    $scope.carInfo = {
      bean: {
        MODEL: '商务车',
        PRICE: '20w',
        BRAND: 'DZ-12312',
        FRAME_NUMBER: 'DX-asdfiwe',
        ENGINE_NUMBER: 'asdfsadf',
        REG_DATA: '2012-2-2',
        TRANSFER_DATA: ''
      }, getInsure: function () {
        insureFac.getInsureByCar(this.bean);
      }
    };

    $scope.$on('Insure.SearInsureByCarUpdate', function () {
      var currMsg = insureFac.getCurrentMes();
      if (currMsg.IsError == true) {
        $ionicLoading.show({
          noBackdrop: true,
          template: currMsg.Message,
          duration: 2000
        });
        $state.go('insure', {reload: true});  //路由跳转
      } else {
        console.log(currMsg);
        var storageKey = 'insure';
        Storage.set(storageKey, currMsg);
        $state.go('insure', {reload: true});  //路由跳转
      }

    });
    var storageKey = 'car';
    if (Storage.get(storageKey) != null) {
      $scope.carInfo = Storage.get(storageKey);
      Storage.remove(storageKey);
    }

  }])
  .controller('insureCtr', ["$scope", "Storage", "carFac", "$state", function ($scope, Storage, carFac, $state) {
    $scope.insure = [
      {
        ID: 1,
        NAME: '太平洋保险',
        DrivingPicUrl: 'http://s.114chn.com/MerchantLogo/8c53f11a330e48b18b72108f8fd5c5e9_big.jpeg'
      },
      {
        ID: 2,
        NAME: '中国人保',
        DrivingPicUrl: 'http://imgsrc.baidu.com/forum/w%3D580/sign=0aab1d5b1e30e924cfa49c397c096e66/720e0cf3d7ca7bcb07e5c77cbf096b63f724a8c2.jpg'
      },
      {
        ID: 1,
        NAME: '太平洋保险',
        DrivingPicUrl: 'http://s.114chn.com/MerchantLogo/8c53f11a330e48b18b72108f8fd5c5e9_big.jpeg'
      },
      {
        ID: 2,
        NAME: '中国人保',
        DrivingPicUrl: 'http://imgsrc.baidu.com/forum/w%3D580/sign=0aab1d5b1e30e924cfa49c397c096e66/720e0cf3d7ca7bcb07e5c77cbf096b63f724a8c2.jpg'
      },
      {
        ID: 1,
        NAME: '太平洋保险',
        DrivingPicUrl: 'http://s.114chn.com/MerchantLogo/8c53f11a330e48b18b72108f8fd5c5e9_big.jpeg'
      },
      {
        ID: 2,
        NAME: '中国人保',
        DrivingPicUrl: 'http://imgsrc.baidu.com/forum/w%3D580/sign=0aab1d5b1e30e924cfa49c397c096e66/720e0cf3d7ca7bcb07e5c77cbf096b63f724a8c2.jpg'
      }
    ];
    var storageKey = 'insure';

    if (Storage.get(storageKey) != null && Storage.get(storageKey).length != 0) {
      $scope.insure = Storage.get(storageKey);
      Storage.remove(storageKey);
    }
    $scope.toInsure = function (id) {
      $state.go('editCustomer', {id: id, reload: true});  //路由跳转
    }
  }])
  .controller('searchCarCtr', ["$scope", "insureFac", "$ionicPopup", "$state", "$ionicLoading", "storageUserFac", function ($scope, insureFac, $ionicPopup, $state, $ionicLoading, storageUserFac) {
    $scope.searchCar = {
      tmpNo: {
        no1: '川',
        no2: ''
      },
      bean: {
        lateNumber: '',
        userName: ''
      },
      submit: function () {
        this.bean.lateNumber = this.tmpNo.no1 + this.tmpNo.no2;
        // insureFac.carSearch(this.bean);
        $state.go('carInfo', {reload: true});  //路由跳转
      }
    };

    var user = storageUserFac.getUser();
    if (user != null && user.NowCar != null) {
      if (user.NowCar.PLATE_NUMBER.length > 2) {
        $scope.searchCar.tmpNo.no1 = user.NowCar.PLATE_NUMBER.substr(0, 1);
        $scope.searchCar.tmpNo.no2 = user.NowCar.PLATE_NUMBER.substr(1);
        $scope.searchCar.bean.userName = user.NAME;
      }

    }
    $scope.$on('Insure.SearUpdate', function () {
      var currMsg = insureFac.getCurrentMes();
      if (currMsg.IsError == true) {
        $ionicLoading.show({
          noBackdrop: true,
          template: currMsg.Message,
          duration: 2000
        });
        $state.go('carInfo', {reload: true});  //路由跳转
      } else {
        var storageKey = 'car';
        Storage.set(storageKey, currMsg);
        $state.go('carInfo', {reload: true});  //路由跳转
      }

    })
  }])

  .controller('toInsureCtr', ["$scope", "$state", "$ionicLoading", function ($scope, $state, $ionicLoading) {

  }])
  .controller('helpButtonCtr', ["$scope", "baiduMap", "$state", "$ionicLoading", function ($scope, baiduMap, $state, $ionicLoading) {
    $scope.getLoc = function () {
      //获得当前定位
      baiduMap.getGeo();
    };
    $scope.$on("geo.Update", function () {
      var currMsg = baiduMap.getCurrentMes();
      console.log(currMsg);
      if (currMsg.IsError == true) {
        $state.go('helpMap', {lat: 30.675567, lon: 104.095573, reload: true});  //路由跳转
      } else {
        //alert("当前的定位是：long"+currMsg.long+",lat:"+currMsg.lat);
        $state.go('helpMap', {lat: currMsg.lat, lon: currMsg.long, reload: true});  //路由跳转
      }

    });
  }])

  .controller('testCtr', ["testFac", "$scope", "$ionicPopup", "$state", "Storage", "$ionicLoading", function (testFac, $scope, $ionicPopup, $state, Storage, $ionicLoading) {
    var storageKey = 'user';
    $scope.test = {
      obj: '',
      url: '',
      login: {
        aesUsername: '',
        aesPassword: ''
      },
      submit: function () {
        //this.obj = this.obj.replace("@(authToken)", Storage.get(storageKey).authToken);
        //var inEnt = JSON.parse(this.obj);
        /**
         var inEnt = {
                    userId: 0,
                    authToken:'yyyy',
                    searchKey:[
                        {
                            K: "lateNumber",
                            V: 'xxxx',
                            T: '=='
                        },
                        {
                            K: 'userName',
                            V: 'cccc',
                            T: '=='
                        }
                        ],
                    id: 0

                };
         **/
//this.url="/GAS/WebApiKnowledge/M_KnowledgeList";
        // this.url="/Ci59/WebApi/UserList";
        //var allEnt = {inEnt: inEnt, inStr: this.obj};
        testFac.toTest(this.url, null);
      },
      loginSubmit: function () {
        var obj = {
          saveKeys: "abd",
          entity: {
            version: "1.1",
            loginType: "手机端登录"
          }
        };
        console.log(obj);
        testFac.login(obj);
      }
    };
    $scope.$on('testLogin.Update', function () {
      var currMsg = testFac.getCurrentMes();
      console.log(currMsg);

      if (currMsg.IsError == true) {
        $ionicLoading.show({
          noBackdrop: true,
          template: currMsg.Message,
          duration: 2000
        });
      } else {

        Storage.set(storageKey, currMsg);
        console.log('登陆成功');
      }
    });
    $scope.$on('test.Update', function () {
      var currMsg = testFac.getCurrentMes();
      console.log(currMsg);

      if (currMsg.IsError == true) {
        $ionicLoading.show({
          noBackdrop: true,
          template: currMsg.Message,
          duration: 2000
        });
      } else {


      }
    });
  }])




    .controller('editOrderCtr', ["common", "$scope", "$stateParams", "$ionicPopup", "$state", "helpFac", "$ionicLoading", "baiduMap", "Storage", function (common,$scope, $stateParams, $ionicPopup, $state, helpFac, $ionicLoading, baiduMap, Storage) {
    var storageKey = 'user';
    var user = Storage.get(storageKey);
    // 定义高度
    document.getElementById('allmap').style.height = $(document).height() - 44 + "px";
    var map = new BMap.Map("allmap");
    map.centerAndZoom("成都", 11);
    baiduMap.getGeo();
    $scope.$on("geo.Update", function () {
      var currMsg = baiduMap.getCurrentMes();
      console.log(currMsg);
      if (currMsg.IsError == true) {
        $ionicLoading.show({
          noBackdrop: true,
          template: currMsg.Message,
          duration: 2000
        });
        //$state.go('helpMap', {lat: 30.675567, lon: 104.095573, reload: true});  //路由跳转
        map.centerAndZoom(new BMap.Point(104.095573, 30.675567), 13);

        helpFac.getHelpList(30.675567, 104.095573);

      } else {
        //alert("当前的定位是：long"+currMsg.long+",lat:"+currMsg.lat);

        map.centerAndZoom(new BMap.Point(currMsg.long, currMsg.lat), 13);

        helpFac.getHelpList(currMsg.lat, currMsg.long);

      }

    });
    $scope.order = {
      title: '',
      oTypeHtml: '',
      bean: {
        GARAGE_ID: 0,
        CLIENT_ID: user.ID,
        GARAGE_TYPE: '',
        REACH_TYPE: '',
        CLIENT_NAME: user.NAME,
        CLIENT_PHONE: user.phone,
        PLATE_NUMBER: user.NowCar == null ? '' : user.NowCar.PLATE_NUMBER,
        CAR_TYPE: user.NowCar == null ? '' : user.NowCar.MODEL,
        BRAND: user.NowCar == null ? '' : user.NowCar.BRAND,
        MODEL: user.NowCar == null ? '' : user.NowCar.FRAME_NUMBER,
        ORDER_TYPE: '',
        REMARK: '',
        CAR_ID: user.NowCar == null ? '' : user.NowCar.ID,
        REACH_TIME: ''
      },
      toOrderList: function () {
        $state.go('orderList', {type: $stateParams.type, reload: true});  //路由跳转
      }
    };
    if ($stateParams.type == 1) {
      $scope.order.title = "维修保养";
      $scope.order.oTypeHtml = '<option value="维护">维护</option><option value="保养">保养</option>';
    } else if ($stateParams.type == 2) {
      $scope.order.title = "道路救援";
      $scope.order.oTypeHtml = '<option value="救援">救援</option>';
    } else if ($stateParams.type == 3) {
      $scope.order.title = "代审车";
      $scope.order.oTypeHtml = '<option value="审车">审车</option>';
    } else {
      $scope.order.title = "我的订单";
    }

    $scope.$on("toHelp.Update", function () {
      var currMsg = helpFac.getCurrentMes();
      console.log(currMsg);
      if (currMsg.IsError == true) {
        $ionicLoading.show({
          noBackdrop: true,
          template: currMsg.Message,
          duration: 2000
        });
      } else {
        $ionicPopup.confirm({
          title: '订单提交成功',
          template: '已经提交订单，是否马上付款?',
          cancelText: '取消',
          okText: '确定'
        }).then(function (res) {
          if (res) {
            common.hint("进入付款页面")
          } else {
            common.hint("取消了")
          }
        });
      }
      ;


    });
    $scope.$on('helpList.Update', function () {
      var currMsg = helpFac.getCurrentMes();
      console.log(currMsg);

      if (currMsg.IsError == true) {
        $ionicLoading.show({
          noBackdrop: true,
          template: currMsg.Message,
          duration: 2000
        });
      } else {
        for (var i = 0; i < currMsg.data.length; i++) {
          var new_point = new BMap.Point(currMsg.data[i].LANG, currMsg.data[i].LAT);
          var marker = new BMap.Marker(new_point);  // 创建标注
          marker.data = currMsg.data[i];
          map.addOverlay(marker);              // 将标注添加到地图中
          if (i == 0) {
            map.panTo(new_point);
          }
          marker.addEventListener("click", function (e) {
            this.setAnimation(BMAP_ANIMATION_BOUNCE); //跳动的动画

            $ionicPopup.confirm({
              title: e.target.data.NAME,
              subTitle: '地址：' + e.target.data.ADDRESS,
              template: '<p>座机：' + e.target.data.PHONE + '</p><p>手机：' + e.target.data.REMARK + '</p><br/><P style="font-weight: bold">是否向其发起{{order.title}}?</P>',
              scope: $scope,
              cancelText: '取消',
              okText: '确定'
            }).then(function (res) {

              if (res) {
                // 自定义弹窗
                var myPopup = $ionicPopup.show({
                  template: '<div class="item item-input item-select"><div class="input-label">订单类型</div><select id="GARAGE_TYPE" ng-model="order.bean.GARAGE_TYPE">' + $scope.order.oTypeHtml + '</select></div>' +
                  '<div  class="item item-input item-select"><div class="input-label">送修方式</div><select  id="REACH_TYPE" ng-model="order.bean.REACH_TYPE"><option value="预约">预约</option><option value="送店">送店</option></select></div>' +
                  '<div class="list"><div class="item"><input type="text"  id="REACH_TIME" ng-model="order.bean.REACH_TIME"   placeholder="到店时间"/></div> ' +
                  '<div class="list"><div class="item"><input type="text" ng-model="order.bean.CLIENT_NAME"  id="CLIENT_NAME"  placeholder="联系人姓名"/></div> ' +
                  '<div class="list"><div class="item"><input type="text" ng-model="order.bean.CLIENT_PHONE"  placeholder="联系人电话"  id="CLIENT_PHONE" /></div> ' +
                  '<div class="list"><div class="item"><input type="text" placeholder="车牌号" ng-model="order.bean.PLATE_NUMBER" id="PLATE_NUMBER" /></div> ' +
                  '<div class="list"><div class="item"><input type="text" placeholder="备注" ng-model="order.bean.REMARK" id="REMARK" /></div> ',
                  scope: $scope,
                  title: $scope.order.title,
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
                      onTap: function () {
                        $scope.order.bean.ORDER_TYPE = $scope.order.bean.GARAGE_TYPE;
                        if ($scope.order.bean.REACH_TYPE == '送店' && $scope.order.bean.REACH_TIME == '') {
                          $ionicLoading.show({
                            noBackdrop: true,
                            template: "送店时间不能为空!",
                            duration: 2000
                          });
                        } else {
                          $scope.order.bean.GARAGE_ID = e.target.data.ID;
                          //alert(+"你选的维修站是："+ e.target.data.NAME);
                          helpFac.toHelp($scope.order.bean);

                        }

                      }
                    }
                  ]
                });

              } else {
                console.log('You are not sure');
              }
            });


          });
        }

      }

    });

  }])

  .controller('maintainOrderList', ["orderFac", "$scope", "$ionicPopup", "$state", "$ionicLoading", "$ionicActionSheet", function (orderFac, $scope, $ionicPopup, $state, $ionicLoading, $ionicActionSheet) {
    $scope.orderList = {
      title: "免费救援",
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
        return orderFac.hasNextPage();
      },
      doRefresh: function () {
        console.log("下拉刷新");
        this.bean.currentPage = 1;
        orderFac.getOrderList(this.bean);
      },
      loadMore: function () {
        if (this.bean.currentPage == 0) {
          this.bean.currentPage = 1;
        } else {
          this.bean.currentPage++;

        }
        console.log("加载更多");

        orderFac.getOrderList(this.bean);

      },
      showSFD: function (o) {
        $ionicActionSheet.show({
          buttons: [
            {text: '跟踪订单'},
            {text: '删除订单'}
          ],
          titleText: '选择操作',
          cancel: function () {
            // add cancel code..
          },
          buttonClicked: function (index) {
            if (index == 0) {
              $state.go('followMaintainOrder', {id: o.ID, reload: true});  //路由跳转
            } else if (index == 1) {
              if (o.STATUS == '已付款') {
                common.hint('付款订单无法删除！');
              } else {
                common.hint("不支持删除订单");
                // orderFac.delOrder(o.id);
              }
            }
          }
        });
      },
      addOrder: function () {
        if ($stateParams.type == 3) {
          baiduMap.getGeo();
          $scope.$on("geo.Update", function () {
            var currMsg = baiduMap.getCurrentMes();
            console.log(currMsg);
            if (currMsg.IsError == true) {
              $ionicLoading.show({
                noBackdrop: true,
                template: currMsg.Message,
                duration: 2000
              });

              $scope.order.bean.LANG = '104.095573';
              $scope.order.bean.LAT = '30.675567';
              // 自定义弹窗
              var myPopup = $ionicPopup.show({
                template: '<div class="item item-input item-select"><div class="input-label">订单类型</div><select id="GARAGE_TYPE" ng-model="order.bean.GARAGE_TYPE">' + $scope.order.oTypeHtml + '</select></div>' +
                '<div  class="item item-input item-select"><div class="input-label">送修方式</div><select  id="REACH_TYPE" ng-model="order.bean.REACH_TYPE"><option value="预约">预约</option><option value="送店">送店</option></select></div>' +
                '<div class="list"><div class="item"><input type="text"  id="REACH_TIME" ng-model="order.bean.REACH_TIME"   placeholder="到店时间"/></div> ' +
                '<div class="list"><div class="item"><input type="text" ng-model="order.bean.CLIENT_NAME"  id="CLIENT_NAME"  placeholder="联系人姓名"/></div> ' +
                '<div class="list"><div class="item"><input type="text" ng-model="order.bean.CLIENT_PHONE"  placeholder="联系人电话"  id="CLIENT_PHONE" /></div> ' +
                '<div class="list"><div class="item"><input type="text" placeholder="车牌号" ng-model="order.bean.PLATE_NUMBER" id="PLATE_NUMBER" /></div> ' +
                '<div class="list"><div class="item"><input type="text" placeholder="备注" ng-model="order.bean.REMARK" id="REMARK" /></div> ',
                scope: $scope,
                title: $scope.order.title,
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
                    onTap: function () {
                      $scope.order.bean.ORDER_TYPE = $scope.order.bean.GARAGE_TYPE;
                      if ($scope.order.bean.REACH_TYPE == '送店' && $scope.order.bean.REACH_TIME == '') {
                        $ionicLoading.show({
                          noBackdrop: true,
                          template: "送店时间不能为空!",
                          duration: 2000
                        });
                      } else {
                        carFac.orderSave($scope.order.bean);

                      }

                    }
                  }
                ]
              });
            } else {
              $scope.order.bean.LANG = currMsg.lon;
              $scope.order.bean.LAT = currMsg.lat;
              // 自定义弹窗
              var myPopup = $ionicPopup.show({
                template: '<div class="item item-input item-select"><div class="input-label">订单类型</div><select id="GARAGE_TYPE" ng-model="order.bean.GARAGE_TYPE">' + $scope.order.oTypeHtml + '</select></div>' +
                '<div  class="item item-input item-select"><div class="input-label">送修方式</div><select  id="REACH_TYPE" ng-model="order.bean.REACH_TYPE"><option value="预约">预约</option><option value="送店">送店</option></select></div>' +
                '<div class="list"><div class="item"><input type="text"  id="REACH_TIME" ng-model="order.bean.REACH_TIME"   placeholder="到店时间"/></div> ' +
                '<div class="list"><div class="item"><input type="text" ng-model="order.bean.CLIENT_NAME"  id="CLIENT_NAME"  placeholder="联系人姓名"/></div> ' +
                '<div class="list"><div class="item"><input type="text" ng-model="order.bean.CLIENT_PHONE"  placeholder="联系人电话"  id="CLIENT_PHONE" /></div> ' +
                '<div class="list"><div class="item"><input type="text" placeholder="车牌号" ng-model="order.bean.PLATE_NUMBER" id="PLATE_NUMBER" /></div> ' +
                '<div class="list"><div class="item"><input type="text" placeholder="备注" ng-model="order.bean.REMARK" id="REMARK" /></div> ',
                scope: $scope,
                title: $scope.order.title,
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
                    onTap: function () {
                      $scope.order.bean.ORDER_TYPE = $scope.order.bean.GARAGE_TYPE;
                      if ($scope.order.bean.REACH_TYPE == '送店' && $scope.order.bean.REACH_TIME == '') {
                        $ionicLoading.show({
                          noBackdrop: true,
                          template: "送店时间不能为空!",
                          duration: 2000
                        });
                      } else {
                        carFac.orderSave($scope.order.bean);

                      }

                    }
                  }
                ]
              });

            }
          });
        } else {
          $state.go('editOrder', {type: $stateParams.type, reload: true});  //路由跳转
        }
      },
      showFilter: function () {
        // 自定义弹窗
        var myPopup = $ionicPopup.show({
          template: '<div  class="item item-input item-select"><div class="input-label">过滤类型</div><select id="filterF"><option value="救援">救援</option><option value="维护">维护</option><option value="保养">保养</option><option value="投保">投保</option> <option value="审车">审车</option>  </select></div>' +
          '<div class="list"><div class="item">降序<input type="radio" name="order" value="desc" checked="checked"/></div> ' +
          '<div class="item">升序<input type="radio" name="order" value="asc"/></div>',
          title: '过滤条件',
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
                var filterF = $('#filterF option:selected').val();
                console.log('你将要过滤的是：' + filterF + '|' + order);

                $scope.orderList.bean.searchKey = [
                  {K: 'ORDER_TYPE', V: filterF, T: '=='}
                ];
                $scope.orderList.bean.orderBy = [
                  {K: 'ID', V: order, T: 'int'}
                ];
                $scope.orderList.doRefresh(this.bean);
              }
            }
          ]
        });
      }
    };
    $scope.$on('orderSave.Update', function () {
      var currMsg = orderFac.getCurrentMes();

      if (currMsg.IsError == true) {
        $ionicLoading.show({
          noBackdrop: true,
          template: currMsg.Message,
          duration: 2000
        });

      } else {
        common.hint("代审车保存成功");
      }
    });

    $scope.$on('orderListFac.Update', function () {
      var currMsg = orderFac.getCurrentMes();

      if (currMsg.IsError == true) {
        $ionicLoading.show({
          noBackdrop: true,
          template: currMsg.Message,
          duration: 2000
        });

      } else {
        $scope.orderList.lists = currMsg;
        console.log(currMsg.data);
        var count = currMsg.totalCount;
        var sum = 0;

        function getLocalTime(nS) {
          return new Date(parseInt(nS) * 1000).toLocaleString().replace(/年|月/g, "-").replace(/日/g, " ");
        }

        for (var a = 0; a < currMsg.data.length; a++) {
          currMsg.data[a].COST = (currMsg.data[a].COST == undefined) ? 0 : currMsg.data[a].COST;
          sum += currMsg.data[a].COST;
        }

        $scope.orderList.sumTitle = '共' + count + '项订单，金额总数' + sum + '元';
      }


      $scope.$broadcast('scroll.refreshComplete');
      $scope.$broadcast('scroll.infiniteScrollComplete');
    });

  }])

  .controller('illegalSearchCtr', ["common", "$scope", "$ionicPopup", "$state", "$ionicLoading", "$ionicActionSheet", function (common, $scope, $ionicPopup, $state, $ionicLoading, $ionicActionSheet) {
    common.GetAllDistrict();

    $scope.$on('Common.GetAllDistrictUpdate', function () {
      var currMsg = common.getCurrentMes();
      console.log(currMsg);
      var dis = [];
      if (currMsg.IsError == true) {
        $ionicLoading.show({
          noBackdrop: true,
          template: currMsg.Message,
          duration: 2000
        });
      } else {
        for (var i = 0; i < currMsg.length; i++) {
          dis.push({text: currMsg[i].K + ' ' + currMsg[i].V});
        }
      }
    });
    $scope.showType = function () {

      var hideSheet = $ionicActionSheet.show({
        buttons: [
          {text: '小型汽车'},
          {text: '中型汽车'}
        ],
        titleText: '选择车辆类型',
        cancelText: '取消',
        cancel: function () {
          // add cancel code..
        },
        buttonClicked: function (index) {
          console.log(index);
          console.log(this.buttons[index].text);
          return true;
        }
      });
    };

    $scope.searchIllegal = {
      bean: {},
      submit: function () {
        carFac.carSearch(this.bean);
      }
    };
  }])
  .controller('trialMapCtr', ["$scope", "$ionicPopup", "$state", "$ionicLoading", function ($scope, $ionicPopup, $state, $ionicLoading) {
    // 定义高度
    document.getElementById('allmap').style.height = $(document).height() - 100 + "px";
    var map = new BMap.Map("allmap");
    map.centerAndZoom("成都", 11);

  }])
  .controller('createTrialCtr', ["$scope", "$ionicPopup", "$state", "$ionicLoading", function ($scope, $ionicPopup, $state, $ionicLoading) {

  }])
  .controller('createMaintainCtr', ["$scope", "$ionicPopup", "$state", "$ionicLoading", function ($scope, $ionicPopup, $state, $ionicLoading) {

  }])
  .controller('createHelpCtr', ["$scope", "$ionicPopup", "$state", "$ionicLoading", function ($scope, $ionicPopup, $state, $ionicLoading) {

  }])

  .controller('trialBackCtr', ["$scope", "$ionicPopup", "$state", "$ionicLoading", function ($scope, $ionicPopup, $state, $ionicLoading) {

  }])
  .controller('serviceListCtr', ["$scope", "userFac", "CarIn", "$ionicPopup", "$state", "$ionicLoading", "sendCodeFac", function ($scope, userFac, CarIn, $ionicPopup, $state, $ionicLoading, sendCodeFac) {

  }])
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
}])
  .controller('urlEchartsCtrl', ["$scope", "$sce", function ($scope, $sce) {
    document.getElementById('myechart').height = $(document).height() - 50 + "px";

    $scope.targetUrl = $sce.trustAsResourceUrl('http://www.baidu.com/'); //$sce.trustAsResourceUrl("http://192.168.2.132/eqs/Echarts/ECharts?code=DDD");

  }])
  .controller('echartCtrl', ["$scope", function ($scope) {
    // 基于准备好的dom，初始化echarts图表
    document.getElementById('main').style.height = $(document).height() - 50 + "px";

    var myChart = echarts.init(document.getElementById('main'));

    var option = {
      tooltip: {
        show: true
      },
      legend: {
        data: ['销量1', '销量2']
      },
      xAxis: [
        {
          type: 'category',
          data: ["衬衫", "羊毛衫", "雪纺衫", "裤子", "高跟鞋", "袜子"]
        }
      ],
      yAxis: [
        {
          type: 'value'
        }
      ],
      series: [
        {
          "name": "销量1",
          "type": "bar",
          "data": [5, 20, 40, 10, 10, 20]
        },
        {
          "name": "销量2",
          "type": "bar",
          "data": [10, 10, 20, 5, 20, 40]
        }
      ]
    };

    // 为echarts对象加载数据
    myChart.setOption(option);
    myChart.on('click', function (params) {
      console.log(params);
    });
  }]);



/**
 * Created by wengzhilai on 2016/8/18.
 */
mainController.controller('addressListCtr', ["$scope", "$state", "storageUserFac", "$ionicLoading", "toPost", function ($scope,$state,storageUserFac, $ionicLoading, toPost) {
    var storageKey = 'address';

    $scope.addressList = {
        showMsg:function()
        {
            $ionicLoading.show({
                noBackdrop: true,
                template: '为方便为您提供上门接车、礼品派送等服务，请准确填写您的地址！',
                duration: 2000
            });
        },
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
        reLoad: function () {
            if (storageUserFac.getUser() == null) {
                $state.go('login', {reload: true });
            }
            if (this.bean.currentPage == null || this.bean.currentPage == 0) {
                toPost.list("AddressList", this.bean,this.reFreshBack);
                //this.loadMore();
            }
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
            toPost.list("AddressList", this.bean,this.reFreshBack);
        },
        reFreshBack: function (currMsg) {
            if (currMsg.IsError == true) {
                $ionicLoading.show({
                    noBackdrop: true,
                    template: currMsg.Message,
                    duration: 2000
                });
            } else {
                $scope.addressList.lists = currMsg;
                console.log("返回数据");
                console.log(currMsg.data);
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
            toPost.list("AddressList", this.bean, this.reFreshBack);
        }
    };

}])

/**
 * Created by wengzhilai on 2016/8/18.
 */
/**
 * Created by Administrator on 2016/8/9.
 */
mainController.controller('addressSaveCtr', ["$scope", "$ionicLoading", "$ionicPopup", "toPost", "baiduMap", "storageUserFac", "$state", "$stateParams", "$cordovaGeolocation", function ($scope, $ionicLoading, $ionicPopup, toPost, baiduMap, storageUserFac, $state, $stateParams, $cordovaGeolocation) {
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
  }]
)
;

/**
 * Created by Administrator on 2016/8/9.
 */
mainController.controller('editCarCtr', ["$scope", "$ionicLoading", "$ionicPopup", "carFac", "toPost", "$state", "$stateParams", "$ionicActionSheet", "$cordovaCamera", "fileUpFac", "fileUpDel", "$cordovaImagePicker", "storageUserFac", function ($scope, $ionicLoading, $ionicPopup, carFac, toPost, $state, $stateParams, $ionicActionSheet, $cordovaCamera, fileUpFac, fileUpDel, $cordovaImagePicker, storageUserFac) {
  $scope.tmpNo = {
    no1: '川',
    no2: ''
  };
  $scope.chkchange = function (chk) {
    chk = "DDD";
  }
  if ($stateParams.id) {
    toPost.single("CarSingle", $stateParams.id, function (ent) {
      $scope.car.bean = ent;
      $scope.tmpNo = {
        no1: ent.PLATE_NUMBER.substr(0, 1),
        no2: ent.PLATE_NUMBER.substr(1)
      };
    })
  }

  $scope.car = {
    bigImage: false,
    bigSrc: '',
    showBigImage: function (ent) {
      var url = $(ent.target).attr("src");
      if (url == null || url == '') {
        //选择上传图片
        this.upImg(ent);
        return;
      }
      fileUpFac.FullScreenImage(url,$scope);
    },
    currEnt: {},
    bean: {},
    save: function () {
      if( $scope.tmpNo.no2!=null && $scope.tmpNo.no2!='')
      {
        $scope.car.bean.PLATE_NUMBER = $scope.tmpNo.no1 + $scope.tmpNo.no2;
      }
      toPost.saveOrUpdate("CarSave", this.bean, function (currMsg) {
        console.log(currMsg);
        if (currMsg.IsError) {
          $ionicLoading.show({
            noBackdrop: true,
            template: currMsg.Message,
            duration: 5000
          });
        }
        else
        {
          $ionicLoading.show({
            noBackdrop: true,
            template: '车辆保存成功！',
            duration: 5000
          });
          $scope.car.bean.ID = currMsg.ID;
          var user = storageUserFac.getUser();
          user.NowCar = currMsg;
          storageUserFac.setUser(user);
          $state.go('myCarList', {}, {reload: true});  //路由跳转
        }

      });
    },
    upImg: function () {
      var thisImgEvent = $("#carImg");
      fileUpFac.upImg(thisImgEvent, $scope.car.bean.DRIVING_PIC_ID, function (currMsg) {
        $scope.car.bean.DRIVING_PIC_ID = currMsg.ID;
        $scope.car.bean.DrivingPicUrl = currMsg.URL;
      },$scope);
    }
  };
}]);

/**
 * Created by Administrator on 2016/8/9.
 */
mainController.controller('myCarListCtr', ["$scope", "$ionicLoading", "$stateParams", "carFac", "$state", "Storage", "toPost", "$ionicActionSheet", "storageUserFac", function ($scope, $ionicLoading, $stateParams, carFac, $state, Storage, toPost, $ionicActionSheet, storageUserFac) {
  var storageKey = 'car';
  $scope.carList = {
    bean: {
      userId: 0,
      authToken: storageUserFac.getUserAuthToken(),
      pageSize: 0,
      id: 0,
      currentPage: 0,
      searchKey: [],
      orderBy: []
    },
    lists: {},
    reLoad: function () {
      if (storageUserFac.getUser() == null) {
        $state.go('login', {reload: true});
      }
      if (this.bean.currentPage == null || this.bean.currentPage == 0) {
        this.loadMore();
      }
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
      toPost.list("CarList", this.bean, $scope.carList.reFreshBack);
      //carFac.getCarList(this.bean);
    },
    reFreshBack: function (currMsg) {
      if (currMsg.IsError == true) {
        $ionicLoading.show({
          noBackdrop: true,
          template: currMsg.Message,
          duration: 2000
        });
      } else {
        $scope.carList.lists = currMsg;
        console.log("返回数据");
        console.log(currMsg.data);
      }
      $scope.$broadcast('scroll.refreshComplete');
      $scope.$broadcast('scroll.infiniteScrollComplete');
    },
    loadMore: function () {
      console.log("加载更多");
      toPost.list("CarList", this.bean, $scope.carList.reFreshBack);
    },
    addCar: function () {
      Storage.remove(storageKey);
      $state.go('editCar', {reload: true});  //路由跳转
    },
    toEditCar: function (obj) {
      Storage.set(storageKey, obj);
      $state.go('editCar', {"id": obj.ID});  //路由跳转
    },
    showSDD: function (id) {
      $ionicActionSheet.show({
        buttons: [
          {text: '设为默认车辆'},
          {text: '删除车辆信息'}
        ],
        titleText: '选择操作',
        cancel: function () {
          // add cancel code..
        },
        buttonClicked: function (index) {
          var obj = {
            userId: 0,
            authToken: storageUserFac.getUserAuthToken(),
            id: id
          }
          if (index == 0) {
            //设为默认车辆
            toPost.Post("CarSetDefault", obj, $scope.carList.setDefaultCallBack)
          } else if (index == 1) {
            //删除车辆
            toPost.Post("CarDelete", obj, $scope.carList.deleteCallBack)
          }
        }
      });
    },
    setDefaultCallBack: function (currMsg) {
      if (currMsg.IsError == true) {
        $ionicLoading.show({
          noBackdrop: true,
          template: currMsg.Message,
          duration: 2000
        });
      } else {
        $ionicLoading.show({
          noBackdrop: true,
          template: '设为默认车辆成功',
          duration: 2000
        });
        $scope.carList.bean.currentPage = 0;
        console.log(currMsg);
        var user = storageUserFac.getUser();
        user.NowCar = currMsg;
        storageUserFac.setUser(user);
        $state.go('myCarList', {}, {reload: true});  //路由跳转
      }
      $scope.$broadcast('scroll.refreshComplete');
      $scope.$broadcast('scroll.infiniteScrollComplete');
    },
    deleteCallBack: function (currMsg) {
      $ionicLoading.show({
        noBackdrop: true,
        template: '删除成功',
        duration: 2000
      });
      $scope.carList.bean.currentPage = 0;
      $scope.carList.loadMore();
      console.log(currMsg);
      $scope.$broadcast('scroll.refreshComplete');
      $scope.$broadcast('scroll.infiniteScrollComplete');
    }
  };
}])

/**
 * Created by wengzhilai on 2016/8/12.
 */
/**
 * Created by wengzhilai on 2016/8/10.
 */

mainController.controller('garageInfoCtr', ["$scope", "toPost", "$ionicPopup", "$ionicLoading", "$cordovaImagePicker", "$cordovaCamera", "$stateParams", "orderFac", "$state", "storageUserFac", "$ionicActionSheet", function ($scope,toPost, $ionicPopup, $ionicLoading, $cordovaImagePicker, $cordovaCamera, $stateParams, orderFac, $state, storageUserFac, $ionicActionSheet) {
  $scope.garage = {
    bean: {
    },
    currEnt: {}
  };
  if ($stateParams.id) {
    toPost.single("GarageSingle", $stateParams.id, function (response) {
      if (response.IsError) {
        common.hint(response.Message);
      } else {
        $scope.garage.bean=response;
      }
    });
  }

}])

/**
 * Created by wengzhilai on 2016/8/9.
 */
mainController.controller('homeCtr', ["common", "$scope", "$rootScope", "$interval", "storageUserFac", "CarIn", "toPost", "$state", "$ionicPopup", "$ionicTabsDelegate", "$cordovaLocalNotification", function (common,$scope, $rootScope, $interval, storageUserFac, CarIn, toPost, $state, $ionicPopup, $ionicTabsDelegate, $cordovaLocalNotification) {
  $scope.user = storageUserFac.getUser();
  $scope.home = {
    init: function () {
      console.log($scope.user)
      if ($scope.user == null || $scope.user.authToken == null || $scope.user.authToken == '') {
        console.log('没登录')
        $state.go('login', {reload: true});  //路由跳转
      }
      else {
        console.log('服务器验证')
        toPost.single("ClientSingle",$scope.user.ID,function (currMsg) {
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
    }
  }
  $scope.select = function (index) {
    $ionicTabsDelegate.select(index);
//        if (index == 1) {
//            $cordovaLocalNotification.isScheduled("1234").then(function (isScheduled) {
//                alert("Notification 1234 Scheduled: " + isScheduled);
//            });
//        }
  };
  if ($scope.user != null && $scope.user.NowCar != null) {
    $scope.NowCar = $scope.user.NowCar;
  }
  $scope.alert = function (str) {
    common.hint(str)
  };

  $scope.toEditCar = function (obj) {
    $state.go('editCar', {"id": obj.ID});  //路由跳转
  }
  if (ionic.Platform.isIOS() || ionic.Platform.isAndroid()) {
    //获取消息提醒
    var timer = $interval(function () {
      var postBean = {
        authToken: storageUserFac.getUserAuthToken(),
        userId: 0,
        pageSize: CarIn.pageSize,
        postBean: currentPage = 1,
        searchKey: [
          {"STATUS": "等待"}
        ]
      };
      $.post(CarIn.api + "MessageGetAll", postBean,
        //回调函数
        function (response) {
          if (!ent.IsError) {
            for (var i = 0; i < ent.data.length; i++) {
              $cordovaLocalNotification.add({
                id: ent.ID,
                date: new Date(),
                message: ent.CONTENT,
                title: ent.TITLE,
                autoCancel: true,
                sound: null
              }).then(function () {
                cordova.plugins.notification.local.on("click", function (notification, state) {
                  //alert('courses');
                }, this)
              });
            }
          }
        },
        //返回类型
        "json").error(function (err) {
      });
    }, 1000 * 60);
  }

}]);

/**
 * Created by Administrator on 2016/8/8.
 */
mainController.controller('selectGarageCtr', ["$ionicPlatform", "$scope", "common", "storageUserFac", "$stateParams", "$ionicPopup", "$state", "toPost", "baiduMap", "$location", function ($ionicPlatform, $scope, common, storageUserFac, $stateParams, $ionicPopup, $state, toPost, baiduMap, $location) {
    var map = null;
    $scope.help = {
        myMark:null,
        counter: null,
        goToHome: function () {
            $state.go('home', {reload: true });
        },
        goTohelpOrderList: function () {
            $state.go('helpOrderList', {reload: true });
        },
        reLoad: function () {
            if(storageUserFac.getUser().ID==null)
            {
                $state.go('login', {reload: true });
            }

            // 定义高度
            document.getElementById('allmap').style.height = $(document).height() - 44 + "px";
            map = new BMap.Map("allmap");
            map.centerAndZoom(new BMap.Point(104.072366, 30.66367), 11);
            var postBean = {};
            toPost.list("RescueQuery", postBean, this.garageCallback);
            this.getNewMark();
        },
        getNewMark:function()        {
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
                              bean: $stateParams.bean,
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
mainController.controller('selectMyLatCtr', ["$scope", "$timeout", "common", "baiduMap", "storageUserFac", "$stateParams", "$ionicPopup", "$state", "$ionicLoading", function ($scope, $timeout, common, baiduMap, storageUserFac, $stateParams, $ionicPopup, $state, $ionicLoading) {
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

  $scope.$on("geo.Update", function () {
    var currMsg = baiduMap.getCurrentMes();
    if (!currMsg.IsError) {
      map.centerAndZoom(new BMap.Point(currMsg.long, currMsg.lat), 13);
      createPointMark(currMsg.long, currMsg.lat);
    } else {
      common.hint("定位失败请用手动定位,请打开位置定位！");
      //单击获取点击的经纬度
      map.addEventListener("click", $scope.Lat.createMark, true);
      map.enableScrollWheelZoom();   //启用滚轮放大缩小，默认禁用
      map.enableContinuousZoom();    //启用地图惯性拖拽，默认禁用
      map.closeInfoWindow();
      map.disableDoubleClickZoom();
    }
  });
}]);

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
 * Created by Administrator on 2016/8/8.
 */
mainController.controller('helpMapCtr', ["$ionicPlatform", "$scope", "common", "storageUserFac", "$stateParams", "$ionicPopup", "$state", "toPost", "baiduMap", "$location", function ($ionicPlatform, $scope, common, storageUserFac, $stateParams, $ionicPopup, $state, toPost, baiduMap, $location) {
    var map = null;
    $scope.help = {
        myMark:null,
        counter: null,
        goToHome: function () {
            $state.go('home', {reload: true });
        },
        goTohelpOrderList: function () {
            $state.go('helpOrderList', {reload: true });
        },
        reLoad: function () {
            if(storageUserFac.getUser().ID==null)
            {
                $state.go('login', {reload: true });
            }

            // 定义高度
            document.getElementById('allmap').style.height = $(document).height() - 44 + "px";
            map = new BMap.Map("allmap");
            map.centerAndZoom(new BMap.Point(104.072366, 30.66367), 11);
            var postBean = {};
            toPost.list("RescueQuery", postBean, this.garageCallback);
            this.getNewMark();
        },
        getNewMark:function()        {
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
 * Created by wengzhilai on 2016/8/11.
 */
mainController.controller('helpOrderListCtr', ["common", "toPost", "$scope", "storageUserFac", "Storage", "$ionicPopup", "$state", "$ionicLoading", "$ionicActionSheet", function (common,toPost, $scope,storageUserFac, Storage, $ionicPopup, $state, $ionicLoading, $ionicActionSheet) {
    $scope.orderList = {
        bean: {
            searchKey : [
                {K: 'CLIENT_ID', V: storageUserFac.getUserId(), T: '=='},
                {
                    K: 'YL_ORDER_RESCUE.GARAGE_TYPE', V: '救援', T: '=='
                }
            ]
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
        reLoad: function () {
            if (this.bean.currentPage == null || this.bean.currentPage == 0) {
                this.loadMore();
            }
        },
        doRefresh: function () {
            console.log("下拉刷新");
            this.bean.currentPage = 1;
            toPost.list("OrderRescueList", this.bean, this.getList);
        },
        getList: function (response) {
            $scope.orderList.lists = response;
            console.log(response);
            $scope.$broadcast('scroll.refreshComplete');
            $scope.$broadcast('scroll.infiniteScrollComplete');
        },
        loadMore: function () {
            if (this.bean.currentPage == null || this.bean.currentPage == 0) {
                this.bean.currentPage = 1;
            } else {
                this.bean.currentPage++;
            }
            console.log("加载更多");
            toPost.list("OrderRescueList", this.bean, this.getList);
        },
        /*操作救援定单*/
        showSFD: function (o) {
            $ionicActionSheet.show({
                buttons: [
                    {text: '跟踪订单'},
                    {text: '删除订单'}
                ],
                titleText: '选择操作',
                cancel: function () {
                    // add cancel code..
                },
                buttonClicked: function (index) {
                    if (index == 0) {
                        $state.go('followHelpOrder', {id: o.ID, reload: true});  //路由跳转
                    } else if (index == 1) {
                        if (o.STATUS == '已付款') {
                          common.hint('付款订单无法删除！');
                        } else {
                          common.hint("不支持删除订单");
                            // orderFac.delOrder(o.id);
                        }
                    }
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
          $state.go("editHelpOrder",{orderType:'审车'})
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
      if (type == "维修" || type == "保养" || type == "救援" || type == "审车") {
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

mainController.controller('orderRuningListCtr', ["$scope", "storageUserFac", "toPost", "$ionicLoading", "$state", "$stateParams", "$ionicPopup", function ($scope,storageUserFac, toPost, $ionicLoading, $state, $stateParams, $ionicPopup) {
    $scope.orderList = {
        bean: {
            userId: 0,
            authToken: '',
            pageSize: 0,
            id: 0,
            currentPage: 0,
            searchKey: [
                {K: 'CLIENT_ID', V: storageUserFac.getUserId(), T: '=='},
                {K: "YL_ORDER_RESCUE", V: "null", T: '!='},
                {K: "YL_ORDER_RESCUE.STATUS", V: "完成", T: '!='},
                {K: "YL_ORDER_RESCUE.GARAGE_ID", V: "null", T: '!='}
            ],
            orderBy: []
        },
        toFollowOrder: function (id, type) {
            if (type == "救援") {
                $state.go('followHelpOrder', {id: id, reload: true});  //路由跳转
            } else if (type == "审车") {
                $state.go('followHelpOrder', {id: id, reload: true});  //路由跳转
            } else if (type == "维修" || type == "保养") {
                $state.go('followHelpOrder', {id: id, reload: true});  //路由跳转
            } else if (type == "投保") {
                $state.go('followInsure', {id: id, reload: true});  //路由跳转
            }
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

            var buB = $("a[class='button button-balanced']").text();
            switch (buB)
            {
                case "救援":
                    this.bean.searchKey=[{K: "ORDER_TYPE", V: buB, T: '=='}];
                    break;
                case "审车":
                    this.bean.searchKey=[{K: "ORDER_TYPE", V: buB, T: '=='}];
                    break;
                case "维保":
                    this.bean.searchKey=[{K: "ORDER_TYPE", V: buB, T: '=='}];
                    break;
                case "保单":
                    this.bean.searchKey=[{K: "ORDER_TYPE", V: "投保", T: '=='}];
                    break;
            }
            toPost.list("OrderList", this.bean, this.callListReturn);
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
 * Created by wengzhilai on 2016/8/12.
 */
/**
 * Created by wengzhilai on 2016/8/10.
 */

mainController.controller('orderSingleCtr', ["$scope", "$ionicPopup", "$ionicLoading", "$cordovaImagePicker", "$cordovaCamera", "$stateParams", "orderFac", "$state", "storageUserFac", "$ionicActionSheet", function ($scope, $ionicPopup, $ionicLoading, $cordovaImagePicker, $cordovaCamera, $stateParams, orderFac, $state, storageUserFac, $ionicActionSheet) {
    if ($stateParams.id) {
        orderFac.OrderSingle($stateParams.id);
    }
    var user = storageUserFac.getUser();
    $scope.insure = {
        bean: {
            ID:0,
            CAR_ID: 0,
            CLIENT_ID: storageUserFac.getUserId(),
            ORDER_NO:'',
            ORDER_TYPE:'',
            PAY_STATUS:'',
            COST:'',
            CREATE_TIME:new Date(),
            LANG:'',
            LAT:'',
            APPRAISE_SCORE:'',
            APPRAISE_CONTENT:'',
            REMARK:'',
            AllFlow:[],
            AllFiles:[],
            SaveProductId: [],
            ClientName: '',
            ClientPhone:''
        },
        currEnt: {},
        userInfo: user,
        carInfo: {},
        carsOptions: [],
        insurerOptions: [],
        insurerInfo: {}
    };

    $scope.$on('Insure.OrderSingle', function () {
        var currMsg = orderFac.getCurrentMes();
        console.log(currMsg);
        $scope.insure.bean = currMsg;

    });

}])
/**
 * Created by Administrator on 2016/8/17.
 */
mainController.controller('editMaintainOrderCtr', ["$scope", "toPost", "common", "$stateParams", "$state", "orderFac", "$ionicLoading", "storageUserFac", function ($scope, toPost, common, $stateParams, $state, orderFac, $ionicLoading, storageUserFac) {
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
}])
;

/**
 * Created by Administrator on 2016/8/21.
 */
mainController.controller('followMaintainOrderCtr', ["$scope", "toPost", "$ionicLoading", "$stateParams", "$state", "$ionicActionSheet", function ($scope, toPost, $ionicLoading, $stateParams, $state, $ionicActionSheet) {
  if ($stateParams.id) {
    toPost.single("RescueSingle", $stateParams.id, function (currMsg) {
      $scope.followOrder.bean = currMsg;
    })
  } else {
    $("#singleDiv").hide();
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

}]);

/**
 * Created by Administrator on 2016/8/20.
 */
mainController.controller('maintainOrderListCtr', ["common", "toPost", "$scope", "Storage", "$ionicPopup", "$state", "$ionicLoading", "$ionicActionSheet", function (common,toPost, $scope, Storage, $ionicPopup, $state, $ionicLoading, $ionicActionSheet) {
    $scope.orderList = {
        bean: {
            searchKey: [
                {
                    K: 'ORDER_TYPE', V: '维保', T: '=='
                }
            ]
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
        reLoad: function () {
            if (this.bean.currentPage == null || this.bean.currentPage == 0) {
                this.loadMore();
            }
        },
        doRefresh: function () {
            console.log("下拉刷新");
            this.bean.currentPage = 1;
            toPost.list("OrderRescueList", this.bean, this.getList);
        },
        getList: function (response) {
            $scope.orderList.lists = response;
            console.log(response);
            $scope.$broadcast('scroll.refreshComplete');
            $scope.$broadcast('scroll.infiniteScrollComplete');
        },
        loadMore: function () {
            if (this.bean.currentPage == null || this.bean.currentPage == 0) {
                this.bean.currentPage = 1;
            } else {
                this.bean.currentPage++;
            }
            console.log("加载更多");
            toPost.list("OrderRescueList", this.bean, this.getList);
        },
        showSFD: function (o) {
            $ionicActionSheet.show({
                buttons: [
                    {text: '跟踪订单'},
                    {text: '删除订单'}
                ],
                titleText: '选择操作',
                cancel: function () {
                    // add cancel code..
                },
                buttonClicked: function (index) {
                    if (index == 0) {
                        $state.go('followMaintainOrder', {id: o.ID, reload: true});  //路由跳转
                    } else if (index == 1) {
                      if (o.STATUS == '已付款') {
                        common.hint('付款订单无法删除！');
                      } else {
                        common.hint("不支持删除订单");
                        // orderFac.delOrder(o.id);
                      }
                    }
                }
            });
        }
    };

}]);

/**
 * Created by Administrator on 2016/8/17.
 */
mainController.controller('maintainMapCtr', ["$scope", "$timeout", "common", "$ionicModal", "$ionicPopover", "storageUserFac", "$stateParams", "$ionicPopup", "$state", "toPost", "baiduMap", function ($scope,$timeout, common,$ionicModal, $ionicPopover,storageUserFac, $stateParams, $ionicPopup, $state, toPost, baiduMap) {
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


}]);

/**
 * Created by wengzhilai on 2016/10/7.
 */
mainController.controller('queryVehCtr', ["common", "$scope", "toPost", "storageUserFac", "$state", "CarIn", "$ionicLoading", "Storage", "queryVehFac", function (common,$scope,toPost,storageUserFac, $state, CarIn, $ionicLoading, Storage, queryVehFac) {
    $scope.queryVeh = {
        Peccancy: {
            IdNo: '',
            Code: '',
            PicCode: '',
            Images: ''
        },
        SendCode: function () {
            if (this.Peccancy.IdNo == null || this.Peccancy.IdNo == '' || this.Peccancy.IdNo.length != 18) {
              common.hint('身份证号码有误，请确认后再试');
                return;
            } else if (this.Peccancy.Code == null || this.Peccancy.Code == '' || this.Peccancy.Code.length != 6) {
                if (!(this.Peccancy.PicCode == null || this.Peccancy.PicCode == '')) {

                    var postBean = {
                        userId: 0,
                        authToken: storageUserFac.getUserAuthToken(),
                        para: [
                            {K: "PicCode", V: this.Peccancy.PicCode},
                            {K: "IdNo", V: this.Peccancy.IdNo}

                        ]
                    };
                    toPost.Post("ClientPeccancy2",postBean, $scope.clientPeccancy2CallBack)
                } else {
                    var postBean = {
                        userId: 0,
                        authToken: storageUserFac.getUserAuthToken(),
                        para: [
                            {K: "IdNo", V: this.Peccancy.IdNo}
                        ]
                    };
                    toPost.Post("ClientPeccancy",postBean,$scope.clientPeccancyCallBack)
                }
            }

        },
        onSubmit: function () {
            var postBean = {
                userId: "",
                authToken: storageUserFac.getUserAuthToken(),
                para: [
                    {K: "Code", V: this.Peccancy.Code}
                ]
            };
            toPost.Post("ClientPeccancy1",postBean,$scope.clientPeccancy1CallBack)

            queryVehFac.clientPeccancy1(this.Peccancy.Code);
        }
    };

    $scope.clientPeccancy1CallBack= function (currMsg) {
        if (currMsg.data != null) {
            var storageKey = 'veh';
            Storage.set(storageKey, currMsg.data);
            $state.go('vehList', {reload: true});  //路由跳转
        } else {
          common.hint("没有违章记录");
        }
    };

    $scope.clientPeccancy2CallBack= function (currMsg) {
      common.hint('短信发送成功');
    };
    $scope.clientPeccancyCallBack= function (currMsg) {
        if (currMsg.Message == '验证码过期') {
            $scope.queryVeh.Peccancy.Images = currMsg.Params;
        }
        else {
            $ionicLoading.show({
                noBackdrop: true,
                template: currMsg.Message,
                duration: 2000
            });
        }
    };
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
 * Created by wengzhilai on 2016/8/7.
 */
mainController.controller('editUserCtr', ["$scope", "common", "$ionicPopup", "$state", "storageUserFac", "fileUpDel", "fileUpFac", "$cordovaCamera", "$cordovaImagePicker", "$ionicLoading", "toPost", "$ionicActionSheet", function ($scope, common, $ionicPopup, $state, storageUserFac, fileUpDel, fileUpFac, $cordovaCamera, $cordovaImagePicker, $ionicLoading, toPost, $ionicActionSheet) {
  $scope.ctrlBar = true;
  $scope.toggleFullScreen = function () {
    var fullElem = document.getElementById("fullScreen");
    if (document.webkitFullscreenElement) {
      document.webkitCancelFullScreen();
      $scope.ctrlBar = true;
    } else {
      fullElem.webkitRequestFullscreen();
      $scope.toggleVideo = function () {
        if ($scope.ctrlBar) {
          $timeout.cancel($scope.ctrlBar);
        }
        $scope.ctrlBar = $timeout(
          function () {
            $scope.ctrlBar = false;
          },
          5000);
      }
    }
  }

  $scope.user = {
    bean: storageUserFac.getUser(),
    currEnt: {},
    upImg: function (obj, OutFileId) {
      $scope.user.currEnt = $(obj.target);
      fileUpFac.upImg($scope.user.currEnt, OutFileId, $scope.user.upCallback,$scope);
    },
    upCallback: function (result) {
      var name = $scope.user.currEnt.attr("name");
      switch (name) {
        case "iconURL":
          $scope.user.bean.ICON_FILES_ID = result.ID;
          $scope.user.bean.iconURL = result.URL;
          break;
        case "idNoUrl":
          $scope.user.bean.ID_NO_PIC_ID = result.ID;
          $scope.user.bean.idNoUrl = result.URL;
          break;
        case "driverPicUrl":
          $scope.user.bean.DRIVER_PIC_ID = result.ID;
          $scope.user.bean.driverPicUrl = result.URL;
          break;
      }

    },
    showBigImage: function (ent) {
      var url = ent.target.getAttribute("src");
      if (url == null || url == '') {
        //选择上传图片
        this.upImg(ent);
        return;
      }
      fileUpFac.FullScreenImage(url, $scope);
    },
    save: function () {
      var obj = {
        authToken: storageUserFac.getUserAuthToken(),
        userId: 0,
        saveKeys: 'NAME,SEX,ADDRESS,ID_NO,ID_NO_PIC_ID,ICON_FILES_ID,DRIVER_PIC_ID',
        entity: this.bean
      };

      if($scope.user.bean.ID_NO!='')

      if ($scope.user.bean.ID_NO!='' || common.regExpIdNo($scope.user.bean.ID_NO, function (msg) {
          $ionicLoading.show({
            noBackdrop: true,
            template: msg,
            duration: 2000
          });
        })) {
        toPost.Post("ClientSave", obj, $scope.user.saveCallback);
      }

    },
    saveCallback: function (result) {
      $ionicLoading.show({
        noBackdrop: true,
        template: "个人信息保存成功！",
        duration: 2000
      });
      storageUserFac.setUser($scope.user.bean);
      $state.go('user', {reload: true});  //路由跳转
    },
    updatePassword: function () {
      var myPopup = $ionicPopup.show({
        template: '' +
        '<input type="password"  placeholder="输入原密码"  ng-model="user.bean.password" style="margin-bottom: 2px">' +
        '<input type="password"  placeholder="输入新密码"  ng-model="user.bean.password1" style="margin-bottom: 2px">' +
        '<input type="password"  placeholder="重复新密码"  ng-model="user.bean.password2" style="margin-bottom: 2px">',
        title: '修改密码',
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
                var obj = {
                  authToken: storageUserFac.getUserAuthToken(),
                  userId: 0,
                  entity: $scope.user.bean.password1,
                  para: [{
                    K: 'oldPwd', V: $scope.user.bean.password
                  }]
                };
                if ($scope.user.bean.password1 == $scope.user.bean.password2) {
                  toPost.Post("UserEditPwd", obj, $scope.user.upPasswordCallback)
                }
                else {
                  common.hint("两次密码不一致");
                }
              }
            }
          },
        ]
      });
    },
    upPasswordCallback: function (result) {
      $ionicLoading.show({
        noBackdrop: true,
        template: "密码更新成功！",
        duration: 2000
      });
    }
  };
}])

/**
 * Created by wengzhilai on 2016/9/30.
 */
mainController.controller('findPwdCtr', ["common", "$scope", "$ionicLoading", "sendCodeFac", "toPost", "$state", function (common,$scope, $ionicLoading, sendCodeFac, toPost, $state) {
    $scope.findPwd = {
        bean: {
            userId: 0,
            authToken: '',
            id: 0,
            para: [
                {
                    K: 'VerifyCode',
                    V: '',
                    T: 'string'
                },
                {
                    K: 'LoginName',
                    V: '',
                    T: 'string'
                },
                {
                    K: 'NewPwd',
                    V: '',
                    T: 'string'
                }
            ]

        },
        onSubmit: function () {
            console.log(this.bean);
            toPost.Post("ResetPassword", this.bean, $scope.findPwd.onSubmitCallback);
        },
        onSubmitCallback: function (currMsg) {
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
        toPost.Post("SendCode",{phone: $scope.findPwd.bean.para[1].V},$scope.SendCodeCallBack);

        sendCodeFac.senCode();
    };
    $scope.SendCodeCallBack=function (currMsg){
        console.log(currMsg);
        if (currMsg.IsError == true) {
            $ionicLoading.show({
                noBackdrop: true,
                template: currMsg.Message,
                duration: 2000
            });
            $("#sendCode").text('获取验证码');
        } else {
            $("#sendCode").text('获取成功');
        }
    };

}])

/**
 * Created by wengzhilai on 2016/8/7.
 */
mainController.controller('loginCtr', ["$scope", "toPost", "common", "$state", "CarIn", "storageUserFac", "Storage", "$ionicLoading", function ($scope, toPost, common,$state, CarIn,storageUserFac,Storage, $ionicLoading) {
    //Storage.set("loginName",'18180770313');

    $scope.login = {
        rememberPwd:(Storage.get("loginPwd")==null)?false:true,
        bean: {
            loginName: Storage.get("loginName"),
            password: Storage.get("loginPwd"),
            version: CarIn.version,
            type: '0',
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
                    if($scope.login.openid==null)
                    {
                      common.hint(window.location.href);
                      common.hint("未获取获取到openid");
                        return;
                    }
                }

                toPost.Post("ClientLogin",this.bean,this.onSubmitBack)
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
 * Created by Administrator on 2016/8/29.
 */
mainController.controller('registerCtr', ["$scope", "Storage", "toPost", "CarIn", "$ionicPopup", "$state", "$ionicLoading", "$stateParams", "$interval", function ($scope, Storage, toPost, CarIn, $ionicPopup, $state, $ionicLoading, $stateParams,$interval) {

    var opendId = Storage.get("openid", '');
    if (opendId != null && opendId != '') {
        $scope.initBack = function (ent) {
            var code = ent.SCENE_STR;
            code = code.substr(code.lastIndexOf("_") + 1);
            $scope.register.bean.pollCode = code;
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
            type: '0',
            pollCode: $stateParams.pollCode
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
                    template: "用户注册成功",
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
}]);

/**
 * Created by wengzhilai on 2016/8/22.
 */
mainController.controller('userCtr', ["$scope", "CarIn", "storageUserFac", "$ionicPopup", "$state", "$ionicLoading", function ($scope, CarIn, storageUserFac, $ionicPopup, $state,$ionicLoading) {
  $scope.user = {
    bean: storageUserFac.getUser(),
    toShare: function () {
      var salesman_id = storageUserFac.getUser().SALESMAN_ID;
      var baseUrl = CarIn.baseUrl;
      var qrcode = baseUrl + '/File/QrCode/salesman_' + salesman_id + '.jpg';
      var qrcodeWeiXin = baseUrl + '/File/QrCode/salesman_weixin_' + salesman_id + '.jpg';
      console.log(qrcodeWeiXin);
      if (navigator.userAgent.toLowerCase().indexOf('micromessenger') > -1) {
        wx.previewImage({
          current: qrcodeWeiXin, // 当前显示图片的http链接
          urls: [qrcode, qrcodeWeiXin] // 需要预览的图片http链接列表
        });
        return;
      }
      window.plugins.socialsharing.share("乐享", "subject", qrcode, qrcode);
    },
    alert: function (str) {
      $ionicLoading.show({
        noBackdrop: true,
        template: str,
        duration: 2000
      });
    }
  };

  $scope.$on('userSave.Update', function () {
    var currMsg = userFac.getCurrentMes();
    $ionicLoading.show({
      noBackdrop: true,
      template: currMsg.Message,
      duration: 2000
    });
  });
  $scope.outLogin = function () {
    var confirmPopup = $ionicPopup.confirm({
      title: '确认注销',
      template: '是否退出登录?',
      okText: '注销',
      cancelText: '取消'
    });
    confirmPopup.then(function (res) {
      if (res) {
        storageUserFac.remove();
        $state.go('login', {reload: true});  //路由跳转
      } else {
        console.log('You are not sure');
      }
    });
  };

}])
