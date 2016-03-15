using System;
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
        private readonly string[] segments = { ".text", ".reginfo", ".data" };
        private string textSegment;
        private string dataSegment;
        public Dictionary<uint, string> Code { get; private set; }

        public MipsProgrammer(MipsCore core)
        {
            if (core == null) throw new ArgumentNullException("core");
            this.core = core;
            textSegment = "";
            dataSegment = "";
            rgx = new Regex("((?<= )([0-9]|[a-f])+(?=\\:{1}))|(?<=([0-9]|[a-f])*:\t*)([0-9]|[a-f]){8}", RegexOptions.IgnoreCase);
            //rgxCode = new Regex("((?<=  )([0-9]|[a-f])+(?=\\:{1}))|(?<=([0-9]|[a-f]){8}( *\t))([a-z]|[A-Z]|[0-9]|\t|\x20|\\(|\\)|-|\x2C|\x3C|\x3E)*", RegexOptions.IgnoreCase);
            rgxCode = new Regex("((?<= )([0-9]|[a-f])+(?=\\:{1}))|(?<=([0-9]|[a-f]){8}( *\t|\t))([a-z]|[A-Z]|[0-9]|\t|\x20|\\(|\\)|-|\x2C)*", RegexOptions.IgnoreCase);
            Code = new Dictionary<uint, string>();
        }

        public void ProgramObjdump(string path)
        {
            if (path == null) throw new ArgumentNullException("path");
            ExtractSegments(path);

            var textMatch = rgx.Matches(textSegment);
            var dataMatch = rgx.Matches(dataSegment);
            var codeMatch = rgxCode.Matches(textSegment);

            if (textMatch.Count / 2 * 4 >= core.InstructionMemory.SizeBytes)
                throw new IndexOutOfRangeException(".text segment is greater than " + core.InstructionMemory.SizeBytes + ".");
            if(dataMatch.Count / 2 * 4 >= core.DataMemory.SizeBytes)
                throw new IndexOutOfRangeException(".data segment is greater than " + core.DataMemory.SizeBytes + ".");

            ProgramRegex(textMatch, core.InstructionMemory);
            ProgramRegex(dataMatch, core.DataMemory);
            ProgramRegex(codeMatch, Code);
        }

        private void ExtractSegments(string path)
        {
            var strCode = System.IO.File.ReadAllText(path);

            textSegment = GetSegment(strCode, ".text");
            if (textSegment == "") throw new ArgumentException("The file " + path + "does not contain a .text segment");
            dataSegment = GetSegment(strCode, ".data");
        }

        private void ProgramRegex(MatchCollection match, IMemory memory)
        {
            uint counter = 0;
            uint address = 0;
            foreach (var stringMatch in from Match codeMatch in match select codeMatch.Value)
            {
                if (counter % 2 == 0)
                    address = Convert.ToUInt32(stringMatch, 16);
                else
                {
                    var instruction = Convert.ToUInt32(stringMatch, 16);
                    memory.WriteWord(new Word(instruction), address);
                }
                counter++;
            }
        }

        private void ProgramRegex(MatchCollection match, IDictionary<uint, string> dict)
        {
            uint counter = 0;
            uint address = 0;
            dict.Clear();
            foreach (var stringMatch in from Match codeMatch in match select codeMatch.Value)
            {
                if (counter%2 == 0)
                    address = Convert.ToUInt32(stringMatch, 16);
                else
                {
                    var code = stringMatch.Replace('\t', ' ');
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
                var tempSegmentEnd = file.IndexOf(s, segmentStart, StringComparison.Ordinal);
                if ((tempSegmentEnd > 0) && (tempSegmentEnd > segmentStart) && (tempSegmentEnd < segmentEnd))
                    segmentEnd = tempSegmentEnd;
            }
            return file.Substring(segmentStart, segmentEnd - segmentStart);
        }
    }
}
