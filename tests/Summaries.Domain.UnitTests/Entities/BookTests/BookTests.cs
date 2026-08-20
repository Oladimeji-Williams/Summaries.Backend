namespace Summaries.Domain.UnitTests.Entities.BookTests;

public sealed class BookTests
{
    [Fact]
    public void Test_Infrastructure_Should_Work()
    {
        // Arrange
        const bool expected = true;

        // Act
        var actual = true;

        // Assert
        actual.Should().Be(expected);
    }
}