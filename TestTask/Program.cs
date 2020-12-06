using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace TestTask
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Dictionary<int, string> numbersEng = FillingEng();
                Dictionary<int, string> numbersUkr = FillingUkr();
                byte lang = 0;
                bool check = true;
                string cents = "";
                char dot = ' ';

                do
                {
                    try
                    {
                        Console.WriteLine("Enter language, please. (1-Ukrainian, 2-English)");
                        lang = byte.Parse(Console.ReadLine());
                        if (lang == 1 || lang == 2) check = true;
                        else check = false;

                    }
                    catch (OverflowException)
                    {
                        check = false;
                    }
                    catch (FormatException) 
                    {
                        check = false;
                    }

                } while (!check);
                Console.WriteLine("Enter number");
                string text = "";
                switch (lang)
                {
                    case 1: dot = ','; break;
                    case 2: dot = '.'; break;
                    default: dot = '.'; break;
                }
                do
                {
                    try
                    {
                        text = Convert.ToString(Console.ReadLine());
                        check = Validate(text, lang, dot);
                    }
                    catch (OverflowException)
                    {
                        Console.WriteLine("You entered too big number. Please retype number");
                        check = false;
                    }
                } while (!check);

                for (int i = 0; i < text.Length; i++)
                {
                    if (text[i] == dot)
                    {
                        cents = $"{text[text.Length - 2]}" + $"{text[text.Length - 1]}";
                        text = text.Substring(0, text.IndexOf(dot));
                        break;
                    }
                }

                Console.WriteLine(RefactNumbersInWords(text, (lang == 1 ? numbersUkr : numbersEng), cents, lang));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        private static bool Validate(string text, int lang, char dot)
        {
            string pattern = "";
            int max = 0;
            switch (lang)
            {
                case 1: pattern = @"^\d{1,10}\,\d{2}$"; break;
                case 2: pattern = @"^\d{1,10}\.\d{2}$"; break;
                default: pattern = @"^\d{1,10}\.\d{2}$"; break;
            }
            if (Regex.IsMatch(text, pattern))
            {
                max = int.Parse(text.Substring(0, text.IndexOf(dot)));
                if (max < 0)
                {
                    Console.WriteLine("You entered number less zero, please enter positive number");
                    return false;
                }
                return true;
            }
            else
            {
                Console.WriteLine($"Incorrect format. Example: 1234567{dot}89");
                return false;
            }

        }
        private static string RefactNumbersInWords(string text, Dictionary<int, string> numbersEng, string cents, byte _lang)
        {
            string output = string.Empty;
            byte lang = _lang;
            string[] arrayOfDischarges = new string[text.Length % 3 == 0 ? text.Length / 3 : text.Length / 3 + 1]; 
            for (int i = 0, j = 0, k = 0; i < text.Length; i++) // розбиття строки на розряди ["1","234","567"]
            {
                if (text.Length % 3 == j && k == 0 && !(text.Length % 3 == 0)) //умова, якщо перший розряд менше 3
                {
                    k++;
                    j = 0;
                }
                if (j == 3) 
                {
                    k++;
                    j = 0;
                }
                arrayOfDischarges[k] += text[i];
                j++;
            }

            int number = 0;
            int num = arrayOfDischarges.Length;
            string def = "10" + num;
            string engfirst = "";

            for (int i = 0; i < arrayOfDischarges.Length; i++)
            {
                number = int.Parse(arrayOfDischarges[i]);

                if ((arrayOfDischarges[i].Length < 3 || arrayOfDischarges[i][0] == '0') && arrayOfDischarges[i] != "000") //якщо розряд менше 3 або перший символ з розряду == 0
                {
                    if (i == arrayOfDischarges.Length - 1) //якщо останній розряд
                    {
                        output += $"{TwoSign(arrayOfDischarges[i], numbersEng)} ";
                    }
                    else output += $"{ReductionToPluralUkr(arrayOfDischarges[i], numbersEng, def, lang, number)} ";
                }
                else if (i == arrayOfDischarges.Length - 1 && arrayOfDischarges[i] != "000") //якщо останній розряд
                {
                    switch (lang)
                    {
                        case 1:
                            engfirst = HundredUkr((number / 100).ToString(), numbersEng);
                            break;
                        case 2:
                            engfirst = (numbersEng[number / 100] + " " + numbersEng[101]);
                            break;
                    }

                    output += $"{engfirst} " + $"{TwoSign(arrayOfDischarges[i].Substring(1, arrayOfDischarges[i].Length - 1), numbersEng)} ";
                }
                else if (arrayOfDischarges[i] != "000")
                {
                    switch (lang)
                    {
                        case 1:
                            engfirst = HundredUkr((number / 100).ToString(), numbersEng); //перший символ прописом з розряду для української
                            break;
                        case 2:
                            engfirst = (numbersEng[number / 100] + " " + numbersEng[101]);//перший символ прописом з розряду for English
                            break;
                    }
                    output += $"{engfirst} " + $"{ReductionToPluralUkr(arrayOfDischarges[i].Substring(1, arrayOfDischarges[i].Length - 1), numbersEng, def, lang, number) } ";
                }
                else output += string.Empty;
                num--;
                def = "10" + num;
            }

            return Output(numbersEng, number, lang, cents, output,arrayOfDischarges[arrayOfDischarges.Length-1]).Replace("  "," ");
        }
        private static string Output(Dictionary<int, string> numbersUkr, int number, byte lang, string cents, string output, string lastDischarge) //повертає правильний вивід валюти і копійок
        {
            int centNumber = int.Parse(cents);
            byte lastNumber = byte.Parse(lastDischarge[lastDischarge.Length - 1].ToString());
            if (output.Length < 2)
            {
                output = "";
            }
            else
            {
                switch (lang)
                {
                    case 1://For Ukrainian
                        if (lastNumber > 1 && lastNumber < 5)
                        {
                            output += numbersUkr[998].Substring(0, numbersUkr[998].Length - 1) + "i" + " "; break; // 2-4 гривні
                        }
                        else if (lastNumber == 1)//одна гривня
                        {
                            output += numbersUkr[998] + " "; break; 
                        }

                        else output += numbersUkr[999] + " "; break; // 5 і більше гривень
                    case 2: //For English
                        if (number > 1)
                        {
                            output += numbersUkr[990] + "s" + " "; break; //dollars
                        }
                        else output += numbersUkr[990] + " "; break; //dollar
                }
            }
            if (number != 0 && cents != "00") { output += numbersUkr[105] + " "; }
            if (cents == "00")
            {
                return output;
            }
            else
            {
                switch (lang)
                {
                    case 1:
                        if (centNumber > 1 && centNumber < 5)
                        {
                            output += TwoSign(cents, numbersUkr) + " " + numbersUkr[108].Substring(0, numbersUkr[108].Length - 1) + "и" + " "; break; // 2-4 копійки
                        }
                        else if (centNumber > 4)
                        {
                            output += TwoSign(cents, numbersUkr) + " " + numbersUkr[109] + " "; break; //5 і більше копійок
                        }
                        else if (centNumber == 1) { output += numbersUkr[1] + " " + numbersUkr[108] + " "; break; }//одна копійка
                        else output += TwoSign(cents, numbersUkr) + " " + numbersUkr[108] + " "; break;
                    case 2:
                        if (centNumber > 1)
                        {
                            output += TwoSign(cents, numbersUkr) + " " + numbersUkr[110] + "s" + " "; break; //cents
                        }
                        else output += TwoSign(cents, numbersUkr) + " " + numbersUkr[110] + " "; break; //cent
                }
            }
            return output;
        }
        private static string ReductionToPluralUkr(string textELement, Dictionary<int, string> numbersUkr, string category, byte lang, int number = 0)//Method for plural in Ukrainian lang
        {
            string text = TwoSign(textELement, numbersUkr) + " ";
            int lastCell = int.Parse(textELement[textELement.Length - 1].ToString());
            string numeral = numbersUkr[int.Parse(category)];

            if (lang == 2)//if English lang
            {
                return text + numeral;
            }
            if (number > 4)
            {
                if (category == "104" || category == "103") //переірка (104-мільярд) в Dictionary (103-мільйон) в Dictionary
                {
                    numeral += "iв"; //5 і більше мільярдів / мільйонів 
                    return text + numeral;
                }
                if (category == "102") //102-тисяча
                {
                    numeral = numeral.Substring(0, numeral.Length - 1);//5 і більше тисяч
                    return text + numeral;
                }
            }
            if (lastCell > 1 && lastCell < 5) 
            {
                if (category == "104" || category == "103")
                {
                    numeral += "и"; // 2-4 мільйони і мільярди
                }
                if (category == "102")
                {
                    numeral = numeral.Substring(0, numeral.Length - 1) + "i"; //2,3,4 тисячі
                }
            }
            if (number == 1)
            {
                if (category == "104" || category == "103") //condition for milliard (104) in Dictionary and million (103) in Dictionary
                {
                    text = numbersUkr[111] + " ";
                    return text + numeral;
                }
            }

            else return text + numeral;
            return text + numeral;
        }
        private static string HundredUkr(string number, Dictionary<int, string> numbersUkr)//повертає назву сотень в українські мові
        {
            return numbersUkr[int.Parse(number + "01")];//для сто, двісті, триста ...
        }
        private static string TwoSign(string text, Dictionary<int, string> numbersEng) //повертає назву прописом для 2 символів
        {
            if (text == "00") return string.Empty;
            string output = string.Empty;
            int number = int.Parse(text);
            if (number > 20)
            {
                if (number % 10 == 0) output = $"{numbersEng[number - (number % 10)]} ";//для 30,40,50 ...
                else output = $"{numbersEng[number - (number % 10)]}{numbersEng[107]}" + $"{numbersEng[number % 10]}"; // для десятків і одиниць разом
            }
            else output = numbersEng[number];
            return output;
        }
        private static Dictionary<int, string> FillingEng()
        {
            Dictionary<int, string> numbersEng = new Dictionary<int, string>();
            numbersEng.Add(0, string.Empty);
            numbersEng.Add(1, "one");
            numbersEng.Add(2, "two");
            numbersEng.Add(3, "three");
            numbersEng.Add(4, "four");
            numbersEng.Add(5, "five");
            numbersEng.Add(6, "six");
            numbersEng.Add(7, "seven");
            numbersEng.Add(8, "eight");
            numbersEng.Add(9, "nine");
            numbersEng.Add(10, "ten");
            numbersEng.Add(11, "eleven");
            numbersEng.Add(12, "twelve");
            numbersEng.Add(13, "thirteen");
            numbersEng.Add(14, "fourteen");
            numbersEng.Add(15, "fifteen");
            numbersEng.Add(16, "sixteen");
            numbersEng.Add(17, "seventeen");
            numbersEng.Add(18, "eighteen");
            numbersEng.Add(19, "nineteen");
            numbersEng.Add(20, "twenty");
            numbersEng.Add(30, "thirty");
            numbersEng.Add(40, "forty");
            numbersEng.Add(50, "fifty");
            numbersEng.Add(60, "sixty");
            numbersEng.Add(70, "seventy");
            numbersEng.Add(80, "eighty");
            numbersEng.Add(90, "ninety");
            numbersEng.Add(110, "cent");
            numbersEng.Add(100, "");
            numbersEng.Add(101, "hundred");
            numbersEng.Add(102, "thousand");
            numbersEng.Add(103, "million");
            numbersEng.Add(104, "milliard");
            numbersEng.Add(105, "and");
            numbersEng.Add(990, "dollar");
            numbersEng.Add(999, "dollars");
            numbersEng.Add(107, "-");

            return numbersEng;
        }
        private static Dictionary<int, string> FillingUkr()
        {
            Dictionary<int, string> numbersEng = new Dictionary<int, string>();
            numbersEng.Add(0, string.Empty);
            numbersEng.Add(1, "одна");
            numbersEng.Add(111, "один");
            numbersEng.Add(2, "двi");
            numbersEng.Add(3, "три");
            numbersEng.Add(4, "чотири");
            numbersEng.Add(5, "п'ять");
            numbersEng.Add(6, "шiсть");
            numbersEng.Add(7, "сiм");
            numbersEng.Add(8, "вiсiм");
            numbersEng.Add(9, "дев'ять");
            numbersEng.Add(10, "десять");
            numbersEng.Add(11, "одинадцять");
            numbersEng.Add(12, "дванадцять");
            numbersEng.Add(13, "тринадцять");
            numbersEng.Add(14, "чотирнадцять");
            numbersEng.Add(15, "п'ятнадцять");
            numbersEng.Add(16, "шiстнадцять");
            numbersEng.Add(17, "сiмнадцять");
            numbersEng.Add(18, "вiсiмнадцять");
            numbersEng.Add(19, "дев'ятнадцять");
            numbersEng.Add(20, "двадцять");
            numbersEng.Add(30, "тридцять");
            numbersEng.Add(40, "сорок");
            numbersEng.Add(50, "п'ятдесят");
            numbersEng.Add(60, "шiстдесят");
            numbersEng.Add(70, "сiмдесят");
            numbersEng.Add(80, "вiсiмдесят");
            numbersEng.Add(90, "дев'яносто");
            numbersEng.Add(109, "копiйок");
            numbersEng.Add(108, "копiйка");
            numbersEng.Add(998, "гривня");
            numbersEng.Add(999, "гривень");
            numbersEng.Add(101, "сто");
            numbersEng.Add(201, "двiстi");
            numbersEng.Add(301, "триста");
            numbersEng.Add(401, "чотириста");
            numbersEng.Add(501, "п'ятсот");
            numbersEng.Add(601, "шiстсот");
            numbersEng.Add(701, "сiмсот");
            numbersEng.Add(801, "вiсiмсот");
            numbersEng.Add(901, "дев'ятсот");
            numbersEng.Add(100, "");
            numbersEng.Add(102, "тисяча");
            numbersEng.Add(103, "мiльйон");
            numbersEng.Add(104, "мiльярд");
            numbersEng.Add(105, "i");
            numbersEng.Add(107, " ");

            return numbersEng;
        }
    }
}
