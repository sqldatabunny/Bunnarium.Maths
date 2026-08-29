namespace Bunnarium.Tools;

/// <summary> Types that inherit this interface have an additional <see cref="ToString(byte, int, int)">ToString</see> method that can applies number-formatting logic to its output. This is useful for creating a cleaner data-inpsection environment when in debug mode.
/// </summary>
public interface IPrintable {

    /// <returns> A string summarizing this an object's state in which numbers are formatted according to the given arguments.
    /// </returns>
    /// <param name="digits"> The number of digits, including the decimal point when the input is a floating point number, to round floating-point numbers to. This includes digits on both sides of the decimal point.
    /// </param>
    /// <param name="integerLength"> The minimum number of digits to display in integers and before decimal points in floating point numbers, prepending 0s to these numbers if necessary to reach that minimum.
    /// </param>
    /// <param name="padToLength"> The length to left-pad numbers too after the <paramref name="digits"/> and <paramref name="integerLength"/> parameters have been applied.
    /// </param>
    public string ToString(byte digits, int integerLength, int padToLength);
    }

