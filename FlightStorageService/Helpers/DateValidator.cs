namespace FlightStorageService.Helpers
{
    public class DateValidator
    {
        public static bool IsDateValid(DateTime date)
        {
            return date >= new DateTime(1753, 1, 1) && date <= new DateTime(9999, 12, 31);
        }

        public static void EnsureValidDate(DateTime date)
        {
            if (!IsDateValid(date))
                throw new ArgumentOutOfRangeException(nameof(date), "The date must be between 1753 and 9999.");
        }
    }
}
