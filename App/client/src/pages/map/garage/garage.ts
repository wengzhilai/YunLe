import { CommonService } from "../../../Service/Common.Service";
import { ToPostService } from "../../../Service/ToPost.Service";
import { NavController, NavParams, ViewController } from 'ionic-angular';
import { Component, ViewChild, ElementRef } from '@angular/core';
import { AppGlobal } from "../../../Classes/AppGlobal";
import { FileUpService } from "../../../Service/FileUp.Service";
import { AlertController } from 'ionic-angular';
import { BaiduMapService } from "../../../Service/BaiduMap.Service";
import { HomePage } from "../../home/home";
import { GrageInfoPage } from "../../garage/info/info";

declare var BMap;
declare var BMAP_ANIMATION_BOUNCE;

@Component({
  selector: 'map-garage',
  templateUrl: 'garage.html',
  providers: [CommonService, ToPostService, FileUpService, BaiduMapService]
})
export class MapGaragePage {
  @ViewChild('allmap') mapElement: ElementRef;
  allmap: any;
  public map: any;
  public BMAP_ANIMATION_BOUNCE: any;
  public myMark: any = null;
  public counter: any = null;

  constructor(public fileUpService: FileUpService,
    public params: NavParams,
    public alertCtrl: AlertController,
    public baiduMapService: BaiduMapService,
    public navCtrl: NavController,
    public viewCtrl: ViewController,
    public commonService: CommonService,
    public toPostService: ToPostService
  ) {

  }
  ionViewWillEnter() {
    if (AppGlobal.getUser().ID == null) {
      this.navCtrl.push(HomePage);
    }
    // 定义高度
    console.log(this.mapElement.nativeElement)
    this.map = new BMap.Map(this.mapElement.nativeElement);
    this.map.centerAndZoom(new BMap.Point(104.072366, 30.66367), 11);
    this.toPostService.single("RescueQuery", null, (currMsg) => {
      this.garageCallback(currMsg)
      this.getNewMark();
    });
  }

  /**
   * 获取当前位置
   */
  getNewMark() {
    this.map.removeOverlay(this.myMark);
    this.baiduMapService.getGeo((location) => {
      var point = new BMap.Point(location.lng, location.lat);
      var myIcon = new BMap.Icon("img/icon_geo.png", new BMap.Size(23, 25));
      this.myMark = new BMap.Marker(point, { icon: myIcon });  // 创建标注
      this.map.addOverlay(this.myMark);              // 将标注添加到地图中
      this.map.panTo(point);
    }, { 'isLoading': false });
  }
  /**
   * 显示所有维修站
   */
  garageCallback(currMsg) {
    for (var i = 0; i < currMsg.data.length; i++) {
      var new_point = new BMap.Point(currMsg.data[i].LANG, currMsg.data[i].LAT);
      var myIcon = new BMap.Icon("img/icon_gcoding.png", new BMap.Size(23, 25));
      var marker = new BMap.Marker(new_point, { icon: myIcon });  // 创建标注
      marker.data = currMsg.data[i];
      this.map.addOverlay(marker);              // 将标注添加到地图中
      marker.addEventListener("click", (e) => {
        let confirm = this.alertCtrl.create({
          title: e.target.data.NAME,
          message: '地址：' + e.target.data.ADDRESS + '<p>座机：' + e.target.data.PHONE + '</p><p>手机：' + e.target.data.REMARK + '</p>',
          buttons: [
            {
              text: '确定',
              handler: () => {
                var garage = {
                  garageId: e.target.data.ID,
                  garageName: e.target.data.NAME
                };
                this.params.get('callBack')(garage);
                this.navCtrl.pop();
              }
            },
            {
              text: '查看详情',
              handler: () => {
                this.navCtrl.push(GrageInfoPage, { id: e.target.data.ID });
              }
            }
          ]
        });
        confirm.present()
      });
    }
  }
  GoBack() {
    this.navCtrl.pop();
  }
}
