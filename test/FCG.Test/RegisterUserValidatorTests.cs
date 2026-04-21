using FCG.Application.DTOs;
using FCG.Application.Validators;
using FluentAssertions;
using Xunit;

namespace FCG.Test;

public class RegisterUserValidatorTests
{
    private readonly RegisterUserValidator _validator;

    public RegisterUserValidatorTests()
    {
        _validator = new RegisterUserValidator();
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenNameIsEmpty()
    {
        // Arrange
        var model = new RegisterUserDto("", "teste@email.com", "Senh@Forte123");

        // Act
        var result = _validator.Validate(model);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Fact]
    public void Validate_ShouldBeValid_WhenModelIsCorrect()
    {
        // Arrange
        var model = new RegisterUserDto("João das Neves", "joao@email.com", "Senh@Forte123");

        // Act
        var result = _validator.Validate(model);

        // Assert
        result.IsValid.Should().BeTrue();
    }
}
