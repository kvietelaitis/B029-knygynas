using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;

namespace Knygynas.Services;

public class WorkerService
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public WorkerService(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<List<IdentityUser>> GetWorkersAsync()
    {
        var users = _userManager.Users.ToList();
        var workers = new List<IdentityUser>();

        foreach (var user in users)
        {
            if (await _userManager.IsInRoleAsync(user, "Worker"))
            {
                workers.Add(user);
            }
        }

        return workers;
    }

    public async Task<IdentityUser?> GetWorkerByIdAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return null;
        }

        return await _userManager.IsInRoleAsync(user, "Worker") ? user : null;
    }

    public async Task<(IdentityResult Result, string? GeneratedPassword)> CreateWorkerAsync(string email)
    {
        await EnsureRoleAsync("Worker");

        var generatedPassword = GeneratePassword();

        var user = new IdentityUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true
        };

        var createResult = await _userManager.CreateAsync(user, generatedPassword);
        if (!createResult.Succeeded)
        {
            return (createResult, null);
        }

        var roleResult = await _userManager.AddToRoleAsync(user, "Worker");
        if (!roleResult.Succeeded)
        {
            return (IdentityResult.Failed(roleResult.Errors.ToArray()), null);
        }

        return (createResult, generatedPassword);
    }

    public async Task<IdentityResult> UpdateWorkerAsync(string id, string email)
    {
        var user = await GetWorkerByIdAsync(id);
        if (user == null)
        {
            return IdentityResult.Failed(new IdentityError
            {
                Code = "WorkerNotFound",
                Description = "Worker not found."
            });
        }

        user.Email = email;
        user.UserName = email;

        return await _userManager.UpdateAsync(user);
    }

    public async Task<bool> DeleteWorkerAsync(string id)
    {
        var user = await GetWorkerByIdAsync(id);
        if (user == null)
        {
            return false;
        }

        var result = await _userManager.DeleteAsync(user);
        return result.Succeeded;
    }

    private async Task EnsureRoleAsync(string roleName)
    {
        if (!await _roleManager.RoleExistsAsync(roleName))
        {
            await _roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }

    private string GeneratePassword()
    {
        var options = _userManager.Options.Password;
        var requiredLength = Math.Max(options.RequiredLength, 12);
        var requiresLower = options.RequireLowercase;
        var requiresUpper = options.RequireUppercase;
        var requiresDigit = options.RequireDigit;
        var requiresNonAlphanumeric = options.RequireNonAlphanumeric;

        const string lowercase = "abcdefghijklmnopqrstuvwxyz";
        const string uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string digits = "0123456789";
        const string nonAlphanumeric = "!@$?_-%";

        var allCharsBuilder = new StringBuilder();
        if (requiresLower) allCharsBuilder.Append(lowercase);
        if (requiresUpper) allCharsBuilder.Append(uppercase);
        if (requiresDigit) allCharsBuilder.Append(digits);
        if (requiresNonAlphanumeric) allCharsBuilder.Append(nonAlphanumeric);

        if (allCharsBuilder.Length == 0)
        {
            allCharsBuilder.Append(lowercase).Append(uppercase).Append(digits);
        }

        var allChars = allCharsBuilder.ToString();
        var requiredUnique = Math.Max(options.RequiredUniqueChars, 1);

        for (var attempt = 0; attempt < 10; attempt++)
        {
            var chars = new List<char>(requiredLength);

            if (requiresLower) chars.Add(GetRandomChar(lowercase));
            if (requiresUpper) chars.Add(GetRandomChar(uppercase));
            if (requiresDigit) chars.Add(GetRandomChar(digits));
            if (requiresNonAlphanumeric) chars.Add(GetRandomChar(nonAlphanumeric));

            while (chars.Count < requiredLength)
            {
                chars.Add(GetRandomChar(allChars));
            }

            Shuffle(chars);

            var uniqueCount = new HashSet<char>(chars).Count;
            if (uniqueCount >= requiredUnique)
            {
                return new string(chars.ToArray());
            }
        }

        var fallback = new char[requiredLength];
        for (var i = 0; i < requiredLength; i++)
        {
            fallback[i] = GetRandomChar(allChars);
        }

        return new string(fallback);
    }

    private static char GetRandomChar(string chars)
    {
        return chars[RandomNumberGenerator.GetInt32(chars.Length)];
    }

    private static void Shuffle(List<char> chars)
    {
        for (var i = chars.Count - 1; i > 0; i--)
        {
            var j = RandomNumberGenerator.GetInt32(i + 1);
            (chars[i], chars[j]) = (chars[j], chars[i]);
        }
    }
}