using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Windows;


namespace TetraUtils
{
    public static class DataUtils
    {
        /// <summary>
        /// copies a linear array to linear array
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        public static void CopyArray<T>(T[] input, out T[] output)
        {
            int x = input.GetLength(0);

            output = new T[x];

            for (int i = 0; i < x; i++)
            {
                output[i] = input[i];
            }
        }
        public static void Nullify<T>(ref T[] arr, T zero)
        {
            int x = arr.GetLength(0);

            for (int i = 0; i < x; i++)
            {
                arr[i] = zero;
            }
        }
        /// <summary>
        /// copies a 2D array to another 2D array
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        public static void CopyArray<T>(T[,] input, out T[,] output)
        {
            int x = input.GetLength(0);
            int y = input.GetLength(1);

            output = new T[x, y];

            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < y; j++)
                {
                    output[i, j] = input[i, j];
                }
            }
        }
        public static void Nullify<T>(ref T[,] arr, T zero)
        {
            int x = arr.GetLength(0);
            int y = arr.GetLength(1);

            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < y; j++)
                {
                    arr[i, j] = zero;
                }
            }
        }
        /// <summary>
        /// copies a 3D array to another 3D array
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        public static void CopyArray<T> (T[,,] input, out T[,,] output)
        {
            int x = input.GetLength(0);
            int y = input.GetLength(1);
            int z = input.GetLength(2);

            output = new T[x, y, z];

            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < y; j++)
                {
                    for (int k = 0; k < z; k++)
                    {
                        output[i, j, k] = input[i, j, k];
                    }
                }
            }
        }

        public static void Nullify<T>(ref T[,,] arr, T zero)
        {
            int x = arr.GetLength(0);
            int y = arr.GetLength(1);
            int z = arr.GetLength(2);

            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < y; j++)
                {
                    for (int k = 0; k < z; k++)
                    {
                        arr[i, j, k] = zero;
                    }
                }
            }
        }
        public static T PickRandom<T>(IEnumerable<T> list)
        {
            return list.ElementAt(Random.Range(0, list.Count()));
        } 

    }
}
