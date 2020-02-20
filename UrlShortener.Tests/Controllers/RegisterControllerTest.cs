using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using UrlShortener.Controllers;
using UrlShortener.Domain.Models;
using UrlShortener.Services;
using Xunit;

namespace UrlShortener.Tests.Controllers
{
    public class RegisterControllerTest
    {
        private readonly Mock<IShortenerService> _mockService;
        private RegisterController _controller;


        public RegisterControllerTest()
        {
            _mockService = new Mock<IShortenerService>();
            InitializeUnauthorizedController();
        }


        [Fact]
        public void RegiterUrl_ModelNotValid()
        {

            /* _mockService.Setup(service => service.CreateRegisteredUrl(It.IsAny<RegisterUrlRequestBody>(), It.IsAny<string>()))
             .Returns(new RegisteredUrl() { LongUrl = "www.google.com", ShortUrl = "AStzHGv", AccountID = "12345" });*/
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
             .Returns(new RegisteredUrl() { LongUrl = "www.google.com", ShortUrl = "AStzHGv", AccountID = "12345" });

            _controller.ModelState.AddModelError("Url", "invalid data");
            RegisterUrlRequestBody request = new RegisterUrlRequestBody() { url = "www.google.com", redirectType = 300 }; //krivi redirectType

            var actionResult = _controller.RegisterUrl(request);
            Assert.IsType<UnauthorizedResult>((UnauthorizedResult)actionResult.Result);

        }

        private void InitializeUnauthorizedController() {
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var context = new DefaultHttpContext();
            var claims = new[] { new Claim(ClaimTypes.NameIdentifier, "12345")};
            var identity = new ClaimsIdentity(claims, "Basic");
            context.User = new GenericPrincipal(identity, new[] { "fake", "roles" });

            ControllerContext controllerContext = new ControllerContext();
            controllerContext.HttpContext = context;

            /*var fakeTenantId = "abcd";
            context.Request.Headers["User"] = fakeTenantId;*/
            mockHttpContextAccessor.Setup(_ => _.HttpContext).Returns(context);
            _controller = new RegisterController(_mockService.Object, mockHttpContextAccessor.Object);
            _controller.ControllerContext = controllerContext;
        }

        private void getAuthorizedControllerContext()
        {

        }
    }
}
