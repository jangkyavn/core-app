'use strict';

var SidebarFilter = function () {
    this.initialize = function () {
        $("#slider-range").slider({
            range: true,
            min: parseInt($('#hidMinPrice').val()),
            max: parseInt($('#hidMaxPrice').val()),
            values: [parseInt($('#hidFromPrice').val()), parseInt($('#hidToPrice').val())],
            slide: function (event, ui) {
                $(".amount-range-price").text(core.formatNumber(ui.values[0], 0) + 'đ - ' + core.formatNumber(ui.values[1], 0) + 'đ');
                $('#hidFromPrice').val(ui.values[0]);
                $('#hidToPrice').val(ui.values[1]);
            }
        });
        $(".amount-range-price").text(core.formatNumber($("#slider-range").slider("values", 0), 0) + 'đ - ' + core.formatNumber($("#slider-range").slider("values", 1), 0) + 'đ');
        $("#slider-range .ui-slider-range").css('background', '#E83F33');
    };
};