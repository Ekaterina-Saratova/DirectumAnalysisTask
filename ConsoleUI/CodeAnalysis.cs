using System.Xml;

namespace ConsoleUI
{
    public static class TwoVersions
    {
        //По имени метода непонятно что он делает. Вариант - GetAttributeValueByElementAndName.
        //Имя параметра string input приемлемое, но filePath было бы лучше.
        //Метод может вернуть null, это стоит указать в типе возвращаемого значения - string? вместо string.
        //То же самое при присвоении string result = null, заменить на string? result = null;
        //Вместо string[] lines использовать var lines, тип однозначен при инициализации переменной.
        //System.IO.File.ReadAllLines(input); - нет необходимости указывать полное имя типа (если конечно в сборке нет другого типа с таким именем).
        //File.ReadAllLines для чтения небольших файлов - нормально, но для больших - не подходит, займет слишком много памяти.

        //Нет обработки исключений. Но так как неизвестен контекст использования метода - сойдет, ответственность на коде, вызывающем метод.
        //Нет валидации файла, что это действительно корректный xml-файл.
        //Слишком много логических действий в одном методе - поиск элемента, потом поиск атрибута. В такой кастомной реализации - лучше разделить.

        static string Func1(string input, string elementName, string attrName)
        {
            string[] lines = System.IO.File.ReadAllLines(input);
            string result = null;
            foreach (var line in lines) //
            {
                var startElEndex = line.IndexOf(elementName);
                if (startElEndex != -1) // Можно заменить на if (startElEndex == -1) continue; чтобы уменьшить вложенность (тут и в каждом таком случае ниже).
                {
                    if (line[startElEndex - 1] == '<')
                    {
                        var endElIndex = line.IndexOf('>', startElEndex - 1); //В xml нет гарантии, что закрывающая скобка элемента будет в той же строке, что и открывающая. 
                        var attrStartIndex = line.IndexOf(attrName, startElEndex, endElIndex - startElEndex + 1); //Поэтому тут в таком случае count будет отрицательным и упадет ArgumentOutOfRangeException.
                        if (attrStartIndex != -1)
                        {
                            //В xml нет гарантии, что открывающая и закрывающая кавычки значения атрибута будут в одной и той же строке.
                            //Потенциально - выход за границы массива.
                            //Потенциально - бесконечный цикл.
                            //Хотя за счет предыдущей ошибки с отрицательным count выполнение сюда не дойдет.
                            int valueStartIndex = attrStartIndex + attrName.Length + 2;
                            while (line[valueStartIndex] != '"')
                            {
                                result += line[valueStartIndex]; //Конкатенация строк в цикле - плохо, тратит память. Конечно вряд ли атрибут будет длинной в 10000 символов. Но лучше использовать StringBuilder.
                                valueStartIndex++;
                            }
                            break;
                        }
                    }
                }
            }

            return result;
        }

        //В xml файле может быть несколько элементов с одинаковым именем, содержащих атрибуты с одинаковым именем.
        //Но исходя из логики первоначального метода, вернуть надо только один.
        //По умолчанию - первый.

        //Вариант - написать класс-сервис, который будет принимать путь к xml файлу,
        //выполнять необходимую валидацию и предоставлять методы доступа к элементам xml-файла.
        //Но в постановке задачи - реализация алгоритма, поэтому оставляю функциональный подход.
        //Писать кастомный парсер - очень грустно.
        public static string? GetXmlAttributeValueByElementAndName(string xmlFilePath, string elementName, string attrName)
        {
            var settings = new XmlReaderSettings();
            settings.ValidationType = ValidationType.DTD;
            using (var reader = XmlReader.Create(xmlFilePath, settings)) // Тут же будет произведена валидация на well formed XML.
            {
                while (reader.Read())
                {
                    if (reader.NodeType != XmlNodeType.Element || reader.Name != elementName) continue;
                    var attrValue = reader.GetAttribute(attrName);
                    if (attrValue != null)
                        return attrValue;
                }
            }

            return null;
        }
    }
}
