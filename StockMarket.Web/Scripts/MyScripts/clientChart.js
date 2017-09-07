window.onload = function () {
    const maxNumberOfStockDataPoints = 20;
    const updateIntervalMilliseconds = 10000;
    let chart = new CanvasJS.Chart("chartContainer", {
        zoomEnabled: false,
        title: {
            text: "Stock Prices",
            fontColor: "#333",
            fontSize: 28,
            fontFamily: "Arial",
            fontWeight: "lighter"
        },
        toolTip: {
            shared: true

        },
        axisX: {
            labelFontSize: 15,
            labelAngle: -90
        },
        axisY: {
            suffix: " PLN",
            includeZero: false,
            labelFontSize: 17
        },
        data: [],
        legend: {
            verticalAlign: "center",
            horizontalAlign: "right",
            fontSize: 17,
            fontWeight: "bold",
            fontFamily: "calibri",
            fontColor: "dimGrey",

            cursor: "pointer",
            itemclick: function (e) {
                if (typeof (e.dataSeries.visible) === "undefined" || e.dataSeries.visible) {
                    e.dataSeries.visible = false;
                }
                else {
                    e.dataSeries.visible = true;
                }
                chart.render();
            }
        }
    });

    let initChartData = function (data) {
        chart.data = [];

        $.each(data, function (i, stock) {

            let dataSeries = {
                type: "line",
                xValueType: "dateTime",
                showInLegend: true,
                name: stock[0].Code,
                dataPoints: []
            };

            $.each(stock, function (k, stockPriceRow) {
                dataSeries.dataPoints.push({
                    x: new Date(parseInt(stockPriceRow.PublicationDate.substr(6))),
                    y: stockPriceRow.Price
                });
            });

            chart.data.push(dataSeries);
        });

        chart.options.data = chart.data;
        chart.render();
    };

    let initChart = function () {
        // this url returns cached stock prices in json format
        let apiUrl = "/stock/latestStockPrices/" + maxNumberOfStockDataPoints;
        let jsonRequestHandler = $.getJSON(apiUrl,
            function () {
            })
            .done(function (data) {
                initChartData(data);
            })
            .fail(function () {
            })
            .always(function () {
            });
    };
    
    initChart();
    setInterval(initChart, updateIntervalMilliseconds);
};
