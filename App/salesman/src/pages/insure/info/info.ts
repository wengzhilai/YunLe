import {Component} from '@angular/core';
import {NavController, NavParams, ActionSheetController} from 'ionic-angular';
import {CommonService} from "../../../Service/Common.Service";
import {ToPostService} from "../../../Service/ToPost.Service";
import {InsureEditPage} from "../edit/edit";
import {TaskHandlePage} from "../../task/handle/handle";


@Component({
  selector: 'insure-info',
  templateUrl: 'info.html',
  providers: [CommonService, ToPostService]

})
export class InsureInfoPage{
  public bean:any = {AllFiles: [],AllFlow:[],NextButton:[],Car:{}};
  public nowInsurer:any = {};

  constructor(public navCtrl:NavController,
              public params:NavParams,
              public alertCtrl:ActionSheetController,
              public commonService:CommonService,
              public toPostService:ToPostService) {
  }

  /*只加载一次*/
  ionViewDidLoad(){
    console.log("ionViewDidLoad");
  }
  ionViewWillEnter() {
    if (this.params.get("id") == null) {
      this.commonService.hint('参数有误')
      return;
    }
    this.toPostService.SingleFun("OrderInsureSingle", this.params.get("id")).then((currMsg)=> {
      if (currMsg.IsError == true) {
        this.commonService.hint(currMsg.Message);
      } else {
        this.bean = currMsg;
        for (var i = 0; i < this.bean.AllInsurePrice.length; i++) {
          if (this.bean.INSURER_ID == this.bean.AllInsurePrice[i].ID) {
            this.nowInsurer = this.bean.AllInsurePrice[i];
          }
        }
      }
    });
  }

  showBigImage(url,title) {
    this.commonService.FullScreenImage(url, title);
  }

  GoEdit()
  {
    this.navCtrl.push(InsureEditPage,{id:this.bean.ID})
  }

  GoBack() {
    this.navCtrl.pop();
  }

  GoAdd() {
    // this.navCtrl.push()
  }
  GoHandle(par:any)
  {
    this.navCtrl.push(TaskHandlePage,par);
  }
}
