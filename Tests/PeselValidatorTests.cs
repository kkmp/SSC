using SSC.Data;
using Xunit;

namespace Tests
{
    public class PeselValidatorTests
    {
        [Theory]
        [InlineData("F", 17, 4, 2000)]
        public void CorrectPeselTest(string sex, int day, int month, int year)
        {
            var pesel = "00241734865";
            PeselValidator peselValidator = new PeselValidator(pesel);
            Assert.NotNull(peselValidator);
            Assert.Equal(peselValidator.Sex, sex);
            Assert.Equal(peselValidator.Day, day);
            Assert.Equal(peselValidator.Month, month);
            Assert.Equal(peselValidator.Year, year);
        }
    }
}