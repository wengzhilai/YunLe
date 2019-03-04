import { CommonService } from "../../../Service/Common.Service";
import { ToPostService } from "../../../Service/ToPost.Service";
import { NavController, NavParams, Button } from 'ionic-angular';
import { Component, ViewChild, ElementRef } from '@angular/core';
import { InsureEditPage } from "../../insure/edit/edit";
import { InsureInfoPage } from "../../insure/info/info";
import { HelpInfoPage } from "../../help/info/info";
import { HelpEditPage } from "../../help/edit/edit";
import { AppGlobal } from "../../../Classes/AppGlobal";


@Component({
  selector: 'order-list',
  templateUrl: 'list.html',
  providers: [CommonService, ToPostService]

})
export class OrderListPage {
  @ViewChild('butRow') butRow: ElementRef
  @ViewChild('but1') but1: Button
  @ViewChild('but2') but2: Button
  @ViewChild('but3') but3: Button
  @ViewChild('but4') but4: Button

  orderType: string = "";
  clientName: string = "";
  showButtons = true;
  public bean = {
    userId: 0,
    authToken: '',
    pageSize: 0,
    id: 0,
    currentPage: 0,
    searchKey: [],
    orderBy: []
  };
  public lists: any = [];
  public isUpdateData: any = true //如果是集团数据，就不需要更新，因为已经把集团的所有用户已经传过来了
  public listsIns: any = [];
  public listsHelp: any = [];
  public listsTrial: any = [];
  public listsMaintain: any = [];

  constructor(
    public navCtrl: NavController,
    public navParams: NavParams,
    public commonService: CommonService,
    public toPostService: ToPostService) {
  }
  /*只加载一次*/
  ionViewDidLoad() {
    console.log("ionViewDidLoad");
  }

  ionViewWillEnter() {
    console.log("ionViewWillEnter");
    console.log("所有参数：")
    console.log(this.navParams.data)
    if (this.navParams.get("allOrder") != null) {
      this.lists = this.navParams.get("allOrder");
      this.isUpdateData = false;
      for (var i = 0; i < this.lists.length; i++) {
        switch (this.lists[i].ORDER_TYPE) {
          case "投保":
            this.listsIns[this.listsIns.length] = this.lists[i];
            break;
          case "救援":
            this.listsHelp[this.listsHelp.length] = this.lists[i];
            break;
          case "审车":
            this.listsTrial[this.listsTrial.length] = this.lists[i];
            break;
          case "维护":
            this.listsMaintain[this.listsMaintain.length] = this.lists[i];
            break;
          case "保养":
            this.listsMaintain[this.listsMaintain.length] = this.lists[i];
            break;
        }
      }
      this.lists = this.listsHelp;
    }
    else {
      if (this.navParams.get("clientId") != null) {
        this.bean.searchKey[this.bean.searchKey.length] = { K: "CLIENT_ID", T: '==', V: this.navParams.get("clientId") };
      }
      else{
        AppGlobal.LoadUser((v) => {
          if(v.RoleAllID.split(',').indexOf('4')==-1)
          {
            this.bean.searchKey[this.bean.searchKey.length] = { K: "CLIENT_ID", T: '==', V: v.ID };
          }
      })
        
      }
      if (this.navParams.get("client") != null) {
        this.bean.searchKey[this.bean.searchKey.length] = { K: "CLIENT_ID", T: '==', V: this.navParams.get("client").ID };
        this.clientName = this.navParams.get("client").NAME;
      }
      if (this.navParams.get("status") != null) {
        if(this.navParams.get("status")=='维修中'){
          this.bean.searchKey[this.bean.searchKey.length] = { K: "STATUS", T: '!=', V: '完成' };
        }
        else{
          this.bean.searchKey[this.bean.searchKey.length] = { K: "STATUS", T: '==', V: this.navParams.get("status") };
        }
      }
      if (this.navParams.get("type") != null) {
        this.showButtons = false;
        this.Show(this.navParams.get("type"));
      }
      else {
        this.doRefresh(null);
      }
    }
  }
  ionViewDidEnter() {
    console.log("ionViewDidEnter");
  }
  doRefresh(refresher) {
    console.log("下拉刷新");
    this.bean.currentPage = 1;
    this.toPostService.ListFun("OrderList", this.bean).then((currMsg) => {
      if (refresher != null)
        refresher.complete();
      if (currMsg.IsError == true) {
        this.commonService.hint(currMsg.Message)
      } else {
        this.lists = currMsg.data;
        this.commonService.showLongToast("总记录数[" + currMsg.totalCount + "]条,已展示记录数[" + this.lists.length + "]条")
      }
    });
  }
  doInfinite(infiniteScroll): Promise<any> {
    console.log('加载更多...');
    infiniteScroll.enable(false);
    if (this.bean.currentPage == 0) {
      this.bean.currentPage = 1;
    } else {
      this.bean.currentPage++;
    }
    return new Promise((resolve) => {
      this.toPostService.ListFun("OrderList", this.bean).then((currMsg: any) => {
        if (currMsg.IsError == true) {
          this.commonService.hint(currMsg.Message)
        } else {
          this.lists = this.lists.concat(currMsg.data);
          this.commonService.showLongToast("总记录数[" + currMsg.totalCount + "]条,已展示记录数[" + this.lists.length + "]条")
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
  }
  Show(type) {
    this.but1.color = "light";
    this.but2.color = "light";
    this.but3.color = "light";
    if (this.but4 != null) this.but4.color = "light";
    this.orderType = type
    switch (type) {
      case "救援":
        this.but1.color = "primary";
        break;
      case "审车":
        this.but2.color = "primary";
        break;
      case "维保":
        this.but3.color = "primary";
        break;
      case "投保":
        if (this.but4 != null) this.but4.color = "primary";
        break;
    }
    var isAdd = true;
    for (var i = 0; i < this.bean.searchKey.length; i++) {
      if (this.bean.searchKey[i].K == "ORDER_TYPE") {
        this.bean.searchKey[i].V = type;
        isAdd = false;
      }
    }
    if (isAdd) {
      this.bean.searchKey[this.bean.searchKey.length] = { K: "ORDER_TYPE", V: type, T: '==' };
    }

    if (this.navParams.get("allOrder") != null) {
      switch (type) {
        case "投保":
          this.lists = this.listsIns;
          break;
        case "救援":
          this.lists = this.listsHelp;
          break;
        case "审车":
          this.lists = this.listsTrial;
          break;
        case "维保":
          this.lists = this.listsMaintain;
          break;
      }
    }
    else {
      this.doRefresh(null);
    }
  }

  GoBack() {
    this.navCtrl.pop();
  }
  GoAdd() {
    switch (this.orderType) {
      case "投保":
        this.navCtrl.push(InsureEditPage, { customer: this.navParams.get("client") })
        break;
      case "救援":
        this.navCtrl.push(HelpEditPage, { orderType: this.orderType, user: this.navParams.get("client") })
        break;
      case "审车":
        this.navCtrl.push(HelpEditPage, { orderType: this.orderType, user: this.navParams.get("client") })
        break;
      case "维保":
        this.navCtrl.push(HelpEditPage, { orderType: this.orderType, user: this.navParams.get("client") })
        break;
      default:
        this.navCtrl.push(HelpEditPage, { orderType: this.orderType, user: this.navParams.get("client") })
        break;
    }
  }
  GoInfo(ent: any) {
    switch (ent.ORDER_TYPE) {
      case "救援":
        this.navCtrl.push(HelpInfoPage, { id: ent.ID })
        break;
      case "审车":
        this.navCtrl.push(HelpInfoPage, { id: ent.ID })
        break;
      case "维修":
        this.navCtrl.push(HelpInfoPage, { id: ent.ID })
        break;
      case "保养":
        this.navCtrl.push(HelpInfoPage, { id: ent.ID })
        break;
      case "投保":
        this.navCtrl.push(InsureInfoPage, { id: ent.ID })
        break;
    }
  }
}
