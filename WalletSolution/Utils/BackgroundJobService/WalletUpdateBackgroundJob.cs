using System.Diagnostics.Tracing;
using WalletSolution.Utils.interfaces;

namespace WalletSolution.Utils.BackgroundJobService
{
    public class WalletUpdateBackgroundJob: CronjobService
    {
        private readonly ILogger<WalletUpdateBackgroundJob> _logger;
        private IServiceScopeFactory _scopeFactory;

        public WalletUpdateBackgroundJob(ILogger<WalletUpdateBackgroundJob> logger, IServiceScopeFactory scopeFactory,
            IScheduleConfig<WalletUpdateBackgroundJob> config) : base(config.CronExpression, config.TimeZoneInfo)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
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
               // UpdateTransactions();

            }
            _logger.LogInformation($"{DateTime.Now:hh:mm:ss} Daily transaction spool is working.");
            return Task.CompletedTask;
        }

        private void UpdateTransactions() { }
        

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            return base.StopAsync(cancellationToken);
        }
    }
}
