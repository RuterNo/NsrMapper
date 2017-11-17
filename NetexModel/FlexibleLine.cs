using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetexModel
{
    public class FlexibleLine
    {
        public FlexibleLineTypeEnumeration FlexibleLineType { get; set; }
        public string BookingContact { get; set; }
        public BookingMethodListOfEnumerations BookingMethods { get; set; }
        public PurchaseWhenEnumeration BookWhen { get; set; }

    }
}
