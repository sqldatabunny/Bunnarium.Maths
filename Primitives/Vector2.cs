using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Bunnarium.Maths.Primitives;

/// <summary> A container for two floating-point values, X and Y. Useful for a variety of purposes, a vector may represent a coordinate, a two-part value, an offset, or a classical vector with a direction and a <see cref="Magnitude">magnitude</see>.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
[DebuggerDisplay("{ToString(), nq}")]
public struct Vector2<T>
    : IFloatingPointVector<Vector2<T>, T>
    , IVectorOfHigherDimension<Vector2<T>, Vector3<T>, T>
    where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {

    /// <inheritdoc
    /// cref="DocStrings.VectorComponentX"/>
    public T X { readonly get; set; }

    /// <inheritdoc
    /// cref="DocStrings.VectorComponentY"/>
    public T Y { readonly get; set; }

    #region Constructors & Factories

    /// <inheritdoc
    /// cref="IVector{TVector, T}.Create(T)"/>
    public Vector2(T xy) {
        X = xy;
        Y = xy;
        }

    public Vector2(T x, T y) {
        X = x;
        Y = y;
        }

    /// <summary> Creates a <see cref="Vector2{T}"/> with the direction of the input <paramref name="angle"/> and <paramref name="length"/>.
    /// </summary>
    public Vector2(Angle<T> angle, T length) {
        X = angle.Cos * length;
        Y = angle.Sin * length;
        }

    /// <summary> Creates a <see cref="Vector2{T}"/> with the direction of the input <paramref name="angle"/>.
    /// </summary>
    public Vector2(Angle<T> angle) {
        X = angle.Cos;
        Y = angle.Sin;
        }

    public static Vector2<T> Create(T value) {
        return new(value, value);
        }

    #endregion Constructors & Factories

    #region Constants

    public static Vector2<T> Down { get; } = new(T.Zero, -T.One);
    public static Vector2<T> Forward { get; } = new(T.One, T.Zero);
    public static Vector2<T> Left { get; } = new(-T.One, T.Zero);
    public static Vector2<T> MaxValue { get; } = new(T.MaxValue, T.MaxValue);
    public static Vector2<T> MinValue { get; } = new(T.MinValue, T.MinValue);

    public static Vector2<T> One { get; } = new(T.One);
    public static Vector2<T> Right { get; } = new(T.One, T.Zero);
    public static Vector2<T> RootN { get; } = new(GenericNumbers<T>.OneOverRootTwo);
    public static Vector2<T> Up { get; } = new(T.Zero, T.One);
    public static Vector2<T> Zero { get; } = new(T.Zero);

    #endregion Constants

    #region Absolute Values

    public static Vector2<T> Abs(in Vector2<T> vector) {
        return new(T.Abs(vector.X), T.Abs(vector.Y));
        }

    public readonly Vector2<T> Abs() {
        return new(T.Abs(X), T.Abs(Y));
        }

    #endregion Absolute Values

    #region Angles & Rotations

    /// <returns> The angle formed by two vectors with respect to the origin.
    /// </returns>
    public static Angle<T> AngleBetween(Vector2<T> reference, Vector2<T> other) {
        var ratio = reference.Dot(other) / (reference.Magnitude * other.Magnitude);
        return Angle<T>.FromRadiansUnchecked(T.Acos(T.Clamp(ratio, T.NegativeOne, T.One)));
        }

    /// <returns> The <see cref="WindingOrder"/> of the three input vectors in 2D space.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WindingOrder GetWindingOrder(Vector2<T> p, Vector2<T> q, Vector2<T> r) {
        var val = (q.Y - p.Y) * (r.X - q.X) - (q.X - p.X) * (r.Y - q.Y);
        return val > T.Zero ? WindingOrder.Clockwise : val < T.Zero ? WindingOrder.Counterclockwise : WindingOrder.Colinear;
        }

    /// <summary> Rotates the input <paramref name="vector"/> around the origin by the given <paramref name="angle"/>.
    /// </summary>
    public static void Rotate(ref Vector2<T> vector, Angle<T> angle) {
        var c = angle.Cos;
        var s = angle.Sin;
        var x = vector.X * c + vector.Y * -s;
        vector.Y = vector.X * s + vector.Y * c;
        vector.X = x;
        }

    /// <inheritdoc cref="Rotate(ref Vector2{T}, Angle{T})"/>
    /// <returns> The rotated vector.
    /// </returns>
    public static Vector2<T> Rotate(Vector2<T> vector, Angle<T> angle) {
        var c = angle.Cos;
        var s = angle.Sin;
        return new(x: vector.X * c + vector.Y * -s, y: vector.X * s + vector.Y * c);
        }

    /// <inheritdoc
    /// cref="AngleBetween(Vector2{T}, Vector2{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Angle<T> AngleTo(Vector2<T> other) {
        return AngleBetween(this, other);
        }

    /// <inheritdoc
    /// cref="GetWindingOrder(Vector2{T}, Vector2{T}, Vector2{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly WindingOrder GetWindingOrderWith(Vector2<T> q, Vector2<T> r) {
        var val = (q.Y - Y) * (r.X - q.X) - (q.X - X) * (r.Y - q.Y);
        return val > T.Zero ? WindingOrder.Clockwise : val < T.Zero ? WindingOrder.Counterclockwise : WindingOrder.Colinear;
        }
    /// <summary> Rotates this vector around the origin by the given <paramref name="angle"/>.
    /// </summary>
    /// <returns> The rotated vector.
    /// </returns>
    public readonly Vector2<T> Rotate(Angle<T> angle) {
        var c = angle.Cos;
        var s = angle.Sin;
        return new(X * c + Y * -s, X * s + Y * c);
        }
    #endregion Angles & Rotations

    #region Complex

    /// <summary> A <see cref="Complex{T}"/> such that the <see cref="Complex{T}.Real">real</see> part is set to <see cref="X">X</see> and the <see cref="Complex{T}.Imaginary">imaginary</see> part is set to <see cref="Y">Y</see>.
    /// </summary>
    public readonly Complex<T> AsComplex {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => new(X, Y);
        }

    /// <returns> A <see cref="Vector2{T}"/> such that <see cref="X">X</see> is set to <see cref="Complex{T}.Real"><paramref name="complex"/>.Real</see> and <see cref="Y">Y</see> is set to  <see cref="Complex{T}.Imaginary"><paramref name="complex"/>.Imaginary</see>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2<T> FromComplex(Complex<T> complex) {
        return new(complex.Real, complex.Imaginary);
        }

    /// <returns> A <see cref="Complex{T}"/> such that the <see cref="Complex{T}.Real">real</see> part is set to <see cref="X"><paramref name="vector"/>.X</see> and the <see cref="Complex{T}.Imaginary">imaginary</see> part is set to <see cref="Y"><paramref name="vector"/>.Y</see>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Complex<T> ToComplex(Vector2<T> vector) {
        return new(vector.X, vector.Y);
        }

    #endregion Complex

    #region Conversions

    public static Vector3<T> Append(in Vector2<T> vector, T value) {
        return new(vector.X, vector.Y, value);
        }

    public static Span<T> ToSpan(ref Vector2<T> vector) {
        return MemoryMarshal.CreateSpan(ref Unsafe.As<Vector2<T>, T>(ref vector), 2);
        }

    public readonly Vector3<T> Append(T value) {
        return Append(in this, value);
        }
    /// <returns> A <see cref="Vector3{T}"/> populated in-part by this vector's values.
    /// </returns>
    /// <param name="z">The value of the output vector's third component.</param>
    public readonly Vector3<T> ToVector3(T z) {
        return new(X, Y, z);
        }

    /// <returns> A <see cref="Vector4{T}"/> populated in-part by this vector's values.
    /// </returns>
    /// <param name="z">The value of the output vector's third component.</param>
    /// <param name="w">The value of the output vector's fourth component.</param>
    public readonly Vector4<T> ToVector4(T z, T w) {
        return new(X, Y, z, w);
        }

    #endregion Conversions

    #region Cross, Outer, and Wedge Products

    /// <returns> The wedge product—equivalent in 2D space to <b><i>scalar cross product</i></b>—representing the signed area of the parallelogram formed by the two vectors.
    /// </returns>
    public static T CrossProduct(Vector2<T> a, Vector2<T> b) {
        return a.X * b.Y - a.Y * b.X;
        }

    /// <returns> A matrix representing the outer product—also known as the <b><i>tensor product</i></b>—of the two input vectors.
    /// </returns>
    public static Matrix2<T> OuterProduct(Vector2<T> a, Vector2<T> b) {
        return new(
            a1: a.X * b.X, a2: a.X * b.Y,
            b1: a.Y * b.X, b2: a.Y * b.Y
            );
        }

    /// <inheritdoc
    /// cref="CrossProduct(Vector2{T}, Vector2{T})"/>
    public static T WedgeProduct(Vector2<T> a, Vector2<T> b) {
        return CrossProduct(a, b);
        }

    /// <inheritdoc
    /// cref="CrossProduct(Vector2{T}, Vector2{T})"/>
    public readonly T CrossProduct(Vector2<T> other) {
        return X * other.Y - Y * other.X;
        }

    /// <inheritdoc
    /// cref="OuterProduct(Vector2{T}, Vector2{T})"/>
    public readonly Matrix2<T> OuterProduct(Vector2<T> other) {
        return new(
            a1: X * other.X, a2: X * other.Y,
            b1: Y * other.X, b2: Y * other.Y
            );
        }

    /// <inheritdoc
    /// cref="CrossProduct(Vector2{T}, Vector2{T})"/>
    public readonly T WedgeProduct(Vector2<T> other) {
        return CrossProduct(other);
        }

    #endregion Cross, Outer, and Wedge Products

    #region Dot

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Dot(Vector2<T> a, Vector2<T> b) {
        return a.X * b.X + a.Y * b.Y;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly T Dot(Vector2<T> other) {
        return X * other.X + Y * other.Y;
        }

    #endregion Dot

    #region Horizontal

    public readonly T AbsoluteAverage => (T.Abs(X) + T.Abs(Y)) / GenericNumbers<T>.Two;
    public readonly T AbsoluteSum => T.Abs(X) + T.Abs(Y);
    public readonly T Average => (X + Y) / GenericNumbers<T>.Two;
    public readonly T Product => X * Y;

    public readonly T Sum => X + Y;

    public static T HorizontalAbsoluteAverage(in Vector2<T> vector) {
        return (T.Abs(vector.X) + T.Abs(vector.Y)) / GenericNumbers<T>.Two;
        }

    public static T HorizontalAbsoluteSum(in Vector2<T> vector) {
        return T.Abs(vector.X) + T.Abs(vector.Y);
        }

    public static T HorizontalAverage(in Vector2<T> vector) {
        return (vector.X + vector.Y) / GenericNumbers<T>.Two;
        }

    public static T HorizontalProduct(in Vector2<T> vector) {
        return vector.X * vector.Y;
        }

    public static T HorizontalSum(in Vector2<T> vector) {
        return vector.X + vector.Y;
        }

    #endregion Horizontal

    #region Floor & Ceiling

    public static void Ceiling(ref Vector2<T> vector) {
        vector.X = T.Ceiling(vector.X);
        vector.Y = T.Ceiling(vector.Y);
        }

    public static Vector2<T> Ceiling(Vector2<T> vector) {
        return new(T.Ceiling(vector.X), T.Ceiling(vector.Y));
        }

    public static void Floor(ref Vector2<T> vector) {
        vector.X = T.Floor(vector.X);
        vector.Y = T.Floor(vector.Y);
        }

    public static Vector2<T> Floor(Vector2<T> vector) {
        return new(T.Floor(vector.X), T.Floor(vector.Y));
        }

    public readonly Vector2<T> Ceiling() {
        return new(T.Ceiling(X), T.Ceiling(Y));
        }

    public readonly Vector2<T> Floor() {
        return new(T.Floor(X), T.Floor(Y));
        }

    #endregion Floor & Ceiling

    #region Lerp

    public static Vector2<T> Lerp(Vector2<T> from, Vector2<T> to, T amount) {
        return from * (T.One - amount) + (to * amount);
        }

    public readonly Vector2<T> Lerp(Vector2<T> to, T amount) {
        return this * (T.One - amount) + (to * amount);
        }

    #endregion Lerp

    #region Lexicographical Ordering

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool LexicographicallyPrecedes(Vector2<T> left, Vector2<T> right) {
        return left.X < right.X || (left.X == right.X && left.Y < right.Y);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool LexicographicallyPrecedes(Vector2<T> other) {
        return X < other.X || (X == other.X && Y < other.Y);
        }

    #endregion Lexicographical Ordering

    #region Magnitude

    public readonly T Magnitude => T.Sqrt(MagnitudeSquared);

    public readonly T MagnitudeSquared => X * X + Y * Y;

    #endregion Magnitude

    #region Min / Max

    public static T HorizontalMax(Vector2<T> vector) {
        return T.Max(vector.X, vector.Y);
        }

    public static T HorizontalMin(Vector2<T> vector) {
        return T.Min(vector.X, vector.Y);
        }

    public static Vector2<T> Max(Vector2<T> left, Vector2<T> right) {
        return new Vector2<T>(T.Max(left.X, right.X), T.Max(left.Y, right.Y));
        }

    public static Vector2<T> Min(Vector2<T> left, Vector2<T> right) {
        return new Vector2<T>(T.Min(left.X, right.X), T.Min(left.Y, right.Y));
        }

    public readonly T HorizontalMax() {
        return T.Max(X, Y);
        }

    public readonly T HorizontalMin() {
        return T.Min(X, Y);
        }

    public readonly Vector2<T> Max(Vector2<T> other) {
        return new Vector2<T>(T.Max(X, other.X), T.Max(Y, other.Y));
        }

    public readonly Vector2<T> Min(Vector2<T> other) {
        return new Vector2<T>(T.Min(X, other.X), T.Min(Y, other.Y));
        }

    #endregion Min / Max

    #region Orthogonality & Orthonormality

    public static bool IsOrthogonalWith(Vector2<T> left, Vector2<T> right) {
        return left.Dot(right) == T.Zero;
        }

    public static bool IsOrthonormalWith(Vector2<T> left, Vector2<T> right) {
        return left.Dot(right) == T.Zero && left.MagnitudeSquared == T.One && right.MagnitudeSquared == T.One;
        }

    public static Vector2<T> Orthogonal(Vector2<T> vector) {
        return new(-vector.Y, vector.X);
        }

    public readonly bool IsOrthogonalWith(Vector2<T> other) {
        return Dot(other) == T.Zero;
        }

    public readonly bool IsOrthonormalWith(Vector2<T> other) {
        return Dot(other) == T.Zero && MagnitudeSquared == T.One && other.MagnitudeSquared == T.One;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Vector2<T> Orthogonal() {
        return Orthogonal(this);
        }
    #endregion Orthogonality & Orthonormality

    #region Normalization

    public readonly bool IsNormalized {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => MagnitudeSquared == T.One;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2<T> Normalize(Vector2<T> vector) {
        return Normalize(vector, T.One);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Normalize(ref Vector2<T> vector) {
        Normalize(ref vector, T.One);
        }

    public static void Normalize(ref Vector2<T> vector, T magnitude) {
        var mag = T.Sqrt(vector.X * vector.X + vector.Y * vector.Y); // inline
        var adjust = magnitude / mag;
        vector.X *= adjust;
        vector.Y *= adjust;
        }

    public static Vector2<T> Normalize(Vector2<T> vector, T magnitude) {
        var mag = T.Sqrt(vector.X * vector.X + vector.Y * vector.Y); // inline
        var adjust = magnitude / mag;
        return new(vector.X * adjust, vector.Y * adjust);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Vector2<T> Normalize() {
        return Normalize(T.One);
        }

    public readonly Vector2<T> Normalize(T magnitude) {
        var mag = T.Sqrt(X * X + Y * Y); // inline
        var adjust = magnitude / mag;
        return new(X * adjust, Y * adjust);
        }

    #endregion Normalization

    #region Negation

    public static Vector2<T> Negate(Vector2<T> vector) {
        return new(-vector.X, -vector.Y);
        }

    public static void Negate(ref Vector2<T> vector) {
        vector.X = -vector.X;
        vector.Y = -vector.Y;
        }

    public readonly Vector2<T> Negate() {
        return new(-X, -Y);
        }

    #endregion Negation

    #region Operators, Equatability & Comparability

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2<T> Add(Vector2<T> left, Vector2<T> right) {
        return left + right;
        }

    public static Vector2<T> ComponentDivide(Vector2<T> left, Vector2<T> right) {
        return new(left.X / right.X, left.Y / right.Y);
        }

    public static Vector2<T> ComponentMultiply(Vector2<T> left, Vector2<T> right) {
        return new(left.X * right.X, left.Y * right.Y);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Vector2<T>((T X, T Y) tuple) {
        return new(tuple.X, tuple.Y);
        }

    public static Vector2<T> operator -(Vector2<T> a, Vector2<T> b) {
        return new(a.X - b.X, a.Y - b.Y);
        }

    public static Vector2<T> operator -(Vector2<T> vector, T value) {
        return new(vector.X - value, vector.Y - value);
        }

    public static Vector2<T> operator -(Vector2<T> a) {
        return new(-a.X, -a.Y);
        }

    public static bool operator !=(Vector2<T> a, Vector2<T> b) {
        return !a.Equals(b);
        }

    public static Vector2<T> operator *(Vector2<T> vector, T value) {
        return new(vector.X * value, vector.Y * value);
        }

    public static Vector2<T> operator *(T value, Vector2<T> vector) {
        return new(vector.X * value, vector.Y * value);
        }

    public static Vector2<T> operator /(Vector2<T> vector, T value) {
        return new(vector.X / value, vector.Y / value);
        }

    public static Vector2<T> operator /(T value, Vector2<T> vector) {
        return new(value / vector.X, value / vector.Y);
        }

    public static Vector2<T> operator +(Vector2<T> a, Vector2<T> b) {
        return new(a.X + b.X, a.Y + b.Y);
        }

    public static Vector2<T> operator +(Vector2<T> vector, T val) {
        return new(vector.X + val, vector.Y + val);
        }

    public static bool operator <(in Vector2<T> left, in Vector2<T> right) {
        return left.X < right.X
            && left.Y < right.Y;
        }

    public static bool operator <=(in Vector2<T> left, in Vector2<T> right) {
        return left.X <= right.X
            && left.Y <= right.Y;
        }

    public static bool operator ==(Vector2<T> a, Vector2<T> b) {
        return a.Equals(b);
        }

    public static bool operator >(in Vector2<T> left, in Vector2<T> right) {
        return left.X > right.X
            && left.Y > right.Y;
        }

    public static bool operator >=(in Vector2<T> left, in Vector2<T> right) {
        return left.X >= right.X
            && left.Y >= right.Y;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2<T> Subtract(Vector2<T> left, Vector2<T> right) {
        return left - right;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Vector2<T> Add(Vector2<T> other) {
        return this + other;
        }

    public readonly Vector2<T> ComponentDivide(Vector2<T> other) {
        return new(X / other.X, Y / other.Y);
        }

    public readonly Vector2<T> ComponentMultiply(Vector2<T> other) {
        return new(X * other.X, Y * other.Y);
        }

    public readonly void Deconstruct(out T X, out T Y) {
        X = this.X; Y = this.Y;
        }
    public readonly bool Equals(Vector2<T> other) {
        return X == other.X && Y == other.Y;
        }

    public override readonly bool Equals(object? obj) {
        return obj is Vector2<T> vector && Equals(vector);
        }

    public override readonly int GetHashCode() {
        unchecked {
            return HashCode.Combine(X, Y);
            }
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Vector2<T> Subtract(Vector2<T> other) {
        return this - other;
        }

    #endregion Operators, Equatability & Comparability

    #region Rounding

    public static void Round(ref Vector2<T> vector) {
        vector.X = T.Round(vector.X);
        vector.Y = T.Round(vector.Y);
        }

    public static void Round(ref Vector2<T> vector, byte digits) {
        vector.X = T.Round(vector.X, digits);
        vector.Y = T.Round(vector.Y, digits);
        }

    public static Vector2<T> Round(Vector2<T> vector) {
        return new(T.Round(vector.X), T.Round(vector.Y));
        }

    public static Vector2<T> Round(Vector2<T> vector, byte digits) {
        return new(T.Round(vector.X, digits), T.Round(vector.Y, digits));
        }

    public readonly Vector2<T> Round() {
        return new(T.Round(X), T.Round(Y));
        }
    public readonly Vector2<T> Round(byte digits) {
        return new(T.Round(X, digits), T.Round(Y, digits));
        }

    #endregion Rounding

    #region Scale

    public static Vector2<T> Scale(Vector2<T> vector, T factor) {
        return vector * factor;
        }

    public static void Scale(ref Vector2<T> vector, T factor) {
        vector.X *= factor;
        vector.Y *= factor;
        }

    public readonly Vector2<T> Scale(T factor) {
        return this * factor;
        }

    #endregion Scale

    #region Sign

    public static Vector2<T> Sign(Vector2<T> vector) {
        return new(vector.X > T.Zero ? T.One : vector.X < T.Zero ? T.NegativeOne : T.Zero, vector.Y > T.Zero ? T.One : vector.Y < T.Zero ? T.NegativeOne : T.Zero);
        }

    public readonly Vector2<T> Sign() {
        return new(X > T.Zero ? T.One : X < T.Zero ? T.NegativeOne : T.Zero, Y > T.Zero ? T.One : Y < T.Zero ? T.NegativeOne : T.Zero);
        }

    #endregion Sign

    #region SizeOf and Intrinsics

    public static int Length { get; } = 2;

    public static unsafe int SizeOf { get; } = sizeof(T) * Length;

    #endregion SizeOf and Intrinsics

    #region Step

    public static Vector2<T> Step(Vector2<T> left, Vector2<T> right) {
        return left + Sign(right - left);
        }

    public readonly Vector2<T> Step(Vector2<T> other) {
        return this + Sign(other - this);
        }

    #endregion Step

    #region Strings

    public override readonly string ToString() {
        return $"[{X}, {Y}]";
        }

    public readonly string ToString(byte digits, int integerLength, int paddingLength) {
        return $"[{X.Stringify(digits, integerLength, paddingLength)}, {Y.Stringify(digits, integerLength, paddingLength)}]";
        }

    #endregion Strings

    #region Swizzling

    public readonly Vector2<T> XX => new(X, X);
    public readonly Vector2<T> XY => new(X, Y);
    public readonly Vector2<T> YX => new(Y, X);
    public readonly Vector2<T> YY => new(Y, Y);

    #endregion Swizzling
    }
