using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NetexMerge
{
    public class StopPlace
    {
        public string Id { get; set; }
        public string Name { get; set; } 
        public string LegacyStopId { get; set; }
        public string MultimodalStopPlaceId { get; set; }
        public string MultimodalStopPlaceName { get; set; }
        public List<Quay> Quays;

        public StopPlace()
        {
            Quays = new List<Quay>();
        }
    }

    public struct Quay
    {
        public string Id { get; set; }
        public string LegacyQuayId { get; set; }
    }
}
