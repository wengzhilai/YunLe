
$(function () {
    ShowHeardBtn();

    $("#GetWhereStr").val(GetWhereStr());

    var obj = GetRequest();
    for (var i = 0; i < obj.length; i++) {
        if (obj[i].V == "@(NOWDATA)") {
            var nowt = new Date();
            obj[i].V = nowt.getFullYear() + "-" + parseInt(nowt.getMonth() + 1) + "-" + nowt.getDate();
        }
        try {
            $("#s_" + obj[i].K + "_value").val(obj[i].V)
            $("#s_" + obj[i].K + "_value").datebox("setValue", obj[i].V)
        } catch (e) { }
    }

    $('.form_datetime').datetimepicker({
        step: 5,
        format: 'Y-m-d H:i'
    });


    $(".form_numberbox").keyup(function () {
        var tmptxt = $(this).val();
        $(this).val(tmptxt.replace(/\D|^0/g, ''));
    }).bind("paste", function () {
        var tmptxt = $(this).val();
        $(this).val(tmptxt.replace(/\D|^0/g, ''));
    }).css("ime-mode", "disabled");
});

function ShowFiled_handler()
{
    var key = $("#ShowFiled_key").val(); 
    var allFiled = $("#QueryFiled").val().split(',');
    var nowFiled = [];
    if (key != '') {
        for (var i = 0; i < allFiled.length; i++) {
            if ($("#ShowFiled_type").val() == "包含") {
                if (allFiled[i].split('|')[1].indexOf(key) > -1) {
                    nowFiled.push(allFiled[i]);
                }
            }
            else {
                if (allFiled[i].split('|')[1].indexOf(key) ==-1) {
                    nowFiled.push(allFiled[i]);
                }
            }
        }
    }
    else {
        nowFiled = allFiled;
    }
    $("#ShowFiled_row").empty();

    var tmple =
'<div class="col-md-3" style="margin: 1px 0px">'+
'    <div class="input-group input-group-sm">'+
'        <span class="input-group-addon">'+
'            <input type="checkbox" name="AllShow_JSITEM" value="{0}" onclick="BoxListSetV(\'AllShow\')">'+
'        </span>'+
'        <div class="form-control">{1}</div>'+
'    </div>'+
'</div>';
    for (var i = 0; i < nowFiled.length; i++) {
        var tmpArr = nowFiled[i].split('|');
        var thisStr = tmple.replace("{0}", tmpArr[0]).replace("{1}", tmpArr[1]);
        $(thisStr).appendTo($("#ShowFiled_row"));
    }
    $("#AllShow").val();
}


var isOpen = false;
$(window).bind("scroll", function () {
    //当滚动条滚动时
    if (isOpen) {
        $("#dialog_FullWin").dialog({ top: $(document).scrollTop() + nowTop });
    }
});

$(window).resize(function () {
    var sumht = $(this).height();
    $("#dg").datagrid({ height: (sumht) });
});


function GetParStr()
{

    var whereJson= JSON.parse(GetWhereStr())
    var paraJson = JSON.parse($("#AllPara").val())
    var reStr = "";

    for (var i = 0; i < whereJson.length; i++) {
        reStr += whereJson[i].ObjFiled + "=" + whereJson[i].Value + "&";
    }

    for (var i = 0; i < paraJson.length; i++)
    {
        reStr += paraJson[i].ParaName + "=" + paraJson[i].Value + "&";
    }
    return reStr
}

function doSearch() {
    isStop = false
    var whereStr = GetWhereStr()
    if (isStop) return;
    $('#dg').datagrid("loadData", []);
    $("#GetWhereStr").val(whereStr);
    $('#dg').datagrid({
        url: 'QueryList',
        queryParams: {
            AllParaStr: $("#AllPara").val(),
            Code: $("#Code").val(),
            WhereStr: $("#GetWhereStr").val()
        }
    });
    Setpager();
}

function Setpager() {
    var pager = $('#dg').datagrid('getPager');    // get the pager of datagrid
    pager.pagination({
        layout: ['first', 'prev', 'next', 'last', 'refresh'],
        buttons: [
        {
            iconCls: 'icon-help',
            handler: function () {
                $.messager.show({
                    title: "报表说明",
                    msg: "<textarea style='width: 100%;height:100%;border-width: 0;'>" + $("#REMARK").val() + "</textarea>",
                    showType: 'slide',
                    width: 350,
                    height: 200
                });
            }
        }
        ]
    });
}
function SetpagerSimple() {
    var pager = $('#dg').datagrid().datagrid('getPager');    // get the pager of datagrid
    pager.pagination({
        layout: []
    });
}

function GoFilter() {
    SetShow();
    SetAllParaStr();
    doSearch();
}
function SetShow()
{
    var allShowFiled = $("#AllShow").val().split(',')
    $("input[name='AllShow_JSITEM']").each(function () {

        var isShow = false;
        for (var i = 0; i < allShowFiled.length; i++) {
            if (allShowFiled[i] == $(this).val()) {
                isShow = true;
                break;
            }
        }
        if (isShow) {
            $('#dg').datagrid('showColumn', $(this).val());
        }
        else {
            $('#dg').datagrid('hideColumn', $(this).val());
        }
    })

   

}

function ClearFilter() {
    $("#AllPara").val("[]");

    var allFilter = $(".filtrClass");
    for (var i = 0; i < allFilter.length; i++) {
        $(allFilter[i]).val()
    }

    var str = $("#searchField").val();
    var cfgJson = $.parseJSON($("#QUERY_CFG_JSON").val());
    var jsonObj = JSON.parse(str);
    var reJson = [];
    for (var i = 0; i < jsonObj.length; i++) {

        /*行对象*/
        var objColum = null;
        for (var x = 0; x < cfgJson.length; x++) {
            if (cfgJson[x].FieldName == jsonObj[i].FieldName) {
                objColum = cfgJson[x]
            }
        }

        var controlType = "text";
        if (objColum.SearchScript != null && objColum.SearchScript != '') {
            var startP = objColum.SearchScript.indexOf('.');
            controlType = objColum.SearchScript.substring(startP + 1, objColum.SearchScript.indexOf('(', startP));
        }
        $("#s_" + jsonObj[i].FieldName + "_value").val('');

    }
}

function SetAllParaStr() {
    var temp = window.location.href;
    var allFilter = $(".filtrClass");
    var nowpar = [];
    try {
        nowpar = JSON.parse($("#AllPara").val())
    } catch (e) { };

    for (var i = 0; i < allFilter.length; i++) {
        var isAdd = true;
        for (var x = 0; x < nowpar.length; x++) {
            if (nowpar[x].ParaName == allFilter[i].id) {
                nowpar[x].Value = $(allFilter[i]).val();
                isAdd = false;
            }
        }
        if (isAdd) {
            var nowMax = nowpar.length;
            var tmp = { "ParaName": allFilter[i].id, "Value": $(allFilter[i]).val() }
            nowpar[nowMax] = tmp;
        }
    }

    $("#ParaMessage").empty();
    $('<span>&nbsp;</span>').appendTo("#ParaMessage")
    var paraMessageStr = [];
    for (var i = 0; i < nowpar.length; i++) {
        if (nowpar[i].ParaName != "code" && nowpar[i].ParaName != "_" && nowpar[i].ParaName != "authToken" && nowpar[i].ParaName != "") {

            var nameStr = $("#" + nowpar[i].ParaName).prev().html();
            if (nameStr == null || nameStr == '') {
                nameStr = $("#" + nowpar[i].ParaName).parent(".form-control").prev().html();
            }
            var valStr = '';
            try
            {
                valStr = $("#" + nowpar[i].ParaName).combotree("getText");
            } catch (e) { }
            if (valStr == null || valStr == '')
            {
                try {
                    valStr = $("#" + nowpar[i].ParaName).combobox("getText");
                } catch (e) { }
            }
            if (valStr == null || valStr == '')
            {
                try {
                    valStr = $("#" + nowpar[i].ParaName).find("option:selected").text();;
                } catch (e) { }
            }
            
            if (valStr == null || valStr == '') {
                valStr = nowpar[i].Value;
            }

            if (nowpar[i].Value != null && nowpar[i].Value != '' && nameStr!=null && nameStr.length < 20) {
                var tmp = "<span><span style='font-weight:bold'>" + nameStr + "</span>'" + valStr + "'</span>";

                if ("DISTRICT_ID" == nowpar[i].ParaName) {
                    try {
                        tmp = "<span><span style='font-weight:bold'>统计范围</span>：'" + $("#dis").combotree("getText") + "'</span>";
                    } catch (e) {
                        tmp = "";
                    }
                }
                if ("LEVEL_ID" == nowpar[i].ParaName) {
                    try
                    {
                        tmp = "<span><span style='font-weight:bold'>统计层级</span>：'" + $("#dis_level").combotree("getText") + "'</span>";
                    } catch (e)
                    {
                        tmp = "";
                    }
                }
                paraMessageStr[paraMessageStr.length] = tmp;
            }
        }
    }
    $(paraMessageStr.join('；&nbsp;')).appendTo("#ParaMessage")

    var strJson = JSON.stringify(nowpar);
    $("#AllPara").val(strJson);
}


function GetWhereStr() {
    var str = $("#searchField").val();

    var cfgJson = $.parseJSON($("#QUERY_CFG_JSON").val());

    var jsonObj = JSON.parse(str);
    var reJson = [];
    for (var i = 0; i < jsonObj.length; i++) {

        /*行对象*/
        var objColum = null;
        for (var x = 0; x < cfgJson.length; x++) {
            if (cfgJson[x].FieldName == jsonObj[i].FieldName) {
                objColum = cfgJson[x]
            }
        }

        var controlType = "text";
        if (objColum.SearchScript != null && objColum.SearchScript != '') {
            var startP = objColum.SearchScript.indexOf('.');
            controlType = objColum.SearchScript.substring(startP + 1, objColum.SearchScript.indexOf('(', startP));
        }

        v = $("#s_" + jsonObj[i].FieldName + "_value").val();
        if (v == null)
        {
            v='';
        }

        
        if (v.indexOf("'") > -1) {
            alert('不能包含非法字符');
            isStop = true;
            return '';
        }
        var tmp_v = v;
        if (v != "") {
            var opTypeText = $("#s_" + jsonObj[i].FieldName + "_type").val();
            reJson.push(
                {
                    ObjFiled: jsonObj[i].FieldName,
                    OpType: opTypeText,
                    Value: tmp_v,
                    FieldType: jsonObj[i].FieldType,
                    FieldName: jsonObj[i].Alias
                })

            if (jsonObj[i].FieldType == 'System.String' || jsonObj[i].FieldType == 'System.DateTime') {
                tmp_v = "'" + v + "'";
            }
            if ($("#s_" + jsonObj[i].FieldName + "_type").val() == 'like') {
                tmp_v = "'%" + v + "%'";
            }
            var opTypeTextName = $("#s_" + jsonObj[i].FieldName + "_type").find("option:selected").text()
            setTimeout(
                '$("#filter_' + jsonObj[i].FieldName + '").next().html("' + objColum.Alias + '[' + opTypeTextName + ']' + v + '")'
            , 1000);
        }

    }
    $("#filterMessage").empty();
    $('<span>&nbsp;</span>').appendTo("#filterMessage")
    var filterMessageStr = [];
    for (var i = 0; i < reJson.length; i++) {

        filterMessageStr[filterMessageStr.length] = "<span><span style='font-weight:bold'>" + reJson[i].FieldName + "</span>" + reJson[i].OpType + "'" + reJson[i].Value + "'</span>";
    }
    $(filterMessageStr.join('；&nbsp;')).appendTo("#filterMessage")
    var reStr = JSON.stringify(reJson)
    $("#GetWhereStr").val(reStr);
    return reStr;
}

function OnChangeDdl(i) {
    var a = $("#Dis" + i).val();
    $.ajax({
        url: "../District/GetAsyn?id=" + a,
        data: {
            t: Math.random()
        },
        success: function (data) {
            var nowIndex = i + 1;
            for (var x = i + 1; x <= 4; x++) {
                $("#Dis" + x).empty();
                $("<option value=''>所有</option>").appendTo("#Dis" + x)//添加下拉框的option
            }
            for (var x = 0; x < data.length; x++) {
                $("<option value='" + data[x].id + "'>" + data[x].text + "</option>").appendTo("#Dis" + nowIndex)//添加下拉框的option
            }
        },
        error: function (data) {

        }
    });

}

function ShowFilter(event, obj, fieldName) {
    var cfgJson = $.parseJSON($("#QUERY_CFG_JSON").val());
    /*行对象*/
    var objColum = null;
    for (var i = 0; i < cfgJson.length; i++) {
        if (cfgJson[i].FieldName == fieldName) {
            objColum = cfgJson[i]
        }
    }

    var controlType = "text";
    if (objColum.SearchScript != null && objColum.SearchScript != '') {
        var startP = objColum.SearchScript.indexOf('.');
        controlType = objColum.SearchScript.substring(startP + 1, objColum.SearchScript.indexOf('(', startP));
    }
    var typeJson = []
    var getValueScript = '$("#s_value").' + controlType + '("getValue")';
    var setValueScript = '$("#s_value").' + controlType + '("setValue",decodeURI($("#s_' + fieldName + '_value").val()))';
    switch (controlType) {
        case "numberbox":
            typeJson = [
        { "id": "=", "text": "等于" },
        { "id": ">", "text": "大于" },
        { "id": "<", "text": "小于" }
            ]
            break;
        case "datetimebox":
            typeJson = [
        { "id": "=", "text": "等于" },
        { "id": ">", "text": "大于" },
        { "id": "<", "text": "小于" }
            ]
            break;
        case "combobox":
            typeJson = [
        { "id": "=", "text": "等于" }
            ]
            break;
        case "text":
            typeJson = [
        { "id": "=", "text": "等于" },
        { "id": "like", "text": "包含" },
        { "id": "in", "text": "存在" }
            ]
            getValueScript = '$("#s_value").val()';
            setValueScript = '$("#s_value").val(decodeURI($("#s_' + fieldName + '_value").val()))';
            break;
        default:
            typeJson = [
        { "id": "=", "text": "等于" },
        { "id": "like", "text": "包含" },
        { "id": "in", "text": "存在" }
            ]
            break;
    }
    $('#s_type').combobox({
        panelHeight: 'auto',
        editable: false,
        data: typeJson,
        valueField: 'id',
        textField: 'text'
    });

    if (objColum.SearchScript != null) {
        objColum.SearchScript = objColum.SearchScript.replace('{@this}', '#s_value')
    }
    $("#div_input").empty();
    $('<input type="text" id="s_value" style="width:150px" />').appendTo($("#div_input"));
    eval(objColum.SearchScript);

    var now_v = $("#s_" + objColum.FieldName + "_value").val()
    now_v = decodeURI(now_v);
    if (now_v != null && now_v != '') {
        $('#s_type').combobox('setValue', $("#s_" + objColum.FieldName + "_type").val());
        eval(setValueScript);
    }

    $('#dlg_txt').dialog({
        title: '设置[' + objColum.Alias + ']过虑条件',
        closed: false,
        width: 300,
        height: 143,
        buttons: [{
            text: '确定',
            iconCls: 'icon-save',
            handler: function () {
                var opType = $('#s_type').combobox('getValue')
                var opTypeText = "[" + $('#s_type').combobox('getText') + "]"
                var opValue = "";
                try {
                    opValue = eval(getValueScript);
                } catch (e) {
                    opValue = eval($("#s_value").val());
                }
                if (opValue == null || opValue == '') {
                    opType = "";
                    opTypeText = ""
                }
                $('#s_' + objColum.FieldName + '_type').val(opType);
                $("#s_" + objColum.FieldName + "_value").val(opValue);
                doSearch();
                $("#filter_" + fieldName).next().html(objColum.Alias + opTypeText + opValue);
                $('#dlg_txt').dialog("close");
            }
        }, {
            text: '清除',
            iconCls: 'icon-remove',
            handler: function () {
                $("#s_" + objColum.FieldName + "_type").val('');
                $("#s_" + objColum.FieldName + "_value").val('');
                doSearch();
                $("#filter_" + fieldName).next().html(objColum.Alias);
                $('#dlg_txt').dialog("close");
            }
        }]
    });


    event.stopPropagation();
}

function ShowDebug() {
    $.ajax({
        url: "GetDubug?t=" + Math.random(),
        contentType: "application/x-www-form-urlencoded; charset=utf-8",
        data: {
            code: $("#Code").val()
        },
        success: function (data) {
            $.messager.show({
                title: "查询语句：" + $("#NAME").val(),
                msg: "<textarea style='width: 100%;height:100%;border-width: 1;'>" + data + "</textarea>",
                showType: 'slide',
                width: 300,
                height: 200
            });
        },
        error: function (data) {
            alert('失败:' + data.responseText);
        }
    });
}

function ShowHeardBtn() {
    var jsDataHeardBtn = [];
    if ($("#HEARD_BTN").val() != "") {
        try {
            jsDataHeardBtn = JSON.parse($("#HEARD_BTN").val());
        } catch (e) { }
    }
    if (jsDataHeardBtn.length > 5) {

        $($("#HEARD_BTN").val()).appendTo("#toolbar");
    }
    else {

        for (var i = 0; i < jsDataHeardBtn.length; i++) {
            var btn = jsDataHeardBtn[i]
            var allAuthStr = "," + $("#NoAuthority").val() + ",";
            if (allAuthStr.indexOf("," + btn.Name + ",") > -1) {
                continue;
            }

            var s = btn.Url.indexOf('@(');
            while (s > -1) {
                e = btn.Url.indexOf(')');
                var t = btn.Url.substr(s + 2, e - s - 2);
                if (eval(t) != null) {
                    btn.Url = btn.Url.replace("@(" + t + ")", eval(t));
                }
                s = btn.Url.indexOf('@(');
            }


            //btn.Url = " '" + btn.Url;
            if (btn.Parameter != null) {
                for (var a = 0; a < btn.Parameter.length; a++) {
                    if (btn.Url.indexOf('?') > 0) {
                        btn.Url += "&" + btn.Parameter[a].Para + "='+getSelections('" + btn.Parameter[a].ObjValue + "')+'";
                    }
                    else {
                        btn.Url += "?" + btn.Parameter[a].Para + "='+getSelections('" + btn.Parameter[a].ObjValue + "')+'";
                    }
                }
            }
            //btn.Url += "'";
            var cliEven ="";
            if (btn.DialogWidth == '') btn.DialogWidth = '0';
            if (btn.DialogHeigth == '') btn.DialogHeigth = '0';
            switch (btn.DialogMode) {
                case "Ajax":
                    cliEven += 'DelAjaxUrl(\'' + btn.Url + '\',\'' + btn.Name + '\')';
                    break;
                case "Div":
                    cliEven += 'DivEditDialog(\'' + btn.Url + '\',\'' + btn.Name + '\',' + btn.DialogWidth + ',' + btn.DialogHeigth + ')'
                    break;
                case "WinOpen":
                    cliEven += 'WindowOpen(\'' + btn.Url + '\',\'' + btn.Name + '\',' + btn.DialogWidth + ',' + btn.DialogHeigth + ')'
                    break;
                case "DivDialog":
                    cliEven += 'DivDialog(\'' + btn.Url + '\',\'' + btn.Name + '\',' + btn.DialogWidth + ',' + btn.DialogHeigth + ')'
                    break;
                case "TopDiv":
                    cliEven += 'parent.DivOpen(\'' + btn.Url + '\',\'' + btn.Name + '\',' + btn.DialogWidth + ',' + btn.DialogHeigth + ')'
                    break;
                case "JsFun":
                    cliEven += btn.Url
                    break;
            }
            //$('<a class="easyui-linkbutton" data-options="plain:true,iconCls:\'' + btn.IconCls + '\'" onclick="' + cliEven + '">' + btn.Name + '</a>').appendTo("#toolbar");

            var ico = btn.IconCls;
            switch (btn.IconCls) {
                case "icon-add":
                    ico = "glyphicon-plus"
                    break;
                case "icon-remove":
                    ico = "glyphicon glyphicon-remove"
                    break;
                default:
                    ico = btn.IconCls
                    break;
            }
            var t = '  <button type="button" class="btn btn-default" onclick="'+cliEven+'"><span class="glyphicon ' + ico + '"></span> ' + btn.Name + '</button>';
            $(t).appendTo("#but");
        }
    }
    $(".easyui-linkbutton").linkbutton({})
}



var cmenu;
function createColumnMenu() {
    cmenu = $('<div/>').appendTo('body');
    cmenu.menu({
        onClick: function (item) {
            if (item.iconCls == 'icon-ok') {
                $('#dg').datagrid('hideColumn', item.name);
                cmenu.menu('setIcon', {
                    target: item.target,
                    iconCls: 'icon-empty'
                });
            } else {
                $('#dg').datagrid('showColumn', item.name);
                cmenu.menu('setIcon', {
                    target: item.target,
                    iconCls: 'icon-ok'
                });
            }
        }
    });
    var fields = $('#dg').datagrid('getColumnFields');
    for (var i = 0; i < fields.length; i++) {
        var field = fields[i];
        var col = $('#dg').datagrid('getColumnOption', field);
        cmenu.menu('appendItem', {
            text: col.title,
            name: field,
            iconCls: 'icon-ok'
        });
    }
}
function getRowIndex(target) {
    var tr = $(target).closest('tr.datagrid-row');
    return parseInt(tr.attr('datagrid-row-index'));
}
function getRowData(target) {
    var data = $('#dg').datagrid('getData');
    return data.rows[getRowIndex(target)];
}

function GetRequest() {
    var url = location.search; //获取url中"?"符后的字串
    var theRequest = [];
    if (url.indexOf("?") != -1) {
        var str = url.substr(1);
        if (str.indexOf("&") != -1) {
            strs = str.split("&");
            for (var i = 0; i < strs.length; i++) {
                theRequest[i] = { "K": strs[i].split("=")[0], "V": strs[i].split("=")[1] }
                //theRequest[strs[i].split("=")[0]] = unescape(strs[i].split("=")[1]);
            }
        } else {
            theRequest[i] = { "K": str.split("=")[0], "V": str.split("=")[1] }
            //theRequest[str.split("=")[0]] = unescape(str.split("=")[1]);
        }
    }
    return theRequest;
}

function getRowIndex(target) {
    var tr = $(target).closest('tr.datagrid-row');
    return parseInt(tr.attr('datagrid-row-index'));
}
function getRowData(target) {
    var data = $('#dg').datagrid('getData');
    return data.rows[getRowIndex(target)];
}
/*用于删除*/
var isSelect = true;
function getSelections(fieldName) {

    if (!isSelect) {
        return;
    }
    if (fieldName == null || fieldName == '') {
        alert('列名不能为空');
        return;
    }
    var selectRows = $("#dg").datagrid("getSelections");
    var arrayObj = new Array();　//创建一个数组
    for (var i = 0; i < selectRows.length; i++) {
        arrayObj[arrayObj.length] = eval("selectRows[i]." + fieldName);
    }

    var reStr = arrayObj.join(',');
    if (reStr.length < 1) {
        alert('请选中行');
        isSelect = false;
        return;
    }
    isSelect = true;
    return reStr;
}

function DelAjaxUrl(url) {
    if (!isSelect) {
        isSelect = true;
        return;
    }
    url = url.replace("~/", bootPATH);
    if (confirm("确定要执行操作？")) {
        $.ajax({
            url: url,
            data: {
                t: Math.random()
            },
            success: function (data) {
                if (data.IsError != null) {
                    if (data.Message == null) data.Message = "";
                    data.Message = data.Message.replace('\r\n', '<br />')
                    data.Message = data.Message.replace('\\r\\n', '<br />')
                    if (data.IsError) {
                        divalert("失败", data.Message);
                        return;
                    }
                    else {
                        divalert("成功", data.Message);
                    }
                }
                $('#dg').datagrid('reload');
            },
            error: function (data) {
                if (data.IsError != null) {
                    if (data.Message == null) data.Message = "";
                    data.Message = data.Message.replace('\r\n', '<br />')
                    data.Message = data.Message.replace('\\r\\n', '<br />')
                    if (data.IsError) {
                        divalert("失败", data.Message);
                        return;
                    }
                    else {
                        divalert("成功", data.Message);
                    }
                }
                else {
                    alert(data.responseText);
                }
            }
        });
    }
}

var nowTop = 0;
function DivEditDialog(url, title, w, h) {
    url = url.replace("~/", bootPATH);
    $('#openWindowIframe')[0].src = "";
    $('#openWindowIframe')[0].src = url;

    $('#dialog_FullWin').dialog({
        title: title,
        closed: false,
        maximized: true,
        onClose: function () {
            $('#openWindowIframe')[0].src = "";
            $('#dg').datagrid('reload');
            isOpen = false;
        },
        buttons: [{
            text: '保存',
            iconCls: 'icon-save',
            handler: function () {
                var myFrame = document.getElementsByName("openWindowIframe"); //获取所有name为myFrame 的iframe
                for (var i = 0; i < myFrame.length; i++) //进行循环
                {
                    myFrame[i].contentWindow.onSubmit(); //kindSubmit();为iframe页面的方法
                }
            }
        }, {
            text: '取消',
            iconCls: 'icon-cancel',
            handler: function () {
                CloseWin();
            }
        }]
    });
    if (w == 0 || h == 0) {

        $('#dialog_FullWin').dialog("maximize");
    }
    else {
        $('#dialog_FullWin').dialog({
            width: w,
            height: h,
            maximized: false,
            onMaximize: function () {
                $("#dialog_FullWin").parent().css("top", $(document).scrollTop() + "px");
            }
        });
        $('#dialog_FullWin').dialog("center");
    }
    isOpen = true;
    nowTop = $('#dialog_FullWin').panel('options').top - $(document).scrollTop();
    if (nowTop < 0) nowTop = 0;
}

function DivDialog(url, title, w, h) {
    url = url.replace("~/", bootPATH);
    $('#openWindowIframe')[0].src = "";
    $('#openWindowIframe')[0].src = url;
    $('#dialog_FullWin').dialog({
        title: title,
        closed: false,
        maximized: true,
        buttons: [],
        onClose: function () {
            $('#openWindowIframe')[0].src = "";
            $('#dg').datagrid('reload');
            isOpen = false;
        }
    });
    if (w == 0 || h == 0) {

        $('#dialog_FullWin').dialog("maximize");
    }
    else {
        $('#dialog_FullWin').dialog({
            width: w,
            height: h,
            maximized: false,
            onMaximize: function () {
                $("#dialog_FullWin").parent().css("top", $(document).scrollTop() + "px");
            }
        });
        $('#dialog_FullWin').dialog("center");

    }
    isOpen = true;
    nowTop = $('#dialog_FullWin').panel('options').top - $(document).scrollTop();


    if (nowTop < 0) nowTop = 0;
}

function AjaxUrl(url, msg) {
    url = url.replace("~/", bootPATH);
    var i = url.indexOf("{input:");
    while (i > 0) {
        var vmsg = url.substr(i + 7, url.indexOf("}") - i - 7);
        filedV = window.prompt("请输入【" + vmsg + "】的值");
        if (!filedV) return;
        url = url.replace("{input:" + vmsg + "}", filedV);
        i = url.indexOf("{input:");
    }
    if (confirm("确定要执行" + msg + "操作？")) {
        $.ajax({
            url: url,
            data: {
                t: Math.random()
            },
            success: function (data) {
                if (data.IsError != null) {

                    if (data.Message == null) data.Message = "";

                    data.Message = data.Message.replace('\r\n', '<br />')
                    data.Message = data.Message.replace('\\r\\n', '<br />')
                    if (data.IsError) {
                        divalert("失败", data.Message);
                        return;
                    }
                    else {
                        divalert("成功", data.Message);
                    }
                }
                $('#dg').datagrid('reload');
            },
            error: function (data) {
                alert(msg + '失败:' + data.responseText);
            }
        });
    }
}
function CloseWin() {
    $('#dialog_FullWin').dialog('close');
    ReLoadData();
}
function ReLoadData() {
    $('#dg').datagrid('reload');
}

var isStop = false;

function doDown() {
    var authTokenStr = "";
    try {
        authTokenStr = authToken;
    } catch (e) { }
    var options = $('#dg').datagrid('options');
    var code = $("#Code").val();
    var sortName = options.sortName;
    var sortOrder = options.sortOrder;
    var whereStr = GetWhereStr()
    $("#GetWhereStr").val(whereStr);
    var AllParaStr = $("#AllPara").val();
    if (whereStr == null) whereStr = "";
    if (sortName == null) sortName = "";

    window.location = 'DownFile?authToken=' + authTokenStr + '&code=' + code + '&sortName=' + sortName + '&sortOrder=' + sortOrder + '&WhereStr=' + whereStr + '&AllParaStr=' + AllParaStr + '&AllShow=' + $("#AllShow").val()
    //OnBegin();
    //$.ajax({
    //    url: 'DownFile?t=' + Math.random(),
    //    data: {
    //        authToken: authTokenStr,
    //        code: code,
    //        sortName: sortName,
    //        sortOrder: sortOrder,
    //        WhereStr: whereStr,
    //        AllParaStr: AllParaStr,
    //    },
    //    success: function (data) {
    //        OnComplete();
    //        window.location = data
    //    },
    //    error: function (data) {
    //        OnComplete();
    //        if (data.IsError != null) {
    //            if (data.Message == null) data.Message = "";
    //            data.Message = data.Message.replace('\r\n', '<br />')
    //            data.Message = data.Message.replace('\\r\\n', '<br />')
    //            if (data.IsError) {
    //                divalert("失败", data.Message);
    //                return;
    //            }
    //            else {
    //                divalert("成功", data.Message);
    //            }
    //        }
    //        else {
    //            alert(data.responseText);
    //        }
    //    }
    //});

}
function onBeforeLoad() {
    //$("#GetWhereStr").val(GetWhereStr());
    return true;
}
function onLoadSuccess() {
    $('#dg').datagrid('clearChecked');
}



function ShowRowBtn(value, row, index) {
    var jsonObj = [];
    try {
        var jsonObj = JSON.parse($("#ROWS_BTN").val())
    } catch (e)
    { }
    var btnStr = '<div class="div-btn-deal">';
    var butnTxtLeng = 0;
    for (var i = 0; i < jsonObj.length; i++) {

        var btn = jsonObj[i];
        var allAuthStr = "," + $("#NoAuthority").val() + ",";
        if (allAuthStr.indexOf("," + btn.Name + ",") > -1) {
            continue;
        }

        if (jsonObj[i].Url == null || jsonObj[i].Url == "" || jsonObj[i].Url == 'null') {
            continue;
        }

        var s = jsonObj[i].Url.indexOf('@(');
        while (s > -1) {
            e = jsonObj[i].Url.indexOf(')');
            var t = jsonObj[i].Url.substr(s + 2, e - s - 2);
            var evelV = null;
            try {
                evelV = eval(t);
            }
            catch (e) {
            }
            if (evelV == null) {
                jsonObj[i].Url = jsonObj[i].Url.replace("@(" + t + ")", eval("row." + t));
            }
            else {
                jsonObj[i].Url = jsonObj[i].Url.replace("@(" + t + ")", evelV);
            }
            s = jsonObj[i].Url.indexOf('@(');
        }
        if (jsonObj[i].Url == null || jsonObj[i].Url == "" || jsonObj[i].Url == 'null') {
            continue;
        }
        jsonObj[i].Url = jsonObj[i].Url.replace("~/", bootPATH);


        var isShow = false;
        if (jsonObj[i].ShowCondition == null || jsonObj[i].ShowCondition.length == 0) isShow = true;
        for (var x = 0; x < jsonObj[i].ShowCondition.length; x++) {
            var v = jsonObj[i].ShowCondition[x].Value;
            str = "row." + jsonObj[i].ShowCondition[x].ObjFiled + jsonObj[i].ShowCondition[x].OpType + v;
            isShow = eval(str);
            if (isShow) break;
        }
        if (!isShow) continue;
        var paraStr = "?";
        if (jsonObj[i].Url.indexOf('?') > 0) {
            paraStr = "&";
        }
        if (jsonObj[i].Parameter != null) {
            for (var x = 0; x < jsonObj[i].Parameter.length; x++) {
                var v = jsonObj[i].Parameter[x].ObjValue;
                if (v == null) continue;
                if (v == "{input}") {
                    filedV = "{input:" + jsonObj[i].Parameter[x].Para + "}";
                }
                else {
                    var t_str = "";
                    if (jsonObj[i].Parameter[x].ObjValue.indexOf("@(") == 0) {
                        var t = jsonObj[i].Parameter[x].ObjValue.replace("@(", "").replace(")", "")
                        if (eval(t) != null) {
                            t_str = t;
                        }
                    }
                    else {
                        t_str = "row." + jsonObj[i].Parameter[x].ObjValue;
                    }
                    var filedV = eval(t_str);
                }
                paraStr += jsonObj[i].Parameter[x].Para + "=" + filedV + "&"
            }
        }


        var tmp = '<a  href="#" ';
        if (jsonObj[i].DialogWidth == '') jsonObj[i].DialogWidth = '0';
        if (jsonObj[i].DialogHeigth == '') jsonObj[i].DialogHeigth = '0';
        switch (jsonObj[i].DialogMode) {
            case "Ajax":
                tmp += ' onclick="AjaxUrl(\'' + jsonObj[i].Url + paraStr + '\',\'' + jsonObj[i].Name + '\')">';
                break;
            case "Div":
                tmp += ' onclick="DivEditDialog(\'' + jsonObj[i].Url + paraStr + '\',\'' + jsonObj[i].Name + '\',' + jsonObj[i].DialogWidth + ',' + jsonObj[i].DialogHeigth + ')"> '
                break;
            case "WinOpen":
                tmp += ' onclick="WindowOpen(\'' + jsonObj[i].Url + paraStr + '\',\'' + jsonObj[i].Name + '\',' + jsonObj[i].DialogWidth + ',' + jsonObj[i].DialogHeigth + ')"> '
                break;
            case "DivDialog":
                tmp += ' onclick="DivDialog(\'' + jsonObj[i].Url + paraStr + '\',\'' + jsonObj[i].Name + '\',' + jsonObj[i].DialogWidth + ',' + jsonObj[i].DialogHeigth + ')"> '
                break;
            case "TopDiv":
                tmp += ' onclick="parent.DivOpen(\'' + jsonObj[i].Url + paraStr + '\',\'' + jsonObj[i].Name + '\',' + jsonObj[i].DialogWidth + ',' + jsonObj[i].DialogHeigth + ')"> '
                break;
            case "JsFun":
                tmp += ' onclick="' + jsonObj[i].Url + '"> '
                break;
        }
        butnTxtLeng += jsonObj[i].Name.length;
        tmp += ' <span class="btn-deal"><span class="btn-deal">' + jsonObj[i].Name + '</span></span></a>  '
        btnStr += tmp;
    }
    btnStr += '</div>'
    return btnStr;
}





