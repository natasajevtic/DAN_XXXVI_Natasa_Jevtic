using System;
using System.Threading;

namespace Zadatak_1
{
    /// <summary>
    /// This program simulates generating matrixes using threads.
    /// </summary>
    class Program
    {
        static int[,] matrix;
        static int[] randomNumbers;
        static object locker = new object();
        /// <summary>
        /// This method initializes matrix, and after another method generated random numbers, this method perform filling created matrix with that numbers.
        /// </summary>
        static void CreateMatrix()
        {
            //locking block of code, that only one thread can access this object in the same time  
            lock (locker)
            {
                //initializing matrix
                matrix = new int[100, 100];
                //stopping current thread to another thread can generate random numbers
                if (randomNumbers == null)
                {
                    Monitor.Wait(locker);
                }
                //after another thread generated random numbers, this thread can continue
                int k = 0;
                //filling created matrix with generated random numbers
                while (k < randomNumbers.Length)
                {
                    for (int i = 0; i < matrix.GetLength(0); i++)
                    {
                        for (int j = 0; j < matrix.GetLength(1); j++)
                        {
                            matrix[i, j] = randomNumbers[k];
                            k++;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// This method generates random numbers, and then signalizes another thread that generating is done.
        /// </summary>
        static void GenerateNumbers()
        {
            //locking block of code, that only one thread can access this object in the same time  
            lock (locker)
            {
                randomNumbers = new int[10000];
                Random random = new Random();
                for (int i = 0; i < randomNumbers.Length; i++)
                {
                    randomNumbers[i] = random.Next(10, 100);
                }
                //sending another thread signal that generating is done
                Monitor.Pulse(locker);
            }
        }
        /// <summary>
        /// This method manages performing of threads.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            Thread creatingMatrix = new Thread(CreateMatrix);
            creatingMatrix.Start();
            Thread generateNumbers = new Thread(GenerateNumbers);
            generateNumbers.Start();
        }
    }
}
