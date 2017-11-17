using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Google.Kml;
using SharpKml;
using SharpKml.Base;
using SharpKml.Dom;
using SharpKml.Engine;


namespace NetexMerge
{
    class FlexConverter
    {
        public void FlexConvert()
        {
            var parser = new Parser();
            using (var stream = System.IO.File.OpenRead("FlexPolygons.xml"))
            {
                parser.Parse(stream);
            }
            string ddddd = "";



        }

    }
}
