using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

namespace Koala
{
    namespace DataTypes
    {

        public class Convert
        {
            public static kData convertStringToDataType(string a)
            {
                string type = a.ToLower();
                switch (type)
                {
                    case "file":    return new kData.File();        break;
                    case "files":   return new kData.FileSet();     break;
                    case "image":   return new kData.Image();       break;
                    case "images":  return new kData.ImageSet();    break;
                    case "video":   return new kData.Video();       break;
                    case "videos":  return new kData.VideoSet();    break;
                }

                Koala.Error.raiseException("Invalid DataType Specified");

                return null;

            }


        }
        public class kData
        {
            public FileStream dataStream;
            public string name;
            public string[] location;


           
            public class Video : kData
            {


            }
            public class VideoSet : kData
            {


            }

            public class Image : kData
            {


            }
            public class ImageSet : kData
            {


            }

            public class File : kData
            {


            }

            public class FileSet : kData
            {


            }
        }

        public class kNumber
        {



        }
    }
}

