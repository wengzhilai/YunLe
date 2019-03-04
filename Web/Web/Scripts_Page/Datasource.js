$(function () {

    var objJson = [];
    try {
        objJson = JSON.parse($("#FieldJson").val());
    } catch (e) { }

    $('#CFG_JSON').datagrid({
        singleSelect: true,
        data: objJson,
        pageSize: 100,
        onClickRow: onClickRowCfgJson
    });



    $('#CFG_JSON').datagrid().datagrid('getPager').pagination({
        buttons: [{
            iconCls: 'icon-add',
            handler: function () {
                $('#CFG_JSON').datagrid('appendRow', {
                });
            }
        },
        {
            iconCls: 'icon-remove',
            handler: function () {
                var row = $('#CFG_JSON').datagrid('getSelected');
                if (row) {
                    var index = $('#CFG_JSON').datagrid('getRowIndex', row);
                    $('#CFG_JSON').datagrid('deleteRow', index);
                }
            }
        }
        ,
        {
            iconCls: 'icon-ok',
            handler: function () {
                $('#CFG_JSON').datagrid('acceptChanges');
                var data = $('#CFG_JSON').datagrid('getData');
                var strJson = JSON.stringify(data.rows);
                $("#QUERY_CFG_JSON").val(strJson);
            }
        }
        ]
    });
});

function onSubmit() {
    var com = $(".combo-text")
    if (com.attr('name') == null) {
        com.attr('name', 'combo_text')
    }

    com = $(".textbox-text")
    if (com.attr('name') == null) {
        com.attr('name', 'textbox_text')
    }

    onLoadSuccess();
    var obj = $('input[type="submit"]');
    obj[0].click();
}

function ChangeType() {
    switch ($("#TYPE").val()) {
        case "月":
            return "{YYYYMM}";
            break;
        case "日":
            return "{YYYYMMDD}";
            break;
    }
    return "";
}

function ReloadCfg() {
    var data = $('#CFG_JSON').datagrid('getData');
    var strJson = JSON.stringify(data.rows);
    $('#CFG_JSON').datagrid({
        url: bootPATH + 'Datasource/ReloadCfg',
        idField: 'ID',
        rownumbers: true,
        singleSelect: true,
        pagination: true,
        fitColumns: true,
        toolbar: '#toolbar',
        pageSize: 100,
        onClickRow: onClickRowCfgJson,
        onLoadSuccess: onLoadSuccess,
        onLoadError: onLoadError,
        title: "select * from " + $("#CODE").val(),
        queryParams: {
            sql: "select * from " + $("#CODE").val(),
            nowJsonStr: strJson,
            dbServerId: $("#DB_SERVER_ID").val()
        }
    });
}


function onBeforeLoad(a, b) {
    return true;
}
function onLoadError(a, b,c,d)
{
    alert(a.responseText);
}

var editIndexCfgJson = undefined;
function onClickRowCfgJson(index) {
    if (editIndexCfgJson != index) {
        if (endEditingCfgJson()) {
            $('#CFG_JSON').datagrid('selectRow', index).datagrid('beginEdit', index);
            editIndexCfgJson = index;
        } else {
            $('#CFG_JSON').datagrid('selectRow', editIndexCfgJson);
        }
    }
}
function endEditingCfgJson() {
    if (editIndexCfgJson == undefined) { return true }
    if ($('#CFG_JSON').datagrid('validateRow', editIndexCfgJson)) {

        $('#CFG_JSON').datagrid('endEdit', editIndexCfgJson);
        $('#CFG_JSON').datagrid('acceptChanges');
        editIndexCfgJson = undefined;
        return true;
    } else {
        return false;
    }
}

function onLoadSuccess(a, b, c, d) {

    var data = $('#CFG_JSON').datagrid('getData');
    if (data.total > 0) {
        $("#FieldJson").val(JSON.stringify(data.rows))
    }
}