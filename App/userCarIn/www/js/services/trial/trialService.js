/**
 * Created by Administrator on 2016/8/23.
 */
mainService.factory('trialFac', function (CarIn, toPost, storageUserFac) {
  var getTrialListUrl = CarIn.TrialList,
    trialSaveUrl = CarIn.TrialSave,
    getSingleTrialUrl = CarIn.TrialSingle;
  return {
    //获得代审车列表
    getTrialList: function (postBean, callback) {
      postBean.searchKey = [{K: 'ORDER_TYPE', V: '审车', T: '=='}, {
        K: 'CLIENT_ID', V: storageUserFac.getUserId(), T: '=='
      }];
      toPost.list(getTrialListUrl, postBean, callback);
    },
    //代审车更新
    TrialSave: function (bean, callback) {
      toPost.saveOrUpdate(trialSaveUrl, bean, callback);
    },
    //通过id获得代审车订单
    getSingleTrial: function (id, callback) {
      toPost.single(getSingleTrialUrl, id, callback);
    }
  }
});
