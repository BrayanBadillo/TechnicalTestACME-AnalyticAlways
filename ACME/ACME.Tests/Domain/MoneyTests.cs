using ACME.Domain.ValueObjects;

namespace ACME.Tests.Domain;

public class MoneyTests
{
    [Fact]
    public void CreateMoney_WithValidData_ShouldCreateMoney()
    {
        // Arrange
        decimal amount = 100;
        string currency = "USD";

        // Act
        var money = new Money(amount, currency);

        // Assert
        Assert.Equal(amount, money.Amount);
        Assert.Equal(currency, money.Currency);
    }

    [Fact]
    public void CreateMoney_WithNegativeAmount_ShouldThrowException()
    {
        // Arrange
        decimal amount = -100;
        string currency = "USD";

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => new Money(amount, currency));
        Assert.Contains("monto", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void CreateMoney_WithInvalidCurrency_ShouldThrowException(string currency)
    {
        // Arrange
        decimal amount = 100;

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => new Money(amount, currency));
        Assert.Contains("moneda", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Equals_SameValues_ShouldReturnTrue()
    {
        // Arrange
        var money1 = new Money(100, "USD");
        var money2 = new Money(100, "USD");

        // Act & Assert
        Assert.Equal(money1, money2);
        Assert.True(money1 == money2);
        Assert.False(money1 != money2);
    }

    [Fact]
    public void Equals_DifferentValues_ShouldReturnFalse()
    {
        // Arrange
        var money1 = new Money(100, "USD");
        var money2 = new Money(200, "USD");
        var money3 = new Money(100, "EUR");

        // Act & Assert
        Assert.NotEqual(money1, money2);
        Assert.NotEqual(money1, money3);
        Assert.False(money1 == money2);
        Assert.True(money1 != money2);
    }

    [Fact]
    public void ToString_ShouldReturnFormattedString()
    {
        // Arrange
        var money = new Money(100, "USD");

        // Act
        var result = money.ToString();

        // Assert
        Assert.Equal("100 USD", result);
    }

    [Fact]
    public void Constructor_Should_Set_Properties()
    {
        var money = new Money(100, "USD");
        Assert.Equal(100, money.Amount);
        Assert.Equal("USD", money.Currency);
    }

    [Fact]
    public void ToString_Should_Return_FormattedString()
    {
        var money = new Money(50, "EUR");
        Assert.Contains("50", money.ToString());
        Assert.Contains("EUR", money.ToString());
    }

    [Fact]
    public void Equals_Should_Return_True_For_Same_Values()
    {
        var m1 = new Money(10, "USD");
        var m2 = new Money(10, "USD");
        Assert.True(m1.Equals(m2));
        Assert.True(m1.Equals((object)m2));
    }

    [Fact]
    public void Equals_Should_Return_False_For_Different_Values()
    {
        var m1 = new Money(10, "USD");
        var m2 = new Money(20, "USD");
        var m3 = new Money(10, "EUR");
        Assert.False(m1.Equals(m2));
        Assert.False(m1.Equals(m3));
        Assert.False(m1.Equals(null));
    }

    [Fact]
    public void GetHashCode_Should_Be_Consistent_For_Equal_Objects()
    {
        var m1 = new Money(10, "USD");
        var m2 = new Money(10, "USD");
        Assert.Equal(m1.GetHashCode(), m2.GetHashCode());
    }
}
