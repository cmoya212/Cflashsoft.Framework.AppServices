using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Cflashsoft.Framework.WcfAppServices
{
    /// <summary>
    /// Wrapper for service proxies that safely dispose of the client.
    /// </summary>
    public class ServiceClient<TClient> : IDisposable where TClient : class, ICommunicationObject, new()
    {
        /// <summary>
        /// Execute a service proxy function and safely dispose of the client.
        /// </summary>
        public static TResult Use<TResult>(Func<TClient, TResult> func)
        {
            using (var service = new ServiceClient<TClient>())
            {
                return func(service.Client);
            }
        }

        /// <summary>
        /// Execute a service proxy function and safely dispose of the client.
        /// </summary>
        public static async Task<TResult> UseAsync<TResult>(Func<TClient, Task<TResult>> func)
        {
            using (var service = new ServiceClient<TClient>())
            {
                return await func(service.Client);
            }
        }

        private bool _disposed = false;
        private TClient _client = null;

        /// <summary>
        /// The service client proxy.
        /// </summary>
        public TClient Client => _client;

        /// <summary>
        /// Initializes a new instance of the ServiceClient class.
        /// </summary>
        public ServiceClient()
        {
            _client = new TClient();
        }

        /// <summary>
        /// Releases all resources used by this service.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases all resources used by this service.
        /// </summary>
        public void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    TClient client = _client;

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

                            _client = null;
                        }
                    }
                }

                _disposed = true;
            }
        }
    }
}
