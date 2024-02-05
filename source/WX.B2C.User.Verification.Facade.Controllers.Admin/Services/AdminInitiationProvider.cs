using System;
using Microsoft.AspNetCore.Http;
using Serilog;
using WX.B2C.User.Verification.Core.Contracts.Dtos;

namespace WX.B2C.User.Verification.Facade.Controllers.Admin.Services
{
    public interface IInitiationProvider
    {
        string GetCurrentInitiator();

        InitiationDto Create(string reason);
    }

    internal class AdminInitiationProvider : IInitiationProvider
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ILogger _logger;

        public AdminInitiationProvider(IHttpContextAccessor contextAccessor, ILogger logger)
        {
            _contextAccessor = contextAccessor ?? throw new ArgumentNullException(nameof(contextAccessor));
            _logger = logger?.ForContext<AdminInitiationProvider>() ?? throw new ArgumentNullException(nameof(logger));
        }

        public string GetCurrentInitiator()
        {
            var adminEmail = _contextAccessor.HttpContext?.User?.Identity?.Name;
            if (adminEmail == null)
                _logger.Warning("Can not resolve admin email");

            return adminEmail;
        }

        public InitiationDto Create(string reason) => InitiationDto.CreateAdmin(GetCurrentInitiator(), reason);
    }
}