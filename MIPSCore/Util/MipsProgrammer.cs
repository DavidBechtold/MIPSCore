using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using MIPSCore.Util._Memory;

namespace MIPSCore.Util
{
    public class MipsProgrammer
    {
        private readonly MipsCore core;
        private readonly Regex rgx;
        private readonly Regex rgxCode;
        private readonly string[] segments = { ".text", ".rodata", ".data", ".sdata", ".pdr", ".sbss", ".comment", ".reginfo"};
        private string textSegment;
        private string dataSegment;
        public Dictionary<uint, string> Code { get; private set; }
        public uint TextSegmentEndAddress;
        public uint DataSegmentEndAddress;

        public MipsProgrammer(MipsCore core)
        {
            if (core == null) throw new ArgumentNullException("core");
            this.core = core;
            textSegment = "";
            dataSegment = "";
            TextSegmentEndAddress = 0;
            DataSegmentEndAddress = 0;
            rgx = new Regex("((?<= )([0-9]|[a-f])+(?=\\:{1}))|(?<=([0-9]|[a-f])+:\\s+)([0-9]|[a-f]){8}", RegexOptions.IgnorePatternWhitespace);
            rgxCode = new Regex("((?<= )([0-9]|[a-f])+(?=\\:{1}))|(?<=([0-9]|[a-f])+:\\s+([0-9]|[a-f]){8}\\s+)([a-z]|[A-Z]|[0-9]|\t|\x20|\\(|\\)|,|-|\x2C)*", RegexOptions.IgnoreCase);
            Code = new Dictionary<uint, string>();
        }

        public void Program(string code)
        {
            if (code == null) throw new ArgumentNullException("code");
            ExtractSegments(code);

            var textMatch = rgx.Matches(textSegment);
            var dataMatch = rgx.Matches(dataSegment);
            var codeMatch = rgxCode.Matches(textSegment);

            TextSegmentEndAddress = ProgramRegex(textMatch, core.InstructionMemory, MemoryType.Text);
            DataSegmentEndAddress = ProgramRegex(dataMatch, core.DataMemory, MemoryType.Data);
            ProgramRegex(codeMatch, Code);
        }

        public void ProgramObjdumpFile(string path)
        {
            if (path == null) throw new ArgumentNullException("path");
            var strCode = System.IO.File.ReadAllText(path);
            Program(strCode);
        }

        private void ExtractSegments(string code)
        {
            textSegment = GetSegment(code, ".text");
            if (textSegment == "") throw new ArgumentException("The file " + code + "does not contain a .text segment");
            dataSegment = GetSegment(code, ".data");
        }

        private enum MemoryType {Text, Data};

        private uint ProgramRegex(IEnumerable match, IMemory memory, MemoryType memoryType)
        {
            uint counter = 0;
            uint address = 0;
            foreach (var stringMatch in from Match codeMatch in match select codeMatch.Value)
            {
                if ((counter % 2) == 0)
                {
                    try { address = Convert.ToUInt32(stringMatch, 16); }
                    catch { throw new FormatException("Format Exception:\nThe loaded objectfile has a wrong format at address " + (address + 4) + "."); }
                }
                else
                {
                    var instruction = Convert.ToUInt32(stringMatch, 16);

                    if (memoryType == MemoryType.Data && address > core.DataMemory.SizeBytes)
                        throw new OutOfMemoryException("Out of Memory:\nThe .data segment of loaded program is greater than .data segment of the MIPSCore (" + core.DataMemory.SizeBytes + ").");
                    if (memoryType == MemoryType.Text && address > core.InstructionMemory.SizeBytes)
                        throw new OutOfMemoryException("Out of Memory:\nThe .text segment of loaded program is greater than .text segment of the MIPSCore (" + core.InstructionMemory.SizeBytes + ").");
                    memory.WriteWord(new Word(instruction), address);
                }
                counter++;
            }
            return address;
        }

        private static void ProgramRegex(IEnumerable match, IDictionary<uint, string> dict)
        {
            uint counter = 0;
            uint address = 0;
            dict.Clear();
            foreach (var stringMatch in from Match codeMatch in match select codeMatch.Value)
            {
                if ((counter % 2) == 0)
                {
                    try { address = Convert.ToUInt32(stringMatch, 16); }
                    catch { throw new FormatException("Format Exception:\nThe loaded objectfile has a wrong format at address " + (address + 4) + "."); }
                }
                else
                {
                    var code = Regex.Replace(stringMatch.Trim(), @"\s+", " "); //remove obsolete whitespaces
                    dict.Add(address, code);
                } 
                counter++;
            }
        }

        private string GetSegment(string file, string segment)
        {
            if (file == null) throw new ArgumentNullException("file");
            if (segment == null) throw new ArgumentNullException("segment");

            var segmentStart = file.IndexOf(segment, StringComparison.Ordinal);
            var segmentEnd = file.Length;
            if (segmentStart <= 0)
                return "";

            foreach (var s in segments)
            {
                if(segment == s)
                    continue;
                var tempSegmentEnd = file.IndexOf(s, segmentStart, StringComparison.Ordinal);
                if ((tempSegmentEnd > 0) && (tempSegmentEnd > segmentStart) && (tempSegmentEnd < segmentEnd))
                    segmentEnd = tempSegmentEnd;
            }
            return file.Substring(segmentStart, segmentEnd - segmentStart);
        }
    }
}
