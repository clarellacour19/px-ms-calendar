using System;
using System.Collections.Generic;
using System.Text;

namespace PG.ABBs.Calendar.Organizer.Content.Domain
{
    public class SharedSetting : ContentfulContentWraper
    {

        public IList<string> ContentTypesWithHreflang { get; set; }

    }
}
