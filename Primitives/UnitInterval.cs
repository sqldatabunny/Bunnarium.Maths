using System.Diagnostics;

namespace Bunnarium.Maths.Primitives;

/// <summary> A value that must be between 0 and 1 (inclusive) that can be used to represent a value between 0% and 100%.
/// </summary>
[DebuggerDisplay("{ToString(), nq}")]
public struct UnitInterval<T>
    : IEquatable<UnitInterval<T>>
    , IComparable<UnitInterval<T>>
    , IPrintable
    where T : unmanaged, IFloatingPoint<T> {

    private T _value;

    public T Value {
        readonly get => _value;
        set => _value = T.Clamp(value, T.Zero, T.One);
        }

    /// <summary> Creates a new <see cref="UnitInterval{T}"/> with a <paramref name="value"/> clamped to 0 and 1, inclusive.
    /// </summary>
    public UnitInterval(T value) {
        _value = T.Clamp(value, T.Zero, T.One);
        }

    /// <summary> Hidden constructor for creating unchecked UnitIntervals as an edge-case optimization.
    /// </summary>
    private UnitInterval(T value, bool hidden) {
        _value = value;
        }

    /// <summary> Creates and returns a unit interval with a value of <paramref name="uncheckedValue"/> without clamping for use in edge-case optimizations. Any subsequent changes to the value will be clamped.
    /// </summary>
    public static UnitInterval<T> CreateUnchecked(T uncheckedValue) {
        return new(uncheckedValue, true);
        }

    public static UnitInterval<T> CreateChecked(T checkedValue) {
        return new(checkedValue);
        }

    /// <summary> A <see cref="UnitInterval{T}"/> with a value of 0.
    /// </summary>
    public static readonly UnitInterval<T> Zero = new(T.Zero);

    /// <summary> A <see cref="UnitInterval{T}"/> with a value of 1.
    /// </summary>
    public static readonly UnitInterval<T> One = new(T.One);

    /// <summary> Implicitly converts a <see cref="UnitInterval{T}"/> to a <typeparamref name="T"/>.
    /// </summary>
    public static implicit operator T(UnitInterval<T> interval) => interval.Value;

    /// <summary> Implicitly converts a <typeparamref name="T"/> to a <see cref="UnitInterval{T}"/>.
    /// </summary>
    public static implicit operator UnitInterval<T>(T value) => new(value);

    public override readonly string ToString() {
        var precision = typeof(T) == typeof(float) ? 8 : (typeof(T) == typeof(double) ? 16 : 4);
        return $"{GenericNumbers<T>.ToDouble(Value * GenericNumbers<T>.FromBinaryInteger(100)).ToString($"F{precision}").PadLeft(precision + 2)}%";
        }

    public readonly string ToString(byte digits, int integerLength, int padToLength) {
        return Extensions.Stringify(Value * GenericNumbers<T>.FromBinaryInteger(100), digits, integerLength, padToLength) + "%";
        }

    #region Equality and comparison

    public override readonly bool Equals(object? obj) {
        return obj is UnitInterval<T> unit && Equals(unit);
        }

    public readonly bool Equals(UnitInterval<T> other) {
        return Value == other.Value;
        }

    public int CompareTo(UnitInterval<T> other) {
        return Value.CompareTo(other.Value);
        }

    public override readonly int GetHashCode() {
        return Value.GetHashCode();
        }

    public static bool operator ==(UnitInterval<T> left, UnitInterval<T> right) {
        return left.Equals(right);
        }

    public static bool operator !=(UnitInterval<T> left, UnitInterval<T> right) {
        return !(left == right);
        }

    public static bool operator <(UnitInterval<T> left, UnitInterval<T> right) {
        return left.CompareTo(right) < 0;
        }

    public static bool operator <=(UnitInterval<T> left, UnitInterval<T> right) {
        return left.CompareTo(right) <= 0;
        }

    public static bool operator >(UnitInterval<T> left, UnitInterval<T> right) {
        return left.CompareTo(right) > 0;
        }

    public static bool operator >=(UnitInterval<T> left, UnitInterval<T> right) {
        return left.CompareTo(right) >= 0;
        }

    #endregion Equality and comparison
    }
