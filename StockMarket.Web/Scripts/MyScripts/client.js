/// <reference path="../jquery.signalR-2.2.2.js" />
/// <reference path="../knockout-3.4.2.debug.js" />
/// <reference path="../dateformat.js" />

(function () {
    //-------------------------------------------------
    let stocksHub = $.connection.stockTickerHub,
        stocksTable,
        walletTable,
        stocksTableUpdateTimeEl,
        stocksArray;
    //-------------------------------------------------
    stocksHub.client.updateStocks = function (stockArray) {
        if (stockArray !== undefined && stockArray !== null) {
            stocksTableUpdateTimeEl.innerHTML = dateFormat(new Date(), "dd.mm.yyyy HH:MM:ss");
            stocksArray.updateItems(stockArray);
        }
    };
    stocksHub.client.marketOpened = function () {
        $("#connStatus").html("Market is opened now.")
            .removeClass("alert-danger")
            .addClass("alert-success");
    };
    stocksHub.client.marketClosed = function () {
        $("#connStatus").html("Market is closed now.")
            .removeClass("alert-success")
            .addClass("alert-danger");
    };
    //-------------------------------------------------
    let StockModel = function (Name, Code, Unit, Price, AvailableAmount) {
        var self = this;

        self.Name = ko.observable(Name);
        self.Code = ko.observable(Code);
        self.Unit = ko.observable(Unit);
        self.Price = ko.observable(Price);
        self.AvailableAmount = ko.observable(AvailableAmount);
    };
    //-------------------------------------------------
    let StocksArray = function () {
        let self = this;

        self.items = ko.observableArray();
    };

    StocksArray.prototype = {
        updateItems: function (newItems) {
            let self = this;

            $.each(newItems, function (index, newItem) {

                let isInArray = ko.utils.arrayFirst(self.items(), function (item) {
                    return item.Name() === newItem.Name;
                });

                if (!isInArray) {
                    let stock = new StockModel(
                        newItem.Name,
                        newItem.Code,
                        newItem.Unit,
                        newItem.Price,
                        newItem.AvailableAmount
                    );
                    self.items.push(stock);
                } else {
                    self.items()[index].Code(newItem.Code);
                    self.items()[index].Unit(newItem.Unit);
                    self.items()[index].Price(newItem.Price);
                    self.items()[index].AvailableAmount(newItem.AvailableAmount);
                }

                updateUserWallet(newItem.Code, newItem.Unit, newItem.Price);
            });
        }
    };
    //-------------------------------------------------
    function updateUserWallet(StockCode, Unit, Price) {
        var tableRowOfStock = document.querySelector('tr[data-name="stock-code"][data-value="' + StockCode + '"]');

        if (tableRowOfStock === null) {
            return;
        }

        var priceEl = tableRowOfStock.querySelector('td[data-name="price"]');
        var unitEl = tableRowOfStock.querySelector('td[data-name="unit"]');
        var amountEl = tableRowOfStock.querySelector('td[data-name="amount"]');
        var valueEl = tableRowOfStock.querySelector('td[data-name="total-value"]');

        priceEl.innerHTML = Price;
        unitEl.innerHTML = Unit;
        valueEl.innerHTML = (amountEl.innerHTML / Unit * Price).toFixed(4);
    }
    //-------------------------------------------------
    function init() {
        stocksTable = document.querySelector("#stocksTable");
        walletTable = document.querySelector("#walletTable");
        stocksTableUpdateTimeEl = document.querySelector("#stocksTableUpdateTime");

        stocksArray = new StocksArray();
        ko.applyBindings(stocksArray);

        return stocksHub.server.getAllStocks()
            .done(function (stocks) {
                stocksArray.updateItems(stocks);
            });
    }

    $(function () {
        $.connection.hub.logging = false;
        $.connection.hub.start()
            .then(init)
            .then(function () {
                return stocksHub.server.getMarketState();
            }).done(function (state) {
                if (state === "Opened") {
                    stocksHub.client.marketOpened();
                } else {
                    stocksHub.client.marketClosed();
                }
            });
    });
    //-------------------------------------------------
})();