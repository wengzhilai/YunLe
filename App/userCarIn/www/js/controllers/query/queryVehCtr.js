/**
 * Created by wengzhilai on 2016/10/7.
 */
mainController.controller('queryVehCtr', function (common,$scope,toPost,storageUserFac, $state, CarIn, $ionicLoading, Storage, queryVehFac) {
    $scope.queryVeh = {
        Peccancy: {
            IdNo: '',
            Code: '',
            PicCode: '',
            Images: ''
        },
        SendCode: function () {
            if (this.Peccancy.IdNo == null || this.Peccancy.IdNo == '' || this.Peccancy.IdNo.length != 18) {
              common.hint('身份证号码有误，请确认后再试');
                return;
            } else if (this.Peccancy.Code == null || this.Peccancy.Code == '' || this.Peccancy.Code.length != 6) {
                if (!(this.Peccancy.PicCode == null || this.Peccancy.PicCode == '')) {

                    var postBean = {
                        userId: 0,
                        authToken: storageUserFac.getUserAuthToken(),
                        para: [
                            {K: "PicCode", V: this.Peccancy.PicCode},
                            {K: "IdNo", V: this.Peccancy.IdNo}

                        ]
                    };
                    toPost.Post("ClientPeccancy2",postBean, $scope.clientPeccancy2CallBack)
                } else {
                    var postBean = {
                        userId: 0,
                        authToken: storageUserFac.getUserAuthToken(),
                        para: [
                            {K: "IdNo", V: this.Peccancy.IdNo}
                        ]
                    };
                    toPost.Post("ClientPeccancy",postBean,$scope.clientPeccancyCallBack)
                }
            }

        },
        onSubmit: function () {
            var postBean = {
                userId: "",
                authToken: storageUserFac.getUserAuthToken(),
                para: [
                    {K: "Code", V: this.Peccancy.Code}
                ]
            };
            toPost.Post("ClientPeccancy1",postBean,$scope.clientPeccancy1CallBack)

            queryVehFac.clientPeccancy1(this.Peccancy.Code);
        }
    };

    $scope.clientPeccancy1CallBack= function (currMsg) {
        if (currMsg.data != null) {
            var storageKey = 'veh';
            Storage.set(storageKey, currMsg.data);
            $state.go('vehList', {reload: true});  //路由跳转
        } else {
          common.hint("没有违章记录");
        }
    };

    $scope.clientPeccancy2CallBack= function (currMsg) {
      common.hint('短信发送成功');
    };
    $scope.clientPeccancyCallBack= function (currMsg) {
        if (currMsg.Message == '验证码过期') {
            $scope.queryVeh.Peccancy.Images = currMsg.Params;
        }
        else {
            $ionicLoading.show({
                noBackdrop: true,
                template: currMsg.Message,
                duration: 2000
            });
        }
    };
})
