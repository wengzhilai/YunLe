import {CommonService} from "../../../Service/Common.Service";
import {ToPostService} from "../../../Service/ToPost.Service";
import {NavController,Button,ToastController} from 'ionic-angular';
import { Component,ViewChild,}                  from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import {AppGlobal} from "../../../Classes/AppGlobal";

@Component({
  selector: 'user-findPwd',
  templateUrl: 'findPwd.html',
  providers: [CommonService,ToPostService]

})
export class UserFindPwdPage  {
  @ViewChild('sendCode') sendCode:Button;
  msg: String;
  userForm: FormGroup;
  validationMessages:any;
  timer:any;
  formErrors:any = {  };
  bean = {
    userId: 0,
    authToken: AppGlobal.getUserAuthToken(),
    id: 0,
    para: []
  }

  constructor(private formBuilder: FormBuilder,
              public navCtrl:NavController,
              public  commonService:CommonService,
              public toastCtrl: ToastController,
              public toPostService:ToPostService) {
    this.userForm = this.formBuilder.group({
      LoginName: ['', [Validators.required, Validators.minLength(11), Validators.maxLength(11)]],
      VerifyCode: ['', [Validators.required, Validators.minLength(4), Validators.maxLength(4)]],
      NewPwd: ['', [Validators.required, Validators.minLength(1), Validators.maxLength(11)]],
    });
    this.validationMessages = {
      'LoginName': {
        'required': '电话号码不能为空.',
        'minlength': '电话号码长度不能少于11位',
        'maxlength': '电话号码长度不能超过11位'
      },
      'VerifyCode': {
        'required': '验证码不能为空.',
        'minlength': '验证码长度不能少于4位',
        'maxlength': '验证码长度不能超过4位'
      },
      'NewPwd': {
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
    const control=this.userForm.get("LoginName")
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
    const thisValue=this.userForm.value
    if(thisValue!=null){
      this.bean.para = [
        {K: 'VerifyCode',V:thisValue.VerifyCode},
        {K: 'LoginName',V:thisValue.LoginName},
        {K: 'NewPwd', V:thisValue.NewPwd}]
    }


    this.toPostService.Post("ResetPassword", this.bean, (currMsg)=> {
      if (currMsg.IsError) {
        this.commonService.hint(currMsg.Message)
      } else {
        this.commonService.hint("重设密码成功");
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
