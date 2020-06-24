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
        object locker2 = new object();
        /// <summary>
        /// This method simulates charging of trucks two by two.
        /// </summary>
        public void Charging()
        {
            semaphor.Wait();
            TimeOfCharge = r.Next(500, 5000);
            Console.WriteLine("{0} is charging.", Name);
            Thread.Sleep(TimeOfCharge);
            Console.WriteLine("{0} is charged and charging lasted {1} miliseconds.", Name, TimeOfCharge);
            semaphor.Release();
        }
        /// <summary>
        /// This method simulates travel of truck to destination.
        /// </summary>
        public void Delivery()
        {
            Console.WriteLine("{0} started to destination.", Name);
            //locking block of code, that only one thread can access this object at the same time  
            lock (locker2)
            {
                TimeOfDelivery = r.Next(500, 5000);
                //int durationOfDelivery = r.Next(500, 5000);
                //TimeOfDelivery = durationOfDelivery;
                Console.WriteLine("Delivery of {0} will be made for {1} miliseconds.", Name, TimeOfDelivery);
                Thread.Sleep(TimeOfDelivery);
                Monitor.Pulse(locker2);
            }
        }
        /// <summary>
        /// This method simulates discharge of truck if truck arrived on the time, or returning to the start point if truck did not arrived on the time.
        /// </summary>
        public void Discharge()
        {
            //locking block of code, that only one thread can access this object at the same time  
            lock (locker2)
            {
                //if truck did not arrived for 3000 miliseconds                
                if (!Monitor.Wait(locker2, 3000))
                {
                    Console.WriteLine("{0} did not arrived to destination. Order is cancelled, and truck is returning to the start point.", Name);
                    Thread.Sleep(3000);
                    Console.WriteLine("{0} returned to the start point after 3000 miliseconds.", Name);
                }
                //if truck arrived for 3000 miliseconds
                else
                {
                    Console.WriteLine("{0} arrived to destination.", Name);
                    TimeOfDischarge = (int)(TimeOfCharge / 1.5);
                    Thread.Sleep(TimeOfDischarge);
                    Console.WriteLine("{0} finished discharge. Discharge lasted {1} miliseconds.", Name, TimeOfDischarge);
                }
            }
        }
    }
}
