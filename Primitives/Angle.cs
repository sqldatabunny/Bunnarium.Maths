using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Bunnarium.Maths.Primitives;

/// <summary> Represents a direction or orientation in 2D space, such that 0 degrees faces right, 90 degrees faces up, 180 degrees faces left, and 270 degrees faces down.
/// </summary>
[DebuggerDisplay("{ToString(), nq}")]
public struct Angle<T>
    : IRotation<Angle<T>, Angle<T>, Vector2<T>, T>
    , IDirection<Angle<T>, Angle<T>, Vector2<T>, T>
    , IVectorWrapper<Angle<T>, Vector1<T>, T>
    , IEquatable<Angle<T>>
    where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {

    #region Data

    T _angle; // stored in radians

    #endregion Data

    #region Constructors and Factories

    /// <summary> Creates a new <see cref="Angle{T}"/> with the direction of a vector with the given <paramref name="x"/> and <paramref name="y"/> values.
    /// </summary>
    public Angle(T x, T y) : this(T.Atan2(y, x) + (y < T.Zero ? RadianConstants.Degrees360 : T.Zero)) {
        }

    /// <summary> Creates a new <see cref="Angle{T}"/> with the direction of the input <paramref name="vector"/>.
    /// </summary>
    public Angle(Vector2<T> vector) : this(T.Atan2(vector.Y, vector.X) + (vector.Y < T.Zero ? RadianConstants.Degrees360 : T.Zero)) {
        }

    /// <inheritdoc cref="FromRadiansUnchecked(T)"/>
    public Angle(T radians) {
        _angle = radians;
        }

    /// <inheritdoc
    /// cref="Angle{T}.Angle(Vector2{T})"/>
    public static Angle<T> FromVector(Vector2<T> vector) {
        return new(vector);
        }

    /// <summary> Creates a new <see cref="Angle{T}"/> with the value of the given <paramref name="vector"/>, which represents radians.
    /// </summary>
    /// <remarks> This value is <b>not</b> <see cref="Normalize(Angle{T})">normalized</see>, so inputs with <see cref="Vector1{T}.X">X</see> values outside the range <c>[0, 360)</c> will result in unnormalized angles.
    /// </remarks>
    public static Angle<T> FromVector(Vector1<T> vector) {
        return FromRadiansUnchecked(vector.X);
        }

    /// <summary> Creates a new <see cref="Angle{T}"/> with the specified <paramref name="degrees"/>.
    /// </summary>
    public static Angle<T> FromDegrees(T degrees) {
        return new(Deg2Rad(degrees));
        }

    /// <summary> Creates an <see cref="Angle{T}"/> with the specified <paramref name="radians"/>, ensuring the result is normalized within the range <c>[0, 2π)</c>.
    /// </summary>
    public static Angle<T> FromRadiansChecked(T radians) {
        var circle = RadianConstants.Degrees360;
        var rads = ((radians % circle) + circle) % circle;
        return new(rads);
        }

    /// <summary> Creates an <see cref="Angle{T}"/> with the specified <paramref name="radians"/> <em>without</em> normalizing the result to the range <c>[0, 2π)</c>.
    /// </summary>
    public static Angle<T> FromRadiansUnchecked(T radians) {
        return new(radians);
        }

    /// <summary> Creates an <see cref="Angle{T}"/> that represents the rotation <paramref name="from"/> one angle <paramref name="to"/> another (i.e., returns <c><paramref name="to"/> - <paramref name="from"/></c> <b><em>without</em></b> <see cref="Normalize(Angle{T})">normalizing</see> it).
    /// </summary>
    public static Angle<T> FromToRotation(Angle<T> from, Angle<T> to) {
        return new(to.Radians - from.Radians);
        }

    /// <summary> Creates an <see cref="Angle{T}"/> that represents the rotation <paramref name="from"/> one angle, represented as a 2D vector, <paramref name="to"/> another (i.e., returns <c><paramref name="to"/> - <paramref name="from"/></c> <b><em>without</em></b> normalization).
    /// </summary>
    public static Angle<T> FromToRotation(Vector2<T> from, Vector2<T> to) {
        var a = Angle<T>.FromVector(from);
        var b = Angle<T>.FromVector(to);
        return FromToRotation(a, b);
        }

    /// <inheritdoc
    /// />
    public static Angle<T> FromLookAt(Vector2<T> source, Vector2<T> target) {
        return new(target - source);
        }

    /// <inheritdoc
    /// />
    public static Angle<T> CreateZero() {
        return Zero;
        }

    #endregion Constructors and Factories

    #region Concatenation

    public static Angle<T> Concatenate(Angle<T> first, Angle<T> second) {
        return new(first.Radians + second.Radians);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Angle<T> Concatenate(Angle<T> other) {
        return Concatenate(this, other);
        }

    #endregion Concatenation

    #region Constants

    /// <summary> An instance of <see cref="Angle{T}"/> with a value of 180 degrees (π radians).
    /// </summary>
    public static readonly Angle<T> Half = new(RadianConstants.Degrees180);

    /// <summary> An instance of <see cref="Angle{T}"/> with a value of 0 degrees (0 radians).
    /// </summary>
    public static Angle<T> Zero { get; } = new(T.Zero);

    /// <inheritdoc
    /// />
    public static Angle<T> Forward => new(T.Zero);

    /// <inheritdoc
    /// />
    public static Angle<T> Right => new(T.Zero);

    /// <inheritdoc
    /// />
    public static Angle<T> Up => new(RadianConstants.Degrees90);

    /// <summary> Fetches an arrow symbol suitable for printing from <see cref="Linguistics.Symbols"/>.
    /// </summary>
    public readonly string SymbolicArrow {
        get {
            var oneEighth = GenericNumbers<T>.FromBinaryInteger(45);
            var oneSixteenth = oneEighth * GenericNumbers<T>.OneHalf;
            var eight = GenericNumbers<T>.FromBinaryInteger(8);
            return GenericNumbers<T>.ToInt32(T.Floor((Normalize(this).Degrees + oneSixteenth) / oneEighth) % eight) switch {
                0 => "🡲",
                1 => "🡵",
                2 => "🡱",
                3 => "🡴",
                4 => "🡰",
                5 => "🡷",
                6 => "🡳",
                7 => "🡶",
                _ => ""
                };
            }
        }

    /// <summary> A collection of pre-computed ratios of π radians to degrees, which is useful for avoiding expensive generic math number instantiation in DEBUG mode.
    /// </summary>
    /// <remarks> Please don't use reflection to access this type's members, as they are fields in DEBUG mode and properties in RELEASE mode.
    /// </remarks>
    public static class RadianConstants {
#if DEBUG
        public static readonly T Almost180Degrees = T.Pi - T.CreateTruncating(0.000001);
        public static readonly T Degrees1 = T.Pi / T.CreateTruncating(180);
        public static readonly T Degrees10 = T.Pi / T.CreateTruncating(18.0);
        public static readonly T Degrees120 = T.Pi / T.CreateTruncating(1.5);
        public static readonly T Degrees15 = T.Pi / T.CreateTruncating(12.0);
        public static readonly T Degrees150 = T.Pi * T.CreateTruncating(5.0 / 6.0);
        public static readonly T Degrees180 = T.Pi;
        public static readonly T Degrees20 = T.Pi / T.CreateTruncating(9.0);
        public static readonly T Degrees210 = T.Pi * T.CreateTruncating(7.0 / 6.0);
        public static readonly T Degrees22p5 = T.Pi / T.CreateTruncating(8.0);
        public static readonly T Degrees240 = T.Pi * T.CreateTruncating(4.0 / 3.0);
        public static readonly T Degrees270 = T.Pi * T.CreateTruncating(1.5);
        public static readonly T Degrees30 = T.Pi / T.CreateTruncating(6.0);
        public static readonly T Degrees300 = T.Pi * T.CreateTruncating(5.0 / 3.0);
        public static readonly T Degrees330 = T.Pi * T.CreateTruncating(11.0 / 6.0);
        public static readonly T Degrees360 = T.Pi * T.CreateTruncating(2.0);
        public static readonly T Degrees45 = T.Pi / T.CreateTruncating(4.0);
        public static readonly T Degrees5 = T.Pi / T.CreateTruncating(36.0);
        public static readonly T Degrees60 = T.Pi / T.CreateTruncating(3.0);
        public static readonly T Degrees90 = T.Pi / T.CreateTruncating(2.0);
        public static readonly T PiUnder180 = T.CreateTruncating(180.0) / T.Pi;
#else
        public static T Almost180Degrees => T.Pi - T.CreateTruncating(0.000001);
        public static T Degrees1 => T.Pi / T.CreateTruncating(180);
        public static T Degrees10 => T.Pi / T.CreateTruncating(18.0);
        public static T Degrees120 => T.Pi / T.CreateTruncating(1.5);
        public static T Degrees15 => T.Pi / T.CreateTruncating(12.0);
        public static T Degrees150 => T.Pi * T.CreateTruncating(5.0 / 6.0);
        public static T Degrees180 => T.Pi;
        public static T Degrees20 => T.Pi / T.CreateTruncating(9.0);
        public static T Degrees210 => T.Pi * T.CreateTruncating(7.0 / 6.0);
        public static T Degrees22p5 => T.Pi / T.CreateTruncating(8.0);
        public static T Degrees240 => T.Pi * T.CreateTruncating(4.0 / 3.0);
        public static T Degrees270 => T.Pi * T.CreateTruncating(1.5);
        public static T Degrees30 => T.Pi / T.CreateTruncating(6.0);
        public static T Degrees300 => T.Pi * T.CreateTruncating(5.0 / 3.0);
        public static T Degrees330 => T.Pi * T.CreateTruncating(11.0 / 6.0);
        public static T Degrees360 => T.Pi * T.CreateTruncating(2.0);
        public static T Degrees45 => T.Pi / T.CreateTruncating(4.0);
        public static T Degrees5 => T.Pi / T.CreateTruncating(36.0);
        public static T Degrees60 => T.Pi / T.CreateTruncating(3.0);
        public static T Degrees90 => T.Pi / T.CreateTruncating(2.0);
        public static T PiUnder180 => T.CreateTruncating(180.0) / T.Pi;
#endif
        }

    #endregion Constants

    #region Dot

    public static T Dot(in Angle<T> left, in Angle<T> right) {
        var a = left.Vector;
        var b = right.Vector;
        return a.X * b.X + a.Y * b.Y;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly T Dot(in Angle<T> other) {
        return Dot(this, other);
        }

    #endregion Dot

    #region Equatability

    public static bool operator ==(Angle<T> left, Angle<T> right) {
        return left.Equals(right);
        }

    public static bool operator !=(Angle<T> left, Angle<T> right) {
        return (left.Equals(right) == false);
        }

    /// <remarks> For <see cref="Angle{T}"/> equality checking, values are normalized prior to comparison, so unequal angles that <em>are</em> equal when normalized but are not themselves normalized will still be considered equal.
    /// </remarks>
    /// <inheritdoc/>
    public readonly bool Equals(Angle<T> other) {
        return Normalize().Radians == other.Normalize().Radians;
        }

    /// <inheritdoc
    /// cref="Equals(Angle{T})"/>
    public override readonly bool Equals(object? obj) {
        return obj is Angle<T> angle1 && Equals(angle1);
        }

    public override readonly int GetHashCode() {
        return Normalize().Radians.GetHashCode();
        }

    #endregion Equatability

    #region Flip

    /// <summary><inheritdoc/></summary>
    /// <remarks><inheritdoc cref="IDirection{TDirection, TRotation, TVector, T}.Flip(TDirection)"/>
    /// <para/> The number of total <see cref="RoundTrips"/> stored in the <paramref name="direction"/>, if it is not normalized, will be preserved. In other words, the angle will stay within the range <c>[2π * R, 2π * (R+1))</c> where <c>R</c> is the integer component of <see cref="RoundTrips">RoundTrips</see>.
    /// </remarks>
    public static Angle<T> Flip(Angle<T> direction) {
        var normalized = Normalize(direction).Radians;
        var localFlip = normalized < RadianConstants.Degrees180
            ? normalized + RadianConstants.Degrees180
            : normalized - RadianConstants.Degrees180;
        return FromRadiansUnchecked(direction.Radians + localFlip  - normalized);
        }

    /// <summary><inheritdoc/></summary>
    /// <remarks><inheritdoc cref="Flip(Angle{T})"/></remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Flip(ref Angle<T> direction) {
        direction = Flip(direction);
        }

    /// <inheritdoc
    /// cref="Flip(Angle{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Angle<T> Flip() {
        return Flip(this);
        }

    #endregion Flip

    #region Interpolation

    public static Angle<T> Lerp(Angle<T> from, Angle<T> to, T amount) {
        var a = from.Radians;
        var b = to.Radians;
        var c = T.Abs(a - b);
        if (c > RadianConstants.Degrees180) {
            if (a > b)
                b += RadianConstants.Degrees360;
            else
                a += RadianConstants.Degrees360;
            }
        return Angle<T>.FromRadiansChecked(a + (b - a) * amount);
        }

    public static Angle<T> LerpUnchecked(Angle<T> from, Angle<T> to, T amount) {
        return from * (T.One - amount) + to * amount;
        }

    /// <remarks> For <see cref="Angle{T}"/>, <see cref="Slerp(Angle{T}, Angle{T}, T)"/> is equivalent to <see cref="Lerp(Angle{T}, Angle{T}, T)"/>.
    /// </remarks>
    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Angle<T> Slerp(Angle<T> from, Angle<T> to, T amount) {
        return Lerp(from, to, amount);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Angle<T> Lerp(Angle<T> to, T amount) {
        return Lerp(this, to, amount);
        }

    public readonly Angle<T> LerpUnchecked(Angle<T> to, T amount) {
        return LerpUnchecked(this, to, amount);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Angle<T> Slerp(Angle<T> to, T amount) {
        return Lerp(this, to, amount);
        }

    #endregion Interpolation

    #region Inversion

    /// <inheritdoc
    /// cref="IRotation{TRotation, TDirection, TVector, T}.Invert(ref TRotation)"/>
    public static void Invert(ref Angle<T> angle) {
        angle.Radians *= T.NegativeOne;
        }

    /// <inheritdoc
    /// cref="IRotation{TRotation, TDirection, TVector, T}.Invert(TRotation)"/>
    public static Angle<T> Invert(Angle<T> rotation) {
        return FromRadiansUnchecked(-rotation.Radians);
        }

    /// <inheritdoc
    /// cref="IRotation{TRotation, TDirection, TVector, T}.Invert()"/>
    public readonly Angle<T> Invert() {
        return Invert(this);
        }

    #endregion Inversion

    #region Negation

    /// <inheritdoc
    /// cref="IRotation{TRotation, TDirection, TVector, T}.Negate(ref TRotation)"/>
    public static void Negate(ref Angle<T> rotation) {
        rotation = Negate(rotation);
        }

    /// <inheritdoc
    /// cref="IRotation{TRotation, TDirection, TVector, T}.Negate(TRotation)"/>
    public static Angle<T> Negate(Angle<T> rotation) {
        var r = rotation.Radians;
        var twoPi = GenericNumbers<T>.TwoPi;
        var trips = T.Truncate(rotation.RoundTrips);
        if (r > T.Zero) {
            return Angle<T>.FromRadiansUnchecked(r - twoPi * (trips + T.One));
            }
        else if (r < T.Zero) {
            return Angle<T>.FromRadiansUnchecked(r + twoPi * (trips + T.One));
            }
        else {
            return Angle<T>.Zero;
            }
        }

    /// <inheritdoc
    /// cref="IRotation{TRotation, TDirection, TVector, T}.Negate()"/>
    public readonly Angle<T> Negate() {
        return Negate(this);
        }

    #endregion Negation

    #region Normalization

    /// <summary> Checks whether the angle is normalized, i.e., whether it is within the range [0, 360) degrees / [0, 2π) radians.
    /// </summary>
    public readonly bool IsNormalized {
        get => _angle >= T.Zero && _angle < RadianConstants.Degrees360;
        }

    public static Angle<T> Normalize(Angle<T> angle) {
        var radians = angle.Radians;
        var fullRevolution = RadianConstants.Degrees360;
        var result = ((radians % fullRevolution) + fullRevolution) % fullRevolution;
        return new(result);
        }

    public static void Normalize(ref Angle<T> angle) {
        var radians = angle.Radians;
        var fullRevolution = RadianConstants.Degrees360;
        var result = ((radians % fullRevolution) + fullRevolution) % fullRevolution;
        angle.Radians = result;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Angle<T> Normalize() {
        return Normalize(this);
        }

    #endregion Normalization

    #region Operators

    public static Angle<T> Add(Angle<T> left, Angle<T> right) {
        return left + right;
        }

    public static Angle<T> Divide(Angle<T> left, Angle<T> right) {
        return left / right;
        }

    public static Angle<T> Divide(Angle<T> angle, T factor) {
        return new(angle.Radians / factor);
        }

    public static Angle<T> Multiply(Angle<T> left, Angle<T> right) {
        return left + right;
        }

    public static Angle<T> Multiply(Angle<T> angle, T factor) {
        return new(angle.Radians * factor);
        }

    public static Angle<T> operator -(Angle<T> a, Angle<T> b) {
        return Concatenate(a, Invert(b));
        }

    public static Angle<T> operator *(Angle<T> angle, T d) {
        return new(angle.Radians * d);
        }

    public static Angle<T> operator *(Angle<T> a, Angle<T> b) {
        return Concatenate(a, b);
        }

    public static Angle<T> operator /(Angle<T> angle, T d) {
        return new(angle.Radians / d);
        }

    public static Angle<T> operator /(Angle<T> a, Angle<T> b) {
        return Concatenate(a, Invert(b));
        }

    public static Angle<T> operator +(Angle<T> a, Angle<T> b) {
        return Concatenate(a, b);
        }

    public static Angle<T> Subtract(Angle<T> left, Angle<T> right) {
        return left - right;
        }

    public readonly Angle<T> Add(Angle<T> other) {
        return this + other;
        }

    public readonly Angle<T> Divide(T factor) {
        return Divide(this, factor);
        }

    public readonly Angle<T> Divide(Angle<T> other) {
        return this / other;
        }

    public readonly Angle<T> Multiply(T factor) {
        return Multiply(this, factor);
        }

    public readonly Angle<T> Multiply(Angle<T> other) {
        return this * other;
        }

    public readonly Angle<T> Subtract(Angle<T> other) {
        return this - other;
        }

    #endregion Operators

    #region Radians and Degrees

    /// <summary> The angle in degrees.
    /// </summary>
    public T Degrees {
        readonly get => _angle * RadianConstants.PiUnder180;
        set => _angle = value / RadianConstants.PiUnder180;
        }

    /// <summary> The angle in radians.
    /// </summary>
    public T Radians {
        readonly get => _angle;
        set => _angle = value;
        }

    /// <summary> Converts the input <paramref name="degrees"/> to radians.
    /// </summary>
    /// <remarks> <see cref="Normalize(Angle{T})">Normalization</see> does <b>not</b> occur in this function, so input values may be outside the range <c>[0, 360)</c> and output values may be outside the range [0, 2π).
    /// </remarks>
    public static T Deg2Rad(T degrees) {
        return degrees / RadianConstants.PiUnder180;
        }

    /// <summary> Converts the input <paramref name="radians"/> to degrees.
    /// </summary>
    /// <remarks> <see cref="Normalize(Angle{T})">Normalization</see> does <b>not</b> occur in this function, so input values may be outside the range <c>[0, 2π)</c> and output values may be outside the range [0, 360).
    /// </remarks>
    public static T Rad2Deg(T radians) {
        return radians * RadianConstants.PiUnder180;
        }

    #endregion Radians and Degrees

    #region Rotation

    /// <inheritdoc
    /// />
    public static void Rotate(ref Angle<T> direction, Angle<T> rotation) {
        direction.Radians += rotation.Radians;
        }

    /// <summary><inheritdoc/></summary>
    /// <remarks> If the angle is normalized, then this will be 0. The return value can also be positive or negative, and it can be fractional.
    /// </remarks>
    public readonly T RoundTrips {
        get => Radians / RadianConstants.Degrees360;
        }

    /// <inheritdoc
    /// cref="IRotation{TRotation, TDirection, TVector, T}.Rotate(TDirection, TRotation)"/>
    public static Angle<T> Rotate(Angle<T> direction, Angle<T> rotation) {
        return direction.Concatenate(rotation);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2<T> Rotate(Vector2<T> vector, Angle<T> rotation) {
        var cos = T.Cos(rotation.Radians);
        var sin = T.Sin(rotation.Radians);
        return new Vector2<T>(vector.X * cos - vector.Y * sin, vector.X * sin + vector.Y * cos);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2<T> RotateAboutAxis(Vector2<T> point, Angle<T> axis, Angle<T> angle) {
        return RotateAboutAxis(point, axis.Vector, angle);
        }

    /// <summary> Rotates a 2D <paramref name="point"/> around the given <paramref name="axis"/> by the given <paramref name="angle"/>.
    /// </summary>
    /// <param name="point"> The point to be rotated.</param>
    /// <param name="axis"> The center point around which to rotate (i.e., pivot point).</param>
    /// <param name="angle"> The rotation angle—positive angles rotate counterclockwise.</param>
    /// <returns> The position of the rotated <paramref name="point"/>.
    /// </returns>
    /// <remarks> Performs rotation by translating the point to the origin, converting it to complex numbers, multiplying by e^(iθ), and then translating it back. <inheritdoc cref="DocStrings.Definition_Angles_Positive"/>
    /// </remarks>
    public static Vector2<T> RotateAboutAxis(Vector2<T> point, Vector2<T> axis, Angle<T> angle) {
        var cP = Vector2<T>.ToComplex(point - axis);
        var cR = Complex<T>.FromAngle(angle);
        var rotated = cP * cR;
        return rotated.Vector + axis;
        }

    public readonly Angle<T> Rotate(Angle<T> other) {
        return Concatenate(other);
        }

    public readonly Vector2<T> Rotate(Vector2<T> vector) {
        var cos = T.Cos(Radians);
        var sin = T.Sin(Radians);
        return new Vector2<T>(vector.X * cos - vector.Y * sin, vector.X * sin + vector.Y * cos);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Vector2<T> RotateAboutAxis(Vector2<T> point, Angle<T> angle) {
        return RotateAboutAxis(point, this, angle);
        }

    #endregion Rotation

    #region Strings

    public override readonly string ToString() {
        return $"[{SymbolicArrow} {Degrees}°]";
        }

    public readonly string ToString(byte digits, int integerLength, int paddingLength) {
        var deg = Degrees.Stringify(digits, integerLength, paddingLength);
        return $"[{SymbolicArrow} {deg}°]";
        }

    #endregion Strings

    #region Trigonometry

    /// <inheritdoc
    /// cref="ITrigonometricFunctions{T}.Acos(T)"/>
    public static Angle<T> FromAcos(T ratio) {
        return FromRadiansUnchecked(T.Acos(ratio));
        }

    /// <inheritdoc
    /// cref="ITrigonometricFunctions{T}.Asin(T)"/>
    public static Angle<T> FromAsin(T ratio) {
        return FromRadiansUnchecked(T.Asin(ratio));
        }

    /// <inheritdoc
    /// cref="ITrigonometricFunctions{T}.Atan(T)"/>
    public static Angle<T> FromAtan(T ratio) {
        return FromRadiansUnchecked(T.Atan(ratio));
        }

    /// <inheritdoc
    /// cref="ITrigonometricFunctions{T}.Cos(T)"/>
    public readonly T Cos => T.Cos(Radians);

    /// <inheritdoc
    /// cref="ITrigonometricFunctions{T}.Sin(T)"/>
    public readonly T Sin => T.Sin(Radians);

    /// <inheritdoc
    /// cref="ITrigonometricFunctions{T}.SinCos(T)"/>
    public readonly (T Sin, T Cos) SinCos => T.SinCos(Radians);

    /// <inheritdoc
    /// cref="ITrigonometricFunctions{T}.Tan(T)"/>
    public readonly T Tan => T.Tan(Radians);

    #endregion Trigonometry

    #region Vectors

    public static int VectorLength => 1;

    public readonly Vector1<T> UnwrapVector {
        get => new(_angle);
        }

    public Vector2<T> Vector {
        readonly get => new(T.Cos(Radians), T.Sin(Radians));
        set {
            _angle = T.Atan2(value.Y, value.X);
            if (value.Y < T.Zero) _angle += RadianConstants.Degrees360;
            }
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref Vector1<T> AsVector(ref Angle<T> angle) {
        return ref Unsafe.As<Angle<T>, Vector1<T>>(ref angle);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref Angle<T> AsWrapper(ref Vector1<T> vector) {
        return ref Unsafe.As<Vector1<T>, Angle<T>>(ref vector);
        }

    #endregion Vectors
    }
