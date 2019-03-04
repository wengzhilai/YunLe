import {CommonService} from "../../../Service/Common.Service";
import {ToPostService} from "../../../Service/ToPost.Service";
import {NavController, NavParams, ToastController} from 'ionic-angular';
import {Component, OnInit} from '@angular/core';
import {AppGlobal} from "../../../Classes/AppGlobal";
import {FileUpService} from "../../../Service/FileUp.Service";
import {AlertController} from 'ionic-angular';
import {FormGroup, FormBuilder, Validators} from '@angular/forms';

@Component({
  selector: 'car-edit',
  templateUrl: 'edit.html',
  providers: [CommonService, ToPostService, FileUpService]
})
export class CarEditPage implements OnInit {
  isNew:any=false;
  cityCode:any;
  public allCityCode=['京',  '沪','津','渝','黑','吉','辽','蒙','冀','新','甘','青','陕','宁','豫','鲁','晋','皖','鄂','湘','苏','川','黔','滇','桂','藏','浙','赣','粤','闽','台','琼','港','澳'];
  public nowInsurer:any = {};
  public user = AppGlobal.getUser()
  public userForm:FormGroup;
  timer:any;
  public validationMessages:any;
  public formErrors = {};
  public bean:any = {};

  constructor(public fileUpService:FileUpService,
              public params:NavParams,
              private formBuilder:FormBuilder,
              public toastCtrl: ToastController,
              public navCtrl:NavController,
              public commonService:CommonService,
              public toPostService:ToPostService,
              public alertCtrl:AlertController) {

    this.SingleBack({})

    if (this.params.get("id") != null) {
      this.toPostService.SingleFun("CarSingle", this.params.get("id")).then((currMsg)=> {
        if (currMsg.IsError == true) {
          this.commonService.hint(currMsg.Message);
        } else {
          this.SingleBack(currMsg)
        }
      });
    }
  }

  ngOnInit() {
  }


  SingleBack(clientEnt) {
    this.bean = clientEnt;
    this.userForm = this.formBuilder.group({
      PLATE_NUMBER: [this.bean.PLATE_NUMBER, [Validators.required,Validators.minLength(7),Validators.maxLength(7)]],
      ENGINE_NUMBER: [this.bean.ENGINE_NUMBER, [Validators.minLength(1),Validators.maxLength(11)]],
      FRAME_NUMBER: [this.bean.FRAME_NUMBER, [Validators.minLength(1),Validators.maxLength(11)]],
      CAR_TYPE: [this.bean.CAR_TYPE, [Validators.minLength(1),Validators.maxLength(11)]],
      PRICE: [this.bean.PRICE, [Validators.minLength(1),Validators.maxLength(11)]],
      BRAND: [this.bean.BRAND, [Validators.minLength(1),Validators.maxLength(11)]],
      REG_DATA: [this.bean.REG_DATA, [Validators.minLength(1),Validators.maxLength(20)]],
      IS_NEW: [this.bean.IS_NEW==1]
    });
    this.validationMessages = { };
  }
  ChangCityCode(){
    if(this.userForm.value.PLATE_NUMBER==null)
    {
      this.userForm.get('PLATE_NUMBER').setValue(this.cityCode);
    }
    else {
      var v=this.userForm.value.PLATE_NUMBER.substr(1);
      this.userForm.get('PLATE_NUMBER').setValue(this.cityCode+v);
    }
  }
  upImg(key) {
    console.log(key);
    this.fileUpService.upImg(this, key, (Key:string, url:string, ID:number) => {
      switch (Key) {
        case "billUrl":
          this.bean.BILL_PIC_ID = ID;
          this.bean.billUrl = url;
          break;
        case "certificatePicUrl":
          this.bean.CERTIFICATE_PIC_ID = ID;
          this.bean.certificatePicUrl = url;
          break;
        case "DrivingPicUrl":
          this.bean.DRIVING_PIC_ID = ID;
          this.bean.DrivingPicUrl = url;
          break;
        case "DrivingPicUrl1":
          this.bean.DRIVING_PIC_ID1 = ID;
          this.bean.DrivingPicUrl1 = url;
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
    console.log(this.bean)
    var postBean = this.userForm.value;
    for (const key in postBean){
      this.bean[key]=postBean[key];
    }
    this.bean.IS_NEW=(postBean.IS_NEW)?1:0;
    console.log(this.bean)

    this.toPostService.SaveOrUpdate("CarSave", this.bean, (currMsg) => {
      if (!currMsg.IsError) {
        var user=AppGlobal.getUser()
        user.NowCar=currMsg
        AppGlobal.setUser(user);
        this.commonService.showLongToast("车辆保存成功！")
        this.navCtrl.pop();
      }
      else {
        this.commonService.hint(currMsg.Message)

      }
    });
  }

  GoBack() {
    this.navCtrl.pop();
  }
}
