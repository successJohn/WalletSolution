using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Drawing;
using WalletSolution.Infrastructure.Persistence;
using WalletSolution.Utils.interfaces;

namespace WalletSolution.Utils.BackgroundJobService
{
    public class WalletUpdateBackgroundJob: CronjobService
    {
        private readonly ILogger<WalletUpdateBackgroundJob> _logger;
        private IServiceScopeFactory _scopeFactory;
        private readonly WalletDbContext _walletDbContext;

        public WalletUpdateBackgroundJob(ILogger<WalletUpdateBackgroundJob> logger, IServiceScopeFactory scopeFactory,
            IScheduleConfig<WalletUpdateBackgroundJob> config, WalletDbContext walletDbContext) : base(config.CronExpression, config.TimeZoneInfo)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
            _walletDbContext = walletDbContext;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Daily spooling of transaction to reporting has started");
            return base.StartAsync(cancellationToken);
        }

        public override Task DoWork(CancellationToken cancellationToken)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                UpdateTransactions();

            }
            _logger.LogInformation($"{DateTime.Now:hh:mm:ss} Daily transaction spool is working.");
            return Task.CompletedTask;
        }

        private void UpdateTransactions()
        {


            var query = "Declare @Rowcount INT = 1" +
                "WHILE (@Rowcount > 0)" +
                "UPDATE TOP (100000)" +
                "SET Balance = Balance * 0.1 " +
                "SET @Rowcount = @@ROWCOUNT";

           
            _walletDbContext.NGNWallets.FromSqlRaw(query);
        }
        

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            return base.StopAsync(cancellationToken);
        }
    }
}
