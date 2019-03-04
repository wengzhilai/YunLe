
 function getUser(){
    var LSuser = wx.getStorageSync('LSuser') || null
    console.log(LSuser)
    return LSuser;
  }

  module.exports = {
  getUser: getUser
}