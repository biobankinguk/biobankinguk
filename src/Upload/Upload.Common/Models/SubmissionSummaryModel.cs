﻿using System;

namespace Biobanks.Common.Models
{
    public class SubmissionSummaryModel
    {
        public int Id { get; set; }
        public int BiobankId { get; set; }
        public DateTime SubmissionTimestamp { get; set; }
        public int TotalRecords { get; set; }
        public int RecordsProcessed { get; set; }
        public int RecordsPassed => RecordsProcessed - RecordsFailed;
        public int RecordsFailed { get;set; }
        public string UploadStatus { get; set; } = string.Empty;
        public DateTime StatusChangeTimestamp { get; set; }
        public int ErrorCount { get; set; }
        public Uri? ErrorUri { get; set; }
    }
}