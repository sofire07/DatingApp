using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Model.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static string GetUsername(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.Name)?.Value;
        }
    }
}
