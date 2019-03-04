/**
 * Created by wengzhilai on 2016/8/7.
 */
mainController.controller('editInsureCtr', function ($scope,toPost, fileUpFac,$ionicPopup, $ionicLoading, Storage, carFac,$cordovaCamera,$cordovaImagePicker, $stateParams,fileUpFac, insureFac, $state, $ionicActionSheet) {


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

})
