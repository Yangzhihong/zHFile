
var $file = $('#file');
$(function () {
    loadFileList("");
    var maxfilesize = '2048';

    $file.fileinput({
        uploadUrl: '/Resource/Upload',
        uploadAsync: true,
        overwriteInitial: true,
        maxFileSize: maxfilesize,
        showClose: false,
        showCaption: true,
        showUpload: false,
        dropZoneEnabled: false,
        uploadExtraData: function () {
            return {
                'currentPath': fileVue.curPath
            }
        },
        browseLabel: '',
        removeLabel: '',
        browseIcon: '<i class="glyphicon glyphicon-folder-open"></i>',
        removeIcon: '<i class="glyphicon glyphicon-remove"></i>',
        removeTitle: 'Cancel or reset changes',
        layoutTemplates: {
            main2: '{preview} ' + ' {remove} {browse}',
            actionUpload: '',
            actionDelete: ''
        },
        allowedFileExtensions: ["jpg", "png", "gif", "txt", "doc", 'docx', "xlsx", 'xls', 'ppt', 'pptx', 'csv', 'pdf', 'zip', 'rar'],
        msgSizeTooLarge: 'The limit size of picture file is ' + maxfilesize + ' KB, please resize and upload again.'
    });

    $file.on("fileuploaded", function (event, data, previewId) {
        $file.fileinput('clear').fileinput('refresh').fileinput('enable');
        $("#inputModal").modal("hide");
        fileVue.showLoading = true;
        loadFileList(fileVue.curPath);
    });

    $('#AddfolderModal').on('shown.bs.modal', function (e) {
        $('input', this).focus();
    });
})

var fileVue = new Vue({
    el: '#resource',
    data: {
        files: [],
        listView: false,
        curPath: "\\",
        subFolder: [{
            link: '\\',
            name: '',
            ioncls: 'zmdi zmdi-home'
        }],
        showLoading: true
    },
    updated: function () {

        //  $("#resource").contextmenu(function () {
        //  
        //      $("#filecontextmenu").dropdown();
        //      return false;
        //  })

    },
    methods: {
        formatDate: function (str) {
            return new Date(str).format('hh:mm:ss dd/MM/yyyy');
        },
        gettypeicon: function (file) {

            switch (file.extension) {
                case "folder":
                    return "fa fa-folder-o";
                case ".pdf":
                    return "fa fa-file-pdf-o";
                case ".txt":
                    return "fa fa-file-text-o";
                case ".gif":
                case ".png":
                case ".jpg":
                    return "fa fa-file-photo-o";
                case ".xls":
                case ".xlsx":
                    return "fa fa-file-excel-o";
                case ".zip":
                case ".rar":
                    return "fa fa-file-zip-o";
                case ".ppt":
                case ".pptx":
                    return "fa fa-file-powerpoint-o";
                case ".doc":
                case ".docx":
                    return "fa fa-file-word-o";
                case ".xml":
                    return "fa fa-file-code-o";
                case ".mp4":
                case ".mp3":
                default:
                    return "fa fa-file-o";
            }
        },
        cuToDir: function (folder, index) {
            this.showLoading = true;
            this.subFolder.length = index + 1;
            this.curPath = folder.link;
            loadFileList(folder.link);
        },
        toDir: function (file) {
            switch (file.extension) {
                case "folder":
                    this.showLoading = true;
                    if (file.path) {
                        this.curPath = file.path;

                        this.subFolder.push({
                            link: file.path,
                            name: file.path.substring(file.path.lastIndexOf("\\") + 1, file.path.length),
                            ioncls: 'zmdi zmdi-folder'
                        });
                    }
                    loadFileList(file.path);
                    break;
                default:
                    window.open(file.fullpath);
                    break;
            }
        },
        formatkeys: function (str) {
            if (this.curPath.indexOf("searchresult:") > -1) {
                var reg = new RegExp(this.curPath.replace("searchresult:", ""), "i");
                var res = str.match(reg);
                if (res && res.length > 0) {
                    return str.replace(res[0], "<b class='c'>" + res[0] + "</b>");
                }
            }
            return str;
        },
        searchfile: function () {
            $result = $("#searchInput").val();
            if ($result) {
                this.curPath = "searchresult:" + $result;
                this.subFolder.length = 1;

                this.subFolder.push({
                    link: '\\',
                    name: $result,
                    ioncls: 'zmdi zmdi-search'
                });

                loadFileList(this.curPath);
            }
        }
    }
});

function loadFileList(path) {
    $.get('/Resource/GetFiles', {
        path
    }, function (data) {
        if (data && data.length > 0) {
            fileVue.files = data;
        } else {
            fileVue.files = [];
        }
        Vue.nextTick(function () {

            console.log("Vue updated" + new Date().getTime());
            if ($.contextMenu) {
                $.contextMenu({
                    selector: '.res-item,.fileitem,#resource',
                    callback: function (key, options) {
                    },
                    items: {
                        "refresh": {
                            name: "Refresh", icon: "fa-refresh", callback: function (itemkey, opt, e) {
                                fileVue.showLoading = true;
                                loadFileList(fileVue.curPath);
                            }
                        },
                        "upload": {
                            name: "UPLOAD FILES", icon: "fa-cloud-upload",
                            disabled: function (key, opt) {
                                if (fileVue.curPath.indexOf("searchresult:") > -1) {
                                    return true;
                                }
                            },
                            callback: function (itemkey, opt, e) {
                                $("#currentPath").val(fileVue.curPath);
                                $("#inputModal").modal("show");
                            }
                        },
                        "create": {
                            name: "CREATE FOLDER",
                            icon: "fa-plus-square",
                            disabled: function (key, opt) {
                                if (fileVue.curPath.indexOf("searchresult:") > -1) {
                                    return true;
                                }
                            },
                            callback: function (itemKey, opt, e) {
                                $('#AddfolderModal').modal("show");
                                $("#folder-name").val("");
                            }
                        },
                        "rename": {
                            name: "RENAME",
                            icon: "fa-edit",
                            disabled: function (key, opt) {
                                if (opt.$trigger[0].nodeName == "DIV") {
                                    return true;
                                }
                                if (fileVue.curPath.indexOf("searchresult:") > -1) {
                                    return true;
                                }
                            },
                            callback: function (itemKey, opt, e) {
                                $("#RenameModal").modal("show");

                                var filename = $(this).attr("filename");

                                if (filename.lastIndexOf(".") > -1) {
                                    filename = filename.substring(0, filename.lastIndexOf("."));
                                }

                                $("#New-name").attr("filename", $(this).attr("filename")).val(filename);


                                $("#New-name").focus().select();
                            }
                        },
                        "delete": {
                            name: "DELETE",
                            icon: "fa-remove",
                            disabled: function (key, opt) {
                                if (opt.$trigger[0].nodeName == "DIV") {
                                    return true;
                                }
                            },
                            callback: function (itemKey, opt, e) {

                                var fileinfo = fileVue.files.find(o => o.filename == $(this).attr("filename"))
                                if (fileinfo) {
                                    if (fileinfo.extension == "folder") {
                                        Delete(fileinfo.path + "*0");
                                    }
                                    else {
                                        Delete(fileinfo.path + "*1");
                                    }
                                }
                            }
                        }
                    }
                });
            }


        })

        fileVue.showLoading = false;
    });
}

function AddNewFolder() {
    $('#AddfolderModal').modal("hide");
    fileVue.showLoading = true;
    if ($("#folder-name").val() == "") {
        showErrorTips("You must enter the folder name!");
        return;
    }

    $.post('/Resource/CreateFolder', {
        path: fileVue.curPath + "\\" + $("#folder-name").val()
    }, function (data) {
        if (data && data.result === "ok") {
            showTips("Create folder OK!");
        }
        else {
            showErrorTips("Create folder Failed!");
        }

        loadFileList(fileVue.curPath);
    });
}

function ReName() {
    fileVue.showLoading = true;
    if ($("#New-name").val() == "") {
        showErrorTips("You must enter the new name!");
        return;
    }

    var fileinfo = fileVue.files.find(o => o.filename == $("#New-name").attr("filename"))

    $.post('/Resource/Rename', {
        path: fileVue.curPath,
        oldFileName: fileinfo.filename,
        newFileName: $("#New-name").val(),
        type: fileinfo.extension == "folder" ? "0" : "1"
    }, function (data) {
        if (data && data.result === "ok") {
            showTips("Operaion complete!");
        }
        else {
            showErrorTips("Operaion Failed!");
        }

        loadFileList(fileVue.curPath);
        $('#RenameModal').modal("hide");
    });
}


function Delete(path) {
    fileVue.showLoading = true;
    $.post('/Resource/Delete', {
        file: path
    }, function (data) {
        if (data && data.result === "ok") {
            showTips("Deleted successfully!");
        }
        else {
            showErrorTips("Delete Failed!");
        }
        loadFileList(fileVue.curPath);
    });

}


function uploadfile() {
    console.log($('#file'));
    $('#file').fileinput("upload");
}

$file.on('fileuploaded', function () {
    $(this).fileinput('enable');
})