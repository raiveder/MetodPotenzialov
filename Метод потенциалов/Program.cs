namespace Метод_потенциалов
{
    internal class Program
    {
        static int[,] matr;
        static int[] mConst;
        static int[] nConst;

        /// <summary>
        /// Определение вырожденности
        /// </summary>
        /// <param name="divisionArray">Распределение</param>
        /// <param name="m">Поставщики</param>
        /// <param name="n">Потребители</param>
        static void degenerate(bool[,] divisionArray, int m, int n)
        {
            int kol = 0;
            foreach (bool item in divisionArray)
            {
                if (item)
                {
                    kol++;
                }
            }
            if (m + n - 1 != kol)
            {
                int min = 16;
                int iMin = 0;
                int jMin = 0;
                for (int i = 0; i < mConst.Length; i++)
                {
                    for (int j = 0; j < nConst.Length; j++)
                    {
                        if (!divisionArray[i, j])
                        {
                            if (matr[i, j] < min) //if (matr[i, j] <= min)
                                {
                                min = matr[i, j];
                                iMin = i;
                                jMin = j;
                            }
                        }
                    }
                }
                divisionArray[iMin, jMin] = true;
            }
        }

        /// <summary>
        /// Определение ячеек для цикла перераспределения
        /// </summary>
        /// <param name="divisionArrayBool">Информация о распределении</param>
        /// <param name="iMax">Индекс i максимальной дельты</param>
        /// <param name="jMax">Индекс j максимальной дельты</param>
        /// <returns>Строковый массив с индексами ячеек для цикла перераспределения</returns>
        static string[] cycle(bool[,] divisionArrayBool, int iMax, int jMax)
        {
            Console.WriteLine("Введите индексы ячеек, через которые будет проходить цикл, без разделителей.\nНапример: 13 - ячейка [1;3].");
            Console.WriteLine("Цикл можно проводить только через ячейки, в которых находятся единицы.");
            Console.WriteLine("Для завершения ввода введите 0.");
            Console.WriteLine("Цикл начинается из ячейки [{0},{1}]. Введите следующие ячейки.", iMax + 1, jMax + 1);
            string[] array = new string[1];
            array[0] = Convert.ToString(iMax) + Convert.ToString(jMax);
            int i = 1;
            string check = "0";
            while (true)
            {
                while (true)
                {
                    if (i % 2 == 0)
                    {
                        Console.Write("Введите ячейку цикла (+х): ");
                    }
                    else
                    {
                        Console.Write("Введите ячейку цикла (-х): ");
                    }

                    check = Console.ReadLine();
                    if (int.TryParse(check, out int result) && result > 10 && result < 35 || check == "0")
                    {
                        if (check == "0")
                        {
                            Console.Clear();
                            return array;
                        }
                        if (divisionArrayBool[Convert.ToInt32(check.Substring(0, 1)) - 1, Convert.ToInt32(check.Substring(1, 1)) - 1])
                        {
                            Array.Resize(ref array, array.Length + 1);
                            array[i] = check;
                            i++;
                            break;
                        }
                    }
                    Console.WriteLine("Ошибка. Введите ячейку корректно!");
                }
            }
        }

        /// <summary>
        /// Расчёт потенциалов, дельты и цикла перераспределения (2, 3, 4 шаги)
        /// </summary>
        /// <param name="divisionArray">Распределение</param>
        /// <param name="divisionArrayBool">Информация о распределении (есть ли связь между поставщиком и потребителем)</param>
        /// <returns>Оптимальное распределение (true) или нет (false)</returns>
        static bool potentials(int[,] divisionArray, bool[,] divisionArrayBool)
        {
            int[] v = new int[mConst.Length];
            int[] u = new int[nConst.Length];
            bool[] vbool = new bool[mConst.Length];
            bool[] ubool = new bool[nConst.Length];
            ubool[0] = true;

            for (int i = 0; i < mConst.Length; i++)
            {
                if (divisionArrayBool[i, 0])
                {
                    v[i] = matr[i, 0];
                    vbool[i] = true;
                }
            }

            bool checkV = true;
            bool checkU = true;
            while (checkV || checkU)
            {
                for (int i = 0; i < mConst.Length; i++)
                {
                    for (int j = 1; j < nConst.Length; j++)
                    {
                        if (divisionArrayBool[i, j])
                        {
                            if (ubool[j])
                            {
                                v[i] = matr[i, j] - u[j];
                                vbool[i] = true;
                            }

                            if (vbool[i])
                            {
                                u[j] = matr[i, j] - v[i];
                                ubool[j] = true;
                            }
                        }
                    }
                }

                checkU = false;
                foreach (bool item in ubool)
                {
                    if (item == false)
                    {
                        checkU = true;
                        break;
                    }
                }
                checkV = false;
                foreach (bool item in vbool)
                {
                    if (item == false)
                    {
                        checkV = true;
                        break;
                    }
                }
            }

            int[,] delta = new int[mConst.Length, nConst.Length];
            for (int i = 0; i < mConst.Length; i++)
            {
                for (int j = 0; j < nConst.Length; j++)
                {
                    if (!divisionArrayBool[i, j])
                    {
                        delta[i, j] = v[i] + u[j] - matr[i, j];
                    }
                }
            }

            bool check = false;
            foreach (int item in delta)
            {
                if (item > 0)
                {
                    check = true;
                    break;
                }
            }
            if (!check)
            {
                return true;
            }

            int max = delta[0, 0];
            int iMax = 0;
            int jMax = 0;
            for (int i = 0; i < mConst.Length; i++)
            {
                for (int j = 0; j < nConst.Length; j++)
                {
                    if (delta[i, j] > max)
                    {
                        max = delta[i, j];
                        iMax = i;
                        jMax = j;
                    }
                }
            }

            for (int i = 0; i < mConst.Length; i++)
            {
                for (int j = 0; j < nConst.Length; j++)
                {
                    if (divisionArrayBool[i, j])
                    {
                        Console.Write(1 + "\t");
                    }
                    else
                    {
                        Console.Write(0 + "\t");
                    }
                }
                Console.WriteLine();
            }

            string[] cycleArray = cycle(divisionArrayBool, iMax, jMax);

            int iMin = Convert.ToInt32(cycleArray[1].Substring(0, 1)) - 1;
            int jMin = Convert.ToInt32(cycleArray[1].Substring(1, 1)) - 1;
            int min = divisionArray[iMin, jMin];
            for (int i = 3; i < cycleArray.Length; i+=2)
            {
                iMin = Convert.ToInt32(cycleArray[i].Substring(0, 1)) - 1;
                jMin = Convert.ToInt32(cycleArray[i].Substring(1, 1)) - 1;
                if (divisionArray[iMin, jMin] < min)
                {
                    min = divisionArray[iMin, jMin];
                }
            }

            int indexI = Convert.ToInt32(cycleArray[0].Substring(0, 1));
            int jndexJ = Convert.ToInt32(cycleArray[0].Substring(1, 1));
            divisionArray[indexI, jndexJ] += min;
            if (divisionArray[indexI, jndexJ] > 0)
            {
                divisionArrayBool[indexI, jndexJ] = true;
            }

            for (int c = 1; c < cycleArray.Length; c++)
            {
                int i = Convert.ToInt32(cycleArray[c].Substring(0, 1)) - 1;
                int j = Convert.ToInt32(cycleArray[c].Substring(1, 1)) - 1;
                if (c % 2 == 0)
                {
                    divisionArray[i, j] += min;
                    if (divisionArray[i, j] > 0)
                    {
                        divisionArrayBool[i, j] = true;
                    }
                }
                else
                {
                    divisionArray[i, j] -= min;
                    if (divisionArray[i, j] == 0)
                    {
                        divisionArrayBool[i, j] = false;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Расчёт функции и заключения договоров
        /// </summary>
        static void calculation()
        {
            int[] m = new int[mConst.Length];
            int[] n = new int[nConst.Length];
            bool[,] divisionArrayBool = new bool[m.Length, n.Length];
            int[,] divisionArray = new int[m.Length, n.Length];
            int[,] firstDivision = new int[m.Length, n.Length];
            bool check = false;
            int func = 0;
            string funcS = "";

            Array.Copy(matr, firstDivision, matr.Length);
            Array.Copy(mConst, m, mConst.Length);
            Array.Copy(nConst, n, nConst.Length);

            int sum = 1;

            while (sum != 0)
            {
                int min = 16;
                int iMin = 0;
                int jMin = 0;

                for (int i = 0; i < m.Length; i++)
                {
                    for (int j = 0; j < n.Length; j++)
                    {
                        if (firstDivision[i, j] < min && firstDivision[i, j] > 0)
                        {
                            min = firstDivision[i, j];
                            iMin = i;
                            jMin = j;
                        }
                    }
                }

                firstDivision[iMin, jMin] = 0;

                if (m[iMin] > 0 && n[jMin] > 0)
                {
                    if (n[jMin] < m[iMin])
                    {
                        func += matr[iMin, jMin] * n[jMin];
                        funcS += matr[iMin, jMin] + "*" + n[jMin] + " + ";
                        divisionArray[iMin, jMin] = n[jMin];
                        divisionArrayBool[iMin, jMin] = true;
                        m[iMin] -= n[jMin];
                        n[jMin] = 0;
                    }
                    else
                    {
                        func += matr[iMin, jMin] * m[iMin];
                        funcS += matr[iMin, jMin] + "*" + m[iMin] + " + ";
                        divisionArray[iMin, jMin] = m[iMin];
                        divisionArrayBool[iMin, jMin] = true;
                        n[jMin] -= m[iMin];
                        m[iMin] = 0;
                    }
                }

                sum = 0;
                foreach (int item in m)
                {
                    sum += item;
                }
            }

            degenerate(divisionArrayBool, m.Length, n.Length);

            funcS = "Ответ: Fопт = " + funcS.Substring(0, funcS.Length - 3) + " = " + func + " у.д.е.";

            check = potentials(divisionArray, divisionArrayBool);

            if (!check)
            {
                while (check != true)
                {
                    funcS = "";
                    func = 0;

                    Array.Copy(mConst, m, mConst.Length);
                    Array.Copy(nConst, n, nConst.Length);

                    for (int i = 0; i < m.Length; i++)
                    {
                        for (int j = 0; j < n.Length; j++)
                        {
                            if (divisionArray[i, j] > 0)
                            {
                                if (n[j] < m[i])
                                {
                                    func += matr[i, j] * divisionArray[i, j];
                                    funcS += matr[i, j] + "*" + divisionArray[i, j] + " + ";
                                }
                                else
                                {
                                    func += matr[i, j] * divisionArray[i, j];
                                    funcS += matr[i, j] + "*" + divisionArray[i, j] + " + ";
                                }
                            }
                        }
                    }

                    funcS = "Ответ: Fопт = " + funcS.Substring(0, funcS.Length - 3) + " = " + func + " у.д.е.";

                    check = potentials(divisionArray, divisionArrayBool);
                }
            }
            Console.WriteLine(funcS + "\n");

            Console.WriteLine("\t\tЗаключение договоров");
            for (int i = 0; i < m.Length; i++)
            {
                for (int j = 0; j < n.Length; j++)
                {
                    if (divisionArray[i, j] > 0)
                    {
                        Console.WriteLine("{0}-й поставщик с {1}-м потребителем на {2} ед. продукции", i + 1, j + 1, divisionArray[i, j]);
                    }
                }
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Заполнение исходных данных по условию задачи
        /// </summary>
        static void fill()
        {
            matr = new int[3, 4];
            matr[0, 0] = 9;
            matr[0, 1] = 5;
            matr[0, 2] = 3;
            matr[0, 3] = 10;
            matr[1, 0] = 6;
            matr[1, 1] = 3;
            matr[1, 2] = 8;
            matr[1, 3] = 2;
            matr[2, 0] = 3;
            matr[2, 1] = 8;
            matr[2, 2] = 4;
            matr[2, 3] = 7;

            mConst = new int[3];
            mConst[0] = 25;
            mConst[1] = 55;
            mConst[2] = 22;

            nConst = new int[4];
            nConst[0] = 45;
            nConst[1] = 15;
            nConst[2] = 22;
            nConst[3] = 20;
        }

        /// <summary>
        /// Ввод исходных данных вручную
        /// </summary>
        static void vvod()
        {
            int m, n;
            while (true)
            {
                Console.Write("Введите количество поставщиков: ");
                if (int.TryParse(Console.ReadLine(), out int result) && result > 1 && result < 11)
                {
                    m = result;
                    break;
                }
                Console.WriteLine("Ошибка. Введите значение корректно!");
            }

            while (true)
            {
                Console.Write("Введите количество потребителей: ");
                if (int.TryParse(Console.ReadLine(), out int result) && result > 1 && result < 11)
                {
                    n = result;
                    break;
                }
                Console.WriteLine("Ошибка. Введите значение корректно!");
            }

            matr = new int[m, n];
            mConst = new int[m];
            nConst = new int[n];

            Console.WriteLine("Заполнение матрицы затрат на перевозку.");
            for (int i = 0; i < mConst.Length; i++)
            {
                for (int j = 0; j < nConst.Length; j++)
                {
                    while (true)
                    {
                        Console.Write("Введите [{0},{1}] элемент матрицы: ", i + 1, j + 1);
                        if (int.TryParse(Console.ReadLine(), out int result) && result > 0 && result < 16)
                        {
                            matr[i, j] = result;
                            break;
                        }
                        Console.WriteLine("Ошибка. Введите значение корректно!");
                    }
                }
            }
            while (true)
            {
                for (int i = 0; i < mConst.Length; i++)
                {
                    while (true)
                    {
                        Console.Write("Введите мощность {0}-го поставщика: ", i + 1);
                        if (int.TryParse(Console.ReadLine(), out int result) && result > 1 && result < 151)
                        {
                            mConst[i] = result;
                            break;
                        }
                        Console.WriteLine("Ошибка. Введите значение корректно!");
                    }
                }

                for (int i = 0; i < nConst.Length; i++)
                {
                    while (true)
                    {
                        Console.Write("Введите спрос {0}-го потребителя: ", i + 1);
                        if (int.TryParse(Console.ReadLine(), out int result) && result > 1 && result < 151)
                        {
                            nConst[i] = result;
                            break;
                        }
                        Console.WriteLine("Ошибка. Введите значение корректно!");
                    }
                }

                Console.Clear();

                int sumM = 0;
                foreach (int item in mConst)
                {
                    sumM += item;
                }
                int sumN = 0;
                foreach (int item in nConst)
                {
                    sumN += item;
                }

                if (sumM == sumN)
                {
                    break;
                }
                Console.WriteLine("Ошибка. Суммарные мощности поставщиков и спросов потребителей не равны!");
            }
        }

        static void Main(string[] args)
        {
            bool check = true;
            while (check)
            {
                Console.WriteLine("1 - Ввести данные вручную.");
                Console.WriteLine("2 - Заполнить данные автоматически (по условию задачи).");
                Console.WriteLine("3 - Очистить экран.");
                Console.WriteLine("4 - Завершить работу.");
                Console.Write("Выберите действие: ");
                if (int.TryParse(Console.ReadLine(), out int result) && result > 0 && result < 5)
                {
                    switch (result)
                    {
                        case 1:
                            Console.Clear();
                            vvod();
                            calculation();
                            break;
                        case 2:
                            Console.Clear();
                            fill();
                            calculation();
                            break;
                        case 3:
                            Console.Clear();
                            break;
                        case 4:
                            check = false;
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Введите действие корректно!");
                }
            }
        }
    }
}