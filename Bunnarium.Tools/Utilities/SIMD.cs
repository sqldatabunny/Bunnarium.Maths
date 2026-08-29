using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.Arm;
using System.Runtime.Intrinsics.X86;
using System.Diagnostics.CodeAnalysis;

namespace Bunnarium.Tools.Utilities;

/// <summary> Extensions and helpers for SIMD functions.
/// </summary>
/// <remarks> This function library is used to keep SIMD cleaner and more concise and to provide platform-agnostic functions with platform-specific implementations (so that checks for specific instruction sets don't need to be performed elsewhere and the most efficient versions of common operations are made readily available).
/// </remarks>
public static unsafe partial class SIMD {

    #region Absolute Values

    /// <inheritdoc
    /// cref="AbsLower(Vector128{float})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<int> AbsLower(Vector128<int> vector) {
        return Vector128.AndNot(vector, MinMaxSignMask128Int);
        }

    /// <inheritdoc
    /// cref="AbsLower(Vector256{double})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector256<long> AbsLower(Vector256<long> vector) {
        return Vector256.AndNot(vector, MinMaxSignMask256Int);
        }

    /// <summary>Returns <c>(|<paramref name="vector"/>.X|, |<paramref name="vector"/>.Y|, <paramref name="vector"/>.Z, <paramref name="vector"/>.W)</c>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<float> AbsLower(Vector128<float> vector) {
        return Vector128.AndNot(vector, MinMaxSignMask128);
        }

    /// <inheritdoc
    /// cref="AbsLower(Vector128{float})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector256<double> AbsLower(Vector256<double> vector) {
        return Vector256.AndNot(vector, MinMaxSignMask256);
        }

    /// <summary>Returns <c>(<paramref name="vector"/>.X, <paramref name="vector"/>.Y, |<paramref name="vector"/>.Z|, |<paramref name="vector"/>.W|)</c>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<float> AbsUpper(Vector128<float> vector) {
        return Vector128.AndNot(vector, BoxSignMask128);
        }

    /// <inheritdoc
    /// cref="AbsUpper(Vector128{float})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector256<double> AbsUpper(Vector256<double> vector) {
        return Vector256.AndNot(vector, BoxSignMask256);
        }

    /// <inheritdoc
    /// cref="AbsUpper(Vector256{double})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector256<long> AbsUpper(Vector256<long> vector) {
        return Vector256.AndNot(vector, BoxSignMask256Int);
        }

    /// <inheritdoc
    /// cref="AbsUpper(Vector128{float})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<int> AbsUpper(Vector128<int> vector) {
        return Vector128.AndNot(vector, BoxSignMask128Int);
        }

    /// <summary>Returns <c>(<paramref name="vector"/>.X, <paramref name="vector"/>.Y, |<paramref name="vector"/>.X|, |<paramref name="vector"/>.Y|)</c>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<float> LowerAbsLower(Vector128<float> vector) {
        return Vector128.AndNot(Shuffle0101(vector), BoxSignMask128);
        }

    /// <inheritdoc
    /// cref="LowerAbsLower(Vector128{float})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector256<double> LowerAbsLower(Vector256<double> vector) {
        return Vector256.AndNot(Shuffle0101(vector), BoxSignMask256);
        }

    /// <inheritdoc
    /// cref="LowerAbsLower(Vector128{float})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector256<long> LowerAbsLower(Vector256<long> vector) {
        return Vector256.AndNot(Shuffle0101(vector), BoxSignMask256Int);
        }

    /// <inheritdoc
    /// cref="LowerAbsLower(Vector128{float})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<int> LowerAbsLower(Vector128<int> vector) {
        return Vector128.AndNot(Shuffle0101(vector), BoxSignMask128Int);
        }

    /// <summary>Returns <c>(<paramref name="vector"/>.Z, <paramref name="vector"/>.W, |<paramref name="vector"/>.Z|, |<paramref name="vector"/>.W|)</c>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<float> UpperAbsUpper(Vector128<float> vector) {
        return Vector128.AndNot(Shuffle2323(vector), BoxSignMask128);
        }

    /// <inheritdoc
    /// cref="UpperAbsUpper(Vector128{float})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector256<double> UpperAbsUpper(Vector256<double> vector) {
        return Vector256.AndNot(Shuffle2323(vector), BoxSignMask256);
        }

    /// <inheritdoc
    /// cref="UpperAbsUpper(Vector128{float})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<int> UpperAbsUpper(Vector128<int> vector) {
        return Vector128.AndNot(Shuffle2323(vector), BoxSignMask128Int);
        }

    /// <inheritdoc
    /// cref="UpperAbsUpper(Vector128{float})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector256<long> UpperAbsUpper(Vector256<long> vector) {
        return Vector256.AndNot(Shuffle2323(vector), BoxSignMask256Int);
        }

    #endregion Absolute Values

    #region Explicit Loads

    /// <summary> Loads two doubles from a pointer.
    /// <para/> The data is loaded unsafely with only the <em>assumption</em> the pointer points to two consecutive doubles.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe Vector128<T> LoadDD<T>(T* source)
        where T : unmanaged, IBinaryFloatingPointIeee754<T> {
        return Vector128.Load(source);
        }

    /// <summary> Loads two doubles from a pointer, followed by two <c>0</c>s.
    /// <para/> The data is loaded unsafely with only the <em>assumption</em> the pointer points to two consecutive doubles.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe Vector256<T> LoadDD00<T>(T* source)
        where T : unmanaged, IBinaryFloatingPointIeee754<T> {
        return Vector256.Create(Vector128.Load(source), Vector128<T>.Zero);
        }

    /// <summary> Loads three doubles from a pointer, followed by a <c>0</c>.
    /// <para/> The data is loaded unsafely with only the <em>assumption</em> the pointer points to three consecutive doubles.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe Vector256<T> LoadDDD0<T>(T* source)
        where T : unmanaged, IBinaryFloatingPointIeee754<T> {
        var s = (double*)source;
        if (Sse2.IsSupported || AdvSimd.IsSupported) {
            var xy = Vector128.Load(s);                     // [x, y]
            var z = Vector128.CreateScalar(s[2]);           // [z, 0]
            return Vector256.Create(xy, z).As<double, T>(); // [x, y, z, 0]
            }
        else {
            return Vector256.Create(s[0], s[1], s[2], default).As<double, T>();
            }
        }

    /// <summary> Loads three doubles from a pointer, followed by a <c>0</c>.
    /// <para/> ⚠️ The data is loaded unsafely with only the <em>assumption</em> the pointer points to <b><em>four</em></b> consecutive doubles, with the fourth double discarded. This is about twice as fast as <see cref="LoadDDD0{T}(T*)"/>, but it must only be used when the identity of the fourth double is known, in-scope, and safe to read without care for what its value may be.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe Vector256<T> LoadDDD0Unsafely<T>(T* source)
        where T : unmanaged, IBinaryFloatingPointIeee754<T> {
        if (Avx.IsSupported) {
            return Avx.Blend(Vector256.Load((double*)source), Vector256<double>.Zero, 0b_1_0_0_0).As<double, T>();
            }
        else {
            var s = (double*)source;
            return Vector256.Create(s[0], s[1], s[2], default).As<double, T>();
            }
        }

    /// <summary> Loads four doubles from a pointer.
    /// <para/> The data is loaded unsafely with only the <em>assumption</em> the pointer points to four consecutive doubles.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe Vector256<T> LoadDDDD<T>(T* source)
        where T : unmanaged, IBinaryFloatingPointIeee754<T> {
        return Vector256.Load(source);
        }

    /// <summary> Loads two floats from a pointer.
    /// <para/> The data is loaded unsafely with only the <em>assumption</em> the pointer points to two consecutive floats.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe Vector64<T> LoadFF<T>(T* source)
        where T : unmanaged, IBinaryFloatingPointIeee754<T> {
        return Vector64.Load(source);
        }

    /// <summary> Loads two floats from a pointer, followed by two <c>0</c>s.
    /// <para/> The data is loaded unsafely with only the <em>assumption</em> the pointer points to two consecutive floats.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe Vector128<T> LoadFF00<T>(T* source)
        where T : unmanaged, IBinaryFloatingPointIeee754<T> {
        return Vector128.CreateScalar(*(double*)source).As<double, T>();
        }

    /// <summary> Loads three floats from a pointer, followed by a <c>0</c>.
    /// <para/> The data is loaded unsafely with only the <em>assumption</em> the pointer points to three consecutive floats.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe Vector128<T> LoadFFF0<T>(T* source)
        where T : unmanaged, IBinaryFloatingPointIeee754<T> {
        float* s = (float*)source;
        if (Sse41.IsSupported) {
            var xy = Vector128.CreateScalar(*(double*)s).AsSingle();        // [x, y, 0, 0]
            var insert = Vector128.CreateScalar(s[2]);                      // [z, 0, 0, 0]
            var result = Sse41.Insert(xy, insert, 0b_00_10_10_00);          // [x, y, z, 0]
            return result.As<float, T>();
            }
        else if (Sse.IsSupported) {
            var xy = Vector128.CreateScalarUnsafe(*(double*)s).AsSingle(); // [x, y, 0, 0]
            var z = Vector128.CreateScalar(s[2]);                          // [z, 0, 0, 0]
            var result = Sse.MoveLowToHigh(xy, z);                         // [x, y, z, 0]
            return result.As<float, T>();
            }
        else if (AdvSimd.IsSupported) {
            var xy = Vector128.CreateScalarUnsafe(*(double*)s).AsSingle(); // [x, y, 0, 0]
            var result = AdvSimd.Insert(xy, 2, s[2]);                      // [x, y, z, 0]
            return result.As<float, T>();
            }
        else {
            return Vector128.Create(s[0], s[1], s[2], default).As<float, T>();
            }
        }

    /// <summary> Loads three floats from a pointer, followed by a <c>0</c>.
    /// <para/> ⚠️ The data is loaded unsafely with only the <em>assumption</em> the pointer points to <b><em>four</em></b> consecutive floats, with the fourth float discarded. This is about twice as fast as <see cref="LoadFFF0{T}(T*)"/> but must only be used when the identity of the fourth float is known, in-scope, and safe to read without care for what its value may be.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe Vector128<T> LoadFFF0Unsafely<T>(T* source)
        where T : unmanaged, IBinaryFloatingPointIeee754<T> {
        if (Sse41.IsSupported) {
            return Sse41.Blend(Vector128.Load((float*)source), Vector128<float>.Zero, 0b_1_0_0_0).As<float, T>();
            }
        else if (AdvSimd.IsSupported) {
            return AdvSimd.Insert(Vector128.Load((float*)source), 3, 0f).As<float, T>();
            }
        else {
            var s = (float*)source;
            return Vector128.Create(s[0], s[1], s[2], 0f).As<float, T>();
            }
        }

    /// <summary> Loads four floats from a pointer.
    /// <para/> The data is loaded unsafely with only the <em>assumption</em> the pointer points to four consecutive floats.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe Vector128<T> LoadFFFF<T>(T* source)
        where T : unmanaged, IBinaryFloatingPointIeee754<T> {
        return Vector128.Load(source);
        }

    /// <summary> Loads six floats from a pointer.
    /// <para/> The data is loaded unsafely with only the <em>assumption</em> the pointer points to six consecutive floats.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe Vector256<T> LoadFFFFFF00<T>(T* source)
        where T : unmanaged, IBinaryFloatingPointIeee754<T> {
        return Vector256.Create(
            lower: LoadFFFF(source),
            upper: LoadFF00(source + 4)
            );
        }

    #endregion Explicit Loads

    #region Cross-Combine Halves

    /// <inheritdoc
    /// cref="CrossCombineHalves{T}(Vector128{T}, Vector128{T}, out Vector128{T}, out Vector128{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CrossCombineHalves<T>(Vector256<T> a, Vector256<T> b, out Vector256<T> outA, out Vector256<T> outB)
    where T : unmanaged {
        if (Vector256.IsHardwareAccelerated && Vector128.IsHardwareAccelerated) {
            outA = Vector256.Create(b.GetLower(), a.GetLower());
            outB = Vector256.Create(a.GetUpper(), b.GetUpper());
            }
        else {
            var ad = Unsafe.BitCast<Vector256<T>, Vector256<double>>(a);
            var bd = Unsafe.BitCast<Vector256<T>, Vector256<double>>(b);
            outA = Unsafe.BitCast<Vector256<double>, Vector256<T>>(
                Vector256.Create(bd[0], bd[1], ad[0], ad[1])
                );
            outB = Unsafe.BitCast<Vector256<double>, Vector256<T>>(
                Vector256.Create(ad[2], ad[3], bd[2], bd[3])
                );
            }
        }

    /// <summary> Outputs:
    /// <para/><paramref name="outA"/>: <c>(<paramref name="b"/>.X, <paramref name="b"/>.Y, <paramref name="a"/>.X, <paramref name="a"/>.Y)</c>
    /// <para/><paramref name="outB"/>: <c>(<paramref name="a"/>.Z, <paramref name="a"/>.W, <paramref name="b"/>.Z, <paramref name="b"/>.W)</c>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CrossCombineHalves<T>(Vector128<T> a, Vector128<T> b, out Vector128<T> outA, out Vector128<T> outB)
        where T : unmanaged {
        if (Sse2.IsSupported) {
            outA = Sse2.UnpackLow(b.As<T, double>(), a.As<T, double>()).As<double, T>();
            outB = Sse2.UnpackHigh(a.As<T, double>(), b.As<T, double>()).As<double, T>();
            }
        else if (Sse.IsSupported && typeof(T) == typeof(float)) {
            outA = Sse.MoveLowToHigh(b.As<T, float>(), a.As<T, float>()).As<float, T>();
            outB = Sse.MoveHighToLow(b.As<T, float>(), a.As<T, float>()).As<float, T>();
            }
        else if (AdvSimd.Arm64.IsSupported) {
            outA = AdvSimd.Arm64.ZipLow(b.As<T, ulong>(), a.As<T, ulong>()).As<ulong, T>();
            outB = AdvSimd.Arm64.ZipHigh(a.As<T, ulong>(), b.As<T, ulong>()).As<ulong, T>();
            }
        else if (Vector64.IsHardwareAccelerated && Vector128.IsHardwareAccelerated) {
            outA = Vector128.Create(b.GetLower(), a.GetLower());
            outB = Vector128.Create(a.GetUpper(), b.GetUpper());
            }
        else {
            var ad = Unsafe.BitCast<Vector128<T>, Vector128<float>>(a);
            var bd = Unsafe.BitCast<Vector128<T>, Vector128<float>>(b);
            outA = Unsafe.BitCast<Vector128<float>, Vector128<T>>(
                Vector128.Create(bd[0], bd[1], ad[0], ad[1])
                );
            outB = Unsafe.BitCast<Vector128<float>, Vector128<T>>(
                Vector128.Create(ad[2], ad[3], bd[2], bd[3])
                );
            }
        }

    #endregion Cross-Combine Halves

    #region Gather

    /// <summary> Returns <c>(<paramref name="lower"/>[<paramref name="x"/>], <paramref name="lower"/>[<paramref name="y"/>], <paramref name="upper"/>[<paramref name="z"/>], <paramref name="upper"/>[<paramref name="w"/>])</c>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<float> Gather(
        Vector128<float> lower,
        Vector128<float> upper,
        [ConstantExpected(Min = 0, Max = 3)] byte x,
        [ConstantExpected(Min = 0, Max = 3)] byte y,
        [ConstantExpected(Min = 0, Max = 3)] byte z,
        [ConstantExpected(Min = 0, Max = 3)] byte w
        ) {
        if (Sse.IsSupported) {
#pragma warning disable CA1857 // The parameter expects a constant for optimal performance
            var control = (byte)((w << 6) | (z << 4) | (y << 2) | x);
            return Sse.Shuffle(lower, upper, control);
#pragma warning restore CA1857 // The parameter expects a constant for optimal performance
            }
        else if (AdvSimd.Arm64.IsSupported) {
            if (x == 0 && y == 2 && z == 0 && w == 2) {
                return AdvSimd.Arm64.UnzipEven(lower, upper);
                }
            else if (x == 1 && y == 3 && z == 1 && w == 3) {
                return AdvSimd.Arm64.UnzipOdd(lower, upper);
                }
            else if (x == 0 && y == 1 && z == 2 && w == 3) {
                return ToLowHigh(lower, upper);
                }
            else if (x == 0 && y == 1 && z == 0 && w == 1) {
                return ToLowLow(lower, upper);
                }
            else if (x == 2 && y == 3 && z == 2 && w == 3) {
                return ToHighHigh(lower, upper);
                }
            else if (x == 2 && y == 3 && z == 0 && w == 1) {
                return ToHighLow(lower, upper);
                }
            else {
#pragma warning disable CA1857 // The parameter expects a constant for optimal performance
                return AdvSimd.Arm64.VectorTableLookup(
                    table: (lower.AsByte(), upper.AsByte()),
                    byteIndexes: VectorTableControl(x, y, z, w)
                    ).AsSingle();
#pragma warning restore CA1857 // The parameter expects a constant for optimal performance
                }
            }
        else if (Vector128.IsHardwareAccelerated && Vector128<double>.IsSupported) {
            return ToLowHigh(
                Vector128.Shuffle(lower, Vector128.Create(x, y, 0, 0)),
                Vector128.Shuffle(upper, Vector128.Create(0, 0, z, w))
                );
            }
        else {
            return Vector128.Create(lower[x], lower[y], upper[z], upper[w]);
            }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static Vector128<byte> VectorTableControl([ConstantExpected] int x, [ConstantExpected] int y, [ConstantExpected] int z, [ConstantExpected] int w) {
            // index pattern based on DirectXMath's XMVectorPermute NEON branch
            // block of byte indices + offsets
            return Vector128.Create(
                0x0302_0100 + 0x0404_0404 * x,
                0x0302_0100 + 0x0404_0404 * y,
                0x1312_1110 + 0x0404_0404 * z,
                0x1312_1110 + 0x0404_0404 * w
                ).AsByte();
            }
        }

    /// <summary> Returns <c>(<paramref name="lower"/>[<paramref name="a"/>], <paramref name="lower"/>[<paramref name="b"/>], <paramref name="lower"/>[<paramref name="c"/>],<paramref name="lower"/>[<paramref name="d"/>], <paramref name="upper"/>[<paramref name="e"/>], <paramref name="upper"/>[<paramref name="f"/>], <paramref name="upper"/>[<paramref name="g"/>], <paramref name="upper"/>[<paramref name="h"/>])</c>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector256<float> Gather(
        Vector256<float> lower,
        Vector256<float> upper,
        [ConstantExpected(Min = 0, Max = 7)] byte a,
        [ConstantExpected(Min = 0, Max = 7)] byte b,
        [ConstantExpected(Min = 0, Max = 7)] byte c,
        [ConstantExpected(Min = 0, Max = 7)] byte d,
        [ConstantExpected(Min = 0, Max = 7)] byte e,
        [ConstantExpected(Min = 0, Max = 7)] byte f,
        [ConstantExpected(Min = 0, Max = 7)] byte g,
        [ConstantExpected(Min = 0, Max = 7)] byte h
        ) {
        if (Avx512F.VL.IsSupported) {
            return Avx512F.VL.PermuteVar8x32x2(
                indices: Vector256.Create(a, b, c, d, e + 8, f + 8, g + 8, h + 8),
                lower: lower,
                upper: upper
                );
            }
        else if (Avx2.IsSupported) {
            var control = Vector256.Create(a, b, c, d, e, f, g, h);
            return Avx.Blend(
                Avx2.PermuteVar8x32(lower, control),
                Avx2.PermuteVar8x32(upper, control),
                0b_1111_0000
                );
            }
        else if (AdvSimd.Arm64.IsSupported) {
            var indices = VectorTableControl(a, b, c, d, e, f, g, h);
            return Vector256.Create(
                AdvSimd.Arm64.VectorTableLookup(
                    table: (lower.GetLower().AsByte(), lower.GetUpper().AsByte()),
                    byteIndexes: indices.GetLower()
                    ).AsSingle(),
                AdvSimd.Arm64.VectorTableLookup(
                    table: (upper.GetLower().AsByte(), upper.GetUpper().AsByte()),
                    byteIndexes: indices.GetUpper()
                    ).AsSingle()
                );
            }
        else return Vector256.Create(lower[a], lower[b], lower[c], lower[d], upper[e], upper[f], upper[g], upper[h]);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static Vector256<byte> VectorTableControl(byte a, byte b, byte c, byte d, byte e, byte f, byte g, byte h) {
            return Vector256.Create(
                0x0302_0100 + (0x0404_0404 * a),
                0x0302_0100 + (0x0404_0404 * b),
                0x0302_0100 + (0x0404_0404 * c),
                0x0302_0100 + (0x0404_0404 * d),
                0x0302_0100 + (0x0404_0404 * e),
                0x0302_0100 + (0x0404_0404 * f),
                0x0302_0100 + (0x0404_0404 * g),
                0x0302_0100 + (0x0404_0404 * h)
                ).AsByte();
            }
        }

    /// <inheritdoc
    /// cref="Gather(Vector128{float}, Vector128{float}, byte, byte, byte, byte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector256<double> Gather(
        Vector256<double> lower,
        Vector256<double> upper,
        [ConstantExpected(Min = 0, Max = 3)] byte x,
        [ConstantExpected(Min = 0, Max = 3)] byte y,
        [ConstantExpected(Min = 0, Max = 3)] byte z,
        [ConstantExpected(Min = 0, Max = 3)] byte w
        ) {
        if (Avx512F.VL.IsSupported) {
            return Avx512F.VL.PermuteVar4x64x2(
                indices: Vector256.Create(x, y, z + 4, w + 4),
                lower: lower,
                upper: upper
                );
            }
        else if (Avx2.IsSupported) {
            var control = (byte)((w << 6) | (z << 4) | (y << 2) | x);
            return Avx.Blend(
#pragma warning disable CA1857 // The parameter expects a constant for optimal performance
                Avx2.Permute4x64(lower, control),
                Avx2.Permute4x64(upper, control),
#pragma warning restore CA1857 // The parameter expects a constant for optimal performance
                0b_1100
                );
            }
        else if (Avx.IsSupported && (x < 2) == (y < 2) && (z < 2) == (w < 2)) { // for each vector half (low vs high) independently: both controls below two or both controls above two
#pragma warning disable CA1857 // The parameter expects a constant for optimal performance

            // [0|1, 0|1] => 0x20 => [0, 1, 4, 5]
            // [2|3, 0|1] => 0x21 => [2, 3, 4, 5]
            // [0|1, 2|3] => 0x30 => [0, 1, 6, 7]
            // [2|3, 2|3] => 0x31 => [2, 3, 6, 7]
            var lanes = Avx.Permute2x128(lower, upper, (byte)((x < 2 ? 0x00 : 0x01) | (z < 2 ? 0x20 : 0x30)));

            // VPERMILPD indexes each value within lower/upper lanes at the lower
            // index (0) or the upper index (1), hence why each index it &'d by 1

            // examples:
            // [0, 1, 0, 1] => lanes.Permute(2 |  8) => [0, 1, 4, 5].Permute(0b_1010) => [0, 1, 4, 5]
            // [0, 1, 2, 3] => lanes.Permute(2 |  8) => [0, 1, 6, 7].Permute(0b_1010) => [0, 1, 6, 7]
            // [2, 3, 1, 1] => lanes.Permute(2 | 12) => [2, 3, 4, 5].Permute(0b_1110) => [2, 3, 5, 5]
            // [3, 2, 1, 1] => lanes.Permute(1 | 12) => [2, 3, 4, 5].Permute(0b_1101) => [3, 2, 5, 5]
            return Avx.Permute(lanes, (byte)((x & 1) | ((y & 1) << 1) | ((z & 1) << 2) | ((w & 1) << 3)));
#pragma warning restore CA1857 // The parameter expects a constant for optimal performance
            }
        else if (AdvSimd.Arm64.IsSupported) {
            var indices = VectorTableControl(x, y, z, w);
            return Vector256.Create(
                AdvSimd.Arm64.VectorTableLookup(
                    table: (lower.GetLower().AsByte(), lower.GetUpper().AsByte()),
                    byteIndexes: indices.GetLower()
                    ).AsDouble(),
                AdvSimd.Arm64.VectorTableLookup(
                    table: (upper.GetLower().AsByte(), upper.GetUpper().AsByte()),
                    byteIndexes: indices.GetUpper()
                    ).AsDouble()
                );
            }
        else if (Vector256.IsHardwareAccelerated && Vector256<double>.IsSupported) {
            return ToLowHigh(
                Vector256.Shuffle(lower, Vector256.Create(x, y, 0L, 0L)),
                Vector256.Shuffle(upper, Vector256.Create(0L, 0L, z, w))
                );
            }
        else {
            return Vector256.Create(lower[x], lower[y], upper[z], upper[w]);
            }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static Vector256<byte> VectorTableControl(byte x, byte y, byte z, byte w) {
            // block of byte indices + offsets
            return Vector256.Create(
                0x0706_0504_0302_0100L + (0x0808_0808_0808_0808L * x),
                0x0706_0504_0302_0100L + (0x0808_0808_0808_0808L * y),
                0x0706_0504_0302_0100L + (0x0808_0808_0808_0808L * z),
                0x0706_0504_0302_0100L + (0x0808_0808_0808_0808L * w)
                ).AsByte();
            }
        }

    #endregion Gather

    #region Horizontal Min/Max

    /// <inheritdoc
    ///cref="HorizontalMax(Vector256{float})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong HorizontalMax(this Vector256<ulong> vector) {
        if (Vector256.IsHardwareAccelerated && Vector256<ulong>.IsSupported) {
            Vector256<ulong> shuffle;
            shuffle = vector.Shuffle2323();             // [A, B, C, D] → [C, D, _, _]
            vector = Vector256.Max(vector, shuffle);    // [Max(A,C), Max(B,D), _, _]
            shuffle = vector.Shuffle1010();             // [Max(A,C), Max(B,D), _, _] → [Max(B,D), Max(A,C), _, _]
            vector = Vector256.Max(vector, shuffle);    // [Max(A,C,B,D), _, _, _]
            return vector.ToScalar();
            }
        else {
            return ulong.Max(ulong.Max(vector[0], vector[1]), ulong.Max(vector[2], vector[3]));
            }
        }

    /// <inheritdoc
    ///cref="HorizontalMax(Vector256{float})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long HorizontalMax(this Vector256<long> vector) {
        if (Vector256.IsHardwareAccelerated && Vector256<long>.IsSupported) {
            Vector256<long> shuffle;
            shuffle = vector.Shuffle2323();             // [A, B, C, D] → [C, D, _, _]
            vector = Vector256.Max(vector, shuffle);    // [Max(A,C), Max(B,D), _, _]
            shuffle = vector.Shuffle1010();             // [Max(A,C), Max(B,D), _, _] → [Max(B,D), Max(A,C), _, _]
            vector = Vector256.Max(vector, shuffle);    // [Max(A,C,B,D), _, _, _]
            return vector.ToScalar();
            }
        else {
            return long.Max(long.Max(vector[0], vector[1]), long.Max(vector[2], vector[3]));
            }
        }

    /// <inheritdoc
    ///cref="HorizontalMax(Vector128{float})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint HorizontalMax(this Vector128<uint> vector) {
        if (AdvSimd.Arm64.IsSupported) {
            return AdvSimd.Arm64.MaxAcross(vector).ToScalar();
            }
        else if (Vector128.IsHardwareAccelerated && Vector128<uint>.IsSupported) {
            Vector128<uint> shuffle;
            shuffle = vector.Shuffle2323();             // [A, B, C, D] → [C, D, _, _]
            vector = Vector128.Max(vector, shuffle);    // [Max(A,C), Max(B,D), _, _]
            shuffle = vector.Shuffle1010();             // [Max(A,C), Max(B,D), _, _] → [Max(B,D), Max(A,C), _, _]
            vector = Vector128.Max(vector, shuffle);    // [Max(A,C,B,D), _, _, _]
            return vector.ToScalar();
            }
        else {
            return uint.Max(uint.Max(vector[0], vector[1]), uint.Max(vector[2], vector[3]));
            }
        }

    /// <inheritdoc
    ///cref="HorizontalMax(Vector128{float})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int HorizontalMax(this Vector128<int> vector) {
        if (AdvSimd.Arm64.IsSupported) {
            return AdvSimd.Arm64.MaxAcross(vector).ToScalar();
            }
        else if (AdvSimd.IsSupported) {
            vector = AdvSimd.Max(vector, AdvSimd.ReverseElement32(vector.AsInt64()).AsInt32()); // [Max(A,C), Max(B,D), _, _]
            vector = AdvSimd.Max(vector, AdvSimd.ExtractVector128(vector, vector, 2));          // swap the two 64-bit halves
            return vector.ToScalar();
            }
        else if (Vector128.IsHardwareAccelerated && Vector128<int>.IsSupported) {
            Vector128<int> shuffle;
            shuffle = vector.Shuffle2323();             // [A, B, C, D] → [C, D, _, _]
            vector = Vector128.Max(vector, shuffle);    // [Max(A,C), Max(B,D), _, _]
            shuffle = vector.Shuffle1010();             // [Max(A,C), Max(B,D), _, _] → [Max(B,D), Max(A,C), _, _]
            vector = Vector128.Max(vector, shuffle);    // [Max(A,C,B,D), _, _, _]
            return vector.ToScalar();
            }
        else {
            return int.Max(int.Max(vector[0], vector[1]), int.Max(vector[2], vector[3]));
            }
        }

    /// <summary> Equivalent to:
    /// <para/><c><see cref="INumber{TSelf}.Max(TSelf, TSelf)">T.Max</see>(<see cref="INumber{TSelf}.Max(TSelf, TSelf)">T.Max</see>(X, Y), <see cref="INumber{TSelf}.Max(TSelf, TSelf)">T.Max</see>(Z, W))
    /// </c>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float HorizontalMax(this Vector128<float> vector) {
        if (AdvSimd.Arm64.IsSupported) {
            return AdvSimd.Arm64.MaxAcross(vector).ToScalar();
            }
        else if (AdvSimd.IsSupported) {
            vector = AdvSimd.Max(vector, AdvSimd.ReverseElement32(vector.AsInt64()).AsSingle());    // [Max(A,C), Max(B,D), _, _]
            vector = AdvSimd.Max(vector, AdvSimd.ExtractVector128(vector, vector, 2));              // swap the two 64-bit halves
            return vector.ToScalar();
            }
        else if (Vector128.IsHardwareAccelerated && Vector128<float>.IsSupported) {
            Vector128<float> shuffle;
            shuffle = vector.Shuffle2323();             // [A, B, C, D] → [C, D, _, _]
            vector = Vector128.Max(vector, shuffle);    // [Max(A,C), Max(B,D), _, _]
            shuffle = vector.Shuffle1010();             // [Max(A,C), Max(B,D), _, _] → [Max(B,D), Max(A,C), _, _]
            vector = Vector128.Max(vector, shuffle);    // [Max(A,C,B,D), _, _, _]
            return vector.ToScalar();
            }
        else {
            return float.Max(float.Max(vector[0], vector[1]), float.Max(vector[2], vector[3]));
            }
        }

    /// <summary> Equivalent to:
    /// <para/><c><see cref="INumber{TSelf}.Max(TSelf, TSelf)">T.Max</see>(<see cref="INumber{TSelf}.Max(TSelf, TSelf)">T.Max</see>(<see cref="INumber{TSelf}.Max(TSelf, TSelf)">T.Max</see>(A, B), <see cref="INumber{TSelf}.Max(TSelf, TSelf)">T.Max</see>(C, D)), <see cref="INumber{TSelf}.Max(TSelf, TSelf)">T.Max</see>(<see cref="INumber{TSelf}.Max(TSelf, TSelf)">T.Max</see>(E, F), <see cref="INumber{TSelf}.Max(TSelf, TSelf)">T.Max</see>(G, H)))
    /// </c>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float HorizontalMax(this Vector256<float> vector) {
        if (AdvSimd.Arm64.IsSupported) {
            var vec = Vector128.Max(vector.GetLower(), vector.GetUpper());
            return AdvSimd.Arm64.MaxAcross(vec).ToScalar();
            }
        else if (AdvSimd.IsSupported) {
            var vec = Vector128.Max(vector.GetLower(), vector.GetUpper());
            vec = AdvSimd.Max(vec, AdvSimd.ReverseElement32(vec.AsInt64()).AsSingle()); // [Max(A,C), Max(B,D), _, _]
            vec = AdvSimd.Max(vec, AdvSimd.ExtractVector128(vec, vec, 2));          // swap the two 64-bit halves
            return vec.ToScalar();
            }
        else if (Vector128.IsHardwareAccelerated && Vector128<float>.IsSupported) {
            var vec = Vector128.Max(vector.GetLower(), vector.GetUpper());
            Vector128<float> shuffle;
            shuffle = vec.Shuffle2323();        // [A, B, C, D] → [C, D, _, _]
            vec = Vector128.Max(vec, shuffle);  // [Max(A,C), Max(B,D), _, _]
            shuffle = vec.Shuffle1010();        // [Max(A,C), Max(B,D), _, _] → [Max(B,D), Max(A,C), _, _]
            vec = Vector128.Max(vec, shuffle);  // [Max(A,C,B,D), _, _, _]
            return vec.ToScalar();
            }
        else {
            return float.Max(
                float.Max(float.Max(vector[0], vector[1]), float.Max(vector[2], vector[3])),
                float.Max(float.Max(vector[4], vector[5]), float.Max(vector[6], vector[7]))
                );
            }
        }

    /// <inheritdoc
    ///cref="HorizontalMax(Vector128{float})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double HorizontalMax(this Vector256<double> vector) {
        if (Vector256.IsHardwareAccelerated && Vector256<double>.IsSupported) {
            Vector256<double> shuffle;
            shuffle = vector.Shuffle2323();             // [A, B, C, D] → [C, D, _, _]
            vector = Vector256.Max(vector, shuffle);    // [Max(A,C), Max(B,D), _, _]
            shuffle = vector.Shuffle1010();             // [Max(A,C), Max(B,D), _, _] → [Max(B,D), Max(A,C), _, _]
            vector = Vector256.Max(vector, shuffle);    // [Max(A,C,B,D), _, _, _]
            return vector.ToScalar();
            }
        else {
            return double.Max(double.Max(vector[0], vector[1]), double.Max(vector[2], vector[3]));
            }
        }

    /// <inheritdoc
    ///cref="HorizontalMin(Vector256{float})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong HorizontalMin(this Vector256<ulong> vector) {
        if (Vector256.IsHardwareAccelerated && Vector256<ulong>.IsSupported) {
            Vector256<ulong> shuffle;
            shuffle = vector.Shuffle2323();             // [A, B, C, D] → [C, D, _, _]
            vector = Vector256.Min(vector, shuffle);    // [Min(A,C), Min(B,D), _, _]
            shuffle = vector.Shuffle1010();             // [Min(A,C), Min(B,D), _, _] → [Min(B,D), Min(A,C), _, _]
            vector = Vector256.Min(vector, shuffle);    // [Min(A,C,B,D), _, _, _]
            return vector.ToScalar();
            }
        else {
            return ulong.Min(ulong.Min(vector[0], vector[1]), ulong.Min(vector[2], vector[3]));
            }
        }

    /// <inheritdoc
    ///cref="HorizontalMin(Vector256{float})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long HorizontalMin(this Vector256<long> vector) {
        if (Vector256.IsHardwareAccelerated && Vector256<long>.IsSupported) {
            Vector256<long> shuffle;
            shuffle = vector.Shuffle2323();             // [A, B, C, D] → [C, D, _, _]
            vector = Vector256.Min(vector, shuffle);    // [Min(A,C), Min(B,D), _, _]
            shuffle = vector.Shuffle1010();             // [Min(A,C), Min(B,D), _, _] → [Min(B,D), Min(A,C), _, _]
            vector = Vector256.Min(vector, shuffle);    // [Min(A,C,B,D), _, _, _]
            return vector.ToScalar();
            }
        else {
            return long.Min(long.Min(vector[0], vector[1]), long.Min(vector[2], vector[3]));
            }
        }

    /// <inheritdoc
    ///cref="HorizontalMin(Vector128{float})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint HorizontalMin(this Vector128<uint> vector) {
        if (AdvSimd.Arm64.IsSupported) {
            return AdvSimd.Arm64.MinAcross(vector).ToScalar();
            }
        else if (Vector128.IsHardwareAccelerated && Vector128<uint>.IsSupported) {
            Vector128<uint> shuffle;
            shuffle = vector.Shuffle2323();             // [A, B, C, D] → [C, D, _, _]
            vector = Vector128.Min(vector, shuffle);    // [Min(A,C), Min(B,D), _, _]
            shuffle = vector.Shuffle1010();             // [Min(A,C), Min(B,D), _, _] → [Min(B,D), Min(A,C), _, _]
            vector = Vector128.Min(vector, shuffle);    // [Min(A,C,B,D), _, _, _]
            return vector.ToScalar();
            }
        else {
            return uint.Min(uint.Min(vector[0], vector[1]), uint.Min(vector[2], vector[3]));
            }
        }

    /// <inheritdoc
    ///cref="HorizontalMin(Vector128{float})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int HorizontalMin(this Vector128<int> vector) {
        if (AdvSimd.Arm64.IsSupported) {
            return AdvSimd.Arm64.MinAcross(vector).ToScalar();
            }
        else if (Vector128.IsHardwareAccelerated && Vector128<int>.IsSupported) {
            Vector128<int> shuffle;
            shuffle = vector.Shuffle2323();             // [A, B, C, D] → [C, D, _, _]
            vector = Vector128.Min(vector, shuffle);    // [Min(A,C), Min(B,D), _, _]
            shuffle = vector.Shuffle1010();             // [Min(A,C), Min(B,D), _, _] → [Min(B,D), Min(A,C), _, _]
            vector = Vector128.Min(vector, shuffle);    // [Min(A,C,B,D), _, _, _]
            return vector.ToScalar();
            }
        else {
            return int.Min(int.Min(vector[0], vector[1]), int.Min(vector[2], vector[3]));
            }
        }

    /// <summary> Equivalent to:
    /// <para/><c><see cref="INumber{TSelf}.Min(TSelf, TSelf)">T.Min</see>(<see cref="INumber{TSelf}.Min(TSelf, TSelf)">T.Min</see>(X, Y), <see cref="INumber{TSelf}.Min(TSelf, TSelf)">T.Min</see>(Z, W))
    /// </c>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float HorizontalMin(this Vector128<float> vector) {
        if (AdvSimd.Arm64.IsSupported) {
            return AdvSimd.Arm64.MinAcross(vector).ToScalar();
            }
        else if (Vector128.IsHardwareAccelerated && Vector128<float>.IsSupported) {
            Vector128<float> shuffle;
            shuffle = vector.Shuffle2323();             // [A, B, C, D] → [C, D, _, _]
            vector = Vector128.Min(vector, shuffle);    // [Min(A,C), Min(B,D), _, _]
            shuffle = vector.Shuffle1010();             // [Min(A,C), Min(B,D), _, _] → [Min(B,D), Min(A,C), _, _]
            vector = Vector128.Min(vector, shuffle);    // [Min(A,C,B,D), _, _, _]
            return vector.ToScalar();
            }
        else {
            return float.Min(float.Min(vector[0], vector[1]), float.Min(vector[2], vector[3]));
            }
        }

    /// <summary> Equivalent to:
    /// <para/><c><see cref="INumber{TSelf}.Min(TSelf, TSelf)">T.Min</see>(<see cref="INumber{TSelf}.Min(TSelf, TSelf)">T.Min</see>(<see cref="INumber{TSelf}.Min(TSelf, TSelf)">T.Min</see>(A, B), <see cref="INumber{TSelf}.Min(TSelf, TSelf)">T.Min</see>(C, D)), <see cref="INumber{TSelf}.Min(TSelf, TSelf)">T.Min</see>(<see cref="INumber{TSelf}.Min(TSelf, TSelf)">T.Min</see>(E, F), <see cref="INumber{TSelf}.Min(TSelf, TSelf)">T.Min</see>(G, H)))
    /// </c>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float HorizontalMin(this Vector256<float> vector) {
        if (AdvSimd.Arm64.IsSupported) {
            var vec = Vector128.Min(vector.GetLower(), vector.GetUpper());
            return AdvSimd.Arm64.MinAcross(vec).ToScalar();
            }
        else if (AdvSimd.IsSupported) {
            var vec = Vector128.Min(vector.GetLower(), vector.GetUpper());
            vec = AdvSimd.Min(vec, AdvSimd.ReverseElement32(vec.AsInt64()).AsSingle()); // [Min(A,C), Min(B,D), _, _]
            vec = AdvSimd.Min(vec, AdvSimd.ExtractVector128(vec, vec, 2));              // swap the two 64-bit halves
            return vec.ToScalar();
            }
        else if (Vector128.IsHardwareAccelerated && Vector128<float>.IsSupported) {
            var vec = Vector128.Min(vector.GetLower(), vector.GetUpper());
            Vector128<float> shuffle;
            shuffle = vec.Shuffle2323();        // [A, B, C, D] → [C, D, _, _]
            vec = Vector128.Min(vec, shuffle);  // [Min(A,C), Min(B,D), _, _]
            shuffle = vec.Shuffle1010();        // [Min(A,C), Min(B,D), _, _] → [Min(B,D), Min(A,C), _, _]
            vec = Vector128.Min(vec, shuffle);  // [Min(A,C,B,D), _, _, _]
            return vec.ToScalar();
            }
        else {
            return float.Min(
                float.Min(float.Min(vector[0], vector[1]), float.Min(vector[2], vector[3])),
                float.Min(float.Min(vector[4], vector[5]), float.Min(vector[6], vector[7]))
                );
            }
        }

    /// <inheritdoc
    ///cref="HorizontalMin(Vector128{float})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double HorizontalMin(this Vector256<double> vector) {
        if (Vector256.IsHardwareAccelerated && Vector256<double>.IsSupported) {
            Vector256<double> shuffle;
            shuffle = vector.Shuffle2323();             // [A, B, C, D] → [C, D, _, _]
            vector = Vector256.Min(vector, shuffle);    // [Min(A,C), Min(B,D), _, _]
            shuffle = vector.Shuffle1010();             // [Min(A,C), Min(B,D), _, _] → [Min(B,D), Min(A,C), _, _]
            vector = Vector256.Min(vector, shuffle);    // [Min(A,C,B,D), _, _, _]
            return vector.ToScalar();
            }
        else {
            return double.Min(double.Min(vector[0], vector[1]), double.Min(vector[2], vector[3]));
            }
        }

    #endregion Horizontal Min/Max

    #region Load2x64

    /// <summary> Returns <c>(<paramref name="left"/>.X, <paramref name="left"/>.Y, <paramref name="right"/>.X, <paramref name="right"/>.Y)</c>
    /// <para/> The data is loaded unsafely from the <paramref name="left"/> and <paramref name="right"/> <typeparamref name="TData"/> while assuming that each properly points to two <see langword="float"/> values in-sequence.
    /// </summary>
    /// <remarks><inheritdoc cref="Load2x64(float*, float*)"/></remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe Vector128<float> Load2x64<TData>(in TData left, in TData right)
        where TData : unmanaged {
        return Load2x64((float*)Unsafe.AsPointer(in left), (float*)Unsafe.AsPointer(in right));
        }

    /// <summary> Returns <c>(<paramref name="left"/>[0], <paramref name="left"/>[1]], <paramref name="right"/>[0], <paramref name="right"/>)</c>
    /// <para/> The data is loaded unsafely from the <paramref name="left"/> and <paramref name="right"/> <see langword="float"/>*s while assuming that each properly points to two <see langword="float"/> values in-sequence.
    /// </summary>
    /// <remarks> No <see cref="Vector256{T}">Vector256</see>&lt;<see langword="double"/>&gt; equivalent to this function is included, as <see cref="Vector256.Create(Vector128{double}, Vector128{double})"/> does not have the same jitting deficiency that <see cref="Vector128.Create(Vector64{float}, Vector64{float})"/> does on x86 systems (as of 2026).
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe Vector128<float> Load2x64(float* left, float* right) {
        if (Sse.IsSupported) {
            return Sse.LoadHigh(Vector128.CreateScalarUnsafe(*(double*)left).AsSingle(), right);
            }
        else if (AdvSimd.IsSupported) {
            return AdvSimd.LoadVector64(left)
                    .ToVector128Unsafe()
                    .WithUpper(AdvSimd.LoadVector64(right));
            }
        else {
            return Vector128.Create(left[0], left[1], right[0], right[1]);
            }
        }

    #endregion Load2x64

    #region Pairwise Add (2-width)

    /// <inheritdoc
    /// cref="PairwiseAdd(Vector128{double}, Vector128{double})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<ulong> PairwiseAdd(Vector128<ulong> a, Vector128<ulong> b) {
        if (Sse2.IsSupported) {
            return Sse2.Add(Sse2.UnpackLow(a, b), Sse2.UnpackHigh(a, b));
            }
        else if (AdvSimd.Arm64.IsSupported) {
            return AdvSimd.Arm64.AddPairwise(a, b);
            }
        else {
            return Vector128.Create(a[0] + a[1], b[0] + b[1]);
            }
        }

    /// <inheritdoc
    /// cref="PairwiseAdd(Vector128{double}, Vector128{double})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<long> PairwiseAdd(Vector128<long> a, Vector128<long> b) {
        if (Sse2.IsSupported) {
            return Sse2.Add(Sse2.UnpackLow(a, b), Sse2.UnpackHigh(a, b));
            }
        else if (AdvSimd.Arm64.IsSupported) {
            return AdvSimd.Arm64.AddPairwise(a, b);
            }
        else {
            return Vector128.Create(a[0] + a[1], b[0] + b[1]);
            }
        }

    /// <summary> Returns <c>(<paramref name="a"/>.X + <paramref name="a"/>.Y, <paramref name="b"/>.X + <paramref name="b"/>.Y)
    /// </c></summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<double> PairwiseAdd(Vector128<double> a, Vector128<double> b) {
        if (Sse3.IsSupported) {
            return Sse3.HorizontalAdd(a, b);
            }
        else if (AdvSimd.Arm64.IsSupported) {
            return AdvSimd.Arm64.AddPairwise(a, b);
            }
        else {
            return Vector128.Create(a[0] + a[1], b[0] + b[1]);
            }
        }

    #endregion Pairwise Add (2-width)

    #region Pairwise Add (4-Width)

    /// <inheritdoc
    /// cref="PairwiseAdd(Vector128{float}, Vector128{float})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<uint> PairwiseAdd(Vector128<uint> a, Vector128<uint> b) {
        if (Ssse3.IsSupported) {
            return Ssse3.HorizontalAdd(a.AsInt32(), b.AsInt32()).AsUInt32();
            }
        else if (AdvSimd.Arm64.IsSupported) {
            return AdvSimd.Arm64.AddPairwise(a, b);
            }
        else {
            return Vector128.Create(a[0] + a[1], a[2] + a[3], b[0] + b[1], b[2] + b[3]);
            }
        }

    /// <inheritdoc
    /// cref="PairwiseAdd(Vector128{float}, Vector128{float})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<int> PairwiseAdd(Vector128<int> a, Vector128<int> b) {
        if (Ssse3.IsSupported) {
            return Ssse3.HorizontalAdd(a, b);
            }
        else if (AdvSimd.Arm64.IsSupported) {
            return AdvSimd.Arm64.AddPairwise(a, b);
            }
        else {
            return Vector128.Create(a[0] + a[1], a[2] + a[3], b[0] + b[1], b[2] + b[3]);
            }
        }

    /// <inheritdoc
    /// cref="PairwiseAdd(Vector128{float}, Vector128{float})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector256<ulong> PairwiseAdd(Vector256<ulong> a, Vector256<ulong> b) {
        if (Vector256.IsHardwareAccelerated && Vector256<ulong>.IsSupported) {
            return Vector256.Create(
                PairwiseAdd(a.GetLower(), a.GetUpper()),
                PairwiseAdd(b.GetLower(), b.GetUpper())
                );
            }
        else {
            return Vector256.Create(a[0] + a[1], a[2] + a[3], b[0] + b[1], b[2] + b[3]);
            }
        }

    /// <inheritdoc
    /// cref="PairwiseAdd(Vector128{float}, Vector128{float})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector256<long> PairwiseAdd(Vector256<long> a, Vector256<long> b) {
        if (Vector256.IsHardwareAccelerated && Vector256<long>.IsSupported) {
            return Vector256.Create(
                PairwiseAdd(a.GetLower(), a.GetUpper()),
                PairwiseAdd(b.GetLower(), b.GetUpper())
                );
            }
        else {
            return Vector256.Create(a[0] + a[1], a[2] + a[3], b[0] + b[1], b[2] + b[3]);
            }
        }

    /// <summary> Returns <c>(<paramref name="a"/>.X + <paramref name="a"/>.Y, <paramref name="a"/>.Z + <paramref name="a"/>.W, <paramref name="b"/>.X + <paramref name="b"/>.Y, <paramref name="b"/>.Z + <paramref name="b"/>.W)
    /// </c></summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<float> PairwiseAdd(Vector128<float> a, Vector128<float> b) {
        if (Sse3.IsSupported) {
            return Sse3.HorizontalAdd(a, b);
            }
        else if (AdvSimd.Arm64.IsSupported) {
            return AdvSimd.Arm64.AddPairwise(a, b);
            }
        else {
            return Vector128.Create(a[0] + a[1], a[2] + a[3], b[0] + b[1], b[2] + b[3]);
            }
        }

    /// <inheritdoc
    /// cref="PairwiseAdd(Vector128{float}, Vector128{float})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector256<double> PairwiseAdd(Vector256<double> a, Vector256<double> b) {
        if (Avx2.IsSupported) {
            return Avx2.Permute4x64(Avx.HorizontalAdd(a, b), 0b_11_01_10_00);
            }
        else if (Vector256.IsHardwareAccelerated && Vector256<double>.IsSupported) {
            return Vector256.Create(
                PairwiseAdd(a.GetLower(), a.GetUpper()),
                PairwiseAdd(b.GetLower(), b.GetUpper())
                );
            }
        else {
            return Vector256.Create(a[0] + a[1], a[2] + a[3], b[0] + b[1], b[2] + b[3]);
            }
        }

    #endregion Pairwise Add (4-Width)

    #region Pairwise Add (8-Width)

    /// <summary> Returns <c>(<paramref name="a"/>.A + <paramref name="a"/>.B, <paramref name="a"/>.C + <paramref name="a"/>.D, <paramref name="a"/>.E + <paramref name="a"/>.F, <paramref name="a"/>.G + <paramref name="a"/>.H, <paramref name="b"/>.A + <paramref name="b"/>.B, <paramref name="b"/>.C + <paramref name="b"/>.D, <paramref name="b"/>.E + <paramref name="b"/>.F, <paramref name="b"/>.G + <paramref name="b"/>.H)
    /// </c></summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector256<float> PairwiseAdd(Vector256<float> a, Vector256<float> b) {
        if (Avx2.IsSupported) {
            return Avx2.PermuteVar8x32(Avx.HorizontalAdd(a, b), Vector256.Create(0, 1, 4, 5, 2, 3, 6, 7));
            }
        else if (Vector256.IsHardwareAccelerated && Vector256<float>.IsSupported) {
            return Vector256.Create(
                PairwiseAdd(a.GetLower(), a.GetUpper()),
                PairwiseAdd(b.GetLower(), b.GetUpper())
                );
            }
        else {
            return Vector256.Create(
                a[0] + a[1], a[2] + a[3],
                a[4] + a[5], a[6] + a[7],
                b[0] + b[1], b[2] + b[3],
                b[4] + b[5], b[6] + b[7]
                );
            }
        }

    /// <inheritdoc
    /// cref="PairwiseAdd(Vector256{float}, Vector256{float})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector256<int> PairwiseAdd(Vector256<int> a, Vector256<int> b) {
        if (Avx2.IsSupported) {
            return Avx2.PermuteVar8x32(Avx2.HorizontalAdd(a, b), Vector256.Create(0, 1, 4, 5, 2, 3, 6, 7));
            }
        else if (Vector256.IsHardwareAccelerated && Vector256<int>.IsSupported) {
            return Vector256.Create(
                PairwiseAdd(a.GetLower(), a.GetUpper()),
                PairwiseAdd(b.GetLower(), b.GetUpper())
                );
            }
        else {
            return Vector256.Create(
                a[0] + a[1], a[2] + a[3],
                a[4] + a[5], a[6] + a[7],
                b[0] + b[1], b[2] + b[3],
                b[4] + b[5], b[6] + b[7]
                );
            }
        }

    /// <inheritdoc
    /// cref="PairwiseAdd(Vector256{float}, Vector256{float})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector256<uint> PairwiseAdd(Vector256<uint> a, Vector256<uint> b) {
        if (Avx2.IsSupported) {
            return Avx2.PermuteVar8x32(Avx2.HorizontalAdd(a.AsInt32(), b.AsInt32()), Vector256.Create(0, 1, 4, 5, 2, 3, 6, 7)).AsUInt32();
            }
        else if (Vector256.IsHardwareAccelerated && Vector256<uint>.IsSupported) {
            return Vector256.Create(
                PairwiseAdd(a.GetLower(), a.GetUpper()),
                PairwiseAdd(b.GetLower(), b.GetUpper())
                );
            }
        else {
            return Vector256.Create(
                a[0] + a[1], a[2] + a[3],
                a[4] + a[5], a[6] + a[7],
                b[0] + b[1], b[2] + b[3],
                b[4] + b[5], b[6] + b[7]
                );
            }
        }

    #endregion Pairwise Add (8-Width)

    #region Strided Pairwise Add

    /// <inheritdoc
    /// cref="StridedAdd(Vector128{float}, Vector128{float})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<uint> StridedAdd(Vector128<uint> a, Vector128<uint> b) {
        if (Sse.IsSupported) {
            return (Sse.Shuffle(a.AsSingle(), b.AsSingle(), 0b_01_00_01_00)
                  + Sse.Shuffle(a.AsSingle(), b.AsSingle(), 0b_11_10_11_10)
                      ).AsUInt32();
            }
        else if (AdvSimd.IsSupported) {
            return AdvSimd.Add(
                Vector128.Create(a.GetLower(), b.GetLower()),
                Vector128.Create(a.GetUpper(), b.GetUpper())
                );
            }
        else {
            return Vector128.Create(a[0] + a[2], a[1] + a[3], b[0] + b[2], b[1] + b[3]);
            }
        }

    /// <inheritdoc
    /// cref="StridedAdd(Vector128{float}, Vector128{float})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<int> StridedAdd(Vector128<int> a, Vector128<int> b) {
        if (Sse.IsSupported) {
            return (Sse.Shuffle(a.AsSingle(), b.AsSingle(), 0b_01_00_01_00)
                  + Sse.Shuffle(a.AsSingle(), b.AsSingle(), 0b_11_10_11_10)
                  ).AsInt32();
            }
        else if (AdvSimd.IsSupported) {
            return AdvSimd.Add(
                Vector128.Create(a.GetLower(), b.GetLower()),
                Vector128.Create(a.GetUpper(), b.GetUpper())
                );
            }
        else {
            return Vector128.Create(a[0] + a[2], a[1] + a[3], b[0] + b[2], b[1] + b[3]);
            }
        }

    /// <summary> Returns <c>(<paramref name="a"/>.X + <paramref name="a"/>.Z, <paramref name="a"/>.Y + <paramref name="a"/>.W, <paramref name="b"/>.X + <paramref name="b"/>.Z, <paramref name="b"/>.Y + <paramref name="b"/>.W)
    /// </c></summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<float> StridedAdd(Vector128<float> a, Vector128<float> b) {
        if (Sse.IsSupported) {
            return Sse.Add(
                Sse.Shuffle(a, b, 0b_01_00_01_00),
                Sse.Shuffle(a, b, 0b_11_10_11_10)
                );
            }
        else if (AdvSimd.IsSupported) {
            return AdvSimd.Add(
                Vector128.Create(a.GetLower(), b.GetLower()),
                Vector128.Create(a.GetUpper(), b.GetUpper())
                );
            }
        else {
            return Vector128.Create(a[0] + a[2], a[1] + a[3], b[0] + b[2], b[1] + b[3]);
            }
        }

    /// <inheritdoc
    /// cref="StridedAdd(Vector128{float}, Vector128{float})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector256<ulong> StridedAdd(Vector256<ulong> a, Vector256<ulong> b) {
        if (Vector256.IsHardwareAccelerated && Vector256<ulong>.IsSupported) {
            return Vector256.Create(
                a.GetLower() + a.GetUpper(),
                b.GetLower() + b.GetUpper()
                );
            }
        else {
            return Vector256.Create(a[0] + a[2], a[1] + a[3], b[0] + b[2], b[1] + b[3]);
            }
        }

    /// <inheritdoc
    /// cref="StridedAdd(Vector128{float}, Vector128{float})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector256<long> StridedAdd(Vector256<long> a, Vector256<long> b) {
        if (Vector256.IsHardwareAccelerated && Vector256<long>.IsSupported) {
            return Vector256.Create(
                a.GetLower() + a.GetUpper(),
                b.GetLower() + b.GetUpper()
                );
            }
        else {
            return Vector256.Create(a[0] + a[2], a[1] + a[3], b[0] + b[2], b[1] + b[3]);
            }
        }

    /// <inheritdoc
    /// cref="StridedAdd(Vector128{float}, Vector128{float})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector256<double> StridedAdd(Vector256<double> a, Vector256<double> b) {
        if (Vector256.IsHardwareAccelerated && Vector256<double>.IsSupported) {
            return Vector256.Create(
                a.GetLower() + a.GetUpper(),
                b.GetLower() + b.GetUpper()
                );
            }
        else {
            return Vector256.Create(a[0] + a[2], a[1] + a[3], b[0] + b[2], b[1] + b[3]);
            }
        }

    /// <summary> Returns <c>(<paramref name="a"/>.A + <paramref name="a"/>.E, <paramref name="a"/>.B + <paramref name="a"/>.F, <paramref name="a"/>.C + <paramref name="a"/>.G, <paramref name="a"/>.D + <paramref name="a"/>.H, <paramref name="b"/>.A + <paramref name="b"/>.E, <paramref name="b"/>.B + <paramref name="b"/>.F, <paramref name="b"/>.C + <paramref name="b"/>.G, <paramref name="b"/>.D + <paramref name="b"/>.H)
    /// </c></summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector256<float> StridedAdd(Vector256<float> a, Vector256<float> b) {
        if (Vector256.IsHardwareAccelerated && Vector256<float>.IsSupported) {
            return Vector256.Create(
                a.GetLower() + a.GetUpper(),
                b.GetLower() + b.GetUpper()
                );
            }
        else {
            return Vector256.Create(
                a[0] + a[4], a[1] + a[5], a[2] + a[6], a[3] + a[7],
                b[0] + b[4], b[1] + b[5], b[2] + b[6], b[3] + b[7]
                );
            }
        }

    /// <inheritdoc
    /// cref="StridedAdd(Vector256{float}, Vector256{float})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector256<uint> StridedAdd(Vector256<uint> a, Vector256<uint> b) {
        if (Vector256.IsHardwareAccelerated && Vector256<uint>.IsSupported) {
            return Vector256.Create(
                a.GetLower() + a.GetUpper(),
                b.GetLower() + b.GetUpper()
                );
            }
        else {
            return Vector256.Create(
                a[0] + a[4], a[1] + a[5], a[2] + a[6], a[3] + a[7],
                b[0] + b[4], b[1] + b[5], b[2] + b[6], b[3] + b[7]
                );
            }
        }

    /// <inheritdoc
    /// cref="StridedAdd(Vector256{float}, Vector256{float})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector256<int> StridedAdd(Vector256<int> a, Vector256<int> b) {
        if (Vector256.IsHardwareAccelerated && Vector256<int>.IsSupported) {
            return Vector256.Create(
                a.GetLower() + a.GetUpper(),
                b.GetLower() + b.GetUpper()
                );
            }
        else {
            return Vector256.Create(
                a[0] + a[4], a[1] + a[5], a[2] + a[6], a[3] + a[7],
                b[0] + b[4], b[1] + b[5], b[2] + b[6], b[3] + b[7]
                );
            }
        }

    #endregion Strided Pairwise Add

    #region Shuffle (0011)

    /// <inheritdoc
    /// cref="Shuffle0011(Vector128{float})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector256<double> Shuffle0011(this Vector256<double> vector) {
        if (Avx2.IsSupported) {
            return Avx2.Permute4x64(vector, 0b_01_01_00_00);
            }
        else if (Vector256.IsHardwareAccelerated && Vector256<double>.IsSupported) {
            return Vector256.Create(SplatX(vector.GetLower()), SplatY(vector.GetLower()));
            }
        else return Vector256.Create(vector[0], vector[0], vector[1], vector[1]);
        }

    /// <inheritdoc
    /// cref="Shuffle0011(Vector128{float})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector256<long> Shuffle0011(this Vector256<long> vector) {
        if (Avx2.IsSupported) {
            return Avx2.Permute4x64(vector, 0b_01_01_00_00);
            }
        else if (Vector256.IsHardwareAccelerated && Vector256<long>.IsSupported) {
            return Vector256.Create(SplatX(vector.GetLower()), SplatY(vector.GetLower()));
            }
        else return Vector256.Create(vector[0], vector[0], vector[1], vector[1]);
        }

    /// <inheritdoc
    /// cref="Shuffle0011(Vector128{float})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector256<ulong> Shuffle0011(this Vector256<ulong> vector) {
        if (Avx2.IsSupported) {
            return Avx2.Permute4x64(vector, 0b_01_01_00_00);
            }
        else if (Vector256.IsHardwareAccelerated && Vector256<ulong>.IsSupported) {
            return Vector256.Create(SplatX(vector.GetLower()), SplatY(vector.GetLower()));
            }
        else return Vector256.Create(vector[0], vector[0], vector[1], vector[1]);
        }

    /// <summary><c><paramref name="vector"/>(X, Y, Z, W) → <paramref name="vector"/>(X, X, Y, Y)
    /// </c></summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<float> Shuffle0011(this Vector128<float> vector) {
        if (Sse.IsSupported) {
            return Sse.Shuffle(vector, vector, 0b_01_01_00_00);
            }
        else if (AdvSimd.Arm64.IsSupported) {
            return AdvSimd.Arm64.ZipLow(vector, vector);
            }
        else return Vector128.Create(vector[0], vector[0], vector[1], vector[1]);
        }

    /// <summary><c><paramref name="vector"/>(A, B, C, D, E, F, G, H) → <paramref name="vector"/>(A, A, B, B, E, E, F, F)
    /// </c></summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector256<float> Shuffle0011(this Vector256<float> vector) {
        if (Avx.IsSupported) {
            return Avx.Shuffle(vector, vector, 0b_01_01_00_00);
            }
        else if (Vector256.IsHardwareAccelerated && Vector128.IsHardwareAccelerated && Vector256<float>.IsSupported && Vector128<float>.IsSupported) {
            return Vector256.Create(
                Shuffle0011(vector.GetLower()),
                Shuffle0011(vector.GetUpper())
                );
            }
        else return Vector256.Create(
            vector[0], vector[0], vector[1], vector[1],
            vector[4], vector[4], vector[5], vector[5]
            );
        }

    /// <inheritdoc
    /// cref="Shuffle0011(Vector128{float})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<int> Shuffle0011(this Vector128<int> vector) {
        if (Sse2.IsSupported) {
            return Sse2.Shuffle(vector, 0b_01_01_00_00);
            }
        else if (AdvSimd.Arm64.IsSupported) {
            return AdvSimd.Arm64.ZipLow(vector, vector);
            }
        else return Vector128.Create(vector[0], vector[0], vector[1], vector[1]);
        }

    /// <inheritdoc
    /// cref="Shuffle0011(Vector128{float})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<uint> Shuffle0011(this Vector128<uint> vector) {
        if (Sse2.IsSupported) {
            return Sse2.Shuffle(vector, 0b_01_01_00_00);
            }
        else if (AdvSimd.Arm64.IsSupported) {
            return AdvSimd.Arm64.ZipLow(vector, vector);
            }
        else return Vector128.Create(vector[0], vector[0], vector[1], vector[1]);
        }

    #endregion Shuffle (0011)

    #region Shuffle (2233)

    /// <inheritdoc
    /// cref="Shuffle2233(Vector128{float})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector256<double> Shuffle2233(this Vector256<double> vector) {
        if (Avx2.IsSupported) {
            return Avx2.Permute4x64(vector, 0b_11_11_10_10);
            }
        else if (Vector256.IsHardwareAccelerated && Vector256<double>.IsSupported) {
            return Vector256.Create(SplatX(vector.GetUpper()), SplatY(vector.GetUpper()));
            }
        else return Vector256.Create(vector[2], vector[2], vector[3], vector[3]);
        }

    /// <inheritdoc
    /// cref="Shuffle2233(Vector128{float})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector256<long> Shuffle2233(this Vector256<long> vector) {
        if (Avx2.IsSupported) {
            return Avx2.Permute4x64(vector, 0b_11_11_10_10);
            }
        else if (Vector256.IsHardwareAccelerated && Vector256<long>.IsSupported) {
            return Vector256.Create(SplatX(vector.GetUpper()), SplatY(vector.GetUpper()));
            }
        else return Vector256.Create(vector[2], vector[2], vector[3], vector[3]);
        }

    /// <inheritdoc
    /// cref="Shuffle2233(Vector128{float})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector256<ulong> Shuffle2233(this Vector256<ulong> vector) {
        if (Avx2.IsSupported) {
            return Avx2.Permute4x64(vector, 0b_11_11_10_10);
            }
        else if (Vector256.IsHardwareAccelerated && Vector256<ulong>.IsSupported) {
            return Vector256.Create(SplatX(vector.GetUpper()), SplatY(vector.GetUpper()));
            }
        else return Vector256.Create(vector[2], vector[2], vector[3], vector[3]);
        }

    /// <summary><c><paramref name="vector"/>(X, Y, Z, W) → <paramref name="vector"/>(Z, Z, W, W)
    /// </c></summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<float> Shuffle2233(this Vector128<float> vector) {
        if (Sse.IsSupported) {
            return Sse.Shuffle(vector, vector, 0b_11_11_10_10);
            }
        else if (AdvSimd.Arm64.IsSupported) {
            return AdvSimd.Arm64.ZipHigh(vector, vector);
            }
        else return Vector128.Create(vector[2], vector[2], vector[3], vector[3]);
        }

    /// <summary><c><paramref name="vector"/>(A, B, C, D, E, F, G, H) → <paramref name="vector"/>(C, C, D, D, G, G, H, H)
    /// </c></summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector256<float> Shuffle2233(this Vector256<float> vector) {
        if (Avx.IsSupported) {
            return Avx.Shuffle(vector, vector, 0b_11_11_10_10);
            }
        else if (Vector256.IsHardwareAccelerated && Vector128.IsHardwareAccelerated && Vector256<float>.IsSupported && Vector128<float>.IsSupported) {
            return Vector256.Create(
                Shuffle2233(vector.GetLower()),
                Shuffle2233(vector.GetUpper())
                );
            }
        else return Vector256.Create(
            vector[2], vector[2], vector[3], vector[3],
            vector[6], vector[6], vector[7], vector[7]
            );
        }

    /// <inheritdoc
    /// cref="Shuffle2233(Vector128{float})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<int> Shuffle2233(this Vector128<int> vector) {
        if (Sse2.IsSupported) {
            return Sse2.Shuffle(vector, 0b_11_11_10_10);
            }
        else if (AdvSimd.Arm64.IsSupported) {
            return AdvSimd.Arm64.ZipHigh(vector, vector);
            }
        else return Vector128.Create(vector[2], vector[2], vector[3], vector[3]);
        }

    /// <inheritdoc
    /// cref="Shuffle2233(Vector128{float})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<uint> Shuffle2233(this Vector128<uint> vector) {
        if (Sse2.IsSupported) {
            return Sse2.Shuffle(vector, 0b_11_11_10_10);
            }
        else if (AdvSimd.Arm64.IsSupported) {
            return AdvSimd.Arm64.ZipHigh(vector, vector);
            }
        else return Vector128.Create(vector[2], vector[2], vector[3], vector[3]);
        }

    #endregion Shuffle (2233)

    #region Shuffle (0022)

    /// <summary><c><paramref name="vector"/>(X, Y, Z, W) → <paramref name="vector"/>(X, X, Z, Z)
    /// </c></summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<float> Shuffle0022(this Vector128<float> vector) {
        if (Sse3.IsSupported) {
            return Sse3.MoveLowAndDuplicate(vector);
            }
        else if (AdvSimd.Arm64.IsSupported) {
            return AdvSimd.Arm64.TransposeEven(vector, vector);
            }
        else return Vector128.Create(vector[0], vector[0], vector[2], vector[2]);
        }

    /// <inheritdoc
    /// cref="Shuffle0022(Vector128{float})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector256<double> Shuffle0022(this Vector256<double> vector) {
        if (Avx.IsSupported) {
            return Avx.DuplicateEvenIndexed(vector);
            }
        else if (Vector256.IsHardwareAccelerated && Vector256<double>.IsSupported) {
            return Vector256.Create(SplatX(vector.GetLower()), SplatX(vector.GetUpper()));
            }
        else return Vector256.Create(vector[0], vector[0], vector[2], vector[2]);
        }

    /// <inheritdoc
    /// cref="Shuffle0022(Vector128{float})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector256<long> Shuffle0022(this Vector256<long> vector) {
        if (Avx2.IsSupported) {
            return Avx2.Permute4x64(vector, 0b_10_10_00_00);
            }
        else if (Vector256.IsHardwareAccelerated && Vector256<long>.IsSupported) {
            return Vector256.Create(SplatX(vector.GetLower()), SplatX(vector.GetUpper()));
            }
        else return Vector256.Create(vector[0], vector[0], vector[2], vector[2]);
        }

    /// <inheritdoc
    /// cref="Shuffle0022(Vector128{float})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector256<ulong> Shuffle0022(this Vector256<ulong> vector) {
        if (Avx2.IsSupported) {
            return Avx2.Permute4x64(vector, 0b_10_10_00_00);
            }
        else if (Vector256.IsHardwareAccelerated && Vector256<ulong>.IsSupported) {
            return Vector256.Create(SplatX(vector.GetLower()), SplatX(vector.GetUpper()));
            }
        else return Vector256.Create(vector[0], vector[0], vector[2], vector[2]);
        }

    /// <summary><c><paramref name="vector"/>(A, B, C, D, E, F, G, H) → <paramref name="vector"/>(A, A, C, C, E, E, G, G)
    /// </c></summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector256<float> Shuffle0022(this Vector256<float> vector) {
        if (Avx.IsSupported) {
            return Avx.DuplicateEvenIndexed(vector);
            }
        else if (Vector256.IsHardwareAccelerated && Vector128.IsHardwareAccelerated && Vector256<float>.IsSupported && Vector128<float>.IsSupported) {
            return Vector256.Create(
                Shuffle0022(vector.GetLower()),
                Shuffle0022(vector.GetUpper())
                );
            }
        else return Vector256.Create(
            vector[0], vector[0], vector[2], vector[2],
            vector[4], vector[4], vector[6], vector[6]
            );
        }

    /// <inheritdoc
    /// cref="Shuffle0022(Vector128{float})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<int> Shuffle0022(this Vector128<int> vector) {
        if (Sse2.IsSupported) {
            return Sse2.Shuffle(vector, 0b_10_10_00_00);
            }
        else if (AdvSimd.Arm64.IsSupported) {
            return AdvSimd.Arm64.TransposeEven(vector, vector);
            }
        else return Vector128.Create(vector[0], vector[0], vector[2], vector[2]);
        }

    /// <inheritdoc
    /// cref="Shuffle0022(Vector128{float})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<uint> Shuffle0022(this Vector128<uint> vector) {
        if (Sse2.IsSupported) {
            return Sse2.Shuffle(vector, 0b_10_10_00_00);
            }
        else if (AdvSimd.Arm64.IsSupported) {
            return AdvSimd.Arm64.TransposeEven(vector, vector);
            }
        else return Vector128.Create(vector[0], vector[0], vector[2], vector[2]);
        }

    #endregion Shuffle (0022)

    #region Shuffle (1133)

    /// <summary><c><paramref name="vector"/>(X, Y, Z, W) → <paramref name="vector"/>(Y, Y, W, W)
    /// </c></summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<float> Shuffle1133(this Vector128<float> vector) {
        if (Sse3.IsSupported) {
            return Sse3.MoveHighAndDuplicate(vector);
            }
        else if (AdvSimd.Arm64.IsSupported) {
            return AdvSimd.Arm64.TransposeOdd(vector, vector);
            }
        else return Vector128.Create(vector[1], vector[1], vector[3], vector[3]);
        }

    /// <inheritdoc
    /// cref="Shuffle1133(Vector128{float})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector256<double> Shuffle1133(this Vector256<double> vector) {
        if (Avx.IsSupported) {
            return Avx.Shuffle(vector, vector, 0b_1111);
            }
        else if (Vector256.IsHardwareAccelerated && Vector256<double>.IsSupported) {
            return Vector256.Create(SplatY(vector.GetLower()), SplatY(vector.GetUpper()));
            }
        else return Vector256.Create(vector[1], vector[1], vector[3], vector[3]);
        }

    /// <inheritdoc
    /// cref="Shuffle1133(Vector128{float})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector256<long> Shuffle1133(this Vector256<long> vector) {
        if (Avx2.IsSupported) {
            return Avx2.Permute4x64(vector, 0b_11_11_01_01);
            }
        else if (Vector256.IsHardwareAccelerated && Vector256<long>.IsSupported) {
            return Vector256.Create(SplatY(vector.GetLower()), SplatY(vector.GetUpper()));
            }
        else return Vector256.Create(vector[1], vector[1], vector[3], vector[3]);
        }

    /// <inheritdoc
    /// cref="Shuffle1133(Vector128{float})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector256<ulong> Shuffle1133(this Vector256<ulong> vector) {
        if (Avx2.IsSupported) {
            return Avx2.Permute4x64(vector, 0b_11_11_01_01);
            }
        else if (Vector256.IsHardwareAccelerated && Vector256<ulong>.IsSupported) {
            return Vector256.Create(SplatY(vector.GetLower()), SplatY(vector.GetUpper()));
            }
        else return Vector256.Create(vector[1], vector[1], vector[3], vector[3]);
        }

    /// <summary><c><paramref name="vector"/>(A, B, C, D, E, F, G, H) → <paramref name="vector"/>(B, B, D, D, F, F, H, H)
    /// </c></summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector256<float> Shuffle1133(this Vector256<float> vector) {
        if (Avx.IsSupported) {
            return Avx.DuplicateOddIndexed(vector);
            }
        else if (Vector256.IsHardwareAccelerated && Vector128.IsHardwareAccelerated && Vector256<float>.IsSupported && Vector128<float>.IsSupported) {
            return Vector256.Create(
                Shuffle1133(vector.GetLower()),
                Shuffle1133(vector.GetUpper())
                );
            }
        else return Vector256.Create(
            vector[1], vector[1], vector[3], vector[3],
            vector[5], vector[5], vector[7], vector[7]
            );
        }

    /// <inheritdoc
    /// cref="Shuffle1133(Vector128{float})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<int> Shuffle1133(this Vector128<int> vector) {
        if (Sse2.IsSupported) {
            return Sse2.Shuffle(vector, 0b_11_11_01_01);
            }
        else if (AdvSimd.Arm64.IsSupported) {
            return AdvSimd.Arm64.TransposeOdd(vector, vector);
            }
        else return Vector128.Create(vector[1], vector[1], vector[3], vector[3]);
        }

    /// <inheritdoc
    /// cref="Shuffle1133(Vector128{float})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<uint> Shuffle1133(this Vector128<uint> vector) {
        if (Sse2.IsSupported) {
            return Sse2.Shuffle(vector, 0b_11_11_01_01);
            }
        else if (AdvSimd.Arm64.IsSupported) {
            return AdvSimd.Arm64.TransposeOdd(vector, vector);
            }
        else return Vector128.Create(vector[1], vector[1], vector[3], vector[3]);
        }

    #endregion Shuffle (1133)

    #region Shuffle (0101)

    /// <summary><c><paramref name="vector"/>(X, Y, Z, W) → <paramref name="vector"/>(X, Y, X, Y)
    /// </c></summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<float> Shuffle0101(this Vector128<float> vector) {
        if (Sse.IsSupported) {
            return Sse.Shuffle(vector, vector, 0b_01_00_01_00);
            }
        else if (AdvSimd.Arm64.IsSupported) {
            return AdvSimd.Arm64.DuplicateSelectedScalarToVector128(vector.AsDouble(), 0).AsSingle();
            }
        else return Vector128.Create(vector[0], vector[1], vector[0], vector[1]);
        }

    /// <summary><c><paramref name="vector"/>(A, B, C, D, E, F, G, H) → <paramref name="vector"/>(A, B, A, B, E, F, E, F)
    /// </c></summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector256<float> Shuffle0101(this Vector256<float> vector) {
        if (Avx.IsSupported) {
            return Avx.Shuffle(vector, vector, 0b_01_00_01_00);
            }
        else if (Vector256.IsHardwareAccelerated && Vector128.IsHardwareAccelerated && Vector256<float>.IsSupported && Vector128<float>.IsSupported) {
            return Vector256.Create(
                Shuffle0101(vector.GetLower()),
                Shuffle0101(vector.GetUpper())
                );
            }
        else return Vector256.Create(
            vector[0], vector[1], vector[0], vector[1],
            vector[4], vector[5], vector[4], vector[5]);
        }

    /// <inheritdoc
    /// cref="Shuffle0101(Vector128{float})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector256<double> Shuffle0101(this Vector256<double> vector) {
        if (Avx2.IsSupported) {
            return Avx2.Permute4x64(vector, 0b_01_00_01_00);
            }
        else if (Vector256.IsHardwareAccelerated && Vector256<double>.IsSupported) {
            return Vector256.Create(vector.GetLower());
            }
        else return Vector256.Create(vector[0], vector[1], vector[0], vector[1]);
        }

    /// <inheritdoc
    /// cref="Shuffle0101(Vector128{float})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector256<long> Shuffle0101(this Vector256<long> vector) {
        if (Avx2.IsSupported) {
            return Avx2.Permute4x64(vector, 0b_01_00_01_00);
            }
        else if (Vector256.IsHardwareAccelerated && Vector256<long>.IsSupported) {
            return Vector256.Create(vector.GetLower());
            }
        else return Vector256.Create(vector[0], vector[1], vector[0], vector[1]);
        }

    /// <inheritdoc
    /// cref="Shuffle0101(Vector128{float})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector256<ulong> Shuffle0101(this Vector256<ulong> vector) {
        if (Avx2.IsSupported) {
            return Avx2.Permute4x64(vector, 0b_01_00_01_00);
            }
        else if (Vector256.IsHardwareAccelerated && Vector256<ulong>.IsSupported) {
            return Vector256.Create(vector.GetLower());
            }
        else return Vector256.Create(vector[0], vector[1], vector[0], vector[1]);
        }

    /// <inheritdoc
    /// cref="Shuffle0101(Vector128{float})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<int> Shuffle0101(this Vector128<int> vector) {
        if (Sse2.IsSupported) {
            return Sse2.Shuffle(vector, 0b_01_00_01_00);
            }
        else if (AdvSimd.Arm64.IsSupported) {
            return AdvSimd.Arm64.DuplicateSelectedScalarToVector128(vector.AsInt64(), 0).AsInt32();
            }
        else return Vector128.Create(vector[0], vector[1], vector[0], vector[1]);
        }

    /// <inheritdoc
    /// cref="Shuffle0101(Vector128{float})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<uint> Shuffle0101(this Vector128<uint> vector) {
        if (Sse2.IsSupported) {
            return Sse2.Shuffle(vector, 0b_01_00_01_00);
            }
        else if (AdvSimd.Arm64.IsSupported) {
            return AdvSimd.Arm64.DuplicateSelectedScalarToVector128(vector.AsUInt64(), 0).AsUInt32();
            }
        else return Vector128.Create(vector[0], vector[1], vector[0], vector[1]);
        }

    #endregion Shuffle (0101)

    #region Shuffle (0202)

    /// <summary><c><paramref name="vector"/>(X, Y, Z, W) → <paramref name="vector"/>(X, Z, X, Z)
    /// </c></summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<float> Shuffle0202(this Vector128<float> vector) {
        if (Sse.IsSupported) {
            return Sse.Shuffle(vector, vector, 0b_10_00_10_00);
            }
        else if (AdvSimd.Arm64.IsSupported) {
            return AdvSimd.Arm64.UnzipEven(vector, vector);
            }
        else return Vector128.Create(vector[0], vector[2], vector[0], vector[2]);
        }

    /// <summary><c><paramref name="vector"/>(A, B, C, D, E, F, G, H) → <paramref name="vector"/>(A, C, A, C, E, G, E, G)
    /// </c></summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector256<float> Shuffle0202(this Vector256<float> vector) {
        if (Avx.IsSupported) {
            return Avx.Shuffle(vector, vector, 0b_10_00_10_00);
            }
        else if (Vector256.IsHardwareAccelerated && Vector128.IsHardwareAccelerated && Vector256<float>.IsSupported && Vector128<float>.IsSupported) {
            return Vector256.Create(
                Shuffle0202(vector.GetLower()),
                Shuffle0202(vector.GetUpper())
                );
            }
        else return Vector256.Create(
            vector[0], vector[2], vector[0], vector[2],
            vector[4], vector[6], vector[4], vector[6]);
        }

    /// <inheritdoc
    /// cref="Shuffle0202(Vector128{float})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector256<double> Shuffle0202(this Vector256<double> vector) {
        if (Avx2.IsSupported) {
            return Avx2.Permute4x64(vector, 0b_10_00_10_00);
            }
        else if (Vector256.IsHardwareAccelerated && Vector256<double>.IsSupported) {
            return Vector256.Create(WeaveLow(vector.GetLower(), vector.GetUpper()));
            }
        else return Vector256.Create(vector[0], vector[2], vector[0], vector[2]);
        }

    /// <inheritdoc
    /// cref="Shuffle0202(Vector128{float})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector256<long> Shuffle0202(this Vector256<long> vector) {
        if (Avx2.IsSupported) {
            return Avx2.Permute4x64(vector, 0b_10_00_10_00);
            }
        else if (Vector256.IsHardwareAccelerated && Vector256<long>.IsSupported) {
            return Vector256.Create(WeaveLow(vector.GetLower(), vector.GetUpper()));
            }
        else return Vector256.Create(vector[0], vector[2], vector[0], vector[2]);
        }

    /// <inheritdoc
    /// cref="Shuffle0202(Vector128{float})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector256<ulong> Shuffle0202(this Vector256<ulong> vector) {
        if (Avx2.IsSupported) {
            return Avx2.Permute4x64(vector, 0b_10_00_10_00);
            }
        else if (Vector256.IsHardwareAccelerated && Vector256<ulong>.IsSupported) {
            return Vector256.Create(WeaveLow(vector.GetLower(), vector.GetUpper()));
            }
        else return Vector256.Create(vector[0], vector[2], vector[0], vector[2]);
        }

    /// <inheritdoc
    /// cref="Shuffle0202(Vector128{float})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<int> Shuffle0202(this Vector128<int> vector) {
        if (Sse2.IsSupported) {
            return Sse2.Shuffle(vector, 0b_10_00_10_00);
            }
        else if (AdvSimd.Arm64.IsSupported) {
            return AdvSimd.Arm64.UnzipEven(vector, vector);
            }
        else return Vector128.Create(vector[0], vector[2], vector[0], vector[2]);
        }

    /// <inheritdoc
    /// cref="Shuffle0202(Vector128{float})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<uint> Shuffle0202(this Vector128<uint> vector) {
        if (Sse2.IsSupported) {
            return Sse2.Shuffle(vector, 0b_10_00_10_00);
            }
        else if (AdvSimd.Arm64.IsSupported) {
            return AdvSimd.Arm64.UnzipEven(vector, vector);
            }
        else return Vector128.Create(vector[0], vector[2], vector[0], vector[2]);
        }

    #endregion Shuffle (0202)

    #region Shuffle (1313)

    /// <summary><c><paramref name="vector"/>(X, Y, Z, W) → <paramref name="vector"/>(Y, W, Y, W)
    /// </c></summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<float> Shuffle1313(this Vector128<float> vector) {
        if (Sse.IsSupported) {
            return Sse.Shuffle(vector, vector, 0b_11_01_11_01);
            }
        else if (AdvSimd.Arm64.IsSupported) {
            return AdvSimd.Arm64.UnzipOdd(vector, vector);
            }
        else return Vector128.Create(vector[1], vector[3], vector[1], vector[3]);
        }

    /// <summary><c><paramref name="vector"/>(A, B, C, D, E, F, G, H) → <paramref name="vector"/>(B, D, B, D, F, H, F, H)
    /// </c></summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector256<float> Shuffle1313(this Vector256<float> vector) {
        if (Avx.IsSupported) {
            return Avx.Shuffle(vector, vector, 0b_11_01_11_01);
            }
        else if (Vector256.IsHardwareAccelerated && Vector128.IsHardwareAccelerated && Vector256<float>.IsSupported && Vector128<float>.IsSupported) {
            return Vector256.Create(
                Shuffle1313(vector.GetLower()),
                Shuffle1313(vector.GetUpper())
                );
            }
        else return Vector256.Create(
            vector[1], vector[3], vector[1], vector[3],
            vector[5], vector[7], vector[5], vector[7]);
        }

    /// <inheritdoc
    /// cref="Shuffle1313(Vector128{float})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector256<double> Shuffle1313(this Vector256<double> vector) {
        if (Avx2.IsSupported) {
            return Avx2.Permute4x64(vector, 0b_11_01_11_01);
            }
        else if (Vector256.IsHardwareAccelerated && Vector256<double>.IsSupported) {
            return Vector256.Create(WeaveHigh(vector.GetLower(), vector.GetUpper()));
            }
        else return Vector256.Create(vector[1], vector[3], vector[1], vector[3]);
        }

    /// <inheritdoc
    /// cref="Shuffle1313(Vector128{float})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector256<long> Shuffle1313(this Vector256<long> vector) {
        if (Avx2.IsSupported) {
            return Avx2.Permute4x64(vector, 0b_11_01_11_01);
            }
        else if (Vector256.IsHardwareAccelerated && Vector256<long>.IsSupported) {
            return Vector256.Create(WeaveHigh(vector.GetLower(), vector.GetUpper()));
            }
        else return Vector256.Create(vector[1], vector[3], vector[1], vector[3]);
        }

    /// <inheritdoc
    /// cref="Shuffle1313(Vector128{float})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector256<ulong> Shuffle1313(this Vector256<ulong> vector) {
        if (Avx2.IsSupported) {
            return Avx2.Permute4x64(vector, 0b_11_01_11_01);
            }
        else if (Vector256.IsHardwareAccelerated && Vector256<ulong>.IsSupported) {
            return Vector256.Create(WeaveHigh(vector.GetLower(), vector.GetUpper()));
            }
        else return Vector256.Create(vector[1], vector[3], vector[1], vector[3]);
        }

    /// <inheritdoc
    /// cref="Shuffle1313(Vector128{float})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<int> Shuffle1313(this Vector128<int> vector) {
        if (Sse2.IsSupported) {
            return Sse2.Shuffle(vector, 0b_11_01_11_01);
            }
        else if (AdvSimd.Arm64.IsSupported) {
            return AdvSimd.Arm64.UnzipOdd(vector, vector);
            }
        else return Vector128.Create(vector[1], vector[3], vector[1], vector[3]);
        }

    /// <inheritdoc
    /// cref="Shuffle1313(Vector128{float})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<uint> Shuffle1313(this Vector128<uint> vector) {
        if (Sse2.IsSupported) {
            return Sse2.Shuffle(vector, 0b_11_01_11_01);
            }
        else if (AdvSimd.Arm64.IsSupported) {
            return AdvSimd.Arm64.UnzipOdd(vector, vector);
            }
        else return Vector128.Create(vector[1], vector[3], vector[1], vector[3]);
        }

    #endregion Shuffle (1313)

    #region Shuffle (2323)

    /// <summary><c><paramref name="vector"/>(X, Y, Z, W) → <paramref name="vector"/>(Z, W, Z, W)
    /// </c></summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<float> Shuffle2323(this Vector128<float> vector) {
        if (Sse.IsSupported) {
            return Sse.Shuffle(vector, vector, 0b_11_10_11_10);
            }
        else if (AdvSimd.Arm64.IsSupported) {
            return AdvSimd.Arm64.DuplicateSelectedScalarToVector128(vector.AsDouble(), 1).AsSingle();
            }
        else return Vector128.Create(vector[2], vector[3], vector[2], vector[3]);
        }

    /// <summary><c><paramref name="vector"/>(A, B, C, D, E, F, G, H) → <paramref name="vector"/>(C, D, C, D, G, H, G, H)
    /// </c></summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector256<float> Shuffle2323(this Vector256<float> vector) {
        if (Avx.IsSupported) {
            return Avx.Shuffle(vector, vector, 0b_11_10_11_10);
            }
        else if (Vector256.IsHardwareAccelerated && Vector128.IsHardwareAccelerated && Vector256<float>.IsSupported && Vector128<float>.IsSupported) {
            return Vector256.Create(
                Shuffle2323(vector.GetLower()),
                Shuffle2323(vector.GetUpper())
                );
            }
        else return Vector256.Create(
            vector[2], vector[3], vector[2], vector[3],
            vector[6], vector[7], vector[6], vector[7]);
        }

    /// <inheritdoc
    /// cref="Shuffle2323(Vector128{float})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector256<double> Shuffle2323(this Vector256<double> vector) {
        if (Avx2.IsSupported) {
            return Avx2.Permute4x64(vector, 0b_11_10_11_10);
            }
        else if (Vector256.IsHardwareAccelerated && Vector256<double>.IsSupported) {
            return Vector256.Create(vector.GetUpper());
            }
        else return Vector256.Create(vector[2], vector[3], vector[2], vector[3]);
        }

    /// <inheritdoc
    /// cref="Shuffle2323(Vector128{float})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector256<long> Shuffle2323(this Vector256<long> vector) {
        if (Avx2.IsSupported) {
            return Avx2.Permute4x64(vector, 0b_11_10_11_10);
            }
        else if (Vector256.IsHardwareAccelerated && Vector256<long>.IsSupported) {
            return Vector256.Create(vector.GetUpper());
            }
        else return Vector256.Create(vector[2], vector[3], vector[2], vector[3]);
        }

    /// <inheritdoc
    /// cref="Shuffle2323(Vector128{float})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<int> Shuffle2323(this Vector128<int> vector) {
        if (Sse2.IsSupported) {
            return Sse2.Shuffle(vector, 0b_11_10_11_10);
            }
        else if (AdvSimd.Arm64.IsSupported) {
            return AdvSimd.Arm64.DuplicateSelectedScalarToVector128(vector.AsInt64(), 1).AsInt32();
            }
        else return Vector128.Create(vector[2], vector[3], vector[2], vector[3]);
        }

    /// <inheritdoc
    /// cref="Shuffle2323(Vector128{float})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector256<ulong> Shuffle2323(this Vector256<ulong> vector) {
        if (Avx2.IsSupported) {
            return Avx2.Permute4x64(vector, 0b_11_10_11_10);
            }
        else if (Vector256.IsHardwareAccelerated && Vector256<ulong>.IsSupported) {
            return Vector256.Create(vector.GetUpper());
            }
        else return Vector256.Create(vector[2], vector[3], vector[2], vector[3]);
        }

    /// <inheritdoc
    /// cref="Shuffle2323(Vector128{float})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<uint> Shuffle2323(this Vector128<uint> vector) {
        if (Sse2.IsSupported) {
            return Sse2.Shuffle(vector, 0b_11_10_11_10);
            }
        else if (AdvSimd.Arm64.IsSupported) {
            return AdvSimd.Arm64.DuplicateSelectedScalarToVector128(vector.AsUInt64(), 1).AsUInt32();
            }
        else return Vector128.Create(vector[2], vector[3], vector[2], vector[3]);
        }

    #endregion Shuffle (2323)

    #region Shuffle (1010)

    /// <summary><c><paramref name="vector"/>(X, Y, Z, W) → <paramref name="vector"/>(Y, X, Y, X)
    /// </c></summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<float> Shuffle1010(this Vector128<float> vector) {
        if (Sse.IsSupported) {
            return Sse.Shuffle(vector, vector, 0b_00_01_00_01);
            }
        else if (AdvSimd.Arm64.IsSupported) {
            Vector128<ulong> dup = AdvSimd.Arm64.DuplicateSelectedScalarToVector128(vector.AsUInt64(), 0);
            return AdvSimd.ReverseElement32(dup).AsSingle();
            }
        else return Vector128.Create(vector[1], vector[0], vector[1], vector[0]);
        }

    /// <inheritdoc
    /// cref="Shuffle1010(Vector128{float})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector256<double> Shuffle1010(this Vector256<double> vector) {
        if (Avx2.IsSupported) {
            return Avx2.Permute4x64(vector, 0b_00_01_00_01);
            }
        else if (Avx.IsSupported) {
            var swappedLanes = Avx.Permute(vector, 0b_0101);
            return Avx.Permute2x128(swappedLanes, swappedLanes, 0b_0000_0000);
            }
        else if (Vector256.IsHardwareAccelerated && Vector256<double>.IsSupported
              && Vector128.IsHardwareAccelerated && Vector128<double>.IsSupported) {
            return Vector256.Create(Vector128.Shuffle(vector.GetLower(), Vector128.Create(1L, 0L)));
            }
        else return Vector256.Create(vector[1], vector[0], vector[1], vector[0]);
        }

    /// <inheritdoc
    /// cref="Shuffle1010(Vector128{float})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector256<long> Shuffle1010(this Vector256<long> vector) {
        if (Avx2.IsSupported) {
            return Avx2.Permute4x64(vector, 0b_00_01_00_01);
            }
        else if (Vector256.IsHardwareAccelerated && Vector256<long>.IsSupported) {
            return Shuffle1010(vector.AsDouble()).AsInt64();
            }
        else return Vector256.Create(vector[1], vector[0], vector[1], vector[0]);
        }

    /// <inheritdoc
    /// cref="Shuffle1010(Vector128{float})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<int> Shuffle1010(this Vector128<int> vector) {
        if (Sse2.IsSupported) {
            return Sse2.Shuffle(vector, 0b_00_01_00_01);
            }
        else if (AdvSimd.Arm64.IsSupported) {
            Vector128<ulong> dup = AdvSimd.Arm64.DuplicateSelectedScalarToVector128(vector.AsUInt64(), 0);
            return AdvSimd.ReverseElement32(dup).AsInt32();
            }
        else return Vector128.Create(vector[1], vector[0], vector[1], vector[0]);
        }

    /// <inheritdoc
    /// cref="Shuffle1010(Vector128{float})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector256<ulong> Shuffle1010(this Vector256<ulong> vector) {
        if (Avx2.IsSupported) {
            return Avx2.Permute4x64(vector, 0b_00_01_00_01);
            }
        else if (Vector256.IsHardwareAccelerated && Vector256<ulong>.IsSupported) {
            return Shuffle1010(vector.AsDouble()).AsUInt64();
            }
        else return Vector256.Create(vector[1], vector[0], vector[1], vector[0]);
        }

    /// <inheritdoc
    /// cref="Shuffle1010(Vector128{float})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<uint> Shuffle1010(this Vector128<uint> vector) {
        if (Sse2.IsSupported) {
            return Sse2.Shuffle(vector, 0b_00_01_00_01);
            }
        else if (AdvSimd.Arm64.IsSupported) {
            Vector128<ulong> dup = AdvSimd.Arm64.DuplicateSelectedScalarToVector128(vector.AsUInt64(), 0);
            return AdvSimd.ReverseElement32(dup).AsUInt32();
            }
        else return Vector128.Create(vector[1], vector[0], vector[1], vector[0]);
        }

    #endregion Shuffle (1010)

    #region Shuffle (3232)

    /// <summary><c><paramref name="vector"/>(X, Y, Z, W) → <paramref name="vector"/>(W, Z, W, Z)
    /// </c></summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<float> Shuffle3232(this Vector128<float> vector) {
        if (Sse.IsSupported) {
            return Sse.Shuffle(vector, vector, 0b_10_11_10_11);
            }
        else if (AdvSimd.Arm64.IsSupported) {
            Vector128<ulong> dup = AdvSimd.Arm64.DuplicateSelectedScalarToVector128(vector.AsUInt64(), 1);
            return AdvSimd.ReverseElement32(dup).AsSingle();
            }
        else return Vector128.Create(vector[3], vector[2], vector[3], vector[2]);
        }

    /// <inheritdoc
    /// cref="Shuffle3232(Vector128{float})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector256<double> Shuffle3232(this Vector256<double> vector) {
        if (Avx2.IsSupported) {
            return Avx2.Permute4x64(vector, 0b_10_11_10_11);
            }
        else if (Avx.IsSupported) {
            var swappedLanes = Avx.Permute(vector, 0b_0101);
            return Avx.Permute2x128(swappedLanes, swappedLanes, 0b_0001_0001);
            }
        else if (Vector256.IsHardwareAccelerated && Vector256<double>.IsSupported
              && Vector128.IsHardwareAccelerated && Vector128<double>.IsSupported) {
            return Vector256.Create(Vector128.Shuffle(vector.GetUpper(), Vector128.Create(1L, 0L)));
            }
        else return Vector256.Create(vector[3], vector[2], vector[3], vector[2]);
        }

    /// <inheritdoc
    /// cref="Shuffle3232(Vector128{float})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector256<long> Shuffle3232(this Vector256<long> vector) {
        if (Avx2.IsSupported) {
            return Avx2.Permute4x64(vector, 0b_10_11_10_11);
            }
        else if (Vector256.IsHardwareAccelerated && Vector256<long>.IsSupported) {
            return Shuffle3232(vector.AsDouble()).AsInt64();
            }
        else return Vector256.Create(vector[3], vector[2], vector[3], vector[2]);
        }

    /// <inheritdoc
    /// cref="Shuffle3232(Vector128{float})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<int> Shuffle3232(this Vector128<int> vector) {
        if (Sse2.IsSupported) {
            return Sse2.Shuffle(vector, 0b_10_11_10_11);
            }
        else if (AdvSimd.Arm64.IsSupported) {
            Vector128<ulong> dup = AdvSimd.Arm64.DuplicateSelectedScalarToVector128(vector.AsUInt64(), 1);
            return AdvSimd.ReverseElement32(dup).AsInt32();
            }
        else return Vector128.Create(vector[3], vector[2], vector[3], vector[2]);
        }

    /// <inheritdoc
    /// cref="Shuffle3232(Vector128{float})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector256<ulong> Shuffle3232(this Vector256<ulong> vector) {
        if (Avx2.IsSupported) {
            return Avx2.Permute4x64(vector, 0b_10_11_10_11);
            }
        else if (Vector256.IsHardwareAccelerated && Vector256<ulong>.IsSupported) {
            return Shuffle3232(vector.AsDouble()).AsUInt64();
            }
        else return Vector256.Create(vector[3], vector[2], vector[3], vector[2]);
        }

    /// <inheritdoc
    /// cref="Shuffle3232(Vector128{float})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<uint> Shuffle3232(this Vector128<uint> vector) {
        if (Sse2.IsSupported) {
            return Sse2.Shuffle(vector, 0b_10_11_10_11);
            }
        else if (AdvSimd.Arm64.IsSupported) {
            Vector128<ulong> dup = AdvSimd.Arm64.DuplicateSelectedScalarToVector128(vector.AsUInt64(), 1);
            return AdvSimd.ReverseElement32(dup).AsUInt32();
            }
        else return Vector128.Create(vector[3], vector[2], vector[3], vector[2]);
        }

    #endregion Shuffle (3232)

    #region Splat

    /// <summary><c><paramref name="vector"/>(X, Y, Z, W) → <paramref name="vector"/>(W, W, W, W)
    /// </c></summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector256<T> SplatW<T>(in Vector256<T> vector) {
        if (typeof(T) == typeof(float) || typeof(T) == typeof(int) || typeof(T) == typeof(uint)) {
            var v = Unsafe.BitCast<Vector256<T>, Vector256<float>>(vector);
            if (Avx.IsSupported) {
                var splattedLanes = Avx.Permute(v, 0b_11_11_11_11);
                return Unsafe.BitCast<Vector256<float>, Vector256<T>>(
                    Avx.Permute2x128(splattedLanes, splattedLanes, 0b_00_00_00_00)
                    );
                }
            else return Vector256.Create(vector[3]);
            }
        else if (typeof(T) == typeof(double) || typeof(T) == typeof(long) || typeof(T) == typeof(ulong)) {
            var v = Unsafe.BitCast<Vector256<T>, Vector256<double>>(vector);
            if (Avx2.IsSupported) {
                return Unsafe.BitCast<Vector256<double>, Vector256<T>>(
                    Avx2.Permute4x64(v, 0b_11_11_11_11)
                    );
                }
            else if (Avx.IsSupported) {
                var splattedLanes = Avx.Permute(v, 0b_11_11);
                return Unsafe.BitCast<Vector256<double>, Vector256<T>>(
                    Avx.Permute2x128(splattedLanes, splattedLanes, 0b_00_01_00_01)
                    );
                }
            else return Vector256.Create(vector[3]);
            }
        else return Vector256.Create(vector[3]);
        }

    /// <inheritdoc
    /// cref="SplatW{T}(in Vector256{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<T> SplatW<T>(in Vector128<T> vector) {
        if (typeof(T) == typeof(float) || typeof(T) == typeof(int) || typeof(T) == typeof(uint)) {
            var v = Unsafe.BitCast<Vector128<T>, Vector128<float>>(vector);
            if (Avx.IsSupported) {
                return Unsafe.BitCast<Vector128<float>, Vector128<T>>(
                    Avx.Permute(v, 0b_11_11_11_11)
                    );
                }
            else if (Sse.IsSupported) {
                return Unsafe.BitCast<Vector128<float>, Vector128<T>>(
                    Sse.Shuffle(v, v, 0b_11_11_11_11)
                    );
                }
            else if (AdvSimd.IsSupported) {
                return Unsafe.BitCast<Vector128<float>, Vector128<T>>(
                    AdvSimd.DuplicateSelectedScalarToVector128(v, 3)
                    );
                }
            else return Vector128.Create(vector[3]);
            }
        else if (typeof(T) == typeof(double) || typeof(T) == typeof(long) || typeof(T) == typeof(ulong)) {
            throw new NotSupportedException("Cannot splat W onto a vector with fewer than 4 components.");
            }
        else throw new NotSupportedException($"Cannot splat {typeof(T).Name}!");
        }

    /// <inheritdoc
    /// cref="SplatX{T}(in Vector256{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<T> SplatX<T>(in Vector128<T> vector) {
        if (typeof(T) == typeof(float) || typeof(T) == typeof(int) || typeof(T) == typeof(uint)) {
            var v = Unsafe.BitCast<Vector128<T>, Vector128<float>>(vector);
            if (Avx.IsSupported) {
                return Unsafe.BitCast<Vector128<float>, Vector128<T>>(
                    Avx.Permute(v, 0b_00_00_00_00)
                    );
                }
            else if (Sse.IsSupported) {
                return Unsafe.BitCast<Vector128<float>, Vector128<T>>(
                    Sse.Shuffle(v, v, 0b_00_00_00_00)
                    );
                }
            else if (AdvSimd.IsSupported) {
                return Unsafe.BitCast<Vector128<float>, Vector128<T>>(
                    AdvSimd.DuplicateSelectedScalarToVector128(v, 0)
                    );
                }
            else return Vector128.Create(vector[0]);
            }
        else if (typeof(T) == typeof(double) || typeof(T) == typeof(long) || typeof(T) == typeof(ulong)) {
            var v = Unsafe.BitCast<Vector128<T>, Vector128<double>>(vector);
            if (Sse2.IsSupported) {
                return Unsafe.BitCast<Vector128<double>, Vector128<T>>(
                    Sse2.Shuffle(v, v, 0b_00)
                    );
                }
            else if (AdvSimd.Arm64.IsSupported) {
                return Unsafe.BitCast<Vector128<double>, Vector128<T>>(
                    AdvSimd.Arm64.DuplicateSelectedScalarToVector128(v, 0)
                    );
                }
            else return Vector128.Create(vector[0]);
            }
        else return Vector128.Create(vector[0]);
        }

    /// <summary><c><paramref name="vector"/>(X, Y, Z, W) → <paramref name="vector"/>(X, X, X, X)
    /// </c></summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector256<T> SplatX<T>(in Vector256<T> vector) {
        if (typeof(T) == typeof(float) || typeof(T) == typeof(int) || typeof(T) == typeof(uint)) {
            var v = Unsafe.BitCast<Vector256<T>, Vector256<float>>(vector);
            if (Avx2.IsSupported) {
                return Unsafe.BitCast<Vector256<float>, Vector256<T>>(
                    Avx2.BroadcastScalarToVector256(v.GetLower())
                    );
                }
            else if (Avx.IsSupported) {
                var lowSplat = Avx.Permute(v, 0b_00_00_00_00);
                var broadcast = Avx.Permute2x128(lowSplat, lowSplat, 0b_00_00_00_00);
                return Unsafe.BitCast<Vector256<float>, Vector256<T>>(broadcast);
                }
            else return Vector256.Create(vector[0]);
            }
        else if (typeof(T) == typeof(double) || typeof(T) == typeof(long) || typeof(T) == typeof(ulong)) {
            var v = Unsafe.BitCast<Vector256<T>, Vector256<double>>(vector);
            if (Avx2.IsSupported) {
                return Unsafe.BitCast<Vector256<double>, Vector256<T>>(
                    Avx2.Permute4x64(v, 0b_00_00_00_00)
                    );
                }
            else if (Avx.IsSupported) {
                var splattedLanes = Avx.Permute(v, 0b_00_00);
                return Unsafe.BitCast<Vector256<double>, Vector256<T>>(
                    Avx.Permute2x128(splattedLanes, splattedLanes, 0b_00_00_00_00)
                    );
                }
            else return Vector256.Create(vector[0]);
            }
        else return Vector256.Create(vector[0]);
        }

    /// <summary><c><paramref name="vector"/>(X, Y) → <paramref name="vector"/>(X, X)
    /// </c></summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector64<float> SplatX(this Vector64<float> vector) {
        if (AdvSimd.Arm64.IsSupported) {
            return AdvSimd.DuplicateSelectedScalarToVector64(vector, 0);
            }
        else return Vector64.Create(vector[0], vector[0]);
        }

    /// <inheritdoc
    /// cref="SplatX(Vector64{float})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<double> SplatX(this Vector128<double> vector) {
        if (Sse3.IsSupported) {
            return Sse3.MoveAndDuplicate(vector);
            }
        else if (AdvSimd.Arm64.IsSupported) {
            return AdvSimd.Arm64.DuplicateSelectedScalarToVector128(vector, 0);
            }
        else return Vector128.Create(vector[0], vector[0]);
        }

    /// <inheritdoc
    /// cref="SplatY{T}(in Vector256{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<T> SplatY<T>(in Vector128<T> vector) {
        if (typeof(T) == typeof(float) || typeof(T) == typeof(int) || typeof(T) == typeof(uint)) {
            var v = Unsafe.BitCast<Vector128<T>, Vector128<float>>(vector);
            if (Avx.IsSupported) {
                return Unsafe.BitCast<Vector128<float>, Vector128<T>>(
                    Avx.Permute(v, 0b_01_01_01_01)
                    );
                }
            else if (Sse.IsSupported) {
                return Unsafe.BitCast<Vector128<float>, Vector128<T>>(
                    Sse.Shuffle(v, v, 0b_01_01_01_01)
                    );
                }
            else if (AdvSimd.IsSupported) {
                return Unsafe.BitCast<Vector128<float>, Vector128<T>>(
                    AdvSimd.DuplicateSelectedScalarToVector128(v, 1)
                    );
                }
            else return Vector128.Create(vector[1]);
            }
        else if (typeof(T) == typeof(double) || typeof(T) == typeof(long) || typeof(T) == typeof(ulong)) {
            var v = Unsafe.BitCast<Vector128<T>, Vector128<double>>(vector);
            if (Sse2.IsSupported) {
                return Unsafe.BitCast<Vector128<double>, Vector128<T>>(
                    Sse2.Shuffle(v, v, 0b_11)
                    );
                }
            else if (AdvSimd.Arm64.IsSupported) {
                return Unsafe.BitCast<Vector128<double>, Vector128<T>>(
                    AdvSimd.Arm64.DuplicateSelectedScalarToVector128(v, 1)
                    );
                }
            else return Vector128.Create(vector[1]);
            }
        else return Vector128.Create(vector[1]);
        }

    /// <summary><c><paramref name="vector"/>(X, Y, Z, W) → <paramref name="vector"/>(Y, Y, Y, Y)
    /// </c></summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector256<T> SplatY<T>(in Vector256<T> vector) {
        if (typeof(T) == typeof(float) || typeof(T) == typeof(int) || typeof(T) == typeof(uint)) {
            var v = Unsafe.BitCast<Vector256<T>, Vector256<float>>(vector);
            if (Avx.IsSupported) {
                var splattedLanes = Avx.Permute(v, 0b_01_01_01_01);
                return Unsafe.BitCast<Vector256<float>, Vector256<T>>(
                    Avx.Permute2x128(splattedLanes, splattedLanes, 0b_00_00_00_00)
                    );
                }
            else return Vector256.Create(vector[1]);
            }
        else if (typeof(T) == typeof(double) || typeof(T) == typeof(long) || typeof(T) == typeof(ulong)) {
            var v = Unsafe.BitCast<Vector256<T>, Vector256<double>>(vector);
            if (Avx2.IsSupported) {
                return Unsafe.BitCast<Vector256<double>, Vector256<T>>(
                    Avx2.Permute4x64(v, 0b_01_01_01_01)
                    );
                }
            else if (Avx.IsSupported) {
                var splattedLanes = Avx.Permute(v, 0b_01_01);
                return Unsafe.BitCast<Vector256<double>, Vector256<T>>(
                    Avx.Permute2x128(splattedLanes, splattedLanes, 0b_00_00_00_00)
                    );
                }
            else return Vector256.Create(vector[1]);
            }
        else return Vector256.Create(vector[1]);
        }

    /// <summary><c><paramref name="vector"/>(X, Y) → <paramref name="vector"/>(Y, Y)
    /// </c></summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector64<float> SplatY(this Vector64<float> vector) {
        if (AdvSimd.Arm64.IsSupported) {
            return AdvSimd.DuplicateSelectedScalarToVector64(vector, 1);
            }
        else return Vector64.Create(vector[1], vector[1]);
        }

    /// <inheritdoc
    /// cref="SplatY(Vector64{float})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<double> SplatY(this Vector128<double> vector) {
        if (Sse2.IsSupported) {
            return Sse2.UnpackHigh(vector, vector);
            }
        else if (AdvSimd.Arm64.IsSupported) {
            return AdvSimd.Arm64.DuplicateSelectedScalarToVector128(vector, 1);
            }
        else return Vector128.Create(vector[1], vector[1]);
        }

    /// <summary><c><paramref name="vector"/>(X, Y, Z, W) → <paramref name="vector"/>(Z, Z, Z, Z)
    /// </c></summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector256<T> SplatZ<T>(in Vector256<T> vector) {
        if (typeof(T) == typeof(float) || typeof(T) == typeof(int) || typeof(T) == typeof(uint)) {
            var v = Unsafe.BitCast<Vector256<T>, Vector256<float>>(vector);
            if (Avx.IsSupported) {
                var splattedLanes = Avx.Permute(v, 0b_10_10_10_10);
                return Unsafe.BitCast<Vector256<float>, Vector256<T>>(
                    Avx.Permute2x128(splattedLanes, splattedLanes, 0b_00_00_00_00)
                    );
                }
            else return Vector256.Create(vector[2]);
            }
        else if (typeof(T) == typeof(double) || typeof(T) == typeof(long) || typeof(T) == typeof(ulong)) {
            var v = Unsafe.BitCast<Vector256<T>, Vector256<double>>(vector);
            if (Avx2.IsSupported) {
                return Unsafe.BitCast<Vector256<double>, Vector256<T>>(
                    Avx2.Permute4x64(v, 0b_10_10_10_10)
                    );
                }
            else if (Avx.IsSupported) {
                var splattedLanes = Avx.Permute(v, 0b_00_00);
                return Unsafe.BitCast<Vector256<double>, Vector256<T>>(
                    Avx.Permute2x128(splattedLanes, splattedLanes, 0b_00_01_00_01)
                    );
                }
            else return Vector256.Create(vector[2]);
            }
        else return Vector256.Create(vector[2]);
        }

    /// <inheritdoc
    /// cref="SplatZ{T}(in Vector256{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<T> SplatZ<T>(in Vector128<T> vector) {
        if (typeof(T) == typeof(float) || typeof(T) == typeof(int) || typeof(T) == typeof(uint)) {
            var v = Unsafe.BitCast<Vector128<T>, Vector128<float>>(vector);
            if (Avx.IsSupported) {
                return Unsafe.BitCast<Vector128<float>, Vector128<T>>(
                    Avx.Permute(v, 0b_10_10_10_10)
                    );
                }
            else if (Sse.IsSupported) {
                return Unsafe.BitCast<Vector128<float>, Vector128<T>>(
                    Sse.Shuffle(v, v, 0b_10_10_10_10)
                    );
                }
            else if (AdvSimd.IsSupported) {
                return Unsafe.BitCast<Vector128<float>, Vector128<T>>(
                    AdvSimd.DuplicateSelectedScalarToVector128(v, 2)
                    );
                }
            else return Vector128.Create(vector[2]);
            }
        else if (typeof(T) == typeof(double) || typeof(T) == typeof(long) || typeof(T) == typeof(ulong)) {
            throw new NotSupportedException("Cannot splat Z onto a vector with fewer than 3 components.");
            }
        else throw new NotSupportedException($"Cannot splat {typeof(T).Name}!");
        }

    #endregion Splat

    #region To Low/High

    /// <summary> Returns <c>(<paramref name="left"/>.Z, <paramref name="left"/>.W, <paramref name="right"/>.Z, <paramref name="right"/>.W)</c>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<float> ToHighHigh(Vector128<float> left, Vector128<float> right) {
        if (Sse.IsSupported) {
            return Sse.Shuffle(left, right, 0b_11_10_11_10);
            }
        else if (AdvSimd.Arm64.IsSupported) {
            return AdvSimd.Arm64.ZipHigh(left.AsDouble(), right.AsDouble()).AsSingle();
            }
        else if (Vector128.IsHardwareAccelerated && Vector128<float>.IsSupported
              && Vector64.IsHardwareAccelerated && Vector64<float>.IsSupported) {
            return Vector128.Create(left.GetUpper(), right.GetUpper());
            }
        else {
            return Vector128.Create(left[2], left[3], right[2], right[3]);
            }
        }

    /// <inheritdoc
    /// cref="ToHighHigh(Vector128{float}, Vector128{float})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector256<double> ToHighHigh(Vector256<double> left, Vector256<double> right) {
        if (Avx.IsSupported) {
            return Avx.Permute2x128(left, right, 0b_0011_0001);
            }
        else if (Vector256.IsHardwareAccelerated && Vector256<double>.IsSupported
              && Vector128.IsHardwareAccelerated && Vector128<double>.IsSupported) {
            return Vector256.Create(left.GetUpper(), right.GetUpper());
            }
        else {
            return Vector256.Create(left[2], left[3], right[2], right[3]);
            }
        }

    /// <inheritdoc
    /// cref="ToHighHigh(Vector128{float}, Vector128{float})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<int> ToHighHigh(Vector128<int> left, Vector128<int> right) {
        if (Sse2.IsSupported) {
            return Sse2.UnpackHigh(left.AsInt64(), right.AsInt64()).AsInt32();
            }
        else if (AdvSimd.Arm64.IsSupported) {
            return AdvSimd.Arm64.ZipHigh(left.AsInt64(), right.AsInt64()).AsInt32();
            }
        else if (Vector128.IsHardwareAccelerated && Vector64.IsHardwareAccelerated && Vector128<int>.IsSupported && Vector64<int>.IsSupported) {
            return Vector128.Create(left.GetUpper(), right.GetUpper());
            }
        else {
            return Vector128.Create(left[2], left[3], right[2], right[3]);
            }
        }

    /// <inheritdoc
    /// cref="ToHighHigh(Vector128{float}, Vector128{float})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector256<long> ToHighHigh(Vector256<long> left, Vector256<long> right) {
        if (Avx.IsSupported) {
            return Avx.Permute2x128(left, right, 0b_0011_0001);
            }
        else if (Vector256.IsHardwareAccelerated && Vector256.IsHardwareAccelerated && Vector128<long>.IsSupported && Vector128<long>.IsSupported) {
            return Vector256.Create(left.GetUpper(), right.GetUpper());
            }
        else {
            return Vector256.Create(left[2], left[3], right[2], right[3]);
            }
        }

    /// <inheritdoc
    /// cref="ToHighHigh(Vector128{float}, Vector128{float})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<uint> ToHighHigh(Vector128<uint> left, Vector128<uint> right) {
        if (Sse2.IsSupported) {
            return Sse2.UnpackHigh(left.AsInt64(), right.AsInt64()).AsUInt32();
            }
        else if (AdvSimd.Arm64.IsSupported) {
            return AdvSimd.Arm64.ZipHigh(left.AsUInt64(), right.AsUInt64()).AsUInt32();
            }
        else if (Vector128.IsHardwareAccelerated && Vector64.IsHardwareAccelerated && Vector128<uint>.IsSupported && Vector64<uint>.IsSupported) {
            return Vector128.Create(left.GetUpper(), right.GetUpper());
            }
        else {
            return Vector128.Create(left[2], left[3], right[2], right[3]);
            }
        }

    /// <inheritdoc
    /// cref="ToHighHigh(Vector128{float}, Vector128{float})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector256<ulong> ToHighHigh(Vector256<ulong> left, Vector256<ulong> right) {
        if (Avx.IsSupported) {
            return Avx.Permute2x128(left, right, 0b_0011_0001);
            }
        else if (Vector256.IsHardwareAccelerated && Vector256.IsHardwareAccelerated && Vector128<ulong>.IsSupported && Vector128<ulong>.IsSupported) {
            return Vector256.Create(left.GetUpper(), right.GetUpper());
            }
        else {
            return Vector256.Create(left[2], left[3], right[2], right[3]);
            }
        }

    /// <summary> Returns <c>(<paramref name="left"/>.Z, <paramref name="left"/>.W, <paramref name="right"/>.X, <paramref name="right"/>.Y)</c>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<float> ToHighLow(Vector128<float> left, Vector128<float> right) {
        if (Sse.IsSupported) {
            return Sse.Shuffle(left, right, 0b_01_00_11_10);
            }
        else if (Vector128.IsHardwareAccelerated && Vector128<float>.IsSupported
              && Vector64.IsHardwareAccelerated && Vector64<float>.IsSupported) {
            return Vector128.Create(left.GetUpper(), right.GetLower());
            }
        else {
            return Vector128.Create(left[2], left[3], right[0], right[1]);
            }
        }

    /// <inheritdoc
    /// cref="ToHighLow(Vector128{float}, Vector128{float})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector256<double> ToHighLow(Vector256<double> left, Vector256<double> right) {
        if (Avx.IsSupported) {
            return Avx.Permute2x128(left, right, 0b_0010_0001);
            }
        else if (Vector256.IsHardwareAccelerated && Vector256<double>.IsSupported
              && Vector128.IsHardwareAccelerated && Vector128<double>.IsSupported) {
            return Vector256.Create(left.GetUpper(), right.GetLower());
            }
        else {
            return Vector256.Create(left[2], left[3], right[0], right[1]);
            }
        }

    /// <inheritdoc
    /// cref="ToHighLow(Vector128{float}, Vector128{float})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<int> ToHighLow(Vector128<int> left, Vector128<int> right) {
        if (Sse2.IsSupported) {
            return Sse2.Shuffle(left.AsDouble(), right.AsDouble(), 0b_01).AsInt32();
            }
        else if (Vector128.IsHardwareAccelerated && Vector64.IsHardwareAccelerated && Vector128<int>.IsSupported && Vector64<int>.IsSupported) {
            return Vector128.Create(left.GetUpper(), right.GetLower());
            }
        else {
            return Vector128.Create(left[2], left[3], right[0], right[1]);
            }
        }

    /// <inheritdoc
    /// cref="ToHighLow(Vector128{float}, Vector128{float})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector256<long> ToHighLow(Vector256<long> left, Vector256<long> right) {
        if (Avx.IsSupported) {
            return Avx.Permute2x128(left, right, 0b_0010_0001);
            }
        else if (Vector256.IsHardwareAccelerated && Vector128.IsHardwareAccelerated && Vector256<long>.IsSupported && Vector128<long>.IsSupported) {
            return Vector256.Create(left.GetUpper(), right.GetLower());
            }
        else {
            return Vector256.Create(left[2], left[3], right[0], right[1]);
            }
        }

    /// <inheritdoc
    /// cref="ToHighLow(Vector128{float}, Vector128{float})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<uint> ToHighLow(Vector128<uint> left, Vector128<uint> right) {
        if (Sse2.IsSupported) {
            return Sse2.Shuffle(left.AsDouble(), right.AsDouble(), 0b_01).AsUInt32();
            }
        else if (Vector128.IsHardwareAccelerated && Vector64.IsHardwareAccelerated && Vector128<uint>.IsSupported && Vector64<uint>.IsSupported) {
            return Vector128.Create(left.GetUpper(), right.GetLower());
            }
        else {
            return Vector128.Create(left[2], left[3], right[0], right[1]);
            }
        }

    /// <inheritdoc
    /// cref="ToHighLow(Vector128{float}, Vector128{float})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector256<ulong> ToHighLow(Vector256<ulong> left, Vector256<ulong> right) {
        if (Avx.IsSupported) {
            return Avx.Permute2x128(left, right, 0b_0010_0001);
            }
        else if (Vector256.IsHardwareAccelerated && Vector128.IsHardwareAccelerated && Vector256<ulong>.IsSupported && Vector128<ulong>.IsSupported) {
            return Vector256.Create(left.GetUpper(), right.GetLower());
            }
        else {
            return Vector256.Create(left[2], left[3], right[0], right[1]);
            }
        }

    /// <summary> Returns <c>(<paramref name="left"/>.X, <paramref name="left"/>.Y, <paramref name="right"/>.Z, <paramref name="right"/>.W)</c>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<float> ToLowHigh(Vector128<float> left, Vector128<float> right) {
        if (Sse41.IsSupported) {
            return Sse41.Blend(left, right, 0b_1100);
            }
        else if (Sse.IsSupported) {
            return Sse.Shuffle(left, right, 0b_11_10_01_00);
            }
        else if (Vector128.IsHardwareAccelerated && Vector128<float>.IsSupported
              && Vector64.IsHardwareAccelerated && Vector64<float>.IsSupported) {
            return Vector128.Create(left.GetLower(), right.GetUpper());
            }
        else {
            return Vector128.Create(left[0], left[1], right[2], right[3]);
            }
        }

    /// <inheritdoc
    /// cref="ToLowHigh(Vector128{float}, Vector128{float})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector256<double> ToLowHigh(Vector256<double> left, Vector256<double> right) {
        if (Avx.IsSupported) {
            return Avx.Blend(left, right, 0b_1100);
            }
        else if (Vector256.IsHardwareAccelerated && Vector128.IsHardwareAccelerated && Vector256<double>.IsSupported && Vector128<double>.IsSupported) {
            return Vector256.Create(left.GetLower(), right.GetUpper());
            }
        else {
            return Vector256.Create(left[0], left[1], right[2], right[3]);
            }
        }

    /// <inheritdoc
    /// cref="ToLowHigh(Vector128{float}, Vector128{float})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<int> ToLowHigh(Vector128<int> left, Vector128<int> right) {
        if (Sse41.IsSupported) {
            return Sse41.Blend(left.AsSingle(), right.AsSingle(), 0b_1100).AsInt32();
            }
        else if (Sse2.IsSupported) {
            return Sse2.Shuffle(left.AsDouble(), right.AsDouble(), 0b_10).AsInt32();
            }
        else if (Vector128.IsHardwareAccelerated && Vector64.IsHardwareAccelerated && Vector128<int>.IsSupported && Vector64<int>.IsSupported) {
            return Vector128.Create(left.GetLower(), right.GetUpper());
            }
        else {
            return Vector128.Create(left[0], left[1], right[2], right[3]);
            }
        }

    /// <inheritdoc
    /// cref="ToLowHigh(Vector128{float}, Vector128{float})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector256<long> ToLowHigh(Vector256<long> left, Vector256<long> right) {
        if (Avx.IsSupported) {
            return Avx.Blend(left.AsDouble(), right.AsDouble(), 0b_1100).AsInt64();
            }
        else if (Vector256.IsHardwareAccelerated && Vector128.IsHardwareAccelerated && Vector256<long>.IsSupported && Vector128<long>.IsSupported) {
            return Vector256.Create(left.GetLower(), right.GetUpper());
            }
        else {
            return Vector256.Create(left[0], left[1], right[2], right[3]);
            }
        }

    /// <inheritdoc
    /// cref="ToLowHigh(Vector128{float}, Vector128{float})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<uint> ToLowHigh(Vector128<uint> left, Vector128<uint> right) {
        if (Sse41.IsSupported) {
            return Sse41.Blend(left.AsSingle(), right.AsSingle(), 0b_1100).AsUInt32();
            }
        else if (Sse2.IsSupported) {
            return Sse2.Shuffle(left.AsDouble(), right.AsDouble(), 0b_10).AsUInt32();
            }
        else if (Vector128.IsHardwareAccelerated && Vector64.IsHardwareAccelerated && Vector128<uint>.IsSupported && Vector64<uint>.IsSupported) {
            return Vector128.Create(left.GetLower(), right.GetUpper());
            }
        else {
            return Vector128.Create(left[0], left[1], right[2], right[3]);
            }
        }

    /// <inheritdoc
    /// cref="ToLowHigh(Vector128{float}, Vector128{float})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector256<ulong> ToLowHigh(Vector256<ulong> left, Vector256<ulong> right) {
        if (Avx.IsSupported) {
            return Avx.Blend(left.AsDouble(), right.AsDouble(), 0b_1100).AsUInt64();
            }
        else if (Vector256.IsHardwareAccelerated && Vector128.IsHardwareAccelerated && Vector256<ulong>.IsSupported && Vector128<ulong>.IsSupported) {
            return Vector256.Create(left.GetLower(), right.GetUpper());
            }
        else {
            return Vector256.Create(left[0], left[1], right[2], right[3]);
            }
        }

    /// <summary> Returns <c>(<paramref name="left"/>.X, <paramref name="left"/>.Y, <paramref name="right"/>.X, <paramref name="right"/>.Y)</c>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<float> ToLowLow(Vector128<float> left, Vector128<float> right) {
        if (Sse.IsSupported) {
            return Sse.Shuffle(left, right, 0b_01_00_01_00);
            }
        else if (AdvSimd.Arm64.IsSupported) {
            return AdvSimd.Arm64.ZipLow(left.AsDouble(), right.AsDouble()).AsSingle();
            }
        else if (Vector128.IsHardwareAccelerated && Vector128<float>.IsSupported
              && Vector64.IsHardwareAccelerated && Vector64<float>.IsSupported) {
            return Vector128.Create(left.GetLower(), right.GetLower());
            }
        else {
            return Vector128.Create(left[0], left[1], right[0], right[1]);
            }
        }

    /// <inheritdoc
    /// cref="ToLowLow(Vector128{float}, Vector128{float})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector256<double> ToLowLow(Vector256<double> left, Vector256<double> right) {
        if (Avx.IsSupported) {
            return Avx.InsertVector128(left, right.GetLower(), 1);
            }
        else if (Vector256.IsHardwareAccelerated && Vector256<double>.IsSupported
              && Vector128.IsHardwareAccelerated && Vector128<double>.IsSupported) {
            return Vector256.Create(left.GetLower(), right.GetLower());
            }
        else {
            return Vector256.Create(left[0], left[1], right[0], right[1]);
            }
        }

    /// <inheritdoc
    /// cref="ToLowLow(Vector128{float}, Vector128{float})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<int> ToLowLow(Vector128<int> left, Vector128<int> right) {
        if (Sse2.IsSupported) {
            return Sse2.UnpackLow(left.AsInt64(), right.AsInt64()).AsInt32();
            }
        else if (AdvSimd.Arm64.IsSupported) {
            return AdvSimd.Arm64.ZipLow(left.AsInt64(), right.AsInt64()).AsInt32();
            }
        else if (Vector128.IsHardwareAccelerated && Vector64.IsHardwareAccelerated && Vector128<int>.IsSupported && Vector64<int>.IsSupported) {
            return Vector128.Create(left.GetLower(), right.GetLower());
            }
        else {
            return Vector128.Create(left[0], left[1], right[0], right[1]);
            }
        }

    /// <inheritdoc
    /// cref="ToLowLow(Vector128{float}, Vector128{float})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector256<long> ToLowLow(Vector256<long> left, Vector256<long> right) {
        if (Avx.IsSupported) {
            return Avx.InsertVector128(left, right.GetLower(), 1);
            }
        else if (Vector256.IsHardwareAccelerated && Vector128.IsHardwareAccelerated && Vector256<long>.IsSupported && Vector128<long>.IsSupported) {
            return Vector256.Create(left.GetLower(), right.GetLower());
            }
        else {
            return Vector256.Create(left[0], left[1], right[0], right[1]);
            }
        }

    /// <inheritdoc
    /// cref="ToLowLow(Vector128{float}, Vector128{float})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<uint> ToLowLow(Vector128<uint> left, Vector128<uint> right) {
        if (Sse2.IsSupported) {
            return Sse2.UnpackLow(left.AsInt64(), right.AsInt64()).AsUInt32();
            }
        else if (AdvSimd.Arm64.IsSupported) {
            return AdvSimd.Arm64.ZipLow(left.AsUInt64(), right.AsUInt64()).AsUInt32();
            }
        else if (Vector128.IsHardwareAccelerated && Vector64.IsHardwareAccelerated && Vector128<uint>.IsSupported && Vector64<uint>.IsSupported) {
            return Vector128.Create(left.GetLower(), right.GetLower());
            }
        else {
            return Vector128.Create(left[0], left[1], right[0], right[1]);
            }
        }

    /// <inheritdoc
    /// cref="ToLowLow(Vector128{float}, Vector128{float})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector256<ulong> ToLowLow(Vector256<ulong> left, Vector256<ulong> right) {
        if (Avx.IsSupported) {
            return Avx.InsertVector128(left, right.GetLower(), 1);
            }
        else if (Vector256.IsHardwareAccelerated && Vector128.IsHardwareAccelerated && Vector256<ulong>.IsSupported && Vector128<ulong>.IsSupported) {
            return Vector256.Create(left.GetLower(), right.GetLower());
            }
        else {
            return Vector256.Create(left[0], left[1], right[0], right[1]);
            }
        }

    #endregion To Low/High

    #region Weave Low/High

    /// <summary> Returns <c>(<paramref name="left"/>.Y, <paramref name="right"/>.Y, <paramref name="left"/>.W, <paramref name="right"/>.W)</c>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector256<double> InterleaveHigh(Vector256<double> left, Vector256<double> right) {
        if (Avx.IsSupported) {
            return Avx.UnpackHigh(left, right);
            }
        else if (Vector256.IsHardwareAccelerated && Vector256<double>.IsSupported
              && Vector128.IsHardwareAccelerated && Vector128<double>.IsSupported) {
            return Vector256.Create(
                WeaveHigh(left.GetLower(), right.GetLower()),
                WeaveHigh(left.GetUpper(), right.GetUpper())
                );
            }
        else {
            return Vector256.Create(left[1], right[1], left[3], right[3]);
            }
        }

    /// <summary> Returns <c>(<paramref name="left"/>.Y, <paramref name="right"/>.Y, <paramref name="left"/>.W, <paramref name="right"/>.W)</c>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<float> InterleaveHigh(Vector128<float> left, Vector128<float> right) {
        if (Sse.IsSupported) {
            var v = Sse.Shuffle(left, right, 0b_11_01_11_01);   // [a.Y, a.W, b.Y, b.W]
            return Sse.Shuffle(v, v, 0b_11_01_10_00);           // [a.Y, b.Y, a.W, b.W]
            }
        else if (AdvSimd.Arm64.IsSupported) {
            return AdvSimd.Arm64.TransposeOdd(left, right);
            }
        else {
            return Vector128.Create(left[1], right[1], left[3], right[3]);
            }
        }

    /// <summary> Returns <c>(<paramref name="left"/>.X, <paramref name="right"/>.X, <paramref name="left"/>.Z, <paramref name="right"/>.Z)</c>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector256<double> InterleaveLow(Vector256<double> left, Vector256<double> right) {
        if (Avx.IsSupported) {
            return Avx.UnpackLow(left, right);
            }
        else if (Vector256.IsHardwareAccelerated && Vector256<double>.IsSupported
              && Vector128.IsHardwareAccelerated && Vector128<double>.IsSupported) {
            return Vector256.Create(
                WeaveLow(left.GetLower(), right.GetLower()),
                WeaveLow(left.GetUpper(), right.GetUpper())
                );
            }
        else {
            return Vector256.Create(left[0], right[0], left[2], right[2]);
            }
        }

    /// <summary> Returns <c>(<paramref name="left"/>.X, <paramref name="right"/>.X, <paramref name="left"/>.Z, <paramref name="right"/>.Z)</c>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<float> InterleaveLow(Vector128<float> left, Vector128<float> right) {
        if (Sse.IsSupported) {
            var v = Sse.Shuffle(left, right, 0b_10_00_10_00);   // [a.X, a.Z, b.X, b.Z]
            return Sse.Shuffle(v, v, 0b_11_01_10_00);           // [a.X, b.X, a.Z, b.Z]
            }
        else if (AdvSimd.Arm64.IsSupported) {
            return AdvSimd.Arm64.TransposeEven(left, right);
            }
        else {
            return Vector128.Create(left[0], right[0], left[2], right[2]);
            }
        }

    /// <summary> Returns <c>(<paramref name="left"/>.Z, <paramref name="right"/>.Z, <paramref name="left"/>.W, <paramref name="right"/>.W)</c>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<float> WeaveHigh(Vector128<float> left, Vector128<float> right) {
        if (Sse.IsSupported) {
            return Sse.UnpackHigh(left, right);
            }
        else if (AdvSimd.Arm64.IsSupported) {
            return AdvSimd.Arm64.ZipHigh(left, right);
            }
        else {
            return Vector128.Create(left[2], right[2], left[3], right[3]);
            }
        }

    /// <summary> Returns <c>(<paramref name="left"/>.Z, <paramref name="right"/>.Z, <paramref name="left"/>.W, <paramref name="right"/>.W)</c>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector256<double> WeaveHigh(Vector256<double> left, Vector256<double> right) {
        if (Avx.IsSupported) {
            return ToHighHigh(InterleaveLow(left, right), InterleaveHigh(left, right));
            }
        else if (Vector256.IsHardwareAccelerated && Vector256<double>.IsSupported
              && Vector128.IsHardwareAccelerated && Vector128<double>.IsSupported) {
            var lu = left.GetUpper();
            var ru = right.GetUpper();
            return Vector256.Create(
                WeaveLow(lu, ru),
                WeaveHigh(lu, ru)
                );
            }
        else {
            return Vector256.Create(left[2], right[2], left[3], right[3]);
            }
        }

    /// <summary> Returns <c>(<paramref name="left"/>.Y, <paramref name="right"/>.Y)</c>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<double> WeaveHigh(Vector128<double> left, Vector128<double> right) {
        if (Sse2.IsSupported) {
            return Sse2.UnpackHigh(left, right);
            }
        else if (AdvSimd.Arm64.IsSupported) {
            return AdvSimd.Arm64.ZipHigh(left, right);
            }
        else {
            return Vector128.Create(left[1], right[1]);
            }
        }

    /// <inheritdoc
    /// cref="WeaveLow(Vector128{float}, Vector128{float})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<ulong> WeaveHigh(Vector128<ulong> left, Vector128<ulong> right) {
        if (Sse2.IsSupported) {
            return Sse2.UnpackHigh(left, right);
            }
        else if (AdvSimd.Arm64.IsSupported) {
            return AdvSimd.Arm64.ZipHigh(left, right);
            }
        else {
            return Vector128.Create(left[1], right[1]);
            }
        }

    /// <inheritdoc
    /// cref="WeaveLow(Vector128{float}, Vector128{float})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<long> WeaveHigh(Vector128<long> left, Vector128<long> right) {
        if (Sse2.IsSupported) {
            return Sse2.UnpackHigh(left, right);
            }
        else if (AdvSimd.Arm64.IsSupported) {
            return AdvSimd.Arm64.ZipHigh(left, right);
            }
        else {
            return Vector128.Create(left[1], right[1]);
            }
        }

    /// <summary> Returns <c>(<paramref name="left"/>.X, <paramref name="right"/>.X, <paramref name="left"/>.Y, <paramref name="right"/>.Y)</c>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<float> WeaveLow(Vector128<float> left, Vector128<float> right) {
        if (Sse.IsSupported) {
            return Sse.UnpackLow(left, right);
            }
        else if (AdvSimd.Arm64.IsSupported) {
            return AdvSimd.Arm64.ZipLow(left, right);
            }
        else {
            return Vector128.Create(left[0], right[0], left[1], right[1]);
            }
        }

    /// <summary> Returns <c>(<paramref name="left"/>.X, <paramref name="right"/>.X, <paramref name="left"/>.Y, <paramref name="right"/>.Y)</c>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector256<double> WeaveLow(Vector256<double> left, Vector256<double> right) {
        if (Avx.IsSupported) {
            return ToLowLow(InterleaveLow(left, right), InterleaveHigh(left, right));
            }
        else if (Vector256.IsHardwareAccelerated && Vector256<double>.IsSupported
              && Vector128.IsHardwareAccelerated && Vector128<double>.IsSupported) {
            var lo = left.GetLower();
            var ro = right.GetLower();
            return Vector256.Create(
                WeaveLow(lo, ro),
                WeaveHigh(lo, ro)
                );
            }
        else {
            return Vector256.Create(left[0], right[0], left[1], right[1]);
            }
        }


    /// <inheritdoc
    /// cref="WeaveLow(Vector128{float}, Vector128{float})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<long> WeaveLow(Vector128<long> left, Vector128<long> right) {
        if (Sse2.IsSupported) {
            return Sse2.UnpackLow(left, right);
            }
        else if (AdvSimd.Arm64.IsSupported) {
            return AdvSimd.Arm64.ZipLow(left, right);
            }
        else {
            return Vector128.Create(left[0], right[0]);
            }
        }

    /// <inheritdoc
    /// cref="WeaveLow(Vector128{float}, Vector128{float})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<ulong> WeaveLow(Vector128<ulong> left, Vector128<ulong> right) {
        if (Sse2.IsSupported) {
            return Sse2.UnpackLow(left, right);
            }
        else if (AdvSimd.Arm64.IsSupported) {
            return AdvSimd.Arm64.ZipLow(left, right);
            }
        else {
            return Vector128.Create(left[0], right[0]);
            }
        }

    /// <summary> Returns <c>(<paramref name="left"/>.X, <paramref name="right"/>.X)</c>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<double> WeaveLow(Vector128<double> left, Vector128<double> right) {
        if (Sse2.IsSupported) {
            return Sse2.UnpackLow(left, right);
            }
        else if (AdvSimd.Arm64.IsSupported) {
            return AdvSimd.Arm64.ZipLow(left, right);
            }
        else {
            return Vector128.Create(left[0], right[0]);
            }
        }

    #endregion Weave Low/High

    /// <summary> Functions for loading vectors from <see langword="unmanaged"/> values. The reference is assumed to contain or be legitimately followed by as many numbers of bits as the vector type, and should <b>not</b> be used to load data already loaded into a CPU register.
    /// </summary>
    public static class Of<T> where T : unmanaged, INumber<T> {

        /// <summary> Loads the <typeparamref name="TData"/> by reference as a 128-bit vector of <typeparamref name="T"/>.
        /// <para/> 📝 <inheritdoc cref="Of{T}"/>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector128<T> Load128<TData>(in TData data)
            where TData : unmanaged {
            return Vector128.Load((T*)Unsafe.AsPointer(in data));
            }

        /// <summary> Loads the <typeparamref name="TData"/> by reference as a 256-bit vector of <typeparamref name="T"/>.
        /// <para/> 📝 <inheritdoc cref="Of{T}"/>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector256<T> Load256<TData>(in TData data)
            where TData : unmanaged {
            return Vector256.Load((T*)Unsafe.AsPointer(in data));
            }

        /// <summary> Loads the <typeparamref name="TData"/> by reference as a 512-bit vector of <typeparamref name="T"/>.
        /// <para/> 📝 <inheritdoc cref="Of{T}"/>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector512<T> Load512<TData>(in TData data)
            where TData : unmanaged {
            return Vector512.Load((T*)Unsafe.AsPointer(in data));
            }

        /// <summary> Loads the <typeparamref name="TData"/> by reference as a 64-bit vector of <typeparamref name="T"/>.
        /// <para/> 📝 <inheritdoc cref="Of{T}"/>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector64<T> Load64<TData>(in TData data)
            where TData : unmanaged {
            return Vector64.Load((T*)Unsafe.AsPointer(in data));
            }
        }
    }
