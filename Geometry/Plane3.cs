using System.Runtime.CompilerServices;

namespace Bunnarium.Maths.Geometry;

/// <summary> Represents a plane in 3-dimensional space.
/// </summary>
/// <remarks> Note: This is a barebones and incomplete placeholder for the Plane3 type that will be released fully later with other geometry primitives. It's included now to support various 4-dimensional matrix factories.
/// </remarks>
public struct Plane3<T>
    // : IPlane<Plane3<T>, Vector3<T>, Direction<T>, Quaternion<T>, AABB3<T>, SpherePrimitive<T>, T>
    // , IPlane<Plane3<T>, Vector3<T>, Direction<T>, Quaternion<T>, Box3<T>, SpherePrimitive<T>, T>
    where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {

    #region Constructors

    /// <summary> Create a <see cref="Plane3{T}"/> from a normal vector and a distance from the origin.
    /// </summary>
    /// <remarks> <b>Note:</b> The plane's <paramref name="normal"/> is <u>not</u> automatically normalized on instantiation.
    /// </remarks>
    public Plane3(Vector3<T> normal, T distance) {
        _normal = normal;
        _distance = distance;
        }

    /// <summary> Create a <see cref="Plane3{T}"/> from a <see cref="Vector4{T}"/> such that the <see cref="Vector4{T}.XYZ"/> component represents the <see cref="Normal"/> and the <see cref="Vector4{T}.W"/> component represents the <see cref="Distance"/>.
    /// </summary>
    /// <remarks><inheritdoc cref="Plane3{T}.Plane3(Vector3{T}, T)"/>
    /// </remarks>
    public Plane3(Vector4<T> normalDistance) : this(normalDistance.XYZ, normalDistance.W) { }

    /// <summary> Create a <see cref="Plane3{T}"/> from a <see cref="Direction{T}">Direction</see> representing the <see cref="Normal">Normal</see> and a scalar representing the <see cref="Distance"/>.
    /// </summary>
    public Plane3(Direction<T> normal, T distance) : this(normal.Vector, distance) { }

    /// <summary> Create a <see cref="Plane3{T}"/> from a normal vector and a distance from the origin.
    /// </summary>
    /// <remarks><inheritdoc cref="Plane3{T}.Plane3(Vector3{T}, T)"/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Plane3<T> Create(Vector3<T> normal, T distance) {
        return new(normal, distance);
        }

    /// <summary> Create a <see cref="Plane3{T}"/> from a <see cref="Direction{T}"/> representing the <see cref="Normal">Normal</see> and a scalar representing the <see cref="Distance">Distance</see>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Plane3<T> Create(Direction<T> normal, T distance) {
        return new(normal, distance);
        }

    /// <summary> Create a <see cref="Plane3{T}"/> from a <see cref="Vector4{T}"/> such that the <see cref="Vector4{T}.XYZ"/> component represents the <see cref="Normal">Normal</see> and the <see cref="Vector4{T}.W"/> component represents the <see cref="Distance"/>.
    /// </summary>
    /// <remarks><inheritdoc cref="Plane3{T}.Plane3(Vector3{T}, T)"/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Plane3<T> Create(Vector4<T> normalDistance) {
        return new(normalDistance.XYZ, normalDistance.W);
        }

    /// <summary> Create a <see cref="Plane3{T}"/> from three points on the plane.
    /// </summary>
    /// <remarks><b>Note:</b> The plane's <see cref="Normal">Normal</see> vector <u>will</u> be normalized on instantiation.
    /// </remarks>
    public static Plane3<T> FromPoints(Vector3<T> a, Vector3<T> b, Vector3<T> c) {
        var normal = (b - a).Cross(c - a).Normalize();
        var distance = normal.Dot(a);
        return new(normal, distance);
        }

    #endregion Constructors

    #region Data

    T _distance;
    Vector3<T> _normal;

    public T Distance {
        readonly get => _distance;
        set => _distance = value;
        }

    /// <inheritdoc
    /// cref="IPlane{Plane, Vector, Direction, Rotation, Box, Round, T}.Normal"/>
    public Vector3<T> Normal {
        readonly get => _normal;
        set => _normal = value;
        }

    #endregion Data

    #region Normalization

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Plane3<T> CopyAndNormalize(in Plane3<T> plane) {
        var magnitude = plane.Normal.Magnitude;
        var factor = T.One / magnitude;
        return new(plane.Normal * factor, plane.Distance * factor);
        }

    [BunnyAttributes.SIMDCandidate]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Normalize(ref Plane3<T> plane) {
        var magnitude = plane.Normal.Magnitude;
        var factor = T.One / magnitude;
        plane.Normal *= factor;
        plane.Distance *= factor;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Plane3<T> Normalize() {
        return CopyAndNormalize(in this);
        }

    #endregion Normalization

    #region Strings

    public override readonly string ToString() {
        return ToString(3, 1, 8);
        }

    public readonly string ToString(byte digits, int integerLength, int paddingLength) {
        var d = Distance.Stringify(digits, integerLength, paddingLength);
        var n = Normal.ToString(digits, integerLength, paddingLength)[1..^1];
        return $"[ Plane 3⤯ Normal=({n}{1..^1}) Distance={d} ]";
        }

    #endregion Strings
    }