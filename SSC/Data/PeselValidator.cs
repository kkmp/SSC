namespace SSC.Data
{
    public class PeselValidator
    {
        private readonly int[]? peselArray;

        public string? Pesel { get; }
        public bool Valid { get; }
        public int Day { get; }
        public int Month { get; }
        public int Year { get; }
        public string? Sex { get; }
        public DateTime? Date { get; }

        public PeselValidator(string pesel)
        {
            if (pesel == null || pesel.Length != 11)
            {
                return;
            }

            this.Pesel = pesel;
            peselArray = pesel.Select(x => int.Parse(x + "")).ToArray();
            if(Checksum() && CheckMonth() && CheckDay())
            {
                Valid = true;
                Sex = GetSex();
                Day = GetBirthDay();
                Month = GetBirthMonth();
                Year = GetBirthYear();
                Date = new DateTime(Year, Month, Day);
            }
        }

        private int GetBirthYear()
        {
            int year;
            int month;
            year = 10 * peselArray[0];
            year += peselArray[1];
            month = 10 * peselArray[2];
            month += peselArray[3];
            if (month > 80 && month < 93)
            {
                year += 1800;
            }
            else if (month > 0 && month < 13)
            {
                year += 1900;
            }
            else if (month > 20 && month < 33)
            {
                year += 2000;
            }
            else if (month > 40 && month < 53)
            {
                year += 2100;
            }
            else if (month > 60 && month < 73)
            {
                year += 2200;
            }
            return year;
        }

        private int GetBirthMonth()
        {
            int month;
            month = 10 * peselArray[2];
            month += peselArray[3];
            if (month > 80 && month < 93)
            {
                month -= 80;
            }
            else if (month > 20 && month < 33)
            {
                month -= 20;
            }
            else if (month > 40 && month < 53)
            {
                month -= 40;
            }
            else if (month > 60 && month < 73)
            {
                month -= 60;
            }
            return month;
        }

        private int GetBirthDay()
        {
            int day;
            day = 10 * peselArray[4];
            day += peselArray[5];
            return day;
        }

        private string GetSex()
        {
            if (peselArray[9] % 2 == 1)
            {
                return "M";
            }
            else
            {
                return "F";
            }
        }

        private bool Checksum()
        {
            int sum = 0;
            int[] wages = { 1, 3, 7, 9, 1, 3, 7, 9, 1, 3 };
            for(int i = 0; i < 10; i++)
            {
                sum += peselArray[i] * wages[i];
            }
            sum %= 10;
            sum = 10 - sum;
            sum %= 10;

            return sum == peselArray[10];
        }

        private bool CheckMonth()
        {
            int month = GetBirthMonth();
            return month > 0 && month < 13;
        }

        private bool CheckDay()
        {
            int year = GetBirthYear();
            int month = GetBirthMonth();
            int day = GetBirthDay();
            if ((day > 0 && day < 32) &&
            (month == 1 || month == 3 || month == 5 ||
            month == 7 || month == 8 || month == 10 ||
            month == 12))
            {
                return true;
            }
            else if ((day > 0 && day < 31) &&
            (month == 4 || month == 6 || month == 9 ||
            month == 11))
            {
                return true;
            }
            else if ((day > 0 && day < 30 && LeapYear(year)) ||
            (day > 0 && day < 29 && !LeapYear(year)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool LeapYear(int year)
        {
            return year % 4 == 0 && year % 100 != 0 || year % 400 == 0;
        }
    }
}
