import { Component, OnInit } from '@angular/core';
import { NavController, NavParams, ActionSheetController } from 'ionic-angular';
import { CommonService } from "../../../Service/Common.Service";
import { ToPostService } from "../../../Service/ToPost.Service";
import { CustomerEditPage } from "../edit/edit";
import { InsureEditPage } from "../../insure/edit/edit";
import { HelpEditPage } from "../../help/edit/edit";
import { OrderListPage } from "../../order/list/list";


@Component({
  selector: 'customer-info',
  templateUrl: 'info.html',
  providers: [CommonService, ToPostService]

})
export class CustomerInfoPage implements OnInit {
  public bean: any = { NowCar: {} };

  constructor(public navCtrl: NavController,
    public params: NavParams,
    public alertCtrl: ActionSheetController,
    public commonService: CommonService,
    public toPostService: ToPostService) {

  }

  ngOnInit() {

  }

  /*只加载一次*/
  ionViewDidLoad() {
    console.log("ionViewDidLoad");
  }
  ionViewWillEnter() {
    this.toPostService.SingleFun("ClientSingle", this.params.get("id")).then((currMsg) => {
      if (currMsg.IsError == true) {
        this.commonService.hint(currMsg.Message);
      } else {
        this.bean = currMsg;
      }
    });
  }

  showBigImage(url) {
    this.commonService.FullScreenImage(url, this);
  }

  ShowMenu() {
    let actionSheet = this.alertCtrl.create({
      title: '操作用户',
      cssClass: 'action-sheets-basic-page',
      buttons: [
        {
          text: '修改资料',
          icon: 'create',
          handler: () => {
            this.navCtrl.push(CustomerEditPage, { id: this.bean.ID })
          }
        },
        {
          text: '重置密码',
          icon: 'key',
          handler: () => {
            let confirm = this.alertCtrl.create({
              title: '重置密码',
              buttons: [
                {
                  text: '取消',
                  handler: () => {
                    console.log('Disagree clicked');
                  }
                },
                {
                  text: '确认',
                  handler: () => {
                    this.toPostService.single("SalesmanClientRestPwd", this.bean.ID, (currMsg) => {
                      if (currMsg.IsError) {
                        this.commonService.hint(currMsg.Message)
                      } else {
                        this.commonService.hint(currMsg.Message)
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
          text: '订单',
          icon: 'reorder',
          handler: () => {
            this.navCtrl.push(OrderListPage, { client: this.bean })
          }
        },
        {
          text: '投保',
          icon: 'cash',
          handler: () => {
            this.navCtrl.push(InsureEditPage, { customer: this.bean })
          }
        },
        {
          text: '救援',
          icon: 'add-circle',
          handler: () => {
            this.navCtrl.push(HelpEditPage, { user: this.bean })
          }
        }
      ]
    });
    actionSheet.present();
  }

  GoBack() {
    this.navCtrl.pop();
  }

  GoAdd() {
    // this.navCtrl.push()
  }
}
