using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FindCountMoreAvgColumnLibrary
{/// <summary>
/// Класс для нахождения количества значений в столбце, которые больше его среднего арифметического
/// </summary>
    public class FindCountMoreAvgColumnClass
    {/// <summary>
     /// Метод для нахождения количества значений каждого столбца, которые больше его среднего арифметического значения
     /// </summary>
     /// <param name="dmas">Входной массив с, хранящимися в нем, данными</param>
     /// <returns>Возращает массив, где первая строка - среднее значение по каждому стоблцу, а вторая - количество 
     /// значений каждого столбца, которые больше его среднего арифметического</returns>
        public static int[][] FindCountMoreAvgColumn(int [,] dmas)
        {
            int[][] resultarray = new int[2][];
            for (int i = 0; i < 2; i++)
            {
                resultarray[i] = new int[dmas.GetLength(1)];
            }
            for (int j = 0; j < dmas.GetLength(1); j++)
            {
                int sum = 0;
            for (int i = 0; i < dmas.GetLength(0); i++)
            {
                    sum += dmas[i, j];
            }
                resultarray[0][j] = sum / dmas.GetLength(0);             
            }
            for (int j = 0; j < dmas.GetLength(1); j++)
            {
                int count = 0;
            for (int i = 0; i < dmas.GetLength(0); i++)
            {
                    if (resultarray[0][j] < dmas[i, j]) count++;
            }
                resultarray[1][j] = count;
            }
            return resultarray;
        }
    }
}
