import {NavController, AlertController, App} from 'ionic-angular';
import {Component}                  from '@angular/core';
import {CommonService} from "../../../Service/Common.Service";
import {AppGlobal} from "../../../Classes/AppGlobal";
import {CarIn} from "../../../Classes/CarIn";
import {FileUpService} from "../../../Service/FileUp.Service";
import {UserLoginPage} from "../login/login";
import {ToPostService} from "../../../Service/ToPost.Service";
import {UserEditPage} from "../edit/edit";
import { OrderListPage } from "../../order/list/list";

declare var wx;

@Component({
  selector: 'user-meuu',
  templateUrl: 'menu.html',
  providers: [CommonService, ToPostService, FileUpService]
})
export class UserMenuPage {
  public user = AppGlobal.getUser()

  constructor(public fileUpService:FileUpService,
              public navCtrl:NavController,
              public appCtrl:App,
              public commonService:CommonService,
              public toPostService:ToPostService,
              public alertCtrl:AlertController) {
  }
  toShare() {
    console.log(this.user)
    var salesman_id = this.user.SALESMAN_ID;
    var baseUrl = CarIn.baseUrl;
    var qrcode = baseUrl + '/File/QrCode/salesman_' + salesman_id + '.jpg';
    var qrcodeWeiXin = baseUrl + '/File/QrCode/salesman_weixin_' + salesman_id + '.jpg';
    if (navigator.userAgent.toLowerCase().indexOf('micromessenger') > -1) {
      wx.previewImage({
        current: qrcodeWeiXin, // 当前显示图片的http链接
        urls: [qrcode, qrcodeWeiXin] // 需要预览的图片http链接列表
      });
      return;
    }
    else {
      this.commonService.FullScreenImage(qrcodeWeiXin, '分享注册');
      //window.plugins.socialsharing.share("乐享", "subject", qrcodeWeiXin, qrcodeWeiXin);
    }
  }

  showAlert(msg) {
    this.commonService.hint(msg);
  }


  outLogin() {
    let confirm = this.alertCtrl.create({
      title: '确认注销',
      message: '是否退出登录?',
      buttons: [
        {
          text: '注销',
          handler: () => {
              this.toPostService.SingleFun("LoginOut", 0).then((ent) => {
                if (ent.IsError) {
                  this.commonService.hint(ent.Message);
                }
                AppGlobal.removeUser();
                this.appCtrl.getRootNav().push(UserLoginPage)
                });
          }
        },
        {
          text: '取消',
          handler: () => {
            console.log('Agree clicked');
          }
        }
      ]
    });
    confirm.present();
  };

  goHome()
  {
    this.navCtrl.parent.select(0);
  }
  goEdit()
  {
    this.navCtrl.push(UserEditPage);
  }
  goOrderList(type){
    this.navCtrl.push(OrderListPage,{status:type});
  }
}
