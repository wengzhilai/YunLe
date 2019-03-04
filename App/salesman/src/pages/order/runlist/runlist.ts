import {CommonService} from "../../../Service/Common.Service";
import {ToPostService} from "../../../Service/ToPost.Service";
import {NavController, NavParams} from 'ionic-angular';
import { Component}  from '@angular/core';
import {InsureEditPage} from "../../insure/edit/edit";
import {InsureInfoPage} from "../../insure/info/info";
import {HelpInfoPage} from "../../help/info/info";


@Component({
  selector: 'runlist-list',
  templateUrl: 'runlist.html',
  providers: [CommonService,ToPostService]

})
export class RunlistListPage {
  public bean = {
    userId: 0,
    authToken: '',
    pageSize: 0,
    id: 0,
    currentPage: 0,
    searchKey: [
      {K: "STATUS", V: "完成", T: '!='}
    ],
    orderBy: []
  };
  public lists:any = [];

  constructor(public navCtrl:NavController,
              public params:NavParams,
              public commonService:CommonService,
              public toPostService:ToPostService) {}
  /*只加载一次*/
  ionViewDidLoad(){
    console.log("ionViewDidLoad");
  }

  ionViewWillEnter() {
    if (this.params.get("clientId") != null) {
      this.bean.searchKey[this.bean.searchKey.length] = {K: "CLIENT_ID", T: '==', V: this.params.get("clientId")};
    }
    this.doRefresh(null);
  }
  doRefresh(refresher) {
    console.log("下拉刷新");
    this.bean.currentPage = 1;
    this.toPostService.ListFun("OrderList", this.bean).then((currMsg:any)=> {
      if (refresher != null)
        refresher.complete();
      if (currMsg.IsError == true) {
        this.commonService.hint(currMsg.Message)
      } else {
        this.lists = currMsg.data;
      }
    });
  }

  doInfinite(infiniteScroll) : Promise<any> {
    console.log('加载更多...');
    if (this.bean.currentPage == 0) {
      this.bean.currentPage = 1;
    } else {
      this.bean.currentPage++;
    }
    return new Promise((resolve) => {
    this.toPostService.ListFun("OrderList", this.bean).then((currMsg:any)=> {
      if (currMsg.IsError == true) {
        this.commonService.hint(currMsg.Message)
      } else {
        this.lists = this.lists.concat(currMsg.data);
      }
      if (currMsg.totalPage <= currMsg.currentPage) {
          infiniteScroll.enable(false);
        }
        else {
          infiniteScroll.enable(true);
        }
        resolve();
      });
    });
  };

  GoBack() {
    this.navCtrl.pop();
  }

  GoAdd() {
    this.navCtrl.push(InsureEditPage)
  }

  GoInfo(ent:any) {
    switch (ent.ORDER_TYPE) {
      case "救援":
        this.navCtrl.push(HelpInfoPage, {id: ent.ID})
        break;
      case "审车":
        this.navCtrl.push(HelpInfoPage, {id: ent.ID})
        break;
      case "维修":
        this.navCtrl.push(HelpInfoPage, {id: ent.ID})
        break;
      case "保养":
        this.navCtrl.push(HelpInfoPage, {id: ent.ID})
        break;
      case "投保":
        this.navCtrl.push(InsureInfoPage, {id: ent.ID})
        break;
    }
  }
}
