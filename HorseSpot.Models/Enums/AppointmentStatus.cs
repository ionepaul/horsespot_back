namespace HorseSpot.Models.Enums
{
    public static class AppointmentStatus
    {
        public const string CREATED = "CREATED";
        public const string DATE_CHANGED_BY_OWNER = "DATE_CHANGED_BY_OWNER";
        public const string DATE_CHANGED_BY_INITIATOR = "DATE_CHANGED_BY_INITIATOR";
        public const string ACCEPTED_BY_OWNER = "ACCEPTED_BY_OWNER";
        public const string ACCEPTED_BY_INITIATOR = "ACCEPTED_BY_INITIATOR";
        public const string SET_SEEN_APPOINTMETS = "SET_SEEN_APPOINTMETS";
    }
}
