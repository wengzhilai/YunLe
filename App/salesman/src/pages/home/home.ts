import { Component } from '@angular/core';
import { NavController, ToastController, IonicApp, Platform } from 'ionic-angular';
import { UserLoginPage } from '../user/login/login';
import { CommonService } from "../../Service/Common.Service";
import { ToPostService } from "../../Service/ToPost.Service";
import { FileUpService } from "../../Service/FileUp.Service";
import { SalesmanUserBean } from "../../Classes/SalesmanUserBean";
import { AppGlobal } from "../../Classes/AppGlobal";
import { UserMenuPage } from "../user/menu/menu";
import { CustomerListPage } from "../customer/list/list";
import { InsureListPage } from "../insure/list/list";
import { OrderListPage } from "../order/list/list";
import { RunlistListPage } from "../order/runlist/runlist";
import { ListGrabPage } from "../help/listgrab/listgrab";
import { TeamTabsPage } from "../team/teamtabs";
import { InsureExpirePage } from "../insure/expire/expire";
import {CarIn} from "../../Classes/CarIn";
import {MessageListPage} from "../../pages/message/list/list";

declare var wx;

@Component({
  selector: 'page-home',
  templateUrl: 'home.html',
  providers: [CommonService, ToPostService, FileUpService]
})
export class HomePage {
  backButtonPressed: boolean = false;  //用于判断返回键是否触发
  public ICON_FILES_ID: number = 0;
  public currEnt;
  public user: SalesmanUserBean = new SalesmanUserBean();

  constructor(
    public ionicApp: IonicApp,
    public platform: Platform,
    public toastCtrl: ToastController,
    public navCtrl: NavController,
    public commonService: CommonService,
    public fileUpService: FileUpService,
    public toPostService: ToPostService
  ) {
    this.registerBackButtonAction();//注册返回按键事件
  }
  registerBackButtonAction() {
    this.platform.registerBackButtonAction(() => {
      //如果想点击返回按钮隐藏toast或loading或Overlay就把下面加上
      // this.ionicApp._toastPortal.getActive() || this.ionicApp._loadingPortal.getActive() || this.ionicApp._overlayPortal.getActive()
      let activePortal = this.ionicApp._modalPortal.getActive();
      if (activePortal) {
        activePortal.dismiss().catch(() => { });
        activePortal.onDidDismiss(() => { });
        return;
      }
      return this.showExit();
    }, 1);
  }

  //双击退出提示框
  showExit() {
    if (this.backButtonPressed) { //当触发标志为true时，即2秒内双击返回按键则退出APP
      this.platform.exitApp();
    } else {
      this.toastCtrl.create({
        message: '再按一次退出应用',
        duration: 2000,
        position: 'bottom'
      }).present();
      this.backButtonPressed = true;
      setTimeout(() => this.backButtonPressed = false, 2000);//2秒内没有再次点击返回则将触发标志标记为false
    }
  }
  ionViewWillEnter() {
    console.log('加载loadUser')
    
    AppGlobal.LoadUser(v => {
      this.init(v);
    })
  }
  init(loadUser) {
    console.log('显示loadUser')
    console.log(loadUser)
    AppGlobal.setUser(loadUser);
    if (loadUser == null || loadUser.authToken == null || loadUser.authToken == '') {
      this.navCtrl.push(UserLoginPage, {});
      return;
    }
    else {
      this.user = loadUser;
      AppGlobal.setUser(loadUser);
      this.toPostService.single("SalesmanSingle", loadUser.ID, (currMsg) => {
        if (currMsg.IsError) {
          this.commonService.hint(currMsg.Message);
          this.navCtrl.push(UserLoginPage, {});
        } else {
          AppGlobal.setUser(currMsg);
        }
      });
    }
  }
  toHelp(id) {
    this.navCtrl.push(UserLoginPage, { name: "grey", job: "IT" });
  }
  toMessageList() {
    this.navCtrl.push(UserLoginPage, { name: "grey", job: "IT" });
  }
  alert(str) {
    this.commonService.hint(str);
  }

  goLogin() {
    this.navCtrl.push(UserLoginPage, { name: "grey", job: "IT" });
  };
  goMenu() {
    this.navCtrl.push(UserMenuPage);
  }
  showMsg() {
    this.toPostService.list("bbsList", {}, (val) => {
      console.log(val);
    })
  }
  GoCustomerList() {
    this.navCtrl.push(CustomerListPage);
  }
  GoInsureList() {
    this.navCtrl.push(InsureListPage)
  }
  GoOrderListPage() {
    this.navCtrl.push(OrderListPage)
  }
  GoRunlistListPage() {
    this.navCtrl.push(RunlistListPage)
  }
  GoListGrabPage() {
    this.navCtrl.push(ListGrabPage)
  }
  GoTeam() {
    this.navCtrl.push(TeamTabsPage)
  }
  GoInsureExpirePage() {
    this.navCtrl.push(InsureExpirePage)
  }
  GoMessageListPage(){
    this.navCtrl.push(MessageListPage)
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
}
