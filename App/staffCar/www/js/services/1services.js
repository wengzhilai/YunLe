/**
 * Created by Administrator on 2016/8/7.
 */
var mainService = angular.module('starter.services', [])

/**保险**/

    .factory('messageFac', function (CarIn, $rootScope, storageUserFac, common) {
        var msg = {},
            returnUrl = CarIn.api + CarIn.MessageReply,
            getAllUrl = CarIn.api + CarIn.MessageGetAll,
            delUrl = CarIn.api + CarIn.MessageDelete,
            sendUrl = CarIn.api + CarIn.MessageSend;
        return {
            getMessageList: function (obj) {
                common.showLoading();
                obj.authToken = storageUserFac.getUserAuthToken();
                obj.pageSize = CarIn.pageSize;
                console.log(obj);
                return $.post(getAllUrl, obj,
                    //回调函数
                    function (response) {
                        console.log(response);
                        if (response.IsError) {
                            common.showError(response);
                        } else {
                            msg = response;
                            $rootScope.$broadcast('MessageList.Update');
                        }
                    },
                    //返回类型
                    "json").error(function (err) {
                        common.showError(err);
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
            sendMessage: function (ent) {
                common.showLoading();
                var obj = {
                    userId: 0,
                    authToken: storageUserFac.getUserAuthToken(),
                    saveKeys: 'ID,NAME,SEX,phone,ADDRESS,NowCar,LOGIN_NAME',
                    entity: ent
                };
                return $.post(sendUrl, obj,
                    //回调函数
                    function (response) {
                        console.log(response);
                        if (response.IsError) {
                            common.showError(response);
                        } else {
                            msg = response;
                            $rootScope.$broadcast('MessageSend.Update');
                        }
                    },
                    //返回类型
                    "json").error(function (err) {
                        common.showError(err);
                    });
            },
            getCurrentMes: function () {
                common.hideLoading();
                return msg;
            }

        };
    })

    .factory('carFac', function (CarIn, $rootScope, storageUserFac, common) {
        var searCarUrl = CarIn.api + CarIn.QueryCar,
            msg = {},
            saveUrl = CarIn.api + CarIn.QueryCar,
            getCarListUrl = CarIn.api + CarIn.CarList,
            searInsureUrl = CarIn.api + CarIn.QueryInsure;

        return {
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
                        common.showError(err);
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
                        common.showError(err);
                    });

            },
            getCurrentMes: function () {
                common.hideLoading();
                return msg;
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
        };
    })
  
    .factory('sendCodeFac', function (CarIn, $ionicLoading, $rootScope, common) {
        var scUrl = CarIn.api + CarIn.SendCode,
            msg = {};
        return {
            senCode: function (obj) {
                common.showLoading();
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

    .factory('helpFac', function (CarIn, $rootScope, storageUserFac, common) {
        var listUrl = CarIn.api + CarIn.HelpList,

            msg = {},
            toHelpUrl = CarIn.api + CarIn.toHelpUrl;
        return {
            toHelp: function (bean) {
                common.showLoading();
                var obj = {
                    userId: 0,
                    authToken: storageUserFac.getUserAuthToken(),
                    saveKeys: 'ORDER_TYPE,GARAGE_TYPE,REACH_TYPE,CLIENT_NAME,CLIENT_PHONE,PLATE_NUMBER,CAR_TYPE,BRAND,MODEL,REMARK',
                    entity: bean
                };
                return $.post(toHelpUrl, obj,
                    //回调函数
                    function (response) {
                        console.log(response);
                        if (response.IsError) {
                            common.showError(response);
                        } else {
                            msg = response;
                            $rootScope.$broadcast('toHelp.Update');
                        }
                    },
                    //返回类型
                    "json").error(function (err) {
                        msg = {IsError: true, Message: '发起救援失败！'};
                        common.showError(err);
                    });
            },
            getHelpList: function (obj) {
                common.showLoading();
                obj.authToken = storageUserFac.getUserAuthToken();
                obj.pageSize = CarIn.pageSize;
                console.log(obj);
                return $.post(listUrl, obj,
                    //回调函数
                    function (response) {
                        console.log(response);
                        if (response.IsError) {
                            common.showError(response);
                        } else {
                            msg = response;
                            $rootScope.$broadcast('helpList.Update');
                        }
                    },
                    //返回类型
                    "json").error(function (err) {
                        msg = {IsError: true, Message: '获取救援列表失败'};
                        common.showError(msg);
                    });

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
    })

    .factory('Chats', function () {
        // Might use a resource here that returns a JSON array

        // Some fake testing data
        var chats = [
            {
                id: 0,
                name: 'Ben Sparrow',
                lastText: 'You on your way?',
                face: 'img/ben.png'
            },
            {
                id: 1,
                name: 'Max Lynx',
                lastText: 'Hey, it\'s me',
                face: 'img/max.png'
            },
            {
                id: 2,
                name: 'Adam Bradleyson',
                lastText: 'I should buy a boat',
                face: 'img/adam.jpg'
            },
            {
                id: 3,
                name: 'Perry Governor',
                lastText: 'Look at my mukluks!',
                face: 'img/perry.png'
            },
            {
                id: 4,
                name: 'Mike Harrington',
                lastText: 'This is wicked good ice cream.',
                face: 'img/mike.png'
            }
        ];

        return {
            all: function () {
                return chats;
            },
            remove: function (chat) {
                chats.splice(chats.indexOf(chat), 1);
            },
            get: function (chatId) {
                for (var i = 0; i < chats.length; i++) {
                    if (chats[i].id === parseInt(chatId)) {
                        return chats[i];
                    }
                }
                return null;
            }
        };
    })


