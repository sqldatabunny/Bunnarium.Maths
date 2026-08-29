using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Bunnarium.Maths.Primitives;

/// <summary> A container for a single float-point value. Vectors usually have at least two values, but this type allows for single-value representations of information in contexts where vectors are used <see cref="IVector{TVector, T}">generically</see>. This type should not be used outside of generic contexts, as many functions that one might call on a vector are trivial in the 1D case, thus making this type inefficient.
/// </summary>
[DebuggerDisplay("{ToString(), nq}")]
[StructLayout(LayoutKind.Sequential)]
public struct Vector1<T>
    : IFloatingPointVector<Vector1<T>, T>
    , IVectorOfHigherDimension<Vector1<T>, Vector2<T>, T>
    where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {

    /// <summary> The vector's component.
    /// </summary>
    public T X { readonly get; set; }

    #region Constructors & Factories

    public Vector1(T x) {
        X = x;
        }

    public static Vector1<T> Create(T value) {
        return new(value);
        }

    #endregion Constructors & Factories

    #region Constants

    public static Vector1<T> Forward {
        get => One;
        }

    public static Vector1<T> MaxValue { get; } = Vector1<T>.Create(T.MaxValue);

    public static Vector1<T> MinValue { get; } = Vector1<T>.Create(T.MinValue);

    public static Vector1<T> One {
        get => new(T.One);
        }

    public static Vector1<T> Right {
        get => One;
        }

    public static Vector1<T> RootN {
        get => One;
        }

    public static Vector1<T> Up {
        get => One;
        }

    public static Vector1<T> Zero {
        get => new(T.Zero);
        }

    #endregion Constants

    #region Absolute Values

    public static Vector1<T> Abs(in Vector1<T> vector) {
        return new(T.Abs(vector.X));
        }

    public readonly Vector1<T> Abs() {
        return new(T.Abs(X));
        }

    #endregion Absolute Values

    #region Conversions

    public static Vector2<T> Append(in Vector1<T> vector, T value) {
        return new(vector.X, value);
        }

    public static Span<T> ToSpan(ref Vector1<T> vector) {
        return MemoryMarshal.CreateSpan(ref Unsafe.As<Vector1<T>, T>(ref vector), 1);
        }

    public readonly Vector2<T> Append(T value) {
        return Append(in this, value);
        }

    #endregion Conversions

    #region Dot

    public static T Dot(Vector1<T> left, Vector1<T> right) {
        return left.X * right.X;
        }

    public readonly T Dot(Vector1<T> other) {
        return X * other.X;
        }

    #endregion Dot

    #region Floor & Ceiling

    public static void Ceiling(ref Vector1<T> vector) {
        vector.X = T.Ceiling(vector.X);
        }

    public static Vector1<T> Ceiling(Vector1<T> vector) {
        return new(T.Ceiling(vector.X));
        }

    public static void Floor(ref Vector1<T> vector) {
        vector.X = T.Floor(vector.X);
        }

    public static Vector1<T> Floor(Vector1<T> vector) {
        return new(T.Floor(vector.X));
        }

    public readonly Vector1<T> Ceiling() {
        return new(T.Ceiling(X));
        }

    public readonly Vector1<T> Floor() {
        return new(T.Floor(X));
        }

    #endregion Floor & Ceiling

    #region Horizontal

    public readonly T AbsoluteAverage => T.Abs(X);
    public readonly T AbsoluteSum => T.Abs(X);
    public readonly T Average => X;
    public readonly T Product => X;
    public readonly T Sum => X;

    public static T HorizontalAbsoluteAverage(in Vector1<T> vector) {
        return T.Abs(vector.X);
        }

    public static T HorizontalAbsoluteSum(in Vector1<T> vector) {
        return T.Abs(vector.X);
        }

    public static T HorizontalAverage(in Vector1<T> vector) {
        return vector.X;
        }

    public static T HorizontalProduct(in Vector1<T> vector) {
        return vector.X;
        }

    public static T HorizontalSum(in Vector1<T> vector) {
        return vector.X;
        }

    #endregion Horizontal

    #region Lerp

    public static Vector1<T> Lerp(Vector1<T> from, Vector1<T> to, T amount) {
        var a = from.X;
        var b = to.X;
        return new(a * (T.One - amount) + b * amount);
        }

    public readonly Vector1<T> Lerp(Vector1<T> to, T amount) {
        var a = X;
        var b = to.X;
        return new(a * (T.One - amount) + b * amount);
        }

    #endregion Lerp

    #region Lexicographical Ordering

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool LexicographicallyPrecedes(Vector1<T> left, Vector1<T> right) {
        return left.X < right.X;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool LexicographicallyPrecedes(Vector1<T> other) {
        return X < other.X;
        }

    #endregion Lexicographical Ordering

    #region Magnitude

    public readonly T Magnitude {
        get => T.Abs(X);
        }

    public readonly T MagnitudeSquared {
        get => X * X;
        }

    #endregion Magnitude

    #region Min / Max

    public static T HorizontalMax(Vector1<T> vector) {
        return vector.X;
        }

    public static T HorizontalMin(Vector1<T> vector) {
        return vector.X;
        }

    public static Vector1<T> Max(Vector1<T> left, Vector1<T> right) {
        return new(T.Max(left.X, right.X));
        }

    public static Vector1<T> Min(Vector1<T> left, Vector1<T> right) {
        return new(T.Min(left.X, right.X));
        }

    public readonly T HorizontalMax() {
        return X;
        }

    public readonly T HorizontalMin() {
        return X;
        }

    public readonly Vector1<T> Max(Vector1<T> other) {
        return new Vector1<T>(
            x: T.Max(X, other.X)
            );
        }

    public readonly Vector1<T> Min(Vector1<T> other) {
        return new Vector1<T>(
            x: T.Min(X, other.X)
            );
        }

    #endregion Min / Max

    #region Negation

    public static Vector1<T> Negate(Vector1<T> vector) {
        return new(-vector.X);
        }

    public static void Negate(ref Vector1<T> vector) {
        vector.X = -vector.X;
        }

    public readonly Vector1<T> Negate() {
        return new(-X);
        }

    #endregion Negation

    #region Normalization

    public readonly bool IsNormalized {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => MagnitudeSquared == T.One;
        }

    public static void Normalize(ref Vector1<T> vector) {
        vector.X /= vector.Magnitude;
        }

    public static Vector1<T> Normalize(Vector1<T> vector) {
        return new(vector.X / T.Abs(vector.X));
        }

    public static void Normalize(ref Vector1<T> vector, T magnitude) {
        vector.X = vector.X / T.Abs(vector.X) * magnitude;
        }

    public static Vector1<T> Normalize(Vector1<T> vector, T magnitude) {
        return new(vector.X / T.Abs(vector.X) * magnitude);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Vector1<T> Normalize() {
        return Normalize(this);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Vector1<T> Normalize(T magnitude) {
        return Normalize(this, magnitude);
        }

    #endregion Normalization

    #region Operators, Equatability & Comparability

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector1<T> Add(Vector1<T> left, Vector1<T> right) {
        return left + right;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector1<T> ComponentDivide(Vector1<T> left, Vector1<T> right) {
        return new(left.X / right.X);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector1<T> ComponentMultiply(Vector1<T> left, Vector1<T> right) {
        return new(left.X * right.X);
        }

    public static bool Equals(Vector1<T> left, Vector1<T> right) {
        return left.X == right.X;
        }

    public static Vector1<T> operator -(Vector1<T> left, Vector1<T> right) {
        return new(left.X - right.X);
        }

    public static Vector1<T> operator -(Vector1<T> vector, T value) {
        return new(vector.X - value);
        }

    public static Vector1<T> operator -(Vector1<T> a) {
        return new(-a.X);
        }

    public static bool operator !=(Vector1<T> left, Vector1<T> right) {
        return (left.X != right.X);
        }

    public static Vector1<T> operator *(Vector1<T> vector, T value) {
        return new(vector.X * value);
        }

    public static Vector1<T> operator *(T value, Vector1<T> vector) {
        return new(value * vector.X);
        }

    public static Vector1<T> operator /(Vector1<T> vector, T value) {
        return new(vector.X / value);
        }

    public static Vector1<T> operator /(T value, Vector1<T> vector) {
        return new(value / vector.X);
        }

    public static Vector1<T> operator +(Vector1<T> vector, T value) {
        return new(vector.X + value);
        }

    public static Vector1<T> operator +(Vector1<T> left, Vector1<T> right) {
        return new(left.X + right.X);
        }

    public static bool operator <(in Vector1<T> left, in Vector1<T> right) {
        return left.X < right.X;
        }

    public static bool operator <=(in Vector1<T> left, in Vector1<T> right) {
        return left.X <= right.X;
        }

    public static bool operator ==(Vector1<T> left, Vector1<T> right) {
        return (left.X == right.X);
        }

    public static bool operator >(in Vector1<T> left, in Vector1<T> right) {
        return left.X > right.X;
        }

    public static bool operator >=(in Vector1<T> left, in Vector1<T> right) {
        return left.X >= right.X;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector1<T> Subtract(Vector1<T> left, Vector1<T> right) {
        return left - right;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Vector1<T> Add(Vector1<T> other) {
        return this + other;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Vector1<T> ComponentDivide(Vector1<T> other) {
        return new(X / other.X);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Vector1<T> ComponentMultiply(Vector1<T> other) {
        return new(X * other.X);
        }

    public readonly bool Equals(Vector1<T> other) {
        return X == other.X;
        }

    public override readonly bool Equals(object? obj) {
        return obj is Vector1<T> vector && Equals(vector);
        }

    public override readonly int GetHashCode() {
        return X.GetHashCode();
        }

    public readonly Vector1<T> Subtract(Vector1<T> other) {
        return this - other;
        }

    #endregion Operators, Equatability & Comparability

    #region Orthogonality & Orthonormality

    public static bool IsOrthogonalWith(Vector1<T> left, Vector1<T> right) {
        return left.Dot(right) == T.Zero;
        }

    public static bool IsOrthonormalWith(Vector1<T> left, Vector1<T> right) {
        return left.Dot(right) == T.Zero && left.MagnitudeSquared == T.One && right.MagnitudeSquared == T.One;
        }

    public static Vector1<T> Orthogonal(Vector1<T> vector) {
        return Zero;
        }

    public readonly bool IsOrthogonalWith(Vector1<T> other) {
        return Dot(other) == T.Zero;
        }

    public readonly bool IsOrthonormalWith(Vector1<T> other) {
        return Dot(other) == T.Zero && MagnitudeSquared == T.One && other.MagnitudeSquared == T.One;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Vector1<T> Orthogonal() {
        return Orthogonal(this);
        }

    #endregion Orthogonality & Orthonormality

    #region Round

    public static void Round(ref Vector1<T> vector) {
        vector.X = T.Round(vector.X);
        }

    public static void Round(ref Vector1<T> vector, byte digits) {
        vector.X = T.Round(vector.X, digits);
        }

    public static Vector1<T> Round(Vector1<T> vector) {
        return new(T.Round(vector.X));
        }

    public static Vector1<T> Round(Vector1<T> vector, byte digits) {
        return new(T.Round(vector.X, digits));
        }

    public readonly Vector1<T> Round(byte digits) {
        return new(T.Round(X, digits));
        }

    public readonly Vector1<T> Round() {
        return new(T.Round(X));
        }

    #endregion Round

    #region Scale

    public static Vector1<T> Scale(Vector1<T> vector, T factor) {
        return vector * factor;
        }

    public static void Scale(ref Vector1<T> vector, T factor) {
        vector.X *= factor;
        }

    public readonly Vector1<T> Scale(T factor) {
        return this * factor;
        }

    #endregion Scale

    #region Sign

    public static Vector1<T> Sign(Vector1<T> vector) {
        return new(vector.X > T.Zero ? T.One : vector.X < T.Zero ? T.NegativeOne : T.Zero);
        }

    public readonly Vector1<T> Sign() {
        return new(X > T.Zero ? T.One : X < T.Zero ? T.NegativeOne : T.Zero);
        }

    #endregion Sign

    #region SizeOf and Intrinsics

    public static int Length { get; } = 1;

    public static unsafe int SizeOf { get; } = sizeof(T) * Length;

    #endregion SizeOf and Intrinsics

    #region Step

    public static Vector1<T> Step(Vector1<T> left, Vector1<T> right) {
        return left + Sign(right - left);
        }

    public readonly Vector1<T> Step(Vector1<T> other) {
        return this + Sign(other - this);
        }

    #endregion Step

    #region Strings

    public override readonly string ToString() {
        return $"[{X}]";
        }

    public readonly string ToString(byte digits, int integerLength, int paddingLength) {
        return $"[{X.Stringify(digits, integerLength, paddingLength)}]";
        }

    #endregion Strings
    }
