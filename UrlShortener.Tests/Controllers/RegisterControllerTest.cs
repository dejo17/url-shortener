using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using UrlShortener.Api.Model;
using UrlShortener.Controllers;
using UrlShortener.Domain.Models;
using UrlShortener.Services;
using Xunit;

namespace UrlShortener.Tests.Controllers
{
    public class RegisterControllerTest
    {
        private readonly Mock<IShortenerService> _mockService;
        private UrlController _controller;


        public RegisterControllerTest()
        {
            _mockService = new Mock<IShortenerService>();
            InitializeControllerWithUnauthorizedContext();
        }

        [Fact]
        public void RegiterUrl_OkResult()
        {
            InitializeControllerWithAuthorizedContext();
            _mockService.Setup(service => service.CreateRegisteredUrl(It.IsAny<RegisterUrlRequestBody>(), It.IsAny<string>()))
             .Returns(new RegisteredUrl() { LongUrl = "www.google.com", ShortUrl = "AStzHGv", AccountID = "54321" });

            RegisterUrlRequestBody request = new RegisterUrlRequestBody() { url = "www.google.com", redirectType = 301 };

            var actionResult = _controller.RegisterUrl(request);
            Assert.IsType<ActionResult<RegisterUrlResponseBody>>(actionResult);

            OkObjectResult okResult = (OkObjectResult)actionResult.Result;
            Assert.IsType<OkObjectResult>((OkObjectResult)actionResult.Result);

            Assert.IsType<RegisterUrlResponseBody>(okResult.Value);

            RegisterUrlResponseBody response = (RegisterUrlResponseBody)okResult.Value;
            Assert.Equal("AStzHGv", response.shortUrl);

        }

        [Fact]
        public void RegiterUrl_ModelNotValid()
        {
            InitializeControllerWithAuthorizedContext();

            _mockService.Setup(service => service.CreateRegisteredUrl(It.IsAny<RegisterUrlRequestBody>(), It.IsAny<string>()))
            .Returns((RegisteredUrl)null);

            _controller.ModelState.AddModelError("Url", "invalid data");
            RegisterUrlRequestBody request = new RegisterUrlRequestBody() { url = "www.google.com" , redirectType = 300}; //krivi redirectType

            var actionResult = _controller.RegisterUrl(request);
            Assert.IsType<BadRequestResult>((BadRequestResult)actionResult.Result);

        }
        [Fact]
        public void RegiterUrl_Unauthorized()
        {

             _mockService.Setup(service => service.CreateRegisteredUrl(It.IsAny<RegisterUrlRequestBody>(), It.IsAny<string>()))
             .Returns(new RegisteredUrl() { LongUrl = "www.google.com", ShortUrl = "AStzHGv", AccountID = "54321" });

            RegisterUrlRequestBody request = new RegisterUrlRequestBody() { url = "www.google.com", redirectType = 300 }; //krivi redirectType

            var actionResult = _controller.RegisterUrl(request);
            Assert.IsType<UnauthorizedResult>((UnauthorizedResult)actionResult.Result);

        }

        private void InitializeControllerWithUnauthorizedContext() {
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var context = new DefaultHttpContext();
            var claims = new[] { new Claim(ClaimTypes.Name, "fake")}; //fake claim, treba biti type NameIdentifier
            var identity = new ClaimsIdentity(claims, "Basic");
            context.User = new GenericPrincipal(identity, new[] { "fake", "roles" });

            ControllerContext controllerContext = new ControllerContext();
            controllerContext.HttpContext = context;

            mockHttpContextAccessor.Setup(_ => _.HttpContext).Returns(context);
            _controller = new UrlController(_mockService.Object, mockHttpContextAccessor.Object);
            _controller.ControllerContext = controllerContext;
        }

        private void InitializeControllerWithAuthorizedContext()
        {
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var context = new DefaultHttpContext();
            var claims = new[] { new Claim(ClaimTypes.NameIdentifier, "12345") }; 
            var identity = new ClaimsIdentity(claims, "Basic");
            context.User = new GenericPrincipal(identity, new[] { "fake", "roles" });

            ControllerContext controllerContext = new ControllerContext();
            controllerContext.HttpContext = context;

            mockHttpContextAccessor.Setup(_ => _.HttpContext).Returns(context);
            _controller = new UrlController(_mockService.Object, mockHttpContextAccessor.Object);
            _controller.ControllerContext = controllerContext;
        }
    }
}
