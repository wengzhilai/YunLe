function onSubmit() {
    var Data = $("#CategroyInfo").datagrid("getData");
    if (Data.total > 0) {
        var json = JSON.stringify(Data.rows);
        $("#CategroyInfoJson").val(json);
    }
    var com = $(".combo-text")
    if (com.attr('name') == null) {
        com.attr('name', 'combo_text')
    }
    var obj = $('input[type="submit"]');
    obj[0].click()
}
//将Checkbo的值和下拉列表框的值转换成JSON
function JsJson() {
            var Json = "";
            var checkbox = document.getElementsByName("checkbox");
            for (var i = 0; i < checkbox.length; i++)
            {
                if (checkbox[i].checked)
                {
                    Json += EntityToJson(checkbox[i].value, null, change(checkbox[i])) + ',';
                }
            }
            if (Json.length > 0) {
                var json = '[' + Json.substring(0, Json.length - 1) + ']';
                $("#COMMI_CATEGORY").val(json);
            }
}

//Json格式
function EntityToJson(id, name, level) {
    var objjson = '{' + '"ID":' + id
        + ',"NAME":"' + name
        + '","LEVEL":"' + level
        + '"}'
    return objjson;
}
//获取Select的值
function change(obj) {
    var id = $(obj).attr('id');
    if ($(obj).attr('checked') == "checked") {
        var chkid = id.substr(13, id.length);
        var txt = $("#Level_" + chkid).val();
        return txt;
    }
}
////获取Select的值
function changes(obj, level) {
    if (level == null) {
        var id = $(obj).attr('id');
        if ($(obj).attr('checked') == "checked") {
            var chkid = id.substr(13, id.length);
            $("#Level_" + chkid).val(level);
        }
    }
}
