using System;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System.Threading.Tasks;

namespace tests
{
    public class Program
    {
        static void Main(string[] args)
        {
            // test azure table
            //Task.Run(TableTests.Run).GetAwaiter().GetResult();
            
            // test azure blob
            Task.Run(BlobTests.Run).GetAwaiter().GetResult();
        }

    }
}
