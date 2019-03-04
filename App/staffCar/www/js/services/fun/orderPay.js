/**
 * Created by wengzhilai on 2016/9/20.
 */
mainService.factory('orderPay', function ($rootScope, storageUserFac, Storage, toPost, common) {
    var posOptions = {timeout: 10000, enableHighAccuracy: false},
        msg = {},
        reLocation = {},
        storageKey = 'user',
        user = Storage.get(storageKey);

    return {
        //支付
        pay: function (orderId,callback) {
            //是微信
            if(navigator.userAgent.toLowerCase().indexOf('micromessenger')>-1) {
                var postBean = {
                    userId: 0,
                    authToken: storageUserFac.getUserAuthToken(),
                    para: [
                        {"K": "order_no", "V": orderId}
                    ],
                    entity: {
                        STATUS: '完成'
                    }
                };
                toPost.Post("PayWeixinSign", postBean, function(ent)
                {
                  if (ent.IsError) {
                    callback(ent);
                    return;
                  }
                  wx.chooseWXPay({
                    timestamp: ent.timeStamp, // 支付签名时间戳，注意微信jssdk中的所有使用timestamp字段均为小写。但最新版的支付后台生成签名使用的timeStamp字段名需大写其中的S字符
                    nonceStr: ent.nonceStr, // 支付签名随机串，不长于 32 位
                    package: ent.packageValue, // 统一支付接口返回的prepay_id参数值，提交格式如：prepay_id=***）
                    signType: ent.signType, // 签名方式，默认为'SHA1'，使用新版支付需传入'MD5'
                    paySign: ent.sign, // 支付签名
                    success: function (res) {
                      callback({IsError:false,Message:'支付成功'})
                    },
                    fail:function (res) {
                      callback({IsError:true,Message:'支付失败:'+JSON.stringify(res)})
                    }
                  });
                });
            }
            else
            {
              callback({IsError:true,Message:"目前只支持微信支付"})
            }
        }
    }

})
