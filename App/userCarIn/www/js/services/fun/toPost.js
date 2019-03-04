/**
 * Created by Administrator on 2016/8/19.
 */
mainService.factory('toPost', function (CarIn, storageUserFac, common,$state) {
  return {
    list: function (apiName, postBean, callback) {
      common.showLoading();
      postBean.authToken = storageUserFac.getUserAuthToken();
      postBean.userId = 0;
      if (postBean.pageSize == null || postBean.pageSize == 0) {
        postBean.pageSize = CarIn.pageSize;
      }
      if (postBean.currentPage == null || postBean.currentPage == 0) {
        postBean.currentPage = 1;
      }
      console.log("请求[" + apiName + "]列表参数：");
      console.log(postBean);
      return $.post(CarIn.api + apiName, postBean,
        //回调函数
        function (response) {
          console.log("返回结果：");
          console.log(response);
          if (response.IsError) {
            if(response.Message=='登录超时')
            {
              $state.go('login', {reload: true});  //路由跳转
            }
            common.showError(response);
          } else {
            common.hideLoading();
          }
          if (callback) {
            console.log("回调方法：" + callback);
            callback(response);
          }
        },
        //返回类型
        "json").error(function (err) {
        console.log(err);
        common.showError({IsError: true, Message: '获取列表失败'});
      });
    },
    saveOrUpdate: function (apiName, bean, callback) {
      var saveKeyStr = common.getBeanNameStr(bean);
      if (saveKeyStr == "") {
        alert("保存参数saveKeys不能为空");
        return;
      }
      common.showLoading();
      var postBean = {
        authToken: storageUserFac.getUserAuthToken(),
        saveKeys: saveKeyStr,
        entity: bean
      };
      console.log("请求[" + apiName + "]参数：");
      console.log(postBean);
      return $.post(CarIn.api + apiName, postBean,
        //回调函数
        function (response) {
          common.hideLoading();
          console.log("返回结果：");
          console.log(response);
          if (response.IsError) {
            if (response.Message == '登录超时') {
              $state.go('login', {reload: true});  //路由跳转
            }
            common.showError(response);
          }
          if (callback) {
            console.log("回调方法：" + callback);
            callback(response);
          }
        },
        //返回类型
        "json").error(function (err) {
        console.log("错误信息：");
        console.log(err);
        common.showError({IsError: true, Message: '更新数据失败【' + apiName + '】：错误代码('+err.status+')'});
      });
    },
    Post: function (apiName, postBean, callback) {
      common.showLoading();
      console.log("请求[" + apiName + "]参数：");
      console.log(postBean);
      return $.post(CarIn.api + apiName, postBean,
        //回调函数
        function (response) {
          common.hideLoading();
          console.log("返回结果：");
          console.log(response);
          if (response.IsError) {
            if(response.Message=='登录超时')
            {
              $state.go('login', {reload: true});  //路由跳转
            }
            common.showError(response);
          }
          if (callback) {
            console.log("回调方法：" + callback);
            callback(response);
          }
        },
        //返回类型
        "json").error(function (err) {
        console.log("错误信息：");
        console.log(err);
        var errBean={IsError: true, Message: '提交数据失败【' + apiName + '】'};
        common.showError(errBean);
        if (callback) {
          console.log("回调方法：" + callback);
          callback(errBean);
        }
      });
    },
    single: function (apiName, id, callback) {
      if (id == null && id == 0) {
        alert("查询id不能为空");
        return;
      }
      common.showLoading();
      var postBean = {
        userId: 0,
        authToken: storageUserFac.getUserAuthToken(),
        id: id
      };
      console.log("请求[" + apiName + "]参数：");
      console.log(postBean);
      return $.post(CarIn.api + apiName, postBean,
        //回调函数
        function (response) {
          common.hideLoading();
          console.log("返回结果：");
          console.log(response);
          if (response.IsError) {
            if(response.Message=='登录超时')
            {
              $state.go('login', {reload: true});  //路由跳转
            }
            common.showError(response);
          }
          if (callback) {
            console.log("回调方法：" + callback);
            callback(response);
          }
        },
        //返回类型
        "json").error(function (err) {
        console.log(err);
        common.showError({IsError: true, Message: '查询单条记录失败'});
      });
    }
  }
});
