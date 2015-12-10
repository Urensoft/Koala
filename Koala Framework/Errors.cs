using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Koala
{
    public class Error
    {
        public static int currentLineNumber = 0;

        public static byte[] errorCodes = { 97, 101, 102, 110, 111, 114, 115, 116, 117, 119, 202, 228 };

        public static void raiseException(string error)
        {
            throw new Exception(error);

        }
    }
}
