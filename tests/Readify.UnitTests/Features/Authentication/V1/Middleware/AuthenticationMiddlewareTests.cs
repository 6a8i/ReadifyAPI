using FluentResults;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Moq;
using Readify.API.Common.Auth;
using Readify.Application.Features.Authentications.V1;
using Readify.Application.Features.Authentications.V1.Models.Response;

namespace Readify.UnitTests.Features.Authentication.V1.Middleware
{
    public class AuthenticationMiddlewareTests
    {
        private readonly Mock<IAuthenticationAppServices> _mockAuthenticationAppServices;
        private readonly Mock<IServiceProvider> _mockServiceProvider;
        private readonly Mock<RequestDelegate> _mockNext;
        private readonly AuthMiddleware _middleware;

        public AuthenticationMiddlewareTests()
        {
            _mockAuthenticationAppServices = new Mock<IAuthenticationAppServices>();
            _mockServiceProvider = new Mock<IServiceProvider>();
            _mockNext = new Mock<RequestDelegate>();
            _middleware = new AuthMiddleware(_mockNext.Object);

            _mockServiceProvider.Setup(sp => sp.GetService(typeof(IAuthenticationAppServices)))
                .Returns(_mockAuthenticationAppServices.Object);
        }

        [Fact]
        public async Task Invoke_AllowsAnonymousEndpoint()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.RequestServices = _mockServiceProvider.Object;
            var endpoint = new Endpoint(null, new EndpointMetadataCollection(new AllowAnonymousAttribute()), null);
            context.SetEndpoint(endpoint);

            // Act
            await _middleware.Invoke(context);

            // Assert
            _mockNext.Verify(next => next(context), Times.Once);
        }

        [Fact]
        public async Task Invoke_ReturnsUnauthorized_WhenTokenIsMissing()
        {
            // Arrange
            var context = new DefaultHttpContext
            {
                RequestServices = _mockServiceProvider.Object,
                Request = { Headers = { ["Authorization"] = string.Empty } }
            };

            var endpoint = new Endpoint(null, null, "/user");
            context.SetEndpoint(endpoint);

            // Act
            await _middleware.Invoke(context);

            // Assert
            Assert.Equal(StatusCodes.Status401Unauthorized, context.Response.StatusCode);
        }

        [Fact]
        public async Task Invoke_ReturnsUnauthorized_WhenTokenIsInvalid()
        {
            // Arrange
            var context = new DefaultHttpContext
            {
                RequestServices = _mockServiceProvider.Object,
                Request = { Headers = { ["Authorization"] = "invalid-token" } }
            };

            var endpoint = new Endpoint(null, null, "/user");
            context.SetEndpoint(endpoint);

            // Act
            await _middleware.Invoke(context);

            // Assert
            Assert.Equal(StatusCodes.Status401Unauthorized, context.Response.StatusCode);
        }

        [Fact]
        public async Task Invoke_ReturnsUnauthorized_WhenTokenIsNotFound()
        {
            // Arrange
            var context = new DefaultHttpContext
            {
                RequestServices = _mockServiceProvider.Object,
                Request = { Headers = { ["Authorization"] = "00000000-0000-0000-0000-000000000000" } }
            };

            var endpoint = new Endpoint(null, null, "/user");
            context.SetEndpoint(endpoint);

            _mockAuthenticationAppServices.Setup(service => service.GetTokenByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(Result.Fail<AuthResponse>("Unauthorized"));
            
            // Act
            await _middleware.Invoke(context);

            // Assert
            Assert.Equal(StatusCodes.Status401Unauthorized, context.Response.StatusCode);
        }

        [Fact]
        public async Task Invoke_ReturnsUnauthorized_WhenTokenHasExpired()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.RequestServices = _mockServiceProvider.Object;
            context.Request.Headers.Authorization = "00000000-0000-0000-0000-000000000000";

            var expiredAuthResponse = new AuthResponse { TokenHasExpired = true };

            var endpoint = new Endpoint(null, null, "/user");
            context.SetEndpoint(endpoint);

            _mockAuthenticationAppServices.Setup(service => service.GetTokenByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(Result.Ok(expiredAuthResponse));


            // Act
            await _middleware.Invoke(context);

            // Assert
            Assert.Equal(StatusCodes.Status401Unauthorized, context.Response.StatusCode);
        }

        [Fact]
        public async Task Invoke_ProcessesRequest_WhenTokenIsValid()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.RequestServices = _mockServiceProvider.Object;
            context.Request.Headers.Authorization = "00000000-0000-0000-0000-000000000000";
            var validAuthResponse = new AuthResponse { TokenHasExpired = false };
            _mockAuthenticationAppServices.Setup(service => service.GetTokenByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(Result.Ok(validAuthResponse));
            context.RequestServices = Mock.Of<IServiceProvider>(sp =>
                sp.GetService(typeof(IAuthenticationAppServices)) == _mockAuthenticationAppServices.Object);

            // Act
            await _middleware.Invoke(context);

            // Assert
            _mockNext.Verify(next => next(context), Times.Once);
        }
    }
}
