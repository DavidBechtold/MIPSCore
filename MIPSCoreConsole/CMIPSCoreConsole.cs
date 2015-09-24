using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MIPSCore;

namespace MIPSCoreConsole
{
    class CMIPSCoreConsole
    {
        private static IMipsCore core;
        private static UInt16 registerReadNumber;

        /* Commands */
        private static CMIPSCoreConsoleCommand clock;
        private static CMIPSCoreConsoleCommand readAllRegister;
        private static CMIPSCoreConsoleCommand readRegister;
        private static CMIPSCoreConsoleCommand readRegisterUnsigned;
        private static CMIPSCoreConsoleCommand readRegisterHex;
        private static CMIPSCoreConsoleCommand controlSignals;
        private static CMIPSCoreConsoleCommand usage;

        static void Main(string[] args)
        {
            /* reads config file and inits all komponents */
            core = new MipsCore();
            if (!serveStartArguments(args))
                return;

            /* install event handler */
            core.Completed += new EventHandler(completed);
            core.Clocked += new EventHandler(clocked);
            core.Exception += new EventHandler(exception);

            /* init commands */
            clock = new CMIPSCoreConsoleCommand("clock");
            readAllRegister = new CMIPSCoreConsoleCommand("rregAll");
            readRegister = new CMIPSCoreConsoleCommand("rreg");
            readRegisterUnsigned = new CMIPSCoreConsoleCommand("rregU");
            readRegisterHex = new CMIPSCoreConsoleCommand("rregH");
            controlSignals = new CMIPSCoreConsoleCommand("control");
            usage = new CMIPSCoreConsoleCommand("usage");

            core.StartCore();
            
            serveCommandLineArguments();
        }

        static void completed(object obj, EventArgs args)
        {
            Console.WriteLine("Completed");
        }

        static void clocked(object obj, EventArgs args)
        {
            Console.WriteLine("ProgramCounter:\t" + core.ProgramCounter());
            Console.WriteLine("CInstruction:\t" + core.ActualInstruction() + "\n");
        }

        static void exception(object obj, EventArgs args)
        {
            Console.WriteLine(core.GetExceptionString());
        }

        static public bool serveStartArguments(string[] args)
        {
            if (args[0] == "-h" || args[0] == "--h" || args[0] == "-help" || args[0] == "--help")
                Console.WriteLine(usageStartArguments(null));

            if (args.Length < 2)
            {
                Console.WriteLine(usageStartArguments("Too little arguments " + args.Length + "."));
                return false;
            }

            if(args[0] != "-p")
            {
                Console.WriteLine(usageStartArguments("Wrong first argument " + args[0] + "."));
                return false;
            }

            if(!System.IO.File.Exists(args[1]))
            {
                Console.WriteLine(usageStartArguments("File \"" + args[1] + "\" doesn't exist."));
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
                        Console.WriteLine(usageStartArguments("Optional argument " + args[i] + " doesn't exist."));
                        return false;
                }
            }

            return true;
        }

        static public string usageStartArguments(string error)
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

        static void serveCommandLineArguments()
        {
            while(true)
            {
                string rawCmd = Console.ReadLine();
                string[] cmd = rawCmd.Split(' ');

                for (int i = 0; i < cmd.Length; i++)
                {
                    cmd[i] = cmd[i].ToLower();
                    if (clock.ToString() == cmd[i])
                        core.SingleClock();
                    else if (readAllRegister.ToString() == cmd[i])
                        Console.WriteLine(core.ToStringAllRegisters());
                    else if (readRegister.ToString() == cmd[i])
                    {
                        if (checkAndGetRegisterNumber(cmd[i++], cmd[i]))
                            Console.WriteLine(core.ToStringRegister(registerReadNumber));
                    }
                    else if (readRegisterUnsigned.ToString() == cmd[i])
                    {
                        if (checkAndGetRegisterNumber(cmd[i++], cmd[i]))
                            Console.WriteLine(core.ToStringRegisterUnsigned(registerReadNumber));
                    }
                    else if (readRegisterHex.ToString() == cmd[i])
                    {
                        if (checkAndGetRegisterNumber(cmd[i++], cmd[i]))
                            Console.WriteLine(core.ToStringRegisterHex(registerReadNumber));
                    }
                    else if (controlSignals.ToString() == cmd[i])
                        Console.WriteLine(core.ReadControlUnitSignals());
                    else if (usage.ToString() == cmd[i])
                        Console.WriteLine(usageCommandLineArguments(null));
                    else
                        Console.WriteLine(usageCommandLineArguments("Unkown command " + cmd[i] + ".\n"));
                }
            }
        }

        static bool checkAndGetRegisterNumber(string cmd, string arg)
        {
            try
            {
                registerReadNumber = Convert.ToUInt16(arg);
            }
            catch
            {
                Console.WriteLine(usageCommandLineArguments("Argument " + arg + " of command " + cmd + " can not be converted to a number.\n"));
                return false;
            }

            if (registerReadNumber >= MipsCore.RegisterCount)
            {
                string error = String.Format("Argument {0} of command {1} must be between 0 and {2}\n", arg, cmd, MipsCore.RegisterCount - 1);
                Console.WriteLine(usageCommandLineArguments(error));
                return false;
            }

            return true;
        }

        static public string usageCommandLineArguments(string error)
        {
            string usageString = "";

            if(error != null)
                usageString += error + "\n\n";

            usageString += "MIPSCoreConsole <command> [args]\n";
            usageString += "------------------------------\n";
            usageString += usage.ToString() + "\t\tShows this usage message.\n";
            usageString += clock.ToString() + "\t\tIn stepping mode step one instruction further.\n";
            usageString += readAllRegister.ToString() + "\t\tReads all registers\n";
            usageString += readRegister.ToString() + " <number>\tRead register value signed decimal.\n";
            usageString += readRegisterUnsigned.ToString() + " <number>\tRead register value unsigned decimal.\n";
            usageString += readRegisterHex.ToString() + " <number>\tRead register value hexadecimal.\n";
            usageString += controlSignals.ToString() + "\t\tPrints the control units signals.\n";

            return usageString;
        }
    }

    public class CMIPSCoreConsoleCommand
    {
        private readonly string command;
        public CMIPSCoreConsoleCommand(string cmd)
        {
            command = cmd;
        }

        public string getCommand
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
