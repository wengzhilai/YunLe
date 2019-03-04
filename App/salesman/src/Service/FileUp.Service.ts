/**
 * Created by wengzhilai on 2017/1/14.
 */
// import {AlertController, LoadingController} from 'ionic-angular';
import {Injectable} from '@angular/core';
import {CarIn} from "../Classes/CarIn";
import {AppGlobal} from "../Classes/AppGlobal";
import {ActionSheetController, AlertController} from 'ionic-angular';
import {ToPostService} from "./ToPost.Service";
import {CommonService} from "./Common.Service";
import { ImagePicker } from '@ionic-native/image-picker';
import { Transfer, TransferObject } from '@ionic-native/transfer';
import { Camera, CameraOptions } from '@ionic-native/camera';
import { PhotoViewer } from '@ionic-native/photo-viewer';
import {Platform} from 'ionic-angular';
import {DateToStringPipe} from "../pipe/DateToString";

declare var wx;

@Injectable()
export class FileUpService {
  private user:any=AppGlobal.getUser();
  private thisKey = null;
  private retunBack = null;
  private fileId = {ID: 0};
  public thisScope;
  private nowSheet;
  public cordova:any;

  constructor(
    private camera: Camera,
    private imagePicker: ImagePicker,
    private transfer: Transfer,
    private photoViewer: PhotoViewer,
    public actionSheetCtrl:ActionSheetController,
              public commonService:CommonService,
              public alertCtrl:AlertController,
              public plt:Platform,
              public toPostService:ToPostService) {

  }


  DelFileCallBack() {
    this.fileId = {ID: 0};
    if (this.retunBack) {
      this.retunBack(this.thisKey,null,null);
    }
  }

  WeiXinCallBack(fileEnt) {
    this.fileId = fileEnt.ID;
    if (this.retunBack) {
      this.retunBack(this.thisKey, fileEnt.URL, fileEnt.ID);
    }
    this.nowSheet();
  }

  upImg(tmpScope, key, callback) {
    console.log('参数')
    this.thisKey = key;
    this.retunBack = callback;
    this.thisScope = tmpScope
    this.nowSheet = this.actionSheetCtrl.create({
      title: '选择图片',
      cssClass: 'action-sheets-basic-page',
      buttons: [
        {
          text: '相机',
          icon: 'camera',
          handler: () => {
            if (navigator.userAgent.toLowerCase().indexOf('micromessenger') > -1) {
              var sourceType = ['camera']
              wx.chooseImage({
                count: 1,
                needResult: 1,
                sizeType: ['original', 'compressed'], // 可以指定是原图还是压缩图，默认二者都有
                sourceType: sourceType, // 可以指定来源是相册还是相机，默认二者都有
                success: (data)=> {
                  wx.uploadImage({
                    localId: data.localIds[0], // 需要上传的图片的本地ID，由chooseImage接口获得
                    isShowProgressTips: 1, // 默认为1，显示进度提示
                    success: (res)=> {
                      var postBean = {
                        authToken: AppGlobal.getUserAuthToken(),
                        para: [{"K": "mediaId", "V": res.serverId}]
                      };
                      this.toPostService.Post("WeiXinFileUp", postBean, (ent)=>{
                        this.WeiXinCallBack(ent);
                      });
                    },
                    fail: (error)=> {
                      alert(JSON.stringify(error));
                    }
                  });
                },
                fail: (res)=> {
                  alert(JSON.stringify(res));
                }

              });
              return;
            }
            else {
              const options: CameraOptions = {
                destinationType: this.camera.DestinationType.DATA_URL,
                encodingType: this.camera.EncodingType.JPEG,
                mediaType: this.camera.MediaType.PICTURE,
                quality: CarIn.quality,
                sourceType: this.camera.PictureSourceType.CAMERA,
                allowEdit: CarIn.isAllowEdit,
                targetWidth: CarIn.imgWidth,
                targetHeight: CarIn.imgHeight,
                saveToPhotoAlbum: CarIn.isSaveToPhotoAlbum,
                correctOrientation: CarIn.isCorrectOrientation
              }


              this.camera.getPicture(options).then((imageData) => {
                this.upLoad(imageData);
              }, (err)=> {
                alert('选择相机错误')
              });
            }
          }
        },
        {
          text: '图库',
          icon: 'images',
          handler: () => {
            if (navigator.userAgent.toLowerCase().indexOf('micromessenger') > -1) {

              var sourceType = ['album']
              wx.chooseImage({
                count: 1,
                needResult: 1,
                sizeType: ['original', 'compressed'], // 可以指定是原图还是压缩图，默认二者都有
                sourceType: sourceType, // 可以指定来源是相册还是相机，默认二者都有
                success: (data)=> {
                  wx.uploadImage({
                    localId: data.localIds[0], // 需要上传的图片的本地ID，由chooseImage接口获得
                    isShowProgressTips: 1, // 默认为1，显示进度提示
                    success: (res)=>{
                      // alert(JSON.stringify(res));
                      AppGlobal.LoadUser((user)=> {
                        this.user = user;
                        if(this.user==null || this.user.authToken==null){
                          alert('请重新登录');
                          return;
                        }
                        var postBean = {
                          authToken:this.user.authToken,
                          para: [{"K": "mediaId", "V":  res.serverId}]
                        };
                        // alert(JSON.stringify(postBean));
                        this.toPostService.Post("WeiXinFileUp", postBean, (fileEnt)=>{
                          // alert('微信上传成功,调用回调');
                          this.WeiXinCallBack(fileEnt);
                          // alert('回调成功');
                        });
                      })
                    },
                    fail: (error)=> {
                      alert(JSON.stringify(error));
                    }
                  });
                },
                fail: (res)=> {
                  alert(JSON.stringify(res));
                }

              });
              return;
            }
            else {
              let options = {
                maximumImagesCount: 1,
                imgWidth: 800,
                imgHeight: 800,
                quality: 50,
                title:'选择图片'
              }
              this.imagePicker.getPictures(options).then((results) => {
                for (var i = 0; i < results.length; i++) {
                  this.upLoad(results[i]);
                }
              }, (err) => {
                this.commonService.hint('选择图库错误'+err)
              });
            }
          }
        },
        {
          text: '资料库',
          icon: "cloud-download",
          handler: () => {
            this.toPostService.ListFun("FileList", {}).then((ent) => {
              this.GetFileListBack(ent);
            });
          }
        },
        {
          text: '删除',
          icon: "trash",
          handler: () => {
            this.retunBack(this.thisKey, null, null);
          }
        }
      ]
    });
    this.nowSheet.present();
  }

  upLoad(fileUrl) {
    var options:any;
    var fileUpUrl = CarIn.api + "FileUp?userId=" + AppGlobal.getUser().ID + "&authToken=" + AppGlobal.getUserAuthToken();
    options = {}
    this.commonService.showLoading('上传中...');
    const fileTransfer: TransferObject = this.transfer.create();


    fileTransfer.upload(fileUrl, fileUpUrl, options)
      .then((data:any) => {
        this.commonService.hideLoading();
        let fileJson = <any>JSON.parse(data.response);
        console.log("返回结果：");
        console.log(fileJson);
        if (data.IsError) {
          this.commonService.hint(fileJson)
        } else {
          console.log("回调方法：" + this.retunBack);
          this.retunBack(this.thisKey, fileJson.URL, fileJson.ID);
          this.commonService.hideLoading();
        }
      }, (err) => {
        this.commonService.hideLoading();
        this.commonService.hint(JSON.stringify(err),'上传错误');
        // error
      })

  }

  GetFileListBack(currMsg) {
    if (currMsg.IsError) {
      return;
    }
    this.thisScope.myPopup = this.alertCtrl.create({
      title: '资料库',
      buttons: [
        {
          text: '取消',
          role: 'cancel',
        }, {
          text: '确定',
          handler: data => {
            console.log('Radio data:', data);
            var valueArr = data.split('|')
            this.retunBack(this.thisKey, valueArr[1], valueArr[0]);
          }
        }
      ]
    });
    for (var i = 0; i < currMsg.data.length; i++) {
      this.thisScope.myPopup.addInput({
        type: 'radio',
        label: new DateToStringPipe().transform(currMsg.data[i].UPLOAD_TIME, "yyyy-MM-dd hh:ss") + '(' + currMsg.data[i].NAME + ')',
        value: currMsg.data[i].ID + "|" + currMsg.data[i].URL,
      })
    }

    this.thisScope.myPopup.present()
  }

}
