using DailyQuizAPI.Features.Users;
using Microsoft.AspNetCore.Identity;

namespace DailyQuizAPI.Features.Users.Create;

public class CreateUserCommandHandler(UserManager<User> userManager)
{
    private readonly UserManager<User> _userManager = userManager;

    public async Task Handle(CreateUserCommand request)
    {
        var user = new User
        {
            UserName = request.UserName,
            Email = request.Email
        };
        var result = await _userManager.CreateAsync(user, request.Password).ConfigureAwait(false);
        if (!result.Succeeded)
            throw new InvalidOperationException(string.Join(", ", result.Errors.Select(e => e.Description)));
    }
}