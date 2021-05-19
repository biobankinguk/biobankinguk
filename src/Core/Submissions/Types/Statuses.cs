namespace Core.Submissions.Types
{
    // TODO: There's also a Status Refdata table. Which is the authority? Can we make this clearer!
    public static class Statuses
    {
        public const string Open = "Open";
        public const string Committed = "Committed";
        public const string Rejected = "Rejected";
        public const string Expired = "Expired";
    }
}
