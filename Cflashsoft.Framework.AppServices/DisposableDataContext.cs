using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cflashsoft.Framework.AppServices
{
    /// <summary>
    /// Generic implementation of DataContext for IDisposable.
    /// </summary>
    public class DisposableDataContext<T> : DataContext<T>, IDisposable where T : class, new()
    {
        private bool _disposed = false;

        /// <summary>
        /// Releases all resources used by this data context.
        /// </summary>
        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases all resources used by this data context.
        /// </summary>
        public virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    IDisposable disposable = this.Data as IDisposable;

                    if (disposable != null)
                    {
                        disposable.Dispose();
                        this.Data = null;
                    }
                }

                _disposed = true;
            }
        }
    }
}
