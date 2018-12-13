'use strict';

var core = {
    configs: {
        pageSize: 10,
        pageIndex: 1,
        domain: 'https://localhost:44328/'
    },
    notify: function (message, type) {
        toastr.options = {
            'positionClass': 'toast-top-center',
            'timeOut': 1500
        };

        if (type === 'success') {
            toastr.success(message);
        } else if (type === 'warning') {
            toastr.warning(message);
        } else if (type === 'error') {
            toastr.error(message);
        } else {
            toastr.info(message);
        }
    },
    confirm: function(message, callBack) {
        swal({
            title: "Thông báo",
            text: message,
            icon: "warning",
            buttons: {
                cancel: {
                    text: "Không",
                    visible: true,
                    closeModal: true
                },
                confirm: {
                    text: "Có",
                    visible: true,
                    closeModal: false
                }
            }
        }).then((isConfirm) => {
            if (isConfirm) {
                callBack();
            }

            swal.close();
        });
    },
    dateFormatJson: function(datetime) {
        if (datetime === null || datetime === '')
            return '';
        var newdate = new Date(datetime);
        var month = newdate.getMonth() + 1;
        var day = newdate.getDate();
        var year = newdate.getFullYear();
        if (month < 10)
            month = "0" + month;
        if (day < 10)
            day = "0" + day;
        return day + "/" + month + "/" + year;
    },
    dateFormatJson2: function (datetime) {
        if (datetime === null || datetime === '')
            return '';
        var newdate = new Date(datetime);
        var month = newdate.getMonth() + 1;
        var day = newdate.getDate();
        var year = newdate.getFullYear();
        if (month < 10)
            month = "0" + month;
        if (day < 10)
            day = "0" + day;
        return year + "-" + month + "-" + day;
    },
    dateTimeFormatJson: function(datetime) {
        if (datetime === null || datetime === '')
            return '';
        var newdate = new Date(parseInt(datetime.substr(6)));
        var month = newdate.getMonth() + 1;
        var day = newdate.getDate();
        var year = newdate.getFullYear();
        var hh = newdate.getHours();
        var mm = newdate.getMinutes();
        var ss = newdate.getSeconds();
        if (month < 10)
            month = "0" + month;
        if (day < 10)
            day = "0" + day;
        if (hh < 10)
            hh = "0" + hh;
        if (mm < 10)
            mm = "0" + mm;
        if (ss < 10)
            ss = "0" + ss;
        return day + "/" + month + "/" + year + " " + hh + ":" + mm + ":" + ss;
    },
    startLoading: function() {
        if ($('.dv-loading').length > 0)
            $('.dv-loading').removeClass('hide');
    },
    stopLoading: function() {
        if ($('.dv-loading').length > 0)
            $('.dv-loading')
                .addClass('hide');
    },
    getStatus: function(status) {
        if (status === 1)
            return '<span class="badge badge-success">Kích hoạt</span>';
        else
            return '<span class="badge badge-danger">Khoá</span>';
    },
    getImage: function(type, image) {
        if (image !== null && image !== '') {
            return `<img width="60" src="/uploaded/images/${type}/${image}" />`;
        } else {
            return `<img width="60" src="/uploaded/images/${type === 'avatars' ? 'no-avatar.png' : 'no-image.png'}" />`;
        }
    },
    formatNumber: function(number, precision) {
        if (!isFinite(number)) {
            return number.toString();
        }

        var a = number.toFixed(precision).split('.');
        a[0] = a[0].replace(/\d(?=(\d{3})+$)/g, '$&,');
        return a.join('.');
    },
    makeSeoAlias: function(input) {
        if (input === undefined || input === '')
            return '';
        //Đổi chữ hoa thành chữ thường
        var slug = input.toLowerCase();

        //Đổi ký tự có dấu thành không dấu
        slug = slug.replace(/á|à|ả|ạ|ã|ă|ắ|ằ|ẳ|ẵ|ặ|â|ấ|ầ|ẩ|ẫ|ậ/gi, 'a');
        slug = slug.replace(/é|è|ẻ|ẽ|ẹ|ê|ế|ề|ể|ễ|ệ/gi, 'e');
        slug = slug.replace(/i|í|ì|ỉ|ĩ|ị/gi, 'i');
        slug = slug.replace(/ó|ò|ỏ|õ|ọ|ô|ố|ồ|ổ|ỗ|ộ|ơ|ớ|ờ|ở|ỡ|ợ/gi, 'o');
        slug = slug.replace(/ú|ù|ủ|ũ|ụ|ư|ứ|ừ|ử|ữ|ự/gi, 'u');
        slug = slug.replace(/ý|ỳ|ỷ|ỹ|ỵ/gi, 'y');
        slug = slug.replace(/đ/gi, 'd');
        //Xóa các ký tự đặt biệt
        slug = slug.replace(/\`|\~|\!|\@|\#|\||\$|\%|\^|\&|\*|\(|\)|\+|\=|\,|\.|\/|\?|\>|\<|\'|\"|\:|\;|_/gi, '');
        //Đổi khoảng trắng thành ký tự gạch ngang
        slug = slug.replace(/ /gi, "-");
        //Đổi nhiều ký tự gạch ngang liên tiếp thành 1 ký tự gạch ngang
        //Phòng trường hợp người nhập vào quá nhiều ký tự trắng
        slug = slug.replace(/\-\-\-\-\-/gi, '-');
        slug = slug.replace(/\-\-\-\-/gi, '-');
        slug = slug.replace(/\-\-\-/gi, '-');
        slug = slug.replace(/\-\-/gi, '-');
        //Xóa các ký tự gạch ngang ở đầu và cuối
        slug = '@' + slug + '@';
        slug = slug.replace(/\@\-|\-\@|\@/gi, '');

        return slug;
    },
    callAjax: function(url, type, data, dataType, okCallback) {
        $.ajax({
            url: url,
            type: type,
            data: data,
            dataType: dataType,
            success: function(res) {
                okCallback(res);
            },
            error: function (request, error) {
                console.log(request);
                console.log(error);
                core.notify(resources.has_error, 'error');
            }
        });
    },
    callAjaxFile: function (url, type, data, okCallback) {
        $.ajax({
            url: url,
            type: type,
            data: data,
            contentType: false,
            processData: false,
            success: function (res) {
                okCallback(res);
            },
            error: function (res) {
                console.log(res);
                core.notify(resources.has_error, 'error');
            }
        });
    },
    unflattern: function(arr) {
        var map = {};
        var roots = [];
        for (var i = 0; i < arr.length; i += 1) {
            var node = arr[i];
            node.children = [];
            map[node.id] = i; // use map to look-up the parents
            if (node.parentId !== null) {
                arr[map[node.parentId]].children.push(node);
            } else {
                roots.push(node);
            }
        }
        return roots;
    }
};

$(document).ajaxSend(function(e, xhr, options) {
    if (options.type.toUpperCase() === "POST" || options.type.toUpperCase() === "PUT") {
        var token = $('form').find("input[name='__RequestVerificationToken']").val();
        xhr.setRequestHeader("RequestVerificationToken", token);
    }
});