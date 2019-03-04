/**
 * Created by wengzhilai on 2016/8/12.
 */
/**
 * Created by wengzhilai on 2016/8/10.
 */

mainController.controller('garageInfoCtr', function ($scope,toPost, $ionicPopup, $ionicLoading, $cordovaImagePicker, $cordovaCamera, $stateParams, orderFac, $state, storageUserFac, $ionicActionSheet) {
  $scope.garage = {
    bean: {
    },
    currEnt: {}
  };
  if ($stateParams.id) {
    toPost.single("GarageSingle", $stateParams.id, function (response) {
      if (response.IsError) {
        common.hint(response.Message);
      } else {
        $scope.garage.bean=response;
      }
    });
  }

})
