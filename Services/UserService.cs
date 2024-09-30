using Microsoft.AspNetCore.Identity;
using oa.Areas.Identity.Data;
using oa.Models;
using System.Security.Claims;

namespace oa.Services;
public class UserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private DbConnectorService _connectorService;

    public UserService(IHttpContextAccessor httpContextAccessor, DbConnectorService dbConnectorService)
    {
        _httpContextAccessor = httpContextAccessor;
        _connectorService = dbConnectorService;
    }

    public AppUserModel GetUser()
    {
        var Id = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        AppUserModel model = GetUserById(Id);
        return model;
    }


    private AppUserModel GetUserById(string userId)
    {

        AppUserModel user = _connectorService.GetUserDetailsById(userId);
        return user;
    }
}