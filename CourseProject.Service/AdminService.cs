using System.Security.Cryptography;
using CourseProject.Dal;
using CourseProject.Dal.Entities;
using CourseProject.Domain.Abstractions;
using CourseProject.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Wiry.Base32;

namespace CourseProject.Service;

public class AdminService:IAdminService
{
    private readonly UserManager<User> _userManager;
    private readonly ApplicationContext _context;

    public AdminService(UserManager<User> userManager, ApplicationContext context)
    {
        _userManager = userManager;
        _context = context;
    }
    
    public async Task<IEnumerable<UserModel>> GetUsers()
    {
        return await (from u in _context.Users
            join ur in _context.UserRoles on u.Id equals ur.UserId into userRoles
            from ur in userRoles.DefaultIfEmpty()
            join r in _context.Roles on ur.RoleId equals r.Id into roles
            from r in roles.DefaultIfEmpty()
            select new UserModel
            {
                Id = u.Id,
                UserName = u.UserName,
                Email = u.Email,
                Role = r.Name == "Admin" ? "Admin" : "User",
                Status = u.LockoutEnd < DateTime.Now.ToUniversalTime() || u.LockoutEnd == null ? "Active" : "Blocked"
            }).ToListAsync();
    }

    public async Task DeleteUsers(string[] userIds)
    {
        foreach (var id in userIds)
        {
            var user = await _userManager.FindByIdAsync(id);
            await _userManager.DeleteAsync(user);
        }
    }

    public async Task ChangeUserStatus(string[] userIds, bool isActive)
    {
        var users = await _context.Users.Where(u => userIds.Contains(u.Id)).ToListAsync();
        if (isActive)
        {
            UnBlockUsers(users);
        }
        else
        {
            BlockUsers(users);
        }
        await _context.SaveChangesAsync();
    }
    
    private void BlockUsers(List<User> users)
    {
        users.ForEach(u =>
        {
            u.LockoutEnd = DateTime.MaxValue;
            u.SecurityStamp = GenerateSecurityStamp();
        });
    }

    private void UnBlockUsers(List<User> users)
    {
        users.ForEach(u => { u.LockoutEnd = null; });
    }

    private string GenerateSecurityStamp()
    {
        byte[] bytes = new byte[20];
        RandomNumberGenerator.Fill(bytes);
        return Base32Encoding.Standard.GetString(bytes);
    }
    
    public async Task SetRole(string[] userIds, string role)
    {
        var users = await GetIdentityUsers(userIds);
        foreach (var user in users.Where(user => userIds.Contains(user.Id)))
        {
            await UpdateUserRole(user, role);
        }
    }

    private async Task UpdateUserRole(User user, string role)
    {
        if(role=="Admin")
        {
            await AddToAdmins(user);
        }
        else
        {
            await RemoveFromAdmins(user);
        }
    }

    private async Task AddToAdmins(User user)
    {
        await _userManager.AddToRoleAsync(user, "Admin");
        await _userManager.RemoveFromRoleAsync(user, "User");
    }

    private async Task RemoveFromAdmins(User user)
    {
        await _userManager.RemoveFromRoleAsync(user, "Admin");
        await _userManager.AddToRoleAsync(user, "User");
    }
    
    private async Task<List<User>> GetIdentityUsers(string[] id)
    {
        return await _userManager.Users.Where(u => id.Contains(u.Id)).ToListAsync();
    }
}