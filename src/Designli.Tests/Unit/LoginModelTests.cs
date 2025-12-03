using Designli.Application.DTOs;
using Designli.Web.Pages;
using Designli.Web.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using Xunit;

namespace Designli.Tests.Unit;

public class LoginModelTests
{
    private readonly Mock<IAuthApiService> _mockAuthApiService;
    private readonly LoginModel _loginModel;
    private readonly Mock<HttpContext> _mockHttpContext;
    private readonly FakeSession _fakeSession;

    public LoginModelTests()
    {
        _mockAuthApiService = new Mock<IAuthApiService>();
        _mockHttpContext = new Mock<HttpContext>();
        _fakeSession = new FakeSession();

        _mockHttpContext.Setup(x => x.Session).Returns(_fakeSession);

        _loginModel = new LoginModel(_mockAuthApiService.Object)
        {
            PageContext = new PageContext { HttpContext = _mockHttpContext.Object }
        };
    }

    [Fact]
    public async Task OnPostAsync_WithValidCredentials_RedirectsToIndex()
    {
        // Arrange
        var validToken = "valid.jwt.token";
        _loginModel.Input = new LoginRequest { Username = "admin", Password = "admin123" };
        _mockAuthApiService.Setup(x => x.LoginAsync("admin", "admin123"))
            .ReturnsAsync(validToken);

        // Act
        var result = await _loginModel.OnPostAsync();

        // Assert
        var redirectResult = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("/Index", redirectResult.PageName);
        Assert.True(_fakeSession.Data.ContainsKey("jwt_token"));
        Assert.Equal(validToken, System.Text.Encoding.UTF8.GetString(_fakeSession.Data["jwt_token"]));
    }

    [Fact]
    public async Task OnPostAsync_WithInvalidCredentials_ReturnsPageWithError()
    {
        // Arrange
        _loginModel.Input = new LoginRequest { Username = "admin", Password = "wrongpassword" };
        _mockAuthApiService.Setup(x => x.LoginAsync("admin", "wrongpassword"))
            .ReturnsAsync((string?)null);

        // Act
        var result = await _loginModel.OnPostAsync();

        // Assert
        var pageResult = Assert.IsType<PageResult>(result);
        Assert.Equal("Invalid login", _loginModel.ErrorMessage);
    }

    [Fact]
    public async Task OnPostAsync_WithEmptyUsername_ReturnsPageWithError()
    {
        // Arrange
        _loginModel.Input = new LoginRequest { Username = "", Password = "password" };

        // Act
        var result = await _loginModel.OnPostAsync();

        // Assert
        var pageResult = Assert.IsType<PageResult>(result);
        Assert.Equal("Username and password are required", _loginModel.ErrorMessage);
    }

    [Fact]
    public async Task OnPostAsync_WithEmptyPassword_ReturnsPageWithError()
    {
        // Arrange
        _loginModel.Input = new LoginRequest { Username = "admin", Password = "" };

        // Act
        var result = await _loginModel.OnPostAsync();

        // Assert
        var pageResult = Assert.IsType<PageResult>(result);
        Assert.Equal("Username and password are required", _loginModel.ErrorMessage);
    }
}

/// <summary>
/// Fake implementation of ISession for testing purposes.
/// This allows us to verify that session data was set correctly.
/// </summary>
public class FakeSession : ISession
{
    public Dictionary<string, byte[]> Data { get; } = new();

    public string Id => "test-session-id";

    public bool IsAvailable => true;

    public IEnumerable<string> Keys => Data.Keys;

    public void Clear() => Data.Clear();

    public Task CommitAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task LoadAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public void Remove(string key)
    {
        if (Data.ContainsKey(key))
            Data.Remove(key);
    }

    public void Set(string key, byte[] value)
    {
        Data[key] = value;
    }

    public bool TryGetValue(string key, out byte[] value)
    {
        if (Data.TryGetValue(key, out var data))
        {
            value = data;
            return true;
        }

        value = Array.Empty<byte>();
        return false;
    }
}
