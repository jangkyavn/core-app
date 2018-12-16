'use strict';

var core = {
    configs: {
        pageSize: 10,
        pageIndex: 1
    },
    notify: function (message, type) {
        toastr.options = {
            'positionClass': 'toast-top-center',
            'timeOut': 1500
        };

        toastr.remove();

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
    confirm: function (message, callBack) {
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
    getPeriod: function (input) {
        var start = moment(input);
        var end = moment(new Date());

        var duration = moment.duration(end.diff(start));
        var diffYear = parseInt(duration.years());
        var diffMonth = parseInt(duration.weeks());
        var diffWeek = parseInt(duration.weeks());
        var diffDay = parseInt(duration.days());
        var diffHour = parseInt(duration.hours());
        var diffMinute = parseInt(duration.minutes());
        var diffSecond = parseInt(duration.seconds());

        var period = '';
        if (diffYear > 0) {
            period = diffYear + ' năm trước';
        } else if (diffMonth > 0 && diffYear === 0) {
            period = diffMonth + ' tháng trước';
        } else if (diffWeek > 0 && diffMonth === 0 && diffYear === 0) {
            period = diffWeek + ' tuần trước';
        } else if (diffDay > 0 && diffWeek === 0 && diffMonth === 0 && diffYear === 0) {
            period = diffDay + ' ngày trước';
        } else if (diffHour > 0 && diffDay === 0 && diffWeek === 0 && diffMonth === 0 && diffYear === 0) {
            period = diffHour + ' giờ trước';
        } else if (diffMinute > 0 && diffHour === 0 && diffDay === 0 && diffWeek === 0 && diffMonth === 0 && diffYear === 0) {
            period = diffMinute + ' phút trước';
        } else if (diffSecond > 0 && diffMinute === 0 && diffHour === 0 && diffDay === 0 && diffWeek === 0 && diffMonth === 0 && diffYear === 0) {
            period = "Vài giây trước";
        } else {
            period = 'Vừa xong';
        }

        return period;
    },
    dateFormatJson: function (datetime) {
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
    dateTimeFormatJson: function (datetime) {
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
    getStatus: function (status) {
        if (status === 1)
            return '<span class="badge badge-success">Kích hoạt</span>';
        else
            return '<span class="badge badge-danger">Khoá</span>';
    },
    getImage: function (type, image) {
        if (image !== null && image !== '') {
            return `<img width="60" src="/uploaded/images/${type}/${image}" />`;
        } else {
            return `<img width="60" src="/uploaded/images/${type === 'avatars' ? 'no-avatar.png' : 'no-image.png'}" />`;
        }
    },
    formatNumber: function (number, precision) {
        if (number === null || number === '') {
            return '';
        }

        if (!isFinite(number)) {
            return number.toString();
        }

        var a = number.toFixed(precision).split('.');
        a[0] = a[0].replace(/\d(?=(\d{3})+$)/g, '$&,');
        return a.join('.');
    },
    makeSeoAlias: function (input) {
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
    callAjax: function (url, type, data, dataType, okCallback) {
        $.ajax({
            url: url,
            type: type,
            data: data,
            dataType: dataType,
            success: function (res) {
                okCallback(res);
            },
            error: function (request, error) {
                console.log(request);
                console.log(error);
                core.notify(message.has_error, 'error');
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
            error: function (request, error) {
                console.log(request);
                console.log(error);
                core.notify(message.has_error, 'error');
            }
        });
    },
    unflattern: function (arr) {
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

$(document).ajaxSend(function (e, xhr, options) {
    if (options.type.toUpperCase() === "POST" || options.type.toUpperCase() === "PUT") {
        var token = $('form').find("input[name='__RequestVerificationToken']").val();
        xhr.setRequestHeader("RequestVerificationToken", token);
    }
});
'use strict';

var message = {
    change_success: 'Thay đổi thành công',
    update_success: 'Cập nhật thành công',
    add_success: 'Thêm mới thành công',
    edit_success: 'Sửa thành công',
    delete_success: 'Xóa thành công',
    has_error: 'Có lỗi xảy ra',
    confirm_delete: 'Bạn có chắc chắn muốn xóa không?',
    confirm_delete_multiple: 'Bạn có chắc chắn muốn xóa các bản ghi đã chọn không?',
    import_file_success: 'Nhập file excel thành công'
};