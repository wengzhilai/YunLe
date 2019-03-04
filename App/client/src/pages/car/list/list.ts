import { CommonService } from "../../../Service/Common.Service";
import { ToPostService } from "../../../Service/ToPost.Service";
import { NavController, NavParams, AlertController, ActionSheetController } from 'ionic-angular';
import { Component } from '@angular/core';
import { CarEditPage } from "../edit/edit";
import { AppGlobal } from "../../../Classes/AppGlobal";


@Component({
  selector: 'car-list',
  templateUrl: 'list.html',
  providers: [CommonService, ToPostService]

})
export class CarListPage {
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
    return this.toPostService.ListFun("CarList", this.bean);
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
    this.navCtrl.parent.select(0);
  }
  GoAdd() {
    this.navCtrl.push(CarEditPage)
  }
  GoInfo(ent: any) {
  }

  ShowMenu(ent: any) {
    let actionSheet = this.actionSheet.create({
      title: '操作用户',
      cssClass: 'action-sheets-basic-page',
      buttons: [
        {
          text: '修改资料',
          icon: 'create',
          handler: () => {
            this.navCtrl.push(CarEditPage, { id: ent.ID })
          }
        },
        {
          text: '删除车辆',
          icon: 'key',
          handler: () => {
            let confirm = this.alertCtrl.create({
              title: '删除车辆',
              buttons: [
                {
                  text: '确认',
                  handler: () => {
                    this.toPostService.single("CarDelete", ent.ID, (currMsg) => {
                      if (currMsg.IsError) {
                        this.commonService.hint(currMsg.Message)
                      } else {
                        this.doRefresh(null)
                      }
                    })
                  }
                }
              ]
            });
            confirm.present();
          }
        },
        {
          text: '设为默认车辆',
          icon: 'reorder',
          handler: () => {
            this.toPostService.single("CarSetDefault", ent.ID, (currMsg) => {
              if (currMsg.IsError) {
                this.commonService.hint(currMsg.Message)
              } else {
                this.commonService.showLongToast('设为默认车辆成功')
                var user = AppGlobal.getUser();
                user.NowCar = currMsg;
                AppGlobal.setUser(user);
              }
            })
          }
        }
      ]
    });
    actionSheet.present();
  }

}
