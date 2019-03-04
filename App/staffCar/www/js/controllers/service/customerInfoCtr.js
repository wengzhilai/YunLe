/**
 * Created by wengzhilai on 2016/10/12.
 */
mainController.controller('customerInfoCtr', function (common,Storage, toPost, fileUpFac, storageUserFac, CarIn, $scope, $ionicLoading, $state, $stateParams, $ionicPopup,$window) {
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


})
