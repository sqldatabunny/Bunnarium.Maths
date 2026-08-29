using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static Bunnarium.Tools.Extensions;

namespace Bunnarium.Maths.Primitives;

/// <summary> A container for three integer values, X, Y, and Z. Useful for a variety of purposes, a vector may represent a coordinate, a two-part value, an offset, or a classical vector with a direction and a <see cref="Magnitude">magnitude</see>.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
[DebuggerDisplay("{ToString(), nq}")]
public struct IntegralVector3<T> : IIntegralVector<IntegralVector3<T>, T>
    where T : unmanaged, IBinaryInteger<T>, IMinMaxValue<T> {

    /// <inheritdoc
    /// cref="DocStrings.VectorComponentX"/>
    public T X { readonly get; set; }

    /// <inheritdoc
    /// cref="DocStrings.VectorComponentY"/>
    public T Y { readonly get; set; }

    /// <inheritdoc
    /// cref="DocStrings.VectorComponentZ"/>
    public T Z { readonly get; set; }

    public readonly void Deconstruct(out T X, out T Y, out T Z) {
        X = this.X;
        Y = this.Y;
        Z = this.Z;
        }

    #region Constructors and Factories

    /// <inheritdoc
    /// cref="IVector{TVector, T}.Create(T)"/>
    public IntegralVector3(T xyz) {
        X = Y = Z = xyz;
        }

    /// <param name="x">The vector's X component.</param>
    /// <param name="y">The vector's Y component.</param>
    /// <param name="z">The vector's Z component.</param>
    public IntegralVector3(T x, T y, T z) {
        X = x;
        Y = y;
        Z = z;
        }

    /// <param name="xy">The vector's X and Y components.</param>
    /// <param name="z">The vector's Z component.</param>
    public IntegralVector3(IntegralVector2<T> xy, T z) {
        X = xy.X;
        Y = xy.Y;
        Z = z;
        }

    public static IntegralVector3<T> Create(T value) {
        return new(value);
        }

    #endregion Constructors and Factories

    #region Constants and Initialization

    public static int Length { get; } = 3;
    public static IntegralVector3<T> MaxValue { get; } = IntegralVector3<T>.Create(T.MaxValue);
    public static IntegralVector3<T> MinValue { get; } = IntegralVector3<T>.Create(T.MinValue);
    public static IntegralVector3<T> One { get; } = new(T.One, T.One, T.One);
    public static IntegralVector3<T> Right { get; } = new(T.One, T.Zero, T.Zero);
    public static unsafe int SizeOf { get; } = sizeof(T) * Length;
    public static IntegralVector3<T> Up { get; } = new(T.Zero, T.One, T.Zero);
    public static IntegralVector3<T> Zero { get; } = new(T.Zero, T.Zero, T.Zero);
    public static IntegralVector3<T> Left { get; } = new(-T.One, T.Zero, T.Zero);
    public static IntegralVector3<T> Down { get; } = new(T.Zero, -T.One, T.Zero);

#if MATRIX_LEFTHANDED_COORDINATE_SYSTEM
    public static IntegralVector3<T> Forward { get; } = new(T.Zero, T.Zero, T.One);
#elif MATRIX_RIGHTHANDED_COORDINATE_SYSTEM
    public static IntegralVector3<T> Forward { get; } = new(T.Zero, T.Zero, -T.One);
#else
    public static IntegralVector3<T> Forward => throw new ApplicationException(Matrix.Docs.BaseMessage);
#endif

#if MATRIX_LEFTHANDED_COORDINATE_SYSTEM
    public static IntegralVector3<T> Backward { get; } = new(T.Zero, T.Zero, -T.One);
#elif MATRIX_RIGHTHANDED_COORDINATE_SYSTEM
    public static IntegralVector3<T> Backward { get; } = new(T.Zero, T.Zero, T.One);
#else
    public static IntegralVector3<T> Backward => throw new ApplicationException(Matrix.Docs.BaseMessage);
#endif

    #endregion Constants and Initialization

    #region Absolute Values

    public static IntegralVector3<T> Abs(in IntegralVector3<T> vector) {
        return new(
            x: T.Abs(vector.X),
            y: T.Abs(vector.Y),
            z: T.Abs(vector.Z)
            );
        }

    public readonly IntegralVector3<T> Abs() {
        return new(
            x: T.Abs(X),
            y: T.Abs(Y),
            z: T.Abs(Z)
            );
        }

    #endregion Absolute Values

    #region Comparability

    public static bool operator <(in IntegralVector3<T> left, in IntegralVector3<T> right) {
        return left.X < right.X
            && left.Y < right.Y
            && left.Z < right.Z;
        }

    public static bool operator <=(in IntegralVector3<T> left, in IntegralVector3<T> right) {
        return left.X <= right.X
            && left.Y <= right.Y
            && left.Z <= right.Z;
        }

    public static bool operator >(in IntegralVector3<T> left, in IntegralVector3<T> right) {
        return left.X > right.X
            && left.Y > right.Y
            && left.Z > right.Z;
        }

    public static bool operator >=(in IntegralVector3<T> left, in IntegralVector3<T> right) {
        return left.X >= right.X
            && left.Y >= right.Y
            && left.Z >= right.Z;
        }

    #endregion Comparability

    #region Conversions

    /// <returns> An <see cref="IntegralVector2{T}"/> populated by this vector's X and Y values.
    /// </returns>
    public readonly IntegralVector2<T> ToIntegralVector2 => new(X, Y);

    /// <returns> An <see cref="IntegralVector4{T}"/> populated in-part by this vector's values.
    /// </returns>
    /// <param name="w">The value of the output vector's fourth component.</param>
    public readonly IntegralVector4<T> ToIntegralVector4(T w) {
        return new(X, Y, Z, w);
        }

    public static Span<T> ToSpan(ref IntegralVector3<T> vector) {
        return MemoryMarshal.CreateSpan(ref Unsafe.As<IntegralVector3<T>, T>(ref vector), 3);
        }

    #endregion Conversions

    #region Cross

    public static IntegralVector3<T> Cross(IntegralVector3<T> a, IntegralVector3<T> b) {
        return new(a.Y * b.Z - b.Y * a.Z, a.Z * b.X - b.Z * a.X, a.X * b.Y - b.X * a.Y);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly IntegralVector3<T> Cross(IntegralVector3<T> other) {
        return Cross(this, other);
        }

    #endregion Cross

    #region Grid-based Functions

    /// <inheritdoc
    /// cref="IIntegralVector{TVector, T}.GetCartesianProduct(TVector)"/>
    public static Sets.CartesianProduct<IntegralVector3<T>, T> GetCartesianProduct(IntegralVector3<T> dimensions) {
        return new(dimensions);
        }

    static IEnumerable<IntegralVector3<T>> IIntegralVector<IntegralVector3<T>, T>.GetCartesianProduct(IntegralVector3<T> dimensions) {
        return new Sets.CartesianProduct<IntegralVector3<T>, T>(dimensions);
        }

    public static bool IsCornerOf(IntegralVector3<T> position, IntegralVector3<T> dimensions) {
        var x = position.X;
        var y = position.Y;
        var z = position.Z;
        return
            (x == T.Zero || x == dimensions.X - T.One) &&
            (y == T.Zero || y == dimensions.Y - T.One) &&
            (z == T.Zero || z == dimensions.Z - T.One);
        }

    public static bool IsOnEdgeButNotCornerOf(IntegralVector3<T> position, IntegralVector3<T> dimensions) {
        var extremes = 0;
        var x = position.X;
        var y = position.Y;
        var z = position.Z;
        if (x == T.Zero || x == dimensions.X - T.One) extremes++;
        if (y == T.Zero || y == dimensions.Y - T.One) extremes++;
        if (z == T.Zero || z == dimensions.Z - T.One) extremes++;
        return extremes == 2;
        }

    public static bool IsOnEdgeOf(IntegralVector3<T> position, IntegralVector3<T> dimensions) {
        var extremes = 0;
        var x = position.X;
        var y = position.Y;
        var z = position.Z;
        if (x == T.Zero || x == dimensions.X - T.One) extremes++;
        if (y == T.Zero || y == dimensions.Y - T.One) extremes++;
        if (z == T.Zero || z == dimensions.Z - T.One) extremes++;
        return extremes >= 2;
        }

    public static bool IsOnOutskirtsOf(IntegralVector3<T> position, IntegralVector3<T> dimensions) {
        return IsOnSurfaceOf(position, dimensions);
        }

    public static bool IsOnSurfaceButNotEdgeOf(IntegralVector3<T> position, IntegralVector3<T> dimensions) {
        var extremes = 0;
        var x = position.X;
        var y = position.Y;
        var z = position.Z;
        if (x == T.Zero || x == dimensions.X - T.One) extremes++;
        if (y == T.Zero || y == dimensions.Y - T.One) extremes++;
        if (z == T.Zero || z == dimensions.Z - T.One) extremes++;
        return extremes == 1;
        }

    public static bool IsOnSurfaceOf(IntegralVector3<T> position, IntegralVector3<T> dimensions) {
        if (position.X == T.Zero || position.Y == T.Zero || position.Z == T.Zero)
            return true;
        return position.X == dimensions.X - T.One ||
                position.Y == dimensions.Y - T.One ||
                position.Z == dimensions.Z - T.One;
        }

    public readonly IEnumerable<IntegralVector3<T>> GetCartesianProduct() {
        return GetCartesianProduct(this);
        }

    public readonly bool IsCornerOf(IntegralVector3<T> dimensions) {
        return IsCornerOf(this, dimensions);
        }

    public readonly bool IsOnEdgeButNotCornerOf(IntegralVector3<T> dimensions) {
        return IsOnEdgeButNotCornerOf(this, dimensions);
        }

    public readonly bool IsOnEdgeOf(IntegralVector3<T> dimensions) {
        return IsOnEdgeOf(this, dimensions);
        }

    public readonly bool IsOnOutskirtsOf(IntegralVector3<T> dimensions) {
        return IsOnOutskirtsOf(this, dimensions);
        }

    public readonly bool IsOnSurfaceButNotEdgeOf(IntegralVector3<T> dimensions) {
        return IsOnSurfaceButNotEdgeOf(this, dimensions);
        }

    public readonly bool IsOnSurfaceOf(IntegralVector3<T> dimensions) {
        return IsOnSurfaceOf(this, dimensions);
        }

    #endregion Grid-based Functions

    #region Horizontal

    public readonly T AbsoluteSum => T.Abs(X) + T.Abs(Y) + T.Abs(Z);
    public readonly T Product => X * Y * Z;
    public readonly T Sum => X + Y + Z;

    public static T HorizontalAbsoluteSum(in IntegralVector3<T> vector) {
        return T.Abs(vector.X) + T.Abs(vector.Y) + T.Abs(vector.Z);
        }

    public static T HorizontalProduct(in IntegralVector3<T> vector) {
        return vector.X * vector.Y * vector.Z;
        }

    public static T HorizontalSum(in IntegralVector3<T> vector) {
        return vector.X + vector.Y + vector.Z;
        }

    #endregion Horizontal

    #region Lexicographical Ordering

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool LexicographicallyPrecedes(IntegralVector3<T> left, IntegralVector3<T> right) {
        return (left.X < right.X)
            || (left.X == right.X
                && (left.Y < right.Y
                || (left.Y == right.Y && left.Z < right.Z)));
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool LexicographicallyPrecedes(IntegralVector3<T> other) {
        return (X < other.X)
            || (X == other.X
                && (Y < other.Y
                || (Y == other.Y && Z < other.Z)));
        }

    #endregion Lexicographical Ordering

    #region Magnitude and Dot

    public readonly T Magnitude {
        get => GenericNumbers<double>.ToBinaryInteger<T>(
                    double.Sqrt(
                        GenericNumbers<double>.FromBinaryInteger(MagnitudeSquared)
                        ));
        }

    public readonly T MagnitudeSquared => X * X + Y * Y + Z * Z;

    public static T Dot(IntegralVector3<T> left, IntegralVector3<T> right) {
        return left.X * right.X + left.Y * right.Y + left.Z * right.Z;
        }

    public readonly T Dot(IntegralVector3<T> other) {
        return X * other.X + Y * other.Y + Z * other.Z;
        }

    #endregion Magnitude and Dot

    #region Max / Min

    public static T HorizontalMax(IntegralVector3<T> vector) {
        return T.Max(T.Max(vector.X, vector.Y), vector.Z);
        }

    public static T HorizontalMin(IntegralVector3<T> vector) {
        return T.Min(T.Min(vector.X, vector.Y), vector.Z);
        }

    public static IntegralVector3<T> Max(IntegralVector3<T> left, IntegralVector3<T> right) {
        return new(
            x: T.Max(left.X, right.X),
            y: T.Max(left.Y, right.Y),
            z: T.Max(left.Z, right.Z)
            );
        }

    public static IntegralVector3<T> Min(IntegralVector3<T> left, IntegralVector3<T> right) {
        return new(
            x: T.Min(left.X, right.X),
            y: T.Min(left.Y, right.Y),
            z: T.Min(left.Z, right.Z)
            );
        }

    public readonly T HorizontalMax() {
        return T.Max(T.Max(X, Y), Z);
        }

    public readonly T HorizontalMin() {
        return T.Min(T.Min(X, Y), Z);
        }

    public readonly IntegralVector3<T> Max(IntegralVector3<T> other) {
        return new IntegralVector3<T>(
            x: T.Max(X, other.X),
            y: T.Max(Y, other.Y),
            z: T.Max(Z, other.Z)
            );
        }

    public readonly IntegralVector3<T> Min(IntegralVector3<T> other) {
        return new IntegralVector3<T>(
            x: T.Min(X, other.X),
            y: T.Min(Y, other.Y),
            z: T.Min(Z, other.Z)
            );
        }

    #endregion Max / Min

    #region Negation

    /// <inheritdoc/>
    /// <remarks><inheritdoc cref="DocStrings.Disclaimer_Negation_IntegralMayBeUnsigned"/></remarks>
    public static IntegralVector3<T> Negate(IntegralVector3<T> vector) {
        ThrowIfUnsigned<T>();
        return new(T.Zero - vector.X, T.Zero - vector.Y, T.Zero - vector.Z);
        }

    /// <inheritdoc/>
    /// <remarks><inheritdoc cref="DocStrings.Disclaimer_Negation_IntegralMayBeUnsigned"/></remarks>
    public static void Negate(ref IntegralVector3<T> vector) {
        ThrowIfUnsigned<T>();
        vector.X = T.Zero - vector.X;
        vector.Y = T.Zero - vector.Y;
        vector.Z = T.Zero - vector.Z;
        }

    /// <inheritdoc/>
    /// <remarks><inheritdoc cref="DocStrings.Disclaimer_Negation_IntegralMayBeUnsigned"/></remarks>
    public readonly IntegralVector3<T> Negate() {
        ThrowIfUnsigned<T>();
        return new(T.Zero - X, T.Zero - Y, T.Zero - Z);
        }

    #endregion Negation

    #region Normalization

    public static IntegralVector3<T> Normalize(IntegralVector3<T> vector, T magnitude) {
        return (vector * magnitude) / vector.Magnitude;
        }

    public static void Normalize(ref IntegralVector3<T> vector, T magnitude) {
        vector = (vector * magnitude) / vector.Magnitude;
        }

    public readonly IntegralVector3<T> Normalize(T magnitude) {
        return (this * magnitude) / Magnitude;
        }

    #endregion Normalization

    #region Operators, Equatability & Comparability

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IntegralVector3<T> Add(IntegralVector3<T> left, IntegralVector3<T> right) {
        return left + right;
        }

    public static IntegralVector3<T> ComponentDivide(IntegralVector3<T> left, IntegralVector3<T> right) {
        return new(
            x: left.X / right.X,
            y: left.Y / right.Y,
            z: left.Z / right.Z
            );
        }

    public static IntegralVector3<T> ComponentMultiply(IntegralVector3<T> left, IntegralVector3<T> right) {
        return new(
            x: left.X * right.X,
            y: left.Y * right.Y,
            z: left.Z * right.Z
            );
        }

    public static IntegralVector3<T> operator -(IntegralVector3<T> left, IntegralVector3<T> right) {
        return new(
            x: left.X - right.X,
            y: left.Y - right.Y,
            z: left.Z - right.Z
            );
        }

    public static bool operator !=(IntegralVector3<T> left, IntegralVector3<T> right) {
        return left.Equals(right) == false;
        }

    public static IntegralVector3<T> operator *(IntegralVector3<T> vector, T value) {
        return new(
            x: vector.X * value,
            y: vector.Y * value,
            z: vector.Z * value
            );
        }

    public static IntegralVector3<T> operator *(T value, IntegralVector3<T> vector) {
        return new(
            x: vector.X * value,
            y: vector.Y * value,
            z: vector.Z * value
            );
        }

    public static IntegralVector3<T> operator /(IntegralVector3<T> vector, T value) {
        return new(
            x: vector.X / value,
            y: vector.Y / value,
            z: vector.Z / value
            );
        }

    public static IntegralVector3<T> operator /(T value, IntegralVector3<T> vector) {
        return new(
            x: value / vector.X,
            y: value / vector.Y,
            z: value / vector.Z
            );
        }

    public static IntegralVector3<T> operator +(IntegralVector3<T> left, IntegralVector3<T> right) {
        return new(
            x: left.X + right.X,
            y: left.Y + right.Y,
            z: left.Z + right.Z
            );
        }

    public static bool operator ==(IntegralVector3<T> left, IntegralVector3<T> right) {
        return left.Equals(right);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IntegralVector3<T> Subtract(IntegralVector3<T> left, IntegralVector3<T> right) {
        return left - right;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly IntegralVector3<T> Add(IntegralVector3<T> other) {
        return this + other;
        }

    public readonly IntegralVector3<T> ComponentDivide(IntegralVector3<T> other) {
        return new(
            x: X / other.X,
            y: Y / other.Y,
            z: Z / other.Z
            );
        }

    public readonly IntegralVector3<T> ComponentMultiply(IntegralVector3<T> other) {
        return new(
            x: X * other.X,
            y: Y * other.Y,
            z: Z * other.Z
            );
        }

    public readonly bool Equals(IntegralVector3<T> other) {
        return
            (X == other.X) &&
            (Y == other.Y) &&
            (Z == other.Z);
        }

    public override readonly bool Equals(object? obj) {
        return obj is IntegralVector3<T> vector && Equals(vector);
        }

    public override readonly int GetHashCode() {
        return HashCode.Combine(X, Y, Z);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly IntegralVector3<T> Subtract(IntegralVector3<T> other) {
        return this - other;
        }

    #endregion Operators, Equatability & Comparability

    #region Orthogonality & Orthonormality

    public static IntegralVector3<T> Orthogonal(IntegralVector3<T> vector) {
        return T.Abs(vector.X) > T.Abs(vector.Y)
            ? new(-vector.Z, T.Zero, +vector.X)
            : new(T.Zero, +vector.Z, -vector.Y);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly IntegralVector3<T> Orthogonal() {
        return Orthogonal(this);
        }

    public static bool IsOrthogonalWith(IntegralVector3<T> left, IntegralVector3<T> right) {
        return Dot(left, right) == T.Zero;
        }

    public static bool IsOrthonormalWith(IntegralVector3<T> left, IntegralVector3<T> right) {
        return Dot(left, right) == T.Zero && left.MagnitudeSquared == T.One && right.MagnitudeSquared == T.One;
        }

    public readonly bool IsOrthogonalWith(IntegralVector3<T> other) {
        return Dot(other) == T.Zero;
        }

    public readonly bool IsOrthonormalWith(IntegralVector3<T> other) {
        return Dot(other) == T.Zero && MagnitudeSquared == T.One && other.MagnitudeSquared == T.One;
        }

    #endregion Orthogonality & Orthonormality

    #region Scale

    public static IntegralVector3<T> Scale(IntegralVector3<T> vector, T factor) {
        return vector * factor;
        }

    public static void Scale(ref IntegralVector3<T> vector, T factor) {
        vector.X *= factor;
        vector.Y *= factor;
        vector.Z *= factor;
        }

    public readonly IntegralVector3<T> Scale(T factor) {
        return this * factor;
        }

    #endregion Scale

    #region Sign

    public static IntegralVector3<T> Sign(IntegralVector3<T> vector) {
        return new(
            x: vector.X > T.Zero ? T.One : vector.X < T.Zero ? -T.One : T.Zero,
            y: vector.Y > T.Zero ? T.One : vector.Y < T.Zero ? -T.One : T.Zero,
            z: vector.Z > T.Zero ? T.One : vector.Z < T.Zero ? -T.One : T.Zero
            );
        }

    public readonly IntegralVector3<T> Sign() {
        return new(
            x: X > T.Zero ? T.One : X < T.Zero ? -T.One : T.Zero,
            y: Y > T.Zero ? T.One : Y < T.Zero ? -T.One : T.Zero,
            z: Z > T.Zero ? T.One : Z < T.Zero ? -T.One : T.Zero
            );
        }

    #endregion Sign

    #region Step

    public static IntegralVector3<T> Step(IntegralVector3<T> left, IntegralVector3<T> right) {
        return left + Sign(right - left);
        }

    public readonly IntegralVector3<T> Step(IntegralVector3<T> other) {
        return this + Sign(other - this);
        }

    #endregion Step

    #region Strings

    public readonly string ToString(byte digits, int integerLength, int padToLength) {
        return $"[{X.Stringify(digits, integerLength, padToLength)}, {Y.Stringify(digits, integerLength, padToLength)}, {Z.Stringify(digits, integerLength, padToLength)}]";
        }

    public override readonly string ToString() {
        return $"[{X}, {Y}, {Z}]";
        }

    #endregion Strings

    #region Swizzling

    #region Vector3 Swizzles

    public readonly IntegralVector3<T> XXX => new(X, X, X);
    public readonly IntegralVector3<T> YXX => new(Y, X, X);
    public readonly IntegralVector3<T> ZXX => new(Z, X, X);
    public readonly IntegralVector3<T> XYX => new(X, Y, X);
    public readonly IntegralVector3<T> YYX => new(Y, Y, X);
    public readonly IntegralVector3<T> ZYX => new(Z, Y, X);
    public readonly IntegralVector3<T> XZX => new(X, Z, X);
    public readonly IntegralVector3<T> YZX => new(Y, Z, X);
    public readonly IntegralVector3<T> ZZX => new(Z, Z, X);
    public readonly IntegralVector3<T> XXY => new(X, X, Y);
    public readonly IntegralVector3<T> YXY => new(Y, X, Y);
    public readonly IntegralVector3<T> ZXY => new(Z, X, Y);
    public readonly IntegralVector3<T> XYY => new(X, Y, Y);
    public readonly IntegralVector3<T> YYY => new(Y, Y, Y);
    public readonly IntegralVector3<T> ZYY => new(Z, Y, Y);
    public readonly IntegralVector3<T> XZY => new(X, Z, Y);
    public readonly IntegralVector3<T> YZY => new(Y, Z, Y);
    public readonly IntegralVector3<T> ZZY => new(Z, Z, Y);
    public readonly IntegralVector3<T> XXZ => new(X, X, Z);
    public readonly IntegralVector3<T> YXZ => new(Y, X, Z);
    public readonly IntegralVector3<T> ZXZ => new(Z, X, Z);
    public readonly IntegralVector3<T> XYZ => new(X, Y, Z);
    public readonly IntegralVector3<T> YYZ => new(Y, Y, Z);
    public readonly IntegralVector3<T> ZYZ => new(Z, Y, Z);
    public readonly IntegralVector3<T> XZZ => new(X, Z, Z);
    public readonly IntegralVector3<T> YZZ => new(Y, Z, Z);
    public readonly IntegralVector3<T> ZZZ => new(Z, Z, Z);

    #endregion IntegralVector3 Swizzles

    #region IntegralVector2 Swizzles

    public readonly IntegralVector2<T> XX => new(X, X);
    public readonly IntegralVector2<T> YX => new(Y, X);
    public readonly IntegralVector2<T> ZX => new(Z, X);
    public readonly IntegralVector2<T> XY => new(X, Y);
    public readonly IntegralVector2<T> YY => new(Y, Y);
    public readonly IntegralVector2<T> ZY => new(Z, Y);
    public readonly IntegralVector2<T> XZ => new(X, Z);
    public readonly IntegralVector2<T> YZ => new(Y, Z);
    public readonly IntegralVector2<T> ZZ => new(Z, Z);

    #endregion IntegralVector2 Swizzles

    #endregion Swizzling
    }
