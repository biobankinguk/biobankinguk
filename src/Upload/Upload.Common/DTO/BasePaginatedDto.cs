using System;

namespace Upload.Common.DTO
{
    public class BasePaginatedDto
    {
        public int Offset { get; set; }

        public int Count { get; set; }

        public int Total { get; set; }

        public Uri? Next { get; set; }

        public Uri? Previous { get; set; }
    }
}
