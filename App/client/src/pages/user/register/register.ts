import {CommonService} from "../../../Service/Common.Service";
import {ToPostService} from "../../../Service/ToPost.Service";
import {NavController,ToastController} from 'ionic-angular';
import { Component,}                  from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import {CarIn} from "../../../Classes/CarIn";

@Component({
  selector: 'user-register',
  templateUrl: 'register.html',
  providers: [CommonService,ToPostService]

})
export class UserRegisterPage  {
  userForm: FormGroup;
  validationMessages:any;
  timer:any;
  formErrors:any = {  };
  bean = {
    loginName: '',
    userName:'',
    password: '',
    version: CarIn.version,
    code: '',
    type: '0',
    pollCode: ''
  }

  constructor(private formBuilder: FormBuilder,
              public navCtrl:NavController,
              public  commonService:CommonService,
              public toastCtrl: ToastController,
              public toPostService:ToPostService) {
    const openid=this.commonService.getQueryString("openid");
    if(openid!=null){
      this.toPostService.Post("WeixinGetUser", {authToken: openid }, (ent)=>{
        var code = ent.SCENE_STR;
        var type = code.substr(0, code.lastIndexOf("_"));
        if (type == "qrscene_salesman") {
          code = code.substr(code.lastIndexOf("_") + 1);
          this.userForm.get('pollCode').setValue(code);
        }
        else {
          this.commonService.hint('你不是会员，请关闭后，选择服务商入口');
          window.close();
        }
      });
    }

    this.userForm = this.formBuilder.group({
      loginName: ['', [Validators.required, Validators.minLength(11), Validators.maxLength(11)]],
      userName: ['', [Validators.required, Validators.minLength(1), Validators.maxLength(11)]],
      pollCode: ['', [Validators.required, Validators.minLength(1), Validators.maxLength(6)]],
      code: ['', [Validators.required, Validators.minLength(4), Validators.maxLength(4)]],
      password: ['', [Validators.required, Validators.minLength(1), Validators.maxLength(11)]],
    });
    this.validationMessages = {
      'loginName': {
        'required': '电话号码不能为空.',
        'minlength': '电话号码长度不能少于11位',
        'maxlength': '电话号码长度不能超过11位'
      },
      'code': {
        'required': '验证码不能为空.',
        'minlength': '验证码长度不能少于4位',
        'maxlength': '验证码长度不能超过4位'
      },
      'userName': {
        'required': '姓名不能为空.',
        'minlength': '姓名长度不能少于4位',
        'maxlength': '姓名长度不能超过4位'
      },
      'pollCode': {
        'required': '推荐码不能为空.',
        'minlength': '推荐码长度不能少于4位',
        'maxlength': '推荐码长度不能超过4位'
      },
        'password': {
        'required': '密码不能为空.',
        'minlength': '密码长度不能少于1位',
        'maxlength': '密码长度不能超过10位'
      }
    };
  }


  sendCodeDisabled=false;
  sendCodeText="发送验证码"
  i=0
  SetTimeValue(){
    if(this.i>0)
    {
      this.i--
      this.sendCodeText=this.i+"秒";
      setTimeout(() => { this.SetTimeValue() }, 1000);
    }
    else {
      this.sendCodeDisabled=false;
      this.sendCodeText="发送验证码"
    }
  }
  SendCode($event){
    const control=this.userForm.get("loginName")
    console.log(control);
    if (control && control.dirty && !control.valid) {
      this.commonService.hint('电话号码无效!')
      return;
    }
    this.sendCodeDisabled = true;
    this.sendCodeText="60秒";
    this.i=60;
    this.SetTimeValue();
    this.toPostService.Post("SendCode", {"K":"phone",V: control.value}, (currMsg)=>{
      if (currMsg.IsError) {
        this.commonService.hint(currMsg.Message)
      } else {
        this.commonService.showLongToast("发送成功");
      }
    })
  }
  submit() {
    if (this.userForm.invalid) {
      this.formErrors = this.commonService.FormValidMsg(this.userForm, this.validationMessages);
      let errMsg="";
      for (const field in this.formErrors) {
        if (this.formErrors[field] != '') {
          errMsg+="<p>"+this.formErrors[field]+"</p>"
        }
      }
      this.commonService.hint(errMsg,'输入无效')
      return;
    }
    console.log(this.userForm.value)
    // this.bean=this.userForm.value
    for(var key in this.userForm.value){
      this.bean[key]=this.userForm.value[key];
    }

    this.toPostService.Post("LoginReg", this.bean, (currMsg)=> {
      if (currMsg.IsError) {
        this.commonService.hint(currMsg.Message)
      } else {
        this.commonService.hint("注册成功");
        this.navCtrl.pop();
      }
    })
  }
  reset() {
    this.userForm.reset();
  }
  GoBack(){
    this.navCtrl.pop();
  }


}
