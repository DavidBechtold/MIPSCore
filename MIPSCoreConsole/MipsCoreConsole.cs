using System;
using MIPSCore;

namespace MIPSCoreConsole
{
    class MipsCoreConsole
    {
        private static IMipsCore core;
        private static ushort registerReadNumber;

        /* Commands */
        private static CmipsCoreConsoleCommand clock;
        private static CmipsCoreConsoleCommand readAllRegister;
        private static CmipsCoreConsoleCommand readRegister;
        private static CmipsCoreConsoleCommand readRegisterUnsigned;
        private static CmipsCoreConsoleCommand readRegisterHex;
        private static CmipsCoreConsoleCommand controlSignals;
        private static CmipsCoreConsoleCommand usage;

        static void Main(string[] args)
        {
            if (args == null) throw new ArgumentNullException("args");

            /* reads config file and inits all komponents */
            core = new MipsCore();
            if (!ServeStartArguments(args)) return;

            /* install event handler */
            core.Completed += Completed;
            core.Clocked += Clocked;
            core.Exception += Exception;

            /* init commands */
            clock = new CmipsCoreConsoleCommand("clock");
            readAllRegister = new CmipsCoreConsoleCommand("rregAll");
            readRegister = new CmipsCoreConsoleCommand("rreg");
            readRegisterUnsigned = new CmipsCoreConsoleCommand("rregU");
            readRegisterHex = new CmipsCoreConsoleCommand("rregH");
            controlSignals = new CmipsCoreConsoleCommand("control");
            usage = new CmipsCoreConsoleCommand("usage");

            core.StartCore();
            
            ServeCommandLineArguments();
        }

        static void Completed(object obj, EventArgs args)
        {
            Console.WriteLine("Completed");
        }

        static void Clocked(object obj, EventArgs args)
        {
            Console.WriteLine("ProgramCounter:\t" + core.ProgramCounter());
            Console.WriteLine("CInstruction:\t" + core.ActualInstruction() + "\n");
        }

        static void Exception(object obj, EventArgs args)
        {
            Console.WriteLine(core.GetExceptionString());
        }

        static public bool ServeStartArguments(string[] args)
        {
            if (args[0] == "-h" || args[0] == "--h" || args[0] == "-help" || args[0] == "--help")
                Console.WriteLine(UsageStartArguments(null));

            if (args.Length < 2)
            {
                Console.WriteLine(UsageStartArguments("Too little arguments " + args.Length + "."));
                return false;
            }

            if(args[0] != "-p")
            {
                Console.WriteLine(UsageStartArguments("Wrong first argument " + args[0] + "."));
                return false;
            }

            if(!System.IO.File.Exists(args[1]))
            {
                Console.WriteLine(UsageStartArguments("File \"" + args[1] + "\" doesn't exist."));
                return false;
            }

            core.ProgramObjdump(args[1]);

            for (int i = 2; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "-s":
                        core.SetMode(ExecutionMode.SingleStep);
                        i++;
                        break;

                    default:
                        Console.WriteLine(UsageStartArguments("Optional argument " + args[i] + " doesn't exist."));
                        return false;
                }
            }

            return true;
        }

        static public string UsageStartArguments(string error)
        {
            string usageString = "";

            if (error != null)
                usageString += error + "\n\n";

            usageString += "MIPSCoreConsole -p file [args]\n";
            usageString += "------------------------------\n";
            usageString += "-p file\t\tProgramm core with the specified objdump file.\n\n";
            usageString += "Arguments:\n";
            usageString += "[-s]\t\tsingleStepMode";

            return usageString;
        }

        static void ServeCommandLineArguments()
        {
            while(true)
            {
                var rawCmd = Console.ReadLine();
                if (rawCmd == null) continue;
                var cmd = rawCmd.Split(' ');

                for (var i = 0; i < cmd.Length; i++)
                {
                    cmd[i] = cmd[i].ToLower();
                    if (clock.ToString() == cmd[i])
                        core.SingleClock();
                    else if (readAllRegister.ToString() == cmd[i])
                        Console.WriteLine(core.ToStringAllRegisters());
                    else if (readRegister.ToString() == cmd[i])
                    {
                        if (CheckAndGetRegisterNumber(cmd[i++], cmd[i]))
                            Console.WriteLine(core.ToStringRegister(registerReadNumber));
                    }
                    else if (readRegisterUnsigned.ToString() == cmd[i])
                    {
                        if (CheckAndGetRegisterNumber(cmd[i++], cmd[i]))
                            Console.WriteLine(core.ToStringRegisterUnsigned(registerReadNumber));
                    }
                    else if (readRegisterHex.ToString() == cmd[i])
                    {
                        if (CheckAndGetRegisterNumber(cmd[i++], cmd[i]))
                            Console.WriteLine(core.ToStringRegisterHex(registerReadNumber));
                    }
                    else if (controlSignals.ToString() == cmd[i])
                        Console.WriteLine(core.ReadControlUnitSignals());
                    else if (usage.ToString() == cmd[i])
                        Console.WriteLine(UsageCommandLineArguments(null));
                    else
                        Console.WriteLine(UsageCommandLineArguments("Unkown command " + cmd[i] + ".\n"));
                }
            }
            // ReSharper disable once FunctionNeverReturns
        }

        static bool CheckAndGetRegisterNumber(string cmd, string arg)
        {
            try
            {
                registerReadNumber = Convert.ToUInt16(arg);
            }
            catch
            {
                Console.WriteLine(UsageCommandLineArguments("Argument " + arg + " of command " + cmd + " can not be converted to a number.\n"));
                return false;
            }

            if (registerReadNumber >= MipsCore.RegisterCount)
            {
                string error = String.Format("Argument {0} of command {1} must be between 0 and {2}\n", arg, cmd, MipsCore.RegisterCount - 1);
                Console.WriteLine(UsageCommandLineArguments(error));
                return false;
            }

            return true;
        }

        static public string UsageCommandLineArguments(string error)
        {
            string usageString = "";

            if(error != null)
                usageString += error + "\n\n";

            usageString += "MIPSCoreConsole <command> [args]\n";
            usageString += "------------------------------\n";
            usageString += usage + "\t\tShows this usage message.\n";
            usageString += clock + "\t\tIn stepping mode step one instruction further.\n";
            usageString += readAllRegister + "\t\tReads all registers\n";
            usageString += readRegister + " <number>\tRead register value signed decimal.\n";
            usageString += readRegisterUnsigned + " <number>\tRead register value unsigned decimal.\n";
            usageString += readRegisterHex + " <number>\tRead register value hexadecimal.\n";
            usageString += controlSignals + "\t\tPrints the control units signals.\n";

            return usageString;
        }
    }

    public class CmipsCoreConsoleCommand
    {
        private readonly string command;
        public CmipsCoreConsoleCommand(string cmd)
        {
            command = cmd;
        }

        public string GetCommand
        {
            get
            {
                return command;
            }
        }

        public override string ToString()
        {
            return command.ToLower();
        }
    }
}
