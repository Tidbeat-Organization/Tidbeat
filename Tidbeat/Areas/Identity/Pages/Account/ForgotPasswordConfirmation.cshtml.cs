// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Tidbeat.Areas.Identity.Pages.Account
{
    /// <summary>
    /// The model class for the register confirmation page.
    /// </summary>
    [AllowAnonymous]
    public class ForgotPasswordConfirmation : PageModel
    {
        /// <summary>
        /// The get method for the register confirmation page.
        /// </summary>
        public void OnGet()
        {
        }
    }
}
