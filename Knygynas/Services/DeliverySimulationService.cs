using Microsoft.EntityFrameworkCore;
using Knygynas.Data;
using Knygynas.Models;

namespace Knygynas.Services;

public class DeliverySimulationService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;
    private readonly ILogger<DeliverySimulationService> _logger;

    public DeliverySimulationService(
        IServiceProvider serviceProvider,
        IConfiguration configuration,
        ILogger<DeliverySimulationService> logger)
    {
        _serviceProvider = serviceProvider;
        _configuration = configuration;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Delivery Simulation Service is starting.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var emailService = scope.ServiceProvider.GetRequiredService<EmailService>();

                    // Fetch active orders (not finished, returned, or cancelled)
                    var activeOrders = await context.Orders
                        .Include(o => o.OrderItems)
                        .Where(o => o.State != OrderState.Finished 
                                 && o.State != OrderState.Returned 
                                 && o.State != OrderState.Cancelled)
                        .ToListAsync(stoppingToken);

                    if (activeOrders.Any())
                    {
                        var speed = _configuration["DeliverySimulation:Speed"] ?? "Quick";
                        _logger.LogInformation("Processing {Count} active orders for delivery simulation (Speed: {Speed}).", activeOrders.Count, speed);

                        foreach (var order in activeOrders)
                        {
                            var elapsedSeconds = (DateTime.UtcNow - order.OrderDate).TotalSeconds;
                            OrderState targetState = order.State;

                            if (speed.Equals("Normal", StringComparison.OrdinalIgnoreCase))
                            {
                                // Normal Mode: Processing -> Prepared (1m/60s) -> Shipped (2m/120s) -> Finished (3m/180s)
                                if (elapsedSeconds >= 180) targetState = OrderState.Finished;
                                else if (elapsedSeconds >= 120) targetState = OrderState.Shipped;
                                else if (elapsedSeconds >= 60) targetState = OrderState.Prepared;
                                else targetState = OrderState.Processing;
                            }
                            else
                            {
                                // Quick Mode: Processing -> Prepared (10s) -> Shipped (20s) -> Finished (30s)
                                if (elapsedSeconds >= 30) targetState = OrderState.Finished;
                                else if (elapsedSeconds >= 20) targetState = OrderState.Shipped;
                                else if (elapsedSeconds >= 10) targetState = OrderState.Prepared;
                                else targetState = OrderState.Processing;
                            }

                            if (order.State != targetState)
                            {
                                var oldState = order.State;
                                order.State = targetState;
                                
                                _logger.LogInformation("Transitioning Order #{OrderId} from {OldState} to {NewState}.", order.Id, oldState, targetState);
                                
                                await context.SaveChangesAsync(stoppingToken);

                                // Fire off email notification about the state change
                                if (!string.IsNullOrEmpty(order.UserId))
                                {
                                    await emailService.SendOrderStatusUpdateEmail(order.UserId, order);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during delivery simulation loop.");
            }

            // Check every 2 seconds
            await Task.Delay(2000, stoppingToken);
        }

        _logger.LogInformation("Delivery Simulation Service is stopping.");
    }
}
