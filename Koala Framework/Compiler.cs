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

        public Koala.Logic klogic               = new Koala.Logic();
        private Queue<Task>     runQueue = new Queue<Task>();
        private Stack<DataTypes.kData> varRefStack = new Stack<DataTypes.kData>();
        private DataTypes.kData currentRef = null;

        public string currentTask = "";

        public void execute(string[] input)
        {
            Koala.Error.currentLineNumber = 0;
            klogic.scanForFunctions(input);
            foreach( string line in input)
            {
                klogic.taskProgress = 0;

                Koala.Error.currentLineNumber++;
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
                                string[] properties = klogic.findProperties(m.Value);
                                klogic.printOut(properties);

                                break;

                            case "getStatement":

                                handleWhichQualifier(m.Value);
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
                            case "performStatement":
                                string name     = values[3];
                                currentTask = "Performing function " + name;

                                string property = "";
                                Match res       = new Regex(" on [^ ]+,",RegexOptions.IgnoreCase).Match(m.Value);
                                if(res.Success) property = res.Value.Substring(4, res.Value.Length - 5);
                                currentRef = klogic.performFunction(name, currentRef, property);


                                break;


                        }


                    }

                }

            }
            
        }
        
        public void handleWhichQualifier(string q)
        {
            Regex r = klogic.parseList["whichStatement"];

            Match m = r.Match(q);
            if (!String.IsNullOrEmpty(m.Value))
            {
                string qualifier = m.Value.Split(':')[1];
                klogic.setQualifier(qualifier);
            }
            

        }
    }
}
