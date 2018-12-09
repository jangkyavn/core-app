'use strict';

var AccountController = function () {
    this.initialize = function () {
        validateForm();
        registerEvents();
    };

    var registerEvents = function () {
        $('#btnLogin').on('click', function () {
            if ($('#loginForm').valid()) {
                var data = {
                    userName: $('#txtUserName').val(),
                    password: $('#txtPassword').val(),
                    rememberMe: $('#chkRemember').prop('checked')
                };

                login(data);
            }
        });

        $('#txtUserName, #txtPassword').on('keypress', function (e) {
            if (e.which === 13) {
                $('#btnLogin').click();
            }
        });
    };

    var validateForm = function () {
        $('#loginForm').validate({
            errorClass: 'text-danger',
            rules: {
                userName: {
                    required: true,
                    maxlength: 20
                },
                password: {
                    required: true,
                    minlength: 6,
                    maxlength: 20
                }
            },
            messages: {
                userName: {
                    required: 'Vui lòng nhập tên đăng nhập',
                    maxlength: 'Tên đăng nhập không vượt quá 20 ký tự'
                },
                password: {
                    required: 'Vui lòng nhập mật khẩu',
                    minlength: 'Mật khẩu phải ít nhất 6 ký tự',
                    maxlength: 'Mật khẩu không vượt quá 20 ký tự'
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

    var login = function(data) {
        core.callAjax('/Admin/Account/Login', 'POST', data, 'json', function(res) {
            if (res.Status === true) {
                window.location.href = '/admin/home/index';
            } else {
                core.notify(res.Message, 'error');
            }
        });
    };
};