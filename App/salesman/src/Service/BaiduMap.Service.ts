/**
 * Created by wengzhilai on 2017/1/12.
 */
import { Http,Jsonp,URLSearchParams  } from '@angular/http';
import {NavController} from "ionic-angular/index";
import {Injectable} from '@angular/core';
import {Platform} from 'ionic-angular';
import {CommonService} from "./Common.Service";
import {AppGlobal} from "../Classes/AppGlobal";
import { Geolocation } from '@ionic-native/geolocation';
import {ToPostService} from "./ToPost.Service";
import 'rxjs/add/operator/toPromise';
import 'rxjs/add/operator/map';

declare var  BMAP_STATUS_SUCCESS;
declare var  BMap;
declare var  wx;

@Injectable()
export class BaiduMapService {
  public posOptions = {timeout: 10000, enableHighAccuracy: false};
  public msg = {};
  public reLocation:any = {};
  public storageKey = 'user';
  public user = AppGlobal.getUser()
  private thisCallBack:any;


  constructor(
    private geolocation: Geolocation,
    private http:Http,
              private jsonp:Jsonp,
              public toPostService:ToPostService,
              private commonService:CommonService,
              public plt:Platform,
              public navCtrl:NavController) {

  }

  searBaidu(map, value, callBack) {
    this.commonService.showLoading()
    var options = {
      onSearchComplete: (results)=> {
        if (local.getStatus() == BMAP_STATUS_SUCCESS) {
          map.clearOverlays();
          this.msg = results;
          callBack(results)
        } else {
          this.commonService.hideLoading()

          this.msg = {IsError: true, Message: '搜索位置失败请使用准确的关键字！'};
          this.commonService.showError(this.msg);
        }
      }
    }
    var local = new BMap.LocalSearch(map, options);
    local.search(value);
  }

  getGeo(callBack, para) {
    this.thisCallBack=callBack
    var option = {maximumAge: 3000, timeout: 10000, enableHighAccuracy: true};
    //是微信
    if (navigator.userAgent.toLowerCase().indexOf('micromessenger') > -1) {
      if (para == null || para.isLoading) {
        this.commonService.showLoading()
      }
      wx.getLocation({
        type: 'wgs84', // 默认为wgs84的gps坐标，如果要返回直接给openLocation用的火星坐标，可传入'gcj02'
        success: (res)=>{
          var latitude = res.latitude; // 纬度，浮点数，范围为90 ~ -90
          var longitude = res.longitude; // 经度，浮点数，范围为180 ~ -180。
          // var speed = res.speed; // 速度，以米/每秒计
          // var accuracy = res.accuracy; // 位置精度
          this.reLocation = {"lng": longitude, "lat": latitude}
          BMap.Convertor.translate(this.reLocation, 0, (location)=> {
            // alert(JSON.stringify(location));
            this.reLocation = location;
            this.commonService.hideLoading();
            // this.thisCallBack(this.reLocation);
            this.getAddress(this.reLocation, (address)=> {
              this.reLocation.address = address;
              if (this.thisCallBack) {
                this.thisCallBack(this.reLocation);
              }
            });

          });
        }
      });
    }
    else if (this.plt.is('ios') || this.plt.is('android')) {
      if (para == null || para.isLoading) {
        this.commonService.showLoading()
      }
      this.geolocation.getCurrentPosition(option).then(
        (position)=> {
          this.reLocation = {"lng": position.coords.longitude, "lat": position.coords.latitude}
          BMap.Convertor.translate(this.reLocation, 0, (location)=> {
            this.reLocation = location;
            this.commonService.hideLoading()
            this.getAddress(location, (address)=> {
              this.reLocation.address = address;
              if (this.thisCallBack) {
                this.thisCallBack(this.reLocation);
              }
            });
          });
        }, 
        (err)=> {
          var errStr = "";
          switch (err.code) {
            case 1 :
              errStr = ("用户选了不允许");//用户选了不允许
              break;
            case 2:
              errStr = ("连不上GPS卫星，或者网络断了");
              //
              break;
            case 3:
              errStr = ("超时了");//超时了
              break;
            default:
              errStr = ("未知错误");//未知错误，其实是err.code==0的时候
              break;
          }
          this.commonService.showError(errStr);
        });
    }

  };

  //获取地址
  getAddress(location, callBack) {
    let params = new URLSearchParams();
    params.set('ak', 'xpiepfbv6zvlVSbLGfMg5sr1');
    params.set('output', 'json');
    params.set('pois', '0');
    params.set('location', location.lat + "," + location.lng);
    params.set('format', 'json');
    params.set('callback', 'JSONP_CALLBACK');
    var url = "http://api.map.baidu.com/geocoder/v2/";
    return this.jsonp.get(url, {search: params})
      .map(response => response.json())
      .subscribe((res:any) => {
        console.log("返回结果：");
        if (callBack) {
          callBack(res.result.formatted_address);
        }
        console.log(res);
      });
  };

  handleError(error:any):Promise<any> {
    console.error('An error occurred', error); // for demo purposes only
    return Promise.reject(error.message || error);
  }
}
