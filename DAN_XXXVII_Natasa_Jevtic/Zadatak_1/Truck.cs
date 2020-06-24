using System;
using System.Threading;

namespace Zadatak_1
{
    class Truck
    {
        public int Route { get; set; }
        public int TimeOfDelivery { get; set; }
        public int TimeOfCharge { get; set; }
        public int TimeOfDischarge { get; set; }
        public string Name { get; set; }
        static SemaphoreSlim semaphor = new SemaphoreSlim(2, 2);
        static Random r = new Random();
        /// <summary>
        /// This method simulates charging of trucks two by two.
        /// </summary>
        public void Charging()
        {
            semaphor.Wait();
            TimeOfCharge = r.Next(500, 5000);
            Console.WriteLine("{0} is charging.", Name);
            Thread.Sleep(TimeOfCharge);
            Console.WriteLine("{0} is charged.", Name);
            semaphor.Release();
        }
    }
}
