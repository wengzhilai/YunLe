/**
 * Created by Administrator on 2016/8/24.
 */
mainService.factory('fileUpDel', function (CarIn, $cordovaFileTransfer, storageUserFac, common) {
    var options = {},
        fileDelUrl = CarIn.api + "FileDel",
        fileUpUrl = CarIn.api + "FileUp?userId=" + storageUserFac.getUserId() + "&authToken=" + storageUserFac.getUserAuthToken();
    return {
        fileDel: function (id, callback) {
            common.showLoading();
            var postBean = {
                userId: 0,
                authToken: storageUserFac.getUserAuthToken(),
                id: id
            };
            console.log("请求["+fileDelUrl+"]参数：");
            console.log(postBean);
            return $.post(fileDelUrl, postBean,
                //回调函数
                function (response) {
                    console.log("返回结果：");
                    console.log(response);
                    if (response.IsError) {
                        common.showError(response);
                    } else {
                        console.log("回调方法：" + callback);
                        callback(response);
                        common.hideLoading();
                    }
                },
                //返回类型
                "json").error(function (err) {
                    console.log(err);
                    common.showError({IsError: true, Message: '删除文件失败'});
                });
        },
        saveFile: function (path, callback) {
            common.showLoading();
            return $cordovaFileTransfer.upload(fileUpUrl, path, options)
                .then(function (result) {
                    //alert(path);
                    if (result.IsError) {
                        common.showError(result);
                    } else {
                        callback(result);
                        common.hideLoading();
                    }
                }, function (err) {

                    common.showError({IsError: true, Message: '文件上传失败'});
                }, function (progress) {
                    // constant progress updates
                });
        }
    }
})
