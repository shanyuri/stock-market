# StockMarket

Stock Market web application written in ASP.NET MVC 5. It's based in <a href="https://docs.microsoft.com/en-us/aspnet/signalr/overview/getting-started/tutorial-server-broadcast-with-signalr">Tutorial: Server Broadcast with SignalR 2
</a> idea. In my example SignalR also takes care of sending new stock prices to all users, and Knockout.js data-binding updates user interface. Simple and nice idea.


Preview
----------
![Stock Market Preview Gif](https://raw.githubusercontent.com/shanyuri/stock-market/master/stock-market-preview.gif)


Features
--------
- User can register with starting amount of money
- User can buy stocks (amount to buy must be a multiplication of stock unit)
- User can sell stocks (amount to sell must be a multiplication of stock unit)
- Chart presenting the latest stock prices, it's updating automatically
- Real-time communiaction between server and client, so there is no need to refresh website to update stock prices table, user wallet and chart


Configuration
-----------------
To run this project in Visual Studio:
- Open "Package Manager Console"
- Select "StockMarket.Infrastructure" as "Default project"
- In "Package Manager Console" type "Update-Database" and press enter 
- Select properties of Solution "StockMarket"
- Select "Multiple startup projects:" option in "Startup Project" section
- Set "Start without debugging" Action for StockMarket.ExampleAPI project
- Set "Start" Action for StockMarket.Web project
- Configuration is done but...

...please look at settings below:
-  "AppSettings" in appsettings.json file (StockMarket.ExampleAPI project)
- "appSettings" section in Web.config file (StockMarket.Web project)
