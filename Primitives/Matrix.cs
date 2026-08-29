#pragma warning disable IDE0018 // Inline variable declaration

using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using static Bunnarium.Tools.Utilities.SIMD;
using static Bunnarium.Maths.Utilities.SIMD;

namespace Bunnarium.Maths.Primitives;

/// <summary> A library of matrix functions that backs Bunnarium's concrete matrix types. Many of these functions are implemented for different <see cref="Docs.HandednessInstructions{T}">handed coordinate systems</see> and <see cref="Docs.MultiplicationConventionInstructions{T}">transformation style conventions</see>, which can be selected for via compiler symbol.
/// </summary>
public static partial class Matrix {

    #region Factories - Axis-Angle

    /// <summary><inheritdoc cref="AxisAngle{T}(Direction{T}, Angle{T}, out Matrix4{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPostmultiplicationConvention{T}"/>
    /// </remarks>
    [BunnyAttributes.Benchmark]
    public static unsafe void CreatePostmultipliedAxisAngle<T>(Direction<T> axis, Angle<T> rotation, out Matrix4<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        var (s, c) = rotation.SinCos;
        var i = T.One - c;
        T x2, y2, z2, xy, xz, yz, sx, sy, sz;
        if (typeof(T) == typeof(double) && Vector512.IsHardwareAccelerated && Vector512<double>.IsSupported) {  // BENCHMARK
            Vector256<T> xyz0 = default;
            Unsafe.Copy(&xyz0, in axis);
            var yzx0f = Vector256.Shuffle(Unsafe.BitCast<Vector256<T>, Vector256<double>>(xyz0), Vector256.Create(1, 2, 0, 3));
            var yzx0t = Unsafe.BitCast<Vector256<double>, Vector256<T>>(yzx0f);
            var v0 = Vector512.Create(Vector256.Create(s), yzx0t);  // ssssyzx0
            var v1 = Vector512.Create(xyz0, xyz0);                  // xyz0xyz0
            var vd = xyz0 * xyz0;
            var vm = v0 * v1;
            sx = vm[0]; sy = vm[1]; sz = vm[2];
            xy = vm[4]; xz = vm[6]; yz = vm[5];
            x2 = vd[0]; y2 = vd[1]; z2 = vd[2];
            }
        else if (typeof(T) == typeof(float) && Vector256.IsHardwareAccelerated && Vector256<float>.IsSupported) {  // BENCHMARK
            Vector128<T> xyz0 = default;
            Unsafe.Copy(&xyz0, in axis);
            var yzx0f = Vector128.Shuffle(Unsafe.BitCast<Vector128<T>, Vector128<float>>(xyz0), Vector128.Create(1, 2, 0, 3));
            var yzx0t = Unsafe.BitCast<Vector128<float>, Vector128<T>>(yzx0f);
            var v0 = Vector256.Create(Vector128.Create(s), yzx0t);  // ssssyzx0
            var v1 = Vector256.Create(xyz0, xyz0);                  // xyz0xyz0
            var vd = xyz0 * xyz0;
            var vm = v0 * v1;
            sx = vm[0]; sy = vm[1]; sz = vm[2];
            xy = vm[4]; xz = vm[6]; yz = vm[5];
            x2 = vd[0]; y2 = vd[1]; z2 = vd[2];
            }
        else if (typeof(T) == typeof(double) && Vector256.IsHardwareAccelerated && Vector256<double>.IsSupported) {
            Vector256<T> xyz0 = default;
            Unsafe.Copy(&xyz0, in axis);
            var dot = xyz0 * xyz0;
            var smv = xyz0 * Vector256.Create(s);
            x2 = dot[0]; y2 = dot[1]; z2 = dot[2];
            sx = smv[0]; sy = smv[1]; sz = smv[2];
            xy = xyz0[0] * xyz0[1];
            xz = xyz0[0] * xyz0[2];
            yz = xyz0[1] * xyz0[2];
            }
        else if (typeof(T) == typeof(float) && Vector128.IsHardwareAccelerated && Vector128<float>.IsSupported) {
            Vector128<T> xyz0 = default;
            Unsafe.Copy(&xyz0, in axis);
            var dot = xyz0 * xyz0;
            var smv = xyz0 * Vector128.Create(s);
            x2 = dot[0]; y2 = dot[1]; z2 = dot[2];
            sx = smv[0]; sy = smv[1]; sz = smv[2];
            xy = xyz0[0] * xyz0[1];
            xz = xyz0[0] * xyz0[2];
            yz = xyz0[1] * xyz0[2];
            }
        else if (typeof(T) == typeof(Half) && Vector64.IsHardwareAccelerated && Vector64<Half>.IsSupported) {
            Vector64<T> xyz0 = default;
            Unsafe.Copy(&xyz0, in axis);
            var dot = xyz0 * xyz0;
            var smv = xyz0 * Vector64.Create(s);
            x2 = dot[0]; y2 = dot[1]; z2 = dot[2];
            sx = smv[0]; sy = smv[1]; sz = smv[2];
            xy = xyz0[0] * xyz0[1];
            xz = xyz0[0] * xyz0[2];
            yz = xyz0[1] * xyz0[2];
            }
        else {
            T x, y, z;
            x = axis.UnwrapVector.X;
            y = axis.UnwrapVector.Y;
            z = axis.UnwrapVector.Z;
            x2 = x * x;
            y2 = y * y;
            z2 = z * z;
            xy = x * y;
            xz = x * z;
            yz = y * z;
            sx = s * x;
            sy = s * y;
            sz = s * z;
            }
        matrix = new( // +-+ / ++- / -++
            a1: c + i * x2,
            a2: i * xy - sz,
            a3: i * xz + sy,
            a4: T.Zero,
            b1: i * xy + sz,
            b2: c + i * y2,
            b3: i * yz - sx,
            b4: T.Zero,
            c1: i * xz - sy,
            c2: i * yz + sx,
            c3: c + i * z2,
            c4: T.Zero,
            d1: T.Zero, d2: T.Zero, d3: T.Zero, d4: T.One
            );
        }

    /// <summary><inheritdoc cref="AxisAngle{T}(Direction{T}, Angle{T}, out Matrix4{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPremultiplicationConvention{T}"/>
    /// </remarks>
    [BunnyAttributes.Benchmark]
    public static unsafe void CreatePremultipliedAxisAngle<T>(Direction<T> axis, Angle<T> rotation, out Matrix4<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        var (s, c) = rotation.SinCos;
        var i = T.One - c;
        T x2, y2, z2, xy, xz, yz, sx, sy, sz;
        if (typeof(T) == typeof(double) && Vector512.IsHardwareAccelerated && Vector512<double>.IsSupported) {  // BENCHMARK
            Vector256<T> xyz0 = default;
            Unsafe.Copy(&xyz0, in axis);
            var yzx0f = Vector256.Shuffle(Unsafe.BitCast<Vector256<T>, Vector256<double>>(xyz0), Vector256.Create(1, 2, 0, 3));
            var yzx0t = Unsafe.BitCast<Vector256<double>, Vector256<T>>(yzx0f);
            var v0 = Vector512.Create(Vector256.Create(s), yzx0t);  // ssssyzx0
            var v1 = Vector512.Create(xyz0, xyz0);                  // xyz0xyz0
            var vd = xyz0 * xyz0;
            var vm = v0 * v1;
            sx = vm[0]; sy = vm[1]; sz = vm[2];
            xy = vm[4]; xz = vm[6]; yz = vm[5];
            x2 = vd[0]; y2 = vd[1]; z2 = vd[2];
            }
        else if (typeof(T) == typeof(float) && Vector256.IsHardwareAccelerated && Vector256<float>.IsSupported) {  // BENCHMARK
            Vector128<T> xyz0 = default;
            Unsafe.Copy(&xyz0, in axis);
            var yzx0f = Vector128.Shuffle(Unsafe.BitCast<Vector128<T>, Vector128<float>>(xyz0), Vector128.Create(1, 2, 0, 3));
            var yzx0t = Unsafe.BitCast<Vector128<float>, Vector128<T>>(yzx0f);
            var v0 = Vector256.Create(Vector128.Create(s), yzx0t);  // ssssyzx0
            var v1 = Vector256.Create(xyz0, xyz0);                  // xyz0xyz0
            var vd = xyz0 * xyz0;
            var vm = v0 * v1;
            sx = vm[0]; sy = vm[1]; sz = vm[2];
            xy = vm[4]; xz = vm[6]; yz = vm[5];
            x2 = vd[0]; y2 = vd[1]; z2 = vd[2];
            }
        else if (typeof(T) == typeof(double) && Vector256.IsHardwareAccelerated && Vector256<double>.IsSupported) {
            Vector256<T> xyz0 = default;
            Unsafe.Copy(&xyz0, in axis);
            var dot = xyz0 * xyz0;
            var smv = xyz0 * Vector256.Create(s);
            x2 = dot[0]; y2 = dot[1]; z2 = dot[2];
            sx = smv[0]; sy = smv[1]; sz = smv[2];
            xy = xyz0[0] * xyz0[1];
            xz = xyz0[0] * xyz0[2];
            yz = xyz0[1] * xyz0[2];
            }
        else if (typeof(T) == typeof(float) && Vector128.IsHardwareAccelerated && Vector128<float>.IsSupported) {
            Vector128<T> xyz0 = default;
            Unsafe.Copy(&xyz0, in axis);
            var dot = xyz0 * xyz0;
            var smv = xyz0 * Vector128.Create(s);
            x2 = dot[0]; y2 = dot[1]; z2 = dot[2];
            sx = smv[0]; sy = smv[1]; sz = smv[2];
            xy = xyz0[0] * xyz0[1];
            xz = xyz0[0] * xyz0[2];
            yz = xyz0[1] * xyz0[2];
            }
        else if (typeof(T) == typeof(Half) && Vector64.IsHardwareAccelerated && Vector64<Half>.IsSupported) {
            Vector64<T> xyz0 = default;
            Unsafe.Copy(&xyz0, in axis);
            var dot = xyz0 * xyz0;
            var smv = xyz0 * Vector64.Create(s);
            x2 = dot[0]; y2 = dot[1]; z2 = dot[2];
            sx = smv[0]; sy = smv[1]; sz = smv[2];
            xy = xyz0[0] * xyz0[1];
            xz = xyz0[0] * xyz0[2];
            yz = xyz0[1] * xyz0[2];
            }
        else {
            T x, y, z;
            x = axis.UnwrapVector.X;
            y = axis.UnwrapVector.Y;
            z = axis.UnwrapVector.Z;
            x2 = x * x;
            y2 = y * y;
            z2 = z * z;
            xy = x * y;
            xz = x * z;
            yz = y * z;
            sx = s * x;
            sy = s * y;
            sz = s * z;
            }
        matrix = new( // ++- / -++ / +-+
            a1: c + i * x2,
            a2: i * xy + sz,
            a3: i * xz - sy,
            a4: T.Zero,
            b1: i * xy - sz,
            b2: c + i * y2,
            b3: i * yz + sx,
            b4: T.Zero,
            c1: i * xz + sy,
            c2: i * yz - sx,
            c3: c + i * z2,
            c4: T.Zero,
            d1: T.Zero, d2: T.Zero, d3: T.Zero, d4: T.One
            );
        }

    /// <summary><inheritdoc cref="IMatrix{T}.I3DMatrix{Matrix}.CreateAxisAngle(Direction{T}, Angle{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.MultiplicationConventionInstructions{T}"/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void AxisAngle<T>(Direction<T> axis, Angle<T> rotation, out Matrix4<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
#if MATRIX_PREMULTIPLIED_CONVENTION
        CreatePremultipliedAxisAngle(axis, rotation, out matrix);
#elif MATRIX_POSTMULTIPLIED_CONVENTION
        CreatePostmultipliedAxisAngle(axis, rotation, out matrix);
#else
        throw new ApplicationException(Matrix.Docs.BaseMessage);
#endif
        }

    #endregion Factories - Axis-Angle

    #region Factories - Look-To / Look-At

    private static void CreatePostmultipliedLeftHandedLookToImpl<T>(Vector3<T> cameraPosition, Vector3<T> normalizedCameraDirection, Vector3<T> cameraUp, out Matrix4<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        var z = normalizedCameraDirection;
        var x = Vector3<T>.Cross(cameraUp, z);
        Vector3<T>.Normalize(ref x);
        var y = Vector3<T>.Cross(z, x);
        Vector3<T>.Normalize(ref y);
        var n = -cameraPosition;
        matrix = new Matrix4<T>(
            x.X, x.Y, x.Z, x.Dot(n),
            y.X, y.Y, y.Z, y.Dot(n),
            z.X, z.Y, z.Z, z.Dot(n),
            T.Zero,
            T.Zero,
            T.Zero,
            T.One
            );
        }

    private static void CreatePremultipliedLeftHandedLookToImpl<T>(Vector3<T> cameraPosition, Vector3<T> normalizedCameraDirection, Vector3<T> cameraUp, out Matrix4<T> matrix)
            where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        var z = normalizedCameraDirection;
        var x = Vector3<T>.Cross(cameraUp, z);
        Vector3<T>.Normalize(ref x);
        var y = Vector3<T>.Cross(z, x);
        Vector3<T>.Normalize(ref y);
        var n = -cameraPosition;
        matrix = new Matrix4<T>(
            x.X, y.X, z.X, T.Zero,
            x.Y, y.Y, z.Y, T.Zero,
            x.Z, y.Z, z.Z, T.Zero,
            x.Dot(n),
            y.Dot(n),
            z.Dot(n),
            T.One
            );
        }

    /// <summary><inheritdoc cref="IMatrix{T}.I3DProjectionMatrix{T}.CreateLookAt(Vector3{T}, Vector3{T}, Vector3{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.HandednessInstructions{T}"/><inheritdoc cref="Docs.MultiplicationConventionInstructions{T}"/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CreateLookAt<T>(Vector3<T> cameraPosition, Vector3<T> cameraTarget, Vector3<T> cameraUp, out Matrix4<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
#if MATRIX_LEFTHANDED_COORDINATE_SYSTEM && MATRIX_PREMULTIPLIED_CONVENTION
        CreatePremultipliedLeftHandedLookAt(cameraPosition, cameraTarget, cameraUp, out matrix);
#elif MATRIX_RIGHTHANDED_COORDINATE_SYSTEM && MATRIX_PREMULTIPLIED_CONVENTION
        CreatePremultipliedRightHandedLookAt(cameraPosition, cameraTarget, cameraUp, out matrix);
#elif MATRIX_LEFTHANDED_COORDINATE_SYSTEM && MATRIX_POSTMULTIPLIED_CONVENTION
        CreatePostmultipliedLeftHandedLookAt(cameraPosition, cameraTarget, cameraUp, out matrix);
#elif MATRIX_RIGHTHANDED_COORDINATE_SYSTEM && MATRIX_POSTMULTIPLIED_CONVENTION
        CreatePostmultipliedRightHandedLookAt(cameraPosition, cameraTarget, cameraUp, out matrix);
#else
        throw new ApplicationException(Matrix.Docs.BaseMessage);
#endif
        }

    /// <summary><inheritdoc cref="IMatrix{T}.I3DProjectionMatrix{Matrix}.CreateLookTo(Vector3{T}, Vector3{T}, Vector3{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.HandednessInstructions{T}"/><inheritdoc cref="Docs.MultiplicationConventionInstructions{T}"/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CreateLookTo<T>(Vector3<T> cameraPosition, Vector3<T> cameraDirection, Vector3<T> cameraUp, out Matrix4<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
#if MATRIX_LEFTHANDED_COORDINATE_SYSTEM && MATRIX_PREMULTIPLIED_CONVENTION
        CreatePremultipliedLeftHandedLookTo(cameraPosition, cameraDirection, cameraUp, out matrix);
#elif MATRIX_RIGHTHANDED_COORDINATE_SYSTEM && MATRIX_PREMULTIPLIED_CONVENTION
        CreatePremultipliedRightHandedLookTo(cameraPosition, cameraDirection, cameraUp, out matrix);
#elif MATRIX_LEFTHANDED_COORDINATE_SYSTEM && MATRIX_POSTMULTIPLIED_CONVENTION
        CreatePostmultipliedLeftHandedLookTo(cameraPosition, cameraDirection, cameraUp, out matrix);
#elif MATRIX_RIGHTHANDED_COORDINATE_SYSTEM && MATRIX_POSTMULTIPLIED_CONVENTION
        CreatePostmultipliedRightHandedLookTo(cameraPosition, cameraDirection, cameraUp, out matrix);
#else
        throw new ApplicationException(Matrix.Docs.BaseMessage);
#endif
        }

    /// <inheritdoc
    /// cref="CreateLookTo{T}(Vector3{T}, Vector3{T}, Vector3{T}, out Matrix4{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CreateLookTo<T>(Vector3<T> cameraPosition, Direction<T> cameraDirection, Direction<T> cameraUp, out Matrix4<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
#if MATRIX_LEFTHANDED_COORDINATE_SYSTEM && MATRIX_PREMULTIPLIED_CONVENTION
        CreatePremultipliedLeftHandedLookTo(cameraPosition, cameraDirection, cameraUp, out matrix);
#elif MATRIX_RIGHTHANDED_COORDINATE_SYSTEM && MATRIX_PREMULTIPLIED_CONVENTION
        CreatePremultipliedRightHandedLookTo(cameraPosition, cameraDirection, cameraUp, out matrix);
#elif MATRIX_LEFTHANDED_COORDINATE_SYSTEM && MATRIX_POSTMULTIPLIED_CONVENTION
        CreatePostmultipliedLeftHandedLookTo(cameraPosition, cameraDirection, cameraUp, out matrix);
#elif MATRIX_RIGHTHANDED_COORDINATE_SYSTEM && MATRIX_POSTMULTIPLIED_CONVENTION
        CreatePostmultipliedRightHandedLookTo(cameraPosition, cameraDirection, cameraUp, out matrix);
#else
        throw new ApplicationException(Matrix.Docs.BaseMessage);
#endif
        }

    /// <summary><inheritdoc cref="CreateLookAt{T}(Vector3{T}, Vector3{T}, Vector3{T}, out Matrix4{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesLeftHandedRotation{T}"/><inheritdoc cref="Docs.UsesPostmultiplicationConvention{T}"/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CreatePostmultipliedLeftHandedLookAt<T>(Vector3<T> cameraPosition, Vector3<T> cameraTarget, Vector3<T> cameraUp, out Matrix4<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        var unnormalizedCameraDirection = cameraTarget - cameraPosition;
        CreatePostmultipliedLeftHandedLookTo(
            cameraPosition: cameraPosition,
            cameraDirection: unnormalizedCameraDirection,
            cameraUp: cameraUp,
            matrix: out matrix
            );
        }

    /// <summary><inheritdoc cref="CreateLookTo{T}(Vector3{T}, Direction{T}, Direction{T}, out Matrix4{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesLeftHandedRotation{T}"/><inheritdoc cref="Docs.UsesPostmultiplicationConvention{T}"/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CreatePostmultipliedLeftHandedLookTo<T>(Vector3<T> cameraPosition, Direction<T> cameraDirection, Direction<T> cameraUp, out Matrix4<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        CreatePostmultipliedLeftHandedLookToImpl(
            cameraPosition: cameraPosition,
            normalizedCameraDirection: cameraDirection.UnwrapVector,
            cameraUp: cameraUp.UnwrapVector,
            matrix: out matrix
            );
        }

    /// <summary><inheritdoc cref="CreateLookTo{T}(Vector3{T}, Vector3{T}, Vector3{T}, out Matrix4{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesLeftHandedRotation{T}"/><inheritdoc cref="Docs.UsesPostmultiplicationConvention{T}"/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CreatePostmultipliedLeftHandedLookTo<T>(Vector3<T> cameraPosition, Vector3<T> cameraDirection, Vector3<T> cameraUp, out Matrix4<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        CreatePostmultipliedLeftHandedLookToImpl(
            cameraPosition: cameraPosition,
            normalizedCameraDirection: Vector3<T>.Normalize(cameraDirection),
            cameraUp: cameraUp,
            matrix: out matrix
            );
        }

    /// <summary><inheritdoc cref="CreateLookAt{T}(Vector3{T}, Vector3{T}, Vector3{T}, out Matrix4{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesRightHandedRotation{T}"/><inheritdoc cref="Docs.UsesPostmultiplicationConvention{T}"/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CreatePostmultipliedRightHandedLookAt<T>(Vector3<T> cameraPosition, Vector3<T> cameraTarget, Vector3<T> cameraUp, out Matrix4<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        var unnormalizedCameraDirection = cameraPosition - cameraTarget;
        CreatePostmultipliedLeftHandedLookTo(
            cameraPosition: cameraPosition,
            cameraDirection: unnormalizedCameraDirection,
            cameraUp: cameraUp,
            matrix: out matrix
            );
        }

    /// <summary><inheritdoc cref="CreateLookTo{T}(Vector3{T}, Direction{T}, Direction{T}, out Matrix4{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesRightHandedRotation{T}"/><inheritdoc cref="Docs.UsesPostmultiplicationConvention{T}"/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CreatePostmultipliedRightHandedLookTo<T>(Vector3<T> cameraPosition, Direction<T> cameraDirection, Direction<T> cameraUp, out Matrix4<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        CreatePostmultipliedLeftHandedLookTo(
            cameraPosition: cameraPosition,
            cameraDirection: Direction<T>.Flip(cameraDirection),
            cameraUp: cameraUp,
            matrix: out matrix
            );
        }

    /// <summary><inheritdoc cref="CreateLookTo{T}(Vector3{T}, Vector3{T}, Vector3{T}, out Matrix4{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesRightHandedRotation{T}"/><inheritdoc cref="Docs.UsesPostmultiplicationConvention{T}"/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CreatePostmultipliedRightHandedLookTo<T>(Vector3<T> cameraPosition, Vector3<T> cameraDirection, Vector3<T> cameraUp, out Matrix4<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        var unnormalizedCameraDirection = Vector3<T>.Negate(cameraDirection);
        CreatePostmultipliedLeftHandedLookTo(
            cameraPosition: cameraPosition,
            cameraDirection: unnormalizedCameraDirection,
            cameraUp: cameraUp,
            matrix: out matrix
            );
        }

    /// <summary><inheritdoc cref="CreateLookAt{T}(Vector3{T}, Vector3{T}, Vector3{T}, out Matrix4{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesLeftHandedRotation{T}"/><inheritdoc cref="Docs.UsesPremultiplicationConvention{T}"/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CreatePremultipliedLeftHandedLookAt<T>(Vector3<T> cameraPosition, Vector3<T> cameraTarget, Vector3<T> cameraUp, out Matrix4<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        var unnormalizedCameraDirection = cameraTarget - cameraPosition;
        CreatePremultipliedLeftHandedLookTo(
            cameraPosition: cameraPosition,
            cameraDirection: unnormalizedCameraDirection,
            cameraUp: cameraUp,
            matrix: out matrix
            );
        }

    /// <summary><inheritdoc cref="CreateLookTo{T}(Vector3{T}, Direction{T}, Direction{T}, out Matrix4{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesLeftHandedRotation{T}"/><inheritdoc cref="Docs.UsesPremultiplicationConvention{T}"/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CreatePremultipliedLeftHandedLookTo<T>(Vector3<T> cameraPosition, Direction<T> cameraDirection, Direction<T> cameraUp, out Matrix4<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        CreatePremultipliedLeftHandedLookToImpl(
            cameraPosition: cameraPosition,
            normalizedCameraDirection: cameraDirection.UnwrapVector,
            cameraUp: cameraUp.UnwrapVector,
            matrix: out matrix
            );
        }

    /// <summary><inheritdoc cref="CreateLookTo{T}(Vector3{T}, Vector3{T}, Vector3{T}, out Matrix4{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesLeftHandedRotation{T}"/><inheritdoc cref="Docs.UsesPremultiplicationConvention{T}"/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CreatePremultipliedLeftHandedLookTo<T>(Vector3<T> cameraPosition, Vector3<T> cameraDirection, Vector3<T> cameraUp, out Matrix4<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        CreatePremultipliedLeftHandedLookToImpl(
            cameraPosition: cameraPosition,
            normalizedCameraDirection: Vector3<T>.Normalize(cameraDirection),
            cameraUp: cameraUp,
            matrix: out matrix
            );
        }

    /// <summary><inheritdoc cref="CreateLookAt{T}(Vector3{T}, Vector3{T}, Vector3{T}, out Matrix4{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesRightHandedRotation{T}"/><inheritdoc cref="Docs.UsesPremultiplicationConvention{T}"/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CreatePremultipliedRightHandedLookAt<T>(Vector3<T> cameraPosition, Vector3<T> cameraTarget, Vector3<T> cameraUp, out Matrix4<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        var unnormalizedCameraDirection = cameraPosition - cameraTarget;
        CreatePremultipliedLeftHandedLookTo(
            cameraPosition: cameraPosition,
            cameraDirection: unnormalizedCameraDirection,
            cameraUp: cameraUp,
            matrix: out matrix
            );
        }

    /// <summary><inheritdoc cref="CreateLookTo{T}(Vector3{T}, Direction{T}, Direction{T}, out Matrix4{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesRightHandedRotation{T}"/><inheritdoc cref="Docs.UsesPremultiplicationConvention{T}"/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CreatePremultipliedRightHandedLookTo<T>(Vector3<T> cameraPosition, Direction<T> cameraDirection, Direction<T> cameraUp, out Matrix4<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        CreatePremultipliedLeftHandedLookTo(
            cameraPosition: cameraPosition,
            cameraDirection: Direction<T>.Flip(cameraDirection),
            cameraUp: cameraUp,
            matrix: out matrix
            );
        }

    /// <summary><inheritdoc cref="CreateLookTo{T}(Vector3{T}, Vector3{T}, Vector3{T}, out Matrix4{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesRightHandedRotation{T}"/><inheritdoc cref="Docs.UsesPremultiplicationConvention{T}"/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CreatePremultipliedRightHandedLookTo<T>(Vector3<T> cameraPosition, Vector3<T> cameraDirection, Vector3<T> cameraUp, out Matrix4<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        var unnormalizedCameraDirection = Vector3<T>.Negate(cameraDirection);
        CreatePremultipliedLeftHandedLookTo(
            cameraPosition: cameraPosition,
            cameraDirection: unnormalizedCameraDirection,
            cameraUp: cameraUp,
            matrix: out matrix
            );
        }

    #endregion Factories - Look-To / Look-At

    #region Factories - Multi-Axis Rotation XYZ

    /// <summary> <inheritdoc cref="CreateRotationXYZ{T}(Angle{T}, Angle{T}, Angle{T}, out Matrix4{T})"/>
    /// </summary>
    /// <remarks> <inheritdoc cref="Docs.UsesPostmultiplicationConvention{T}"/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void CreatePostmultipliedRotationXYZ<T>(Angle<T> rotationX, Angle<T> rotationY, Angle<T> rotationZ, out Matrix4<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        var (sin, cos) = SinCos(rotationX, rotationY, rotationZ);
        var szsy = sin.Z * sin.Y;
        var czsy = cos.Z * sin.Y;

        // inlined with post-multiplied rotation matrices
        matrix = new(
            a1: cos.Z * cos.Y,
            a2: czsy * sin.X - sin.Z * cos.X,
            a3: czsy * cos.X + sin.Z * sin.X,
            a4: T.Zero,
            b1: sin.Z * cos.Y,
            b2: szsy * sin.X + cos.Z * cos.X,
            b3: szsy * cos.X - cos.Z * sin.X,
            b4: T.Zero,
            c1: -sin.Y,
            c2: cos.Y * sin.X,
            c3: cos.Y * cos.X,
            c4: T.Zero,
            d1: T.Zero, d2: T.Zero, d3: T.Zero, d4: T.One
            );
        }

    /// <summary> <inheritdoc cref="CreateRotationXYZ{T}(Angle{T}, Angle{T}, Angle{T}, out Matrix4{T})"/>
    /// </summary>
    /// <remarks> <inheritdoc cref="Docs.UsesPremultiplicationConvention{T}"/>
    /// </remarks>
    public static void CreatePremultipliedRotationXYZ<T>(Angle<T> rotationX, Angle<T> rotationY, Angle<T> rotationZ, out Matrix4<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        var (sin, cos) = SinCos(rotationX, rotationY, rotationZ);
        var sxsy = sin.X * sin.Y;
        var cxsy = cos.X * sin.Y;

        // inlined with pre-multiplied rotation matrices
        matrix = new(
            a1: cos.Y * cos.Z,
            a2: cos.Y * sin.Z,
            a3: -sin.Y,
            a4: T.Zero,
            b1: sxsy * cos.Z - cos.X * sin.Z, // sx * sy * cz - cx * sz
            b2: sxsy * sin.Z + cos.X * cos.Z, // sx * sy * sz + cx * cz
            b3: sin.X * cos.Y,
            b4: T.Zero,
            c1: cxsy * cos.Z + sin.X * sin.Z, // cx * sy * cz + sx * sz
            c2: cxsy * sin.Z - sin.X * cos.Z, // cx * sy * sz - sx * cz
            c3: cos.X * cos.Y,
            c4: T.Zero,
            d1: T.Zero, d2: T.Zero, d3: T.Zero, d4: T.One
            );
        }

    /// <summary>
    /// <inheritdoc cref="Docs.IsExtrinsicXYZRotation{T}"/>
    /// <inheritdoc cref="Docs.EquivalentIntrinsicRotationIs{T}"/>
    /// <c><see cref="CreateRotationZYX{T}(Angle{T}, Angle{T}, Angle{T}, out Matrix4{T})">extrinsic ZYX</see></c>.
    /// </summary>
    /// <remarks> <inheritdoc cref="Docs.MultiplicationConventionInstructions{T}"/>
    /// </remarks>
    public static void CreateRotationXYZ<T>(Angle<T> rotationX, Angle<T> rotationY, Angle<T> rotationZ, out Matrix4<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
#if MATRIX_PREMULTIPLIED_CONVENTION
        CreatePremultipliedRotationXYZ(rotationX, rotationY, rotationZ, out matrix);
#elif MATRIX_POSTMULTIPLIED_CONVENTION
        CreatePostmultipliedRotationXYZ(rotationX, rotationY, rotationZ, out matrix);
#else
        throw new ApplicationException(Matrix.Docs.BaseMessage);
#endif
        }

    #endregion Factories - Multi-Axis Rotation XYZ

    #region Factories - Multi-Axis Rotation XZY

    /// <summary> <inheritdoc cref="CreateRotationXZY{T}(Angle{T}, Angle{T}, Angle{T}, out Matrix4{T})"/>
    /// </summary>
    /// <remarks> <inheritdoc cref="Docs.UsesPostmultiplicationConvention{T}"/>
    /// </remarks>
    public static void CreatePostmultipliedRotationXZY<T>(Angle<T> rotationX, Angle<T> rotationZ, Angle<T> rotationY, out Matrix4<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        var (sin, cos) = SinCos(rotationX, rotationY, rotationZ);
        var sxsy = sin.X * sin.Y;
        var cxsy = cos.X * sin.Y;
        var sxcy = sin.X * cos.Y;
        var cxcy = cos.X * cos.Y;

        // inlined with post-multiplied rotation matrices
        matrix = new(
            a1: cos.Y * cos.Z,          // cy * cz
            a2: sxsy - cxcy * sin.Z,    // sx * sy - cx * cy * sz
            a3: sxcy * sin.Z + cxsy,    // sx * cy * sz + cx * sy
            a4: T.Zero,
            b1: sin.Z,                  // sz
            b2: cos.X * cos.Z,          // cx * cz
            b3: sin.X * -cos.Z,         // sx * -cz
            b4: T.Zero,
            c1: sin.Y * -cos.Z,         // sy * -cz
            c2: cxsy * sin.Z + sxcy,    // cx * sy * sz + sx * cy
            c3: cxcy - sxsy * sin.Z,    // cx * cy - sx * sy * sz
            c4: T.Zero,
            d1: T.Zero, d2: T.Zero, d3: T.Zero, d4: T.One
            );
        }

    /// <summary> <inheritdoc cref="CreateRotationXZY{T}(Angle{T}, Angle{T}, Angle{T}, out Matrix4{T})"/>
    /// </summary>
    /// <remarks> <inheritdoc cref="Docs.UsesPremultiplicationConvention{T}"/>
    /// </remarks>
    public static void CreatePremultipliedRotationXZY<T>(Angle<T> rotationX, Angle<T> rotationZ, Angle<T> rotationY, out Matrix4<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        var (sin, cos) = SinCos(rotationX, rotationY, rotationZ);
        var cxcy = cos.X * cos.Y;
        var sxsy = sin.X * sin.Y;
        var sxcy = sin.X * cos.Y;
        var cxsy = cos.X * sin.Y;

        // inlined with pre-multiplied rotation matrices
        matrix = new(
            a1: cos.Y * cos.Z,          // cy * cz
            a2: sin.Z,                  // sz
            a3: sin.Y * -cos.Z,         // sy * -cz
            a4: T.Zero,
            b1: sxsy - cxcy * sin.Z,    // sx * sy - cx * cy * sz
            b2: cos.X * cos.Z,          // cx * cz
            b3: cxsy * sin.Z + sxcy,    // cx * sy * sz + sx * cy
            b4: T.Zero,
            c1: sxcy * sin.Z + cxsy,    // sx * cy * sz + cx * sy
            c2: sin.X * -cos.Z,         // sx * -cz
            c3: cxcy - sxsy * sin.Z,    // cx * cy - sx * sy * sz
            c4: T.Zero,
            d1: T.Zero, d2: T.Zero, d3: T.Zero, d4: T.One
            );
        }

    /// <summary>
    /// <inheritdoc cref="Docs.IsExtrinsicXZYRotation{T}"/>
    /// <inheritdoc cref="Docs.EquivalentIntrinsicRotationIs{T}"/>
    /// <c><see cref="CreateRotationYZX{T}(Angle{T}, Angle{T}, Angle{T}, out Matrix4{T})">extrinsic YZX</see></c>.
    /// </summary>
    /// <remarks> <inheritdoc cref="Docs.MultiplicationConventionInstructions{T}"/>
    /// </remarks>
    public static void CreateRotationXZY<T>(Angle<T> rotationX, Angle<T> rotationZ, Angle<T> rotationY, out Matrix4<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
#if MATRIX_PREMULTIPLIED_CONVENTION
        CreatePremultipliedRotationXZY(rotationX, rotationZ, rotationY, out matrix);
#elif MATRIX_POSTMULTIPLIED_CONVENTION
        CreatePostmultipliedRotationXZY(rotationX, rotationZ, rotationY, out matrix);
#else
        throw new ApplicationException(Matrix.Docs.BaseMessage);
#endif
        }

    #endregion Factories - Multi-Axis Rotation XZY

    #region Factories - Multi-Axis Rotation YXZ

    /// <summary> <inheritdoc cref="CreateRotationYXZ{T}(Angle{T}, Angle{T}, Angle{T}, out Matrix4{T})"/>
    /// </summary>
    /// <remarks> <inheritdoc cref="Docs.UsesPostmultiplicationConvention{T}"/>
    /// </remarks>
    public static void CreatePostmultipliedRotationYXZ<T>(Angle<T> rotationY, Angle<T> rotationX, Angle<T> rotationZ, out Matrix4<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        var (sin, cos) = SinCos(rotationX, rotationY, rotationZ);
        var sxsy = sin.X * sin.Y;
        var cysz = cos.Y * sin.Z;
        var cycz = cos.Y * cos.Z;

        // inlined with post-multiplied rotation matrices
        matrix = new(
            a1: cycz - sxsy * sin.Z,            // cz * cy - sz * sx * sy
            a2: cos.X * -sin.Z,                 // -sz * cx
            a3: sin.X * cysz + sin.Y * cos.Z,   // sz * sx * cy + cz * sy
            a4: T.Zero,
            b1: sxsy * cos.Z + cysz,            // cz * sx * sy + sz * cy
            b2: cos.X * cos.Z,                  // cz * cx
            b3: sin.Y * sin.Z - sin.X * cycz,   // sz * sy - cz * sx * cy
            b4: T.Zero,
            c1: cos.X * -sin.Y,                 // -cx * sy
            c2: sin.X,                          // sx
            c3: cos.X * cos.Y,                  // cx * cy
            c4: T.Zero,
            d1: T.Zero, d2: T.Zero, d3: T.Zero, d4: T.One
            );
        }

    /// <summary> <inheritdoc cref="CreateRotationYXZ{T}(Angle{T}, Angle{T}, Angle{T}, out Matrix4{T})"/>
    /// </summary>
    /// <remarks> <inheritdoc cref="Docs.UsesPremultiplicationConvention{T}"/>
    /// </remarks>
    public static void CreatePremultipliedRotationYXZ<T>(Angle<T> rotationY, Angle<T> rotationX, Angle<T> rotationZ, out Matrix4<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        var (sin, cos) = SinCos(rotationX, rotationY, rotationZ);
        var sxcy = sin.X * cos.Y;
        var sxsy = sin.X * sin.Y;

        // inlined with pre-multiplied rotation matrices
        matrix = new(
            a1: cos.Y * cos.Z - sxsy * sin.Z,   // cy * cz - sx * sy * sz
            a2: cos.Y * sin.Z + sxsy * cos.Z,   // cy * sz + sx * sy * cz
            a3: -sin.Y * cos.X,
            a4: T.Zero,
            b1: -cos.X * sin.Z,
            b2: cos.X * cos.Z,
            b3: sin.X,
            b4: T.Zero,
            c1: sin.Y * cos.Z + sxcy * sin.Z,    // sy * cz + sx * cy * sz
            c2: sin.Y * sin.Z - sxcy * cos.Z,   // sy * sz - sx * cy * cz
            c3: cos.X * cos.Y,
            c4: T.Zero,
            d1: T.Zero, d2: T.Zero, d3: T.Zero, d4: T.One
            );
        }

    /// <summary>
    /// <inheritdoc cref="Docs.IsExtrinsicYXZRotation{T}"/>
    /// <inheritdoc cref="Docs.EquivalentIntrinsicRotationIs{T}"/>
    /// <c><see cref="CreateRotationZXY{T}(Angle{T}, Angle{T}, Angle{T}, out Matrix4{T})">extrinsic ZXY</see></c>.
    /// </summary>
    /// <remarks> <inheritdoc cref="Docs.MultiplicationConventionInstructions{T}"/>
    /// </remarks>
    public static void CreateRotationYXZ<T>(Angle<T> rotationY, Angle<T> rotationX, Angle<T> rotationZ, out Matrix4<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
#if MATRIX_PREMULTIPLIED_CONVENTION
        CreatePremultipliedRotationYXZ(rotationY, rotationX, rotationZ, out matrix);
#elif MATRIX_POSTMULTIPLIED_CONVENTION
        CreatePostmultipliedRotationYXZ(rotationY, rotationX, rotationZ, out matrix);
#else
        throw new ApplicationException(Matrix.Docs.BaseMessage);
#endif
        }

    #endregion Factories - Multi-Axis Rotation YXZ

    #region Factories - Multi-Axis Rotation YZX

    /// <summary> <inheritdoc cref="CreateRotationYZX{T}(Angle{T}, Angle{T}, Angle{T}, out Matrix4{T})"/>
    /// </summary>
    /// <remarks> <inheritdoc cref="Docs.UsesPostmultiplicationConvention{T}"/>
    /// </remarks>
    public static void CreatePostmultipliedRotationYZX<T>(Angle<T> rotationY, Angle<T> rotationZ, Angle<T> rotationX, out Matrix4<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        var (sin, cos) = SinCos(rotationX, rotationY, rotationZ);
        var cxcy = cos.X * cos.Y;
        var sxsy = sin.X * sin.Y;
        var sxcy = sin.X * cos.Y;
        var cxsy = cos.X * sin.Y;

        // inlined with post-multiplied rotation matrices
        matrix = new(
            a1: cos.Y * cos.Z,          // cy * cz
            a2: -sin.Z,                 // -sz
            a3: sin.Y * cos.Z,          // sy * cz
            a4: T.Zero,
            b1: sxsy + cxcy * sin.Z,    // sx * sy + cx * cy * sz
            b2: cos.X * cos.Z,          // cx * cz
            b3: cxsy * sin.Z - sxcy,    // cx * sy * sz - sx * cy
            b4: T.Zero,
            c1: sxcy * sin.Z - cxsy,    // sx * cy * sz - cx * sy
            c2: sin.X * cos.Z,          // sx * cz
            c3: cxcy + sxsy * sin.Z,    // cx * cy + sx * sy * sz
            c4: T.Zero,
            d1: T.Zero, d2: T.Zero, d3: T.Zero, d4: T.One
            );
        }

    /// <summary> <inheritdoc cref="CreateRotationYZX{T}(Angle{T}, Angle{T}, Angle{T}, out Matrix4{T})"/>
    /// </summary>
    /// <remarks> <inheritdoc cref="Docs.UsesPremultiplicationConvention{T}"/>
    /// </remarks>
    public static void CreatePremultipliedRotationYZX<T>(Angle<T> rotationY, Angle<T> rotationZ, Angle<T> rotationX, out Matrix4<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        var (sin, cos) = SinCos(rotationX, rotationY, rotationZ);
        var sxsy = sin.X * sin.Y;
        var cxsy = cos.X * sin.Y;
        var sxcy = sin.X * cos.Y;
        var cxcy = cos.X * cos.Y;

        // inlined with pre-multiplied rotation matrices
        matrix = new(
            a1: cos.Y * cos.Z,          // cy * cz
            a2: sxsy + cxcy * sin.Z,    // sx * sy + cx * cy * sz
            a3: sxcy * sin.Z - cxsy,    // sx * cy * sz - cx * sy
            a4: T.Zero,
            b1: -sin.Z,                 // -sz
            b2: cos.X * cos.Z,          // cx * cz
            b3: sin.X * cos.Z,          // sx * cz
            b4: T.Zero,
            c1: sin.Y * cos.Z,          // sy * cz
            c2: cxsy * sin.Z - sxcy,    // cx * sy * sz - sx * cy
            c3: cxcy + sxsy * sin.Z,    // cx * cy + sx * sy * sz
            c4: T.Zero,
            d1: T.Zero, d2: T.Zero, d3: T.Zero, d4: T.One
            );
        }

    /// <summary>
    /// <inheritdoc cref="Docs.IsExtrinsicYZXRotation{T}"/>
    /// <inheritdoc cref="Docs.EquivalentIntrinsicRotationIs{T}"/>
    /// <c><see cref="CreateRotationXZY{T}(Angle{T}, Angle{T}, Angle{T}, out Matrix4{T})">extrinsic XZY</see></c>.
    /// </summary>
    /// <remarks> <inheritdoc cref="Docs.MultiplicationConventionInstructions{T}"/>
    /// </remarks>
    public static void CreateRotationYZX<T>(Angle<T> rotationY, Angle<T> rotationZ, Angle<T> rotationX, out Matrix4<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
#if MATRIX_PREMULTIPLIED_CONVENTION
        CreatePremultipliedRotationYZX(rotationY, rotationZ, rotationX, out matrix);
#elif MATRIX_POSTMULTIPLIED_CONVENTION
        CreatePostmultipliedRotationYZX(rotationY, rotationZ, rotationX, out matrix);
#else
        throw new ApplicationException(Matrix.Docs.BaseMessage);
#endif
        }

    #endregion Factories - Multi-Axis Rotation YZX

    #region Factories - Multi-Axis Rotation ZXY

    /// <summary> <inheritdoc cref="CreateRotationZXY{T}(Angle{T}, Angle{T}, Angle{T}, out Matrix4{T})"/>
    /// </summary>
    /// <remarks> <inheritdoc cref="Docs.UsesPostmultiplicationConvention{T}"/>
    /// </remarks>
    public static void CreatePostmultipliedRotationZXY<T>(Angle<T> rotationZ, Angle<T> rotationX, Angle<T> rotationY, out Matrix4<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        var (sin, cos) = SinCos(rotationX, rotationY, rotationZ);
        var sxcy = sin.X * cos.Y;
        var sxsy = sin.X * sin.Y;

        // inlined with post-multiplied rotation matrices
        matrix = new(
            a1: sxsy * sin.Z + cos.Y * cos.Z,   // sx * sy * sz + cy * cz
            a2: sxsy * cos.Z - cos.Y * sin.Z,   // sx * sy * cz - cy * sz
            a3: sin.Y * cos.X,                  // sy * cx
            a4: T.Zero,
            b1: cos.X * sin.Z,                  // cx * sz
            b2: cos.X * cos.Z,                  // cx * cz
            b3: -sin.X,                         // -sx
            b4: T.Zero,
            c1: sxcy * sin.Z - sin.Y * cos.Z,   // sx * cy * sz - sy * cz
            c2: sxcy * cos.Z + sin.Y * sin.Z,   // sx * cy * cz + sy * sz
            c3: cos.X * cos.Y,                  // cx * cy
            c4: T.Zero,
            d1: T.Zero, d2: T.Zero, d3: T.Zero, d4: T.One
            );
        }

    /// <summary> <inheritdoc cref="CreateRotationZXY{T}(Angle{T}, Angle{T}, Angle{T}, out Matrix4{T})"/>
    /// </summary>
    /// <remarks> <inheritdoc cref="Docs.UsesPremultiplicationConvention{T}"/>
    /// </remarks>
    public static void CreatePremultipliedRotationZXY<T>(Angle<T> rotationZ, Angle<T> rotationX, Angle<T> rotationY, out Matrix4<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        var (sin, cos) = SinCos(rotationX, rotationY, rotationZ);
        var sxsy = sin.X * sin.Y;
        var cysz = cos.Y * sin.Z;
        var cycz = cos.Y * cos.Z;

        // inlined with pre-multiplied rotation matrices
        matrix = new(
            a1: cycz + sxsy * sin.Z,            // cz * cy + sz * sx * sy
            a2: cos.X * sin.Z,                  // sz * cx
            a3: sin.X * cysz - sin.Y * cos.Z,   // sz * sx * cy - cz * sy
            a4: T.Zero,
            b1: sxsy * cos.Z - cysz,            // cz * sx * sy - sz * cy
            b2: cos.X * cos.Z,                  // cz * cx
            b3: sin.Y * sin.Z + sin.X * cycz,   // sz * sy + cz * sx * cy
            b4: T.Zero,
            c1: cos.X * sin.Y,                  // cx * sy
            c2: -sin.X,                         // -sx
            c3: cos.X * cos.Y,                  // cx * cy
            c4: T.Zero,
            d1: T.Zero, d2: T.Zero, d3: T.Zero, d4: T.One
            );
        }

    /// <summary>
    /// <inheritdoc cref="Docs.IsExtrinsicZXYRotation{T}"/>
    /// <inheritdoc cref="Docs.EquivalentIntrinsicRotationIs{T}"/>
    /// <c><see cref="CreateRotationYXZ{T}(Angle{T}, Angle{T}, Angle{T}, out Matrix4{T})">extrinsic YXZ</see></c>.
    /// </summary>
    /// <remarks> <inheritdoc cref="Docs.MultiplicationConventionInstructions{T}"/>
    /// </remarks>
    public static void CreateRotationZXY<T>(Angle<T> rotationZ, Angle<T> rotationX, Angle<T> rotationY, out Matrix4<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
#if MATRIX_PREMULTIPLIED_CONVENTION
        CreatePremultipliedRotationZXY(rotationZ, rotationX, rotationY, out matrix);
#elif MATRIX_POSTMULTIPLIED_CONVENTION
        CreatePostmultipliedRotationZXY(rotationZ, rotationX, rotationY, out matrix);
#else
        throw new ApplicationException(Matrix.Docs.BaseMessage);
#endif
        }

    #endregion Factories - Multi-Axis Rotation ZXY

    #region Factories - Multi-Axis Rotation ZYX

    /// <summary> <inheritdoc cref="CreateRotationZYX{T}(Angle{T}, Angle{T}, Angle{T}, out Matrix4{T})"/>
    /// </summary>
    /// <remarks> <inheritdoc cref="Docs.UsesPostmultiplicationConvention{T}"/>
    /// </remarks>
    public static void CreatePostmultipliedRotationZYX<T>(Angle<T> rotationZ, Angle<T> rotationY, Angle<T> rotationX, out Matrix4<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        var (sin, cos) = SinCos(rotationX, rotationY, rotationZ);
        var sxsy = sin.X * sin.Y;
        var cxsy = cos.X * sin.Y;

        // inlined with post-multiplied rotation matrices
        matrix = new(
            a1: cos.Y * cos.Z,
            a2: cos.Y * -sin.Z,
            a3: sin.Y,
            a4: T.Zero,
            b1: cos.X * sin.Z + sxsy * cos.Z,
            b2: cos.X * cos.Z - sxsy * sin.Z,
            b3: -sin.X * cos.Y,
            b4: T.Zero,
            c1: sin.X * sin.Z - cxsy * cos.Z,
            c2: sin.X * cos.Z + cxsy * sin.Z,
            c3: cos.X * cos.Y,
            c4: T.Zero,
            d1: T.Zero, d2: T.Zero, d3: T.Zero, d4: T.One
            );
        }

    /// <summary> <inheritdoc cref="CreateRotationZYX{T}(Angle{T}, Angle{T}, Angle{T}, out Matrix4{T})"/>
    /// </summary>
    /// <remarks> <inheritdoc cref="Docs.UsesPremultiplicationConvention{T}"/>
    /// </remarks>
    public static void CreatePremultipliedRotationZYX<T>(Angle<T> rotationZ, Angle<T> rotationY, Angle<T> rotationX, out Matrix4<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        var (sin, cos) = SinCos(rotationX, rotationY, rotationZ);
        var szsy = sin.Z * sin.Y;
        var czsy = cos.Z * sin.Y;

        // inlined with pre-multiplied rotation matrices
        matrix = new(
            a1: cos.Z * cos.Y,
            a2: sin.Z * cos.X + czsy * sin.X, // sz * cx + cz * sy * sx
            a3: sin.Z * sin.X - czsy * cos.X, // sz * sx - cz * sy * cx
            a4: T.Zero,
            b1: -sin.Z * cos.Y,
            b2: cos.Z * cos.X - szsy * sin.X, // cz * cx - sz * sy * sx
            b3: cos.Z * sin.X + szsy * cos.X, // cz * sx + sz * sy * cx
            b4: T.Zero,
            c1: sin.Y,
            c2: -cos.Y * sin.X,
            c3: cos.Y * cos.X,
            c4: T.Zero,
            d1: T.Zero, d2: T.Zero, d3: T.Zero, d4: T.One
            );
        }

    /// <summary>
    /// <inheritdoc cref="Docs.IsExtrinsicZYXRotation{T}"/>
    /// <inheritdoc cref="Docs.EquivalentIntrinsicRotationIs{T}"/>
    /// <c><see cref="CreateRotationXYZ{T}(Angle{T}, Angle{T}, Angle{T}, out Matrix4{T})">extrinsic XYZ</see></c>.
    /// </summary>
    /// <remarks> <inheritdoc cref="Docs.MultiplicationConventionInstructions{T}"/>
    /// </remarks>
    public static void CreateRotationZYX<T>(Angle<T> rotationZ, Angle<T> rotationY, Angle<T> rotationX, out Matrix4<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
#if MATRIX_PREMULTIPLIED_CONVENTION
        CreatePremultipliedRotationZYX(rotationZ, rotationY, rotationX, out matrix);
#elif MATRIX_POSTMULTIPLIED_CONVENTION
        CreatePostmultipliedRotationZYX(rotationZ, rotationY, rotationX, out matrix);
#else
        throw new ApplicationException(Matrix.Docs.BaseMessage);
#endif
        }

    #endregion Factories - Multi-Axis Rotation ZYX

    #region Factories - Orthographic

    /// <summary> <inheritdoc cref="IMatrix{T}.I3DProjectionMatrix{Matrix}.CreateOrthographic(T, T, T, T)"/>
    /// </summary>
    /// <remarks> <inheritdoc cref="Docs.HandednessInstructions{T}"/><inheritdoc cref="Docs.MultiplicationConventionInstructions{T}"/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CreateOrthographic<T>(T width, T height, T nearPlane, T farPlane, out Matrix4<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
#if MATRIX_LEFTHANDED_COORDINATE_SYSTEM && MATRIX_POSTMULTIPLIED_CONVENTION
        CreatePostmultipliedLeftHandedOrthographic(width, height, nearPlane, farPlane, out matrix);
#elif MATRIX_RIGHTHANDED_COORDINATE_SYSTEM && MATRIX_POSTMULTIPLIED_CONVENTION
        CreatePostmultipliedRightHandedOrthographic(width, height, nearPlane, farPlane, out matrix);
#elif MATRIX_LEFTHANDED_COORDINATE_SYSTEM && MATRIX_PREMULTIPLIED_CONVENTION
        CreatePremultipliedLeftHandedOrthographic(width, height, nearPlane, farPlane, out matrix);
#elif MATRIX_RIGHTHANDED_COORDINATE_SYSTEM && MATRIX_PREMULTIPLIED_CONVENTION
        CreatePremultipliedRightHandedOrthographic(width, height, nearPlane, farPlane, out matrix);
#else
        throw new ApplicationException(Matrix.Docs.BaseMessage);
#endif
        }

    /// <summary> <inheritdoc cref="CreateOrthographic{T}(T, T, T, T, out Matrix4{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesLeftHandedRotation{T}"/><inheritdoc cref="Docs.UsesPostmultiplicationConvention{T}"/>
    /// </remarks>
    public static void CreatePostmultipliedLeftHandedOrthographic<T>(T width, T height, T nearPlane, T farPlane,
        out Matrix4<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        var range = T.One / (farPlane - nearPlane);
        var two = GenericNumbers<T>.Two;
        matrix = new(
            two / width, T.Zero, T.Zero, T.Zero,
            T.Zero, two / height, T.Zero, T.Zero,
            T.Zero, T.Zero, range, -nearPlane * range,
            T.Zero, T.Zero, T.Zero, T.One
        );
        }

    /// <summary> <inheritdoc cref="CreateOrthographic{T}(T, T, T, T, out Matrix4{T})"/>
    /// </summary>
    /// <remarks> <inheritdoc cref="Docs.UsesRightHandedRotation{T}"/><inheritdoc cref="Docs.UsesPostmultiplicationConvention{T}"/>
    /// </remarks>
    public static void CreatePostmultipliedRightHandedOrthographic<T>(T width, T height, T nearPlane, T farPlane,
        out Matrix4<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        var range = T.One / (nearPlane - farPlane); // as opposed to (farPlane-nearPlane) for left-handed coordinates
        var two = GenericNumbers<T>.Two;
        matrix = new(
            two / width, T.Zero, T.Zero, T.Zero,
            T.Zero, two / height, T.Zero, T.Zero,
            T.Zero, T.Zero, range, nearPlane * range, // as opposed to D3 being negated for left-handed coordinates
            T.Zero, T.Zero, T.Zero, T.One
        );
        }

    /// <summary> <inheritdoc cref="CreateOrthographic{T}(T, T, T, T, out Matrix4{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesLeftHandedRotation{T}"/><inheritdoc cref="Docs.UsesPremultiplicationConvention{T}"/>
    /// </remarks>
    public static void CreatePremultipliedLeftHandedOrthographic<T>(T width, T height, T nearPlane, T farPlane,
        out Matrix4<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        var range = T.One / (farPlane - nearPlane);
        var two = GenericNumbers<T>.Two;
        matrix = new(
            two / width, T.Zero, T.Zero, T.Zero,
            T.Zero, two / height, T.Zero, T.Zero,
            T.Zero, T.Zero, range, T.Zero,
            T.Zero, T.Zero, -nearPlane * range, T.One
        );
        }

    /// <summary> <inheritdoc cref="CreateOrthographic{T}(T, T, T, T, out Matrix4{T})"/>
    /// </summary>
    /// <remarks> <inheritdoc cref="Docs.UsesRightHandedRotation{T}"/><inheritdoc cref="Docs.UsesPremultiplicationConvention{T}"/>
    /// </remarks>
    public static void CreatePremultipliedRightHandedOrthographic<T>(T width, T height, T nearPlane, T farPlane,
        out Matrix4<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        var range = T.One / (nearPlane - farPlane); // as opposed to (farPlane-nearPlane) for left-handed coordinates
        var two = GenericNumbers<T>.Two;
        matrix = new(
            two / width, T.Zero, T.Zero, T.Zero,
            T.Zero, two / height, T.Zero, T.Zero,
            T.Zero, T.Zero, range, T.Zero,
            T.Zero, T.Zero, nearPlane * range, T.One // as opposed to D3 being negated for left-handed coordinates
        );
        }

    #endregion Factories - Orthographic

    #region Factories - Orthographic Off-center

    /// <summary> <inheritdoc cref="IMatrix{T}.I3DProjectionMatrix{Matrix}.CreateOrthographicOffCenter(T, T, T, T, T, T)"/>
    /// </summary>
    /// <remarks> <inheritdoc cref="Docs.HandednessInstructions{T}"/><inheritdoc cref="Docs.MultiplicationConventionInstructions{T}"/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CreateOrthographicOffCenter<T>(T left, T right, T top, T bottom, T nearPlane, T farPlane,
        out Matrix4<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
#if MATRIX_LEFTHANDED_COORDINATE_SYSTEM && MATRIX_PREMULTIPLIED_CONVENTION
        CreatePremultipliedLeftHandedOrthographicOffCenter(left, right, top, bottom, nearPlane, farPlane, out matrix);
#elif MATRIX_RIGHTHANDED_COORDINATE_SYSTEM && MATRIX_PREMULTIPLIED_CONVENTION
        CreatePremultipliedRightHandedOrthographicOffCenter(left, right, top, bottom, nearPlane, farPlane, out matrix);
#elif MATRIX_LEFTHANDED_COORDINATE_SYSTEM && MATRIX_POSTMULTIPLIED_CONVENTION
        CreatePostmultipliedLeftHandedOrthographicOffCenter(left, right, top, bottom, nearPlane, farPlane, out matrix);
#elif MATRIX_RIGHTHANDED_COORDINATE_SYSTEM && MATRIX_POSTMULTIPLIED_CONVENTION
        CreatePostmultipliedRightHandedOrthographicOffCenter(left, right, top, bottom, nearPlane, farPlane, out matrix);
#else
        throw new ApplicationException(Matrix.Docs.BaseMessage);
#endif
        }

    /// <summary> <inheritdoc cref="IMatrix{T}.I3DProjectionMatrix{Matrix}.CreateOrthographicOffCenter(T, T, T, T, T, T)"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesLeftHandedRotation{T}"/><inheritdoc cref="Docs.UsesPostmultiplicationConvention{T}"/>
    /// </remarks>
    [BunnyAttributes.SIMDCandidate]
    public static void CreatePostmultipliedLeftHandedOrthographicOffCenter<T>(T left, T right, T top, T bottom, T nearPlane,
        T farPlane, out Matrix4<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        var rWidth = T.One / (right - left);
        var rHeight = T.One / (top - bottom);
        var range = T.One / (farPlane - nearPlane);
        matrix = new(
            rWidth + rWidth,
            T.Zero,
            T.Zero,
            -(left + right) * rWidth,
            T.Zero,
            rHeight + rHeight,
            T.Zero,
            -(top + bottom) * rHeight,
            T.Zero,
            T.Zero,
            range,
            -nearPlane * range,
            T.Zero,
            T.Zero,
            T.Zero,
            T.One
        );
        }

    /// <summary> <inheritdoc cref="IMatrix{T}.I3DProjectionMatrix{Matrix}.CreateOrthographicOffCenter(T, T, T, T, T, T)"/>
    /// </summary>
    /// <remarks> <inheritdoc cref="Docs.UsesRightHandedRotation{T}"/><inheritdoc cref="Docs.UsesPostmultiplicationConvention{T}"/>
    /// </remarks>
    [BunnyAttributes.SIMDCandidate]
    public static void CreatePostmultipliedRightHandedOrthographicOffCenter<T>(T left, T right, T top, T bottom, T nearPlane,
        T farPlane, out Matrix4<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        var rWidth = T.One / (right - left);
        var rHeight = T.One / (top - bottom);
        var range = T.One / (nearPlane - farPlane); // as opposed to (farPlane-nearPlane) for left-handed coordinates
        matrix = new(
            rWidth + rWidth,
            T.Zero,
            T.Zero,
            -(left + right) * rWidth,
            T.Zero,
            rHeight + rHeight,
            T.Zero,
            -(top + bottom) * rHeight,
            T.Zero,
            T.Zero,
            range,
            nearPlane * range, // as opposed to C4 being negated for left-handed coordinates
            T.Zero,
            T.Zero,
            T.Zero,
            T.One
        );
        }

    /// <summary> <inheritdoc cref="IMatrix{T}.I3DProjectionMatrix{Matrix}.CreateOrthographicOffCenter(T, T, T, T, T, T)"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesLeftHandedRotation{T}"/><inheritdoc cref="Docs.UsesPremultiplicationConvention{T}"/>
    /// </remarks>
    [BunnyAttributes.SIMDCandidate]
    public static void CreatePremultipliedLeftHandedOrthographicOffCenter<T>(T left, T right, T top, T bottom, T nearPlane,
        T farPlane, out Matrix4<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        var rWidth = T.One / (right - left);
        var rHeight = T.One / (top - bottom);
        var range = T.One / (farPlane - nearPlane);
        matrix = new(
            rWidth + rWidth,
            T.Zero,
            T.Zero,
            T.Zero,
            T.Zero,
            rHeight + rHeight,
            T.Zero,
            T.Zero,
            T.Zero,
            T.Zero,
            range,
            T.Zero,
            -(left + right) * rWidth,
            -(top + bottom) * rHeight,
            -nearPlane * range,
            T.One
        );
        }

    /// <summary> <inheritdoc cref="IMatrix{T}.I3DProjectionMatrix{Matrix}.CreateOrthographicOffCenter(T, T, T, T, T, T)"/>
    /// </summary>
    /// <remarks> <inheritdoc cref="Docs.UsesRightHandedRotation{T}"/><inheritdoc cref="Docs.UsesPremultiplicationConvention{T}"/>
    /// </remarks>
    [BunnyAttributes.SIMDCandidate]
    public static void CreatePremultipliedRightHandedOrthographicOffCenter<T>(T left, T right, T top, T bottom, T nearPlane,
        T farPlane, out Matrix4<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        var rWidth = T.One / (right - left);
        var rHeight = T.One / (top - bottom);
        var range = T.One / (nearPlane - farPlane); // as opposed to (farPlane-nearPlane) for left-handed coordinates
        matrix = new(
            rWidth + rWidth,
            T.Zero,
            T.Zero,
            T.Zero,
            T.Zero,
            rHeight + rHeight,
            T.Zero,
            T.Zero,
            T.Zero,
            T.Zero,
            range,
            T.Zero,
            -(left + right) * rWidth,
            -(top + bottom) * rHeight,
            nearPlane * range, // as opposed to D3 being negated for left-handed coordinates
            T.One
        );
        }

    #endregion Factories - Orthographic Off-center

    #region Factories - Perspective

    /// <summary> <inheritdoc cref="IMatrix{T}.I3DProjectionMatrix{Matrix}.CreatePerspective(T, T, T, T)"/>
    /// </summary>
    /// <remarks> <inheritdoc cref="Docs.HandednessInstructions{T}"/><inheritdoc cref="Docs.MultiplicationConventionInstructions{T}"/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CreatePerspective<T>(T width, T height, T nearPlane, T farPlane, out Matrix4<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
#if MATRIX_LEFTHANDED_COORDINATE_SYSTEM && MATRIX_PREMULTIPLIED_CONVENTION
        CreatePremultipliedLeftHandedPerspective(width, height, nearPlane, farPlane, out matrix);
#elif MATRIX_RIGHTHANDED_COORDINATE_SYSTEM && MATRIX_PREMULTIPLIED_CONVENTION
        CreatePremultipliedRightHandedPerspective(width, height, nearPlane, farPlane, out matrix);
#elif MATRIX_LEFTHANDED_COORDINATE_SYSTEM && MATRIX_POSTMULTIPLIED_CONVENTION
        CreatePostmultipliedLeftHandedPerspective(width, height, nearPlane, farPlane, out matrix);
#elif MATRIX_RIGHTHANDED_COORDINATE_SYSTEM && MATRIX_POSTMULTIPLIED_CONVENTION
        CreatePostmultipliedRightHandedPerspective(width, height, nearPlane, farPlane, out matrix);
#else
        throw new ApplicationException(Matrix.Docs.BaseMessage);
#endif
        }

    /// <summary><inheritdoc cref="CreatePerspective{T}(T, T, T, T, out Matrix4{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesLeftHandedRotation{T}"/><inheritdoc cref="Docs.UsesPostmultiplicationConvention{T}"/>
    /// </remarks>
    public static void CreatePostmultipliedLeftHandedPerspective<T>(T width, T height, T nearPlane, T farPlane, out Matrix4<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        var range = farPlane / (farPlane - nearPlane);
        matrix = new(
            (nearPlane + nearPlane) / width, T.Zero, T.Zero, T.Zero,
            T.Zero, (nearPlane + nearPlane) / height, T.Zero, T.Zero,
            T.Zero, T.Zero, range, -nearPlane * range,
            T.Zero, T.Zero, T.One, T.Zero
            );
        }

    /// <summary><inheritdoc cref="CreatePerspective{T}(T, T, T, T, out Matrix4{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesRightHandedRotation{T}"/><inheritdoc cref="Docs.UsesPostmultiplicationConvention{T}"/>
    /// </remarks>
    public static void CreatePostmultipliedRightHandedPerspective<T>(T width, T height, T nearPlane, T farPlane, out Matrix4<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        var range = farPlane / (nearPlane - farPlane); // as opposed to (farPlane-nearPlane) for left-handed coordinates
        matrix = new(
            (nearPlane + nearPlane) / width, T.Zero, T.Zero, T.Zero,
            T.Zero, (nearPlane + nearPlane) / height, T.Zero, T.Zero,
            T.Zero, T.Zero, range, nearPlane * range, // as opposed to C4 being inverted for left-handed coordinates
            T.Zero, T.Zero, T.NegativeOne, T.Zero // as opposed to D3 being +1 for left-handed coordinates
            );
        }

    /// <summary><inheritdoc cref="CreatePerspective{T}(T, T, T, T, out Matrix4{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesLeftHandedRotation{T}"/><inheritdoc cref="Docs.UsesPremultiplicationConvention{T}"/>
    /// </remarks>
    public static void CreatePremultipliedLeftHandedPerspective<T>(T width, T height, T nearPlane, T farPlane, out Matrix4<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        var range = farPlane / (farPlane - nearPlane);
        matrix = new(
            (nearPlane + nearPlane) / width, T.Zero, T.Zero, T.Zero,
            T.Zero, (nearPlane + nearPlane) / height, T.Zero, T.Zero,
            T.Zero, T.Zero, range, T.One,
            T.Zero, T.Zero, -nearPlane * range, T.Zero
            );
        }

    /// <summary><inheritdoc cref="CreatePerspective{T}(T, T, T, T, out Matrix4{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesRightHandedRotation{T}"/><inheritdoc cref="Docs.UsesPremultiplicationConvention{T}"/>
    /// </remarks>
    public static void CreatePremultipliedRightHandedPerspective<T>(T width, T height, T nearPlane, T farPlane, out Matrix4<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        var range = farPlane / (nearPlane - farPlane); // as opposed to (farPlane-nearPlane) for left-handed coordinates
        matrix = new(
            (nearPlane + nearPlane) / width, T.Zero, T.Zero, T.Zero,
            T.Zero, (nearPlane + nearPlane) / height, T.Zero, T.Zero,
            T.Zero, T.Zero, range, T.NegativeOne, // as opposed to C4 being +1 for left-handed coordinates
            T.Zero, T.Zero, nearPlane * range, T.Zero // as opposed to D3 being inverted for left-handed coordinates
            );
        }

    #endregion Factories - Perspective

    #region Factories - Perspective Off-Center

    /// <summary> <inheritdoc cref="IMatrix{T}.I3DProjectionMatrix{Matrix}.CreatePerspectiveOffCenter(T, T, T, T, T, T)"/>
    /// </summary>
    /// <remarks> <inheritdoc cref="Docs.HandednessInstructions{T}"/><inheritdoc cref="Docs.MultiplicationConventionInstructions{T}"/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CreatePerspectiveOffCenter<T>(T left, T right, T top, T bottom, T nearPlane, T farPlane, out Matrix4<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
#if MATRIX_LEFTHANDED_COORDINATE_SYSTEM && MATRIX_PREMULTIPLIED_CONVENTION
        CreatePremultipliedLeftHandedPerspectiveOffCenter(left, right, top, bottom, nearPlane, farPlane, out matrix);
#elif MATRIX_RIGHTHANDED_COORDINATE_SYSTEM && MATRIX_PREMULTIPLIED_CONVENTION
        CreatePremultipliedRightHandedPerspectiveOffCenter(left, right, top, bottom, nearPlane, farPlane, out matrix);
#elif MATRIX_LEFTHANDED_COORDINATE_SYSTEM && MATRIX_POSTMULTIPLIED_CONVENTION
        CreatePostmultipliedLeftHandedPerspectiveOffCenter(left, right, top, bottom, nearPlane, farPlane, out matrix);
#elif MATRIX_RIGHTHANDED_COORDINATE_SYSTEM && MATRIX_POSTMULTIPLIED_CONVENTION
        CreatePostmultipliedRightHandedPerspectiveOffCenter(left, right, top, bottom, nearPlane, farPlane, out matrix);
#else
        throw new ApplicationException(Matrix.Docs.BaseMessage);
#endif
        }

    /// <summary><inheritdoc cref="CreatePerspectiveOffCenter{T}(T, T, T, T, T, T, out Matrix4{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesLeftHandedRotation{T}"/><inheritdoc cref="Docs.UsesPostmultiplicationConvention{T}"/>
    /// </remarks>
    [BunnyAttributes.SIMDCandidate]
    public static void CreatePostmultipliedLeftHandedPerspectiveOffCenter<T>(T left, T right, T top, T bottom, T nearPlane, T farPlane, out Matrix4<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        var near2 = nearPlane + nearPlane;
        var rWidth = T.One / (right - left);
        var rHeight = T.One / (top - bottom);
        var range = farPlane / (farPlane - nearPlane);
        matrix = new(
            a1: near2 * rWidth,
            a2: T.Zero,
            a3: -(left + right) * rWidth,
            a4: T.Zero,
            b1: T.Zero,
            b2: near2 * rHeight,
            b3: -(top + bottom) * rHeight,
            b4: T.Zero,
            c1: T.Zero,
            c2: T.Zero,
            c3: range,
            c4: -range * nearPlane,
            d1: T.Zero,
            d2: T.Zero,
            d3: T.One,
            d4: T.Zero
            );
        }

    /// <summary><inheritdoc cref="CreatePerspectiveOffCenter{T}(T, T, T, T, T, T, out Matrix4{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesRightHandedRotation{T}"/><inheritdoc cref="Docs.UsesPostmultiplicationConvention{T}"/>
    /// </remarks>
    [BunnyAttributes.SIMDCandidate]
    public static void CreatePostmultipliedRightHandedPerspectiveOffCenter<T>(T left, T right, T top, T bottom, T nearPlane, T farPlane, out Matrix4<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        var near2 = nearPlane + nearPlane;
        var rWidth = T.One / (right - left);
        var rHeight = T.One / (top - bottom);
        var range = farPlane / (nearPlane - farPlane); // as opposed to (farPlane-nearPlane) for left-handed coordinates
        matrix = new(
            a1: near2 * rWidth,
            a2: T.Zero,
            a3: (left + right) * rWidth,  // as opposed to negated for left-handed coordinates
            a4: T.Zero,
            b1: T.Zero,
            b2: near2 * rHeight,
            b3: (top + bottom) * rHeight, // as opposed to negated for left-handed coordinates
            b4: T.Zero,
            c1: T.Zero,
            c2: T.Zero,
            c3: range,
            c4: range * nearPlane, // as opposed to negated for left-handed coordinates
            d1: T.Zero,
            d2: T.Zero,
            d3: T.NegativeOne, // as opposed to +1 for left-handed coordinates
            d4: T.Zero
            );
        }

    /// <summary><inheritdoc cref="CreatePerspectiveOffCenter{T}(T, T, T, T, T, T, out Matrix4{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesLeftHandedRotation{T}"/><inheritdoc cref="Docs.UsesPremultiplicationConvention{T}"/>
    /// </remarks>
    [BunnyAttributes.SIMDCandidate]
    public static void CreatePremultipliedLeftHandedPerspectiveOffCenter<T>(T left, T right, T top, T bottom, T nearPlane, T farPlane, out Matrix4<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        var near2 = nearPlane + nearPlane;
        var rWidth = T.One / (right - left);
        var rHeight = T.One / (top - bottom);
        var range = farPlane / (farPlane - nearPlane);
        matrix = new(
            a1: near2 * rWidth,
            a2: T.Zero,
            a3: T.Zero,
            a4: T.Zero,
            b1: T.Zero,
            b2: near2 * rHeight,
            b3: T.Zero,
            b4: T.Zero,
            c1: -(left + right) * rWidth,
            c2: -(top + bottom) * rHeight,
            c3: range,
            c4: T.One,
            d1: T.Zero,
            d2: T.Zero,
            d3: -range * nearPlane,
            d4: T.Zero
            );
        }

    /// <summary><inheritdoc cref="CreatePerspectiveOffCenter{T}(T, T, T, T, T, T, out Matrix4{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesRightHandedRotation{T}"/><inheritdoc cref="Docs.UsesPremultiplicationConvention{T}"/>
    /// </remarks>
    [BunnyAttributes.SIMDCandidate]
    public static void CreatePremultipliedRightHandedPerspectiveOffCenter<T>(T left, T right, T top, T bottom, T nearPlane, T farPlane, out Matrix4<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        var near2 = nearPlane + nearPlane;
        var rWidth = T.One / (right - left);
        var rHeight = T.One / (top - bottom);
        var range = farPlane / (nearPlane - farPlane); // as opposed to (farPlane-nearPlane) for left-handed coordinates
        matrix = new(
            a1: near2 * rWidth,
            a2: T.Zero,
            a3: T.Zero,
            a4: T.Zero,
            b1: T.Zero,
            b2: near2 * rHeight,
            b3: T.Zero,
            b4: T.Zero,
            c1: (left + right) * rWidth,  // as opposed to negated for left-handed coordinates
            c2: (top + bottom) * rHeight, // as opposed to negated for left-handed coordinates
            c3: range,
            c4: T.NegativeOne, // as opposed to +1 for left-handed coordinates
            d1: T.Zero,
            d2: T.Zero,
            d3: range * nearPlane, // as opposed to negated for left-handed coordinates
            d4: T.Zero
            );
        }

    #endregion Factories - Perspective Off-Center

    #region Factories - Perspective FOV

    /// <summary> <inheritdoc cref="IMatrix{T}.I3DProjectionMatrix{Matrix}.CreatePerspectiveFOV(Angle{T}, T, T, T)"/>
    /// </summary>
    /// <remarks> <inheritdoc cref="Docs.HandednessInstructions{T}"/><inheritdoc cref="Docs.MultiplicationConventionInstructions{T}"/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CreatePerspectiveFOV<T>(Angle<T> fov, T aspectRatio, T nearPlane, T farPlane, out Matrix4<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
#if MATRIX_LEFTHANDED_COORDINATE_SYSTEM && MATRIX_PREMULTIPLIED_CONVENTION
        CreatePremultipliedLeftHandedPerspectiveFOV(fov, aspectRatio, nearPlane, farPlane, out matrix);
#elif MATRIX_RIGHTHANDED_COORDINATE_SYSTEM && MATRIX_PREMULTIPLIED_CONVENTION
        CreatePremultipliedRightHandedPerspectiveFOV(fov, aspectRatio, nearPlane, farPlane, out matrix);
#elif MATRIX_LEFTHANDED_COORDINATE_SYSTEM && MATRIX_POSTMULTIPLIED_CONVENTION
        CreatePostmultipliedLeftHandedPerspectiveFOV(fov, aspectRatio, nearPlane, farPlane, out matrix);
#elif MATRIX_RIGHTHANDED_COORDINATE_SYSTEM && MATRIX_POSTMULTIPLIED_CONVENTION
        CreatePostmultipliedRightHandedPerspectiveFOV(fov, aspectRatio, nearPlane, farPlane, out matrix);
#else
        throw new ApplicationException(Matrix.Docs.BaseMessage);
#endif
        }

    /// <summary><inheritdoc cref="CreatePerspectiveFOV{T}(Angle{T}, T, T, T, out Matrix4{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesLeftHandedRotation{T}"/><inheritdoc cref="Docs.UsesPostmultiplicationConvention{T}"/>
    /// </remarks>
    [BunnyAttributes.SIMDCandidate]
    public static void CreatePostmultipliedLeftHandedPerspectiveFOV<T>(Angle<T> fov, T aspectRatio, T nearPlane, T farPlane, out Matrix4<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        var (sin, cos) = T.SinCos(fov.Radians * GenericNumbers<T>.OneHalf);
        var height = cos / sin;
        var width = height / aspectRatio;
        var range = farPlane / (farPlane - nearPlane);
        matrix = new(
            width, T.Zero, T.Zero, T.Zero,
            T.Zero, height, T.Zero, T.Zero,
            T.Zero, T.Zero, range, -range * nearPlane,
            T.Zero, T.Zero, T.One, T.Zero
            );
        }

    /// <summary><inheritdoc cref="CreatePerspectiveFOV{T}(Angle{T}, T, T, T, out Matrix4{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesRightHandedRotation{T}"/><inheritdoc cref="Docs.UsesPostmultiplicationConvention{T}"/>
    /// </remarks>
    [BunnyAttributes.SIMDCandidate]
    public static void CreatePostmultipliedRightHandedPerspectiveFOV<T>(Angle<T> fov, T aspectRatio, T nearPlane, T farPlane, out Matrix4<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        var (sin, cos) = T.SinCos(fov.Radians * GenericNumbers<T>.OneHalf);
        var height = cos / sin;
        var width = height / aspectRatio;
        var range = farPlane / (nearPlane - farPlane); // as opposed to (farPlane-nearPlane) for left-handed coordinates
        matrix = new(
            width, T.Zero, T.Zero, T.Zero,
            T.Zero, height, T.Zero, T.Zero,
            T.Zero, T.Zero, range, range * nearPlane, // as opposed to C4 being inverted for left-handed coordinates
            T.Zero, T.Zero, T.NegativeOne, T.Zero // as opposed to D3 being +1 for left-handed coordinates
            );
        }

    /// <summary><inheritdoc cref="CreatePerspectiveFOV{T}(Angle{T}, T, T, T, out Matrix4{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesLeftHandedRotation{T}"/><inheritdoc cref="Docs.UsesPremultiplicationConvention{T}"/>
    /// </remarks>
    [BunnyAttributes.SIMDCandidate]
    public static void CreatePremultipliedLeftHandedPerspectiveFOV<T>(Angle<T> fov, T aspectRatio, T nearPlane, T farPlane, out Matrix4<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        var (sin, cos) = T.SinCos(fov.Radians * GenericNumbers<T>.OneHalf);
        var height = cos / sin;
        var width = height / aspectRatio;
        var range = farPlane / (farPlane - nearPlane);
        matrix = new(
            width, T.Zero, T.Zero, T.Zero,
            T.Zero, height, T.Zero, T.Zero,
            T.Zero, T.Zero, range, T.One,
            T.Zero, T.Zero, -range * nearPlane, T.Zero
            );
        }

    /// <summary><inheritdoc cref="CreatePerspectiveFOV{T}(Angle{T}, T, T, T, out Matrix4{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesRightHandedRotation{T}"/><inheritdoc cref="Docs.UsesPremultiplicationConvention{T}"/>
    /// </remarks>
    [BunnyAttributes.SIMDCandidate]
    public static void CreatePremultipliedRightHandedPerspectiveFOV<T>(Angle<T> fov, T aspectRatio, T nearPlane, T farPlane, out Matrix4<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        var (sin, cos) = T.SinCos(fov.Radians * GenericNumbers<T>.OneHalf);
        var height = cos / sin;
        var width = height / aspectRatio;
        var range = farPlane / (nearPlane - farPlane); // as opposed to (farPlane-nearPlane) for left-handed coordinates
        matrix = new(
            width, T.Zero, T.Zero, T.Zero,
            T.Zero, height, T.Zero, T.Zero,
            T.Zero, T.Zero, range, T.NegativeOne, // as opposed to C4 being +1 for left-handed coordinates
            T.Zero, T.Zero, range * nearPlane, T.Zero // as opposed to D3 being inverted for left-handed coordinates
            );
        }

    #endregion Factories - Perspective FOV

    #region Factories - Reflection

    /// <summary><inheritdoc cref="CreateReflection{T}(Plane3{T}, out Matrix4{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPostmultiplicationConvention{T}"/>
    /// </remarks>
    [BunnyAttributes.SIMDCandidate]
    public static void CreatePostmultipliedReflection<T>(Plane3<T> plane, out Matrix4<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        var negativeTwo = -GenericNumbers<T>.Two;
        var norm = plane.Normalize();
        var v = norm.Normal;
        var d = -norm.Distance;
        var x = v.X;
        var y = v.Y;
        var z = v.Z;
        var nx = x * negativeTwo;
        var ny = y * negativeTwo;
        var nz = z * negativeTwo;
        matrix = new(
            a1: nx * x + T.One,
            a2: ny * x,
            a3: nz * x,
            a4: nx * d,
            b1: nx * y,
            b2: ny * y + T.One,
            b3: nz * y,
            b4: ny * d,
            c1: nx * z,
            c2: ny * z,
            c3: nz * z + T.One,
            c4: nz * d,
            d1: T.Zero,
            d2: T.Zero,
            d3: T.Zero,
            d4: T.One
            );
        }

    /// <summary><inheritdoc cref="CreateReflection{T}(Plane3{T}, out Matrix4{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPremultiplicationConvention{T}"/>
    /// </remarks>
    [BunnyAttributes.SIMDCandidate]
    public static void CreatePremultipliedReflection<T>(Plane3<T> plane, out Matrix4<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        var negativeTwo = -GenericNumbers<T>.Two;
        var norm = plane.Normalize();
        var v = norm.Normal;
        var d = -norm.Distance;
        var x = v.X;
        var y = v.Y;
        var z = v.Z;
        var nx = x * negativeTwo;
        var ny = y * negativeTwo;
        var nz = z * negativeTwo;
        matrix = new(
            a1: nx * x + T.One,
            a2: ny * x,
            a3: nz * x,
            a4: T.Zero,
            b1: nx * y,
            b2: ny * y + T.One,
            b3: nz * y,
            b4: T.Zero,
            c1: nx * z,
            c2: ny * z,
            c3: nz * z + T.One,
            c4: T.Zero,
            d1: nx * d,
            d2: ny * d,
            d3: nz * d,
            d4: T.One
            );
        }

    /// <summary><inheritdoc cref="IMatrix{T}.I3DProjectionMatrix{Matrix}.CreateReflection(Plane3{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.MultiplicationConventionInstructions{T}"/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CreateReflection<T>(Plane3<T> plane, out Matrix4<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
#if MATRIX_PREMULTIPLIED_CONVENTION
        CreatePremultipliedReflection(plane, out matrix);
#elif MATRIX_POSTMULTIPLIED_CONVENTION
        CreatePostmultipliedReflection(plane, out matrix);
#else
        throw new ApplicationException(Matrix.Docs.BaseMessage);
#endif
        }

    #endregion Factories - Reflection

    #region Factories - Rotation 🡺 Scale

    /// <summary><inheritdoc cref="CreateRotationScale{T}(Quaternion{T}, Vector3{T}, out Matrix4{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPostmultiplicationConvention{T}"/>
    /// </remarks>
    [BunnyAttributes.SIMDCandidate]
    public static void CreatePostmultipliedRotationScale<T>(Quaternion<T> rotation, Vector3<T> scale, out Matrix3x4<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        CreatePostmultipliedRotation(rotation, out Matrix4<T> r);
        matrix = new(
            a1: r.A1 * scale.X, a2: r.A2 * scale.Y, a3: r.A3 * scale.Z, a4: T.Zero,
            b1: r.B1 * scale.X, b2: r.B2 * scale.Y, b3: r.B3 * scale.Z, b4: T.Zero,
            c1: r.C1 * scale.X, c2: r.C2 * scale.Y, c3: r.C3 * scale.Z, c4: T.Zero
        );
        }

    /// <summary><inheritdoc cref="CreateRotationScale{T}(Quaternion{T}, Vector3{T}, out Matrix4{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPostmultiplicationConvention{T}"/>
    /// </remarks>
    [BunnyAttributes.SIMDCandidate]
    public static void CreatePostmultipliedRotationScale<T>(Quaternion<T> rotation, Vector3<T> scale, out Matrix4<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        CreatePostmultipliedRotation(rotation, out Matrix4<T> r);
        matrix = new(
            a1: r.A1 * scale.X, a2: r.A2 * scale.Y, a3: r.A3 * scale.Z, a4: T.Zero,
            b1: r.B1 * scale.X, b2: r.B2 * scale.Y, b3: r.B3 * scale.Z, b4: T.Zero,
            c1: r.C1 * scale.X, c2: r.C2 * scale.Y, c3: r.C3 * scale.Z, c4: T.Zero,
            T.Zero, T.Zero, T.Zero, T.One
            );
        }

    /// <summary><inheritdoc cref="CreateRotationScale{T}(Quaternion{T}, Vector3{T}, out Matrix4{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPostmultiplicationConvention{T}"/>
    /// </remarks>
    [BunnyAttributes.SIMDCandidate]
    public static void CreatePostmultipliedRotationScale<T>(Quaternion<T> rotation, Vector3<T> scale, out Matrix3<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        CreatePostmultipliedRotation(rotation, out Matrix4<T> r);
        matrix = new(
            a1: r.A1 * scale.X, a2: r.A2 * scale.Y, a3: r.A3 * scale.Z,
            b1: r.B1 * scale.X, b2: r.B2 * scale.Y, b3: r.B3 * scale.Z,
            c1: r.C1 * scale.X, c2: r.C2 * scale.Y, c3: r.C3 * scale.Z
            );
        }

    /// <summary><inheritdoc cref="CreateRotationScale{T}(Angle{T}, Vector2{T}, out Matrix3{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPostmultiplicationConvention{T}"/>
    /// </remarks>
    [BunnyAttributes.SIMDCandidate]
    public static void CreatePostmultipliedRotationScale<T>(Angle<T> rotation, Vector2<T> scale, out Matrix3<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        CreatePostmultipliedRotation2D(rotation.Radians, out Vector2<T> rowA, out Vector2<T> rowB);
        matrix = new(
            a1: rowA.X * scale.X, a2: rowA.Y * scale.Y, a3: T.Zero,
            b1: rowB.X * scale.X, b2: rowB.Y * scale.Y, b3: T.Zero,
            c1: T.Zero, c2: T.Zero, c3: T.One
            );
        }

    /// <summary><inheritdoc cref="CreateRotationScale{T}(Angle{T}, Vector2{T}, out Matrix2{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPostmultiplicationConvention{T}"/>
    /// </remarks>
    [BunnyAttributes.SIMDCandidate]
    public static void CreatePostmultipliedRotationScale<T>(Angle<T> rotation, Vector2<T> scale, out Matrix2x3<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        CreatePostmultipliedRotation2D(rotation.Radians, out var rowA, out var rowB);
        matrix = new(
            a1: rowA.X * scale.X, a2: rowA.Y * scale.Y, a3: T.Zero,
            b1: rowB.X * scale.X, b2: rowB.Y * scale.Y, b3: T.Zero
            );
        }

    /// <summary><inheritdoc cref="CreateRotationScale{T}(Angle{T}, Vector2{T}, out Matrix2{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPostmultiplicationConvention{T}"/>
    /// </remarks>
    [BunnyAttributes.SIMDCandidate]
    public static void CreatePostmultipliedRotationScale<T>(Angle<T> rotation, Vector2<T> scale, out Matrix2<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        CreatePostmultipliedRotation2D(rotation.Radians, out var rowA, out var rowB);
        matrix = new(
            a1: rowA.X * scale.X, a2: rowA.Y * scale.Y,
            b1: rowB.X * scale.X, b2: rowB.Y * scale.Y
            );
        }

    /// <summary><inheritdoc cref="CreateRotationScale{T}(Quaternion{T}, Vector3{T}, out Matrix4{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPremultiplicationConvention{T}"/>
    /// </remarks>
    [BunnyAttributes.SIMDCandidate]
    public static void CreatePremultipliedRotationScale<T>(Quaternion<T> rotation, Vector3<T> scale, out Matrix4<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        CreatePremultipliedRotation(rotation, out Matrix4<T> r);
        matrix = new(
            a1: r.A1 * scale.X, a2: r.A2 * scale.X, a3: r.A3 * scale.X, a4: T.Zero,
            b1: r.B1 * scale.Y, b2: r.B2 * scale.Y, b3: r.B3 * scale.Y, b4: T.Zero,
            c1: r.C1 * scale.Z, c2: r.C2 * scale.Z, c3: r.C3 * scale.Z, c4: T.Zero,
            d1: T.Zero, d2: T.Zero, d3: T.Zero, d4: T.One
            );
        }

    /// <summary><inheritdoc cref="CreateRotationScale{T}(Quaternion{T}, Vector3{T}, out Matrix4{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPremultiplicationConvention{T}"/>
    /// </remarks>
    [BunnyAttributes.SIMDCandidate]
    public static void CreatePremultipliedRotationScale<T>(Quaternion<T> rotation, Vector3<T> scale, out Matrix3<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        CreatePremultipliedRotation(rotation, out Matrix4<T> r);
        matrix = new(
            a1: r.A1 * scale.X, a2: r.A2 * scale.X, a3: r.A3 * scale.X,
            b1: r.B1 * scale.Y, b2: r.B2 * scale.Y, b3: r.B3 * scale.Y,
            c1: r.C1 * scale.Z, c2: r.C2 * scale.Z, c3: r.C3 * scale.Z
            );
        }

    /// <summary><inheritdoc cref="CreateRotationScale{T}(Angle{T}, Vector2{T}, out Matrix3{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPremultiplicationConvention{T}"/>
    /// </remarks>
    [BunnyAttributes.SIMDCandidate]
    public static void CreatePremultipliedRotationScale<T>(Angle<T> rotation, Vector2<T> scale, out Matrix3<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        CreatePremultipliedRotation2D(rotation.Radians, out Vector2<T> rowA, out Vector2<T> rowB);
        matrix = new(
            a1: rowA.X * scale.X, a2: rowA.Y * scale.X, a3: T.Zero,
            b1: rowB.X * scale.Y, b2: rowB.Y * scale.Y, b3: T.Zero,
            c1: T.Zero, c2: T.Zero, c3: T.One
            );
        }

    /// <summary><inheritdoc cref="CreateRotationScale{T}(Quaternion{T}, Vector3{T}, out Matrix4{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPremultiplicationConvention{T}"/>
    /// </remarks>
    [BunnyAttributes.SIMDCandidate]
    public static void CreatePremultipliedRotationScale<T>(Quaternion<T> rotation, Vector3<T> scale, out Matrix4x3<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        CreatePremultipliedRotation(rotation, out Matrix4<T> r);
        matrix = new(
            a1: r.A1 * scale.X, a2: r.A2 * scale.X, a3: r.A3 * scale.X,
            b1: r.B1 * scale.Y, b2: r.B2 * scale.Y, b3: r.B3 * scale.Y,
            c1: r.C1 * scale.Z, c2: r.C2 * scale.Z, c3: r.C3 * scale.Z,
            d1: T.Zero, d2: T.Zero, d3: T.Zero
            );
        }

    /// <summary><inheritdoc cref="CreateRotationScale{T}(Angle{T}, Vector2{T}, out Matrix2{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPremultiplicationConvention{T}"/>
    /// </remarks>
    [BunnyAttributes.SIMDCandidate]
    public static void CreatePremultipliedRotationScale<T>(Angle<T> rotation, Vector2<T> scale, out Matrix3x2<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        CreatePremultipliedRotation2D(rotation.Radians, out var rowA, out var rowB);
        matrix = new(
            a1: rowA.X * scale.X, a2: rowA.Y * scale.X,
            b1: rowB.X * scale.Y, b2: rowB.Y * scale.Y,
            c1: T.Zero, c2: T.Zero
            );
        }

    /// <summary><inheritdoc cref="CreateRotationScale{T}(Angle{T}, Vector2{T}, out Matrix2{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPremultiplicationConvention{T}"/>
    /// </remarks>
    [BunnyAttributes.SIMDCandidate]
    public static void CreatePremultipliedRotationScale<T>(Angle<T> rotation, Vector2<T> scale, out Matrix2<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        CreatePremultipliedRotation2D(rotation.Radians, out var rowA, out var rowB);
        matrix = new(
            a1: rowA.X * scale.X, a2: rowA.Y * scale.X,
            b1: rowB.X * scale.Y, b2: rowB.Y * scale.Y
            );
        }

    /// <summary><inheritdoc cref="IMatrix{T}.IMatrixBase{Matrix, Vector, Direction, Rotation}.CreateRotationScale(Rotation, Vector)"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.MultiplicationConventionInstructions{T}"/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CreateRotationScale<T>(Angle<T> rotation, Vector2<T> scale, out Matrix3<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
#if MATRIX_PREMULTIPLIED_CONVENTION
        CreatePremultipliedRotationScale(rotation, scale, out matrix);
#elif MATRIX_POSTMULTIPLIED_CONVENTION
        CreatePostmultipliedRotationScale(rotation, scale, out matrix);
#else
        throw new ApplicationException(Matrix.Docs.BaseMessage);
#endif
        }

    /// <summary><inheritdoc cref="IMatrix{T}.IMatrixBase{Matrix, Vector, Direction, Rotation}.CreateRotationScale(Rotation, Vector)"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.MultiplicationConventionInstructions{T}"/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CreateRotationScale<T>(Quaternion<T> rotation, Vector3<T> scale, out Matrix3<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
#if MATRIX_PREMULTIPLIED_CONVENTION
        CreatePremultipliedRotationScale(rotation, scale, out matrix);
#elif MATRIX_POSTMULTIPLIED_CONVENTION
        CreatePostmultipliedRotationScale(rotation, scale, out matrix);
#else
        throw new ApplicationException(Matrix.Docs.BaseMessage);
#endif
        }

    /// <summary><inheritdoc cref="IMatrix{T}.IMatrixBase{Matrix, Vector, Direction, Rotation}.CreateRotationScale(Rotation, Vector)"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.MultiplicationConventionInstructions{T}"/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CreateRotationScale<T>(Quaternion<T> rotation, Vector3<T> scale, out Matrix4<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
#if MATRIX_PREMULTIPLIED_CONVENTION
        CreatePremultipliedRotationScale(rotation, scale, out matrix);
#elif MATRIX_POSTMULTIPLIED_CONVENTION
        CreatePostmultipliedRotationScale(rotation, scale, out matrix);
#else
        throw new ApplicationException(Matrix.Docs.BaseMessage);
#endif
        }

    /// <summary><inheritdoc cref="IMatrix{T}.IMatrixBase{Matrix, Vector, Direction, Rotation}.CreateRotationScale(Rotation, Vector)"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.MultiplicationConventionInstructions{T}"/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CreateRotationScale<T>(Angle<T> rotation, Vector2<T> scale, out Matrix2<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
#if MATRIX_PREMULTIPLIED_CONVENTION
        CreatePremultipliedRotationScale(rotation, scale, out matrix);
#elif MATRIX_POSTMULTIPLIED_CONVENTION
        CreatePostmultipliedRotationScale(rotation, scale, out matrix);
#else
        throw new ApplicationException(Matrix.Docs.BaseMessage);
#endif
        }

    #endregion Factories - Rotation 🡺 Scale

    #region Factories - Rotation 🡺 Translation

    /// <summary><inheritdoc cref="CreateRotationTranslation{T}(Angle{T}, Vector2{T}, out Matrix3{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPostmultiplicationConvention{T}"/>
    /// </remarks>
    public static void CreatePostmultipliedRotationTranslation<T>(Angle<T> rotation, Vector2<T> translation, out Matrix2x3<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        CreatePostmultipliedRotation2D(rotation.Radians, out var rowA, out var rowB);
        matrix = new(
            a1: rowA.X, a2: rowA.Y, a3: translation.X,
            b1: rowB.X, b2: rowB.Y, b3: translation.Y
            );
        }

    /// <summary><inheritdoc cref="CreateRotationTranslation{T}(Angle{T}, Vector2{T}, out Matrix3{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPostmultiplicationConvention{T}"/>
    /// </remarks>
    public static void CreatePostmultipliedRotationTranslation<T>(Angle<T> rotation, Vector2<T> translation, out Matrix3<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        CreatePostmultipliedRotation2D(rotation.Radians, out var rowA, out var rowB);
        matrix = new(
            a1: rowA.X, a2: rowA.Y, a3: translation.X,
            b1: rowB.X, b2: rowB.Y, b3: translation.Y,
            c1: T.Zero, c2: T.Zero, c3: T.One
            );
        }

    /// <summary><inheritdoc cref="CreateRotationTranslation{T}(Quaternion{T}, Vector3{T}, out Matrix4{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPostmultiplicationConvention{T}"/>
    /// </remarks>
    public static void CreatePostmultipliedRotationTranslation<T>(Quaternion<T> rotation, Vector3<T> translation, out Matrix3x4<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        CreatePostmultipliedRotation(rotation, out Matrix4<T> temp);
        matrix = new(
            a1: temp.A1, a2: temp.A2, a3: temp.A3, a4: translation.X,
            b1: temp.B1, b2: temp.B2, b3: temp.B3, b4: translation.Y,
            c1: temp.C1, c2: temp.C2, c3: temp.C3, c4: translation.Z
            );
        }

    /// <summary><inheritdoc cref="CreateRotationTranslation{T}(Quaternion{T}, Vector3{T}, out Matrix4{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPostmultiplicationConvention{T}"/>
    /// </remarks>
    public static void CreatePostmultipliedRotationTranslation<T>(Quaternion<T> rotation, Vector3<T> translation, out Matrix4<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        CreatePostmultipliedRotation(rotation, out matrix);
        matrix.A4 = translation.X;
        matrix.B4 = translation.Y;
        matrix.C4 = translation.Z;
        matrix.D4 = T.One;
        }

    /// <summary><inheritdoc cref="CreateRotationTranslation{T}(Angle{T}, Vector2{T}, out Matrix3{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPremultiplicationConvention{T}"/>
    /// </remarks>
    public static void CreatePremultipliedRotationTranslation<T>(Angle<T> rotation, Vector2<T> translation, out Matrix3x2<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        CreatePremultipliedRotation2D(rotation.Radians, out var rowA, out var rowB);
        matrix = new(
            a1: rowA.X, a2: rowA.Y,
            b1: rowB.X, b2: rowB.Y,
            c1: translation.X, c2: translation.Y
            );
        }

    /// <summary><inheritdoc cref="CreateRotationTranslation{T}(Angle{T}, Vector2{T}, out Matrix3{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPremultiplicationConvention{T}"/>
    /// </remarks>
    public static void CreatePremultipliedRotationTranslation<T>(Angle<T> rotation, Vector2<T> translation, out Matrix3<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        CreatePremultipliedRotation2D(rotation.Radians, out var rowA, out var rowB);
        matrix = new(
            a1: rowA.X, a2: rowA.Y, a3: T.Zero,
            b1: rowB.X, b2: rowB.Y, b3: T.Zero,
            c1: translation.X, c2: translation.Y, c3: T.One
            );
        }

    /// <summary><inheritdoc cref="CreateRotationTranslation{T}(Quaternion{T}, Vector3{T}, out Matrix4{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPremultiplicationConvention{T}"/>
    /// </remarks>
    public static void CreatePremultipliedRotationTranslation<T>(Quaternion<T> rotation, Vector3<T> translation, out Matrix4x3<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        CreatePremultipliedRotation(rotation, out Matrix4<T> temp);
        matrix = new(
            a1: temp.A1, a2: temp.A2, a3: temp.A3,
            b1: temp.B1, b2: temp.B2, b3: temp.B3,
            c1: temp.C1, c2: temp.C2, c3: temp.C3,
            d1: translation.X, d2: translation.Y, d3: translation.Z
            );
        }

    /// <summary><inheritdoc cref="CreateRotationTranslation{T}(Quaternion{T}, Vector3{T}, out Matrix4{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPremultiplicationConvention{T}"/>
    /// </remarks>
    public static void CreatePremultipliedRotationTranslation<T>(Quaternion<T> rotation, Vector3<T> translation, out Matrix4<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        CreatePremultipliedRotation(rotation, out matrix);
        matrix.D1 = translation.X;
        matrix.D2 = translation.Y;
        matrix.D3 = translation.Z;
        matrix.D4 = T.One;
        }

    /// <summary><inheritdoc cref="IMatrix{T}.ITranslationMatrix{Matrix, Vector, Direction, Rotation}.CreateRotationTranslation(Rotation, Vector)"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.MultiplicationConventionInstructions{T}"/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CreateRotationTranslation<T>(Angle<T> rotation, Vector2<T> translation, out Matrix3<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
#if MATRIX_PREMULTIPLIED_CONVENTION
        CreatePremultipliedRotationTranslation(rotation, translation, out matrix);
#elif MATRIX_POSTMULTIPLIED_CONVENTION
        CreatePostmultipliedRotationTranslation(rotation, translation, out matrix);
#else
        throw new ApplicationException(Matrix.Docs.BaseMessage);
#endif
        }

    /// <summary><inheritdoc cref="IMatrix{T}.ITranslationMatrix{Matrix, Vector, Direction, Rotation}.CreateRotationTranslation(Rotation, Vector)"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.MultiplicationConventionInstructions{T}"/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CreateRotationTranslation<T>(Quaternion<T> rotation, Vector3<T> translation, out Matrix4<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
#if MATRIX_PREMULTIPLIED_CONVENTION
        CreatePremultipliedRotationTranslation(rotation, translation, out matrix);
#elif MATRIX_POSTMULTIPLIED_CONVENTION
        CreatePostmultipliedRotationTranslation(rotation, translation, out matrix);
#else
        throw new ApplicationException(Matrix.Docs.BaseMessage);
#endif
        }

    #endregion Factories - Rotation 🡺 Translation

    #region Factories - Scale 🡺 Translation

    /// <summary><inheritdoc cref="CreateScaleTranslation{T}(Vector2{T}, Vector2{T}, out Matrix3{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPostmultiplicationConvention{T}"/>
    /// </remarks>
    public static void CreatePostmultipliedScaleTranslation<T>(Vector2<T> scale, Vector2<T> translation, out Matrix2x3<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        matrix = new(
            a1: scale.X, a2: +T.Zero, a3: translation.X,
            b1: +T.Zero, b2: scale.Y, b3: translation.Y
            );
        }

    /// <summary><inheritdoc cref="CreateScaleTranslation{T}(Vector2{T}, Vector2{T}, out Matrix3{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPostmultiplicationConvention{T}"/>
    /// </remarks>
    public static void CreatePostmultipliedScaleTranslation<T>(Vector2<T> scale, Vector2<T> translation, out Matrix3<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        matrix = new(
            a1: scale.X, a2: +T.Zero, a3: translation.X,
            b1: +T.Zero, b2: scale.Y, b3: translation.Y,
            c1: +T.Zero, c2: +T.Zero, c3: T.One
            );
        }

    /// <summary><inheritdoc cref="CreateScaleTranslation{T}(Vector3{T}, Vector3{T}, out Matrix4{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPostmultiplicationConvention{T}"/>
    /// </remarks>
    public static void CreatePostmultipliedScaleTranslation<T>(Vector3<T> scale, Vector3<T> translation, out Matrix4<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        matrix = new(
            a1: scale.X, a2: T.Zero, a3: T.Zero, a4: translation.X,
            b1: T.Zero, b2: scale.Y, b3: T.Zero, b4: translation.Y,
            c1: T.Zero, c2: T.Zero, c3: scale.Z, c4: translation.Z,
            d1: T.Zero, d2: T.Zero, d3: T.Zero, d4: T.One
            );
        }

    /// <summary><inheritdoc cref="CreateScaleTranslation{T}(Vector3{T}, Vector3{T}, out Matrix4{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPostmultiplicationConvention{T}"/>
    /// </remarks>
    public static void CreatePostmultipliedScaleTranslation<T>(Vector3<T> scale, Vector3<T> translation, out Matrix3x4<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        matrix = new(
            a1: scale.X, a2: T.Zero, a3: T.Zero, a4: translation.X,
            b1: T.Zero, b2: scale.Y, b3: T.Zero, b4: translation.Y,
            c1: T.Zero, c2: T.Zero, c3: scale.Z, c4: translation.Z
            );
        }

    /// <summary><inheritdoc cref="CreateScaleTranslation{T}(Vector2{T}, Vector2{T}, out Matrix3{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPremultiplicationConvention{T}"/>
    /// </remarks>
    public static void CreatePremultipliedScaleTranslation<T>(Vector2<T> scale, Vector2<T> translation, out Matrix3x2<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        matrix = new(
            a1: scale.X, a2: +T.Zero,
            b1: +T.Zero, b2: scale.Y,
            c1: translation.X, c2: translation.Y
            );
        }

    /// <summary><inheritdoc cref="CreateScaleTranslation{T}(Vector2{T}, Vector2{T}, out Matrix3{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPremultiplicationConvention{T}"/>
    /// </remarks>
    public static void CreatePremultipliedScaleTranslation<T>(Vector2<T> scale, Vector2<T> translation, out Matrix3<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        matrix = new(
            a1: scale.X, a2: +T.Zero, a3: T.Zero,
            b1: +T.Zero, b2: scale.Y, b3: T.Zero,
            c1: translation.X, c2: translation.Y, c3: T.One
            );
        }

    /// <summary><inheritdoc cref="CreateScaleTranslation{T}(Vector3{T}, Vector3{T}, out Matrix4{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPremultiplicationConvention{T}"/>
    /// </remarks>
    public static void CreatePremultipliedScaleTranslation<T>(Vector3<T> scale, Vector3<T> translation, out Matrix4<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        matrix = new(
            a1: scale.X, a2: T.Zero, a3: T.Zero, a4: T.Zero,
            b1: T.Zero, b2: scale.Y, b3: T.Zero, b4: T.Zero,
            c1: T.Zero, c2: T.Zero, c3: scale.Z, c4: T.Zero,
            d1: translation.X, d2: translation.Y, d3: translation.Z, d4: T.One
            );
        }

    /// <summary><inheritdoc cref="CreateScaleTranslation{T}(Vector3{T}, Vector3{T}, out Matrix4{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPremultiplicationConvention{T}"/>
    /// </remarks>
    public static void CreatePremultipliedScaleTranslation<T>(Vector3<T> scale, Vector3<T> translation, out Matrix4x3<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        matrix = new(
            a1: scale.X, a2: +T.Zero, a3: +T.Zero,
            b1: +T.Zero, b2: scale.Y, b3: +T.Zero,
            c1: +T.Zero, c2: +T.Zero, c3: scale.Z,
            d1: translation.X, d2: translation.Y, d3: translation.Z
            );
        }

    /// <summary><inheritdoc cref="IMatrix{T}.ITranslationMatrix{Matrix, Vector, Direction, Rotation}.CreateScaleTranslation(Vector, Vector)"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.MultiplicationConventionInstructions{T}"/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CreateScaleTranslation<T>(Vector2<T> scale, Vector2<T> translation, out Matrix3<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
#if MATRIX_PREMULTIPLIED_CONVENTION
        CreatePremultipliedScaleTranslation(scale, translation, out matrix);
#elif MATRIX_POSTMULTIPLIED_CONVENTION
        CreatePostmultipliedScaleTranslation(scale, translation, out matrix);
#else
        throw new ApplicationException(Matrix.Docs.BaseMessage);
#endif
        }

    /// <summary><inheritdoc cref="IMatrix{T}.ITranslationMatrix{Matrix, Vector, Direction, Rotation}.CreateScaleTranslation(Vector, Vector)"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.MultiplicationConventionInstructions{T}"/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CreateScaleTranslation<T>(Vector3<T> scale, Vector3<T> translation, out Matrix4<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
#if MATRIX_PREMULTIPLIED_CONVENTION
        CreatePremultipliedScaleTranslation(scale, translation, out matrix);
#elif MATRIX_POSTMULTIPLIED_CONVENTION
        CreatePostmultipliedScaleTranslation(scale, translation, out matrix);
#else
        throw new ApplicationException(Matrix.Docs.BaseMessage);
#endif
        }

    #endregion Factories - Scale 🡺 Translation

    #region Factories - Rotation 🡺 Scale 🡺 Translation

    /// <summary><inheritdoc cref="CreateRotationScaleTranslation{T}(Quaternion{T}, Vector3{T}, Vector3{T}, out Matrix4{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPostmultiplicationConvention{T}"/>
    /// </remarks>
    public static void CreatePostmultipliedRotationScaleTranslation<T>(Quaternion<T> rotation, Vector3<T> scale, Vector3<T> translation, out Matrix3x4<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        CreatePostmultipliedRotation(rotation, out Matrix4<T> r);
        matrix = new(
            a1: r.A1 * scale.X, a2: r.A2 * scale.Y, a3: r.A3 * scale.Z, a4: translation.X,
            b1: r.B1 * scale.X, b2: r.B2 * scale.Y, b3: r.B3 * scale.Z, b4: translation.Y,
            c1: r.C1 * scale.X, c2: r.C2 * scale.Y, c3: r.C3 * scale.Z, c4: translation.Z
            );
        }

    /// <summary><inheritdoc cref="CreateRotationScaleTranslation{T}(Quaternion{T}, Vector3{T}, Vector3{T}, out Matrix4{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPostmultiplicationConvention{T}"/>
    /// </remarks>
    public static void CreatePostmultipliedRotationScaleTranslation<T>(Quaternion<T> rotation, Vector3<T> scale, Vector3<T> translation, out Matrix4<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        CreatePostmultipliedRotation(rotation, out matrix);
        matrix.A1 *= scale.X;
        matrix.A2 *= scale.Y;
        matrix.A3 *= scale.Z;
        matrix.B1 *= scale.X;
        matrix.B2 *= scale.Y;
        matrix.B3 *= scale.Z;
        matrix.C1 *= scale.X;
        matrix.C2 *= scale.Y;
        matrix.C3 *= scale.Z;
        matrix.A4 = translation.X;
        matrix.B4 = translation.Y;
        matrix.C4 = translation.Z;
        matrix.D4 = T.One;
        }

    /// <summary><inheritdoc cref="CreateRotationScaleTranslation{T}(Angle{T}, Vector2{T}, Vector2{T}, out Matrix3{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPostmultiplicationConvention{T}"/>
    /// </remarks>
    public static void CreatePostmultipliedRotationScaleTranslation<T>(Angle<T> rotation, Vector2<T> scale, Vector2<T> translation, out Matrix2x3<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        CreatePostmultipliedRotation2D(rotation.Radians, out var rowA, out var rowB);
        matrix = new(
            a1: rowA.X * scale.X, a2: rowA.Y * scale.Y, a3: translation.X,
            b1: rowB.X * scale.X, b2: rowB.Y * scale.Y, b3: translation.Y
            );
        }

    /// <summary><inheritdoc cref="CreateRotationScaleTranslation{T}(Angle{T}, Vector2{T}, Vector2{T}, out Matrix3{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPostmultiplicationConvention{T}"/>
    /// </remarks>
    public static void CreatePostmultipliedRotationScaleTranslation<T>(Angle<T> rotation, Vector2<T> scale, Vector2<T> translation, out Matrix3<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        CreatePostmultipliedRotation2D(rotation.Radians, out var rowA, out var rowB);
        matrix = new(
            a1: rowA.X * scale.X, a2: rowA.Y * scale.Y, a3: translation.X,
            b1: rowB.X * scale.X, b2: rowB.Y * scale.Y, b3: translation.Y,
            c1: T.Zero, c2: T.Zero, c3: T.One
            );
        }

    /// <summary><inheritdoc cref="CreateRotationScaleTranslation{T}(Quaternion{T}, Vector3{T}, Vector3{T}, out Matrix4{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPremultiplicationConvention{T}"/>
    /// </remarks>
    public static void CreatePremultipliedRotationScaleTranslation<T>(Quaternion<T> rotation, Vector3<T> scale, Vector3<T> translation, out Matrix4x3<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        CreatePremultipliedRotation(rotation, out Matrix4<T> temp);
        matrix = new(
            a1: temp.A1 * scale.X, a2: temp.A2 * scale.X, a3: temp.A3 * scale.X,
            b1: temp.B1 * scale.Y, b2: temp.B2 * scale.Y, b3: temp.B3 * scale.Y,
            c1: temp.C1 * scale.Z, c2: temp.C2 * scale.Z, c3: temp.C3 * scale.Z,
            d1: translation.X, d2: translation.Y, d3: translation.Z
            );
        }

    /// <summary><inheritdoc cref="CreateRotationScaleTranslation{T}(Quaternion{T}, Vector3{T}, Vector3{T}, out Matrix4{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPremultiplicationConvention{T}"/>
    /// </remarks>
    public static void CreatePremultipliedRotationScaleTranslation<T>(Quaternion<T> rotation, Vector3<T> scale, Vector3<T> translation, out Matrix4<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        CreatePremultipliedRotation(rotation, out matrix);
        matrix.A1 *= scale.X;
        matrix.A2 *= scale.X;
        matrix.A3 *= scale.X;
        matrix.B1 *= scale.Y;
        matrix.B2 *= scale.Y;
        matrix.B3 *= scale.Y;
        matrix.C1 *= scale.Z;
        matrix.C2 *= scale.Z;
        matrix.C3 *= scale.Z;
        matrix.D1 = translation.X;
        matrix.D2 = translation.Y;
        matrix.D3 = translation.Z;
        matrix.D4 = T.One;
        }

    /// <summary><inheritdoc cref="CreateRotationScaleTranslation{T}(Angle{T}, Vector2{T}, Vector2{T}, out Matrix3{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPremultiplicationConvention{T}"/>
    /// </remarks>
    public static void CreatePremultipliedRotationScaleTranslation<T>(Angle<T> rotation, Vector2<T> scale, Vector2<T> translation, out Matrix3x2<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        CreatePremultipliedRotation2D(rotation.Radians, out var rowA, out var rowB);
        matrix = new(
            a1: rowA.X * scale.X, a2: rowA.Y * scale.X,
            b1: rowB.X * scale.Y, b2: rowB.Y * scale.Y,
            c1: translation.X, c2: translation.Y
            );
        }

    /// <summary><inheritdoc cref="CreateRotationScaleTranslation{T}(Angle{T}, Vector2{T}, Vector2{T}, out Matrix3{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPremultiplicationConvention{T}"/>
    /// </remarks>
    public static void CreatePremultipliedRotationScaleTranslation<T>(Angle<T> rotation, Vector2<T> scale, Vector2<T> translation, out Matrix3<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        CreatePremultipliedRotation2D(rotation.Radians, out var rowA, out var rowB);
        matrix = new(
            a1: rowA.X * scale.X, a2: rowA.Y * scale.X, a3: T.Zero,
            b1: rowB.X * scale.Y, b2: rowB.Y * scale.Y, b3: T.Zero,
            c1: translation.X, c2: translation.Y, c3: T.One
            );
        }

    /// <summary><inheritdoc cref="IMatrix{T}.ITranslationMatrix{Matrix, Vector, Direction, Rotation}.CreateRotationScaleTranslation(Rotation, Vector, Vector)"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.MultiplicationConventionInstructions{T}"/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CreateRotationScaleTranslation<T>(Quaternion<T> rotation, Vector3<T> scale, Vector3<T> translation, out Matrix4<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
#if MATRIX_PREMULTIPLIED_CONVENTION
        CreatePremultipliedRotationScaleTranslation(rotation, scale, translation, out matrix);
#elif MATRIX_POSTMULTIPLIED_CONVENTION
        CreatePostmultipliedRotationScaleTranslation(rotation, scale, translation, out matrix);
#else
        throw new ApplicationException(Matrix.Docs.BaseMessage);
#endif
        }

    /// <summary><inheritdoc cref="IMatrix{T}.ITranslationMatrix{Matrix, Vector, Direction, Rotation}.CreateRotationScaleTranslation(Rotation, Vector, Vector)"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.MultiplicationConventionInstructions{T}"/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CreateRotationScaleTranslation<T>(Angle<T> rotation, Vector2<T> scale, Vector2<T> translation, out Matrix3<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
#if MATRIX_PREMULTIPLIED_CONVENTION
        CreatePremultipliedRotationScaleTranslation(rotation, scale, translation, out matrix);
#elif MATRIX_POSTMULTIPLIED_CONVENTION
        CreatePostmultipliedRotationScaleTranslation(rotation, scale, translation, out matrix);
#else
        throw new ApplicationException(Matrix.Docs.BaseMessage);
#endif
        }

    #endregion Factories - Rotation 🡺 Scale 🡺 Translation

    #region Factories - Shadow

    /// <summary><inheritdoc cref="CreateShadow{T}(Vector3{T}, Plane3{T}, out Matrix4{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPostmultiplicationConvention{T}"/>
    /// </remarks>
    [BunnyAttributes.SIMDCandidate]
    public static void CreatePostmultipliedShadow<T>(Vector3<T> lightDirection, Plane3<T> plane, out Matrix4<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        var norm = plane.Normalize();
        var n = norm.Normal;
        var d = norm.Distance;
        var dot = n.X * lightDirection.X + n.Y * lightDirection.Y + n.Z * lightDirection.Z; // inline
        var x = -n.X;
        var y = -n.Y;
        var z = -n.Z;
        matrix = new(
            a1: x * lightDirection.X + dot,
            a2: y * lightDirection.X,
            a3: z * lightDirection.X,
            a4: d * lightDirection.X,
            b1: x * lightDirection.Y,
            b2: y * lightDirection.Y + dot,
            b3: z * lightDirection.Y,
            b4: d * lightDirection.Y,
            c1: x * lightDirection.Z,
            c2: y * lightDirection.Z,
            c3: z * lightDirection.Z + dot,
            c4: d * lightDirection.Z,
            d1: T.Zero,
            d2: T.Zero,
            d3: T.Zero,
            d4: dot
            );
        }

    /// <summary><inheritdoc cref="CreateShadow{T}(Vector3{T}, Vector3{T}, Plane3{T}, out Matrix4{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPostmultiplicationConvention{T}"/>
    /// </remarks>
    public static void CreatePostmultipliedShadow<T>(Vector3<T> lightSource, Vector3<T> lightTarget, Plane3<T> plane, out Matrix4<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        var lightDirection = lightSource - lightTarget;
        CreatePostmultipliedShadow(lightDirection, plane, out matrix);
        }

    /// <summary><inheritdoc cref="CreateShadow{T}(Vector3{T}, Plane3{T}, out Matrix4{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPremultiplicationConvention{T}"/>
    /// </remarks>
    [BunnyAttributes.SIMDCandidate]
    public static void CreatePremultipliedShadow<T>(Vector3<T> lightDirection, Plane3<T> plane, out Matrix4<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        var norm = plane.Normalize();
        var n = norm.Normal;
        var d = norm.Distance;
        var dot = n.X * lightDirection.X + n.Y * lightDirection.Y + n.Z * lightDirection.Z; // inline
        var x = -n.X;
        var y = -n.Y;
        var z = -n.Z;
        matrix = new(
            a1: x * lightDirection.X + dot,
            a2: x * lightDirection.Y,
            a3: x * lightDirection.Z,
            a4: T.Zero,
            b1: y * lightDirection.X,
            b2: y * lightDirection.Y + dot,
            b3: y * lightDirection.Z,
            b4: T.Zero,
            c1: z * lightDirection.X,
            c2: z * lightDirection.Y,
            c3: z * lightDirection.Z + dot,
            c4: T.Zero,
            d1: d * lightDirection.X,
            d2: d * lightDirection.Y,
            d3: d * lightDirection.Z,
            d4: dot
            );
        }

    /// <summary><inheritdoc cref="CreateShadow{T}(Vector3{T}, Vector3{T}, Plane3{T}, out Matrix4{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPremultiplicationConvention{T}"/>
    /// </remarks>
    public static void CreatePremultipliedShadow<T>(Vector3<T> lightSource, Vector3<T> lightTarget, Plane3<T> plane, out Matrix4<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        var lightDirection = lightSource - lightTarget;
        CreatePremultipliedShadow(lightDirection, plane, out matrix);
        }

    /// <summary><inheritdoc cref="IMatrix{T}.I3DProjectionMatrix{Matrix}.CreateShadow(Vector3{T}, Plane3{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.MultiplicationConventionInstructions{T}"/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CreateShadow<T>(Vector3<T> lightDirection, Plane3<T> plane, out Matrix4<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
#if MATRIX_PREMULTIPLIED_CONVENTION
        CreatePremultipliedShadow(lightDirection, plane, out matrix);
#elif MATRIX_POSTMULTIPLIED_CONVENTION
        CreatePostmultipliedShadow(lightDirection, plane, out matrix);
#else
        throw new ApplicationException(Matrix.Docs.BaseMessage);
#endif
        }

    /// <summary><inheritdoc cref="IMatrix{T}.I3DProjectionMatrix{Matrix}.CreateShadow(Vector3{T}, Vector3{T}, Plane3{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.MultiplicationConventionInstructions{T}"/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CreateShadow<T>(Vector3<T> lightSource, Vector3<T> lightTarget, Plane3<T> plane, out Matrix4<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
#if MATRIX_PREMULTIPLIED_CONVENTION
        CreatePremultipliedShadow(lightSource, lightTarget, plane, out matrix);
#elif MATRIX_POSTMULTIPLIED_CONVENTION
        CreatePostmultipliedShadow(lightSource, lightTarget, plane, out matrix);
#else
        throw new ApplicationException(Matrix.Docs.BaseMessage);
#endif
        }

    #endregion Factories - Shadow

    #region Factories - Single-Axis Rotation - 2D

    /// <summary> Returns the signs of the 2D rotation matrix cells of interest, depending on handedness.
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.MultiplicationConventionInstructions{T}"/>
    /// </remarks>
    public static (T A2, T B1) CreateRotationSigns<T>()
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
#if MATRIX_PREMULTIPLIED_CONVENTION
        return (T.One, T.NegativeOne);
#elif MATRIX_POSTMULTIPLIED_CONVENTION
        return (T.NegativeOne, T.One);
#else
        throw new ApplicationException(Matrix.Docs.BaseMessage);
#endif
        }

    /// <summary> Returns the signs of a 2D rotation matrix.
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.MultiplicationConventionInstructions{T}"/>
    /// <para/><b>Pre-multiplied convention:</b>
    /// <para/><c>[[+1, +1]
    /// <para/>•[-1, +1]]
    /// </c>
    /// <para/><b>Post-multiplied convention:</b>
    /// <para/><c>[[+1, -1]
    /// <para/>•[+1, +1]]
    /// </c>
    /// </remarks>
    public static void CreateRotationSigns<T>(out Vector2<T> rowA, out Vector2<T> rowB)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
#if MATRIX_PREMULTIPLIED_CONVENTION
        rowA = Vector2<T>.One;
        rowB = new Vector2<T>(T.NegativeOne, T.One);
#elif MATRIX_POSTMULTIPLIED_CONVENTION
        rowA = new Vector2<T>(T.One, T.NegativeOne);
        rowB = Vector2<T>.One;
#else
        throw new ApplicationException(Matrix.Docs.BaseMessage);
#endif
        }

    /// <summary><inheritdoc cref="CreateRotationSigns{T}(out Vector2{T}, out Vector2{T})"/>
    /// <para/> The second row's contents are swapped, which is a useful precalculation for some practical vectorized operations.
    /// </summary>
    /// <inheritdoc
    /// cref="CreateRotationSigns{T}(out Vector2{T}, out Vector2{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CreateRotationSignsWithSwappedSecondRow(out Vector128<float> signs) {
#if MATRIX_PREMULTIPLIED_CONVENTION
        signs = Vector128.Create(+1f, +1f, +1f, -1f);
#elif MATRIX_POSTMULTIPLIED_CONVENTION
        signs = Vector128.Create(+1f, -1f, +1f, +1f);
#else
        throw new ApplicationException(Matrix.Docs.BaseMessage);
#endif
        }

    /// <inheritdoc
    /// cref="CreateRotationSignsWithSwappedSecondRow(out Vector128{float})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CreateRotationSignsWithSwappedSecondRow(out Vector256<double> signs) {
#if MATRIX_PREMULTIPLIED_CONVENTION
        signs = Vector256.Create(+1d, +1d, +1d, -1d);
#elif MATRIX_POSTMULTIPLIED_CONVENTION
        signs = Vector256.Create(+1d, -1d, +1d, +1d);
#else
        throw new ApplicationException(Matrix.Docs.BaseMessage);
#endif
        }

    /// <inheritdoc
    /// cref="CreateRotationSignsWithSwappedSecondRow(out Vector128{float})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CreateRotationSignsWithSwappedSecondRow(out Vector128<double> rowA, out Vector128<double> rowB) {
#if MATRIX_PREMULTIPLIED_CONVENTION
        rowA = Vector128.Create(+1d, +1d);
        rowB = Vector128.Create(+1d, -1d);
#elif MATRIX_POSTMULTIPLIED_CONVENTION
        rowA = Vector128.Create(+1d, -1d);
        rowB = Vector128.Create(+1d, +1d);
#else
        throw new ApplicationException(Matrix.Docs.BaseMessage);
#endif
        }

    /// <inheritdoc
    /// cref="CreateRotationSignsWithSwappedSecondRow(out Vector128{float})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CreateRotationSignsWithSwappedSecondRow(out Vector64<float> rowA, out Vector64<float> rowB) {
#if MATRIX_PREMULTIPLIED_CONVENTION
        rowA = Vector64.Create(+1f, +1f);
        rowB = Vector64.Create(+1f, -1f);
#elif MATRIX_POSTMULTIPLIED_CONVENTION
        rowA = Vector64.Create(+1f, -1f);
        rowB = Vector64.Create(+1f, +1f);
#else
        throw new ApplicationException(Matrix.Docs.BaseMessage);
#endif
        }

    /// <inheritdoc
    /// cref="CreateRotationSigns{T}(out Vector2{T}, out Vector2{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CreateRotationSigns(out Vector128<float> signs) {
#if MATRIX_PREMULTIPLIED_CONVENTION
        signs = Vector128.Create(+1f, +1f, -1f, +1f);
#elif MATRIX_POSTMULTIPLIED_CONVENTION
        signs = Vector128.Create(+1f, -1f, +1f, +1f);
#else
        throw new ApplicationException(Matrix.Docs.BaseMessage);
#endif
        }

    /// <inheritdoc
    /// cref="CreateRotationSigns{T}(out Vector2{T}, out Vector2{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CreateRotationSigns(out Vector256<double> signs) {
#if MATRIX_PREMULTIPLIED_CONVENTION
        signs = Vector256.Create(+1d, +1d, -1d, +1d);
#elif MATRIX_POSTMULTIPLIED_CONVENTION
        signs = Vector256.Create(+1d, -1d, +1d, +1d);
#else
        throw new ApplicationException(Matrix.Docs.BaseMessage);
#endif
        }

    /// <inheritdoc
    /// cref="CreateRotationSigns{T}(out Vector2{T}, out Vector2{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CreateRotationSigns(out Vector128<double> rowA, out Vector128<double> rowB) {
#if MATRIX_PREMULTIPLIED_CONVENTION
        rowA = Vector128.Create(+1d, +1d);
        rowB = Vector128.Create(-1d, +1d);
#elif MATRIX_POSTMULTIPLIED_CONVENTION
        rowA = Vector128.Create(+1d, -1d);
        rowB = Vector128.Create(+1d, +1d);
#else
        throw new ApplicationException(Matrix.Docs.BaseMessage);
#endif
        }

    /// <inheritdoc
    /// cref="CreateRotationSigns{T}(out Vector2{T}, out Vector2{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CreateRotationSigns(out Vector64<float> rowA, out Vector64<float> rowB) {
#if MATRIX_PREMULTIPLIED_CONVENTION
        rowA = Vector64.Create(+1f, +1f);
        rowB = Vector64.Create(-1f, +1f);
#elif MATRIX_POSTMULTIPLIED_CONVENTION
        rowA = Vector64.Create(+1f, -1f);
        rowB = Vector64.Create(+1f, +1f);
#else
        throw new ApplicationException(Matrix.Docs.BaseMessage);
#endif
        }

    /// <inheritdoc
    /// cref="CreateRotationSigns{T}(out Vector2{T}, out Vector2{T})"/>
    public static void CreateRotationSigns<T>(out Matrix2<T> matrix)
    where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
#if MATRIX_PREMULTIPLIED_CONVENTION
        matrix = new(T.One, T.One, T.NegativeOne, T.One);
#elif MATRIX_POSTMULTIPLIED_CONVENTION
        matrix = new(T.One, T.NegativeOne, T.One, T.One);
#else
        throw new ApplicationException(Matrix.Docs.BaseMessage);
#endif
        }

    /// <summary><inheritdoc cref="CreatePremultipliedRotation2D{T}(T, out Vector2{T}, out Vector2{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPostmultiplicationConvention{T}"/>
    /// </remarks>
    public static void CreatePostmultipliedRotation2D<T>(T radians, out Vector2<T> rowA, out Vector2<T> rowB)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        rowA = rowB = default;
        var (sin, cos) = T.SinCos(radians);
        rowA.X = +cos; rowA.Y = -sin;
        rowB.X = +sin; rowB.Y = +cos;
        }

    /// <summary> Outputs the first and second row of a 2D transformation matrix representing rotation by a given number of <paramref name="radians"/>.
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPremultiplicationConvention{T}"/>
    /// </remarks>
    public static void CreatePremultipliedRotation2D<T>(T radians, out Vector2<T> rowA, out Vector2<T> rowB)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        rowA = rowB = default;
        var (sin, cos) = T.SinCos(radians);
        rowA.X = +cos; rowA.Y = +sin;
        rowB.X = -sin; rowB.Y = +cos;
        }

    /// <summary><inheritdoc cref="IMatrix{Numeric}.IMatrixBase{Matrix, Vector, Direction, Rotation}.CreateRotation(Rotation)"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.MultiplicationConventionInstructions{T}"/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CreateRotation<T>(Angle<T> rotation, out Matrix2<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        matrix = default;
        Vector2<T> rowA, rowB;
#if MATRIX_PREMULTIPLIED_CONVENTION
        CreatePremultipliedRotation2D(rotation.Radians, out rowA, out rowB);
#elif MATRIX_POSTMULTIPLIED_CONVENTION
        CreatePostmultipliedRotation2D(rotation.Radians, out rowA, out rowB);
#else
        throw new ApplicationException(Matrix.Docs.BaseMessage);
#endif
        matrix.A1 = rowA.X;
        matrix.A2 = rowA.Y;
        matrix.B1 = rowB.X;
        matrix.B2 = rowB.Y;
        }

    #endregion Factories - Single-Axis Rotation - 2D

    #region Factories - Single-Axis Rotation - X

    /// <summary><inheritdoc cref="CreatePremultipliedRotationX{T}(T, out Vector4{T}, out Vector4{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPostmultiplicationConvention{T}"/>
    /// </remarks>
    public static void CreatePostmultipliedRotationX<T>(T radians, out Vector4<T> rowB, out Vector4<T> rowC)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        rowB = rowC = default;
        var (sin, cos) = T.SinCos(radians);
        rowB.Y = +cos; rowB.Z = -sin;
        rowC.Y = +sin; rowC.Z = +cos;
        }

    /// <summary> Outputs the second and third row of a 3D transformation matrix representing rotation around the X axis by a given number of <paramref name="radians"/>.
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPremultiplicationConvention{T}"/>
    /// </remarks>
    public static void CreatePremultipliedRotationX<T>(T radians, out Vector4<T> rowB, out Vector4<T> rowC)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        rowB = rowC = default;
        var (sin, cos) = T.SinCos(radians);
        rowB.Y = +cos; rowB.Z = +sin;
        rowC.Y = -sin; rowC.Z = +cos;
        }

    /// <summary><inheritdoc cref="IMatrix{T}.I3DMatrix{T}.CreateRotationX(Angle{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.MultiplicationConventionInstructions{T}"/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CreateRotationX<T>(T radians, out Vector4<T> rowB, out Vector4<T> rowC)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
#if MATRIX_PREMULTIPLIED_CONVENTION
        CreatePremultipliedRotationX(radians, out rowB, out rowC);
#elif MATRIX_POSTMULTIPLIED_CONVENTION
        CreatePostmultipliedRotationX(radians, out rowB, out rowC);
#else
        throw new ApplicationException(Matrix.Docs.BaseMessage);
#endif
        }

    /// <summary><inheritdoc cref="IMatrix{T}.I3DMatrix{T}.CreateRotationX(Angle{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.MultiplicationConventionInstructions{T}"/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CreateRotationX<T>(T radians, out Matrix4<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        matrix = default;

        Vector4<T> rowB, rowC;
#if MATRIX_PREMULTIPLIED_CONVENTION
        CreatePremultipliedRotationX(radians, out rowB, out rowC);
#elif MATRIX_POSTMULTIPLIED_CONVENTION
        CreatePostmultipliedRotationX(radians, out rowB, out rowC);
#else
        throw new ApplicationException(Matrix.Docs.BaseMessage);
#endif
        matrix.B2 = rowB.Y;
        matrix.B3 = rowB.Z;
        matrix.C2 = rowC.Y;
        matrix.C3 = rowC.Z;
        matrix.A1 = T.One;
        matrix.D4 = T.One;
        }

    #endregion Factories - Single-Axis Rotation - X

    #region Factories - Single-Axis Rotation - Y

    /// <summary><inheritdoc cref="CreatePremultipliedRotationY{T}(T, out Vector4{T}, out Vector4{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPostmultiplicationConvention{T}"/>
    /// </remarks>
    public static void CreatePostmultipliedRotationY<T>(T radians, out Vector4<T> rowA, out Vector4<T> rowC)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        rowA = rowC = default;
        var (sin, cos) = T.SinCos(radians);
        rowA.X = +cos; rowA.Z = +sin;
        rowC.X = -sin; rowC.Z = +cos;
        }

    /// <summary> Outputs the first and third row of a 3D transformation matrix representing rotation around the Y axis by a given number of <paramref name="radians"/>.
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPremultiplicationConvention{T}"/>
    /// </remarks>
    public static void CreatePremultipliedRotationY<T>(T radians, out Vector4<T> rowA, out Vector4<T> rowC)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        rowA = rowC = default;
        var (sin, cos) = T.SinCos(radians);
        rowA.X = +cos; rowA.Z = -sin;
        rowC.X = +sin; rowC.Z = +cos;
        }

    /// <summary><inheritdoc cref="IMatrix{T}.I3DMatrix{T}.CreateRotationY(Angle{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.MultiplicationConventionInstructions{T}"/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CreateRotationY<T>(T radians, out Vector4<T> rowA, out Vector4<T> rowC)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
#if MATRIX_PREMULTIPLIED_CONVENTION
        CreatePremultipliedRotationY(radians, out rowA, out rowC);
#elif MATRIX_POSTMULTIPLIED_CONVENTION
        CreatePostmultipliedRotationY(radians, out rowA, out rowC);
#else
        throw new ApplicationException(Matrix.Docs.BaseMessage);
#endif
        }


    /// <summary><inheritdoc cref="IMatrix{T}.I3DMatrix{T}.CreateRotationY(Angle{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.MultiplicationConventionInstructions{T}"/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CreateRotationY<T>(T radians, out Matrix4<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        matrix = default;
        Vector4<T> rowA, rowC;
#if MATRIX_PREMULTIPLIED_CONVENTION
        CreatePremultipliedRotationY(radians, out rowA, out rowC);
#elif MATRIX_POSTMULTIPLIED_CONVENTION
        CreatePostmultipliedRotationY(radians, out rowA, out rowC);
#else
        throw new ApplicationException(Matrix.Docs.BaseMessage);
#endif
        matrix.A1 = rowA.X;
        matrix.A3 = rowA.Z;
        matrix.C1 = rowC.X;
        matrix.C3 = rowC.Z;
        matrix.B2 = T.One;
        matrix.D4 = T.One;
        }

    #endregion Factories - Single-Axis Rotation - Y

    #region Factories - Single-Axis Rotation - Z

    /// <summary><inheritdoc cref="CreatePremultipliedRotationZ{T}(T, out Vector4{T}, out Vector4{T})"/>
    /// </summary>
    /// <remarks>This function can be substituted with the lighterweight <see cref="CreatePostmultipliedRotation2D{T}(T, out Vector2{T}, out Vector2{T})"/> when the empty 3rd and 4th elements aren't required.
    /// <para/> <inheritdoc cref="Docs.UsesPostmultiplicationConvention{T}"/>
    /// </remarks>
    public static void CreatePostmultipliedRotationZ<T>(T radians, out Vector4<T> rowA, out Vector4<T> rowB)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        rowA = rowB = default;
        var (sin, cos) = T.SinCos(radians);
        rowA.X = +cos; rowA.Y = -sin;
        rowB.X = +sin; rowB.Y = +cos;
        }

    /// <summary> Outputs the first and second row of a 3D transformation matrix representing rotation around the Z axis by a given number of <paramref name="radians"/>.
    /// </summary>
    /// <remarks> This function can be substituted with the lighterweight <see cref="CreatePremultipliedRotation2D{T}(T, out Vector2{T}, out Vector2{T})"/> when the empty 3rd and 4th elements aren't required.
    /// <para/> <inheritdoc cref="Docs.UsesPremultiplicationConvention{T}"/>
    /// </remarks>
    public static void CreatePremultipliedRotationZ<T>(T radians, out Vector4<T> rowA, out Vector4<T> rowB)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        rowA = rowB = default;
        var (sin, cos) = T.SinCos(radians);
        rowA.X = +cos; rowA.Y = +sin;
        rowB.X = -sin; rowB.Y = +cos;
        }


    /// <summary><inheritdoc cref="IMatrix{T}.I3DMatrix{T}.CreateRotationZ(Angle{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.MultiplicationConventionInstructions{T}"/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CreateRotationZ<T>(T radians, out Vector4<T> rowA, out Vector4<T> rowB)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
#if MATRIX_PREMULTIPLIED_CONVENTION
        CreatePremultipliedRotationZ(radians, out rowA, out rowB);
#elif MATRIX_POSTMULTIPLIED_CONVENTION
        CreatePostmultipliedRotationZ(radians, out rowA, out rowB);
#else
        throw new ApplicationException(Matrix.Docs.BaseMessage);
#endif
        }

    /// <summary><inheritdoc cref="IMatrix{T}.I3DMatrix{T}.CreateRotationZ(Angle{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.MultiplicationConventionInstructions{T}"/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CreateRotationZ<T>(T radians, out Matrix4<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        matrix = default;
        Vector2<T> rowA, rowB;
#if MATRIX_PREMULTIPLIED_CONVENTION
        CreatePremultipliedRotation2D(radians, out rowA, out rowB);
#elif MATRIX_POSTMULTIPLIED_CONVENTION
        CreatePostmultipliedRotation2D(radians, out rowA, out rowB);
#else
        throw new ApplicationException(Matrix.Docs.BaseMessage);
#endif
        matrix.A1 = rowA.X;
        matrix.A2 = rowA.Y;
        matrix.B1 = rowB.X;
        matrix.B2 = rowB.Y;
        matrix.C3 = T.One;
        matrix.D4 = T.One;
        }

    #endregion Factories - Single-Axis Rotation - Z

    #region Factories - Scale

    /// <summary><inheritdoc cref="CreateScale{T}(Vector2{T}, out Matrix3{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPostmultiplicationConvention{T}"/>
    /// </remarks>
    public static void CreatePostmultipliedScale<T>(Vector2<T> scale, out Matrix2x3<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        matrix = new(
            a1: scale.X, a2: +T.Zero, a3: T.Zero,
            b1: +T.Zero, b2: scale.Y, b3: T.Zero
            );
        }

    /// <summary><inheritdoc cref="CreateScale{T}(Vector3{T}, out Matrix4{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPostmultiplicationConvention{T}"/>
    /// </remarks>
    public static void CreatePostmultipliedScale<T>(Vector3<T> scale, out Matrix3x4<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        matrix = new(
            a1: scale.X, a2: +T.Zero, a3: +T.Zero, a4: T.Zero,
            b1: +T.Zero, b2: scale.Y, b3: +T.Zero, b4: T.Zero,
            c1: +T.Zero, c2: +T.Zero, c3: scale.Z, c4: T.Zero
            );
        }

    /// <summary><inheritdoc cref="CreateScale{T}(Vector2{T}, out Matrix3{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPremultiplicationConvention{T}"/>
    /// </remarks>
    public static void CreatePremultipliedScale<T>(Vector2<T> scale, out Matrix3x2<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        matrix = new(
            a1: scale.X, a2: +T.Zero,
            b1: +T.Zero, b2: scale.Y,
            c1: +T.Zero, c2: +T.Zero
            );
        }

    /// <summary><inheritdoc cref="CreateScale{T}(Vector3{T}, out Matrix4{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPremultiplicationConvention{T}"/>
    /// </remarks>
    public static void CreatePremultipliedScale<T>(Vector3<T> scale, out Matrix4x3<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        matrix = new(
            a1: scale.X, a2: +T.Zero, a3: +T.Zero,
            b1: +T.Zero, b2: scale.Y, b3: +T.Zero,
            c1: +T.Zero, c2: +T.Zero, c3: scale.Z,
            d1: +T.Zero, d2: +T.Zero, d3: +T.Zero
            );
        }

    /// <summary><inheritdoc cref="IMatrix{T}.IMatrixBase{Matrix, Vector, Direction, Rotation}.CreateScale(Vector)"/>
    /// </summary>
    public static void CreateScale<T>(Vector3<T> scale, out Matrix4<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        matrix = new(
            a1: scale.X, a2: T.Zero, a3: T.Zero, a4: T.Zero,
            b1: T.Zero, b2: scale.Y, b3: T.Zero, b4: T.Zero,
            c1: T.Zero, c2: T.Zero, c3: scale.Z, c4: T.Zero,
            d1: T.Zero, d2: T.Zero, d3: T.Zero, d4: T.One
            );
        }

    /// <summary><inheritdoc cref="IMatrix{T}.IMatrixBase{Matrix, Vector, Direction, Rotation}.CreateScale(Vector)"/>
    /// </summary>
    public static void CreateScale<T>(Vector2<T> scale, out Matrix2<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        matrix = new(
            a1: scale.X, a2: +T.Zero,
            b1: +T.Zero, b2: scale.Y
            );
        }

    /// <summary><inheritdoc cref="IMatrix{T}.IMatrixBase{Matrix, Vector, Direction, Rotation}.CreateScale(Vector)"/>
    /// </summary>
    public static void CreateScale<T>(Vector3<T> scale, out Matrix3<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        matrix = new(
            a1: scale.X, a2: +T.Zero, a3: +T.Zero,
            b1: +T.Zero, b2: scale.Y, b3: +T.Zero,
            c1: +T.Zero, c2: +T.Zero, c3: scale.Z
            );
        }

    /// <summary><inheritdoc cref="IMatrix{T}.IMatrixBase{Matrix, Vector, Direction, Rotation}.CreateScale(Vector)"/>
    /// </summary>
    public static void CreateScale<T>(Vector2<T> scale, out Matrix3<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        matrix = new(
            a1: scale.X, a2: +T.Zero, a3: T.Zero,
            b1: +T.Zero, b2: scale.Y, b3: T.Zero,
            c1: +T.Zero, c2: +T.Zero, c3: T.One
            );
        }

    #endregion Factories - Scale

    #region Factories - Translation

    /// <summary><inheritdoc cref="CreateTranslation{T}(Vector2{T}, out Matrix3{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPostmultiplicationConvention{T}"/>
    /// </remarks>
    public static void CreatePostmultipliedTranslation<T>(Vector2<T> translation, out Matrix2x3<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        matrix = new(
            a1: +T.One, a2: T.Zero, a3: translation.X,
            b1: T.Zero, b2: +T.One, b3: translation.Y
            );
        }

    /// <summary><inheritdoc cref="CreateTranslation{T}(Vector2{T}, out Matrix3{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPostmultiplicationConvention{T}"/>
    /// </remarks>
    public static void CreatePostmultipliedTranslation<T>(Vector2<T> translation, out Matrix3<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        matrix = new(
            a1: +T.One, a2: T.Zero, a3: translation.X,
            b1: T.Zero, b2: +T.One, b3: translation.Y,
            c1: T.Zero, c2: T.Zero, c3: T.One
            );
        }

    /// <summary><inheritdoc cref="CreateTranslation{T}(Vector3{T}, out Matrix4{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPostmultiplicationConvention{T}"/>
    /// </remarks>
    public static void CreatePostmultipliedTranslation<T>(Vector3<T> translation, out Matrix4<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        matrix = new(
            a1: +T.One, a2: T.Zero, a3: T.Zero, a4: translation.X,
            b1: T.Zero, b2: +T.One, b3: T.Zero, b4: translation.Y,
            c1: T.Zero, c2: T.Zero, c3: +T.One, c4: translation.Z,
            d1: T.Zero, d2: T.Zero, d3: T.Zero, d4: T.One
            );
        }

    /// <summary><inheritdoc cref="CreateTranslation{T}(Vector3{T}, out Matrix4{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPostmultiplicationConvention{T}"/>
    /// </remarks>
    public static void CreatePostmultipliedTranslation<T>(Vector3<T> translation, out Matrix3x4<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        matrix = new(
            a1: +T.One, a2: T.Zero, a3: T.Zero, a4: translation.X,
            b1: T.Zero, b2: +T.One, b3: T.Zero, b4: translation.Y,
            c1: T.Zero, c2: T.Zero, c3: +T.One, c4: translation.Z
            );
        }

    /// <summary><inheritdoc cref="CreateTranslation{T}(Vector2{T}, out Matrix3{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPremultiplicationConvention{T}"/>
    /// </remarks>
    public static void CreatePremultipliedTranslation<T>(Vector2<T> translation, out Matrix3x2<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        matrix = new(
            a1: +T.One, a2: T.Zero,
            b1: T.Zero, b2: +T.One,
            c1: translation.X, c2: translation.Y
            );
        }

    /// <summary><inheritdoc cref="CreateTranslation{T}(Vector2{T}, out Matrix3{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPremultiplicationConvention{T}"/>
    /// </remarks>
    public static void CreatePremultipliedTranslation<T>(Vector2<T> translation, out Matrix3<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        matrix = new(
            a1: +T.One, a2: T.Zero, a3: T.Zero,
            b1: T.Zero, b2: +T.One, b3: T.Zero,
            c1: translation.X, c2: translation.Y, c3: T.One
            );
        }

    /// <summary><inheritdoc cref="CreateTranslation{T}(Vector3{T}, out Matrix4{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPremultiplicationConvention{T}"/>
    /// </remarks>
    public static void CreatePremultipliedTranslation<T>(Vector3<T> translation, out Matrix4<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        matrix = new(
            a1: +T.One, a2: T.Zero, a3: T.Zero, a4: T.Zero,
            b1: T.Zero, b2: +T.One, b3: T.Zero, b4: T.Zero,
            c1: T.Zero, c2: T.Zero, c3: +T.One, c4: T.Zero,
            d1: translation.X, d2: translation.Y, d3: translation.Z, d4: T.One
            );
        }

    /// <summary><inheritdoc cref="CreateTranslation{T}(Vector3{T}, out Matrix4{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPremultiplicationConvention{T}"/>
    /// </remarks>
    public static void CreatePremultipliedTranslation<T>(Vector3<T> translation, out Matrix4x3<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        matrix = new(
            a1: +T.One, a2: T.Zero, a3: T.Zero,
            b1: T.Zero, b2: +T.One, b3: T.Zero,
            c1: T.Zero, c2: T.Zero, c3: +T.One,
            d1: translation.X, d2: translation.Y, d3: translation.Z
            );
        }

    /// <summary><inheritdoc cref="IMatrix{T}.ITranslationMatrix{Matrix, Vector, Direction, Rotation}.CreateTranslation(Vector)"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.MultiplicationConventionInstructions{T}"/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CreateTranslation<T>(Vector2<T> translation, out Matrix3<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
#if MATRIX_PREMULTIPLIED_CONVENTION
        CreatePremultipliedTranslation(translation, out matrix);
#elif MATRIX_POSTMULTIPLIED_CONVENTION
        CreatePostmultipliedTranslation(translation, out matrix);
#else
        throw new ApplicationException(Matrix.Docs.BaseMessage);
#endif
        }

    /// <summary><inheritdoc cref="IMatrix{T}.ITranslationMatrix{Matrix, Vector, Direction, Rotation}.CreateTranslation(Vector)"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.MultiplicationConventionInstructions{T}"/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CreateTranslation<T>(Vector3<T> translation, out Matrix4<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
#if MATRIX_PREMULTIPLIED_CONVENTION
        CreatePremultipliedTranslation(translation, out matrix);
#elif MATRIX_POSTMULTIPLIED_CONVENTION
        CreatePostmultipliedTranslation(translation, out matrix);
#else
        throw new ApplicationException(Matrix.Docs.BaseMessage);
#endif
        }

    #endregion Factories - Translation

    #region Matrix 🡺 Quaternion

    /// <summary><inheritdoc cref="CreateQuaternionComponentsFromMatrix{T}(in Matrix4{T}, out T, out T, out T, out T)"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPostmultiplicationConvention{T}"/>
    /// </remarks>
    [BunnyAttributes.SIMDCandidate]
    public static void CreateQuaternionComponentsFromPostmultipliedMatrix<T>(in Matrix4<T> matrix, out T X, out T Y, out T Z, out T W)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        var half = GenericNumbers<T>.OneHalf;
        var c3 = matrix.C3;
        T num0, num1, sqr4, inv4;
        if (c3 < T.Zero) {
            num0 = matrix.B2 - matrix.A1;
            num1 = T.One - c3;
            if (num0 <= T.Zero) {
                sqr4 = num1 - num0;
                inv4 = half / T.Sqrt(sqr4);
                X = sqr4 * inv4;
                Y = (matrix.A2 + matrix.B1) * inv4;
                Z = (matrix.A3 + matrix.C1) * inv4;
                W = (matrix.C2 - matrix.B3) * inv4; // post-multiplication; swap subtraction order for pre-multiplication
                }
            else {
                sqr4 = num1 + num0;
                inv4 = half / T.Sqrt(sqr4);
                X = (matrix.A2 + matrix.B1) * inv4;
                Y = sqr4 * inv4;
                Z = (matrix.B3 + matrix.C2) * inv4;
                W = (matrix.A3 - matrix.C1) * inv4; // post-multiplication; swap subtraction order for pre-multiplication
                }
            }
        else {
            num0 = matrix.B2 + matrix.A1;
            num1 = T.One + c3;
            if (num0 <= T.Zero) {
                sqr4 = num1 - num0;
                inv4 = half / T.Sqrt(sqr4);
                X = (matrix.A3 + matrix.C1) * inv4;
                Y = (matrix.B3 + matrix.C2) * inv4;
                Z = sqr4 * inv4;
                W = (matrix.B1 - matrix.A2) * inv4; // post-multiplication; swap subtraction order for pre-multiplication
                }
            else {
                sqr4 = num1 + num0;
                inv4 = half / T.Sqrt(sqr4);
                X = (matrix.C2 - matrix.B3) * inv4; // post-multiplication; swap subtraction order for pre-multiplication
                Y = (matrix.A3 - matrix.C1) * inv4; // post-multiplication; swap subtraction order for pre-multiplication
                Z = (matrix.B1 - matrix.A2) * inv4; // post-multiplication; swap subtraction order for pre-multiplication
                W = sqr4 * inv4;
                }
            }
        var magnitude = T.Sqrt(X * X + Y * Y + Z * Z + W * W);
        X /= magnitude;
        Y /= magnitude;
        Z /= magnitude;
        W /= magnitude;
        }

    /// <summary><inheritdoc cref="CreateQuaternionComponentsFromMatrix{T}(in Matrix4{T}, out T, out T, out T, out T)"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPremultiplicationConvention{T}"/>
    /// </remarks>
    [BunnyAttributes.SIMDCandidate]
    public static void CreateQuaternionComponentsFromPremultipliedMatrix<T>(in Matrix4<T> matrix, out T X, out T Y, out T Z, out T W)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        var half = GenericNumbers<T>.OneHalf;
        var c3 = matrix.C3;
        T num0, num1, sqr4, inv4;
        if (c3 < T.Zero) {
            num0 = matrix.B2 - matrix.A1;
            num1 = T.One - c3;
            if (num0 <= T.Zero) {
                sqr4 = num1 - num0;
                inv4 = half / T.Sqrt(sqr4);
                X = sqr4 * inv4;
                Y = (matrix.A2 + matrix.B1) * inv4;
                Z = (matrix.A3 + matrix.C1) * inv4;
                W = (matrix.B3 - matrix.C2) * inv4; // pre-multiplication; swap subtraction order for post-multiplication
                }
            else {
                sqr4 = num1 + num0;
                inv4 = half / T.Sqrt(sqr4);
                X = (matrix.A2 + matrix.B1) * inv4;
                Y = sqr4 * inv4;
                Z = (matrix.B3 + matrix.C2) * inv4;
                W = (matrix.C1 - matrix.A3) * inv4; // pre-multiplication; swap subtraction order for post-multiplication
                }
            }
        else {
            num0 = matrix.B2 + matrix.A1;
            num1 = T.One + c3;
            if (num0 <= T.Zero) {
                sqr4 = num1 - num0;
                inv4 = half / T.Sqrt(sqr4);
                X = (matrix.A3 + matrix.C1) * inv4;
                Y = (matrix.B3 + matrix.C2) * inv4;
                Z = sqr4 * inv4;
                W = (matrix.A2 - matrix.B1) * inv4; // pre-multiplication; swap subtraction order for post-multiplication
                }
            else {
                sqr4 = num1 + num0;
                inv4 = half / T.Sqrt(sqr4);
                X = (matrix.B3 - matrix.C2) * inv4; // pre-multiplication; swap subtraction order for post-multiplication
                Y = (matrix.C1 - matrix.A3) * inv4; // pre-multiplication; swap subtraction order for post-multiplication
                Z = (matrix.A2 - matrix.B1) * inv4; // pre-multiplication; swap subtraction order for post-multiplication
                W = sqr4 * inv4;
                }
            }
        var magnitude = T.Sqrt(X * X + Y * Y + Z * Z + W * W);
        X /= magnitude;
        Y /= magnitude;
        Z /= magnitude;
        W /= magnitude;
        }

    /// <summary> Calculates the <see cref="Quaternion{T}.X">X</see>, <see cref="Quaternion{T}.Y">Y</see>, <see cref="Quaternion{T}.Z">Z</see>, <see cref="Quaternion{T}.W">W</see> components of a quaternion from a given <paramref name="matrix"/>.
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.MultiplicationConventionInstructions{T}"/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CreateQuaternionComponentsFromMatrix<T>(in Matrix4<T> matrix, out T X, out T Y, out T Z, out T W)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
#if MATRIX_PREMULTIPLIED_CONVENTION
        CreateQuaternionComponentsFromPremultipliedMatrix(matrix, out X, out Y, out Z, out W);
#elif MATRIX_POSTMULTIPLIED_CONVENTION
        CreateQuaternionComponentsFromPostmultipliedMatrix(matrix, out X, out Y, out Z, out W);
#else
        throw new ApplicationException(Matrix.Docs.BaseMessage);
#endif
        }

    /// <summary><inheritdoc cref="CreateQuaternionFromMatrix{T}(in Matrix4{T}, out Quaternion{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPostmultiplicationConvention{T}"/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CreateQuaternionFromPostmultipliedMatrix<T>(in Matrix4<T> matrix, out Quaternion<T> quaternion)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        CreateQuaternionComponentsFromPostmultipliedMatrix(in matrix, out var X, out var Y, out var Z, out var W);
        quaternion = new(X, Y, Z, W);
        }

    /// <summary><inheritdoc cref="CreateQuaternionFromMatrix{T}(in Matrix4{T}, out Quaternion{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPremultiplicationConvention{T}"/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CreateQuaternionFromPremultipliedMatrix<T>(in Matrix4<T> matrix, out Quaternion<T> quaternion)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        CreateQuaternionComponentsFromPremultipliedMatrix(in matrix, out var X, out var Y, out var Z, out var W);
        quaternion = new(X, Y, Z, W);
        }

    /// <summary><inheritdoc cref="IMatrix{Numeric}.IMatrixBase{Matrix, Vector, Direction, Rotation}.GetRotation()"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.MultiplicationConventionInstructions{T}"/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CreateQuaternionFromMatrix<T>(in Matrix4<T> matrix, out Quaternion<T> quaternion)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
#if MATRIX_PREMULTIPLIED_CONVENTION
        CreateQuaternionFromPremultipliedMatrix(matrix, out quaternion);
#elif MATRIX_POSTMULTIPLIED_CONVENTION
        CreateQuaternionFromPostmultipliedMatrix(matrix, out quaternion);
#else
        throw new ApplicationException(Matrix.Docs.BaseMessage);
#endif
        }

    #endregion Matrix 🡺 Quaternion

    #region Quaternion 🡺 Matrix

    /// <summary><inheritdoc cref="CreateRotation{T}(Quaternion{T}, out Matrix4{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPostmultiplicationConvention{T}"/>
    /// </remarks>
    [BunnyAttributes.SIMDCandidate("Can this be improved?")]
    public static void CreatePostmultipliedRotation<T>(Quaternion<T> rotation, out Matrix4<T> matrix)
     where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        var two = GenericNumbers<T>.Two;
        if (typeof(T) == typeof(float) && Vector128.IsHardwareAccelerated && Vector128<float>.IsSupported) {
            var rtn = Unsafe.BitCast<Quaternion<T>, Vector128<T>>(rotation);
            var sqr = rtn * rtn;
            var v4w = Vector128.Create(rotation.W) * rtn;
            var xy = rotation.X * rotation.Y;
            var xz = rotation.X * rotation.Z;
            var yz = rotation.Y * rotation.Z;
            matrix = new(
                a1: T.One - two * (sqr[1] + sqr[2]),    // 1 - 2(y^2 + z^2)
                a2: two * (xy - v4w[2]),                // 2(xy - wz)
                a3: two * (xz + v4w[1]),                // 2(xz + wy)
                a4: T.Zero,
                b1: two * (xy + v4w[2]),                // 2(xy + wz)
                b2: T.One - two * (sqr[0] + sqr[2]),    // 1 - 2(x^2 + z^2)
                b3: two * (yz - v4w[0]),                // 2(yz - wx)
                b4: T.Zero,
                c1: two * (xz - v4w[1]),                // 2(xz - wy)
                c2: two * (yz + v4w[0]),                // 2(yz + wx)
                c3: T.One - two * (sqr[0] + sqr[1]),    // 1 - 2(x^2 + y^2)
                c4: T.Zero,
                d1: T.Zero, d2: T.Zero, d3: T.Zero, d4: T.One
                );
            }
        else if (typeof(T) == typeof(double) && Vector256.IsHardwareAccelerated && Vector256<double>.IsSupported) {
            var rtn = Unsafe.BitCast<Quaternion<T>, Vector256<T>>(rotation);
            var sqr = rtn * rtn;
            var v4w = Vector256.Create(rotation.W) * rtn;
            var xy = rotation.X * rotation.Y;
            var xz = rotation.X * rotation.Z;
            var yz = rotation.Y * rotation.Z;
            matrix = new(
                a1: T.One - two * (sqr[1] + sqr[2]),    // 1 - 2(y^2 + z^2)
                a2: two * (xy - v4w[2]),                // 2(xy - wz)
                a3: two * (xz + v4w[1]),                // 2(xz + wy)
                a4: T.Zero,
                b1: two * (xy + v4w[2]),                // 2(xy + wz)
                b2: T.One - two * (sqr[0] + sqr[2]),    // 1 - 2(x^2 + z^2)
                b3: two * (yz - v4w[0]),                // 2(yz - wx)
                b4: T.Zero,
                c1: two * (xz - v4w[1]),                // 2(xz - wy)
                c2: two * (yz + v4w[0]),                // 2(yz + wx)
                c3: T.One - two * (sqr[0] + sqr[1]),    // 1 - 2(x^2 + y^2)
                c4: T.Zero,
                d1: T.Zero, d2: T.Zero, d3: T.Zero, d4: T.One
                );
            }
        else {
            var x2 = rotation.X * rotation.X;
            var y2 = rotation.Y * rotation.Y;
            var z2 = rotation.Z * rotation.Z;
            var wx = rotation.W * rotation.X;
            var wy = rotation.W * rotation.Y;
            var wz = rotation.W * rotation.Z;
            var xy = rotation.X * rotation.Y;
            var xz = rotation.X * rotation.Z;
            var yz = rotation.Y * rotation.Z;
            matrix = new(
                a1: T.One - two * (y2 + z2),            // 1 - 2(y^2 + z^2)
                a2: two * (xy - wz),                    // 2(xy - wz)
                a3: two * (xz + wy),                    // 2(xz + wy)
                a4: T.Zero,
                b1: two * (xy + wz),                    // 2(xy + wz)
                b2: T.One - two * (x2 + z2),            // 1 - 2(x^2 + z^2)
                b3: two * (yz - wx),                    // 2(yz - wx)
                b4: T.Zero,
                c1: two * (xz - wy),                    // 2(xz - wy)
                c2: two * (yz + wx),                    // 2(yz + wx)
                c3: T.One - two * (x2 + y2),            // 1 - 2(x^2 + y^2)
                c4: T.Zero,
                d1: T.Zero, d2: T.Zero, d3: T.Zero, d4: T.One
                );
            }
        }

    /// <summary><inheritdoc cref="CreateRotation{T}(Quaternion{T}, out Matrix4{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPremultiplicationConvention{T}"/>
    /// </remarks>
    [BunnyAttributes.SIMDCandidate("Can this be improved?")]
    public static void CreatePremultipliedRotation<T>(Quaternion<T> rotation, out Matrix4<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        var two = GenericNumbers<T>.Two;
        if (typeof(T) == typeof(float) && Vector128.IsHardwareAccelerated && Vector128<float>.IsSupported) {
            var rtn = Unsafe.BitCast<Quaternion<T>, Vector128<T>>(rotation);
            var sqr = rtn * rtn;
            var v4w = Vector128.Create(rotation.W) * rtn;
            var xy = rotation.X * rotation.Y;
            var xz = rotation.X * rotation.Z;
            var yz = rotation.Y * rotation.Z;
            matrix = new(
                a1: T.One - two * (sqr[1] + sqr[2]),    // 1 - 2(y^2 + z^2)
                a2: two * (xy + v4w[2]),                // 2(xy + wz)
                a3: two * (xz - v4w[1]),                // 2(xz - wy)
                a4: T.Zero,
                b1: two * (xy - v4w[2]),                // 2(xy - wz)
                b2: T.One - two * (sqr[0] + sqr[2]),    // 1 - 2(x^2 + z^2)
                b3: two * (yz + v4w[0]),                // 2(yz + wx)
                b4: T.Zero,
                c1: two * (xz + v4w[1]),                // 2(xz + wy)
                c2: two * (yz - v4w[0]),                // 2(yz - wx)
                c3: T.One - two * (sqr[0] + sqr[1]),    // 1 - 2(x^2 + y^2)
                c4: T.Zero,
                d1: T.Zero, d2: T.Zero, d3: T.Zero, d4: T.One
                );
            }
        else if (typeof(T) == typeof(double) && Vector256.IsHardwareAccelerated && Vector256<double>.IsSupported) {
            var rtn = Unsafe.BitCast<Quaternion<T>, Vector256<T>>(rotation);
            var sqr = rtn * rtn;
            var v4w = Vector256.Create(rotation.W) * rtn;
            var xy = rotation.X * rotation.Y;
            var xz = rotation.X * rotation.Z;
            var yz = rotation.Y * rotation.Z;
            matrix = new(
                a1: T.One - two * (sqr[1] + sqr[2]),    // 1 - 2(y^2 + z^2)
                a2: two * (xy + v4w[2]),                // 2(xy + wz)
                a3: two * (xz - v4w[1]),                // 2(xz - wy)
                a4: T.Zero,
                b1: two * (xy - v4w[2]),                // 2(xy - wz)
                b2: T.One - two * (sqr[0] + sqr[2]),    // 1 - 2(x^2 + z^2)
                b3: two * (yz + v4w[0]),                // 2(yz + wx)
                b4: T.Zero,
                c1: two * (xz + v4w[1]),                // 2(xz + wy)
                c2: two * (yz - v4w[0]),                // 2(yz - wx)
                c3: T.One - two * (sqr[0] + sqr[1]),    // 1 - 2(x^2 + y^2)
                c4: T.Zero,
                d1: T.Zero, d2: T.Zero, d3: T.Zero, d4: T.One
                );
            }
        else {
            var x2 = rotation.X * rotation.X;
            var y2 = rotation.Y * rotation.Y;
            var z2 = rotation.Z * rotation.Z;
            var wx = rotation.W * rotation.X;
            var wy = rotation.W * rotation.Y;
            var wz = rotation.W * rotation.Z;
            var xy = rotation.X * rotation.Y;
            var xz = rotation.X * rotation.Z;
            var yz = rotation.Y * rotation.Z;
            matrix = new(
                a1: T.One - two * (y2 + z2),            // 1 - 2(y^2 + z^2)
                a2: two * (xy + wz),                    // 2(xy + wz)
                a3: two * (xz - wy),                    // 2(xz - wy)
                a4: T.Zero,
                b1: two * (xy - wz),                    // 2(xy - wz)
                b2: T.One - two * (x2 + z2),            // 1 - 2(x^2 + z^2)
                b3: two * (yz + wx),                    // 2(yz + wx)
                b4: T.Zero,
                c1: two * (xz + wy),                    // 2(xz + wy)
                c2: two * (yz - wx),                    // 2(yz - wx)
                c3: T.One - two * (x2 + y2),            // 1 - 2(x^2 + y^2)
                c4: T.Zero,
                d1: T.Zero, d2: T.Zero, d3: T.Zero, d4: T.One
                );
            }
        }

    /// <summary><inheritdoc cref="IMatrix{Numeric}.IMatrixBase{Matrix, Vector, Direction, Rotation}.CreateRotation(Rotation)"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.MultiplicationConventionInstructions{T}"/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CreateRotation<T>(Quaternion<T> rotation, out Matrix4<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
#if MATRIX_PREMULTIPLIED_CONVENTION
        CreatePremultipliedRotation(rotation, out matrix);
#elif MATRIX_POSTMULTIPLIED_CONVENTION
        CreatePostmultipliedRotation(rotation, out matrix);
#else
        throw new ApplicationException(Matrix.Docs.BaseMessage);
#endif
        }

    #endregion Quaternion 🡺 Matrix

    #region Transform (Vectors)

    /// <summary><inheritdoc cref="IMatrix{T}.IMatrixBase{Matrix, Vector, Direction, Rotation}.Transform(Vector)"/>
    /// </summary>
    /// <remarks> 📝 The result is divided by its homogeneous <c>W</c> coordinate, so a projecting <paramref name="matrix"/> returns a 3D point. To receive undivided homogenous coordinates, call <see cref="Transform{T}(in Matrix4{T}, Vector4{T}, out Vector4{T})"/> instead.
    /// <para/><inheritdoc cref="Docs.MultiplicationConventionInstructions{T}"/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Transform<T>(in Matrix4<T> matrix, Vector3<T> vector, out Vector3<T> result)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
#if MATRIX_PREMULTIPLIED_CONVENTION
        TransformPremultipliedRowVector(vector, matrix, out result);
#elif MATRIX_POSTMULTIPLIED_CONVENTION
        TransformPostmultipliedColumnVector(matrix, vector, out result);
#else
        throw new ApplicationException(Matrix.Docs.BaseMessage);
#endif
        }

    /// <summary><inheritdoc cref="IMatrix{T}.IMatrixBase{Matrix, Vector, Direction, Rotation}.Transform(Vector)"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.MultiplicationConventionInstructions{T}"/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Transform<T>(in Matrix4<T> matrix, Vector4<T> vector, out Vector4<T> result)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
#if MATRIX_PREMULTIPLIED_CONVENTION
        TransformPremultipliedRowVector(vector, matrix, out result);
#elif MATRIX_POSTMULTIPLIED_CONVENTION
        TransformPostmultipliedColumnVector(matrix, vector, out result);
#else
        throw new ApplicationException(Matrix.Docs.BaseMessage);
#endif
        }

    /// <summary><inheritdoc cref="IMatrix{T}.IMatrixBase{Matrix, Vector, Direction, Rotation}.Transform(Vector)"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.MultiplicationConventionInstructions{T}"/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Transform<T>(in Matrix2<T> matrix, Vector2<T> vector, out Vector2<T> result)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
#if MATRIX_PREMULTIPLIED_CONVENTION
        TransformPremultipliedRowVector(vector, matrix, out result);
#elif MATRIX_POSTMULTIPLIED_CONVENTION
        TransformPostmultipliedColumnVector(matrix, vector, out result);
#else
        throw new ApplicationException(Matrix.Docs.BaseMessage);
#endif
        }

    /// <summary><inheritdoc cref="IMatrix{T}.IMatrixBase{Matrix, Vector, Direction, Rotation}.Transform(Vector)"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.MultiplicationConventionInstructions{T}"/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Transform<T>(in Matrix3<T> matrix, Vector3<T> vector, out Vector3<T> result)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
#if MATRIX_PREMULTIPLIED_CONVENTION
        TransformPremultipliedRowVector(vector, matrix, out result);
#elif MATRIX_POSTMULTIPLIED_CONVENTION
        TransformPostmultipliedColumnVector(matrix, vector, out result);
#else
        throw new ApplicationException(Matrix.Docs.BaseMessage);
#endif
        }

    /// <summary><inheritdoc cref="IMatrix{T}.IMatrixBase{Matrix, Vector, Direction, Rotation}.Transform(Vector)"/>
    /// </summary>
    /// <remarks> 📝 The result is divided by its homogeneous <c>Z</c> coordinate, so a projecting <paramref name="matrix"/> returns a 2D point. To receive undivided homogenous coordinates, call <see cref="Transform{T}(in Matrix3{T}, Vector3{T}, out Vector3{T})"/> instead.
    /// <para/><inheritdoc cref="Docs.MultiplicationConventionInstructions{T}"/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Transform<T>(in Matrix3<T> matrix, Vector2<T> vector, out Vector2<T> result)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
#if MATRIX_PREMULTIPLIED_CONVENTION
        TransformPremultipliedRowVector(vector, matrix, out result);
#elif MATRIX_POSTMULTIPLIED_CONVENTION
        TransformPostmultipliedColumnVector(matrix, vector, out result);
#else
        throw new ApplicationException(Matrix.Docs.BaseMessage);
#endif
        }

    /// <summary><inheritdoc cref="Transform{T}(in Matrix4{T}, Vector3{T}, out Vector3{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPostmultiplicationConvention{T}"/>
    /// </remarks>
    [BunnyAttributes.SIMDCandidate]
    public static void TransformPostmultipliedColumnVector<T>(in Matrix4<T> matrix, Vector3<T> vector, out Vector3<T> result)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        var w = matrix.D1 * vector.X + matrix.D2 * vector.Y + matrix.D3 * vector.Z + matrix.D4;
        var inverseW = T.One / w;
        result = new(
            (matrix.A1 * vector.X + matrix.A2 * vector.Y + matrix.A3 * vector.Z + matrix.A4) * inverseW,
            (matrix.B1 * vector.X + matrix.B2 * vector.Y + matrix.B3 * vector.Z + matrix.B4) * inverseW,
            (matrix.C1 * vector.X + matrix.C2 * vector.Y + matrix.C3 * vector.Z + matrix.C4) * inverseW
            );
        }

    /// <summary><inheritdoc cref="Transform{T}(in Matrix4{T}, Vector3{T}, out Vector3{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPostmultiplicationConvention{T}"/>
    /// </remarks>
    [BunnyAttributes.SIMDCandidate]
    public static void TransformPostmultipliedColumnVector<T>(in Matrix3x4<T> matrix, Vector3<T> vector, out Vector3<T> result)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        result = new(
            matrix.A1 * vector.X + matrix.A2 * vector.Y + matrix.A3 * vector.Z + matrix.A4,
            matrix.B1 * vector.X + matrix.B2 * vector.Y + matrix.B3 * vector.Z + matrix.B4,
            matrix.C1 * vector.X + matrix.C2 * vector.Y + matrix.C3 * vector.Z + matrix.C4
            );
        }

    /// <summary><inheritdoc cref="Transform{T}(in Matrix3{T}, Vector3{T}, out Vector3{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPostmultiplicationConvention{T}"/>
    /// </remarks>
    [BunnyAttributes.SIMDCandidate]
    public static void TransformPostmultipliedColumnVector<T>(in Matrix2x3<T> matrix, Vector2<T> vector, out Vector2<T> result)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        result = new(
            matrix.A1 * vector.X + matrix.A2 * vector.Y + matrix.A3,
            matrix.B1 * vector.X + matrix.B2 * vector.Y + matrix.B3
            );
        }

    /// <summary><inheritdoc cref="Transform{T}(in Matrix4{T}, Vector4{T}, out Vector4{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPostmultiplicationConvention{T}"/>
    /// </remarks>
    [BunnyAttributes.SIMDCandidate]
    public static void TransformPostmultipliedColumnVector<T>(in Matrix4<T> matrix, Vector4<T> vector, out Vector4<T> result)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        result = new(
            matrix.A1 * vector.X + matrix.A2 * vector.Y + matrix.A3 * vector.Z + matrix.A4 * vector.W,
            matrix.B1 * vector.X + matrix.B2 * vector.Y + matrix.B3 * vector.Z + matrix.B4 * vector.W,
            matrix.C1 * vector.X + matrix.C2 * vector.Y + matrix.C3 * vector.Z + matrix.C4 * vector.W,
            matrix.D1 * vector.X + matrix.D2 * vector.Y + matrix.D3 * vector.Z + matrix.D4 * vector.W
            );
        }

    /// <summary><inheritdoc cref="Transform{T}(in Matrix2{T}, Vector2{T}, out Vector2{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPostmultiplicationConvention{T}"/>
    /// </remarks>
    [BunnyAttributes.SIMDCandidate]
    public static void TransformPostmultipliedColumnVector<T>(in Matrix2<T> matrix, Vector2<T> vector, out Vector2<T> result)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        result = new(
            matrix.A1 * vector.X + matrix.A2 * vector.Y,
            matrix.B1 * vector.X + matrix.B2 * vector.Y
        );
        }

    /// <summary><inheritdoc cref="Transform{T}(in Matrix3{T}, Vector3{T}, out Vector3{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPostmultiplicationConvention{T}"/>
    /// </remarks>
    [BunnyAttributes.SIMDCandidate]
    public static void TransformPostmultipliedColumnVector<T>(in Matrix3<T> matrix, Vector2<T> vector, out Vector2<T> result)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        var w = matrix.C1 * vector.X + matrix.C2 * vector.Y + matrix.C3;
        var inverseW = T.One / w;
        result = new(
            (matrix.A1 * vector.X + matrix.A2 * vector.Y + matrix.A3) * inverseW,
            (matrix.B1 * vector.X + matrix.B2 * vector.Y + matrix.B3) * inverseW
        );
        }

    /// <summary><inheritdoc cref="Transform{T}(in Matrix3{T}, Vector3{T}, out Vector3{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPostmultiplicationConvention{T}"/>
    /// </remarks>
    [BunnyAttributes.SIMDCandidate]
    public static void TransformPostmultipliedColumnVector<T>(in Matrix3<T> matrix, Vector3<T> vector, out Vector3<T> result)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        result = new(
            matrix.A1 * vector.X + matrix.A2 * vector.Y + matrix.A3 * vector.Z,
            matrix.B1 * vector.X + matrix.B2 * vector.Y + matrix.B3 * vector.Z,
            matrix.C1 * vector.X + matrix.C2 * vector.Y + matrix.C3 * vector.Z
        );
        }

    /// <summary><inheritdoc cref="Transform{T}(in Matrix4{T}, Vector3{T}, out Vector3{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPremultiplicationConvention{T}"/>
    /// </remarks>
    [BunnyAttributes.SIMDCandidate]
    public static void TransformPremultipliedRowVector<T>(Vector3<T> vector, in Matrix4<T> matrix, out Vector3<T> result)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        var w = vector.X * matrix.A4 + vector.Y * matrix.B4 + vector.Z * matrix.C4 + matrix.D4;
        var inverseW = T.One / w;
        result = new(
            (vector.X * matrix.A1 + vector.Y * matrix.B1 + vector.Z * matrix.C1 + matrix.D1) * inverseW,
            (vector.X * matrix.A2 + vector.Y * matrix.B2 + vector.Z * matrix.C2 + matrix.D2) * inverseW,
            (vector.X * matrix.A3 + vector.Y * matrix.B3 + vector.Z * matrix.C3 + matrix.D3) * inverseW
            );
        }

    /// <summary><inheritdoc cref="Transform{T}(in Matrix4{T}, Vector3{T}, out Vector3{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPremultiplicationConvention{T}"/>
    /// </remarks>
    [BunnyAttributes.SIMDCandidate]
    public static void TransformPremultipliedRowVector<T>(Vector3<T> vector, in Matrix4x3<T> matrix, out Vector3<T> result)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        result = new(
            vector.X * matrix.A1 + vector.Y * matrix.B1 + vector.Z * matrix.C1 + matrix.D1,
            vector.X * matrix.A2 + vector.Y * matrix.B2 + vector.Z * matrix.C2 + matrix.D2,
            vector.X * matrix.A3 + vector.Y * matrix.B3 + vector.Z * matrix.C3 + matrix.D3
            );
        }

    /// <summary><inheritdoc cref="Transform{T}(in Matrix3{T}, Vector3{T}, out Vector3{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPremultiplicationConvention{T}"/>
    /// </remarks>
    [BunnyAttributes.SIMDCandidate]
    public static void TransformPremultipliedRowVector<T>(Vector2<T> vector, in Matrix3x2<T> matrix, out Vector2<T> result)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        result = new(
            vector.X * matrix.A1 + vector.Y * matrix.B1 + matrix.C1,
            vector.X * matrix.A2 + vector.Y * matrix.B2 + matrix.C2
            );
        }

    /// <summary><inheritdoc cref="Transform{T}(in Matrix4{T}, Vector4{T}, out Vector4{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPremultiplicationConvention{T}"/>
    /// </remarks>
    [BunnyAttributes.SIMDCandidate]
    public static void TransformPremultipliedRowVector<T>(Vector4<T> vector, in Matrix4<T> matrix, out Vector4<T> result)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        result = new(
            vector.X * matrix.A1 + vector.Y * matrix.B1 + vector.Z * matrix.C1 + vector.W * matrix.D1,
            vector.X * matrix.A2 + vector.Y * matrix.B2 + vector.Z * matrix.C2 + vector.W * matrix.D2,
            vector.X * matrix.A3 + vector.Y * matrix.B3 + vector.Z * matrix.C3 + vector.W * matrix.D3,
            vector.X * matrix.A4 + vector.Y * matrix.B4 + vector.Z * matrix.C4 + vector.W * matrix.D4
            );
        }

    /// <summary><inheritdoc cref="Transform{T}(in Matrix2{T}, Vector2{T}, out Vector2{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPremultiplicationConvention{T}"/>
    /// </remarks>
    [BunnyAttributes.SIMDCandidate]
    public static void TransformPremultipliedRowVector<T>(Vector2<T> vector, in Matrix2<T> matrix, out Vector2<T> result)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        result = new(
            vector.X * matrix.A1 + vector.Y * matrix.B1,
            vector.X * matrix.A2 + vector.Y * matrix.B2
        );
        }

    /// <summary><inheritdoc cref="Transform{T}(in Matrix3{T}, Vector3{T}, out Vector3{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPremultiplicationConvention{T}"/>
    /// </remarks>
    [BunnyAttributes.SIMDCandidate]
    public static void TransformPremultipliedRowVector<T>(Vector3<T> vector, in Matrix3<T> matrix, out Vector3<T> result)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        result = new(
            vector.X * matrix.A1 + vector.Y * matrix.B1 + vector.Z * matrix.C1,
            vector.X * matrix.A2 + vector.Y * matrix.B2 + vector.Z * matrix.C2,
            vector.X * matrix.A3 + vector.Y * matrix.B3 + vector.Z * matrix.C3
        );
        }

    /// <summary><inheritdoc cref="Transform{T}(in Matrix3{T}, Vector3{T}, out Vector3{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPremultiplicationConvention{T}"/>
    /// </remarks>
    [BunnyAttributes.SIMDCandidate]
    public static void TransformPremultipliedRowVector<T>(Vector2<T> vector, in Matrix3<T> matrix, out Vector2<T> result)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        var w = vector.X * matrix.A3 + vector.Y * matrix.B3 + matrix.C3;
        var inverseW = T.One / w;
        result = new(
            (vector.X * matrix.A1 + vector.Y * matrix.B1 + matrix.C1) * inverseW,
            (vector.X * matrix.A2 + vector.Y * matrix.B2 + matrix.C2) * inverseW
        );
        }

    #endregion Transform (Vectors)

    #region Extraction - Translation

    /// <summary><inheritdoc cref="IMatrix{T}.ITranslationMatrix{Matrix, Vector, Direction, Rotation}.GetTranslation(in Matrix)"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.MultiplicationConventionInstructions{T}"/>
    /// </remarks>
    public static void GetTranslation<T>(in Matrix4<T> matrix, out Vector3<T> translation)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
#if MATRIX_PREMULTIPLIED_CONVENTION
        GetTranslationFromPremultiplied(in matrix, out translation);
#elif MATRIX_POSTMULTIPLIED_CONVENTION
        GetTranslationFromPostmultiplied(in matrix, out translation);
#else
        throw new ApplicationException(Matrix.Docs.BaseMessage);
#endif
        }

    /// <summary><inheritdoc cref="IMatrix{T}.ITranslationMatrix{Matrix, Vector, Direction, Rotation}.GetTranslation(in Matrix)"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.MultiplicationConventionInstructions{T}"/>
    /// </remarks>
    public static void GetTranslation<T>(in Matrix3<T> matrix, out Vector2<T> translation)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
#if MATRIX_PREMULTIPLIED_CONVENTION
        GetTranslationFromPremultiplied(in matrix, out translation);
#elif MATRIX_POSTMULTIPLIED_CONVENTION
        GetTranslationFromPostmultiplied(in matrix, out translation);
#else
        throw new ApplicationException(Matrix.Docs.BaseMessage);
#endif
        }

    /// <summary><inheritdoc cref="GetTranslation{T}(in Matrix4{T}, out Vector3{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPostmultiplicationConvention{T}"/>
    /// </remarks>
    public static void GetTranslationFromPostmultiplied<T>(in Matrix4<T> matrix, out Vector3<T> translation)
          where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        translation = new(matrix.A4, matrix.B4, matrix.C4);
        }

    /// <summary><inheritdoc cref="GetTranslation{T}(in Matrix4{T}, out Vector3{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPostmultiplicationConvention{T}"/>
    /// </remarks>
    public static void GetTranslationFromPostmultiplied<T>(in Matrix3x4<T> matrix, out Vector3<T> translation)
          where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        translation = new(matrix.A4, matrix.B4, matrix.C4);
        }

    /// <summary><inheritdoc cref="GetTranslation{T}(in Matrix3{T}, out Vector2{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPostmultiplicationConvention{T}"/>
    /// </remarks>
    public static void GetTranslationFromPostmultiplied<T>(in Matrix3<T> matrix, out Vector2<T> translation)
          where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        translation = new(matrix.A3, matrix.B3);
        }

    /// <summary><inheritdoc cref="GetTranslation{T}(in Matrix3{T}, out Vector2{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPostmultiplicationConvention{T}"/>
    /// </remarks>
    public static void GetTranslationFromPostmultiplied<T>(in Matrix2x3<T> matrix, out Vector2<T> translation)
          where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        translation = new(matrix.A3, matrix.B3);
        }

    /// <summary><inheritdoc cref="GetTranslation{T}(in Matrix4{T}, out Vector3{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPremultiplicationConvention{T}"/>
    /// </remarks>
    public static void GetTranslationFromPremultiplied<T>(in Matrix4<T> matrix, out Vector3<T> translation)
          where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        translation = new(matrix.D1, matrix.D2, matrix.D3);
        }

    /// <summary><inheritdoc cref="GetTranslation{T}(in Matrix4{T}, out Vector3{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPremultiplicationConvention{T}"/>
    /// </remarks>
    public static void GetTranslationFromPremultiplied<T>(in Matrix4x3<T> matrix, out Vector3<T> translation)
          where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        translation = new(matrix.D1, matrix.D2, matrix.D3);
        }

    /// <summary><inheritdoc cref="GetTranslation{T}(in Matrix3{T}, out Vector2{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPremultiplicationConvention{T}"/>
    /// </remarks>
    public static void GetTranslationFromPremultiplied<T>(in Matrix3<T> matrix, out Vector2<T> translation)
          where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        translation = new(matrix.C1, matrix.C2);
        }

    /// <summary><inheritdoc cref="GetTranslation{T}(in Matrix3{T}, out Vector2{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPremultiplicationConvention{T}"/>
    /// </remarks>
    public static void GetTranslationFromPremultiplied<T>(in Matrix3x2<T> matrix, out Vector2<T> translation)
          where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        translation = new(matrix.C1, matrix.C2);
        }

    #endregion Extraction - Translation

    #region Extraction - Scale

    /// <summary><inheritdoc cref="IMatrix{T}.IMatrixBase{Matrix, Vector, Direction, Rotation}.GetScale(in Matrix)"/>
    /// <para/> 📝 The output <paramref name="scale"/> is assigned a W component that is the matrix's fourth basis magnitude. This value is relevant for four-dimensional linear algebra but <em>not</em> for matrices where the fourth row/column (depending on the multiplication convention) represents a <see cref="GetTranslation{T}(in Matrix4{T}, out Vector3{T})">translation</see>. For a 3D scale, use <see cref="GetScale{T}(in Matrix4{T}, out Vector3{T})"/> instead.
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.MultiplicationConventionInstructions{T}"/>
    /// </remarks>
    public static void GetScale<T>(in Matrix4<T> matrix, out Vector4<T> scale)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
#if MATRIX_PREMULTIPLIED_CONVENTION
        GetScaleFromPremultiplied(in matrix, out scale);
#elif MATRIX_POSTMULTIPLIED_CONVENTION
        GetScaleFromPostmultiplied(in matrix, out scale);
#else
        throw new ApplicationException(Matrix.Docs.BaseMessage);
#endif
        }

    /// <summary><inheritdoc cref="IMatrix{T}.IMatrixBase{Matrix, Vector, Direction, Rotation}.GetScale(in Matrix)"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.MultiplicationConventionInstructions{T}"/>
    /// </remarks>
    public static void GetScale<T>(in Matrix3<T> matrix, out Vector3<T> scale)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
#if MATRIX_PREMULTIPLIED_CONVENTION
        GetScaleFromPremultiplied(in matrix, out scale);
#elif MATRIX_POSTMULTIPLIED_CONVENTION
        GetScaleFromPostmultiplied(in matrix, out scale);
#else
        throw new ApplicationException(Matrix.Docs.BaseMessage);
#endif
        }

    /// <summary><inheritdoc cref="IMatrix{T}.IMatrixBase{Matrix, Vector, Direction, Rotation}.GetScale(in Matrix)"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.MultiplicationConventionInstructions{T}"/>
    /// </remarks>
    public static void GetScale<T>(in Matrix2<T> matrix, out Vector2<T> scale)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
#if MATRIX_PREMULTIPLIED_CONVENTION
        GetScaleFromPremultiplied(in matrix, out scale);
#elif MATRIX_POSTMULTIPLIED_CONVENTION
        GetScaleFromPostmultiplied(in matrix, out scale);
#else
        throw new ApplicationException(Matrix.Docs.BaseMessage);
#endif
        }

    /// <summary><inheritdoc cref="IMatrix{T}.IMatrixBase{Matrix, Vector, Direction, Rotation}.GetScale(in Matrix)"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.MultiplicationConventionInstructions{T}"/>
    /// </remarks>
    public static void GetScale<T>(in Matrix4<T> matrix, out Vector3<T> scale)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
#if MATRIX_PREMULTIPLIED_CONVENTION
        GetScaleFromPremultiplied(in matrix, out scale);
#elif MATRIX_POSTMULTIPLIED_CONVENTION
        GetScaleFromPostmultiplied(in matrix, out scale);
#else
        throw new ApplicationException(Matrix.Docs.BaseMessage);
#endif
        }

    /// <summary><inheritdoc cref="IMatrix{T}.IMatrixBase{Matrix, Vector, Direction, Rotation}.GetScale(in Matrix)"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.MultiplicationConventionInstructions{T}"/>
    /// </remarks>
    public static void GetScale<T>(in Matrix3<T> matrix, out Vector2<T> scale)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
#if MATRIX_PREMULTIPLIED_CONVENTION
        GetScaleFromPremultiplied(in matrix, out scale);
#elif MATRIX_POSTMULTIPLIED_CONVENTION
        GetScaleFromPostmultiplied(in matrix, out scale);
#else
        throw new ApplicationException(Matrix.Docs.BaseMessage);
#endif
        }

    /// <summary><inheritdoc cref="GetScale{T}(in Matrix4{T}, out Vector4{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPostmultiplicationConvention{T}"/>
    /// </remarks>
    [BunnyAttributes.SIMDCandidate]
    public static void GetScaleFromPostmultiplied<T>(in Matrix4<T> matrix, out Vector4<T> scale)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        var det = matrix.Determinant;
        var a = det < T.Zero ? T.NegativeOne : T.One;
        scale = new(
            x: T.Sqrt(matrix.A1 * matrix.A1 + matrix.B1 * matrix.B1 + matrix.C1 * matrix.C1 + matrix.D1 * matrix.D1) * a,
            y: T.Sqrt(matrix.A2 * matrix.A2 + matrix.B2 * matrix.B2 + matrix.C2 * matrix.C2 + matrix.D2 * matrix.D2),
            z: T.Sqrt(matrix.A3 * matrix.A3 + matrix.B3 * matrix.B3 + matrix.C3 * matrix.C3 + matrix.D3 * matrix.D3),
            w: T.Sqrt(matrix.A4 * matrix.A4 + matrix.B4 * matrix.B4 + matrix.C4 * matrix.C4 + matrix.D4 * matrix.D4)
            );
        }

    /// <summary><inheritdoc cref="GetScale{T}(in Matrix3{T}, out Vector3{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPostmultiplicationConvention{T}"/>
    /// </remarks>
    [BunnyAttributes.SIMDCandidate]
    public static void GetScaleFromPostmultiplied<T>(in Matrix3<T> matrix, out Vector3<T> scale)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        var det = matrix.Determinant;
        var a = det < T.Zero ? T.NegativeOne : T.One;
        scale = new(
            x: T.Sqrt(matrix.A1 * matrix.A1 + matrix.B1 * matrix.B1 + matrix.C1 * matrix.C1) * a,
            y: T.Sqrt(matrix.A2 * matrix.A2 + matrix.B2 * matrix.B2 + matrix.C2 * matrix.C2),
            z: T.Sqrt(matrix.A3 * matrix.A3 + matrix.B3 * matrix.B3 + matrix.C3 * matrix.C3)
            );
        }

    /// <summary><inheritdoc cref="GetScale{T}(in Matrix2{T}, out Vector2{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPostmultiplicationConvention{T}"/>
    /// </remarks>
    [BunnyAttributes.SIMDCandidate]
    public static void GetScaleFromPostmultiplied<T>(in Matrix2<T> matrix, out Vector2<T> scale)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        var det = matrix.Determinant;
        var a = det < T.Zero ? T.NegativeOne : T.One;
        scale = new(
            x: T.Sqrt(matrix.A1 * matrix.A1 + matrix.B1 * matrix.B1) * a,
            y: T.Sqrt(matrix.A2 * matrix.A2 + matrix.B2 * matrix.B2)
            );
        }

    /// <summary><inheritdoc cref="GetScale{T}(in Matrix4{T}, out Vector3{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPostmultiplicationConvention{T}"/>
    /// </remarks>
    [BunnyAttributes.SIMDCandidate]
    public static void GetScaleFromPostmultiplied<T>(in Matrix4<T> matrix, out Vector3<T> scale)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        var det = matrix.Determinant;
        var a = det < T.Zero ? T.NegativeOne : T.One;
        scale = new(
            x: T.Sqrt(matrix.A1 * matrix.A1 + matrix.B1 * matrix.B1 + matrix.C1 * matrix.C1) * a,
            y: T.Sqrt(matrix.A2 * matrix.A2 + matrix.B2 * matrix.B2 + matrix.C2 * matrix.C2),
            z: T.Sqrt(matrix.A3 * matrix.A3 + matrix.B3 * matrix.B3 + matrix.C3 * matrix.C3)
            );
        }

    /// <summary><inheritdoc cref="GetScale{T}(in Matrix4{T}, out Vector3{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPostmultiplicationConvention{T}"/>
    /// </remarks>
    [BunnyAttributes.SIMDCandidate]
    public static void GetScaleFromPostmultiplied<T>(in Matrix3x4<T> matrix, out Vector3<T> scale)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        var det = matrix.Determinant;
        var a = det < T.Zero ? T.NegativeOne : T.One;
        scale = new(
            x: T.Sqrt(matrix.A1 * matrix.A1 + matrix.B1 * matrix.B1 + matrix.C1 * matrix.C1) * a,
            y: T.Sqrt(matrix.A2 * matrix.A2 + matrix.B2 * matrix.B2 + matrix.C2 * matrix.C2),
            z: T.Sqrt(matrix.A3 * matrix.A3 + matrix.B3 * matrix.B3 + matrix.C3 * matrix.C3)
            );
        }

    /// <summary><inheritdoc cref="GetScale{T}(in Matrix3{T}, out Vector2{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPostmultiplicationConvention{T}"/>
    /// </remarks>
    [BunnyAttributes.SIMDCandidate]
    public static void GetScaleFromPostmultiplied<T>(in Matrix3<T> matrix, out Vector2<T> scale)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        var det = matrix.Determinant;
        var a = det < T.Zero ? T.NegativeOne : T.One;
        scale = new(
            x: T.Sqrt(matrix.A1 * matrix.A1 + matrix.B1 * matrix.B1) * a,
            y: T.Sqrt(matrix.A2 * matrix.A2 + matrix.B2 * matrix.B2)
            );
        }

    /// <summary><inheritdoc cref="GetScale{T}(in Matrix3{T}, out Vector2{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPostmultiplicationConvention{T}"/>
    /// </remarks>
    [BunnyAttributes.SIMDCandidate]
    public static void GetScaleFromPostmultiplied<T>(in Matrix2x3<T> matrix, out Vector2<T> scale)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        var det = matrix.Determinant;
        var a = det < T.Zero ? T.NegativeOne : T.One;
        scale = new(
            x: T.Sqrt(matrix.A1 * matrix.A1 + matrix.B1 * matrix.B1) * a,
            y: T.Sqrt(matrix.A2 * matrix.A2 + matrix.B2 * matrix.B2)
            );
        }

    /// <summary><inheritdoc cref="GetScale{T}(in Matrix4{T}, out Vector4{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPremultiplicationConvention{T}"/>
    /// </remarks>
    [BunnyAttributes.SIMDCandidate]
    public static void GetScaleFromPremultiplied<T>(in Matrix4<T> matrix, out Vector4<T> scale)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        var det = matrix.Determinant;
        var a = det < T.Zero ? T.NegativeOne : T.One;
        scale = new(
            x: T.Sqrt(matrix.A1 * matrix.A1 + matrix.A2 * matrix.A2 + matrix.A3 * matrix.A3 + matrix.A4 * matrix.A4) * a,
            y: T.Sqrt(matrix.B1 * matrix.B1 + matrix.B2 * matrix.B2 + matrix.B3 * matrix.B3 + matrix.B4 * matrix.B4),
            z: T.Sqrt(matrix.C1 * matrix.C1 + matrix.C2 * matrix.C2 + matrix.C3 * matrix.C3 + matrix.C4 * matrix.C4),
            w: T.Sqrt(matrix.D1 * matrix.D1 + matrix.D2 * matrix.D2 + matrix.D3 * matrix.D3 + matrix.D4 * matrix.D4)
            );
        }

    /// <summary><inheritdoc cref="GetScale{T}(in Matrix3{T}, out Vector3{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPremultiplicationConvention{T}"/>
    /// </remarks>
    [BunnyAttributes.SIMDCandidate]
    public static void GetScaleFromPremultiplied<T>(in Matrix3<T> matrix, out Vector3<T> scale)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        var det = matrix.Determinant;
        var a = det < T.Zero ? T.NegativeOne : T.One;
        scale = new(
            x: T.Sqrt(matrix.A1 * matrix.A1 + matrix.A2 * matrix.A2 + matrix.A3 * matrix.A3) * a,
            y: T.Sqrt(matrix.B1 * matrix.B1 + matrix.B2 * matrix.B2 + matrix.B3 * matrix.B3),
            z: T.Sqrt(matrix.C1 * matrix.C1 + matrix.C2 * matrix.C2 + matrix.C3 * matrix.C3)
            );
        }

    /// <summary><inheritdoc cref="GetScale{T}(in Matrix2{T}, out Vector2{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPremultiplicationConvention{T}"/>
    /// </remarks>
    [BunnyAttributes.SIMDCandidate]
    public static void GetScaleFromPremultiplied<T>(in Matrix2<T> matrix, out Vector2<T> scale)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        var det = matrix.Determinant;
        var a = det < T.Zero ? T.NegativeOne : T.One;
        scale = new(
            x: T.Sqrt(matrix.A1 * matrix.A1 + matrix.A2 * matrix.A2) * a,
            y: T.Sqrt(matrix.B1 * matrix.B1 + matrix.B2 * matrix.B2)
            );
        }

    /// <summary><inheritdoc cref="GetScale{T}(in Matrix4{T}, out Vector3{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPremultiplicationConvention{T}"/>
    /// </remarks>
    [BunnyAttributes.SIMDCandidate]
    public static void GetScaleFromPremultiplied<T>(in Matrix4<T> matrix, out Vector3<T> scale)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        var det = matrix.Determinant;
        var a = det < T.Zero ? T.NegativeOne : T.One;
        scale = new(
            x: T.Sqrt(matrix.A1 * matrix.A1 + matrix.A2 * matrix.A2 + matrix.A3 * matrix.A3) * a,
            y: T.Sqrt(matrix.B1 * matrix.B1 + matrix.B2 * matrix.B2 + matrix.B3 * matrix.B3),
            z: T.Sqrt(matrix.C1 * matrix.C1 + matrix.C2 * matrix.C2 + matrix.C3 * matrix.C3)
            );
        }

    /// <summary><inheritdoc cref="GetScale{T}(in Matrix4{T}, out Vector3{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPremultiplicationConvention{T}"/>
    /// </remarks>
    [BunnyAttributes.SIMDCandidate]
    public static void GetScaleFromPremultiplied<T>(in Matrix4x3<T> matrix, out Vector3<T> scale)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        var det = matrix.Determinant;
        var a = det < T.Zero ? T.NegativeOne : T.One;
        scale = new(
            x: T.Sqrt(matrix.A1 * matrix.A1 + matrix.A2 * matrix.A2 + matrix.A3 * matrix.A3) * a,
            y: T.Sqrt(matrix.B1 * matrix.B1 + matrix.B2 * matrix.B2 + matrix.B3 * matrix.B3),
            z: T.Sqrt(matrix.C1 * matrix.C1 + matrix.C2 * matrix.C2 + matrix.C3 * matrix.C3)
            );
        }

    /// <summary><inheritdoc cref="GetScale{T}(in Matrix3{T}, out Vector2{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPremultiplicationConvention{T}"/>
    /// </remarks>
    [BunnyAttributes.SIMDCandidate]
    public static void GetScaleFromPremultiplied<T>(in Matrix3<T> matrix, out Vector2<T> scale)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        var det = matrix.Determinant;
        var a = det < T.Zero ? T.NegativeOne : T.One;
        scale = new(
            x: T.Sqrt(matrix.A1 * matrix.A1 + matrix.A2 * matrix.A2) * a,
            y: T.Sqrt(matrix.B1 * matrix.B1 + matrix.B2 * matrix.B2)
            );
        }

    /// <summary><inheritdoc cref="GetScale{T}(in Matrix3{T}, out Vector2{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPremultiplicationConvention{T}"/>
    /// </remarks>
    [BunnyAttributes.SIMDCandidate]
    public static void GetScaleFromPremultiplied<T>(in Matrix3x2<T> matrix, out Vector2<T> scale)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        var det = matrix.Determinant;
        var a = det < T.Zero ? T.NegativeOne : T.One;
        scale = new(
            x: T.Sqrt(matrix.A1 * matrix.A1 + matrix.A2 * matrix.A2) * a,
            y: T.Sqrt(matrix.B1 * matrix.B1 + matrix.B2 * matrix.B2)
            );
        }

    #endregion Extraction - Scale

    #region Extraction - Rotation

    /// <summary><inheritdoc cref="IMatrix{T}.IMatrixBase{Matrix, Vector, Direction, Rotation}.GetRotation(in Matrix)"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.MultiplicationConventionInstructions{T}"/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void GetRotation<T>(in Matrix2<T> matrix, out Angle<T> rotation)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
#if MATRIX_PREMULTIPLIED_CONVENTION
        GetRotationFromPremultiplied(in matrix, out rotation);
#elif MATRIX_POSTMULTIPLIED_CONVENTION
        GetRotationFromPostmultiplied(in matrix, out rotation);
#else
        throw new ApplicationException(Matrix.Docs.BaseMessage);
#endif
        }

    /// <summary><inheritdoc cref="IMatrix{T}.IMatrixBase{Matrix, Vector, Direction, Rotation}.GetRotation(in Matrix)"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.MultiplicationConventionInstructions{T}"/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void GetRotation<T>(in Matrix3<T> matrix, out Angle<T> rotation)
where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
#if MATRIX_PREMULTIPLIED_CONVENTION
        GetRotationFromPremultiplied(in matrix, out rotation);
#elif MATRIX_POSTMULTIPLIED_CONVENTION
        GetRotationFromPostmultiplied(in matrix, out rotation);
#else
        throw new ApplicationException(Matrix.Docs.BaseMessage);
#endif
        }

    /// <summary><inheritdoc cref="IMatrix{T}.IMatrixBase{Matrix, Vector, Direction, Rotation}.GetRotation(in Matrix)"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.MultiplicationConventionInstructions{T}"/>
    /// </remarks>
    public static void GetRotation<T>(in Matrix4<T> matrix, out Quaternion<T> rotation)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
#if MATRIX_PREMULTIPLIED_CONVENTION
        GetRotationFromPremultiplied(in matrix, out rotation);
#elif MATRIX_POSTMULTIPLIED_CONVENTION
        GetRotationFromPostmultiplied(in matrix, out rotation);
#else
        throw new ApplicationException(Matrix.Docs.BaseMessage);
#endif
        }

    /// <summary><inheritdoc cref="IMatrix{T}.IMatrixBase{Matrix, Vector, Direction, Rotation}.GetRotation(in Matrix)"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.MultiplicationConventionInstructions{T}"/>
    /// </remarks>
    public static void GetRotation<T>(in Matrix3<T> matrix, out Quaternion<T> rotation)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
#if MATRIX_PREMULTIPLIED_CONVENTION
        GetRotationFromPremultiplied(in matrix, out rotation);
#elif MATRIX_POSTMULTIPLIED_CONVENTION
        GetRotationFromPostmultiplied(in matrix, out rotation);
#else
        throw new ApplicationException(Matrix.Docs.BaseMessage);
#endif
        }

    /// <summary><inheritdoc cref="GetRotation{T}(in Matrix3{T}, out Angle{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPostmultiplicationConvention{T}"/>
    /// </remarks>
    public static void GetRotationFromPostmultiplied<T>(in Matrix3<T> matrix, out Angle<T> rotation)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        rotation = Angle<T>.FromRadiansUnchecked(T.Atan2(matrix.B1, matrix.A1));
        }

    /// <summary><inheritdoc cref="GetRotation{T}(in Matrix3{T}, out Angle{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPostmultiplicationConvention{T}"/>
    /// </remarks>
    public static void GetRotationFromPostmultiplied<T>(in Matrix2<T> matrix, out Angle<T> rotation)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        rotation = Angle<T>.FromRadiansUnchecked(T.Atan2(matrix.B1, matrix.A1));
        }

    /// <summary><inheritdoc cref="GetRotation{T}(in Matrix3{T}, out Angle{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPostmultiplicationConvention{T}"/>
    /// </remarks>
    public static void GetRotationFromPostmultiplied<T>(in Matrix2x3<T> matrix, out Angle<T> rotation)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        rotation = Angle<T>.FromRadiansUnchecked(T.Atan2(matrix.B1, matrix.A1));
        }

    /// <summary><inheritdoc cref="GetRotation{T}(in Matrix4{T}, out Quaternion{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPostmultiplicationConvention{T}"/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void GetRotationFromPostmultiplied<T>(in Matrix4<T> matrix, out Quaternion<T> rotation)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        var copy = matrix;
        Remove3DScaleFromPostmultiplied(ref copy);
        Matrix.CreateQuaternionComponentsFromPostmultipliedMatrix(in copy, out var X, out var Y, out var Z, out var W);
        rotation = new(X, Y, Z, W);
        }

    /// <summary><inheritdoc cref="GetRotation{T}(in Matrix4{T}, out Quaternion{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPostmultiplicationConvention{T}"/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void GetRotationFromPostmultiplied<T>(in Matrix3x4<T> matrix, out Quaternion<T> rotation)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        UnsafeConvert(in matrix, out Matrix4<T> copy);
        Remove3DScaleFromPostmultiplied(ref copy);
        Matrix.CreateQuaternionComponentsFromPostmultipliedMatrix(in copy, out var X, out var Y, out var Z, out var W);
        rotation = new(X, Y, Z, W);
        }

    /// <summary><inheritdoc cref="GetRotation{T}(in Matrix3{T}, out Quaternion{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPostmultiplicationConvention{T}"/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void GetRotationFromPostmultiplied<T>(in Matrix3<T> matrix, out Quaternion<T> rotation)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        UnsafeConvert(in matrix, out Matrix4<T> copy);
        Remove3DScaleFromPostmultiplied(ref copy);
        Matrix.CreateQuaternionComponentsFromPostmultipliedMatrix(in copy, out var X, out var Y, out var Z, out var W);
        rotation = new(X, Y, Z, W);
        }

    /// <summary><inheritdoc cref="GetRotation{T}(in Matrix3{T}, out Angle{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPremultiplicationConvention{T}"/>
    /// </remarks>
    public static void GetRotationFromPremultiplied<T>(in Matrix3<T> matrix, out Angle<T> rotation)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        rotation = Angle<T>.FromRadiansUnchecked(T.Atan2(matrix.A2, matrix.A1));
        }

    /// <summary><inheritdoc cref="GetRotation{T}(in Matrix3{T}, out Angle{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPremultiplicationConvention{T}"/>
    /// </remarks>
    public static void GetRotationFromPremultiplied<T>(in Matrix2<T> matrix, out Angle<T> rotation)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        rotation = Angle<T>.FromRadiansUnchecked(T.Atan2(matrix.A2, matrix.A1));
        }

    /// <summary><inheritdoc cref="GetRotation{T}(in Matrix3{T}, out Angle{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPremultiplicationConvention{T}"/>
    /// </remarks>
    public static void GetRotationFromPremultiplied<T>(in Matrix3x2<T> matrix, out Angle<T> rotation)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        rotation = Angle<T>.FromRadiansUnchecked(T.Atan2(matrix.A2, matrix.A1));
        }

    /// <summary><inheritdoc cref="GetRotation{T}(in Matrix4{T}, out Quaternion{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPremultiplicationConvention{T}"/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void GetRotationFromPremultiplied<T>(in Matrix4<T> matrix, out Quaternion<T> rotation)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        var copy = matrix;
        Remove3DScaleFromPremultiplied(ref copy);
        Matrix.CreateQuaternionComponentsFromPremultipliedMatrix(in copy, out var X, out var Y, out var Z, out var W);
        rotation = new(X, Y, Z, W);
        }

    /// <summary><inheritdoc cref="GetRotation{T}(in Matrix4{T}, out Quaternion{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPremultiplicationConvention{T}"/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void GetRotationFromPremultiplied<T>(in Matrix4x3<T> matrix, out Quaternion<T> rotation)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        UnsafeConvert(in matrix, out Matrix4<T> copy);
        Remove3DScaleFromPremultiplied(ref copy);
        Matrix.CreateQuaternionComponentsFromPremultipliedMatrix(in copy, out var X, out var Y, out var Z, out var W);
        rotation = new(X, Y, Z, W);
        }

    /// <summary><inheritdoc cref="GetRotation{T}(in Matrix3{T}, out Quaternion{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPremultiplicationConvention{T}"/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void GetRotationFromPremultiplied<T>(in Matrix3<T> matrix, out Quaternion<T> rotation)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        UnsafeConvert(in matrix, out Matrix4<T> copy);
        Remove3DScaleFromPremultiplied(ref copy);
        Matrix.CreateQuaternionComponentsFromPremultipliedMatrix(in copy, out var X, out var Y, out var Z, out var W);
        rotation = new(X, Y, Z, W);
        }

    #endregion Extraction - Rotation

    #region Component Removal - Translation

    /// <summary><inheritdoc cref="IMatrix{Numeric}.ITranslationMatrix{Matrix, Vector, Direction, Rotation}.RemoveTranslation(ref Matrix)"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.MultiplicationConventionInstructions{T}"/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Remove2DTranslation<T>(ref Matrix3<T> matrix)
    where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
#if MATRIX_PREMULTIPLIED_CONVENTION
        Remove2DTranslationFromPremultiplied(ref matrix);
#elif MATRIX_POSTMULTIPLIED_CONVENTION
        Remove2DTranslationFromPostmultiplied(ref matrix);
#else
        throw new ApplicationException(Matrix.Docs.BaseMessage);
#endif
        }

    /// <summary><inheritdoc cref="IMatrix{Numeric}.ITranslationMatrix{Matrix, Vector, Direction, Rotation}.RemoveTranslation(ref Matrix)"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.MultiplicationConventionInstructions{T}"/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Remove2DTranslation<T>(ref Matrix3x2<T> matrix)
    where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
#if MATRIX_PREMULTIPLIED_CONVENTION
        Remove2DTranslationFromPremultiplied(ref matrix);
#elif MATRIX_POSTMULTIPLIED_CONVENTION
        // do nothing
#else
        throw new ApplicationException(Matrix.Docs.BaseMessage);
#endif
        }

    /// <summary><inheritdoc cref="IMatrix{Numeric}.ITranslationMatrix{Matrix, Vector, Direction, Rotation}.RemoveTranslation(ref Matrix)"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.MultiplicationConventionInstructions{T}"/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Remove2DTranslation<T>(ref Matrix2x3<T> matrix)
    where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
#if MATRIX_PREMULTIPLIED_CONVENTION
        // do nothing
#elif MATRIX_POSTMULTIPLIED_CONVENTION
        Remove2DTranslationFromPostmultiplied(ref matrix);
#else
        throw new ApplicationException(Matrix.Docs.BaseMessage);
#endif
        }

    /// <summary><inheritdoc cref="IMatrix{Numeric}.ITranslationMatrix{Matrix, Vector, Direction, Rotation}.RemoveTranslation(ref Matrix)"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPostmultiplicationConvention{T}"/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Remove2DTranslationFromPostmultiplied<T>(ref Matrix2x3<T> matrix)
            where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        matrix.A3 = T.Zero; matrix.B3 = T.Zero;
        }

    /// <summary><inheritdoc cref="IMatrix{Numeric}.ITranslationMatrix{Matrix, Vector, Direction, Rotation}.RemoveTranslation(ref Matrix)"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPostmultiplicationConvention{T}"/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Remove2DTranslationFromPostmultiplied<T>(ref Matrix3<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        matrix.A3 = T.Zero; matrix.B3 = T.Zero; matrix.C3 = T.One;
        }

    /// <summary><inheritdoc cref="IMatrix{Numeric}.ITranslationMatrix{Matrix, Vector, Direction, Rotation}.RemoveTranslation(ref Matrix)"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPremultiplicationConvention{T}"/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Remove2DTranslationFromPremultiplied<T>(ref Matrix3x2<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        matrix.C1 = T.Zero; matrix.C2 = T.Zero;
        }

    /// <summary><inheritdoc cref="IMatrix{Numeric}.ITranslationMatrix{Matrix, Vector, Direction, Rotation}.RemoveTranslation(ref Matrix)"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPremultiplicationConvention{T}"/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Remove2DTranslationFromPremultiplied<T>(ref Matrix3<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        matrix.C1 = T.Zero; matrix.C2 = T.Zero; matrix.C3 = T.One;
        }

    /// <summary><inheritdoc cref="IMatrix{Numeric}.ITranslationMatrix{Matrix, Vector, Direction, Rotation}.RemoveTranslation(ref Matrix)"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.MultiplicationConventionInstructions{T}"/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Remove3DTranslation<T>(ref Matrix4<T> matrix)
    where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
#if MATRIX_PREMULTIPLIED_CONVENTION
        Remove3DTranslationFromPremultiplied(ref matrix);
#elif MATRIX_POSTMULTIPLIED_CONVENTION
        Remove3DTranslationFromPostmultiplied(ref matrix);
#else
        throw new ApplicationException(Matrix.Docs.BaseMessage);
#endif
        }

    /// <summary><inheritdoc cref="IMatrix{Numeric}.ITranslationMatrix{Matrix, Vector, Direction, Rotation}.RemoveTranslation(ref Matrix)"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.MultiplicationConventionInstructions{T}"/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Remove3DTranslation<T>(ref Matrix4x3<T> matrix)
    where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
#if MATRIX_PREMULTIPLIED_CONVENTION
        Remove3DTranslationFromPremultiplied(ref matrix);
#elif MATRIX_POSTMULTIPLIED_CONVENTION
        // do nothing
#else
        throw new ApplicationException(Matrix.Docs.BaseMessage);
#endif
        }

    /// <summary><inheritdoc cref="IMatrix{Numeric}.ITranslationMatrix{Matrix, Vector, Direction, Rotation}.RemoveTranslation(ref Matrix)"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.MultiplicationConventionInstructions{T}"/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Remove3DTranslation<T>(ref Matrix3x4<T> matrix)
    where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
#if MATRIX_PREMULTIPLIED_CONVENTION
        // do nothing
#elif MATRIX_POSTMULTIPLIED_CONVENTION
        Remove3DTranslationFromPostmultiplied(ref matrix);
#else
        throw new ApplicationException(Matrix.Docs.BaseMessage);
#endif
        }

    /// <summary><inheritdoc cref="IMatrix{Numeric}.ITranslationMatrix{Matrix, Vector, Direction, Rotation}.RemoveTranslation(ref Matrix)"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPostmultiplicationConvention{T}"/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Remove3DTranslationFromPostmultiplied<T>(ref Matrix3x4<T> matrix)
    where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        matrix.A4 = T.Zero; matrix.B4 = T.Zero; matrix.C4 = T.Zero;
        }

    /// <summary><inheritdoc cref="IMatrix{Numeric}.ITranslationMatrix{Matrix, Vector, Direction, Rotation}.RemoveTranslation(ref Matrix)"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPostmultiplicationConvention{T}"/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Remove3DTranslationFromPostmultiplied<T>(ref Matrix4<T> matrix)
    where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        matrix.A4 = T.Zero; matrix.B4 = T.Zero; matrix.C4 = T.Zero; matrix.D4 = T.One;
        }

    /// <summary><inheritdoc cref="IMatrix{Numeric}.ITranslationMatrix{Matrix, Vector, Direction, Rotation}.RemoveTranslation(ref Matrix)"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPremultiplicationConvention{T}"/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Remove3DTranslationFromPremultiplied<T>(ref Matrix4x3<T> matrix)
    where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        matrix.D1 = T.Zero; matrix.D2 = T.Zero; matrix.D3 = T.Zero;
        }

    /// <summary><inheritdoc cref="IMatrix{Numeric}.ITranslationMatrix{Matrix, Vector, Direction, Rotation}.RemoveTranslation(ref Matrix)"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPremultiplicationConvention{T}"/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Remove3DTranslationFromPremultiplied<T>(ref Matrix4<T> matrix)
    where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        matrix.D1 = T.Zero; matrix.D2 = T.Zero; matrix.D3 = T.Zero; matrix.D4 = T.One;
        }

    #endregion Component Removal - Translation

    #region Component Removal - Scale

    /// <summary><inheritdoc cref="IMatrix{Numeric}.I2DMatrix{Matrix}.Remove2DScale(ref Matrix)"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.MultiplicationConventionInstructions{T}"/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Remove2DScale<T>(ref Matrix3x2<T> matrix)
    where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
#if MATRIX_PREMULTIPLIED_CONVENTION
        Remove2DScaleFromPremultiplied(ref matrix);
#elif MATRIX_POSTMULTIPLIED_CONVENTION
        Remove2DScaleFromPostmultiplied(ref matrix);
#else
        throw new ApplicationException(Matrix.Docs.BaseMessage);
#endif
        }

    /// <inheritdoc
    /// cref="Remove2DScale{T}(ref Matrix3x2{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Remove2DScale<T>(ref Matrix2x3<T> matrix)
    where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
#if MATRIX_PREMULTIPLIED_CONVENTION
        Remove2DScaleFromPremultiplied(ref matrix);
#elif MATRIX_POSTMULTIPLIED_CONVENTION
        Remove2DScaleFromPostmultiplied(ref matrix);
#else
        throw new ApplicationException(Matrix.Docs.BaseMessage);
#endif
        }

    /// <inheritdoc
    /// cref="Remove2DScale{T}(ref Matrix3x2{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Remove2DScale<T>(ref Matrix3<T> matrix)
    where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
#if MATRIX_PREMULTIPLIED_CONVENTION
        Remove2DScaleFromPremultiplied(ref matrix);
#elif MATRIX_POSTMULTIPLIED_CONVENTION
        Remove2DScaleFromPostmultiplied(ref matrix);
#else
        throw new ApplicationException(Matrix.Docs.BaseMessage);
#endif
        }

    /// <inheritdoc
    /// cref="Remove2DScale{T}(ref Matrix3x2{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Remove2DScale<T>(ref Matrix2<T> matrix)
    where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
#if MATRIX_PREMULTIPLIED_CONVENTION
        Remove2DScaleFromPremultiplied(ref matrix);
#elif MATRIX_POSTMULTIPLIED_CONVENTION
        Remove2DScaleFromPostmultiplied(ref matrix);
#else
        throw new ApplicationException(Matrix.Docs.BaseMessage);
#endif
        }

    /// <summary><inheritdoc cref="Remove2DScale{T}(ref Matrix3x2{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPostmultiplicationConvention{T}"/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Remove2DScaleFromPostmultiplied<T>(ref Matrix3<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        UnsafeConvert(in matrix, out Matrix2x3<T> m);
        Remove2DScaleFromPostmultiplied(ref m);
        UnsafeConvert(in m, out matrix);
        }

    /// <summary><inheritdoc cref="Remove2DScale{T}(ref Matrix3x2{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPostmultiplicationConvention{T}"/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Remove2DScaleFromPostmultiplied<T>(ref Matrix2<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        UnsafeConvert(in matrix, out Matrix3x2<T> m);
        Remove2DScaleFromPostmultiplied(ref m);
        UnsafeConvert(in m, out matrix);
        }

    /// <summary><inheritdoc cref="Remove2DScale{T}(ref Matrix3x2{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPostmultiplicationConvention{T}"/>
    /// </remarks>
    [BunnyAttributes.SIMDCandidate]
    public static void Remove2DScaleFromPostmultiplied<T>(ref Matrix3x2<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        var rcpA = T.One / T.Sqrt(matrix.A1 * matrix.A1 + matrix.B1 * matrix.B1);
        var rcpB = T.One / T.Sqrt(matrix.A2 * matrix.A2 + matrix.B2 * matrix.B2);
        matrix = new(
            matrix.A1 * rcpA, matrix.A2 * rcpB,
            matrix.B1 * rcpA, matrix.B2 * rcpB,
            matrix.C1, matrix.C2
            );
        }

    /// <summary><inheritdoc cref="Remove2DScale{T}(ref Matrix3x2{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPostmultiplicationConvention{T}"/>
    /// </remarks>
    [BunnyAttributes.SIMDCandidate]
    public static void Remove2DScaleFromPostmultiplied<T>(ref Matrix2x3<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        var rcpA = T.One / T.Sqrt(matrix.A1 * matrix.A1 + matrix.B1 * matrix.B1);
        var rcpB = T.One / T.Sqrt(matrix.A2 * matrix.A2 + matrix.B2 * matrix.B2);
        matrix = new(
            matrix.A1 * rcpA, matrix.A2 * rcpB, matrix.A3,
            matrix.B1 * rcpA, matrix.B2 * rcpB, matrix.B3
            );
        }

    /// <summary><inheritdoc cref="Remove2DScale{T}(ref Matrix3x2{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPremultiplicationConvention{T}"/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Remove2DScaleFromPremultiplied<T>(ref Matrix3<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        UnsafeConvert(in matrix, out Matrix3x2<T> m);
        Remove2DScaleFromPremultiplied(ref m);
        UnsafeConvert(in m, out matrix);
        }

    /// <summary><inheritdoc cref="Remove2DScale{T}(ref Matrix3x2{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPremultiplicationConvention{T}"/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Remove2DScaleFromPremultiplied<T>(ref Matrix2<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        UnsafeConvert(in matrix, out Matrix3x2<T> m);
        Remove2DScaleFromPremultiplied(ref m);
        UnsafeConvert(in m, out matrix);
        }

    /// <summary><inheritdoc cref="Remove2DScale{T}(ref Matrix3x2{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPremultiplicationConvention{T}"/>
    /// </remarks>
    [BunnyAttributes.SIMDCandidate]
    public static void Remove2DScaleFromPremultiplied<T>(ref Matrix3x2<T> matrix)
    where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        var rcpA = T.One / T.Sqrt(matrix.A1 * matrix.A1 + matrix.A2 * matrix.A2);
        var rcpB = T.One / T.Sqrt(matrix.B1 * matrix.B1 + matrix.B2 * matrix.B2);
        matrix = new(
            matrix.A1 * rcpA, matrix.A2 * rcpA,
            matrix.B1 * rcpB, matrix.B2 * rcpB,
            matrix.C1, matrix.C2
            );
        }

    /// <summary><inheritdoc cref="Remove2DScale{T}(ref Matrix3x2{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPremultiplicationConvention{T}"/>
    /// </remarks>
    [BunnyAttributes.SIMDCandidate]
    public static void Remove2DScaleFromPremultiplied<T>(ref Matrix2x3<T> matrix)
    where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        var rcpA = T.One / T.Sqrt(matrix.A1 * matrix.A1 + matrix.A2 * matrix.A2);
        var rcpB = T.One / T.Sqrt(matrix.B1 * matrix.B1 + matrix.B2 * matrix.B2);
        matrix = new(
            matrix.A1 * rcpA, matrix.A2 * rcpA, matrix.A3,
            matrix.B1 * rcpB, matrix.B2 * rcpB, matrix.B3
            );
        }

    /// <summary><inheritdoc cref="IMatrix{Numeric}.I3DMatrix{Matrix}.Remove3DScale(ref Matrix)"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.MultiplicationConventionInstructions{T}"/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Remove3DScale<T>(ref Matrix3x4<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
#if MATRIX_PREMULTIPLIED_CONVENTION
        Remove3DScaleFromPremultiplied(ref matrix);
#elif MATRIX_POSTMULTIPLIED_CONVENTION
        Remove3DScaleFromPostmultiplied(ref matrix);
#else
        throw new ApplicationException(Matrix.Docs.BaseMessage);
#endif
        }

    /// <summary><inheritdoc cref="Remove3DScale{T}(ref Matrix3x4{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.MultiplicationConventionInstructions{T}"/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Remove3DScale<T>(ref Matrix4<T> matrix)
    where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
#if MATRIX_PREMULTIPLIED_CONVENTION
        Remove3DScaleFromPremultiplied(ref matrix);
#elif MATRIX_POSTMULTIPLIED_CONVENTION
        Remove3DScaleFromPostmultiplied(ref matrix);
#else
        throw new ApplicationException(Matrix.Docs.BaseMessage);
#endif
        }

    /// <summary><inheritdoc cref="Remove3DScale{T}(ref Matrix3x4{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.MultiplicationConventionInstructions{T}"/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Remove3DScale<T>(ref Matrix4x3<T> matrix)
    where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
#if MATRIX_PREMULTIPLIED_CONVENTION
        Remove3DScaleFromPremultiplied(ref matrix);
#elif MATRIX_POSTMULTIPLIED_CONVENTION
        Remove3DScaleFromPostmultiplied(ref matrix);
#else
        throw new ApplicationException(Matrix.Docs.BaseMessage);
#endif
        }

    /// <summary><inheritdoc cref="Remove3DScale{T}(ref Matrix3x4{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.MultiplicationConventionInstructions{T}"/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Remove3DScale<T>(ref Matrix3<T> matrix)
    where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
#if MATRIX_PREMULTIPLIED_CONVENTION
        Remove3DScaleFromPremultiplied(ref matrix);
#elif MATRIX_POSTMULTIPLIED_CONVENTION
        Remove3DScaleFromPostmultiplied(ref matrix);
#else
        throw new ApplicationException(Matrix.Docs.BaseMessage);
#endif
        }

    /// <summary><inheritdoc cref="Remove3DScale{T}(ref Matrix3x4{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPostmultiplicationConvention{T}"/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Remove3DScaleFromPostmultiplied<T>(ref Matrix3x4<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        if (Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double)) {
            ref var m = ref Unsafe.As<Matrix3x4<T>, Vector256<double>>(ref matrix);

            var a = m;
            var b = Unsafe.Add(ref m, 1);
            var c = Unsafe.Add(ref m, 2);

            var rcp = Vector256<double>.One / Vector256.Sqrt(a * a + b * b + c * c).WithElement(3, 1d);

            m = a * rcp;
            Unsafe.Add(ref m, 1) = b * rcp;
            Unsafe.Add(ref m, 2) = c * rcp;
            }
        else if (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float)) {
            ref var m = ref Unsafe.As<Matrix3x4<T>, Vector128<float>>(ref matrix);

            var a = m;
            var b = Unsafe.Add(ref m, 1);
            var c = Unsafe.Add(ref m, 2);

            var rcp = Vector128<float>.One / Vector128.Sqrt(a * a + b * b + c * c).WithElement(3, 1f);

            m = a * rcp;
            Unsafe.Add(ref m, 1) = b * rcp;
            Unsafe.Add(ref m, 2) = c * rcp;
            }
        else {
            var rcpA = T.One / T.Sqrt(matrix.A1 * matrix.A1 + matrix.B1 * matrix.B1 + matrix.C1 * matrix.C1);
            var rcpB = T.One / T.Sqrt(matrix.A2 * matrix.A2 + matrix.B2 * matrix.B2 + matrix.C2 * matrix.C2);
            var rcpC = T.One / T.Sqrt(matrix.A3 * matrix.A3 + matrix.B3 * matrix.B3 + matrix.C3 * matrix.C3);
            matrix = new(
                matrix.A1 * rcpA, matrix.A2 * rcpB, matrix.A3 * rcpC, matrix.A4,
                matrix.B1 * rcpA, matrix.B2 * rcpB, matrix.B3 * rcpC, matrix.B4,
                matrix.C1 * rcpA, matrix.C2 * rcpB, matrix.C3 * rcpC, matrix.C4
                );
            }
        }

    /// <summary><inheritdoc cref="Remove3DScale{T}(ref Matrix3x4{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPostmultiplicationConvention{T}"/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Remove3DScaleFromPostmultiplied<T>(ref Matrix4x3<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        Remove3DScaleFromPostmultiplied(ref Unsafe.As<Matrix4x3<T>, Matrix3<T>>(ref matrix));
        }

    /// <summary><inheritdoc cref="Remove3DScale{T}(ref Matrix3x4{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPostmultiplicationConvention{T}"/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Remove3DScaleFromPostmultiplied<T>(ref Matrix3<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        UnsafeConvert(in matrix, out Matrix3x4<T> m);
        Remove3DScaleFromPostmultiplied(ref m);
        UnsafeConvert(in m, out matrix);
        }

    /// <summary><inheritdoc cref="Remove3DScale{T}(ref Matrix3x4{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPostmultiplicationConvention{T}"/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Remove3DScaleFromPostmultiplied<T>(ref Matrix4<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        Remove3DScaleFromPostmultiplied(ref Unsafe.As<Matrix4<T>, Matrix3x4<T>>(ref matrix));
        }

    /// <summary><inheritdoc cref="Remove3DScale{T}(ref Matrix3x4{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPremultiplicationConvention{T}"/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Remove3DScaleFromPremultiplied<T>(ref Matrix3x4<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        if (Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double)) {
            var mask = Vector256.Create(~0, ~0, ~0, 0).AsDouble();
            ref var m = ref Unsafe.As<Matrix3x4<T>, Vector256<double>>(ref matrix);

            var a = m & mask;
            var b = Unsafe.Add(ref m, 1) & mask;
            var c = Unsafe.Add(ref m, 2) & mask;

            var rcp = Vector256<double>.One / Vector256.Sqrt(Vector256.Create(
                Vector256.Sum(a * a),
                Vector256.Sum(b * b),
                Vector256.Sum(c * c),
                1d
                ));

            m = a * SplatX(rcp);
            Unsafe.Add(ref m, 1) = b * SplatY(rcp);
            Unsafe.Add(ref m, 2) = c * SplatZ(rcp);
            }
        else if (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float)) {
            var mask = Vector128.Create(~0, ~0, ~0, 0).AsSingle();
            ref var m = ref Unsafe.As<Matrix3x4<T>, Vector128<float>>(ref matrix);

            var a = m & mask;
            var b = Unsafe.Add(ref m, 1) & mask;
            var c = Unsafe.Add(ref m, 2) & mask;

            var rcp = Vector128<float>.One / Vector128.Sqrt(Vector128.Create(
                Vector128.Sum(a * a),
                Vector128.Sum(b * b),
                Vector128.Sum(c * c),
                1f
                ));

            m = a * SplatX(rcp);
            Unsafe.Add(ref m, 1) = b * SplatY(rcp);
            Unsafe.Add(ref m, 2) = c * SplatZ(rcp);
            }
        else {
            var rcpA = T.One / T.Sqrt(matrix.A1 * matrix.A1 + matrix.A2 * matrix.A2 + matrix.A3 * matrix.A3);
            var rcpB = T.One / T.Sqrt(matrix.B1 * matrix.B1 + matrix.B2 * matrix.B2 + matrix.B3 * matrix.B3);
            var rcpC = T.One / T.Sqrt(matrix.C1 * matrix.C1 + matrix.C2 * matrix.C2 + matrix.C3 * matrix.C3);
            matrix = new(
                matrix.A1 * rcpA, matrix.A2 * rcpA, matrix.A3 * rcpA, T.Zero,
                matrix.B1 * rcpB, matrix.B2 * rcpB, matrix.B3 * rcpB, T.Zero,
                matrix.C1 * rcpC, matrix.C2 * rcpC, matrix.C3 * rcpC, T.Zero
                );
            }
        }

    /// <summary><inheritdoc cref="Remove3DScale{T}(ref Matrix3x4{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPremultiplicationConvention{T}"/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Remove3DScaleFromPremultiplied<T>(ref Matrix3<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        UnsafeConvert(in matrix, out Matrix3x4<T> m);
        Remove3DScaleFromPremultiplied(ref m);
        UnsafeConvert(in m, out matrix);
        }

    /// <summary><inheritdoc cref="Remove3DScale{T}(ref Matrix3x4{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPremultiplicationConvention{T}"/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Remove3DScaleFromPremultiplied<T>(ref Matrix4x3<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        Remove3DScaleFromPremultiplied(ref Unsafe.As<Matrix4x3<T>, Matrix3<T>>(ref matrix));
        }

    /// <summary><inheritdoc cref="Remove3DScale{T}(ref Matrix3x4{T})"/>
    /// </summary>
    /// <remarks><inheritdoc cref="Docs.UsesPremultiplicationConvention{T}"/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Remove3DScaleFromPremultiplied<T>(ref Matrix4<T> matrix)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        Remove3DScaleFromPremultiplied(ref Unsafe.As<Matrix4<T>, Matrix3x4<T>>(ref matrix));
        }

    #endregion Component Removal - Scale

    #region Conversion

    public static unsafe void UnsafeConvert<T>(in Matrix2<T> matrix, out Matrix3<T> ret)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        ret = default;
        var bytes = (uint)(sizeof(T) * 2);
        Unsafe.CopyBlockUnaligned(
            ref Unsafe.As<Vector3<T>, byte>(ref Matrix3<T>.GetRowA(ref ret)),
            ref Unsafe.As<Vector2<T>, byte>(ref Matrix2<T>.GetRowA(ref Unsafe.AsRef(in matrix))),
            bytes);
        Unsafe.CopyBlockUnaligned(
            ref Unsafe.As<Vector3<T>, byte>(ref Matrix3<T>.GetRowB(ref ret)),
            ref Unsafe.As<Vector2<T>, byte>(ref Matrix2<T>.GetRowB(ref Unsafe.AsRef(in matrix))),
            bytes);
        ret.C3 = T.One;
        }

    public static unsafe void UnsafeConvert<T>(in Matrix3<T> matrix, out Matrix2<T> ret)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        ret = default;
        var bytes = (uint)(sizeof(T) * 2);
        Unsafe.CopyBlockUnaligned(
            ref Unsafe.As<Vector2<T>, byte>(ref Matrix2<T>.GetRowA(ref ret)),
            ref Unsafe.As<Vector3<T>, byte>(ref Matrix3<T>.GetRowA(ref Unsafe.AsRef(in matrix))),
            bytes);
        Unsafe.CopyBlockUnaligned(
            ref Unsafe.As<Vector2<T>, byte>(ref Matrix2<T>.GetRowB(ref ret)),
            ref Unsafe.As<Vector3<T>, byte>(ref Matrix3<T>.GetRowB(ref Unsafe.AsRef(in matrix))),
            bytes);
        }

    public static unsafe void UnsafeConvert<T>(in Matrix2<T> matrix, out Matrix2x3<T> ret)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        ret = default;
        var bytes = (uint)(sizeof(T) * 2);
        Unsafe.CopyBlockUnaligned(
            ref Unsafe.As<Vector3<T>, byte>(ref Matrix2x3<T>.GetRowA(ref ret)),
            ref Unsafe.As<Vector2<T>, byte>(ref Matrix2<T>.GetRowA(ref Unsafe.AsRef(in matrix))),
            bytes);
        Unsafe.CopyBlockUnaligned(
            ref Unsafe.As<Vector3<T>, byte>(ref Matrix2x3<T>.GetRowB(ref ret)),
            ref Unsafe.As<Vector2<T>, byte>(ref Matrix2<T>.GetRowB(ref Unsafe.AsRef(in matrix))),
            bytes);
        }

    public static unsafe void UnsafeConvert<T>(in Matrix2x3<T> matrix, out Matrix3<T> ret)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        ret = default;
        var bytes = (uint)(sizeof(T) * 6);
        Unsafe.CopyBlockUnaligned(
            ref Unsafe.As<Matrix3<T>, byte>(ref ret),
            ref Unsafe.As<Matrix2x3<T>, byte>(ref Unsafe.AsRef(in matrix)),
            bytes);
        ret.C3 = T.One;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void UnsafeConvert<T>(in Matrix3<T> matrix, out Matrix2x3<T> ret)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        ret = Unsafe.As<Matrix3<T>, Matrix2x3<T>>(ref Unsafe.AsRef(in matrix));
        }

    public static unsafe void UnsafeConvert<T>(in Matrix2x3<T> matrix, out Matrix2<T> ret)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        ret = default;
        var bytes = (uint)(sizeof(T) * 2);
        Unsafe.CopyBlockUnaligned(
            ref Unsafe.As<Vector2<T>, byte>(ref Matrix2<T>.GetRowA(ref ret)),
            ref Unsafe.As<Vector3<T>, byte>(ref Matrix2x3<T>.GetRowA(ref Unsafe.AsRef(in matrix))),
            bytes);
        Unsafe.CopyBlockUnaligned(
            ref Unsafe.As<Vector2<T>, byte>(ref Matrix2<T>.GetRowB(ref ret)),
            ref Unsafe.As<Vector3<T>, byte>(ref Matrix2x3<T>.GetRowB(ref Unsafe.AsRef(in matrix))),
            bytes);
        }

    public static unsafe void UnsafeConvert<T>(in Matrix3<T> matrix, out Matrix3x4<T> ret)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        ret = default;
        var bytes = (uint)(sizeof(T) * 3);
        Unsafe.CopyBlockUnaligned(
            ref Unsafe.As<Vector4<T>, byte>(ref Matrix3x4<T>.GetRowA(ref ret)),
            ref Unsafe.As<Vector3<T>, byte>(ref Matrix3<T>.GetRowA(ref Unsafe.AsRef(in matrix))),
            bytes);
        Unsafe.CopyBlockUnaligned(
            ref Unsafe.As<Vector4<T>, byte>(ref Matrix3x4<T>.GetRowB(ref ret)),
            ref Unsafe.As<Vector3<T>, byte>(ref Matrix3<T>.GetRowB(ref Unsafe.AsRef(in matrix))),
            bytes);
        Unsafe.CopyBlockUnaligned(
            ref Unsafe.As<Vector4<T>, byte>(ref Matrix3x4<T>.GetRowC(ref ret)),
            ref Unsafe.As<Vector3<T>, byte>(ref Matrix3<T>.GetRowC(ref Unsafe.AsRef(in matrix))),
            bytes);
        }

    public static unsafe void UnsafeConvert<T>(in Matrix3<T> matrix, out Matrix4<T> ret)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        ret = default;
        var bytes = (uint)(sizeof(T) * 3);
        Unsafe.CopyBlockUnaligned(
            ref Unsafe.As<Vector4<T>, byte>(ref Matrix4<T>.GetRowA(ref ret)),
            ref Unsafe.As<Vector3<T>, byte>(ref Matrix3<T>.GetRowA(ref Unsafe.AsRef(in matrix))),
            bytes);
        Unsafe.CopyBlockUnaligned(
            ref Unsafe.As<Vector4<T>, byte>(ref Matrix4<T>.GetRowB(ref ret)),
            ref Unsafe.As<Vector3<T>, byte>(ref Matrix3<T>.GetRowB(ref Unsafe.AsRef(in matrix))),
            bytes);
        Unsafe.CopyBlockUnaligned(
            ref Unsafe.As<Vector4<T>, byte>(ref Matrix4<T>.GetRowC(ref ret)),
            ref Unsafe.As<Vector3<T>, byte>(ref Matrix3<T>.GetRowC(ref Unsafe.AsRef(in matrix))),
            bytes);
        ret.D4 = T.One;
        }

    public static unsafe void UnsafeConvert<T>(in Matrix3x4<T> matrix, out Matrix4<T> ret)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        ret = default;
        var bytes = (uint)(sizeof(T) * 12);
        Unsafe.CopyBlockUnaligned(
            ref Unsafe.As<Matrix4<T>, byte>(ref ret),
            ref Unsafe.As<Matrix3x4<T>, byte>(ref Unsafe.AsRef(in matrix)),
            bytes);
        ret.D4 = T.One;
        }

    public static unsafe void UnsafeConvert<T>(in Matrix3x4<T> matrix, out Matrix3<T> ret)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        ret = default;
        var bytes = (uint)(sizeof(T) * 3);
        Unsafe.CopyBlockUnaligned(
            ref Unsafe.As<Vector3<T>, byte>(ref Matrix3<T>.GetRowA(ref ret)),
            ref Unsafe.As<Vector4<T>, byte>(ref Matrix3x4<T>.GetRowA(ref Unsafe.AsRef(in matrix))),
            bytes);
        Unsafe.CopyBlockUnaligned(
            ref Unsafe.As<Vector3<T>, byte>(ref Matrix3<T>.GetRowB(ref ret)),
            ref Unsafe.As<Vector4<T>, byte>(ref Matrix3x4<T>.GetRowB(ref Unsafe.AsRef(in matrix))),
            bytes);
        Unsafe.CopyBlockUnaligned(
            ref Unsafe.As<Vector3<T>, byte>(ref Matrix3<T>.GetRowC(ref ret)),
            ref Unsafe.As<Vector4<T>, byte>(ref Matrix3x4<T>.GetRowC(ref Unsafe.AsRef(in matrix))),
            bytes);
        }

    public static unsafe void UnsafeConvert<T>(in Matrix4x3<T> matrix, out Matrix4<T> ret)
    where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        ret = default;
        var bytes = (uint)(sizeof(T) * 3);
        Unsafe.CopyBlockUnaligned(
            ref Unsafe.As<Vector4<T>, byte>(ref Matrix4<T>.GetRowA(ref ret)),
            ref Unsafe.As<Vector3<T>, byte>(ref Matrix4x3<T>.GetRowA(ref Unsafe.AsRef(in matrix))),
            bytes);
        Unsafe.CopyBlockUnaligned(
            ref Unsafe.As<Vector4<T>, byte>(ref Matrix4<T>.GetRowB(ref ret)),
            ref Unsafe.As<Vector3<T>, byte>(ref Matrix4x3<T>.GetRowB(ref Unsafe.AsRef(in matrix))),
            bytes);
        Unsafe.CopyBlockUnaligned(
            ref Unsafe.As<Vector4<T>, byte>(ref Matrix4<T>.GetRowC(ref ret)),
            ref Unsafe.As<Vector3<T>, byte>(ref Matrix4x3<T>.GetRowC(ref Unsafe.AsRef(in matrix))),
            bytes);
        Unsafe.CopyBlockUnaligned(
            ref Unsafe.As<Vector4<T>, byte>(ref Matrix4<T>.GetRowD(ref ret)),
            ref Unsafe.As<Vector3<T>, byte>(ref Matrix4x3<T>.GetRowD(ref Unsafe.AsRef(in matrix))),
            bytes);
        ret.D4 = T.One;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void UnsafeConvert<T>(in Matrix4x3<T> matrix, out Matrix3<T> ret)
    where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        ret = Unsafe.As<Matrix4x3<T>, Matrix3<T>>(ref Unsafe.AsRef(in matrix));
        }

    public static unsafe void UnsafeConvert<T>(in Matrix3<T> matrix, out Matrix4x3<T> ret)
    where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        ret = default;
        var bytes = (uint)(sizeof(T) * 9);
        Unsafe.CopyBlockUnaligned(
            ref Unsafe.As<Matrix4x3<T>, byte>(ref ret),
            ref Unsafe.As<Matrix3<T>, byte>(ref Unsafe.AsRef(in matrix)),
            bytes);
        }

    public static unsafe void UnsafeConvert<T>(in Matrix4<T> matrix, out Matrix4x3<T> ret)
    where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        ret = default;
        var bytes = (uint)(sizeof(T) * 3);
        Unsafe.CopyBlockUnaligned(
            ref Unsafe.As<Vector3<T>, byte>(ref Matrix4x3<T>.GetRowA(ref ret)),
            ref Unsafe.As<Vector4<T>, byte>(ref Matrix4<T>.GetRowA(ref Unsafe.AsRef(in matrix))),
            bytes);
        Unsafe.CopyBlockUnaligned(
            ref Unsafe.As<Vector3<T>, byte>(ref Matrix4x3<T>.GetRowB(ref ret)),
            ref Unsafe.As<Vector4<T>, byte>(ref Matrix4<T>.GetRowB(ref Unsafe.AsRef(in matrix))),
            bytes);
        Unsafe.CopyBlockUnaligned(
            ref Unsafe.As<Vector3<T>, byte>(ref Matrix4x3<T>.GetRowC(ref ret)),
            ref Unsafe.As<Vector4<T>, byte>(ref Matrix4<T>.GetRowC(ref Unsafe.AsRef(in matrix))),
            bytes);
        Unsafe.CopyBlockUnaligned(
            ref Unsafe.As<Vector3<T>, byte>(ref Matrix4x3<T>.GetRowD(ref ret)),
            ref Unsafe.As<Vector4<T>, byte>(ref Matrix4<T>.GetRowD(ref Unsafe.AsRef(in matrix))),
            bytes);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void UnsafeConvert<T>(in Matrix4<T> matrix, out Matrix3x4<T> ret)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        ret = Unsafe.As<Matrix4<T>, Matrix3x4<T>>(ref Unsafe.AsRef(in matrix));
        }

    public static unsafe void UnsafeConvert<T>(in Matrix4<T> matrix, out Matrix3<T> ret)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        ret = default;
        var bytes = (uint)(sizeof(T) * 3);
        Unsafe.CopyBlockUnaligned(
            ref Unsafe.As<Vector3<T>, byte>(ref Matrix3<T>.GetRowA(ref ret)),
            ref Unsafe.As<Vector4<T>, byte>(ref Matrix4<T>.GetRowA(ref Unsafe.AsRef(in matrix))),
            bytes);
        Unsafe.CopyBlockUnaligned(
            ref Unsafe.As<Vector3<T>, byte>(ref Matrix3<T>.GetRowB(ref ret)),
            ref Unsafe.As<Vector4<T>, byte>(ref Matrix4<T>.GetRowB(ref Unsafe.AsRef(in matrix))),
            bytes);
        Unsafe.CopyBlockUnaligned(
            ref Unsafe.As<Vector3<T>, byte>(ref Matrix3<T>.GetRowC(ref ret)),
            ref Unsafe.As<Vector4<T>, byte>(ref Matrix4<T>.GetRowC(ref Unsafe.AsRef(in matrix))),
            bytes);
        }

    public static unsafe void UnsafeConvert<T>(in Matrix3<T> matrix, out Matrix3x2<T> ret)
    where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        ret = default;
        var bytes = (uint)(sizeof(T) * 2);
        Unsafe.CopyBlockUnaligned(
             ref Unsafe.As<Vector2<T>, byte>(ref Matrix3x2<T>.GetRowA(ref ret)),
             ref Unsafe.As<Vector3<T>, byte>(ref Matrix3<T>.GetRowA(ref Unsafe.AsRef(in matrix))),
             bytes);
        Unsafe.CopyBlockUnaligned(
            ref Unsafe.As<Vector2<T>, byte>(ref Matrix3x2<T>.GetRowB(ref ret)),
            ref Unsafe.As<Vector3<T>, byte>(ref Matrix3<T>.GetRowB(ref Unsafe.AsRef(in matrix))),
            bytes);
        Unsafe.CopyBlockUnaligned(
            ref Unsafe.As<Vector2<T>, byte>(ref Matrix3x2<T>.GetRowC(ref ret)),
            ref Unsafe.As<Vector3<T>, byte>(ref Matrix3<T>.GetRowC(ref Unsafe.AsRef(in matrix))),
            bytes);
        }

    public static unsafe void UnsafeConvert<T>(in Matrix3x2<T> matrix, out Matrix3<T> ret)
    where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        ret = default;
        var bytes = (uint)(sizeof(T) * 2);
        Unsafe.CopyBlockUnaligned(
             ref Unsafe.As<Vector3<T>, byte>(ref Matrix3<T>.GetRowA(ref ret)),
             ref Unsafe.As<Vector2<T>, byte>(ref Matrix3x2<T>.GetRowA(ref Unsafe.AsRef(in matrix))),
             bytes);
        Unsafe.CopyBlockUnaligned(
            ref Unsafe.As<Vector3<T>, byte>(ref Matrix3<T>.GetRowB(ref ret)),
            ref Unsafe.As<Vector2<T>, byte>(ref Matrix3x2<T>.GetRowB(ref Unsafe.AsRef(in matrix))),
            bytes);
        Unsafe.CopyBlockUnaligned(
            ref Unsafe.As<Vector3<T>, byte>(ref Matrix3<T>.GetRowC(ref ret)),
            ref Unsafe.As<Vector2<T>, byte>(ref Matrix3x2<T>.GetRowC(ref Unsafe.AsRef(in matrix))),
            bytes);
        ret.C3 = T.One;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void UnsafeConvert<T>(in Matrix3x2<T> matrix, out Matrix2<T> ret)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        ret = Unsafe.As<Matrix3x2<T>, Matrix2<T>>(ref Unsafe.AsRef(in matrix));
        }

    public static unsafe void UnsafeConvert<T>(in Matrix2<T> matrix, out Matrix3x2<T> ret)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        ret = default;
        var bytes = (uint)(sizeof(T) * 4);
        Unsafe.CopyBlockUnaligned(
            ref Unsafe.As<Matrix3x2<T>, byte>(ref ret),
            ref Unsafe.As<Matrix2<T>, byte>(ref Unsafe.AsRef(in matrix)),
            bytes);
        }

    #endregion Conversion

    #region Documentation

    public static class Docs {

        public const string BaseMessage = $"No coordinate system handedness and/or multiplication convention was set. One of MATRIX_LEFTHANDED_COORDINATE_SYSTEM / MATRIX_RIGHTHANDED_COORDINATE_SYSTEM and MATRIX_PREMULTIPLIED_CONVENTION / MATRIX_POSTMULTIPLIED_CONVENTION must be set in Bunnarium.Maths";

        /// <summary> For intrinsic rotations, in which each rotation uses the local axis following any previous rotations, the equivalent extrinsic rotation is
        /// </summary>
        public static void EquivalentIntrinsicRotationIs<T>() { /* do nothing */ }

        /// <remarks> The handedness of the operation depends on either the <c>MATRIX_LEFTHANDED_COORDINATE_SYSTEM</c> or the <c>MATRIX_RIGHTHANDED_COORDINATE_SYSTEM</c> compiler symbol being set in <see cref="Bunnarium.Maths"/>. If neither symbol is set, then this function will throw an exception.
        /// </remarks>
        public static void HandednessInstructions<T>() { /* do nothing */ }

        /// <summary><inheritdoc cref="IMatrix{T}.I3DMatrix{T}.CreateRotationXYZ(Angle{T}, Angle{T}, Angle{T})"/>
        /// </summary>
        public static void IsExtrinsicXYZRotation<T>() { /* do nothing */ }

        /// <summary><inheritdoc cref="IMatrix{T}.I3DMatrix{T}.CreateRotationXZY(Angle{T}, Angle{T}, Angle{T})"/>
        /// </summary>
        public static void IsExtrinsicXZYRotation<T>() { /* do nothing */ }

        /// <summary><inheritdoc cref="IMatrix{T}.I3DMatrix{T}.CreateRotationYXZ(Angle{T}, Angle{T}, Angle{T})"/>
        /// </summary>
        public static void IsExtrinsicYXZRotation<T>() { /* do nothing */ }

        /// <summary><inheritdoc cref="IMatrix{T}.I3DMatrix{T}.CreateRotationYZX(Angle{T}, Angle{T}, Angle{T})"/>
        /// </summary>
        public static void IsExtrinsicYZXRotation<T>() { /* do nothing */ }

        /// <summary><inheritdoc cref="IMatrix{T}.I3DMatrix{T}.CreateRotationZXY(Angle{T}, Angle{T}, Angle{T})"/>
        /// </summary>
        public static void IsExtrinsicZXYRotation<T>() { /* do nothing */ }

        /// <summary><inheritdoc cref="IMatrix{T}.I3DMatrix{T}.CreateRotationZYX(Angle{T}, Angle{T}, Angle{T})"/>
        /// </summary>
        public static void IsExtrinsicZYXRotation<T>() { /* do nothing */ }

        /// <remarks> This operation assumes the use of row-major matrices, such that row elements are stored consecutively in memory. To convert a <see cref="IMatrix{T}.ISquareMatrix{Matrix}">square</see> row-major matrix to a column-major matrix, call <see cref="IMatrix{T}.ISquareMatrix{Matrix}.Transpose(in Matrix)">Transpose()</see> on a row-major matrix.
        /// </remarks>
        public static void IsRowMajor<Matrix, T>() { /* do nothing */ }

        /// <remarks> The multiplication convention determines how transformations compose and how vectors are transformed. Premultiplied (row-vector) systems apply right-to-left with <c>v×M</c> syntax; postmultiplied (column-vector) systems apply left-to-right with <c>M×v</c> syntax. The multiplication convention is set one of these two compiler symbols: <c>MATRIX_PREMULTIPLIED_CONVENTION</c> and <c>MATRIX_POSTMULTIPLIED_CONVENTION</c>.
        /// </remarks>
        public static void MultiplicationConventionInstructions<T>() { /* do nothing */}

        /// <remarks> The rotation is performed in a left-handed (clockwise) rotation.
        /// <para/><inheritdoc cref="Docs.IsRowMajor"/>
        /// </remarks>
        public static void UsesLeftHandedRotation<T>() { /* do nothing */ }

        /// <remarks> This symbol uses a <b>postmultiplied</b>, column-vector <see cref="Docs.MultiplicationConventionInstructions{T}">transformation convention</see>. In this convention, matrices are multiplied in right-to-left order and vectors are evaluated as column vectors with <c>M*v</c> syntax.
        /// </remarks>
        public static void UsesPostmultiplicationConvention<T>() { /* do nothing */ }

        /// <remarks> This symbol uses a <b>premultiplied</b>, row-vector <see cref="Docs.MultiplicationConventionInstructions{T}">transformation convention</see>. In this convention, matrices are multiplied in left-to-right order and vectors are evaluated as row vectors with <c>v*M</c> syntax.
        /// </remarks>
        public static void UsesPremultiplicationConvention<T>() { /* do nothing */ }

        /// <remarks> The rotation is performed in a right-handed (counterclockwise) rotation.
        /// <para/><inheritdoc cref="Docs.IsRowMajor"/>
        /// </remarks>
        public static void UsesRightHandedRotation<T>() { /* do nothing */ }


        #region ASCII Matrices

        /// <remarks><c>
        ///	╔══════╦══════╗<para/>
        ///	║. A1 .║. A2 .║<para/>
        ///	║. . . ║ . . .║<para/>
        ///	╠══════╠══════╣<para/>
        ///	║. B1 .║. B2 .║<para/>
        ///	║. . . ║ . . .║<para/>
        ///	╚══════╩══════╝
        /// </c></remarks>
        public static void DisplayMatrix2() { }

        /// <remarks><c>
        ///	╔══════╦══════╦══════╗<para/>
        ///	║. A1 .║. A2 .║. A3 .║<para/>
        ///	║. . . ║ . . .║ . . .║<para/>
        ///	╠══════╠══════╠══════╣<para/>
        ///	║. B1 .║. B2 .║. B3 .║<para/>
        ///	║. . . ║ . . .║ . . .║<para/>
        ///	╠══════╠══════╠══════╣<para/>
        ///	║. C1 .║. C2 .║. C3 .║<para/>
        ///	║. . . ║ . . .║ . . .║<para/>
        ///	╚══════╩══════╩══════╝
        /// </c></remarks>
        public static void DisplayMatrix3() { }

        /// <remarks><c>
        ///	╔══════╦══════╦══════╗<para/>
        ///	║. A1 .║. A2 .║. A3 .║<para/>
        ///	║. . . ║ . . .║ . . .║<para/>
        ///	╠══════╠══════╠══════╣<para/>
        ///	║. B1 .║. B2 .║. B3 .║<para/>
        ///	║. . . ║ . . .║ . . .║<para/>
        ///	╚══════╩══════╩══════╝
        /// </c></remarks>
        public static void DisplayMatrix2x3() { }

        /// <remarks><c>
        ///	╔══════╦══════╗<para/>
        ///	║. A1 .║. A2 .║<para/>
        ///	║. . . ║ . . .║<para/>
        ///	╠══════╠══════╣<para/>
        ///	║. B1 .║. B2 .║<para/>
        ///	║. . . ║ . . .║<para/>
        ///	╠══════╠══════╣<para/>
        ///	║. C1 .║. C2 .║<para/>
        ///	║. . . ║ . . .║<para/>
        ///	╚══════╩══════╝
        /// </c></remarks>
        public static void DisplayMatrix3x2() { }

        /// <remarks><c>
        ///	╔══════╦══════╦══════╦══════╗<para/>
        ///	║. A1 .║. A2 .║. A3 .║. A4 .║<para/>
        ///	║. . . ║ . . .║. . . ║ . . .║<para/>
        ///	╠══════╠══════╠══════╠══════╣<para/>
        ///	║. B1 .║. B2 .║. B3 .║. B4 .║<para/>
        ///	║. . . ║ . . .║. . . ║ . . .║<para/>
        ///	╠══════╠══════╠══════╠══════╣<para/>
        ///	║. C1 .║. C2 .║. C3 .║. C4 .║<para/>
        ///	║. . . ║ . . .║. . . ║ . . .║<para/>
        ///	╠══════╠══════╠══════╠══════╣<para/>
        ///	║. D1 .║. D2 .║. D3 .║. D4 .║<para/>
        ///	║. . . ║ . . .║. . . ║ . . .║<para/>
        ///	╚══════╩══════╩══════╩══════╝
        /// </c></remarks>
        public static void DisplayMatrix4() { }

        /// <remarks><c>
        ///	╔══════╦══════╦══════╦══════╗<para/>
        ///	║. A1 .║. A2 .║. A3 .║. A4 .║<para/>
        ///	║. . . ║ . . .║. . . ║ . . .║<para/>
        ///	╠══════╠══════╠══════╠══════╣<para/>
        ///	║. B1 .║. B2 .║. B3 .║. B4 .║<para/>
        ///	║. . . ║ . . .║. . . ║ . . .║<para/>
        ///	╠══════╠══════╠══════╠══════╣<para/>
        ///	║. C1 .║. C2 .║. C3 .║. C4 .║<para/>
        ///	║. . . ║ . . .║. . . ║ . . .║<para/>
        ///	╚══════╩══════╩══════╩══════╝
        /// </c></remarks>
        public static void DisplayMatrix3x4() { }

        /// <remarks><c>
        ///	╔══════╦══════╦══════╗<para/>
        ///	║. A1 .║. A2 .║. A3 .║<para/>
        ///	║. . . ║ . . .║ . . .║<para/>
        ///	╠══════╠══════╠══════╣<para/>
        ///	║. B1 .║. B2 .║. B3 .║<para/>
        ///	║. . . ║ . . .║ . . .║<para/>
        ///	╠══════╠══════╠══════╣<para/>
        ///	║. C1 .║. C2 .║. C3 .║<para/>
        ///	║. . . ║ . . .║ . . .║<para/>
        ///	╠══════╠══════╠══════╣<para/>
        ///	║. D1 .║. D2 .║. D3 .║<para/>
        ///	║. . . ║ . . .║ . . .║<para/>
        ///	╚══════╩══════╩══════╝
        /// </c></remarks>
        public static void DisplayMatrix4x3() { }

        #endregion ASCII Matrices
        }

    #endregion Documentation

    #region Compilation Checks

#if (MATRIX_LEFTHANDED_COORDINATE_SYSTEM && MATRIX_RIGHTHANDED_COORDINATE_SYSTEM)
#error MATRIX_LEFTHANDED_COORDINATE_SYSTEM and MATRIX_RIGHTHANDED_COORDINATE_SYSTEM are mutually exclusive!
#elif !(MATRIX_LEFTHANDED_COORDINATE_SYSTEM || MATRIX_RIGHTHANDED_COORDINATE_SYSTEM)
#error One of MATRIX_LEFTHANDED_COORDINATE_SYSTEM or MATRIX_RIGHTHANDED_COORDINATE_SYSTEM must be set!
#endif
#if (MATRIX_PREMULTIPLIED_CONVENTION && MATRIX_POSTMULTIPLIED_CONVENTION)
#error MATRIX_PREMULTIPLIED_CONVENTION and MATRIX_POSTMULTIPLIED_CONVENTION are mutually exclusive!
#elif !(MATRIX_PREMULTIPLIED_CONVENTION || MATRIX_POSTMULTIPLIED_CONVENTION)
#error One of MATRIX_PREMULTIPLIED_CONVENTION or MATRIX_POSTMULTIPLIED_CONVENTION must be set!
#endif

    #endregion Compilation Checks
    }

#pragma warning restore IDE0018 // Inline variable declaration
