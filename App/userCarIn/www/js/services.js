/**
 * Created by Administrator on 2016/8/7.
 */
var mainService = angular.module('starter.services', [])
    //违章查询
    .factory('queryVehFac', ["CarIn", "$rootScope", "storageUserFac", "common", function (CarIn, $rootScope, storageUserFac, common) {
      var ClientPeccancyUrl = CarIn.api + CarIn.ClientPeccancy,
        ClientPeccancy2Url = CarIn.api + CarIn.ClientPeccancy2,
        ClientPeccancy1Url = CarIn.api + CarIn.ClientPeccancy1,
        msg = {};

      return {
        clientPeccancy1: function (Code) {
          var obj = {
            userId: "",
            authToken: storageUserFac.getUserAuthToken(),
            para: [
              {K: "Code", V: Code}
            ]
          };
          return $.post(ClientPeccancy1Url, obj,
            //回调函数
            function (response) {
              console.log(response);
              if (response.IsError) {
                common.showError(response);
              } else {
                msg = response;
                $rootScope.$broadcast('clientPeccancy1.Update');
              }
            },
            //返回类型
            "json").error(function (err) {
            common.showError(err);
          });
        },
        clientPeccancy2: function (PicCode, IdNo) {
          var obj = {
            userId: 0,
            authToken: storageUserFac.getUserAuthToken(),
            para: [
              {K: "PicCode", V: PicCode},
              {K: "IdNo", V: IdNo}

            ]
          };
          return $.post(ClientPeccancy2Url, obj,
            //回调函数
            function (response) {
              console.log(response);
              if (response.IsError) {
                common.showError(response);
              } else {
                msg = response;
                $rootScope.$broadcast('clientPeccancy2.Update');
              }
            },
            //返回类型
            "json").error(function (err) {
            common.showError(err);
          });
        },
        clientPeccancy: function (IdNo) {
          var obj = {
            userId: 0,
            authToken: storageUserFac.getUserAuthToken(),
            para: [
              {K: "IdNo", V: IdNo}
            ]
          };
          return $.post(ClientPeccancyUrl, obj,
            //回调函数
            function (response) {
              console.log(response);
              if (response.IsError) {
                if (response.Message == "验证码过期") {
                  msg = response;
                  $rootScope.$broadcast('clientPeccancy.Update');
                } else {
                  common.showError(response);
                }
              } else {
                msg = response;
                $rootScope.$broadcast('clientPeccancy.Update');
              }
            },
            //返回类型
            "json").error(function (err) {
            common.showError(err);
          });
        }, getCurrentMes: function () {
          common.hideLoading();
          return msg;
        }

      }


    }])

    .factory('testFac', ["common", "CarIn", "$rootScope", "Storage", "common", function (common, CarIn, $rootScope, Storage, common) {
      var msg = {},
        baseUrl = 'http://192.168.2.132/ci59',
        logUrl = baseUrl + '/WebApi/VillageVisitList';
      return {
        login: function (obj) {
          console.log(obj);
          common.showLoading();
          common.clearAll();
          return $.post(logUrl, obj,
            //回调函数
            function (response) {
              console.log(response);
              if (response.IsError) {
                common.showError(response);
              } else {
                msg = response;
                $rootScope.$broadcast('testLogin.Update');
              }
            },
            //返回类型
            "json").error(function (err) {
            msg = {IsError: true, Message: '测试登录失败'};
            common.showError(msg);
          });
        },
        toTest: function (url, obj) {
          //common.showLoading();
          obj = {
            authToken: 'd951b4aa294c48a9a9cf8a2279b9c568',
            pageSize: 20,
            currentPage: 1,
            _searchKey: "[{key:'zz',value:'xx'}]",
            inStr: "aaaa"
          };
          console.log(obj);
          return $.post(logUrl, obj,
            //回调函数
            function (response) {
              console.log(response);
              if (response.IsError) {
                common.showError(response);
              } else {
                msg = response;
                $rootScope.$broadcast('test.Update');
              }
            },
            //返回类型
            "json").error(function (err) {
            common.showError(err);
          });
        }, getCurrentMes: function () {
          common.hideLoading();
          return msg;
        }
      }
    }])
    /**保险**/
    .factory('carFac', ["CarIn", "$rootScope", "storageUserFac", "common", function (CarIn, $rootScope, storageUserFac, common) {
      var searCarUrl = CarIn.api + CarIn.QueryCar,
        msg = {},
        getCarListUrl = CarIn.api + CarIn.CarList,
        getCarByIdUrl = CarIn.api + CarIn.CarList,
        saveUrl = CarIn.api + CarIn.CarSave,
        delCarUrl = CarIn.api + CarIn.CarDelete,
        setDefaultUrl = CarIn.api + CarIn.CarSetDefault,

        orderSaveUrl = CarIn.api + CarIn.orderSave;
      return {
        orderSave: function (ent) {
          common.showLoading();
          var obj = {
            userId: 0,
            authToken: storageUserFac.getUserAuthToken(),
            saveKeys: 'CAR_ID,ORDER_TYPE,LANG,LAT,REMARK',
            entity: ent
          };
          return $.post(orderSaveUrl, obj,
            //回调函数
            function (response) {
              console.log(response);
              if (response.IsError) {
                common.showError(response);
              } else {
                msg = response;
                $rootScope.$broadcast('orderSave.Update');
              }
            },
            //返回类型
            "json").error(function (err) {
            msg = {IsError: true, Message: '代审车保存失败'};
            common.showError(msg);
          });
        },
        delCar: function (id) {
          common.showLoading();
          var obj = {
            userId: 0, authToken: storageUserFac.getUserAuthToken(), id: id
          };
          console.log(obj);
          return $.post(delCarUrl, obj,
            //回调函数
            function (response) {
              console.log(response);
              if (response.IsError) {
                common.showError(response);
              } else {
                msg = response;
                $rootScope.$broadcast('CarDel.Update');
              }
            },
            //返回类型
            "json").error(function (err) {
            msg = {IsError: true, Message: '删除车辆失败'};
            common.showError(msg);
          });
        },
        setDefault: function (id) {
          common.showLoading();
          var obj = {
            userId: 0, authToken: storageUserFac.getUserAuthToken(), id: id
          };
          console.log(obj);
          return $.post(setDefaultUrl, obj,
            //回调函数
            function (response) {
              console.log(response);
              if (response.IsError) {
                common.showError(response);
              } else {
                msg = response;
                $rootScope.$broadcast('CarSetDefault.Update');
              }
            },
            //返回类型
            "json").error(function (err) {
            msg = {IsError: true, Message: '设为默认车辆失败'};
            common.showError(msg);
          });
        },
        save: function (ent) {
          common.showLoading();
          var obj = {
            userId: 0,
            authToken: storageUserFac.getUserAuthToken(),
            saveKeys: 'ID,PLATE_NUMBER,ENGINE_NUMBER,FRAME_NUMBER,CAR_TYPE,PRICE,BRAND,REG_DATA,DRIVING_PIC_ID',
            entity: ent
          };
          // alert( window.JSON.stringify(obj));
          return $.post(saveUrl, obj,
            //回调函数
            function (response) {
              console.log(response);
              if (response.IsError) {
                common.showError(response);
              } else {
                msg = response;
                $rootScope.$broadcast('Car.Update');
              }
            },
            //返回类型
            "json").error(function (err) {
            msg = {IsError: true, Message: '保存车辆失败'};
            common.showError(msg);
          });
        },
        getCarList: function (obj) {
          common.showLoading();
          obj.authToken = storageUserFac.getUserAuthToken();

          console.log(obj);
          return $.post(getCarListUrl, obj,
            //回调函数
            function (response) {
              console.log(response);
              if (response.IsError) {
                common.showError(response);
              } else {
                msg = response;
                $rootScope.$broadcast('CarList.Update');
              }
            },
            //返回类型
            "json").error(function (err) {
            msg = {IsError: true, Message: '获取我的爱车列表失败'};
            common.showError(msg);
          });
        },
        hasNextPage: function () {
          if (msg.totalPage <= msg.currentPage) {
            return false;
          }
          if (msg.IsError == true) {
            return false;
          } else {
            return true;
          }
        },
        getInsureByCar: function (searObj) {
          common.showLoading();
          var obj = {
            userId: 0,
            authToken: storageUserFac.getUserAuthToken(),
            entity: searObj
          };
          return $.post(searInsureUrl, obj,
            //回调函数
            function (response) {
              console.log(response);
              if (response.IsError) {
                common.showError(response);
              } else {
                msg = response;
                $rootScope.$broadcast('Car.SearInsureByCarUpdate');
              }
            },
            //返回类型
            "json").error(function (err) {
            msg = {IsError: true, Message: '查询保险公司出错！'};
            common.showError(msg);
          });
        },
        carSearch: function (searObj) {
          common.showLoading();
          var obj = {
            userId: 0,
            authToken: storageUserFac.getUserAuthToken(),
            id: 0,
            para: [
              {
                K: 'lateNumber',
                V: searObj.lateNumber,
                T: '=='
              },
              {
                K: 'userName',
                V: searObj.userName,
                T: '=='
              }
            ]
          };
          return $.post(searCarUrl, obj,
            //回调函数
            function (response) {
              console.log(response);
              if (response.IsError) {
                common.showError(response);
              } else {
                msg = response;
                $rootScope.$broadcast('Car.SearUpdate');
              }
            },
            //返回类型
            "json").error(function (err) {
            msg = {IsError: true, Message: '查询车辆信息出错！'};
            common.showError(msg);
          });

        },
        getCurrentMes: function () {
          common.hideLoading();
          return msg;
        }
      };
    }])

    .factory('sendCodeFac', ["CarIn", "$rootScope", "common", function (CarIn, $rootScope, common) {
      var scUrl = CarIn.api + CarIn.SendCode,
        msg = {};
      return {
        senCode: function (obj) {
          common.showLoading();
          console.log(obj);
          return $.post(scUrl, obj,
            //回调函数
            function (response) {
              console.log(response);
              if (response.IsError) {
                common.showError(response);
              } else {
                msg = response;
                $rootScope.$broadcast('sendCode.Update');
              }
            },
            //返回类型
            "json").error(function (err) {
            msg = {IsError: true, Message: '登录失败'};
            common.showError(msg);
          });
        },
        getCurrentMes: function () {
          common.hideLoading();
          return msg;
        }
      }
    }])
  ;




/**
 * Created by wengzhilai on 2016/9/20.
 */
mainService.factory('baiduMap', ["$rootScope", "$cordovaGeolocation", "Storage", "CarIn", "common", function ($rootScope, $cordovaGeolocation, Storage, CarIn, common) {
  var posOptions = {timeout: 10000, enableHighAccuracy: false},
    msg = {},
    reLocation = {},
    storageKey = 'user',
    user = Storage.get(storageKey);

  return {
    //在地图中查找关键字位置
    searBaidu: function (map, value) {
      common.showLoading();
      var options = {
        onSearchComplete: function (results) {
          if (local.getStatus() == BMAP_STATUS_SUCCESS) {
            map.clearOverlays();
            msg = results;
            $rootScope.$broadcast('baiduMap.Search');
          } else {
            msg = {IsError: true, Message: '搜索位置失败请使用准确的关键字！'};
            common.showError(msg);
          }

        }
      }
      var local = new BMap.LocalSearch(map, options);
      local.search(value);
    },
    //获取经纬度和地址
    getGeo: function (callBack, para) {
      var getAddressFun = this.getAddress;
      var option = {maximumAge: 3000, timeout: 10000, enableHighAccuracy: true};
      //是微信
      if (navigator.userAgent.toLowerCase().indexOf('micromessenger') > -1) {
        if (para == null || para.isLoading) {
          common.showLoading();
        }
        wx.getLocation({
          type: 'wgs84', // 默认为wgs84的gps坐标，如果要返回直接给openLocation用的火星坐标，可传入'gcj02'
          success: function (res) {
            var latitude = res.latitude; // 纬度，浮点数，范围为90 ~ -90
            var longitude = res.longitude; // 经度，浮点数，范围为180 ~ -180。
            var speed = res.speed; // 速度，以米/每秒计
            var accuracy = res.accuracy; // 位置精度
            reLocation = {"lng": longitude, "lat": latitude}
            BMap.Convertor.translate(reLocation, 0, function (location) {
              reLocation = location;
              common.hideLoading();
              getAddressFun(location, function (address) {
                reLocation.address = address;
                if (callBack) {
                  callBack(reLocation);
                }
              });
            });
          }
        });
      }
      else if(ionic.Platform.isAndroid() || ionic.Platform.isIOS()) {
        if (para == null || para.isLoading) {
          common.showLoading();
        }
        $cordovaGeolocation.getCurrentPosition(option).then(
          function (position) {
            //alert('纬度: '          + position.coords.latitude          + '\n' +
            //  '经度: '         + position.coords.longitude         + '\n' +
            //  '海拔: '          + position.coords.altitude          + '\n' +
            //  '水平精度: '          + position.coords.accuracy          + '\n' +
            //  '垂直精度: ' + position.coords.altitudeAccuracy  + '\n' +
            //  '方向: '           + position.coords.heading           + '\n' +
            //  '速度: '             + position.coords.speed             + '\n' +
            //  '时间戳: '         + position.timestamp                + '\n');

            reLocation = {"lng": position.coords.longitude, "lat": position.coords.latitude}
            BMap.Convertor.translate(reLocation, 0, function (location) {
              reLocation = location;
              common.hideLoading();
              getAddressFun(location, function (address) {
                reLocation.address = address;
                if (callBack) {
                  callBack(reLocation);
                }
              });
            });
          },
          function (err) {
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
            common.showError(errStr);
          });
      }

    },
    //获取地址
    getAddress: function (location, callBack) {
      common.showLoading();
      var url = "http://api.map.baidu.com/geocoder/v2/?ak=xpiepfbv6zvlVSbLGfMg5sr1&callback=renderReverse&location=" + location.lat + "," + location.lng + "&output=json&pois=0";
      $.ajax({
        url: url,
        type: 'GET',
        dataType: 'JSONP',
        success: function (jsonObj) {
          if (callBack) {
            callBack(jsonObj.result.formatted_address);
          }
          common.hideLoading();
        }
      });
    },
    getCurrentMes: function () {
      common.hideLoading();
      return msg;
    }
  }

}])

/**
 * Created by wengzhilai on 2016/9/19.
 */
mainService.factory('common', ["$rootScope", "$cordovaFileTransfer", "$cordovaGeolocation", "$cordovaCamera", "$cordovaImagePicker", "Storage", "CarIn", "$ionicLoading", "$window", "$ionicPopup", function ($rootScope,$cordovaFileTransfer, $cordovaGeolocation,
                                        $cordovaCamera, $cordovaImagePicker, Storage, CarIn, $ionicLoading,$window,$ionicPopup) {
    var GetAllDisUrl = CarIn.api + CarIn.GetAllDistrict,
        msg = {},
        loadingOpts = {
            template: '<ion-spinner icon="lines" class="spinner-calm"></ion-spinner>',
            duration: 10000
        },
        storageKey = 'user';
    return {
      clearAll: function () {
        Storage.clearAll();
      },
      getQueryString: function (name) {
        var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");
        var r = window.location.search.substr(1).match(reg);
        if (r != null) return unescape(r[2]);
        return null;
      },
      clearOnlyUser: function () {
        var user = Storage.get(storageKey);
        Storage.clearAll();
        Storage.set(storageKey, user);
      },
      showError: function (error) {
        this.hideLoading();
        if (error.IsError) {
          $ionicLoading.show({
            noBackdrop: true,
            template: error.Message,
            duration: 2000
          });
        } else {
          $ionicLoading.show({
            noBackdrop: true,
            template: error,
            duration: 2000
          });
        }
      },
      showLoading: function () {
        /*显示加载动画*/
        $ionicLoading.show(loadingOpts);
      },
      hideLoading: function () {
        $ionicLoading.hide();
      },
      dateFormat: function (dt, fmt) {
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
      },
      //验证身份证是否合法
      regExpIdNo: function (person_id, callBack) {
        if (person_id != "") {
          function Append_zore(temp) {
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
            //alert(birthday +":"+(dateStr.getFullYear()+"-"+ Append_zore(dateStr.getMonth()+1)+"-"+ Append_zore(dateStr.getDate())))
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
      },
      getCurrentMes: function () {
        this.hideLoading();
        return msg;
      },
      getBeanNameStr: function (bean) {
        var tmpArr = [];
        for (var item in bean) {
          var objV=eval("bean."+item);
          if(typeof(objV)!="string" || objV.indexOf('/Date(')!=0)
          {
            tmpArr.push(item);
          }
        }

        return tmpArr.join(",");
      },
      getCameraOption: function () {
        return {
          quality: CarIn.quality,
          destinationType: Camera.DestinationType.DATA_URL,
          sourceType: Camera.PictureSourceType.CAMERA,
          allowEdit: CarIn.isAllowEdit,
          encodingType: Camera.EncodingType.JPEG,
          targetWidth: CarIn.imgWidth,
          targetHeight: CarIn.imgHeight,
          popoverOptions: CameraPopoverOptions,
          saveToPhotoAlbum: CarIn.isSaveToPhotoAlbum,
          correctOrientation: CarIn.isCorrectOrientation
        };
      },
      getImageOption: function () {
        return {
          maximumImagesCount: CarIn.maximumImagesCount,
          width: CarIn.imgWidth,
          height: CarIn.imgHeight,
          quality: CarIn.quality
        };
      },
      callPhone: function (mobilePhone) {
        $window.location.href = "tel:" + mobilePhone;
      },
      hint:function (msg,title) {
        if (title == null || title == '') {
          title = '提示';
        }
        $ionicPopup.alert({
          title:title,
          template: msg
        });
      }
    }
}])

/**
 * Created by Administrator on 2016/8/24.
 */
mainService.factory('fileUpDel', ["CarIn", "$cordovaFileTransfer", "storageUserFac", "common", function (CarIn, $cordovaFileTransfer, storageUserFac, common) {
    var options = {},
        fileDelUrl = CarIn.api + "FileDel",
        fileUpUrl = CarIn.api + "FileUp?userId=" + storageUserFac.getUserId() + "&authToken=" + storageUserFac.getUserAuthToken();
    return {
        fileDel: function (id, callback) {
            common.showLoading();
            var postBean = {
                userId: 0,
                authToken: storageUserFac.getUserAuthToken(),
                id: id
            };
            console.log("请求["+fileDelUrl+"]参数：");
            console.log(postBean);
            return $.post(fileDelUrl, postBean,
                //回调函数
                function (response) {
                    console.log("返回结果：");
                    console.log(response);
                    if (response.IsError) {
                        common.showError(response);
                    } else {
                        console.log("回调方法：" + callback);
                        callback(response);
                        common.hideLoading();
                    }
                },
                //返回类型
                "json").error(function (err) {
                    console.log(err);
                    common.showError({IsError: true, Message: '删除文件失败'});
                });
        },
        saveFile: function (path, callback) {
            common.showLoading();
            return $cordovaFileTransfer.upload(fileUpUrl, path, options)
                .then(function (result) {
                    //alert(path);
                    if (result.IsError) {
                        common.showError(result);
                    } else {
                        callback(result);
                        common.hideLoading();
                    }
                }, function (err) {

                    common.showError({IsError: true, Message: '文件上传失败'});
                }, function (progress) {
                    // constant progress updates
                });
        }
    }
}])

/**
 * Created by wengzhilai on 2016/8/16.
 */
mainService.factory('fileUpFac', ["CarIn", "$ionicLoading", "$timeout", "$ionicPopup", "$cordovaImagePicker", "$ionicActionSheet", "$cordovaFileTransfer", "$cordovaCamera", "$rootScope", "storageUserFac", "common", "fileUpDel", "toPost", function (CarIn, $ionicLoading, $timeout, $ionicPopup, $cordovaImagePicker, $ionicActionSheet, $cordovaFileTransfer, $cordovaCamera, $rootScope, storageUserFac, common, fileUpDel, toPost) {
  var msg = {},
    options = {},
    fileDelUrl = CarIn.api + "FileDel",
    url = CarIn.api + "FileUp?userId=" + storageUserFac.getUserId() + "&authToken=" + storageUserFac.getUserAuthToken();
  var thisImgEvent = null;
  var retunBack = null;
  var fileId = 0;
  var thisScope;
  var nowSheet;
  function fileUpFac_upCallback(result) {
    fileId = window.JSON.parse(result.response).ID;
    thisImgEvent.attr("data-id", fileId);
    if (retunBack) {
      retunBack(window.JSON.parse(result.response));
    }
    nowSheet();
  }

  function DelFileCallBack() {
    fileId = 0;
    thisImgEvent.attr("src",'');
    if (retunBack) {
      retunBack({ID:0,URL:null});
    }
  }

  function WeiXinCallBack(fileEnt) {
    fileId = fileEnt.ID;
    thisImgEvent.attr("data-id", fileId.ID);
    if (retunBack) {
      retunBack(fileEnt);
    }
    nowSheet();
  }

  return {
    upImg: function (obj, OutFileId, callback,tmpScope) {
      console.log('参数')
      console.log(OutFileId)
      thisImgEvent = obj;
      fileId = OutFileId;
      retunBack = callback;
      thisScope=tmpScope
      nowSheet=$ionicActionSheet.show({
        buttons: [
          {text: '<b>相机</b>'},
          {text: '<b>图库</b>'},
          {text: '<b>资料库</b>'}
        ],
        destructiveText: '删除',
        titleText: '选择图片来源',
        cancelText: '关闭',
        cancel: function () {
        },
        destructiveButtonClicked: function () {
          fileUpDel.fileDel(fileId, DelFileCallBack);
        },
        buttonClicked: function (index) {
          if(index==2) {
            toPost.list("FileList", {}, function (currMsg) {
              if (currMsg.IsError) {
                return;
              }
              thisScope.allFileList = currMsg.data;
              thisScope.checkImg=function (ent) {
                WeiXinCallBack(ent)
                thisScope.myPopup.close()
              }
              var templateStr="";
              templateStr+='<div class="item item-body col" style="text-align: center" ng-repeat="o in allFileList">';
              templateStr+='  <img name="driverPicImg" src="{{o.URL|imgUrl}}" height="100px" ng-click="checkImg(o)" width="100px" />';
              templateStr+='  <p>{{o.NAME}}</p>';
              templateStr+='</div>';
              templateStr+='';
              thisScope.myPopup = $ionicPopup.show({
                template: templateStr,
                title: '资料库',
                subTitle: '请选择您上传过的图片',
                scope: thisScope,
                buttons: [
                  {text: '取消',
                    type: 'button-positive',
                  }
                ]
              });
            })

            return;
          }

          if (navigator.userAgent.toLowerCase().indexOf('micromessenger') > -1) {
            var sourceType=['album', 'camera'];
            switch (index) {
              case 0:
                sourceType = ['camera']
                break;
              case 1:
                sourceType = ['album']
                break;
            }
            wx.chooseImage({
              count: 1,
              needResult: 1,
              sizeType: ['original', 'compressed'], // 可以指定是原图还是压缩图，默认二者都有
              sourceType: sourceType, // 可以指定来源是相册还是相机，默认二者都有
              success: function (data) {
                localIds = data.localIds[0]; // 返回选定照片的本地ID列表，localId可以作为img标签的src属性显示图片
                wx.uploadImage({
                  localId: localIds, // 需要上传的图片的本地ID，由chooseImage接口获得
                  isShowProgressTips: 1, // 默认为1，显示进度提示
                  success: function (res) {
                    mediaId = res.serverId; // 返回图片的服务器端ID
                    var postBean = {
                      authToken: storageUserFac.getUserAuthToken(),
                      para: [{"K": "mediaId", "V": mediaId}]
                    };
                    toPost.Post("WeiXinFileUp", postBean, WeiXinCallBack);

                  },
                  fail: function (error) {
                    picPath = '';
                    localIds = '';
                    alert(JSON.stringify(error));
                  }
                });
              },
              fail: function (res) {
                alert(JSON.stringify(res));
              }

            });
            return;
          }
          else {
            switch (index) {
              case 0:
                $cordovaCamera.getPicture(common.getCameraOption()).then(function (imageData) {
                  thisImgEvent.attr("src", "data:image/jpeg;base64," + imageData);
                  thisImgEvent.attr("data-id", "");
                  fileUpDel.saveFile("data:image/jpeg;base64," + imageData, fileUpFac_upCallback);
                }, function (err) {
                  alert('选择相机错误')
                  // error
                });
                return;
              case 1:
                $cordovaImagePicker.getPictures(common.getImageOption())
                  .then(function (results) {
                    for (var i = 0; i < results.length; i++) {
                      thisImgEvent.attr("src", results[i]);
                      thisImgEvent.attr("data-id", "");
                      fileUpDel.saveFile(results[i], fileUpFac_upCallback);
                    }
                  }, function (error) {
                    alert('选择图库错误')
                    // error getting photos
                  });
                break;
              default:
                break;
            }
          }
        }
      });
    },
    FullScreenImage: function (url, nowscope,title) {
      if(title=null)title="";
      if (url == null || url == '' || url == undefined || url == "img/noPic.png") {
        return ;
      }
      if (navigator.userAgent.toLowerCase().indexOf('micromessenger') > -1) {
        wx.previewImage({
          current: url, // 当前显示图片的http链接
          urls: [url] // 需要预览的图片http链接列表
        });
        return;
      }
      if (ionic.Platform.isAndroid() || ionic.Platform.isIOS()) {

        PhotoViewer.show(url, title ,{share:true});
        return;
        var imageName = url.substr(url.lastIndexOf("/") + 1);
        var targetPath='';
        if(ionic.Platform.isAndroid())
        {
          targetPath = cordova.file.externalRootDirectory + imageName;
        }
        else if(ionic.Platform.isIOS())
        {
          targetPath = cordova.file.documentsDirectory + imageName;
        }
        var trustHosts = true
        var options = {};
        $cordovaFileTransfer.download(url, targetPath, options, trustHosts).then(function (result) {
          window.FullScreenImage.showImageURL(targetPath);
        }, function (err) {
          var errStr= JSON.stringify(err);
          alert('打开失败:' + errStr);
        }, function (progress) {
          $timeout(function () {
            var downloadProgress = (progress.loaded / progress.total) * 100;
            $ionicLoading.show({
              template: "已经下载：" + Math.floor(downloadProgress) + "%"
            });
            if (downloadProgress > 99) {
              $ionicLoading.hide();
            }
          })
        });
      }
      else {

        nowscope.Url = url;
        console.log(nowscope.Url);
        nowscope.myPopupClose = function () {
          myPopup.close()
        }
        var myPopup = $ionicPopup.show({
          template: '<img ng-click="myPopupClose()" style="display: block; margin: 0px;" src="{{Url}}"/>',
          title: '',
          scope: nowscope
        });
      }
    }
  }

}])

/**
 * Created by wengzhilai on 2016/9/20.
 */
mainService.factory('orderPay', ["$rootScope", "storageUserFac", "Storage", "toPost", "common", function ($rootScope, storageUserFac, Storage, toPost, common) {
    var posOptions = {timeout: 10000, enableHighAccuracy: false},
        msg = {},
        reLocation = {},
        storageKey = 'user',
        user = Storage.get(storageKey);

    return {
        //支付
        pay: function (orderId,callback) {
            //是微信
            if(navigator.userAgent.toLowerCase().indexOf('micromessenger')>-1) {
                var postBean = {
                    userId: 0,
                    authToken: storageUserFac.getUserAuthToken(),
                    para: [
                        {"K": "order_no", "V": orderId}
                    ],
                    entity: {
                        STATUS: '完成'
                    }
                };
                toPost.Post("PayWeixinSign", postBean, function(ent)
                {
                  if (ent.IsError) {
                    callback(ent);
                    return;
                  }
                  wx.chooseWXPay({
                    timestamp: ent.timeStamp, // 支付签名时间戳，注意微信jssdk中的所有使用timestamp字段均为小写。但最新版的支付后台生成签名使用的timeStamp字段名需大写其中的S字符
                    nonceStr: ent.nonceStr, // 支付签名随机串，不长于 32 位
                    package: ent.packageValue, // 统一支付接口返回的prepay_id参数值，提交格式如：prepay_id=***）
                    signType: ent.signType, // 签名方式，默认为'SHA1'，使用新版支付需传入'MD5'
                    paySign: ent.sign, // 支付签名
                    success: function (res) {
                      callback({IsError:false,Message:'支付成功'})
                    },
                    fail:function (res) {
                      callback({IsError:true,Message:'支付失败:'+JSON.stringify(res)})
                    }
                  });
                });
            }
            else
            {
              callback({IsError:true,Message:"目前只支持微信支付"})
            }
        }
    }

}])

/**
 * Created by wengzhilai on 2016/9/19.
 */
mainService.factory('Storage', ["$window", function ($window) {
    return {
        //存储单个属性
        set :function(key,value){
            $window.localStorage[key]=value;
        },
        //读取单个属性
        get:function(key,defaultValue){
            return  $window.localStorage[key] || defaultValue;
        },
        //存储对象，以JSON格式存储
        setObject:function(key,value){
            $window.localStorage[key]=JSON.stringify(value);
        },
        //读取对象
        getObject: function (key) {
            try
            {
                return JSON.parse($window.localStorage[key]||'{}');
            }catch(e)
            {
                return JSON.parse('{}');
            }
        },
        remove: function (key) {
            $window.localStorage[key]="";
            return $window.localStorage.removeItem(key);
        },
        clearAll: function () {
            return $window.localStorage.clear();
        }
    };
}])
/**
 * Created by wengzhilai on 2016/9/19.
 */
mainService.factory('storageUserFac', ["CarIn", "$rootScope", "Storage", function (CarIn, $rootScope, Storage) {
    var storageKey = 'user',
        user = Storage.getObject('user');
    return {
        setUser: function (ent) {
            user = ent;
            Storage.setObject(storageKey, ent);
        },
        getUser: function () {
            return Storage.getObject(storageKey);
        },
        remove:function(){
            return Storage.remove(storageKey);
        },
        getUserAuthToken: function () {
            return user.authToken;
        },
        //推荐码
        getUserPollCode: function () {
            return user.REQUEST_CODE;
        },
        getUserId: function () {
            return user.ID;
        },
        getUserName: function () {
            return user.NAME;
        },
        getUserPhone: function () {
            return user.phone;
        },
        getUserNowCar: function () {
            return user.NowCar;
        },
        //团队编号
        getTeamId: function () {
            return user.TEAM_ID;
        }

    }

}])
/**
 * Created by Administrator on 2016/8/19.
 */
mainService.factory('toPost', ["CarIn", "storageUserFac", "common", "$state", function (CarIn, storageUserFac, common,$state) {
  return {
    list: function (apiName, postBean, callback) {
      common.showLoading();
      postBean.authToken = storageUserFac.getUserAuthToken();
      postBean.userId = 0;
      if (postBean.pageSize == null || postBean.pageSize == 0) {
        postBean.pageSize = CarIn.pageSize;
      }
      if (postBean.currentPage == null || postBean.currentPage == 0) {
        postBean.currentPage = 1;
      }
      console.log("请求[" + apiName + "]列表参数：");
      console.log(postBean);
      return $.post(CarIn.api + apiName, postBean,
        //回调函数
        function (response) {
          console.log("返回结果：");
          console.log(response);
          if (response.IsError) {
            if(response.Message=='登录超时')
            {
              $state.go('login', {reload: true});  //路由跳转
            }
            common.showError(response);
          } else {
            common.hideLoading();
          }
          if (callback) {
            console.log("回调方法：" + callback);
            callback(response);
          }
        },
        //返回类型
        "json").error(function (err) {
        console.log(err);
        common.showError({IsError: true, Message: '获取列表失败'});
      });
    },
    saveOrUpdate: function (apiName, bean, callback) {
      var saveKeyStr = common.getBeanNameStr(bean);
      if (saveKeyStr == "") {
        alert("保存参数saveKeys不能为空");
        return;
      }
      common.showLoading();
      var postBean = {
        authToken: storageUserFac.getUserAuthToken(),
        saveKeys: saveKeyStr,
        entity: bean
      };
      console.log("请求[" + apiName + "]参数：");
      console.log(postBean);
      return $.post(CarIn.api + apiName, postBean,
        //回调函数
        function (response) {
          common.hideLoading();
          console.log("返回结果：");
          console.log(response);
          if (response.IsError) {
            if (response.Message == '登录超时') {
              $state.go('login', {reload: true});  //路由跳转
            }
            common.showError(response);
          }
          if (callback) {
            console.log("回调方法：" + callback);
            callback(response);
          }
        },
        //返回类型
        "json").error(function (err) {
        console.log("错误信息：");
        console.log(err);
        common.showError({IsError: true, Message: '更新数据失败【' + apiName + '】：错误代码('+err.status+')'});
      });
    },
    Post: function (apiName, postBean, callback) {
      common.showLoading();
      console.log("请求[" + apiName + "]参数：");
      console.log(postBean);
      return $.post(CarIn.api + apiName, postBean,
        //回调函数
        function (response) {
          common.hideLoading();
          console.log("返回结果：");
          console.log(response);
          if (response.IsError) {
            if(response.Message=='登录超时')
            {
              $state.go('login', {reload: true});  //路由跳转
            }
            common.showError(response);
          }
          if (callback) {
            console.log("回调方法：" + callback);
            callback(response);
          }
        },
        //返回类型
        "json").error(function (err) {
        console.log("错误信息：");
        console.log(err);
        var errBean={IsError: true, Message: '提交数据失败【' + apiName + '】'};
        common.showError(errBean);
        if (callback) {
          console.log("回调方法：" + callback);
          callback(errBean);
        }
      });
    },
    single: function (apiName, id, callback) {
      if (id == null && id == 0) {
        alert("查询id不能为空");
        return;
      }
      common.showLoading();
      var postBean = {
        userId: 0,
        authToken: storageUserFac.getUserAuthToken(),
        id: id
      };
      console.log("请求[" + apiName + "]参数：");
      console.log(postBean);
      return $.post(CarIn.api + apiName, postBean,
        //回调函数
        function (response) {
          common.hideLoading();
          console.log("返回结果：");
          console.log(response);
          if (response.IsError) {
            if(response.Message=='登录超时')
            {
              $state.go('login', {reload: true});  //路由跳转
            }
            common.showError(response);
          }
          if (callback) {
            console.log("回调方法：" + callback);
            callback(response);
          }
        },
        //返回类型
        "json").error(function (err) {
        console.log(err);
        common.showError({IsError: true, Message: '查询单条记录失败'});
      });
    }
  }
}]);

/**
 * Created by wengzhilai on 2016/8/7.
 */
mainService.factory('insureFac', ["CarIn", "$rootScope", "storageUserFac", "common", function (CarIn, $rootScope, storageUserFac, common) {
    var searCarUrl = CarIn.api + CarIn.QueryCar,
        searInsureUrl = CarIn.api + CarIn.QueryInsure,
        OrderInsureSingleUrl = CarIn.api + CarIn.OrderInsureSingle,
        OrderInsureSaveUrl = CarIn.api + CarIn.OrderInsureSave,
        getOrderListUrl = CarIn.api + CarIn.OrderList,
        OrderInsureListUrl = CarIn.api + CarIn.OrderInsureList,
        OrderSingleUrl=CarIn.api + CarIn.OrderSingle
        msg = {};
    return {
        hasNextPage: function () {
            if (msg.totalPage <= msg.currentPage) {
                return false;
            }
            if (msg.IsError == true) {
                return false;
            } else {
                return true;
            }
        },
        getOrderList: function (obj) {
            common.showLoading();
            obj.authToken = storageUserFac.getUserAuthToken(),
                obj.pageSize = CarIn.pageSize;
            console.log(obj);
            return $.post(getOrderListUrl, obj,
                //回调函数
                function (response) {
                    console.log(response);
                    if (response.IsError) {
                        common.showError(response);
                    } else {
                        msg = response;
                        $rootScope.$broadcast('insureList.Update');
                    }
                },
                //返回类型
                "json").error(function (err) {
                    msg = {IsError: true, Message: '获取投保订单列表失败'};
                    common.showError(msg);
                });

        },
        getOrderInsureList: function (obj) {
            common.showLoading();
            obj.authToken = storageUserFac.getUserAuthToken();
            obj.pageSize = CarIn.pageSize;
            console.log(obj);
            return $.post(OrderInsureListUrl, obj,
                //回调函数
                function (response) {
                    console.log(response);
                    if (response.IsError) {
                        common.showError(response);
                    } else {
                        msg = response;
                        $rootScope.$broadcast('insureList.Update');
                    }
                },
                //返回类型
                "json").error(function (err) {
                    msg = {IsError: true, Message: '获取投保订单列表失败'};
                    common.showError(msg);
                });

        },
        OrderInsureSave: function (ent) {
            common.showLoading();
            var obj = {
                userId: 0,
                authToken: storageUserFac.getUserAuthToken(),
                saveKeys: 'CAR_ID,CLIENT_ID,INSURER_ID,DELIVERY,SaveProductId',
                entity: ent
            };
            return $.post(OrderInsureSaveUrl, obj,
                //回调函数
                function (response) {
                    console.log(response);
                    if (response.IsError) {
                        common.showError(response);
                    } else {
                        msg = response;
                        $rootScope.$broadcast('Insure.OrderInsureSave');
                    }
                },
                //返回类型
                "json").error(function (err) {
                    msg = {IsError: true, Message: '保存保险订单出错！'};
                    common.showError(msg);
                });
        },
        OrderInsureSingle: function (id) {
            common.showLoading();
            var obj = {
                userId: 0,
                authToken: storageUserFac.getUserAuthToken(),
                id: id
            };
            return $.post(OrderInsureSingleUrl, obj,
                //回调函数
                function (response) {
                    console.log(response);
                    if (response.IsError) {
                        common.showError(response);
                    } else {
                        msg = response;
                        $rootScope.$broadcast('Insure.OrderSingle');
                    }
                },
                //返回类型
                "json").error(function (err) {
                    msg = {IsError: true, Message: '查询保险订单出错！'};
                    common.showError(msg);
                });
        },
        getInsureByCar: function (searObj) {
            common.showLoading();
            var obj = {
                userId: 0,
                authToken: storageUserFac.getUserAuthToken(),
                entity: searObj
            };
            return $.post(searInsureUrl, obj,
                //回调函数
                function (response) {
                    console.log(response);
                    if (response.IsError) {
                        common.showError(response);
                    } else {
                        msg = response;
                        $rootScope.$broadcast('Insure.SearInsureByCarUpdate');
                    }
                },
                //返回类型
                "json").error(function (err) {
                    msg = {IsError: true, Message: '查询保险公司出错！'};
                    common.showError(msg);
                });
        },
        carSearch: function (searObj) {
            common.showLoading();
            var obj = {
                userId: 0,
                authToken: user.authToken,
                id: 0,
                para: [
                    {
                        K: 'lateNumber',
                        V: searObj.lateNumber,
                        T: '=='
                    },
                    {
                        K: 'userName',
                        V: searObj.userName,
                        T: '=='
                    }
                ]
            };
            return $.post(searCarUrl, obj,
                //回调函数
                function (response) {
                    console.log(response);
                    if (response.IsError) {
                        common.showError(response);
                    } else {
                        msg = response;
                        $rootScope.$broadcast('Insure.SearUpdate');
                    }
                },
                //返回类型
                "json").error(function (err) {
                    msg = {IsError: true, Message: '查询车辆信息出错！'};
                    common.showError(msg);
                });

        },
        getCurrentMes: function () {
            common.hideLoading();
            return msg;
        }
    }
}])
/**
 * Created by wengzhilai on 2016/8/11.
 */
mainService.factory('orderFac', ["CarIn", "$rootScope", "storageUserFac", "common", function (CarIn, $rootScope, storageUserFac, common) {
    var url = CarIn.api + CarIn.OrderList,
        orderRescueListUrl = CarIn.api + CarIn.OrderRescueList,
        orderSaveUrl = CarIn.api + CarIn.orderSave,
        helpOrderSaveUrl = CarIn.api + CarIn.toHelpUrl,
        maintainOrderSaveUrl = CarIn.api + CarIn.toHelpUrl,
        rescueSingleUrl = CarIn.api + CarIn.rescueSingle,
        trialOrderSaveUrl = CarIn.api + CarIn.orderSave,
        OrderSingleUrl=CarIn.api + CarIn.OrderSingle,
    msg = {};
    return {
        finish: function () {
            var obj = {
                userId: 0,
                authToken: storageUserFac.getUserAuthToken(),
                saveKeys: 'STATUS',
                entity: {
                    STATUS: '完成'
                }
            };
            return $.post(maintainOrderSaveUrl, obj,
                //回调函数
                function (response) {
                    console.log(response);
                    if (response.IsError) {
                        common.showError(response);
                    } else {
                        msg = response;
                        $rootScope.$broadcast('finish.Update');
                    }
                },
                //返回类型
                "json").error(function (err) {
                    common.showError(err);
                });
        },
        trialOrderSave: function (ent) {
            common.showLoading();
            var obj = {
                userId: 0,
                authToken: storageUserFac.getUserAuthToken(),
                saveKeys: 'HITCH_TYPE,REACH_TYPE,CLIENT_NAME,CLIENT_PHONE,GARAGE_TYPE,ORDER_TYPE,REACH_TIME,ADDRESS',
                entity: ent
            };
            return $.post(trialOrderSaveUrl, obj,
                //回调函数
                function (response) {
                    console.log(response);
                    if (response.IsError) {
                        common.showError(response);
                    } else {
                        msg = response;
                        $rootScope.$broadcast('trialOrderSave.Update');
                    }
                },
                //返回类型
                "json").error(function (err) {
                    common.showError(err);
                });
        },
        maintainOrderSave: function (ent) {
            common.showLoading();
            var obj = {
                userId: 0,
                authToken: storageUserFac.getUserAuthToken(),
                saveKeys: 'GARAGE_ID,HITCH_TYPE,REACH_TYPE,CLIENT_NAME,CLIENT_PHONE,PLATE_NUMBER,CAR_TYPE,REMARK,GARAGE_TYPE,ORDER_TYPE,REACH_TIME,ADDRESS',
                entity: ent
            };
            return $.post(maintainOrderSaveUrl, obj,
                //回调函数
                function (response) {
                    console.log(response);
                    if (response.IsError) {
                        common.showError(response);
                    } else {
                        msg = response;
                        $rootScope.$broadcast('maintainOrderSave.Update');
                    }
                },
                //返回类型
                "json").error(function (err) {
                    common.showError(err);
                });
        },
        helpOrderSave: function (ent) {
            common.showLoading();
            var obj = {
                userId: 0,
                authToken: storageUserFac.getUserAuthToken(),
                saveKeys: 'GARAGE_ID,HITCH_TYPE,REACH_TYPE,CLIENT_NAME,CLIENT_PHONE,PLATE_NUMBER,CAR_TYPE,REMARK,GARAGE_TYPE,ORDER_TYPE,REACH_TIME,AllFiles',
                entity: ent
            };
            //alert(window.JSON.stringify(obj));
            return $.post(helpOrderSaveUrl, obj,
                //回调函数
                function (response) {
                    console.log(response);
                    if (response.IsError) {
                        common.showError(response);
                    } else {
                        msg = response;
                        $rootScope.$broadcast('helpOrderSave.Update');
                    }
                },
                //返回类型
                "json").error(function (err) {
                    msg = {IsError: true, Message: '发起救援失败！'};
                    common.showError(msg);
                });
        },
        /*获取救援单信息*/
        rescueSingle: function (id) {
            common.showLoading();
            var obj = {
                userId: 0,
                authToken: storageUserFac.getUserAuthToken(),
                id: id
            };
            return $.post(rescueSingleUrl, obj,
                //回调函数
                function (response) {
                    console.log(response);
                    if (response.IsError) {
                        common.showError(response);
                    } else {
                        msg = response;
                        $rootScope.$broadcast('order.rescueSingleUpdate');
                    }
                },
                //返回类型
                "json").error(function (err) {
                    msg = {IsError: true, Message: '获取跟踪失败'};
                    common.showError(msg);
                });
        },
        getOrderList: function (obj) {
            common.showLoading();
            obj.authToken = storageUserFac.getUserAuthToken(),
                obj.pageSize = CarIn.pageSize;
            console.log(obj);
            return $.post(url, obj,
                //回调函数
                function (response) {
                    console.log(response);
                    if (response.IsError) {
                        common.showError(response);
                    } else {
                        msg = response;
                        $rootScope.$broadcast('orderListFac.Update');
                    }
                },
                //返回类型
                "json").error(function (err) {
                    msg = {IsError: true, Message: '获取订单列表失败'};
                    common.showError(msg);
                });

        },
        OrderSingle: function (id) {
            common.showLoading();
            var obj = {
                userId: 0,
                authToken: storageUserFac.getUserAuthToken(),
                id: id
            };
            return $.post(OrderSingleUrl, obj,
                //回调函数
                function (response) {
                    console.log(response);
                    if (response.IsError) {
                        common.showError(response);
                    } else {
                        msg = response;
                        $rootScope.$broadcast('Insure.OrderSingle');
                    }
                },
                //返回类型
                "json").error(function (err) {
                    msg = {IsError: true, Message: '查询订单出错！'};
                    common.showError(msg);
                });
        },
        /* 获取救援列表*/
        getOrderRescueList: function (obj) {
            common.showLoading();
            obj.authToken = storageUserFac.getUserAuthToken(),
                obj.pageSize = CarIn.pageSize;
            console.log(obj);
            return $.post(orderRescueListUrl, obj,
                //回调函数
                function (response) {
                    console.log(response);
                    if (response.IsError) {
                        common.showError(response);
                    } else {
                        msg = response;
                        $rootScope.$broadcast('orderRescueListFac.Update');
                    }
                },
                //返回类型
                "json").error(function (err) {
                    msg = {IsError: true, Message: '获取订单列表失败'};
                    common.showError(msg);
                });

        },
        delOrder: function (id) {

        },
        getCurrentMes: function () {
            common.hideLoading();
            return msg;
        },
        hasNextPage: function () {
            if (msg.totalPage <= msg.currentPage) {
                return false;
            }
            if (msg.IsError == true) {
                return false;
            } else {
                return true;
            }

        }
    }
}])
/**
 * Created by Administrator on 2016/8/23.
 */
mainService.factory('trialFac', ["CarIn", "toPost", "storageUserFac", function (CarIn, toPost, storageUserFac) {
  var getTrialListUrl = CarIn.TrialList,
    trialSaveUrl = CarIn.TrialSave,
    getSingleTrialUrl = CarIn.TrialSingle;
  return {
    //获得代审车列表
    getTrialList: function (postBean, callback) {
      postBean.searchKey = [{K: 'ORDER_TYPE', V: '审车', T: '=='}, {
        K: 'CLIENT_ID', V: storageUserFac.getUserId(), T: '=='
      }];
      toPost.list(getTrialListUrl, postBean, callback);
    },
    //代审车更新
    TrialSave: function (bean, callback) {
      toPost.saveOrUpdate(trialSaveUrl, bean, callback);
    },
    //通过id获得代审车订单
    getSingleTrial: function (id, callback) {
      toPost.single(getSingleTrialUrl, id, callback);
    }
  }
}]);
