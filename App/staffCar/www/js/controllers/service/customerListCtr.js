/**
 * Created by wengzhilai on 2016/9/30.
 */

mainController.controller('customerListCtr', function (Storage, $ionicLoading,toPost, $ionicActionSheet, $scope, $ionicPopup, $cordovaImagePicker, $cordovaCamera, fileUpFac, $state) {
    $scope.customerList = {
        bean: {
            userId: 0,
            authToken: '',
            pageSize: 0,
            id: 0,
            currentPage: 0,
            searchKey: [],
            orderBy: []
        },
        lists: {},
        bigImage: false,
        bigSrc: '',
        showBigImage: function (ent) {
            var src =$(ent.target).attr("src");
            if (src == null || src == '') {
                //选择上传图片
                this.upImg(ent);
                return;
            }
            this.bigSrc = src;
            this.bigImage = true;                   //显示大图
        },
        hideBigImage: function () {
            this.bigImage = false;
        },
        toTel: function (obj) {
            window.plugins.CallNumber.callNumber(function (result) {
            }, function (result) {
            }, phone, true);
        },
        addCustomer: function () {
            $state.go('editCustomer', {reload: true});  //路由跳转
        },
        hasNextPage: function () {
            if (this.lists.totalPage == null || this.lists.totalPage == 0) {
                return false;
            }
            else if (this.lists.totalPage <= this.lists.currentPage) {
                return false;
            }
            return true;
        },
        doRefresh: function () {
            console.log("下拉刷新");
            this.bean.currentPage = 1;
            if ($('#searchKey').val() != '') {
                this.bean.searchKey = [
                    {K: 'searchName', V: $('#searchKey').val(), T: 'like'}
                ];
            }
            toPost.list("SalesmanClientList", this.bean, $scope.callListReturn);
        },
        loadMore: function () {
            if (this.bean.currentPage == 0) {
                this.bean.currentPage = 1;
            } else {
                this.bean.currentPage++;
            }
            console.log("加载更多");
            if ($('#searchKey').val() != '') {
                this.bean.searchKey = [
                    {K: 'searchName', V: $('#searchKey').val(), T: 'like'}
                ];
            }
            toPost.list("SalesmanClientList", this.bean, $scope.callListReturn);
        }

    };

    $scope.callListReturn = function (currMsg) {
        if (currMsg.IsError == true) {
            $ionicLoading.show({
                noBackdrop: true,
                template: currMsg.Message,
                duration: 2000
            });

        } else {
            $scope.customerList.lists = currMsg;
        }
        $scope.$broadcast('scroll.refreshComplete');
        $scope.$broadcast('scroll.infiniteScrollComplete');
    };

    /**
     $scope.startVib=function(){
            // 震动 1000ms
            $cordovaVibration.vibrate(1000);

        };
     **/


    $scope.showOrderPopup = function (obj) {
        // 自定义弹窗
        var myPopup = $ionicPopup.show({
            template: '<div  class="item item-input item-select"><div class="input-label">排序字段</div><select id="orderF"><option value="">--请选择--</option><option value="ID">ID</option></select></div>' +
                '<div class="list"><div class="item">升序<input type="radio" name="order" value="asc" checked="checked"/></div>' +
                '<div class="item">降序<input type="radio" name="order" value="desc"/> </div>',
            title: '选择排序方式',
            buttons: [
                {
                    text: '<b>取消</b>',
                    type: 'button-positive',
                    onTap: function () {
                        myPopup.close();
                    }
                },
                {
                    text: '<b>确定</b>',
                    type: 'button-positive',
                    onTap: function (e) {
                        var order = $('input[name="order"]:checked').val();
                        var orderF = $('#orderF option:selected').val();
                        if (orderF != '') {
                            $scope.customerList.bean.orderBy = [
                                {K: orderF, V: order, T: ''}
                            ];

                            obj.target.innerHTML = orderF + ' ' + order;
                        } else {

                            obj.target.innerHTML = '排序';
                        }
                    }
                }
            ]
        });
    };
    $scope.showUserPopup = function (obj) {
        // 自定义弹窗
        var myPopup = $ionicPopup.show({
            template: '<div  class="item item-input item-select"><div class="input-label">订单类型</div><select id="OrderType"><option value="" selected="selected">--请选择--</option><option value="救援">救援</option><option value="维护">维护</option><option value="保养">保养</option><option value="投保">投保</option><option value="审车">审车</option></select></div>',

            title: '选择订单类型',
            scope: $scope,
            buttons: [
                {
                    text: '<b>取消</b>',
                    type: 'button-positive',
                    onTap: function () {
                        myPopup.close();
                    }
                },
                {
                    text: '<b>确定</b>',
                    type: 'button-positive',
                    onTap: function (e) {
                        var OrderType = $('#OrderType option:selected').val();
                        if (OrderType != '') {
                            if ($('#searchKey').val() != '') {
                                $scope.bean.searchKey = [
                                    {K: 'searchName', V: $('#searchKey').val(), T: 'like'},
                                    {
                                        K: 'OrderType',
                                        V: OrderType,
                                        T: '=='
                                    }
                                ];
                            } else {
                                $scope.customerList.bean.searchKey = [
                                    {K: 'OrderType', V: OrderType, T: '=='}
                                ];
                            }

                            obj.target.innerHTML = OrderType;
                        } else {
                            obj.target.innerHTML = '全部客户';

                        }
                    }
                }
            ]
        });
    };
 })
