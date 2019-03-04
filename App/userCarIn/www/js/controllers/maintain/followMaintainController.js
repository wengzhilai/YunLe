/**
 * Created by Administrator on 2016/8/21.
 */
mainController.controller('followMaintainOrderCtr', function ($scope, toPost, $ionicLoading, $stateParams, $state, $ionicActionSheet) {
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

});
