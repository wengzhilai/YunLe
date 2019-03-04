// pages/user/login.js

var toPost = require('../../../utils/ToPost.js')
Page({
  data: {
    loginName: '123',
    password: '222'
  },
  onLoad: function (options) {
    // 页面初始化 options为页面跳转所带来的参数
  },
  onReady: function () {

    // 页面渲染完成
  },
  onShow: function () {
    // 页面显示
  },
  onHide: function () {
    // 页面隐藏
  },
  onUnload: function () {
    // 页面关闭
  },
  formSubmit: function (e) {
    console.log(e.detail.value)
    var that = this;//把this对象复制到临时变量that
    toPost.Post("ClientLogin", e.detail.value, function (ent) {
      console.log(ent);
      if (ent.IsError) {
        wx.showModal({
          title: '提示',
          content: ent.Message,
          showCancel:false
        })

      }
      else {
        wx.setStorageSync('LSuser', ent);
        wx.navigateTo({
        url: '../../index/index'
      })
      }
    })
  }
})