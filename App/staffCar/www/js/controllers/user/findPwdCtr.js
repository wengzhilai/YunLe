/**
 * Created by wengzhilai on 2016/9/18.
 */
mainController.controller('findPwdCtr', function (common,$scope, $ionicLoading, sendCodeFac, toPost, $state) {
    $scope.findPwd = {
        bean: {
            userId: 0,
            authToken: '',
            id: 0,
            para: [{
                K: 'VerifyCode',
                V: '',
                T: 'string'
            }, {
                K: 'LoginName',
                V: '',
                T: 'string'
            }, {
                K: 'NewPwd',
                V: '',
                T: 'string'
            }]
        },
        onSubmit: function () {
            console.log(this.bean);
            toPost.Post("ResetPassword",this.bean,this.onSubmitBack)
            userFac.findPwd(this.bean);
        },
        onSubmitBack: function (currMsg) {
            console.log(currMsg);
            if (currMsg.IsError == true) {
                $ionicLoading.show({
                    noBackdrop: true,
                    template: currMsg.Message,
                    duration: 2000
                });
            } else {
              common.hint("重设密码成功");
                $state.go('login', {reload: true});  //路由跳转
            }
        }
    };

    $scope.SendCode = function () {
        $("#sendCode").text('发送中...');
        toPost.Post("SendCode",{phone: $scope.findPwd.bean.para[1].V},$scope.SendCodeBack)
    };
    $scope.SendCodeBack = function (currMsg) {
        console.log(currMsg);
        if (currMsg.IsError == true) {
            $ionicLoading.show({
                noBackdrop: true,
                template: currMsg.Message,
                duration: 2000
            });
        } else {
            $("#sendCode").text('获取成功');
        }
    };

})
