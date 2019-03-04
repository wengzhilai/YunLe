var now_C;
var instance;
var nowMenuObj;
$(function () {
    $("#tabs").tabs();
    $("#editNodeFolw").dialog({
        autoOpen: false,
        minHeight: 200,
        minWidth: 600
    });

    $(".easyui-linkbutton").button();
    if ($("#X_Y").val() != "") {
        var placeJson = JSON.parse($("#X_Y").val());
        for (var i = 0; i < placeJson.length; i++) {
            AddFolw(placeJson[i].Id, $("#menu_" + placeJson[i].Id).text(), placeJson[i].X, placeJson[i].Y);
        }
    }

    var FlowList = JSON.parse($("#FlowListStr").val());
    var x = 0;
    var y = 0;
    for (var i = 0; i < FlowList.length; i++) {
        var s = "NODE_" + FlowList[i].FROM_FLOWNODE_ID;
        var t = "NODE_" + FlowList[i].TO_FLOWNODE_ID;
        if ($("#" + s).html() == null) {
            AddFolw(FlowList[i].FROM_FLOWNODE_ID, $("#menu_" + FlowList[i].FROM_FLOWNODE_ID).text(), (x * 50) + 100, 10);
            x++;
        }
        if ($("#" + t).html() == null) {
            AddFolw(FlowList[i].TO_FLOWNODE_ID, $("#menu_" + FlowList[i].TO_FLOWNODE_ID).text(), 10, (y * 50) + 100);
            y++;
        }
    }





    $('.MenuBtn').draggable({
        revert: true,
        proxy: 'clone',
        start: function () {
            $(this).draggable('option', { style: "cursor:not-allowed;z-index:10" });
        },
        stop: function (event, ui) {
            $(this).draggable('option', { style: "cursor:move" });
            var d = ui.offset;
            var nodeID = $(this).attr('nodeID');
            var _left = d.left - parseInt($("#page_man").css("left").replace('px', ''));
            var _top = d.top - parseInt($("#page_man").css("top").replace('px', ''));

            if (AddFolw(nodeID, $(this).text(), _left, _top)) {
                setJsPlumb();
            }
        }
    });


    jsPlumb.ready(function () {

        instance = jsPlumb.getInstance({
            Endpoint: ["Dot", { radius: 2 }],
            HoverPaintStyle: { strokeStyle: "#1e8151", lineWidth: 2 },
            ConnectionOverlays: [
                ["Arrow", {
                    location: 1,
                    id: "arrow",
                    length: 14,
                    foldback: 0.8
                }],
                ["Label", { label: "FOO", id: "label", cssClass: "aLabel" }]
            ],
            Container: "page_man"
        });

        window.jsp = instance;
        
        setJsPlumb();

        var FlowList = JSON.parse($("#FlowListStr").val());
        for (var i = 0; i < FlowList.length; i++) {
            var s = "NODE_" + FlowList[i].FROM_FLOWNODE_ID;
            var t = "NODE_" + FlowList[i].TO_FLOWNODE_ID;
            instance.connect({
                source: s,
                target: t,
                label: FlowList[i].STATUS,
                labelStyle: {
                    cssClass: "aLabel"
                }
            });
        }
    });
});
/*
当指定人修改时激发
*/
function ChangeAssigner()
{
    //$('#tr_role').hide();
    //if ($('#ASSIGNER').val() == '0') {
    //    $('#tr_role').show()
    //}
}

function setJsPlumb()
{
    $("#page_man .w").bind('contextmenu', function (e) {
        e.preventDefault();
        nowMenuObj = this;

        $('#mm').menu('show', {
            left: e.pageX,
            top: e.pageY
        });
    });


    instance.draggable($("#page_man .w"));

    instance.bind("dblclick", function (connection, originalEvent) {
        now_C = connection;
        var jsonObj = JSON.parse($("#FlowListStr").val());
        var isExist = false;
        for (var i = 0; i < jsonObj.length; i++) {
            if ("NODE_" + jsonObj[i].FROM_FLOWNODE_ID == connection.sourceId && "NODE_" + jsonObj[i].TO_FLOWNODE_ID == connection.targetId) {
                $("#FROM_FLOWNODE_ID").val(jsonObj[i].FROM_FLOWNODE_ID);
                $("#TO_FLOWNODE_ID").val(jsonObj[i].TO_FLOWNODE_ID);
                $("#HANDLE").val(jsonObj[i].HANDLE);
                $("#STATUS").val(jsonObj[i].STATUS);
                $("#flowRemark").val(jsonObj[i].REMARK);
                $("#AllRoleStr").val(jsonObj[i].AllRoleStr);
                $("#ASSIGNER").val(jsonObj[i].ASSIGNER);
                $("#EXPIRE_HOUR").val(jsonObj[i].EXPIRE_HOUR);

                BoxListDisplayV('AllRoleStr');
                isExist = true;

                ChangeAssigner();
                break;
            }
        }
        if (!isExist) {
            jsPlumb.detach(connection);
        }
        else {
            $('#editNodeFolw').dialog('open')
        }
        //$("#FlowListStr").val(JSON.stringify(jsonObj));
        //alert("double click on connection from " + connection.sourceId + " to " + connection.targetId);
    });

    instance.bind("connection", function (info) {
        var jsonObj = JSON.parse($("#FlowListStr").val());
        var isAdd = true;
        for (var i = 0; i < jsonObj.length; i++) {
            if ("NODE_" + jsonObj[i].FROM_FLOWNODE_ID == info.sourceId && "NODE_" + jsonObj[i].TO_FLOWNODE_ID == info.targetId) {
                isAdd = false;
                info.connection.getOverlay("label").setLabel(jsonObj[i].STATUS);
                break;
            }
        }
        if (isAdd) {
            var t = {
                FROM_FLOWNODE_ID: info.sourceId.replace('NODE_', ''),
                TO_FLOWNODE_ID: info.targetId.replace('NODE_', ''),
                HANDLE: '0',
                STATUS: info.connection.id,
                REMARK: ''
            }
            jsonObj[jsonObj.length] = t;
        }
        $("#FlowListStr").val(JSON.stringify(jsonObj));
    });
    instance.bind("connectionDetached", function (connection, originalEvent) {
        now_C = connection;
        var jsonObj = JSON.parse($("#FlowListStr").val());
        var tmp = [];
        for (var i = 0; i < jsonObj.length; i++) {
            if ("NODE_" + jsonObj[i].FROM_FLOWNODE_ID == connection.sourceId && "NODE_" + jsonObj[i].TO_FLOWNODE_ID == connection.targetId) {

            }
            else {
                tmp[tmp.length] = jsonObj[i];
            }
        }
        $("#FlowListStr").val(JSON.stringify(tmp));
        $('#editNodeFolw').dialog("close")

    });

    instance.bind("contextmenu", function (component, originalEvent) {
        //alert("context menu on component " + component.id);
        originalEvent.preventDefault();
        return false;
    });

    instance.doWhileSuspended(function () {

        instance.makeSource($("#page_man .w"), {
            filter: ".ep",				// only supported by jquery
            anchor: "Continuous",
            connector: ["StateMachine", { curviness: 20 }],
            connectorStyle: { strokeStyle: "#5c96bc", lineWidth: 2, outlineColor: "transparent", outlineWidth: 4 },
            maxConnections: 5,
            onMaxConnections: function (info, e) {
                alert("Maximum connections (" + info.maxConnections + ") reached");
            }
        });

        instance.makeTarget($("#page_man .w"), {
            dropOptions: { hoverClass: "dragHover" },
            anchor: "Continuous"
        });




    });
}


function onSubmit() {
    var t = instance.getConnections();
    var allChild = $("#page_man").children(".w")
    var x_y = [];
    for (var i = 0; i < allChild.length; i++)
    {
        var doc = $(allChild[i])
        var t = {
            Id: doc.attr('id').replace('NODE_',''),
            Y: parseInt(doc.position().top),
            X: parseInt(doc.position().left)
        }
        x_y[x_y.length] = t;
    }

    var jsonObjStr = JSON.stringify(x_y)
    $("#X_Y").val(jsonObjStr)
    var com = $(".combo-text")
    if (com.attr('name') == null) {
        com.attr('name', 'combo_text')
    }

    var obj = $('input[type="submit"]');
    obj[0].click()
}

function AddFolw(nodeID, text, left, top)
{

    if ($("#NODE_" + nodeID).html() == null) {
        if (left > 0 && top > 0) {
            var str = '<div class="w" id="NODE_' + nodeID + '" style="top:' + top + 'px;left:' + left + 'px">' + text + '<div class="ep"></div>';
            $(str).appendTo($("#page_man"));
            return true;
        }
    }
    return false;
}

function SaveNode()
{
    if (isNaN($("#EXPIRE_HOUR").val())) {
        alert("处理时长必须是有效的数字");
        return;
    }
    var jsonObj = JSON.parse($("#FlowListStr").val());
    for (var i = 0; i < jsonObj.length; i++) {
        if (jsonObj[i].FROM_FLOWNODE_ID == $("#FROM_FLOWNODE_ID").val() && jsonObj[i].TO_FLOWNODE_ID == $("#TO_FLOWNODE_ID").val()) {
            jsonObj[i].HANDLE=$("#HANDLE").val();
            jsonObj[i].STATUS=$("#STATUS").val();
            jsonObj[i].REMARK = $("#flowRemark").val();
            jsonObj[i].ASSIGNER = $("#ASSIGNER").val();
            jsonObj[i].AllRoleStr = $("#AllRoleStr").val();
            jsonObj[i].EXPIRE_HOUR = $("#EXPIRE_HOUR").val();
            break;
        }
    }
    $("#FlowListStr").val(JSON.stringify(jsonObj));
    now_C.getOverlays()[1].setLabel(jsonObj[i].STATUS);
    $('#editNodeFolw').dialog('close')
}

function DelNode() {
    if (confirm('确认', '您确信要删除本记录?')) {
        instance.detach(now_C);
        return;
    }
}


function DeleteAllConnections()
{
    instance.detachAllConnections($(nowMenuObj).attr("id"));
}
function DeleteObj() {
    DeleteAllConnections();
    $(nowMenuObj).remove();
}


function BoxListSetV(name,obj) {
    var f_str = "";
    if (obj != null) {
        var nowStr = "," + $("#" + name).val() + ","
        if ($("#" + name).val() == '') nowStr = ",";
        var thisValue = "," + $(obj).attr("value") + ",";
        if ($(obj).attr("checked")==null) {
            nowStr = nowStr.replace(","+$(obj).attr("value") + ",", ",");
        }
        else {
            if (nowStr.indexOf(thisValue) == -1) {
                nowStr = nowStr + $(obj).attr("value") + ",";
            }
        }
        f_str = nowStr.substr(1, nowStr.length - 2);
        $("#" + name).val(f_str);
    }
    else {
        //$("input[name='" + name + "_JSITEM']").each(function () {
        //    if ($(this).attr("checked") == 'checked') {
        //        f_str += "," + $(this).attr("value");
        //    }
        //})
        //if (f_str.length > 0) f_str = f_str.substr(1);
        //$("#" + name).val(f_str);
    }
}

function BoxListDisplayV(name) {
    var nowStr = "," + $("#" + name).val() + ","
    $("input[name='" + name + "_JSITEM']").each(function () {
        $(this).removeAttr("checked");

        var tmp = "," + $(this).val() + ",";
        //alert(nowStr.indexOf(tmp) + "\r\n" + nowStr + "\r\n" + tmp);
        if (nowStr.indexOf(tmp) > -1) {
            $(this).attr("checked", "true");
        }
    })

}

