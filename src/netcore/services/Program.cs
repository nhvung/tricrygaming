using System;
using System.Threading.Tasks;

#if DataTransferAPI
namespace DataTransferAPILib
#endif

{
    public class Program
    {
        const string PRIVATE_KEY = "921cf1eb-c12d-4c19-a148-be62720276f8";
        static Task Main(string[] args)
        {
            string guid = Guid.NewGuid().ToString().ToLower();

#if DataTransferAPI
            return new VSHost("DataTransferAPI", "Data Transfer API", 2001, PRIVATE_KEY).RunAsync(args);
#endif

        }
    }
}
