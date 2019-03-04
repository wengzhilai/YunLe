/**
 * Created by wengzhilai on 2016/11/6.
 */
mainController.controller('bbsListCtr', function ($ionicPopup,common,$scope, toPost, $ionicLoading, $ionicModal,fileUpFac,storageUserFac) {
  $scope.callPhone = function (mobilePhone) {
    common.callPhone(mobilePhone);
  };
  $scope.user=storageUserFac.getUser();
  $ionicModal.fromTemplateUrl('templates/bbs/bbsAdd.html', {
    scope: $scope
  }).then(function(modal) {
    $scope.modal = modal;
  });
  $scope.AllMessageType=[];
  $scope.addBean={
    MESSAGE_TYPE_ID:1,
    CONTENT:'',
    AllFiles:[],
    fileIdStr:''
  }
  $scope.func={
    upImg: function (obj) {
      $scope.bbsList.currEnt = $(obj.target);
      var indexNo = $scope.bbsList.currEnt.attr("data-indexNo");
      fileUpFac.upImg($scope.bbsList.currEnt, $scope.addBean.AllFiles[indexNo].ID, $scope.func.upCallback,$scope);
    },
    upCallback: function (result) {
      var indexNo = $scope.bbsList.currEnt.attr("data-indexNo");
      $scope.addBean.AllFiles[indexNo].ID = result.ID;
      $scope.addBean.AllFiles[indexNo].URL = result.URL;
    },
    AddImg:function(){
      $scope.addBean.AllFiles[$scope.addBean.AllFiles.length] = {"indexNo": $scope.addBean.AllFiles.length};
    },
    createEnt:function(){
      for(var i=0;i<$scope.addBean.AllFiles.length;i++)
      {
        if($scope.addBean.AllFiles[i].ID!=null)
        {
          $scope.addBean.fileIdStr+=","+$scope.addBean.AllFiles[i].ID;
        }
      }
      toPost.saveOrUpdate("BbsSave",$scope.addBean,function(currMsg){
        if (!currMsg.IsError){
          $ionicLoading.show({
            noBackdrop: true,
            template: "保存成功",
            duration: 2000
          });
          $scope.bbsList.doRefresh();
        }
        else {
        }
        $scope.modal.hide();
      });
    }
  }


  $scope.bbsList = {
    nowTabsIndex:0,
    bbsReplay:'',
    currEnt:null,
    bean: {
      userId: 0,
      authToken: '',
      pageSize: 0,
      id: 0,
      currentPage: 0,
      searchKey: [],
      para:[],
      orderBy: []
    },
    lists: {},
    init:function () {
      toPost.Post("MessageTypeAll",{authToken : storageUserFac.getUserAuthToken()},function (mageType) {
        $scope.AllMessageType=mageType;
      })
      this.loadMore();
    },
    onTab:function(obj,index){
      $(".tab-item").removeClass("active")
      $(obj.target).addClass("active");
      this.nowTabsIndex=index;
      this.doRefresh();
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

      var buB = $("a[class='button button-balanced']").text();
      switch (buB)
      {
        case "经验交流":
          this.bean.searchKey=[{K: "MESSAGE_TYPE_ID", V: 3, T: '=='}];
          break;
        case "问题咨询":
          this.bean.searchKey=[{K: "MESSAGE_TYPE_ID", V: 1, T: '=='}];
          break;
        case "修改意见":
          this.bean.searchKey=[{K: "MESSAGE_TYPE_ID", V: 2, T: '=='}];
          break;
      }
      this.bean.para=[{K: "type", V: this.nowTabsIndex, T: '=='}];
      toPost.list("bbsList", this.bean, this.callListReturn);
    },
    callListReturn:function(currMsg){
      if (currMsg.IsError == true) {
        $ionicLoading.show({
          noBackdrop: true,
          template: currMsg.Message,
          duration: 2000
        });
      } else {
        $scope.bbsList.lists = currMsg;
      }
      $scope.$broadcast('scroll.refreshComplete');
      $scope.$broadcast('scroll.infiniteScrollComplete');
    },
    loadMore: function () {
      if (this.bean.currentPage == 0) {
        this.bean.currentPage = 1;
      } else {
        this.bean.currentPage++;
      }
      console.log("加载更多");
      toPost.list("bbsList", this.bean, this.callListReturn);
    },
    filterKey: function (obj) {
      if ($("a[class='button button-balanced']").length != 0) {
        $("a[class='button button-balanced']").each(function () {
          $(this).attr("class", "button");
        });
      }
      obj.target.setAttribute("class", "button button-balanced");
      this.doRefresh();
    },
    toFollowOrder:function (bbsBean) {
      $scope.bbsList.bbsReplay="";
      // 自定义弹窗
      var myPopup = $ionicPopup.show({
        template: '<input type="text" ng-model="bbsList.bbsReplay">',
        title: '请输入回内容',
        subTitle:bbsBean.CONTENT ,
        scope: $scope,
        buttons: [
          { text: '取消' },
          {
            text: '<b>回复</b>',
            type: 'button-positive',
            onTap: function(e) {
              if ($scope.bbsList.bbsReplay=="") {

                common.hint( '回复信息必填');
              } else {
                var postBean = {
                  CONTENT:$scope.bbsList.bbsReplay,
                  PARENT_ID:bbsBean.ID
                };
                toPost.saveOrUpdate("BbsSave",postBean,function (currMsg) {
                  if (!currMsg.IsError){
                    $ionicLoading.show({
                      noBackdrop: true,
                      template: "保存成功",
                      duration: 2000
                    });
                    $scope.bbsList.doRefresh();
                  }

                })
                return true;
              }
            }
          },
        ]
      });
    }
  };
})
