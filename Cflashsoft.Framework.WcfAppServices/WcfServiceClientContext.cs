using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Cflashsoft.Framework.AppServices;
using Cflashsoft.Framework.Types;

namespace Cflashsoft.Framework.WcfAppServices
{
    /// <summary>
    /// Represents a wrapper for WCF Service Client and used by UoW AppServices classes to share a context.
    /// </summary>
    public class WcfServiceClientContext<TClient> : DataContext<TClient>, IDisposable where TClient : class, ICommunicationObject, new()
    {
        private bool _disposed = false;

        /// <summary>
        /// Wcf service proxy client.
        /// </summary>
        public TClient Client
        {
            get
            {
                return this.Data;
            }
        }

        /// <summary>
        /// Initializes the wcf service context and create proxy instance.
        /// </summary>
        public override void InitializeDataContext(AppContextBase appContext)
        {
            this.AppContext = appContext;
            this.Data = new TClient();
        }

        /// <summary>
        /// Releases all resources used by this data context.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases all resources used by this data context.
        /// </summary>
        public void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    TClient client = this.Data;

                    if (client != null)
                    {
                        try
                        {
                            IDisposable disposableClient = client as IDisposable;

                            if (disposableClient != null)
                                disposableClient.Dispose();
                            else
                                client.Close();
                        }
                        catch (CommunicationException)
                        {
                            client.Abort();
                        }
                        catch (TimeoutException)
                        {
                            client.Abort();
                        }
                        catch (Exception)
                        {
                            client.Abort();
                            throw;
                        }
                        finally
                        {
                            if (client.State != CommunicationState.Closed)
                                client.Abort();

                            this.Data = null;
                        }
                    }
                }

                _disposed = true;
            }
        }
    }
}
