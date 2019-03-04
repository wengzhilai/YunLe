$(function () {
    $("#btn_Save").hide();
    $('#filter').keyup(function (e) {
        if (e.keyCode == 13) {
            treeFilter();
        }
    });
});

KindEditor.ready(function(K) {
    var editor = K.editor({
        cssPath: '../Scripts/kindeditor/plugins/code/prettify.css',
        uploadJson: '../Scripts/kindeditor/asp.net/upload_json.ashx',
        fileManagerJson: '../Scripts/kindeditor/asp.net/file_manager_json.ashx',
        allowFileManager : true
    });
    K('#image1').click(function() {
        editor.loadPlugin('image', function() {
            editor.plugin.imageDialog({
                imageUrl : K('#url1').val(),
                clickFn : function(url, title, width, height, border, align) {
                    $('#ICON_URL').val(url);
                    editor.hideDialog();
                }
            });
        });
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

    var obj = $('input[type="submit"]');
    obj[0].click();
}




function delpic(url) {
    $.ajax({
        url: "DelPic?url=" + url,
        success: function (data) {
            alert("已经删除"+data);
        },
        error: function (data) {
            alert('失败');
        }
    });
}

function OnClick(node) {
    var ChannelID = node.id;
    if (ChannelID.indexOf('_') == 0) {
        var iChannelID = ChannelID.substring(1, ChannelID.length);
        var AllChannelId = $("#hideChannelId").val();
        if (AllChannelId.length <= 0) {
            AllChannelId = iChannelID;
            var html = '<label style="width:270px">' + '<input id="' + iChannelID + '" checked="checked" name="AllChannelId" type="checkbox" value="' + iChannelID + '" style="margin:0 0 0 10px; line-height:1em;vertical-align:-3px;border:none;" /> ' + node.text + '</label>';
            $('#ChannelNameList').append(html);
        }
        else {
            if (AllChannelId.indexOf(iChannelID) == -1) {
                AllChannelId += "," + iChannelID;
                var html = '<label style="width:270px">' + '<input id="' + iChannelID + '" checked="checked"  name="AllChannelId" type="checkbox" value="' + iChannelID + '"  style="margin:0 0 0 10px; line-height:1em;vertical-align:-3px;border:none;"/> ' + node.text + '</label>';
                $('#ChannelNameList').append(html);
            }
        }
        $("#hideChannelId").val(AllChannelId);
    }
}


function treeFilter() {
    var $tree = $("#treeChannel");

    var filterVal = $.trim($("#filter").val());
    if (!filterVal) {
        revert();
        return;
    }
    $tree.tree({
        url: '../ChannelInfo/GetAsyn?key=' + filterVal
    });
}


function revert() {
    $('#treeChannel').tree({
        url: '../ChannelInfo/GetAsyn'
    });
}




