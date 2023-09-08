using Serilog;
using VpnService;

var programData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
Log.Logger = new LoggerConfiguration().WriteTo.Console().WriteTo.File(Path.Combine(programData, "SlakterService/vpn-service.log"))
    .CreateLogger();

var host = Host.CreateDefaultBuilder(args)
    .UseSerilog()
    .UseWindowsService()
    .ConfigureServices(services => { services.AddHostedService<Worker>(); })
    .Build();

await host.RunAsync();