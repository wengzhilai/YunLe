$(function () {
    doSearch();
    //alert($("#column2D44").offset().top);
    //alert($("#column2D44").offset().left);
    //$("#column2D44").css("height", $(window).height() - $("#column2D44").offset().top+"px");
})
var isStop = false;
function doSearch() {
    isStop = false
    var whereStr = GetWhereStr()
    if (isStop) return;
    $("#GetWhereStr").val(whereStr);

    $.ajax({
        url: "QueryList?t=" + Math.random(),
        dateType:"Json",
        data: { 
            chartsType: $("#CHARTS_TYPE").val(),
            AllParaStr: $("#AllPara").val(),
            Code: $("#CODE").val(),
            WhereStr: $("#GetWhereStr").val()
        },
        success: function (data) {
            data.chart = JSON.parse($("#CHARTS_CFG").val());
            //var strJSON = {
            //    "chart": JSON.parse($("#CHARTS_CFG").val()),
            //    "data":JSON.parse(data)
            //};
            var column2D4 = new FusionCharts({
                "swfUrl": "../FusionChart/" + $("#CHARTS_TYPE").val() + "/" + $("#showType").val() + ".swf",
                "height": $(window).height() - $("#column2D44").offset().top-10,
                "width": $(window).width() - $("#column2D44").offset().left-10
            });
            column2D4.setJSONData(data);
            column2D4.setTransparent(true);
            column2D4.render("column2D44");
        },
        error: function (data) {
            alert(data.responseText);
        }
    });

    }

function doDown() {
    var code = $("#CODE").val();
    var WhereStr = $("#GetWhereStr").val();
    var AllParaStr = $("#AllPara").val();
    if (WhereStr == null) WhereStr = "";
    window.location =bootPATH+ 'Query/DownFile?code=' + code + '&WhereStr=' + WhereStr + '&AllParaStr=' + AllParaStr + ''
}
function changeType()
{
    $("#showType").val($("#allType").val());
    doSearch();
}

function GetWhereStr() {
    var str = $("#searchField").val();
    var jsonObj = JSON.parse(str);
    var reStr = new Array();
    var reJson = [];
    for (var i = 0; i < jsonObj.length; i++) {
        var v = $("#s_" + jsonObj[i].FieldName + "_value").val();
        switch (jsonObj[i].FieldType) {
            case "System.DateTime":
                v = $("#s_" + jsonObj[i].FieldName + "_value").datebox('getValue');
                break;
        }

        if (v.indexOf("'") > -1) {
            alert('不能包含非法字符');
            isStop = true;
            return '';
        }
        var tmp_v = v;
        if (v != "") {
            reJson.push(
                {
                    ObjFiled: jsonObj[i].FieldName,
                    OpType: $("#s_" + jsonObj[i].FieldName + "_type").val(),
                    Value: tmp_v,
                    FieldType: jsonObj[i].FieldType
                })

            if (jsonObj[i].FieldType == 'System.String' || jsonObj[i].FieldType == 'System.DateTime') {
                tmp_v = "'" + v + "'";
            }
            if ($("#s_" + jsonObj[i].FieldName + "_type").val() == 'like') {
                tmp_v = "'%" + v + "%'";
            }
            reStr[reStr.length] = jsonObj[i].FieldName + " " + $("#s_" + jsonObj[i].FieldName + "_type").val() + " " + tmp_v
        }
    }
    return JSON.stringify(reJson)
    //return reStr.join(" and ");
}

//搜索按钮事件
function IsOpen(doc) {
    $('#SearchTable').toggle();
    var htm = $(doc).html();
    var cls = $(doc).attr("data-options");
    //var searchHit = $("#SearchTable").height();
    var sumht = $(this).height();
    //$("#dg").datagrid({ height: (sumht) });
    if (htm.indexOf("icon-i-open") > 0) {
        $(doc).html(htm.replace("icon-i-open", "icon-i-contraction"));
        $(doc).attr("data-options", cls.replace("icon-i-open", "icon-i-contraction"));
    }
    else {
        $(doc).html(htm.replace("icon-i-contraction", "icon-i-open"));
        $(doc).attr("data-options", cls.replace("icon-i-contraction", "icon-i-open"));
    }
}

function ShowDebug() {
    $.ajax({
        url: "GetDubug?t=" + Math.random(),
        data: {
            code: $("#CODE").val()
        },
        success: function (data) {
            $.messager.show({
                title: "查询语句：" + $("#CODE").val(),
                msg: "<textarea style='width: 100%;height:100%;border-width: 1;'>" + data + "</textarea>",
                showType: 'slide',
                width: 400,
                height: 200
            });
        },
        error: function (data) {
            alert('失败:' + data.responseText);
        }
    });
}



