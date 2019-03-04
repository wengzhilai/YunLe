/**
 * Created by wengzhilai on 2016/9/19.
 */
mainService.factory('storageUserFac', function (CarIn, $rootScope, Storage) {
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

})