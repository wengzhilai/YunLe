$(function () {
    var objJson = [];
    try {
        objJson = JSON.parse($("#ToServerStr").val());
    } catch (e) { }

    $('#TO_SERVER').datagrid({
        idField: 'ID',
        rownumbers: true,
        singleSelect: true,
        data: objJson,
        pageSize: 100,
        onClickRow: function (rowIndex, rowData) {
            $('#TO_SERVER').datagrid('acceptChanges');
            $('#TO_SERVER').datagrid('selectRow', rowIndex).datagrid('beginEdit', rowIndex);
        }
    });


    var objJson1 = [];
    try {
        objJson1 = JSON.parse($("#FILED").val());
    } catch (e) { }

    $('#CFG_JSON').datagrid({
        idField: 'ID',
        rownumbers: true,
        singleSelect: true,
        data: objJson1,
        pageSize: 100,
        onClickRow: function (rowIndex, rowData) {
            $('#CFG_JSON').datagrid('acceptChanges');
            $('#CFG_JSON').datagrid('selectRow', rowIndex).datagrid('beginEdit', rowIndex);
        }
    });


})




function onSubmit() {
    var com = $(".combo-text")
    if (com.attr('name') == null) {
        com.attr('name', 'combo_text')
    }

    com = $(".textbox-text")
    if (com.attr('name') == null) {
        com.attr('name', 'textbox_text')
    }

    $('#TO_SERVER').datagrid('acceptChanges');
    var data = $('#TO_SERVER').datagrid('getData');
    var strJson = JSON.stringify(data.rows);
    $("#ToServerStr").val(strJson)

    $('#CFG_JSON').datagrid('acceptChanges');
    var data = $('#CFG_JSON').datagrid('getData');
    var strJson = JSON.stringify(data.rows);
    $("#FILED").val(strJson)

    var obj = $('input[type="submit"]');
    obj[0].click()
}



function ReloadCfg() {
    if ($("#FormServerStr").val() == '') {
        alert('选择下载的服务器');
        return;
    }
    $('#CFG_JSON').datagrid({
        url: 'ReloadFiledCfg',
        idField: 'ID',
        rownumbers: true,
        singleSelect: true,
        pageSize: 100,
        onClickRow: function (rowIndex, rowData) {
            $('#CFG_JSON').datagrid('acceptChanges');
            $('#CFG_JSON').datagrid('selectRow', rowIndex).datagrid('beginEdit', rowIndex);
        },
        onLoadSuccess: function () {
            try {
                var data = $('#CFG_JSON').datagrid('getData');
                var strJson = JSON.stringify(data.rows);
                $("#FILED").val(strJson);
            }
            catch (e) {
                alert('加格式错误');
            }
        },
        onLoadError: function (a) {
            alert("失败：\r\n" + a.responseText);
        },
        queryParams: {
            sql: $("#SELECT_SCRIPT").val(),
            idArrStr: $("#FormServerStr").val(),
            replaceStr: '[]'
        }
    });
}
function MakeCreateScript() {
    var data = $('#CFG_JSON').datagrid('getData');
    if (data == null) {
        alert('请先获取数据');
    }
    var strJson = JSON.stringify(data.rows);
    $("#FILED").val(strJson)
    $.ajax({
        url: "DataDownGetCreateScript",
        type: "POST",
        data: {
            tableName: $("#CREATE_TABLE_NAME").val(),
            createType: $("#CREATE_TYPE").val(),
            filedListStr: strJson
        },
        success: function (text) {
            $("#CREATE_SCRIPT").val(text);
        },
        error: function (e) {
            alert("生成失败:"+e.responseText);
        }
    });
}


