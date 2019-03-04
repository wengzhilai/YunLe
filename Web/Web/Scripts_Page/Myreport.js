$(function () {
    $('.tablename').datagrid({
        loadMsg: '数据加载中，请稍等。。。。',
        rownumbers: true,
        idField: 'ID',
        columns: [[
        {
            field: 'ID', title: '选择', formatter: function (val, rec, index) {
                return "<input type='checkbox' name='tablename' value=" + rec.ID + ">";
            }
        },
        { field: 'NAME', title: '宽表名', width: 100 }
        ]],
        onClickRow: function (rowIndex, rowData) {
            if ($('input[name="tablename"]').attr("checked")) {
                if ($('input[name="tablename"]').attr('value') == rowData.ID) {
                    $('#' + rowData.ID).css("display", "normal");
                }
            }
        }
    });

    $('.field').datagrid({
        loadMsg: '数据加载中，请稍等。。。。',
        rownumbers: true,
        idField: 'ID',
        columns: [[
        {
            field: 'CK', title: '选择', formatter: function (val, rec, index) {
                return "<input type='checkbox' name='field'  id=" + rec.ID + " value=" + rec.ID + ">";
            }
        },
        { field: 'ID', title: 'ID', width: 100 },
        { field: 'FIELD_NAME', title: '字段名', width: 110 },
        { field: 'FIELD_TYPE', title: '字段类型', width: 120 }
        ]],
        onClickRow: function (rowIndex, rowData) {
            if ($('#' + rowData.ID).attr("checked")) {
                $('#SOURCE_FILED_B').html('');
                $('#fieldname').html($('#' + rowData.ID).attr('id'));
                if ($('#' + rowData.ID).attr("value") == rowData.ID) {
                    if (rowData.FIELD_TYPE == "int") {
                        $('#SOURCE_FILED_B').html("<option value='大于'>大于</option><option value='等于'>等于</option><option value='小于'>小于</option><option value='存在'>存在</option>");
                    }
                    else if (rowData.FIELD_TYPE == "string") {
                        $('#SOURCE_FILED_B').html("<option value='包含'>包含</option><option value='等于'>等于</option><option value='存在'>存在</option>");
                    }
                    else if (rowData.FIELD_TYPE == "Date") {
                        $('#SOURCE_FILED_B').html("<option value='大于'>大于</option><option value='等于'>等于</option><option value='小于'>小于</option>");
                    }
                }
            }
            else {
                $('#SOURCE_FILED_B').html(''); 
                $('#fieldname').html('');
            }
        }
    });
});
