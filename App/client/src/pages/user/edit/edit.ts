import {NavController, ToastController,AlertController} from 'ionic-angular';
import {Component,OnInit} from '@angular/core';
import {FormGroup, FormBuilder, Validators} from '@angular/forms';
import {CommonService} from "../../../Service/Common.Service";
import {ToPostService} from "../../../Service/ToPost.Service";
import {AppGlobal} from "../../../Classes/AppGlobal";
import {FileUpService} from "../../../Service/FileUp.Service";

@Component({
  selector: 'user-edit',
  templateUrl: 'edit.html',
  providers: [CommonService, ToPostService, FileUpService]
})
export class UserEditPage implements OnInit {
  public user = AppGlobal.getUser()
  userForm:FormGroup;
  public validationMessages:any;
  public formErrors = {};
  timer:any;
  public bean:any={};
  constructor(public fileUpService:FileUpService,
              private formBuilder:FormBuilder,
              public navCtrl:NavController,
              public toastCtrl: ToastController,
              public commonService:CommonService,
              public toPostService:ToPostService,
              public alertCtrl:AlertController) {}

  ngOnInit() {
    console.log('UserEditPage');
    this.bean = AppGlobal.getUser();
    console.log(this.bean);

    this.userForm = this.formBuilder.group({
      NAME: [this.bean.NAME, [Validators.required, Validators.minLength(1), Validators.maxLength(11)]],
      SEX: [this.bean.SEX, [Validators.required, Validators.minLength(1), Validators.maxLength(1)]],
      phone: [this.bean.phone, [Validators.maxLength(11), Validators.minLength(11)]],
      REQUEST_CODE: [this.bean.REQUEST_CODE, [Validators.maxLength(4), Validators.minLength(4)]],
      ID_NO: [this.bean.ID_NO, [Validators.maxLength(18), Validators.minLength(18)]]
    });
    this.validationMessages = {
      'NAME': {
        'required': '不能为空.',
        'minlength': '长度不能少于1位',
        'maxlength': '长度不能超过10位'
      },
      'SEX': {
        'required': '密码不能为空'
      },
      'phone': {
        'minlength': '长度不能少于11位',
        'maxlength': '长度不能超过11位'
      },
      'ID_NO': {
        'minlength': '长度不能少于18位',
        'maxlength': '长度不能超过18位'
      },
    };
  }

  upImg(key) {
      this.fileUpService.upImg(this, key, (Key:string, url:string, ID:number) => {
          switch (key) {
            case "iconURL":
              this.bean.iconURL = url;
              this.bean.ICON_FILES_ID = ID;
              break;
            case "idNoUrl":
              this.bean.idNoUrl = url;
              this.bean.ID_NO_PIC_ID = ID;
              break;
          }
        });
  }

  showBigImage(url) {
    this.commonService.FullScreenImage(url, this);
  }

  save() {
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
    var postBean=this.userForm.value;
    postBean.ID=this.bean.ID;
    postBean.ICON_FILES_ID=this.bean.ICON_FILES_ID;
    postBean.ID_NO_PIC_ID=this.bean.ID_NO_PIC_ID;


      this.toPostService.SaveOrUpdate("ClientSave", postBean, (currMsg) => {
          if (!currMsg.IsError) {
            this.commonService.hint("个人信息保存成功！")
            AppGlobal.getInstance()
            AppGlobal.setUser(currMsg);
            this.navCtrl.pop();
          }
          else {
            this.commonService.hint(currMsg.Message)
          }
    });
  }

  updatePassword() {
    let alert = this.alertCtrl.create();
    alert.setTitle('修改密码');

    alert.addInput({
      type: 'password',
      placeholder: '原密码',
      label: '原密码1'
    });
    alert.addInput({
      type: 'password',
      placeholder: '新密码',
      label: '新密码1'
    });
    alert.addInput({
      type: 'password',
      placeholder: '重复密码',
      label: '重复密码1'
    });

    alert.addButton('取消');
    alert.addButton({
      text: '确定',
      handler: data => {
        console.log('Radio data:', data);
        if (data[1] != data[2]) {
          this.commonService.hint('重复密码不一致')
          return;
        }

        var subBean = {
          authToken: AppGlobal.getUserAuthToken(),
          userId: 0,
          entity: data[1],
          para: [{K: 'oldPwd', V: data[0]}]
        };
        console.log(subBean)
        this.toPostService.PostFun("UserEditPwd", subBean).then((currMsg) => {
          if (!currMsg.IsError) {
            this.commonService.hint("修改密码成功！")
          }
          else {
            this.commonService.hint(currMsg.Message)
          }
        });
      }
    });
    alert.present();
  }

  GoBack() {
    this.navCtrl.pop();
  }
}
