$(function () {
    if ($("#dlg_Div").html() == '') {
        makeDiv();
    }
    $(".datagrid-mask").remove();
    $(".datagrid-mask-msg").remove();
    $("#init_ManDiv").show();
    $("span[for]").attr("style", "cursor:help");
    $("span[for]").unbind().bind('click', function (e) {
        var name = $(this).text();
        $.ajax({
            url: "GetProperDescription?t=" + Math.random(),
            data: {
                proName: $(this).attr("for")
            },
            success: function (data) {
                $.messager.show({
                    title: "说明：" + name,
                    msg: "<textarea style='width: 100%;height:100%;border-width: 0;'>" + data + "</textarea>",
                    showType: 'slide',
                    width: 350,
                    height: 200
                });
            },
            error: function (data) {
                alert('失败:' + data.responseText);
            }
        });
    });

})

/*
*  easyui combobox样式调整
*/
function Combobox_Exented(tagstr) {
    switch (tagstr.CLASS) {
        case "form-control":
            var tag = $("#" + tagstr.ID.replace("#", ""));
            tag.next("span").removeAttr("style");
            tag.next("span").addClass("form-control");
            var w = tagstr.width == undefined ? "100%" : tagstr.width + "px";
            tag.next("span").children("input").attr("style", "width:" + w + ";padding:0");
            var h = tag.parent().height();
            tag.next("span").children("span").children("a").attr("style", "height:" + h + "px");
            break;
        case "query-control":
            var tag = $("#" + tagstr.ID.replace("#", ""));
            tag.next("span").removeAttr("style");
            tag.next("span").addClass("form-control");
            tag.next("span").find("a").attr("style", "width:18px;height:34px");
            break;
        default:
            break;
    }

    if (tagstr.disabled == "disabled") {
        tag.next("span").attr("disabled", "disabled");
        tag.next("span").children("span").children().removeClass("textbox-icon");
        tag.next("span").children("input").attr("disabled", "disabled");
    }
}

function WindowOpen(url, title, w, h) {
    if (url == null || url == '') {
        alert('无连接地址');
        return;
    }
    if (title == null || title == '') {
        title = '';
    }
    if (w == null || w == '') {
        w = 0;
    }
    if (h == null || h == '') {
        h = 0;
    }

    url = url.replace("~/", bootPATH);
    if (w == 0 || h == 0) {
        window.open(url, title, 'height=' + screen.width + ', width=' + screen.width + ', top=0,left=0, toolbar=no, menubar=no, scrollbars=no, resizable=no,location=no, status=no')
    }
    else {
        window.open(url, title, 'height=' + h + ', width=' + w + ', top=0,left=0, toolbar=no, menubar=no, scrollbars=no, resizable=no,location=no, status=no')
    }
}
 
function WindowOpenByFileID(id, title, w, h) {
    $.ajax({
        url: "../Fun/GetFile?t=" + Math.random(),
        dateType: "JSON",
        data: {
            fileId: id
        },
        success: function (data) {
            WindowOpen(data.URL, title, w, h)
        },
        error: function (data) {
            alert('失败:' + data.responseText);
        }
    });
}

/*
用于动态绑定下接框
*/
function AjaxBindSelect(id, sql) {
    $.ajax({
        url: "ExecSql?t=" + Math.random(),
        dataType: "json",
        data: {
            sql: sql
        },
        success: function (data) {
            $("#" + id).empty();
            for (var x = 0; x < data.length; x++) {
                $("<option value='" + data[x].ID + "'>" + data[x].TEXT + "</option>").appendTo("#" + id)//添加下拉框的option
            }
        },
        error: function (data) {
            alert('失败:' + data.responseText);
        }
    });
}
/*
用于绑定下接框,最当前时间，当前时间向前推几个月
*/
function BindSelectMonth(id, num,dText,dValue,format) {
    $("#" + id).empty();
    if (dText != null && dValue != null) {
        $("<option value='" + dValue + "'>" + dText + "</option>").appendTo("#" + id)
    }
    for (var x = 0; x < num; x++) {
        var myDate = new Date();
        var m = myDate.getMonth() - x;
        var y = myDate.getFullYear();
        if (m < 0) {
            var tempYearNum = parseInt(-m / 12);
            if ((-m % 12) == 0) {
                tempYearNum--;
            }
                m = (12 * (tempYearNum + 1)) + m;
                y = y - 1 - tempYearNum;
        }
        m++;
        if (m < 10) m = "0" + '' + m;
        var v = y + '' + m;
        if (format != null) {
            v = DataFormat(new Date(Date.parse(y + '-' + m + '-01')), format);
        }
        var t = y + '年' + m + '月';
        $("<option value='" + v + "'>" + t + "</option>").appendTo("#" + id)//添加下拉框的option
    }
}
/*
用于绑定下接框,最当前时间
*/
function BindSelectMonthStart (id, startMonth, dText, dValue) {
    var myDate = new Date();
    var y = myDate.getFullYear() - parseInt(startMonth.substr(0, 4));
    var m = myDate.getMonth() - parseInt(startMonth.substr(4, 2));
    var reInt = y * 12 + m+2;
    return BindSelectMonth(id, reInt, dText, dValue);
}
function onSubmit() {
    var com = $(".combo-text")
    if (com.attr('name') == null) {
        com.attr('name', 'combo_text')
    }

    com = $(".textbox-text")
    if (com.attr('name') == null) {
        com.attr('name', 'textbox_text')
    }


    var obj = $('input[type="submit"]');
    obj[0].click();
}
function OnSuccess(data) {
    if (data.IsError != null) {
        if (data.IsError) {
            divalert("失败", data.Message);
            return;
        }
        else {
            alert(data.Message);
        }
    }
    try {
        var p = parent.window.opener;
        if (p != null) {
            parent.window.opener.location.reload();
            parent.window.opener = null;
            parent.window.close();
        }
        else {

            parent.CloseWin();
        }

        //  parent.CloseWin();
    }
    catch (e) { }

}
function OnFailure(data, a, b) {
    $(".datagrid-mask").remove();
    $(".datagrid-mask-msg").remove();
    $("#init_ManDiv").show();
    if (data.IsError != null) {
        if (data.IsError) {
            divalert("失败", data.Message);
            return;
        }
        else {
            divalert("成功", data.Message);
        }
        return;
    }
}
function OnBegin(msg) {
    $("#init_ManDiv").hide();
    try
    {
        if (msg == null || msg.readyState!=null)
        {
            msg = "正在处理，请稍候。。。";
        }
    } catch (e) { msg = "正在处理，请稍候。。。"; }
    $("<div class=\"datagrid-mask\" ></div>").css({ display: "block", width: "100%", height: $(window).height() }).appendTo("body");
    $("<div class=\"datagrid-mask-msg\"></div>").html(msg).appendTo("body").css({ display: "block", left: ($(document.body).outerWidth(true) - 190) / 2, top: ($(window).height() - 45) / 2, height: 45 });
}
function OnComplete(data, a, b) {
    $("#init_ManDiv").show();
    $(".datagrid-mask").remove();
    $(".datagrid-mask-msg").remove();
}
function GetRequest() {
    var url = location.search; //获取url中"?"符后的字串
    var theRequest = new Object();
    if (url.indexOf("?") != -1) {
        var str = url.substr(1);
        strs = str.split("&");
        for (var i = 0; i < strs.length; i++) {
            theRequest[strs[i].split("=")[0]] = unescape(strs[i].split("=")[1]);
        }
    }
    return theRequest;
}
function divalert(title, message) {
    $.messager.alert(title, message);
}
function JsOpen(url, title, w, h, objFun) {
    if (w == 0) w = $(window).width()
    if (h == 0) h = $(window).height()
    url = url.replace("~/", bootPATH);
    window.open(url, objFun, "dialogWidth=" + w + "px;dialogHeight=" + h + "px;");
    if (objFun != null) objFun();
}
var DivOpenVar = '';
function DivOpen(url, title, w, h, objFun) {
    url = url.replace("~/", bootPATH);
    $('#dlg_iframe')[0].src = url;
    if (w == 0 || h == 0) {
        $('#dlg_Div').dialog({
            title: title,
            closed: false,
            maximized: true,
            onClose: function () {
                $('#dlg_iframe')[0].src = "";
                if (objFun != null) objFun(DivOpenVar);
            }
        });
        $('#dlg_Div').dialog("maximize");
    }
    else {
        $('#dlg_Div').dialog({
            width: w,
            height: h,
            title: title,
            closed: false,
            onClose: function () {
                $('#dlg_iframe')[0].src = "";
                if (objFun != null) objFun(DivOpenVar);
            },
            onMaximize: function () {
                $("#dlg_Div").parent().css("top", $(document).scrollTop() + "px");
            }
        });
    }
}
function DivEditDialog(url, title, w, h, objFun) {
    url = url.replace("~/", bootPATH);
    $('#dlg_iframe')[0].src = url;
    if (w == 0 || h == 0) {
        $('#dlg_Div').dialog({
            title: title,
            closed: false,
            maximized: true,
            onClose: function () {
                $('#dlg_iframe')[0].src = "";
                if (objFun != null) objFun(DivOpenVar);
            },
            buttons: [{
                text: '保存',
                iconCls: 'icon-save',
                handler: function () {
                    var myFrame = document.getElementsByName("dlg_iframe"); //获取所有name为myFrame 的iframe
                    for (var i = 0; i < myFrame.length; i++) //进行循环
                    {
                        myFrame[i].contentWindow.onSubmit(); //kindSubmit();为iframe页面的方法
                    }
                    if (objFun != null) objFun(DivOpenVar);

                }
            }, {
                text: '取消',
                iconCls: 'icon-cancel',
                handler: function () {
                    CloseWin();
                }
            }]
        });
        $('#dlg_Div').dialog("maximize");
    }
    else {
        $('#dlg_Div').dialog({
            width: w,
            height: h,
            title: title,
            closed: false,
            onClose: function () {
                $('#dlg_iframe')[0].src = "";
                if (objFun != null) objFun(DivOpenVar);
            },
            onMaximize: function () {
                $("#dlg_Div").parent().css("top", $(document).scrollTop() + "px");
            },
            buttons: [{
                text: '保存',
                iconCls: 'icon-save',
                handler: function () {
                    var myFrame = document.getElementsByName("dlg_iframe"); //获取所有name为myFrame 的iframe
                    for (var i = 0; i < myFrame.length; i++) //进行循环
                    {
                        myFrame[i].contentWindow.onSubmit(); //kindSubmit();为iframe页面的方法
                    }
                    if (objFun != null) objFun(DivOpenVar);

                }
            }, {
                text: '取消',
                iconCls: 'icon-cancel',
                handler: function () {
                    CloseWin();
                }
            }]
        });
    }
}
function CloseWin() {
    $('#dlg_Div').dialog('close');
    $('#dlg_iframe')[0].src = "";
}
function AjaxUrl(url, msg, objFun) {
    url = url.replace("~/", bootPATH);
    if (confirm("确定要执行" + msg + "操作？")) {
        $.ajax({
            url: url,
            data: {
                t: Math.random()
            },
            success: function (data) {
                if (objFun != null) objFun();

                if (data.Message == null) data.Message = "";
                data.Message = data.Message.replace('\r\n', '<br />')
                data.Message = data.Message.replace('\\r\\n', '<br />')
                if (data.IsError != null) {
                    if (data.IsError) {
                        divalert("失败", data.Message);
                        return;
                    }
                    else {
                        divalert("成功", data.Message);
                    }
                }
                else {
                    divalert(data);
                }
            },
            error: function (data) {
                alert(msg + '失败:' + data.responseText);
            }
        });
    }
}
function makeDiv() {
    $('<div id="dlg" class="easyui-dialog" title="提示框" data-options="closed: true" style="padding:10px"><div id="MsgBox" style="text-align:center"></div></div>').appendTo("body");
    $('<div id="dlg_Div" title="添加" class="easyui-dialog" style="width: 550px; height: 250px" data-options="width:550,height:250,closed: true,modal:true,maximizable:true,resizable:true"><iframe scrolling="auto" id="dlg_iframe" name="dlg_iframe" frameborder="0" style="width: 100%; height: 98%;"></iframe></div>').appendTo("body");
};
makeDiv();
OnBegin();
function DelDatagridRow(name, objFun) {
    var row = $('#' + name).datagrid("getSelected");
    if (row != null) {
        $.messager.confirm('删除确认', '确定要删除吗?', function (r) {
            if (r) {
                var rowIndex = $('#' + name).datagrid('getRowIndex', row)
                $('#' + name).datagrid('deleteRow', rowIndex);
                if (objFun != null) objFun();
            }
        });
    }
}
/*
用于加载父节点和子节点插入到树
$('#tt').tree({
    url: 'data/tree6_data.json',
    loadFilter: function (rows) {
        return convert(rows);
    }
});
[
{"id":1,"parendId":0,"name":"Foods"},
{"id":2,"parentId":1,"name":"Fruits"},
]
*/
function convert(rows, valueStr) {

    valueStr = "," + valueStr + ",";
    function exists(rows, parentId) {
        for (var i = 0; i < rows.length; i++) {
            if (rows[i].id == parentId) return true;
        }
        return false;
    }

    var nodes = [];
    // 得到所有的顶级
    for (var i = 0; i < rows.length; i++) {
        var row = rows[i];
        if (!exists(rows, row.parentId)) {

            if (valueStr.indexOf(',' + row.id + ',') > -1) {
                nodes.push({ id: row.id, text: row.name, checked: true })
            }
            else {
                nodes.push({
                    id: row.id,
                    text: row.name
                });
            }
        }
    }

    var toDo = [];
    for (var i = 0; i < nodes.length; i++) {
        toDo.push(nodes[i]);
    }
    while (toDo.length) {
        var node = toDo.shift();    // 从集合中把第一个元素删除，并返回这个元素的值。
        for (var i = 0; i < rows.length; i++) {
            var row = rows[i];
            if (row.parentId == node.id) {
                var child = { id: row.id, text: row.name, checked: false };
                if (valueStr.indexOf(',' + row.id + ',') > -1) {
                    child = { id: row.id, text: row.name, checked: true };
                    node.checked = false;
                }

                if (node.children) {
                    node.children.push(child);
                    node.state = "closed";
                } else {
                    node.children = [child];
                }
                toDo.push(child);
            }
        }
    }
    return nodes;
}

//获取当前时间
function CurentTime() {
    var now = new Date();
    var hh = now.getHours();
    var mm = now.getMinutes();
    var ss = now.getTime() % 60000;
    ss = (ss - (ss % 1000)) / 1000;
    var clock = hh + ':';
    if (mm < 10) clock += '0';
    clock += mm + ':';
    if (ss < 10) clock += '0';
    clock += ss;
    return (clock);
}


//---------------------------------------------------  
// 日期格式化  
// 格式 YYYY/yyyy/YY/yy 表示年份  
// MM/M 月份  
// W/w 星期  
// dd/DD/d/D 日期  
// hh/HH/h/H 时间  
// mm/m 分钟  
// ss/SS/s/S 秒  
//---------------------------------------------------  
function DataFormat(thisDate, formatStr) {
    var str = formatStr;
    var Week = ['日', '一', '二', '三', '四', '五', '六'];

    str = str.replace(/yyyy|YYYY/, thisDate.getFullYear());
    str = str.replace(/yy|YY/, (thisDate.getYear() % 100) > 9 ? (thisDate.getYear() % 100).toString() : '0' + (thisDate.getYear() % 100));

    str = str.replace(/MM/, thisDate.getMonth() + 1 > 9 ? (thisDate.getMonth() + 1) : '0' + (thisDate.getMonth() + 1));
    str = str.replace(/M/g, thisDate.getMonth());

    str = str.replace(/w|W/g, Week[thisDate.getDay()]);

    str = str.replace(/dd|DD/, thisDate.getDate() > 9 ? thisDate.getDate().toString() : '0' + thisDate.getDate());
    str = str.replace(/d|D/g, thisDate.getDate());

    str = str.replace(/hh|HH/, thisDate.getHours() > 9 ? thisDate.getHours().toString() : '0' + thisDate.getHours());
    str = str.replace(/h|H/g, thisDate.getHours());
    str = str.replace(/mm/, thisDate.getMinutes() > 9 ? thisDate.getMinutes().toString() : '0' + thisDate.getMinutes());
    str = str.replace(/m/g, thisDate.getMinutes());
    str = str.replace(/ss|SS/, thisDate.getSeconds() > 9 ? thisDate.getSeconds().toString() : '0' + thisDate.getSeconds());
    str = str.replace(/s|S/g, thisDate.getSeconds());
    return str;
}
/*
时间加减天数
thisDate:时间对你
AddDayNum:可为负数
*/
function addDate(thisDate, AddDayNum) {
    var a = new Date(thisDate)
    a = a.valueOf()
    a = a + AddDayNum * 24 * 60 * 60 * 1000
    a = new Date(a)
    return a;
}

function GetData(runData, formatStr) {
    var t = new Date();//你已知的时间
    if (runData == null || runData == '') {
        return DataFormat(t, formatStr);
    }
    var type = runData.substring(runData.length - 1);
    var v = parseInt(runData.substr(0, runData.length - 1));
    var t_s = t.getTime();//转化为时间戳毫秒数
    var nt = new Date();//定义一个新时间
    switch (type) {
        case "y":
            nt.setTime(t_s + v * 1000 * 60 * 60 * 24 * 365);//设置新时间比旧时间多一天
            break;
        case "m":
            nt.setTime(t_s + v * 1000 * 60 * 60 * 24 * 30);//设置新时间比旧时间多一天
            break;
        case "d":
            nt.setTime(t_s + v * 1000 * 60 * 60 * 24);//设置新时间比旧时间多一天
            break;
    }
    return DataFormat(nt, formatStr);

}


/*
获取上传文件方法
obj:按钮对象
event:事件
objName:上传控件
*/
function MoveFilePic(obj, event, objName) {
    //检查是否为 IE 6-8：
    if (!$.support.leadingWhitespace) {
        var event = event || window.event;
        $("#" + objName).attr("style", "position:absolute;filter:alpha(opacity=0); opacity:0; top:0px; width:20px;display:block");
        var a = document.getElementById(objName);
        a.style.left = $("#" + objName).position().left + event.offsetX - 10 + 'px';
        a.style.top = event.offsetY + 'px';
    }
    else {
        $(obj).unbind("mousemove"); //移除mousemove
        $(obj).unbind("click").click(function () {
            $('input[id=' + objName + ']').click();
        });
    }
}

/*
把值设置为选中
*/
function BoxListDisplayV(name) {
    var nowStr = "," + $("#" + name).val() + ","
    $("input[name='" + name + "_JSITEM']").each(function () {
        $(this).removeAttr("checked");
        if (nowStr.indexOf("," + $(this).attr("value") + ",") > -1) {
            $(this).attr("checked", "true");
        }
    })
}
/*
把选中的设置为值
*/
function BoxListSetV(name) {
    var num = [];
    $("input[name='" + name + "_JSITEM']").each(function () {
        if ($(this).attr("checked") == 'checked') {
            num.push($(this).attr("value"));
        }
    })
    $("#" + name).val(num.join(","));
}
function ChangePic(urlObj, idObj, fileObj,ReCallBack) {
    OnBegin();
    $.ajaxFileUpload
    (
        {
            url: '../Fun/FileUp', //用于文件上传的服务器端请求地址
            type: 'post',
            data: { nameType: '0', fileType: 'image', dirPath: "~/UpFiles/Lawyer/" },//nameType(命名方式):0表示随机文件名，1表示使用原文件名;fileType(文件类型):image、flash、media、file
            secureuri: false, //一般设置为false
            fileElementId: fileObj, //文件上传空间的id属性  <input type="file" id="file" name="file" />
            dataType: 'json', //返回值类型 一般设置为json
            success: function (data, status)  //服务器成功响应处理函数
            {
                OnComplete();
                if (data.Message == null) data.Message = "";
                data.Message = data.Message.replace('\r\n', '<br />')
                data.Message = data.Message.replace('\\r\\n', '<br />')
                if (data.IsError != null) {
                    if (data.IsError) {
                        divalert("失败", data.Message);
                        return;
                    }
                    else {
                        $("#" + urlObj).val(data.Message)
                        if (idObj != null) {
                            $.ajax({
                                url: "../Fun/AddFile?t=" + Math.random(),
                                dateType: "JSON",
                                data: {
                                    name: data.Params.split('|')[1],
                                    size: data.Params.split('|')[0],
                                    savePath: data.Message
                                },
                                success: function (data) {
                                    $("#" + idObj).val(data.ID)
                                    if (ReCallBack != null)
                                    {
                                        ReCallBack(data);
                                    }
                                },
                                error: function (data) {
                                    alert('失败:' + data.responseText);
                                }
                            });
                        }

                    }
                }
                else {
                    divalert(data);
                }
            },
            error: function (data, status, e)//服务器响应失败处理函数
            {
                OnComplete();
                alert(data);
            }
        }
    )
    return false;
}