'use strict';

var WholePrice = function () {
    var self = this;
    var cachedObj = {
        colors: [],
        sizes: []
    };

    this.initialize = function () {
        registerEvents();
    };

    var registerEvents = function () {
        $('body').on('click', '.btn-whole-price', function (e) {
            e.preventDefault();
            var that = $(this).data('id');
            $('#hidId').val(that);
            loadWholePrices();
            $('#manageWholePriceModal').modal('show');
        });

        $('body').on('click', '.btn-delete-whole-price', function (e) {
            e.preventDefault();
            $(this).closest('tr').remove();
        });

        $('#btn-add-whole-price').on('click', function () {
            var template = $('#wholePriceTemplate').html();
            var render = Mustache.render(template, {
                Id: 0,
                FromQuantity: 0,
                ToQuantity: 0,
                Price: 0
            });
            $('#wholePriceContent').append(render);
        });

        $("#btnSaveWholePrice").on('click', function () {
            var priceList = [];
            $.each($('#wholePriceContent').find('tr'), function (i, item) {
                priceList.push({
                    Id: $(item).data('id'),
                    ProductId: $('#hidId').val(),
                    FromQuantity: $(item).find('input.txtQuantityFrom').first().val(),
                    ToQuantity: $(item).find('input.txtQuantityTo').first().val(),
                    Price: $(item).find('input.txtWholePrice').first().val(),
                });
            });

            core.callAjax('/Admin/Product/SaveWholePrice', 'POST', {
                productId: $('#hidId').val(),
                wholePrices: priceList
            }, 'json', function (res) {
                $('#manageWholePriceModal').modal('hide');
                $('#wholePriceContent').html('');
            });
        });
    };

    var loadWholePrices = function () {
        core.callAjax('/Admin/Product/GetWholePrices', 'GET', {
            productId: $('#hidId').val()
        }, 'json', function (res) {
            var render = '';
            var template = $('#wholePriceTemplate').html();
            $.each(res, function (i, item) {
                render += Mustache.render(template, {
                    Id: item.Id,
                    FromQuantity: item.FromQuantity,
                    ToQuantity: item.ToQuantity,
                    Price: item.Price
                });
            });
            $('#wholePriceContent').html(render);
        });
    };
};