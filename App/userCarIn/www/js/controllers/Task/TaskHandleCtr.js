/**
 * Created by wengzhilai on 2016/12/12.
 */
mainController.controller('TaskHandleCtr', function (Storage,$timeout, toPost, fileUpFac, storageUserFac, CarIn, $scope, $ionicLoading, $state, $stateParams, $ionicPopup) {
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
})
