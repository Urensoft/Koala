using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;


namespace Koala
{


    public class Compiler
    {

        Koala.Logic klogic               = new Koala.Logic();
        private Queue<Task>     runQueue = new Queue<Task>();
        private Stack<DataTypes.kData> varRefStack = new Stack<DataTypes.kData>();
        private DataTypes.kData currentRef = null;


        public void execute(string[] input)
        {
            int lineNumber = 0;
            foreach( string line in input)
            {
                lineNumber++;
                foreach (KeyValuePair<string,Regex> logicCombo in klogic.parseList )
                {
                    Match m = logicCombo.Value.Match(line);

                    if (!String.IsNullOrEmpty(m.Value))                 // found logic statement
                    {
                        string[] values     = m.Value.Split(' ');
                        string param1, param2;

                        switch (logicCombo.Key)
                        {
                            case "loadStatement":
                                DataTypes.kData var1        = DataTypes.Convert.stringToDataType(values[2]);
                                var1.location               = DataTypes.Convert.stringToLocation( values[5] );
                                klogic.loadBytesForDataType(var1);
                                //varRefStack.Push(var1);
                                currentRef                  = var1;
                                break;

                            case "storeStatement":
                                // Dequeue the last referenced object and store it in Koala.Logic.Memory
                                param1 = values[3].Replace(".","").Replace(",","");
                                //klogic.storeObjectAsString(varRefStack.Pop(), param1);
                                klogic.storeObjectAsString(currentRef, param1);
                                break;

                            case "repeatStatement":
                                UInt64 amount;
                                if (!UInt64.TryParse(values[2], out amount)) Error.raiseException("Invalid Number Entered");
                                klogic.repeatTaskTimes(amount);
                                break;

                            case "withStatement":
                                param1      = values[1].Replace(",","").Replace(".","");
                                currentRef  = klogic.memory[param1];

                                break;

                            case "printStatement":
                                string property     = values[2];
                                string name         = values[4].Replace(",", "").Replace(".", "");
                                DataTypes.kData val = klogic.memory[name];
                                klogic.printOut(property, val);
                                break;

                            case "getStatement":
                                param1 = values[2];
                                //klogic.getThe(param1, varRefStack.Pop());
                                currentRef = klogic.getThe(param1, currentRef,true);

                                break;
                            case "saveStatement":
                                param1  = values[1];

                                param2  = DataTypes.Convert.stringToPath( values[3] ); 
                                if(param1 == "this")    klogic.saveToFile(param2, currentRef);
                                else                    klogic.saveToFile(param2, klogic.memory[param2]);

                                break;


                        }


                    }

                }

            }
            
        }
        

    }
}
