using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DAMS.Interface;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using DAMS.Interface;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public class JobsScheduler : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly TimeSpan _scheduledTime; // The specific time to run the scheduler.

    public JobsScheduler(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _scheduledTime = new TimeSpan(18, 16, 0); // Set the desired time (11:00 AM).
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var currentTime = DateTime.Now.TimeOfDay;
                var delay = CalculateDelay(currentTime);

                Console.WriteLine($"Next execution in: {delay.TotalSeconds} seconds.");

                // Wait until the scheduled time
                await Task.Delay(delay, stoppingToken);

                using (var scope = _serviceProvider.CreateScope())
                {
                    var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

                    List<string> receivers = new List<string> { "adityasawant5814@gmail.com" };
                    await emailService.SendEmailAsync(receivers, "Scheduler Test", "Testing email from JobsScheduler", null);

                    Console.WriteLine($"Email sent successfully at {DateTime.Now}.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred in JobsScheduler: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// Calculates the delay needed to wait until the next scheduled time.
    /// </summary>
    /// <param name="currentTime">The current time of day.</param>
    /// <returns>The delay until the next scheduled time.</returns>
    private TimeSpan CalculateDelay(TimeSpan currentTime)
    {
        if (currentTime > _scheduledTime)
        {
            // If the current time is past the scheduled time, schedule it for the next day.
            return (TimeSpan.FromHours(24) - currentTime) + _scheduledTime;
        }
        else
        {
            // Schedule it for today.
            return _scheduledTime - currentTime;
        }
    }
}

