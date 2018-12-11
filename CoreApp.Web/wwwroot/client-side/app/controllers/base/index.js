﻿'use strict';

var BaseController = {
    initialize: function () {
        BaseController.loadHeaderCart();
        BaseController.initialControls();
        BaseController.registerEvents();
    },
    registerEvents: function () {
        $('body').on('click', '.remove-cart', function (e) {
            e.preventDefault();
            var productId = parseInt($(this).data('id'));

            core.callAjax('/Cart/RemoveFromCart', 'POST', { productId }, 'json', function () {
                core.notify(resources.delete_to_item_cart_success, 'success');
                BaseController.loadHeaderCart();
            });
        });

        $('body').on('click', '.btn-country', function (e) {
            e.preventDefault();
            var culture = $(this).data('value');

            $('#hidCulture').val(culture);
            $('#selectLanguage').submit();
        });
    },
    initialControls: function () {
        $("#txtKeywordSearch").autocomplete({
            minLength: 0,
            source: function (request, response) {
                $.ajax({
                    url: "/Product/SuggestSearchResult",
                    type: 'get',
                    dataType: "json",
                    data: {
                        keyword: request.term
                    },
                    success: function (data) {
                        response(data);
                    }
                });
            },
            select: function (event, ui) {
                $("#txtKeywordSearch").val(ui.item.Name);

                return false;
            },
            open: function () {
                $('.ui-autocomplete').css('width', '550px');
                $('.ui-autocomplete').css('max-height', '500px');
                $('.ui-autocomplete').css('overflow-y', 'auto');
            }
        }).autocomplete("instance")._renderItem = function (ul, item) {
            return $("<li>")
                .append(`
                    <div style='display:flex; flex-direction:row; flex:10'>
                        <div style='flex:2'>
                            <img src='/uploaded/images/products/${item.Image}' width='100' />
                        </div>
                        <div style='flex:8; display:flex; flex-direction:column; font-size: 20px;'>
                            <a href='/${item.SeoAlias}-p.${item.Id}.html'>${item.Name}</a>
                            <div>${core.formatNumber(item.Price, 0)}</div>
                        </div>
                    </div>
                `)
                .appendTo(ul);
        };
    },
    loadHeaderCart: function () {
        core.callAjax('/Cart/GetCart', 'GET', null, 'json', function (res) {
            var template = $('#templateHeaderCart').html();
            var render = '<ul id="cart-sidebar" class="mini-products-list">';

            if (res.length) {
                $.each(res, function (index, item) {
                    var url = `/${item.Product.SeoAlias}-p.${item.Product.Id}.html`;
                    var sourceImage = `/uploaded/images/products/${item.Product.Image}`;
                    var idx = index + 1;

                    render += Mustache.render(template,
                        {
                            IsLast: idx === res.length ? 'last' : '',
                            EvenOdd: idx % 2 === 0 ? 'even' : 'odd',
                            ProductId: item.Product.Id,
                            ProductName: item.Product.Name,
                            Image: sourceImage,
                            Price: core.formatNumber(item.Price, 0),
                            Quantity: item.Quantity,
                            Url: url
                        });
                });

                let totalPrice = res.reduce((weight, animal) => {
                    return weight += (animal.Price * animal.Quantity);
                }, 0);

                render += `</ul><div class="top-subtotal">Tổng tiền: <span class="price">${core.formatNumber(totalPrice, 0)} VNĐ</span></div>
                            <div class="actions">
                                <button class="btn-checkout" type="button" onClick="location.href='/dat-hang.html'"><i class="fa fa-check"></i><span>Đặt hàng</span></button>
                                <button class="view-cart" type="button" onClick="location.href='/gio-hang.html'"><i class="fa fa-shopping-cart"></i><span>Xem giỏ</span></button>
                            </div>`;
            }
            else {
                render = `<div style="text-align: center; padding: 1rem 0;">
                                    Không có sản phẩm nào trong giỏ hàng
                                </div>`;
            }

            $('#cartContent').html(render);
            $('#cartTotal').text(res.length);
        });
    }
};

BaseController.initialize();