import { AlertController, LoadingController, ToastController, Platform } from 'ionic-angular';
import { Transfer, TransferObject } from '@ionic-native/transfer';
import { Injectable } from '@angular/core';
import { PhotoViewer } from '@ionic-native/photo-viewer';
import { ImgUrlPipe } from "../pipe/imgUrl";

declare var wx;
declare var cordova;

@Injectable()
export class CommonService {
  public loader;
  constructor(
    private photoViewer: PhotoViewer,
    private transfer: Transfer,
    public loadingCtrl: LoadingController,
    public toastCtrl: ToastController,
    public plt: Platform,
    public alertCtrl: AlertController) {
  }

  getQueryString(name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");
    var r = window.location.search.substr(1).match(reg);
    if (r != null) return r[2];
    return null;
  }

  showError(error) {
    this.hideLoading();
    var msg = "";
    if (error.IsError) {
      msg = error.Message;
    } else {
      msg = error;
    }
    console.log(msg);
    let toast = this.toastCtrl.create({
      message: msg,
      duration: 2000,
      position: 'bottom'
    });
    toast.present(toast);

  };

  showLoading(msg = "加载中...") {
    this.loader = this.loadingCtrl.create({
        content: msg,
        duration: 10000
      });
      this.loader.present();
  };

  hideLoading() {
    if (this.loader != null) {
      this.loader.dismiss();
    }
  }

  dateFormat(dt, fmt) {
    var o = {
      "M+": dt.getMonth() + 1,                 //月份
      "d+": dt.getDate(),                    //日
      "h+": dt.getHours(),                   //小时
      "m+": dt.getMinutes(),                 //分
      "s+": dt.getSeconds(),                 //秒
      "q+": Math.floor((dt.getMonth() + 3) / 3), //季度
      "S": dt.getMilliseconds()             //毫秒
    };
    if (/(y+)/.test(fmt))
      fmt = fmt.replace(RegExp.$1, (dt.getFullYear() + "").substr(4 - RegExp.$1.length));
    for (var k in o)
      if (new RegExp("(" + k + ")").test(fmt))
        fmt = fmt.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k]) : (("00" + o[k]).substr(("" + o[k]).length)));
    return fmt;
  };

  //验证身份证是否合法
  regExpIdNo(person_id, callBack) {
    if (person_id != "") {
      var Append_zore = function (temp) {
        if (temp < 10) {
          return "0" + temp;
        }
        else {
          return temp;
        }
      }

      //获取证件号码
      var aCity = {
        11: "北京",
        12: "天津",
        13: "河北",
        14: "山西",
        15: "内蒙古",
        21: "辽宁",
        22: "吉林",
        23: "黑龙江",
        31: "上海",
        32: "江苏",
        33: "浙江",
        34: "安徽",
        35: "福建",
        36: "江西",
        37: "山东",
        41: "河南",
        42: "湖北",
        43: "湖南",
        44: "广东",
        45: "广西",
        46: "海南",
        50: "重庆",
        51: "四川",
        52: "贵州",
        53: "云南",
        54: "西藏",
        61: "陕西",
        62: "甘肃",
        63: "青海",
        64: "宁夏",
        65: "新疆",
        71: "台湾",
        81: "香港",
        82: "澳门",
        91: "国外"
      };
      //合法性验证
      var sum = 0;
      //出生日期
      var birthday;
      //错误信息
      var errorMsg = "";
      //验证长度与格式规范性的正则
      var pattern = new RegExp(/(^\d{15}$)|(^\d{17}(\d|x|X)$)/i);
      if (pattern.exec(person_id)) {
        //验证身份证的合法性的正则
        pattern = new RegExp(/^[1-9]\d{7}((0\d)|(1[0-2]))(([0|1|2]\d)|3[0-1])\d{3}$/);
        if (pattern.exec(person_id)) {
          //获取15位证件号中的出生日期并转位正常日期
          birthday = "19" + person_id.substring(6, 8) + "-" + person_id.substring(8, 10) + "-" + person_id.substring(10, 12);
        }
        else {
          person_id = person_id.replace(/x|X$/i, "a");
          //获取18位证件号中的出生日期
          birthday = person_id.substring(6, 10) + "-" + person_id.substring(10, 12) + "-" + person_id.substring(12, 14);
          //校验18位身份证号码的合法性
          for (var i = 17; i >= 0; i--) {
            sum += (Math.pow(2, i) % 11) * parseInt(person_id.charAt(17 - i), 11);
          }
          if (sum % 11 != 1) {
            errorMsg = "身份证号码不符合国定标准，请核对！";
            if (callBack) {
              callBack(errorMsg);
            } else {
              alert(errorMsg);
            }
            //$("#person_id").val("");
            return false;
          }
        }
        //检测证件地区的合法性
        if (aCity[parseInt(person_id.substring(0, 2))] == null) {
          errorMsg = "证件地区未知，请核对！";
          if (callBack) {
            callBack(errorMsg);
          } else {
            alert(errorMsg);
          }
          return false;
        }
        var dateStr = new Date(birthday.replace(/-/g, "/"));
        if (birthday != (dateStr.getFullYear() + "-" + Append_zore(dateStr.getMonth() + 1) + "-" + Append_zore(dateStr.getDate()))) {
          errorMsg = "证件出生日期非法！";
          if (callBack) {
            callBack(errorMsg);
          } else {
            alert(errorMsg);
          }
          return false;
        }
        return true;
      }
      else {
        errorMsg = "证件号码格式非法！";
        if (callBack) {
          callBack(errorMsg);
        } else {
          alert(errorMsg);
        }
        return false;
      }
    }
    else {
      errorMsg = "请输入证件号！";
      if (callBack) {
        callBack(errorMsg);
      } else {
        alert(errorMsg);
      }
      return false;
    }
  }

  callPhone(mobilePhone) {
    window.location.href = "tel:" + mobilePhone;
  }

  getBeanNameStr(bean) {
    var tmpArr = [];
    for (var item in bean) {
      var objV = bean[item];
      if (typeof (objV) != "string" || objV.indexOf('/Date(') != 0) {
        tmpArr.push(item);
      }
    }

    return tmpArr.join(",");
  }

  hint(msg, title = null) {
    if (title == null || title == '') {
      title = '提示';
    }
    let alert = this.alertCtrl.create({
      title: title,
      subTitle: msg,
      buttons: ['确定']
    });
    alert.present();
  }
  showLongToast(msg) {
    let toast = this.toastCtrl.create({
      message: msg,
      duration: 2000,
    });
    toast.present();
  }
  FullScreenImage(url, picTitle) {
    if(picTitle==null){
      picTitle="图片";
    }
    if (url == null || url == '' || url == undefined || url == "img/noPic.png") {
      return;
    }
    url = new ImgUrlPipe().transform(url);
    console.log(url);
    if (navigator.userAgent.toLowerCase().indexOf('micromessenger') > -1) {
      console.log('微信');
      console.log(url);
      wx.previewImage({
        current: url, // 当前显示图片的http链接
        urls: [url] // 需要预览的图片http链接列表
      });
      return;
    }
    if (this.plt.is('ios') || this.plt.is('android')) {
      console.log('真机');
      var imageName = url.substr(url.lastIndexOf("/") + 1);
      let targetPath = '';
      if (this.plt.is('android')) {
        targetPath = cordova.file.dataDirectory + imageName;
      }
      else if (this.plt.is('ios')) {
        targetPath = cordova.file.dataDirectory + imageName;
      }
      const fileTransfer: TransferObject = this.transfer.create();

      fileTransfer.download(url, targetPath).then((result) => {
        this.photoViewer.show(targetPath, picTitle, { share: false });
      }, (err) => {
        var errStr = JSON.stringify(err);
        alert('打开失败:' + errStr);
      });
    }
    else {
      this.hint('<img style="display: block; margin: 0px;" src="' + url + '"/>', '图片')
    }
  }
  FormValidMsg(userForm, validationMessages) {
    var formErrors: any = {};
    if (!userForm) {
      return;
    }
    for (const field in userForm.value) {
      formErrors[field] = '';
      const control = userForm.get(field);
      if (control && control.dirty && !control.valid) {
        for (const key in control.errors) {
          const messages = validationMessages[field];
          if (messages != null && messages[key] != null) {
            formErrors[field] += messages[key];
          }
          else {
            var keyStr = key;
            switch (key) {
              case "required":
                keyStr = '不能为空';
                break
              case "minlength":
                keyStr = '文字太少了';
                break
              case "required":
                keyStr = '不能为空';
                break
            }
            formErrors[field] += keyStr;
          }
        }
      }
    }
    return formErrors;
  }
}
