/**
 * Created by wengzhilai on 2016/8/7.
 */
mainController.controller('loginCtr', function ($scope, toPost, $state,common, CarIn,storageUserFac,Storage, $ionicLoading) {
    //Storage.set("loginName",'18180770313');
    $scope.login = {
        rememberPwd:(Storage.get("loginPwd")==null)?false:true,
        bean: {
            loginName: Storage.get("loginName"),
            password: Storage.get("loginPwd"),
            version: CarIn.version,
            type: '1',
            openid:common.getQueryString("openid")
        },
        user: {},
        reLoadJS: function () {
            location.reload();
        },
        submit: function () {
            //alert(document.loginForm.loginName.checkValidity()===false);
            if (document.loginForm.checkValidity() === false) {
                if (document.loginForm.loginName.checkValidity() === false) {
                  common.hint("登录名不能为空");
                } else if (document.loginForm.password.checkValidity() === false) {
                  common.hint("密码不能为空");
                }
            } else {
                Storage.set("loginName", $scope.login.bean.loginName);
                if($scope.login.rememberPwd)
                {
                    Storage.set("loginPwd", $scope.login.bean.password);
                }else
                {
                    Storage.remove("loginPwd");
                }
                if(navigator.userAgent.toLowerCase().indexOf('micromessenger')>-1) {
                    $scope.login.openid=common.getQueryString("openid");
                    if($scope.login.openid==null) {
                      common.hint(window.location.href);
                      common.hint("未获取获取到openid");
                      return;
                    }
                }
                toPost.Post("SalesmanLogin",this.bean,this.onSubmitBack)
            }
        },
        onSubmitBack: function (currMsg) {
            if (currMsg.IsError) {
                $ionicLoading.show({
                    noBackdrop: true,
                    template: currMsg.Message,
                    duration: 2000
                });
            } else {
                storageUserFac.setUser(currMsg);
                $state.go('home', {reload: true});  //路由跳转
            }
        }
    };
})
