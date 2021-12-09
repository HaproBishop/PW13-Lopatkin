using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Win32;

namespace LibMas
{/// <summary>
/// Класс для шаблонных действий с массивом
/// </summary>
    public class WorkMas
    {
        public static int[] _mas;
        public static int[,] _dmas;
        public static bool _twomas;
        public static void Open_File(in string filename) //Открытие файла 
        {
            StreamReader file = new StreamReader(filename);
            _twomas = Convert.ToBoolean(file.ReadLine());//Считывание значения (двумерный(true) и одномерный(false))
            if (_twomas == false) //Чтение данных для одномерного массива
            {
                int length = Convert.ToInt32(file.ReadLine());
                _mas = new int[length];
                for (int i = 0; i < length; i++)
                {
                    _mas[i] = Convert.ToInt32(file.ReadLine());
                }
            }
            else if(_twomas == true) //Чтение данных для двумерного массива
            {
                int rowslength = Convert.ToInt32(file.ReadLine());
                int columnslength = Convert.ToInt32(file.ReadLine());
                _dmas = new int[rowslength, columnslength];
                for (int i = 0; i < _dmas.GetLength(0); i++)
                {
                    for (int j = 0; j < _dmas.GetLength(1); j++)
                    {
                        _dmas[i,j] = Convert.ToInt32(file.ReadLine());
                    }
                }
            }
            file.Close();
        }
        public static void Save_File(in string filename)
        {            
            StreamWriter file = new StreamWriter(filename);
            if (_twomas == false) //Сохранение данных таблицы с одномерным массивом
            {
                file.WriteLine("false");
                file.WriteLine(_mas.Length);
                for (int i = 0; i < _mas.Length; i++)
                {
                    file.WriteLine(_mas[i]);
                }
            }
            else if (_twomas == true) //Сохранение данных таблицы с двумерным массивом
            {
                file.WriteLine("true");
                file.WriteLine(_dmas.GetLength(0));
                file.WriteLine(_dmas.GetLength(1));
                for (int i = 0; i < _dmas.GetLength(0); i++)
                {
                    for (int j = 0; j < _dmas.GetLength(1); j++)
                    {
                        file.WriteLine(_dmas[i,j]);
                    }
                }
            }
            file.Close();
        }
        public static int[] ClearTable() //Очистка таблицы 
        {
            _mas = null;
            _dmas = null;
            return null;
        }
        public static void CreateMas(in int length) //Создание пустой таблицы(скелет). Одномерный массив
        {
            _mas = new int[length]; 
        }
        public static void CreateMas(in int rows, in int columns)//Создание пустой таблицы. Двумерный массив
        {
            _dmas = new int[rows,columns];
        }

        public static void FillMas(in int range) //Заполнение таблицы для одномерного массива
        {
            Random rnd = new Random();
            for (int i = 0; i < _mas.Length; i++)
            {
                _mas[i] = rnd.Next(range);
            }
        }
        public static void FillDMas(in int range) //Заполнение таблицы для двумерного массива
        {
            Random rnd = new Random();            
            for (int i = 0; i < _dmas.GetLength(0); i++)
            {
                for (int j = 0; j < _dmas.GetLength(1); j++)
                {
                    _dmas[i, j] = rnd.Next(range);
                }                
            }
        }
    }
}
