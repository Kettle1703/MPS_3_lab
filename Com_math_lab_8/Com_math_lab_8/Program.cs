using System.Text;
using static System.Console;

class MPS
{
    static Random rnd = new Random();
    public delegate double Func();
    // поля для Rand_var_use_lib
    static int order = 8;  // сколько знаков после запятой будет
    static int max_value = (int)Math.Pow(10, order) + 1;
    static double denom = Math.Pow(10, order);

    // поля для Lin_congruent
    static double prev = 1;  // R0, в последствии предыдущее значение метода
    static double now;  // текущее значение случайно величины
    static double m = 4294967296;  // модуль
    static double k = 1664525;  // множитель 
    static double b = 1013904223;  // приращение


    public static double Rand_var_use_lib()
    {
        return rnd.Next(0, max_value) / denom;
    }

    public static double Lin_congruent()
    {
        now = (k * prev + b) % m;
        prev = now;
        return now / (m - 1);
    }

    public static void Analysis(Func fun)
    {
        // анализ случайно величины переданной через делегат
        Dictionary<double, int> data = new Dictionary<double, int>(1500000);  // словарь хранящий сколько раз выпало каждое число
        double summa = 0;  // сумма всех чисел для математического ожидания
        double numerator = 0;  // числитель у дисперсии 
        int coun = 1500000;  // сколько раз будет взято число от случайной величины
        double less = 0;  // количество чисел случайной величины <= 0.5
        double main_volume = 0;  // количество чисел принадлежащие отрезку [M(x) - sqrt(D(x)), M(x) + sqrt(D(x))]
        double math_expect;  // математическое ожидание
        double variance;  // дисперсия
        bool flag = true;  // флаг для определения первого повтора
        for (int i = 0; i < coun; i++)
        {
            double number = fun();
            summa += number;
            if (!data.ContainsKey(number))
                data.Add(number, 1);
            else
            {
                data[number]++;
                if (flag)
                {
                    flag = false;
                    WriteLine($"Первый повтор на {i + 1} числе");
                }
            }
        }
        math_expect = summa / coun;
        foreach (var i in data)
        {
            numerator += i.Value * Math.Pow(i.Key - math_expect, 2);
            if (i.Key <= 0.5)
                less++;
        }
        variance = numerator / (coun - 1);
        double left = math_expect - Math.Sqrt(variance);
        double right = math_expect + Math.Sqrt(variance);
        foreach (var i in data)
        {
            if (left <= i.Key && i.Key <= right)
                main_volume++;
        }
        WriteLine($"M[x] = {math_expect}\nD[x] = {variance}\nКоличество уникальных элементов: {data.Count}");
        WriteLine($"[0, 0.5] - {less / coun}\n(0.5, 1] - {(coun - less) / coun}\n[M(x) - sqrt(D(x)), M(x) + sqrt(D(x))] или [{left}, {right}]: {main_volume / coun * 100} %");
    }
    public static int Main()
    {
        WriteLine("Случайная величина с использованием библиотек.");
        Analysis(Rand_var_use_lib);
        WriteLine("\nСлучайная величина на основе линейного конгруэнтного метода");
        Analysis(Lin_congruent);
        return 0;
    }
}

