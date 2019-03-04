
$(function () {
    //$('#PARENT_ID').combobox({
    //    valueField: 'id',
    //    textField: 'text',
    //    onLoadSuccess: function (rec) {
    //        alert('ddd');
    //    }
    //});
});



function newUser()
{
    var url = bootPATH+"Home/Iframe?url=" + "Details";
    $('#dialog_DD').dialog({
        title: '编辑模块',
        width: 700,
        height: 500,
        closed: false,
        cache: false,
        href: url,
        modal: true
    });
}

function editUser() {

    var row = $('#dg').datagrid('getSelected');
    if (!row) {
        alert('请选中一行');
        return;
    }

    var url = bootPATH + "Home/Iframe?url=" + "Details?id=" + row.ID;
    $('#dialog_DD').dialog({
        title: '编辑模块',
        width: 700,
        height: 500,
        closed: false,
        cache: false,
        href: url,
        modal: true
    });
}


function destroyUser() {


    var rows = $('#dg').datagrid('getSelections');
    if (rows.length > 0) {
        if (confirm("确定删除选中记录？")) {
            var ids = [];
            for (var i = 0, l = rows.length; i < l; i++) {
                var r = rows[i];
                ids.push(r.ID);
            }
            var id = ids.join(',');
            $.ajax({
                url: "Delete?id=" + id,
                success: function (text) {
                    $('#dg').datagrid('reload');
                },
                error: function () {
                }
            });
        }
    } else {
        alert("请选中一条记录");
    }
}
