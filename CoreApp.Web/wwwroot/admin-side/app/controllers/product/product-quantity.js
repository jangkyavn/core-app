'use strict';

var ProductQuantity = function () {
    var self = this;
    var cachedObj = {
        colors: [],
        sizes: []
    };

    this.initialize = function () {
        loadColors();
        loadSizes();
        registerEvents();
    };

    var registerEvents = function () {
        $('body').on('click', '.btn-quantity', function (e) {
            e.preventDefault();
            var id = $(this).data('id');
            $('#hidId').val(id);
            loadQuantities();
            $('#manageQuantityModal').modal('show');
        });

        $('body').on('click', '.btn-delete-quantity', function (e) {
            e.preventDefault();
            $(this).closest('tr').remove();
        });

        $('#btn-add-quantity').on('click', function () {
            var template = $('#productQuantityTemplate').html();
            var render = Mustache.render(template, {
                Id: 0,
                Colors: getColorOptions(null),
                Sizes: getSizeOptions(null),
                Quantity: 0
            });
            $('#productQuantityContent').append(render);
        });

        $("#btnSaveQuantity").on('click', function () {
            var quantityList = [];
            $.each($('#productQuantityContent').find('tr'), function (i, item) {
                quantityList.push({
                    Id: $(item).data('id'),
                    ProductId: $('#hidId').val(),
                    Quantity: $(item).find('input.txtQuantity').first().val(),
                    SizeId: $(item).find('select.ddlSizeId').first().val(),
                    ColorId: $(item).find('select.ddlColorId').first().val(),
                });
            });

            core.callAjax('/Admin/Product/SaveQuantities', 'POST', {
                productId: $('#hidId').val(),
                quantities: quantityList
            }, 'json', function (res) {
                core.notify(message.update_success, 'success');
                $('#manageQuantityModal').modal('hide');
                $('#productQuantityContent').html('');
            });
        });
    };

    var loadColors = function () {
        core.callAjax('/Admin/Color/GetAll', 'GET', null, 'json', function (res) {
            cachedObj.colors = res;
        });
    };

    var loadSizes = function () {
        core.callAjax('/Admin/Size/GetAll', 'GET', null, 'json', function (res) {
            cachedObj.sizes = res;
        });
    };

    var getColorOptions = function (selectedId) {
        var colors = "<select class='form-control ddlColorId'>";
        $.each(cachedObj.colors, function (i, color) {
            if (selectedId === color.Id)
                colors += '<option value="' + color.Id + '" selected="select">' + color.Name + '</option>';
            else
                colors += '<option value="' + color.Id + '">' + color.Name + '</option>';
        });
        colors += "</select>";
        return colors;
    };

    var getSizeOptions = function (selectedId) {
        var sizes = "<select class='form-control ddlSizeId'>";
        $.each(cachedObj.sizes, function (i, size) {
            if (selectedId === size.Id)
                sizes += '<option value="' + size.Id + '" selected="select">' + size.Name + '</option>';
            else
                sizes += '<option value="' + size.Id + '">' + size.Name + '</option>';
        });
        sizes += "</select>";
        return sizes;
    };

    var loadQuantities = function () {
        core.callAjax('/Admin/Product/GetQuantities', 'GET', {
            productId: $('#hidId').val()
        }, 'json', function (res) {
            var render = '';
            var template = $('#productQuantityTemplate').html();

            $.each(res, function (i, item) {
                render += Mustache.render(template, {
                    Id: item.Id,
                    Colors: getColorOptions(item.ColorId),
                    Sizes: getSizeOptions(item.SizeId),
                    Quantity: item.Quantity
                });
            });

            $('#productQuantityContent').html(render);
        });
    };
};