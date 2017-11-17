using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace NetexMerge
{
    class Program
    {

        static void Main(string[] args)
        {
            Console.WriteLine("*****************************************************");
            Console.WriteLine("*                                                   *");
            Console.WriteLine("*                  NSR mapper                       *");
            Console.WriteLine("*                   Ruter As                        *");
            Console.WriteLine("*                                                   *");
            Console.WriteLine("*****************************************************");
            Console.WriteLine("");
            Console.WriteLine("Converting Entur NSR data to OIR structured data to import in Hastus");
            Console.WriteLine("Also creates a mapping file to use for REIS2");
            Console.WriteLine("Extracting only StopPlaces with imported-id from the desired PTA");
            Console.WriteLine("");
            Console.WriteLine("Enter PTA ID (RUT, SKY etc)");
            Console.WriteLine("For REIS2 mappings for South-Eastern Norway, enter SEN.");
            var pta = Console.ReadLine();
            var converter = new Converter(pta);
            converter.Convert();
            
        }

       
    }
}
