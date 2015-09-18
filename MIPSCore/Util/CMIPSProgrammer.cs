using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using MIPSCore;
using MIPSCore.Util;
using MIPSCore.InstructionSet;

namespace MIPSCore.Util
{
    public class CMIPSProgrammer
    {
        private CCore core;
        private string textSegment;
        private string dataSegment;
        private Regex rgx;
        private Regex rgxCode;
        private string[] Segments = { ".text", ".reginfo", ".data" };
        public ArrayList Code { get; private set; }

        public CMIPSProgrammer(CCore core)
        {
            this.core = core;
            textSegment = "";
            dataSegment = "";
            rgx = new Regex("((?<=  )([0-9]|[a-f])+(?=\\:{1}))|(?<=([0-9]|[a-f])*:\t*)([0-9]|[a-f]){8}", RegexOptions.IgnoreCase);
            rgxCode = new Regex("(?<=([0-9]|[a-f]){8}( *\t))([a-z]|[A-Z]|[0-9]|\t| |,|<|>)*", RegexOptions.IgnoreCase);
            Code = new ArrayList();

        }

        public void programObjdump(string path)
        {
            if (path == null)
                throw new ArgumentNullException(this.GetType().Name + ": Argument path is null");
            extractSegments(path);

            MatchCollection textMatch = rgx.Matches(textSegment);
            MatchCollection dataMatch = rgx.Matches(dataSegment);
            MatchCollection codeMatch = rgxCode.Matches(textSegment);

            if (textMatch.Count / 2 * 4 >= core.getInstructionMemory.sizeBytes)
                throw new IndexOutOfRangeException(this.GetType().Name + ".text segment is greater than " + core.getInstructionMemory.sizeBytes + ".");
            if(dataMatch.Count / 2 * 4 >= core.getDataMemory.sizeBytes)
                throw new IndexOutOfRangeException(this.GetType().Name + ".data segment is greater than " + core.getDataMemory.sizeBytes + ".");

            programRegex(textMatch, core.getInstructionMemory);
            programRegex(dataMatch, core.getDataMemory);
            programRegex(codeMatch, Code);
        }

        private void extractSegments(string path)
        {
            string strCode = System.IO.File.ReadAllText(path);

            textSegment = getSegment(strCode, ".text");
            if (textSegment == "") throw new ArgumentException("The file " + path + "does not contain a .text segment");
            dataSegment = getSegment(strCode, ".data");
        }

        private void programRegex(MatchCollection match, IMemory memory)
        {
            UInt32 counter = 0;
            UInt32 address = 0;
            UInt32 instruction = 0;
            foreach (Match codeMatch in match)
            {
                string stringMatch = codeMatch.Value;

                if (counter % 2 == 0)
                    address = Convert.ToUInt32(stringMatch, 16);
                else
                {
                    instruction = Convert.ToUInt32(stringMatch, 16);
                    memory.writeWord(new CWord((UInt32)instruction), address);
                }
                counter++;
            }
        }

        private void programRegex(MatchCollection match, ArrayList list)
        {
            list.Clear();
            foreach (Match codeMatch in match)
            {
                string stringMatch = codeMatch.Value;
                stringMatch = stringMatch.Replace('\t', ' ');
                list.Add(stringMatch);
            }
        }

        private string getSegment(string file, string segment)
        {
            if (file == null) throw new ArgumentNullException("file");
            if (segment == null) throw new ArgumentNullException("segment");

            int segmentStart = file.IndexOf(segment);
            int segmentEnd = file.Length;
            if (segmentStart <= 0)
                return "";

            foreach (var s in Segments)
            {
                int tempSegmentEnd = file.IndexOf(s, segmentStart);
                if ((tempSegmentEnd > 0) && (tempSegmentEnd > segmentStart) && (tempSegmentEnd < segmentEnd))
                    segmentEnd = tempSegmentEnd;
            }

            return file.Substring(segmentStart, segmentEnd - segmentStart);
        }
    }
}
