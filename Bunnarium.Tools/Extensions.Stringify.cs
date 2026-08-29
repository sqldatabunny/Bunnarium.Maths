using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace Bunnarium.Tools;

public static partial class Extensions {

    /// <summary> Returns a string representing the number, formatted for printability (particularly for monospace fonts).
    /// </summary>
    /// <remarks>
    /// <para/><b>Examples:</b>
    /// <para/><c>10000.Stringify(5, 0, 0)</c> → <c>10000</c>
    /// <para/><c>10000.Stringify(4, 0, 0)</c> → <c>1E+4</c> // shortens to exponential form
    /// <para/><c>10000.Stringify(7, 0, 0)</c> → <c>10000</c> // a larger <paramref name="digits"/> value on an integer type won't automatically add decimal values or prepend to a length without an actionable <paramref name="padToLength"/> argument
    /// <para/><c>10000f.Stringify(7, 0, 0)</c> → <c>10000.0</c> // a floating point value with a larger <paramref name="digits"/> argument <em>will</em> add decimals...
    /// <para/><c>10000f.Stringify(6, 0, 0)</c> → <c>10000</c> // ... but not if it's only enough to add a decimal point
    /// <para/><c>9.999.Stringify(5, 0, 0)</c> → <c>9.999</c> // <paramref name="digits"/> is large enough to fully represent the number
    /// <para/><c>9.9999.Stringify(5, 0, 0)</c> → <c>10.00</c> // <paramref name="digits"/> is not large enough to fully represent the number without rounding
    /// <para/><c>9.9999.Stringify(4, 0, 0)</c> → <c>10.0</c>
    /// <para/><c>9.9999.Stringify(3, 0, 0)</c> → <c>10</c> // the output won't align to a length of <c>3</c> without an actionable <paramref name="padToLength"/> argument.
    /// <para/><c>9.9999.Stringify(3, 0, 3)</c> → <c>_10</c> // rather than rendering only the rounded number and a decimal mark, the result will be left-padded
    /// <para/><c>123.45.Stringify(5, 0, 6)</c> → <c>_123.5</c>
    /// <para/><c>123.45.Stringify(5, 0, 6, padWithZeroes: true)</c> → <c>0123.5</c>
    /// <para/><c>67.67.Stringify(5, 2, 5)</c> → <c>67.67</c>
    /// <para/><c>67.67.Stringify(6, 2, 5)</c> → <c>67.670</c>
    /// <para/><c>67.67.Stringify(6, 3, 5, padWithZeroes: false)</c> → <c>067.67</c> // a prepending <paramref name="integerLength"/> value will still prepend <c>0</c>s when <paramref name="padWithZeroes"/>  is <see langword="false"/>
    /// </remarks>
    /// <param name="value"> The value to format.
    /// </param>
    /// <param name="digits"> The number of digits, including any <see cref="NumberFormatInfo.NumberDecimalDigits">decimal</see> character (but excluding any <see cref="NumberFormatInfo.NegativeSign">negative sign</see> characters, to fit the number to. For floating point values, the number of digits to the right of the decimal will be <c><paramref name="digits"/> - <paramref name="integerLength"/> - 1</c>.
    /// </param>
    /// <param name="integerLength"> The minimum number of digits to print in the integer part of the number. For floating point values, the budget for digits to the right of the decimal will be what remains after this value and any <see cref="NumberFormatInfo.NumberDecimalDigits">decimal</see> and <see cref="NumberFormatInfo.NegativeSign">negative sign</see> characters are applied.
    /// </param>
    /// <param name="padToLength"> The string length to left-pad the output to if the resulting number is shorter than <paramref name="padToLength"/>.
    /// </param>
    /// <param name="padWithZeroes"> Whether the number should be prepended with <c>0</c>s or with spaces. Note that a prepending <paramref name="integerLength"/> value will still prepend <c>0</c>s when <paramref name="padWithZeroes"/>  is <see langword="false"/>.
    /// </param>
    /// <param name="allowExponential"> Whether the number can be represented in exponential form (e.g., <c>1.23E+2</c>) when the number won't fit within the <paramref name="digits"/> budget without it but will with it.
    /// </param>
    /// <param name="provider">Optionally provides a <see cref="NumberFormatInfo"/> to format rendered value.
    /// </param>
    public static string Stringify<T>(
        this T value,
        byte digits,
        int integerLength,
        int padToLength,
        bool padWithZeroes = false,
        bool allowExponential = true,
        IFormatProvider? provider = null
        )
    where T : unmanaged, INumberBase<T> {
        Debug.Assert(integerLength >= 0);
        Debug.Assert(padToLength >= 0);

        // these checks will fold into constants
        var isInteger =
               typeof(T) != typeof(decimal)
            && typeof(T) != typeof(double)
            && typeof(T) != typeof(float)
            && typeof(T) != typeof(Half);

        var maxDecimals = isInteger
            ? digits
            : typeof(T) == typeof(decimal)
                ? 29
                : typeof(T) == typeof(double)
                    ? 17
                    : typeof(T) == typeof(float)
                        ? 9
                        : 5; // System.Half

        var format = provider is null
            ? NumberFormatInfo.InvariantInfo
            : NumberFormatInfo.GetInstance(provider);

        var sb = new StringBuilder(Math.Max(digits + 1, padToLength));

        if (T.IsFinite(value) == false) {
            sb.Append(value.ToString(
                format: null,
                formatProvider: format
                ));
            }
        else {
            var integers = Math.Max(
                CharacteristicLength(FormatUnsigned(value, "0.################")),
                integerLength
                );

            // calculate the number of decimals allowed from the remaining budget
            var decimals = isInteger
                ? 0
                : Math.Clamp(digits - integers - 1, 0, maxDecimals);

            var rounded = FormatUnsigned(value, $"F{decimals.ToString(CultureInfo.InvariantCulture)}");

            // a hack to resolve edge case e.g., (9.999.Stringify(4, 1, 0) -> 10.00 instead of 10.0)
            if (decimals > 0 && CharacteristicLength(rounded) > integers) {
                var places = Math.Clamp(digits - CharacteristicLength(rounded) - 1, 0, maxDecimals);
                rounded = FormatUnsigned(value, $"F{places.ToString(CultureInfo.InvariantCulture)}");
                }

            string? renderExponential;

            //  a non-zero value rounded away to nothing within the budget
            var underflowed = (T.IsZero(value) == false) && rounded.AsSpan().IndexOfAnyExcept('0', '.') < 0;

            if (allowExponential && (CharacteristicLength(rounded) > digits || underflowed)) {
                var exponentText = FormatUnsigned(value, "0.################E+0");
                var exponentWidth = exponentText.Length - exponentText.IndexOf('E') - 1;
                var mantissaDigits = Math.Clamp(digits - exponentWidth - 3, 0, maxDecimals);

                exponentText = FormatUnsigned(value, ExponentPattern(mantissaDigits));

                if (mantissaDigits > 0 && exponentText.Length > digits) {
                    var expDecimals = Math.Max(mantissaDigits - exponentText.Length + digits, 0);
                    exponentText = FormatUnsigned(value, ExponentPattern(expDecimals));
                    }
                renderExponential = exponentText;
                }
            else renderExponential = null;

            // append the sign
            if (T.IsNegative(value))
                sb.Append('-');

            // append the exponential value if that's the best representation within the digit budget
            if (renderExponential is not null && renderExponential.Length <= rounded.Length) {
                sb.Append(renderExponential);
                }
            else {
                var zeroes = integerLength - CharacteristicLength(rounded);
                if (zeroes > 0) sb.Append('0', zeroes);
                sb.Append(rounded);
                }

            // format numeric symbols to the provided number format
            if (format.NegativeSign != "-")
                sb.Replace("-", format.NegativeSign);
            if (format.PositiveSign != "+")
                sb.Replace("+", format.PositiveSign);
            if (format.NumberDecimalSeparator != ".")
                sb.Replace(".", format.NumberDecimalSeparator);
            }

        // prepend any 0s empty spaces
        var emptySpaces = padToLength - sb.Length;

        if (emptySpaces <= 0)
            return sb.ToString();

        var prepend0s = padWithZeroes && T.IsFinite(value);

        if (prepend0s) {
            sb.Insert(
                index: T.IsNegative(value) ? 1 : 0,
                value: "0",
                count: emptySpaces
                );
            }
        else { // prepend spaces
            sb.Insert(
                index: 0,
                value: " ",
                count: emptySpaces
                );
            }

        return sb.ToString();

        static int CharacteristicLength(string text) {
            var decimalAt = text.IndexOf('.');
            return decimalAt != -1
                 ? decimalAt
                 : text.Length;
            }

        static string FormatUnsigned(T value, string format) {
            var text = value.ToString(format, CultureInfo.InvariantCulture);
            return text[0] == '-' ? text[1..] : text;
            }

        static string ExponentPattern(int decimals) {
            return decimals == 0
                 ? "0E+0"
                 : $"0.{new string('0', decimals)}E+0";
            }
        }
    }
