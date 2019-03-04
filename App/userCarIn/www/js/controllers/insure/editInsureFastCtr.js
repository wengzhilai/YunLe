/**
 * Created by wengzhilai on 2016/8/7.
 */
mainController.controller('editInsureFastCtr', function ($timeout, $scope, toPost, storageUserFac, common, fileUpFac, $ionicPopup, $ionicLoading, Storage, carFac, $cordovaCamera, $cordovaImagePicker, $stateParams, fileUpFac, insureFac, $state, $ionicActionSheet) {

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

})
