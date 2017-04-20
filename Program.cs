using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddServicePrincipalWithOwner
{
    class Program
    {
        static void Main(string[] args)
        {
            new ServicePrincipals().CreateServicePrincipalWithOwner();
            Console.ReadLine();
        }
    }
}
