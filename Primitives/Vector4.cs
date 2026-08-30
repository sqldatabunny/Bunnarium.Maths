using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using static Bunnarium.Tools.Utilities.SIMD;

namespace Bunnarium.Maths.Primitives;

/// <summary> A container for four floating-point values, X, Y, Z, and W. Useful for a variety of purposes, a vector may represent a coordinate, a three-part value, an offset, or a classical vector with a direction and a <see cref="Magnitude">magnitude</see>.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
[DebuggerDisplay("{ToString(), nq}")]
public struct Vector4<T> : IFloatingPointVector<Vector4<T>, T>
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

    /// <inheritdoc
    /// cref="DocStrings.VectorComponentW"/>
    public T W { readonly get; set; }

    #region Constructors & Factories

    public Vector4(T x, T y, T z, T w) {
        X = x;
        Y = y;
        Z = z;
        W = w;
        }

    /// <param name="xyz"> The vector's X, Y, and Z components.</param>
    /// <param name="w"> The vector's W component.</param>
    public Vector4(Vector3<T> xyz, T w) {
        X = xyz.X;
        Y = xyz.Y;
        Z = xyz.Z;
        W = w;
        }

    /// <param name="xy"> The vector's X and Y components.</param>
    /// <param name="z"> The vector's Z component.</param>
    /// <param name="w"> The vector's W component.</param>
    public Vector4(Vector2<T> xy, T z, T w) {
        X = xy.X;
        Y = xy.Y;
        Z = z;
        W = w;
        }

    /// <inheritdoc
    /// cref="IVector{TVector, T}.Create(T)"/>
    public Vector4(T xyzw) {
        X = Y = Z = W = xyzw;
        }

    public static Vector4<T> Create(T value) {
        return new(value, value, value, value);
        }

    #endregion Constructors & Factories

    #region Constants

    public static Vector4<T> Down { get; } = new(T.Zero, T.NegativeOne, T.Zero, T.Zero);

    public static Vector4<T> Left { get; } = new(T.NegativeOne, T.Zero, T.Zero, T.Zero);
    public static Vector4<T> MaxValue { get; } = new(T.MaxValue, T.MaxValue, T.MaxValue, T.MaxValue);
    public static Vector4<T> MinValue { get; } = new(T.MinValue, T.MinValue, T.MinValue, T.MinValue);
    public static Vector4<T> One { get; } = new(T.One);
    public static Vector4<T> Right { get; } = new(T.One, T.Zero, T.Zero, T.Zero);
    public static Vector4<T> RootN { get; } = new(GenericNumbers<T>.FromDouble(0.5)); // OneOverRootFour
    public static Vector4<T> Up { get; } = new(T.Zero, T.One, T.Zero, T.Zero);
    public static Vector4<T> Zero { get; } = new(T.Zero);

#if MATRIX_LEFTHANDED_COORDINATE_SYSTEM
    public static Vector4<T> Forward { get; } = new(T.Zero, T.Zero, T.One, T.Zero);
    public static Vector4<T> Backward { get; } = new(T.Zero, T.Zero, T.NegativeOne, T.Zero);
#elif MATRIX_RIGHTHANDED_COORDINATE_SYSTEM
    public static Vector4<T> Forward { get; } = new(T.Zero, T.Zero, T.NegativeOne, T.Zero);
    public static Vector4<T> Backward { get; } = new(T.Zero, T.Zero, T.One, T.Zero);
#else
    public static Vector4<T> Forward => throw new ApplicationException(Matrix.Docs.BaseMessage);
    public static Vector4<T> Backward => throw new ApplicationException(Matrix.Docs.BaseMessage);
#endif

    #endregion Constants

    #region Absolute Values

    public static Vector4<T> Abs(in Vector4<T> vector) {
        return new(T.Abs(vector.X), T.Abs(vector.Y), T.Abs(vector.Z), T.Abs(vector.W));
        }

    public readonly Vector4<T> Abs() {
        return new(T.Abs(X), T.Abs(Y), T.Abs(Z), T.Abs(W));
        }

    #endregion Absolute Values

    #region Conversions

    /// <returns> A <see cref="Vector2{T}"/> populated by this vector's X and Y values.
    /// </returns>
    public readonly Vector2<T> ToVector2 => new(X, Y);

    /// <returns> A <see cref="Vector3{T}"/> populated by this vector's X, Y, and Z values.
    /// </returns>
    public readonly Vector3<T> ToVector3 => new(X, Y, Z);

    public static Span<T> ToSpan(ref Vector4<T> vector) {
        return MemoryMarshal.CreateSpan(ref Unsafe.As<Vector4<T>, T>(ref vector), 4);
        }

    #endregion Conversions

    #region Dot

    public static T Dot(Vector4<T> a, Vector4<T> b) {
        return a.X * b.X + a.Y * b.Y + a.Z * b.Z + a.W * b.W;
        }

    public readonly T Dot(Vector4<T> other) {
        return X * other.X + Y * other.Y + Z * other.Z + W * other.W;
        }

    #endregion Dot

    #region Floor & Ceiling

    public static void Ceiling(ref Vector4<T> vector) {
        vector.X = T.Ceiling(vector.X);
        vector.Y = T.Ceiling(vector.Y);
        vector.Z = T.Ceiling(vector.Z);
        vector.W = T.Ceiling(vector.W);
        }

    public static Vector4<T> Ceiling(Vector4<T> vector) {
        return new(T.Ceiling(vector.X), T.Ceiling(vector.Y), T.Ceiling(vector.Z), T.Ceiling(vector.W));
        }

    public static void Floor(ref Vector4<T> vector) {
        vector.X = T.Floor(vector.X);
        vector.Y = T.Floor(vector.Y);
        vector.Z = T.Floor(vector.Z);
        vector.W = T.Floor(vector.W);
        }

    public static Vector4<T> Floor(Vector4<T> vector) {
        return new(T.Floor(vector.X), T.Floor(vector.Y), T.Floor(vector.Z), T.Floor(vector.W));
        }

    public readonly Vector4<T> Ceiling() {
        return new(T.Ceiling(X), T.Ceiling(Y), T.Ceiling(Z), T.Ceiling(W));
        }

    public readonly Vector4<T> Floor() {
        return new(T.Floor(X), T.Floor(Y), T.Floor(Z), T.Floor(W));
        }

    #endregion Floor & Ceiling

    #region Horizontal

    public readonly T AbsoluteAverage => (T.Abs(X) + T.Abs(Y) + T.Abs(Z) + T.Abs(W)) / GenericNumbers<T>.Four;
    public readonly T AbsoluteSum => T.Abs(X) + T.Abs(Y) + T.Abs(Z) + T.Abs(W);
    public readonly T Average => (X + Y + Z + W) / GenericNumbers<T>.Four;
    public readonly T Product => X * Y * Z * W;
    public readonly T Sum => X + Y + Z + W;

    public static T HorizontalAbsoluteAverage(in Vector4<T> vector) {
        return (T.Abs(vector.X) + T.Abs(vector.Y) + T.Abs(vector.Z) + T.Abs(vector.W)) / GenericNumbers<T>.Four;
        }

    public static T HorizontalAbsoluteSum(in Vector4<T> vector) {
        return T.Abs(vector.X) + T.Abs(vector.Y) + T.Abs(vector.Z) + T.Abs(vector.W);
        }

    public static T HorizontalAverage(in Vector4<T> vector) {
        return (vector.X + vector.Y + vector.Z + vector.W) / GenericNumbers<T>.Four;
        }

    public static T HorizontalProduct(in Vector4<T> vector) {
        return vector.X * vector.Y * vector.Z * vector.W;
        }

    public static T HorizontalSum(in Vector4<T> vector) {
        return vector.X + vector.Y + vector.Z + vector.W;
        }

    #endregion Horizontal

    #region Lerp

    public static Vector4<T> Lerp(Vector4<T> from, Vector4<T> to, T amount) {
        return from * (T.One - amount) + (to * amount);
        }

    public readonly Vector4<T> Lerp(Vector4<T> to, T amount) {
        return this * (T.One - amount) + (to * amount);
        }

    #endregion Lerp

    #region Lexicographical Ordering

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool LexicographicallyPrecedes(Vector4<T> left, Vector4<T> right) {
        return (left.X < right.X)
            || (left.X == right.X
                && (left.Y < right.Y
                || (left.Y == right.Y
                    && (left.Z < right.Z
                    || (left.Z == right.Z && left.W < right.W)))));
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool LexicographicallyPrecedes(Vector4<T> other) {
        return (X < other.X)
            || (X == other.X
                && (Y < other.Y
                || (Y == other.Y
                    && (Z < other.Z
                    || (Z == other.Z && W < other.W)))));
        }

    #endregion Lexicographical Ordering

    #region Magnitude

    public readonly T Magnitude => T.Sqrt(MagnitudeSquared);
    public readonly T MagnitudeSquared => X * X + Y * Y + Z * Z + W * W;

    #endregion Magnitude

    #region Min / Max

    public static T HorizontalMax(Vector4<T> vector) {
        return T.Max(T.Max(T.Max(vector.X, vector.Y), vector.Z), vector.W);
        }

    public static T HorizontalMin(Vector4<T> vector) {
        return T.Min(T.Min(T.Min(vector.X, vector.Y), vector.Z), vector.W);
        }

    public static Vector4<T> Max(Vector4<T> left, Vector4<T> right) {
        return new Vector4<T>(
            x: T.Max(left.X, right.X),
            y: T.Max(left.Y, right.Y),
            z: T.Max(left.Z, right.Z),
            w: T.Max(left.W, right.W)
            );
        }

    public static Vector4<T> Min(Vector4<T> left, Vector4<T> right) {
        return new Vector4<T>(
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

    public readonly Vector4<T> Max(Vector4<T> other) {
        return new Vector4<T>(
            x: T.Max(X, other.X),
            y: T.Max(Y, other.Y),
            z: T.Max(Z, other.Z),
            w: T.Max(W, other.W)
            );
        }

    public readonly Vector4<T> Min(Vector4<T> other) {
        return new Vector4<T>(
            x: T.Min(X, other.X),
            y: T.Min(Y, other.Y),
            z: T.Min(Z, other.Z),
            w: T.Min(W, other.W)
            );
        }

    #endregion Min / Max

    #region Negation

    public static Vector4<T> Negate(Vector4<T> vector) {
        return new(-vector.X, -vector.Y, -vector.Z, -vector.W);
        }

    public static void Negate(ref Vector4<T> vector) {
        vector.X = -vector.X;
        vector.Y = -vector.Y;
        vector.Z = -vector.Z;
        vector.W = -vector.W;
        }

    public readonly Vector4<T> Negate() {
        return new(-X, -Y, -Z, -W);
        }

    #endregion Negation

    #region Normalization

    public readonly bool IsNormalized {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => MagnitudeSquared == T.One;
        }

    public static Vector4<T> Normalize(Vector4<T> vector, T magnitude) {
        var mag = T.Sqrt(vector.X * vector.X + vector.Y * vector.Y + vector.Z * vector.Z + vector.W * vector.W);
        var mul = magnitude / mag;
        return vector * mul;
        }

    public static void Normalize(ref Vector4<T> vector, T magnitude) {
        var x = vector.X;
        var y = vector.Y;
        var z = vector.Z;
        var w = vector.W;
        var mag = T.Sqrt(x * x + y * y + z * z + w * w);
        var factor = magnitude / mag;
        vector.X *= factor;
        vector.Y *= factor;
        vector.Z *= factor;
        vector.W *= factor;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4<T> Normalize(Vector4<T> vector) {
        return Normalize(vector, T.One);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Normalize(ref Vector4<T> vector) {
        Normalize(ref vector, T.One);
        }

    public readonly Vector4<T> Normalize(T magnitude) {
        var mag = T.Sqrt(X * X + Y * Y + Z * Z + W * W);
        var mul = magnitude / mag;
        return this * mul;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Vector4<T> Normalize() {
        return Normalize(T.One);
        }

    #endregion Normalization

    #region Operators, Equatability & Comparability

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4<T> Add(Vector4<T> left, Vector4<T> right) {
        return left + right;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4<T> ComponentDivide(Vector4<T> left, Vector4<T> right) {
        if (Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double)) {
            return Unsafe.BitCast<Vector256<double>, Vector4<T>>(Unsafe.BitCast<Vector4<T>, Vector256<double>>(left) / Unsafe.BitCast<Vector4<T>, Vector256<double>>(right));
            }
        else if (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float)) {
            return Unsafe.BitCast<Vector128<float>, Vector4<T>>(Unsafe.BitCast<Vector4<T>, Vector128<float>>(left) / Unsafe.BitCast<Vector4<T>, Vector128<float>>(right));
            }
        else {
            return new(left.X / right.X, left.Y / right.Y, left.Z / right.Z, left.W / right.W);
            }
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4<T> ComponentMultiply(Vector4<T> left, Vector4<T> right) {
        if (Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double)) {
            return Unsafe.BitCast<Vector256<double>, Vector4<T>>(Unsafe.BitCast<Vector4<T>, Vector256<double>>(left) * Unsafe.BitCast<Vector4<T>, Vector256<double>>(right));
            }
        else if (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float)) {
            return Unsafe.BitCast<Vector128<float>, Vector4<T>>(Unsafe.BitCast<Vector4<T>, Vector128<float>>(left) * Unsafe.BitCast<Vector4<T>, Vector128<float>>(right));
            }
        else {
            return new(left.X * right.X, left.Y * right.Y, left.Z * right.Z, left.W * right.W);
            }
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Vector4<T>((T X, T Y, T Z, T W) tuple) {
        return new(tuple.X, tuple.Y, tuple.Z, tuple.W);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4<T> operator -(Vector4<T> left, Vector4<T> right) {
        if (Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double)) {
            return Unsafe.BitCast<Vector256<double>, Vector4<T>>(Unsafe.BitCast<Vector4<T>, Vector256<double>>(left) - Unsafe.BitCast<Vector4<T>, Vector256<double>>(right));
            }
        else if (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float)) {
            return Unsafe.BitCast<Vector128<float>, Vector4<T>>(Unsafe.BitCast<Vector4<T>, Vector128<float>>(left) - Unsafe.BitCast<Vector4<T>, Vector128<float>>(right));
            }
        else {
            return new(left.X - right.X, left.Y - right.Y, left.Z - right.Z, left.W - right.W);
            }
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4<T> operator -(Vector4<T> vector, T value) {
        if (Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double)) {
            return Unsafe.BitCast<Vector256<double>, Vector4<T>>(Unsafe.BitCast<Vector4<T>, Vector256<double>>(vector) - Vector256.Create(value).AsDouble());
            }
        else if (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float)) {
            return Unsafe.BitCast<Vector128<float>, Vector4<T>>(Unsafe.BitCast<Vector4<T>, Vector128<float>>(vector) - Vector128.Create(value).AsSingle());
            }
        else {
            return new(vector.X - value, vector.Y - value, vector.Z - value, vector.W - value);
            }
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4<T> operator -(Vector4<T> vector) {
        if (Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double)) {
            return Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.Xor(Unsafe.BitCast<Vector4<T>, Vector256<double>>(vector), Vector256.Create(double.NegativeZero)));
            }
        else if (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float)) {
            return Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.Xor(Unsafe.BitCast<Vector4<T>, Vector128<float>>(vector), Vector128.Create(float.NegativeZero)));
            }
        else {
            return new(-vector.X, -vector.Y, -vector.Z, -vector.W);
            }
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(Vector4<T> left, Vector4<T> right) {
        return (left == right) == false;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4<T> operator *(T value, Vector4<T> vector) {
        return vector * value;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4<T> operator *(Vector4<T> vector, T value) {
        if (Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double)) {
            return Unsafe.BitCast<Vector256<double>, Vector4<T>>(Unsafe.BitCast<Vector4<T>, Vector256<double>>(vector) * Vector256.Create(value).AsDouble());
            }
        else if (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float)) {
            return Unsafe.BitCast<Vector128<float>, Vector4<T>>(Unsafe.BitCast<Vector4<T>, Vector128<float>>(vector) * Vector128.Create(value).AsSingle());
            }
        else {
            return new(vector.X * value, vector.Y * value, vector.Z * value, vector.W * value);
            }
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4<T> operator /(T value, Vector4<T> vector) {
        if (Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double)) {
            return Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.Create(value).AsDouble() / Unsafe.BitCast<Vector4<T>, Vector256<double>>(vector));
            }
        else if (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float)) {
            return Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.Create(value).AsSingle() / Unsafe.BitCast<Vector4<T>, Vector128<float>>(vector));
            }
        else {
            return new(value / vector.X, value / vector.Y, value / vector.Z, value / vector.W);
            }
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4<T> operator /(Vector4<T> vector, T value) {
        if (Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double)) {
            return Unsafe.BitCast<Vector256<double>, Vector4<T>>(Unsafe.BitCast<Vector4<T>, Vector256<double>>(vector) / Vector256.Create(value).AsDouble());
            }
        else if (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float)) {
            return Unsafe.BitCast<Vector128<float>, Vector4<T>>(Unsafe.BitCast<Vector4<T>, Vector128<float>>(vector) / Vector128.Create(value).AsSingle());
            }
        else {
            return new(vector.X / value, vector.Y / value, vector.Z / value, vector.W / value);
            }
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4<T> operator +(Vector4<T> left, Vector4<T> right) {
        if (Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double)) {
            return Unsafe.BitCast<Vector256<double>, Vector4<T>>(Unsafe.BitCast<Vector4<T>, Vector256<double>>(left) + Unsafe.BitCast<Vector4<T>, Vector256<double>>(right));
            }
        else if (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float)) {
            return Unsafe.BitCast<Vector128<float>, Vector4<T>>(Unsafe.BitCast<Vector4<T>, Vector128<float>>(left) + Unsafe.BitCast<Vector4<T>, Vector128<float>>(right));
            }
        else {
            return new(left.X + right.X, left.Y + right.Y, left.Z + right.Z, left.W + right.W);
            }
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4<T> operator +(Vector4<T> vector, T value) {
        if (Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double)) {
            return Unsafe.BitCast<Vector256<double>, Vector4<T>>(Unsafe.BitCast<Vector4<T>, Vector256<double>>(vector) + Vector256.Create(value).AsDouble());
            }
        else if (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float)) {
            return Unsafe.BitCast<Vector128<float>, Vector4<T>>(Unsafe.BitCast<Vector4<T>, Vector128<float>>(vector) + Vector128.Create(value).AsSingle());
            }
        else {
            return new(vector.X + value, vector.Y + value, vector.Z + value, vector.W + value);
            }
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <(in Vector4<T> left, in Vector4<T> right) {
        if (Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double)) {
            return Vector256.LessThanAll(Unsafe.BitCast<Vector4<T>, Vector256<double>>(left), Unsafe.BitCast<Vector4<T>, Vector256<double>>(right));
            }
        else if (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float)) {
            return Vector128.LessThanAll(Unsafe.BitCast<Vector4<T>, Vector128<float>>(left), Unsafe.BitCast<Vector4<T>, Vector128<float>>(right));
            }
        else {
            return left.X < right.X
                && left.Y < right.Y
                && left.Z < right.Z
                && left.W < right.W;
            }
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <=(in Vector4<T> left, in Vector4<T> right) {
        if (Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double)) {
            return Vector256.LessThanOrEqualAll(Unsafe.BitCast<Vector4<T>, Vector256<double>>(left), Unsafe.BitCast<Vector4<T>, Vector256<double>>(right));
            }
        else if (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float)) {
            return Vector128.LessThanOrEqualAll(Unsafe.BitCast<Vector4<T>, Vector128<float>>(left), Unsafe.BitCast<Vector4<T>, Vector128<float>>(right));
            }
        else {
            return left.X <= right.X
                && left.Y <= right.Y
                && left.Z <= right.Z
                && left.W <= right.W;
            }
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(Vector4<T> left, Vector4<T> right) {
        if (Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double)) {
            return Vector256.EqualsAll(Unsafe.BitCast<Vector4<T>, Vector256<double>>(left), Unsafe.BitCast<Vector4<T>, Vector256<double>>(right));
            }
        else if (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float)) {
            return Vector128.EqualsAll(Unsafe.BitCast<Vector4<T>, Vector128<float>>(left), Unsafe.BitCast<Vector4<T>, Vector128<float>>(right));
            }
        else {
            return left.X == right.X
                && left.Y == right.Y
                && left.Z == right.Z
                && left.W == right.W;
            }
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >(in Vector4<T> left, in Vector4<T> right) {
        if (Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double)) {
            return Vector256.GreaterThanAll(Unsafe.BitCast<Vector4<T>, Vector256<double>>(left), Unsafe.BitCast<Vector4<T>, Vector256<double>>(right));
            }
        else if (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float)) {
            return Vector128.GreaterThanAll(Unsafe.BitCast<Vector4<T>, Vector128<float>>(left), Unsafe.BitCast<Vector4<T>, Vector128<float>>(right));
            }
        else {
            return left.X > right.X
                && left.Y > right.Y
                && left.Z > right.Z
                && left.W > right.W;
            }
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >=(in Vector4<T> left, in Vector4<T> right) {
        if (Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double)) {
            return Vector256.GreaterThanOrEqualAll(Unsafe.BitCast<Vector4<T>, Vector256<double>>(left), Unsafe.BitCast<Vector4<T>, Vector256<double>>(right));
            }
        else if (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float)) {
            return Vector128.GreaterThanOrEqualAll(Unsafe.BitCast<Vector4<T>, Vector128<float>>(left), Unsafe.BitCast<Vector4<T>, Vector128<float>>(right));
            }
        else {
            return left.X >= right.X
                && left.Y >= right.Y
                && left.Z >= right.Z
                && left.W >= right.W;
            }
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4<T> Subtract(Vector4<T> left, Vector4<T> right) {
        return left - right;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Vector4<T> Add(Vector4<T> other) {
        return this + other;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Vector4<T> ComponentDivide(Vector4<T> other) {
        return ComponentDivide(this, other);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Vector4<T> ComponentMultiply(Vector4<T> other) {
        return ComponentMultiply(this, other);
        }

    public readonly void Deconstruct(out T x, out T y, out T z, out T w) {
        x = this.X; y = this.Y; z = this.Z; w = this.W;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool Equals(Vector4<T> other) {
        return X == other.X
            && Y == other.Y
            && Z == other.Z
            && W == other.W;
        }

    public override readonly bool Equals(object? obj) {
        return obj is Vector4<T> vector && Equals(vector);
        }

    public override readonly int GetHashCode() {
        return HashCode.Combine(X, Y, Z, W);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Vector4<T> Subtract(Vector4<T> other) {
        return this - other;
        }

    #endregion Operators, Equatability & Comparability

    #region Orthogonality & Orthonormality

    public static bool IsOrthogonalWith(Vector4<T> left, Vector4<T> right) {
        return left.Dot(right) == T.Zero;
        }

    public static bool IsOrthonormalWith(Vector4<T> left, Vector4<T> right) {
        return left.Dot(right) == T.Zero && left.MagnitudeSquared == T.One && right.MagnitudeSquared == T.One;
        }

    public static Vector4<T> Orthogonal(Vector4<T> vector) {
        if (Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double)) {
            var v = Unsafe.BitCast<Vector4<T>, Vector256<double>>(vector);
            var basis = LeastDominantMask(v);
            var ret = basis - v * (Vector256.Sum(basis * v) / Vector256.Sum(v * v));
            ret = Vector256.ConditionalSelect(Vector256.IsNaN(ret), Vector256<double>.Zero, ret);
            return Unsafe.BitCast<Vector256<double>, Vector4<T>>(ret);
            }
        else if (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float)) {
            var v = Unsafe.BitCast<Vector4<T>, Vector128<float>>(vector);
            var basis = LeastDominantMask(v);
            var ret = basis - v * (Vector128.Sum(basis * v) / Vector128.Sum(v * v));
            ret = Vector128.ConditionalSelect(Vector128.IsNaN(ret), Vector128<float>.Zero, ret);
            return Unsafe.BitCast<Vector128<float>, Vector4<T>>(ret);
            }
        else {
            var absX = T.Abs(vector.X);
            var absY = T.Abs(vector.Y);
            var absZ = T.Abs(vector.Z);
            var absW = T.Abs(vector.W);

            Vector4<T> basis;
            if (absX <= absY && absX <= absZ && absX <= absW)
                basis = new(T.One, T.Zero, T.Zero, T.Zero);
            else if (absY <= absZ && absY <= absW)
                basis = new(T.Zero, T.One, T.Zero, T.Zero);
            else if (absZ <= absW)
                basis = new(T.Zero, T.Zero, T.One, T.Zero);
            else
                basis = new(T.Zero, T.Zero, T.Zero, T.One);

            return vector.MagnitudeSquared != T.Zero
                ? basis - vector * (Dot(basis, vector) / vector.MagnitudeSquared) // gram-schmidt method
                : Vector4<T>.Zero;
            }
        }

    public readonly bool IsOrthogonalWith(Vector4<T> other) {
        return Dot(other) == T.Zero;
        }

    public readonly bool IsOrthonormalWith(Vector4<T> other) {
        return Dot(other) == T.Zero && MagnitudeSquared == T.One && other.MagnitudeSquared == T.One;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Vector4<T> Orthogonal() {
        return Orthogonal(this);
        }

    #endregion Orthogonality & Orthonormality

    #region Rounding

    public static Vector4<T> Round(Vector4<T> vector) {
        return new(T.Round(vector.X), T.Round(vector.Y), T.Round(vector.Z), T.Round(vector.W));
        }

    public static Vector4<T> Round(Vector4<T> vector, byte digits) {
        return new(T.Round(vector.X, digits), T.Round(vector.Y, digits), T.Round(vector.Z, digits), T.Round(vector.W, digits));
        }

    public static void Round(ref Vector4<T> vector) {
        vector.X = T.Round(vector.X);
        vector.Y = T.Round(vector.Y);
        vector.Z = T.Round(vector.Z);
        vector.W = T.Round(vector.W);
        }

    public static void Round(ref Vector4<T> vector, byte digits) {
        vector.X = T.Round(vector.X, digits);
        vector.Y = T.Round(vector.Y, digits);
        vector.Z = T.Round(vector.Z, digits);
        vector.W = T.Round(vector.W, digits);
        }

    public readonly Vector4<T> Round() {
        return new(T.Round(X), T.Round(Y), T.Round(Z), T.Round(W));
        }

    public readonly Vector4<T> Round(byte digits) {
        return new(T.Round(X, digits), T.Round(Y, digits), T.Round(Z, digits), T.Round(W, digits));
        }

    #endregion Rounding

    #region Scale

    public static Vector4<T> Scale(Vector4<T> vector, T factor) {
        return vector * factor;
        }

    public static void Scale(ref Vector4<T> vector, T factor) {
        vector.X *= factor;
        vector.Y *= factor;
        vector.Z *= factor;
        vector.W *= factor;
        }

    public readonly Vector4<T> Scale(T factor) {
        return this * factor;
        }

    #endregion Scale

    #region Sign

    public static Vector4<T> Sign(Vector4<T> vector) {
        return new(
            x: vector.X > T.Zero ? T.One : vector.X < T.Zero ? T.NegativeOne : T.Zero,
            y: vector.Y > T.Zero ? T.One : vector.Y < T.Zero ? T.NegativeOne : T.Zero,
            z: vector.Z > T.Zero ? T.One : vector.Z < T.Zero ? T.NegativeOne : T.Zero,
            w: vector.W > T.Zero ? T.One : vector.W < T.Zero ? T.NegativeOne : T.Zero
            );
        }

    public readonly Vector4<T> Sign() {
        return new(
            x: X > T.Zero ? T.One : X < T.Zero ? T.NegativeOne : T.Zero,
            y: Y > T.Zero ? T.One : Y < T.Zero ? T.NegativeOne : T.Zero,
            z: Z > T.Zero ? T.One : Z < T.Zero ? T.NegativeOne : T.Zero,
            w: W > T.Zero ? T.One : W < T.Zero ? T.NegativeOne : T.Zero
            );
        }

    #endregion Sign

    #region SizeOf and Intrinsics

    public static int Length { get; } = 4;

    public static unsafe int SizeOf { get; } = sizeof(T) * Length;

    #endregion SizeOf and Intrinsics

    #region Step

    public static Vector4<T> Step(Vector4<T> left, Vector4<T> right) {
        return left + Sign(right - left);
        }

    public readonly Vector4<T> Step(Vector4<T> other) {
        return this + Sign(other - this);
        }

    #endregion Step

    #region Strings

    public override readonly string ToString() {
        return $"[{X}, {Y}, {Z}, {W}]";
        }

    public readonly string ToString(byte digits, int integerLength, int paddingLength) {
        return $"[{X.Stringify(digits, integerLength, paddingLength)}, {Y.Stringify(digits, integerLength, paddingLength)}, {Z.Stringify(digits, integerLength, paddingLength)}, {W.Stringify(digits, integerLength, paddingLength)}]";
        }

    #endregion Strings

    #region Swizzling

    #region Vector2 Swizzles

    public readonly Vector2<T> XX => new(X, X);
    public readonly Vector2<T> YX => new(Y, X);
    public readonly Vector2<T> ZX => new(Z, X);
    public readonly Vector2<T> WX => new(W, X);
    public readonly Vector2<T> XY => new(X, Y);
    public readonly Vector2<T> YY => new(Y, Y);
    public readonly Vector2<T> ZY => new(Z, Y);
    public readonly Vector2<T> WY => new(W, Y);
    public readonly Vector2<T> XZ => new(X, Z);
    public readonly Vector2<T> YZ => new(Y, Z);
    public readonly Vector2<T> ZZ => new(Z, Z);
    public readonly Vector2<T> WZ => new(W, Z);
    public readonly Vector2<T> XW => new(X, W);
    public readonly Vector2<T> YW => new(Y, W);
    public readonly Vector2<T> ZW => new(Z, W);
    public readonly Vector2<T> WW => new(W, W);

    #endregion Vector2 Swizzles

    #region Vector3 Swizzles

    public readonly Vector3<T> XXX => new(X, X, X);
    public readonly Vector3<T> YXX => new(Y, X, X);
    public readonly Vector3<T> ZXX => new(Z, X, X);
    public readonly Vector3<T> WXX => new(W, X, X);
    public readonly Vector3<T> XYX => new(X, Y, X);
    public readonly Vector3<T> YYX => new(Y, Y, X);
    public readonly Vector3<T> ZYX => new(Z, Y, X);
    public readonly Vector3<T> WYX => new(W, Y, X);
    public readonly Vector3<T> XZX => new(X, Z, X);
    public readonly Vector3<T> YZX => new(Y, Z, X);
    public readonly Vector3<T> ZZX => new(Z, Z, X);
    public readonly Vector3<T> WZX => new(W, Z, X);
    public readonly Vector3<T> XWX => new(X, W, X);
    public readonly Vector3<T> YWX => new(Y, W, X);
    public readonly Vector3<T> ZWX => new(Z, W, X);
    public readonly Vector3<T> WWX => new(W, W, X);
    public readonly Vector3<T> XXY => new(X, X, Y);
    public readonly Vector3<T> YXY => new(Y, X, Y);
    public readonly Vector3<T> ZXY => new(Z, X, Y);
    public readonly Vector3<T> WXY => new(W, X, Y);
    public readonly Vector3<T> XYY => new(X, Y, Y);
    public readonly Vector3<T> YYY => new(Y, Y, Y);
    public readonly Vector3<T> ZYY => new(Z, Y, Y);
    public readonly Vector3<T> WYY => new(W, Y, Y);
    public readonly Vector3<T> XZY => new(X, Z, Y);
    public readonly Vector3<T> YZY => new(Y, Z, Y);
    public readonly Vector3<T> ZZY => new(Z, Z, Y);
    public readonly Vector3<T> WZY => new(W, Z, Y);
    public readonly Vector3<T> XWY => new(X, W, Y);
    public readonly Vector3<T> YWY => new(Y, W, Y);
    public readonly Vector3<T> ZWY => new(Z, W, Y);
    public readonly Vector3<T> WWY => new(W, W, Y);
    public readonly Vector3<T> XXZ => new(X, X, Z);
    public readonly Vector3<T> YXZ => new(Y, X, Z);
    public readonly Vector3<T> ZXZ => new(Z, X, Z);
    public readonly Vector3<T> WXZ => new(W, X, Z);
    public readonly Vector3<T> XYZ => new(X, Y, Z);
    public readonly Vector3<T> YYZ => new(Y, Y, Z);
    public readonly Vector3<T> ZYZ => new(Z, Y, Z);
    public readonly Vector3<T> WYZ => new(W, Y, Z);
    public readonly Vector3<T> XZZ => new(X, Z, Z);
    public readonly Vector3<T> YZZ => new(Y, Z, Z);
    public readonly Vector3<T> ZZZ => new(Z, Z, Z);
    public readonly Vector3<T> WZZ => new(W, Z, Z);
    public readonly Vector3<T> XWZ => new(X, W, Z);
    public readonly Vector3<T> YWZ => new(Y, W, Z);
    public readonly Vector3<T> ZWZ => new(Z, W, Z);
    public readonly Vector3<T> WWZ => new(W, W, Z);
    public readonly Vector3<T> XXW => new(X, X, W);
    public readonly Vector3<T> YXW => new(Y, X, W);
    public readonly Vector3<T> ZXW => new(Z, X, W);
    public readonly Vector3<T> WXW => new(W, X, W);
    public readonly Vector3<T> XYW => new(X, Y, W);
    public readonly Vector3<T> YYW => new(Y, Y, W);
    public readonly Vector3<T> ZYW => new(Z, Y, W);
    public readonly Vector3<T> WYW => new(W, Y, W);
    public readonly Vector3<T> XZW => new(X, Z, W);
    public readonly Vector3<T> YZW => new(Y, Z, W);
    public readonly Vector3<T> ZZW => new(Z, Z, W);
    public readonly Vector3<T> WZW => new(W, Z, W);
    public readonly Vector3<T> XWW => new(X, W, W);
    public readonly Vector3<T> YWW => new(Y, W, W);
    public readonly Vector3<T> ZWW => new(Z, W, W);
    public readonly Vector3<T> WWW => new(W, W, W);

    #endregion Vector3 Swizzles

    #region Vector4 Swizzles

    public readonly Vector4<T> XXXX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(SplatX(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(SplatX(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this))) : new Vector4<T>(X, X, X, X));

    public readonly Vector4<T> YXXX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(1L, 0L, 0L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(1, 0, 0, 0))) : new Vector4<T>(Y, X, X, X));

    public readonly Vector4<T> ZXXX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(2L, 0L, 0L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(2, 0, 0, 0))) : new Vector4<T>(Z, X, X, X));

    public readonly Vector4<T> WXXX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(3L, 0L, 0L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(3, 0, 0, 0))) : new Vector4<T>(W, X, X, X));

    public readonly Vector4<T> XYXX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(0L, 1L, 0L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(0, 1, 0, 0))) : new Vector4<T>(X, Y, X, X));

    public readonly Vector4<T> YYXX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(1L, 1L, 0L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(1, 1, 0, 0))) : new Vector4<T>(Y, Y, X, X));

    public readonly Vector4<T> ZYXX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(2L, 1L, 0L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(2, 1, 0, 0))) : new Vector4<T>(Z, Y, X, X));

    public readonly Vector4<T> WYXX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(3L, 1L, 0L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(3, 1, 0, 0))) : new Vector4<T>(W, Y, X, X));

    public readonly Vector4<T> XZXX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(0L, 2L, 0L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(0, 2, 0, 0))) : new Vector4<T>(X, Z, X, X));

    public readonly Vector4<T> YZXX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(1L, 2L, 0L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(1, 2, 0, 0))) : new Vector4<T>(Y, Z, X, X));

    public readonly Vector4<T> ZZXX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(2L, 2L, 0L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(2, 2, 0, 0))) : new Vector4<T>(Z, Z, X, X));

    public readonly Vector4<T> WZXX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(3L, 2L, 0L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(3, 2, 0, 0))) : new Vector4<T>(W, Z, X, X));

    public readonly Vector4<T> XWXX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(0L, 3L, 0L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(0, 3, 0, 0))) : new Vector4<T>(X, W, X, X));

    public readonly Vector4<T> YWXX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(1L, 3L, 0L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(1, 3, 0, 0))) : new Vector4<T>(Y, W, X, X));

    public readonly Vector4<T> ZWXX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(2L, 3L, 0L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(2, 3, 0, 0))) : new Vector4<T>(Z, W, X, X));

    public readonly Vector4<T> WWXX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(3L, 3L, 0L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(3, 3, 0, 0))) : new Vector4<T>(W, W, X, X));

    public readonly Vector4<T> XXYX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(0L, 0L, 1L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(0, 0, 1, 0))) : new Vector4<T>(X, X, Y, X));

    public readonly Vector4<T> YXYX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(1L, 0L, 1L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(1, 0, 1, 0))) : new Vector4<T>(Y, X, Y, X));

    public readonly Vector4<T> ZXYX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(2L, 0L, 1L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(2, 0, 1, 0))) : new Vector4<T>(Z, X, Y, X));

    public readonly Vector4<T> WXYX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(3L, 0L, 1L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(3, 0, 1, 0))) : new Vector4<T>(W, X, Y, X));

    public readonly Vector4<T> XYYX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(0L, 1L, 1L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(0, 1, 1, 0))) : new Vector4<T>(X, Y, Y, X));

    public readonly Vector4<T> YYYX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(1L, 1L, 1L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(1, 1, 1, 0))) : new Vector4<T>(Y, Y, Y, X));

    public readonly Vector4<T> ZYYX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(2L, 1L, 1L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(2, 1, 1, 0))) : new Vector4<T>(Z, Y, Y, X));

    public readonly Vector4<T> WYYX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(3L, 1L, 1L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(3, 1, 1, 0))) : new Vector4<T>(W, Y, Y, X));

    public readonly Vector4<T> XZYX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(0L, 2L, 1L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(0, 2, 1, 0))) : new Vector4<T>(X, Z, Y, X));

    public readonly Vector4<T> YZYX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(1L, 2L, 1L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(1, 2, 1, 0))) : new Vector4<T>(Y, Z, Y, X));

    public readonly Vector4<T> ZZYX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(2L, 2L, 1L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(2, 2, 1, 0))) : new Vector4<T>(Z, Z, Y, X));

    public readonly Vector4<T> WZYX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(3L, 2L, 1L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(3, 2, 1, 0))) : new Vector4<T>(W, Z, Y, X));

    public readonly Vector4<T> XWYX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(0L, 3L, 1L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(0, 3, 1, 0))) : new Vector4<T>(X, W, Y, X));

    public readonly Vector4<T> YWYX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(1L, 3L, 1L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(1, 3, 1, 0))) : new Vector4<T>(Y, W, Y, X));

    public readonly Vector4<T> ZWYX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(2L, 3L, 1L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(2, 3, 1, 0))) : new Vector4<T>(Z, W, Y, X));

    public readonly Vector4<T> WWYX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(3L, 3L, 1L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(3, 3, 1, 0))) : new Vector4<T>(W, W, Y, X));

    public readonly Vector4<T> XXZX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(0L, 0L, 2L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(0, 0, 2, 0))) : new Vector4<T>(X, X, Z, X));

    public readonly Vector4<T> YXZX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(1L, 0L, 2L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(1, 0, 2, 0))) : new Vector4<T>(Y, X, Z, X));

    public readonly Vector4<T> ZXZX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(2L, 0L, 2L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(2, 0, 2, 0))) : new Vector4<T>(Z, X, Z, X));

    public readonly Vector4<T> WXZX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(3L, 0L, 2L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(3, 0, 2, 0))) : new Vector4<T>(W, X, Z, X));

    public readonly Vector4<T> XYZX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(0L, 1L, 2L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(0, 1, 2, 0))) : new Vector4<T>(X, Y, Z, X));

    public readonly Vector4<T> YYZX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(1L, 1L, 2L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(1, 1, 2, 0))) : new Vector4<T>(Y, Y, Z, X));

    public readonly Vector4<T> ZYZX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(2L, 1L, 2L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(2, 1, 2, 0))) : new Vector4<T>(Z, Y, Z, X));

    public readonly Vector4<T> WYZX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(3L, 1L, 2L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(3, 1, 2, 0))) : new Vector4<T>(W, Y, Z, X));

    public readonly Vector4<T> XZZX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(0L, 2L, 2L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(0, 2, 2, 0))) : new Vector4<T>(X, Z, Z, X));

    public readonly Vector4<T> YZZX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(1L, 2L, 2L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(1, 2, 2, 0))) : new Vector4<T>(Y, Z, Z, X));

    public readonly Vector4<T> ZZZX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(2L, 2L, 2L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(2, 2, 2, 0))) : new Vector4<T>(Z, Z, Z, X));

    public readonly Vector4<T> WZZX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(3L, 2L, 2L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(3, 2, 2, 0))) : new Vector4<T>(W, Z, Z, X));

    public readonly Vector4<T> XWZX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(0L, 3L, 2L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(0, 3, 2, 0))) : new Vector4<T>(X, W, Z, X));

    public readonly Vector4<T> YWZX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(1L, 3L, 2L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(1, 3, 2, 0))) : new Vector4<T>(Y, W, Z, X));

    public readonly Vector4<T> ZWZX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(2L, 3L, 2L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(2, 3, 2, 0))) : new Vector4<T>(Z, W, Z, X));

    public readonly Vector4<T> WWZX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(3L, 3L, 2L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(3, 3, 2, 0))) : new Vector4<T>(W, W, Z, X));

    public readonly Vector4<T> XXWX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(0L, 0L, 3L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(0, 0, 3, 0))) : new Vector4<T>(X, X, W, X));

    public readonly Vector4<T> YXWX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(1L, 0L, 3L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(1, 0, 3, 0))) : new Vector4<T>(Y, X, W, X));

    public readonly Vector4<T> ZXWX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(2L, 0L, 3L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(2, 0, 3, 0))) : new Vector4<T>(Z, X, W, X));

    public readonly Vector4<T> WXWX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(3L, 0L, 3L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(3, 0, 3, 0))) : new Vector4<T>(W, X, W, X));

    public readonly Vector4<T> XYWX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(0L, 1L, 3L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(0, 1, 3, 0))) : new Vector4<T>(X, Y, W, X));

    public readonly Vector4<T> YYWX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(1L, 1L, 3L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(1, 1, 3, 0))) : new Vector4<T>(Y, Y, W, X));

    public readonly Vector4<T> ZYWX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(2L, 1L, 3L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(2, 1, 3, 0))) : new Vector4<T>(Z, Y, W, X));

    public readonly Vector4<T> WYWX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(3L, 1L, 3L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(3, 1, 3, 0))) : new Vector4<T>(W, Y, W, X));

    public readonly Vector4<T> XZWX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(0L, 2L, 3L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(0, 2, 3, 0))) : new Vector4<T>(X, Z, W, X));

    public readonly Vector4<T> YZWX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(1L, 2L, 3L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(1, 2, 3, 0))) : new Vector4<T>(Y, Z, W, X));

    public readonly Vector4<T> ZZWX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(2L, 2L, 3L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(2, 2, 3, 0))) : new Vector4<T>(Z, Z, W, X));

    public readonly Vector4<T> WZWX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(3L, 2L, 3L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(3, 2, 3, 0))) : new Vector4<T>(W, Z, W, X));

    public readonly Vector4<T> XWWX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(0L, 3L, 3L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(0, 3, 3, 0))) : new Vector4<T>(X, W, W, X));

    public readonly Vector4<T> YWWX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(1L, 3L, 3L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(1, 3, 3, 0))) : new Vector4<T>(Y, W, W, X));

    public readonly Vector4<T> ZWWX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(2L, 3L, 3L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(2, 3, 3, 0))) : new Vector4<T>(Z, W, W, X));

    public readonly Vector4<T> WWWX => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(3L, 3L, 3L, 0L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(3, 3, 3, 0))) : new Vector4<T>(W, W, W, X));

    public readonly Vector4<T> XXXY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(0L, 0L, 0L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(0, 0, 0, 1))) : new Vector4<T>(X, X, X, Y));

    public readonly Vector4<T> YXXY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(1L, 0L, 0L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(1, 0, 0, 1))) : new Vector4<T>(Y, X, X, Y));

    public readonly Vector4<T> ZXXY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(2L, 0L, 0L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(2, 0, 0, 1))) : new Vector4<T>(Z, X, X, Y));

    public readonly Vector4<T> WXXY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(3L, 0L, 0L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(3, 0, 0, 1))) : new Vector4<T>(W, X, X, Y));

    public readonly Vector4<T> XYXY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(0L, 1L, 0L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(0, 1, 0, 1))) : new Vector4<T>(X, Y, X, Y));

    public readonly Vector4<T> YYXY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(1L, 1L, 0L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(1, 1, 0, 1))) : new Vector4<T>(Y, Y, X, Y));

    public readonly Vector4<T> ZYXY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(2L, 1L, 0L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(2, 1, 0, 1))) : new Vector4<T>(Z, Y, X, Y));

    public readonly Vector4<T> WYXY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(3L, 1L, 0L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(3, 1, 0, 1))) : new Vector4<T>(W, Y, X, Y));

    public readonly Vector4<T> XZXY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(0L, 2L, 0L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(0, 2, 0, 1))) : new Vector4<T>(X, Z, X, Y));

    public readonly Vector4<T> YZXY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(1L, 2L, 0L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(1, 2, 0, 1))) : new Vector4<T>(Y, Z, X, Y));

    public readonly Vector4<T> ZZXY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(2L, 2L, 0L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(2, 2, 0, 1))) : new Vector4<T>(Z, Z, X, Y));

    public readonly Vector4<T> WZXY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(3L, 2L, 0L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(3, 2, 0, 1))) : new Vector4<T>(W, Z, X, Y));

    public readonly Vector4<T> XWXY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(0L, 3L, 0L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(0, 3, 0, 1))) : new Vector4<T>(X, W, X, Y));

    public readonly Vector4<T> YWXY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(1L, 3L, 0L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(1, 3, 0, 1))) : new Vector4<T>(Y, W, X, Y));

    public readonly Vector4<T> ZWXY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(2L, 3L, 0L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(2, 3, 0, 1))) : new Vector4<T>(Z, W, X, Y));

    public readonly Vector4<T> WWXY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(3L, 3L, 0L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(3, 3, 0, 1))) : new Vector4<T>(W, W, X, Y));

    public readonly Vector4<T> XXYY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(0L, 0L, 1L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(0, 0, 1, 1))) : new Vector4<T>(X, X, Y, Y));

    public readonly Vector4<T> YXYY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(1L, 0L, 1L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(1, 0, 1, 1))) : new Vector4<T>(Y, X, Y, Y));

    public readonly Vector4<T> ZXYY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(2L, 0L, 1L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(2, 0, 1, 1))) : new Vector4<T>(Z, X, Y, Y));

    public readonly Vector4<T> WXYY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(3L, 0L, 1L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(3, 0, 1, 1))) : new Vector4<T>(W, X, Y, Y));

    public readonly Vector4<T> XYYY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(0L, 1L, 1L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(0, 1, 1, 1))) : new Vector4<T>(X, Y, Y, Y));

    public readonly Vector4<T> YYYY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(SplatY(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(SplatY(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this))) : new Vector4<T>(Y, Y, Y, Y));

    public readonly Vector4<T> ZYYY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(2L, 1L, 1L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(2, 1, 1, 1))) : new Vector4<T>(Z, Y, Y, Y));

    public readonly Vector4<T> WYYY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(3L, 1L, 1L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(3, 1, 1, 1))) : new Vector4<T>(W, Y, Y, Y));

    public readonly Vector4<T> XZYY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(0L, 2L, 1L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(0, 2, 1, 1))) : new Vector4<T>(X, Z, Y, Y));

    public readonly Vector4<T> YZYY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(1L, 2L, 1L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(1, 2, 1, 1))) : new Vector4<T>(Y, Z, Y, Y));

    public readonly Vector4<T> ZZYY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(2L, 2L, 1L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(2, 2, 1, 1))) : new Vector4<T>(Z, Z, Y, Y));

    public readonly Vector4<T> WZYY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(3L, 2L, 1L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(3, 2, 1, 1))) : new Vector4<T>(W, Z, Y, Y));

    public readonly Vector4<T> XWYY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(0L, 3L, 1L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(0, 3, 1, 1))) : new Vector4<T>(X, W, Y, Y));

    public readonly Vector4<T> YWYY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(1L, 3L, 1L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(1, 3, 1, 1))) : new Vector4<T>(Y, W, Y, Y));

    public readonly Vector4<T> ZWYY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(2L, 3L, 1L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(2, 3, 1, 1))) : new Vector4<T>(Z, W, Y, Y));

    public readonly Vector4<T> WWYY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(3L, 3L, 1L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(3, 3, 1, 1))) : new Vector4<T>(W, W, Y, Y));

    public readonly Vector4<T> XXZY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(0L, 0L, 2L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(0, 0, 2, 1))) : new Vector4<T>(X, X, Z, Y));

    public readonly Vector4<T> YXZY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(1L, 0L, 2L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(1, 0, 2, 1))) : new Vector4<T>(Y, X, Z, Y));

    public readonly Vector4<T> ZXZY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(2L, 0L, 2L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(2, 0, 2, 1))) : new Vector4<T>(Z, X, Z, Y));

    public readonly Vector4<T> WXZY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(3L, 0L, 2L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(3, 0, 2, 1))) : new Vector4<T>(W, X, Z, Y));

    public readonly Vector4<T> XYZY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(0L, 1L, 2L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(0, 1, 2, 1))) : new Vector4<T>(X, Y, Z, Y));

    public readonly Vector4<T> YYZY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(1L, 1L, 2L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(1, 1, 2, 1))) : new Vector4<T>(Y, Y, Z, Y));

    public readonly Vector4<T> ZYZY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(2L, 1L, 2L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(2, 1, 2, 1))) : new Vector4<T>(Z, Y, Z, Y));

    public readonly Vector4<T> WYZY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(3L, 1L, 2L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(3, 1, 2, 1))) : new Vector4<T>(W, Y, Z, Y));

    public readonly Vector4<T> XZZY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(0L, 2L, 2L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(0, 2, 2, 1))) : new Vector4<T>(X, Z, Z, Y));

    public readonly Vector4<T> YZZY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(1L, 2L, 2L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(1, 2, 2, 1))) : new Vector4<T>(Y, Z, Z, Y));

    public readonly Vector4<T> ZZZY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(2L, 2L, 2L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(2, 2, 2, 1))) : new Vector4<T>(Z, Z, Z, Y));

    public readonly Vector4<T> WZZY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(3L, 2L, 2L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(3, 2, 2, 1))) : new Vector4<T>(W, Z, Z, Y));

    public readonly Vector4<T> XWZY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(0L, 3L, 2L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(0, 3, 2, 1))) : new Vector4<T>(X, W, Z, Y));

    public readonly Vector4<T> YWZY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(1L, 3L, 2L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(1, 3, 2, 1))) : new Vector4<T>(Y, W, Z, Y));

    public readonly Vector4<T> ZWZY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(2L, 3L, 2L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(2, 3, 2, 1))) : new Vector4<T>(Z, W, Z, Y));

    public readonly Vector4<T> WWZY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(3L, 3L, 2L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(3, 3, 2, 1))) : new Vector4<T>(W, W, Z, Y));

    public readonly Vector4<T> XXWY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(0L, 0L, 3L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(0, 0, 3, 1))) : new Vector4<T>(X, X, W, Y));

    public readonly Vector4<T> YXWY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(1L, 0L, 3L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(1, 0, 3, 1))) : new Vector4<T>(Y, X, W, Y));

    public readonly Vector4<T> ZXWY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(2L, 0L, 3L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(2, 0, 3, 1))) : new Vector4<T>(Z, X, W, Y));

    public readonly Vector4<T> WXWY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(3L, 0L, 3L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(3, 0, 3, 1))) : new Vector4<T>(W, X, W, Y));

    public readonly Vector4<T> XYWY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(0L, 1L, 3L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(0, 1, 3, 1))) : new Vector4<T>(X, Y, W, Y));

    public readonly Vector4<T> YYWY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(1L, 1L, 3L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(1, 1, 3, 1))) : new Vector4<T>(Y, Y, W, Y));

    public readonly Vector4<T> ZYWY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(2L, 1L, 3L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(2, 1, 3, 1))) : new Vector4<T>(Z, Y, W, Y));

    public readonly Vector4<T> WYWY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(3L, 1L, 3L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(3, 1, 3, 1))) : new Vector4<T>(W, Y, W, Y));

    public readonly Vector4<T> XZWY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(0L, 2L, 3L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(0, 2, 3, 1))) : new Vector4<T>(X, Z, W, Y));

    public readonly Vector4<T> YZWY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(1L, 2L, 3L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(1, 2, 3, 1))) : new Vector4<T>(Y, Z, W, Y));

    public readonly Vector4<T> ZZWY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(2L, 2L, 3L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(2, 2, 3, 1))) : new Vector4<T>(Z, Z, W, Y));

    public readonly Vector4<T> WZWY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(3L, 2L, 3L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(3, 2, 3, 1))) : new Vector4<T>(W, Z, W, Y));

    public readonly Vector4<T> XWWY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(0L, 3L, 3L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(0, 3, 3, 1))) : new Vector4<T>(X, W, W, Y));

    public readonly Vector4<T> YWWY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(1L, 3L, 3L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(1, 3, 3, 1))) : new Vector4<T>(Y, W, W, Y));

    public readonly Vector4<T> ZWWY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(2L, 3L, 3L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(2, 3, 3, 1))) : new Vector4<T>(Z, W, W, Y));

    public readonly Vector4<T> WWWY => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(3L, 3L, 3L, 1L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(3, 3, 3, 1))) : new Vector4<T>(W, W, W, Y));

    public readonly Vector4<T> XXXZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(0L, 0L, 0L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(0, 0, 0, 2))) : new Vector4<T>(X, X, X, Z));

    public readonly Vector4<T> YXXZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(1L, 0L, 0L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(1, 0, 0, 2))) : new Vector4<T>(Y, X, X, Z));

    public readonly Vector4<T> ZXXZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(2L, 0L, 0L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(2, 0, 0, 2))) : new Vector4<T>(Z, X, X, Z));

    public readonly Vector4<T> WXXZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(3L, 0L, 0L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(3, 0, 0, 2))) : new Vector4<T>(W, X, X, Z));

    public readonly Vector4<T> XYXZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(0L, 1L, 0L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(0, 1, 0, 2))) : new Vector4<T>(X, Y, X, Z));

    public readonly Vector4<T> YYXZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(1L, 1L, 0L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(1, 1, 0, 2))) : new Vector4<T>(Y, Y, X, Z));

    public readonly Vector4<T> ZYXZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(2L, 1L, 0L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(2, 1, 0, 2))) : new Vector4<T>(Z, Y, X, Z));

    public readonly Vector4<T> WYXZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(3L, 1L, 0L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(3, 1, 0, 2))) : new Vector4<T>(W, Y, X, Z));

    public readonly Vector4<T> XZXZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(0L, 2L, 0L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(0, 2, 0, 2))) : new Vector4<T>(X, Z, X, Z));

    public readonly Vector4<T> YZXZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(1L, 2L, 0L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(1, 2, 0, 2))) : new Vector4<T>(Y, Z, X, Z));

    public readonly Vector4<T> ZZXZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(2L, 2L, 0L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(2, 2, 0, 2))) : new Vector4<T>(Z, Z, X, Z));

    public readonly Vector4<T> WZXZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(3L, 2L, 0L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(3, 2, 0, 2))) : new Vector4<T>(W, Z, X, Z));

    public readonly Vector4<T> XWXZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(0L, 3L, 0L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(0, 3, 0, 2))) : new Vector4<T>(X, W, X, Z));

    public readonly Vector4<T> YWXZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(1L, 3L, 0L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(1, 3, 0, 2))) : new Vector4<T>(Y, W, X, Z));

    public readonly Vector4<T> ZWXZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(2L, 3L, 0L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(2, 3, 0, 2))) : new Vector4<T>(Z, W, X, Z));

    public readonly Vector4<T> WWXZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(3L, 3L, 0L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(3, 3, 0, 2))) : new Vector4<T>(W, W, X, Z));

    public readonly Vector4<T> XXYZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(0L, 0L, 1L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(0, 0, 1, 2))) : new Vector4<T>(X, X, Y, Z));

    public readonly Vector4<T> YXYZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(1L, 0L, 1L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(1, 0, 1, 2))) : new Vector4<T>(Y, X, Y, Z));

    public readonly Vector4<T> ZXYZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(2L, 0L, 1L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(2, 0, 1, 2))) : new Vector4<T>(Z, X, Y, Z));

    public readonly Vector4<T> WXYZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(3L, 0L, 1L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(3, 0, 1, 2))) : new Vector4<T>(W, X, Y, Z));

    public readonly Vector4<T> XYYZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(0L, 1L, 1L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(0, 1, 1, 2))) : new Vector4<T>(X, Y, Y, Z));

    public readonly Vector4<T> YYYZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(1L, 1L, 1L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(1, 1, 1, 2))) : new Vector4<T>(Y, Y, Y, Z));

    public readonly Vector4<T> ZYYZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(2L, 1L, 1L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(2, 1, 1, 2))) : new Vector4<T>(Z, Y, Y, Z));

    public readonly Vector4<T> WYYZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(3L, 1L, 1L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(3, 1, 1, 2))) : new Vector4<T>(W, Y, Y, Z));

    public readonly Vector4<T> XZYZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(0L, 2L, 1L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(0, 2, 1, 2))) : new Vector4<T>(X, Z, Y, Z));

    public readonly Vector4<T> YZYZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(1L, 2L, 1L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(1, 2, 1, 2))) : new Vector4<T>(Y, Z, Y, Z));

    public readonly Vector4<T> ZZYZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(2L, 2L, 1L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(2, 2, 1, 2))) : new Vector4<T>(Z, Z, Y, Z));

    public readonly Vector4<T> WZYZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(3L, 2L, 1L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(3, 2, 1, 2))) : new Vector4<T>(W, Z, Y, Z));

    public readonly Vector4<T> XWYZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(0L, 3L, 1L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(0, 3, 1, 2))) : new Vector4<T>(X, W, Y, Z));

    public readonly Vector4<T> YWYZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(1L, 3L, 1L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(1, 3, 1, 2))) : new Vector4<T>(Y, W, Y, Z));

    public readonly Vector4<T> ZWYZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(2L, 3L, 1L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(2, 3, 1, 2))) : new Vector4<T>(Z, W, Y, Z));

    public readonly Vector4<T> WWYZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(3L, 3L, 1L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(3, 3, 1, 2))) : new Vector4<T>(W, W, Y, Z));

    public readonly Vector4<T> XXZZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(0L, 0L, 2L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(0, 0, 2, 2))) : new Vector4<T>(X, X, Z, Z));

    public readonly Vector4<T> YXZZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(1L, 0L, 2L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(1, 0, 2, 2))) : new Vector4<T>(Y, X, Z, Z));

    public readonly Vector4<T> ZXZZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(2L, 0L, 2L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(2, 0, 2, 2))) : new Vector4<T>(Z, X, Z, Z));

    public readonly Vector4<T> WXZZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(3L, 0L, 2L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(3, 0, 2, 2))) : new Vector4<T>(W, X, Z, Z));

    public readonly Vector4<T> XYZZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(0L, 1L, 2L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(0, 1, 2, 2))) : new Vector4<T>(X, Y, Z, Z));

    public readonly Vector4<T> YYZZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(1L, 1L, 2L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(1, 1, 2, 2))) : new Vector4<T>(Y, Y, Z, Z));

    public readonly Vector4<T> ZYZZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(2L, 1L, 2L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(2, 1, 2, 2))) : new Vector4<T>(Z, Y, Z, Z));

    public readonly Vector4<T> WYZZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(3L, 1L, 2L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(3, 1, 2, 2))) : new Vector4<T>(W, Y, Z, Z));

    public readonly Vector4<T> XZZZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(0L, 2L, 2L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(0, 2, 2, 2))) : new Vector4<T>(X, Z, Z, Z));

    public readonly Vector4<T> YZZZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(1L, 2L, 2L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(1, 2, 2, 2))) : new Vector4<T>(Y, Z, Z, Z));

    public readonly Vector4<T> ZZZZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(SplatZ(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(SplatZ(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this))) : new Vector4<T>(Z, Z, Z, Z));

    public readonly Vector4<T> WZZZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(3L, 2L, 2L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(3, 2, 2, 2))) : new Vector4<T>(W, Z, Z, Z));

    public readonly Vector4<T> XWZZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(0L, 3L, 2L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(0, 3, 2, 2))) : new Vector4<T>(X, W, Z, Z));

    public readonly Vector4<T> YWZZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(1L, 3L, 2L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(1, 3, 2, 2))) : new Vector4<T>(Y, W, Z, Z));

    public readonly Vector4<T> ZWZZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(2L, 3L, 2L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(2, 3, 2, 2))) : new Vector4<T>(Z, W, Z, Z));

    public readonly Vector4<T> WWZZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(3L, 3L, 2L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(3, 3, 2, 2))) : new Vector4<T>(W, W, Z, Z));

    public readonly Vector4<T> XXWZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(0L, 0L, 3L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(0, 0, 3, 2))) : new Vector4<T>(X, X, W, Z));

    public readonly Vector4<T> YXWZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(1L, 0L, 3L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(1, 0, 3, 2))) : new Vector4<T>(Y, X, W, Z));

    public readonly Vector4<T> ZXWZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(2L, 0L, 3L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(2, 0, 3, 2))) : new Vector4<T>(Z, X, W, Z));

    public readonly Vector4<T> WXWZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(3L, 0L, 3L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(3, 0, 3, 2))) : new Vector4<T>(W, X, W, Z));

    public readonly Vector4<T> XYWZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(0L, 1L, 3L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(0, 1, 3, 2))) : new Vector4<T>(X, Y, W, Z));

    public readonly Vector4<T> YYWZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(1L, 1L, 3L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(1, 1, 3, 2))) : new Vector4<T>(Y, Y, W, Z));

    public readonly Vector4<T> ZYWZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(2L, 1L, 3L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(2, 1, 3, 2))) : new Vector4<T>(Z, Y, W, Z));

    public readonly Vector4<T> WYWZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(3L, 1L, 3L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(3, 1, 3, 2))) : new Vector4<T>(W, Y, W, Z));

    public readonly Vector4<T> XZWZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(0L, 2L, 3L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(0, 2, 3, 2))) : new Vector4<T>(X, Z, W, Z));

    public readonly Vector4<T> YZWZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(1L, 2L, 3L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(1, 2, 3, 2))) : new Vector4<T>(Y, Z, W, Z));

    public readonly Vector4<T> ZZWZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(2L, 2L, 3L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(2, 2, 3, 2))) : new Vector4<T>(Z, Z, W, Z));

    public readonly Vector4<T> WZWZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(3L, 2L, 3L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(3, 2, 3, 2))) : new Vector4<T>(W, Z, W, Z));

    public readonly Vector4<T> XWWZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(0L, 3L, 3L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(0, 3, 3, 2))) : new Vector4<T>(X, W, W, Z));

    public readonly Vector4<T> YWWZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(1L, 3L, 3L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(1, 3, 3, 2))) : new Vector4<T>(Y, W, W, Z));

    public readonly Vector4<T> ZWWZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(2L, 3L, 3L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(2, 3, 3, 2))) : new Vector4<T>(Z, W, W, Z));

    public readonly Vector4<T> WWWZ => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(3L, 3L, 3L, 2L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(3, 3, 3, 2))) : new Vector4<T>(W, W, W, Z));

    public readonly Vector4<T> XXXW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(0L, 0L, 0L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(0, 0, 0, 3))) : new Vector4<T>(X, X, X, W));

    public readonly Vector4<T> YXXW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(1L, 0L, 0L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(1, 0, 0, 3))) : new Vector4<T>(Y, X, X, W));

    public readonly Vector4<T> ZXXW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(2L, 0L, 0L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(2, 0, 0, 3))) : new Vector4<T>(Z, X, X, W));

    public readonly Vector4<T> WXXW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(3L, 0L, 0L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(3, 0, 0, 3))) : new Vector4<T>(W, X, X, W));

    public readonly Vector4<T> XYXW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(0L, 1L, 0L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(0, 1, 0, 3))) : new Vector4<T>(X, Y, X, W));

    public readonly Vector4<T> YYXW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(1L, 1L, 0L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(1, 1, 0, 3))) : new Vector4<T>(Y, Y, X, W));

    public readonly Vector4<T> ZYXW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(2L, 1L, 0L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(2, 1, 0, 3))) : new Vector4<T>(Z, Y, X, W));

    public readonly Vector4<T> WYXW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(3L, 1L, 0L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(3, 1, 0, 3))) : new Vector4<T>(W, Y, X, W));

    public readonly Vector4<T> XZXW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(0L, 2L, 0L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(0, 2, 0, 3))) : new Vector4<T>(X, Z, X, W));

    public readonly Vector4<T> YZXW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(1L, 2L, 0L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(1, 2, 0, 3))) : new Vector4<T>(Y, Z, X, W));

    public readonly Vector4<T> ZZXW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(2L, 2L, 0L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(2, 2, 0, 3))) : new Vector4<T>(Z, Z, X, W));

    public readonly Vector4<T> WZXW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(3L, 2L, 0L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(3, 2, 0, 3))) : new Vector4<T>(W, Z, X, W));

    public readonly Vector4<T> XWXW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(0L, 3L, 0L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(0, 3, 0, 3))) : new Vector4<T>(X, W, X, W));

    public readonly Vector4<T> YWXW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(1L, 3L, 0L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(1, 3, 0, 3))) : new Vector4<T>(Y, W, X, W));

    public readonly Vector4<T> ZWXW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(2L, 3L, 0L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(2, 3, 0, 3))) : new Vector4<T>(Z, W, X, W));

    public readonly Vector4<T> WWXW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(3L, 3L, 0L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(3, 3, 0, 3))) : new Vector4<T>(W, W, X, W));

    public readonly Vector4<T> XXYW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(0L, 0L, 1L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(0, 0, 1, 3))) : new Vector4<T>(X, X, Y, W));

    public readonly Vector4<T> YXYW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(1L, 0L, 1L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(1, 0, 1, 3))) : new Vector4<T>(Y, X, Y, W));

    public readonly Vector4<T> ZXYW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(2L, 0L, 1L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(2, 0, 1, 3))) : new Vector4<T>(Z, X, Y, W));

    public readonly Vector4<T> WXYW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(3L, 0L, 1L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(3, 0, 1, 3))) : new Vector4<T>(W, X, Y, W));

    public readonly Vector4<T> XYYW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(0L, 1L, 1L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(0, 1, 1, 3))) : new Vector4<T>(X, Y, Y, W));

    public readonly Vector4<T> YYYW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(1L, 1L, 1L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(1, 1, 1, 3))) : new Vector4<T>(Y, Y, Y, W));

    public readonly Vector4<T> ZYYW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(2L, 1L, 1L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(2, 1, 1, 3))) : new Vector4<T>(Z, Y, Y, W));

    public readonly Vector4<T> WYYW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(3L, 1L, 1L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(3, 1, 1, 3))) : new Vector4<T>(W, Y, Y, W));

    public readonly Vector4<T> XZYW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(0L, 2L, 1L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(0, 2, 1, 3))) : new Vector4<T>(X, Z, Y, W));

    public readonly Vector4<T> YZYW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(1L, 2L, 1L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(1, 2, 1, 3))) : new Vector4<T>(Y, Z, Y, W));

    public readonly Vector4<T> ZZYW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(2L, 2L, 1L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(2, 2, 1, 3))) : new Vector4<T>(Z, Z, Y, W));

    public readonly Vector4<T> WZYW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(3L, 2L, 1L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(3, 2, 1, 3))) : new Vector4<T>(W, Z, Y, W));

    public readonly Vector4<T> XWYW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(0L, 3L, 1L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(0, 3, 1, 3))) : new Vector4<T>(X, W, Y, W));

    public readonly Vector4<T> YWYW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(1L, 3L, 1L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(1, 3, 1, 3))) : new Vector4<T>(Y, W, Y, W));

    public readonly Vector4<T> ZWYW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(2L, 3L, 1L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(2, 3, 1, 3))) : new Vector4<T>(Z, W, Y, W));

    public readonly Vector4<T> WWYW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(3L, 3L, 1L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(3, 3, 1, 3))) : new Vector4<T>(W, W, Y, W));

    public readonly Vector4<T> XXZW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(0L, 0L, 2L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(0, 0, 2, 3))) : new Vector4<T>(X, X, Z, W));

    public readonly Vector4<T> YXZW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(1L, 0L, 2L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(1, 0, 2, 3))) : new Vector4<T>(Y, X, Z, W));

    public readonly Vector4<T> ZXZW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(2L, 0L, 2L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(2, 0, 2, 3))) : new Vector4<T>(Z, X, Z, W));

    public readonly Vector4<T> WXZW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(3L, 0L, 2L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(3, 0, 2, 3))) : new Vector4<T>(W, X, Z, W));

    public readonly Vector4<T> XYZW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(0L, 1L, 2L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(0, 1, 2, 3))) : new Vector4<T>(X, Y, Z, W));

    public readonly Vector4<T> YYZW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(1L, 1L, 2L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(1, 1, 2, 3))) : new Vector4<T>(Y, Y, Z, W));

    public readonly Vector4<T> ZYZW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(2L, 1L, 2L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(2, 1, 2, 3))) : new Vector4<T>(Z, Y, Z, W));

    public readonly Vector4<T> WYZW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(3L, 1L, 2L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(3, 1, 2, 3))) : new Vector4<T>(W, Y, Z, W));

    public readonly Vector4<T> XZZW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(0L, 2L, 2L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(0, 2, 2, 3))) : new Vector4<T>(X, Z, Z, W));

    public readonly Vector4<T> YZZW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(1L, 2L, 2L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(1, 2, 2, 3))) : new Vector4<T>(Y, Z, Z, W));

    public readonly Vector4<T> ZZZW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(2L, 2L, 2L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(2, 2, 2, 3))) : new Vector4<T>(Z, Z, Z, W));

    public readonly Vector4<T> WZZW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(3L, 2L, 2L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(3, 2, 2, 3))) : new Vector4<T>(W, Z, Z, W));

    public readonly Vector4<T> XWZW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(0L, 3L, 2L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(0, 3, 2, 3))) : new Vector4<T>(X, W, Z, W));

    public readonly Vector4<T> YWZW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(1L, 3L, 2L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(1, 3, 2, 3))) : new Vector4<T>(Y, W, Z, W));

    public readonly Vector4<T> ZWZW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(2L, 3L, 2L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(2, 3, 2, 3))) : new Vector4<T>(Z, W, Z, W));

    public readonly Vector4<T> WWZW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(3L, 3L, 2L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(3, 3, 2, 3))) : new Vector4<T>(W, W, Z, W));

    public readonly Vector4<T> XXWW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(0L, 0L, 3L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(0, 0, 3, 3))) : new Vector4<T>(X, X, W, W));

    public readonly Vector4<T> YXWW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(1L, 0L, 3L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(1, 0, 3, 3))) : new Vector4<T>(Y, X, W, W));

    public readonly Vector4<T> ZXWW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(2L, 0L, 3L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(2, 0, 3, 3))) : new Vector4<T>(Z, X, W, W));

    public readonly Vector4<T> WXWW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(3L, 0L, 3L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(3, 0, 3, 3))) : new Vector4<T>(W, X, W, W));

    public readonly Vector4<T> XYWW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(0L, 1L, 3L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(0, 1, 3, 3))) : new Vector4<T>(X, Y, W, W));

    public readonly Vector4<T> YYWW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(1L, 1L, 3L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(1, 1, 3, 3))) : new Vector4<T>(Y, Y, W, W));

    public readonly Vector4<T> ZYWW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(2L, 1L, 3L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(2, 1, 3, 3))) : new Vector4<T>(Z, Y, W, W));

    public readonly Vector4<T> WYWW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(3L, 1L, 3L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(3, 1, 3, 3))) : new Vector4<T>(W, Y, W, W));

    public readonly Vector4<T> XZWW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(0L, 2L, 3L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(0, 2, 3, 3))) : new Vector4<T>(X, Z, W, W));

    public readonly Vector4<T> YZWW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(1L, 2L, 3L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(1, 2, 3, 3))) : new Vector4<T>(Y, Z, W, W));

    public readonly Vector4<T> ZZWW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(2L, 2L, 3L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(2, 2, 3, 3))) : new Vector4<T>(Z, Z, W, W));

    public readonly Vector4<T> WZWW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(3L, 2L, 3L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(3, 2, 3, 3))) : new Vector4<T>(W, Z, W, W));

    public readonly Vector4<T> XWWW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(0L, 3L, 3L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(0, 3, 3, 3))) : new Vector4<T>(X, W, W, W));

    public readonly Vector4<T> YWWW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(1L, 3L, 3L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(1, 3, 3, 3))) : new Vector4<T>(Y, W, W, W));

    public readonly Vector4<T> ZWWW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(Vector256.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this), Vector256.Create(2L, 3L, 3L, 3L))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(Vector128.ShuffleNative(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this), Vector128.Create(2, 3, 3, 3))) : new Vector4<T>(Z, W, W, W));

    public readonly Vector4<T> WWWW => Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double) ? Unsafe.BitCast<Vector256<double>, Vector4<T>>(SplatW(Unsafe.BitCast<Vector4<T>, Vector256<double>>(this))) : (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float) ? Unsafe.BitCast<Vector128<float>, Vector4<T>>(SplatW(Unsafe.BitCast<Vector4<T>, Vector128<float>>(this))) : new Vector4<T>(W, W, W, W));

    #endregion Vector4 Swizzles

    #endregion Swizzling
    }
