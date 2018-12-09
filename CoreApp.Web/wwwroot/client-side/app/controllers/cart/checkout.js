'use strict';

var CheckoutProduct = function () {
    this.initialize = function () {
        $('.box-border').hide();
        $('.box-border:first').show();

        validateBaseInformationForm();
        validateDeliveryAddressForm();
        registerEvents();
    };

    var registerEvents = function () {
        $('.btn-continue').click(function () {
            var step = parseInt($(this).data('step'));
            var isSuccess = false;
            if (step === 1) {
                var phoneNumber = $('#lblPhoneNumber').text();
                var address = $('#lblAddress').text();

                if (phoneNumber === '???') {
                    $('#lblPhoneNumberError').show();
                }

                if (address === '???') {
                    $('#lblAddressError').show();
                }

                if (phoneNumber !== '???' && address !== '???') {
                    isSuccess = true;
                }
            } else if (step === 2) {
                if ($('.rdo-payment-method:checked').length > 0) {
                    $('#lblPaymentMethodError').hide();
                    var paymentMethod = $('.rdo-payment-method:checked').val();
                    $('#PaymentMethod').val(paymentMethod);
                    isSuccess = true;
                } else {
                    $('#lblPaymentMethodError').show();
                }
            } else {
                ////
            }

            if (isSuccess) {
                $('.box-border').each(function () {
                    if ($(this).is(':visible')) {
                        $(this).slideToggle('slow', function () {
                            $(this).next().next().slideToggle('slow');
                        });
                    }
                });
            }
        });

        $('.btn-back').on('click', function () {
            $('.box-border').each(function () {
                if ($(this).is(':visible')) {
                    $(this).slideToggle('slow', function () {
                        $(this).prev().prev().slideToggle('slow');
                    });
                }
            });
        });

        $('#baseInformationLayout').on('click', '#btnUpdateBaseInformation', function (e) {
            e.preventDefault();

            $('#btnStepOne').hide();
            $('#baseInformationFormLayout').show();
            $('#baseInformationLayout').hide();
            $('#deliveryAddressLayout').hide();
        });

        $('#deliveryAddressLayout').on('click', '#btnUpdateDeliveryAddress', function (e) {
            e.preventDefault();

            $('#btnStepOne').hide();
            $('#deliveryAddressFormLayout').show();
            $('#deliveryAddressLayout').hide();
            $('#baseInformationLayout').hide();

            loadCities();
            loadDictricts();
            loadWards();

            getDetailDeliveryAddress();
        });

        $('#btnSaveBaseInformation').on('click', function () {
            if ($('#baseInformationForm').valid()) {
                var data = {
                    email: $('#txtEmail').val(),
                    fullName: `${$('#txtLastName').val()} ${$('#txtFirstName').val()}`,
                    phoneNumber: $('#txtPhoneNumber').val()
                };

                saveBaseInformation(data);
            }
        });

        $('#btnSaveDeliveryAddress').on('click', function () {
            if ($('#deliveryAddressForm').valid()) {
                var address = $('#txtAddress').val();
                var city = $('#ddlCity').find(':selected').data('name');
                var district = $('#ddlDistrict').find(':selected').data('name');
                var ward = $('#ddlWard').find(':selected').data('name');

                var data = `${address}, ${ward}, ${district}, ${city}`;

                saveDeliveryAddress(data);
            }
        });

        $('#ddlCity').on('change', function () {
            loadDictricts();
        });

        $('#ddlDistrict').on('change', function () {
            loadWards();
        });

        $('.rdo-payment-method').on('change', function () {
            $('#lblPaymentMethodError').hide();
        });
    };

    var validateBaseInformationForm = function () {
        $('#baseInformationForm').validate({
            errorClass: 'text-danger',
            rules: {
                lastName: {
                    required: true,
                    maxlength: 20
                },
                firstName: {
                    required: true,
                    maxlength: 20
                },
                phoneNumber: {
                    required: true,
                    maxlength: 15
                }
            },
            messages: {
                lastName: {
                    required: 'Vui lòng nhập họ của bạn.',
                    maxlength: 'Vui lòng nhập họ không quá 20 ký tự.'
                },
                firstName: {
                    required: 'Vui lòng nhập tên của bạn.',
                    maxlength: 'Vui lòng nhập tên không quá 20 ký tự.'
                },
                phoneNumber: {
                    required: 'Vui lòng nhập số điện thoại.',
                    maxlength: 'Vui lòng nhập số điện thoại không quá 15 ký tự.'
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

    var validateDeliveryAddressForm = function () {
        $('#deliveryAddressForm').validate({
            errorClass: 'text-danger',
            rules: {
                address: {
                    required: true,
                    maxlength: 100
                },
                city: {
                    required: true
                },
                district: {
                    required: true
                },
                ward: {
                    required: true
                }
            },
            messages: {
                address: {
                    required: 'Vui lòng nhập địa chỉ.',
                    maxlength: 'Vui lòng nhập địa chỉ không quá 100 ký tự.'
                },
                city: {
                    required: 'Vui lòng chọn tỉnh/thành phố.'
                },
                district: {
                    required: 'Vui lòng chọn quận/huyện.'
                },
                ward: {
                    required: 'Vui lòng chọn phường/xã.'
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

    var loadBaseInformationContent = function () {
        var id = $('#hidUserId').val();
        core.callAjax('/User/GetById', 'GET', { id }, 'json', function (res) {

            $('#baseInformationLayout').html('');
            $('#baseInformationLayout').html(`
                <a href="#" class="close" style="font-size: 15px; color: blue" id="btnUpdateBaseInformation">Cập nhật</a>
                <h5>Thông tin cơ bản</h5>
                <p>Họ tên: <strong>${res.FullName}</strong></p>
                <p>Email: <strong>${res.Email}</strong></p>
                <p>Số điện thoại: <strong id="lblPhoneNumber">${res.PhoneNumber}</strong></p>
            `);
        });
    };

    var saveBaseInformation = function (data) {
        core.callAjax('/User/Update', 'POST', data, 'json', function (res) {
            if (res) {
                core.notify(resources.update_success, 'success');
                $('#btnStepOne').show();
                $('#baseInformationFormLayout').hide();
                $('#baseInformationLayout').show();
                $('#deliveryAddressLayout').show();
                loadBaseInformationContent();
            } else {
                core.notify(resources.has_error, 'error');
            }
        });
    };

    var loadDeliveryAddressContent = function () {
        var id = $('#hidUserId').val();
        core.callAjax('/User/GetById', 'GET', { id }, 'json', function (res) {

            $('#deliveryAddressLayout').html('');
            $('#deliveryAddressLayout').html(`
                <a href="#" class="close" style="font-size: 15px; color: blue" id="btnUpdateDeliveryAddress">Cập nhật</a>
                <h5>Địa chỉ giao hàng</h5>
                <p>Địa chỉ: <strong id="lblAddress">${res.Address}</strong></p>
            `);
        });
    };

    var saveDeliveryAddress = function (address) {
        core.callAjax('/User/UpdateAddress', 'POST', { address }, 'json', function (res) {
            if (res) {
                core.notify(resources.update_success, 'success');
                $('#btnStepOne').show();
                $('#deliveryAddressFormLayout').hide();
                $('#deliveryAddressLayout').show();
                $('#baseInformationLayout').show();
                loadDeliveryAddressContent();
            } else {
                core.notify(resources.has_error, 'error');
            }
        });
    };

    var getDetailDeliveryAddress = function () {
        var addressFull = $('#lblAddress').text();

        if (addressFull !== null && addressFull !== '') {
            var arrAddress = addressFull.split(',');
            var city = arrAddress[arrAddress.length - 1].trim();
            var district = arrAddress[arrAddress.length - 2].trim();
            var ward = arrAddress[arrAddress.length - 3].trim();
            $('#txtAddress').val(arrAddress[0].trim());
            var cityId = 0;

            core.callAjax('/User/LoadCities', 'GET', null, 'json', function (cities) {
                $.each(cities, function (idx, item) {
                    if (city === item.Name_With_Type) {
                        cityId = item.Code;
                        return false;
                    }
                });

                console.log(cityId);

                $("#ddlCity").val(cityId);
                var districtId = 0;
                core.callAjax('/User/LoadDistricts', 'GET', { cityId }, 'json', function (districts) {
                    $.each(districts, function (idx, item) {
                        if (district === item.Name_With_Type) {
                            districtId = item.Code;
                            return false;
                        }
                    });

                    var wardId = 0;
                    loadDictricts(function () {
                        $("#ddlDistrict").val(districtId);

                        core.callAjax('/User/LoadWards', 'GET', { districtId }, 'json', function (wards) {
                            $.each(wards, function (idx, item) {
                                if (ward === item.Name_With_Type) {
                                    wardId = item.Code;
                                    return false;
                                }
                            });

                            loadWards(function () {
                                $("#ddlWard").val(wardId);
                            });
                        });
                    });
                });
            });
        }
    };

    var loadCities = function () {
        core.callAjax('/User/LoadCities', 'GET', null, 'json', function (res) {
            var render = '<option value="" disabled selected>Chọn Tỉnh/Thành phố</option>';

            $.each(res, function (index, item) {
                render += `<option value=${item.Code} data-name='${item.Name_With_Type}'>${item.Name}</option>`;
            });

            $('#ddlCity').html(render);
        });
    };

    var loadDictricts = function (callback) {
        var cityId = $('#ddlCity').val();

        if (cityId !== '' && cityId !== null) {
            $('#ddlDistrict').prop('disabled', false);

            core.callAjax('/User/LoadDistricts', 'GET', { cityId }, 'json', function (res) {
                var render = '<option value="" disabled selected>Chọn Quận/Huyện</option>';

                $.each(res, function (index, item) {
                    render += `<option value=${item.Code} data-name='${item.Name_With_Type}'>${item.Name}</option>`;
                });

                $('#ddlDistrict').html(render);
                if (callback) {
                    callback();
                }
            });
        } else {
            var render = '<option value="">Chọn Quận/Huyện</option>';
            $('#ddlDistrict').html(render);
            $('#ddlDistrict').prop('disabled', true);
        }
    };

    var loadWards = function (callback) {
        var districtId = $('#ddlDistrict').val();

        if (districtId !== '' && districtId !== null) {
            $('#ddlWard').prop('disabled', false);

            core.callAjax('/User/LoadWards', 'GET', { districtId }, 'json', function (res) {
                var render = '<option value="" disabled selected>Chọn Phường/Xã</option>';

                $.each(res, function (index, item) {
                    render += `<option value=${item.Code} data-name='${item.Name_With_Type}'>${item.Name}</option>`;
                });

                $('#ddlWard').html(render);
                if (callback) {
                    callback();
                }
            });
        } else {
            var render = '<option value="">Chọn Phường/Xã</option>';
            $('#ddlWard').html(render);
            $('#ddlWard').prop('disabled', true);
        }
    };
};
