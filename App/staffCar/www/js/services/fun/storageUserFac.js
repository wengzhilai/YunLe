/**
 * Created by wengzhilai on 2016/9/19.
 */
mainService.factory('storageUserFac', function (CarIn, $rootScope, Storage) {
    var storageKey = 'user',
        user = Storage.getObject(storageKey);
    return {
        setUser: function (ent) {
            user = ent;
            Storage.setObject(storageKey, ent);
        },
        getUser: function () {
            return Storage.getObject(storageKey);
        },
        getUserAuthToken: function () {
            return (user==null)?null:user.authToken;
        },
        //推荐码
        getUserPollCode: function () {
            return (user==null)?null:user.REQUEST_CODE;
        },
        getUserId: function () {
            return (user==null)?null:user.ID;
        },
        getUserName: function () {
            return (user==null)?null:user.NAME;
        },
        getUserPhone: function () {
            return (user==null)?null:user.phone;
        },
        getUserNowCar: function () {
            return (user==null)?null:user.NowCar;
        },
        //团队编号
        getTeamId: function () {
            return (user==null)?null:user.TEAM_ID;
        },
        clearAll: function () {
            user=null;
            Storage.remove(storageKey);
            return Storage.clearAll();
        }

    }

})
