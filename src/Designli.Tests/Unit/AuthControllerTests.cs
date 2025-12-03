using Designli.Api.Controllers;
using Designli.Application.DTOs;
using Designli.Domain.Entities;
using Designli.Domain.Interfaces;
using Designli.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Designli.Tests.Unit;

public class AuthControllerTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IJwtTokenService> _mockJwtTokenService;
    private readonly AuthController _authController;

    public AuthControllerTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockJwtTokenService = new Mock<IJwtTokenService>();
        _authController = new AuthController(_mockUserRepository.Object, _mockJwtTokenService.Object);
    }

    [Fact]
    public void Login_WithValidCredentials_ReturnsOkResultWithToken()
    {
        // Arrange
        var loginRequest = new LoginRequest { Username = "admin", Password = "admin123" };
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword("admin123");
        var user = new UserApp { Username = "admin", PasswordHash = hashedPassword };
        var expectedToken = "valid.jwt.token";

        _mockUserRepository.Setup(x => x.GetByUsername("admin")).Returns(user);
        _mockJwtTokenService.Setup(x => x.GenerateToken("admin")).Returns(expectedToken);

        // Act
        var result = _authController.Login(loginRequest);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
        
        var resultJson = System.Text.Json.JsonSerializer.Serialize(okResult.Value);
        var resultElement = System.Text.Json.JsonDocument.Parse(resultJson).RootElement;
        var token = resultElement.GetProperty("token").GetString();
        Assert.Equal(expectedToken, token);
    }

    [Fact]
    public void Login_WithInvalidUsername_ReturnsUnauthorized()
    {
        // Arrange
        var loginRequest = new LoginRequest { Username = "invalid", Password = "password" };
        _mockUserRepository.Setup(x => x.GetByUsername("invalid")).Returns((UserApp?)null);

        // Act
        var result = _authController.Login(loginRequest);

        // Assert
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.Equal("Invalid credentials", unauthorizedResult.Value);
    }

    [Fact]
    public void Login_WithInvalidPassword_ReturnsUnauthorized()
    {
        // Arrange
        var loginRequest = new LoginRequest { Username = "admin", Password = "wrongpassword" };
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword("admin123");
        var user = new UserApp { Username = "admin", PasswordHash = hashedPassword };

        _mockUserRepository.Setup(x => x.GetByUsername("admin")).Returns(user);

        // Act
        var result = _authController.Login(loginRequest);

        // Assert
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.Equal("Invalid credentials", unauthorizedResult.Value);
    }

    [Fact]
    public void GetUsers_WithValidAuthorization_ReturnsUserList()
    {
        // Arrange
        var users = new List<UserApp>
        {
            new UserApp { Username = "admin", PasswordHash = "hash1" },
            new UserApp { Username = "john", PasswordHash = "hash2" },
            new UserApp { Username = "sarah", PasswordHash = "hash3" }
        };

        _mockUserRepository.Setup(x => x.GetAll()).Returns(users);

        // Act
        var result = _authController.GetUsers();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedUsers = Assert.IsAssignableFrom<IEnumerable<string>>(okResult.Value);
        Assert.Equal(3, returnedUsers.Count());
    }
}
