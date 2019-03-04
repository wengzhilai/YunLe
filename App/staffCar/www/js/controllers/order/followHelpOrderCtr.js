/**
 * Created by wengzhilai on 2016/10/13.
 */
mainController.controller('followHelpOrderCtr', function ($scope, common, $cordovaFileTransfer,orderPay , toPost, fileUpFac, $ionicModal, $stateParams, $state, $ionicLoading, storageUserFac) {
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
})
