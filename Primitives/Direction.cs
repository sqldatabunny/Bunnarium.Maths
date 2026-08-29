using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Bunnarium.Maths.Primitives;

/// <summary> Represents a direction or orientation in 3D space. Defined as a unit vector (a <see cref="Vector3{T}">Vector3</see> with a <see cref="Vector3{T}.Magnitude">magnitude</see> of 1).
/// </summary>
[DebuggerDisplay("{ToString(), nq}")]
public struct Direction<T>
    : IDirection<Direction<T>, Quaternion<T>, Vector3<T>, T>
    , IVectorWrapper<Direction<T>, Vector3<T>, T>
    , IEquatable<Direction<T>>
    where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {

    #region Data

    Vector3<T> _vector;

    #endregion Data

    #region Constructors

    /// <summary> Creates a new <see cref="Direction{T}"/> instance with the direction of the specified <paramref name="vector"/>.
    /// </summary>
    public Direction(Vector3<T> vector) {
        _vector = vector.Normalize(T.One);
        }

    private Direction(Vector3<T> vector, bool hidden) {
        _vector = vector;
        }

    public static Direction<T> Forward => new(Vector3<T>.Forward);

    public static Direction<T> Right => new(Vector3<T>.Right);

    public static Direction<T> Up => new(Vector3<T>.Up);

    /// <summary> Creates a new <see cref="Direction{T}"/> instance with the direction of the specified <paramref name="vector"/> without normalizing it. Only use this when the input vector is already normalized.
    /// </summary>
    public static Direction<T> CreatePreNormalized(Vector3<T> vector) {
        return new(vector, hidden: true);
        }

    /// <summary> Creates a new <see cref="Direction{T}"/> instance with the direction of the specified <paramref name="x"/><paramref name="y"/><paramref name="z"/> vector without normalizing it. Only use this when the input vector is already normalized.
    /// </summary>
    public static Direction<T> CreatePreNormalized(T x, T y, T z) {
        return new(new(x, y, z), hidden: true);
        }

    public static Direction<T> FromVector(Vector3<T> vector) {
        return new(vector);
        }

    #endregion Constructors

    #region Vector Wrapper

    public static int VectorLength => 3;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref Vector3<T> AsVector(ref Direction<T> direction) {
        return ref Unsafe.As<Direction<T>, Vector3<T>>(ref direction);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref Direction<T> AsWrapper(ref Vector3<T> vector) {
        return ref Unsafe.As<Vector3<T>, Direction<T>>(ref vector);
        }

    public readonly Vector3<T> UnwrapVector {
        get => _vector;
        }

    public Vector3<T> Vector {
        readonly get => _vector;
        set => _vector = value.Normalize();
        }

    #endregion Vector Wrapper

    #region Dot

    public static T Dot(in Direction<T> a, in Direction<T> b) {
        return Vector3<T>.Dot(a.UnwrapVector, b.UnwrapVector);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly T Dot(in Direction<T> other) {
        return Dot(in this, in other);
        }

    #endregion Dot

    #region Flip

    public static Direction<T> Flip(Direction<T> direction) {
        return new Direction<T>(direction._vector.Negate());
        }

    public static void Flip(ref Direction<T> direction) {
        Vector3<T>.Negate(ref direction._vector);
        }

    public readonly Direction<T> Flip() {
        return Flip(this);
        }

    #endregion Flip

    #region Normalization

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Direction<T> Normalize(Direction<T> direction) {
        return new(direction.Vector.Normalize());
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Normalize(ref Direction<T> direction) {
        Vector3<T>.Normalize(ref direction._vector);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Direction<T> Normalize() {
        return Normalize(this);
        }

    #endregion Normalization

    #region Rotation

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Direction<T> Rotate(Quaternion<T> rotation, Direction<T> direction) {
        return new(rotation.Rotate(direction._vector));
        }

    public static Vector3<T> RotateAboutAxis(Vector3<T> point, Direction<T> axis, Angle<T> angle) {
        var quaternion = Quaternion<T>.FromAxisAngle(axis, angle); // the angle will be normalized down the stack, so there's no need to normalize the input here.
        return quaternion.Rotate(point);
        }

    public readonly Direction<T> Rotate(Quaternion<T> rotation) {
        return new(rotation.Rotate(_vector));
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Vector3<T> RotateAboutAxis(Vector3<T> point, Angle<T> angle) {
        return RotateAboutAxis(point, this, angle); // the angle will be normalized down the stack, so there's no need to normalize the input here.
        }

    #endregion Rotation

    #region Transformations

    /// <summary> The <see cref="Vector3{T}">vector</see> that is normal to this direction's <see cref="Vector">vector</see> (the <see cref="Vector3{T}.Cross(Vector3{T}, Vector3{T})">cross product</see> of this direction's vector and the <see cref="Vector3{T}.Up">up vector</see>).
    /// </summary>
    public readonly Direction<T> Normal {
        get {
            var cross = Vector.Cross(Vector3<T>.Up);
            if (cross.MagnitudeSquared < Epsilon<T>.Strict)
                cross = Vector.Cross(Vector3<T>.Right);
            return new(cross);
            }
        }

    #endregion Transformations

    #region Operators and Equatability

    public static Direction<T> operator -(Direction<T> target, Quaternion<T> rotation) {
        return Rotate(rotation.Invert(), target);
        }

    public static bool operator !=(Direction<T> target, Direction<T> other) {
        return target.Equals(other) == false;
        }

    public static Direction<T> operator +(Direction<T> target, Quaternion<T> rotation) {
        return Rotate(rotation, target);
        }

    public static bool operator ==(Direction<T> target, Direction<T> other) {
        return target.Equals(other);
        }

    public readonly bool Equals(Direction<T> other) {
        return Vector == other.Vector;
        }

    public override readonly bool Equals(object? obj) {
        return obj is Direction<T> direction && Equals(direction);
        }

    public override readonly int GetHashCode() {
        return _vector.GetHashCode();
        }

    #endregion Operators and Equatability

    #region Strings

    /// <inheritdoc
    /// cref="Angle{T}.SymbolicArrow"/>
    public readonly string Arrow => new Angle<T>(Vector.ToVector2).SymbolicArrow;

    public override readonly string ToString() {
        return $"[{Arrow} ⮂{_vector.X} ⮁{_vector.Y} ⭮{_vector.Z}]";
        }

    public readonly string ToString(byte digits, int integerLength, int paddingLength) {
        var r = _vector.Round(digits);
        return $"[{Arrow} ⮂{r.X.Stringify(digits, integerLength, paddingLength)} ⮁{r.Y.Stringify(digits, integerLength, paddingLength)} ⭮{r.Z.Stringify(digits, integerLength, paddingLength)}]";
        }

    #endregion Strings
    }
