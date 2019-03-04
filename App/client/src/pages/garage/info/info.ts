import {Component, OnInit} from '@angular/core';
import {NavController, NavParams, ActionSheetController} from 'ionic-angular';
import {CommonService} from "../../../Service/Common.Service";
import {ToPostService} from "../../../Service/ToPost.Service";

@Component({
  selector: 'garage-info',
  templateUrl: 'info.html',
  providers: [CommonService, ToPostService]

})
export class GrageInfoPage implements OnInit {
  public bean:any = {AllFiles: [],AllFlow:[],NextButton:[]};

  constructor(public navCtrl:NavController,
              public params:NavParams,
              public alertCtrl:ActionSheetController,
              public commonService:CommonService,
              public toPostService:ToPostService) {
    if (this.params.get("id") == null) {
      this.commonService.hint('参数有误')
      return;
    }
    this.toPostService.SingleFun("GarageSingle", this.params.get("id")).then((currMsg)=> {
      if (currMsg.IsError == true) {
        this.commonService.hint(currMsg.Message);
      } else {
        this.bean = currMsg;
      }
    });
  }

  ngOnInit() {

  }

  showBigImage(url,title) {
    this.commonService.FullScreenImage(url,title);
  }

  GoBack() {
    this.navCtrl.pop();
  }

}
