/**
 * Created by wengzhilai on 2016/8/7.
 */
mainController.controller('editUserCtr', function (common,$scope, $ionicPopup, $state, toPost, storageUserFac, $cordovaCamera, $cordovaImagePicker, $ionicLoading, fileUpFac, $ionicActionSheet) {
    $scope.user = {
        bean:  storageUserFac.getUser(),
        currEnt: {},
        upImg: function (obj, OutFileId) {
            $scope.user.currEnt = $(obj.target);
            fileUpFac.upImg($scope.user.currEnt, OutFileId, $scope.user.upCallback,$scope);
        },
        upCallback: function (result) {
            var name = $scope.user.currEnt.attr("name");
            switch(name)
            {
                case "iconURL":
                    $scope.user.bean.ICON_FILES_ID = result.ID;
                    $scope.user.bean.iconURL = result.URL;
                    break;
                case "idNoUrl":
                    $scope.user.bean.ID_NO_PIC = result.ID;
                    $scope.user.bean.idNoUrl = result.URL;
                    break;
                case "driverPicUrl":
                    $scope.user.bean.DRIVER_PIC_ID = result.ID;
                    $scope.user.bean.driverPicUrl =result.URL;
                    break;
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
        save: function () {
            toPost.saveOrUpdate("SalesmanSave",this.bean,this.saveBack);
        },
        saveBack: function (currMsg) {
            if(!currMsg.IsError) {
                $ionicLoading.show({
                    noBackdrop: true,
                    template: "个人信息保存成功！",
                    duration: 2000
                });
                storageUserFac.setUser($scope.user.bean);
                $state.go('user', {reload: true});  //路由跳转
            }
            else
            {
                $ionicLoading.show({
                    noBackdrop: true,
                    template:currMsg.Message,
                    duration: 2000
                });
            }
        },
        updatePassword: function () {
            var myPopup = $ionicPopup.show({
                template: '<input type="password" ng-model="user.bean.password">',
                title: '请输入你要修改的新密码',
                scope: $scope,
                buttons: [
                    {text: '取消'},
                    {
                        text: '<b>保存</b>',
                        type: 'button-positive',
                        onTap: function (e) {
                            if (!$scope.user.bean.password) {
                              common.hint("输入新密码才可以保存");
                            } else {
                                var subBean = {
                                    authToken: storageUserFac.getUserAuthToken(),
                                    userId: 0,
                                    entity: $scope.user.bean.password
                                };
                                toPost.Post("UserEditPwd",subBean,$scope.user.updatePasswordBack);
                            }
                        }
                    }
                ]
            });
        },
        updatePasswordBack: function (currMsg){
            $ionicLoading.show({
                noBackdrop: true,
                template: "密码更新成功！",
                duration: 2000
            });
        }
    };
})
