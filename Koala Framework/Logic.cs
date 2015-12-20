using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Koala.DataTypes;
using System.IO;
using System.Drawing;

namespace Koala
{
    public class Logic
    {
        public DataTypes.Convert converter;

        public Logic()
        {
            converter = new DataTypes.Convert(ref memory, ref privateMemory);

        }

        public Dictionary<string, object> privateMemory     = new Dictionary<string, object>();
        public Dictionary<string, kData> memory             = new Dictionary<string, kData>();
        public Dictionary<string, kFunction> functionList = new Dictionary<string, kFunction>();


        public int taskProgress = 0;

        string[] inputs = new string[]
        {
            "object",
            "string"
        };

        public List<Regex> operatorList = new List<Regex>() {

            new Regex(@"\([^\(\)]+\)"),
            new Regex(@"[a-zA-Z0-9\.]+\^[a-zA-Z0-9\.]+"),
            new Regex(@"[a-zA-Z0-9\.]+\/[a-zA-Z0-9\.]+"),
            new Regex(@"[a-zA-Z0-9\.]+\*[a-zA-Z0-9\.]+"),
            new Regex(@"[a-zA-Z0-9\.]+\%[a-zA-Z0-9\.]+"),
            new Regex(@"[a-zA-Z0-9\.]+\+[a-zA-Z0-9\.]+"),
            new Regex(@"[a-zA-Z0-9\.]+\-[a-zA-Z0-9\.]+"),


        };




        public Dictionary<string, Regex> parseList = new Dictionary<string, Regex>()
        {
            {   "withStatement"     , new Regex(@"with .+\,",RegexOptions.IgnoreCase)                                                   },
            {   "whichStatement"      , new Regex(@"which (are|is): .+(\,|\.)",RegexOptions.IgnoreCase)    },

            {   "loadStatement"     , new Regex("load the .+ found (in|at) \".+\"(\\,|\\.)",RegexOptions.IgnoreCase)                    },
            {   "storeStatement"    , new Regex(@"store (this|these|them|those) as .+(\,|\.)",RegexOptions.IgnoreCase)                  },
            {   "repeatStatement"   , new Regex(@"repeat \d+ (time|times)(\,|\.)",RegexOptions.IgnoreCase)                              },
            {   "printStatement"    , new Regex(@"print out .+\,",RegexOptions.IgnoreCase)                                              },
            {   "createStatement"   , new Regex(@"create (the|an|a) (integer|number|string|variable) ,",RegexOptions.IgnoreCase)        },
            {   "saveStatement"     , new Regex("save .+ to \".+\"(\\,|\\.)",RegexOptions.IgnoreCase)                                   },
            {   "performStatement"  , new Regex(@"(perform|execute|run) the function [^ ]+ on [^ ]+(\,|\.)",RegexOptions.IgnoreCase)                                   },


            //image
            {   "getStatement"      , new Regex(@"get the .+(\,|\.)",RegexOptions.IgnoreCase)    },

        };



        List<Qualifier> qualifiers = new List<Qualifier>();


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

        public void scanForFunctions(string[] input)
        {
            string allData = String.Join("\n", input);

            Regex definitions = new Regex(@"(define the function [^,.]+ as:)(([ \n])*(f\([^,]+,))+", RegexOptions.IgnoreCase);
            MatchCollection m = definitions.Matches(allData);

            foreach(Match match in m)
            {
                int index       = match.Value.IndexOf(':');
                string fname    = Regex.Replace(match.Value.Substring(0, index+1), @"(define the function )|( as:)",  "", RegexOptions.IgnoreCase);

                string funcValues           = match.Value.Substring(index, match.Value.Length - index);
                MatchCollection funcs       = Regex.Matches(funcValues, @"((f\([^,]+,))+");

                kFunction item = new kFunction();

                foreach ( Match f in funcs)
                {
                    item.expressions.Add(f.Value);
                }
                functionList[fname] = item;

            }






        }
        public kData handleInputVariable(string i)
        {
            long iValue         = 0;
            double dValue       = 0.0;
            bool parsedDouble   = false;
            bool parsedInteger  = false;


            if (i.Contains('.')) parsedDouble = double.TryParse(i, out dValue);
            else parsedInteger = long.TryParse(i, out iValue);

            if (parsedDouble || parsedInteger)
            {
                if (parsedDouble)   return new Number(dValue);
                else                return new Number(iValue);
            }
            else
            {
                string obj = i.Replace(" ", "");
                if (memory.ContainsKey(obj)) return memory[obj];
                else // could be string
                {

                }
            }
            return null;
        }

        public void setQualifier(string s)
        {
            qualifiers = new List<Qualifier>();
            Regex q1 = new Regex(@"(less than )|\<"             , RegexOptions.IgnoreCase);
            Regex q2 = new Regex(@"(greater than )|\>"          , RegexOptions.IgnoreCase);
            Regex q3 = new Regex(@"((equals)|(equal to) )|\="   , RegexOptions.IgnoreCase);


            string[] list = splice(s);

            foreach (string qualifier in list)
            {
                Match m1 = q1.Match(qualifier);  // LESS THAN
                if (m1.Success)
                {
                    kData data = handleInputVariable(q1.Replace(qualifier, ""));
                    qualifiers.Add(new Qualifier(null, "<", data));
                    continue;
                }
                Match m2 = q2.Match(qualifier); // GREATER THAN
                if (m2.Success)
                {
                    kData data = handleInputVariable(q2.Replace(qualifier, ""));
                    qualifiers.Add(new Qualifier(null, ">", data));
                    continue;
                }
                Match m3 = q3.Match(qualifier);
                if (m3.Success)
                {
                    kData data = handleInputVariable(q3.Replace(qualifier, ""));
                    qualifiers.Add(new Qualifier(null, "=", data));
                    continue;
                }

            }

        }

        public string[] splice(string a)
        {
            Regex r = new Regex(@"(and)|\,|\&", RegexOptions.IgnoreCase);

            return r.Split(a);
        }


       
        public string evaluateExpression(string expression)
        {
            expression          = expression.Replace(" ", "").Replace(",", "");
            string output       = ""; 

            for(int r = 0; r<operatorList.Count; r++)
            {
                Regex rx            = operatorList[r];
                Match m             = rx.Match(expression);

                while (m.Success)
                {
                    switch (r)
                    {
                        case 0:             //0 brackets
                            output = evaluateExpression(m.Value.Replace("(", "").Replace(")", ""));
                            break;
                        case 1:             //1 orders
                            output = ALU.Power(DataTypes.Convert.convertStringsToValues(m.Value.Split('^'))     ).ToString();
                            break;
                        case 2:             //2 division
                            output = ALU.Divide(DataTypes.Convert.convertStringsToValues(m.Value.Split('/'))    ).ToString();
                            break;
                        case 3:             //3 multiplication
                            output = ALU.Multiply(DataTypes.Convert.convertStringsToValues(m.Value.Split('*'))  ).ToString();
                            break;
                        case 4:             //4 modulo
                            output = ALU.Modulo(DataTypes.Convert.convertStringsToValues(m.Value.Split('%'))    ).ToString();
                            break;
                        case 5:             //5 addition
                            output = ALU.Add(DataTypes.Convert.convertStringsToValues(m.Value.Split('+'))       ).ToString();
                            break;
                        case 6:             //6 subtraction
                            output = ALU.Subtract(DataTypes.Convert.convertStringsToValues(m.Value.Split('-'))  ).ToString();
                            break;
                    }
                    if (!String.IsNullOrEmpty(output))
                    {
                        expression = expression.Replace(m.Value, output);
                        double testOut;
                        if (!double.TryParse(output, out testOut)) output = "(" + output + ")";

                    }
                    m = rx.Match(expression);
                }

            }
            //check for pronumeral

            MatchCollection mc  = Regex.Matches(expression, @"[a-zA-Z0-9_]+");
            
            foreach ( Match m in mc)
            {
                object a =DataTypes.Convert.stringToObject(m.Value);
                if(a != null)
                {
                    expression = expression.Replace( m.Value,a.ToString() );
                }
            }


            return expression;
        }

        public bool valuePassesQualifier(Number value)
        {
            bool logicalAnd = true;

            if (qualifiers.Count == 0) return true;
            else
            {
                List<bool> results = new List<bool>();

                foreach (Qualifier q in qualifiers)
                {
                    bool result = false;
                    switch (q.operation)
                    {
                        case ">":
                            result  = value.isGreaterThan(q.var2);
                            break;

                        case "<":
                            result  = value.isLessThan(q.var2);
                            break;

                        case "=":
                            result  = value.isEqualTo(q.var2);
                            break;
                            /*case "<=":
                                return value.isEqualTo(q.var2);
                            case ">=":
                                break;*/
                    }

                    if (logicalAnd && result == false) return false;
                    else results.Add(result);
                }

                if (results.Contains(true))  return true;
                else                         return false;
            }
        }

        public kData getThe(string property, kData obj, bool evaluate)
        {
            switch (property)
            {
                case "pixels":
                    return getThePixels(obj,null);
                    break;

                case "bytes":
                    kData newData = new kData();
                    newData.bytes = new byte[obj.bytes.Length];
                    if (evaluate)
                    {
                        for (UInt32 i = 0; i < obj.bytes.Length; i++)
                        {
                            byte b = obj.bytes[i];
                            if (valuePassesQualifier(new Number((long)b))) newData.bytes[i] = b;
                            else newData.bytes[i] = 0;
                        }
                    }

                    return newData;

                case "rows":

                    return getTheRow(obj);
            }

            Koala.Error.raiseException("Unknown property " + property);
            return null;

        }

        public kData getThePixels(kData obj, kFunction func)
        {

            Bitmap bmp = new Bitmap(new MemoryStream(obj.bytes));
            Bitmap outBmp = new Bitmap(bmp.Width, bmp.Height);

            for (int y = 0; y < bmp.Height; y++)
            {
                taskProgress = (int)(((float)y / bmp.Height)*100.0);
                for (int x = 0; x < bmp.Width; x++)
                {
                    Color pix = bmp.GetPixel(x, y);


                    if (func == null)
                    {
                        bool a = valuePassesQualifier(new Number((long)pix.R));
                        bool b = valuePassesQualifier(new Number((long)pix.G));
                        bool c = valuePassesQualifier(new Number((long)pix.B));

                        if (a && b && c) outBmp.SetPixel(x, y, pix);
                        else outBmp.SetPixel(x, y, Color.Black);
                    }
                    else
                    {

                        privateMemory["r"] = (long)pix.R;
                        privateMemory["g"] = (long)pix.G;
                        privateMemory["b"] = (long)pix.B;
                        privateMemory["a"] = (long)pix.A;

                        foreach (string e in func.expressions)
                        {
                            string[] values     = e.Split('=');
                            string result       = evaluateExpression(values[1]);
                            string name         = Regex.Replace(values[0], @"f\(|\) *","", RegexOptions.IgnoreCase).ToLower();
                            privateMemory[name] = DataTypes.Convert.stringToObject( result );
                        }
                        long r = (long)privateMemory["r"];
                        long g = (long)privateMemory["g"];
                        long b = (long)privateMemory["b"];
                        long a = (long)privateMemory["a"];


                        outBmp.SetPixel(x, y, Color.FromArgb((int)a,(int)r,(int)g,(int)b));

                    }

                }
            }

            return new DataTypes.Image(outBmp);
        }

        public kData getTheRow(kData obj)
        {

            string[] rows;
            if (string.IsNullOrEmpty(obj.location))
            {
                string result = System.Text.Encoding.UTF8.GetString(obj.bytes);
                rows = Regex.Split(result, "(\n)|(\r)|(\r\n)");

            }
            else rows = System.IO.File.ReadAllLines(obj.location);

            string outString = "";
        

            long rowCount = 0;
            foreach (string row in rows)
            {
                rowCount++;
                if (valuePassesQualifier(new Number(rowCount)))  outString += row;
            }

            kData output = new kData();
            output.bytes = Encoding.UTF8.GetBytes(outString);
            return output;
        }

        public kData performFunction(string functionName, kData target, string properties)
        {
            kData result = null;
            if (functionList.ContainsKey(functionName))
            {
                kFunction f = functionList[functionName];

                switch (properties)
                {
                    case "pixels":
                        return getThePixels(target, f);
                        break;

                }

                foreach (string e in f.expressions)
                {
                }
            }
            else
            {
                Koala.Error.raiseException("call to an undefined function: " + functionName);
            }
            return result;
            
        }


        public void handleExpression(string expression)
        {
            string[] operations = Regex.Split(expression, @"(,|( and ))");

            foreach (string a in operations)
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

        public string[] findProperties(string s)
        {
            Regex propertyRx = new Regex(@"[^ ]+ of [^ \,\.]+[^ \,\.]", RegexOptions.IgnoreCase);
            Regex varRx = new Regex(@"(( and)|\,) [^ \,]+", RegexOptions.IgnoreCase);


            List<string> found = new List<string>();
            string filtered = Regex.Replace(s, @"(print out )", "", RegexOptions.IgnoreCase);
            MatchCollection pmc = propertyRx.Matches(filtered);
            MatchCollection vmc = varRx.Matches(filtered);

            if (filtered.IndexOf(" ") < 0) found.Add(filtered.Replace(",", ""));

            foreach (Match m in pmc)
            {
                found.Add(m.Value);
            }
            foreach (Match m in vmc)
            {
                string variable = Regex.Replace(m.Value, @"(( and )|\,|\.)", "", RegexOptions.IgnoreCase);
                found.Add(variable);
            }
            return found.ToArray();

        }

        public void printOut(string[] values)
        {


            foreach (string value in values)
            {
                int ofIndex = value.IndexOf("of", StringComparison.OrdinalIgnoreCase);
                string objName = null;
                string property = null;

                if (ofIndex >= 0)    // split into property and name
                {
                    property = value.Substring(0, ofIndex).Replace(" ", "");
                    objName = value.Substring(ofIndex + 2, value.Length - ofIndex - 2).Replace(" ", "");
                }
                else
                {                       // just name of variable
                    objName = value.Replace(" ", "");
                }

                if (memory.ContainsKey(objName))
                {
                    if (String.IsNullOrEmpty(property)) // is variable but no property specified, should print overview
                    {
                        Console.WriteLine(DataTypes.Convert.objectToString(memory[objName]));
                    }
                    else
                    {
                        Console.WriteLine(DataTypes.Convert.propertyOfObjectToString(property.ToLower(), memory[objName]));
                    }

                }
                else
                {
                    if (String.IsNullOrEmpty(property)) // not a property and not a variable, static text to be printed
                    {
                        Console.WriteLine(objName);
                    }
                    else
                    {
                        Koala.Error.raiseException(objName + " does not contain the property of " + property);
                    }

                }
            }

        }

        // FILE IO

        public void saveToFile(string path, kData obj)
        {
            if (obj.GetType() == typeof(DataTypes.Image))
            {
                DataTypes.Image img = (DataTypes.Image)obj;
                img.bmpRepresentation.Save(path);
            }
            else
            {
                System.IO.File.WriteAllBytes(path, obj.bytes);

            }

            Console.WriteLine("Data saved to \"" + path +'\"');
        }



    }

    public class Qualifier {

        public Qualifier(kData v1,string op,kData v2)
        {
            //var1 = v1;
            operation = op;
            if(v2.GetType() == typeof(Number)) var2 = (Number)v2;
        }

        public Number var1
        {
            get;
            set;
        }

        public string operation
        {
            get;
            set;
        }
        public Number var2
        {
            get;
            set;
        }


    }

    public static class ALU
    {
       /* public bool isNumeric(string n)
        {
            if (n.Contains('.'))
            {
                double val;
                return double.TryParse(n, out val);
            }
            else
            {
                long val;
                return long.TryParse(n, out val);
            }

        }*/

        public static object Add(object[] v)
        {
            object valueA = v[0];
            object valueB = v[1];

            Type a = valueA.GetType();
            if (a == typeof(double))
            {
                if(valueB.GetType() == a)   return (double)valueA + (double)valueB;     // double double
                else                        return (double)valueA + (long)valueB;       // double long
            }
            else
            {
                if (valueB.GetType() == a)  return (long)valueA + (long)valueB;         // long long
                else                        return (long)valueA + (double)valueB;       // long double
            }                                               

        }

        public static object Subtract(object[] v)
        {
            object valueA = v[0];
            object valueB = v[1];

            Type a = valueA.GetType();
            if (a == typeof(double))
            {
                if ( valueB.GetType() == a)     return (double)valueA - (double)valueB;     // double double
                else                            return (double)valueA - (long)valueB;       // double long
            }
            else
            {
                if (valueB.GetType() == a)      return (long)valueA - (long)valueB;         // long long
                else                            return (long)valueA - (double)valueB;       // long double
            }



        }
        public static object Multiply(object[] v)
        {
            object valueA = v[0];
            object valueB = v[1];

            Type a = valueA.GetType();
            if (a == typeof(double))
            { 
                if (valueB.GetType() == a)      return (double)valueA * (double)valueB;     // double double
                else                            return (double)valueA * (long)valueB;       // double long
            }
            else
            {
                if (valueB.GetType() == a)      return (long)valueA * (long)valueB;         // long long
                else                            return (long)valueA * (double)valueB;       // long double
            }


        }
        public static object Divide(object[] v)
        {
            object valueA = v[0];
            object valueB = v[1];

            Type a = valueA.GetType();
            if (a == typeof(double))
            {
                if (valueB.GetType() == a)   return (double)valueA / (double)valueB;     // double double
                else                         return (double)valueA / (long)valueB;       // double long
            }
            else
            {
                if (valueB.GetType() == a)  return (long)valueA / (long)valueB;         // long long
                else                        return (long)valueA / (double)valueB;       // long double
            }

        }
        public static object Modulo(object[] v)
        {
            object valueA = v[0];
            object valueB = v[1];

            Type a = valueA.GetType();
            if (a == typeof(double))
            { 
                if (a == typeof(double) && valueB.GetType() == a)   return (double)valueA % (double)valueB;     // double double
                else                                                return (double)valueA % (long)valueB;       // double long
            }
            else
            {
                if (a == typeof(long) && valueB.GetType() == a)     return (long)valueA % (long)valueB;         // long long
                else                                                return (long)valueA % (double)valueB;       // long double
            }
        }
           
        public static object Power(object[] v)
        {
            object valueA = v[0];
            object valueB = v[1];
            Type a = valueA.GetType();
            if (a == typeof(double))
            {
                if (valueB.GetType() == a)      return Math.Pow( (double)valueA , (double)valueB);     // double double
                else                            return Math.Pow( (double)valueA , (long)valueB);       // double long
            }
            else
            {
                if (valueB.GetType() == a)      return Math.Pow( (long)valueA , (long)valueB );         // long long
                else                            return Math.Pow( (long)valueA , (double)valueB );       // long double
            }
        }
    }

}
