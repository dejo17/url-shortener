using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using UrlShortener.Controllers;
using UrlShortener.Domain.Models;
using UrlShortener.Services;
using Xunit;

namespace UrlShortener.Tests
{
    public class AccountControllerTest
    {

        private readonly Mock<IAccountService> _mockService;
        private readonly AccountController _controller;

        
        public AccountControllerTest()
        {
            _mockService = new Mock<IAccountService>();
            _controller = new AccountController(_mockService.Object);
        }

        [Fact]
        public void GetAccount_AllParametersOk_AccountExists()
        {
            _mockService.Setup(service => service.GetAccount(It.IsAny<string>()))
              .Returns(new Account { AccountID = "12345", Password = "dummy" });

            AccountRequestBody request = new AccountRequestBody() { AccountId = "12345"};
           
            var actionResult = _controller.GetAccount(request);       
            Assert.IsType<ActionResult<AccountResponseBody>>(actionResult);
            
            ConflictObjectResult conflictResult = (ConflictObjectResult)actionResult.Result;
            Assert.IsType<AccountResponseBody>(conflictResult.Value);

            AccountResponseBody responseBody = (AccountResponseBody)conflictResult.Value;
            Assert.False(responseBody.success);
            Assert.Null(responseBody.password);
            Assert.Equal("Account exists", responseBody.description);

        }

        [Fact]
        public void GetAccount_AllParametersOk_AccountCreated()
        {
            _mockService.Setup(service => service.GetAccount(It.IsAny<string>()))
              .Returns( (Account) null );

            _mockService.Setup(service => service.CreateAccount(It.IsAny<string>()))
              .Returns(new Account { AccountID = "12345", Password = "dummy" });

            AccountRequestBody request = new AccountRequestBody() { AccountId = "12345" };

            var actionResult = _controller.GetAccount(request);
            Assert.IsType<ActionResult<AccountResponseBody>>(actionResult);

            CreatedResult createdResult = (CreatedResult)actionResult.Result;
            Assert.IsType<AccountResponseBody>(createdResult.Value);

            AccountResponseBody responseBody = (AccountResponseBody)createdResult.Value;
            Assert.True(responseBody.success);
            Assert.Equal("dummy",responseBody.password);
            Assert.Equal("Account created successfully", responseBody.description);

        }

        [Fact]
        public void GetAccount_ModelNotValid()
        {
            _mockService.Setup(service => service.GetAccount(It.IsAny<string>()))
             .Returns((Account)null);
           
            _controller.ModelState.AddModelError("Url", "invalid data");
            AccountRequestBody request = new AccountRequestBody() { AccountId = "12" };
            
            var actionResult = _controller.GetAccount(request);
            Assert.IsType<BadRequestResult>((BadRequestResult)actionResult.Result);

        }
    }
}
