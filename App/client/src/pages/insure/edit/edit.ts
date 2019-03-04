import { CommonService } from "../../../Service/Common.Service";
import { ToPostService } from "../../../Service/ToPost.Service";
import { NavController, NavParams } from 'ionic-angular';
import { Component } from '@angular/core';
import { AppGlobal } from "../../../Classes/AppGlobal";
import { FileUpService } from "../../../Service/FileUp.Service";
import { AlertController } from 'ionic-angular';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';

@Component({
  selector: 'insure-edit',
  templateUrl: 'edit.html',
  providers: [CommonService, ToPostService, FileUpService]
})
export class InsureEditPage {
  public purchaseWayArr = ['续保', '三责20万', '三责50万', '三责100万'];
  public nowInsurer: any = {};
  public user = AppGlobal.getUser()
  public userForm: FormGroup;
  public validationMessages: any;
  public formErrors = {};
  public bean: any = {};

  constructor(public fileUpService: FileUpService,
    public params: NavParams,
    private formBuilder: FormBuilder,
    public navCtrl: NavController,
    public commonService: CommonService,
    public toPostService: ToPostService,
    public alertCtrl: AlertController) {
    this.SingleBack(this.SetBean());


    if (this.params.get("id") != null) {
      this.toPostService.SingleFun("OrderInsureSingle", this.params.get("id")).then((currMsg) => {
        if (currMsg.IsError == true) {
          this.commonService.hint(currMsg.Message);
        } else {
          this.bean = currMsg;
          for (var i = 0; i < this.bean.AllFiles.length; i++) {
            this.bean.AllFiles[i].indexNo = i;
          }

          this.SingleBack(currMsg)
          for (var i = 0; i < this.bean.AllInsurePrice.length; i++) {
            if (this.bean.INSURER_ID == this.bean.AllInsurePrice[i].ID) {
              this.nowInsurer = this.bean.AllInsurePrice[i];
            }
          }
        }
      });
    }
    else {
      this.SetBean();
      this.toPostService.SingleFun("QueryInsure", null).then((currMsg) => {
        if (currMsg.IsError == true) {
          this.commonService.hint(currMsg.Message);
        } else {
          this.bean.AllInsurePrice = currMsg;
          this.bean.INSURER_ID = 1;
          if (currMsg.length > 0) {
            this.nowInsurer = currMsg[0];
          }
          for (var i = 0; i < this.bean.AllInsurePrice.length; i++) {
            for (var a = 0; a < this.bean.AllInsurePrice[i].AllProductPrice.length; a++) {
              this.bean.AllInsurePrice[i].AllProductPrice[a].isCheck = (this.bean.AllInsurePrice[i].AllProductPrice[a].IS_MUST == 1)
            }
          }
        }
      });
    }

    var isStaff = false
    for (var i = 0; i < this.user.RoleAllID.length; i++) {
      if (this.user.RoleAllID[i] == 3) {
        isStaff = true;
      }
    }
    console.log("customer")
    console.log(this.params.get("customer"))
    console.log(isStaff)
    if (this.params.get("customer") != null || !isStaff) {

      var nowUser: any = this.params.get("customer");
      if (this.params.get("customer") == null) //表示是用户
      {
        nowUser = AppGlobal.getUser();
      }
      this.bean.Client = nowUser;
      this.bean.CLIENT_ID = nowUser.ID;
      this.bean.CAR_USERNAME = this.bean.Client.NAME
      if (this.bean.Client.AllCar.length > 0) {
        this.bean.Car = this.bean.Client.NowCar;
        this.bean.CarNumber = this.bean.Car.PLATE_NUMBER;
      }

      this.bean.Car.idNoUrl = this.bean.Client.idNoUrl
      this.bean.Car.ID_NO_PIC_ID = this.bean.Client.ID_NO_PIC_ID;
      this.bean.Car.idNoUrl1 = this.bean.Client.idNoUrl1
      this.bean.Car.ID_NO_PIC_ID1 = this.bean.Client.ID_NO_PIC_ID1;
      this.userForm.get('CAR_USERNAME').setValue(nowUser.NAME);
    }
  }

  SetBean() {
    this.bean = {
      Car: { PLATE_NUMBER: '', billUrl: '', certificatePicUrl: '' },
      Client: { AllCar: [] },
      SaveProductId: [],
      AllInsurePrice: [],
      CLIENT_ID: 0,
      AllFiles: []
    }
    return this.bean;
  }

  SelectedInsure(event: string) {
    for (var i = 0; i < this.bean.AllInsurePrice.length; i++) {
      if (this.bean.AllInsurePrice[i].ID == event)
        this.nowInsurer = this.bean.AllInsurePrice[i];
    }
  }

  SelectedCar(event: string) {
    for (var i = 0; i < this.bean.Client.AllCar.length; i++) {
      if (this.bean.Client.AllCar[i].PLATE_NUMBER == event) {
        this.bean.Car = this.bean.Client.AllCar[i];
      }
    }
  }

  SingleBack(clientEnt) {
    this.bean = clientEnt;
    this.userForm = this.formBuilder.group({
      CAR_USERNAME: [this.bean.CAR_USERNAME, [Validators.required,
      Validators.minLength(1),
      Validators.maxLength(11)
      ]],
      DELIVERY: [this.bean.DELIVERY, [Validators.minLength(1), Validators.maxLength(100)]],
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
  }

  changepurchaseWay() {
    var purchaseWay = this.bean.PURCHASE_WAY;
    switch (purchaseWay) {
      case "三责20万":
        this.SetValue(this.nowInsurer.AllProductPrice, 4, '20万')
        this.SetValue(this.nowInsurer.AllProductPrice, 5, '1万')
        this.SetValue(this.nowInsurer.AllProductPrice, 6, '1万')
        this.SetValue(this.nowInsurer.AllProductPrice, 11, '0.2万')
        break;
      case "三责50万":
        this.SetValue(this.nowInsurer.AllProductPrice, 4, '50万')
        this.SetValue(this.nowInsurer.AllProductPrice, 5, '2万')
        this.SetValue(this.nowInsurer.AllProductPrice, 6, '2万')
        this.SetValue(this.nowInsurer.AllProductPrice, 11, '1万')
        break;
      case "三责100万":
        this.SetValue(this.nowInsurer.AllProductPrice, 4, '100万')
        this.SetValue(this.nowInsurer.AllProductPrice, 5, '10万')
        this.SetValue(this.nowInsurer.AllProductPrice, 6, '10万')
        this.SetValue(this.nowInsurer.AllProductPrice, 11, '2万')
        break;
    }
  }

  SetValue(obj, id, value) {
    for (var i = 0; i < obj.length; i++) {
      if (obj[i].ID == id) {
        obj[i].maxPay = value
        obj[i].isCheck = true
        return;
      }
      for (var a = 0; a < obj[i].ChildItem.length; a++) {
        if (obj[i].ChildItem[a].ID == id) {
          obj[i].ChildItem[a].maxPay = value
          obj[i].ChildItem[a].isCheck = true
          return;
        }
      }
    }
  }

  AddImg() {
    var indexNo = this.bean.AllFiles.length;
    this.bean.AllFiles[indexNo] = { "indexNo": this.bean.AllFiles.length };
    this.upImg('allFile_' + indexNo)
  }

  upImg(key) {
    console.log(key);
    this.fileUpService.upImg(this, key, (Key: string, url: string, ID: number) => {
      switch (Key) {
        case "idNoUrl":
          this.bean.Car.ID_NO_PIC_ID = ID;
          this.bean.Car.idNoUrl = url;
          break;
        case "idNoUrl1":
          this.bean.Car.ID_NO_PIC_ID1 = ID;
          this.bean.Car.idNoUrl1 = url;
          break;
        case "DrivingPicUrl":
          this.bean.Car.DRIVING_PIC_ID = ID;
          this.bean.Car.DrivingPicUrl = url;
          break;
        case "DrivingPicUrl1":
          this.bean.Car.DRIVING_PIC_ID1 = ID;
          this.bean.Car.DrivingPicUrl1 = url;
          break;
        case "driverPicUrl":
          this.bean.DRIVER_PIC_ID = ID;
          this.bean.driverPicUrl = url;
          break;
        case "driverPicUrl1":
          this.bean.DRIVER_PIC_ID1 = ID;
          this.bean.driverPicUrl1 = url;
          break;
        case "recognizeePicUrl":
          this.bean.RECOGNIZEE_PIC_ID = ID;
          this.bean.recognizeePicUrl = url;
          break;
        case "recognizeePicUrl1":
          this.bean.RECOGNIZEE_PIC_ID1 = ID;
          this.bean.recognizeePicUrl1 = url;
          break;
        case "billUrl":
          this.bean.Car.BILL_PIC_ID = ID;
          this.bean.Car.billUrl = url;
          break;
        case "certificatePicUrl":
          this.bean.Car.CERTIFICATE_PIC_ID = ID;
          this.bean.Car.certificatePicUrl = url;
          break;
        default:
          if (Key.indexOf('allFile_') == 0) {
            let indexNo = Key.replace('allFile_', '')
            for (var i = 0; i < this.bean.AllFiles.length; i++) {
              if (this.bean.AllFiles[i].indexNo == indexNo) {
                if (ID == null || ID == 0) {
                  this.bean.AllFiles.splice(i, 1); //删除
                  for (var x = 0; x < this.bean.AllFiles.length; x++) {
                    this.bean.AllFiles[x].indexNo = x;
                  }
                } else {
                  this.bean.AllFiles[i].ID = ID;
                  this.bean.AllFiles[i].URL = url;
                }
                break;
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
      this.formErrors = this.commonService.FormValidMsg(this.userForm, this.validationMessages);
      let errMsg = "";
      for (const field in this.formErrors) {
        if (this.formErrors[field] != '') {
          errMsg += "<p>" + this.formErrors[field] + "</p>"
        }
      }
      this.commonService.hint(errMsg, '输入无效')
      return;
    }

    this.bean.SaveProductId = [];
    for (var i = 0; i < this.bean.AllInsurePrice.length; i++) {
      if (this.bean.AllInsurePrice[i].ID == this.bean.INSURER_ID) {
        for (var a = 0; a < this.bean.AllInsurePrice[i].AllProductPrice.length; a++) {
          if (this.bean.AllInsurePrice[i].AllProductPrice[a].isCheck) {
            this.bean.SaveProductId[this.bean.SaveProductId.length] = {
              PRODUCT_ID: this.bean.AllInsurePrice[i].AllProductPrice[a].ID,
              MAX_PAY: this.bean.AllInsurePrice[i].AllProductPrice[a].maxPay
            };
            for (var a1 = 0; a1 < this.bean.AllInsurePrice[i].AllProductPrice[a].ChildItem.length; a1++) {
              if (this.bean.AllInsurePrice[i].AllProductPrice[a].ChildItem[a1].isCheck) {
                this.bean.SaveProductId[this.bean.SaveProductId.length] = {
                  PRODUCT_ID: this.bean.AllInsurePrice[i].AllProductPrice[a].ChildItem[a1].ID,
                  MAX_PAY: this.bean.AllInsurePrice[i].AllProductPrice[a].ChildItem[a1].maxPay
                };
              }
            }
          }
        }
      }
    }

    var postBean = this.userForm.value;
    postBean.ID = this.bean.ID;
    postBean.INSURER_ID = this.bean.INSURER_ID;
    postBean.ICON_FILES_ID = this.bean.ICON_FILES_ID;
    postBean.ID_NO_PIC = this.bean.ID_NO_PIC;
    postBean.AllCar = this.bean.AllCar;
    postBean.SaveProductId = this.bean.SaveProductId; //投保信息
    postBean.Client = this.bean.Client; //客户信息
    postBean.Car = this.bean.Car; //车辆信息
    postBean.PURCHASE_WAY = this.bean.PURCHASE_WAY;//投保方式
    postBean.AllFiles = this.bean.AllFiles;

    this.toPostService.SaveOrUpdate("OrderInsureSave", postBean, (currMsg) => {
      if (!currMsg.IsError) {
        this.commonService.showLongToast("资料已上传，等待业务员与您联系！")
        this.navCtrl.pop();
      }
      else {
        this.commonService.hint(currMsg.Message)
      }
    });
  }

  AddCar() {
    this.bean.AllCar[this.bean.AllCar.length] = { PLATE_NUMBER: '川A' };
  }

  GoBack() {
    this.navCtrl.pop();
  }
}
