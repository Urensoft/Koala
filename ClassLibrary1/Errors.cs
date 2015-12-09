using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Koala
{
    public class Error
    {
        public static void raiseException(string error)
        {
            throw new Exception(error);

        }
    }
}
