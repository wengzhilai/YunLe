/**
 * Created by wengzhilai on 2016/8/16.
 */
mainService.factory('fileUpFac', function (CarIn, $ionicLoading, $timeout, $ionicPopup, $cordovaImagePicker, $ionicActionSheet, $cordovaFileTransfer, $cordovaCamera, $rootScope, storageUserFac, common, fileUpDel, toPost) {
  var msg = {},
    options = {},
    fileDelUrl = CarIn.api + "FileDel",
    url = CarIn.api + "FileUp?userId=" + storageUserFac.getUserId() + "&authToken=" + storageUserFac.getUserAuthToken();
  var thisImgEvent = null;
  var retunBack = null;
  var fileId = 0;
  var thisScope;
  var nowSheet;
  function fileUpFac_upCallback(result) {
    fileId = window.JSON.parse(result.response).ID;
    thisImgEvent.attr("data-id", fileId);
    if (retunBack) {
      retunBack(window.JSON.parse(result.response));
    }
    nowSheet();
  }

  function DelFileCallBack() {
    fileId = 0;
    thisImgEvent.attr("src",'');
    if (retunBack) {
      retunBack({ID:0,URL:null});
    }
  }

  function WeiXinCallBack(fileEnt) {
    fileId = fileEnt.ID;
    thisImgEvent.attr("data-id", fileId.ID);
    if (retunBack) {
      retunBack(fileEnt);
    }
    nowSheet();
  }

  return {
    upImg: function (obj, OutFileId, callback,tmpScope) {
      console.log('参数')
      console.log(OutFileId)
      thisImgEvent = obj;
      fileId = OutFileId;
      retunBack = callback;
      thisScope=tmpScope
      nowSheet=$ionicActionSheet.show({
        buttons: [
          {text: '<b>相机</b>'},
          {text: '<b>图库</b>'},
          {text: '<b>资料库</b>'}
        ],
        destructiveText: '删除',
        titleText: '选择图片来源',
        cancelText: '关闭',
        cancel: function () {
        },
        destructiveButtonClicked: function () {
          fileUpDel.fileDel(fileId, DelFileCallBack);
        },
        buttonClicked: function (index) {
          if(index==2) {
            toPost.list("FileList", {}, function (currMsg) {
              if (currMsg.IsError) {
                return;
              }
              thisScope.allFileList = currMsg.data;
              thisScope.checkImg=function (ent) {
                WeiXinCallBack(ent)
                thisScope.myPopup.close()
              }
              var templateStr="";
              templateStr+='<div class="item item-body col" style="text-align: center" ng-repeat="o in allFileList">';
              templateStr+='  <img name="driverPicImg" src="{{o.URL|imgUrl}}" height="100px" ng-click="checkImg(o)" width="100px" />';
              templateStr+='  <p>{{o.NAME}}</p>';
              templateStr+='</div>';
              templateStr+='';
              thisScope.myPopup = $ionicPopup.show({
                template: templateStr,
                title: '资料库',
                subTitle: '请选择您上传过的图片',
                scope: thisScope,
                buttons: [
                  {text: '取消',
                    type: 'button-positive',
                  }
                ]
              });
            })

            return;
          }

          if (navigator.userAgent.toLowerCase().indexOf('micromessenger') > -1) {
            var sourceType=['album', 'camera'];
            switch (index) {
              case 0:
                sourceType = ['camera']
                break;
              case 1:
                sourceType = ['album']
                break;
            }
            wx.chooseImage({
              count: 1,
              needResult: 1,
              sizeType: ['original', 'compressed'], // 可以指定是原图还是压缩图，默认二者都有
              sourceType: sourceType, // 可以指定来源是相册还是相机，默认二者都有
              success: function (data) {
                localIds = data.localIds[0]; // 返回选定照片的本地ID列表，localId可以作为img标签的src属性显示图片
                wx.uploadImage({
                  localId: localIds, // 需要上传的图片的本地ID，由chooseImage接口获得
                  isShowProgressTips: 1, // 默认为1，显示进度提示
                  success: function (res) {
                    mediaId = res.serverId; // 返回图片的服务器端ID
                    var postBean = {
                      authToken: storageUserFac.getUserAuthToken(),
                      para: [{"K": "mediaId", "V": mediaId}]
                    };
                    toPost.Post("WeiXinFileUp", postBean, WeiXinCallBack);

                  },
                  fail: function (error) {
                    picPath = '';
                    localIds = '';
                    alert(JSON.stringify(error));
                  }
                });
              },
              fail: function (res) {
                alert(JSON.stringify(res));
              }

            });
            return;
          }
          else {
            switch (index) {
              case 0:
                $cordovaCamera.getPicture(common.getCameraOption()).then(function (imageData) {
                  thisImgEvent.attr("src", "data:image/jpeg;base64," + imageData);
                  thisImgEvent.attr("data-id", "");
                  fileUpDel.saveFile("data:image/jpeg;base64," + imageData, fileUpFac_upCallback);
                }, function (err) {
                  alert('选择相机错误')
                  // error
                });
                return;
              case 1:
                $cordovaImagePicker.getPictures(common.getImageOption())
                  .then(function (results) {
                    for (var i = 0; i < results.length; i++) {
                      thisImgEvent.attr("src", results[i]);
                      thisImgEvent.attr("data-id", "");
                      fileUpDel.saveFile(results[i], fileUpFac_upCallback);
                    }
                  }, function (error) {
                    alert('选择图库错误')
                    // error getting photos
                  });
                break;
              default:
                break;
            }
          }
        }
      });
    },
    FullScreenImage: function (url, nowscope,title) {
      if(title=null)title="";
      if (url == null || url == '' || url == undefined || url == "img/noPic.png") {
        return ;
      }
      if (navigator.userAgent.toLowerCase().indexOf('micromessenger') > -1) {
        wx.previewImage({
          current: url, // 当前显示图片的http链接
          urls: [url] // 需要预览的图片http链接列表
        });
        return;
      }
      if (ionic.Platform.isAndroid() || ionic.Platform.isIOS()) {

        PhotoViewer.show(url, title ,{share:true});
        return;
        var imageName = url.substr(url.lastIndexOf("/") + 1);
        var targetPath='';
        if(ionic.Platform.isAndroid())
        {
          targetPath = cordova.file.externalRootDirectory + imageName;
        }
        else if(ionic.Platform.isIOS())
        {
          targetPath = cordova.file.documentsDirectory + imageName;
        }
        var trustHosts = true
        var options = {};
        $cordovaFileTransfer.download(url, targetPath, options, trustHosts).then(function (result) {
          window.FullScreenImage.showImageURL(targetPath);
        }, function (err) {
          var errStr= JSON.stringify(err);
          alert('打开失败:' + errStr);
        }, function (progress) {
          $timeout(function () {
            var downloadProgress = (progress.loaded / progress.total) * 100;
            $ionicLoading.show({
              template: "已经下载：" + Math.floor(downloadProgress) + "%"
            });
            if (downloadProgress > 99) {
              $ionicLoading.hide();
            }
          })
        });
      }
      else {

        nowscope.Url = url;
        console.log(nowscope.Url);
        nowscope.myPopupClose = function () {
          myPopup.close()
        }
        var myPopup = $ionicPopup.show({
          template: '<img ng-click="myPopupClose()" style="display: block; margin: 0px;" src="{{Url}}"/>',
          title: '',
          scope: nowscope
        });
      }
    }
  }

})
