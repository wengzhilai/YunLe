import { Component } from '@angular/core';
import { NavController, NavParams } from 'ionic-angular';
import { CommonService } from "../../../Service/Common.Service";

@Component({
  selector: 'team-teaminfo',
  templateUrl: 'teaminfo.html',
  providers: [CommonService]
})
export class TeamTeaminfoPage {

  public lists: any = [];

  constructor(
    public commonService: CommonService,
    public navCtrl: NavController,
    public navParams: NavParams) {
    console.log("页面【TeamTeaminfoPage】参数", navParams.data);
    //集团传过来的参数
    if (this.navParams.get("teamInfo") != null) {
      this.lists = this.navParams.get("teamInfo");
    }
  }
  showBigImage(url) {
    console.log(url)
    this.commonService.FullScreenImage(url, '推荐码');
  }
  ionViewDidLoad() {
    console.log('ionViewDidLoad TeamPage');
  }

  GoBack() {
    this.navCtrl.pop();
  }
}
