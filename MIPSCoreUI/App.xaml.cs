using System;
using System.IO;
using System.Linq;
using System.Windows;
using MIPSCore.Util.MIPSEventArgs;
using MipsCore = MIPSCore.MipsCore;

namespace MIPSCoreUI
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App
    {
        private void OnStartup(object sender, StartupEventArgs e)
        {
            if (TryRunSmokeMode(e.Args))
                return;

            var mainWindow = new MainWindow();
            MainWindow = mainWindow;
            mainWindow.Show();
        }

        private bool TryRunSmokeMode(string[] args)
        {
            if (args == null || args.Length == 0 || !args.Contains("--smoke-asm"))
                return false;

            var smokeArgIndex = Array.IndexOf(args, "--smoke-asm");
            if (smokeArgIndex < 0 || smokeArgIndex + 1 >= args.Length)
            {
                ShutdownWithCode(2, "Fehlendes Argument: --smoke-asm <Pfad-zur-Assemblerdatei>");
                return true;
            }

            var asmFilePath = args[smokeArgIndex + 1];
            if (!File.Exists(asmFilePath))
            {
                ShutdownWithCode(2, $"Assembler-Datei nicht gefunden: {asmFilePath}");
                return true;
            }

            string errorMessage = null;

            try
            {
                var core = new MipsCore();
                core.Exception += (source, eventArgs) =>
                {
                    var mipsArgs = eventArgs as MIPSEventArgs;
                    errorMessage = mipsArgs != null ? mipsArgs.Message : eventArgs.ToString();
                };

                core.ProgramAssembler(asmFilePath);
            }
            catch (Exception exception)
            {
                errorMessage = exception.ToString();
            }

            if (!string.IsNullOrEmpty(errorMessage))
            {
                ShutdownWithCode(1, errorMessage);
                return true;
            }

            ShutdownWithCode(0, null);
            return true;
        }

        private void ShutdownWithCode(int exitCode, string errorMessage)
        {
            if (!string.IsNullOrEmpty(errorMessage))
            {
                Console.Error.WriteLine(errorMessage);
            }

            Environment.ExitCode = exitCode;
            Shutdown();
        }
    }
}
