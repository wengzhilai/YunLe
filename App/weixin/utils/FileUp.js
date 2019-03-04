function upImg(tmpScope, key, callback) {
    var user = getApp().getUser();
    wx.chooseImage({
        success: function (res) {
            var tempFilePaths = res.tempFilePaths
            console.log(getApp().data.servsers)
            wx.uploadFile({
                url: getApp().data.servsers + 'FileUp',
                filePath: tempFilePaths[0],
                name: 'file',
                formData: {
                    'userId': user.ID,
                    'authToken': user.authToken
                },
                success: function (res) {
                    console.log(res)
                    
                    var data =JSON.parse(res.data)
                    console.log(data)
                    console.log(data.IsError)
                    if (data.IsError) {
                        console.log(1)
                        wx.showModal({
                            title: '提示',
                            content: data.Message,
                            success: function (res) {
                                if (res.confirm) {
                                    console.log('用户点击确定')
                                }
                            }
                        })
                    }
                    else {
                        if (callback) {
                            callback(res.data)
                        }
                    }
                    //do something
                }
            })
        }
    })
}


module.exports = {
    upImg: upImg
}
