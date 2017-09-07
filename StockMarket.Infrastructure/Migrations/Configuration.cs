namespace StockMarket.Infrastructure.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using StockMarket.Core.Models;
    using StockMarket.Infrastructure.Context;
    using StockMarket.Infrastructure.Extensions;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<StockMarketDbContext>
    {
        private StockMarketDbContext _context;

        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(StockMarketDbContext context)
        {
            //if (System.Diagnostics.Debugger.IsAttached == false)
            //{
            //    System.Diagnostics.Debugger.Launch();
            //}

            _context = context;

            SeedStocks();
            SeedRoles();
            SeedUsers();
            SeedUserStocks();
        }

        private void SeedStocks()
        {
            var publicationDate = DateTime.Now.AddSeconds(-90);

            var stocks = new List<Stock>
            {
                new Stock
                {
                    Name = "House Stark",
                    Code = "STARK",
                    AvailableAmount = 10000,
                    Values = new List<StockValue>
                    {
                        new StockValue { Unit = 1, Price = 6M, PublicationDate = publicationDate },
                        new StockValue { Unit = 1, Price = 6.025M, PublicationDate = publicationDate.AddSeconds(30) },
                        new StockValue { Unit = 1, Price = 6.05M, PublicationDate = publicationDate.AddSeconds(60) },
                        new StockValue { Unit = 1, Price = 6.075M, PublicationDate = publicationDate.AddSeconds(90) },
                    }
                },
                new Stock
                {
                    Name = "House Targaryen",
                    Code = "TARGARYEN",
                    AvailableAmount = 80000,
                    Values = new List<StockValue>
                    {
                        new StockValue { Unit = 1, Price = 5M, PublicationDate = publicationDate },
                        new StockValue { Unit = 1, Price = 6.0512M, PublicationDate = publicationDate.AddSeconds(30) },
                        new StockValue { Unit = 1, Price = 7.0429M, PublicationDate = publicationDate.AddSeconds(60) },
                        new StockValue { Unit = 1, Price = 10.11M, PublicationDate = publicationDate.AddSeconds(90) },
                    }
                },
                new Stock
                {
                    Name = "House Lannister",
                    Code = "LANNISTER",
                    AvailableAmount = 10000,
                    Values = new List<StockValue>
                    {
                        new StockValue { Unit = 1, Price = 9M, PublicationDate = publicationDate },
                        new StockValue { Unit = 1, Price = 9.05M, PublicationDate = publicationDate.AddSeconds(30) },
                        new StockValue { Unit = 1, Price = 9.09M, PublicationDate = publicationDate.AddSeconds(60) },
                        new StockValue { Unit = 1, Price = 9.5M, PublicationDate = publicationDate.AddSeconds(90) }
                    }
                },
                new Stock
                {
                    Name = "House Greyjoy ",
                    Code = "GREYJOY",
                    AvailableAmount = 2000,
                    Values = new List<StockValue>
                    {
                        new StockValue { Unit = 5, Price = 6.07M, PublicationDate = publicationDate },
                        new StockValue { Unit = 5, Price = 6.04M, PublicationDate = publicationDate.AddSeconds(30) },
                        new StockValue { Unit = 5, Price = 6.012M, PublicationDate = publicationDate.AddSeconds(60) },
                        new StockValue { Unit = 5, Price = 6.041M, PublicationDate = publicationDate.AddSeconds(90) },
                    }
                },
                new Stock
                {
                    Name = "Night King",
                    Code = "NKING",
                    AvailableAmount = 20000,
                    Values = new List<StockValue>
                    {
                        new StockValue { Unit = 10, Price = 5.0734M, PublicationDate = publicationDate },
                        new StockValue { Unit = 10, Price = 6.07327M, PublicationDate = publicationDate.AddSeconds(30) },
                        new StockValue { Unit = 10, Price = 6.142M, PublicationDate = publicationDate.AddSeconds(60) },
                        new StockValue { Unit = 10, Price = 7.053M, PublicationDate = publicationDate.AddSeconds(90) },
                    }
                },
                new Stock
                {
                    Name = "Nights Watch",
                    Code = "NWATCH",
                    AvailableAmount = 2000,
                    Values = new List<StockValue>
                    {
                        new StockValue { Unit = 100, Price = 5.021M, PublicationDate = publicationDate },
                        new StockValue { Unit = 100, Price = 5.0127M, PublicationDate = publicationDate.AddSeconds(30) },
                        new StockValue { Unit = 100, Price = 5.04356M, PublicationDate = publicationDate.AddSeconds(60) },
                        new StockValue { Unit = 100, Price = 5.1237M, PublicationDate = publicationDate.AddSeconds(90) },
                    }
                }
            };

            stocks.ForEach(item => _context.Stock.AddOrUpdate(item));
            _context.SaveChanges();
        }
        
        private void SeedRoles()
        {
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(_context));

            if (!roleManager.RoleExists("User"))
            {
                var role = new IdentityRole { Name = "User" };
                roleManager.Create(role);
            }

            if (!roleManager.RoleExists("Admin"))
            {
                var role = new IdentityRole { Name = "Admin" };
                roleManager.Create(role);
            }

            _context.SaveChanges();
        }

        private void SeedUsers()
        {
            var userStore = new UserStore<User>(_context);
            var userManager = new UserManager<User>(userStore);
            var userRoleName = "User";
            var firstUsername = "daniel@niepodam.pl";
            var secondUsername = "tester@niepodam.pl";
            var userPassword = "Niepodam1";

            if (!_context.UserApp.Any(u => u.UserName == firstUsername))
            {
                var user = new User { UserName = firstUsername, Money = 100000, Email = firstUsername };
                var userResult = userManager.Create(user, password: userPassword);
                if (userResult.Succeeded)
                {
                    userManager.AddToRole(user.Id, userRoleName);
                    _context.SaveChanges();
                }
            }

            if (!_context.UserApp.Any(u => u.UserName == secondUsername))
            {
                var user = new User { UserName = secondUsername, Money = 100000, Email = secondUsername };
                var userResult = userManager.Create(user, password: userPassword);
                if (userResult.Succeeded)
                {
                    userManager.AddToRole(user.Id, userRoleName);
                    _context.SaveChanges();
                }
            }
        }

        private void SeedUserStocks()
        {
            var firstUserId = _context.Users.FirstOrDefault(u => u.UserName == "daniel@niepodam.pl").Id;
            var secondUserId = _context.Users.FirstOrDefault(u => u.UserName == "tester@niepodam.pl").Id;
            var stocks = _context.StockValue
                .AsNoTracking()
                .Where(x => x.PublicationDate == _context.StockValue.Select(y => y.PublicationDate).Max())
                .ToList();

            var random = new Random();
            var minUnit = 10;
            var maxUnit = 100;

            var userStocks = new List<UserStocks>();

            foreach (var stock in stocks)
            {
                userStocks.Add(new UserStocks
                {
                    UserID = firstUserId,
                    StockID = stock.StockID,
                    Amount = stock.Unit * random.Next(minUnit, maxUnit)
                });

                userStocks.Add(new UserStocks
                {
                    UserID = secondUserId,
                    StockID = stock.StockID,
                    Amount = stock.Unit * random.Next(minUnit, maxUnit)
                });
            }

            userStocks.ForEach(item => _context.Set<UserStocks>().AddIfNotExists(
                item,
                x => x.UserID == item.UserID && x.StockID == item.StockID));
            _context.SaveChanges();
        }
    }
}
