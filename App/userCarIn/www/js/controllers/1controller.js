var mainController=angular.module('starter.controllers', [])
  .controller('followOrderCtr', function (common,$scope, orderFac, $ionicLoading, $stateParams, $state, $ionicActionSheet) {
    if ($stateParams.id) {
      orderFac.rescueSingle($stateParams.id);
    } else {
      common.hint("无订单号无法跟踪")
    }

    $scope.$on('order.rescueSingleUpdate', function () {
      var currMsg = orderFac.getCurrentMes();

      $scope.followOrder = currMsg;

    });

  })
  .controller('editCarCtr', function ($scope, $ionicLoading, $ionicPopup, carFac, $state, $stateParams, $ionicActionSheet, $cordovaCamera, fileUpFac, $cordovaImagePicker, Storage) {
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

  })

  .controller('vehListCtr', function ($scope, $ionicLoading, $state, Storage, $ionicActionSheet) {
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

  })
  .controller('myCarListCtr', function ($scope, $ionicLoading, carFac, $state, Storage, $ionicActionSheet) {
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


  })

  .controller('carInfoCtr', function ($scope, Storage, insureFac, $state) {
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

  })
  .controller('insureCtr', function ($scope, Storage, carFac, $state) {
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
  })
  .controller('searchCarCtr', function ($scope, insureFac, $ionicPopup, $state, $ionicLoading, storageUserFac) {
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
  })

  .controller('toInsureCtr', function ($scope, $state, $ionicLoading) {

  })
  .controller('helpButtonCtr', function ($scope, baiduMap, $state, $ionicLoading) {
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
  })

  .controller('testCtr', function (testFac, $scope, $ionicPopup, $state, Storage, $ionicLoading) {
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
  })




    .controller('editOrderCtr', function (common,$scope, $stateParams, $ionicPopup, $state, helpFac, $ionicLoading, baiduMap, Storage) {
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

  })

  .controller('maintainOrderList', function (orderFac, $scope, $ionicPopup, $state, $ionicLoading, $ionicActionSheet) {
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

  })

  .controller('illegalSearchCtr', function (common, $scope, $ionicPopup, $state, $ionicLoading, $ionicActionSheet) {
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
  })
  .controller('trialMapCtr', function ($scope, $ionicPopup, $state, $ionicLoading) {
    // 定义高度
    document.getElementById('allmap').style.height = $(document).height() - 100 + "px";
    var map = new BMap.Map("allmap");
    map.centerAndZoom("成都", 11);

  })
  .controller('createTrialCtr', function ($scope, $ionicPopup, $state, $ionicLoading) {

  })
  .controller('createMaintainCtr', function ($scope, $ionicPopup, $state, $ionicLoading) {

  })
  .controller('createHelpCtr', function ($scope, $ionicPopup, $state, $ionicLoading) {

  })

  .controller('trialBackCtr', function ($scope, $ionicPopup, $state, $ionicLoading) {

  })
  .controller('serviceListCtr', function ($scope, userFac, CarIn, $ionicPopup, $state, $ionicLoading, sendCodeFac) {

  })
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
  .controller('urlEchartsCtrl', function ($scope, $sce) {
    document.getElementById('myechart').height = $(document).height() - 50 + "px";

    $scope.targetUrl = $sce.trustAsResourceUrl('http://www.baidu.com/'); //$sce.trustAsResourceUrl("http://192.168.2.132/eqs/Echarts/ECharts?code=DDD");

  })
  .controller('echartCtrl', function ($scope) {
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
  });


