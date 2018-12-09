'use strict';

var BaseController = function () {
    this.initialize = function () {
        var href = window.location.href;

        $('.nav-item a').each(function (i, item) {
            if (href.indexOf($(this).attr('href')) >= 0) {
                $(this).parent('li').removeClass('nav-item');
                $(this).parent('li').addClass('active');
            }
        });

        $('.menu-content a.menu-item').each(function (e, i) {
            if (href.indexOf($(this).attr('href')) >= 0) {
                $(this).parent('li').addClass('active');
                $(this).closest('li.nav-item').addClass('open');
            }
        });
    };
};