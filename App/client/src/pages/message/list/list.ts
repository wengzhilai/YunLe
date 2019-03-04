import { CommonService } from "../../../Service/Common.Service";
import { ToPostService } from "../../../Service/ToPost.Service";
import { NavController, NavParams, AlertController, ActionSheetController } from 'ionic-angular';
import { Component } from '@angular/core';

@Component({
  selector: 'message-list',
  templateUrl: 'list.html',
  providers: [CommonService, ToPostService]

})
export class MessageListPage {
  public bean: any = {
    userId: 0,
    authToken: '',
    pageSize: 0,
    id: 0,
    currentPage: 0,
    searchKey: [],
    orderBy: []
  };
  public lists: any = [];
  constructor(
    public navCtrl: NavController,
    public params: NavParams,
    public commonService: CommonService,
    public alertCtrl: AlertController,
    public actionSheet: ActionSheetController,
    public toPostService: ToPostService) { }

  ionViewWillEnter() {
    this.doRefresh(null);
  }
  PostList() {
    return this.toPostService.ListFun("MessageGetAll", this.bean);
  }
  doRefresh(refresher) {
    console.log("下拉刷新");
    this.bean.currentPage = 1;
    this.PostList().then((currMsg) => {
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
    if (this.bean.currentPage == 0) {
      this.bean.currentPage = 1;
    } else {
      this.bean.currentPage++;
    }
    return new Promise((resolve) => {

      this.PostList().then((currMsg) => {
        infiniteScroll.complete();
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

  GoBack() {
    this.navCtrl.parent.select(1);
  }
  GoAdd() {
    // this.navCtrl.push(MessageEditPage)
  }
  GoInfo(ent: any) {
  }

}
