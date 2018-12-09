'use strict';

var ProductImage = function () {
    var self = this;
    var parent = parent;

    var images = [];

    this.initialize = function () {
        registerEvents();
    };

    var registerEvents = function () {
        $('body').on('click', '.btn-images', function (e) {
            e.preventDefault();
            var that = $(this).data('id');
            $('#hidId').val(that);
            loadImages();
            $('#manageImageModal').modal('show');
        });

        $('body').on('click', '.btn-delete-image', function (e) {
            e.preventDefault();
            $(this).closest('div').remove();
        });

        $("#fileImage").on('change', function () {
            var fileUpload = $(this).get(0);
            var files = fileUpload.files;
            var data = new FormData();
            for (var i = 0; i < files.length; i++) {
                data.append(files[i].name, files[i]);
            }

            core.callAjaxFile('/Admin/Upload/UploadImage?type=products', 'POST', data, function (res) {
                images.push(res);
                var path = `/uploaded/images/products/${res}`;
                $('#productImageContent').append('<div class="col-md-3"><img width="100"  data-path="' + path + '" src="' + path + '"></div>');
            });
        });

        $("#btnSaveImages").on('click', function () {
            var imageList = [];
            $.each($('#productImageContent').find('img'), function (i, item) {
                imageList.push($(this).data('path'));
            });

            core.callAjax('/Admin/Product/SaveImages', 'POST', {
                productId: $('#hidId').val(),
                images: imageList
            }, 'json', function (res) {
                $('#manageImageModal').modal('hide');
                $('#productImageContent').html('');
                core.notify(message.update_success, 'success');
            });
        });
    };

    var loadImages = function () {
        core.callAjax('/Admin/Product/GetImages', 'GET', {
            productId: $('#hidId').val()
        }, 'json', function (res) {
            var render = '';
            $.each(res, function (i, item) {
                var path = `${item.Path}`;
                render += `<div class="col-md-3">
                                <img width="100" src="${path}">
                                <br/>
                                <a href="#" class="btn-delete-image">Xóa</a>
                            </div>`;
            });

            $('#productImageContent').html(render);
        });
    };
};