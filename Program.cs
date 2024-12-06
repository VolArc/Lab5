using System.Text.RegularExpressions;

namespace Lab5;

internal static class Program {
    private static void Main() {
        Console.CursorVisible = false;
        var isRunning = true;
        var option = 0;
        while (isRunning) {
            option = Menu(["Выход", "Работа с двумерным массивом", "Работа с рванным массивом", "Работа со строкой"], option);
            switch (option) {
                case 1:
                    WorkWith2DArray();
                    break;
                case 2:
                    WorkWithJaggedArray();
                    break;
                case 3:
                    WorkWithString();
                    break;
                default:
                    isRunning = false;
                    break;
            }
        }
        Console.CursorVisible = true;
    }

    private static void WorkWith2DArray() {
        const int limit = 500;
        var rowsNumber = InputWithLimit("Введите количество строк: ", 1, 20);
        var columnsNumber = InputWithLimit("Введите количество столбцов: ", 1, limit / rowsNumber);
        var array = Fill2DArray(rowsNumber, columnsNumber,
            Menu(["Ввести элементы вручную", "Сгенерировать случайные числа"]) == 1);
        var arrayString = Print2DArray(array);
        var option = 0;
        var isRunning = true;
        var showArray = false;
        var highlightedMessage = "";
        while (isRunning) {
            option = Menu(["Назад", "Добавить столбец в начало матрицы", showArray ? "Скрыть матрицу" : "Показать матрицу"], option, highlightedMessage, showArray ? arrayString : "");
            switch (option) {
                case 1:
                    if (limit - (columnsNumber + 1) * rowsNumber >= 0) {
                        array = AddColumn(array);
                        columnsNumber++;
                        arrayString = Print2DArray(array);
                    }
                    else highlightedMessage = "Достигнут лимит элементов в матрице";
                    break;
                case 2:
                    showArray = !showArray;
                    break;
                default:
                    isRunning = false;
                    break;
            }
        }
    }
    private static void WorkWithJaggedArray() {
        var rowsNumber = InputWithLimit("Введите количество строк: ", 1, 20);
        var array = FillJaggedArray(rowsNumber);
        var arrayString = PrintJaggedArray(array);
        var isRunning = true;
        var option = 0;
        var showArray = false;
        while (isRunning) {
            option = Menu(["Назад", "Удалить строки", showArray ? "Скрыть массив" : "Показать массив"], option, "", showArray ? arrayString : "");
            switch (option) {
                case 1:
                    array = DeleteRows(array, Menu(["Ввести номера", "Выбрать строки"]) == 1);
                    arrayString = PrintJaggedArray(array);
                    if (array.Length == 0) {
                        isRunning = false;
                        Console.Clear();
                        Console.WriteLine("Вы массив удалили. Нажмите любую кнопку, чтобы отформатировать диск.");
                        Console.ReadKey();
                    }
                    break;
                case 2:
                    showArray = !showArray;
                    break;
                default:
                    isRunning = false;
                    break;
            }
        }
    }

    private static void WorkWithString() {
        const string keywordsString = "abstract\nas\nbase\nbool\nbreak\nbyte\ncase\ncatch\nchar\nchecked\nclass\nconst\ncontinue\ndecimal\ndefault\ndelegate\ndo\ndouble\nelse\nenum\nevent\nexplicit\nextern\nfalse\nfinally\nfixed\nfloat\nfor\nforeach\ngoto\nif\nimplicit\nin\nint\ninterface\ninternal\nis\nlock\nlong\nnamespace\nnew\nnull\nobject\noperator\nout\noverride\nparams\nprivate\nprotected\npublic\nreadonly\nref\nreturn\nsbyte\nsealed\nshort\nsizeof\nstackalloc\nstatic\nstring\nstruct\nswitch\nthis\nthrow\ntrue\ntry\ntypeof\nuint\nulong\nunchecked\nunsafe\nushort\nusing\nvirtual\nvoid\nvolatile\nwhile\nadd\nand\nalias\nascending\nargs\nasync\nawait\nby\ndescending\ndynamic\nequals\nfrom\nget\nglobal\ngroup\ninit\ninto\njoin\nlet\nmanaged\nnameof\nnint\nnot\nnotnull\nnuint\non\norderby\npartial\npartial\nrecord\nremove\nselect\nset\nunmanaged\nunmanaged\nvalue\nvar\nwhen\nwhere\nwhere\nwith\nyield";
        var pattern = @"\b(" + keywordsString.Replace('\n', '|') + @")\b";
        var keyWords = keywordsString.Split('\n');
        var userInput = InputString("Введите строку для поиска ключевых слов: ");
        List<int> sharpPositions = [];
        
        for (var i = 0; i < userInput.Length; i++) {
            if (userInput[i] == '#') sharpPositions.Add(i);
        }

        var matches = Regex.Matches(userInput, pattern);
        var foundKeywords = new Dictionary<string, int>();
        
        foreach (var keyword in keyWords) foundKeywords[keyword] = 0;
        
        foreach (Match match in matches) {
            var foundKeyword = match.Value;
            if (foundKeywords.ContainsKey(foundKeyword)) foundKeywords[foundKeyword]++;
        }
        
        const string punctuation = "`~!@$%^&*()_+-=[]{}\\|/?><,.#;:'\"";
        userInput = punctuation.Aggregate(userInput, (current, mark) => current.Replace(mark.ToString(), " " + mark + " "));
        var splittedString = userInput.Split(' ');
        var highlightsString = "";
        foreach (var word in splittedString) {
            if (keyWords.Contains(word)) highlightsString += $"#{word}# ";
            else highlightsString += $"{word} ";
        }
        highlightsString = punctuation.Aggregate(highlightsString, (current, mark) => current.Replace(" " + mark + " ", mark.ToString()));
        Console.Clear();
        var highlight = false;
        var position = 0;
        foreach (var symbol in highlightsString) {
            if (symbol == '#' && !sharpPositions.Contains(position)) {
                highlight = !highlight;
                position--;
            }
            else if (highlight && symbol != '#') {
                Console.BackgroundColor = ConsoleColor.Yellow;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.Write(symbol);
                Console.ResetColor();
            }
            else Console.Write(symbol);
            position++;
        }
        Console.ResetColor();
        Console.WriteLine();
        foreach (var word in foundKeywords.Where(word => word.Value != 0)) Console.WriteLine($"{word.Key} — {word.Value}");
        Console.ReadKey();
    }

    #region Inputs
    private static int InputInt(string message) {
        int number;
        Console.Write(message);
        Console.CursorVisible = true;

        while (!int.TryParse(Console.ReadLine(), out number))
            Console.Write($"Ошибка!!! Введите целое число.\n{message}");

        Console.CursorVisible = false;

        return number;
    }

    private static int InputWithLimit(string message, int limitMin, int limitMax = int.MaxValue) {
        int number;

        do {
            number = InputInt(message);
            if (number < limitMin || number > limitMax) Console.WriteLine($"Ошибка!!! Число должно быть не меньше {limitMin} и не больше {limitMax}!");
        } while (number < limitMin || number > limitMax);

        return number;
    }
    
    private static string InputString(string message) {
        string input;
        Console.WriteLine(message);
        Console.CursorVisible = true;

        do {
            input = Console.ReadLine();
            if (input is "" or " ") Console.Write("Ошибка!!! Введите не пустую строку.\n");
            while (input.Contains("  ")) input = input.Replace("  ", " ");
        } while (input is "" or " ");

        Console.CursorVisible = false;

        return input;
    }
    #endregion
    
    
    
     private static int Menu(string[] options, int option = 0, string highlightedMessage = "", string arrayMessage = "") {
        var work = true;
        var length = options.Length;
        var selectedOption = option >= 0 && option < length ? option : 0;

        while (work) {
            Console.Clear();
            Console.WriteLine("Выберите один из следующих вариантов:");
            for (var i = 0; i < length; i++) {
                Console.ResetColor();
                if (i == selectedOption) {
                    Console.BackgroundColor = ConsoleColor.Cyan;
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.WriteLine($"{i}. {options[i]}");
                    Console.ResetColor();
                }
                else 
                    Console.WriteLine($"{i}. {options[i]}");
            }
            
            if (highlightedMessage != "") {
                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.Black;
                var highlight = false;
                foreach (var symbol in highlightedMessage) {
                    if (symbol == '#') highlight = !highlight;
                    else if (highlight) {
                        Console.BackgroundColor = ConsoleColor.DarkBlue;
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write(symbol);
                        Console.BackgroundColor = ConsoleColor.White;
                        Console.ForegroundColor = ConsoleColor.Black;
                    }
                    else Console.Write(symbol);
                }
                Console.ResetColor();
                Console.WriteLine();
            }

            if (arrayMessage != "") {
                var highlight = false;
                foreach (var symbol in arrayMessage) {
                    if (symbol == '\n') highlight = !highlight;
                    if (highlight && symbol != '\n') {
                        Console.BackgroundColor = ConsoleColor.DarkBlue;
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write(symbol);
                        Console.ResetColor();
                    }
                    else Console.Write(symbol);
                }
                Console.ResetColor();
            }

            var key = Console.ReadKey();
            ConsoleKey[] keys = [ConsoleKey.D0, ConsoleKey.D1, ConsoleKey.D2, ConsoleKey.D3, ConsoleKey.D4, ConsoleKey.D5, ConsoleKey.D6, ConsoleKey.D7, ConsoleKey.D8, ConsoleKey.D9];
            switch (key.Key) {
                case ConsoleKey.UpArrow:
                    selectedOption = (selectedOption - 1 + length) % length;
                    break;
                case ConsoleKey.DownArrow:
                    selectedOption = (selectedOption + 1) % length;
                    break;
                case ConsoleKey.Enter:
                    work = false;
                    break;
                default: {
                    for (var i = 0; i < (length < keys.Length ? length : keys.Length); i++) {
                        if (key.Key != keys[i]) continue;
                        selectedOption = i;
                        break;
                    }

                    break;
                }
            }
            Console.ResetColor();
            Console.Clear();
        }
        
        
        return selectedOption;
    }

    private static Tuple<int, int> ChooseRows(int[][] array) {
        var length = array.Length;
        var row = 0;
        var row1 = -1;
        var row2 = -1;

        while (true) {
            Console.Clear();
            Console.WriteLine("Выберите строки (Чтобы удалить выбранные строки нажмите Y, чтобы вернуться в меню нажмите N):");
            for (var i = 0; i < length; i++) {
                Console.ResetColor();
                if (i == row) {
                    Console.BackgroundColor = ConsoleColor.Cyan;
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.WriteLine($"{i + 1}. {PrintArray(array[i])}");
                    Console.ResetColor();
                }
                else if (i == row1 || i == row2) {
                    Console.BackgroundColor = ConsoleColor.Magenta;
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.WriteLine($"{i + 1}. {PrintArray(array[i])}");
                    Console.ResetColor();
                }
                else if (row1 != -1 && row2 != -1 && i < row2 && i > row1) {
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.WriteLine($"{i + 1}. {PrintArray(array[i])}");
                    Console.ResetColor();
                }
                else 
                    Console.WriteLine($"{i + 1}. {PrintArray(array[i])}");
            }

            if (row1 != -1 || row2 != -1) {
                Console.BackgroundColor = ConsoleColor.DarkBlue;
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(row1 == -1 ? $"Выбрана строка {row2 + 1}." : $"Выбраны строки с {row1 + 1} по {row2 + 1}.");
                Console.ResetColor();
            }

            var key = Console.ReadKey();
            ConsoleKey[] keys = [ConsoleKey.D1, ConsoleKey.D2, ConsoleKey.D3, ConsoleKey.D4, ConsoleKey.D5, ConsoleKey.D6, ConsoleKey.D7, ConsoleKey.D8, ConsoleKey.D9, ConsoleKey.D0];
            switch (key.Key) {
                case ConsoleKey.UpArrow:
                    row = (row - 1 + length) % length;
                    break;
                case ConsoleKey.DownArrow:
                    row = (row + 1) % length;
                    break;
                case ConsoleKey.Enter: {
                    if (row1 == row) row1 = -1;
                    else if (row2 == row) row2 = -1;
                    else if (row1 == -1) row1 = row;
                    else row2 = row;
                    if (row1 > row2) (row1, row2) = (row2, row1);
                    break;
                }
                case ConsoleKey.Y when row1 != -1 || row2 != -1:
                    return row1 != -1 && row2 != -1 ? Tuple.Create(row1, row2) :
                        row2 == -1 ? Tuple.Create(row1, row1) : Tuple.Create(row2, row2);
                case ConsoleKey.N:
                    return Tuple.Create(-1, -1);
                default: {
                    for (var i = 0; i < (length < keys.Length ? length : keys.Length); i++) {
                        if (key.Key != keys[i]) continue;
                        row = i;
                        break;
                    }

                    break;
                }
            }
            Console.ResetColor();
            Console.Clear();
        }
    }
     
    private static int[,] Fill2DArray(int rowsNumber, int columnsNumber, bool randomFilling) {
        var length = columnsNumber * rowsNumber;
        var array = new int[rowsNumber, columnsNumber];
        if (randomFilling) {
            var minValue = InputInt("Введите нижнюю границу: ");
            var maxValue = InputWithLimit("Введите верхнюю границу: ", minValue);
            //var fillArrayBar = new LoadBar("Заполнение массива.", 0, length - 1);
            var random = new Random();

            for (var i = 0; i < length; i++) {
                array[i / columnsNumber, i % columnsNumber] = random.Next(minValue, maxValue);
                //fillArrayBar.RenewIteration(i);
            }
            
        }
        else {
            Console.WriteLine("Введите элементы массива:");

            for (var i = 0; i < length; i++)
                array[i / columnsNumber, i % columnsNumber] = InputInt($"{i / columnsNumber + 1} строка, {i % columnsNumber + 1} столбец ? ");
        }
        
        return array;
    }
    
    private static string PrintArray(int[] array, int[]? highlights = null) {
        var arrayString = "";
        if (highlights != null) 
            for (var i = 0; i < array.Length; i++) 
                arrayString += highlights.Contains(i) ? $"#{array[i]}# " : $"{array[i]} ";
        else
            arrayString = array.Aggregate(arrayString, (current, t) => current + $"{t} ");

        return arrayString;
    }

    private static int[][] FillJaggedArray(int rowsNumber) {
        var array = new int [rowsNumber][];
        var randomFilling = Menu(["Ввести элементы вручную", "Сгенерировать случайные числа"]);
        var minValue = InputInt("Введите нижнюю границу: ");
        var maxValue = InputWithLimit("Введите верхнюю границу: ", minValue);
        for (var i = 0; i < rowsNumber; i++)
            array[i] = FillArray(InputWithLimit("Введите длину строки: ", 1, 50), randomFilling == 1, minValue, maxValue);
        return array;
    }
    
    private static int[] FillArray(int length, bool randomFilling, int minValue = 1, int maxValue = -1) {
        var array = new int[length];
        if (randomFilling) {
            if (maxValue < minValue) {
                minValue = InputInt("Введите нижнюю границу: ");
                maxValue = InputWithLimit("Введите верхнюю границу: ", minValue);
            }

            var random = new Random();

            for (var i = 0; i < length; i++) {
                array[i] = random.Next(minValue, maxValue);
            }
            
        }
        else {
            Console.WriteLine("Введите элементы массива: ");

            for (var i = 0; i < length; i++)
                array[i] = InputInt($"{i + 1} ? ");
        }
        
        return array;
    }

    private static int[,] AddColumn(int[,] array) {
        var tempArray = Fill2DArray(array.GetLength(0), 1, Menu (["Ввести элементы вручную", "Сгенерировать случайные числа"]) == 1);
        var finalArray = new int[array.GetLength(0), array.GetLength(1) + 1];
        for (var i = 0; i < array.GetLength(0); i++) finalArray[i, 0] = tempArray[i, 0];
        for (var i = 0; i < array.Length; i++)
            finalArray[i / array.GetLength(1), i % array.GetLength(1) + 1] =
                array[i / array.GetLength(1), i % array.GetLength(1)];
        return finalArray;
    }

    private static string Print2DArray(int[,] array) {
        var arrayString = "";
        for (var i = 0; i < array.GetLength(0); i++) {
            for (var j = 0; j < array.GetLength(1); j++)
                arrayString += $"{array[i, j]} ";
            arrayString += '\n';
        }

        return arrayString;
    }

    private static string PrintJaggedArray(int[][] array) {
        var arrayString = "";
        var rowsNumber = array.Length;
        for (var i = 0; i < rowsNumber; i++) {
            for (var j = 0; j < array[i].Length; j++)
                arrayString += $"{array[i][j]} ";
            arrayString += '\n';
        }

        return arrayString;
    }

    private static int[][] DeleteRows(int[][] array, bool chooseRows) {
        int border1, border2;
        if (!chooseRows) {
            border1 = InputWithLimit("Введите первую границу: ", 1, array.Length);
            border2 = InputWithLimit("Введите вторую границу: ", border1--, array.Length + 1);
            border2--;
        }
        else (border1, border2) = ChooseRows(array);
        if (border1 > border2) (border1, border2) = (border2, border1);
        if (border1 == -1 || border2 == -1) return array;
        var newArray = new int[array.Length - (border2 - border1 + 1)][];
        var j = 0;
        
        for (var i = 0; i < array.Length; i++) {
            if (i == border1) i = border2;
            else {
                newArray[j++] = array[i];
            }
        }

        return newArray;
    }
}

