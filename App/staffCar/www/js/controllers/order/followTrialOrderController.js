/**
 * Created by wengzhilai on 2016/8/13.
 */
mainController.controller('followTrialOrderCtr', function ($scope, orderFac, $ionicLoading, $stateParams, $state, $ionicActionSheet) {
    if ($stateParams.id) {
        orderFac.OrderSingle($stateParams.id);
    } else {
      common.hint("无订单号无法跟踪");
    }
    $scope.followOrder = {
        bean: {},
        showTrialSingle: function () {
            var temp = $("#trialSingleDiv").is(":hidden");

            if (temp) {
                $("#trialSingleDiv").show();
            } else {
                $("#trialSingleDiv").hide();
            }

        }
    };
    $scope.$on('order.SingleUpdate', function () {
        var currMsg = orderFac.getCurrentMes();

        $scope.followOrder.bean = currMsg;

    });

})
