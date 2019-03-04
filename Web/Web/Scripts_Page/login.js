
function OnSuccess(data) {
    if (data.IsError != null) {
        if (data.IsError) {
            $(".send").click();
            if (data.Message.indexOf('密码复杂度不够') > -1) {
                DivOpen('~/User/UserResetPwdFrom', '修改密码', 500, 300)
            }
            else {
                divalert("失败", data.Message);
            }
            return;
        }
        else {
            alert(data.Message);
        }
    }
    else {
        OnBegin();
        window.location = "../Home/Index";
    }
}

//浏览器全屏化
$(document).ready(function () {     //使用jquery的ready方法似的加载运行  
    if (window.screen) {              //判断浏览器是否支持window.screen判断浏览器是否支持screen  
        var myw = screen.availWidth;   //定义一个myw，接受到当前全屏的宽  
        var myh = screen.availHeight;  //定义一个myw，接受到当前全屏的高  
        window.moveTo(0, 0);           //把window放在左上脚  
        window.resizeTo(myw, myh);     //把当前窗体的长宽跳转为myw和myh  
    }
});

//页面打开，查找cookie中是否存在用户
$(function () {
    //查找cookie是否存在用户
    if ($.cookie('loginname') != null) {
        var account = $.cookie('loginname');
        $("#LOGIN_NAME").val(account);
        $("#chkcookie").attr('checked', "checked");
    }
    //登陆按钮添加回车事件
    document.onkeydown = function (e) {
        var ev = document.all ? window.event : e;
        if (ev.keyCode == 13) {
            $("#btnOnSubmit").click();
        }
    }
   
});
//登陆按钮
function OnSubmit(obj) {
    var loginname = $("#LOGIN_NAME").val();
    var psw = $("#PASSWORD").val();
    var Md5LoginName = hex_md5(loginname);
    var Md5PassWord = hex_md5(psw);
    var AseLoginName = Encrypt(loginname);
    var AsePassWord = Encrypt(psw);
    if (loginname.length <= 0 && psw.length > 0) {
        $("#LOGIN_NAME").css("border-color", "#ccc");
    }
    else if (psw.length <= 0 && loginname.length > 0) {
        $("#PASSWORD").css("border-color", "#ccc");
    }
    else if (loginname.length <= 0 && psw.length <= 0) {
        $("#LOGIN_NAME").css("border-color", "#ccc");
        $("#PASSWORD").css("border-color", "#ccc");
    }
    else {
        $("#PASSWORD").css("border-color", "#ccc");
        $("#LOGIN_NAME").css("border-color", "#ccc");
        //$(obj).button('loading');
        //objectdata = obj;
        $("#LOGIN_NAME").val("");
        $("#PASSWORD").val("");
        $("#Md5LoginName").val(Md5LoginName);
        $("#Md5PassWord").val(Md5PassWord);
        $("#AseLoginName").val(AseLoginName);
        $("#AsePassWord").val(AsePassWord);
        
       // $("#btnsubmit").click();
    }
}

//输入框不为空，改变border颜色
function TextBlur(obj) {
    if ($(obj).val().length > 0) {
        $(obj).css("border-color", "#ccc");
    }
    var loginname = $("#LOGIN_NAME").val();
    var psw = $("#PASSWORD").val();
    if (loginname.length > 0 && psw.length > 0) {
        $("#txterror").html('');
    }

}
//添加到收藏夹
function SaveUrl(name) {
    var ctrl = (navigator.userAgent.toLowerCase()).indexOf('mac') != -1 ? 'Command/Cmd' : 'CTRL';
    if (document.all) {
        window.external.addFavorite(window.location, name)
    } else if (window.sidebar) {
        window.sidebar.addPanel(name, window.location, "")
    } else {　　　　//添加收藏的快捷键  
        alert('添加失败\n您可以尝试通过快捷键' + ctrl + ' + D 加入到收藏夹~')
    }
}

