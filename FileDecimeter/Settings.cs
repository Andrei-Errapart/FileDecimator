using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;


namespace FileDecimeter
{
    public class Settings
    {
        #region INTERNALS
        class SeparatorCharacter
        {
            public string Name;
            public char Character;

            public static SeparatorCharacter Create(string name, char character)
            {
                return new SeparatorCharacter()
                {
                    Name = name,
                    Character = character
                };
            }
        }

        static readonly SeparatorCharacter[] _Separators = new SeparatorCharacter[] {
            SeparatorCharacter.Create("tab", '\t'),
            SeparatorCharacter.Create("space", ' ')
        };

        const string NAME_DOT = "Dot";
        const string NAME_COLUMN = "Column";
        const string NAME_NEGATIVE = "Negative";
        const string NAME_SEPARATOR = "Separator";
        const string TEXT_FALSE = "False";
        const string TEXT_TRUE = "True";

        static readonly char[] KEYVAL_SEPARATORS = new char[] { '=' };

        static bool StringCaseEqual(string s1, string s2)
        {
            return string.Equals(s1, s2, StringComparison.InvariantCultureIgnoreCase);
        }
        #endregion INTERNALS

        #region PUBLIC
        public char Dot = ',';
        public int Column = 3;
        public bool Negative = false;
        public char Separator = '\t';

        public static Settings Read(string filename)
        {
            var lines = File.ReadAllLines(filename);
            var r = new Settings();
            foreach (var line in lines)
            {
                var v = line.Split(KEYVAL_SEPARATORS, 2);
                if (v.Length >= 2)
                {
                    string key = v[0].Trim();
                    string val = v[1].Trim();
                    if (StringCaseEqual(key, NAME_DOT))
                    {
                        if (val.Length > 0)
                        {
                            r.Dot = val[0];
                        }
                    }
                    if (StringCaseEqual(key, NAME_COLUMN))
                    {
                        r.Column = int.Parse(val, NumberStyles.Any, CultureInfo.InvariantCulture);
                    }
                    else if (StringCaseEqual(key, NAME_NEGATIVE))
                    {
                        r.Negative = StringCaseEqual(val, TEXT_TRUE);
                    }
                    else if (StringCaseEqual(key, NAME_SEPARATOR))
                    {
                        bool found = false;
                        foreach (var sc in _Separators)
                        {
                            if (StringCaseEqual(sc.Name, val))
                            {
                                r.Separator = sc.Character;
                                found = true;
                                break;
                            }
                        }
                        if (!found && val.Length > 0)
                        {
                            r.Separator = val[0];
                        }
                    }
                }
            }
            return r;
        }

        public void Write(string filename)
        {
            using (var fo = new StreamWriter(filename))
            {
                fo.WriteLine(NAME_DOT + "=" + Dot);
                fo.WriteLine(NAME_COLUMN + "=" + Column.ToString(CultureInfo.InvariantCulture));
                string sep = new string(Separator, 1);
                foreach (var sc in _Separators)
                {
                    if (sc.Character == Separator)
                    {
                        sep = sc.Name;
                    }
                }
                fo.WriteLine(NAME_NEGATIVE + "=" + (Negative ? TEXT_TRUE : TEXT_FALSE));
                fo.WriteLine(NAME_SEPARATOR + "=" + sep);
            }
        }
        #endregion PUBLIC
    }
}
