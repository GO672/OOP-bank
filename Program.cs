using System;
using MyWill;
namespace MyWill
{
    public enum Currency
    {
        Dollars,
        Rubles,
    }

    public enum ExpenseCategory
    {
        Food = 1,
        Taxi,
        Sport,
        Medicine,
        Restaurants,
        Rent,
        Investments,
        Clothes,
        Fun,
        Other
    }

    public enum IncomeCategory
    {
        Salary = 1,
        Scholarship,
        Other
    }


    public class Money : IEquatable<Money>, IComparable<Money>
    {
        public char sign;
        public int integerPart;
        public int fractionalPart;
        public Currency currency;

        public Money()
        {
            Random rand = new Random();
            sign = rand.Next(2) == 0 ? '+' : '-';
            integerPart = rand.Next(1000);
            fractionalPart = rand.Next(100);
            currency = (Currency)rand.Next(Enum.GetValues(typeof(Currency)).Length);
        }

        public Money(char sign, int integerPart, int fractionalPart, Currency currency)
        {
            if (sign == '+' || sign == '-')
            {
                this.sign = sign;
            }
            this.integerPart = integerPart;
            this.fractionalPart = fractionalPart;
            this.currency = currency;
        }

        public Money(Money other)
        {
            if (sign == '+' || sign == '-')
            {
                this.sign = other.sign;
            }
            this.integerPart = other.integerPart;
            this.fractionalPart = other.fractionalPart;
            this.currency = other.currency;
        }

        public Money(string input)
        {
            string[] parts = input.Split(' ');

            string numberPart = parts[0];
            this.sign = numberPart[0];

            string numberWithoutSign = numberPart.Substring(1);

            string[] numberParts = numberWithoutSign.Split('.');

            this.integerPart = int.Parse(numberParts[0]);
            this.fractionalPart = int.Parse(numberParts[1]);

            this.currency = (Currency)Enum.Parse(typeof(Currency), parts[1]);

        }




        public char GetSign()
        {
            return this.sign;
        }
        public int GetIntegerPart()
        {
            return this.integerPart;
        }
        public int GetFractionalPart()
        {
            return this.fractionalPart;
        }
        public Currency GetCurrency()
        {
            return this.currency;
        }

        public void SetSign(char sign)
        {
            this.sign = sign;
        }
        public void SetIntegerPart(int integerPart)
        {
            this.integerPart = integerPart;
        }
        public void SetFractionalPart(int fractionalPart)
        {
            this.fractionalPart = fractionalPart;
        }
        public void SetCurrency(Currency currency)
        {
            this.currency = currency;
        }

        public Money Conv(Currency targetCurrency)
        {
            if (this.currency == targetCurrency)
            {
                return this;
            }

            if (this.currency == Currency.Dollars && targetCurrency == Currency.Rubles)
            {
                int usdToRubExchangeRate = 70;
                int totalCentsInUSD = this.integerPart * 100 + this.fractionalPart;
                int totalCentsInRUB = totalCentsInUSD * usdToRubExchangeRate;

                char resultSign = '+';
                if (totalCentsInRUB < 0)
                {
                    resultSign = '-';
                    totalCentsInRUB *= -1;
                }

                int resultIntegerPart = totalCentsInRUB / 100;
                int resultFractionalPart = totalCentsInRUB % 100;

                return new Money(resultSign, resultIntegerPart, resultFractionalPart, targetCurrency);
            }

            throw new ArgumentException("Unsupported currency conversion.");
        }

        public void AddMoney(char sign, int integerPart, int fractionalPart)
        {

            if (this.sign == '+' && sign == '+')
            {
                if (this.fractionalPart / 10 <= 0 && fractionalPart / 10 > 0)
                {

                    this.fractionalPart *= 10;
                    this.integerPart += integerPart + 1;
                    this.fractionalPart = (this.fractionalPart + fractionalPart) - 100;
                }
                else if (this.fractionalPart / 10 > 0 && fractionalPart / 10 <= 0)
                {
                    fractionalPart *= 10;
                    this.integerPart += integerPart + 1;
                    this.fractionalPart = (this.fractionalPart + fractionalPart) - 100;
                }
                else if (this.fractionalPart / 10 > 0 && fractionalPart / 10 > 0)
                {
                    if (this.fractionalPart + fractionalPart >= 100)
                    {
                        this.integerPart += integerPart + 1;
                        this.fractionalPart = (this.fractionalPart + fractionalPart) - 100;
                    }
                    else
                    {
                        this.integerPart += integerPart;
                        this.fractionalPart += fractionalPart;
                        this.fractionalPart *= 10;
                    }
                }
                else
                {
                    if (this.fractionalPart + fractionalPart >= 10)
                    {
                        this.integerPart += integerPart + 1;
                        this.fractionalPart = (this.fractionalPart + fractionalPart) - 10;
                        this.fractionalPart *= 10;
                    }
                    else
                    {
                        this.integerPart += integerPart;
                        this.fractionalPart += fractionalPart;
                        this.fractionalPart *= 10;
                    }
                }
            }
            else if (this.sign == '-' && sign == '-')
            {
                this.sign = '-';

                if (this.fractionalPart / 10 <= 0 && fractionalPart / 10 > 0)
                {

                    this.fractionalPart *= 10;
                    this.integerPart += integerPart + 1;
                    this.fractionalPart = (this.fractionalPart + fractionalPart) - 100;
                }
                else if (this.fractionalPart / 10 > 0 && fractionalPart / 10 <= 0)
                {
                    fractionalPart *= 10;
                    this.integerPart += integerPart + 1;
                    this.fractionalPart = (this.fractionalPart + fractionalPart) - 100;
                }
                else if (this.fractionalPart / 10 > 0 && fractionalPart / 10 > 0)
                {
                    if (this.fractionalPart + fractionalPart >= 100)
                    {
                        this.integerPart += integerPart + 1;
                        this.fractionalPart = (this.fractionalPart + fractionalPart) - 100;
                    }
                    else
                    {
                        this.integerPart += integerPart;
                        this.fractionalPart += fractionalPart;
                        this.fractionalPart *= 10;
                    }
                }
                else
                {
                    if (this.fractionalPart + fractionalPart >= 10)
                    {
                        this.integerPart += integerPart + 1;
                        this.fractionalPart = (this.fractionalPart + fractionalPart) - 10;
                        this.fractionalPart *= 10;
                    }
                    else
                    {
                        this.integerPart += integerPart;
                        this.fractionalPart += fractionalPart;
                        this.fractionalPart *= 10;
                    }
                }
            }
            else if (this.sign == '-' && sign == '+')
            {
                if (this.fractionalPart / 10 <= 0 && fractionalPart / 10 > 0)
                {
                    this.fractionalPart *= 10;
                }
                else if (this.fractionalPart / 10 > 0 && fractionalPart / 10 <= 0)
                {
                    fractionalPart *= 10;
                }

                if (this.integerPart >= integerPart && this.fractionalPart >= fractionalPart)
                {
                    this.sign = '-';
                    this.integerPart -= integerPart;
                    this.fractionalPart -= fractionalPart;
                    this.fractionalPart *= 10;
                }
                else if (this.integerPart <= integerPart && this.fractionalPart <= fractionalPart)
                {
                    this.sign = '+';
                    this.integerPart = integerPart - this.integerPart;
                    this.fractionalPart = fractionalPart - this.fractionalPart;
                    this.fractionalPart *= 10;
                }
                else if (this.integerPart <= integerPart && this.fractionalPart >= fractionalPart)
                {
                    this.sign = '+';
                    this.integerPart = (integerPart - this.integerPart) - 1;

                    if (this.fractionalPart / 10 > 0)
                    {
                        this.fractionalPart = 100 - (this.fractionalPart - fractionalPart);
                    }
                    else
                    {
                        this.fractionalPart = 10 - (this.fractionalPart - fractionalPart);
                        this.fractionalPart *= 10;
                    }
                }
                else if (this.integerPart >= integerPart && this.fractionalPart <= fractionalPart)
                {
                    this.sign = '-';
                    this.integerPart = (this.integerPart - integerPart) - 1;

                    if (this.fractionalPart / 10 > 0)
                    {
                        this.fractionalPart = 100 - (fractionalPart - this.fractionalPart);
                    }
                    else
                    {
                        this.fractionalPart = 10 - (fractionalPart - this.fractionalPart);
                        this.fractionalPart *= 10;
                    }
                }
            }
            else if (this.sign == '+' && sign == '-')
            {
                if (this.fractionalPart / 10 <= 0 && fractionalPart / 10 > 0)
                {
                    this.fractionalPart *= 10;
                }
                else if (this.fractionalPart / 10 > 0 && fractionalPart / 10 <= 0)
                {
                    fractionalPart *= 10;
                }

                if (this.integerPart >= integerPart && this.fractionalPart >= fractionalPart)
                {
                    this.sign = '+';
                    this.integerPart -= integerPart;
                    this.fractionalPart -= fractionalPart;
                    this.fractionalPart *= 10;
                }
                else if (this.integerPart <= integerPart && this.fractionalPart <= fractionalPart)
                {
                    this.sign = '-';

                    this.integerPart = integerPart - this.integerPart;
                    this.fractionalPart = fractionalPart - this.fractionalPart;
                    this.fractionalPart *= 10;
                }
                else if (this.integerPart <= integerPart && this.fractionalPart >= fractionalPart)
                {
                    this.sign = '-';

                    this.integerPart = (integerPart - this.integerPart) - 1;

                    if (this.fractionalPart / 10 > 0)
                    {
                        this.fractionalPart = 100 - (this.fractionalPart - fractionalPart);
                    }
                    else
                    {
                        this.fractionalPart = 10 - (this.fractionalPart - fractionalPart);
                        this.fractionalPart *= 10;
                    }

                }
                else if (this.integerPart >= integerPart && this.fractionalPart <= fractionalPart)
                {
                    this.sign = '+';
                    this.integerPart = (this.integerPart - integerPart) - 1;

                    if (this.fractionalPart / 10 > 0)
                    {
                        this.fractionalPart = 100 - (fractionalPart - this.fractionalPart);
                    }
                    else
                    {
                        this.fractionalPart = 10 - (fractionalPart - this.fractionalPart);
                        this.fractionalPart *= 10;
                    }
                }
            }
        }
        public void SubMoney(char sign, int integerPart, int fractionalPart)
        {
            this.sign = (this.sign == '+') ? '-' : '+';
            AddMoney(sign, integerPart, fractionalPart);
            this.sign = (this.sign == '+') ? '-' : '+';
        }
        public void MAdd2(Money money)
        {
            if (this.fractionalPart < 0)
            {
                this.fractionalPart *= -1;
            }
            if (this.currency == money.GetCurrency())
            {
                if (this.sign == '+' && money.GetSign() == '+')
                {
                    this.sign = '+';

                    if (this.fractionalPart / 10 <= 0 && money.GetFractionalPart() / 10 > 0)
                    {
                        this.fractionalPart *= 10;
                        this.integerPart += money.GetIntegerPart() + 1;
                        this.fractionalPart = (this.fractionalPart + money.GetFractionalPart()) - 100;
                    }
                    else if (this.fractionalPart / 10 > 0 && money.GetFractionalPart() / 10 <= 0)
                    {
                        money.fractionalPart *= 10;
                        this.integerPart += money.GetIntegerPart() + 1;
                        this.fractionalPart = (this.fractionalPart + money.GetFractionalPart()) - 100;
                    }
                    else if (this.fractionalPart / 10 > 0 && money.GetFractionalPart() / 10 > 0)
                    {
                        if (this.fractionalPart + money.GetFractionalPart() >= 100)
                        {
                            this.integerPart += money.GetIntegerPart() + 1;
                            this.fractionalPart = (this.fractionalPart + money.GetFractionalPart()) - 100;
                        }
                        else
                        {
                            this.integerPart += money.GetIntegerPart();
                            this.fractionalPart += money.GetFractionalPart();
                            this.fractionalPart *= 10;
                        }
                    }
                    else
                    {
                        if (this.fractionalPart + money.GetFractionalPart() >= 10)
                        {
                            this.integerPart += money.GetIntegerPart() + 1;
                            this.fractionalPart = (this.fractionalPart + money.GetFractionalPart()) - 10;
                            this.fractionalPart *= 10;
                        }
                        else
                        {
                            this.integerPart += money.GetIntegerPart();
                            this.fractionalPart += money.GetFractionalPart();
                            this.fractionalPart *= 10;
                        }
                    }
                }
                else if (this.sign == '-' && money.GetSign() == '-')
                {
                    this.sign = '-';

                    if (this.fractionalPart / 10 <= 0 && money.GetFractionalPart() / 10 > 0)
                    {
                        this.fractionalPart *= 10;
                        this.integerPart += money.GetIntegerPart() + 1;
                        this.fractionalPart = (this.fractionalPart + money.GetFractionalPart()) - 100;
                    }
                    else if (this.fractionalPart / 10 > 0 && money.GetFractionalPart() / 10 <= 0)
                    {
                        money.fractionalPart *= 10;
                        this.integerPart += money.GetIntegerPart() + 1;
                        this.fractionalPart = (this.fractionalPart + money.GetFractionalPart()) - 100;
                    }
                    else if (this.fractionalPart / 10 > 0 && money.GetFractionalPart() / 10 > 0)
                    {
                        if (this.fractionalPart + money.GetFractionalPart() >= 100)
                        {
                            this.integerPart += money.GetIntegerPart() + 1;
                            this.fractionalPart = (this.fractionalPart + money.GetFractionalPart()) - 100;
                        }
                        else
                        {
                            this.integerPart += money.GetIntegerPart();
                            this.fractionalPart += money.GetFractionalPart();
                            this.fractionalPart *= 10;
                        }
                    }
                    else
                    {
                        if (this.fractionalPart + money.GetFractionalPart() >= 10)
                        {
                            this.integerPart += money.GetIntegerPart() + 1;
                            this.fractionalPart = (this.fractionalPart + money.GetFractionalPart()) - 10;
                            this.fractionalPart *= 10;
                        }
                        else
                        {
                            this.integerPart += money.GetIntegerPart();
                            this.fractionalPart += money.GetFractionalPart();
                            this.fractionalPart *= 10;
                        }
                    }
                }
                else if (this.sign == '-' && money.GetSign() == '+')
                {
                    if (this.fractionalPart / 10 <= 0 && money.GetFractionalPart() / 10 > 0)
                    {
                        this.fractionalPart *= 10;
                    }
                    else if (this.fractionalPart / 10 > 0 && money.GetFractionalPart() / 10 <= 0)
                    {
                        money.fractionalPart *= 10;
                    }

                    if (this.integerPart >= money.GetIntegerPart() && this.fractionalPart >= money.GetFractionalPart())
                    {
                        this.sign = '-';
                        this.integerPart -= money.GetIntegerPart();
                        this.fractionalPart -= money.GetFractionalPart();
                        this.fractionalPart *= 10;
                    }
                    else if (this.integerPart <= money.GetIntegerPart() && this.fractionalPart <= money.GetFractionalPart())
                    {
                        this.sign = '+';
                        this.integerPart = money.GetIntegerPart() - this.integerPart;
                        this.fractionalPart = money.GetFractionalPart() - this.fractionalPart;
                        this.fractionalPart *= 10;
                    }
                    else if (this.integerPart <= money.GetIntegerPart() && this.fractionalPart >= money.GetFractionalPart())
                    {
                        this.sign = '+';
                        this.integerPart = (money.GetIntegerPart() - this.integerPart) - 1;

                        if (this.fractionalPart / 10 > 0)
                        {
                            this.fractionalPart = 100 - (this.fractionalPart - money.GetFractionalPart());
                        }
                        else
                        {
                            this.fractionalPart = 10 - (this.fractionalPart - money.GetFractionalPart());
                            this.fractionalPart *= 10;
                        }
                    }
                    else if (this.integerPart >= money.GetIntegerPart() && this.fractionalPart <= money.GetFractionalPart())
                    {
                        this.sign = '-';
                        this.integerPart = (this.integerPart - money.GetIntegerPart()) - 1;

                        if (this.fractionalPart / 10 > 0)
                        {
                            this.fractionalPart = 100 - (money.GetFractionalPart() - this.fractionalPart);
                        }
                        else
                        {
                            this.fractionalPart = 10 - (money.GetFractionalPart() - this.fractionalPart);
                            this.fractionalPart *= 10;
                        }
                    }
                }
                else if (this.sign == '+' && money.GetSign() == '-')
                {
                    if (this.fractionalPart / 10 <= 0 && money.GetFractionalPart() / 10 > 0)
                    {
                        this.fractionalPart *= 10;
                    }
                    else if (this.fractionalPart / 10 > 0 && money.GetFractionalPart() / 10 <= 0)
                    {
                        money.SetSign('+');
                        money.fractionalPart *= 10;
                    }

                    if (this.integerPart >= money.GetIntegerPart() && this.fractionalPart >= money.GetFractionalPart())
                    {
                        this.sign = '+';
                        this.integerPart -= money.GetIntegerPart();
                        this.fractionalPart -= money.GetFractionalPart();
                        this.fractionalPart *= 10;
                    }
                    else if (this.integerPart <= money.GetIntegerPart() && this.fractionalPart <= money.GetFractionalPart())
                    {
                        this.sign = '-';
                        this.integerPart = money.GetIntegerPart() - this.integerPart;
                        this.fractionalPart = money.GetFractionalPart() - this.fractionalPart;
                        this.fractionalPart *= 10;
                    }
                    else if (this.integerPart <= money.GetIntegerPart() && this.fractionalPart >= money.GetFractionalPart())
                    {
                        this.sign = '-';
                        this.integerPart = (money.GetIntegerPart() - this.integerPart) - 1;

                        if (this.fractionalPart / 10 > 0)
                        {
                            this.fractionalPart = 100 - (this.fractionalPart - money.GetFractionalPart());
                        }
                        else
                        {
                            this.fractionalPart = 10 - (this.fractionalPart - money.GetFractionalPart());
                            this.fractionalPart *= 10;
                        }
                    }
                    else if (this.integerPart >= money.GetIntegerPart() && this.fractionalPart <= money.GetFractionalPart())
                    {
                        this.sign = '+';
                        this.integerPart = (this.integerPart - money.GetIntegerPart()) - 1;

                        if (this.fractionalPart / 10 > 0)
                        {
                            this.fractionalPart = 100 - (money.GetFractionalPart() - this.fractionalPart);
                        }
                        else
                        {
                            this.fractionalPart = 10 - (money.GetFractionalPart() - this.fractionalPart);
                            this.fractionalPart *= 10;
                        }
                    }
                }
            }
            if (this.fractionalPart < 0)
            {
                this.fractionalPart *= -1;
                this.integerPart -= 1 ;
            }
        }

        public void MSub2(Money money)
        {
            this.sign = (this.sign == '+') ? '-' : '+';
            MAdd2(money);
            this.sign = (this.sign == '+') ? '-' : '+';
        }
        public bool Equals(Money other)
        {
            if (other == null)
                return false;

            return this.currency == other.currency &&
                  this.sign == other.sign &&
                  this.integerPart == other.integerPart &&
                  this.fractionalPart == other.fractionalPart;
        }
        public int CompareTo(Money other)
        {
            if (other == null) return 1;

            if (this.currency != other.currency)
            {
                throw new ArgumentException("Cannot compare money with different currencies.");
            }

            if (this.sign != other.sign)
            {
                return this.sign == '+' ? 1 : -1;
            }

            int integerComparison = this.integerPart.CompareTo(other.integerPart);

            if (integerComparison != 0)
            {
                return this.sign == '+' ? integerComparison : -integerComparison;
            }

            return this.sign == '+' ? this.fractionalPart.CompareTo(other.fractionalPart) : -this.fractionalPart.CompareTo(other.fractionalPart);
        }
        public Money MSum(Money otherMoney)
        {
            if (this.currency != otherMoney.currency)
            {
                throw new ArgumentException("Currency mismatch for addition.");
            }

            int totalCentsThis = this.integerPart * 100 + this.fractionalPart;
            int totalCentsOther = otherMoney.integerPart * 100 + otherMoney.fractionalPart;
            int totalCentsSum = totalCentsThis + totalCentsOther;

            char resultSign = '+';
            if (totalCentsSum < 0)
            {
                resultSign = '-';
                totalCentsSum *= -1;
            }

            int resultIntegerPart = totalCentsSum / 100;
            int resultFractionalPart = totalCentsSum % 100;

            return new Money(resultSign, resultIntegerPart, resultFractionalPart, this.currency);
        }

        public Money MDif(Money otherMoney)
        {
            if (this.currency != otherMoney.currency)
            {
                throw new ArgumentException("Currency mismatch for subtraction.");
            }

            int totalCentsThis = this.integerPart * 100 + this.fractionalPart;
            int totalCentsOther = otherMoney.integerPart * 100 + otherMoney.fractionalPart;
            int totalCentsDif = totalCentsThis - totalCentsOther;

            char resultSign = '+';
            if (totalCentsDif < 0)
            {
                resultSign = '-';
                totalCentsDif *= -1;
            }

            int resultIntegerPart = totalCentsDif / 100;
            int resultFractionalPart = totalCentsDif % 100;

            return new Money(resultSign, resultIntegerPart, resultFractionalPart, this.currency);
        }

        public string DisplayMoney()
        {
            string fractionalPartString = (fractionalPart % 10 == 0) ? (fractionalPart / 10).ToString() : (fractionalPart % 10).ToString();
            return $"{sign}{integerPart}.{fractionalPart:D2} {currency}";
        }
    }
}
