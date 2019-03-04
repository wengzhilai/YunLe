$(function () {
    $('#tt').tree({
        url: 'AjaxAllDistrict?rootId=1',
        onLoadSuccess: function (node, data) {
            $('#tt').tree('collapseAll');
        },
        loadFilter: function (rows) {
            return convert(rows);
        }, onClick: function (node) {
            onSelectTreeItem(node.id);
        }
    });



});


function DeleteById(id) {
    $.ajax({
        url: "Delete?id=" + id,
        success: function () {
            $('#tt').tree('reload');
        }
    });

}
function onSelectTreeItem(id) {
    $.ajax({
        url: "GetDistrictById?id=" + id,
        dataType: 'json',


        success: function (data) {

            $("#ID").val(data["ID"]);
            $("#NAME").val(data["NAME"]);
            $("#IN_USE").val(data["IN_USE"]);

            $("#PARENT_ID").combotree("setValue", data["PARENT_ID"]);

            //alert("ID" + $("#ID").val());
            //alert("NAME" + $("#NAME").val());
            //alert("IN_USE" + $("#IN_USE").val());
            //alert("PARENT_ID" + $("#PARENT_ID").combotree("getValue"));
        }
    });

}
function AjaxSubmit() {

    var ID = $("#ID").val();
    var NAME = $("#NAME").val();
    var IN_USE = $("#IN_USE").val();
    var PARENT_ID = $("#PARENT_ID").combotree("getValue");

    $.ajax({
        url: "Index",
        type: "post",
        data: { ID: ID, NAME: NAME, IN_USE: IN_USE, PARENT_ID: PARENT_ID },


        success: function (data) {
            $('#tt').tree('reload');

        }
    });
}
function ClearForm() {

    $("#ID").val(0);
    $("#NAME").val("");
    $("#IN_USE").val(0);

    $("#PARENT_ID").combotree("setValue", "");

}



function convert(rows) {
    function exists(rows, parentId) {
        for (var i = 0; i < rows.length; i++) {
            if (rows[i].id == parentId) return true;
        }
        return false;
    }

    var nodes = [];
    // get the top level nodes
    for (var i = 0; i < rows.length; i++) {
        var row = rows[i];
        if (!exists(rows, row.parentId)) {
            nodes.push({
                id: row.id,
                text: row.name + '  <a href="javascript:void(0)" onclick="DeleteById(' + row.id + ')"><img src="../Scripts/easyui/themes/icons/no.png" /></a>'
            });
        }
    }



    var toDo = [];
    for (var i = 0; i < nodes.length; i++) {
        toDo.push(nodes[i]);
    }
    while (toDo.length) {
        var node = toDo.shift();	// the parent node

        // get the children nodes
        for (var i = 0; i < rows.length; i++) {
            var row = rows[i];
            if (row.parentId == node.id) {

                var child = { id: row.id, text: row.name + '  <a href="javascript:void(0)" onclick="DeleteById(' + row.id + ')"><img src="../Scripts/easyui/themes/icons/no.png" /></a>', checked: false };


                if (node.children) {

                    node.children.push(child);
                } else {
                    node.children = [child];
                }
                toDo.push(child);
            }
        }
    }
    return nodes;
}



