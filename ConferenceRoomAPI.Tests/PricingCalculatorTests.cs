using ConferenceRoomAPI.Utilities;
using Xunit;

namespace ConferenceRoomAPI.Tests
{
    public class PricingCalculatorTests
    {
        [Fact]
        public void CalculatePrice_StandardHours_ReturnsBasePrice()
        {
            var startTime = new DateTime(2024, 9, 1, 10, 0, 0);
            var endTime = new DateTime(2024, 9, 1, 12, 0, 0);
            var baseRate = 2000m;
            var services = new List<decimal>();

            var result = PricingCalculator.CalculatePrice(startTime, endTime, baseRate, services);

            Assert.Equal(4000m, result.BasePrice);
            Assert.Equal(0m, result.Discount);
            Assert.Equal(0m, result.ServicesPrice);
            Assert.Equal(4000m, result.TotalPrice);
        }

        [Fact]
        public void CalculatePrice_PeakHours_AppliesMarkup()
        {
            var startTime = new DateTime(2024, 9, 1, 12, 0, 0);
            var endTime = new DateTime(2024, 9, 1, 14, 0, 0);
            var baseRate = 3500m;
            var services = new List<decimal>();

            var result = PricingCalculator.CalculatePrice(startTime, endTime, baseRate, services);

            var expectedPrice = 3500m * 2 * 1.15m;
            Assert.Equal(expectedPrice, result.BasePrice);
            Assert.True(result.Discount < 0);
            Assert.Equal(expectedPrice, result.TotalPrice);
        }

        [Fact]
        public void CalculatePrice_EveningHours_AppliesDiscount()
        {
            var startTime = new DateTime(2024, 9, 1, 18, 0, 0);
            var endTime = new DateTime(2024, 9, 1, 22, 0, 0);
            var baseRate = 1500m;
            var services = new List<decimal>();

            var result = PricingCalculator.CalculatePrice(startTime, endTime, baseRate, services);

            var basePrice = 1500m * 4 * 0.8m;
            var discount = 1500m * 4 * 0.2m;
            Assert.Equal(basePrice, result.BasePrice);
            Assert.Equal(discount, result.Discount);
            Assert.Equal(basePrice, result.TotalPrice);
        }

        [Fact]
        public void CalculatePrice_EarlyMorningHours_AppliesDiscount()
        {
            var startTime = new DateTime(2024, 9, 1, 6, 0, 0);
            var endTime = new DateTime(2024, 9, 1, 9, 0, 0);
            var baseRate = 2000m;
            var services = new List<decimal>();

            var result = PricingCalculator.CalculatePrice(startTime, endTime, baseRate, services);

            var basePrice = 2000m * 3 * 0.9m;
            var discount = 2000m * 3 * 0.1m;
            Assert.Equal(basePrice, result.BasePrice);
            Assert.Equal(discount, result.Discount);
            Assert.Equal(basePrice, result.TotalPrice);
        }

        [Fact]
        public void CalculatePrice_WithServices_IncludesServicePrices()
        {
            var startTime = new DateTime(2024, 9, 1, 10, 0, 0);
            var endTime = new DateTime(2024, 9, 1, 12, 0, 0);
            var baseRate = 2000m;
            var services = new List<decimal> { 500m, 300m };

            var result = PricingCalculator.CalculatePrice(startTime, endTime, baseRate, services);

            Assert.Equal(4000m, result.BasePrice);
            Assert.Equal(800m, result.ServicesPrice);
            Assert.Equal(4800m, result.TotalPrice);
        }

        [Fact]
        public void CalculatePrice_SingleHourBooking_CalculatesCorrectly()
        {
            var startTime = new DateTime(2024, 9, 1, 10, 0, 0);
            var endTime = new DateTime(2024, 9, 1, 11, 30, 0);
            var baseRate = 2000m;
            var services = new List<decimal>();

            var result = PricingCalculator.CalculatePrice(startTime, endTime, baseRate, services);

            var expectedPrice = 2000m * 2;
            Assert.Equal(expectedPrice, result.BasePrice);
        }

        [Fact]
        public void CalculatePrice_MixedTimeSlots_AppliesPeakRate()
        {
            var startTime = new DateTime(2024, 9, 1, 11, 0, 0);
            var endTime = new DateTime(2024, 9, 1, 15, 0, 0);
            var baseRate = 2000m;
            var services = new List<decimal>();

            var result = PricingCalculator.CalculatePrice(startTime, endTime, baseRate, services);

            var expectedPrice = 2000m * 4 * 1.15m;
            Assert.Equal(expectedPrice, result.BasePrice);
        }

        [Fact]
        public void CalculatePrice_ZeroServices_ReturnsZeroServicePrice()
        {
            var startTime = new DateTime(2024, 9, 1, 10, 0, 0);
            var endTime = new DateTime(2024, 9, 1, 12, 0, 0);
            var baseRate = 2000m;
            var services = new List<decimal>();

            var result = PricingCalculator.CalculatePrice(startTime, endTime, baseRate, services);

            Assert.Equal(0m, result.ServicesPrice);
        }

        [Fact]
        public void CalculatePrice_LargeBooking_AppliesPeakRate()
        {
            var startTime = new DateTime(2024, 9, 1, 9, 0, 0);
            var endTime = new DateTime(2024, 9, 1, 18, 0, 0);
            var baseRate = 3500m;
            var services = new List<decimal> { 500m, 300m, 700m };

            var result = PricingCalculator.CalculatePrice(startTime, endTime, baseRate, services);

            var expectedBasePrice = 3500m * 9 * 1.15m;
            var expectedServicesPrice = 500m + 300m + 700m;
            Assert.Equal(expectedBasePrice, result.BasePrice);
            Assert.Equal(expectedServicesPrice, result.ServicesPrice);
            Assert.Equal(expectedBasePrice + expectedServicesPrice, result.TotalPrice);
        }
    }
}
