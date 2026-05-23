using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.ComponentModel;

namespace MIPSCore.Util
{
    class Compiler
    {
        private readonly string gccPath;
        private readonly string gccAssemblerPath;
        private readonly string gccLinkerPath;
        private readonly string gccObjdumpPath;
        private readonly string linkerScriptPath;

        public Compiler() 
        {
            var compilerRootPath = ResolveCompilerRootPath();
            gccPath = Path.Combine(compilerRootPath, "bin", "mips-linux-gnu-gcc.exe");
            gccAssemblerPath = Path.Combine(compilerRootPath, "bin", "mips-linux-gnu-as.exe");
            gccLinkerPath = Path.Combine(compilerRootPath, "bin", "mips-linux-gnu-ld.exe");
            gccObjdumpPath = Path.Combine(compilerRootPath, "bin", "mips-linux-gnu-objdump.exe");
            linkerScriptPath = Path.Combine(compilerRootPath, "MIPSCore.ld");

            if (!File.Exists(gccPath))
                throw new FileNotFoundException($"Compiler nicht gefunden: {gccPath}");
            if (!File.Exists(gccAssemblerPath))
                throw new FileNotFoundException($"Assembler nicht gefunden: {gccAssemblerPath}");
            if (!File.Exists(gccLinkerPath))
                throw new FileNotFoundException($"Linker nicht gefunden: {gccLinkerPath}");
            if (!File.Exists(gccObjdumpPath))
                throw new FileNotFoundException($"Objectdump nicht gefunden: {gccObjdumpPath}");
            if (!File.Exists(linkerScriptPath))
                throw new FileNotFoundException($"Linker-Script nicht gefunden: {linkerScriptPath}");
        }
        public string Compile(string[] files)
        {
            if(files == null)
                return "";

            var filenames = GetFilenames(files);

            //compile
            Process p = new Process();
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.FileName = gccPath;
            p.StartInfo.Arguments = GenerateCompileArguments(files);
            p.Start();
     
            string error = p.StandardError.ReadToEnd();
            string output = p.StandardOutput.ReadToEnd();
            
            p.WaitForExit();
            var exit = p.ExitCode;

            //link
            p.StartInfo.FileName = gccLinkerPath;
            p.StartInfo.Arguments = GenerateLinkArguments(filenames);
            p.Start();

            error = p.StandardError.ReadToEnd();
            output = p.StandardOutput.ReadToEnd();

            p.WaitForExit();
            exit = p.ExitCode;

            //objdump
            p.StartInfo.FileName = gccObjdumpPath;
            p.StartInfo.Arguments = "-D exe";
            p.Start();

            output = p.StandardOutput.ReadToEnd();

            p.WaitForExit();
            exit = p.ExitCode;

            //delete files
            File.Delete("exe");
            foreach (var s in filenames)
                File.Delete(s + ".o");

            //write to tmp file
            StreamWriter file = new System.IO.StreamWriter("tmp.txt");
            file.WriteLine(output);
            file.Close();
            return output;
        }

        public string CompileAssembler(string[] asmFiles)
        {
            if (asmFiles == null || asmFiles.Length == 0)
            {
                return "Keine Assembler-Dateien angegeben.";
            }
                

            var filenames = GetFilenames(asmFiles);
            string error, output;
            int exit;

            Process p = new Process();
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.UseShellExecute = false;

            // **Kompilieren von Assembler-Dateien**
            foreach (var asmFile in asmFiles)
            {
                p.StartInfo.FileName = GCCAssemblerPath;

                p.StartInfo.Arguments = $"--trap -mips32 -O0 -o {Path.GetFileNameWithoutExtension(asmFile)}.o \"{asmFile}\"";
                p.Start();

                error = p.StandardError.ReadToEnd();
                output = p.StandardOutput.ReadToEnd();

                p.WaitForExit();
                exit = p.ExitCode;
                if (exit != 0)
                    throw new Exception($"Fehler beim Kompilieren der Assembler-Datei {asmFile}:\n{error}");
            }

            // **Linken der Assembler-Objektdateien**
            p = new Process();
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.FileName = gccLinkerPath;
            p.StartInfo.Arguments = GenerateLinkArguments(filenames);
            p.Start();

            error = p.StandardError.ReadToEnd();
            output = p.StandardOutput.ReadToEnd();

            p.WaitForExit();
            exit = p.ExitCode;
            if (exit != 0)
                return $"Fehler beim Linken der Assembler-Dateien:\n{error}";

            // **Objdump zur Disassemblierung**
            p = new Process();
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.FileName = gccObjdumpPath;
            p.StartInfo.Arguments = "-D exe";
            p.Start();

            output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();
            exit = p.ExitCode;
            if (exit != 0)
                return $"Fehler bei objdump:\n{error}";

            // **Temporäre Dateien löschen**
            File.Delete("exe");
            foreach (var s in filenames)
                File.Delete(s + ".o");

            // **Disassembly in temporäre Datei speichern**
            File.WriteAllText("tmp.txt", output);

            return output;
        }


        private string GenerateCompileArguments(string[] files)
        {
            string result = "-c ";
            foreach (var s in files)
                result += "\"" + s + "\"" + " ";
            return result;
        }

        private string GenerateLinkArguments(string[] filenames)
        {
            string result = $"-T \"{linkerScriptPath}\" -o exe ";
            foreach (var s in filenames)
                result += s + ".o ";
            return result;
        }

        private string ResolveCompilerRootPath()
        {
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var candidates = new[]
            {
                Path.Combine(baseDirectory, "Compiler"),
                Path.Combine(Directory.GetCurrentDirectory(), "Compiler"),
                Path.Combine(baseDirectory, "..", "Compiler"),
                Path.Combine(baseDirectory, "..", "..", "Compiler")
            }
            .Select(Path.GetFullPath)
            .Distinct();

            foreach (var candidate in candidates)
            {
                if (File.Exists(Path.Combine(candidate, "bin", "mips-linux-gnu-gcc.exe")) &&
                    File.Exists(Path.Combine(candidate, "MIPSCore.ld")))
                {
                    return candidate;
                }
            }

            throw new FileNotFoundException(
                "Compiler-Ordner nicht gefunden. Geprüfte Pfade: " + string.Join(", ", candidates));
        }

        private string[] GetFilenames(string[] files)
        {
            var result = new string[files.Count()];
            for (int i = 0; i < files.Count(); i++)
            {
                result[i] = Path.GetFileNameWithoutExtension(files[i]);
            }
            return result;
        }
    }
}
