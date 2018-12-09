'use strict';

var HomeController = function () {
    this.initialize = function () {
        initDateRangePicker();
        loadData();
        registerEvents();
    };

    var registerEvents = function () {
        $('#daterange').on('apply.daterangepicker', function (ev, picker) {
            var fromDate = picker.startDate.format('MM/DD/YYYY');
            var toDate = picker.endDate.format('MM/DD/YYYY');

            loadData(fromDate, toDate);
        });
    };

    var initDateRangePicker = function () {
        var date = new Date();

        $('input[name="daterange"]').daterangepicker({
            "startDate": new Date(date.getFullYear(), date.getMonth(), 1),
            "endDate": new Date(date.getFullYear(), date.getMonth() + 1, 0),
            "locale": {
                "format": "DD/MM/YYYY",
                "applyLabel": "Áp dụng",
                "cancelLabel": "Hủy",
                "fromLabel": "Từ",
                "toLabel": "Tới",
                "daysOfWeek": [
                    "CN",
                    "T2",
                    "T3",
                    "T4",
                    "T5",
                    "T6",
                    "T7"
                ],
                "monthNames": [
                    "Tháng 1",
                    "Tháng 2",
                    "Tháng 3",
                    "Tháng 4",
                    "Tháng 5",
                    "Tháng 6",
                    "Tháng 7",
                    "Tháng 8",
                    "Tháng 9",
                    "Tháng 10",
                    "Tháng 11",
                    "Tháng 12"
                ]
            }
        });
    };

    var initChart = function (data) {
        var arrRevenue = [];
        var arrProfit = [];

        $.each(data, function (i, item) {
            arrRevenue.push({
                x: new Date(item.Date),
                y: item.Revenue
            });
        });

        $.each(data, function (i, item) {
            arrProfit.push({
                x: new Date(item.Date),
                y: item.Profit
            });
        });

        var options = {
            exportEnabled: true,
            animationEnabled: true,
            title: {
                text: "Danh thu và lợi nhuận"
            },
            axisX: {
                valueFormatString: "MM/YYYY"
            },
            axisY: {
                title: "Doanh thu",
                titleFontColor: "#4F81BC",
                lineColor: "#4F81BC",
                labelFontColor: "#4F81BC",
                tickColor: "#4F81BC",
                includeZero: false
            },
            axisY2: {
                title: "Lợi nhuận",
                titleFontColor: "#C0504E",
                lineColor: "#C0504E",
                labelFontColor: "#C0504E",
                tickColor: "#C0504E",
                includeZero: false
            },
            toolTip: {
                shared: true
            },
            legend: {
                cursor: "pointer",
                itemclick: toggleDataSeries
            },
            data: [{
                type: "spline",
                name: "Doanh thu",
                showInLegend: true,
                xValueFormatString: "DD-MM",
                yValueFormatString: "#,###",
                dataPoints: arrRevenue
            },
            {
                type: "spline",
                name: "Lợi nhuận",
                axisYType: "secondary",
                showInLegend: true,
                xValueFormatString: "DD-MM",
                yValueFormatString: "#,###",
                dataPoints: arrProfit
            }]
        };

        $("#chartContainer").CanvasJSChart(options);

        function toggleDataSeries(e) {
            if (typeof (e.dataSeries.visible) === "undefined" || e.dataSeries.visible) {
                e.dataSeries.visible = false;
            } else {
                e.dataSeries.visible = true;
            }
            e.chart.render();
        }
    };

    var loadData = function (fromDate, toDate) {
        core.callAjax('/Admin/Home/GetRevenues', 'GET', { fromDate, toDate }, 'json', function (res) {
            initChart(res);
        });
    };
};