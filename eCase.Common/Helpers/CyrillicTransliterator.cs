using System.Collections.Generic;
using System.Text;

namespace eCase.Common.Helpers
{
    public class CyrillicTransliterator
    {
        public static string TransliterateCyrillic(string cyrillicString)
        {
            Dictionary<char, string> transliterateDictionary =
                new Dictionary<char, string>();

            InitializeTransliterateDictionary(transliterateDictionary);

            StringBuilder stringBuilder = new StringBuilder();
            foreach (char ch in cyrillicString)
            {
                if (transliterateDictionary.ContainsKey(ch))
                {
                    stringBuilder.Append(transliterateDictionary[ch]);
                }
                else
                {
                    stringBuilder.Append(ch);
                }
            }

            return stringBuilder.ToString();
        }

        private static void InitializeTransliterateDictionary(Dictionary<char, string> transliterateDictionary)
        {
            transliterateDictionary.Add('а', "a");
            transliterateDictionary.Add('А', "A");
            transliterateDictionary.Add('б', "b");
            transliterateDictionary.Add('Б', "B");
            transliterateDictionary.Add('в', "v");
            transliterateDictionary.Add('В', "V");
            transliterateDictionary.Add('г', "g");
            transliterateDictionary.Add('Г', "G");
            transliterateDictionary.Add('д', "d");
            transliterateDictionary.Add('Д', "D");
            transliterateDictionary.Add('е', "e");
            transliterateDictionary.Add('Е', "E");
            transliterateDictionary.Add('ж', "zh");
            transliterateDictionary.Add('Ж', "ZH");
            transliterateDictionary.Add('з', "z");
            transliterateDictionary.Add('З', "Z");
            transliterateDictionary.Add('и', "i");
            transliterateDictionary.Add('И', "I");
            transliterateDictionary.Add('й', "y");
            transliterateDictionary.Add('Й', "Y");
            transliterateDictionary.Add('к', "k");
            transliterateDictionary.Add('К', "K");
            transliterateDictionary.Add('л', "l");
            transliterateDictionary.Add('Л', "L");
            transliterateDictionary.Add('м', "m");
            transliterateDictionary.Add('М', "M");
            transliterateDictionary.Add('н', "n");
            transliterateDictionary.Add('Н', "N");
            transliterateDictionary.Add('о', "o");
            transliterateDictionary.Add('О', "O");
            transliterateDictionary.Add('п', "p");
            transliterateDictionary.Add('П', "P");
            transliterateDictionary.Add('р', "r");
            transliterateDictionary.Add('Р', "R");
            transliterateDictionary.Add('с', "s");
            transliterateDictionary.Add('С', "S");
            transliterateDictionary.Add('т', "t");
            transliterateDictionary.Add('Т', "T");
            transliterateDictionary.Add('у', "u");
            transliterateDictionary.Add('У', "U");
            transliterateDictionary.Add('ф', "f");
            transliterateDictionary.Add('Ф', "F");
            transliterateDictionary.Add('х', "h");
            transliterateDictionary.Add('Х', "H");
            transliterateDictionary.Add('ц', "ts");
            transliterateDictionary.Add('Ц', "TS");
            transliterateDictionary.Add('ч', "ch");
            transliterateDictionary.Add('Ч', "CH");
            transliterateDictionary.Add('ш', "sh");
            transliterateDictionary.Add('Ш', "SH");
            transliterateDictionary.Add('щ', "sht");
            transliterateDictionary.Add('Щ', "SHT");
            transliterateDictionary.Add('ъ', "a");
            transliterateDictionary.Add('Ъ', "A");
            transliterateDictionary.Add('ь', "y");
            transliterateDictionary.Add('Ь', "Y");
            transliterateDictionary.Add('ю', "yu");
            transliterateDictionary.Add('Ю', "YU");
            transliterateDictionary.Add('я', "ya");
            transliterateDictionary.Add('Я', "YA");
        }
    }
}
