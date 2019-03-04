import {NavController,AlertController} from 'ionic-angular';
import {Component} from '@angular/core';
import {CommonService} from "../../../Service/Common.Service";
import {ToPostService} from "../../../Service/ToPost.Service";
import {AppGlobal} from "../../../Classes/AppGlobal";
import {CarIn} from "../../../Classes/CarIn";
import {FileUpService} from "../../../Service/FileUp.Service";
import {UserLoginPage} from "../login/login";
import {UserEditPage} from "../edit/edit";
import {CustomerListPage} from "../../customer/list/list";
import {OrderListPage} from "../../order/list/list";
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
              public commonService:CommonService,
              public toPostService:ToPostService,
              public alertCtrl:AlertController) {
  }
  toShare() {
    console.log(this.user)
    var user_id = this.user.ID;
    var baseUrl = CarIn.baseUrl;
    var qrcode = baseUrl + '/File/QrCode/salesman_' + user_id + '.jpg';
    var qrcodeWeiXin = baseUrl + '/File/QrCode/salesman_weixin_' + user_id + '.jpg';
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

  watting() {
    this.commonService.hint("后续开放，敬请期待");
  }

  sendFre() {
    this.commonService.hint("发送给朋友");
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
                  this.outLoginBack(ent);
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
  outLoginBack(errobj:any)
  {
    if (errobj.IsError) {
      this.commonService.hint(errobj.Message);
    }
    AppGlobal.removeUser();
    this.navCtrl.push(UserLoginPage)
  }
  goHome()
  {
    this.navCtrl.pop();
    //this.navCtrl.push(HomePage);
  }
  goEdit()
  {
    this.navCtrl.push(UserEditPage);
  }
  GoCustomerList(){
    this.navCtrl.push(CustomerListPage);
  }
  GoOrderListPage()
  {
    this.navCtrl.push(OrderListPage)
  }
}
