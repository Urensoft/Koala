using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Koala.DataTypes;
using System.IO;

namespace Koala
{
    public class Logic
    {

        public Dictionary<string, kData> memory = new Dictionary<string, kData>();

        string[] inputs = new string[]
        {
            "object",
            "string"
        };

        public Dictionary<string, Regex> parseList = new Dictionary<string, Regex>()
        {
            {   "loadStatement"     , new Regex("load the .+ found (in|at) \".+\"(,|.)",RegexOptions.IgnoreCase)                    },
            {   "storeStatement"    , new Regex(@"store (this|these|them|those) as .+(,|.)",RegexOptions.IgnoreCase)                },
            {   "repeatStatement"   , new Regex(@"repeat \d+ (time|times)(,|.)",RegexOptions.IgnoreCase)                            },
            {   "withStatement"     , new Regex(@"with .+,",RegexOptions.IgnoreCase)                                                },
            {   "printStatement"    , new Regex(@"print out .+ of .+,",RegexOptions.IgnoreCase)                                     },
            {   "createStatement"   , new Regex(@"create (the|an|a) (integer|number|string|variable) ,",RegexOptions.IgnoreCase)    },
            {   "saveStatement"     , new Regex("save .+ to \".+\"(,|.)",RegexOptions.IgnoreCase)    },

            //image
            {   "getStatement"      , new Regex(@"get the .+ which (are|is) .+(,|.)",RegexOptions.IgnoreCase)    },
            {   "whichStatement"      , new Regex(@"white are .+ which (are|is) .+(,|.)",RegexOptions.IgnoreCase)    },




        };


        string[] logicStatements = new string[] {   "load the [object] found in [filepath],",
                                                    "store [object] as [string]",
                                                    "With ",
                                                    "Repeat [integer] times",
                                                    "found in [folder]",
                                                    "found at [filepath" };
        string[] mathStatements = new string[] { "Squareroot of ", "Square of", "and call it", "With " };

        public void loadBytesForDataType(DataTypes.kData obj)
        {
            FileAttributes attr = System.IO.File.GetAttributes(obj.location);
            if (attr.HasFlag(FileAttributes.Directory))
            {

            }
            else
            {
                if (!System.IO.File.Exists(obj.location)) Koala.Error.raiseException("Item at " + obj.location + " is neither File nor Directory");
                else
                {
                    obj.bytes = System.IO.File.ReadAllBytes(obj.location);
                    //obj.dataStream = new System.IO.FileStream(obj.location, FileMode.Open, FileAccess.Read);

                }
            }
        }

        public void setQualifier(string s)
        {


        }

        public bool valuePassesQualifier()
        {
            return true;
        }

        public kData getThe(string property, kData obj,bool evaluate)
        {
            
            switch (property)
            {
                case "pixel":

                    break;

                case "bytes":
                    kData newData = new kData();
                    newData.bytes = new byte[obj.bytes.Length];
                    if (evaluate)
                    {
                        for (UInt32 i = 0; i<obj.bytes.Length; i++)
                        {
                            byte b = obj.bytes[i];
                            if (valuePassesQualifier())     newData.bytes[i] = b;
                            else                            newData.bytes[i] = 0;
                        }
                    }
                    

                    return newData;

                break;
            }


            Koala.Error.raiseException("Unknown property " + property);
            return null;

        }



        public void handleExpression(string expression)
        {

            string[] operations = Regex.Split(expression, @"(,|( and ))");

            foreach(string a in operations)
            {


            }


        }

        public void storeObjectAsString(DataTypes.kData obj, string name)
        {
            memory[name] = obj;
        }

        public void repeatTaskTimes(UInt64 times)
        {


        }

        public void printOut(string property,kData obj)
        {


            switch (property)
            {
                case "location":
                    Console.WriteLine(obj.location);
                    break;
            }


        }

        // FILE IO

        public void saveToFile(string path, kData obj)
        {

            System.IO.File.WriteAllBytes(path, obj.bytes);

        }



    }
}
