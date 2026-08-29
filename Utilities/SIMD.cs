using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.Arm;
using System.Runtime.Intrinsics.X86;
using static Bunnarium.Tools.Utilities.SIMD;

namespace Bunnarium.Maths.Utilities;

/// <summary> Extensions and helpers for math primitive-specific SIMD functions.
/// </summary>
/// <remarks><inheritdoc cref="Tools.Utilities.SIMD"/></remarks>
public static class SIMD {

    #region Loads (Vectors)

    /// <inheritdoc
    /// cref="Load128XYXY{T}(Vector2{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<T> Load128XYXY<T>(this IntegralVector2<T> vector)
        where T : unmanaged, IBinaryInteger<T>, IMinMaxValue<T> {
        if (typeof(T) == typeof(int) || typeof(T) == typeof(uint)) {
            var d = Unsafe.BitCast<IntegralVector2<T>, double>(vector);
            if (Sse3.IsSupported) {
                return Sse3.MoveAndDuplicate(Vector128.CreateScalarUnsafe(d)).As<double, T>();
                }
            else if (AdvSimd.Arm64.IsSupported) {
                return AdvSimd.Arm64.DuplicateSelectedScalarToVector128(Vector128.CreateScalarUnsafe(d), 0).As<double, T>();
                }
            else {
                return Vector128.Create(d, d).As<double, T>();
                }
            }
        throw new NotSupportedException($"{nameof(Load128XYXY)} is only valid for {nameof(T)} = (int | uint).");
        }

    /// <summary> Loads and duplicates a vector, returning (<c><paramref name="vector"/>.X, <paramref name="vector"/>.Y, <paramref name="vector"/>.X, <paramref name="vector"/>.Y</c>)
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<T> Load128XYXY<T>(this Vector2<T> vector)
    where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        if (typeof(T) == typeof(float)) {
            var d = Unsafe.BitCast<Vector2<T>, double>(vector);
            if (Sse3.IsSupported) {
                return Sse3.MoveAndDuplicate(Vector128.CreateScalarUnsafe(d)).As<double, T>();
                }
            else if (AdvSimd.Arm64.IsSupported) {
                return AdvSimd.Arm64.DuplicateSelectedScalarToVector128(Vector128.CreateScalarUnsafe(d), 0).As<double, T>();
                }
            else {
                return Vector128.Create(d, d).As<double, T>();
                }
            }
        throw new NotSupportedException($"{nameof(Load128XYXY)} is only valid for {nameof(T)} = float.");
        }

    /// <inheritdoc
    /// cref="Load128XYXY{T}(Vector2{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe Vector256<T> Load256XYXY<T>(this IntegralVector2<T> vector)
        where T : unmanaged, IBinaryInteger<T>, IMinMaxValue<T> {
        if (typeof(T) == typeof(int) || typeof(T) == typeof(uint)) {
            if (Avx.IsSupported) {
                return Avx.BroadcastVector128ToVector256((double*)Unsafe.AsPointer(ref vector)).As<double, T>();
                }
            else {
                var d = (double*)Unsafe.AsPointer(ref vector);
                return Vector256.Create(d[0], d[1], d[0], d[1]).As<double, T>();
                }
            }
        else if (typeof(T) == typeof(float)) {
            if (Avx2.IsSupported) {
                return Avx2.BroadcastScalarToVector256(
                    Vector128.CreateScalarUnsafe(Unsafe.BitCast<IntegralVector2<T>, double>(vector))
                    ).AsSingle().As<float, T>();
                }
            else if (Avx.IsSupported) {
                return Avx.BroadcastScalarToVector256((double*)Unsafe.AsPointer(ref vector)).As<double, T>();
                }
            else {
                var f = (float*)Unsafe.AsPointer(ref vector);
                return Vector256.Create(f[0], f[1], f[0], f[1], f[0], f[1], f[0], f[1]).As<float, T>();
                }
            }
        throw new NotSupportedException($"{nameof(Load256XYXY)} is only valid for {nameof(T)} = double or {nameof(T)} = (int | uint).");
        }

    /// <inheritdoc
    /// cref="Load128XYXY{T}(Vector2{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe Vector256<T> Load256XYXY<T>(this Vector2<T> vector)
        where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        if (typeof(T) == typeof(double)) {
            if (Avx.IsSupported) {
                return Avx.BroadcastVector128ToVector256((double*)Unsafe.AsPointer(ref vector)).As<double, T>();
                }
            else {
                var d = (double*)Unsafe.AsPointer(ref vector);
                return Vector256.Create(d[0], d[1], d[0], d[1]).As<double, T>();
                }
            }
        else if (typeof(T) == typeof(float)) {
            if (Avx2.IsSupported) {
                return Avx2.BroadcastScalarToVector256(
                    Vector128.CreateScalarUnsafe(Unsafe.BitCast<Vector2<T>, double>(vector))
                    ).AsSingle().As<float, T>();
                }
            else if (Avx.IsSupported) {
                return Avx.BroadcastScalarToVector256((double*)Unsafe.AsPointer(ref vector)).As<double, T>();
                }
            else {
                var f = (float*)Unsafe.AsPointer(ref vector);
                return Vector256.Create(f[0], f[1], f[0], f[1], f[0], f[1], f[0], f[1]).As<float, T>();
                }
            }
        throw new NotSupportedException($"{nameof(Load256XYXY)} is only valid for {nameof(T)} = double or {nameof(T)} = float.");
        }

    /// <summary> Loads a <see cref="Vector3{T}">Vector3&lt;float&gt;</see> to a vector with its fourth component set to <c>0</c>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe Vector128<float> LoadVector3(in Vector3<float> vector) {
        return LoadFFF0((float*)Unsafe.AsPointer(in vector));
        }

    /// <summary> Loads a <see cref="Vector3{T}">Vector3&lt;double&gt;</see> to a vector with its fourth component set to <c>0</c>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe Vector256<double> LoadVector3(in Vector3<double> vector) {
        return LoadDDD0((double*)Unsafe.AsPointer(in vector));
        }

    #endregion Loads (Vectors)

    #region SinCos

    /// <summary> Returns <c>((sin(<paramref name="rotationX"/>), sin(<paramref name="rotationY"/>), sin(<paramref name="rotationZ"/>), 0), (cos(<paramref name="rotationX"/>), cos(<paramref name="rotationY"/>), cos(<paramref name="rotationZ"/>), 0))</c>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static (Vector4<T> Sin, Vector4<T> Cos) SinCos<T>(
    Angle<T> rotationX,
    Angle<T> rotationY,
    Angle<T> rotationZ
    ) where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        if (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float)) {
            var angles = Vector128.Create(
            Unsafe.BitCast<Angle<T>, float>(rotationX),
            Unsafe.BitCast<Angle<T>, float>(rotationY),
            Unsafe.BitCast<Angle<T>, float>(rotationZ),
            0f
            );
            return Unsafe.BitCast<(Vector128<float>, Vector128<float>), (Vector4<T>, Vector4<T>)>(Vector128.SinCos(angles));
            }
        else if (Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double)) {
            var angles = Vector256.Create(
            Unsafe.BitCast<Angle<T>, double>(rotationX),
            Unsafe.BitCast<Angle<T>, double>(rotationY),
            Unsafe.BitCast<Angle<T>, double>(rotationZ),
            0d
            );
            return Unsafe.BitCast<(Vector256<double>, Vector256<double>), (Vector4<T>, Vector4<T>)>(Vector256.SinCos(angles));
            }
        else {
            T sx, sy, sz, cx, cy, cz;
            (sx, cx) = T.SinCos(rotationX.Radians);
            (sy, cy) = T.SinCos(rotationY.Radians);
            (sz, cz) = T.SinCos(rotationZ.Radians);
            return (new(sx, sy, sz, T.Zero), new(cx, cy, cz, T.Zero));
            }
        }

    /// <inheritdoc
    /// cref="SinCos{T}(Angle{T}, Angle{T}, Angle{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static (Vector4<T> Sin, Vector4<T> Cos) SinCos<T>(
        T rotationX,
        T rotationY,
        T rotationZ
        ) where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {
        if (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float)) {
            var angles = Vector128.Create(
            Unsafe.BitCast<T, float>(rotationX),
            Unsafe.BitCast<T, float>(rotationY),
            Unsafe.BitCast<T, float>(rotationZ),
            0f
            );
            return Unsafe.BitCast<(Vector128<float>, Vector128<float>), (Vector4<T>, Vector4<T>)>(Vector128.SinCos(angles));
            }
        else if (Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double)) {
            var angles = Vector256.Create(
            Unsafe.BitCast<T, double>(rotationX),
            Unsafe.BitCast<T, double>(rotationY),
            Unsafe.BitCast<T, double>(rotationZ),
            0d
            );
            return Unsafe.BitCast<(Vector256<double>, Vector256<double>), (Vector4<T>, Vector4<T>)>(Vector256.SinCos(angles));
            }
        else {
            T sx, sy, sz, cx, cy, cz;
            (sx, cx) = T.SinCos(rotationX);
            (sy, cy) = T.SinCos(rotationY);
            (sz, cz) = T.SinCos(rotationZ);
            return (new(sx, sy, sz, T.Zero), new(cx, cy, cz, T.Zero));
            }
        }

    #endregion SinCos
    }
