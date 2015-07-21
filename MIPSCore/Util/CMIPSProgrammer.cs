using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        Regex rgx;
        public CMIPSProgrammer(CCore core)
        {
            this.core = core;
            textSegment = "";
            dataSegment = "";
            rgx = new Regex("((?<=  )([0-9]|[a-f])+(?=\\:{1}))|(?<=([0-9]|[a-f])*:\t*)([0-9]|[a-f]){8}", RegexOptions.IgnoreCase);
        }

        public void programObjdump(string path)
        {
            if (path == null)
                throw new ArgumentNullException(this.GetType().Name + ": Argument path is null");
            extractSegments(path);

            MatchCollection textMatch = rgx.Matches(textSegment);
            MatchCollection dataMatch = rgx.Matches(dataSegment);

            if (textMatch.Count / 2 * 4 >= core.getInstructionMemory.sizeBytes)
                throw new IndexOutOfRangeException(this.GetType().Name + ".text segment is greater than " + core.getInstructionMemory.sizeBytes + ".");
            if(dataMatch.Count / 2 * 4 >= core.getDataMemory.sizeBytes)
                throw new IndexOutOfRangeException(this.GetType().Name + ".data segment is greater than " + core.getDataMemory.sizeBytes + ".");

            programRegex(textMatch, core.getInstructionMemory);
            programRegex(dataMatch, core.getDataMemory);
            
        }

        public void extractSegments(string path)
        {
            string strCode = System.IO.File.ReadAllText(path);
            int textSegmentStart = strCode.IndexOf(".text:");
            int dataSegmentStart = strCode.IndexOf(".data:");

            if (textSegmentStart == -1)
                throw new ArgumentException("The file " + path + "does not contain a .text segment");

            if (dataSegmentStart == -1)
                textSegment = strCode;
            else if (dataSegmentStart > textSegmentStart)
            {
                textSegment = strCode.Substring(textSegmentStart, dataSegmentStart - textSegmentStart);
                dataSegment = strCode.Substring(dataSegmentStart);
            }
            else
            {
                textSegment = strCode.Substring(dataSegmentStart, textSegmentStart - dataSegmentStart);
                dataSegment = strCode.Substring(textSegmentStart);
            }
        }

        public void programRegex(MatchCollection match, IMemory memory)
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
    }
}
