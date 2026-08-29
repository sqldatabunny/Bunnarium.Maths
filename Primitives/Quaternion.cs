using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.InteropServices;
using static Bunnarium.Tools.Utilities.SIMD;
namespace Bunnarium.Maths.Primitives;

/// <summary> A Hamiltonian Quaternion representing a 3-dimensional rotation or orientation.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
[DebuggerDisplay("{ToString(), nq}")]
public struct Quaternion<T>
    : IRotation<Quaternion<T>, Direction<T>, Vector3<T>, T>
    , IEquatable<Quaternion<T>>
    , IVectorWrapper<Quaternion<T>, Vector4<T>, T>
    where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {

    #region Config

    static bool ValueIsNormalized(T value) {
        return T.Abs(T.One - value) < Epsilon<T>.Strict;
        }

    #endregion Config

    #region Data

    public T X {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly get;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set;
        }

    public T Y {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly get;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set;
        }

    public T Z {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly get;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set;
        }

    public T W {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly get;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set;
        }

    /// <summary> The quaternion's <see cref="Vector3{T}">vector</see> component.
    /// </summary>
    public Vector3<T> XYZ {
        readonly get => new(X, Y, Z);
        set { X = value.X; Y = value.Y; Z = value.Z; }
        }

    #endregion Data

    #region Constructors

    private static readonly Quaternion<T> _identity = new(T.Zero, T.Zero, T.Zero, T.One);

    /// <param name="x"> The X component of the quaternion's vector (imaginary) part.</param>
    /// <param name="y"> The Y component of the quaternion's vector (imaginary) part.</param>
    /// <param name="z"> The Z component of the quaternion's vector (imaginary) part.</param>
    /// <param name="w"> The quaternion's scalar (real) part.</param>
    public Quaternion(T x, T y, T z, T w) {
        X = x;
        Y = y;
        Z = z;
        W = w;
        }

    /// <inheritdoc
    /// cref="FromVector4(Vector4{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Quaternion(Vector4<T> vector) : this(vector.X, vector.Y, vector.Z, vector.W) { }

    /// <summary> Creates a <see cref="Quaternion{T}"/> from the given <paramref name="vector"/> (imaginary part) and <paramref name="scalar"/> (real part) components.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Quaternion(Vector3<T> vector, T scalar) : this(vector.X, vector.Y, vector.Z, scalar) { }

    /// <inheritdoc
    /// cref="Quaternion{T}.Quaternion(Matrix3x4{T})"/>
    public Quaternion(Matrix3<T> matrix) {
        Matrix.UnsafeConvert(in matrix, out Matrix4<T> m);
        Matrix.CreateQuaternionComponentsFromMatrix(m, out var x, out var y, out var z, out var w);
        X = x;
        Y = y;
        Z = z;
        W = w;
        }

    /// <summary> Creates a new <see cref="Quaternion{T}"/> from a <paramref name="matrix"/>.
    /// </summary>
    public Quaternion(Matrix3x4<T> matrix) {
        Matrix.UnsafeConvert(in matrix, out Matrix4<T> m);
        Matrix.CreateQuaternionComponentsFromMatrix(m, out var x, out var y, out var z, out var w);
        X = x;
        Y = y;
        Z = z;
        W = w;
        }

    /// <inheritdoc
    /// cref="Quaternion{T}.Quaternion(Matrix3x4{T})"/>
    public Quaternion(Matrix4x3<T> matrix) {
        Matrix.UnsafeConvert(in matrix, out Matrix4<T> m);
        Matrix.CreateQuaternionComponentsFromMatrix(m, out var x, out var y, out var z, out var w);
        X = x;
        Y = y;
        Z = z;
        W = w;
        }

    /// <inheritdoc
    /// cref="Quaternion{T}.Quaternion(Matrix3x4{T})"/>
    public Quaternion(Matrix4<T> matrix) {
        Matrix.CreateQuaternionComponentsFromMatrix(matrix, out var x, out var y, out var z, out var w);
        X = x;
        Y = y;
        Z = z;
        W = w;
        }

    /// <summary> Creates a new <see cref="Quaternion{T}"/> from the given <paramref name="pitch"/>, <paramref name="yaw"/>, and <paramref name="roll"/>.
    /// </summary>
    /// <remarks> Pitch (⮁ / rotation on X-axis) is applied first, followed by yaw (⮂ / rotation on Y-axis) and then Roll (⭮ / rotation on Z-axis)
    /// </remarks>
    /// <param name="pitch">The angle that the quaternion rotates upwards relative to the subject's normal.</param>
    /// <param name="yaw">The angle that the quaternion rotates to the right relative to the subject's normal.</param>
    /// <param name="roll">The angle that the quaternion rotates to the clockwise relative to the subject's normal.</param>
    public Quaternion(Angle<T> pitch, Angle<T> yaw, Angle<T> roll) : this(pitch.Normalize().Radians, yaw.Normalize().Radians, roll.Normalize().Radians) { }

    private Quaternion(T pitch, T yaw, T roll) {
        var oneHalf = GenericNumbers<T>.OneHalf;
        var halfPitch = pitch * oneHalf;
        var halfYaw = yaw * oneHalf;
        var halfRoll = roll * oneHalf;
        var (s, c) = SIMD.SinCos(halfPitch, halfYaw, halfRoll);
        X = c.Y * s.X * c.Z + s.Y * c.X * s.Z;
        Y = s.Y * c.X * c.Z - c.Y * s.X * s.Z;
        Z = c.Y * c.X * s.Z - s.Y * s.X * c.Z;
        W = c.Y * c.X * c.Z + s.Y * s.X * s.Z;
        }

    /// <inheritdoc
    /// cref="Quaternion{T}.Quaternion(Vector3{T}, Angle{T})"/>
    public Quaternion(Direction<T> axis, Angle<T> angle) : this(axis.Vector, angle) { }

    /// <summary> Creates a new <see cref="Quaternion{T}"/> representing the rotation by the given <paramref name="angle"/> around an <paramref name="axis"/>.
    /// </summary>
    [BunnyAttributes.SIMDCandidate]
    public Quaternion(Vector3<T> axis, Angle<T> angle) {
        Debug.Assert(
        condition: ValueIsNormalized(axis.MagnitudeSquared),
        message: $"The {nameof(axis)} must be normalized ({axis})."
        );
        var half = angle.Normalize() * GenericNumbers<T>.OneHalf;
        var vector = half.Sin * axis;
        X = vector.X;
        Y = vector.Y;
        Z = vector.Z;
        W = half.Cos;
        }

    #endregion Constructors

    #region Factories

    /// <inheritdoc
    /// cref="Quaternion{T}.Quaternion(Angle{T}, Angle{T}, Angle{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Quaternion<T> FromPitchYawRoll(T pitch, T yaw, T roll) {
        return new(pitch, yaw, roll);
        }

    /// <inheritdoc
    /// cref="Quaternion{T}.Quaternion(Angle{T}, Angle{T}, Angle{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Quaternion<T> FromPitchYawRoll(Angle<T> pitch, Angle<T> yaw, Angle<T> roll) {
        return new(pitch, yaw, roll);
        }

    /// <summary> Creates a new <see cref="Quaternion{T}"/> from the given <paramref name="pitch"/>, <paramref name="roll"/>, and <paramref name="yaw"/>.
    /// </summary>
    /// <remarks> Pitch (⮁ / rotation on X-axis) is applied first, followed by Roll (⭮ / rotation on Z-axis) and then yaw (⮂ / rotation on Y-axis)
    /// </remarks>
    /// <inheritdoc cref="Quaternion{T}.Quaternion(Angle{T}, Angle{T}, Angle{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Quaternion<T> FromPitchRollYaw(T pitch, T roll, T yaw) {
        var oneHalf = GenericNumbers<T>.OneHalf;
        var halfPitch = pitch * oneHalf;
        var halfRoll = roll * oneHalf;
        var halfYaw = yaw * oneHalf;
        var (s, c) = SIMD.SinCos(halfPitch, halfYaw, halfRoll);
        return new(
            w: c.Y * c.Z * c.X - s.Y * s.Z * s.X,
            x: c.Y * c.Z * s.X + s.Y * s.Z * c.X,
            y: s.Y * c.Z * c.X + c.Y * s.Z * s.X,
            z: c.Y * s.Z * c.X - s.Y * c.Z * s.X
            );
        }

    /// <inheritdoc cref="FromPitchRollYaw(T, T, T)"/>
    /// <inheritdoc cref="Quaternion{T}.Quaternion(Angle{T}, Angle{T}, Angle{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Quaternion<T> FromPitchRollYaw(Angle<T> pitch, Angle<T> roll, Angle<T> yaw) {
        return FromPitchRollYaw(pitch.Normalize().Radians, roll.Normalize().Radians, yaw.Normalize().Radians);
        }

    /// <inheritdoc
    /// cref="Quaternion{T}.Quaternion(Vector3{T}, Angle{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Quaternion<T> FromAxisAngle(Direction<T> axis, Angle<T> angle) {
        return new(axis, angle);
        }

    /// <inheritdoc
    /// cref="Quaternion{T}.Quaternion(Vector3{T}, Angle{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Quaternion<T> FromAxisAngle(Vector3<T> axis, Angle<T> angle) {
        return new(axis, angle);
        }

    /// <summary> An identity representing no rotation yet is normalized.
    /// </summary>
    public static ref readonly Quaternion<T> Identity {
        get => ref _identity;
        }

    /// <inheritdoc
    /// cref="FromLookAt(Direction{T}, Vector3{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Quaternion<T> FromLookAt(Direction<T> towards) {
        return FromLookAt(towards, Vector3<T>.Up);
        }

    /// <inheritdoc
    /// cref="FromLookAt(Vector3{T}, Vector3{T}, Vector3{T})" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Quaternion<T> FromLookAt(Vector3<T> source, Vector3<T> target) {
        return FromLookAt(source, target, Vector3<T>.Up);
        }

    /// <inheritdoc
    /// cref="FromLookAt(Vector3{T}, Vector3{T})"/>
    /// <param name="towards"> The direction to look towards.</param>
    /// <param name="upVector"><inheritdoc cref="FromLookAt(Vector3{T}, Vector3{T})"/></param>
    public static Quaternion<T> FromLookAt(Direction<T> towards, Vector3<T> upVector) {
        var forward = towards.UnwrapVector.Normalize();
        var cross = upVector.Cross(forward);
        if (cross.MagnitudeSquared < Epsilon<T>.Strict)
            cross = Vector3<T>.Right.Cross(forward);
        var right = cross.Normalize();
        var up = forward.Cross(right);
        var matrix = new Matrix3<T>(
            rowA: right,
            rowB: up,
            rowC: forward
            );
        return new(matrix);
        }

    /// <inheritdoc cref="IRotation{TRotation, TDirection, TVector, T}.FromLookAt(TVector, TVector)"/>
    /// <param name="source"><inheritdoc/></param>
    /// <param name="target"><inheritdoc/></param>
    /// <param name="upVector"> The direction that should be treated as upwards, if not <see cref="Vector3{T}.Up"/>. This is useful when the <paramref name="target"/> argument is nearly or exactly upwards relative to the <paramref name="source"/> argument. </param>
    public static Quaternion<T> FromLookAt(Vector3<T> source, Vector3<T> target, Vector3<T> upVector) {
        Vector3<T> delta;
#if MATRIX_LEFTHANDED_COORDINATE_SYSTEM
        delta = target - source;
#elif MATRIX_RIGHTHANDED_COORDINATE_SYSTEM
        delta = source - target;
#else
        throw new ApplicationException(Matrix.Docs.BaseMessage);
#endif
        if (delta.MagnitudeSquared < Epsilon<T>.Strict)
            return Identity;
        return FromLookAt(Direction<T>.CreatePreNormalized(delta), upVector); // the normalization will happen in the factory
        }

    /// <inheritdoc cref="FromToRotation(Direction{T}, Direction{T})"/>
    ///<remarks> The input vectors <b><u>must</u> both be normalized.</b>
    ///</remarks>
    public static Quaternion<T> FromToRotation(Vector3<T> from, Vector3<T> to) {
        Debug.Assert(
            condition: ValueIsNormalized(from.MagnitudeSquared),
            message: $"{nameof(from)} not normalized (length = {from.MagnitudeSquared})"
            );
        Debug.Assert(
            condition: ValueIsNormalized(to.MagnitudeSquared),
            message: $"{nameof(to)} not normalized (length = {to.MagnitudeSquared})"
            );
        var dot = Vector3<T>.Dot(from, to);
        if (dot <= T.NegativeOne + Epsilon<T>.Strict) {
            var axis = Vector3<T>.Cross(from, Vector3<T>.Up);
            if (axis.MagnitudeSquared < Epsilon<T>.Strict)
                axis = Vector3<T>.Cross(from, Vector3<T>.Right);
            return new Quaternion<T>(axis.Normalize(), T.Zero);
            }
        var cross = Vector3<T>.Cross(from, to);
        return new Quaternion<T>(
            vector: cross,
            scalar: T.Sqrt(from.MagnitudeSquared * to.MagnitudeSquared) + dot
            ).Normalize();
        }

    /// <inheritdoc
    /// cref="FromToRotation(Direction{T}, Direction{T})"/>
    ///<remarks> The input vectors <b><u>must</u> both be normalized.</b>
    ///</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Quaternion<T> FromToRotation(Direction<T> from, Direction<T> to) {
        return FromToRotation(from.Vector, to.Vector);
        }

    /// <summary> Creates a new <see cref="Quaternion{T}"/> from a <see cref="Vector4{T}"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Quaternion<T> FromVector4(Vector4<T> vector) {
        return new(vector);
        }

    public static Quaternion<T> Zero { get; } = Identity;

    #endregion Factories

    #region Concatenation

    /// <summary> Creates a quaternion representing a rotation by the <paramref name="left"/> quaternion followed by a rotation by <paramref name="right"/> quaternion.
    ///  </summary>
    /// <remarks> <b>Note:</b> The order of the quaternions matters; inputs in flipped order <em>usually</em> yield different results.
    /// </remarks>
    [BunnyAttributes.SIMDCandidate]
    public static Quaternion<T> Concatenate(Quaternion<T> left, Quaternion<T> right) {
        var (x1, y1, z1, w1) = left;
        var (x2, y2, z2, w2) = right;
        return new(
            x: (x2 * w1 + x1 * w2) + (y2 * z1 - z2 * y1),
            y: (y2 * w1 + y1 * w2) + (z2 * x1 - x2 * z1),
            z: (z2 * w1 + z1 * w2) + (x2 * y1 - y2 * x1),
            w: (w2 * w1 - x2 * x1) - (y2 * y1 + z2 * z1)
            );
        }

    /// <summary> Creates a quaternion representing a rotation by <see langword="this"/> quaternion followed by a rotation by the <paramref name="other"/> quaternion.
    ///  </summary>
    /// <remarks><inheritdoc cref="Concatenate(Quaternion{T}, Quaternion{T})"/>
    ///  </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Quaternion<T> Concatenate(Quaternion<T> other) {
        return Concatenate(this, other);
        }

    #endregion Concatenation

    #region Deconstruction

    /// <summary> Deconstructs this quaternion.
    /// </summary>
    public readonly void Deconstruct(out T x, out T y, out T z, out T w) {
        x = X;
        y = Y;
        z = Z;
        w = W;
        }

    #endregion Deconstruction

    #region Dot

    public static T Dot(in Quaternion<T> a, in Quaternion<T> b) {
        if (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float))
            return Vector128.Dot(
                Unsafe.As<Quaternion<T>, Vector128<T>>(ref Unsafe.AsRef(in a)),
                Unsafe.As<Quaternion<T>, Vector128<T>>(ref Unsafe.AsRef(in b))
                );
        else if (Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double))
            return Vector256.Dot(
                Unsafe.As<Quaternion<T>, Vector256<T>>(ref Unsafe.AsRef(in a)),
                Unsafe.As<Quaternion<T>, Vector256<T>>(ref Unsafe.AsRef(in b))
                );
        else if (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && typeof(T) == typeof(Half))
            return Vector64.Dot(
                Unsafe.As<Quaternion<T>, Vector64<T>>(ref Unsafe.AsRef(in a)),
                Unsafe.As<Quaternion<T>, Vector64<T>>(ref Unsafe.AsRef(in b))
                );
        else
            return a.X * b.X + a.Y * b.Y + a.Z * b.Z + a.W * b.W;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly T Dot(in Quaternion<T> other) {
        return Dot(in this, in other);
        }

    #endregion Dot

    #region Equatability

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(Quaternion<T> a, Quaternion<T> b) {
        return a.Equals(b) == false;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(Quaternion<T> a, Quaternion<T> b) {
        return a.Equals(b);
        }

    [BunnyAttributes.SIMDCandidate]
    public readonly bool Equals(Quaternion<T> other) {
        return X == other.X && Y == other.Y && Z == other.Z && W == other.W;
        }

    public override readonly bool Equals(object? obj) {
        return obj is Quaternion<T> quaternion && quaternion.Equals(this);
        }

    [BunnyAttributes.SIMDCandidate]
    public override readonly int GetHashCode() {
        return HashCode.Combine(X.GetHashCode(), Y.GetHashCode(), Z.GetHashCode(), W.GetHashCode());
        }

    #endregion Equatability

    #region Euler Angles

    /// <summary> The pitch (⮁ / rotation on X-axis), yaw (⮂ / rotation on Y-axis), and Roll (⭮ / rotation on Z-axis) of this <paramref name="quaternion"/>.
    /// </summary>
    /// <remarks> The quaternion decodes rotation pitch-yaw-roll order.
    /// </remarks>
    [BunnyAttributes.SIMDCandidate]
    [BunnyAttributes.Planned("Potential improvements via https://pmc.ncbi.nlm.nih.gov/articles/PMC9648712/ or https://journals.plos.org/plosone/article?id=10.1371/journal.pone.0276302")]
    public static (T Pitch, T Yaw, T Roll) GetPitchYawRoll(in Quaternion<T> quaternion) {
        (var x, var y, var z, var w) = quaternion;
        var sqx = x * x;
        var sqy = y * y;
        var sqz = z * z;
        var sqw = w * w;
        var dot = sqx + sqy + sqz + sqw;
        var two = GenericNumbers<T>.Two;
        T pitch, yaw, roll;

        var test = w * x - y * z;
        if (test > (GenericNumbers<T>.OneHalf - Epsilon<T>.Lenient) * dot) {
            pitch = Angle<T>.Up.Radians;
            yaw = two * T.Atan2(y, w);
            roll = T.Zero;
            }
        else if (test < -(GenericNumbers<T>.OneHalf - Epsilon<T>.Lenient) * dot) {
            pitch = -Angle<T>.Up.Radians;
            yaw = two * T.Atan2(y, w);
            roll = T.Zero;
            }
        else {
            pitch = T.Asin(two * test / dot);
            yaw = T.Atan2(two * (x * z + w * y), sqw - sqx - sqy + sqz);
            roll = T.Atan2(two * (x * y + w * z), sqw - sqx + sqy - sqz);
            }
        return (pitch, yaw, roll);
        }

    /// <summary> The pitch (⮁ / rotation on X-axis), Roll (⭮ / rotation on Z-axis), and yaw (⮂ / rotation on Y-axis) of this <paramref name="quaternion"/>.
    /// </summary>
    /// <remarks> The quaternion decodes rotation pitch-roll-yaw order.
    /// </remarks>
    [BunnyAttributes.SIMDCandidate]
    [BunnyAttributes.Citation("https://www.euclideanspace.com/maths/geometry/rotations/conversions/quaternionToEuler/index.htm")]
    public static (T Pitch, T Roll, T Yaw) GetPitchRollYaw(in Quaternion<T> quaternion) {
        (var x, var y, var z, var w) = quaternion;
        var sqx = x * x;
        var sqy = y * y;
        var sqz = z * z;
        var sqw = w * w;
        var dot = sqx + sqy + sqz + sqw;
        var two = GenericNumbers<T>.Two;
        T pitch, yaw, roll;
        var test = x * y + z * w;
        if (test > (GenericNumbers<T>.OneHalf - Epsilon<T>.Lenient) * dot) {
            roll = Angle<T>.Up.Radians;
            pitch = T.Zero;
            yaw = two * T.Atan2(x, w);
            }
        else if (test < -(GenericNumbers<T>.OneHalf - Epsilon<T>.Lenient) * dot) {
            roll = -Angle<T>.Up.Radians;
            pitch = T.Zero;
            yaw = -two * T.Atan2(x, w);
            }
        else {
            roll = T.Asin(two * test / dot);
            pitch = T.Atan2(two * (w * x - y * z), sqw - sqx + sqy - sqz);
            yaw = T.Atan2(two * (w * y - x * z), sqw + sqx - sqy - sqz);
            }
        return (pitch, roll, yaw);
        }

    /// <inheritdoc
    /// cref="GetPitchYawRoll(in Quaternion{T})"/>
    public readonly (T Pitch, T Yaw, T Roll) PitchYawRoll {
        get => GetPitchYawRoll(this);
        }

    /// <inheritdoc
    /// cref="GetPitchRollYaw(in Quaternion{T})"/>
    public readonly (T Pitch, T Roll, T Yaw) PitchRollYaw {
        get => GetPitchRollYaw(this);
        }

    #endregion Euler Angles

    #region Interpolation

    [BunnyAttributes.SIMDCandidate]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Quaternion<T> Lerp(Quaternion<T> from, Quaternion<T> to, T amount) {
        return FromVector4(LerpUnchecked(from, to, amount).UnwrapVector.Normalize());
        }

    /// <remarks> For <see cref="Quaternion{T}">Quaternions</see>, this is identical to <see cref="Lerp(Quaternion{T}, Quaternion{T}, T)"/>.
    /// </remarks>
    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Quaternion<T> LerpUnchecked(Quaternion<T> from, Quaternion<T> to, T amount) {
        var inverse = T.One - amount;
        Vector4<T> a, b, c;
        a = from.Vector4;
        b = to.Vector4;
        c = a.Dot(b) >= T.Zero
            ? a * inverse + b * amount
            : a * inverse - b * amount;
        return FromVector4(c);
        }

    [BunnyAttributes.Citation("DirectXMath's XMQuaternionSlerpV")]
    public static Quaternion<T> Slerp(Quaternion<T> from, Quaternion<T> to, T amount) {
        if (Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double)) {
            var cosOmega = Vector256.Create(Quaternion<T>.Dot(from, to));
            var control = Vector256.LessThan(cosOmega, Vector256.Create(T.Zero));

            var sign = Vector256.ConditionalSelect(control, Vector256.Create(T.NegativeOne), Vector256.Create(T.One));

            cosOmega *= sign;
            control = Vector256.LessThan(cosOmega, Vector256.Create(T.One - Epsilon<T>.Strict));

            var sinOmega = Vector256.Sqrt(Vector256.Create(T.One) - (cosOmega * cosOmega));
            var omega = sinOmega.Atan2(cosOmega);

            var vec0 = (Vector256.Xor(Vector256.Create(amount).AsDouble(), Vector256.Create(double.NegativeZero, 0d, 0d, 0d)) + Vector256.Create(1d, 0d, 0d, 0d)).As<double, T>();

            var invSinOmega = Vector256<T>.One / sinOmega;
            var slerp0 = Vector256.ConditionalSelect(
                condition: control,
                left: Vector256.Sin((vec0 * omega).As<T, double>()).As<double, T>() * invSinOmega,
                right: vec0
                );

            var slerp1 = SplatY(slerp0);
            slerp0 = SplatX(slerp0);
            slerp1 *= sign;
            return Unsafe.BitCast<Vector256<T>, Quaternion<T>>((Unsafe.BitCast<Quaternion<T>, Vector256<T>>(to) * slerp1) + (Unsafe.BitCast<Quaternion<T>, Vector256<T>>(from) * slerp0));
            }
        else if (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float)) {
            var cosOmega = Vector128.Create(Quaternion<T>.Dot(from, to));
            var control = Vector128.LessThan(cosOmega, Vector128.Create(T.Zero));

            var sign = Vector128.ConditionalSelect(control, Vector128.Create(T.NegativeOne), Vector128.Create(T.One));

            cosOmega *= sign;
            control = Vector128.LessThan(cosOmega, Vector128.Create(T.One - Epsilon<T>.Strict));

            var sinOmega = Vector128.Sqrt(Vector128.Create(T.One) - (cosOmega * cosOmega));
            var omega = sinOmega.Atan2(cosOmega);

            var vec0 = (Vector128.Xor(Vector128.Create(amount).AsSingle(), Vector128.Create(float.NegativeZero, 0f, 0f, 0f)) + Vector128.Create(1f, 0f, 0f, 0f)).As<float, T>();

            var invSinOmega = Vector128<T>.One / sinOmega;
            var slerp0 = Vector128.ConditionalSelect(
                condition: control,
                left: Vector128.Sin((vec0 * omega).As<T, float>()).As<float, T>() * invSinOmega,
                right: vec0
                );

            var slerp1 = SplatY(slerp0);
            slerp0 = SplatX(slerp0);
            slerp1 *= sign;
            return Unsafe.BitCast<Vector128<T>, Quaternion<T>>((Unsafe.BitCast<Quaternion<T>, Vector128<T>>(to) * slerp1) + (Unsafe.BitCast<Quaternion<T>, Vector128<T>>(from) * slerp0));
            }
        else {
            T a, b;
            var dot = Dot(from, to);
            bool flag = false;
            if (dot < T.Zero) {
                flag = true;
                dot *= T.NegativeOne;
                }
            if (T.Abs(T.One - dot) < Epsilon<T>.Strict) {
                a = T.One - amount;
                b = flag ? -amount : amount;
                }
            else {
                var acos = T.Acos(dot);
                var invs = T.One / T.Sin(acos);
                a = T.Sin((T.One - amount) * acos) * invs;
                b = T.Sin(amount * acos) * invs * (flag ? T.NegativeOne : T.One);
                }
            return new Quaternion<T>(
                x: a * from.X + b * to.X,
                y: a * from.Y + b * to.Y,
                z: a * from.Z + b * to.Z,
                w: a * from.W + b * to.W
                );
            }
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Quaternion<T> Lerp(Quaternion<T> other, T amount) {
        return Lerp(this, other, amount);
        }

    /// <inheritdoc
    /// cref="LerpUnchecked(Quaternion{T}, Quaternion{T}, T)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Quaternion<T> LerpUnchecked(Quaternion<T> other, T amount) {
        return LerpUnchecked(this, other, amount);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Quaternion<T> Slerp(Quaternion<T> other, T amount) {
        return Slerp(this, other, amount);
        }

    #endregion Interpolation

    #region Inversion

    /// <summary><inheritdoc/></summary>
    /// <returns> A quaternion such that rotating a subject by this <paramref name="quaternion"/> followed by rotating it by the output quaternion would yield an <see cref="Identity"/> quaterion (functionally resulting in no rotation).
    /// </returns>
    public static Quaternion<T> Invert(Quaternion<T> quaternion) {
        if (Vector256.IsHardwareAccelerated && Vector256<double>.IsSupported && typeof(T) == typeof(double)) {
            var vec = Unsafe.BitCast<Quaternion<T>, Vector256<T>>(quaternion);
            var dot = Vector256.Sum(vec * vec);
            var adj = Vector256.Create(-1d, -1d, -1d, 1d);
            vec *= Unsafe.BitCast<Vector256<double>, Vector256<T>>(adj);
            vec /= Vector256.Create(dot);
            return Unsafe.BitCast<Vector256<T>, Quaternion<T>>(vec);
            }
        else if (Vector128.IsHardwareAccelerated && Vector128<float>.IsSupported && typeof(T) == typeof(float)) {
            var vec = Unsafe.BitCast<Quaternion<T>, Vector128<T>>(quaternion);
            var dot = Vector128.Sum(vec * vec);
            var adj = Vector128.Create(-1f, -1f, -1f, 1f);
            vec *= Unsafe.BitCast<Vector128<float>, Vector128<T>>(adj);
            vec /= Vector128.Create(dot);
            return Unsafe.BitCast<Vector128<T>, Quaternion<T>>(vec);
            }
        else if (Vector64.IsHardwareAccelerated && Vector64<Half>.IsSupported && typeof(T) == typeof(Half)) {
            var vec = Unsafe.BitCast<Quaternion<T>, Vector64<T>>(quaternion);
            var dot = Vector64.Sum(vec * vec);
            var adj = Vector64.Create((ushort)0xBC00, 0xBC00, 0xBC00, 0x3C00); // unsafe cast to Half values of -1, -1, -1, 1
            vec *= Unsafe.BitCast<Vector64<ushort>, Vector64<T>>(adj);
            vec /= Vector64.Create(dot);
            return Unsafe.BitCast<Vector64<T>, Quaternion<T>>(vec);
            }
        else {
            var invDot = T.One / Dot(quaternion, quaternion);
            return new Quaternion<T>(
                x: -quaternion.X * invDot,
                y: -quaternion.Y * invDot,
                z: -quaternion.Z * invDot,
                w: +quaternion.W * invDot
                );
            }
        }

    /// <summary><inheritdoc/></summary>
    /// <remarks> Adjusts the quaternion such that multiplying its new value by its original value would yield an identity quaternion (functionally resulting in no rotation).
    /// </remarks>
    public static void Invert(ref Quaternion<T> quaternion) {
        if (Vector256.IsHardwareAccelerated && Vector256<double>.IsSupported && typeof(T) == typeof(double)) {
            var vec = Unsafe.BitCast<Quaternion<T>, Vector256<T>>(quaternion);
            var dot = Vector256.Sum(vec * vec);
            var adj = Vector256.Create(-1d, -1d, -1d, 1d);
            vec *= Unsafe.BitCast<Vector256<double>, Vector256<T>>(adj);
            vec /= Vector256.Create(dot);
            quaternion = Unsafe.BitCast<Vector256<T>, Quaternion<T>>(vec);
            }
        else if (Vector128.IsHardwareAccelerated && Vector128<float>.IsSupported && typeof(T) == typeof(float)) {
            var vec = Unsafe.BitCast<Quaternion<T>, Vector128<T>>(quaternion);
            var dot = Vector128.Sum(vec * vec);
            var adj = Vector128.Create(-1f, -1f, -1f, 1f);
            vec *= Unsafe.BitCast<Vector128<float>, Vector128<T>>(adj);
            vec /= Vector128.Create(dot);
            quaternion = Unsafe.BitCast<Vector128<T>, Quaternion<T>>(vec);
            }
        else if (Vector64.IsHardwareAccelerated && Vector64<Half>.IsSupported && typeof(T) == typeof(Half)) {
            var vec = Unsafe.BitCast<Quaternion<T>, Vector64<T>>(quaternion);
            var dot = Vector64.Sum(vec * vec);
            var adj = Vector64.Create((ushort)0xBC00, 0xBC00, 0xBC00, 0x3C00); // unsafe cast to Half values of -1, -1, -1, 1
            vec *= Unsafe.BitCast<Vector64<ushort>, Vector64<T>>(adj);
            vec /= Vector64.Create(dot);
            quaternion = Unsafe.BitCast<Vector64<T>, Quaternion<T>>(vec);
            }
        else {
            var invDot = T.One / Dot(quaternion, quaternion);
            quaternion = new Quaternion<T>(
                x: -quaternion.X * invDot,
                y: -quaternion.Y * invDot,
                z: -quaternion.Z * invDot,
                w: +quaternion.W * invDot
                );
            }
        }

    /// <inheritdoc
    /// cref="Invert(Quaternion{T})"/>
    public readonly Quaternion<T> Invert() {
        return Invert(this);
        }

    #endregion Inversion

    #region Negation

    [BunnyAttributes.SIMDCandidate]
    public static void Negate(ref Quaternion<T> quaternion) {
        quaternion.X = -quaternion.X;
        quaternion.Y = -quaternion.Y;
        quaternion.Z = -quaternion.Z;
        quaternion.W = -quaternion.W;
        }

    [BunnyAttributes.SIMDCandidate]
    public static Quaternion<T> Negate(Quaternion<T> quaternion) {
        return new Quaternion<T>(
            x: -quaternion.X,
            y: -quaternion.Y,
            z: -quaternion.Z,
            w: -quaternion.W
            );
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Quaternion<T> Negate() {
        return Negate(this);
        }

    #endregion Negation

    #region Normalization

    public static Quaternion<T> Normalize(Quaternion<T> quaternion) {
        if (Vector256.IsHardwareAccelerated && Vector256<double>.IsSupported && typeof(T) == typeof(double)) {
            var vec = Unsafe.BitCast<Quaternion<T>, Vector256<T>>(quaternion);
            var len = T.Sqrt(Vector256.Sum(vec * vec));
            vec /= Vector256.Create(len);
            return Unsafe.BitCast<Vector256<T>, Quaternion<T>>(vec);
            }
        else if (Vector128.IsHardwareAccelerated && Vector128<float>.IsSupported && typeof(T) == typeof(float)) {
            var vec = Unsafe.BitCast<Quaternion<T>, Vector128<T>>(quaternion);
            var len = T.Sqrt(Vector128.Sum(vec * vec));
            vec /= Vector128.Create(len);
            return Unsafe.BitCast<Vector128<T>, Quaternion<T>>(vec);
            }
        else if (Vector64.IsHardwareAccelerated && Vector64<Half>.IsSupported && typeof(T) == typeof(Half)) {
            var vec = Unsafe.BitCast<Quaternion<T>, Vector64<T>>(quaternion);
            var len = T.Sqrt(Vector64.Sum(vec * vec));
            vec /= Vector64.Create(len);
            return Unsafe.BitCast<Vector64<T>, Quaternion<T>>(vec);
            }
        else {
            var len = T.One / T.Sqrt(Dot(quaternion, quaternion));
            return new(
                x: quaternion.X * len,
                y: quaternion.Y * len,
                z: quaternion.Z * len,
                w: quaternion.W * len
                );
            }
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Normalize(ref Quaternion<T> quaternion) {
        if (Vector256.IsHardwareAccelerated && Vector256<double>.IsSupported && typeof(T) == typeof(double)) {
            var vec = Unsafe.BitCast<Quaternion<T>, Vector256<T>>(quaternion);
            var len = T.Sqrt(Vector256.Sum(vec * vec));
            vec /= Vector256.Create(len);
            quaternion = Unsafe.BitCast<Vector256<T>, Quaternion<T>>(vec);
            }
        else if (Vector128.IsHardwareAccelerated && Vector128<float>.IsSupported && typeof(T) == typeof(float)) {
            var vec = Unsafe.BitCast<Quaternion<T>, Vector128<T>>(quaternion);
            var len = T.Sqrt(Vector128.Sum(vec * vec));
            vec /= Vector128.Create(len);
            quaternion = Unsafe.BitCast<Vector128<T>, Quaternion<T>>(vec);
            }
        else if (Vector64.IsHardwareAccelerated && Vector64<Half>.IsSupported && typeof(T) == typeof(Half)) {
            var vec = Unsafe.BitCast<Quaternion<T>, Vector64<T>>(quaternion);
            var len = T.Sqrt(Vector64.Sum(vec * vec));
            vec /= Vector64.Create(len);
            quaternion = Unsafe.BitCast<Vector64<T>, Quaternion<T>>(vec);
            }
        else {
            var len = T.One / T.Sqrt(Dot(quaternion, quaternion));
            quaternion = new(
                x: quaternion.X * len,
                y: quaternion.Y * len,
                z: quaternion.Z * len,
                w: quaternion.W * len
                );
            }
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Quaternion<T> Normalize() {
        return Normalize(this);
        }

    #endregion Normalization

    #region Operator Functions

    [BunnyAttributes.SIMDCandidate]
    public static Quaternion<T> Add(Quaternion<T> left, Quaternion<T> right) {
        return new(left.Vector + right.Vector, left.W + right.W);
        }

    /// <returns> The rotation quaternion needed to go from one orientation quaternion to the other.
    /// </returns>
    [BunnyAttributes.SIMDCandidate]
    public static Quaternion<T> Divide(Quaternion<T> left, Quaternion<T> right) {
        var x = left.X;
        var y = left.Y;
        var z = left.Z;
        var w = left.W;
        var s = new Vector4<T>(-right.Vector, right.W) / Dot(right, right);
        return new Quaternion<T>(
            x: x * s.W + s.X * w + (y * s.Z - z * s.Y),
            y: y * s.W + s.Y * w + (z * s.X - x * s.Z),
            z: z * s.W + s.Z * w + (x * s.Y - y * s.X),
            w: w * s.W - (x * s.X + y * s.Y + z * s.Z)
            );
        }

    [BunnyAttributes.SIMDCandidate]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Quaternion<T> Divide(Quaternion<T> quaternion, T factor) {
        return quaternion * (T.One / factor);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Quaternion<T> Multiply(Quaternion<T> left, Quaternion<T> right) {
        return Concatenate(left, right);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Quaternion<T> Multiply(Quaternion<T> quaternion, T factor) {
        return Slerp(Identity, quaternion, factor);
        }

    [BunnyAttributes.SIMDCandidate]
    public static Quaternion<T> Subtract(Quaternion<T> left, Quaternion<T> right) {
        return new(left.Vector - right.Vector, left.W - right.W);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Quaternion<T> Add(Quaternion<T> other) {
        return Add(this, other);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Quaternion<T> Divide(Quaternion<T> other) {
        return Divide(this, other);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Quaternion<T> Divide(T factor) {
        return Divide(this, factor);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Quaternion<T> Multiply(T factor) {
        return Multiply(this, factor);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Quaternion<T> Multiply(Quaternion<T> other) {
        return Multiply(this, other);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Quaternion<T> Subtract(Quaternion<T> other) {
        return Subtract(this, other);
        }

    #endregion Operator Functions

    #region Operators

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Quaternion<T> operator -(Quaternion<T> left, Quaternion<T> right) {
        return Subtract(left, right);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Quaternion<T> operator *(Quaternion<T> left, Quaternion<T> right) {
        return Multiply(left, right);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Quaternion<T> operator *(Quaternion<T> quaternion, T factor) {
        return Multiply(quaternion, factor);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Quaternion<T> operator /(Quaternion<T> left, Quaternion<T> right) {
        return Divide(left, right);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Quaternion<T> operator /(Quaternion<T> quaternion, T factor) {
        return Divide(quaternion, factor);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Quaternion<T> operator +(Quaternion<T> left, Quaternion<T> right) {
        return Add(left, right);
        }

    #endregion Operators

    #region Rotation

    /// <summary><inheritdoc/></summary>
    /// <remarks> For quaternions, this value will always fall in the range <c>[0, 0.5)</c>.
    /// </remarks>
    public readonly T RoundTrips {
        get => T.Acos(T.Min(T.Abs(W), T.One)) / T.Pi;
        }

    public static void Rotate(ref Direction<T> direction, Quaternion<T> rotation) {
        var vector = direction.Vector;
        rotation.Rotate(ref vector);
        direction.Vector = vector;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Direction<T> Rotate(Direction<T> direction, Quaternion<T> rotation) {
        return Direction<T>.CreatePreNormalized(Rotate(direction.Vector, rotation));
        }

    [BunnyAttributes.SIMDCandidate]
    public static Vector3<T> Rotate(Vector3<T> vector, Quaternion<T> rotation) {
        var x = (rotation.Y * vector.Z - rotation.Z * vector.Y);
        var y = (rotation.Z * vector.X - rotation.X * vector.Z);
        var z = (rotation.X * vector.Y - rotation.Y * vector.X);
        x += x;
        y += y;
        z += z;
        return new(
            x: vector.X + x * rotation.W + (rotation.Y * z - rotation.Z * y),
            y: vector.Y + y * rotation.W + (rotation.Z * x - rotation.X * z),
            z: vector.Z + z * rotation.W + (rotation.X * y - rotation.Y * x)
            );
        }

    /// <summary> Rotates the given <paramref name="vector"/> in-place with the given <paramref name="rotation"/>.
    /// </summary>
    [BunnyAttributes.SIMDCandidate]
    public static void Rotate(ref Vector3<T> vector, Quaternion<T> rotation) {
        var x = (rotation.Y * vector.Z - rotation.Z * vector.Y);
        var y = (rotation.Z * vector.X - rotation.X * vector.Z);
        var z = (rotation.X * vector.Y - rotation.Y * vector.X);
        x += x;
        y += y;
        z += z;
        vector.X = vector.X + x * rotation.W + (rotation.Y * z - rotation.Z * y);
        vector.Y = vector.Y + y * rotation.W + (rotation.Z * x - rotation.X * z);
        vector.Z = vector.Z + z * rotation.W + (rotation.X * y - rotation.Y * x);
        }

    /// <summary> Rotates the given <paramref name="direction"/> in-place with <see langword="this"/> quaternion.
    /// </summary>
    public readonly void Rotate(ref Direction<T> direction) {
        var vector = direction.Vector;
        Rotate(ref vector);
        direction.Vector = vector;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Direction<T> Rotate(Direction<T> direction) {
        return Direction<T>.CreatePreNormalized(Rotate(direction.Vector));
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Vector3<T> Rotate(Vector3<T> vector) {
        return Rotate(vector, this);
        }

    /// <summary> Rotates the given <paramref name="vector"/> in-place with <see langword="this"/> quaternion.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly void Rotate(ref Vector3<T> vector) {
        Rotate(ref vector, this);
        }

    #endregion Rotation

    #region Strings

    public override readonly string ToString() {
        var (Pitch, Yaw, Roll) = PitchYawRoll;
        return $"[⮁{Angle<T>.Rad2Deg(Pitch)}° ⮂{Angle<T>.Rad2Deg(Yaw)}° ⭮{Angle<T>.Rad2Deg(Roll)}°]";
        }

    public readonly string ToString(byte roundingDigits) {
        var (Pitch, Yaw, Roll) = PitchYawRoll;
        return $"[⮁{T.Round(Angle<T>.Rad2Deg(Pitch), roundingDigits)}° ⮂{T.Round(Angle<T>.Rad2Deg(Yaw), roundingDigits)}° ⭮{T.Round(Angle<T>.Rad2Deg(Roll), roundingDigits)}°]";
        }

    public readonly string ToString(byte digits, int integerLength, int paddingLength) {
        var (Pitch, Yaw, Roll) = this.PitchYawRoll;
        var pitch = Angle<T>.Rad2Deg(Pitch).Stringify(digits, integerLength, paddingLength);
        var yaw = Angle<T>.Rad2Deg(Yaw).Stringify(digits, integerLength, paddingLength);
        var roll = Angle<T>.Rad2Deg(Roll).Stringify(digits, integerLength, paddingLength);
        return $"[⮁{pitch}° ⮂{yaw}° ⭮{roll}°]";
        }

    #endregion Strings

    #region Vectors

    public static int VectorLength => 4;

    public Vector4<T> UnwrapVector => Unsafe.As<Quaternion<T>, Vector4<T>>(ref this);

    /// <summary> A vector representing this quaternion's vector part (X, Y, Z).
    /// </summary>
    public Vector3<T> Vector {
        readonly get => XYZ;
        set => XYZ = value;
        }

    /// <summary> A vector representing this quaternion's vector and scalar parts.
    /// </summary>
    public Vector4<T> Vector4 {
        readonly get => new(X, Y, Z, W);
        set {
            X = value.X;
            Y = value.Y;
            Z = value.Z;
            W = value.W;
            }
        }

    public static ref Vector4<T> AsVector(ref Quaternion<T> wrapped) {
        return ref Unsafe.As<Quaternion<T>, Vector4<T>>(ref wrapped);
        }

    public static ref Quaternion<T> AsWrapper(ref Vector4<T> vector) {
        return ref Unsafe.As<Vector4<T>, Quaternion<T>>(ref vector);
        }

    public static Quaternion<T> FromVector(Vector4<T> vector) {
        return new(vector);
        }

    #endregion Vectors
    }
