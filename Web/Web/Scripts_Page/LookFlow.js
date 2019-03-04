$(function () {

    if ($("#X_Y").html() != null && $("#X_Y").val() != "") {
        var placeJson = JSON.parse($("#X_Y").val());
        for (var i = 0; i < placeJson.length; i++) {
            /*添加节点*/
            AddFolw(placeJson[i].Id, $("#menu_" + placeJson[i].Id).text(), placeJson[i].X, placeJson[i].Y);
        }
    }

    jsPlumb.ready(function () {
        var instance = jsPlumb.getInstance({
            Endpoint: ["Dot", { radius: 2 }],
            HoverPaintStyle: { strokeStyle: "#1e8151", lineWidth: 2 },
            ConnectionOverlays: [
                ["Arrow", {
                    location: 1,
                    id: "arrow",
                    length: 14,
                    foldback: 0.8
                }]
            ],
            Container: "statemachine-demo"
        });
        window.jsp = instance;
        var windows = jsPlumb.getSelector("#statemachine-demo .w");
        //运行拖动
        instance.draggable(windows);
        // suspend drawing and initialise.
        instance.doWhileSuspended(function () {
            instance.makeSource(windows, {
                filter: ".ep",				// only supported by jquery
                anchor: "Continuous",
                connector: ["StateMachine", { curviness: 20 }],
                connectorStyle: { strokeStyle: "#5c96bc", lineWidth: 2, outlineColor: "transparent", outlineWidth: 4 },
                maxConnections: 5,
                onMaxConnections: function (info, e) {
                    alert("Maximum connections (" + info.maxConnections + ") reached");
                }
            });

            instance.makeTarget(windows, {
                dropOptions: { hoverClass: "dragHover" },
                anchor: "Continuous"
            });

            /*连接各点*/
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
    //修改使用过的样式
    for (var i = 0; i < useNode.length; i++)
    {
        var idStr = "#NODE_" + useNode[i].nodeid;
        $(idStr).removeClass("w");
        if (useNode[i].ishandle == 1) {
            $(idStr).addClass("wused");
            var url=useNode[i].url
            if ( url!= null) {
                url = url + "?id=" + useNode[i].flowId
                //$(idStr).attr("onclick", "JSopenWindow('" + url + "','" + useNode[i].name + "')");
                //点击文字弹出子页
                //$(idStr).children("span").attr("onclick", "JSopenWindow('" + url + "','" + useNode[i].name + "')");
            }
        }
        else {
            $(idStr).addClass("wnow");
        }
    }

})
function AddFolw(nodeID, text, left, top) {
    if (left > 0 && top > 0) {
        var str = '<div class="w " id="NODE_' + nodeID + '" style="top:' + top + 'px;left:' + left + 'px"><span>' + text + '</span><div class="ep"></div>';
        $(str).appendTo($("#statemachine-demo"));
    }
}

function JSopenWindow(url, title) {
    url = url.replace("~/", bootPATH);
    $('#openWindowIframe')[0].src = url;

    $('#dialog_FullWin').dialog({
        width: 800,
        height: 350,
        title: title,
        closed: false
    });
}
