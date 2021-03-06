﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Linq;

namespace Zadatak_1
{
    /// <summary>
    /// This program simulates charge and discharge of trucks using threads.
    /// </summary>
    class Program
    {
        static Truck[] trucks = new Truck[10];
        static int[] routes;
        static List<int> numbersDivisibleBy3;
        static int[] choosenRoutes;
        static object locker = new object();
        static bool isGenerated = true;
        static Thread[] threadsForTrucks = new Thread[10];
        static Thread[] threadsForDestination = new Thread[10];
        /// <summary>
        /// This method generates random numbers that represents possible routes of trucks and writes them in file.
        /// </summary>
        static void GenerateRoutes()
        {
            //locking block of code, that only one thread can access this object at the same time  
            lock (locker)
            {
                //initialization of array of routes
                routes = new int[1000];
                //filling array of routes with random numbers
                Random random = new Random();
                for (int i = 0; i < routes.Length; i++)
                {
                    routes[i] = random.Next(1, 5001);
                }
                //writing every number from array of routes to file
                StreamWriter writer = new StreamWriter(@"../../Routes.txt");
                foreach (int route in routes)
                {
                    writer.WriteLine(route);
                }
                writer.Close();
                Console.WriteLine("Marks of possible routes are generated.");
                //sending signal to another thread that routes are generated                
                Monitor.Pulse(locker);                
            }
        }
        /// <summary>
        /// This method reads possible routes from file, and choose ten distinct, for every truck one route.
        /// </summary>
        static void ChooseRoutes()
        {
            //locking block of code, that only one thread can access this object at the same time 
            lock (locker)
            {
                //while routes are not generated, wait for it
                while (routes == null)
                {
                    //if cannot generate routes for 3 seconds
                    if (!Monitor.Wait(locker, 3000))
                    {
                        isGenerated = false;
                        Random r = new Random();
                        choosenRoutes = new int[10];
                        string[] linesFromFile = File.ReadAllLines(@"../../Routes.txt");
                        int[] numbersFromFile = Array.ConvertAll(linesFromFile, x => Int32.Parse(x));
                        for (int i = 0; i < choosenRoutes.Length; i++)
                        {
                            choosenRoutes[i] = numbersFromFile[r.Next(0, numbersFromFile.Length)];
                        }
                    }
                }
                //if routes are generated
                if (isGenerated == true)
                {
                    //initialization of list of numbers that are divisible by 3
                    numbersDivisibleBy3 = new List<int>();
                    //reading all lines from file
                    string[] lines = File.ReadAllLines(@"../../Routes.txt");
                    //converting array of string to array of int
                    int[] numbers = Array.ConvertAll(lines, x => Int32.Parse(x));
                    //finding numbers that are divisible by 3
                    numbersDivisibleBy3 = numbers.Where(x => x % 3 == 0).ToList();
                    //sorting list
                    numbersDivisibleBy3.Sort();
                    //finding 10 distinct smallest number and putting them in new array
                    choosenRoutes = new int[10];
                    List<int> distinctRoutes = numbersDivisibleBy3.Distinct().ToList();
                    choosenRoutes = distinctRoutes.GetRange(0, 10).ToArray();
                }
                Console.Write("Choosen routes are: ");
                foreach (int route in choosenRoutes)
                {
                    Console.Write(route + " ");
                }
                Console.WriteLine("\nCharge of trucks can start.");
            }
        }
        /// <summary>
        /// This method assignments routes to trucks.
        /// </summary>
        static void RouteAssignment()
        {
            for (int i = 0; i < 10; i++)
            {
                trucks[i].Route = choosenRoutes[i];
                Console.WriteLine("Route {1} is assigned to {0}.", trucks[i].Name, trucks[i].Route);
            }
        }
        /// <summary>
        /// This method manages creating and performing of threads.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            //creating 10 objects of class Truck and setting their names
            for (int i = 0; i < trucks.Length; i++)
            {
                trucks[i] = new Truck() { Name = string.Format("Truck_{0}", i + 1) };
            }
            Thread generateRoutes = new Thread(GenerateRoutes);
            Thread menager = new Thread(ChooseRoutes);
            menager.Start();
            generateRoutes.Start();
            //blocking other threads while this two threads not finish their job
            generateRoutes.Join();
            menager.Join();
            //creating 10 threads that performs charge of trucks
            for (int i = 0; i < threadsForTrucks.Length; i++)
            {
                threadsForTrucks[i] = new Thread(trucks[i].Charging);
            }           
            for (int i = 0; i < threadsForTrucks.Length; i++)
            {
                threadsForTrucks[i].Start();
            }
            for (int i = 0; i < threadsForTrucks.Length; i++)
            {
                threadsForTrucks[i].Join();
            }
            Thread routeAssignment = new Thread(RouteAssignment);
            routeAssignment.Start();
            //creating 10 threads that performs delivery of trucks
            for (int i = 0; i < threadsForTrucks.Length; i++)
            {
                threadsForTrucks[i] = new Thread(trucks[i].Delivery);
            }
            //creating 10 threads that represent destinations which waiting for delivery and discharge
            for (int i = 0; i < threadsForDestination.Length; i++)
            {
                threadsForDestination[i] = new Thread(trucks[i].Discharge);
            }
            for (int i = 0; i < threadsForDestination.Length; i++)
            {
                threadsForDestination[i].Start();
            }
            for (int i = 0; i < threadsForTrucks.Length; i++)
            {
                threadsForTrucks[i].Start();
            }
            Console.ReadLine();
        }
    }
}
