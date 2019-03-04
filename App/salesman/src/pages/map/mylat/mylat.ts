import { CommonService } from "../../../Service/Common.Service";
import { ToPostService } from "../../../Service/ToPost.Service";
import { NavController, NavParams, ViewController } from 'ionic-angular';
import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { FileUpService } from "../../../Service/FileUp.Service";
import { AlertController } from 'ionic-angular';
import { BaiduMapService } from "../../../Service/BaiduMap.Service";
import { HelpEditPage } from "../../help/edit/edit";

declare var BMap;
declare var BMAP_STATUS_SUCCESS;
@Component({
  selector: 'map-mylat',
  templateUrl: 'mylat.html',
  providers: [CommonService, ToPostService, FileUpService, BaiduMapService]
})
export class MapMylatPage implements OnInit {
  @ViewChild('allmap') mapElement: ElementRef;

  public garage: any;
  public map: any;
  public myLocation: any;
  public url: any = "editHelpOrder";
  public searchKey: any = ""
  public allPlace: any = []
  public ShowList: any = true
  public nowPoint: any;
  public nowMarker: any;
  constructor(public params: NavParams,
    public baiduMapService: BaiduMapService,
    public navCtrl: NavController,
    public commonService: CommonService,
    public toPostService: ToPostService,
    public alertCtrl: AlertController,
    public viewCtrl: ViewController
  ) {

  }

  ngOnInit() {
    console.log(this.params.data)
    this.myLocation = this.params.get("mylocation");
    this.map = new BMap.Map(this.mapElement.nativeElement);
    this.map.addEventListener("click", (e) => {
      this.createMark(e)
    }, true);
    this.map.centerAndZoom(new BMap.Point(104.072366, 30.66367), 13);
    this.map.enableScrollWheelZoom();   //启用滚轮放大缩小，默认禁用
    this.map.enableContinuousZoom();    //启用地图惯性拖拽，默认禁用
    this.map.closeInfoWindow();
    this.map.disableDoubleClickZoom();
    //表示有坐标传过来
    if (this.myLocation != null && this.myLocation.lng != null) {
      this.createMark({ "title": "", "point": { "lng": this.myLocation.lng, "lat": this.myLocation.lat } });
    } else {
      this.baiduMapService.getGeo((myLocation) => {
        this.createMark({ "title": myLocation.address, "point": { "lng": myLocation.lng, "lat": myLocation.lat } });
      }, null);
    }
  }


  toEditHelp() {
    this.navCtrl.push(HelpEditPage, {
      location: this.myLocation,
      garage: this.params.get("garage"),
      id: this.params.get("id")
    })
  }

  createMark(e) {
    //alert(e.point.lng + "," + e.point.lat);
    this.map.clearOverlays(); //清除地图上所有标记
    this.nowPoint = new BMap.Point(e.point.lng, e.point.lat);
    var myIcon = new BMap.Icon("img/icon_gcoding.png", new BMap.Size(23, 25));
    this.map.setCenter(this.nowPoint);
    this.map.panTo(this.nowPoint);
    this.nowMarker = new BMap.Marker(this.nowPoint, { icon: myIcon });  // 创建标注
    this.map.addOverlay(this.nowMarker);              // 将标注添加到地图中
    this.nowMarker.enableDragging();
    this.myLocation = { lng: e.point.lng, lat: e.point.lat }
    this.nowMarker.addEventListener("click", (e) => {
      this.selectMark(e)
    }, true);
  }

  selectMark(e) {
    if (this.nowMarker != null) {
      this.myLocation.lng = this.nowMarker.point.lng
      this.myLocation.lat = this.nowMarker.point.lat
    }
    console.log(this.myLocation)

    if (this.myLocation == null) {
      this.commonService.hint('请在地图上选择点')
      return
    }
    console.log(this.myLocation);
    this.baiduMapService.getAddress(this.myLocation, (address) => {
      this.myLocation.address = address;
      console.log(this.myLocation);
      this.params.get('callBack')(this.myLocation);
      this.viewCtrl.dismiss()
    });
  }

  GoBack() {
    this.viewCtrl.dismiss()
  }
}
