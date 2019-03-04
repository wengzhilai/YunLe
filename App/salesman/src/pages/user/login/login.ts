import { Storage } from "@ionic/storage"
import { Component } from '@angular/core';
import { NavController, ToastController, IonicApp, Platform } from 'ionic-angular';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { CommonService } from "../../../Service/Common.Service";
import { ToPostService } from "../../../Service/ToPost.Service";
import { HomePage } from "../../home/home";
import { AppGlobal } from "../../../Classes/AppGlobal";
import { UserFindPwdPage } from "../findPwd/findPwd";
import { UserRegisterPage } from "../register/register";

@Component({
  selector: 'user-login',
  templateUrl: 'login.html',
  providers: [CommonService, ToPostService]

})
export class UserLoginPage {
  msg: String;
  userForm: FormGroup;
  timer: any;
  formErrors = {};
  validationMessages: any;
  bean = {
    openid: "",
    loginName: "",
    password: ""
  }
  rememberPwd: any;
  backButtonPressed: boolean = false;  //用于判断返回键是否触发
  constructor(
    public ionicApp: IonicApp,
    public storage: Storage,
    public platform: Platform,
    private formBuilder: FormBuilder,
    public navCtrl: NavController,
    public toastCtrl: ToastController,
    public commonService: CommonService,
    public toPostService: ToPostService) {
    this.registerBackButtonAction();//注册返回按键事件
    this.userForm = this.formBuilder.group({
      loginName: ['', [Validators.required, Validators.minLength(4), Validators.maxLength(11)]],
      password: ['', [Validators.required]]
    });
    this.validationMessages = {
      'loginName': {
        'required': '登录名不能为空.',
        'minlength': '登录名长度不能少于4位',
        'maxlength': '登录名长度不能超过10位',
        'forbiddenName': '名称不能重复'
      },
      'password': {
        'required': '密码不能为空'
      }
    }
    this.storage.get("loginName").then(v => {
      if (v != null) {
        this.userForm.get('loginName').setValue(v);
      }
    })
    this.storage.get("password").then(v => {
      if (v != null) {
        this.userForm.get('password').setValue(v);
      }
    })
  }

  ionViewDidLoad() {

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



  submit() {
    if (this.userForm.invalid) {
      this.commonService.hint('输入无效!')
      return;
    }
    this.bean = this.userForm.value;
    this.storage.set("loginName", this.bean.loginName);
    if (this.rememberPwd) {
      this.storage.set("password", this.bean.password);
    }
    else {
      this.storage.remove("password");
    }
    if (navigator.userAgent.toLowerCase().indexOf('micromessenger') > -1) {
      this.bean.openid = this.commonService.getQueryString("openid");
      if (this.bean.openid == null) {
        this.commonService.hint(window.location.href);
        this.commonService.hint("未获取获取到openid");
        return;
      }
    }
    this.toPostService.Post("SalesmanLogin", this.bean, (currMsg) => {
      if (currMsg.IsError) {
        this.commonService.hint(currMsg.Message)
      } else {
        AppGlobal.getInstance()
        AppGlobal.setUser(currMsg)
        // this.navCtrl.pop();
        this.navCtrl.push(HomePage, {});
      }
    })
  }

  reset() {
    this.userForm.reset();
  }

  GoFindPwd() {
    this.navCtrl.push(UserFindPwdPage, {});
  }
  GoRegister() {
    this.navCtrl.push(UserRegisterPage);
  }


}
