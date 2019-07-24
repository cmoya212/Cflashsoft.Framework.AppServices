using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cflashsoft.Framework.Types;

namespace Cflashsoft.Framework.AppServices
{
    /// <summary>
    /// Represents the base class for UoW service classes that will contain the actual UoW methods. 
    /// </summary>
    public abstract class AppServicesBase
    {
        private AppContextBase _appContext = null;

        /// <summary>
        /// Shared application services context.
        /// </summary>
        public AppContextBase AppContext
        {
            get
            {
                return _appContext;
            }
        }

        /// <summary>
        /// Initializes the class for use by an inherited class instance. This constructor can only be called by an inherited class.
        /// </summary>
        protected AppServicesBase()
        {

        }

        /// <summary>
        /// Initializes the class for use by an inherited class instance. This constructor can only be called by an inherited class.
        /// </summary>
        protected AppServicesBase(AppContextBase appContext)
        {
            InitializeAppService(appContext);
        }

        /// <summary>
        /// Initialize the app service. Throws an exception if the context has already been set.
        /// </summary>
        internal void InitializeAppService(AppContextBase appContext)
        {
            if (_appContext == null)
            {
                _appContext = appContext;

                OnInitialize();
            }
            else
            {
                throw new InvalidOperationException("App service has already been initialized.");
            }
        }

        /// <summary>
        /// Detach this app service from the AppContext.
        /// </summary>
        internal void DetachFromAppContext()
        {
            if (_appContext != null)
            {
                _appContext = null;
            }
        }

        /// <summary>
        /// Get an object that contains shared data between UoW classes of the same type.
        /// </summary>
        protected virtual object GetSharedDataObject()
        {
            return _appContext.GetSharedDataObject(typeof(object));
        }

        /// <summary>
        /// Get an object that contains shared data between UoW classes of the same type.
        /// </summary>
        protected object GetSharedDataObject(Type type)
        {
            return _appContext.GetSharedDataObject(type);
        }

        /// <summary>
        /// Set an object that contains shared data between UoW classes of the same type.
        /// </summary>
        protected void AddSharedDataObject(object o)
        {
            _appContext.AddSharedDataObject(o);
        }

        /// <summary>
        /// Checks for a condition; if the condition is false, throws an UnauthorizedAccess exception.
        /// </summary>
        protected virtual void AssertAuthorization(bool value)
        {
            if (!value)
                throw new UnauthorizedAccessException();
        }

        /// <summary>
        /// The UoW object has been created and initialized.
        /// </summary>
        protected abstract void OnInitialize();
    }
}
