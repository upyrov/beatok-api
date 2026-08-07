using Beatok.API.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Beatok.API.Attributes;

public class AdminAttribute: TypeFilterAttribute
{
    public AdminAttribute(): base(typeof(AdminAuthorizationFilter)) { }
}