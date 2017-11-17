using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace NetexMerge
{
    class Merge
    {
        // This class contains legacy code that may be taken into use later
        private static void NetexMerge()
        {
            Console.WriteLine("Include TimingLinks (y/n)");
            var mergeTimingLinks = Console.ReadLine().ToLower() == "y";

            var files = Directory.GetFiles(@"c:\\git\netexmerge\\netexmerge\\netexmerge\\bin\\debug\\xml").ToList();
            var stopPlaceDict = new Dictionary<StopPlaceType, HashSet<QuayType>>();
            while (files.Count > 0)
            {
                var file = files[0];
                var lineName = file.Split('-')[0];
                var filesToMerge = files.Where(p => p.Split('-')[0] == lineName).ToList();
                MergeFiles(filesToMerge, mergeTimingLinks, stopPlaceDict);
                var mergedFiles = files.Where(p => p.StartsWith(lineName)).ToList();
                foreach (var f in mergedFiles) files.Remove(f);

            }
        }



        private static void MergeFiles(List<string> files, bool mergeTimingLinks, Dictionary<StopPlaceType, HashSet<QuayType>> stopPlaceDict)
        {
            var xs = new XmlSerializer(typeof(PublicationDeliveryType));
            var initial = new PublicationDeliveryType();
            var toMerge = new PublicationDeliveryType();

            Console.WriteLine("Merging files: ");

            var lineName = files[0].Split('-')[0];
            using (StreamReader sr = new StreamReader(files[0]))
            {
                initial = (PublicationDeliveryType)xs.Deserialize(sr);
                Console.WriteLine("Read file " + files[0].ToString());
            }
            files.Remove(files[0]);

            var initialFrames = initial.dataObjects.CompositeFrame.frames;

            //AddStopPlaces(initialFrames, stopPlaceDict);
            //var membersList = initialFrames.ServiceFrame.Network.groupsOfLines.GroupOfLines.members.ToList();
            //var routePointsList = initialFrames.ServiceFrame.routePoints.ToList();
            var routeList = initialFrames.ServiceFrame.routes.ToList();
            var linesList = initialFrames.ServiceFrame.lines.ToList();
            //var destinationDisplayList = initialFrames.ServiceFrame.destinationDisplays.ToList();
            //var scheduledStopPointsList = initialFrames.ServiceFrame.scheduledStopPoints.ToList();
            //var stopAssignmentsList = initialFrames.ServiceFrame.stopAssignments.ToList();
            //var timingLinkList = new List<TimingLinkType>();
            //if(mergeTimingLinks) timingLinkList = initialFrames.ServiceFrame.timingLinks.ToList();
            var journeyPatternList = initialFrames.ServiceFrame.journeyPatterns.ToList();
            var serviceJourneyList = initialFrames.TimetableFrame.vehicleJourneys.ToList();

            while (files.Count > 0)
            {
                using (var sr = new StreamReader(files[0])) toMerge = (PublicationDeliveryType)xs.Deserialize(sr);
                Console.WriteLine("Read file " + files[0]);
                // add objects from the file to merge
                var toMergeFrames = toMerge.dataObjects.CompositeFrame.frames;
                //AddStopPlaces(toMergeFrames, stopPlaceDict);

                //membersList.AddRange(toMergeFrames.ServiceFrame.Network.groupsOfLines.GroupOfLines.members);
                //routePointsList.AddRange(toMergeFrames.ServiceFrame.routePoints);
                routeList.AddRange(toMergeFrames.ServiceFrame.routes);
                linesList.AddRange(toMergeFrames.ServiceFrame.lines);
                //destinationDisplayList.AddRange(toMergeFrames.ServiceFrame.destinationDisplays);
                //scheduledStopPointsList.AddRange(toMergeFrames.ServiceFrame.scheduledStopPoints);
                //stopAssignmentsList.AddRange(toMergeFrames.ServiceFrame.stopAssignments);
                //if(mergeTimingLinks) timingLinkList.AddRange(toMergeFrames.ServiceFrame.timingLinks);
                journeyPatternList.AddRange(toMergeFrames.ServiceFrame.journeyPatterns);
                serviceJourneyList.AddRange(toMergeFrames.TimetableFrame.vehicleJourneys);

                files.Remove(files[0]);
            }
            /*
            var stopPlaceList = new List<StopPlaceType>();


            foreach (var sp in stopPlaceDict)
            {
                StopPlaceType spt = sp.Key;
                spt.quays = sp.Value.ToArray();
                stopPlaceList.Add(spt);

            }
            */

            //initialFrames.SiteFrame.stopPlaces = stopPlaceList.GroupBy(p => p.id).Select(p => p.First()).ToArray();
            //initialFrames.ServiceFrame.Network.groupsOfLines.GroupOfLines.members = membersList.GroupBy(p => p.@ref).Select(p => p.First()).ToArray();
            //initialFrames.ServiceFrame.routePoints = routePointsList.GroupBy(p => p.id).Select(p => p.First()).ToArray();
            initialFrames.ServiceFrame.routes = routeList.GroupBy(p => p.id).Select(p => p.First()).ToArray();
            initialFrames.ServiceFrame.lines = linesList.GroupBy(p => p.id).Select(p => p.First()).ToArray();
            //initialFrames.ServiceFrame.destinationDisplays = destinationDisplayList.GroupBy(p => p.id).Select(p => p.First()).ToArray();
            //initialFrames.ServiceFrame.scheduledStopPoints = scheduledStopPointsList.GroupBy(p => p.id).Select(p => p.First()).ToArray();
            //initialFrames.ServiceFrame.stopAssignments = stopAssignmentsList.GroupBy(p => p.id).Select(p => p.First()).ToArray();
            //if(mergeTimingLinks) initialFrames.ServiceFrame.timingLinks = timingLinkList.GroupBy(p => p.id).Select(p => p.First()).ToArray();
            initialFrames.ServiceFrame.journeyPatterns = journeyPatternList.GroupBy(p => p.id).Select(p => p.First()).ToArray();
            initialFrames.TimetableFrame.vehicleJourneys = serviceJourneyList.GroupBy(p => p.id)
                .Select(p => p.First())
                .ToArray();


            using (var writer = new StreamWriter(lineName + ".xml"))
            {
                xs.Serialize(writer, initial);
            }


            Console.WriteLine("Saved file: " + lineName);

        }

        private static void AddStopPlaces(framesType initialFrames, Dictionary<StopPlaceType, HashSet<QuayType>> stopPlaceDict)
        {
            foreach (var sp in initialFrames.SiteFrame.stopPlaces)
            {
                var hs = new HashSet<QuayType>();
                foreach (var quay in sp.quays)
                {
                    hs.Add(quay);
                }
                sp.quays = null;
                stopPlaceDict.Add(sp, hs);
            }
        }
    }
}
