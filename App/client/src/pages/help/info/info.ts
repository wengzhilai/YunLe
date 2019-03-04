import {Component} from '@angular/core';
import {NavController, NavParams, ActionSheetController} from 'ionic-angular';
import {CommonService} from "../../../Service/Common.Service";
import {ToPostService} from "../../../Service/ToPost.Service";
import {HelpEditPage} from "../edit/edit";
import {TaskHandlePage} from "../../task/handle/handle";


@Component({
  selector: 'help-info',
  templateUrl: 'info.html',
  providers: [CommonService, ToPostService]

})
export class HelpInfoPage {
  public bean:any = {AllFiles: [],AllFlow:[],NextButton:[]};

  constructor(public navCtrl:NavController,
              public params:NavParams,
              public alertCtrl:ActionSheetController,
              public commonService:CommonService,
              public toPostService:ToPostService) {

  }
  ionViewWillEnter() {
    if (this.params.get("id") == null) {
      this.commonService.hint('参数有误')
      return;
    }
    this.toPostService.SingleFun("RescueSingle", this.params.get("id")).then((currMsg)=> {
      if (currMsg.IsError == true) {
        this.commonService.hint(currMsg.Message);
      } else {
        this.bean = currMsg;
      }
    });
  }


  showBigImage(url,title) {
    this.commonService.FullScreenImage(url, title);
  }

  GoEdit()
  {
    this.navCtrl.push(HelpEditPage,{id:this.bean.ID})
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
