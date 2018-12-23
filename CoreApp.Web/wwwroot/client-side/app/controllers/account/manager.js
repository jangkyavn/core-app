'use strict';

var ManageAccount = function () {
    this.initialize = function () {
        loadCities();
        loadDictricts();
        loadWards();
        getDetailDeliveryAddress();
        validateUserForm();
        registerEvents();
    };

    var registerEvents = function () {
        $('#txtAddress').keypress(function (e) {
            if (e.which === 44) {
                return false;
            }
        });

        $('#ddlCity').on('change', function () {
            loadDictricts();
        });

        $('#ddlDistrict').on('change', function () {
            loadWards();
        });

        $('#fUploadFile').on('change', function () {
            var fileUpload = $(this).get(0);
            var files = fileUpload.files;
            var data = new FormData();

            for (var i = 0; i < files.length; i++) {
                data.append(files[i].name, files[i]);
            }

            core.callAjaxFile('/Admin/Upload/UploadImage?type=avatars', 'POST', data, function (res) {
                if (res) {
                    $('#hidAvatar').val(res);
                    $('#imgAvatar').attr('src', `/uploaded/images/avatars/${res}`);
                } else {
                    $('#hidAvatar').val('');
                    $('#imgAvatar').attr('src', `/uploaded/images/no-avatar.png`);
                    $('#fUploadFile').val(null);
                }
            });
        });

        $('#btnSaveUserForm').on('click', function () {
            if ($('#userForm').valid()) {
                var address = $('#txtAddress').val();
                var city = $('#ddlCity').find(':selected').data('name');
                var district = $('#ddlDistrict').find(':selected').data('name');
                var ward = $('#ddlWard').find(':selected').data('name');

                var data = {
                    email: $('#txtEmail').val(),
                    fullName: `${$('#txtLastName').val()} ${$('#txtFirstName').val()}`,
                    phoneNumber: $('#txtPhoneNumber').val(),
                    address: `${address}, ${ward}, ${district}, ${city}`,
                    avatar: $('#hidAvatar').val()
                };

                core.callAjax('/User/UpdateFull', 'POST', data, 'json', function (res) {
                    if (res) {
                        core.notify(resources.update_success, 'success');
                    } else {
                        core.notify(resources.has_error, 'error');
                    }
                });
            }
        });
    };

    var validateUserForm = function () {
        $('#userForm').validate({
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
                },
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
                },
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

    var getDetailDeliveryAddress = function () {
        var addressFull = $('#txtAddress').text();

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
