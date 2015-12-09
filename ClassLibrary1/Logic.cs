using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Koala.DataTypes;

namespace Koala
{
    public class Logic
    {

        public Dictionary<string, object> memory = new Dictionary<string, object>();

        string[] inputs = new string[]
        {
            "object",
            "string"
        };

        public Dictionary<string, Regex> parseList = new Dictionary<string, Regex>()
        {
            {   "loadStatement"     , new Regex("load the .+ found (in|at) \".+\"(,|.)",RegexOptions.IgnoreCase)},
            {   "storeStatement"    , new Regex(@"store (this|these|them|those) as .+(,|.)",RegexOptions.IgnoreCase)               },
            {   "repeatStatement"   , new Regex(@"repeat \d+ (time|times)",RegexOptions.IgnoreCase)             },
            {   "withStatement"     , new Regex(@"with .+,",RegexOptions.IgnoreCase)                            },
            {   "printStatement"    , new Regex(@"print out \d+ (time|times)",RegexOptions.IgnoreCase)          },

        };


        string[] logicStatements = new string[] {   "load the [object] found in [filepath],",
                                                    "store [object] as [string]",
                                                    "With ",
                                                    "Repeat [integer] times",
                                                    "found in [folder]",
                                                    "found at [filepath" };
        string[] mathStatements = new string[] { "Squareroot of ", "Square of", "and call it", "With " };

        public object loadTheDataFoundIn(DataTypes.kData obj, string path)
        {

            return null;
        }

        public void storeObjectAsString(object obj, string name)
        {
            memory.Add(name, obj);
        }

        public void repeatTaskTimes(UInt64 times)
        {


        }



    }
}
