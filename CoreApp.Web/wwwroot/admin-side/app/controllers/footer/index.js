'use strict';

var footerController = function () {
    this.initialize = function () {
        validateForm();
        loadData();
        registerControls();
        registerEvents();
    };

    var registerEvents = function () {
        $('#btnAddNew').on('click', function () {
            resetForm();
            $('#modalTitle').text('Thêm mới footer');
            $('#addEditModal').modal({
                show: true,
                backdrop: 'static'
            });
        });

        $('#btnSave').on('click', function () {
            if ($('#footerForm').valid()) {
                var data = {
                    Id: $('#hidId').val(),
                    Content: $('#txtContent').val()
                };

                core.callAjax('/Admin/Footer/SaveEntity', 'POST', data, 'json', function (res) {
                    if (res) {
                        core.notify(message.add_success, 'success');
                    } else {
                        core.notify(message.edit_success, 'success');
                    }

                    $('#addEditModal').modal('hide');
                    loadData();
                });
            }
        });

        $('body').on('click', '.btn-preview', function () {
            $('#previewModal').modal({
                show: true,
                backdrop: 'static'
            });

            setTimeout(function () {
                document.getElementById('frame').contentWindow.location.reload();
                $('#div_iframe').scrollTop(2900);
            }, 200);
        });

        $('body').on('click', '.btn-edit', function (e) {
            e.preventDefault();
            resetForm();
            $('#modalTitle').text('Sửa footer');
            var id = $(this).data('id');
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
                core.callAjax('/Admin/Footer/Delete', 'POST', { id }, 'json', function () {
                    core.notify(message.delete_success, 'success');
                    loadData();
                });
            });
        });
    };

    var registerControls = function () {
        //CKEDITOR.replace('txtContent', {
        //    extraPlugins: 'colorbutton',
        //    filebrowserImageUploadUrl: '/Admin/Upload/UploadImageForCKEditor',
        //    filebrowserUploadMethod: 'form',
        //    height: '30rem'
        //});

        ////Fix: cannot click on element ck in modal
        //$.fn.modal.Constructor.prototype._enforceFocus = function () {
        //    //var $modalElement = this.$element;
        //    //$(document).on('focusin.modal', function (e) {
        //    //    if ($modalElement[0] !== e.target
        //    //        && !$modalElement.has(e.target).length
        //    //        && $(e.target).parentsUntil('*[role="dialog"]').length === 0) {
        //    //        $modalElement.focus();
        //    //    }
        //    //});
        //};
    };

    var validateForm = function () {
        $('#footerForm').validate({
            errorClass: 'text-danger',
            rules: {
                content: {
                    required: true
                }
            },
            messages: {
                content: {
                    required: 'Hãy nhập nội dung footer'
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
        core.callAjax('/Admin/Footer/GetFooter', 'GET', { id }, 'json', function (res) {
            $('#hidId').val(res.Id);
            $('#txtContent').val(res.Content);

            callBack();
        });
    };

    var resetForm = function () {
        $('#hidId').val('');
        $('#txtContent').val('');

        $("#footerForm").validate().resetForm();
        $('input, textarea, select, file, number').removeClass('is-valid');
        $('input, textarea, select, file, number').removeClass('is-invalid');
    };

    var loadData = function () {
        core.callAjax('/Admin/Footer/GetFooter', 'GET', {
            id: 'DefaultFooterId'
        }, 'json', function (res) {
            var render = '';
            var template = $('#tbodyTemplate').html();

            render += Mustache.render(template, {
                NumberOrder: 1,
                Id: res.Id,
                Content: res.Content
            });

            $('#tbodyContent').html(render);

            $('#lblTotalRecords').text(1);
        });
    };
};
