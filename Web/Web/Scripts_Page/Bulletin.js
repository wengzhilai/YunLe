$(function () {

    KindEditor.create('textarea[name="CONTENT"]', {
        cssPath: '../Scripts/kindeditor/plugins/code/prettify.css',
        uploadJson: '../Scripts/kindeditor/asp.net/upload_json.ashx',
        fileManagerJson: '../Scripts/kindeditor/asp.net/file_manager_json.ashx',
        allowFileManager: true,
        resizeType: 1,
        allowPreviewEmoticons: false,
        allowImageUpload: true,
        items: [
            'fontname', 'fontsize', '|', 'forecolor', 'hilitecolor', 'bold', 'italic', 'underline',
            'removeformat', '|', 'justifyleft', 'justifycenter', 'justifyright', 'insertorderedlist',
            'insertunorderedlist', '|', 'emoticons', 'image', 'link']
    });


    var strJson = $("#AllFilesStr").val();
    if (strJson.length > 5) {
        $('#dg_AllFiles').datagrid('loadData', JSON.parse(strJson));
    }

    $("#uploadify").uploadify({
        swf: bootPATH + '/Scripts/uploadify/uploadify.swf',
        uploader: bootPATH + '/Scripts/uploadify/UploadHandler.ashx',
        cancelImg: bootPATH + '/Scripts/uploadify/cancel.png',
        buttonText: "　",
        fileTypeExts: "*.doc;*.docx;*.xls;*.xlsx;*.ppt;*.htm;*.html;*.txt;*.zip;*.rar;*.gz;*.bz2;*.apk;*.app",
      
        formData: {
            'folder': '~/UpFiles/Bulletin/',
            'customname': Date.now().toString()
        },
        onUploadSuccess: function (a, b, c, d, e) {
            var fileName = a.name;
            var bArr = b.split(',');
            if (bArr[0] == "0") //上传文件出错
            {
                alert(bArr[1]);
                return;
            }
            if (bArr.length > 1) {
                fileName = bArr[1];
            }
            $.ajax({
                url: "AddFile?t=" + Math.random(),
                dateType: "JSON",
                data: {
                    name: a.name,
                    size: a.size,
                    savePath: '~/UpFiles/Bulletin/' + fileName
                },
                success: function (data) {
                    $('#dg_AllFiles').datagrid('appendRow', JSON.parse(data));
                    var data = $('#dg_AllFiles').datagrid('getData');
                    var strJson = JSON.stringify(data.rows);
                    $("#AllFilesStr").val(strJson)
                },
                error: function (data) {
                    alert('失败:' + data.responseText);
                }
            });
        }
    });


});


function ChangePic() {
    OnBegin();
    $.ajaxFileUpload
    (
        {
            url: '../Fun/FileUp', //用于文件上传的服务器端请求地址
            type: 'post',
            data: { nameType: '0', fileType: 'image', dirPath: "~/UpFiles/Bulletin/" },//nameType(命名方式):0表示随机文件名，1表示使用原文件名;fileType(文件类型):image、flash、media、file
            secureuri: false, //一般设置为false
            fileElementId: '_FilePic', //文件上传空间的id属性  <input type="file" id="file" name="file" />
            dataType: 'json', //返回值类型 一般设置为json
            success: function (data, status)  //服务器成功响应处理函数
            {
                OnComplete();
                if (data.Message == null) data.Message = "";
                data.Message = data.Message.replace('\r\n', '<br />')
                data.Message = data.Message.replace('\\r\\n', '<br />')
                if (data.IsError != null) {
                    if (data.IsError) {
                        divalert("失败", data.Message);
                        return;
                    }
                    else {
                        $("#PIC").val(data.Message)
                        //divalert("成功", data.Message);
                    }
                }
                else {
                    divalert(data);
                }
            },
            error: function (data, status, e)//服务器响应失败处理函数
            {
                OnComplete();
                alert(data);
            }
        }
    )
    return false;
}


function ChangeAllFiles() {
    OnBegin();
    $.ajaxFileUpload
    (
        {
            url: '../Fun/FileUp', //用于文件上传的服务器端请求地址
            type: 'post',
            data: { nameType: '0', fileType: '*', dirPath: "~/UpFiles/Bulletin/" },//nameType(命名方式):0表示随机文件名，1表示使用原文件名;fileType(文件类型):image、flash、media、file
            secureuri: false, //一般设置为false
            fileElementId: '_Up_AllFiles', //文件上传空间的id属性  <input type="file" id="file" name="file" />
            dataType: 'json', //返回值类型 一般设置为json
            success: function (data, status)  //服务器成功响应处理函数
            {
                OnComplete();
                if (data.Message == null) data.Message = "";
                data.Message = data.Message.replace('\r\n', '<br />')
                data.Message = data.Message.replace('\\r\\n', '<br />')
                if (data.IsError != null) {
                    if (data.IsError) {
                        divalert("失败", data.Message);
                        return;
                    }
                    else {

                        $.ajax({
                            url: "AddFile?t=" + Math.random(),
                            dateType: "JSON",
                            data: {
                                name: data.Params.split('|')[1],
                                size: data.Params.split('|')[0],
                                savePath: data.Message
                            },
                            success: function (data) {
                                $('#dg_AllFiles').datagrid('appendRow', JSON.parse(data));
                                var data = $('#dg_AllFiles').datagrid('getData');
                                var strJson = JSON.stringify(data.rows);
                                $("#AllFilesStr").val(strJson)
                            },
                            error: function (data) {
                                alert('失败:' + data.responseText);
                            }
                        });



                        //divalert("成功", data.Message);
                    }
                }
                else {
                    divalert(data);
                }
            },
            error: function (data, status, e)//服务器响应失败处理函数
            {
                OnComplete();
                alert(data);
            }
        }
    )
    return false;
}


function ShowRowBtn(value, row, index) {
    var tmp = '<a  class="easyui-linkbutton l-btn l-btn-small" href="#" ';
    tmp += ' onclick="DelFile(' + row.ID + ',' + index + ')">';
    tmp += ' <span class="l-btn-left"><span class="l-btn-text">删除</span></span></a>  '
    return tmp;
}
function DelFile(id, index) {
    if (window.confirm("确定要删除该文件吗？")) {
        $.ajax({
            url: "DelFile?t=" + Math.random(),
            data: {
                fileID: id
            },
            success: function (data) {
                $('#dg_AllFiles').datagrid('deleteRow', index);
                var data = $('#dg_AllFiles').datagrid('getData');
                var strJson = JSON.stringify(data.rows);
                $("#AllFilesStr").val(strJson)
            },
            error: function (data) {
                alert('失败');
            }
        });
    }
}