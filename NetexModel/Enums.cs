using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetexModel
{
   
    public enum FlexibleLineTypeEnumeration
    {
        corridorService,
        mainRouteWithFlexibleEnds,
        flexibleAreasOnly,
        hailAndRideSections,
        fixedStopAreaWide,
        mixedFlexible,
        mixedFlexibleAndFixed,
        Fixed
    }

    public enum BookingMethodListOfEnumerations
    {
        callDriver,
        callOffice,
        online,
        phoneAtStop,
        text
    }

    public enum PurchaseWhenEnumeration
    {
        timeOfTravelOnly,
        dayOfTravelOnly,
        untilPreviousDay,
        advanceOnly,
        advanceAndDayOfTravel
    }

    public enum FlexibleRouteTypeEnumeration
    {
        flexibleAreasOnly,
        hailAndRideSections,
        mixed,
        Fixed,
    }
}
