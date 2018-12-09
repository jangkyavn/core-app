'use strict';

var userController = function () {
    this.initialize = function () {
        validateForm();
        loadData();
        registerEvents();
    };

    var registerEvents = function () {
        $('#btnAddNew').on('click', function () {
            resetForm();
            $('#wrapPassword').show();
            $('#modalTitle').text('Thêm mới người dùng');
            $('#addEditModal').modal({
                show: true,
                backdrop: 'static'
            });
        });

        $('#btnSelectFile').on('click', function () {
            $('#fUploadFile').click();
        });

        $('#fUploadFile').on('change', function () {
            var fileUpload = $(this).get(0);
            var files = fileUpload.files;
            var data = new FormData();

            for (var i = 0; i < files.length; i++) {
                data.append(files[i].name, files[i]);
            }

            core.callAjaxFile('/Admin/Upload/UploadImage?type=avatars', 'POST', data, function (res) {
                $('#txtAvatar').val(res);
                $('#iAvatar').attr('src', `/uploaded/images/avatars/${res}`);
            });
        });

        $('#btnSave').on('click', function () {
            if ($('#userForm').valid()) {
                var roles = [];
                $.each($('input[name="roles"]'), function (i, item) {
                    if ($(item).prop('checked') === true)
                        roles.push($(item).prop('value'));
                });

                var data = {
                    Id: $('#txtId').val(),
                    DateCreated: $('#txtDateCreated').val(),
                    UserName: $('#txtUserName').val(),
                    Email: $('#txtEmail').val(),
                    FullName: $('#txtFullName').val(),
                    Password: $('#txtPassword').val(),
                    BirthDay: $('#txtBirthDay').val(),
                    PhoneNumber: $('#txtPhoneNumber').val(),
                    Avatar: $('#txtAvatar').val(),
                    Address: $('#txtAddress').val(),
                    Gender: $('#chkGender').prop('checked'),
                    Status: $('#chkStatus').prop('checked') ? 1 : 0,
                    Roles: roles
                };

                console.log(data);

                core.callAjax('/Admin/User/SaveEntity', 'POST', data, 'json', function (res) {
                    if (res) {
                        core.notify(message.add_success, 'success');
                    } else {
                        core.notify(message.edit_success, 'success');
                    }

                    $('#addEditModal').modal('hide');
                    loadData(true);
                });
            }
        });

        $('#btnDeleteMultiple').on('click', function () {
            var listId = [];

            $('.chk-item:checked').each(function () {
                listId.push($(this).data('id'));
            });

            core.confirm(message.confirm_delete_multiple, function () {
                core.callAjax('/Admin/User/DeleteMultiple', 'POST', {
                    jsonId: JSON.stringify(listId)
                }, 'json', function () {
                    core.notify(message.delete_success, 'success');
                    loadData(true);
                });
            });
        });

        $('#btnSearch').on('click', function () {
            loadData(true);
        });

        $('#btnRefresh').on('click', function () {
            $('#txtKeyword').val('');

            loadData(true);
        });

        $('body').on('click', '.btn-edit', function (e) {
            e.preventDefault();
            resetForm();
            $('#modalTitle').text('Sửa người dùng');
            var id = $(this).data('id');
            $('#wrapPassword').hide();
            getById(id, function () {
                $('#addEditModal').modal({
                    show: true,
                    backdrop: 'static'
                });
            });
        });

        $('table').on('click', '.btn-delete', function (e) {
            e.preventDefault();
            var id = $(this).data('id');

            core.confirm(message.confirm_delete, function () {
                core.callAjax('/Admin/User/Delete', 'POST', { id }, 'json', function () {
                    core.notify(message.delete_success, 'success');
                    loadData(true);
                });
            });
        });

        $('#chkAll').change(function () {
            var checkboxes = $(this).closest('table').find(':checkbox');
            checkboxes.prop('checked', $(this).is(':checked'));
            $('#btnDeleteMultiple').prop('disabled', !$(this).is(':checked'));
        });

        $('table').on('change', '.chk-item', function () {
            if ($('.chk-item:checked').length === $('.chk-item').length) {
                $('#chkAll').prop('indeterminate', false);
                $('#chkAll').prop('checked', true);
                $('#btnDeleteMultiple').prop('disabled', false);
            } else if ($('.chk-item:checked').length !== 0) {
                $('#chkAll').prop('indeterminate', true);
                $('#chkAll').prop('checked', false);
                $('#btnDeleteMultiple').prop('disabled', false);
            } else {
                $('#chkAll').prop('indeterminate', false);
                $('#chkAll').prop('checked', false);
                $('#btnDeleteMultiple').prop('disabled', true);
            }
        });

        $('#ddlShowPage').on('change', function () {
            core.configs.pageIndex = 1;
            core.configs.pageSize = parseInt($(this).val());

            loadData(true);
        });
    };

    var validateForm = function () {
        $('#userForm').validate({
            errorClass: 'text-danger',
            rules: {
                userName: {
                    required: true,
                    maxlength: 20
                },
                email: {
                    required: true,
                    email: true,
                    maxlength: 50
                },
                fullName: {
                    required: true,
                    maxlength: 50
                },
                password: {
                    required: true,
                    minlength: 6,
                    maxlength: 20
                },
                confirmPassword: {
                    required: true,
                    equalTo: '#txtPassword'
                },
                phoneNumber: {
                    maxlength: 20
                },
                address: {
                    maxlength: 500
                }
            },
            messages: {
                userName: {
                    required: 'Hãy nhập tên đăng nhập',
                    maxlength: 'Tên đăng nhập không vượt quá 20 ký tự'
                },
                email: {
                    required: 'Hãy nhập email',
                    email: 'Email không hợp lệ',
                    maxlength: 'Email không vượt quá 50 ký tự'
                },
                fullName: {
                    required: 'Hãy nhập họ tên',
                    maxlength: 'Họ tên không vượt quá 50 ký tự'
                },
                password: {
                    required: 'Hãy nhập mật khẩu',
                    minlength: 'Mật khẩu phải ít nhất 6 ký tự',
                    maxlength: 'Mật khẩu không vượt quá 20 ký tự'
                },
                confirmPassword: {
                    required: 'Hãy nhập xác nhận mật khẩu',
                    equalTo: 'Xác nhận mật khẩu không khớp'
                },
                phoneNumber: {
                    maxlength: 'Số điện thoại không vượt quá 20 ký tự'
                },
                address: {
                    maxlength: 'Địa chỉ không vượt quá 500 ký tự'
                }
            },
            highlight: function (element) {
                $(element).addClass('is-invalid').removeClass('is-valid');
            },
            unhighlight: function (element) {
                $(element).addClass('is-valid').removeClass('is-invalid');
            }
        });
    };

    var getById = function (id, callBack) {
        core.callAjax('/Admin/User/GetById', 'GET', { id }, 'json', function (res) {
            $('#txtId').val(res.Id);
            $('#txtDateCreated').val(res.DateCreated);
            $('#txtUserName').val(res.UserName);
            $('#txtEmail').val(res.Email);
            $('#txtFullName').val(res.FullName);
            $('#txtPassword').val(res.Password);
            $('#txtConfirmPassword').val(res.Password);
            $('#txtBirthDay').val(core.dateFormatJson2(res.BirthDay));
            $('#txtPhoneNumber').val(res.PhoneNumber);
            if (res.Avatar !== null && res.Avatar !== '') {
                $('#txtAvatar').val(res.Avatar);
                $('#iAvatar').attr('src', `/uploaded/images/avatars/${res.Avatar}`);
            }
            $('#txtAddress').val(res.Address);
            $('#chkGender').prop('checked', res.Gender);
            $('#chkStatus').prop('checked', res.Status);
            loadRoles(res.Roles);

            console.log(res);
            callBack();
        });
    };

    var resetForm = function () {
        $('#txtId').val(null);
        $('#txtDateCreated').val(null);
        $('#txtUserName').val('');
        $('#txtEmail').val('');
        $('#txtFullName').val('');
        $('#txtPassword').val('');
        $('#txtConfirmPassword').val('');
        $('#txtBirthDay').val(null);
        $('#txtPhoneNumber').val('');
        $('#txtAvatar').val('');
        $('#fUploadFile').val(null);
        $('#iAvatar').attr('src', '');
        $('#txtAddress').val('');
        $('#chkGender').prop('checked', true);
        $('#chkStatus').prop('checked', false);

        $('.nav-tabs a:first').tab('show');
        $("#userForm").validate().resetForm();
        $('input, textarea, select, file, number').removeClass('is-valid');
        $('input, textarea, select, file, number').removeClass('is-invalid');

        loadRoles();
    };

    var loadRoles = function (selectedRoles) {
        core.callAjax('/Admin/Role/GetAll', 'GET', null, 'json', function (res) {
            var data = res;
            var render = '';
            var template = $('#rolesTemplate').html();

            $.each(data, function (i, item) {
                var checked = '';
                if (selectedRoles !== undefined && selectedRoles.indexOf(item.Name) !== -1) {
                    checked = 'checked';
                }

                render += Mustache.render(template, {
                    Id: item.Id,
                    Name: item.Name,
                    Description: item.Description,
                    Checked: checked
                });
            });

            $('#rolesContent').html(render);
        });
    };

    var loadData = function (isPageChanged) {
        core.callAjax('/Admin/User/GetAllPaging', 'GET', {
            keyword: $('#txtKeyword').val(),
            page: core.configs.pageIndex,
            pageSize: core.configs.pageSize
        }, 'json', function (res) {
            var render = '';
            var template = $('#tbodyTemplate').html();
            var numberOrder = res.FirstRowOnPage;

            $.each(res.Results, function (index, item) {
                render += Mustache.render(template, {
                    NumberOrder: numberOrder++,
                    Id: item.Id,
                    UserName: item.UserName,
                    Email: item.Email,
                    FullName: item.FullName,
                    Avatar: core.getImage('avatars', item.Avatar),
                    Status: core.getStatus(item.Status)
                });
            });

            $('#tbodyContent').html(render);

            $('#lblTotalRecords').text(res.RowCount);
            $('#lblFirstRow').text(res.FirstRowOnPage);
            $('#lblLastRow').text(res.LastRowOnPage);

            if (core.configs.pageSize < res.RowCount) {
                $('#paginationUL').show();

                wrapPaging(res.RowCount, function () {
                    loadData();
                }, isPageChanged);
            } else {
                $('#paginationUL').hide();
            }

            $('#chkAll').prop('checked', false);
            $('#chkAll').prop('indeterminate', false);
            $('#btnDeleteMultiple').prop('disabled', true); 
        });
    };

    var wrapPaging = function (recordCount, callBack, changePageSize) {
        var totalsize = Math.ceil(recordCount / (core.configs.pageSize === -1 ? recordCount : core.configs.pageSize));

        //Unbind pagination if it existed or click change pagesize
        if ($('#paginationUL a').length === 0 || changePageSize === true) {
            $('#paginationUL').empty();
            $('#paginationUL').removeData("twbs-pagination");
            $('#paginationUL').unbind("page");
        }

        //Bind Pagination Event
        $('#paginationUL').twbsPagination({
            totalPages: totalsize,
            visiblePages: 7,
            first: 'Đầu',
            prev: 'Trước',
            next: 'Sau',
            last: 'Cuối',
            onPageClick: function (event, p) {
                core.configs.pageIndex = p;
                setTimeout(callBack(), 200);
            }
        });
    };
};