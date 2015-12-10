using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;
using System.Reflection;
namespace Koala
{
    namespace DataTypes
    {

        public class Convert
        {

            public static string objectToString(kData obj)
            {
                string results = obj.GetType().Name+ " {\n";

                foreach (FieldInfo f in obj.GetType().GetFields().ToList())
                {
                    object val = f.GetValue(obj);
                    results += String.Format("\t{0}={1},\n", f.Name, val );
                    if (f.FieldType.IsArray)
                    {
                        if (f.FieldType == typeof(byte[]))
                        {
                            byte[] b = (byte[])val;
                            results += String.Format("\t{0}={1},\n", f.Name + ".length", b.Length);
                        }

                    } 


                    // do stuff here
                }
                return results+"}\n";
            }


            public static string propertyOfObjectToString(string property, kData obj)
            {
                if (obj is Image)
                {
                    Image img = (Image)obj;

                    switch (property)
                    {
                        case "width":   return img.width.ToString();
                        case "height":  return img.height.ToString();
                    }
                }

                switch (property)
                {
                    case "location":    return obj.location;
                    case "bytes":       return System.Text.Encoding.UTF8.GetString(obj.bytes, 0, obj.bytes.Length);
                }

                Koala.Error.raiseException("Unknown property " + property);
                return null;

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



            public static kData stringToDataType(string a)
            {
                string type = a.ToLower();
                switch (type)
                {
                    case "file":    return new DataTypes.File();
                    case "files":   return new DataTypes.FileSet();     
                    case "image":   return new DataTypes.Image();
                    case "images":  return new DataTypes.ImageSet();
                    case "video":   return new DataTypes.Video();   
                    case "videos":  return new DataTypes.VideoSet();    
                }

                Koala.Error.raiseException("Invalid DataType Specified");

                return null;

            }


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
            public UInt32 height;
            public UInt32 width;



            public byte[] lookAtPixel(UInt32 x, UInt32 y)
            {
                byte[] pixel = new byte[4];

                try
                {
                    UInt64 index = (y * width) + x;

                    pixel[0] = bytes[index];
                    pixel[1] = bytes[index + 1];
                    pixel[2] = bytes[index + 2];
                    pixel[3] = bytes[index + 3];

                }
                catch
                {
                    Koala.Error.raiseException("X or Y outside of Image Bounds");
                }

                return pixel;


            }


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
            public UInt64 IntegerValue
            {
                get;
                set;
            }
            public double DecimalValue
            {
                get;
                set;
            }

        }


    }
}

