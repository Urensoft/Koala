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
        private Queue<DataTypes.kData> varRefQueue = new Queue<DataTypes.kData>();


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

                        switch (logicCombo.Key)
                        {
                            case "loadStatement":
                                
                                DataTypes.kData var1        = DataTypes.Convert.convertStringToDataType(values[2]);
                                var1.location               = new string[] { values[5] };
                                klogic.loadTheDataFoundIn(var1, values[4]);
                                varRefQueue.Enqueue(var1);
                                break;
                            case "storeStatement":
                                // Dequeue the last referenced object and store it in Koala.Logic.Memory
                                klogic.storeObjectAsString(varRefQueue.Dequeue(), values[3]);
                                break;
                            case "repeatStatement":
                                UInt64 amount;
                                if (!UInt64.TryParse(values[2], out amount)) Error.raiseException("Invalid Number Entered");
                                klogic.repeatTaskTimes(amount);
                                break;
                            case "withStatement":

                                varRefQueue.Enqueue(new DataTypes.kData() );

                                break;

                            case "writeStatement":

                                Console.WriteLine( varRefQueue.Dequeue() );

                                break;
                        }


                    }
                }
            }
            
        }
        

    }
}
