﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Directory.Search.Dto.Results
{
    public class CapabilitySummary
    {
        public CapabilitySummary()
        {
            AssociatedData = new List<AssociatedDataSummary>();
        }

        public string Disease { get; set; }
        public string Protocols { get; set; }
        public string AnnualDonorExpectation { get; set; }

        public IList<AssociatedDataSummary> AssociatedData { get; set; }
    }
}
