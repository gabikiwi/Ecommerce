using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ECommerce.ProductCatalog.Model;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;

namespace ECommerce.ProductCatalog
{
    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    internal sealed class ProductCatalog : StatefulService, IProductCatalogService
    {

        private IProductRepository _repo;

        public ProductCatalog(StatefulServiceContext context)
            : base(context)
        { }

        public async Task AddProduct(Product product)
        {
            await _repo.AddProduct(product);
        }

        public async Task<IEnumerable<Product>> GetAllProducts()
        {
            return await _repo.GetAllProducts();
        }

        /// <summary>
        /// Optional override to create listeners (e.g., HTTP, Service Remoting, WCF, etc.) for this service replica to handle client or user requests.
        /// </summary>
        /// <remarks>
        /// For more information on service communication, see https://aka.ms/servicefabricservicecommunication
        /// </remarks>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            // return new ServiceReplicaListener[0];
            //return new[] { new ServiceReplicaListener(context => this.CreateServiceReplicaListeners(context)) };
            return this.CreateServiceRemotingReplicaListeners();
        }

        /// <summary>
        /// This is the main entry point for your service replica.
        /// This method executes when this replica of your service becomes primary and has write status.
        /// </summary>
        /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service replica.</param>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {

            //@GC evry stateful service has StateManage defined on his based class 
            _repo = new ServiceFabricProductRepository(this.StateManager);

            var product1 = new Product
            {
                Id = Guid.NewGuid(),
                Name = "Lenovo Monitor",
                Description = "Computer Monitor",
                Price = 500,
                Availability = 100
            };

            var product2 = new Product
            {
                Id = Guid.NewGuid(),
                Name = "Chromebook",
                Description = "Laptop",
                Price = 700,
                Availability = 50
            };

            var product3 = new Product
            {
                Id = Guid.NewGuid(),
                Name = "Backpack",
                Description = "Accessories",
                Price = 700,
                Availability = 50
            };
            // TODO: Replace the following sample code with your own logic 
            //       or remove this RunAsync override if it's not needed in your service.

            //var myDictionary = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, long>>("myDictionary");

            //while (true)
            //{
            //    cancellationToken.ThrowIfCancellationRequested();

            //    using (var tx = this.StateManager.CreateTransaction())
            //    {
            //        var result = await myDictionary.TryGetValueAsync(tx, "Counter");

            //        ServiceEventSource.Current.ServiceMessage(this.Context, "Current Counter Value: {0}",
            //            result.HasValue ? result.Value.ToString() : "Value does not exist.");

            //        await myDictionary.AddOrUpdateAsync(tx, "Counter", 0, (key, value) => ++value);

            //        // If an exception is thrown before calling CommitAsync, the transaction aborts, all changes are 
            //        // discarded, and nothing is saved to the secondary replicas.
            //        await tx.CommitAsync();
            //    }

            //    await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            //}


            await _repo.AddProduct(product1);
            await _repo.AddProduct(product2);
            await _repo.AddProduct(product3);

            string valueString = "Before variable all";
            Console.WriteLine(valueString);

            IEnumerable<Product> all = await _repo.GetAllProducts();
        }
    }
}
