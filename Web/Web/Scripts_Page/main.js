$(function () {
    SetChrildMenuHeight();
    SetMainContentHeight();

});
//窗体大小改变时重置内容高度
$(window).resize(function () { SetMainContentHeight(); });
//设置导航子级导航高度
function SetChrildMenuHeight() {
    $("#mainbody").children(".menu").children(".menu-item").css("height", "26px").css("padding", "3px 1px");
}
//设置左导航隐藏展开
function SetContentLeftShowHide(index) {
    if (index == 1) {
        $("#content-center").animate({ 'width': 0, 'border-right-width': 0 }, 300, null, function () {
            $("#content-left").show();
            RestTabSize();
        });
    }
    else {
        $("#content-center").show();
        $("#content-left").hide();
        $("#content-center").animate({ 'width': 180, 'border-right-width': 5 }, 300, null, function () {
            RestTabSize();
        });
    }

}
//重置内容高度
function SetMainContentHeight() {
    var tab = $('#mytab').tabs('getSelected');
    var tabindex = $('#mytab').tabs('getTabIndex', tab);
    //body高度
    var win_h = $(window).height();
    //顶部高度
    var header_h = $("#main-header").height();
    //低部高度
    var footer_h = $("#main-footer").height() + 5;
    //内容上内边距+下边距
    var content_p = 5 + 2;
    //新高度
    var new_h = win_h - header_h - footer_h - content_p;
    $("#content-left").height(new_h);
    $("#content-center").height(new_h);
    $("#content-right").height(new_h);
    $("#left-menu").height(new_h);
    //内容宽度
    //var content_w = $("#content-right").width();
    $("#mytab").tabs({
        height: new_h,
    });
    RestTabSize();


    $('#mytab').tabs('select', tabindex);
}

//重置tab大小
function RestTabSize() {
    $("#mytab").tabs('resize');
}



//主页顶部区域显示隐藏
function SetTopShowHide(obj) {
    var state = $("#main-header-top").css("display");
    if (state == "block") {
        $("#main-header-top").slideUp(300, function () {
            $(obj).children("span").attr("class", "glyphicon glyphicon-chevron-down");
            SetMainContentHeight();
        });
    }
    else {
        $("#main-header-top").slideDown(300, function () {
            $(obj).children("span").attr("class", "glyphicon glyphicon-chevron-up");
            SetMainContentHeight();
        });
    }

}


//打开页面
function GetTab(url, name) {
    if (url != null && url.length > 0) {
        url = url.replace("~/", bootPATH);
        url = bootPATH + "Home/Iframe?url=" + url;

        if ($('#mytab').tabs('exists', name)) {
            $('#mytab').tabs('select', name);
        } else {
            $('#mytab').tabs('add', {
                width: '100%',
                title: name,
                href: url,
                closable: true,
                extractor: function (data) {
                    return data;
                }
            });
            /*双击关闭TAB选项卡*/
            $(".tabs-inner").dblclick(function () {
                var subtitle = $(this).children(".tabs-closable").text();
                $('#tt').tabs('close', subtitle);
            })
            $(".tabs-inner").each(function (i, n) {
                if (i != 0) {
                    $(n).bind('contextmenu', function (e) {
                        $('#mm').menu('show', {
                            left: e.pageX,
                            top: e.pageY
                        });
                        var subtitle = $(this).children(".tabs-closable").text();
                        $('#mm').data("currtab", subtitle);
                        $('#mytab').tabs('select', subtitle);
                        return false;
                    });
                }
            })
        }
    }
}

//打开菜单页面并且同步菜单
function OpenTab(url, name, id) {
    GetTab(url, name);
}

