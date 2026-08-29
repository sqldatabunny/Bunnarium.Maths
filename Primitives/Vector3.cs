using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Bunnarium.Maths.Primitives;

/// <summary> A container for three floating-point values, X, Y, and Z. Useful for a variety of purposes, a vector may represent a coordinate, a three-part value, an offset, or a classical vector with a direction and a <see cref="Magnitude">magnitude</see>.
/// </summary>
[DebuggerDisplay("{ToString(), nq}")]
[StructLayout(LayoutKind.Sequential)]
public struct Vector3<T>
    : IFloatingPointVector<Vector3<T>, T>
    , IVectorOfHigherDimension<Vector3<T>, Vector4<T>, T>
    where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {

    /// <inheritdoc
    /// cref="DocStrings.VectorComponentX"/>
    public T X { readonly get; set; }

    /// <inheritdoc
    /// cref="DocStrings.VectorComponentY"/>
    public T Y { readonly get; set; }

    /// <inheritdoc
    /// cref="DocStrings.VectorComponentZ"/>
    public T Z { readonly get; set; }

    #region Constructors & Factories

    public Vector3(T x, T y, T z) {
        X = x;
        Y = y;
        Z = z;
        }

    /// <param name="xy">The vector's X and Y components.</param>
    /// <param name="z">The vector's Z component.</param>
    public Vector3(Vector2<T> xy, T z) {
        X = xy.X;
        Y = xy.Y;
        Z = z;
        }

    /// <inheritdoc
    /// cref="IVector{TVector, T}.Create(T)"/>
    public Vector3(T xyz) {
        X = Y = Z = xyz;
        }

    /// <summary> Creates a <see cref="Vector3{T}"/> with the direction of the input <paramref name="direction"/> and <paramref name="length"/>.
    /// </summary>
    public Vector3(Direction<T> direction, T length) {
        var vec = direction.Vector * length;
        X = vec.X;
        Y = vec.Y;
        Z = vec.Z;
        }

    /// <summary> Creates a <see cref="Vector3{T}"/> with the direction of the input <paramref name="direction"/>.
    /// </summary>
    public Vector3(Direction<T> direction) {
        var vec = direction.Vector;
        X = vec.X;
        Y = vec.Y;
        Z = vec.Z;
        }

    public static Vector3<T> Create(T value) {
        return new(value, value, value);
        }

    #endregion Constructors & Factories

    #region Constants

    public static Vector3<T> Down { get; } = new(T.Zero, T.NegativeOne, T.Zero);

    public static Vector3<T> Left { get; } = new(T.NegativeOne, T.Zero, T.Zero);
    public static Vector3<T> MaxValue { get; } = new(T.MaxValue);
    public static Vector3<T> MinValue { get; } = new(T.MinValue);
    public static Vector3<T> One { get; } = new(T.One);
    public static Vector3<T> Right { get; } = new(T.One, T.Zero, T.Zero);
    public static Vector3<T> RootN { get; } = new(GenericNumbers<T>.OneOverRootThree);
    public static Vector3<T> Up { get; } = new(T.Zero, T.One, T.Zero);
    public static Vector3<T> Zero { get; } = new(T.Zero);
#if MATRIX_LEFTHANDED_COORDINATE_SYSTEM
    public static Vector3<T> Forward { get; } = new(T.Zero, T.Zero, T.One);
#elif MATRIX_RIGHTHANDED_COORDINATE_SYSTEM
    public static Vector3<T> Forward { get; } = new(T.Zero, T.Zero, T.NegativeOne);
#else
    public static Vector3<T> Forward => throw new ApplicationException(Matrix.Docs.BaseMessage);
#endif

#if MATRIX_LEFTHANDED_COORDINATE_SYSTEM
    public static Vector3<T> Backward { get; } = new(T.Zero, T.Zero, T.NegativeOne);
#elif MATRIX_RIGHTHANDED_COORDINATE_SYSTEM
    public static Vector3<T> Backward { get; } = new(T.Zero, T.Zero, T.One);
#else
    public static Vector3<T> Backward => throw new ApplicationException(Matrix.Docs.BaseMessage);
#endif

    #endregion Constants

    #region Absolute Values

    public static Vector3<T> Abs(in Vector3<T> vector) {
        return new(T.Abs(vector.X), T.Abs(vector.Y), T.Abs(vector.Z));
        }

    public readonly Vector3<T> Abs() {
        return new(T.Abs(X), T.Abs(Y), T.Abs(Z));
        }

    #endregion Absolute Values

    #region Angles & Rotations

    /// <inheritdoc
    /// cref="Vector2{T}.AngleBetween(Vector2{T}, Vector2{T})"/>
    public static Angle<T> AngleBetween(Vector3<T> reference, Vector3<T> other) {
        var ratio = reference.Dot(other) / (reference.Magnitude * other.Magnitude);
        return Angle<T>.FromRadiansUnchecked(T.Acos(T.Clamp(ratio, T.NegativeOne, T.One)));
        }

    /// <inheritdoc cref="Rotate(ref Vector3{T}, Quaternion{T})"/>
    /// <returns> The rotated vector.
    /// </returns>
    public static Vector3<T> Rotate(Vector3<T> vector, Quaternion<T> quaternion) {
        return quaternion.Rotate(vector);
        }

    /// <summary> Rotates the input <paramref name="vector"/> around the origin by the given <paramref name="quaternion"/>.
    /// </summary>
    public static void Rotate(ref Vector3<T> vector, Quaternion<T> quaternion) {
        Quaternion<T>.Rotate(ref vector, quaternion);
        }

    /// <inheritdoc
    /// cref="Vector2{T}.AngleTo(Vector2{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Angle<T> AngleTo(Vector3<T> other) {
        return AngleBetween(this, other);
        }

    /// <summary> Rotates this vector around the origin by the given <paramref name="quaternion"/>.
    /// </summary>
    /// <returns> The rotated vector.
    /// </returns>
    public readonly Vector3<T> Rotate(Quaternion<T> quaternion) {
        return quaternion.Rotate(this);
        }

    #endregion Angles & Rotations

    #region Conversions

    /// <returns> A <see cref="Vector2{T}"/> populated by this vector's X and Y values.
    /// </returns>
    public readonly Vector2<T> ToVector2 => new(X, Y);

    public static Vector4<T> Append(in Vector3<T> vector, T value) {
        return new(vector, value);
        }

    public static Span<T> ToSpan(ref Vector3<T> vector) {
        return MemoryMarshal.CreateSpan(ref Unsafe.As<Vector3<T>, T>(ref vector), 3);
        }

    public readonly Vector4<T> Append(T value) {
        return Append(in this, value);
        }

    /// <returns> A <see cref="Vector4{T}"/> populated in-part by this vector's values.
    /// </returns>
    /// <param name="w">The value of the output vector's fourth component.</param>
    public readonly Vector4<T> ToVector4(T w) {
        return new(X, Y, Z, w);
        }

    #endregion Conversions

    #region Cross

    public static Vector3<T> Cross(Vector3<T> a, Vector3<T> b) {
        return new(a.Y * b.Z - b.Y * a.Z, a.Z * b.X - b.Z * a.X, a.X * b.Y - b.X * a.Y);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Vector3<T> Cross(Vector3<T> other) {
        return Cross(this, other);
        }

    #endregion Cross

    #region Dot

    public static T Dot(Vector3<T> a, Vector3<T> b) {
        return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
        }

    public readonly T Dot(Vector3<T> other) {
        return X * other.X + Y * other.Y + Z * other.Z;
        }

    #endregion Dot

    #region Floor & Ceiling

    public static void Ceiling(ref Vector3<T> vector) {
        vector.X = T.Ceiling(vector.X);
        vector.Y = T.Ceiling(vector.Y);
        vector.Z = T.Ceiling(vector.Z);
        }

    public static Vector3<T> Ceiling(Vector3<T> vector) {
        return new(T.Ceiling(vector.X), T.Ceiling(vector.Y), T.Ceiling(vector.Z));
        }

    public static void Floor(ref Vector3<T> vector) {
        vector.X = T.Floor(vector.X);
        vector.Y = T.Floor(vector.Y);
        vector.Z = T.Floor(vector.Z);
        }

    public static Vector3<T> Floor(Vector3<T> vector) {
        return new(T.Floor(vector.X), T.Floor(vector.Y), T.Floor(vector.Z));
        }

    public readonly Vector3<T> Ceiling() {
        return new(T.Ceiling(X), T.Ceiling(Y), T.Ceiling(Z));
        }

    public readonly Vector3<T> Floor() {
        return new(T.Floor(X), T.Floor(Y), T.Floor(Z));
        }

    #endregion Floor & Ceiling

    #region Horizontal

    public readonly T AbsoluteAverage => (T.Abs(X) + T.Abs(Y) + T.Abs(Z)) / GenericNumbers<T>.Three;
    public readonly T AbsoluteSum => T.Abs(X) + T.Abs(Y) + T.Abs(Z);
    public readonly T Average => (X + Y + Z) / GenericNumbers<T>.Three;
    public readonly T Product => X * Y * Z;
    public readonly T Sum => X + Y + Z;

    public static T HorizontalAbsoluteAverage(in Vector3<T> vector) {
        return (T.Abs(vector.X) + T.Abs(vector.Y) + T.Abs(vector.Z)) / GenericNumbers<T>.Three;
        }

    public static T HorizontalAbsoluteSum(in Vector3<T> vector) {
        return T.Abs(vector.X) + T.Abs(vector.Y) + T.Abs(vector.Z);
        }

    public static T HorizontalAverage(in Vector3<T> vector) {
        return (vector.X + vector.Y + vector.Z) / GenericNumbers<T>.Three;
        }

    public static T HorizontalProduct(in Vector3<T> vector) {
        return vector.X * vector.Y * vector.Z;
        }

    public static T HorizontalSum(in Vector3<T> vector) {
        return vector.X + vector.Y + vector.Z;
        }

    #endregion Horizontal

    #region Lerp

    public static Vector3<T> Lerp(Vector3<T> from, Vector3<T> to, T amount) {
        return from * (T.One - amount) + (to * amount);
        }

    public readonly Vector3<T> Lerp(Vector3<T> to, T amount) {
        return this * (T.One - amount) + (to * amount);
        }

    #endregion Lerp

    #region Lexicographical Ordering

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool LexicographicallyPrecedes(Vector3<T> left, Vector3<T> right) {
        return (left.X < right.X)
            || (left.X == right.X
                && (left.Y < right.Y
                || (left.Y == right.Y && left.Z < right.Z)));
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool LexicographicallyPrecedes(Vector3<T> other) {
        return (X < other.X)
            || (X == other.X
                && (Y < other.Y
                || (Y == other.Y && Z < other.Z)));
        }

    #endregion Lexicographical Ordering

    #region Magnitude

    public readonly T Magnitude => T.Sqrt(MagnitudeSquared);
    public readonly T MagnitudeSquared => X * X + Y * Y + Z * Z;

    #endregion Magnitude

    #region Min / Max

    public static T HorizontalMax(Vector3<T> vector) {
        return T.Max(T.Max(vector.X, vector.Y), vector.Z);
        }

    public static T HorizontalMin(Vector3<T> vector) {
        return T.Min(T.Min(vector.X, vector.Y), vector.Z);
        }

    public static Vector3<T> Max(Vector3<T> left, Vector3<T> right) {
        return new(T.Max(left.X, right.X), T.Max(left.Y, right.Y), T.Max(left.Z, right.Z));
        }

    public static Vector3<T> Min(Vector3<T> left, Vector3<T> right) {
        return new(T.Min(left.X, right.X), T.Min(left.Y, right.Y), T.Min(left.Z, right.Z));
        }

    public readonly T HorizontalMax() {
        return T.Max(T.Max(X, Y), Z);
        }

    public readonly T HorizontalMin() {
        return T.Min(T.Min(X, Y), Z);
        }

    public readonly Vector3<T> Max(Vector3<T> other) {
        return new Vector3<T>(
            x: T.Max(X, other.X),
            y: T.Max(Y, other.Y),
            z: T.Max(Z, other.Z)
            );
        }

    public readonly Vector3<T> Min(Vector3<T> other) {
        return new Vector3<T>(
            x: T.Min(X, other.X),
            y: T.Min(Y, other.Y),
            z: T.Min(Z, other.Z)
            );
        }

    #endregion Min / Max

    #region Negation

    public static Vector3<T> Negate(Vector3<T> vector) {
        return new(-vector.X, -vector.Y, -vector.Z);
        }

    public static void Negate(ref Vector3<T> vector) {
        vector.X = -vector.X;
        vector.Y = -vector.Y;
        vector.Z = -vector.Z;
        }

    public readonly Vector3<T> Negate() {
        return new(-X, -Y, -Z);
        }

    #endregion Negation

    #region Normalization

    public readonly bool IsNormalized {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => MagnitudeSquared == T.One;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Normalize(ref Vector3<T> vector) {
        Normalize(ref vector, T.One);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3<T> Normalize(Vector3<T> vector) {
        return Normalize(vector, T.One);
        }

    public static void Normalize(ref Vector3<T> vector, T magnitude) {
        var x = vector.X;
        var y = vector.Y;
        var z = vector.Z;
        var mag = T.Sqrt(x * x + y * y + z * z);
        var adjust = magnitude / mag;
        vector.X *= adjust;
        vector.Y *= adjust;
        vector.Z *= adjust;
        }

    public static Vector3<T> Normalize(Vector3<T> vector, T magnitude) {
        var mag = T.Sqrt(vector.X * vector.X + vector.Y * vector.Y + vector.Z * vector.Z);
        var adjust = magnitude / mag;
        return new(vector.X * adjust, vector.Y * adjust, vector.Z * adjust);
        }

    public readonly Vector3<T> Normalize(T magnitude) {
        var mag = T.Sqrt(X * X + Y * Y + Z * Z);
        var adjust = magnitude / mag;
        return new(X * adjust, Y * adjust, Z * adjust);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Vector3<T> Normalize() {
        return Normalize(T.One);
        }

    #endregion Normalization

    #region Operators, Equatability & Comparability

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3<T> Add(Vector3<T> left, Vector3<T> right) {
        return left + right;
        }

    public static Vector3<T> ComponentDivide(Vector3<T> a, Vector3<T> b) {
        return new(a.X / b.X, a.Y / b.Y, a.Z / b.Z);
        }

    public static Vector3<T> ComponentMultiply(Vector3<T> a, Vector3<T> b) {
        return new(a.X * b.X, a.Y * b.Y, a.Z * b.Z);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Vector3<T>((T X, T Y, T Z) tuple) {
        return new(tuple.X, tuple.Y, tuple.Z);
        }

    public static Vector3<T> operator -(Vector3<T> a) {
        return new(-a.X, -a.Y, -a.Z);
        }

    public static Vector3<T> operator -(Vector3<T> a, Vector3<T> b) {
        return new(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }

    public static Vector3<T> operator -(Vector3<T> vector, T value) {
        return new(vector.X - value, vector.Y - value, vector.Z - value);
        }

    public static bool operator !=(Vector3<T> a, Vector3<T> b) {
        return !a.Equals(b);
        }

    public static Vector3<T> operator *(T val, Vector3<T> vector) {
        return new(vector.X * val, vector.Y * val, vector.Z * val);
        }

    public static Vector3<T> operator *(Vector3<T> vector, T value) {
        return new(vector.X * value, vector.Y * value, vector.Z * value);
        }

    public static Vector3<T> operator /(Vector3<T> vector, T value) {
        return new(vector.X / value, vector.Y / value, vector.Z / value);
        }

    public static Vector3<T> operator /(T value, Vector3<T> vector) {
        return new(value / vector.X, value / vector.Y, value / vector.Z);
        }

    public static Vector3<T> operator +(Vector3<T> a, Vector3<T> b) {
        return new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

    public static Vector3<T> operator +(Vector3<T> vector, T value) {
        return new(vector.X + value, vector.Y + value, vector.Z + value);
        }

    public static bool operator <(in Vector3<T> left, in Vector3<T> right) {
        return left.X < right.X
            && left.Y < right.Y
            && left.Z < right.Z;
        }

    public static bool operator <=(in Vector3<T> left, in Vector3<T> right) {
        return left.X <= right.X
            && left.Y <= right.Y
            && left.Z <= right.Z;
        }

    public static bool operator ==(Vector3<T> a, Vector3<T> b) {
        return a.Equals(b);
        }

    public static bool operator >(in Vector3<T> left, in Vector3<T> right) {
        return left.X > right.X
            && left.Y > right.Y
            && left.Z > right.Z;
        }

    public static bool operator >=(in Vector3<T> left, in Vector3<T> right) {
        return left.X >= right.X
            && left.Y >= right.Y
            && left.Z >= right.Z;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3<T> Subtract(Vector3<T> left, Vector3<T> right) {
        return left - right;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Vector3<T> Add(Vector3<T> other) {
        return this + other;
        }

    public readonly Vector3<T> ComponentDivide(Vector3<T> other) {
        return new(X / other.X, Y / other.Y, Z / other.Z);
        }

    public readonly Vector3<T> ComponentMultiply(Vector3<T> other) {
        return new(X * other.X, Y * other.Y, Z * other.Z);
        }

    public readonly void Deconstruct(out T X, out T Y, out T Z) {
        X = this.X; Y = this.Y; Z = this.Z;
        }

    public readonly bool Equals(Vector3<T> other) {
        return X == other.X && Y == other.Y && Z == other.Z;
        }

    public override readonly bool Equals(object? obj) {
        return obj is Vector3<T> vector && Equals(vector);
        }

    public override readonly int GetHashCode() {
        return HashCode.Combine(X, Y, Z);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Vector3<T> Subtract(Vector3<T> other) {
        return this - other;
        }

    #endregion Operators, Equatability & Comparability

    #region Orthogonality & Orthonormality

    public static bool IsOrthogonalWith(Vector3<T> left, Vector3<T> right) {
        return left.Dot(right) == T.Zero;
        }

    public static bool IsOrthonormalWith(Vector3<T> left, Vector3<T> right) {
        return left.Dot(right) == T.Zero && left.MagnitudeSquared == T.One && right.MagnitudeSquared == T.One;
        }

    public static Vector3<T> Orthogonal(Vector3<T> vector) {
        return T.Abs(vector.X) > T.Abs(vector.Y)
            ? new(-vector.Z, T.Zero, +vector.X)
            : new(T.Zero, +vector.Z, -vector.Y);
        }

    public readonly bool IsOrthogonalWith(Vector3<T> other) {
        return Dot(other) == T.Zero;
        }

    public readonly bool IsOrthonormalWith(Vector3<T> other) {
        return Dot(other) == T.Zero && MagnitudeSquared == T.One && other.MagnitudeSquared == T.One;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Vector3<T> Orthogonal() {
        return Orthogonal(this);
        }

    #endregion Orthogonality & Orthonormality

    #region Rounding

    public static Vector3<T> Round(Vector3<T> vector) {
        return new(T.Round(vector.X), T.Round(vector.Y), T.Round(vector.Z));
        }

    public static Vector3<T> Round(Vector3<T> vector, byte digits) {
        return new(T.Round(vector.X, digits), T.Round(vector.Y, digits), T.Round(vector.Z, digits));
        }

    public static void Round(ref Vector3<T> vector) {
        vector.X = T.Round(vector.X);
        vector.Y = T.Round(vector.Y);
        vector.Z = T.Round(vector.Z);
        }

    public static void Round(ref Vector3<T> vector, byte digits) {
        vector.X = T.Round(vector.X, digits);
        vector.Y = T.Round(vector.Y, digits);
        vector.Z = T.Round(vector.Z, digits);
        }

    public readonly Vector3<T> Round(byte digits) {
        return new(T.Round(X, digits), T.Round(Y, digits), T.Round(Z, digits));
        }

    public readonly Vector3<T> Round() {
        return new(T.Round(X), T.Round(Y), T.Round(Z));
        }

    #endregion Rounding

    #region Scale

    public static Vector3<T> Scale(Vector3<T> vector, T factor) {
        return vector * factor;
        }

    public static void Scale(ref Vector3<T> vector, T factor) {
        vector.X *= factor;
        vector.Y *= factor;
        vector.Z *= factor;
        }

    public readonly Vector3<T> Scale(T factor) {
        return this * factor;
        }

    #endregion Scale

    #region Sign

    public static Vector3<T> Sign(Vector3<T> vector) {
        return new(
            x: vector.X > T.Zero ? T.One : vector.X < T.Zero ? T.NegativeOne : T.Zero,
            y: vector.Y > T.Zero ? T.One : vector.Y < T.Zero ? T.NegativeOne : T.Zero,
            z: vector.Z > T.Zero ? T.One : vector.Z < T.Zero ? T.NegativeOne : T.Zero
            );
        }

    public readonly Vector3<T> Sign() {
        return new(
            x: X > T.Zero ? T.One : X < T.Zero ? T.NegativeOne : T.Zero,
            y: Y > T.Zero ? T.One : Y < T.Zero ? T.NegativeOne : T.Zero,
            z: Z > T.Zero ? T.One : Z < T.Zero ? T.NegativeOne : T.Zero
            );
        }

    #endregion Sign

    #region SizeOf and Intrinsics

    public static int Length { get; } = 3;

    public static unsafe int SizeOf { get; } = sizeof(T) * Length;

    #endregion SizeOf and Intrinsics

    #region Step

    public static Vector3<T> Step(Vector3<T> left, Vector3<T> right) {
        return left + Sign(right - left);
        }

    public readonly Vector3<T> Step(Vector3<T> other) {
        return this + Sign(other - this);
        }

    #endregion Step

    #region Strings

    public override readonly string ToString() {
        return $"[{X}, {Y}, {Z}]";
        }

    public readonly string ToString(byte digits, int integerLength, int paddingLength) {
        return $"[{X.Stringify(digits, integerLength, paddingLength)}, {Y.Stringify(digits, integerLength, paddingLength)}, {Z.Stringify(digits, integerLength, paddingLength)}]";
        }

    #endregion Strings

    #region Swizzling

    #region Vector3 Swizzles

    public readonly Vector3<T> XXX => new(X, X, X);
    public readonly Vector3<T> YXX => new(Y, X, X);
    public readonly Vector3<T> ZXX => new(Z, X, X);
    public readonly Vector3<T> XYX => new(X, Y, X);
    public readonly Vector3<T> YYX => new(Y, Y, X);
    public readonly Vector3<T> ZYX => new(Z, Y, X);
    public readonly Vector3<T> XZX => new(X, Z, X);
    public readonly Vector3<T> YZX => new(Y, Z, X);
    public readonly Vector3<T> ZZX => new(Z, Z, X);
    public readonly Vector3<T> XXY => new(X, X, Y);
    public readonly Vector3<T> YXY => new(Y, X, Y);
    public readonly Vector3<T> ZXY => new(Z, X, Y);
    public readonly Vector3<T> XYY => new(X, Y, Y);
    public readonly Vector3<T> YYY => new(Y, Y, Y);
    public readonly Vector3<T> ZYY => new(Z, Y, Y);
    public readonly Vector3<T> XZY => new(X, Z, Y);
    public readonly Vector3<T> YZY => new(Y, Z, Y);
    public readonly Vector3<T> ZZY => new(Z, Z, Y);
    public readonly Vector3<T> XXZ => new(X, X, Z);
    public readonly Vector3<T> YXZ => new(Y, X, Z);
    public readonly Vector3<T> ZXZ => new(Z, X, Z);
    public readonly Vector3<T> XYZ => new(X, Y, Z);
    public readonly Vector3<T> YYZ => new(Y, Y, Z);
    public readonly Vector3<T> ZYZ => new(Z, Y, Z);
    public readonly Vector3<T> XZZ => new(X, Z, Z);
    public readonly Vector3<T> YZZ => new(Y, Z, Z);
    public readonly Vector3<T> ZZZ => new(Z, Z, Z);

    #endregion Vector3 Swizzles

    #region Vector2 Swizzles

    public readonly Vector2<T> XX => new(X, X);
    public readonly Vector2<T> YX => new(Y, X);
    public readonly Vector2<T> ZX => new(Z, X);
    public readonly Vector2<T> XY => new(X, Y);
    public readonly Vector2<T> YY => new(Y, Y);
    public readonly Vector2<T> ZY => new(Z, Y);
    public readonly Vector2<T> XZ => new(X, Z);
    public readonly Vector2<T> YZ => new(Y, Z);
    public readonly Vector2<T> ZZ => new(Z, Z);

    #endregion Vector2 Swizzles

    #endregion Swizzling
    }
