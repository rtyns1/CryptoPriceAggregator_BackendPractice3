using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;


namespace CryptoPriceAggregator_BackendPractice3.Services
{
    public interface ICryptoPriceService
    {
        // this is interface which simply is a contract that all the 2 API service classes will have the method in here, because thats is waht program.cs expects
        // and it allows us to use dependency injection and polymorphism, so we can swap out the implementation without changing the code that uses it

        Task<decimal> GetPriceAsync(string cryptoSymbol, CancellationToken cancellationToken);
        // this method is asynchronous bcz it will be making an API call which is an i/o operation that can take some time and we dont want to block the thread while waiting for a response.
        // this method means that any class that implements this interface must provide an implementation for GetPriceAsync, which takes a crypto sumbol and a cancellationtoken, and returns a task which will eventually produce a decimal value price
        
    }

}
