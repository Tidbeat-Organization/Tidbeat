// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Tidbeat.Areas.Identity.Pages.Account.Manage
{
    /// <summary>
    /// The model class for the manage nav pages.
    /// </summary>
    public static class ManageNavPages
    {
        /// <summary>
        /// Defines the Edit Photo page.
        /// </summary>
        public static string EditPhoto => "EditPhoto";
        /// <summary>
        /// Defines the Index page.
        /// </summary>
        public static string Index => "Index";

        /// <summary>
        /// Defines the Email page.
        /// </summary>
        public static string Email => "Email";

        /// <summary>
        /// Defines the ChangePassword page.
        /// </summary>
        public static string ChangePassword => "ChangePassword";

        /// <summary>
        /// Defines the DownloadPersonalData page.
        /// </summary>
        public static string DownloadPersonalData => "DownloadPersonalData";

        /// <summary>
        /// Defines the DeletePersonalData page.
        /// </summary>
        public static string DeletePersonalData => "DeletePersonalData";

        /// <summary>
        /// Defines the ExternalLogins page.
        /// </summary>
        public static string ExternalLogins => "ExternalLogins";

        /// <summary>
        /// Defines the PersonalData page.
        /// </summary>
        public static string PersonalData => "PersonalData";

        /// <summary>
        /// Defines the TwoFactorAuthentication page.
        /// </summary>
        public static string TwoFactorAuthentication => "TwoFactorAuthentication";

        /// <summary>
        /// Gets the nav page class for the EditPhoto.
        /// </summary>
        /// <param name="viewContext">The view context.</param>
        /// <returns>The nav page class for the EditPhoto.</returns>
        public static string EditPhotoNavClass(ViewContext viewContext) => PageNavClass(viewContext, EditPhoto);
        /// <summary>
        /// Gets the nav page class for the Index.
        /// </summary>
        /// <param name="viewContext">The view context.</param>
        /// <returns>The nav page class for the Index.</returns>
        public static string IndexNavClass(ViewContext viewContext) => PageNavClass(viewContext, Index);

        /// <summary>
        /// Gets the nav page class for the Email.
        /// </summary>
        /// <param name="viewContext">The view context.</param>
        /// <returns>The nav page class for the Email.</returns>
        public static string EmailNavClass(ViewContext viewContext) => PageNavClass(viewContext, Email);

        /// <summary>
        /// Gets the nav page class for the ChangePassword.
        /// </summary>
        /// <param name="viewContext">The view context.</param>
        /// <returns>The nav page class for the ChangePassword.</returns>
        public static string ChangePasswordNavClass(ViewContext viewContext) => PageNavClass(viewContext, ChangePassword);

        /// <summary>
        /// Gets the nav page class for the DownloadPersonalData.
        /// </summary>
        /// <param name="viewContext">The view context.</param>
        /// <returns>The nav page class for the DownloadPersonalData.</returns>
        public static string DownloadPersonalDataNavClass(ViewContext viewContext) => PageNavClass(viewContext, DownloadPersonalData);

        /// <summary>
        /// Gets the nav page class for the DeletePersonalData.
        /// </summary>
        /// <param name="viewContext">The view context.</param>
        /// <returns>The nav page class for the DeletePersonalData.</returns>
        public static string DeletePersonalDataNavClass(ViewContext viewContext) => PageNavClass(viewContext, DeletePersonalData);

        /// <summary>
        /// Gets the nav page class for the ExternalLogins.
        /// </summary>
        /// <param name="viewContext">The view context.</param>
        /// <returns>The nav page class for the ExternalLogins.</returns>
        public static string ExternalLoginsNavClass(ViewContext viewContext) => PageNavClass(viewContext, ExternalLogins);

        /// <summary>
        /// Gets the nav page class for the PersonalData.
        /// </summary>
        /// <param name="viewContext">The view context.</param>
        /// <returns>The nav page class for the PersonalData.</returns>
        public static string PersonalDataNavClass(ViewContext viewContext) => PageNavClass(viewContext, PersonalData);

        /// <summary>
        /// Gets the nav page class for the TwoFactorAuthentication.
        /// </summary>
        /// <param name="viewContext">The view context.</param>
        /// <returns>The nav page class for the TwoFactorAuthentication.</returns>
        public static string TwoFactorAuthenticationNavClass(ViewContext viewContext) => PageNavClass(viewContext, TwoFactorAuthentication);

        /// <summary>
        /// Generates the PageNavClass.
        /// </summary>
        /// <param name="viewContext">The view context.</param>
        /// <param name="page">The page.</param>
        /// <returns>The nav page class for the EditPhoto.</returns>
        public static string PageNavClass(ViewContext viewContext, string page)
        {
            var activePage = viewContext.ViewData["ActivePage"] as string
                ?? System.IO.Path.GetFileNameWithoutExtension(viewContext.ActionDescriptor.DisplayName);
            return string.Equals(activePage, page, StringComparison.OrdinalIgnoreCase) ? "active" : null;
        }
    }
}