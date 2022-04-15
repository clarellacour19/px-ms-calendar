// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HrefLangContainer.cs" company="The Procter & Gamble Company">
//   Copyright © The Procter & Gamble Company 2020 - All Rights Reserved.
// </copyright>
// <summary>
//   Defines the HrefLangContainer.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace PG.ABBs.Calendar.Organizer.Content.Domain
{
    using System;
    using System.Collections.Generic;

    public class HrefLangContainer
    {
        public Dictionary<string,Object> Hreflang { get; set; }

        public Dictionary<string, string> GpocId { get; set; }

        public Dictionary<string, string> Path { get; set; }
    }
}
