using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
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
using Microsoft.Extensions.Hosting.Internal;
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

            var configuration = BuildConfiguration(args, options, out var environment);
            options.Environment = environment;

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

            if (options.Insecure) {
                Console.Error.WriteLine("!!! Enabling Insecure Login !!!");
                Startup.Insecure = true;
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

        public static IConfigurationRoot BuildConfiguration(String[] args, CommonOptions options, out String environment) {
            var configurationBasePath =
                GetAbsolute(options?.ConfigPath) ?? AppContext.BaseDirectory;

            environment =
                options?.Environment ??
                Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ??
                Environments.Development;
            var configuration =
                new ConfigurationBuilder()
                    .SetBasePath(configurationBasePath)
                    .AddJsonFile("appsettings.json", optional: false)
                    .AddJsonFile($"appsettings.{environment}.json", optional: false)
                    .AddEnvironmentVariables()
                    .AddCommandLine(args);

            /*
             * if this is the development environment then manually include the user secrets for the
             * runtime and the unit tests
             */
            if(environment == Environments.Development)
            {
                configuration.AddUserSecrets("d72db2b5-597e-4bc0-a92d-a033bdf5ac7e");
            }

            return configuration.Build();
        }

        private static String GetAbsolute(String path) {
            return !String.IsNullOrWhiteSpace(path) ? Path.GetFullPath(path) : null;
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
