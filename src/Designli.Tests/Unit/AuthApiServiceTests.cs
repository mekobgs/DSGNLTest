using Designli.Web.Services;
using Microsoft.AspNetCore.Http;
using Moq;
using Moq.Protected;
using System.Net;
using Xunit;

namespace Designli.Tests.Unit;

public class AuthApiServiceTests
{
    private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
    private readonly HttpClient _httpClient;
    private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
    private readonly AuthApiService _authApiService;

    public AuthApiServiceTests()
    {
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_mockHttpMessageHandler.Object)
        {
            BaseAddress = new Uri("https://localhost:7203/")
        };
        _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        _authApiService = new AuthApiService(_httpClient, _mockHttpContextAccessor.Object);
    }

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ReturnsToken()
    {
        // Arrange
        var responseContent = "{\"token\":\"valid.jwt.token\"}";
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseContent, System.Text.Encoding.UTF8, "application/json")
        };

        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);

        // Act
        var token = await _authApiService.LoginAsync("admin", "admin123");

        // Assert
        Assert.NotNull(token);
        Assert.Equal("valid.jwt.token", token);
    }

    [Fact]
    public async Task LoginAsync_WithInvalidCredentials_ReturnsNull()
    {
        // Arrange
        var response = new HttpResponseMessage(HttpStatusCode.Unauthorized)
        {
            Content = new StringContent("Unauthorized", System.Text.Encoding.UTF8, "application/json")
        };

        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);

        // Act
        var token = await _authApiService.LoginAsync("admin", "wrongpassword");

        // Assert
        Assert.Null(token);
    }

    [Fact]
    public async Task GetUsersAsync_WithValidToken_ReturnsUserList()
    {
        // Arrange
        var users = new List<string> { "admin", "john", "sarah" };
        var responseContent = System.Text.Json.JsonSerializer.Serialize(users);
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseContent, System.Text.Encoding.UTF8, "application/json")
        };

        var mockHttpContext = new Mock<HttpContext>();
        var mockSession = new Mock<ISession>();
        mockSession.Setup(x => x.TryGetValue("jwt_token", out It.Ref<byte[]>.IsAny))
            .Returns(true)
            .Callback((string key, out byte[] value) =>
            {
                value = System.Text.Encoding.UTF8.GetBytes("valid.token");
            });
        mockHttpContext.Setup(x => x.Session).Returns(mockSession.Object);
        _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(mockHttpContext.Object);

        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);

        // Act
        var result = await _authApiService.GetUsersAsync();

        // Assert
        Assert.NotEmpty(result);
        Assert.Equal(3, result.Count);
    }

    [Fact]
    public async Task GetUsersAsync_WithoutToken_ReturnsEmptyList()
    {
        // Arrange
        var mockHttpContext = new Mock<HttpContext>();
        var mockSession = new Mock<ISession>();
        mockSession.Setup(x => x.TryGetValue("jwt_token", out It.Ref<byte[]>.IsAny))
            .Returns(false);
        mockHttpContext.Setup(x => x.Session).Returns(mockSession.Object);
        _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(mockHttpContext.Object);

        // Act
        var result = await _authApiService.GetUsersAsync();

        // Assert
        Assert.Empty(result);
    }
}
