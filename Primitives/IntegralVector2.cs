using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static Bunnarium.Tools.Extensions;
namespace Bunnarium.Maths.Primitives;

/// <summary> A container for two integer values, X and Y. Useful for a variety of purposes, a vector may represent a coordinate, a two-part value, an offset, or a classical vector with a direction and a <see cref="Magnitude">magnitude</see>.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
[DebuggerDisplay("{ToString(), nq}")]
public struct IntegralVector2<T> : IIntegralVector<IntegralVector2<T>, T>
    where T : unmanaged, IBinaryInteger<T>, IMinMaxValue<T> {

    /// <inheritdoc
    /// cref="DocStrings.VectorComponentX"/>
    public T X { readonly get; set; }

    /// <inheritdoc
    /// cref="DocStrings.VectorComponentY"/>
    public T Y { readonly get; set; }

    public readonly void Deconstruct(out T X, out T Y) {
        X = this.X;
        Y = this.Y;
        }

    #region Constructors and Factories

    public IntegralVector2(T x, T y) {
        X = x;
        Y = y;
        }

    /// <inheritdoc
    /// cref="IVector{TVector, T}.Create(T)"/>
    public IntegralVector2(T xy) {
        X = Y = xy;
        }

    public static IntegralVector2<T> Create(T value) {
        return new(value, value);
        }

    #endregion Constructors and Factories

    #region Constants and Initialization

    public static IntegralVector2<T> Forward { get; } = new(T.One, T.Zero);
    public static IntegralVector2<T> Backward { get; } = new(-T.One, T.Zero);

    public static int Length { get; } = 2;
    public static IntegralVector2<T> MaxValue { get; } = IntegralVector2<T>.Create(T.MaxValue);
    public static IntegralVector2<T> MinValue { get; } = IntegralVector2<T>.Create(T.MinValue);
    public static IntegralVector2<T> One { get; } = new(T.One, T.One);

    public static IntegralVector2<T> Right { get; } = new(T.One, T.Zero);
    public static unsafe int SizeOf { get; } = sizeof(T) * Length;
    public static IntegralVector2<T> Up { get; } = new(T.Zero, T.One);
    public static IntegralVector2<T> Zero { get; } = new(T.Zero, T.Zero);
    public static IntegralVector2<T> Left { get; } = new(-T.One, T.Zero);
    public static IntegralVector2<T> Down { get; } = new(T.Zero, -T.One);

    #endregion Constants and Initialization

    #region Absolute Values

    public static IntegralVector2<T> Abs(in IntegralVector2<T> vector) {
        return new(
            x: T.Abs(vector.X),
            y: T.Abs(vector.Y)
            );
        }

    public readonly IntegralVector2<T> Abs() {
        return new(
            x: T.Abs(X),
            y: T.Abs(Y)
            );
        }

    #endregion Absolute Values

    #region Comparability

    public static bool operator <(in IntegralVector2<T> left, in IntegralVector2<T> right) {
        return left.X < right.X
            && left.Y < right.Y;
        }

    public static bool operator <=(in IntegralVector2<T> left, in IntegralVector2<T> right) {
        return left.X <= right.X
            && left.Y <= right.Y;
        }

    public static bool operator >(in IntegralVector2<T> left, in IntegralVector2<T> right) {
        return left.X > right.X
            && left.Y > right.Y;
        }

    public static bool operator >=(in IntegralVector2<T> left, in IntegralVector2<T> right) {
        return left.X >= right.X && left.Y >= right.Y;
        }

    #endregion Comparability

    #region Conversions

    /// <returns> An <see cref="IntegralVector3{T}"/> populated in-part by this vector's values.
    /// </returns>
    /// <param name="z">The value of the output vector's third component.</param>
    public readonly IntegralVector3<T> ToIntegralVector3(T z) {
        return new(X, Y, z);
        }

    /// <returns> An <see cref="IntegralVector4{T}"/> populated in-part by this vector's values.
    /// </returns>
    /// <param name="z">The value of the output vector's third component.</param>
    /// <param name="w">The value of the output vector's fourth component.</param>
    public readonly IntegralVector4<T> ToIntegralVector4(T z, T w) {
        return new(X, Y, z, w);
        }

    public static Span<T> ToSpan(ref IntegralVector2<T> vector) {
        return MemoryMarshal.CreateSpan(ref Unsafe.As<IntegralVector2<T>, T>(ref vector), 2);
        }

    #endregion Conversions

    #region Grid-based functions

    /// <inheritdoc
    /// cref="IIntegralVector{TVector, T}.GetCartesianProduct(TVector)"/>
    public static Sets.CartesianProduct<IntegralVector2<T>, T> GetCartesianProduct(IntegralVector2<T> dimensions) {
        return new(dimensions);
        }

    static IEnumerable<IntegralVector2<T>> IIntegralVector<IntegralVector2<T>, T>.GetCartesianProduct(IntegralVector2<T> dimensions) {
        return new Sets.CartesianProduct<IntegralVector2<T>, T>(dimensions);
        }

    public static bool IsCornerOf(IntegralVector2<T> position, IntegralVector2<T> dimensions) {
        var x = position.X;
        var y = position.Y;
        var w = dimensions.X - T.One;
        var h = dimensions.Y - T.One;
        return (x == T.Zero || x == w) &&
               (y == T.Zero || y == h);
        }

    public static bool IsOnEdgeButNotCornerOf(IntegralVector2<T> position, IntegralVector2<T> dimensions) {
        var x = position.X;
        var y = position.Y;
        var w = dimensions.X - T.One;
        var h = dimensions.Y - T.One;
        return ((y == T.Zero || y == h) && (x != T.Zero && x != w)) ||
               ((x == T.Zero || x == w) && (y != T.Zero && y != h));
        }

    public static bool IsOnEdgeOf(IntegralVector2<T> position, IntegralVector2<T> dimensions) {
        return position.X == T.Zero
            || position.Y == T.Zero
            || position.X == dimensions.X - T.One
            || position.Y == dimensions.Y - T.One;
        }

    public static bool IsOnOutskirtsOf(IntegralVector2<T> position, IntegralVector2<T> dimensions) {
        return IsOnEdgeOf(position, dimensions);
        }

    public static bool IsOnSurfaceButNotEdgeOf(IntegralVector2<T> position, IntegralVector2<T> dimensions) {
        return IsOnEdgeOf(position, dimensions) == false;
        }

    /// <summary><inheritdoc/></summary>
    /// <remarks> For this function, <see cref="IntegralVector2{T}"/> is a degenerate case that always resolves to <see langword="true"/>.
    /// </remarks>
    public static bool IsOnSurfaceOf(IntegralVector2<T> position, IntegralVector2<T> dimensions) {
        return true;
        }

    public readonly IEnumerable<IntegralVector2<T>> GetCartesianProduct() {
        return GetCartesianProduct(this);
        }

    public readonly bool IsCornerOf(IntegralVector2<T> dimensions) {
        return IsCornerOf(this, dimensions);
        }

    public readonly bool IsOnEdgeButNotCornerOf(IntegralVector2<T> dimensions) {
        return IsOnEdgeButNotCornerOf(this, dimensions);
        }

    public readonly bool IsOnEdgeOf(IntegralVector2<T> dimensions) {
        return IsOnEdgeOf(this, dimensions);
        }

    public readonly bool IsOnOutskirtsOf(IntegralVector2<T> dimensions) {
        return IsOnOutskirtsOf(this, dimensions);
        }

    public readonly bool IsOnSurfaceButNotEdgeOf(IntegralVector2<T> dimensions) {
        return IsOnSurfaceButNotEdgeOf(this, dimensions);
        }

    /// <inheritdoc
    /// cref="IsOnSurfaceOf(IntegralVector2{T}, IntegralVector2{T})"/>
    public readonly bool IsOnSurfaceOf(IntegralVector2<T> dimensions) {
        return IsOnSurfaceOf(this, dimensions);
        }

    #endregion Grid-based functions

    #region Horizontal

    public readonly T AbsoluteSum => T.Abs(X) + T.Abs(Y);
    public readonly T Product => X * Y;
    public readonly T Sum => X + Y;

    public static T HorizontalAbsoluteSum(in IntegralVector2<T> vector) {
        return T.Abs(vector.X) + T.Abs(vector.Y);
        }

    public static T HorizontalProduct(in IntegralVector2<T> vector) {
        return vector.X * vector.Y;
        }

    public static T HorizontalSum(in IntegralVector2<T> vector) {
        return vector.X + vector.Y;
        }

    #endregion Horizontal

    #region Lexicographical Ordering

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool LexicographicallyPrecedes(IntegralVector2<T> left, IntegralVector2<T> right) {
        return left.X < right.X || (left.X == right.X && left.Y < right.Y);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool LexicographicallyPrecedes(IntegralVector2<T> other) {
        return X < other.X || (X == other.X && Y < other.Y);
        }

    #endregion Lexicographical Ordering

    #region Magnitude and Dot

    public readonly T Magnitude {
        get => GenericNumbers<double>.ToBinaryInteger<T>(
                    double.Sqrt(
                        GenericNumbers<double>.FromBinaryInteger(MagnitudeSquared)
                        ));
        }

    public readonly T MagnitudeSquared => X * X + Y * Y;

    public static T Dot(IntegralVector2<T> left, IntegralVector2<T> right) {
        return left.X * right.X + left.Y * right.Y;
        }

    public readonly T Dot(IntegralVector2<T> other) {
        return X * other.X + Y * other.Y;
        }

    #endregion Magnitude and Dot

    #region Max / Min

    public static T HorizontalMax(IntegralVector2<T> vector) {
        return T.Max(vector.X, vector.Y);
        }

    public static T HorizontalMin(IntegralVector2<T> vector) {
        return T.Min(vector.X, vector.Y);
        }

    public static IntegralVector2<T> Max(IntegralVector2<T> left, IntegralVector2<T> right) {
        return new(
            x: T.Max(left.X, right.X),
            y: T.Max(left.Y, right.Y)
            );
        }

    public static IntegralVector2<T> Min(IntegralVector2<T> left, IntegralVector2<T> right) {
        return new(
            x: T.Min(left.X, right.X),
            y: T.Min(left.Y, right.Y)
            );
        }

    public readonly T HorizontalMax() {
        return T.Max(X, Y);
        }

    public readonly T HorizontalMin() {
        return T.Min(X, Y);
        }

    public readonly IntegralVector2<T> Max(IntegralVector2<T> other) {
        return new IntegralVector2<T>(
            x: T.Max(X, other.X),
            y: T.Max(Y, other.Y)
            );
        }

    public readonly IntegralVector2<T> Min(IntegralVector2<T> other) {
        return new IntegralVector2<T>(
            x: T.Min(X, other.X),
            y: T.Min(Y, other.Y)
            );
        }

    #endregion Max / Min

    #region Negation

    /// <inheritdoc/>
    /// <remarks><inheritdoc cref="DocStrings.Disclaimer_Negation_IntegralMayBeUnsigned"/></remarks>
    public static IntegralVector2<T> Negate(IntegralVector2<T> vector) {
        ThrowIfUnsigned<T>();
        return new(T.Zero - vector.X, T.Zero - vector.Y);
        }

    /// <inheritdoc/>
    /// <remarks><inheritdoc cref="DocStrings.Disclaimer_Negation_IntegralMayBeUnsigned"/></remarks>
    public static void Negate(ref IntegralVector2<T> vector) {
        ThrowIfUnsigned<T>();
        vector.X = T.Zero - vector.X;
        vector.Y = T.Zero - vector.Y;
        }

    /// <inheritdoc/>
    /// <remarks><inheritdoc cref="DocStrings.Disclaimer_Negation_IntegralMayBeUnsigned"/></remarks>
    public readonly IntegralVector2<T> Negate() {
        ThrowIfUnsigned<T>();
        return new(T.Zero - X, T.Zero - Y);
        }

    #endregion Negation

    #region Normalization

    public static IntegralVector2<T> Normalize(IntegralVector2<T> vector, T magnitude) {
        return (vector * magnitude) / vector.Magnitude;
        }

    public static void Normalize(ref IntegralVector2<T> vector, T magnitude) {
        vector = (vector * magnitude) / vector.Magnitude;
        }

    public readonly IntegralVector2<T> Normalize(T magnitude) {
        return (this * magnitude) / Magnitude;
        }

    #endregion Normalization

    #region Operators, Equatability & Comparability

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IntegralVector2<T> Add(IntegralVector2<T> left, IntegralVector2<T> right) {
        return left + right;
        }

    public static IntegralVector2<T> ComponentDivide(IntegralVector2<T> left, IntegralVector2<T> right) {
        return new(
            x: left.X / right.X,
            y: left.Y / right.Y
            );
        }

    public static IntegralVector2<T> ComponentMultiply(IntegralVector2<T> left, IntegralVector2<T> right) {
        return new(
            x: left.X * right.X,
            y: left.Y * right.Y
            );
        }

    public static IntegralVector2<T> operator -(IntegralVector2<T> left, IntegralVector2<T> right) {
        return new(
            x: left.X - right.X,
            y: left.Y - right.Y
            );
        }

    public static bool operator !=(IntegralVector2<T> left, IntegralVector2<T> right) {
        return left.Equals(right) == false;
        }

    public static IntegralVector2<T> operator *(IntegralVector2<T> vector, T value) {
        return new(
            x: vector.X * value,
            y: vector.Y * value
            );
        }

    public static IntegralVector2<T> operator *(T value, IntegralVector2<T> vector) {
        return new(
            x: vector.X * value,
            y: vector.Y * value
            );
        }

    public static IntegralVector2<T> operator /(IntegralVector2<T> vector, T value) {
        return new(
            x: vector.X / value,
            y: vector.Y / value
            );
        }

    public static IntegralVector2<T> operator /(T value, IntegralVector2<T> vector) {
        return new(
            x: value / vector.X,
            y: value / vector.Y
            );
        }

    public static IntegralVector2<T> operator +(IntegralVector2<T> left, IntegralVector2<T> right) {
        return new(
            x: left.X + right.X,
            y: left.Y + right.Y
            );
        }

    public static bool operator ==(IntegralVector2<T> left, IntegralVector2<T> right) {
        return left.Equals(right);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IntegralVector2<T> Subtract(IntegralVector2<T> left, IntegralVector2<T> right) {
        return left - right;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly IntegralVector2<T> Add(IntegralVector2<T> other) {
        return this + other;
        }

    public readonly IntegralVector2<T> ComponentDivide(IntegralVector2<T> other) {
        return new(
            x: X / other.X,
            y: Y / other.Y
            );
        }

    public readonly IntegralVector2<T> ComponentMultiply(IntegralVector2<T> other) {
        return new(
            x: X * other.X,
            y: Y * other.Y
            );
        }

    public readonly bool Equals(IntegralVector2<T> other) {
        return
            (X == other.X) &&
            (Y == other.Y);
        }

    public override readonly bool Equals(object? obj) {
        return obj is IntegralVector2<T> vector && Equals(vector);
        }

    public override readonly int GetHashCode() {
        return HashCode.Combine(X, Y);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly IntegralVector2<T> Subtract(IntegralVector2<T> other) {
        return this - other;
        }

    #endregion Operators, Equatability & Comparability

    #region Orthogonality & Orthonormality

    public static IntegralVector2<T> Orthogonal(IntegralVector2<T> vector) {
        return new(-vector.Y, vector.X);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly IntegralVector2<T> Orthogonal() {
        return Orthogonal(this);
        }

    public static bool IsOrthogonalWith(IntegralVector2<T> left, IntegralVector2<T> right) {
        return Dot(left, right) == T.Zero;
        }

    public static bool IsOrthonormalWith(IntegralVector2<T> left, IntegralVector2<T> right) {
        return Dot(left, right) == T.Zero && left.MagnitudeSquared == T.One && right.MagnitudeSquared == T.One;
        }

    public readonly bool IsOrthogonalWith(IntegralVector2<T> other) {
        return Dot(other) == T.Zero;
        }

    public readonly bool IsOrthonormalWith(IntegralVector2<T> other) {
        return Dot(other) == T.Zero && MagnitudeSquared == T.One && other.MagnitudeSquared == T.One;
        }

    #endregion Orthogonality & Orthonormality

    #region Scale

    public static IntegralVector2<T> Scale(IntegralVector2<T> vector, T factor) {
        return vector * factor;
        }

    public static void Scale(ref IntegralVector2<T> vector, T factor) {
        vector.X *= factor;
        vector.Y *= factor;
        }

    public readonly IntegralVector2<T> Scale(T factor) {
        return this * factor;
        }

    #endregion Scale

    #region Sign

    public static IntegralVector2<T> Sign(IntegralVector2<T> vector) {
        return new(
            x: vector.X > T.Zero ? T.One : vector.X < T.Zero ? -T.One : T.Zero,
            y: vector.Y > T.Zero ? T.One : vector.Y < T.Zero ? -T.One : T.Zero
            );
        }

    public readonly IntegralVector2<T> Sign() {
        return new(
            x: X > T.Zero ? T.One : X < T.Zero ? -T.One : T.Zero,
            y: Y > T.Zero ? T.One : Y < T.Zero ? -T.One : T.Zero
            );
        }

    #endregion Sign

    #region Step

    public static IntegralVector2<T> Step(IntegralVector2<T> left, IntegralVector2<T> right) {
        return left + Sign(right - left);
        }

    public readonly IntegralVector2<T> Step(IntegralVector2<T> other) {
        return this + Sign(other - this);
        }

    #endregion Step

    #region Strings

    public readonly string ToString(byte digits, int integerLength, int padToLength) {
        return $"[{X.Stringify(digits, integerLength, padToLength)}, {Y.Stringify(digits, integerLength, padToLength)}]";
        }

    public override readonly string ToString() {
        return $"[{X}, {Y}]";
        }

    #endregion Strings

    #region Swizzling

    public readonly IntegralVector2<T> YX => new(Y, X);
    public readonly IntegralVector2<T> XY => new(X, Y);
    public readonly IntegralVector2<T> XX => new(X, X);
    public readonly IntegralVector2<T> YY => new(Y, Y);

    #endregion Swizzling
    }
