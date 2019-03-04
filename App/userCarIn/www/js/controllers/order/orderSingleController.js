/**
 * Created by wengzhilai on 2016/8/12.
 */
/**
 * Created by wengzhilai on 2016/8/10.
 */

mainController.controller('orderSingleCtr', function ($scope, $ionicPopup, $ionicLoading, $cordovaImagePicker, $cordovaCamera, $stateParams, orderFac, $state, storageUserFac, $ionicActionSheet) {
    if ($stateParams.id) {
        orderFac.OrderSingle($stateParams.id);
    }
    var user = storageUserFac.getUser();
    $scope.insure = {
        bean: {
            ID:0,
            CAR_ID: 0,
            CLIENT_ID: storageUserFac.getUserId(),
            ORDER_NO:'',
            ORDER_TYPE:'',
            PAY_STATUS:'',
            COST:'',
            CREATE_TIME:new Date(),
            LANG:'',
            LAT:'',
            APPRAISE_SCORE:'',
            APPRAISE_CONTENT:'',
            REMARK:'',
            AllFlow:[],
            AllFiles:[],
            SaveProductId: [],
            ClientName: '',
            ClientPhone:''
        },
        currEnt: {},
        userInfo: user,
        carInfo: {},
        carsOptions: [],
        insurerOptions: [],
        insurerInfo: {}
    };

    $scope.$on('Insure.OrderSingle', function () {
        var currMsg = orderFac.getCurrentMes();
        console.log(currMsg);
        $scope.insure.bean = currMsg;

    });

})