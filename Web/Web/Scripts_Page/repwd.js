

$(function () {
    $("#step1").css("color", "Orange");
    $(".next").css("display", "none");
    $(".next1").css("display", "none");
    $("#btn1").click(function () {
       var posturl = "../Login/UserGetPhone";
       var loginname = $('#userName').val();
       var phone = $('#phone').val();

       var Md5LoginName = hex_md5(loginname);
       var AseLoginName = Encrypt(loginname);
      
       var AsePhone = Encrypt(phone);

       $.ajax({
            type: "POST",
            url: posturl,
            async: false,
            data: { name: AseLoginName, phone: AsePhone },
            success: function (data) {
                if (data.IsError) {
                    divalert("失败", data.Message);
                    $("#userName").val('');
                    $("#phone").val('');
                    return;
                }
                $("#phone").text(data.PHONE_NO); 
                $(".pre").css("display", "none");
                $(".next").css("display", "block");
                $("#step2").css("color", "Orange");
                $(".SG1").css("background-color", "Orange");
            }
        });
    });
    $("#SendCode").click(function () {
     
        $(function () {
            var sendcode = $("#SendCode");
            var time = 120;
            var setIntervalId = setInterval(function () {
                sendcode.attr("disabled", true);
                time = time - 1;
                sendcode.val('');
                sendcode.val(time + "秒" + "重新获取");
                if (time == 0) {
                    clearInterval(setIntervalId);
                    sendcode.val("点击重新获取验证码");
                    sendcode.attr("disabled", false);
                    return;
                }
            }, 1000);
        });
        var postusrl = "../Login/UserSendVerifyCode";
        $.post(postusrl, { username: $("#userName").val() }, function (data) {
            if (data.IsError) {
                divalert("失败", data.Message);
                
                return;
            }
        });
    });
    $("#btn2").click(function () {
        if ($("#Code").val().trim() != "") {
            $(".next").css("display", "none");
            $(".next1").css("display", "block");
            $("#step3").css("color", "Orange");
            $(".SG2").css("background-color", "Orange");
        }
        else {
          divalert("失败", "请输入验证码");
        }
    });
    $("#btn3").click(function () {
        var posturl = "../Login/UserResetPwdByVerifyCode";
        if ($("#newPwd").val() != $("#renewPwd").val()) {
            divalert("失败", "密码不一致");
            return;
        }
        var userName = Encrypt($("#userName").val());
        var newPwd = hex_md5($('#newPwd').val());
        var code = hex_md5($("#Code").val());
        $.ajax({
            type: "POST",
            url: posturl,
            async: false,
            data:{username:userName,code:code,newpwd:newPwd}, 
            success: function (data) {  
                if (data.IsError) {
                    divalert("失败", data.Message);
                    $("#step1").css("color", "Orange");
                    $("#step2").css("color", "#Orange");
                    $("#step3").css("color", "#747070");
                    $(".next").css("display", "block");
                    $(".next1").css("display", "none");
                    $(".SG2").css("background-color", "#cfc9c9");
                    $("#Code").val("");
                    return;
                }
                else {
                    divalert("提示", "恭喜你修改成功");
                }
            }
        });
    });
})