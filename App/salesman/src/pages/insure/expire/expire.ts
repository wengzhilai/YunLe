import {CommonService} from "../../../Service/Common.Service";
import {ToPostService} from "../../../Service/ToPost.Service";
import {NavController, NavParams} from 'ionic-angular';
import { Component}  from '@angular/core';
import {InsureInfoPage} from "../info/info";
import {InsureEditPage} from "../edit/edit";


@Component({
  selector: 'insure-expire',
  templateUrl: 'expire.html',
  providers: [CommonService,ToPostService]

})
export class InsureExpirePage  {
  public bean= {
    userId: 0,
    authToken: '',
    pageSize: 0,
    id: 0,
    currentPage: 0,
    searchKey: [{K: "IsWarm", V: "1", T: '=='}],
    orderBy: []
  };
  public lists:any= [];
  constructor(
              public navCtrl:NavController,
              public params:NavParams,
              public commonService:CommonService,
              public toPostService:ToPostService) {
    if (this.params.get("clientId") != null) {
      this.bean.searchKey[this.bean.searchKey.length]={K:"CLIENT_ID",T:'==',V:this.params.get("clientId")};
    }

    this.doRefresh(null);
  }

  doRefresh(refresher) {
    console.log("下拉刷新");
    this.bean.currentPage = 1;
      this.toPostService.ListFun("OrderInsureList", this.bean).then(currMsg=> {
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
    this.toPostService.ListFun("OrderList",this.bean).then((currMsg:any)=>{
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
    this.navCtrl.push(InsureEditPage)
  }
  GoInfo(ent:any) {
    this.navCtrl.push(InsureInfoPage, {id: ent.ID})
  }
}
