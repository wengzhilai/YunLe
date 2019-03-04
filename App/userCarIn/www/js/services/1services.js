/**
 * Created by Administrator on 2016/8/7.
 */
var mainService = angular.module('starter.services', [])
    //违章查询
    .factory('queryVehFac', function (CarIn, $rootScope, storageUserFac, common) {
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


    })

    .factory('testFac', function (common, CarIn, $rootScope, Storage, common) {
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
    })
    /**保险**/
    .factory('carFac', function (CarIn, $rootScope, storageUserFac, common) {
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
    })

    .factory('sendCodeFac', function (CarIn, $rootScope, common) {
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
    })
  ;



