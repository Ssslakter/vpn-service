using System.Diagnostics;
using System.ServiceProcess;

namespace VpnService;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;

    private readonly List<string> _programs = new()
        { "clion64", "dataspell64", "pycharm64", "datagrip64", "rider64", "Code" };

    private const string VpnService = "OVPNConnectorService";

    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var serviceController = new ServiceController(VpnService);
            var active = _programs.Where(IsProgramRunning).ToList();

            if (active.Any() && serviceController.Status == ServiceControllerStatus.Stopped)
            {
                _logger.LogInformation("Starting VPN...");
                _logger.LogInformation("Active programs: {Join}", string.Join(", ", active));
                serviceController.Start();
                _logger.LogInformation("VPN started");
            }
            else if (!active.Any() && serviceController.Status == ServiceControllerStatus.Running)
            {
                _logger.LogInformation("Stopping VPN...");
                serviceController.Stop();
                _logger.LogInformation("VPN stopped");
            }

            await Task.Delay(700, stoppingToken);
        }
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        var serviceController = new ServiceController(VpnService);
        if (serviceController.Status != ServiceControllerStatus.Running) return base.StopAsync(cancellationToken);
        _logger.LogInformation("Shutting down. Stopping VPN...");
        serviceController.Stop();
        return base.StopAsync(cancellationToken);
    }

    private static bool IsProgramRunning(string programName)
    {
        var processes = Process.GetProcessesByName(programName);
        return processes.Length > 0;
    }
}