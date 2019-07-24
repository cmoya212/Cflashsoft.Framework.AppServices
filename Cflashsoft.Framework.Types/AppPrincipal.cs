using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Security.Principal;

namespace Cflashsoft.Framework.Types
{
    /// <summary>
    /// Represents a generic principal with extended properties for app development.
    /// </summary>
    public class AppPrincipal : GenericPrincipal
    {
        /// <summary>
        /// Additional info for the user that this principal represents.
        /// </summary>
        public AppUserInfo UserInfo { get; protected set; }

        /// <summary>
        /// Original principal for reference, useful when the default thread principal is swapped out of the thread context.
        /// </summary>
        public IPrincipal OriginalPrincipal { get; protected set; }

        /// <summary>
        /// Initializes a new instance of AppPrincipal class.
        /// </summary>
        public AppPrincipal(IIdentity identity, string[] roles)
            : base(identity, roles)
        {
            this.UserInfo = new AppUserInfo(AppUserIdType.UserNameString, 0, Guid.Empty, identity.Name, identity.Name, string.Empty);
        }

        /// <summary>
        /// Initializes a new instance of AppPrincipal class.
        /// </summary>
        public AppPrincipal(int id, string role)
            : base(new GenericIdentity(id.ToString()), new string[] { role })
        {
            this.UserInfo = new AppUserInfo(AppUserIdType.IdInteger, id, Guid.Empty, id.ToString(), "User " + id.ToString(), string.Empty);
        }

        /// <summary>
        /// Initializes a new instance of AppPrincipal class.
        /// </summary>
        public AppPrincipal(string userName, string role)
            : base(new GenericIdentity(userName), new string[] { role })
        {
            this.UserInfo = new AppUserInfo(AppUserIdType.UserNameString, 0, Guid.Empty, userName, userName, string.Empty);
        }

        /// <summary>
        /// Initializes a new instance of AppPrincipal class.
        /// </summary>
        public AppPrincipal(int id, string[] roles)
            : base(new GenericIdentity(id.ToString()), roles)
        {
            this.UserInfo = new AppUserInfo(AppUserIdType.IdInteger, id, Guid.Empty, id.ToString(), "User " + id.ToString(), string.Empty);
        }

        /// <summary>
        /// Initializes a new instance of AppPrincipal class.
        /// </summary>
        public AppPrincipal(string userName, string[] roles)
            : base(new GenericIdentity(userName), roles)
        {
            this.UserInfo = new AppUserInfo(AppUserIdType.UserNameString, 0, Guid.Empty, userName, userName, string.Empty);
        }

        /// <summary>
        /// Initializes a new instance of AppPrincipal class.
        /// </summary>
        public AppPrincipal(int id, ClaimsPrincipal principal)
            : base(principal.Claims.Any(c => c.Type == "http://schemas.microsoft.com/identity/claims/objectidentifier") ? new GenericIdentity(principal.Claims.Where(c => c.Type == "http://schemas.microsoft.com/identity/claims/objectidentifier").First().Value) : new GenericIdentity(id.ToString()), principal.Claims.Where(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Select(c => c.Value).ToArray())
        {
            AppUserIdType appUserIdType = AppUserIdType.IdInteger;

            Guid oId = Guid.Empty;

            Claim oIdClaim = principal.Claims.Where(c => c.Type == "http://schemas.microsoft.com/identity/claims/objectidentifier").FirstOrDefault();

            if (oIdClaim != null && !string.IsNullOrEmpty(oIdClaim.Value))
            {
                try
                {
                    oId = Guid.Parse(oIdClaim.Value);
                    appUserIdType = AppUserIdType.OidGuid;
                }
                catch
                {
                    //do nothing
                }
            }

            string displayName = string.Empty;

            Claim nameClaim = principal.Claims.Where(c => c.Type == "name").FirstOrDefault();

            if (nameClaim != null && !string.IsNullOrEmpty(nameClaim.Value))
            {
                displayName = nameClaim.Value;
            }
            else
            {
                displayName = "User " + id.ToString();
            }

            string primaryEmail = string.Empty;

            Claim emailClaim = principal.Claims.Where(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/email").FirstOrDefault();

            if (emailClaim != null && !string.IsNullOrEmpty(emailClaim.Value))
            {
                primaryEmail = emailClaim.Value;
            }

            this.UserInfo = new AppUserInfo(appUserIdType, id, oId, this.Identity.Name, displayName, primaryEmail);
            this.OriginalPrincipal = principal;
        }

        /// <summary>
        /// Initializes a new instance of AppPrincipal class.
        /// </summary>
        public AppPrincipal(int id, Guid oId, ClaimsPrincipal principal)
            : base(oId != Guid.Empty ? new GenericIdentity(oId.ToString()) : new GenericIdentity(id.ToString()), principal.Claims.Where(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Select(c => c.Value).ToArray())
        {
            AppUserIdType appUserIdType = oId != Guid.Empty ? AppUserIdType.OidGuid : AppUserIdType.IdInteger;

            string displayName = string.Empty;

            Claim nameClaim = principal.Claims.Where(c => c.Type == "name").FirstOrDefault();

            if (nameClaim != null && !string.IsNullOrEmpty(nameClaim.Value))
            {
                displayName = nameClaim.Value;
            }
            {
                displayName = "User " + id.ToString();
            }

            string primaryEmail = string.Empty;

            Claim emailClaim = principal.Claims.Where(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/email").FirstOrDefault();

            if (emailClaim != null && !string.IsNullOrEmpty(emailClaim.Value))
            {
                primaryEmail = emailClaim.Value;
            }

            this.UserInfo = new AppUserInfo(appUserIdType, id, oId, this.Identity.Name, displayName, primaryEmail);
            this.OriginalPrincipal = principal;
        }

        /// <summary>
        /// Initializes a new instance of AppPrincipal class.
        /// </summary>
        public AppPrincipal(int id, Guid oId, string primaryEmail, ClaimsPrincipal principal)
            : base(oId != Guid.Empty ? new GenericIdentity(oId.ToString()) : new GenericIdentity(id.ToString()), principal.Claims.Where(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Select(c => c.Value).ToArray())
        {
            AppUserIdType appUserIdType = oId != Guid.Empty ? AppUserIdType.OidGuid : AppUserIdType.IdInteger;

            string displayName = string.Empty;

            Claim nameClaim = principal.Claims.Where(c => c.Type == "name").FirstOrDefault();

            if (nameClaim != null && !string.IsNullOrEmpty(nameClaim.Value))
            {
                displayName = nameClaim.Value;
            }
            else
            {
                displayName = "User " + id.ToString();
            }

            this.UserInfo = new AppUserInfo(appUserIdType, id, oId, this.Identity.Name, displayName, primaryEmail ?? string.Empty);
            this.OriginalPrincipal = principal;
        }

        /// <summary>
        /// Initializes a new instance of AppPrincipal class.
        /// </summary>
        public AppPrincipal(int id, Guid oId, string displayName, string primaryEmail, string role, IPrincipal principal)
            : base(oId != Guid.Empty ? new GenericIdentity(oId.ToString()) : new GenericIdentity(id.ToString()), new string[] { role })
        {
            this.UserInfo = new AppUserInfo(oId != Guid.Empty ? AppUserIdType.OidGuid : AppUserIdType.IdInteger, id, oId, this.Identity.Name, displayName, primaryEmail ?? string.Empty);
            this.OriginalPrincipal = principal;
        }

        /// <summary>
        /// Initializes a new instance of AppPrincipal class.
        /// </summary>
        public AppPrincipal(int id, Guid oId, string displayName, string primaryEmail, string[] roles, IPrincipal principal)
            : base(oId != Guid.Empty ? new GenericIdentity(oId.ToString()) : new GenericIdentity(id.ToString()), roles)
        {
            this.UserInfo = new AppUserInfo(oId != Guid.Empty ? AppUserIdType.OidGuid : AppUserIdType.IdInteger, id, oId, this.Identity.Name, displayName, primaryEmail ?? string.Empty);
            this.OriginalPrincipal = principal;
        }

        /// <summary>
        /// Determines whether the current principal belongs the specified roles.
        /// </summary>
        public bool IsInRoles(IEnumerable<string> roles)
        {
            bool result = false;

            foreach (string role in roles)
            {
                if (IsInRole(role))
                {
                    result = true;
                    break;
                }
            }

            return result;
        }
    }
}
