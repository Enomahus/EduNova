using Infrastructure.Persistence.Entities;
using Infrastructure.Persistence.MariaDb.Contexts;
using Infrastructure.Persistence.MariaDb.Exceptions;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Persistence.MariaDb.Seeders;

public abstract class SeederBase(WritableDbContext context, UserManager<UserDao> userManager)
{
    public abstract Task SeedDataAsync();

    protected readonly WritableDbContext _context = context;
    protected readonly UserManager<UserDao> _userManager = userManager;

    protected async Task SeedUserAsync(
        string userName,
        string firstName,
        string lastName,
        string email,
        string phoneNumber,
        string password,
        IEnumerable<string> roles
    )
    {
        // Check if the user already exists
        var existingUser = await _userManager.FindByNameAsync(userName);
        if (existingUser == null)
        {
            var user = new UserDao()
            {
                UserName = userName,
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                PhoneNumber = phoneNumber,
            };
            await SeedUserAsync(user, password, roles);
        }
    }

    protected async Task SeedUserAsync(UserDao user, string password, IEnumerable<string> roles)
    {
        ArgumentNullException.ThrowIfNull(user, nameof(user));
        ArgumentNullException.ThrowIfNull(password, nameof(password));
        ArgumentNullException.ThrowIfNull(roles, nameof(roles));

        var existingUser = await _userManager.FindByNameAsync(user.UserName!);
        if (existingUser == null)
        {
            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                throw new DataSeedException($"Could not create user: {user.UserName}");
            }

            await _userManager.AddToRolesAsync(user, roles);
        }
    }
}
