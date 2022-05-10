using System;
using System.IO;
using System.Text;

namespace FileMeneger
{
    class Program
    {
        /// <summary>
        /// Точка входа.
        /// </summary>
        static void Main()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Добро пожаловать в файловый менеджер\n");
            Console.ResetColor();
            DriveInfo drive = SelectingDrive();
            DirectoryInfo di = new DirectoryInfo($"{drive}/");
            bool b;
            do
            {
                b = Manager(di);
            }
            while (b == false);
        }

        /// <summary>
        /// Метод, из которого выбираются действия.
        /// </summary>
        /// <param name="di"> Исходная директория. </param>
        /// <returns> true, если пользователь решил поднятся к предыдущей директории, false, если выбрал что то другое. </returns>
        public static bool Manager(DirectoryInfo di)
        {
            DirectoryContent(di, out DirectoryInfo[] directories, out FileInfo[] files, out int directoriesCounter, out int filesCounter);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Выберите команду:\n 0. Назад\n 1. Выбор папки\n 2. Выбор файла\n 3. Cоздать текстовый файл\n 4. Выйти из программы\n");
            Console.ResetColor();
            string cmdString = Console.ReadLine();
            int command;
            if (int.TryParse(cmdString, out command))
            {
                switch (command)
                {
                    case 0:
                        return true;
                    case 1:
                        DirectoryInfo dir = SelectingDirectory(directories, directoriesCounter);
                        bool b;
                        do
                        {
                            b = Manager(dir);
                        }
                        while (b == false);
                        return false;
                    case 2:
                        SelectingFile(files, filesCounter);
                        return false;
                    case 3:
                        return ChosingCodeForFileCreating(di);
                    case 4:
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.WriteLine("До встречи, солнышко");
                        Environment.Exit(0);
                        return false;
                    default:
                        Console.WriteLine("Такой команды нет");
                        return false;
                }
            }
            else
            {
                Console.WriteLine("Такой команды нет");
                return false;
            }
        }

        /// <summary>
        /// Выбор кодировки для создания текстового файла и перенаправление на соответствующий метод.
        /// </summary>
        /// <param name="di"> Текущая директория. </param>
        /// <returns> false. </returns>
        private static bool ChosingCodeForFileCreating(DirectoryInfo di)
        {
            Console.WriteLine("Выберите кодировку:\n 1. UTF-8\n 2. ASCII\n 3. UTF-32");
            string codeS = Console.ReadLine();
            int code;
            if (int.TryParse(codeS, out code))
            {
                switch (code)
                {
                    case 1:
                        CreatingFileUTF8(di);
                        return false;
                    case 2:
                        CreatingFileASCII(di);
                        return false;
                    case 3:
                        CreatingFileUTF32(di);
                        return false;
                    default:
                        Console.WriteLine("Такой команды нет");
                        return false;
                }
            }
            else
            {
                Console.WriteLine("Такой команды нет");
                return false;
            }
        }

        /// <summary>
        /// Вывод списка дисков, и выбор одиного из них.
        /// </summary>
        /// <returns> Выбранный диск. </returns>
        public static DriveInfo SelectingDrive()
        {
            int drivesCounter = 1;
            DriveInfo[] allDrives = DriveInfo.GetDrives();
            Console.WriteLine("Доступные диски:");
            foreach (DriveInfo d in allDrives)
            {
                Console.WriteLine("{0}. Диск {1}", drivesCounter, d.Name);
                ++drivesCounter;
            }
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Выберите диск и введите его номер");
            Console.ResetColor();
            string selctedDriveString = Console.ReadLine();
            int selctedDrive;
            if (int.TryParse(selctedDriveString, out selctedDrive) && selctedDrive >= 1 && selctedDrive <= drivesCounter -1)
            {
                DriveInfo di = allDrives[selctedDrive - 1];
                Console.WriteLine("Если хотите изменить выбор, нажмите Escape, если нет, нажмите любую другую клавишу");
                ConsoleKeyInfo ki;
                ki = Console.ReadKey(true);
                if (ki.Key == ConsoleKey.Escape)
                {
                    return SelectingDrive();
                }
                return di;
            }
            else
            {
                Console.WriteLine("Выбранного номера диска не существует, введите номер одного из представленных дисков");
                return SelectingDrive();
            }
        }

        /// <summary>
        /// Вывод всех файлов и папок текущей директории.
        /// </summary>
        /// <param name="di"> Текущая директория</param>
        /// <param name="directories"> Массив поддиректорий. </param>
        /// <param name="files"> Массив файлов в текущей директории. </param>
        /// <param name="directoriesCounter"> Количество поддиректорий. </param>
        /// <param name="filesCounter"> Количество файлов в текущей директории. </param>
        public static void DirectoryContent(DirectoryInfo di, out DirectoryInfo[] directories, out FileInfo[] files, out int directoriesCounter, out int filesCounter)
        {
            directories = di.GetDirectories();
            files = di.GetFiles();
            directoriesCounter = 1;
            Console.WriteLine("\nПапки:");
            foreach(var dir in directories)
            {
                if ((dir.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
                {
                    Console.WriteLine("{0}. Папка {1}(скрытая)", directoriesCounter, dir.Name);
                    ++directoriesCounter;
                    continue;
                }
                Console.WriteLine("{0}. Папка {1}", directoriesCounter, dir.Name);
                ++directoriesCounter;
            }
            filesCounter = 1;
            Console.WriteLine("\nФайлы:");
            foreach (var file in files)
            {
                if ((file.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
                {
                    Console.WriteLine("{0}. Файл {1}(скрытый)", filesCounter, file.Name);
                    ++filesCounter;
                    continue;
                }
                Console.WriteLine("{0}. Файл {1}", filesCounter, file.Name);
                ++filesCounter;
            }

            Console.WriteLine();
        }

        /// <summary>
        /// Функция перехода в другую директорию (выбор папки).
        /// </summary>
        /// <param name="directories"> Массив поддиректорий. </param>
        /// <param name="directoriesCounter"> Количество поддиректорий. </param>
        /// <returns> Возвращает выбранную поддиректорию (папку). </returns>
        public static DirectoryInfo SelectingDirectory(DirectoryInfo[] directories, int directoriesCounter)
        {

            Console.WriteLine("Выберите папку");
            string selctedDirectoryString = Console.ReadLine();
            int selctedDirectory;
            if (int.TryParse(selctedDirectoryString, out selctedDirectory) && selctedDirectory >= 1 && selctedDirectory <= directoriesCounter - 1)
            {
                Console.WriteLine("Если хотите изменить выбор, нажмите Escape, если нет, нажмите любую другую клавишу");
                ConsoleKeyInfo ki;
                ki = Console.ReadKey(true);
                if (ki.Key == ConsoleKey.Escape)
                {
                    return SelectingDirectory(directories, directoriesCounter);
                }
                return directories[selctedDirectory - 1];
            }
            else
            {
                Console.WriteLine("Выбранного номера директории не существует, введите номер одной из представленных директорий");
                return SelectingDirectory(directories, directoriesCounter);
            }
        }

        /// <summary>
        /// Выбор операции с файлом и перенаправление на соответствующий метод.
        /// </summary>
        /// <param name="file"> Файл, над которым будет производиться операция. </param>
        public static void ChosingOperationWithFile(FileInfo file)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Выберите операцию:\n 1. Вывести текстовый файл\n 2. Скопировать файл\n 3. Переместить файл в другую директорию\n 4. Удалить файл\n 5. Выполнить конкатенцию с другими файлами");
            Console.ResetColor();
            string commandString = Console.ReadLine();
            int command;
            if (int.TryParse(commandString, out command))
            {
                switch (command)
                {
                    case 1:
                        ChosingCodeForFileOutput(file);
                        break;
                    case 2:
                        CopyingFile(file);
                        break;
                    case 3:
                        MovingFile(file);
                        break;
                    case 4:
                        DeletingFile(file);
                        break;
                    case 5:
                        Concatenation(file);
                        break;

                    default:
                        Console.WriteLine("Такой команды нет");
                        break;
                }
            }
            else
            {
                Console.WriteLine("Такой команды нет");
            }
        }

        /// <summary>
        /// Выбор кодировки для вывода файла и перенаправление на соответствующий метод.
        /// </summary>
        /// <param name="file"> Выводящийся файл. </param>
        private static void ChosingCodeForFileOutput(FileInfo file)
        {
            Console.WriteLine("Выберите кодировку:\n 1. UTF-8\n 2. ASCII\n 3. UTF-32");
            string codeS = Console.ReadLine();
            int code;
            if (int.TryParse(codeS, out code))
            {
                switch (code)
                {
                    case 1:
                        FileOutputUTF8(file);
                        break;
                    case 2:
                        FileOutputASCII(file);
                        break;
                    case 3:
                        FileOutputUTF32(file);
                        break;
                    default:
                        Console.WriteLine("Такой команды нет");
                        break;
                }
            }
            else
            {
                Console.WriteLine("Такой команды нет");
            }
        }

        /// <summary>
        /// Выбор файла.
        /// </summary>
        /// <param name="files"> Массив доступных для выбора файлов. </param>
        /// <param name="filesCounter"> Количество доступных для выбора файлов. </param>
        public static void SelectingFile(FileInfo[] files, int filesCounter)
        {
            Console.WriteLine("Выберите файл");
            string selctedFileString = Console.ReadLine();
            int selctedFile;
            if (int.TryParse(selctedFileString, out selctedFile) && selctedFile >= 1 && selctedFile <= filesCounter)
            {
                Console.WriteLine("Если хотите изменить выбор, нажмите Escape, если нет, нажмите любую другую клавишу");
                ConsoleKeyInfo ki;
                ki = Console.ReadKey(true);
                if (ki.Key == ConsoleKey.Escape)
                {
                    SelectingFile(files, filesCounter);
                    return;
                }
                ChosingOperationWithFile(files[selctedFile - 1]);
            }
            else
            {
                Console.WriteLine("Выбранного номера директории не существует, введите номер одной из представленных директорий");
                SelectingFile(files, filesCounter);
            }
        }

        /// <summary>
        /// Вывод файла в кодировке Utf-8.
        /// </summary>
        /// <param name="file"> Выводящийся файл. </param>
        public static void FileOutputUTF8(FileInfo file)
        {
            using (FileStream fs = file.OpenRead())
            {
                byte[] b = new byte[1024];
                UTF8Encoding temp = new UTF8Encoding(true);
                Console.WriteLine($"Содержимое файла {file.Name}:\n");
                while (fs.Read(b, 0, b.Length) > 0)
                {
                    Console.WriteLine(temp.GetString(b));
                }
            }
        }

        /// <summary>
        /// Вывод файла в кодировке ASCII.
        /// </summary>
        /// <param name="file"> Выводящийся файл. </param>
        public static void FileOutputASCII(FileInfo file)
        {
            ASCIIEncoding ascii = new ASCIIEncoding();
            StreamReader sr = new StreamReader(file.FullName);
            string line = sr.ReadLine();
            Console.WriteLine($"Содержимое файла {file.Name}:\n");
            while (line != null)
            {
                Byte[] encodedBytes = ascii.GetBytes(line);
                String decodedString = ascii.GetString(encodedBytes);
                Console.WriteLine(decodedString);
                line = sr.ReadLine();
            }
            sr.Close();
        }

        /// <summary>
        /// Вывод файла в кодировке Utf-32.
        /// </summary>
        /// <param name="file"> Выводящийся файл. </param>
        public static void FileOutputUTF32(FileInfo file)
        {
            var enc = new UTF32Encoding();
            StreamReader sr = new StreamReader(file.FullName);
            string line = sr.ReadLine();
            Console.WriteLine($"Содержимое файла {file.Name}:\n");
            while (line != null)
            {
                Byte[] encodedBytes = enc.GetBytes(line);
                String decodedString = enc.GetString(encodedBytes);
                Console.WriteLine(decodedString);
                line = sr.ReadLine();
            }
            sr.Close();
        }

        /// <summary>
        /// Копирование файла в новый файл.
        /// </summary>
        /// <param name="file"> Искодный файл. </param>
        public static void CopyingFile(FileInfo file)
        {
            Console.WriteLine("Введите директорию пустого файла (включая сам файл), куда хотите скопировать исходный файл");
            string path = Console.ReadLine();
            try
            {
                file.CopyTo(path, true);
                Console.WriteLine("{0} был скопирован в {1}.", file, path);
            }
            catch
            {
                Console.WriteLine("Скопировать файл не удалось, проверьте корректность введенного пути");
            }
        }

        /// <summary>
        /// Создание файла в кодировке Utf-8.
        /// </summary>
        /// <param name="di"> Директория, в которой создается файл. </param>
        public static void CreatingFileUTF8(DirectoryInfo di)
        {
            Console.WriteLine("Введите имя нового файла");
            string fileName = Console.ReadLine();
            FileInfo fi = new FileInfo($"{di.FullName}\u002F{fileName}.txt");
            if (!fi.Exists)
            {
                using (StreamWriter sw = fi.CreateText())
                {
                    ConsoleKeyInfo ki;
                    Console.WriteLine("Введите текст файла, для завершения ввода нажмите ESC");
                    do
                    {
                        sw.WriteLine(Console.ReadLine());
                        ki = Console.ReadKey(true);
                    }
                    while (ki.Key != ConsoleKey.Escape);
                }
            }
            else
            {
                Console.WriteLine("Этот файл уже существует");
            }
            Console.WriteLine($"Файл {fileName} был создан");
        }

        /// <summary>
        /// Создание файла в кодировке ASCII.
        /// </summary>
        /// <param name="di"> Директория, в которой создается файл. </param>
        public static void CreatingFileASCII(DirectoryInfo di)
        {
            Console.WriteLine("Введите имя нового файла");
            string fileName = Console.ReadLine();
            FileInfo file = new FileInfo($"{di.FullName}\u002F{fileName}.txt");
            ASCIIEncoding ascii = new ASCIIEncoding();
            if (!file.Exists)
            {
                using (StreamWriter sw = file.CreateText())
                {
                    ConsoleKeyInfo ki;
                    Console.WriteLine("Введите текст файла, для завершения ввода нажмите ESC");
                    do
                    {
                        string line = Console.ReadLine();
                        Byte[] encodedBytes = ascii.GetBytes(line);
                        String decodedString = ascii.GetString(encodedBytes);
                        sw.WriteLine(decodedString);
                        ki = Console.ReadKey(true);
                    }
                    while (ki.Key != ConsoleKey.Escape);
                }
            }
            else
            {
                Console.WriteLine("Этот файл уже существует");
            }
            Console.WriteLine($"Файл {fileName} был создан");
        }

        /// <summary>
        /// Создание файла в кодировке Utf-32.
        /// </summary>
        /// <param name="di"> Директория, в которой создается файл. </param>
        public static void CreatingFileUTF32(DirectoryInfo di)
        {
            Console.WriteLine("Введите имя нового файла");
            string fileName = Console.ReadLine();
            FileInfo file = new FileInfo($"{di.FullName}\u002F{fileName}.txt");
            var enc = new UTF32Encoding();
            if (!file.Exists)
            {
                using (StreamWriter sw = file.CreateText())
                {
                    ConsoleKeyInfo ki;
                    Console.WriteLine("Введите текст файла, для завершения ввода нажмите ESC");
                    do
                    {
                        string line = Console.ReadLine();
                        Byte[] encodedBytes = enc.GetBytes(line);
                        String decodedString = enc.GetString(encodedBytes);
                        sw.WriteLine(decodedString);
                        ki = Console.ReadKey(true);
                    }
                    while (ki.Key != ConsoleKey.Escape);
                }
            }
            else
            {
                Console.WriteLine("Этот файл уже существует");
            }
            Console.WriteLine($"Файл {fileName} был создан");
        }

        /// <summary>
        /// Удаление файла.
        /// </summary>
        /// <param name="file"> Удаляемый файл. </param>
        public static void DeletingFile(FileInfo file)
        {
            try
            {
                file.Delete();
                Console.WriteLine("Файл {0} был удален", file.Name);
            }
            catch
            {
                Console.WriteLine("Удаление данного файла невозможно");
            }
        }

        /// <summary>
        /// Перемещение файла в другую директорию.
        /// </summary>
        /// <param name="file"> Перемещаемый файл. </param>
        public static void MovingFile(FileInfo file)
        {
            
            Console.WriteLine("Введите новый путь к файлу");
            string path = Console.ReadLine();
            try
            {
                file.MoveTo(path, true);
                Console.WriteLine("{0} был перемещен в {1}.", file, path);
            }
            catch
            {
                Console.WriteLine("Переместить файл не удалось, введен путь к катлогу, а не к файлу");
            }
        }

        /// <summary>
        /// Конкатенация файлов.
        /// </summary>
        /// <param name="file"> Файл, с которым конкатинируют другие файлы. </param>
        public static void Concatenation(FileInfo file)
        {
            Console.WriteLine("Введите пути к файлам, с которыми хотите выполнить конкатенацию");
            ConsoleKeyInfo ki;
            string path = file.FullName;
            string text = "";
            do
            {
                Console.WriteLine("Введите путm к файлe");
                string pathToFile = Console.ReadLine();
                try
                {
                    text += File.ReadAllText(pathToFile);
                    File.Delete(pathToFile);
                }
                catch
                {
                    Console.WriteLine("Не удалось считать файл");
                }
                Console.WriteLine("Для завершения ввода путей нажмите ESC, а если хотите продолжить ввод, нажмите любую клавишу");
                ki = Console.ReadKey(true);
            }
            while (ki.Key != ConsoleKey.Escape);
            try
            {
                File.AppendAllText(path, text);
            }
            catch
            {
                Console.WriteLine("Не удалось выполнить конкатенацию");
            }
            string readText = File.ReadAllText(path);
            Console.WriteLine("Конкатенация выполнена. Полученный файл:\n");
            Console.WriteLine(readText);
        }
    }
}

