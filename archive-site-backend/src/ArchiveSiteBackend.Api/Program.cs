using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using ArchiveSiteBackend.Api.CommandLineOptions;
using ArchiveSiteBackend.Api.Commands;
using ArchiveSiteBackend.Api.Helpers;
using CommandLine;
using CommandLine.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using HostOptions = ArchiveSiteBackend.Api.CommandLineOptions.HostOptions;

namespace ArchiveSiteBackend.Api {
    public static class Program {
        private static readonly Parser CommandLineParser = new Parser(settings => {
            settings.CaseSensitive = true;
            settings.IgnoreUnknownArguments = true;
        });

        public static void Main(String[] args) {
            var commandLine =
                CommandLineParser.ParseArguments(
                    args,
                    typeof(InitializeOptions),
                    typeof(HostOptions)
                );

            commandLine
                .WithParsed<InitializeOptions>(opts => InitializeDatabase(args, opts))
                .WithParsed<HostOptions>(opts => RunWebHost(args, opts))
                .WithNotParsed(errors => { DisplayHelp(commandLine); });
        }

        private static void InitializeDatabase(String[] args, InitializeOptions options) {
            if (options.Debug) {
                WaitForDebugger();
            }

            var configuration = BuildConfiguration(args, options, out _);

            var serviceCollection = new ServiceCollection();

            var startup = new Startup(configuration);
            startup.ConfigureServices(serviceCollection, skipHosting: true);

            using var provider = serviceCollection.BuildServiceProvider();
            var command = provider.GetRequiredService<InitializeCommand>();

            // TODO: support cancellation when the user hits Ctrl+C or the process gets a kill signal
            command.InvokeAsync(options, CancellationToken.None).AwaitSynchronously();
        }

        private static void RunWebHost(String[] args, HostOptions options) {
            if (options.Debug) {
                WaitForDebugger();
            }

            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => {
                    webBuilder
                        .UseConfiguration(BuildConfiguration(args, options, out var environment))
                        .UseEnvironment(environment)
                        .UseStartup<Startup>();
                })
                .Build()
                .Run();
        }

        private static IConfigurationRoot BuildConfiguration(String[] args, CommonOptions options, out String environment) {
            // AppContext.BaseDirectory is within the ./bin/config/framework
            var configurationBasePath =
                GetAbsolute(options.ConfigPath) ?? GetParent(AppContext.BaseDirectory, 3);

            environment =
                options.Environment ??
                Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ??
                Environments.Development;
            var configuration =
                new ConfigurationBuilder()
                    .SetBasePath(configurationBasePath)
                    .AddJsonFile("appsettings.json", optional: false)
                    .AddJsonFile($"appsettings.{environment}.json", optional: false)
                    .AddEnvironmentVariables()
                    .AddCommandLine(args)
                    .Build();
            return configuration;
        }

        private static String GetAbsolute(String path) {
            return !String.IsNullOrWhiteSpace(path) ? Path.GetFullPath(path) : null;
        }

        private static String GetParent(String path, Int32 ancestor) {
            return Enumerable.Range(0, ancestor).Aggregate(path.TrimEnd('/'), (p, _) => Directory.GetParent(p).FullName);
        }

        private static void WaitForDebugger() {
            while (!Debugger.IsAttached) {
                Thread.Sleep(100);
            }
        }

        private static void DisplayHelp<T>(ParserResult<T> result) {
            var helpText = HelpText.AutoBuild(
                result,
                h => {
                    var asm = typeof(Program).Assembly;
                    h.AdditionalNewLineAfterOption = false;
                    h.Heading = $"{asm.GetName().Name} {asm.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion}";

                    return h;
                });

            Console.Error.WriteLine(helpText.ToString());
        }
    }
}
