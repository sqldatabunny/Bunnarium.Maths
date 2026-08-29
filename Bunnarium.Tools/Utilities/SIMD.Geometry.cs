using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.Arm;
using System.Runtime.Intrinsics.X86;
using X86 = System.Runtime.Intrinsics.X86;

namespace Bunnarium.Tools.Utilities;
public static unsafe partial class SIMD {

    #region Experimental

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector256<float> Cross4x(Vector256<float> a, Vector256<float> b) {
        var m = a * Vector256.Shuffle(b, Vector256.Create(1, 0, 3, 2, 5, 4, 7, 6))
                  * Vector256.Create(Vector128.Create(1f, -1f, 1f, -1f));
        return PairwiseAdd(m, m);
        }

    #endregion

    #region Vectors

    private static readonly Vector128<float> BoxSignMask128
        = Vector128.Create(+0f, +0f, -0f, -0f);

    private static readonly Vector128<int> BoxSignMask128Int
        = Vector128.Create(+0, +0, -0, -0);

    private static readonly Vector256<double> BoxSignMask256
    = Vector256.Create(+0d, +0d, -0d, -0d);

    private static readonly Vector256<long> BoxSignMask256Int
        = Vector256.Create(+0L, +0L, -0L, -0L);

    private static readonly Vector128<double> Half128D
        = Vector128.Create(0.5);

    private static readonly Vector128<float> Half128F
        = Vector128.Create(0.5f);

    private static readonly Vector256<double> Half256D
                                = Vector256.Create(0.5);
    private static readonly Vector64<float> Half64F
        = Vector64.Create(0.5f);

    private static readonly Vector128<float> MinMaxSignMask128
        = Vector128.Create(-0.0f, -0.0f, +0.0f, +0.0f);

    private static readonly Vector128<int> MinMaxSignMask128Int
        = Vector128.Create(-0, -0, +0, +0);

    private static readonly Vector256<double> MinMaxSignMask256
        = Vector256.Create(-0.0, -0.0, +0.0, +0.0);

    private static readonly Vector256<long> MinMaxSignMask256Int
            = Vector256.Create(-0L, -0L, +0L, +0L);

    #endregion Vectors

    #region Cross Product (2D)

    /// <summary> Returns the 2D cross product (or the wedge product) of two 2D vectors.
    /// <para/> <c><paramref name="a"/>.X * <paramref name="b"/>.Y - <paramref name="a"/>.Y * <paramref name="b"/>.X</c>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Cross2D(Vector128<double> a, Vector128<double> b) {
        return Vector128.Sum(a * Vector128.Shuffle(b, Vector128.Create(1L, 0L))
             * Vector128.Create(1d, -1d));
        }

    /// <inheritdoc
    /// cref="Cross2x(Vector128{float}, Vector128{float})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector256<double> Cross2x(Vector256<double> a, Vector256<double> b) {
        var m = a * Vector256.Shuffle(b, Vector256.Create(1L, 0L, 3L, 2L))
                  * Vector256.Create(1d, -1d, 1d, -1d);  // (+(ax * by), -(ay * bx)) repeating
        return PairwiseAdd(m, m);
        }

    /// <summary> Returns <c>(<paramref name="a"/>.XY × <paramref name="b"/>.XY, <paramref name="a"/>.ZW × <paramref name="b"/>.ZW, <paramref name="a"/>.XY × <paramref name="b"/>.XY, <paramref name="a"/>.ZW × <paramref name="b"/>.ZW)</c>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<float> Cross2x(Vector128<float> a, Vector128<float> b) {
        var m = a * Vector128.Shuffle(b, Vector128.Create(1, 0, 3, 2))
                  * Vector128.Create(1f, -1f, 1f, -1f); // (+(ax * by), -(ay * bx)) repeating
        return PairwiseAdd(m, m);
        }

    /// <inheritdoc
    /// cref="Cross2xHorizontal(Vector128{float}, Vector128{float})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector256<double> Cross2xHorizontal(Vector256<double> a, Vector256<double> b) {
        var indices = Vector256.Create(3L, 2L, 1L, 0L);
        var signs = Vector256.Create(1d, -1d, 1d, -1d);
        return PairwiseAdd(
            a * Vector256.Shuffle(a, indices) * signs,
            b * Vector256.Shuffle(b, indices) * signs
            );
        }

    /// <summary> Returns <c>((<paramref name="a"/>.XY × <paramref name="a"/>.ZW), -(<paramref name="a"/>.XY × <paramref name="a"/>.ZW), (<paramref name="b"/>.XY × <paramref name="b"/>.ZW), -(<paramref name="b"/>.XY × <paramref name="b"/>.ZW))</c>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<float> Cross2xHorizontal(Vector128<float> a, Vector128<float> b) {
        var indices = Vector128.Create(3, 2, 1, 0);
        var signs = Vector128.Create(1f, -1f, 1f, -1f);
        return PairwiseAdd(
            a * Vector128.Shuffle(a, indices) * signs,
            b * Vector128.Shuffle(b, indices) * signs
            );
        }

    #endregion Cross Product (2D)

    #region Cross Product (3D)

    /// <summary> Returns <c>(<paramref name="a"/> × <paramref name="b"/>)</c>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [BunnyAttributes.Citation("https://geometrian.com/resources/cross_product/")]
    public static Vector128<float> Cross3D(Vector128<float> a, Vector128<float> b) {
        var vec0 = Vector128.Shuffle(a, Vector128.Create(1, 2, 0, 3));
        var vec1 = Vector128.Shuffle(b, Vector128.Create(2, 0, 1, 3));
        var vec2 = vec0 * b;
        return vec0 * vec1 - Vector128.Shuffle(vec2, Vector128.Create(1, 2, 0, 3));
        }

    /// <inheritdoc
    /// cref="Cross3D(Vector128{float}, Vector128{float})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [BunnyAttributes.Citation("https://geometrian.com/resources/cross_product/")]
    public static Vector256<double> Cross3D(Vector256<double> a, Vector256<double> b) {
        var vec0 = Vector256.Shuffle(a, Vector256.Create(1L, 2L, 0L, 3L));
        var vec1 = Vector256.Shuffle(b, Vector256.Create(2L, 0L, 1L, 3L));
        var vec2 = vec0 * b;
        return vec0 * vec1 - Vector256.Shuffle(vec2, Vector256.Create(1L, 2L, 0L, 3L));
        }

    #endregion Cross Product (3D)

    #region Dot Products

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Dot(Vector128<double> a, Vector128<double> b) {
        return Vector128.Sum(a * b);
        }

    /// <inheritdoc
    /// cref="Dot2x(Vector128{float}, Vector128{float})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector256<double> Dot2x(Vector256<double> a, Vector256<double> b) {
        var m = a * b;
        return PairwiseAdd(m, m);
        }

    /// <summary>Returns <c>(Dot(<paramref name="a"/>.XY, <paramref name="b"/>.XY), Dot(<paramref name="a"/>.ZW, <paramref name="b"/>.ZW), Dot(<paramref name="a"/>.XY, <paramref name="b"/>.XY), Dot(<paramref name="a"/>.ZW, <paramref name="b"/>.ZW))
    /// </c>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<float> Dot2x(Vector128<float> a, Vector128<float> b) {
        var m = a * b;
        return PairwiseAdd(m, m);
        }
    #endregion Dot Products

    #region Conversions - Centerpoint/HalfLengths → Min/Max (2D)

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<float> ToMinMax(in Vector128<float> centerpointHalfLengths) {
        return ToMinMax(
            centerpointHalfLengths.Shuffle0101(),
            centerpointHalfLengths.Shuffle2323()
            );
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector256<double> ToMinMax(in Vector256<double> centerpointHalfLengths) {
        return ToMinsMaxs(
            Vector256.Create(centerpointHalfLengths.GetLower()),
            Vector256.Create(centerpointHalfLengths.GetUpper())
            );
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector256<double> ToMinMax(in Vector128<double> centerpoint, in Vector128<double> halfLengths) {
        return ToMinsMaxs(
            Vector256.Create(centerpoint),
            Vector256.Create(halfLengths)
            );
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<float> ToMinMax(in Vector128<float> centers, in Vector128<float> halfDims) {
        if (Sse.IsSupported) {
            return Sse.Add(centers, Sse.Xor(halfDims, MinMaxSignMask128));
            }
        else if (AdvSimd.IsSupported) {
            return AdvSimd.Add(centers, AdvSimd.Xor(halfDims.AsInt32(), MinMaxSignMask128.AsInt32()).AsSingle());
            }
        else {
            return centers + Vector128.Xor(halfDims, MinMaxSignMask128);
            }
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector256<double> ToMinMax(double radius) {
        return Vector256.Xor(Vector256.Create(radius), MinMaxSignMask256);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<float> ToMinMax(float radius) {
        return Vector128.Xor(Vector128.Create(radius), MinMaxSignMask128);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector256<float> ToMinsMaxs(in Vector128<float> centers, in Vector128<float> halfDims) {
        Vector128<float> minMax128;
        if (Sse.IsSupported) {
            minMax128 = Sse.Add(centers, Sse.Xor(halfDims, MinMaxSignMask128));
            }
        else if (AdvSimd.IsSupported) {
            minMax128 = AdvSimd.Add(centers, AdvSimd.Xor(halfDims.AsInt32(), MinMaxSignMask128.AsInt32()).AsSingle());
            }
        else {
            minMax128 = centers + Vector128.Xor(halfDims, MinMaxSignMask128);
            }
        return Vector256.Create(minMax128);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector256<double> ToMinsMaxs(in Vector256<double> centers, in Vector256<double> halfDims) {
        if (Avx.IsSupported) {
            return Avx.Add(centers, Avx.Xor(halfDims, MinMaxSignMask256));
            }
        else {
            return centers + Vector256.Xor(halfDims, MinMaxSignMask256);
            }
        }

    #endregion Conversions - Centerpoint/HalfLengths → Min/Max (2D)

    #region Conversions - Min/Max → Centerpoint/HalfLengths (2D)

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ToCenterpointHalfLengths(in Vector64<float> min, in Vector64<float> max, out Vector64<float> centerpoint, out Vector64<float> halfLengths) {
        centerpoint = (min + max) * Half64F;
        halfLengths = (max - min) * Half64F;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ToCenterpointHalfLengths(in Vector128<double> min, in Vector128<double> max, out Vector128<double> centerpoint, out Vector128<double> halfLengths) {
        centerpoint = (min + max) * Half128D;
        halfLengths = (max - min) * Half128D;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ToCenterpointHalfLengths(in Vector256<double> minMax, out Vector128<double> centerpoint, out Vector128<double> halfLengths) {
        var min = minMax.GetLower();
        var max = minMax.GetUpper();
        centerpoint = (min + max) * Half128D;
        halfLengths = (max - min) * Half128D;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<float> ToCenterpointHalfLengths(in Vector128<float> minMax) {
        return StridedAdd(
            minMax, // [+min, +max]
            Vector128.Xor(
                Vector128.Shuffle(minMax, Vector128.Create(2, 3, 0, 1)),
                BoxSignMask128
                )   // [+max, -min]
            ) * Half128F;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector256<double> ToCenterpointHalfLengths(in Vector256<double> minMax) {
        return StridedAdd(
            minMax, // [+min, +max]
            Vector256.Xor(
                Vector256.Shuffle(minMax, Vector256.Create(2L, 3L, 0L, 1L)),
                BoxSignMask256
                )   // [+max, -min]
            ) * Half256D;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ToCenterpointHalfLengths(in Vector128<float> minMax, out Vector128<float> centerpointHalfLengths) {
        centerpointHalfLengths = ToCenterpointHalfLengths(in minMax);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ToCenterpointHalfLengths(in Vector256<double> minMax, out Vector256<double> centerpointHalfLengths) {
        centerpointHalfLengths = ToCenterpointHalfLengths(in minMax);
        }

    #endregion Conversions - Min/Max → Centerpoint/HalfLengths (2D)

    #region MinMax - Contains Other Min/Max

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool MinMaxContainsComparison(Vector128<float> minMax, Vector128<float> other) {
        if (Vector256.IsHardwareAccelerated && Vector256<float>.IsSupported) {
            var x = Vector256.Create(minMax, other);
            var y = Vector256.Create(other, minMax);
            return (Vector256.LessThanOrEqual(x, y).ExtractMostSignificantBits() & 0b_1100_0011) == 0b_1100_0011;
            }
        else {
            return (Vector128.LessThanOrEqual(minMax, other).ExtractMostSignificantBits() & 0b_0011) == 0b_0011
                && (Vector128.GreaterThanOrEqual(minMax, other).ExtractMostSignificantBits() & 0b_1100) == 0b_1100;
            }
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool MinMaxContainsComparison(Vector256<double> minMax, Vector256<double> other) {
        return (Vector256.LessThanOrEqual(minMax, other).ExtractMostSignificantBits() & 0b_0011) == 0b_0011
            && (Vector256.GreaterThanOrEqual(minMax, other).ExtractMostSignificantBits() & 0b_1100) == 0b_1100;
        }

    #endregion MinMax - Contains Other Min/Max

    #region MinMax - Contains Other Min/Max Without Intersecting

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool MinMaxContainsComparisonWithoutIntersecting(Vector128<float> minMax, Vector128<float> other) {
        if (Vector256.IsHardwareAccelerated && Vector256<float>.IsSupported) {
            var x = Vector256.Create(minMax, other);
            var y = Vector256.Create(other, minMax);
            return (Vector256.LessThan(x, y).ExtractMostSignificantBits() & 0b_1100_0011) == 0b_1100_0011;
            }
        else {
            return (Vector128.LessThan(minMax, other).ExtractMostSignificantBits() & 0b_0011) == 0b_0011
                && (Vector128.GreaterThan(minMax, other).ExtractMostSignificantBits() & 0b_1100) == 0b_1100;
            }
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool MinMaxContainsComparisonWithoutIntersecting(Vector256<double> minMax, Vector256<double> other) {
        return (Vector256.LessThan(minMax, other).ExtractMostSignificantBits() & 0b_0011) == 0b_0011
            && (Vector256.GreaterThan(minMax, other).ExtractMostSignificantBits() & 0b_1100) == 0b_1100;
        }

    #endregion MinMax - Contains Other Min/Max Without Intersecting

    #region MinMax - Contains Point

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool MinMaxContainsPoint(in Vector128<double> min, in Vector128<double> max, in Vector128<double> point) {
        return Vector128.BitwiseAnd(
            left: Vector128.GreaterThanOrEqual(point, min),
            right: Vector128.LessThanOrEqual(point, max)
            ).ExtractMostSignificantBits() == 0b_11;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool MinMaxContainsPoint(in Vector64<float> min, in Vector64<float> max, in Vector64<float> point) {
        return Vector64.BitwiseAnd(
            left: Vector64.GreaterThanOrEqual(point, min),
            right: Vector64.LessThanOrEqual(point, max)
            ).ExtractMostSignificantBits() == 0b_11;
        }

    #endregion MinMax - Contains Point

    #region MinMax - Contains Points

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool MinMaxContainsPoints(Vector128<double> boxMin, Vector128<double> boxMax, Vector128<double> a, Vector128<double> b, Vector128<double> c, Vector128<double> d) {
        if (Vector128.GreaterThanAny(boxMin, a) || Vector128.GreaterThanAny(boxMin, b) || Vector128.GreaterThanAny(boxMin, c) || Vector128.GreaterThanAny(boxMin, d))
            return false;
        return Vector128.GreaterThanOrEqualAll(boxMax, a)
            && Vector128.GreaterThanOrEqualAll(boxMax, b)
            && Vector128.GreaterThanOrEqualAll(boxMax, c)
            && Vector128.GreaterThanOrEqualAll(boxMax, d);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool MinMaxContainsPoints(Vector128<double> boxMin, Vector128<double> boxMax, Vector128<double> a, Vector128<double> b, Vector128<double> c) {
        if (Vector128.GreaterThanAny(boxMin, a) || Vector128.GreaterThanAny(boxMin, b) || Vector128.GreaterThanAny(boxMin, c))
            return false;
        return Vector128.GreaterThanOrEqualAll(boxMax, a)
            && Vector128.GreaterThanOrEqualAll(boxMax, b)
            && Vector128.GreaterThanOrEqualAll(boxMax, c);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool MinMaxContainsPoints(Vector128<double> boxMin, Vector128<double> boxMax, Vector128<double> a, Vector128<double> b) {
        if (Vector128.GreaterThanAny(boxMin, a) || Vector128.GreaterThanAny(boxMin, b))
            return false;
        return Vector128.GreaterThanOrEqualAll(boxMax, a)
            && Vector128.GreaterThanOrEqualAll(boxMax, b);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool MinMaxContainsPoints(Vector64<float> boxMin, Vector64<float> boxMax, Vector64<float> a, Vector64<float> b, Vector64<float> c, Vector64<float> d) {
        if (Vector64.GreaterThanAny(boxMin, a) || Vector64.GreaterThanAny(boxMin, b) || Vector64.GreaterThanAny(boxMin, c) || Vector64.GreaterThanAny(boxMin, d))
            return false;
        return Vector64.GreaterThanOrEqualAll(boxMax, a)
            && Vector64.GreaterThanOrEqualAll(boxMax, b)
            && Vector64.GreaterThanOrEqualAll(boxMax, c)
            && Vector64.GreaterThanOrEqualAll(boxMax, d);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool MinMaxContainsPoints(Vector64<float> boxMin, Vector64<float> boxMax, Vector64<float> a, Vector64<float> b, Vector64<float> c) {
        if (Vector64.GreaterThanAny(boxMin, a) || Vector64.GreaterThanAny(boxMin, b) || Vector64.GreaterThanAny(boxMin, c))
            return false;
        return Vector64.GreaterThanOrEqualAll(boxMax, a)
            && Vector64.GreaterThanOrEqualAll(boxMax, b)
            && Vector64.GreaterThanOrEqualAll(boxMax, c);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool MinMaxContainsPoints(Vector64<float> boxMin, Vector64<float> boxMax, Vector64<float> a, Vector64<float> b) {
        if (Vector64.GreaterThanAny(boxMin, a) || Vector64.GreaterThanAny(boxMin, b))
            return false;
        return Vector64.GreaterThanOrEqualAll(boxMax, a)
            && Vector64.GreaterThanOrEqualAll(boxMax, b);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool MinMaxContainsPoints(Vector128<float> minMax, Vector128<float> a, Vector128<float> b) {
        var signs = Vector128.Create(+0.0f, +0.0f, -0.0f, -0.0f);
        var mm = Vector128.Xor(minMax, signs);
        var ax = Vector128.Xor(a, signs);
        var bx = Vector128.Xor(b, signs);
        return Vector128.ExtractMostSignificantBits(
                Vector128.BitwiseAnd(
                    Vector128.LessThanOrEqual(mm, ax),
                    Vector128.LessThanOrEqual(mm, bx))) == 0b_1111;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool MinMaxContainsPoints(Vector128<float> minMax, Vector128<float> a, Vector128<float> b, Vector128<float> c) {
        var signs = Vector128.Create(+0.0f, +0.0f, -0.0f, -0.0f);
        var mm = Vector128.Xor(minMax, signs);
        var ax = Vector128.Xor(a, signs);
        var bx = Vector128.Xor(b, signs);
        var cx = Vector128.Xor(c, signs);
        return Vector128.ExtractMostSignificantBits(
                Vector128.BitwiseAnd(Vector128.BitwiseAnd(
                    Vector128.LessThanOrEqual(mm, ax),
                    Vector128.LessThanOrEqual(mm, bx)),
                    Vector128.LessThanOrEqual(mm, cx))) == 0b_1111;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool MinMaxContainsPoints(Vector128<float> minMax, Vector128<float> a, Vector128<float> b, Vector128<float> c, Vector128<float> d) {
        var signs = Vector128.Create(+0.0f, +0.0f, -0.0f, -0.0f);
        var mm = Vector128.Xor(minMax, signs);
        var ax = Vector128.Xor(a, signs);
        var bx = Vector128.Xor(b, signs);
        var cx = Vector128.Xor(c, signs);
        var dx = Vector128.Xor(d, signs);
        return Vector128.BitwiseAnd(
                    Vector128.BitwiseAnd(
                        Vector128.LessThanOrEqual(mm, ax),
                        Vector128.LessThanOrEqual(mm, bx)
                        ),
                    Vector128.BitwiseAnd(
                        Vector128.LessThanOrEqual(mm, cx),
                        Vector128.LessThanOrEqual(mm, dx)
                        )).ExtractMostSignificantBits() == 0b_1111;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool MinMaxContainsPoints(Vector256<double> minMax, Vector256<double> a, Vector256<double> b, Vector256<double> c, Vector256<double> d) {
        var signs = Vector256.Create(+0.0, +0.0, -0.0, -0.0);
        var mm = Vector256.Xor(minMax, signs);
        var ax = Vector256.Xor(a, signs);
        var bx = Vector256.Xor(b, signs);
        var cx = Vector256.Xor(c, signs);
        var dx = Vector256.Xor(d, signs);
        return Vector256.BitwiseAnd(
                    Vector256.BitwiseAnd(
                        Vector256.LessThanOrEqual(mm, ax),
                        Vector256.LessThanOrEqual(mm, bx)
                        ),
                    Vector256.BitwiseAnd(
                        Vector256.LessThanOrEqual(mm, cx),
                        Vector256.LessThanOrEqual(mm, dx)
                        )).ExtractMostSignificantBits() == 0b_1111;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool MinMaxContainsPoints(Vector256<double> minMax, Vector256<double> a, Vector256<double> b) {
        var signs = Vector256.Create(+0.0, +0.0, -0.0, -0.0);
        var mm = Vector256.Xor(minMax, signs);
        var ax = Vector256.Xor(a, signs);
        var bx = Vector256.Xor(b, signs);
        return Vector256.ExtractMostSignificantBits(
                Vector256.BitwiseAnd(
                    Vector256.LessThanOrEqual(mm, ax),
                    Vector256.LessThanOrEqual(mm, bx))) == 0b_1111;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool MinMaxContainsPoints(Vector256<double> minMax, Vector256<double> a, Vector256<double> b, Vector256<double> c) {
        var signs = Vector256.Create(+0.0, +0.0, -0.0, -0.0);
        var mm = Vector256.Xor(minMax, signs);
        var ax = Vector256.Xor(a, signs);
        var bx = Vector256.Xor(b, signs);
        var cx = Vector256.Xor(c, signs);
        return Vector256.ExtractMostSignificantBits(
                Vector256.BitwiseAnd(Vector256.BitwiseAnd(
                    Vector256.LessThanOrEqual(mm, ax),
                    Vector256.LessThanOrEqual(mm, bx)),
                    Vector256.LessThanOrEqual(mm, cx))) == 0b_1111;
        }

    #endregion MinMax - Contains Points

    #region MinMax - Contains Point Without Intersecting

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool MinMaxContainsPointWithoutIntersecting(in Vector128<double> min, in Vector128<double> max, in Vector128<double> point) {
        return Vector128.BitwiseAnd(
            left: Vector128.GreaterThan(point, min),
            right: Vector128.LessThan(point, max)
            ).ExtractMostSignificantBits() == 0b_11;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool MinMaxContainsPointWithoutIntersecting(in Vector64<float> min, in Vector64<float> max, in Vector64<float> point) {
        return Vector64.BitwiseAnd(
            left: Vector64.GreaterThan(point, min),
            right: Vector64.LessThan(point, max)
            ).ExtractMostSignificantBits() == 0b_11;
        }

    #endregion MinMax - Contains Point Without Intersecting

    #region MinMax - Contains Points Without Intersecting

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool MinMaxContainsPointsWithoutIntersecting(Vector128<double> boxMin, Vector128<double> boxMax, Vector128<double> a, Vector128<double> b, Vector128<double> c, Vector128<double> d) {
        if (Vector128.GreaterThanOrEqualAny(boxMin, a) || Vector128.GreaterThanOrEqualAny(boxMin, b) || Vector128.GreaterThanOrEqualAny(boxMin, c) || Vector128.GreaterThanOrEqualAny(boxMin, d))
            return false;
        return Vector128.GreaterThanAll(boxMax, a)
            && Vector128.GreaterThanAll(boxMax, b)
            && Vector128.GreaterThanAll(boxMax, c)
            && Vector128.GreaterThanAll(boxMax, d);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool MinMaxContainsPointsWithoutIntersecting(Vector128<double> boxMin, Vector128<double> boxMax, Vector128<double> a, Vector128<double> b, Vector128<double> c) {
        if (Vector128.GreaterThanOrEqualAny(boxMin, a) || Vector128.GreaterThanOrEqualAny(boxMin, b) || Vector128.GreaterThanOrEqualAny(boxMin, c))
            return false;
        return Vector128.GreaterThanAll(boxMax, a)
            && Vector128.GreaterThanAll(boxMax, b)
            && Vector128.GreaterThanAll(boxMax, c);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool MinMaxContainsPointsWithoutIntersecting(Vector128<double> boxMin, Vector128<double> boxMax, Vector128<double> a, Vector128<double> b) {
        if (Vector128.GreaterThanOrEqualAny(boxMin, a) || Vector128.GreaterThanOrEqualAny(boxMin, b))
            return false;
        return Vector128.GreaterThanAll(boxMax, a)
            && Vector128.GreaterThanAll(boxMax, b);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool MinMaxContainsPointsWithoutIntersecting(Vector64<float> boxMin, Vector64<float> boxMax, Vector64<float> a, Vector64<float> b, Vector64<float> c, Vector64<float> d) {
        if (Vector64.GreaterThanOrEqualAny(boxMin, a) || Vector64.GreaterThanOrEqualAny(boxMin, b) || Vector64.GreaterThanOrEqualAny(boxMin, c) || Vector64.GreaterThanOrEqualAny(boxMin, d))
            return false;
        return Vector64.GreaterThanAll(boxMax, a)
            && Vector64.GreaterThanAll(boxMax, b)
            && Vector64.GreaterThanAll(boxMax, c)
            && Vector64.GreaterThanAll(boxMax, d);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool MinMaxContainsPointsWithoutIntersecting(Vector64<float> boxMin, Vector64<float> boxMax, Vector64<float> a, Vector64<float> b, Vector64<float> c) {
        if (Vector64.GreaterThanOrEqualAny(boxMin, a) || Vector64.GreaterThanOrEqualAny(boxMin, b) || Vector64.GreaterThanOrEqualAny(boxMin, c))
            return false;
        return Vector64.GreaterThanAll(boxMax, a)
            && Vector64.GreaterThanAll(boxMax, b)
            && Vector64.GreaterThanAll(boxMax, c);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool MinMaxContainsPointsWithoutIntersecting(Vector64<float> boxMin, Vector64<float> boxMax, Vector64<float> a, Vector64<float> b) {
        if (Vector64.GreaterThanOrEqualAny(boxMin, a) || Vector64.GreaterThanOrEqualAny(boxMin, b))
            return false;
        return Vector64.GreaterThanAll(boxMax, a)
            && Vector64.GreaterThanAll(boxMax, b);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool MinMaxContainsPointsWithoutIntersecting(Vector128<float> minMax, Vector128<float> a, Vector128<float> b) {
        var signs = Vector128.Create(+0.0f, +0.0f, -0.0f, -0.0f);
        var mm = Vector128.Xor(minMax, signs);
        var ax = Vector128.Xor(a, signs);
        var bx = Vector128.Xor(b, signs);
        return Vector128.ExtractMostSignificantBits(
                Vector128.BitwiseAnd(
                    Vector128.LessThan(mm, ax),
                    Vector128.LessThan(mm, bx))) == 0b_1111;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool MinMaxContainsPointsWithoutIntersecting(Vector128<float> minMax, Vector128<float> a, Vector128<float> b, Vector128<float> c) {
        var signs = Vector128.Create(+0.0f, +0.0f, -0.0f, -0.0f);
        var mm = Vector128.Xor(minMax, signs);
        var ax = Vector128.Xor(a, signs);
        var bx = Vector128.Xor(b, signs);
        var cx = Vector128.Xor(c, signs);
        return Vector128.ExtractMostSignificantBits(
                Vector128.BitwiseAnd(Vector128.BitwiseAnd(
                    Vector128.LessThan(mm, ax),
                    Vector128.LessThan(mm, bx)),
                    Vector128.LessThan(mm, cx))) == 0b_1111;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool MinMaxContainsPointsWithoutIntersecting(Vector128<float> minMax, Vector128<float> a, Vector128<float> b, Vector128<float> c, Vector128<float> d) {
        var signs = Vector128.Create(+0.0f, +0.0f, -0.0f, -0.0f);
        var mm = Vector128.Xor(minMax, signs);
        var ax = Vector128.Xor(a, signs);
        var bx = Vector128.Xor(b, signs);
        var cx = Vector128.Xor(c, signs);
        var dx = Vector128.Xor(d, signs);
        return Vector128.BitwiseAnd(
                    Vector128.BitwiseAnd(
                        Vector128.LessThan(mm, ax),
                        Vector128.LessThan(mm, bx)
                        ),
                    Vector128.BitwiseAnd(
                        Vector128.LessThan(mm, cx),
                        Vector128.LessThan(mm, dx)
                        )).ExtractMostSignificantBits() == 0b_1111;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool MinMaxContainsPointsWithoutIntersecting(Vector256<double> minMax, Vector256<double> a, Vector256<double> b, Vector256<double> c, Vector256<double> d) {
        var signs = Vector256.Create(+0.0, +0.0, -0.0, -0.0);
        var mm = Vector256.Xor(minMax, signs);
        var ax = Vector256.Xor(a, signs);
        var bx = Vector256.Xor(b, signs);
        var cx = Vector256.Xor(c, signs);
        var dx = Vector256.Xor(d, signs);
        return Vector256.BitwiseAnd(
                    Vector256.BitwiseAnd(
                        Vector256.LessThan(mm, ax),
                        Vector256.LessThan(mm, bx)
                        ),
                    Vector256.BitwiseAnd(
                        Vector256.LessThan(mm, cx),
                        Vector256.LessThan(mm, dx)
                        )).ExtractMostSignificantBits() == 0b_1111;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool MinMaxContainsPointsWithoutIntersecting(Vector256<double> minMax, Vector256<double> a, Vector256<double> b) {
        var signs = Vector256.Create(+0.0, +0.0, -0.0, -0.0);
        var mm = Vector256.Xor(minMax, signs);
        var ax = Vector256.Xor(a, signs);
        var bx = Vector256.Xor(b, signs);
        return Vector256.ExtractMostSignificantBits(
                Vector256.BitwiseAnd(
                    Vector256.LessThan(mm, ax),
                    Vector256.LessThan(mm, bx))) == 0b_1111;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool MinMaxContainsPointsWithoutIntersecting(Vector256<double> minMax, Vector256<double> a, Vector256<double> b, Vector256<double> c) {
        var signs = Vector256.Create(+0.0, +0.0, -0.0, -0.0);
        var mm = Vector256.Xor(minMax, signs);
        var ax = Vector256.Xor(a, signs);
        var bx = Vector256.Xor(b, signs);
        var cx = Vector256.Xor(c, signs);
        return Vector256.ExtractMostSignificantBits(
                Vector256.BitwiseAnd(Vector256.BitwiseAnd(
                    Vector256.LessThan(mm, ax),
                    Vector256.LessThan(mm, bx)),
                    Vector256.LessThan(mm, cx))) == 0b_1111;
        }

    #endregion MinMax - Contains Points Without Intersecting

    #region Dominance Masks

    /// <inheritdoc
    /// cref="LeastDominantMask(Vector128{float})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector256<double> LeastDominantMask(Vector256<double> vector) {
        var abs = Vector256.Abs(vector);
        var bits = Vector256.Equals(
            abs,
            Vector256.Create(abs.HorizontalMin())
            ).ExtractMostSignificantBits() & 0b_1111U;

        var lowestBit = X86.Bmi1.IsSupported
        ? X86.Bmi1.ExtractLowestSetBit(bits)
        : bits & (uint)-(int)bits;

        var lane = Vector256.Equals(
            Vector256.Create((ulong)lowestBit),
            Vector256.Create(0b_0001UL, 0b_0010UL, 0b_0100UL, 0b_1000UL)
            ).AsDouble();

        return Vector256.ConditionalSelect(lane, Vector256<double>.One, Vector256<double>.Zero);
        }

    /// <summary> The elementary basis vector for the lane of <paramref name="vector"/> holding the lowest-maginitude value (ties resolve toward the lower lanes).
    /// </summary>
    /// <remarks><u>Example:</u>
    /// <para/><c>LeastDominantMask([+5, -2, +3, +1]) => [0, 0, 0, 1]</c>
    /// <para/><u>Example:</u>
    /// <para/><c>LeastDominantMask([+1.0, -2.0, +0.5, -0.5]) => [0, 0, 1, 0]</c>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<float> LeastDominantMask(Vector128<float> vector) {
        var abs = Vector128.Abs(vector);
        var bits = Vector128.Equals(
            abs,
            Vector128.Create(abs.HorizontalMin())
            ).ExtractMostSignificantBits() & 0b_1111U;

        var lowestBit = X86.Bmi1.IsSupported
        ? X86.Bmi1.ExtractLowestSetBit(bits)
        : bits & (uint)-(int)bits;

        var lane = Vector128.Equals(
            Vector128.Create(lowestBit),
            Vector128.Create(0b_0001U, 0b_0010U, 0b_0100U, 0b_1000U)
            ).AsSingle();

        return Vector128.ConditionalSelect(lane, Vector128<float>.One, Vector128<float>.Zero);
        }

    /// <inheritdoc
    /// cref="MostDominantMask(Vector128{float})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector256<double> MostDominantMask(Vector256<double> vector) {
        var abs = Vector256.Abs(vector);
        var bits = Vector256.Equals(
            abs,
            Vector256.Create(abs.HorizontalMax())
            ).ExtractMostSignificantBits() & 0b_1111U;

        var lowestBit = X86.Bmi1.IsSupported
        ? X86.Bmi1.ExtractLowestSetBit(bits)
        : bits & (uint)-(int)bits;

        var lane = Vector256.Equals(
            Vector256.Create((ulong)lowestBit),
            Vector256.Create(0b_0001UL, 0b_0010UL, 0b_0100UL, 0b_1000UL)
            ).AsDouble();

        return Vector256.ConditionalSelect(lane, Vector256<double>.One, Vector256<double>.Zero);
        }

    /// <summary> The elementary basis vector for the lane of <paramref name="vector"/> holding the highest-maginitude value (ties resolve toward the lower lanes).
    /// </summary>
    /// <remarks><u>Example:</u>
    /// <para/><c>LeastDominantMask([+1, -2, +3, +5]) => [0, 0, 0, 1]</c>
    /// <para/><u>Example:</u>
    /// <para/><c>LeastDominantMask([+2, -3, +3, -1]) => [0, 1, 0, 0]</c>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<float> MostDominantMask(Vector128<float> vector) {
        var abs = Vector128.Abs(vector);
        var bits = Vector128.Equals(
            abs,
            Vector128.Create(abs.HorizontalMax())
            ).ExtractMostSignificantBits() & 0b_1111U;

        var lowestBit = X86.Bmi1.IsSupported
        ? X86.Bmi1.ExtractLowestSetBit(bits)
        : bits & (uint)-(int)bits;

        var lane = Vector128.Equals(
            Vector128.Create(lowestBit),
            Vector128.Create(0b_0001U, 0b_0010U, 0b_0100U, 0b_1000U)
            ).AsSingle();

        return Vector128.ConditionalSelect(lane, Vector128<float>.One, Vector128<float>.Zero);
        }

    #endregion
    }