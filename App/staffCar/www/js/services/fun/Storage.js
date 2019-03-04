/**
 * Created by wengzhilai on 2016/9/19.
 */
mainService.factory('Storage', function ($window) {
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
})