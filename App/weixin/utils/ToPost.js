var api = 'http://139.129.194.140/YL/WebApi/';
function Post(apiName, postBean, callback) {
    console.log('请求['+apiName+']数据'+postBean)
    wx.request({
        url: api+apiName, //仅为示例，并非真实的接口地址
        data: postBean,
        header: {
            'content-type': 'application/json'
        },
        success: function (res) {
            console.log(res.data)
            if(callback){
                callback(res.data)
            }
        }
    })
}

module.exports = {
  Post: Post
}