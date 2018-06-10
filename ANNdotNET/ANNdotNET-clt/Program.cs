using ANNdotNET_clt.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANNdotNET_clt
{
    class Program
    {
        static void Main(string[] args)
        {

            CNTK106A_Tutorial.Train(CNTK.DeviceDescriptor.CPUDevice);


            //
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey ();
        }
    }
}
