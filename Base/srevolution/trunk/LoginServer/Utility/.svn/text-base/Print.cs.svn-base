using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoginServer
{
    public partial class Systems
    {
        public class Print
        {
            public static void Format(String format, Object arg0)
            {
                Format(null, format, new Object[] { arg0 });
            }

            public static void Format(String format, Object arg0, Object arg1)
            {
                Format(null, format, new Object[] { arg0, arg1 });
            }

            public static void Format(String format, Object arg0, Object arg1, Object arg2)
            {
                Format(null, format, new Object[] { arg0, arg1, arg2 });
            }

            public static void Format(string format, params object[] args)
            {
                Format(null, format, args);
            }

            public static void Format(IFormatProvider provider, string format, params object[] args)
            {
                StringBuilder b = new StringBuilder();
                FormatHelper(b, provider, format, args);
            }

            internal static void FormatHelper(StringBuilder result, IFormatProvider provider, string format, params object[] args)
            {
                if (format == null || args == null)
                    throw new ArgumentNullException();

                int ptr = 0;
                int start = ptr;
                while (ptr < format.Length)
                {
                    char c = format[ptr++];

                    if (c == '{')
                    {
                        result.Append(format, start, ptr - start - 1);

                        // check for escaped open bracket

                        if (format[ptr] == '{')
                        {
                            start = ptr++;
                            continue;
                        }

                        // parse specifier

                        int n, width;
                        bool left_align;
                        string arg_format;

                        ParseFormatSpecifier(format, ref ptr, out n, out width, out left_align, out arg_format);
                        if (n >= args.Length)
                            throw new FormatException("Index (zero based) must be greater than or equal to zero and less than the size of the argument list.");

                        // format argument

                        object arg = args[n];

                        string str;
                        ICustomFormatter formatter = null;
                        if (provider != null)
                            formatter = provider.GetFormat(typeof(ICustomFormatter))
                                as ICustomFormatter;
                        if (arg == null)
                            str = String.Empty;
                        else if (formatter != null)
                            str = formatter.Format(arg_format, arg, provider);
                        else if (arg is IFormattable)
                            str = ((IFormattable)arg).ToString(arg_format, provider);
                        else
                            str = arg.ToString();

                        // pad formatted string and append to result

                        if (width > str.Length)
                        {
                            const char padchar = ' ';
                            int padlen = width - str.Length;

                            if (left_align)
                            {
                                result.Append(str);
                                result.Append(padchar, padlen);
                            }
                            else
                            {
                                result.Append(padchar, padlen);
                                result.Append(str);
                            }
                        }
                        else
                            result.Append(str);

                        start = ptr;
                    }
                    else if (c == '}' && ptr < format.Length && format[ptr] == '}')
                    {
                        result.Append(format, start, ptr - start - 1);
                        start = ptr++;
                    }
                    else if (c == '}')
                    {
                        throw new FormatException("Input string was not in a correct format.");
                    }
                }

                if (start < format.Length)
                    result.Append(format, start, format.Length - start);
            }
            private static void ParseFormatSpecifier(string str, ref int ptr, out int n, out int width,
                                              out bool left_align, out string format)
            {
                // parses format specifier of form:
                //   N,[\ +[-]M][:F]}
                //
                // where:

                try
                {
                    // N = argument number (non-negative integer)

                    n = ParseDecimal(str, ref ptr);
                    if (n < 0)
                        throw new FormatException("Input string was not in a correct format.");

                    // M = width (non-negative integer)

                    if (str[ptr] == ',')
                    {
                        // White space between ',' and number or sign.
                        ++ptr;
                        while (Char.IsWhiteSpace(str[ptr]))
                            ++ptr;
                        int start = ptr;

                        format = str.Substring(start, ptr - start);

                        left_align = (str[ptr] == '-');
                        if (left_align)
                            ++ptr;

                        width = ParseDecimal(str, ref ptr);
                        if (width < 0)
                            throw new FormatException("Input string was not in a correct format.");
                    }
                    else
                    {
                        width = 0;
                        left_align = false;
                        format = String.Empty;
                    }

                    // F = argument format (string)

                    if (str[ptr] == ':')
                    {
                        int start = ++ptr;
                        while (str[ptr] != '}')
                            ++ptr;

                        format += str.Substring(start, ptr - start);
                    }
                    else
                        format = null;

                    if (str[ptr++] != '}')
                        throw new FormatException("Input string was not in a correct format.");
                }
                catch (IndexOutOfRangeException)
                {
                    throw new FormatException("Input string was not in a correct format.");
                }
            }
            private static int ParseDecimal(string str, ref int ptr)
            {
                int p = ptr;
                int n = 0;
                while (true)
                {
                    char c = str[p];
                    if (c < '0' || '9' < c)
                        break;

                    n = n * 10 + c - '0';
                    ++p;
                }

                if (p == ptr)
                    return -1;

                ptr = p;
                return n;
            }
        }
    }
}
