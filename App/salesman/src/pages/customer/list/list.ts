import {CommonService} from "../../../Service/Common.Service";
import {ToPostService} from "../../../Service/ToPost.Service";
import {NavController, NavParams} from 'ionic-angular';
import { Component} from '@angular/core';
import {CustomerInfoPage} from "../info/info";
import {CustomerEditPage} from "../edit/edit";


@Component({
  selector: 'customer-list',
  templateUrl: 'list.html',
  providers: [CommonService,ToPostService]

})
export class CustomerListPage  {
  public bean= {
    userId: 0,
    authToken: '',
    pageSize: 0,
    id: 0,
    currentPage: 0,
    searchKey: [],
    orderBy: []
  };
  public isUpdateData:any=true //如果是集团数据，就不需要更新，因为已经把集团的所有用户已经传过来了
  public lists:any= [];
  constructor(
              public navCtrl:NavController,
              public navParams: NavParams,
              public commonService:CommonService,
              public toPostService:ToPostService) {
    console.log("所有参数：")
    console.log(this.navParams.data)
    if(this.navParams.get("allSalesman")!=null)
    {
      this.lists=this.navParams.get("allSalesman");
      this.isUpdateData=false;
    }
    else {
      this.doRefresh(null);
    }
  }

  doRefresh(refresher) {
    console.log("下拉刷新");
    this.bean.currentPage = 1;
      this.toPostService.ListFun("SalesmanClientList", this.bean).then(currMsg=> {
          if (refresher != null)
            refresher.complete();
          if (currMsg.IsError == true) {
            this.commonService.hint(currMsg.Message)
          } else {
            this.lists = currMsg.data;
          }
        });
  }
  doInfinite(infiniteScroll): Promise<any>  {
    console.log('加载更多...');
    if (this.bean.currentPage == 0) {
      this.bean.currentPage = 1;
    } else {
      this.bean.currentPage++;
    }
    return new Promise((resolve) => {
    this.toPostService.ListFun("SalesmanClientList",this.bean).then((currMsg:any)=>{
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
  }

  GoBack(){
    this.navCtrl.pop();
  }
  GoAdd(){
    this.navCtrl.push(CustomerEditPage)
  }
  GoInfo(ent:any) {
    // this.navCtrl.push(TabsPage, {ID: ent.ID})
    this.navCtrl.push(CustomerInfoPage, {id: ent.ID})
  }
}
