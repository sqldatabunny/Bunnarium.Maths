using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using static Bunnarium.Tools.Extensions;
using static Bunnarium.Tools.Utilities.SIMD;

namespace Bunnarium.Maths.Primitives;

/// <summary> A container for four integer values, X, Y, Z, and W. Useful for a variety of purposes, a vector may represent a coordinate, a two-part value, an offset, or a classical vector with a direction and a <see cref="Magnitude">magnitude</see>.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
[DebuggerDisplay("{ToString(), nq}")]
public struct IntegralVector4<T> : IIntegralVector<IntegralVector4<T>, T>
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

    /// <inheritdoc
    /// cref="DocStrings.VectorComponentW"/>
    public T W { readonly get; set; }

    #region Constructors and Factories

    /// <param name="x">The vector's X component.</param>
    /// <param name="y">The vector's Y component.</param>
    /// <param name="z">The vector's Z component.</param>
    /// <param name="w">The vector's W component.</param>
    public IntegralVector4(T x, T y, T z, T w) {
        X = x;
        Y = y;
        Z = z;
        W = w;
        }

    /// <inheritdoc
    /// cref="IVector{TVector, T}.Create(T)"/>
    public IntegralVector4(T xyzw) {
        X = Y = Z = W = xyzw;
        }

    /// <param name="xyz">The vector's X, Y, and Z components.</param>
    /// <param name="w">The vector's W component.</param>
    public IntegralVector4(IntegralVector3<T> xyz, T w) {
        X = xyz.X;
        Y = xyz.Y;
        Z = xyz.Z;
        W = w;
        }

    /// <param name="xy">The vector's X and Y components.</param>
    /// <param name="zw">The vector's Z and W components.</param>
    public IntegralVector4(IntegralVector2<T> xy, IntegralVector2<T> zw) {
        X = xy.X;
        Y = xy.Y;
        Z = zw.X;
        W = zw.Y;
        }

    public static IntegralVector4<T> Create(T value) {
        return new(value);
        }

    #endregion Constructors and Factories

    #region Constants and Initialization

    public static int Length { get; } = 4;
    public static IntegralVector4<T> MaxValue { get; } = IntegralVector4<T>.Create(T.MaxValue);
    public static IntegralVector4<T> MinValue { get; } = IntegralVector4<T>.Create(T.MinValue);
    public static IntegralVector4<T> One { get; } = new(T.One, T.One, T.One, T.One);
    public static IntegralVector4<T> Right { get; } = new(T.One, T.Zero, T.Zero, T.Zero);
    public static unsafe int SizeOf { get; } = sizeof(T) * Length;
    public static IntegralVector4<T> Up { get; } = new(T.Zero, T.One, T.Zero, T.Zero);
    public static IntegralVector4<T> Zero { get; } = new(T.Zero, T.Zero, T.Zero, T.Zero);
    public static IntegralVector4<T> Left { get; } = new(-T.One, T.Zero, T.Zero, T.Zero);
    public static IntegralVector4<T> Down { get; } = new(T.Zero, -T.One, T.Zero, T.Zero);

#if MATRIX_LEFTHANDED_COORDINATE_SYSTEM
    public static IntegralVector4<T> Forward { get; } = new(T.Zero, T.Zero, T.One, T.Zero);
#elif MATRIX_RIGHTHANDED_COORDINATE_SYSTEM
    public static IntegralVector4<T> Forward { get; } = new(T.Zero, T.Zero, -T.One, T.Zero);
#else
    public static IntegralVector4<T> Forward => throw new ApplicationException(Matrix.Docs.BaseMessage);
#endif

#if MATRIX_LEFTHANDED_COORDINATE_SYSTEM
    public static IntegralVector4<T> Backward { get; } = new(T.Zero, T.Zero, -T.One, T.Zero);
#elif MATRIX_RIGHTHANDED_COORDINATE_SYSTEM
    public static IntegralVector4<T> Backward { get; } = new(T.Zero, T.Zero, T.One, T.Zero);
#else
    public static IntegralVector4<T> Backward => throw new ApplicationException(Matrix.Docs.BaseMessage);
#endif

    #endregion Constants and Initialization

    #region Absolute Values

    public static IntegralVector4<T> Abs(in IntegralVector4<T> vector) {
        return new(
            x: T.Abs(vector.X),
            y: T.Abs(vector.Y),
            z: T.Abs(vector.Z),
            w: T.Abs(vector.W)
            );
        }

    public readonly IntegralVector4<T> Abs() {
        return new(
            x: T.Abs(X),
            y: T.Abs(Y),
            z: T.Abs(Z),
            w: T.Abs(W)
            );
        }

    #endregion Absolute Values

    #region Comparability

    [BunnyAttributes.SIMDCandidate]
    public static bool operator <(in IntegralVector4<T> left, in IntegralVector4<T> right) {
        return left.X < right.X
            && left.Y < right.Y
            && left.Z < right.Z
            && left.W < right.W;
        }

    [BunnyAttributes.SIMDCandidate]
    public static bool operator <=(in IntegralVector4<T> left, in IntegralVector4<T> right) {
        return left.X <= right.X
            && left.Y <= right.Y
            && left.Z <= right.Z
            && left.W <= right.W;
        }

    [BunnyAttributes.SIMDCandidate]
    public static bool operator >(in IntegralVector4<T> left, in IntegralVector4<T> right) {
        return left.X > right.X
            && left.Y > right.Y
            && left.Z > right.Z
            && left.W > right.W;
        }

    [BunnyAttributes.SIMDCandidate]
    public static bool operator >=(in IntegralVector4<T> left, in IntegralVector4<T> right) {
        return left.X >= right.X
            && left.Y >= right.Y
            && left.Z >= right.Z
            && left.W >= right.W;
        }

    #endregion Comparability

    #region Conversions

    /// <returns> An <see cref="IntegralVector2{T}"/> populated by this vector's X and Y values.
    /// </returns>
    public readonly IntegralVector2<T> ToIntegralVector2 => new(X, Y);

    /// <returns> An <see cref="IntegralVector3{T}"/> populated by this vector's X, Y, and Z values.
    /// </returns>
    public readonly IntegralVector3<T> ToIntegralVector3 => new(X, Y, Z);

    public static Span<T> ToSpan(ref IntegralVector4<T> vector) {
        return MemoryMarshal.CreateSpan(ref Unsafe.As<IntegralVector4<T>, T>(ref vector), 4);
        }

    #endregion Conversions

    #region Grid-based Functions

    /// <inheritdoc
    /// cref="IIntegralVector{TVector, T}.GetCartesianProduct(TVector)"/>
    public static Sets.CartesianProduct<IntegralVector4<T>, T> GetCartesianProduct(IntegralVector4<T> dimensions) {
        return new(dimensions);
        }

    static IEnumerable<IntegralVector4<T>> IIntegralVector<IntegralVector4<T>, T>.GetCartesianProduct(IntegralVector4<T> dimensions) {
        return new Sets.CartesianProduct<IntegralVector4<T>, T>(dimensions);
        }

    public static bool IsCornerOf(IntegralVector4<T> position, IntegralVector4<T> dimensions) {
        var x = position.X;
        var y = position.Y;
        var z = position.Z;
        var w = position.W;
        var a = dimensions.X - T.One;
        var b = dimensions.Y - T.One;
        var c = dimensions.Z - T.One;
        var d = dimensions.W - T.One;
        return
            (x == T.Zero || x == a) &&
            (y == T.Zero || y == b) &&
            (z == T.Zero || z == c) &&
            (w == T.Zero || w == d);
        }

    public static bool IsOnEdgeButNotCornerOf(IntegralVector4<T> position, IntegralVector4<T> dimensions) {
        var extremes = 0;
        var x = position.X;
        var y = position.Y;
        var z = position.Z;
        var w = position.W;
        if (x == T.Zero || x == dimensions.X - T.One) extremes++;
        if (y == T.Zero || y == dimensions.Y - T.One) extremes++;
        if (z == T.Zero || z == dimensions.Z - T.One) extremes++;
        if (w == T.Zero || w == dimensions.W - T.One) extremes++;
        return extremes == 3;
        }

    public static bool IsOnEdgeOf(IntegralVector4<T> position, IntegralVector4<T> dimensions) {
        var extremes = 0;
        var x = position.X;
        var y = position.Y;
        var z = position.Z;
        var w = position.W;
        if (x == T.Zero || x == dimensions.X - T.One) extremes++;
        if (y == T.Zero || y == dimensions.Y - T.One) extremes++;
        if (z == T.Zero || z == dimensions.Z - T.One) extremes++;
        if (w == T.Zero || w == dimensions.W - T.One) extremes++;
        return extremes >= 3;
        }

    public static bool IsOnOutskirtsOf(IntegralVector4<T> position, IntegralVector4<T> dimensions) {
        if (position.X == T.Zero || position.Y == T.Zero || position.Z == T.Zero || position.W == T.Zero)
            return true;
        return position.X == dimensions.X - T.One ||
                position.Y == dimensions.Y - T.One ||
                position.Z == dimensions.Z - T.One ||
                position.W == dimensions.W - T.One;
        }

    public static bool IsOnSurfaceButNotEdgeOf(IntegralVector4<T> position, IntegralVector4<T> dimensions) {
        var extremes = 0;
        var x = position.X;
        var y = position.Y;
        var z = position.Z;
        var w = position.W;
        if (x == T.Zero || x == dimensions.X - T.One) extremes++;
        if (y == T.Zero || y == dimensions.Y - T.One) extremes++;
        if (z == T.Zero || z == dimensions.Z - T.One) extremes++;
        if (w == T.Zero || w == dimensions.W - T.One) extremes++;
        return extremes == 2;
        }

    public static bool IsOnSurfaceOf(IntegralVector4<T> position, IntegralVector4<T> dimensions) {
        var extremes = 0;
        var x = position.X;
        var y = position.Y;
        var z = position.Z;
        var w = position.W;
        if (x == T.Zero || x == dimensions.X - T.One) extremes++;
        if (y == T.Zero || y == dimensions.Y - T.One) extremes++;
        if (z == T.Zero || z == dimensions.Z - T.One) extremes++;
        if (w == T.Zero || w == dimensions.W - T.One) extremes++;
        return extremes >= 2;
        }

    public readonly IEnumerable<IntegralVector4<T>> GetCartesianProduct() {
        return GetCartesianProduct(this);
        }

    public readonly bool IsCornerOf(IntegralVector4<T> dimensions) {
        return IsCornerOf(this, dimensions);
        }

    public readonly bool IsOnEdgeButNotCornerOf(IntegralVector4<T> dimensions) {
        return IsOnEdgeButNotCornerOf(this, dimensions);
        }

    public readonly bool IsOnEdgeOf(IntegralVector4<T> dimensions) {
        return IsOnEdgeOf(this, dimensions);
        }

    public readonly bool IsOnOutskirtsOf(IntegralVector4<T> dimensions) {
        return IsOnOutskirtsOf(this, dimensions);
        }

    public readonly bool IsOnSurfaceButNotEdgeOf(IntegralVector4<T> dimensions) {
        return IsOnSurfaceButNotEdgeOf(this, dimensions);
        }

    public readonly bool IsOnSurfaceOf(IntegralVector4<T> dimensions) {
        return IsOnSurfaceOf(this, dimensions);
        }

    #endregion Grid-based Functions

    #region Horizontal

    public readonly T AbsoluteSum => T.Abs(X) + T.Abs(Y) + T.Abs(Z) + T.Abs(W);
    public readonly T Product => X * Y * Z * W;
    public readonly T Sum => X + Y + Z + W;

    public static T HorizontalAbsoluteSum(in IntegralVector4<T> vector) {
        return T.Abs(vector.X) + T.Abs(vector.Y) + T.Abs(vector.Z) + T.Abs(vector.W);
        }

    public static T HorizontalProduct(in IntegralVector4<T> vector) {
        return vector.X * vector.Y * vector.Z * vector.W;
        }

    public static T HorizontalSum(in IntegralVector4<T> vector) {
        return vector.X + vector.Y + vector.Z + vector.W;
        }

    #endregion Horizontal

    #region Lexicographical Ordering

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool LexicographicallyPrecedes(IntegralVector4<T> left, IntegralVector4<T> right) {
        return (left.X < right.X)
            || (left.X == right.X
                && (left.Y < right.Y
                || (left.Y == right.Y
                    && (left.Z < right.Z
                    || (left.Z == right.Z && left.W < right.W)))));
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool LexicographicallyPrecedes(IntegralVector4<T> other) {
        return (X < other.X)
            || (X == other.X
                && (Y < other.Y
                || (Y == other.Y
                    && (Z < other.Z
                    || (Z == other.Z && W < other.W)))));
        }

    #endregion Lexicographical Ordering

    #region Magnitude and Dot

    public readonly T Magnitude {
        get => GenericNumbers<double>.ToBinaryInteger<T>(
                    double.Sqrt(
                        GenericNumbers<double>.FromBinaryInteger(MagnitudeSquared)
                        ));
        }

    public readonly T MagnitudeSquared => X * X + Y * Y + Z * Z + W * W;

    public static T Dot(IntegralVector4<T> left, IntegralVector4<T> right) {
        return left.X * right.X + left.Y * right.Y + left.Z * right.Z + left.W * right.W;
        }

    public readonly T Dot(IntegralVector4<T> other) {
        return X * other.X + Y * other.Y + Z * other.Z + W * other.W;
        }

    #endregion Magnitude and Dot

    #region Max / Min

    public static T HorizontalMax(IntegralVector4<T> vector) {
        return T.Max(T.Max(T.Max(vector.X, vector.Y), vector.Z), vector.W);
        }

    public static T HorizontalMin(IntegralVector4<T> vector) {
        return T.Min(T.Min(T.Min(vector.X, vector.Y), vector.Z), vector.W);
        }

    public static IntegralVector4<T> Max(IntegralVector4<T> left, IntegralVector4<T> right) {
        return new(
            x: T.Max(left.X, right.X),
            y: T.Max(left.Y, right.Y),
            z: T.Max(left.Z, right.Z),
            w: T.Max(left.W, right.W)
            );
        }

    public static IntegralVector4<T> Min(IntegralVector4<T> left, IntegralVector4<T> right) {
        return new(
            x: T.Min(left.X, right.X),
            y: T.Min(left.Y, right.Y),
            z: T.Min(left.Z, right.Z),
            w: T.Min(left.W, right.W)
            );
        }

    public readonly T HorizontalMax() {
        return T.Max(T.Max(T.Max(X, Y), Z), W);
        }

    public readonly T HorizontalMin() {
        return T.Min(T.Min(T.Min(X, Y), Z), W);
        }

    public readonly IntegralVector4<T> Max(IntegralVector4<T> other) {
        return new IntegralVector4<T>(
            x: T.Max(X, other.X),
            y: T.Max(Y, other.Y),
            z: T.Max(Z, other.Z),
            w: T.Max(W, other.W)
            );
        }

    public readonly IntegralVector4<T> Min(IntegralVector4<T> other) {
        return new IntegralVector4<T>(
            x: T.Min(X, other.X),
            y: T.Min(Y, other.Y),
            z: T.Min(Z, other.Z),
            w: T.Min(W, other.W)
            );
        }

    #endregion Max / Min

    #region Negation

    /// <inheritdoc/>
    /// <remarks><inheritdoc cref="DocStrings.Disclaimer_Negation_IntegralMayBeUnsigned"/></remarks>
    public static IntegralVector4<T> Negate(IntegralVector4<T> vector) {
        ThrowIfUnsigned<T>();
        return new(T.Zero - vector.X, T.Zero - vector.Y, T.Zero - vector.Z, T.Zero - vector.W);
        }

    /// <inheritdoc/>
    /// <remarks><inheritdoc cref="DocStrings.Disclaimer_Negation_IntegralMayBeUnsigned"/></remarks>
    public static void Negate(ref IntegralVector4<T> vector) {
        ThrowIfUnsigned<T>();
        vector.X = T.Zero - vector.X;
        vector.Y = T.Zero - vector.Y;
        vector.Z = T.Zero - vector.Z;
        vector.W = T.Zero - vector.W;
        }

    /// <inheritdoc/>
    /// <remarks><inheritdoc cref="DocStrings.Disclaimer_Negation_IntegralMayBeUnsigned"/></remarks>
    public readonly IntegralVector4<T> Negate() {
        ThrowIfUnsigned<T>();
        return new(T.Zero - X, T.Zero - Y, T.Zero - Z, T.Zero - W);
        }

    #endregion Negation

    #region Normalization

    public static IntegralVector4<T> Normalize(IntegralVector4<T> vector, T magnitude) {
        return (vector * magnitude) / vector.Magnitude;
        }

    public static void Normalize(ref IntegralVector4<T> vector, T magnitude) {
        vector = (vector * magnitude) / vector.Magnitude;
        }

    public readonly IntegralVector4<T> Normalize(T magnitude) {
        return (this * magnitude) / Magnitude;
        }

    #endregion Normalization

    #region Operators, Equatability & Comparability

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IntegralVector4<T> Add(IntegralVector4<T> left, IntegralVector4<T> right) {
        return left + right;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IntegralVector4<T> ComponentDivide(IntegralVector4<T> left, IntegralVector4<T> right) {
        // no SIMD for Vector256s, those intrinsics don't exist on typical hardware. may be worth re-evaluating later
        if (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(int)) {
            return Unsafe.BitCast<Vector128<T>, IntegralVector4<T>>(Unsafe.BitCast<IntegralVector4<T>, Vector128<T>>(left) / Unsafe.BitCast<IntegralVector4<T>, Vector128<T>>(right));
            }
        else {
            return new(
                x: left.X / right.X,
                y: left.Y / right.Y,
                z: left.Z / right.Z,
                w: left.W / right.W
                );
            }
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IntegralVector4<T> ComponentMultiply(IntegralVector4<T> left, IntegralVector4<T> right) {
        if (Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8) {
            return Unsafe.BitCast<Vector256<T>, IntegralVector4<T>>(Unsafe.BitCast<IntegralVector4<T>, Vector256<T>>(left) * Unsafe.BitCast<IntegralVector4<T>, Vector256<T>>(right));
            }
        else if (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4) {
            return Unsafe.BitCast<Vector128<T>, IntegralVector4<T>>(Unsafe.BitCast<IntegralVector4<T>, Vector128<T>>(left) * Unsafe.BitCast<IntegralVector4<T>, Vector128<T>>(right));
            }
        else if (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2) {
            return Unsafe.BitCast<Vector64<T>, IntegralVector4<T>>(Unsafe.BitCast<IntegralVector4<T>, Vector64<T>>(left) * Unsafe.BitCast<IntegralVector4<T>, Vector64<T>>(right));
            }
        else {
            return new(
                x: left.X * right.X,
                y: left.Y * right.Y,
                z: left.Z * right.Z,
                w: left.W * right.W
                );
            }
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator IntegralVector4<T>((T X, T Y, T Z, T W) tuple) {
        return new(tuple.X, tuple.Y, tuple.Z, tuple.W);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IntegralVector4<T> operator -(IntegralVector4<T> left, IntegralVector4<T> right) {
        if (Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8) {
            return Unsafe.BitCast<Vector256<T>, IntegralVector4<T>>(Unsafe.BitCast<IntegralVector4<T>, Vector256<T>>(left) - Unsafe.BitCast<IntegralVector4<T>, Vector256<T>>(right));
            }
        else if (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4) {
            return Unsafe.BitCast<Vector128<T>, IntegralVector4<T>>(Unsafe.BitCast<IntegralVector4<T>, Vector128<T>>(left) - Unsafe.BitCast<IntegralVector4<T>, Vector128<T>>(right));
            }
        else if (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2) {
            return Unsafe.BitCast<Vector64<T>, IntegralVector4<T>>(Unsafe.BitCast<IntegralVector4<T>, Vector64<T>>(left) - Unsafe.BitCast<IntegralVector4<T>, Vector64<T>>(right));
            }
        else {
            return new(
                x: left.X - right.X,
                y: left.Y - right.Y,
                z: left.Z - right.Z,
                w: left.W - right.W
                );
            }
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(IntegralVector4<T> left, IntegralVector4<T> right) {
        return (left == right) == false;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IntegralVector4<T> operator *(IntegralVector4<T> vector, T value) {
        if (Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8) {
            return Unsafe.BitCast<Vector256<T>, IntegralVector4<T>>(Unsafe.BitCast<IntegralVector4<T>, Vector256<T>>(vector) * Vector256.Create(value));
            }
        else if (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4) {
            return Unsafe.BitCast<Vector128<T>, IntegralVector4<T>>(Unsafe.BitCast<IntegralVector4<T>, Vector128<T>>(vector) * Vector128.Create(value));
            }
        else if (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2) {
            return Unsafe.BitCast<Vector64<T>, IntegralVector4<T>>(Unsafe.BitCast<IntegralVector4<T>, Vector64<T>>(vector) * Vector64.Create(value));
            }
        else {
            return new(
                x: vector.X * value,
                y: vector.Y * value,
                z: vector.Z * value,
                w: vector.W * value
                );
            }
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IntegralVector4<T> operator *(T value, IntegralVector4<T> vector) {
        return vector * value;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IntegralVector4<T> operator /(IntegralVector4<T> vector, T value) {
        if (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(int)) {
            return Unsafe.BitCast<Vector128<T>, IntegralVector4<T>>(Unsafe.BitCast<IntegralVector4<T>, Vector128<T>>(vector) / Vector128.Create(value));
            }
        else {
            return new(
                x: vector.X / value,
                y: vector.Y / value,
                z: vector.Z / value,
                w: vector.W / value
                );
            }
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IntegralVector4<T> operator /(T value, IntegralVector4<T> vector) {
        if (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(int)) {
            return Unsafe.BitCast<Vector128<T>, IntegralVector4<T>>(Vector128.Create(value) / Unsafe.BitCast<IntegralVector4<T>, Vector128<T>>(vector));
            }
        else {
            return new(
                x: value / vector.X,
                y: value / vector.Y,
                z: value / vector.Z,
                w: value / vector.W
                );
            }
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IntegralVector4<T> operator +(IntegralVector4<T> left, IntegralVector4<T> right) {
        if (Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8) {
            return Unsafe.BitCast<Vector256<T>, IntegralVector4<T>>(Unsafe.BitCast<IntegralVector4<T>, Vector256<T>>(left) + Unsafe.BitCast<IntegralVector4<T>, Vector256<T>>(right));
            }
        else if (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4) {
            return Unsafe.BitCast<Vector128<T>, IntegralVector4<T>>(Unsafe.BitCast<IntegralVector4<T>, Vector128<T>>(left) + Unsafe.BitCast<IntegralVector4<T>, Vector128<T>>(right));
            }
        else if (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2) {
            return Unsafe.BitCast<Vector64<T>, IntegralVector4<T>>(Unsafe.BitCast<IntegralVector4<T>, Vector64<T>>(left) + Unsafe.BitCast<IntegralVector4<T>, Vector64<T>>(right));
            }
        else {
            return new(
                x: left.X + right.X,
                y: left.Y + right.Y,
                z: left.Z + right.Z,
                w: left.W + right.W
                );
            }
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(IntegralVector4<T> left, IntegralVector4<T> right) {
        if (Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8) {
            return Vector256.EqualsAll(Unsafe.BitCast<IntegralVector4<T>, Vector256<T>>(left), Unsafe.BitCast<IntegralVector4<T>, Vector256<T>>(right));
            }
        else if (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4) {
            return Vector128.EqualsAll(Unsafe.BitCast<IntegralVector4<T>, Vector128<T>>(left), Unsafe.BitCast<IntegralVector4<T>, Vector128<T>>(right));
            }
        else if (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2) {
            return Vector64.EqualsAll(Unsafe.BitCast<IntegralVector4<T>, Vector64<T>>(left), Unsafe.BitCast<IntegralVector4<T>, Vector64<T>>(right));
            }
        else {
            return (left.X == right.X)
                && (left.Y == right.Y)
                && (left.Z == right.Z)
                && (left.W == right.W);
            }
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IntegralVector4<T> Subtract(IntegralVector4<T> left, IntegralVector4<T> right) {
        return left - right;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly IntegralVector4<T> Add(IntegralVector4<T> other) {
        return this + other;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly IntegralVector4<T> ComponentDivide(IntegralVector4<T> other) {
        return ComponentDivide(this, other);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly IntegralVector4<T> ComponentMultiply(IntegralVector4<T> other) {
        return ComponentMultiply(this, other);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly void Deconstruct(out T x, out T y, out T z, out T w) {
        x = this.X;
        y = this.Y;
        z = this.Z;
        w = this.W;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool Equals(IntegralVector4<T> other) {
        return this == other;
        }

    public override readonly bool Equals(object? obj) {
        return obj is IntegralVector4<T> vector && Equals(vector);
        }

    public override readonly int GetHashCode() {
        return HashCode.Combine(X, Y, Z, W);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly IntegralVector4<T> Subtract(IntegralVector4<T> other) {
        return this - other;
        }

    #endregion Operators, Equatability & Comparability

    #region Orthogonality & Orthonormality

    public static IntegralVector4<T> Orthogonal(IntegralVector4<T> vector) {
        return new(-vector.Y, vector.X, -vector.W, vector.Z);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly IntegralVector4<T> Orthogonal() {
        return Orthogonal(this);
        }

    public static bool IsOrthogonalWith(IntegralVector4<T> left, IntegralVector4<T> right) {
        return Dot(left, right) == T.Zero;
        }

    public static bool IsOrthonormalWith(IntegralVector4<T> left, IntegralVector4<T> right) {
        return Dot(left, right) == T.Zero && left.MagnitudeSquared == T.One && right.MagnitudeSquared == T.One;
        }

    public readonly bool IsOrthogonalWith(IntegralVector4<T> other) {
        return Dot(other) == T.Zero;
        }

    public readonly bool IsOrthonormalWith(IntegralVector4<T> other) {
        return Dot(other) == T.Zero && MagnitudeSquared == T.One && other.MagnitudeSquared == T.One;
        }

    #endregion Orthogonality & Orthonormality

    #region Scale

    public static IntegralVector4<T> Scale(IntegralVector4<T> vector, T factor) {
        return vector * factor;
        }

    public static void Scale(ref IntegralVector4<T> vector, T factor) {
        vector.X *= factor;
        vector.Y *= factor;
        vector.Z *= factor;
        vector.W *= factor;
        }

    public readonly IntegralVector4<T> Scale(T factor) {
        return this * factor;
        }

    #endregion Scale

    #region Sign

    public static IntegralVector4<T> Sign(IntegralVector4<T> vector) {
        return new(
            x: vector.X > T.Zero ? T.One : vector.X < T.Zero ? -T.One : T.Zero,
            y: vector.Y > T.Zero ? T.One : vector.Y < T.Zero ? -T.One : T.Zero,
            z: vector.Z > T.Zero ? T.One : vector.Z < T.Zero ? -T.One : T.Zero,
            w: vector.W > T.Zero ? T.One : vector.W < T.Zero ? -T.One : T.Zero
            );
        }

    public readonly IntegralVector4<T> Sign() {
        return new(
            x: X > T.Zero ? T.One : X < T.Zero ? -T.One : T.Zero,
            y: Y > T.Zero ? T.One : Y < T.Zero ? -T.One : T.Zero,
            z: Z > T.Zero ? T.One : Z < T.Zero ? -T.One : T.Zero,
            w: W > T.Zero ? T.One : W < T.Zero ? -T.One : T.Zero
            );
        }

    #endregion Sign

    #region Step

    public static IntegralVector4<T> Step(IntegralVector4<T> left, IntegralVector4<T> right) {
        return left + Sign(right - left);
        }

    public readonly IntegralVector4<T> Step(IntegralVector4<T> other) {
        return this + Sign(other - this);
        }

    #endregion Step

    #region Strings

    public readonly string ToString(byte digits, int integerLength, int padToLength) {
        return $"[{X.Stringify(digits, integerLength, padToLength)}, {Y.Stringify(digits, integerLength, padToLength)}, {Z.Stringify(digits, integerLength, padToLength)}, {W.Stringify(digits, integerLength, padToLength)}]";
        }

    public readonly override string ToString() {
        return $"[{X}, {Y}, {Z}, {W}]";
        }

    #endregion Strings

    #region Swizzling

    #region IntegralVector2 Swizzles

    public readonly IntegralVector2<T> XX => new(X, X);
    public readonly IntegralVector2<T> YX => new(Y, X);
    public readonly IntegralVector2<T> ZX => new(Z, X);
    public readonly IntegralVector2<T> WX => new(W, X);
    public readonly IntegralVector2<T> XY => new(X, Y);
    public readonly IntegralVector2<T> YY => new(Y, Y);
    public readonly IntegralVector2<T> ZY => new(Z, Y);
    public readonly IntegralVector2<T> WY => new(W, Y);
    public readonly IntegralVector2<T> XZ => new(X, Z);
    public readonly IntegralVector2<T> YZ => new(Y, Z);
    public readonly IntegralVector2<T> ZZ => new(Z, Z);
    public readonly IntegralVector2<T> WZ => new(W, Z);
    public readonly IntegralVector2<T> XW => new(X, W);
    public readonly IntegralVector2<T> YW => new(Y, W);
    public readonly IntegralVector2<T> ZW => new(Z, W);
    public readonly IntegralVector2<T> WW => new(W, W);

    #endregion

    #region IntegralVector3 Swizzles
    public readonly IntegralVector3<T> XXX => new(X, X, X);
    public readonly IntegralVector3<T> YXX => new(Y, X, X);
    public readonly IntegralVector3<T> ZXX => new(Z, X, X);
    public readonly IntegralVector3<T> WXX => new(W, X, X);
    public readonly IntegralVector3<T> XYX => new(X, Y, X);
    public readonly IntegralVector3<T> YYX => new(Y, Y, X);
    public readonly IntegralVector3<T> ZYX => new(Z, Y, X);
    public readonly IntegralVector3<T> WYX => new(W, Y, X);
    public readonly IntegralVector3<T> XZX => new(X, Z, X);
    public readonly IntegralVector3<T> YZX => new(Y, Z, X);
    public readonly IntegralVector3<T> ZZX => new(Z, Z, X);
    public readonly IntegralVector3<T> WZX => new(W, Z, X);
    public readonly IntegralVector3<T> XWX => new(X, W, X);
    public readonly IntegralVector3<T> YWX => new(Y, W, X);
    public readonly IntegralVector3<T> ZWX => new(Z, W, X);
    public readonly IntegralVector3<T> WWX => new(W, W, X);
    public readonly IntegralVector3<T> XXY => new(X, X, Y);
    public readonly IntegralVector3<T> YXY => new(Y, X, Y);
    public readonly IntegralVector3<T> ZXY => new(Z, X, Y);
    public readonly IntegralVector3<T> WXY => new(W, X, Y);
    public readonly IntegralVector3<T> XYY => new(X, Y, Y);
    public readonly IntegralVector3<T> YYY => new(Y, Y, Y);
    public readonly IntegralVector3<T> ZYY => new(Z, Y, Y);
    public readonly IntegralVector3<T> WYY => new(W, Y, Y);
    public readonly IntegralVector3<T> XZY => new(X, Z, Y);
    public readonly IntegralVector3<T> YZY => new(Y, Z, Y);
    public readonly IntegralVector3<T> ZZY => new(Z, Z, Y);
    public readonly IntegralVector3<T> WZY => new(W, Z, Y);
    public readonly IntegralVector3<T> XWY => new(X, W, Y);
    public readonly IntegralVector3<T> YWY => new(Y, W, Y);
    public readonly IntegralVector3<T> ZWY => new(Z, W, Y);
    public readonly IntegralVector3<T> WWY => new(W, W, Y);
    public readonly IntegralVector3<T> XXZ => new(X, X, Z);
    public readonly IntegralVector3<T> YXZ => new(Y, X, Z);
    public readonly IntegralVector3<T> ZXZ => new(Z, X, Z);
    public readonly IntegralVector3<T> WXZ => new(W, X, Z);
    public readonly IntegralVector3<T> XYZ => new(X, Y, Z);
    public readonly IntegralVector3<T> YYZ => new(Y, Y, Z);
    public readonly IntegralVector3<T> ZYZ => new(Z, Y, Z);
    public readonly IntegralVector3<T> WYZ => new(W, Y, Z);
    public readonly IntegralVector3<T> XZZ => new(X, Z, Z);
    public readonly IntegralVector3<T> YZZ => new(Y, Z, Z);
    public readonly IntegralVector3<T> ZZZ => new(Z, Z, Z);
    public readonly IntegralVector3<T> WZZ => new(W, Z, Z);
    public readonly IntegralVector3<T> XWZ => new(X, W, Z);
    public readonly IntegralVector3<T> YWZ => new(Y, W, Z);
    public readonly IntegralVector3<T> ZWZ => new(Z, W, Z);
    public readonly IntegralVector3<T> WWZ => new(W, W, Z);
    public readonly IntegralVector3<T> XXW => new(X, X, W);
    public readonly IntegralVector3<T> YXW => new(Y, X, W);
    public readonly IntegralVector3<T> ZXW => new(Z, X, W);
    public readonly IntegralVector3<T> WXW => new(W, X, W);
    public readonly IntegralVector3<T> XYW => new(X, Y, W);
    public readonly IntegralVector3<T> YYW => new(Y, Y, W);
    public readonly IntegralVector3<T> ZYW => new(Z, Y, W);
    public readonly IntegralVector3<T> WYW => new(W, Y, W);
    public readonly IntegralVector3<T> XZW => new(X, Z, W);
    public readonly IntegralVector3<T> YZW => new(Y, Z, W);
    public readonly IntegralVector3<T> ZZW => new(Z, Z, W);
    public readonly IntegralVector3<T> WZW => new(W, Z, W);
    public readonly IntegralVector3<T> XWW => new(X, W, W);
    public readonly IntegralVector3<T> YWW => new(Y, W, W);
    public readonly IntegralVector3<T> ZWW => new(Z, W, W);
    public readonly IntegralVector3<T> WWW => new(W, W, W);

    #endregion IntegralVector3 Swizzles

    #region IntegralVector4 Swizzles

    public readonly IntegralVector4<T> XXXX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(SplatX(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(SplatX(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(0, 0, 0, 0))) : new IntegralVector4<T>(X, X, X, X)));

    public readonly IntegralVector4<T> YXXX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(1L, 0L, 0L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(1, 0, 0, 0))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(1, 0, 0, 0))) : new IntegralVector4<T>(Y, X, X, X)));

    public readonly IntegralVector4<T> ZXXX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(2L, 0L, 0L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(2, 0, 0, 0))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(2, 0, 0, 0))) : new IntegralVector4<T>(Z, X, X, X)));

    public readonly IntegralVector4<T> WXXX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(3L, 0L, 0L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(3, 0, 0, 0))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(3, 0, 0, 0))) : new IntegralVector4<T>(W, X, X, X)));

    public readonly IntegralVector4<T> XYXX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(0L, 1L, 0L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(0, 1, 0, 0))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(0, 1, 0, 0))) : new IntegralVector4<T>(X, Y, X, X)));

    public readonly IntegralVector4<T> YYXX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(1L, 1L, 0L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(1, 1, 0, 0))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(1, 1, 0, 0))) : new IntegralVector4<T>(Y, Y, X, X)));

    public readonly IntegralVector4<T> ZYXX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(2L, 1L, 0L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(2, 1, 0, 0))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(2, 1, 0, 0))) : new IntegralVector4<T>(Z, Y, X, X)));

    public readonly IntegralVector4<T> WYXX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(3L, 1L, 0L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(3, 1, 0, 0))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(3, 1, 0, 0))) : new IntegralVector4<T>(W, Y, X, X)));

    public readonly IntegralVector4<T> XZXX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(0L, 2L, 0L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(0, 2, 0, 0))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(0, 2, 0, 0))) : new IntegralVector4<T>(X, Z, X, X)));

    public readonly IntegralVector4<T> YZXX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(1L, 2L, 0L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(1, 2, 0, 0))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(1, 2, 0, 0))) : new IntegralVector4<T>(Y, Z, X, X)));

    public readonly IntegralVector4<T> ZZXX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(2L, 2L, 0L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(2, 2, 0, 0))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(2, 2, 0, 0))) : new IntegralVector4<T>(Z, Z, X, X)));

    public readonly IntegralVector4<T> WZXX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(3L, 2L, 0L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(3, 2, 0, 0))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(3, 2, 0, 0))) : new IntegralVector4<T>(W, Z, X, X)));

    public readonly IntegralVector4<T> XWXX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(0L, 3L, 0L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(0, 3, 0, 0))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(0, 3, 0, 0))) : new IntegralVector4<T>(X, W, X, X)));

    public readonly IntegralVector4<T> YWXX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(1L, 3L, 0L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(1, 3, 0, 0))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(1, 3, 0, 0))) : new IntegralVector4<T>(Y, W, X, X)));

    public readonly IntegralVector4<T> ZWXX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(2L, 3L, 0L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(2, 3, 0, 0))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(2, 3, 0, 0))) : new IntegralVector4<T>(Z, W, X, X)));

    public readonly IntegralVector4<T> WWXX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(3L, 3L, 0L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(3, 3, 0, 0))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(3, 3, 0, 0))) : new IntegralVector4<T>(W, W, X, X)));

    public readonly IntegralVector4<T> XXYX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(0L, 0L, 1L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(0, 0, 1, 0))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(0, 0, 1, 0))) : new IntegralVector4<T>(X, X, Y, X)));

    public readonly IntegralVector4<T> YXYX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(1L, 0L, 1L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(1, 0, 1, 0))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(1, 0, 1, 0))) : new IntegralVector4<T>(Y, X, Y, X)));

    public readonly IntegralVector4<T> ZXYX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(2L, 0L, 1L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(2, 0, 1, 0))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(2, 0, 1, 0))) : new IntegralVector4<T>(Z, X, Y, X)));

    public readonly IntegralVector4<T> WXYX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(3L, 0L, 1L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(3, 0, 1, 0))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(3, 0, 1, 0))) : new IntegralVector4<T>(W, X, Y, X)));

    public readonly IntegralVector4<T> XYYX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(0L, 1L, 1L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(0, 1, 1, 0))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(0, 1, 1, 0))) : new IntegralVector4<T>(X, Y, Y, X)));

    public readonly IntegralVector4<T> YYYX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(1L, 1L, 1L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(1, 1, 1, 0))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(1, 1, 1, 0))) : new IntegralVector4<T>(Y, Y, Y, X)));

    public readonly IntegralVector4<T> ZYYX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(2L, 1L, 1L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(2, 1, 1, 0))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(2, 1, 1, 0))) : new IntegralVector4<T>(Z, Y, Y, X)));

    public readonly IntegralVector4<T> WYYX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(3L, 1L, 1L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(3, 1, 1, 0))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(3, 1, 1, 0))) : new IntegralVector4<T>(W, Y, Y, X)));

    public readonly IntegralVector4<T> XZYX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(0L, 2L, 1L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(0, 2, 1, 0))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(0, 2, 1, 0))) : new IntegralVector4<T>(X, Z, Y, X)));

    public readonly IntegralVector4<T> YZYX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(1L, 2L, 1L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(1, 2, 1, 0))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(1, 2, 1, 0))) : new IntegralVector4<T>(Y, Z, Y, X)));

    public readonly IntegralVector4<T> ZZYX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(2L, 2L, 1L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(2, 2, 1, 0))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(2, 2, 1, 0))) : new IntegralVector4<T>(Z, Z, Y, X)));

    public readonly IntegralVector4<T> WZYX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(3L, 2L, 1L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(3, 2, 1, 0))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(3, 2, 1, 0))) : new IntegralVector4<T>(W, Z, Y, X)));

    public readonly IntegralVector4<T> XWYX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(0L, 3L, 1L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(0, 3, 1, 0))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(0, 3, 1, 0))) : new IntegralVector4<T>(X, W, Y, X)));

    public readonly IntegralVector4<T> YWYX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(1L, 3L, 1L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(1, 3, 1, 0))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(1, 3, 1, 0))) : new IntegralVector4<T>(Y, W, Y, X)));

    public readonly IntegralVector4<T> ZWYX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(2L, 3L, 1L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(2, 3, 1, 0))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(2, 3, 1, 0))) : new IntegralVector4<T>(Z, W, Y, X)));

    public readonly IntegralVector4<T> WWYX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(3L, 3L, 1L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(3, 3, 1, 0))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(3, 3, 1, 0))) : new IntegralVector4<T>(W, W, Y, X)));

    public readonly IntegralVector4<T> XXZX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(0L, 0L, 2L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(0, 0, 2, 0))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(0, 0, 2, 0))) : new IntegralVector4<T>(X, X, Z, X)));

    public readonly IntegralVector4<T> YXZX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(1L, 0L, 2L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(1, 0, 2, 0))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(1, 0, 2, 0))) : new IntegralVector4<T>(Y, X, Z, X)));

    public readonly IntegralVector4<T> ZXZX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(2L, 0L, 2L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(2, 0, 2, 0))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(2, 0, 2, 0))) : new IntegralVector4<T>(Z, X, Z, X)));

    public readonly IntegralVector4<T> WXZX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(3L, 0L, 2L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(3, 0, 2, 0))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(3, 0, 2, 0))) : new IntegralVector4<T>(W, X, Z, X)));

    public readonly IntegralVector4<T> XYZX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(0L, 1L, 2L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(0, 1, 2, 0))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(0, 1, 2, 0))) : new IntegralVector4<T>(X, Y, Z, X)));

    public readonly IntegralVector4<T> YYZX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(1L, 1L, 2L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(1, 1, 2, 0))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(1, 1, 2, 0))) : new IntegralVector4<T>(Y, Y, Z, X)));

    public readonly IntegralVector4<T> ZYZX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(2L, 1L, 2L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(2, 1, 2, 0))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(2, 1, 2, 0))) : new IntegralVector4<T>(Z, Y, Z, X)));

    public readonly IntegralVector4<T> WYZX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(3L, 1L, 2L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(3, 1, 2, 0))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(3, 1, 2, 0))) : new IntegralVector4<T>(W, Y, Z, X)));

    public readonly IntegralVector4<T> XZZX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(0L, 2L, 2L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(0, 2, 2, 0))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(0, 2, 2, 0))) : new IntegralVector4<T>(X, Z, Z, X)));

    public readonly IntegralVector4<T> YZZX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(1L, 2L, 2L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(1, 2, 2, 0))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(1, 2, 2, 0))) : new IntegralVector4<T>(Y, Z, Z, X)));

    public readonly IntegralVector4<T> ZZZX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(2L, 2L, 2L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(2, 2, 2, 0))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(2, 2, 2, 0))) : new IntegralVector4<T>(Z, Z, Z, X)));

    public readonly IntegralVector4<T> WZZX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(3L, 2L, 2L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(3, 2, 2, 0))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(3, 2, 2, 0))) : new IntegralVector4<T>(W, Z, Z, X)));

    public readonly IntegralVector4<T> XWZX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(0L, 3L, 2L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(0, 3, 2, 0))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(0, 3, 2, 0))) : new IntegralVector4<T>(X, W, Z, X)));

    public readonly IntegralVector4<T> YWZX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(1L, 3L, 2L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(1, 3, 2, 0))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(1, 3, 2, 0))) : new IntegralVector4<T>(Y, W, Z, X)));

    public readonly IntegralVector4<T> ZWZX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(2L, 3L, 2L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(2, 3, 2, 0))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(2, 3, 2, 0))) : new IntegralVector4<T>(Z, W, Z, X)));

    public readonly IntegralVector4<T> WWZX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(3L, 3L, 2L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(3, 3, 2, 0))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(3, 3, 2, 0))) : new IntegralVector4<T>(W, W, Z, X)));

    public readonly IntegralVector4<T> XXWX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(0L, 0L, 3L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(0, 0, 3, 0))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(0, 0, 3, 0))) : new IntegralVector4<T>(X, X, W, X)));

    public readonly IntegralVector4<T> YXWX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(1L, 0L, 3L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(1, 0, 3, 0))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(1, 0, 3, 0))) : new IntegralVector4<T>(Y, X, W, X)));

    public readonly IntegralVector4<T> ZXWX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(2L, 0L, 3L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(2, 0, 3, 0))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(2, 0, 3, 0))) : new IntegralVector4<T>(Z, X, W, X)));

    public readonly IntegralVector4<T> WXWX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(3L, 0L, 3L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(3, 0, 3, 0))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(3, 0, 3, 0))) : new IntegralVector4<T>(W, X, W, X)));

    public readonly IntegralVector4<T> XYWX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(0L, 1L, 3L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(0, 1, 3, 0))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(0, 1, 3, 0))) : new IntegralVector4<T>(X, Y, W, X)));

    public readonly IntegralVector4<T> YYWX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(1L, 1L, 3L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(1, 1, 3, 0))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(1, 1, 3, 0))) : new IntegralVector4<T>(Y, Y, W, X)));

    public readonly IntegralVector4<T> ZYWX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(2L, 1L, 3L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(2, 1, 3, 0))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(2, 1, 3, 0))) : new IntegralVector4<T>(Z, Y, W, X)));

    public readonly IntegralVector4<T> WYWX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(3L, 1L, 3L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(3, 1, 3, 0))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(3, 1, 3, 0))) : new IntegralVector4<T>(W, Y, W, X)));

    public readonly IntegralVector4<T> XZWX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(0L, 2L, 3L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(0, 2, 3, 0))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(0, 2, 3, 0))) : new IntegralVector4<T>(X, Z, W, X)));

    public readonly IntegralVector4<T> YZWX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(1L, 2L, 3L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(1, 2, 3, 0))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(1, 2, 3, 0))) : new IntegralVector4<T>(Y, Z, W, X)));

    public readonly IntegralVector4<T> ZZWX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(2L, 2L, 3L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(2, 2, 3, 0))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(2, 2, 3, 0))) : new IntegralVector4<T>(Z, Z, W, X)));

    public readonly IntegralVector4<T> WZWX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(3L, 2L, 3L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(3, 2, 3, 0))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(3, 2, 3, 0))) : new IntegralVector4<T>(W, Z, W, X)));

    public readonly IntegralVector4<T> XWWX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(0L, 3L, 3L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(0, 3, 3, 0))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(0, 3, 3, 0))) : new IntegralVector4<T>(X, W, W, X)));

    public readonly IntegralVector4<T> YWWX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(1L, 3L, 3L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(1, 3, 3, 0))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(1, 3, 3, 0))) : new IntegralVector4<T>(Y, W, W, X)));

    public readonly IntegralVector4<T> ZWWX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(2L, 3L, 3L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(2, 3, 3, 0))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(2, 3, 3, 0))) : new IntegralVector4<T>(Z, W, W, X)));

    public readonly IntegralVector4<T> WWWX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(3L, 3L, 3L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(3, 3, 3, 0))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(3, 3, 3, 0))) : new IntegralVector4<T>(W, W, W, X)));

    public readonly IntegralVector4<T> XXXY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(0L, 0L, 0L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(0, 0, 0, 1))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(0, 0, 0, 1))) : new IntegralVector4<T>(X, X, X, Y)));

    public readonly IntegralVector4<T> YXXY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(1L, 0L, 0L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(1, 0, 0, 1))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(1, 0, 0, 1))) : new IntegralVector4<T>(Y, X, X, Y)));

    public readonly IntegralVector4<T> ZXXY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(2L, 0L, 0L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(2, 0, 0, 1))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(2, 0, 0, 1))) : new IntegralVector4<T>(Z, X, X, Y)));

    public readonly IntegralVector4<T> WXXY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(3L, 0L, 0L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(3, 0, 0, 1))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(3, 0, 0, 1))) : new IntegralVector4<T>(W, X, X, Y)));

    public readonly IntegralVector4<T> XYXY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(0L, 1L, 0L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(0, 1, 0, 1))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(0, 1, 0, 1))) : new IntegralVector4<T>(X, Y, X, Y)));

    public readonly IntegralVector4<T> YYXY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(1L, 1L, 0L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(1, 1, 0, 1))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(1, 1, 0, 1))) : new IntegralVector4<T>(Y, Y, X, Y)));

    public readonly IntegralVector4<T> ZYXY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(2L, 1L, 0L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(2, 1, 0, 1))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(2, 1, 0, 1))) : new IntegralVector4<T>(Z, Y, X, Y)));

    public readonly IntegralVector4<T> WYXY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(3L, 1L, 0L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(3, 1, 0, 1))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(3, 1, 0, 1))) : new IntegralVector4<T>(W, Y, X, Y)));

    public readonly IntegralVector4<T> XZXY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(0L, 2L, 0L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(0, 2, 0, 1))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(0, 2, 0, 1))) : new IntegralVector4<T>(X, Z, X, Y)));

    public readonly IntegralVector4<T> YZXY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(1L, 2L, 0L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(1, 2, 0, 1))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(1, 2, 0, 1))) : new IntegralVector4<T>(Y, Z, X, Y)));

    public readonly IntegralVector4<T> ZZXY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(2L, 2L, 0L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(2, 2, 0, 1))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(2, 2, 0, 1))) : new IntegralVector4<T>(Z, Z, X, Y)));

    public readonly IntegralVector4<T> WZXY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(3L, 2L, 0L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(3, 2, 0, 1))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(3, 2, 0, 1))) : new IntegralVector4<T>(W, Z, X, Y)));

    public readonly IntegralVector4<T> XWXY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(0L, 3L, 0L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(0, 3, 0, 1))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(0, 3, 0, 1))) : new IntegralVector4<T>(X, W, X, Y)));

    public readonly IntegralVector4<T> YWXY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(1L, 3L, 0L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(1, 3, 0, 1))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(1, 3, 0, 1))) : new IntegralVector4<T>(Y, W, X, Y)));

    public readonly IntegralVector4<T> ZWXY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(2L, 3L, 0L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(2, 3, 0, 1))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(2, 3, 0, 1))) : new IntegralVector4<T>(Z, W, X, Y)));

    public readonly IntegralVector4<T> WWXY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(3L, 3L, 0L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(3, 3, 0, 1))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(3, 3, 0, 1))) : new IntegralVector4<T>(W, W, X, Y)));

    public readonly IntegralVector4<T> XXYY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(0L, 0L, 1L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(0, 0, 1, 1))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(0, 0, 1, 1))) : new IntegralVector4<T>(X, X, Y, Y)));

    public readonly IntegralVector4<T> YXYY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(1L, 0L, 1L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(1, 0, 1, 1))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(1, 0, 1, 1))) : new IntegralVector4<T>(Y, X, Y, Y)));

    public readonly IntegralVector4<T> ZXYY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(2L, 0L, 1L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(2, 0, 1, 1))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(2, 0, 1, 1))) : new IntegralVector4<T>(Z, X, Y, Y)));

    public readonly IntegralVector4<T> WXYY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(3L, 0L, 1L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(3, 0, 1, 1))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(3, 0, 1, 1))) : new IntegralVector4<T>(W, X, Y, Y)));

    public readonly IntegralVector4<T> XYYY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(0L, 1L, 1L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(0, 1, 1, 1))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(0, 1, 1, 1))) : new IntegralVector4<T>(X, Y, Y, Y)));

    public readonly IntegralVector4<T> YYYY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(SplatY(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(SplatY(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(1, 1, 1, 1))) : new IntegralVector4<T>(Y, Y, Y, Y)));

    public readonly IntegralVector4<T> ZYYY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(2L, 1L, 1L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(2, 1, 1, 1))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(2, 1, 1, 1))) : new IntegralVector4<T>(Z, Y, Y, Y)));

    public readonly IntegralVector4<T> WYYY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(3L, 1L, 1L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(3, 1, 1, 1))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(3, 1, 1, 1))) : new IntegralVector4<T>(W, Y, Y, Y)));

    public readonly IntegralVector4<T> XZYY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(0L, 2L, 1L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(0, 2, 1, 1))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(0, 2, 1, 1))) : new IntegralVector4<T>(X, Z, Y, Y)));

    public readonly IntegralVector4<T> YZYY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(1L, 2L, 1L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(1, 2, 1, 1))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(1, 2, 1, 1))) : new IntegralVector4<T>(Y, Z, Y, Y)));

    public readonly IntegralVector4<T> ZZYY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(2L, 2L, 1L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(2, 2, 1, 1))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(2, 2, 1, 1))) : new IntegralVector4<T>(Z, Z, Y, Y)));

    public readonly IntegralVector4<T> WZYY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(3L, 2L, 1L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(3, 2, 1, 1))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(3, 2, 1, 1))) : new IntegralVector4<T>(W, Z, Y, Y)));

    public readonly IntegralVector4<T> XWYY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(0L, 3L, 1L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(0, 3, 1, 1))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(0, 3, 1, 1))) : new IntegralVector4<T>(X, W, Y, Y)));

    public readonly IntegralVector4<T> YWYY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(1L, 3L, 1L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(1, 3, 1, 1))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(1, 3, 1, 1))) : new IntegralVector4<T>(Y, W, Y, Y)));

    public readonly IntegralVector4<T> ZWYY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(2L, 3L, 1L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(2, 3, 1, 1))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(2, 3, 1, 1))) : new IntegralVector4<T>(Z, W, Y, Y)));

    public readonly IntegralVector4<T> WWYY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(3L, 3L, 1L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(3, 3, 1, 1))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(3, 3, 1, 1))) : new IntegralVector4<T>(W, W, Y, Y)));

    public readonly IntegralVector4<T> XXZY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(0L, 0L, 2L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(0, 0, 2, 1))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(0, 0, 2, 1))) : new IntegralVector4<T>(X, X, Z, Y)));

    public readonly IntegralVector4<T> YXZY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(1L, 0L, 2L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(1, 0, 2, 1))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(1, 0, 2, 1))) : new IntegralVector4<T>(Y, X, Z, Y)));

    public readonly IntegralVector4<T> ZXZY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(2L, 0L, 2L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(2, 0, 2, 1))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(2, 0, 2, 1))) : new IntegralVector4<T>(Z, X, Z, Y)));

    public readonly IntegralVector4<T> WXZY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(3L, 0L, 2L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(3, 0, 2, 1))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(3, 0, 2, 1))) : new IntegralVector4<T>(W, X, Z, Y)));

    public readonly IntegralVector4<T> XYZY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(0L, 1L, 2L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(0, 1, 2, 1))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(0, 1, 2, 1))) : new IntegralVector4<T>(X, Y, Z, Y)));

    public readonly IntegralVector4<T> YYZY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(1L, 1L, 2L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(1, 1, 2, 1))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(1, 1, 2, 1))) : new IntegralVector4<T>(Y, Y, Z, Y)));

    public readonly IntegralVector4<T> ZYZY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(2L, 1L, 2L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(2, 1, 2, 1))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(2, 1, 2, 1))) : new IntegralVector4<T>(Z, Y, Z, Y)));

    public readonly IntegralVector4<T> WYZY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(3L, 1L, 2L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(3, 1, 2, 1))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(3, 1, 2, 1))) : new IntegralVector4<T>(W, Y, Z, Y)));

    public readonly IntegralVector4<T> XZZY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(0L, 2L, 2L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(0, 2, 2, 1))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(0, 2, 2, 1))) : new IntegralVector4<T>(X, Z, Z, Y)));

    public readonly IntegralVector4<T> YZZY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(1L, 2L, 2L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(1, 2, 2, 1))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(1, 2, 2, 1))) : new IntegralVector4<T>(Y, Z, Z, Y)));

    public readonly IntegralVector4<T> ZZZY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(2L, 2L, 2L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(2, 2, 2, 1))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(2, 2, 2, 1))) : new IntegralVector4<T>(Z, Z, Z, Y)));

    public readonly IntegralVector4<T> WZZY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(3L, 2L, 2L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(3, 2, 2, 1))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(3, 2, 2, 1))) : new IntegralVector4<T>(W, Z, Z, Y)));

    public readonly IntegralVector4<T> XWZY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(0L, 3L, 2L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(0, 3, 2, 1))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(0, 3, 2, 1))) : new IntegralVector4<T>(X, W, Z, Y)));

    public readonly IntegralVector4<T> YWZY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(1L, 3L, 2L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(1, 3, 2, 1))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(1, 3, 2, 1))) : new IntegralVector4<T>(Y, W, Z, Y)));

    public readonly IntegralVector4<T> ZWZY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(2L, 3L, 2L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(2, 3, 2, 1))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(2, 3, 2, 1))) : new IntegralVector4<T>(Z, W, Z, Y)));

    public readonly IntegralVector4<T> WWZY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(3L, 3L, 2L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(3, 3, 2, 1))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(3, 3, 2, 1))) : new IntegralVector4<T>(W, W, Z, Y)));

    public readonly IntegralVector4<T> XXWY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(0L, 0L, 3L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(0, 0, 3, 1))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(0, 0, 3, 1))) : new IntegralVector4<T>(X, X, W, Y)));

    public readonly IntegralVector4<T> YXWY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(1L, 0L, 3L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(1, 0, 3, 1))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(1, 0, 3, 1))) : new IntegralVector4<T>(Y, X, W, Y)));

    public readonly IntegralVector4<T> ZXWY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(2L, 0L, 3L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(2, 0, 3, 1))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(2, 0, 3, 1))) : new IntegralVector4<T>(Z, X, W, Y)));

    public readonly IntegralVector4<T> WXWY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(3L, 0L, 3L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(3, 0, 3, 1))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(3, 0, 3, 1))) : new IntegralVector4<T>(W, X, W, Y)));

    public readonly IntegralVector4<T> XYWY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(0L, 1L, 3L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(0, 1, 3, 1))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(0, 1, 3, 1))) : new IntegralVector4<T>(X, Y, W, Y)));

    public readonly IntegralVector4<T> YYWY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(1L, 1L, 3L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(1, 1, 3, 1))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(1, 1, 3, 1))) : new IntegralVector4<T>(Y, Y, W, Y)));

    public readonly IntegralVector4<T> ZYWY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(2L, 1L, 3L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(2, 1, 3, 1))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(2, 1, 3, 1))) : new IntegralVector4<T>(Z, Y, W, Y)));

    public readonly IntegralVector4<T> WYWY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(3L, 1L, 3L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(3, 1, 3, 1))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(3, 1, 3, 1))) : new IntegralVector4<T>(W, Y, W, Y)));

    public readonly IntegralVector4<T> XZWY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(0L, 2L, 3L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(0, 2, 3, 1))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(0, 2, 3, 1))) : new IntegralVector4<T>(X, Z, W, Y)));

    public readonly IntegralVector4<T> YZWY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(1L, 2L, 3L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(1, 2, 3, 1))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(1, 2, 3, 1))) : new IntegralVector4<T>(Y, Z, W, Y)));

    public readonly IntegralVector4<T> ZZWY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(2L, 2L, 3L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(2, 2, 3, 1))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(2, 2, 3, 1))) : new IntegralVector4<T>(Z, Z, W, Y)));

    public readonly IntegralVector4<T> WZWY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(3L, 2L, 3L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(3, 2, 3, 1))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(3, 2, 3, 1))) : new IntegralVector4<T>(W, Z, W, Y)));

    public readonly IntegralVector4<T> XWWY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(0L, 3L, 3L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(0, 3, 3, 1))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(0, 3, 3, 1))) : new IntegralVector4<T>(X, W, W, Y)));

    public readonly IntegralVector4<T> YWWY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(1L, 3L, 3L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(1, 3, 3, 1))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(1, 3, 3, 1))) : new IntegralVector4<T>(Y, W, W, Y)));

    public readonly IntegralVector4<T> ZWWY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(2L, 3L, 3L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(2, 3, 3, 1))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(2, 3, 3, 1))) : new IntegralVector4<T>(Z, W, W, Y)));

    public readonly IntegralVector4<T> WWWY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(3L, 3L, 3L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(3, 3, 3, 1))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(3, 3, 3, 1))) : new IntegralVector4<T>(W, W, W, Y)));

    public readonly IntegralVector4<T> XXXZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(0L, 0L, 0L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(0, 0, 0, 2))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(0, 0, 0, 2))) : new IntegralVector4<T>(X, X, X, Z)));

    public readonly IntegralVector4<T> YXXZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(1L, 0L, 0L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(1, 0, 0, 2))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(1, 0, 0, 2))) : new IntegralVector4<T>(Y, X, X, Z)));

    public readonly IntegralVector4<T> ZXXZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(2L, 0L, 0L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(2, 0, 0, 2))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(2, 0, 0, 2))) : new IntegralVector4<T>(Z, X, X, Z)));

    public readonly IntegralVector4<T> WXXZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(3L, 0L, 0L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(3, 0, 0, 2))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(3, 0, 0, 2))) : new IntegralVector4<T>(W, X, X, Z)));

    public readonly IntegralVector4<T> XYXZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(0L, 1L, 0L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(0, 1, 0, 2))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(0, 1, 0, 2))) : new IntegralVector4<T>(X, Y, X, Z)));

    public readonly IntegralVector4<T> YYXZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(1L, 1L, 0L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(1, 1, 0, 2))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(1, 1, 0, 2))) : new IntegralVector4<T>(Y, Y, X, Z)));

    public readonly IntegralVector4<T> ZYXZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(2L, 1L, 0L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(2, 1, 0, 2))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(2, 1, 0, 2))) : new IntegralVector4<T>(Z, Y, X, Z)));

    public readonly IntegralVector4<T> WYXZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(3L, 1L, 0L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(3, 1, 0, 2))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(3, 1, 0, 2))) : new IntegralVector4<T>(W, Y, X, Z)));

    public readonly IntegralVector4<T> XZXZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(0L, 2L, 0L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(0, 2, 0, 2))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(0, 2, 0, 2))) : new IntegralVector4<T>(X, Z, X, Z)));

    public readonly IntegralVector4<T> YZXZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(1L, 2L, 0L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(1, 2, 0, 2))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(1, 2, 0, 2))) : new IntegralVector4<T>(Y, Z, X, Z)));

    public readonly IntegralVector4<T> ZZXZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(2L, 2L, 0L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(2, 2, 0, 2))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(2, 2, 0, 2))) : new IntegralVector4<T>(Z, Z, X, Z)));

    public readonly IntegralVector4<T> WZXZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(3L, 2L, 0L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(3, 2, 0, 2))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(3, 2, 0, 2))) : new IntegralVector4<T>(W, Z, X, Z)));

    public readonly IntegralVector4<T> XWXZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(0L, 3L, 0L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(0, 3, 0, 2))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(0, 3, 0, 2))) : new IntegralVector4<T>(X, W, X, Z)));

    public readonly IntegralVector4<T> YWXZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(1L, 3L, 0L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(1, 3, 0, 2))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(1, 3, 0, 2))) : new IntegralVector4<T>(Y, W, X, Z)));

    public readonly IntegralVector4<T> ZWXZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(2L, 3L, 0L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(2, 3, 0, 2))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(2, 3, 0, 2))) : new IntegralVector4<T>(Z, W, X, Z)));

    public readonly IntegralVector4<T> WWXZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(3L, 3L, 0L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(3, 3, 0, 2))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(3, 3, 0, 2))) : new IntegralVector4<T>(W, W, X, Z)));

    public readonly IntegralVector4<T> XXYZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(0L, 0L, 1L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(0, 0, 1, 2))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(0, 0, 1, 2))) : new IntegralVector4<T>(X, X, Y, Z)));

    public readonly IntegralVector4<T> YXYZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(1L, 0L, 1L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(1, 0, 1, 2))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(1, 0, 1, 2))) : new IntegralVector4<T>(Y, X, Y, Z)));

    public readonly IntegralVector4<T> ZXYZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(2L, 0L, 1L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(2, 0, 1, 2))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(2, 0, 1, 2))) : new IntegralVector4<T>(Z, X, Y, Z)));

    public readonly IntegralVector4<T> WXYZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(3L, 0L, 1L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(3, 0, 1, 2))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(3, 0, 1, 2))) : new IntegralVector4<T>(W, X, Y, Z)));

    public readonly IntegralVector4<T> XYYZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(0L, 1L, 1L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(0, 1, 1, 2))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(0, 1, 1, 2))) : new IntegralVector4<T>(X, Y, Y, Z)));

    public readonly IntegralVector4<T> YYYZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(1L, 1L, 1L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(1, 1, 1, 2))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(1, 1, 1, 2))) : new IntegralVector4<T>(Y, Y, Y, Z)));

    public readonly IntegralVector4<T> ZYYZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(2L, 1L, 1L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(2, 1, 1, 2))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(2, 1, 1, 2))) : new IntegralVector4<T>(Z, Y, Y, Z)));

    public readonly IntegralVector4<T> WYYZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(3L, 1L, 1L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(3, 1, 1, 2))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(3, 1, 1, 2))) : new IntegralVector4<T>(W, Y, Y, Z)));

    public readonly IntegralVector4<T> XZYZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(0L, 2L, 1L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(0, 2, 1, 2))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(0, 2, 1, 2))) : new IntegralVector4<T>(X, Z, Y, Z)));

    public readonly IntegralVector4<T> YZYZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(1L, 2L, 1L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(1, 2, 1, 2))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(1, 2, 1, 2))) : new IntegralVector4<T>(Y, Z, Y, Z)));

    public readonly IntegralVector4<T> ZZYZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(2L, 2L, 1L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(2, 2, 1, 2))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(2, 2, 1, 2))) : new IntegralVector4<T>(Z, Z, Y, Z)));

    public readonly IntegralVector4<T> WZYZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(3L, 2L, 1L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(3, 2, 1, 2))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(3, 2, 1, 2))) : new IntegralVector4<T>(W, Z, Y, Z)));

    public readonly IntegralVector4<T> XWYZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(0L, 3L, 1L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(0, 3, 1, 2))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(0, 3, 1, 2))) : new IntegralVector4<T>(X, W, Y, Z)));

    public readonly IntegralVector4<T> YWYZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(1L, 3L, 1L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(1, 3, 1, 2))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(1, 3, 1, 2))) : new IntegralVector4<T>(Y, W, Y, Z)));

    public readonly IntegralVector4<T> ZWYZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(2L, 3L, 1L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(2, 3, 1, 2))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(2, 3, 1, 2))) : new IntegralVector4<T>(Z, W, Y, Z)));

    public readonly IntegralVector4<T> WWYZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(3L, 3L, 1L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(3, 3, 1, 2))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(3, 3, 1, 2))) : new IntegralVector4<T>(W, W, Y, Z)));

    public readonly IntegralVector4<T> XXZZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(0L, 0L, 2L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(0, 0, 2, 2))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(0, 0, 2, 2))) : new IntegralVector4<T>(X, X, Z, Z)));

    public readonly IntegralVector4<T> YXZZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(1L, 0L, 2L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(1, 0, 2, 2))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(1, 0, 2, 2))) : new IntegralVector4<T>(Y, X, Z, Z)));

    public readonly IntegralVector4<T> ZXZZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(2L, 0L, 2L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(2, 0, 2, 2))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(2, 0, 2, 2))) : new IntegralVector4<T>(Z, X, Z, Z)));

    public readonly IntegralVector4<T> WXZZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(3L, 0L, 2L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(3, 0, 2, 2))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(3, 0, 2, 2))) : new IntegralVector4<T>(W, X, Z, Z)));

    public readonly IntegralVector4<T> XYZZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(0L, 1L, 2L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(0, 1, 2, 2))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(0, 1, 2, 2))) : new IntegralVector4<T>(X, Y, Z, Z)));

    public readonly IntegralVector4<T> YYZZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(1L, 1L, 2L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(1, 1, 2, 2))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(1, 1, 2, 2))) : new IntegralVector4<T>(Y, Y, Z, Z)));

    public readonly IntegralVector4<T> ZYZZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(2L, 1L, 2L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(2, 1, 2, 2))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(2, 1, 2, 2))) : new IntegralVector4<T>(Z, Y, Z, Z)));

    public readonly IntegralVector4<T> WYZZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(3L, 1L, 2L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(3, 1, 2, 2))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(3, 1, 2, 2))) : new IntegralVector4<T>(W, Y, Z, Z)));

    public readonly IntegralVector4<T> XZZZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(0L, 2L, 2L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(0, 2, 2, 2))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(0, 2, 2, 2))) : new IntegralVector4<T>(X, Z, Z, Z)));

    public readonly IntegralVector4<T> YZZZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(1L, 2L, 2L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(1, 2, 2, 2))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(1, 2, 2, 2))) : new IntegralVector4<T>(Y, Z, Z, Z)));

    public readonly IntegralVector4<T> ZZZZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(SplatZ(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(SplatZ(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(2, 2, 2, 2))) : new IntegralVector4<T>(Z, Z, Z, Z)));

    public readonly IntegralVector4<T> WZZZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(3L, 2L, 2L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(3, 2, 2, 2))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(3, 2, 2, 2))) : new IntegralVector4<T>(W, Z, Z, Z)));

    public readonly IntegralVector4<T> XWZZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(0L, 3L, 2L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(0, 3, 2, 2))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(0, 3, 2, 2))) : new IntegralVector4<T>(X, W, Z, Z)));

    public readonly IntegralVector4<T> YWZZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(1L, 3L, 2L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(1, 3, 2, 2))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(1, 3, 2, 2))) : new IntegralVector4<T>(Y, W, Z, Z)));

    public readonly IntegralVector4<T> ZWZZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(2L, 3L, 2L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(2, 3, 2, 2))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(2, 3, 2, 2))) : new IntegralVector4<T>(Z, W, Z, Z)));

    public readonly IntegralVector4<T> WWZZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(3L, 3L, 2L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(3, 3, 2, 2))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(3, 3, 2, 2))) : new IntegralVector4<T>(W, W, Z, Z)));

    public readonly IntegralVector4<T> XXWZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(0L, 0L, 3L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(0, 0, 3, 2))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(0, 0, 3, 2))) : new IntegralVector4<T>(X, X, W, Z)));

    public readonly IntegralVector4<T> YXWZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(1L, 0L, 3L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(1, 0, 3, 2))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(1, 0, 3, 2))) : new IntegralVector4<T>(Y, X, W, Z)));

    public readonly IntegralVector4<T> ZXWZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(2L, 0L, 3L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(2, 0, 3, 2))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(2, 0, 3, 2))) : new IntegralVector4<T>(Z, X, W, Z)));

    public readonly IntegralVector4<T> WXWZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(3L, 0L, 3L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(3, 0, 3, 2))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(3, 0, 3, 2))) : new IntegralVector4<T>(W, X, W, Z)));

    public readonly IntegralVector4<T> XYWZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(0L, 1L, 3L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(0, 1, 3, 2))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(0, 1, 3, 2))) : new IntegralVector4<T>(X, Y, W, Z)));

    public readonly IntegralVector4<T> YYWZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(1L, 1L, 3L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(1, 1, 3, 2))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(1, 1, 3, 2))) : new IntegralVector4<T>(Y, Y, W, Z)));

    public readonly IntegralVector4<T> ZYWZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(2L, 1L, 3L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(2, 1, 3, 2))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(2, 1, 3, 2))) : new IntegralVector4<T>(Z, Y, W, Z)));

    public readonly IntegralVector4<T> WYWZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(3L, 1L, 3L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(3, 1, 3, 2))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(3, 1, 3, 2))) : new IntegralVector4<T>(W, Y, W, Z)));

    public readonly IntegralVector4<T> XZWZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(0L, 2L, 3L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(0, 2, 3, 2))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(0, 2, 3, 2))) : new IntegralVector4<T>(X, Z, W, Z)));

    public readonly IntegralVector4<T> YZWZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(1L, 2L, 3L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(1, 2, 3, 2))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(1, 2, 3, 2))) : new IntegralVector4<T>(Y, Z, W, Z)));

    public readonly IntegralVector4<T> ZZWZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(2L, 2L, 3L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(2, 2, 3, 2))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(2, 2, 3, 2))) : new IntegralVector4<T>(Z, Z, W, Z)));

    public readonly IntegralVector4<T> WZWZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(3L, 2L, 3L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(3, 2, 3, 2))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(3, 2, 3, 2))) : new IntegralVector4<T>(W, Z, W, Z)));

    public readonly IntegralVector4<T> XWWZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(0L, 3L, 3L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(0, 3, 3, 2))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(0, 3, 3, 2))) : new IntegralVector4<T>(X, W, W, Z)));

    public readonly IntegralVector4<T> YWWZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(1L, 3L, 3L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(1, 3, 3, 2))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(1, 3, 3, 2))) : new IntegralVector4<T>(Y, W, W, Z)));

    public readonly IntegralVector4<T> ZWWZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(2L, 3L, 3L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(2, 3, 3, 2))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(2, 3, 3, 2))) : new IntegralVector4<T>(Z, W, W, Z)));

    public readonly IntegralVector4<T> WWWZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(3L, 3L, 3L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(3, 3, 3, 2))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(3, 3, 3, 2))) : new IntegralVector4<T>(W, W, W, Z)));

    public readonly IntegralVector4<T> XXXW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(0L, 0L, 0L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(0, 0, 0, 3))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(0, 0, 0, 3))) : new IntegralVector4<T>(X, X, X, W)));

    public readonly IntegralVector4<T> YXXW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(1L, 0L, 0L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(1, 0, 0, 3))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(1, 0, 0, 3))) : new IntegralVector4<T>(Y, X, X, W)));

    public readonly IntegralVector4<T> ZXXW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(2L, 0L, 0L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(2, 0, 0, 3))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(2, 0, 0, 3))) : new IntegralVector4<T>(Z, X, X, W)));

    public readonly IntegralVector4<T> WXXW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(3L, 0L, 0L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(3, 0, 0, 3))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(3, 0, 0, 3))) : new IntegralVector4<T>(W, X, X, W)));

    public readonly IntegralVector4<T> XYXW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(0L, 1L, 0L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(0, 1, 0, 3))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(0, 1, 0, 3))) : new IntegralVector4<T>(X, Y, X, W)));

    public readonly IntegralVector4<T> YYXW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(1L, 1L, 0L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(1, 1, 0, 3))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(1, 1, 0, 3))) : new IntegralVector4<T>(Y, Y, X, W)));

    public readonly IntegralVector4<T> ZYXW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(2L, 1L, 0L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(2, 1, 0, 3))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(2, 1, 0, 3))) : new IntegralVector4<T>(Z, Y, X, W)));

    public readonly IntegralVector4<T> WYXW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(3L, 1L, 0L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(3, 1, 0, 3))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(3, 1, 0, 3))) : new IntegralVector4<T>(W, Y, X, W)));

    public readonly IntegralVector4<T> XZXW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(0L, 2L, 0L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(0, 2, 0, 3))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(0, 2, 0, 3))) : new IntegralVector4<T>(X, Z, X, W)));

    public readonly IntegralVector4<T> YZXW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(1L, 2L, 0L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(1, 2, 0, 3))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(1, 2, 0, 3))) : new IntegralVector4<T>(Y, Z, X, W)));

    public readonly IntegralVector4<T> ZZXW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(2L, 2L, 0L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(2, 2, 0, 3))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(2, 2, 0, 3))) : new IntegralVector4<T>(Z, Z, X, W)));

    public readonly IntegralVector4<T> WZXW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(3L, 2L, 0L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(3, 2, 0, 3))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(3, 2, 0, 3))) : new IntegralVector4<T>(W, Z, X, W)));

    public readonly IntegralVector4<T> XWXW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(0L, 3L, 0L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(0, 3, 0, 3))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(0, 3, 0, 3))) : new IntegralVector4<T>(X, W, X, W)));

    public readonly IntegralVector4<T> YWXW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(1L, 3L, 0L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(1, 3, 0, 3))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(1, 3, 0, 3))) : new IntegralVector4<T>(Y, W, X, W)));

    public readonly IntegralVector4<T> ZWXW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(2L, 3L, 0L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(2, 3, 0, 3))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(2, 3, 0, 3))) : new IntegralVector4<T>(Z, W, X, W)));

    public readonly IntegralVector4<T> WWXW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(3L, 3L, 0L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(3, 3, 0, 3))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(3, 3, 0, 3))) : new IntegralVector4<T>(W, W, X, W)));

    public readonly IntegralVector4<T> XXYW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(0L, 0L, 1L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(0, 0, 1, 3))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(0, 0, 1, 3))) : new IntegralVector4<T>(X, X, Y, W)));

    public readonly IntegralVector4<T> YXYW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(1L, 0L, 1L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(1, 0, 1, 3))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(1, 0, 1, 3))) : new IntegralVector4<T>(Y, X, Y, W)));

    public readonly IntegralVector4<T> ZXYW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(2L, 0L, 1L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(2, 0, 1, 3))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(2, 0, 1, 3))) : new IntegralVector4<T>(Z, X, Y, W)));

    public readonly IntegralVector4<T> WXYW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(3L, 0L, 1L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(3, 0, 1, 3))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(3, 0, 1, 3))) : new IntegralVector4<T>(W, X, Y, W)));

    public readonly IntegralVector4<T> XYYW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(0L, 1L, 1L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(0, 1, 1, 3))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(0, 1, 1, 3))) : new IntegralVector4<T>(X, Y, Y, W)));

    public readonly IntegralVector4<T> YYYW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(1L, 1L, 1L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(1, 1, 1, 3))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(1, 1, 1, 3))) : new IntegralVector4<T>(Y, Y, Y, W)));

    public readonly IntegralVector4<T> ZYYW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(2L, 1L, 1L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(2, 1, 1, 3))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(2, 1, 1, 3))) : new IntegralVector4<T>(Z, Y, Y, W)));

    public readonly IntegralVector4<T> WYYW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(3L, 1L, 1L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(3, 1, 1, 3))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(3, 1, 1, 3))) : new IntegralVector4<T>(W, Y, Y, W)));

    public readonly IntegralVector4<T> XZYW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(0L, 2L, 1L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(0, 2, 1, 3))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(0, 2, 1, 3))) : new IntegralVector4<T>(X, Z, Y, W)));

    public readonly IntegralVector4<T> YZYW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(1L, 2L, 1L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(1, 2, 1, 3))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(1, 2, 1, 3))) : new IntegralVector4<T>(Y, Z, Y, W)));

    public readonly IntegralVector4<T> ZZYW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(2L, 2L, 1L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(2, 2, 1, 3))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(2, 2, 1, 3))) : new IntegralVector4<T>(Z, Z, Y, W)));

    public readonly IntegralVector4<T> WZYW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(3L, 2L, 1L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(3, 2, 1, 3))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(3, 2, 1, 3))) : new IntegralVector4<T>(W, Z, Y, W)));

    public readonly IntegralVector4<T> XWYW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(0L, 3L, 1L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(0, 3, 1, 3))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(0, 3, 1, 3))) : new IntegralVector4<T>(X, W, Y, W)));

    public readonly IntegralVector4<T> YWYW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(1L, 3L, 1L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(1, 3, 1, 3))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(1, 3, 1, 3))) : new IntegralVector4<T>(Y, W, Y, W)));

    public readonly IntegralVector4<T> ZWYW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(2L, 3L, 1L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(2, 3, 1, 3))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(2, 3, 1, 3))) : new IntegralVector4<T>(Z, W, Y, W)));

    public readonly IntegralVector4<T> WWYW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(3L, 3L, 1L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(3, 3, 1, 3))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(3, 3, 1, 3))) : new IntegralVector4<T>(W, W, Y, W)));

    public readonly IntegralVector4<T> XXZW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(0L, 0L, 2L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(0, 0, 2, 3))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(0, 0, 2, 3))) : new IntegralVector4<T>(X, X, Z, W)));

    public readonly IntegralVector4<T> YXZW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(1L, 0L, 2L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(1, 0, 2, 3))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(1, 0, 2, 3))) : new IntegralVector4<T>(Y, X, Z, W)));

    public readonly IntegralVector4<T> ZXZW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(2L, 0L, 2L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(2, 0, 2, 3))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(2, 0, 2, 3))) : new IntegralVector4<T>(Z, X, Z, W)));

    public readonly IntegralVector4<T> WXZW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(3L, 0L, 2L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(3, 0, 2, 3))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(3, 0, 2, 3))) : new IntegralVector4<T>(W, X, Z, W)));

    public readonly IntegralVector4<T> XYZW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(0L, 1L, 2L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(0, 1, 2, 3))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(0, 1, 2, 3))) : new IntegralVector4<T>(X, Y, Z, W)));

    public readonly IntegralVector4<T> YYZW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(1L, 1L, 2L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(1, 1, 2, 3))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(1, 1, 2, 3))) : new IntegralVector4<T>(Y, Y, Z, W)));

    public readonly IntegralVector4<T> ZYZW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(2L, 1L, 2L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(2, 1, 2, 3))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(2, 1, 2, 3))) : new IntegralVector4<T>(Z, Y, Z, W)));

    public readonly IntegralVector4<T> WYZW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(3L, 1L, 2L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(3, 1, 2, 3))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(3, 1, 2, 3))) : new IntegralVector4<T>(W, Y, Z, W)));

    public readonly IntegralVector4<T> XZZW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(0L, 2L, 2L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(0, 2, 2, 3))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(0, 2, 2, 3))) : new IntegralVector4<T>(X, Z, Z, W)));

    public readonly IntegralVector4<T> YZZW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(1L, 2L, 2L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(1, 2, 2, 3))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(1, 2, 2, 3))) : new IntegralVector4<T>(Y, Z, Z, W)));

    public readonly IntegralVector4<T> ZZZW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(2L, 2L, 2L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(2, 2, 2, 3))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(2, 2, 2, 3))) : new IntegralVector4<T>(Z, Z, Z, W)));

    public readonly IntegralVector4<T> WZZW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(3L, 2L, 2L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(3, 2, 2, 3))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(3, 2, 2, 3))) : new IntegralVector4<T>(W, Z, Z, W)));

    public readonly IntegralVector4<T> XWZW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(0L, 3L, 2L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(0, 3, 2, 3))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(0, 3, 2, 3))) : new IntegralVector4<T>(X, W, Z, W)));

    public readonly IntegralVector4<T> YWZW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(1L, 3L, 2L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(1, 3, 2, 3))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(1, 3, 2, 3))) : new IntegralVector4<T>(Y, W, Z, W)));

    public readonly IntegralVector4<T> ZWZW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(2L, 3L, 2L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(2, 3, 2, 3))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(2, 3, 2, 3))) : new IntegralVector4<T>(Z, W, Z, W)));

    public readonly IntegralVector4<T> WWZW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(3L, 3L, 2L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(3, 3, 2, 3))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(3, 3, 2, 3))) : new IntegralVector4<T>(W, W, Z, W)));

    public readonly IntegralVector4<T> XXWW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(0L, 0L, 3L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(0, 0, 3, 3))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(0, 0, 3, 3))) : new IntegralVector4<T>(X, X, W, W)));

    public readonly IntegralVector4<T> YXWW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(1L, 0L, 3L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(1, 0, 3, 3))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(1, 0, 3, 3))) : new IntegralVector4<T>(Y, X, W, W)));

    public readonly IntegralVector4<T> ZXWW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(2L, 0L, 3L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(2, 0, 3, 3))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(2, 0, 3, 3))) : new IntegralVector4<T>(Z, X, W, W)));

    public readonly IntegralVector4<T> WXWW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(3L, 0L, 3L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(3, 0, 3, 3))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(3, 0, 3, 3))) : new IntegralVector4<T>(W, X, W, W)));

    public readonly IntegralVector4<T> XYWW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(0L, 1L, 3L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(0, 1, 3, 3))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(0, 1, 3, 3))) : new IntegralVector4<T>(X, Y, W, W)));

    public readonly IntegralVector4<T> YYWW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(1L, 1L, 3L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(1, 1, 3, 3))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(1, 1, 3, 3))) : new IntegralVector4<T>(Y, Y, W, W)));

    public readonly IntegralVector4<T> ZYWW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(2L, 1L, 3L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(2, 1, 3, 3))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(2, 1, 3, 3))) : new IntegralVector4<T>(Z, Y, W, W)));

    public readonly IntegralVector4<T> WYWW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(3L, 1L, 3L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(3, 1, 3, 3))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(3, 1, 3, 3))) : new IntegralVector4<T>(W, Y, W, W)));

    public readonly IntegralVector4<T> XZWW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(0L, 2L, 3L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(0, 2, 3, 3))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(0, 2, 3, 3))) : new IntegralVector4<T>(X, Z, W, W)));

    public readonly IntegralVector4<T> YZWW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(1L, 2L, 3L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(1, 2, 3, 3))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(1, 2, 3, 3))) : new IntegralVector4<T>(Y, Z, W, W)));

    public readonly IntegralVector4<T> ZZWW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(2L, 2L, 3L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(2, 2, 3, 3))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(2, 2, 3, 3))) : new IntegralVector4<T>(Z, Z, W, W)));

    public readonly IntegralVector4<T> WZWW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(3L, 2L, 3L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(3, 2, 3, 3))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(3, 2, 3, 3))) : new IntegralVector4<T>(W, Z, W, W)));

    public readonly IntegralVector4<T> XWWW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(0L, 3L, 3L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(0, 3, 3, 3))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(0, 3, 3, 3))) : new IntegralVector4<T>(X, W, W, W)));

    public readonly IntegralVector4<T> YWWW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(1L, 3L, 3L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(1, 3, 3, 3))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(1, 3, 3, 3))) : new IntegralVector4<T>(Y, W, W, W)));

    public readonly IntegralVector4<T> ZWWW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this), Vector256.Create(2L, 3L, 3L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this), Vector128.Create(2, 3, 3, 3))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(2, 3, 3, 3))) : new IntegralVector4<T>(Z, W, W, W)));

    public readonly IntegralVector4<T> WWWW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && sizeof(T) == 8 ? Unsafe.BitCast<Vector256<long>, IntegralVector4<T>>(SplatW(Unsafe.BitCast<IntegralVector4<T>, Vector256<long>>(this))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && sizeof(T) == 4 ? Unsafe.BitCast<Vector128<int>, IntegralVector4<T>>(SplatW(Unsafe.BitCast<IntegralVector4<T>, Vector128<int>>(this))) : (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && sizeof(T) == 2 ? Unsafe.BitCast<Vector64<short>, IntegralVector4<T>>(Vector64.ShuffleNative(Unsafe.BitCast<IntegralVector4<T>, Vector64<short>>(this), Vector64.Create(3, 3, 3, 3))) : new IntegralVector4<T>(W, W, W, W)));

    #endregion IntegralVector4 Swizzles

    #endregion Swizzling
    }
