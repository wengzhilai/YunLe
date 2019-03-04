//index.js
//获取应用实例
var app = getApp()
var AppGlobal = require('../../utils/AppGlobal.js')
var toPost = require('../../utils/ToPost.js')

Page({
  data: {
    motto: 'Hello World',
    userInfo: {}
  },

  //事件处理函数
  bindViewTap: function () {
    wx.navigateTo({
      url: '../logs/logs'
    })
  },
  GoHelpEdit: function (e) {
    console.log(e.currentTarget.dataset.tt)
    wx.navigateTo({
      url: '../help/edit/edit?type=' + e.currentTarget.dataset.tt
    })
  },
  onLoad: function () {
    var that = this
    var user = app.getUser();
    if (user == null || user.ID == null) {
      wx.navigateTo({
        url: '../user/login/login'
      })
    }
    else {
      toPost.Post("ClientSingle", user.ID,function(currMsg) {
        if (currMsg.IsError) {
          wx.navigateTo({
        url: '../user/login/login'
      })
        } else {
          app.setUser(currMsg);
        }
      });
    }
  }
})
