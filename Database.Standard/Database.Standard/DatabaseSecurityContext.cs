using System;
using System.Security.Principal;
using Interstates.Control.Framework.Security;

namespace Interstates.Control.Database
{
    /// <summary>
    /// Used when a SecurityContext to database profile is needed.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The <see cref="SecurityContext"/> An implementation of the
    /// <see cref="SecurityContext"/> base class. It is used where a <see cref="SecurityContext"/>
    /// is required for connecting to a Sql Server.
    /// </para>
    /// </remarks>
    public sealed class DatabaseSecurityContext : SecurityContext
    {
        private bool _useWindowsAuthentication;
        private string _userName;
        private string _password;
        private WindowsSecurityContext _securityContext;

        /// <summary>
        /// Sql Server default constructor.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Constructor to connect to a Sql Server using windows authentication. The currently logged on user is used to logon.
        /// </para>
        /// </remarks>
        public DatabaseSecurityContext()
        {
            _useWindowsAuthentication = true;
        }

        /// <summary>
        /// Sql Server constructor to use the windows credentials of the currently logged on user to connect.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Constructor to connect to a Sql Server using windows authentication. The currently logged on user is used to logon.
        /// </para>
        /// </remarks>
        public DatabaseSecurityContext(WindowsSecurityContext securityContext)
        {
            _useWindowsAuthentication = true;
            _securityContext = securityContext;
        }
        
        /// <summary>
        /// Sql Server constructor to use the windows credentials of the specified user to connect.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Constructor to connect using windows authentication.
        /// </para>
        /// </remarks>
        public DatabaseSecurityContext(string userName, string domain, string password)
        {
            _useWindowsAuthentication = true;
            _securityContext = new WindowsSecurityContext();
            _securityContext.UserName = userName;
            _securityContext.DomainName = domain;
            _securityContext.Password = password;
        }

        /// <summary>
        /// Constructor to use a Sql Server user account.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Constructor to connect using Sql Server user authentication.
        /// </para>
        /// </remarks>
        public DatabaseSecurityContext(string userName, string password)
        {
            _useWindowsAuthentication = false;
            _userName = userName;
            _password = password;
        }

        /// <summary>
        /// Impersonate this SecurityContext
        /// </summary>
        /// <param name="state">State supplied by the caller</param>
        /// <returns><c>null</c></returns>
        /// <remarks>
        /// <para>
        /// No impersonation is done and <c>null</c> is always returned.
        /// </para>
        /// </remarks>
        public override IDisposable Impersonate(object state)
        {
            DatabaseProfile profile = state as DatabaseProfile;

            if (profile == null)
                throw new ArgumentException("state", "The state must be a DBProfile object.");

            // Here we want to impersonate the user that is trying to connect.
            // If they want to use windows authentication, we need to impersonate the user and set the connection string attribute.
            if (_useWindowsAuthentication)
            {
                // Tell the profile we are using windows impersonation
                if (profile.ProviderType == "SQL")
                {
                    profile.ConnectionStringBuilder.Add("Integrated Security", "SSPI");
                }
                if (_securityContext == null)
                    return new DisposableImpersonationContext(profile, new WindowsSecurityContext().Impersonate(state));
                else
                    return new DisposableImpersonationContext(profile, _securityContext.Impersonate(state));
            }
            else
            {
                profile.ConnectionStringBuilder.Add("UID", _userName);
                profile.ConnectionStringBuilder.Add("PWD", _password);
                return new DisposableImpersonationContext(profile, null);
            }
        }


        #region DisposableImpersonationContext class

        /// <summary>
        /// Adds <see cref="IDisposable"/> to <see cref="WindowsImpersonationContext"/>
        /// </summary>
        /// <remarks>
        /// <para>
        /// Helper class to expose the <see cref="WindowsImpersonationContext"/>
        /// through the <see cref="IDisposable"/> interface.
        /// </para>
        /// </remarks>
        private sealed class DisposableImpersonationContext : IDisposable
        {
            private readonly IDisposable _contextImpersonation;
            private readonly DatabaseProfile _profile;

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="profile">The database profile to use impersonation for.</param>
            /// <param name="impersonationContext">the impersonation context being wrapped</param>
            /// <remarks>
            /// <para>
            /// Constructor
            /// </para>
            /// </remarks>
            public DisposableImpersonationContext(DatabaseProfile profile, IDisposable impersonationContext)
            {
                _profile = profile;
                _contextImpersonation = impersonationContext;
            }

            /// <summary>
            /// Revert the impersonation
            /// </summary>
            /// <remarks>
            /// <para>
            /// Revert the impersonation
            /// </para>
            /// </remarks>
            public void Dispose()
            {
                if (_profile != null)
                {
                    if (_profile.ProviderType == "SQL")
                    {
                        _profile.ConnectionStringBuilder.Remove("UID");
                        _profile.ConnectionStringBuilder.Remove("User");
                        _profile.ConnectionStringBuilder.Remove("PWD");
                        _profile.ConnectionStringBuilder.Remove("Password");
                    }
                }
                if (_contextImpersonation != null)
                {
                    _contextImpersonation.Dispose();
                    if (_profile.ProviderType == "SQL")
                    {
                        _profile.ConnectionStringBuilder.Remove("Integrated Security");
                    }
                }
            }
        }

        #endregion    
    }

}
