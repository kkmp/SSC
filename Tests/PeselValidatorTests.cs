using SSC.Data;
using Xunit;

namespace Tests
{
    public class PeselValidatorTests
    {
        [Fact]
        public void CorrectPeselTest()
        {
            var pesel = "00241734865";
            PeselValidator peselValidator = new PeselValidator(pesel);
            Assert.True(peselValidator.Valid);
            Assert.Equal(peselValidator.Sex, "F");
            Assert.Equal(peselValidator.Day, 17);
            Assert.Equal(peselValidator.Month, 4);
            Assert.Equal(peselValidator.Year, 2000);
        }
    }
}