'use strict';

var AccountController = function () {
    this.initialize = function () {
        loadDay(31);
        loadMonth();
        loadYear();

        initialControls();
        registerEvents();
    };

    var registerEvents = function () {
        $('#ddlMonth').on('change', function () {
            handleDate();
        });

        $('#ddlYear').on('change', function () {
            handleDate();
        });
    };

    var initialControls = function () {
        $('#PhoneNumber').mask('(84) 000-000-000');
    };

    var handleDate = function () {
        var date = parseInt($('#ddlDay').val());
        var month = parseInt($('#ddlMonth').val());
        var year = parseInt($('#ddlYear').val());

        if (month !== '' && year !== '') {
            if (month === 2) {
                if ((year % 4 === 0 && year % 100 !== 0) || year % 400 === 0) {
                    loadDay(29);
                } else {
                    loadDay(28);
                }
            } else {
                if (month === 4 || month === 6 || month === 9 || month === 11) {
                    loadDay(30);
                } else {
                    loadDay(31);
                }
            }

            $('#ddlDay').val(isNaN(date) ? '' : date);
        }
    };

    var loadDay = function (date) {
        var option = '<option value="">Ngày</option>';

        for (var i = 1; i <= date; i++) {
            option += `<option value=${i} style="color:black">${i}</option>`;
        }

        $('#ddlDay').html(option);
    };

    var loadMonth = function () {
        var option = '<option value="">Tháng</option>';

        for (var i = 1; i <= 12; i++) {
            option += `<option value=${i} style="color:black">${i}</option>`;
        }

        $('#ddlMonth').html(option);
    };

    var loadYear = function () {
        var option = '<option value="">Năm</option>';
        var currentYear = new Date().getFullYear();

        for (var i = currentYear; i >= (currentYear - 100); i--) {
            option += `<option value=${i} style="color:black">${i}</option>`;
        }

        $('#ddlYear').html(option);
    };
};