namespace ConferenceRoomAPI.Utilities
{
    public class PricingCalculator
    {
        public class PriceResult
        {
            public decimal BasePrice { get; set; }
            public decimal Discount { get; set; }
            public decimal ServicesPrice { get; set; }
            public decimal TotalPrice { get; set; }
            public string TimeSlotDescription { get; set; }
        }

        public static PriceResult CalculatePrice(
            DateTime startTime,
            DateTime endTime,
            decimal baseHourlyRate,
            List<decimal> servicesPrices)
        {
            var result = new PriceResult();
            int durationHours = (int)Math.Ceiling((endTime - startTime).TotalHours);

            decimal discountMultiplier = GetPriceMultiplier(startTime, endTime);
            result.BasePrice = baseHourlyRate * durationHours * discountMultiplier;
            result.Discount = (baseHourlyRate * durationHours) - result.BasePrice;
            result.ServicesPrice = servicesPrices.Sum();
            result.TotalPrice = result.BasePrice + result.ServicesPrice;
            result.TimeSlotDescription = GetTimeSlotDescription(startTime, endTime);

            return result;
        }

        private static decimal GetPriceMultiplier(DateTime startTime, DateTime endTime)
        {
            int startHour = startTime.Hour;
            int endHour = endTime.Hour;

            bool hasPeakHours = IsTimeInRange(startHour, endHour, 12, 14);
            bool hasEveningHours = IsTimeInRange(startHour, endHour, 18, 23);
            bool hasEarlyMorningHours = IsTimeInRange(startHour, endHour, 6, 9);

            if (hasPeakHours)
                return 1.15m;

            if (hasEveningHours)
                return 0.8m;

            if (hasEarlyMorningHours)
                return 0.9m;

            return 1.0m;
        }

        private static bool IsTimeInRange(int startHour, int endHour, int rangeStart, int rangeEnd)
        {
            return (startHour >= rangeStart && startHour < rangeEnd) ||
                   (endHour > rangeStart && endHour <= rangeEnd) ||
                   (startHour <= rangeStart && endHour >= rangeEnd);
        }

        private static string GetTimeSlotDescription(DateTime startTime, DateTime endTime)
        {
            int startHour = startTime.Hour;
            int endHour = endTime.Hour;

            if (IsTimeInRange(startHour, endHour, 12, 14))
                return "Peak hours (12:00-14:00): 15% markup";
            if (IsTimeInRange(startHour, endHour, 18, 23))
                return "Evening hours (18:00-23:00): 20% discount";
            if (IsTimeInRange(startHour, endHour, 6, 9))
                return "Early morning (06:00-09:00): 10% discount";

            return "Standard hours (09:00-18:00): base rate";
        }
    }
}
