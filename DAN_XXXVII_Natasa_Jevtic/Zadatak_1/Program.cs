using System;

namespace Zadatak_1
{
    class Program
    {
        static Truck[] trucks = new Truck[10];

        static void Main(string[] args)
        {
            for (int i = 0; i < trucks.Length; i++)
            {
                trucks[i] = new Truck() { Name = string.Format("Truck_{0}", i + 1) };
            }
            foreach (var item in trucks)
            {
                Console.WriteLine(item.Name);
            }
        }        
    }
}
