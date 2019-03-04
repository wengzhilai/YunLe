// pages/user/login.js

var toPost = require('../../../utils/ToPost.js')
var fileUp = require('../../../utils/FileUp.js')
Page({
  data: {
    orderTypeArr: ['救援', '保养', '维修', '审车'],
    hitchTypeArr: ['爆胎', '接电', '抛锚', '交通事故', '其它'],

    bean: {
      ORDER_TYPE: '爆胎', REACH_TYPE: '送店', REACH_TIME: '2017-3-2',
      AllFiles: []
    }
  },
  onLoad: function (options) {
    // 页面初始化 options为页面跳转所带来的参数
    if(options.type!=null){

    wx.setNavigationBarTitle({
      title: options.type,
      success: function (res) {
        // success
      }
    })
    }
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

  },
  bindPickerChange: function (e) {
    console.log('picker发送选择改变，携带值为', e.target.dataset.range)
    if (e.target.dataset.range) {
      this.data.bean[e.target.dataset.bindstr] = e.target.dataset.range[e.detail.value];
    }
    else {
      this.data.bean[e.target.dataset.bindstr] = e.detail.value;
    }
    this.setData(this.data)
  },
  AddImg:function(){
    var indexNo = this.data.bean.AllFiles.length;
    
    this.data.bean.AllFiles[indexNo] = {"indexNo": this.data.bean.AllFiles.length};
    this.setData(this.data)
  },
  upImg(e){
    console.log(e)
    var indexNo=e.target.dataset.indexNo
    fileUp.upImg(this,'allFile_' + indexNo,function(Key, url, ID)     {

    })
    
  }
})