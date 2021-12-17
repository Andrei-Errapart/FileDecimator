using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;


namespace FileDecimeter
{
    class Program
    {
        const string SETTINGS_FILENAME = "FileDecimeter.ini";
        static readonly char[] SEPARATORS = new char[] { ';', ' ', '\t' };

        // ------------------------------------------------------------------
        static void printUsage()
        {
            Console.WriteLine("Usage: FileDecimeter INPUT_FILE OUTPUT_FILE");
        }

        // ------------------------------------------------------------------
        static string truncateDecimals(string s, char dot, bool negative)
        {
            var s_real = dot=='.' ? s : s.Replace(dot, '.'); 
            var m = double.Parse(s_real, NumberStyles.Any, CultureInfo.InvariantCulture);
            var m10 = (negative ? -0.1 : 0.1) * (m > 0  ? Math.Floor(10 * m) : Math.Ceiling(10 * m));
            var r = m10.ToString("#.#", CultureInfo.InvariantCulture);
            var r_real = dot == '.' ? r : r.Replace('.', dot);
            return r_real;
        }

        // ------------------------------------------------------------------
        static void convert(string inputFilename, string outputFilename, Settings settings, char[] inputSeparators)
        {
            var col = settings.Column - 1;
            var sep = settings.Separator;
            var sb = new StringBuilder();

            Console.WriteLine("Input file: " + inputFilename);
            Exception first_error = null;
            int first_error_line = 0;
            int error_count = 0;

            using (var fin = new StreamReader(inputFilename))
            {
                int line_number = 0;
                Console.WriteLine("Output file: " + outputFilename);
                using (var fout = new StreamWriter(outputFilename))
                {
                    while (!fin.EndOfStream)
                    {
                        var line = fin.ReadLine().Trim();
                        var new_line = line;
                        ++line_number;
                        var v = line.Split(inputSeparators);
                        if (v.Length > col)
                        {
                            var old_s = v[col];
                            try
                            {
                                v[col] = truncateDecimals(old_s, settings.Dot, settings.Negative);
                                sb.Remove(0, sb.Length);
                                for (int i = 0; i < v.Length; ++i)
                                {
                                    if (i > 0)
                                    {
                                        sb.Append(sep);
                                    }
                                    sb.Append(v[i]);
                                }
                                new_line = sb.ToString();
                            }
                            catch (Exception ex)
                            {
                                if (first_error == null)
                                {
                                    first_error = ex;
                                    first_error_line = line_number;
                                }
                                ++error_count;
                                if (error_count < 3)
                                {
                                    Console.WriteLine(line_number.ToString() + ":" + ex.Message);
                                }
                            }
                        }
                        fout.WriteLine(new_line);
                    }
                }
            }
            Console.WriteLine("Completed, with " + error_count.ToString() + " errors.");
        }

        // ------------------------------------------------------------------
        static void Main(string[] args)
        {
            // 1. Read the settings file.
            Settings settings = null;
            if (File.Exists(SETTINGS_FILENAME))
            {
                try
                {
                    settings = Settings.Read(SETTINGS_FILENAME);
                }
                catch (Exception ex)
                {
                    // pass.
                    Console.WriteLine("Error reading settings file: " + ex.Message);
                }
            }
            else
            {
                Console.WriteLine("Settings file " + SETTINGS_FILENAME + " not found.");
            }
            if (settings == null)
            {
                Console.WriteLine("Creating new settings file " + SETTINGS_FILENAME);
                settings = new Settings();
                settings.Write(SETTINGS_FILENAME);
            }

            // 2. Check the usage.
            if (args.Length != 2)
            {
                printUsage();
                return;
            }

            // 3. Process the files.
            try
            {
                char[] separators;
                if (settings.Dot == ',')
                {
                    separators = SEPARATORS;
                }
                else
                {
                    separators = new char[SEPARATORS.Length + 1];
                    for (int i = 0; i < SEPARATORS.Length; ++i)
                    {
                        separators[i + 1] = SEPARATORS[i];
                    }
                    separators[0] = ',';
                }
                convert(args[0], args[1], settings, separators);
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine("Error: " + ex.Message);
            }
        }
    }
}
