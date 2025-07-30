using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace DailyQuizAPI.Features.Crosscutting.Users;

public class RollbackTokenProvider<TUser>(IDataProtectionProvider dataProtectionProvider, IOptions<RollbackTokenProviderOptions> options, ILogger<DataProtectorTokenProvider<TUser>> logger) : DataProtectorTokenProvider<TUser>(dataProtectionProvider, options, logger) where TUser : class
{
}
