/**
 * Created by wengzhilai on 2016/11/14.
 */
mainController.controller('expireInsureCtr', function (common,$scope,toPost, fileUpFac,$ionicPopup, $ionicLoading, Storage,$cordovaCamera,$cordovaImagePicker, $stateParams,fileUpFac, insureFac, $state, $ionicActionSheet) {
  console.log($stateParams)
  if($stateParams.expireInsureList==null)
  {
    common.hint('参数有误');
  }
  else {
    $scope.lists=$stateParams.expireInsureList;
  }
});
