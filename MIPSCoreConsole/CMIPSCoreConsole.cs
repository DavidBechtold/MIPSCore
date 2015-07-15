using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MIPSCore.Core;

namespace MIPSCoreConsole
{
    class CMIPSCoreConsole
    {
        private static ICore core;
        private static UInt16 registerReadNumber;
        static void Main(string[] args)
        {
            /* reads config file and inits all komponents */
            core = new CCore();
            if (!serveStartArguments(args))
                return;

            /* install event handler */
            core.completed += new EventHandler(completed);
            core.clocked += new EventHandler(clocked);

            core.startCore();
            
            serveCommandLineArguments();
        }

        static void completed(object obj, EventArgs args)
        {
            Console.WriteLine("Completed");
        }

        static void clocked(object obj, EventArgs args)
        {
            Console.WriteLine("ProgramCounter:\t" + core.programCounter());
            Console.WriteLine("Instruction:\t" + core.actualInstruction() + "\n");
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

            core.programObjdump(args[1]);

            for (int i = 2; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "-s":
                        core.setMode(ExecutionMode.singleStep);
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

                for(int i = 0; i < cmd.Length; i++)
                    switch (cmd[i])
                    {
                        case "step":
                            core.singleStep();
                            break;

                        case "rreg":
                            if (!checkAndGetRegisterNumber(cmd[i++], cmd[i]))
                                break;
                            Console.WriteLine(core.readRegister(registerReadNumber));
                            break;

                        case "rregU":
                            if (!checkAndGetRegisterNumber(cmd[i++], cmd[i]))
                                break;
                            Console.WriteLine(core.readRegisterUnsigned(registerReadNumber));
                            break;

                        case "rregH":
                            if (!checkAndGetRegisterNumber(cmd[i++], cmd[i]))
                                break;
                            Console.WriteLine(core.readRegisterHex(registerReadNumber));
                            break;

                        case "control":
                            Console.WriteLine(core.readControlUnitSignals());
                            break;

                        default:
                        case "usage":
                            Console.WriteLine(usageCommandLineArguments(null));
                            break;    
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

            if (registerReadNumber > CCore.RegisterCount)
            {
                Console.WriteLine(usageCommandLineArguments("Argument " + arg + " of command " + cmd + " must be between 0 and " + CCore.RegisterCount + ".\n"));
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
            usageString += "usage\t\tShows this usage message.\n";
            usageString += "step\t\tIn stepping mode step one instruction further.\n";
            usageString += "rreg <number>\tRead register value signed decimal.\n";
            usageString += "rregU <number>\tRead register value unsigned decimal.\n";
            usageString += "rregH <number>\tRead register value hexadecimal.\n";
            usageString += "control\t\tPrints the control units signals.\n";

           

            return usageString;
        }
    }
}
