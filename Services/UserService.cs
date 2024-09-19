using Microsoft.AspNetCore.Identity;
using OnAccount.Areas.Identity.Data;
using OnAccount.Models;
using System.Security.Claims;

namespace OnAccount.Services;
public class UserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public AppUserModel GetUser()
    {
        var Id = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        AppUserModel model = GetUserById(Id);
        return model;
    }


    private AppUserModel GetUserById(string userId)
    {

        DbConnectorService dbConnectorService = new DbConnectorService();
        AppUserModel user = dbConnectorService.GetUserDetailsById(userId);
        return user;
    }
}