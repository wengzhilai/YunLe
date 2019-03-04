import { Component, ViewChild, OnInit } from '@angular/core';
import { NavController, Tabs, Tab, App, IonicApp, Platform, ToastController } from 'ionic-angular';
import { UserLoginPage } from '../user/login/login';
import { CommonService } from "../../Service/Common.Service";
import { ToPostService } from "../../Service/ToPost.Service";
import { FileUpService } from "../../Service/FileUp.Service";
import { AppGlobal } from "../../Classes/AppGlobal";
import { UserMenuPage } from "../user/menu/menu";
import { HelpEditPage } from "../help/edit/edit";
import { InsureListPage } from "../insure/list/list";
import { OrderListPage } from "../order/list/list";
import { CarListPage } from "../car/list/list";
import { CarEditPage } from "../car/edit/edit";
import { MessageListPage } from "../message/list/list";

@Component({
  selector: 'page-home',
  templateUrl: 'home.html',
  providers: [CommonService, ToPostService, FileUpService]
})
export class BlankPage {
  NowCar: any = {};
  public ICON_FILES_ID: number = 0;
  public currEnt;
  public user: any;

  constructor(
    public navCtrl: NavController,
    public commonService: CommonService,
    public fileUpService: FileUpService,
    public appCtrl: App,
    public toPostService: ToPostService
  ) {

  }

  ionViewWillEnter() {
    AppGlobal.LoadUser(v => {
      this.init(v);
    })
  }

  init(loadUser) {
    console.log('显示loadUser')
    console.log(loadUser)
    AppGlobal.setUser(loadUser);
    if (loadUser == null || loadUser.authToken == null || loadUser.authToken == '') {
      this.appCtrl.getRootNav().push(UserLoginPage);
    }
    else {
      this.toPostService.SingleFun("ClientSingle", loadUser.ID).then(currMsg => {
        if (currMsg.IsError) {
          this.appCtrl.getRootNav().push(UserLoginPage);
        } else {
          AppGlobal.setUser(currMsg);
          this.user = currMsg;
          if (currMsg.NowCar != null) {
            this.NowCar = currMsg.NowCar
          }
        }
      });
    }
  }
  hint(msg) {
    this.commonService.hint(msg);
  }
  IonSelect(type: any) {
    // this.commonService.hint(type);
  }
  goMenu() {
    this.navCtrl.push(UserMenuPage);
  }


  GoInsureList() {
    this.navCtrl.push(InsureListPage)
  }
  GoHelpEdit(type: any) {
    this.navCtrl.push(HelpEditPage, { orderType: type })
  }
  GoOrderList(type: any) {
    this.navCtrl.push(OrderListPage, { clientId: this.user.ID, type: type })
  }
  GoCarEditPage(id) {
    this.navCtrl.push(CarEditPage, { id: id })
  }
}


@Component({
  template: `
<ion-tabs  #myTabs >
  <ion-tab #tab0 tabTitle="首页" (ionSelect)="IonSelect('首页')" tabIcon="home"></ion-tab>
  <ion-tab #tab1 tabTitle="消息" (ionSelect)="IonSelect('消息')" tabIcon="chatbubbles"></ion-tab>
  <ion-tab #tab2 tabTitle="爱车" (ionSelect)="IonSelect('爱车')" tabIcon="car"></ion-tab>
  <ion-tab #tab3 tabTitle="我的" (ionSelect)="IonSelect('我的')" tabIcon="person"></ion-tab>
</ion-tabs>
`
})
export class HomePage implements OnInit {
  @ViewChild('myTabs') tabRef: Tabs;
  @ViewChild('tab0') tab0: Tab;
  @ViewChild('tab1') tab1: Tab;
  @ViewChild('tab2') tab2: Tab;
  @ViewChild('tab3') tab3: Tab;
  backButtonPressed: boolean = false;  //用于判断返回键是否触发

  constructor(
    public ionicApp: IonicApp,
    public platform: Platform,
    public toastCtrl: ToastController,

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
  ngOnInit() {
    this.tab0.root = BlankPage;
    this.tab1.root = MessageListPage;
    this.tab2.root = CarListPage;
    this.tab3.root = UserMenuPage;
  }
  hint(msg) {
    // this.commonService.hint(msg);
  }
  IonSelect(type: any) {
    // console.log(type)
    //this.tab0.setPages(BlankPage)
    // this.commonService.hint(type);
  }
}
