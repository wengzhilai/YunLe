import {NavController, NavParams,AlertController,ToastController} from 'ionic-angular';
import {Component, OnInit} from '@angular/core';
import {FormGroup, FormBuilder, Validators} from '@angular/forms';
import {CommonService} from "../../../Service/Common.Service";
import {ToPostService} from "../../../Service/ToPost.Service";
import {AppGlobal} from "../../../Classes/AppGlobal";
import {FileUpService} from "../../../Service/FileUp.Service";
import {CustomerListPage} from "../list/list";

@Component({
  selector: 'customer-edit',
  templateUrl: 'edit.html',
  providers: [CommonService, ToPostService, FileUpService]
})
export class CustomerEditPage implements OnInit {
  public user = AppGlobal.getUser()
  public userForm:FormGroup;
  public validationMessages:any;
  public timer:any;
  public formErrors = {};
  public bean:any = {};

  constructor(public fileUpService:FileUpService,
              public params:NavParams,
              private formBuilder:FormBuilder,
              public navCtrl:NavController,
              public toastCtrl: ToastController,
              public commonService:CommonService,
              public toPostService:ToPostService,
              public alertCtrl:AlertController) {
    this.SingleBack({});

    if (this.params.get("id") != null) {
      this.toPostService.SingleFun("ClientSingle", this.params.get("id")).then(currMsg=> {
        if (currMsg.IsError == true) {
          this.commonService.hint(currMsg.Message);
        } else {
          this.SingleBack(currMsg);
        }
      });
    }
    else {
      this.bean= {AllCar: []};
    }
  }

  ngOnInit() {

  }

  SingleBack(clientEnt) {
    this.bean = clientEnt;
    this.userForm = this.formBuilder.group({
      NAME: [this.bean.NAME, [Validators.required,
        Validators.minLength(1),
        Validators.maxLength(11)
      ]],
      SEX: [this.bean.SEX, [Validators.minLength(1), Validators.maxLength(1)]],
      LOGIN_NAME: [this.bean.phone, [Validators.maxLength(11)]],
      ID_NO: [this.bean.ID_NO, [Validators.maxLength(18), Validators.minLength(18)]],
      ADDRESS: [this.bean.ADDRESS, [Validators.minLength(0), Validators.maxLength(100)]],
      REMARK: [this.bean.REMARK, [Validators.minLength(0), Validators.maxLength(200)]]
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
      'LOGIN_NAME': {
        'minlength': '长度不能少于11位',
        'maxlength': '长度不能超过11位'
      },
      'ID_NO': {
        'minlength': '长度不能少于18位',
        'maxlength': '长度不能超过18位'
      },
    };

    this.userForm.valueChanges.subscribe(data => {
      this.formErrors = this.commonService.FormValidMsg(this.userForm, this.validationMessages);
    });
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
            default:
              var keyArr = key.split('_')
              if (keyArr.length > 0) {
                for (var i = 0; i < this.bean.AllCar.length; i++) {
                  if (this.bean.AllCar[i].PLATE_NUMBER == keyArr[0]) {
                    if (keyArr[1] == 0) {
                      this.bean.AllCar[i].DrivingPicUrl = url;
                      this.bean.AllCar[i].DRIVING_PIC_ID = ID;
                    }
                    else if (keyArr[1] == 1) {
                      this.bean.AllCar[i].DrivingPicUrl1 = url;
                      this.bean.AllCar[i].DRIVING_PIC_ID1 = ID;
                    }
                  }
                }
              }
              break;
          }
        });
  }

  showBigImage(url) {
    this.commonService.FullScreenImage(url, this);
  }

  save() {
    if (this.userForm.invalid) {
      console.log(this.formErrors)
      console.log(this.userForm)
      this.commonService.hint('输入无效!')
      return;
    }
    var postBean = this.userForm.value;
    postBean.ID = this.bean.ID;
    postBean.ICON_FILES_ID = this.bean.ICON_FILES_ID;
    postBean.ID_NO_PIC_ID = this.bean.ID_NO_PIC_ID;
    postBean.AllCar = this.bean.AllCar;
    this.toPostService.SaveOrUpdate("SalesmanClientAdd", postBean, (currMsg) => {
      if (!currMsg.IsError) {
        this.commonService.hint("个人信息保存成功！")
        this.navCtrl.push(CustomerListPage);
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



  AddCar() {
    this.bean.AllCar[this.bean.AllCar.length] = {PLATE_NUMBER:'川A'};
  }

  GoBack() {
    this.navCtrl.pop();
  }
}
