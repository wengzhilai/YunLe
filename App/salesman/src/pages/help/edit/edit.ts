import { CommonService } from "../../../Service/Common.Service";
import { ToPostService } from "../../../Service/ToPost.Service";
import { Component } from '@angular/core';
import { AppGlobal } from "../../../Classes/AppGlobal";
import { Func } from "../../../Classes/Func";
import { FileUpService } from "../../../Service/FileUp.Service";
import { AlertController, App, NavController, NavParams, ViewController } from 'ionic-angular';
import { BaiduMapService } from "../../../Service/BaiduMap.Service";
import { MapMylatPage } from "../../map/mylat/mylat";
import { MapGaragePage } from "../../map/garage/garage";
import { DateToStringPipe } from "../../../pipe/DateToString";

@Component({
  selector: 'help-edit',
  templateUrl: 'edit.html',
  providers: [CommonService, ToPostService, FileUpService, BaiduMapService]
})
export class HelpEditPage {
  public OrderTypeStr = ['救援', '保养', '维修', '审车'];
  //当前位置
  public mylocation: any;
  //维修站
  public garage: any;
  public user: any = AppGlobal.getUser()
  public bean: any = {
    ID: 0,
    ORDER_NO: '',
    REACH_TYPE: '接车',
    ORDER_TYPE: '救援',
    REMARK: '',
    HITCH_TYPE: '',
    REACH_TIME: '',
    AllFiles: [],
    AllFlow: [],
    CLIENT_NAME: this.user.NAME,
    CLIENT_PHONE: this.user.LOGIN_NAME
  };

  constructor(public fileUpService: FileUpService,
    public navParams: NavParams,
    public baiduMapService: BaiduMapService,
    public navCtrl: NavController,
    public commonService: CommonService,
    public toPostService: ToPostService,
    public alertCtrl: AlertController,
    public viewCtrl: ViewController,
    public appCtrl: App) {
  }

  SetBean() {
    this.bean = {
      AllFiles: []
    }
    return this.bean;
  }

  ionViewDidLoad() {
    console.log("所有参数：")
    console.log(this.navParams.data)

    this.bean.REACH_TIME = new Func().dateFormat(new Date(), 'yyyy-MM-ddThh:mm');

    if (this.navParams.get("orderType")) {
      this.bean.ORDER_TYPE = this.navParams.get("orderType");
    }
    if (this.user.garage != null) {
      this.garage = { garageId: this.user.garage.ID, garageName: this.user.garage.NAME }
      this.bean.GARAGE_ID = this.garage.garageId;
      this.bean.GarageName = this.garage.garageName;
    }
    if (this.navParams.get("user")) {
      var thisClient = this.navParams.get("user");
      this.bean.CLIENT_NAME = thisClient.NAME;
      this.bean.CLIENT_PHONE = thisClient.LOGIN_NAME;
      this.bean.PLATE_NUMBER = thisClient.NowCar == null ? '' : thisClient.NowCar.PLATE_NUMBER;
      this.bean.CAR_TYPE = thisClient.NowCar == null ? '' : thisClient.NowCar.MODEL;
      this.bean.BRAND = thisClient.NowCar == null ? '' : thisClient.NowCar.BRAND;
      this.bean.MODEL = thisClient.NowCar == null ? '' : thisClient.NowCar.FRAME_NUMBER;
      this.bean.CAR_ID = thisClient.NowCar == null ? '' : thisClient.NowCar.ID;
      this.bean.CLIENT_ID = thisClient.ID;
    }
    else if (this.user.SALESMAN_ID != null) {
      var thisClient = this.user;
      this.bean.CLIENT_NAME = thisClient.NAME;
      this.bean.CLIENT_PHONE = thisClient.LOGIN_NAME;
      this.bean.PLATE_NUMBER = thisClient.NowCar == null ? '' : thisClient.NowCar.PLATE_NUMBER;
      this.bean.CAR_TYPE = thisClient.NowCar == null ? '' : thisClient.NowCar.MODEL;
      this.bean.BRAND = thisClient.NowCar == null ? '' : thisClient.NowCar.BRAND;
      this.bean.MODEL = thisClient.NowCar == null ? '' : thisClient.NowCar.FRAME_NUMBER;
      this.bean.CAR_ID = thisClient.NowCar == null ? '' : thisClient.NowCar.ID;
      this.bean.CLIENT_ID = thisClient.ID;
    }


    if (this.navParams.get("bean")) {
      this.bean = this.navParams.get("bean");
      this.bean.REACH_TIME = new DateToStringPipe().transform(this.bean.REACH_TIME, "yyyy-MM-dd hh:mm")
      this.bean.PICK_TIME = new DateToStringPipe().transform(this.bean.PICK_TIME, "yyyy-MM-dd hh:mm")
    }

    if (this.navParams.get("id") != null) {
      this.toPostService.SingleFun("RescueSingle", this.navParams.get("id")).then((currMsg) => {
        if (currMsg.IsError == true) {
          this.commonService.hint(currMsg.Message);
        } else {
          this.bean = currMsg;
          console.log(this.bean.PICK_TIME);
          this.bean.REACH_TIME = new DateToStringPipe().transform(this.bean.REACH_TIME, "yyyy-MM-dd")
          this.bean.PICK_TIME = new DateToStringPipe().transform(this.bean.PICK_TIME, "yyyy-MM-ddThh:mm")
          console.log(this.bean.PICK_TIME);
          for (var i = 0; i < this.bean.AllFiles.length; i++) {
            this.bean.AllFiles[i].indexNo = i;
          }
          this.garage = {
            garageId: this.bean.GARAGE_ID,
            garageName: this.bean.GarageName
          };
        }
      });
    }

    if (this.navParams.get("garage")) {
      this.garage = this.navParams.get("garage");
      this.bean.GARAGE_ID = this.garage.garageId;
      this.bean.GarageName = this.garage.garageName;
    }
    if (this.navParams.get("mylocation")) {
      this.mylocation = this.navParams.get("mylocation");
      this.bean.LANG = this.mylocation.lng;
      this.bean.LAT = this.mylocation.lat;
      this.bean.ADDRESS = this.mylocation.address;
    }
    if (location == null) {
      this.baiduMapService.getGeo(function (myLocation) {
        this.mylocation = { "lng": myLocation.lng, "lat": myLocation.lat }
        this.bean.LANG = myLocation.lng;
        this.bean.LAT = myLocation.lat;
        this.bean.ADDRESS = myLocation.address;
      }, null);
    }
  }
  callBackLocation = (par) => {
    return new Promise((resolve, reject) => {
      if (par != null) {
        this.mylocation = par;
        this.bean.LANG = this.mylocation.lng;
        this.bean.LAT = this.mylocation.lat;
        this.bean.ADDRESS = this.mylocation.address;
        resolve('ok');
      } else {
        reject(Error('error'));
      }
    })
  };
  selectLat() {
    let actionSheet = this.alertCtrl.create({
      title: '定位方式选择',
      cssClass: 'action-sheets-basic-page',
      buttons: [
        {
          text: '手动定位',
          handler: () => {
            this.appCtrl.getRootNav().push(MapMylatPage, {
              mylocation: this.mylocation,
              garage: this.garage,
              id: this.navParams.get("id"),
              bean: this.bean,
              reload: true,
              callBack: this.callBackLocation
            });  //路由跳转
          }
        },
        {
          text: '自动定位',
          handler: () => {
            console.log('自动定位');
            this.baiduMapService.getGeo((myLocation) => {
              this.mylocation = { "lng": myLocation.lng, "lat": myLocation.lat }
              this.bean.LANG = myLocation.lng;
              this.bean.LAT = myLocation.lat;
              this.bean.ADDRESS = myLocation.address;
            }, null);
          }
        }
      ]
    });
    actionSheet.present();
  };
  callBackGarage = (par) => {
    return new Promise((resolve, reject) => {
      if (par != null) {
        this.garage = par;
        console.log(this.garage);
        this.bean.GARAGE_ID = this.garage.garageId;
        this.bean.GarageName = this.garage.garageName;
        resolve('ok');
      } else {
        reject(Error('error'));
      }
    })
  };
  selectGarage() {
    this.appCtrl.getRootNav().push(MapGaragePage, {
      mylocation: this.mylocation,
      garage: this.garage,
      id: this.navParams.get("id"),
      bean: this.bean,
      reload: true,
      callBack: this.callBackGarage
    });  //路由跳转
  };

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

  SaveEnt() {

    if (this.bean.LANG == null || this.bean.LAT == null) {
      this.commonService.hint("车辆位置，还未设置");
      return;
    }
    this.toPostService.SaveOrUpdate("RescueSave", this.bean, (currMsg) => {
      if (!currMsg.IsError) {
        this.commonService.showLongToast("您的订单已经提交成功")
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
