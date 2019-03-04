import { CommonService } from "../../../Service/Common.Service";
import { ToPostService } from "../../../Service/ToPost.Service";
import { NavController, NavParams, ViewController } from 'ionic-angular';
import { Component, OnInit } from '@angular/core';
import { AppGlobal } from "../../../Classes/AppGlobal";
import { FileUpService } from "../../../Service/FileUp.Service";
import { AlertController } from 'ionic-angular';
import { FormBuilder } from '@angular/forms';
import { Func } from "../../../Classes/Func";

@Component({
  selector: 'task-handle',
  templateUrl: 'handle.html',
  providers: [CommonService, ToPostService, FileUpService]
})
export class TaskHandlePage implements OnInit {
  public taskId: any;
  public butName: any;
  public taskFlowId: any;
  public orderId: any;
  public returnUrl: any;

  public bean: any = {
    AllFiles: [],
    orderId: '',
    taskID: '',
    taskFlowId: '',
    cost: '',
    body: '',
    fileIdStr: '',
    stauts: '',
    PickTime: ''
  };

  constructor(public fileUpService: FileUpService,
    public params: NavParams,
    private formBuilder: FormBuilder,
    public navCtrl: NavController,
    public commonService: CommonService,
    public toPostService: ToPostService,
    public alertCtrl: AlertController,
    public viewCtrl: ViewController
  ) {

  }
  SetBean() {
    this.bean = {
      AllFiles: [],
      orderId: '',
      taskID: '',
      taskFlowId: '',
      cost: '',
      body: '',
      fileIdStr: '',
      stauts: '',
      PickTime: ''
    }
    return this.bean;
  }

  ngOnInit() {
    if (this.params.get("taskId") != null) this.taskId = this.params.get("taskId");
    if (this.params.get("butName") != null) this.butName = this.params.get("butName");
    if (this.params.get("taskFlowId") != null) this.taskFlowId = this.params.get("taskFlowId");
    if (this.params.get("orderId") != null) this.orderId = this.params.get("orderId");
    if (this.params.get("returnUrl") != null) this.returnUrl = this.params.get("returnUrl");

    this.bean.taskID = this.taskId
    this.bean.stauts = this.butName;
    this.bean.taskFlowId = this.taskFlowId;
    this.bean.orderId = this.orderId;

    if (this.bean.stauts == '接车') {
      this.bean.PickTime = new Func().dateFormat(new Date(), "yyyy-MM-dd");
    }
  }

  AddImg() {
    var indexNo = this.bean.AllFiles.length;
    this.bean.AllFiles[indexNo] = { "indexNo": this.bean.AllFiles.length };
    this.upImg('allFile_' + indexNo)
  }
  upImg(key) {
    console.log(key);
    this.fileUpService.upImg(this, key, (Key: string, url: string, ID: number) => {
      switch (Key) {
        default:
          if (Key.indexOf('allFile_') == 0) {
            let indexNo = Key.replace('allFile_', '')
            for (var i = 0; i < this.bean.AllFiles.length; i++) {
              if (this.bean.AllFiles[i].indexNo == indexNo) {
                if (ID == null || ID == 0) {
                  this.bean.AllFiles.splice(i, 1); //删除
                  for (var x = 0; x < this.bean.AllFiles.length; x++) {
                    this.bean.AllFiles[x].indexNo = x;
                  }
                } else {
                  this.bean.AllFiles[i].ID = ID;
                  this.bean.AllFiles[i].URL = url;
                }
                break;
              }
            }
          }
          break;
      }
    });
  }

  showBigImage(url) {
    this.commonService.FullScreenImage(url, this);
  }

  save() {
    // if (this.userForm.invalid) {
    //   console.log(this.formErrors)
    //   console.log(this.userForm)
    //   this.commonService.hint('输入无效!')
    //   return;
    // }

    for (var i = 0; i < this.bean.AllFiles.length; i++) {
      if (this.bean.AllFiles[i].ID != null) {
        this.bean.fileIdStr += "," + this.bean.AllFiles[i].ID;
      }
    }

    if (this.bean.cost != null && this.bean.cost != '') {
      this.bean.body += "\r\n费用：" + this.bean.cost + '\r\n';
    }
    if (this.bean.PickTime != null && this.bean.PickTime != '') {
      this.bean.body += "\r\n接车时间：" + this.bean.PickTime + '\r\n';
    }

    var postBean = {
      authToken: AppGlobal.getUserAuthToken(),
      para: [
        { K: "orderId", V: this.bean.orderId },
        { K: "taskID", V: this.bean.taskID },
        { K: "taskFlowId", V: this.bean.taskFlowId },
        { K: "cost", V: this.bean.cost },
        { K: "body", V: this.bean.body },
        { K: "fileIdStr", V: this.bean.fileIdStr },
        { K: "stauts", V: this.bean.stauts }
      ]
    };
    this.toPostService.Post("OrderSaveStatus", postBean, (currMsg) => {
      if (!currMsg.IsError) {
        this.commonService.showLongToast("提交成功！")
        this.navCtrl.pop();
        // this.viewCtrl.dismiss();
        // switch (this.returnUrl) {
        //   case "InsureInfoPage":
        //     this.navCtrl.push(InsureInfoPage, {id: this.orderId})
        //     break;
        //   case "HelpInfoPage":
        //     this.navCtrl.push(HelpInfoPage, {id: this.orderId})
        //     break;
        // }
      }
      else {
        this.commonService.hint(currMsg.Message)
      }
    });
  }


  GoBack() {
    this.navCtrl.pop();
  }
}
