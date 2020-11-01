using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using ArchiveSiteBackend.Api.Commands;
using ArchiveSiteBackend.Api.Helpers;
using ArchiveSiteBackend.Api.Options;
using CommandLine;
using CommandLine.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using HostOptions = ArchiveSiteBackend.Api.Options.HostOptions;

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
            var configuration = BuildConfiguration(args, options);

            var serviceCollection = new ServiceCollection();

            var startup = new Startup(configuration);
            startup.ConfigureServices(serviceCollection);

            using var provider = serviceCollection.BuildServiceProvider();
            var command = provider.GetRequiredService<InitializeCommand>();

            // TODO: support cancellation when the user hits Ctrl+C or the process gets a kill signal
            command.InvokeAsync(options, CancellationToken.None).AwaitSynchronously();
        }

        private static void RunWebHost(String[] args, HostOptions options) {
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => {
                    webBuilder
                        .UseConfiguration(BuildConfiguration(args, options))
                        .UseStartup<Startup>();
                })
                .Build()
                .Run();
        }

        private static IConfigurationRoot BuildConfiguration(String[] args, CommonOptions options) {
            // AppContext.BaseDirectory is within the ./bin/config/framework
            var configurationBasePath =
                GetAbsolute(options.ConfigPath) ?? GetParent(AppContext.BaseDirectory, 3);

            var configuration =
                new ConfigurationBuilder()
                    .SetBasePath(configurationBasePath)
                    .AddJsonFile("appsettings.json", optional: false)
                    .AddJsonFile($"appsettings.{options.Environment}.json", optional: false)
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
