/**
 * Created by wengzhilai on 2016/8/12.
 */
mainController.controller('followMaintainOrderCtr', function (common,$scope, orderFac, $ionicLoading, $stateParams, $state, $ionicActionSheet) {
    if ($stateParams.id) {
        orderFac.rescueSingle($stateParams.id);
    } else {
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
    $scope.$on('order.rescueSingleUpdate', function () {
        var currMsg = orderFac.getCurrentMes();
        if (currMsg.IsError) {
            $ionicLoading.show({
                noBackdrop: true,
                template: currMsg.Message,
                duration: 2000
            });
        } else {
            $scope.followOrder.bean = currMsg;
        }

    });

})
