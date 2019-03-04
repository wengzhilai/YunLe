$(function () {
    $('#tt').tree({
        url: 'AjaxAllModule?roleID=0',
        checkbox: true,
        onLoadSuccess: function (node, data) {
            $('#tt').tree('collapseAll');
        },
        loadFilter: function (rows) {
            return convert(rows, $('#ModuleAllStr').val());
        }
    });


    var strJson = $("#RoleConfigsStr").val();
    if (strJson.length > 5) {
        onClickRow: onClickRow
        $('#dg_RoleConfig').datagrid('loadData', JSON.parse(strJson));
    }
});

var editIndex = undefined;
function onClickRow(index) {
    if (editIndex != index) {
        if (endEditing()) {
            $('#dg_RoleConfig').datagrid('selectRow', index).datagrid('beginEdit', index);
            editIndex = index;
        } else {
            $('#dg_RoleConfig').datagrid('selectRow', editIndex);
        }
    }
}
function endEditing() {
    if (editIndex == undefined) { return true }
    if ($('#dg_RoleConfig').datagrid('validateRow', editIndex)) {

        $('#dg_RoleConfig').datagrid('endEdit', editIndex);
        $('#dg_RoleConfig').datagrid('acceptChanges');
        editIndex = undefined;
        return true;
    } else {
        return false;
    }
}

function onSubmit() {

    var com = $(".combo-text")
    if (com.attr('name') == null) {
        com.attr('name', 'combo_text')
    }
    $("#ModuleAllStr").val(getChecked());

    $('#dg_RoleConfig').datagrid('acceptChanges');

    var data = $('#dg_RoleConfig').datagrid('getData');
    var strJson = JSON.stringify(data.rows);
    $("#RoleConfigsStr").val(strJson)

    var obj = $('input[type="submit"]');
    obj[0].click()
}




function getChecked() {
    var nodes = $('#tt').tree('getChecked', "indeterminate");

    var arrayObj = new Array();　//创建一个数组
    for (var i = 0; i < nodes.length; i++) {
        arrayObj[arrayObj.length] = nodes[i].id;
    }
    var nodes = $('#tt').tree('getChecked');
    for (var i = 0; i < nodes.length; i++) {
        arrayObj[arrayObj.length] = nodes[i].id;
    }

    return arrayObj.join(',');
}