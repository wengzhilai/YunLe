import { Component, ViewChild } from '@angular/core';
import { Platform, AlertController, LoadingController } from 'ionic-angular';
import { Storage } from "@ionic/storage"
import { Transfer, TransferObject } from '@ionic-native/transfer';
import { AppVersion } from '@ionic-native/app-version';
import { SplashScreen } from '@ionic-native/splash-screen';
import { StatusBar } from '@ionic-native/status-bar';
import { FileOpener } from '@ionic-native/file-opener';
import { Location } from '@angular/common';
import { HomePage } from '../pages/home/home';
import { ToPostService } from "../Service/ToPost.Service";
import { CommonService } from "../Service/Common.Service";
import { UserLoginPage } from "../pages/user/login/login";
import { AppGlobal } from "../Classes/AppGlobal";

declare var wx;
declare var cordova;

@Component({
  templateUrl: 'app.html',
  providers: [CommonService, ToPostService, Location]
})
export class MyApp {
  @ViewChild('nav') navCtrl;
  rootPage = HomePage;
  location: Location;
  constructor(
    private platform: Platform, 
    private statusBar: StatusBar, 
    private splashScreen: SplashScreen,
    private transfer: Transfer,
    private appVersion: AppVersion,
    private fileOpener: FileOpener,
    public commonService: CommonService,
    private alertCtrl: AlertController,
    public toPostService: ToPostService,
    public loadingCtrl: LoadingController,
    public storage: Storage

  ) {
    platform.ready().then(() => {
      statusBar.styleDefault();
      splashScreen.hide();
    });
    AppGlobal.SetStorage(storage);
    this.presentLoadingDefault();
  }
  presentLoadingDefault() {
    if (navigator.userAgent.toLowerCase().indexOf('micromessenger') > -1) {
      var openid = this.commonService.getQueryString("openid");
      var code = this.commonService.getQueryString("code");
      var state = this.commonService.getQueryString("state");
      if (openid == null && code == null && state == null) {

      }
      else if (openid != null) {
        var postBean = {
          "openid": openid
        }
        this.storage.set("openid", openid);
        this.toPostService.Post("ClientLogin", postBean, (currMsg) => {
          if (currMsg.IsError) {
            this.navCtrl.push(UserLoginPage, {});
          }
          else {
            AppGlobal.setUser(currMsg)
            this.toPostService.Post("WeiXinJSSDKSign", { "authToken": location.href.split('#')[0] }, (reBean) => {
              // alert('认证成功');
              // alert(JSON.stringify(reBean));
              this.RegWenXin(reBean);
            });
          }
        })
        return;
      }
      else if (code != null && state != null) {
      }
      console.log("是微信");
      return;
    }
    else if (this.platform.is('android')) {
      //升级系统
      this.appVersion.getVersionCode().then((version) => {
        var nowversionNum = version
        //获取服务器上版本
        var postBean = {
          userId: 0,
          id: nowversionNum,
          para: [
            { "K": "type", "V": "client" }
          ]
        };
        this.toPostService.Post("CheckUpdate", postBean, (ent) => {
          if (ent.IsError) {
            alert("获取版本错误：" + ent.Message);
          }
          else {
            if (nowversionNum != ent.ID) {

              let alert = this.alertCtrl.create({
                title: '版本升级',
                message: ent.REMARK,
                buttons: [
                  {
                    text: '取消',
                    role: 'cancel',
                    handler: () => {
                    }
                  },
                  {
                    text: '升级',
                    handler: () => {
                      var url = ent.UPDATE_URL;
                      var apkName = url.substr(url.lastIndexOf("/") + 1);
                      var targetPath = cordova.file.externalRootDirectory + apkName;
                     const fileTransfer: TransferObject = this.transfer.create();
                      let uploading = this.loadingCtrl.create({
                        content: "安装包正在下载...",
                        dismissOnPageChange: false
                      });
                      uploading.present();

                      fileTransfer.onProgress((event) => {
                        var downloadProgress = (event.loaded / event.total) * 100;
                        uploading.setContent("已经下载：" + Math.floor(downloadProgress) + "%");
                        if (downloadProgress > 99) {
                          uploading.dismissAll();
                        }
                      });

                      fileTransfer.download(url, targetPath, true).then((result) => {
                        uploading.dismissAll();
                        this.fileOpener.open(targetPath, 'application/vnd.android.package-archive'
                        ).then(function () {
                        }, function (err) {
                          this.commonService.hint('安装失败:' + err);
                        });
                      }, (err) => {
                        var errStr = JSON.stringify(err);
                        this.commonService.hint('下载失败:' + errStr);
                      });

                      
                    }
                  }
                ]
              });
              alert.present();
            }
          }
        })


      });
    }
  }

  RegWenXin(reBean) {
    try {
      wx.config({
        debug: false, // 开启调试模式,调用的所有api的返回值会在客户端alert出来，若要查看传入的参数，可以在pc端打开，参数信息会通过log打出，仅在pc端时才会打印。
        appId: reBean.appId, // 必填，公众号的唯一标识
        timestamp: reBean.timestamp, // 必填，生成签名的时间戳
        nonceStr: reBean.nonceStr, // 必填，生成签名的随机串
        signature: reBean.signature,// 必填，签名，见附录1
        jsApiList: [
          // 所有要调用的 API 都要加到这个列表中
          'chooseImage',
          'previewImage',
          'uploadImage',
          'downloadImage'
        ] // 必填，需要使用的JS接口列表，所有JS接口列表见附录2
      });
      wx.ready(() => {
        // this.commonService.showLongToast("微信注册成功")
      });
      wx.error((res) => {
        this.commonService.showLongToast("微信注册失败：" + JSON.stringify(res))
      });
    }
    catch (e) {
      //this.commonService.showLongToast("微信加载失败：" + e)
      setTimeout(() => {
        this.RegWenXin(reBean);
      }, 5000)
    }
  }
}
