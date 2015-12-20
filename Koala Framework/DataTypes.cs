using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;
using System.Reflection;
using System.Drawing;

namespace Koala
{
    namespace DataTypes
    {

        public class Convert
        {
            public static Dictionary<string, object> privateMemory = new Dictionary<string, object>();
            public static Dictionary<string, kData> memory = new Dictionary<string, kData>();

            public Convert(ref Dictionary<string, kData> mem, ref Dictionary<string, object> pmem)
            {
                privateMemory   = pmem;
                memory          = mem;

            }
   

            public static string objectToString(kData obj)
            {
                string results = obj.GetType().Name + " {\n";

                foreach (FieldInfo f in obj.GetType().GetFields().ToList())
                {
                    object val = f.GetValue(obj);
                    results += String.Format("{0}={1},\n", f.Name, val);
                    if (f.FieldType.IsArray)
                    {
                        if (f.FieldType == typeof(byte[]))
                        {
                            byte[] b = (byte[])val;
                            results += String.Format("{0}={1},\n", f.Name + ".length", b.Length);
                        }

                    }


                    // do stuff here
                }
                return results + "}\n";
            }


            public static string propertyOfObjectToString(string property, kData obj)
            {
                if (obj is Image)
                {
                    Image img = (Image)obj;

                    switch (property)
                    {
                        case "width": return img.width.ToString();
                        case "height": return img.height.ToString();
                    }
                }

                switch (property)
                {
                    case "location": return obj.location;
                    case "bytes": return System.Text.Encoding.UTF8.GetString(obj.bytes, 0, obj.bytes.Length);
                }

                Koala.Error.raiseException("Unknown property " + property);
                return null;

            }


            public static object stringToObject(string a)
            {
                double retDouble;
                long retLong;
                bool found = false;

                object[] values = new object[2];

                if (a.Contains('.'))
                {
                    found = double.TryParse(a, System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out retDouble);
                    if (found) return retDouble;
                }
                else
                {
                    found = long.TryParse(a, System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out retLong);
                    if (found) return retLong;
                }

                object outObj;
                if (privateMemory.TryGetValue(a, out outObj))
                {
                    return outObj;
                }
                else
                {
                    Koala.Error.raiseException("Unknown value " + a);
                    return null;
                }

            }

            public static string stringToPath(string a)
            {
                return a.Substring(1, a.Length - 3);

            }
            public static string stringToLocation(string a)
            {
                string path = a.Substring(1, a.Length - 3);

                return path;

            }


            public static object[] convertStringsToValues(string[] s)
            {
                object[] values = new object[2];

                values[0] = stringToObject(s[0]);
                values[1] = stringToObject(s[1]);


                kData res;
                object outObj;
                // CHECK FOR PRONUMERALS 
                if (values[0] == null)
                {
                    if(privateMemory.TryGetValue(s[0],out outObj))
                    {
                        values[0] = outObj;
                    }
                    else if(memory.TryGetValue(s[0], out res))
                    {

                    }
                    else
                    {
                        Koala.Error.raiseException("Unknown value " + s[0]);
                    }
                }

                if (values[1] == null)
                {
                    if (privateMemory.TryGetValue(s[1], out outObj))
                    {
                        values[1] = outObj;
                    }
                    else if (memory.TryGetValue(s[1], out res))
                    {

                    }
                    else
                    {
                        Koala.Error.raiseException("unknown value " + s[1]);
                    }
                }



                return values;
            }




            public static kData stringToDataType(string a)
            {
                string type = a.ToLower();
                switch (type)
                {
                    case "file": return new DataTypes.File();
                    case "files": return new DataTypes.FileSet();
                    case "image": return new DataTypes.Image();
                    case "images": return new DataTypes.ImageSet();
                    case "video": return new DataTypes.Video();
                    case "videos": return new DataTypes.VideoSet();
                }

                Koala.Error.raiseException("Invalid DataType Specified");

                return null;

            }


        }

    

        public class kFunction{

            public string name
            {
                set;
                get;
            }

            public List<string> expressions = new List<string>();

        }
        public class kData
        {
            public string name;
            public string location;
            public bool isDirectory;
            public bool isSet = false;

            public byte[] bytes;

            public Dictionary<string, Regex> classExpressions;







        }


        public class Video : kData
        {

        }
        public class VideoSet : kData
        {

        }

        public class Image : kData
        {
            public Image()
            {

            }
            public Image(Bitmap b)
            {
                bmpRepresentation   = b;
                height  = (UInt32)b.Height;
                width   = (UInt32)b.Width;

            }
            public Bitmap bmpRepresentation;
            public UInt32 height;
            public UInt32 width;


        }
        public class ImageSet : kData
        {
            public List<Image> images = new List<Image>();


        }

        public class File : kData
        {


        }

        public class FileSet : kData
        {


        }

        public class Number : kData
        {

            public Number(object d)
            {

                Value = d;
            }

            object Value;


            public bool isGreaterThan(Number num2)
            {
                if (num2.Value.GetType() == typeof(double) && Value.GetType() == typeof(double))        return ((double)num2.Value < (double)Value);
                else if (num2.Value.GetType() == typeof(double) && Value.GetType() == typeof(long))     return ((double)num2.Value < (long)Value);
                else if (num2.Value.GetType() == typeof(long) && Value.GetType() == typeof(double))     return ((long)num2.Value < (double)Value);
                else                                                                                    return ((long)num2.Value < (long)Value);
            }

            public bool isLessThan(Number num2)
            {
                if (num2.Value.GetType() == typeof(double) && Value.GetType() == typeof(double))        return ((double)num2.Value > (double)Value);
                else if (num2.Value.GetType() == typeof(double) && Value.GetType() == typeof(long))     return ((double)num2.Value > (long)Value);
                else if (num2.Value.GetType() == typeof(long) && Value.GetType() == typeof(double))     return ((long)num2.Value > (double)Value);
                else                                                                                    return ((long)num2.Value > (long)Value);
            }

            public bool isEqualTo(Number num2)
            {
                if (num2.Value.GetType() == typeof(double) && Value.GetType() == typeof(double))        return ((double)num2.Value == (double)Value);
                else if (num2.Value.GetType() == typeof(double) && Value.GetType() == typeof(long))     return ((double)num2.Value == (long)Value);
                else if (num2.Value.GetType() == typeof(long) && Value.GetType() == typeof(double))     return ((long)num2.Value == (double)Value);
                else                                                                                    return ((long)num2.Value == (long)Value);
            }


        }

    }

}

