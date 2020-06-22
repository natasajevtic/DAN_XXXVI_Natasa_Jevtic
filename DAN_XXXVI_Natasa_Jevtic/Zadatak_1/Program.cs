using System;
using System.IO;
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
        static int[] oddNumbers;
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
                while (randomNumbers == null)
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
        /// This method every odd number from matrix places in array and then in file. After that signalizes another thread that array is created and  writed in file.
        /// </summary>
        static void CreateArrayOfOddNumbers()
        {
            //locking block of code, that only one thread can access this object in the same time  
            lock (locker)
            {
                int countOfOddNumbers = 0;
                //calculating how much matrix has odd numbers
                for (int i = 0; i < matrix.GetLength(0); i++)
                {
                    for (int j = 0; j < matrix.GetLength(1); j++)
                    {
                        if (matrix[i, j] % 2 != 0)
                        {
                            countOfOddNumbers++;
                        }
                    }
                }
                //initializing array of odd numbers
                oddNumbers = new int[countOfOddNumbers];
                int k = 0;
                //placing odd numbers from matrix in array of odd numbers
                while (k < countOfOddNumbers)
                {
                    for (int i = 0; i < matrix.GetLength(0); i++)
                    {
                        for (int j = 0; j < matrix.GetLength(1); j++)
                        {
                            if (matrix[i, j] % 2 != 0)
                            {
                                oddNumbers[k] = matrix[i, j];
                                k++;
                            }
                        }
                    }
                }
                //writing created array of numbers in txt
                StreamWriter str = new StreamWriter(@"../../Array.txt");
                foreach (int number in oddNumbers)
                {
                    str.WriteLine(number);
                }
                str.Close();
                //sending another thread signal that creating array of odd numbers is done
                Monitor.Pulse(locker);
            }
        }
        /// <summary>
        /// This method waits to another tread to create and write array of odd numbers in txt, and after that displays numbers from file to console.
        /// </summary>
        static void PrintOddNumbers()
        {
            //locking block of code, that only one thread can access this object in the same time
            lock (locker)
            {
                //while array is not created, waiting another thread to do that
                while (oddNumbers == null)
                {
                    Monitor.Wait(locker);
                }
                //reading numbers from file, and displaying them to console
                string[] numbers = File.ReadAllLines(@"../../Array.txt");
                foreach (string number in numbers)
                {
                    Console.WriteLine(number);
                }
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
            //blocking other threads while this two threads do not end their job
            Thread oddNumbers = new Thread(CreateArrayOfOddNumbers);
            Thread printer = new Thread(PrintOddNumbers);
            oddNumbers.Start();
            printer.Start();
            Console.ReadLine();
        }
    }
}
