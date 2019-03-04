/**
 * Created by wengzhilai on 2016/10/13.
 */
mainController.controller('followInsureCtr', function ($scope, common, orderPay, $cordovaFileTransfer, toPost, fileUpFac, $ionicModal, $stateParams, $state, $ionicLoading, storageUserFac) {
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
})
