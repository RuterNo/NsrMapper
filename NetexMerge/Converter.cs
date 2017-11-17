using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Messaging;
using System.Xml;
using System.Xml.Linq;

namespace NetexMerge
{
    public class Converter
    {
        private List<string> Ptas = new List<string>();
        private readonly List<string> SenPtas = new List<string>
        {
            "RUT",
            "BRA",
            "OPP",
            "OST",
            "HED",
            "VKT",
            "TEL"
        };
        
        public Converter(string pta)
        {
            if (pta == "SEN") Ptas = SenPtas;
            else
            {
                Ptas.Add(pta);
            }
        }
        public void Convert()
        {
            var stopPlaces = new List<StopPlace>();
            Console.WriteLine("Reading Netex data from NSR export NSR.xml");
            XElement root = XElement.Load("NSR.xml");
            
           
            XNamespace xn = "http://www.netex.org.uk/netex";
            var NsrStopPlaces =
                from el in root.Descendants(xn + "StopPlace")
                select el;
            Console.WriteLine("Indexing " + NsrStopPlaces.Count() + " StopPlace instances from NSR");
            int finalCount = 0;

            foreach (var sp in NsrStopPlaces)
            {
                finalCount++;
                if(finalCount % 10000 == 0) Console.WriteLine(finalCount);
                var stopPlace = new StopPlace();
                
                foreach (var NsrQuay in sp.Descendants(xn + "Quay"))
                {
                    var quay = new Quay();
                    quay.LegacyQuayId = ExtractLegacyId(NsrQuay.Element(xn + "keyList")
                        .Elements()
                        .First(p => p.Element(xn + "Key").Value == "imported-id")
                        .Element(xn + "Value")
                        .Value);
                    if (quay.LegacyQuayId == "" ) continue;
                    
                   
                    quay.Id = NsrQuay.Attribute("id").Value;
                    
                    stopPlace.Quays.Add(quay);
                
                    
               
                    
                } // foreach quay
                stopPlace.Name = (sp.Element(xn + "Name") != null ? sp.Element(xn + "Name").Value : stopPlace.MultimodalStopPlaceName);
                stopPlace.Id = sp.Attribute("id").Value;
                var keyLists = sp.Element(xn + "keyList")
                    .Elements();
                if(!keyLists.Any(p => p.Element(xn + "Key").Value == "imported-id")) continue;
                var importedIds = keyLists.First(p => p.Element(xn + "Key").Value == "imported-id");
                
                stopPlace.LegacyStopId = ExtractLegacyId(importedIds.Value);
                if(String.IsNullOrEmpty(stopPlace.LegacyStopId)) continue;
                try
                {
                    stopPlace.MultimodalStopPlaceId = sp.Element(xn + "ParentSiteRef").Attribute("ref").Value;
                    stopPlace.MultimodalStopPlaceName = root
                        .Descendants(xn + "StopPlace")
                        .First(p => p.Attribute("id").Value == stopPlace.MultimodalStopPlaceId)
                        .Element(xn + "Name").Value;

                }
                catch
                {
                }
                
                stopPlaces.Add(stopPlace);
            }
           
            
            
            WriteQuaysFile(stopPlaces);
            WriteStopsQuaysFile(stopPlaces);
            WriteReis2Mappings(stopPlaces);

        }

        private string ExtractLegacyId(string importedIdString)
        {
            var approved = new HashSet<string>();
            var candidates = importedIdString.Split(',');
            foreach (var c in candidates)
            {
                var parts = c.Split(':');
                int dummy;
                if (Ptas.Any(p => parts[0] == p))
                {
                    if(int.TryParse(parts[2], out dummy)) approved.Add(int.Parse(parts[2]).ToString());
                }
            }
            if (approved.Count > 0) return approved.First();
            return "";
        }

        private void WriteReis2Mappings(List<StopPlace> stopPlaces)
        {

            var r2mappings = new List<string>();
            foreach (var stopPlace in stopPlaces)
            {
                foreach (var quay in stopPlace.Quays)
                {
                    r2mappings.Add(quay.Id + ";" + quay.LegacyQuayId);
                }

                r2mappings.Add(stopPlace.Id + ";" + stopPlace.LegacyStopId);
                File.WriteAllLines("mappings.csv", r2mappings);
            }
        }
        private void WriteQuaysFile(List<StopPlace> stopPlaces)
        {
            
            XmlWriterSettings settings = new XmlWriterSettings { Indent = true };

            XmlWriter xw1 = XmlWriter.Create("quays-oir.xml", settings);
            
            xw1.WriteStartDocument();
            xw1.WriteStartElement("object_interface");
            
            XNamespace xn = "http://www.netex.org.uk/netex";
            
            var initialCount = stopPlaces.Count();
            int finalCount = 0;
            foreach (var sp in stopPlaces)
            {
               
                foreach (var q in sp.Quays)
                {
                   
                    xw1.WriteStartElement("quay");
                    xw1.WriteStartElement("quay_identifier");
                    xw1.WriteString(q.Id);
                    xw1.WriteEndElement();
                    xw1.WriteStartElement("quay_description");
                    xw1.WriteString(sp.Name);
                    xw1.WriteEndElement();
                    xw1.WriteStartElement("quay_stpl_identifier");
                    xw1.WriteString(sp.Id);
                    xw1.WriteEndElement();

                    xw1.WriteStartElement("quay_stpl_description");
                    xw1.WriteString(sp.Name);
                    xw1.WriteEndElement();

                    xw1.WriteStartElement("quay_mstpl_identifier");
                    xw1.WriteString(sp.MultimodalStopPlaceId);
                    xw1.WriteEndElement();
                    xw1.WriteStartElement("quay_mstpl_description");
                    xw1.WriteString(sp.MultimodalStopPlaceName);
                    xw1.WriteEndElement();

                    xw1.WriteEndElement(); // quay

                    finalCount++;
                } // foreach quay
               
            }
            xw1.WriteEndElement(); // object_interface
            xw1.Dispose();
            
            Console.WriteLine("Written " + finalCount + " Quays to OIR files.");

        }

        private void WriteStopsQuaysFile(List<StopPlace> stopPlaces)
        {
           
            XmlWriterSettings settings = new XmlWriterSettings { Indent = true };
            XmlWriter xw2 = XmlWriter.Create("stops-quays-oir.xml", settings);
            int finalCount = 0;
            xw2.WriteStartDocument();
            xw2.WriteStartElement("object_interface");
            XNamespace xn = "http://www.netex.org.uk/netex";
            
            foreach (var sp in stopPlaces)
            {
                
                foreach (var q in sp.Quays)
                {
                    
                    xw2.WriteStartElement("stop");
                    xw2.WriteStartElement("stp_identifier");
                    xw2.WriteString(q.LegacyQuayId);
                    xw2.WriteEndElement();
                    xw2.WriteStartElement("stp_quay");
                    xw2.WriteString(q.Id);
                    xw2.WriteEndElement();
                    xw2.WriteEndElement(); // stop
                    finalCount++;
                } // foreach quay
                
            }
            xw2.WriteEndElement(); // object_interface
            
            xw2.Dispose();
            Console.WriteLine("Written " + finalCount + " Stops/Quays to OIR file.");

        }
    }
}
