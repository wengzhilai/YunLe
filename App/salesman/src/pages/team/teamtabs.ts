import { Component,ViewChild } from '@angular/core';
import {NavController, NavParams, Tabs, Tab} from 'ionic-angular';
import {OrderListPage} from "../order/list/list";
import {TeamTeaminfoPage} from "./teaminfo/teaminfo";
import {ToPostService} from "../../Service/ToPost.Service";
import {CommonService} from "../../Service/Common.Service";
import {SalesmanListPage} from "../salesman/list/list";

/*
  Generated class for the Team page.

  See http://ionicframework.com/docs/v2/components/#navigation for more info on
  Ionic pages and navigation.
*/
@Component({
  selector: 'team-teamtabs',
  templateUrl: 'teamtabs.html',
  providers: [CommonService,ToPostService]
})
export class TeamTabsPage {
  @ViewChild('myTabs') tabRef: Tabs;
  @ViewChild('tab1') tab1: Tab;
  @ViewChild('tab2') tab2: Tab;
  @ViewChild('tab3') tab3: Tab;

  public  teamParams:any={teamInfo:null};
  public  customerParams:any={allSalesman:null};
  public  orderParams:any={allOrder:null};
  public title="团队信息";
  public bean= {
    userId: 0,
    authToken: '',
    pageSize: 0,
    id: 0,
    currentPage: 0,
    searchKey: [{K: "ORDER_TYPE", V: "投保", T: '=='}],
    orderBy: []
  };
  public lists:any= [];



  constructor(public navCtrl: NavController,
              public toPostService:ToPostService,
              public commonService:CommonService,
              public navParams: NavParams)  {
  }
  doRefresh(refresher) {
    
  }

  ionViewWillEnter() {
    this.bean.currentPage = 1;
    this.toPostService.list("TeamMyAll", this.bean,(currMsg:any)=> {
      if (currMsg.IsError == true) {
        this.commonService.hint(currMsg.Message)
      } else {
        this.lists = currMsg.data;
        this.teamParams.teamInfo=currMsg.data;
        this.customerParams.allSalesman=currMsg.data[0].allSalesman;
        this.orderParams.allOrder=currMsg.data[0].allOrder;

        this.tab1.rootParams=this.teamParams;
        this.tab2.rootParams=this.customerParams;
        this.tab3.rootParams=this.orderParams;

        this.tab1.root = TeamTeaminfoPage;
        this.tab2.root = SalesmanListPage;
        this.tab3.root = OrderListPage;
        this.tabRef.select(0);
      }
    });
  }

  GoBack(){
    this.navCtrl.pop();
  }

  IonSelect(title){
    this.title=title
  }
}
