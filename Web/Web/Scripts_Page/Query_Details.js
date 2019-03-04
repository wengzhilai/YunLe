
var products = [{ ID: "Div", NAME: "Div编辑框" }, { ID: "DivDialog", NAME: "Div对话框" }, { ID: "Ajax", NAME: "Ajax执行" }, { ID: "WinOpen", NAME: "弹出框" }, { ID: "TopDiv", NAME: "置顶Div对话框" }, { ID: "JsFun", NAME: "JS方法" }];
$(function () {
    var jsData = [];
    if ($("#QUERY_CFG_JSON").val() != "") {
        jsData = JSON.parse($("#QUERY_CFG_JSON").val());
    }
    $('#CFG_JSON').datagrid({
        data: jsData,
        idField: 'FieldName',
        rownumbers: true,
        singleSelect: true,
        pagination: true,
        fitColumns: true,
        onClickRow: onClickRow,
        pageSize: 10
    });
    


    //显示添加，修改按键
    ShowQueryCfgPage();






    var jsDataHeardBtn = [];
    if ($("#HEARD_BTN").val() != "") {
        try{
            jsDataHeardBtn = JSON.parse($("#HEARD_BTN").val());
        }catch(e){}
    }
    $('#dg_HEARD_BTN').datagrid({
        data: jsDataHeardBtn,
        rownumbers: true,
        singleSelect: true,
        pagination: true,
        fitColumns: true,
        onClickRow: onCl,
        pageSize: 10,
        view: detailview,
        detailFormatter: function (index, row) {
            return '<div class="ddv" style="padding:2px"><table class="ddv"></table></div>';
        },
        onExpandRow: function (index, row) {
            var ddv = $(this).datagrid('getRowDetail', index).find('table.ddv');
            var thisPara = row.Parameter;
            ddv.datagrid({
                fitColumns: true,
                singleSelect: true,
                pagination: true,
                onClickRow: function (rowIndex, rowData) {
                    ddv.datagrid('acceptChanges');
                    ddv.datagrid('selectRow', rowIndex).datagrid('beginEdit', rowIndex);
                },
                height: 'auto',
                title: '参数',
                data: thisPara,
                columns: [[
                    { field: 'Para', title: '参数', width: 100, editor: 'text' },
                    { field: 'ObjValue', title: '对象值', width: 100, editor: 'text' }
                ]],
                onResize: function () {
                    $('#dg_HEARD_BTN').datagrid('fixDetailRowHeight', index);
                },
                onLoadSuccess: function () {
                    setTimeout(function () {
                        $('#dg_HEARD_BTN').datagrid('fixDetailRowHeight', index);
                    }, 0);
                }
            });

            var pager = ddv.datagrid().datagrid('getPager');    // get the pager of datagrid
            pager.pagination({
                buttons: [{
                    iconCls: 'icon-add',
                    handler: function (a,b) {
                        ddv.datagrid('appendRow', {
                            Para: 'id',
                            ObjValue: 'ID',
                        });
                    }
                },
                {
                    iconCls: 'icon-remove',
                    handler: function (a,b) {
                        var row = ddv.datagrid('getSelected');
                        if (row) {
                            var indexRow = ddv.datagrid('getRowIndex', row);
                            ddv.datagrid('deleteRow', indexRow);
                        }
                    }
                }
                ,
                {
                    iconCls: 'icon-ok',
                    handler: function () {
                        ddv.datagrid('acceptChanges');
                        var data = ddv.datagrid('getData');
                        row.Parameter = data.rows;
                    }
                }
                ]
            });

            $('#dg_HEARD_BTN').datagrid('fixDetailRowHeight', index);
        }
    });


    var pager = $('#dg_HEARD_BTN').datagrid().datagrid('getPager');    // get the pager of datagrid
    pager.pagination({
        buttons: [{
            iconCls: 'icon-add',
            handler: function () {
                $('#dg_HEARD_BTN').datagrid('appendRow', {
                    DialogMode: 'JsFun',
                    Name: '导出',
                    IconCls: 'glyphicon glyphicon-download-alt',
                    Url: 'doDown()',
                    DialogWidth: 0,
                    DialogHeigth: 0,
                    Parameter: []
                });
                $('#dg_HEARD_BTN').datagrid('appendRow', {
                    DialogMode: 'Div',
                    Name: '添加',
                    IconCls: 'icon-add',
                    Url: '~//Details',
                    DialogWidth: 0,
                    DialogHeigth: 0,
                    Parameter:[]
                });
                $('#dg_HEARD_BTN').datagrid('appendRow', {
                    "DialogMode": "Ajax",
                    "Name": "删除",
                    "IconCls": "icon-remove",
                    "Url": "~//Delete",
                    "DialogWidth": 0,
                    "DialogHeigth": 0,
                    "Parameter": [{ "Para": "id", "ObjValue": "ID" }]
                });
            }
        },
        {
            iconCls: 'icon-remove',
            handler: function () {
                var row = $('#dg_HEARD_BTN').datagrid('getSelected');
                if (row) {
                    var index = $('#dg_HEARD_BTN').datagrid('getRowIndex', row);
                    $('#dg_HEARD_BTN').datagrid('deleteRow', index);
                }
            }
        }
        ,
        {
            iconCls: 'icon-ok',
            handler: function () {
                $('#dg_HEARD_BTN').datagrid('acceptChanges');
                var data = $('#dg_HEARD_BTN').datagrid('getData');
                var strJson = JSON.stringify(data.rows);
                $("#HEARD_BTN").val(strJson);
            }
        }
        ]
    });
    
    var rowsBtnSsdata = [];
    if ($("#ROWS_BTN").val() != "") {
        try {
            rowsBtnSsdata = JSON.parse($("#ROWS_BTN").val());
        } catch (e) { }
    }

    $('#dg_ROWS_BTN').datagrid({
        view: detailview,
        detailFormatter: function (index, row) {
            return '<div style="padding:2px"><table id="ddv-' + index + '-2"></table><br /><table id="ddv-' + index + '-1"></table></div>';
        },
        height: 280,
        onExpandRow: function (index, row) {
            nowBtnIndex = index;
            var thisCond = row.ShowCondition;
            var thisPara = row.Parameter;
            $('#ddv-' + index + '-1').datagrid({
                fitColumns: true,
                singleSelect: true,
                loadMsg: '',
                height: 'auto',
                title: '显示条件',
                data: thisCond,
                columns: [[
                    { field: 'ObjFiled', title: '对象字段', width: 100, editor: 'text' },
                    { field: 'OpType', title: '操作符', width: 60, editor: 'text' },
                    { field: 'Value', title: '值', width: 100, editor: 'text' },
                    {
                        field: 'action', title: '操作', width: 80, align: 'center',
                        formatter: function (value, row, index) {
                            if (row.editing) {
                                var s = '<a href="javascript:void(0)" onclick="saverow1(this)">保存</a> ';
                                var c = '<a href="javascript:void(0)" onclick="cancelrow1(this)">取消</a>';
                                return s + c;
                            } else {
                                var e = '<a href="javascript:void(0)" onclick="editrow1(this)">修改</a> ';
                                var d = '<a href="javascript:void(0)" onclick="deleterow1(this)">删除</a>';
                                return e + d;
                            }
                        }
                    }
                ]],
                onBeforeEdit: function (index, row) {
                    row.editing = true;
                    updateActions1(index);
                },
                onAfterEdit: function (index, row) {
                    row.editing = false;
                    updateActions1(index);
                },
                onCancelEdit: function (index, row) {
                    row.editing = false;
                    updateActions1(index);
                },
                onResize: function () {
                    $('#dg_ROWS_BTN').datagrid('fixDetailRowHeight', index);
                },
                onLoadSuccess: function () {
                    setTimeout(function () {
                        $('#dg_ROWS_BTN').datagrid('fixDetailRowHeight', index);
                    }, 0);
                }
            });


            $('#ddv-' + index + '-2').datagrid({
                fitColumns: true,
                singleSelect: true,
                loadMsg: '',
                height: 'auto',
                title: '参数',
                data: thisPara,
                columns: [[
                    { field: 'Para', title: '参数', width: 100, editor: 'text' },
                    { field: 'ObjValue', title: '对象值', width: 100, editor: 'text' },
                    {
                        field: 'action', title: '操作', width: 80, align: 'center',
                        formatter: function (value, row, index) {
                            if (row.editing) {
                                var s = '<a href="javascript:void(0)" onclick="saverow2(this)">保存</a> ';
                                var c = '<a href="javascript:void(0)" onclick="cancelrow2(this)">取消</a>';
                                return s + c;
                            } else {
                                var e = '<a href="javascript:void(0)" onclick="editrow2(this)">修改</a> ';
                                var d = '<a href="javascript:void(0)" onclick="deleterow2(this)">删除</a>';
                                return e + d;
                            }
                        }
                    }
                ]],
                onBeforeEdit: function (index, row) {
                    row.editing = true;
                    updateActions2(index);
                },
                onAfterEdit: function (index, row) {
                    row.editing = false;
                    updateActions2(index);
                },
                onCancelEdit: function (index, row) {
                    row.editing = false;
                    updateActions2(index);
                },
                onResize: function () {
                    $('#dg_ROWS_BTN').datagrid('fixDetailRowHeight', index);
                },
                onLoadSuccess: function () {
                    setTimeout(function () {
                        $('#dg_ROWS_BTN').datagrid('fixDetailRowHeight', index);
                    }, 0);
                }
            });
            $('#dg').datagrid('fixDetailRowHeight', index);
        },
        singleSelect: true,
        idField: 'ID',
        data: rowsBtnSsdata,
        columns: [[
            {
                field: 'DialogMode', title: '对话框模式', width: 120,
                formatter: function (value) {
                    for (var i = 0; i < products.length; i++) {
                        if (products[i].ID == value) return products[i].NAME;
                    }
                    return value;
                },
                editor: {
                    type: 'combobox',
                    options: {
                        valueField: 'ID',
                        textField: 'NAME',
                        data: products,
                        required: true,
                        panelHeight: "auto"
                    }
                }
            },
            { field: 'Name', title: '按钮名', width: 80, editor: 'text' },
            { field: 'Url', title: '地址', width: 140, editor: 'text' },
            { field: 'DialogWidth', title: '对话框宽', width: 60, editor: 'text' },
            { field: 'DialogHeigth', title: '对话框高', width: 60, editor: 'text' },
            {
                field: 'action', title: '操作', width: 160, align: 'center',
                formatter: function (value, row, index) {
                    if (row.editing) {
                        var s = '<a href="javascript:void(0)" onclick="saverow(this)">保存</a> ';
                        var c = '<a href="javascript:void(0)" onclick="cancelrow(this)">取消</a>';

                        return s + c;
                    } else {
                        var e = '<a href="javascript:void(0)" onclick="editrow(this)">修改</a> ';
                        var a = '<a href="javascript:void(0)" onclick="insertPara(this)">参数</a> ';
                        var b = '<a href="javascript:void(0)" onclick="insertContion(this)">条件</a>';
                        return e + a + b;
                    }
                }
            }
        ]],
        onBeforeEdit: function (index, row) {
            row.editing = true;
            nowBtnIndex = index
            updateActions(index);
        },
        onAfterEdit: function (index, row) {
            row.editing = false;
            nowBtnIndex = index
            updateActions(index);
        },
        onCancelEdit: function (index, row) {
            row.editing = false;
            nowBtnIndex = index
            updateActions(index);
        }
    });
    var pager = $('#dg_ROWS_BTN').datagrid().datagrid('getPager');    // get the pager of datagrid
    pager.pagination({
        buttons: [{
            iconCls: 'icon-add',
            handler: function () {
                var row = $('#dg_ROWS_BTN').datagrid('getSelected');
                if (row) {
                    var index = $('#dg_ROWS_BTN').datagrid('getRowIndex', row);
                } else {
                    index = 0;
                }
                $('#dg_ROWS_BTN').datagrid('appendRow', {
                    DialogMode: 'Div',
                    Name: '修改',
                    DialogWidth: 550,
                    DialogHeigth: 250,
                    ShowCondition: [],
                    Parameter: [{ Para: 'id', ObjValue: 'ID' }]
                });
            }
        },
        {
            iconCls: 'icon-remove',
            handler: function () {

                $.messager.confirm('警告', '确定要删除吗?', function (r) {
                    if (r) {
                        var row = $('#dg_ROWS_BTN').datagrid('getSelected');
                        if (row) {
                            var index = $('#dg_ROWS_BTN').datagrid('getRowIndex', row);
                            $('#dg_ROWS_BTN').datagrid('deleteRow', index);
                        }
                    }
                });

                
            }
        }
        ,
        {
            iconCls: 'icon-ok',
            handler: function () {

                $('#dg_HEARD_BTN').datagrid('acceptChanges');
                var data = $('#dg_HEARD_BTN').datagrid('getData');
                var strJson = JSON.stringify(data.rows);
                $("#HEARD_BTN").val(strJson);

                $('#dg_ROWS_BTN').datagrid('acceptChanges');
                var data = $('#dg_ROWS_BTN').datagrid('getData');
                for (var i = 0; i < data.rows.length; i++) {
                    try {
                        data.rows[i].ShowCondition = $('#ddv-' + i + '-1').datagrid('getData').rows;
                        data.rows[i].Parameter = $('#ddv-' + i + '-2').datagrid('getData').rows;
                    } catch (e) {
                        data.rows[i].ShowCondition = [];
                    }
                }
                var strJson = JSON.stringify(data.rows);
                $("#ROWS_BTN").val(strJson)
            }
        }]
    });
});

function ShowQueryCfgPage()
{
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
                editIndex = undefined;
                var data = $('#CFG_JSON').datagrid('getData');
                var strJson = JSON.stringify(data.rows);
                $("#QUERY_CFG_JSON").val(strJson);
            }
        }
        ]
    });
}


function OnChangeSearchType(newValue, oldValue) {
    if (editIndex == null) return;
    var ed = $('#CFG_JSON').datagrid('getEditor', { index: editIndex, field: 'SearchScript' });
    var v = "";

    switch (newValue)
    {
        case "numberbox":
            v = "$('{@this}').numberbox({min:0,precision:0});"
            break;
        case "datetimebox":
            v = "$('{@this}').datetimebox({showSeconds: false,required: false});"
            break;
        case "combobox":
            v="$('{@this}').combobox({\r\n";
            v+="panelHeight:'auto',\r\n";
            v+="editable:false,\r\n";
            v+="valueField: 'id',\r\n";
            v+="textField: 'text',\r\n";
            v+="data:[\r\n";
            v+="{'id':'是','text':'是'},\r\n";
            v+="{'id':'否','text':'否'}\r\n";
            v+="]\r\n";
            v += "})";
            break;
        case "combobox_1":
            v = "$('{@this}').combobox({\r\n";
            v += "panelHeight:'auto',\r\n";
            v += "editable:false,\r\n";
            v += "valueField: 'ID',\r\n";
            v += "textField: 'TEXT',\r\n";
            v += "url: 'ExecSql?SQL=SELECT ID,NAME TEXT FROM PS_USER'\r\n";
            v += "})";
            break;
    }
    $(ed.target).val(v);
}
function DownRDLC() {
    if ($("#ID").val() != "") {
        window.location = "DownRDLC?id=" + $("#ID").val();
    }
    else {
        alert('请先保存');
    }
}



var editIndex = undefined;
function onClickRow(index) {
    if (editIndex != index) {
        if (endEditing()) {
            $('#CFG_JSON').datagrid('selectRow', index).datagrid('beginEdit', index);
            editIndex = index;
        } else {
            $('#CFG_JSON').datagrid('selectRow', editIndex);
        }
    }
}
function endEditing() {
    if (editIndex == undefined) { return true }
    if ($('#CFG_JSON').datagrid('validateRow', editIndex)) {
        $('#CFG_JSON').datagrid('endEdit', editIndex);
        $('#CFG_JSON').datagrid('acceptChanges');
        editIndex = undefined;
        return true;
    } else {
        return false;
    }
}

function onCl(rowIndex, rowData) {
    $('#dg_HEARD_BTN').datagrid('acceptChanges');
    $('#dg_HEARD_BTN').datagrid('selectRow', rowIndex).datagrid('beginEdit', rowIndex);
}

var editIndexHeardBtn = undefined;
function onClickRowHeardBtn(index) {
    if (editIndexHeardBtn != index) {
        if (endEditingHeardBtn()) {
            $('#dg_HEARD_BTN').datagrid('selectRow', index).datagrid('beginEdit', index);
            editIndexHeardBtn = index;
        } else {
            $('#dg_HEARD_BTN').datagrid('selectRow', editIndexHeardBtn);
        }
    }
}
function endEditingHeardBtn() {
    if (editIndexHeardBtn == undefined) { return true }
    if ($('#dg_HEARD_BTN').datagrid('validateRow', editIndexHeardBtn)) {
        $('#dg_HEARD_BTN').datagrid('endEdit', editIndexHeardBtn);
        $('#dg_HEARD_BTN').datagrid('acceptChanges');
        editIndexHeardBtn = undefined;
        return true;
    } else {
        return false;
    }
}

function ReloadCfg() {

    $.ajax({
        url: 'ReloadCfg',
        type: "post",
        dataType:'json',
        data: {
            sql: $("#QUERY_CONF").val(),
            code: $("#CODE").val()
        },
        success: function (data) {
            if (data.IsError != null) {
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
                $('#CFG_JSON').datagrid({
                    data: data,
                    idField: 'ID',
                    rownumbers: true,
                    singleSelect: true,
                    pagination: true,
                    fitColumns: true,
                    pageSize: 100,
                    onLoadSuccess: onLoadSuccess
                });
                ShowQueryCfgPage();
            }
        },
        error: function (data) {
            alert('失败:' + data.responseText);
        }
    });



}

function onLoadSuccess(data) {
    var data = $('#CFG_JSON').datagrid('getData');
    var strJson = JSON.stringify(data.rows);
    $("#QUERY_CFG_JSON").val(strJson)
}



var nowBtnIndex = 0;


function updateActions(index) {
    $('#dg_ROWS_BTN').datagrid('updateRow', {
        index: index,
        row: {}
    });
}

function updateActions1(index) {
    var id = 'ddv-' + nowBtnIndex + '-1'
    $('#' + id).datagrid('updateRow', {
        index: index,
        row: {}
    });
}
function updateActions2(index) {
    var id = 'ddv-' + nowBtnIndex + '-2'
    $('#' + id).datagrid('updateRow', {
        index: index,
        row: {}
    });
}
function getRowIndex(target) {
    var tr = $(target).closest('tr.datagrid-row');
    return parseInt(tr.attr('datagrid-row-index'));
}
function editrow(target) {
    $('#dg_ROWS_BTN').datagrid('beginEdit', getRowIndex(target));
}
function editrow1(target) {
    var id = 'ddv-' + nowBtnIndex + '-1'
    $('#' + id).datagrid('beginEdit', getRowIndex(target));
}
function editrow2(target) {
    var id = 'ddv-' + nowBtnIndex + '-2'
    $('#' + id).datagrid('beginEdit', getRowIndex(target));
}

function deleterow1(target) {
    var id = 'ddv-' + nowBtnIndex + '-1'
    $.messager.confirm('警告', '确定要删除吗?', function (r) {
        if (r) {
            $('#' + id).datagrid('deleteRow', getRowIndex(target));
        }
    });
}
function deleterow2(target) {
    var id = 'ddv-' + nowBtnIndex + '-2'
    $.messager.confirm('警告', '确定要删除吗?', function (r) {
        if (r) {
            $('#' + id).datagrid('deleteRow', getRowIndex(target));
        }
    });
}
function saverow(target) {
    $('#dg_ROWS_BTN').datagrid('endEdit', getRowIndex(target));
}
function saverow1(target) {
    var id = 'ddv-' + nowBtnIndex + '-1'
    $('#' + id).datagrid('endEdit', getRowIndex(target));
}
function saverow2(target) {
    var id = 'ddv-' + nowBtnIndex + '-2'
    $('#' + id).datagrid('endEdit', getRowIndex(target));
}
function cancelrow(target) {
    $('#dg_ROWS_BTN').datagrid('cancelEdit', getRowIndex(target));
}
function cancelrow1(target) {
    var id = 'ddv-' + nowBtnIndex + '-1'
    $('#' + id).datagrid('cancelEdit', getRowIndex(target));
}
function cancelrow2(target) {
    var id = 'ddv-' + nowBtnIndex + '-2'
    $('#' + id).datagrid('cancelEdit', getRowIndex(target));
}




function insertPara() {
    var id = 'ddv-' + nowBtnIndex + '-2'
    $('#' + id).datagrid('appendRow', {});
}
function insertContion() {
    var id = 'ddv-' + nowBtnIndex + '-1'
    $('#' + id).datagrid('appendRow', {});
}