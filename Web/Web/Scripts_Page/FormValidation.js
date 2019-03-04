/***********表单输入验证**************/
//座机号码验证
function IsTelPhone(s) {
    var patrn = /\d{3}-\d{8}|\d{4}-\d{7}/;
    if (!patrn.exec(s)) return false
    return true
}

//手机号码验证
function IsMobilePhone(s) {
    var patrn = /^1[34578]\d{9}$/;
    if (!patrn.exec(s)) return false
    return true
}

//邮政编码校验
function IsZipCode(s) {
    var patrn = /^[a-zA-Z0-9 ]{3,12}$/;
    if (!patrn.exec(s)) return false
    return true
}
//邮箱验证
function IsEmali(str) {
    var filter = /^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$/;
    if (filter.test(str)) return true;
    else return false;
}
//验证是否为数字
function IsInteger(str) {
    var regu = /^[-]{0,1}[0-9]{1,}$/;
    return regu.test(str);
}
/***********表单输入验证**************/