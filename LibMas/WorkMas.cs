using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Win32;

namespace LibMas
{
    public class WorkMas
    {
        public static int[] mas;
        public static int[,] dmas;
        public static bool twomas;
        public static void Open_File(in string filename) //Открытие файла 
        {
            StreamReader file = new StreamReader(filename);
            twomas = Convert.ToBoolean(file.ReadLine());//Считывание значения (двумерный(true) и одномерный(false))
            if (twomas == false) //Чтение данных для одномерного массива
            {
                int length = Convert.ToInt32(file.ReadLine());
                mas = new int[length];
                for (int i = 0; i < length; i++)
                {
                    mas[i] = Convert.ToInt32(file.ReadLine());
                }
            }
            else if(twomas == true) //Чтение данных для двумерного массива
            {
                int rowslength = Convert.ToInt32(file.ReadLine());
                int columnslength = Convert.ToInt32(file.ReadLine());
                dmas = new int[rowslength, columnslength];
                for (int i = 0; i < dmas.GetLength(0); i++)
                {
                    for (int j = 0; j < dmas.GetLength(1); j++)
                    {
                        dmas[i,j] = Convert.ToInt32(file.ReadLine());
                    }
                }
            }
            file.Close();
        }
        public static void Save_File(in string filename)
        {            
            StreamWriter file = new StreamWriter(filename);
            if (twomas == false) //Сохранение данных таблицы с одномерным массивом
            {
                file.WriteLine("false");
                file.WriteLine(mas.Length);
                for (int i = 0; i < mas.Length; i++)
                {
                    file.WriteLine(mas[i]);
                }
            }
            else if (twomas == true) //Сохранение данных таблицы с двумерным массивом
            {
                file.WriteLine("true");
                file.WriteLine(dmas.GetLength(0));
                file.WriteLine(dmas.GetLength(1));
                for (int i = 0; i < dmas.GetLength(0); i++)
                {
                    for (int j = 0; j < dmas.GetLength(1); j++)
                    {
                        file.WriteLine(dmas[i,j]);
                    }
                }
            }
            file.Close();
        }
        public static int[] ClearTable() //Очистка таблицы 
        {
            mas = null;
            dmas = null;
            return null;
        }
        public static void CreateMas(in int length) //Создание пустой таблицы(скелет). Одномерный массив
        {
            mas = new int[length]; 
        }
        public static void CreateMas(in int rows, in int columns)//Создание пустой таблицы. Двумерный массив
        {
            dmas = new int[rows,columns];
        }

        public static void FillMas(in int range) //Заполнение таблицы для одномерного массива
        {
            Random rnd = new Random();
            for (int i = 0; i < mas.Length; i++)
            {
                mas[i] = rnd.Next(range);
            }
        }
        public static void FillDMas(in int range) //Заполнение таблицы для двумерного массива
        {
            Random rnd = new Random();            
            for (int i = 0; i < dmas.GetLength(0); i++)
            {
                for (int j = 0; j < dmas.GetLength(1); j++)
                {
                    dmas[i, j] = rnd.Next(range);
                }                
            }
        }
    }
}
