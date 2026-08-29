namespace Bunnarium.Maths.Geometry;

/// <summary> Defines epsilon values for geometric tests, such as intersections.
/// </summary>
public static class Epsilon<T>
    where T : unmanaged, IFloatingPoint<T> {

    /// <remarks>
    /// <list type="bullet">
    /// <item><term><see langword="double"/></term> <description><c>1.0E-10</c></description></item>
    /// <item><term><see langword="float"/></term> <description><c>1.0E-6</c></description></item>
    /// <item><term><see langword="Half"/></term> <description><c>1.0E-3</c></description></item>
    /// </list>
    /// </remarks>
    public static readonly T VeryStrict = GetVeryStrictEpsilon();

    /// <remarks>
    /// <list type="bullet">
    /// <item> <term><see langword="double"/></term> <description><c>1.0E-8</c></description> </item>
    /// <item><term><see langword="float"/></term> <description><c>1.0E-5</c></description></item>
    /// <item><term><see langword="Half"/></term> <description><c>2.5E-3</c></description></item>
    /// </list>
    /// </remarks>
    public static readonly T Strict = GetStrictEpsilon();

    /// <remarks>
    /// <list type="bullet">
    /// <item> <term><see langword="double"/></term> <description><c>1.0E-7</c></description></item>
    /// <item><term><see langword="float"/></term> <description><c>1.0E-4</c></description></item>
    /// <item><term><see langword="Half"/></term> <description><c>5.0E-3</c></description></item>
    /// </list>
    /// </remarks>
    public static readonly T Lenient = GetLenientEpsilon();

    /// <remarks>
    /// <list type="bullet">
    /// <item> <term><see langword="double"/></term> <description><c>1.0E-5</c></description> </item>
    /// <item><term><see langword="float"/></term> <description><c>1.0E-3</c></description></item>
    /// <item><term><see langword="Half"/></term> <description><c>1.0E-2</c></description></item>
    /// </list>
    /// </remarks>
    public static readonly T VeryLenient = GetVeryLenientEpsilon();

    static T GetVeryStrictEpsilon() {
        if (typeof(T) == typeof(double))
            return T.CreateChecked(1.0E-10);
        if (typeof(T) == typeof(float))
            return T.CreateChecked(1.0E-6);
        if (typeof(T) == typeof(Half))
            return T.CreateChecked(1.0E-3);
        throw new TypeLoadException($"Type not supported");
        }

    static T GetStrictEpsilon() {
        if (typeof(T) == typeof(double))
            return T.CreateChecked(1.0E-8);
        if (typeof(T) == typeof(float))
            return T.CreateChecked(1.0E-5);
        if (typeof(T) == typeof(Half))
            return T.CreateChecked(2.5E-3);
        throw new TypeLoadException($"Type not supported");
        }

    static T GetLenientEpsilon() {
        if (typeof(T) == typeof(double))
            return T.CreateChecked(1.0E-7);
        if (typeof(T) == typeof(float))
            return T.CreateChecked(1.0E-4);
        if (typeof(T) == typeof(Half))
            return T.CreateChecked(5E-3);
        throw new TypeLoadException($"Type not supported");
        }

    static T GetVeryLenientEpsilon() {
        if (typeof(T) == typeof(double))
            return T.CreateChecked(1.0E-5);
        if (typeof(T) == typeof(float))
            return T.CreateChecked(1.0E-3);
        if (typeof(T) == typeof(Half))
            return T.CreateChecked(1E-2);
        throw new TypeLoadException($"Type not supported");
        }

    // public static implicit operator T(Epsilon<T> _) => Value; // usable if ever made a struct
    }
