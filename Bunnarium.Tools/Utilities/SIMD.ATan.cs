using System.Runtime.Intrinsics;
namespace Bunnarium.Tools.Utilities;

[BunnyAttributes.Citation("Adopted from DirectXMath")]
public static unsafe partial class SIMD {


    /// <summary> Returns the <see cref="Math.Atan(double)">arctangent</see> of each element in the input <paramref name="vector"/>.
    /// </summary>
    /// <remarks> ⚠️ <b>Note:</b> For double-precision values, the average error between this method and <see cref="Math.Atan(double)"/> is <c>~5.62e-13</c>.
    /// </remarks>
    public static Vector128<T> Atan<T>(this Vector128<T> vector) where T : unmanaged, IBinaryFloatingPointIeee754<T> {

        if (typeof(T) == typeof(float)) {

            var V = Unsafe.BitCast<Vector128<T>, Vector128<float>>(vector);

            if (Vector128.IsHardwareAccelerated && Vector128<float>.IsSupported) {

                var absV = Vector128.Abs(V);
                var invV = Vector128<float>.One / absV;

                var largeMask = Vector128.GreaterThan(absV, Vector128<float>.One);
                var x = Vector128.ConditionalSelect(largeMask, invV, absV);

                var sign = (V & Vector128.Create(-0.0f)) | Vector128<float>.One;

                var x2 = x * x;

                // using the constants from DirectXMath
                var p = Vector128.Create(+0.0028662257f);                                   // x^17
                p = Vector128.FusedMultiplyAdd(p, x2, Vector128.Create(-0.0161657367f));    // x^15
                p = Vector128.FusedMultiplyAdd(p, x2, Vector128.Create(+0.0429096138f));    // x^13
                p = Vector128.FusedMultiplyAdd(p, x2, Vector128.Create(-0.0752896400f));    // x^11
                p = Vector128.FusedMultiplyAdd(p, x2, Vector128.Create(+0.1065626393f));    // x^9
                p = Vector128.FusedMultiplyAdd(p, x2, Vector128.Create(-0.1420889944f));    // x^7
                p = Vector128.FusedMultiplyAdd(p, x2, Vector128.Create(+0.1999355085f));    // x^5
                p = Vector128.FusedMultiplyAdd(p, x2, Vector128.Create(-0.3333314528f));    // x^3
                p = Vector128.FusedMultiplyAdd(p, x2, Vector128<float>.One);                // x^1
                p *= x;

                var halfPi = Vector128.Create(MathF.PI / 2.0f);
                var result = Vector128.ConditionalSelect(
                    condition: largeMask,
                    left: sign * (halfPi - p),
                    right: sign * p
                    );

                return Unsafe.BitCast<Vector128<float>, Vector128<T>>(result);
                }
            else {
                var ret = Vector128.Create(
                    e0: float.Atan(V[0]),
                    e1: float.Atan(V[1]),
                    e2: float.Atan(V[2]),
                    e3: float.Atan(V[3])
                    );
                return Unsafe.BitCast<Vector128<float>, Vector128<T>>(ret);
                }
            }
        else if (typeof(T) == typeof(double)) {

            var V = Unsafe.BitCast<Vector128<T>, Vector128<double>>(vector);

            if (Vector128.IsHardwareAccelerated && Vector128<double>.IsSupported) {

                var absV = Vector128.Abs(V);
                var invV = Vector128<double>.One / absV;

                var largeMask = Vector128.GreaterThan(absV, Vector128<double>.One);
                var x = Vector128.ConditionalSelect(largeMask, invV, absV);

                var sign = (V & Vector128.Create(-0.0)) | Vector128<double>.One;

                var x2 = x * x;

                /* 27-degree minimax polynomial (accuracy=~5.62e-13, this is the most accurate iteration count that we could achieve with the Remez algorithm before suffering numerical instability issues */
                var p = Vector128.Create(-2.3593211833982126e-04);                                  // x^27
                p = Vector128.FusedMultiplyAdd(p, x2, Vector128.Create(+1.9543019071630030e-03));   // x^25
                p = Vector128.FusedMultiplyAdd(p, x2, Vector128.Create(-7.6115819997874369e-03));   // x^23
                p = Vector128.FusedMultiplyAdd(p, x2, Vector128.Create(+1.8820639329690764e-02));   // x^21
                p = Vector128.FusedMultiplyAdd(p, x2, Vector128.Create(-3.3981266187505396e-02));   // x^19
                p = Vector128.FusedMultiplyAdd(p, x2, Vector128.Create(+4.9445401817719919e-02));   // x^17
                p = Vector128.FusedMultiplyAdd(p, x2, Vector128.Create(-6.3109550003480008e-02));   // x^15
                p = Vector128.FusedMultiplyAdd(p, x2, Vector128.Create(+7.5933114542454952e-02));   // x^13
                p = Vector128.FusedMultiplyAdd(p, x2, Vector128.Create(-9.0713498883979435e-02));   // x^11
                p = Vector128.FusedMultiplyAdd(p, x2, Vector128.Create(+1.1108488465450911e-01));   // x^9
                p = Vector128.FusedMultiplyAdd(p, x2, Vector128.Create(-1.4285491043242010e-01));   // x^7
                p = Vector128.FusedMultiplyAdd(p, x2, Vector128.Create(+1.9999989165458310e-01));   // x^5
                p = Vector128.FusedMultiplyAdd(p, x2, Vector128.Create(-3.3333333086700506e-01));   // x^3
                p = Vector128.FusedMultiplyAdd(p, x2, Vector128.Create(+9.9999999998328404e-01));   // x^1
                p *= x;

                var halfPi = Vector128.Create(Math.PI / 2.0);
                var result = Vector128.ConditionalSelect(
                    condition: largeMask,
                    left: sign * (halfPi - p),
                    right: sign * p
                    );

                return Unsafe.BitCast<Vector128<double>, Vector128<T>>(result);
                }
            else {
                var ret = Vector128.Create(
                    e0: double.Atan(V[0]),
                    e1: double.Atan(V[1])
                    );
                return Unsafe.BitCast<Vector128<double>, Vector128<T>>(ret);
                }
            }
        else { // half-precision
            throw new NotImplementedException("Half-precision Vector128s weren't supported at time of writing...");
            }
        }

    /// <inheritdoc
    /// cref="Atan{T}(Vector128{T})"/>
    public static Vector256<T> Atan<T>(this Vector256<T> vector) where T : unmanaged, IBinaryFloatingPointIeee754<T> {

        if (typeof(T) == typeof(float)) {

            var V = Unsafe.BitCast<Vector256<T>, Vector256<float>>(vector);

            if (Vector256.IsHardwareAccelerated && Vector256<float>.IsSupported) {

                var absV = Vector256.Abs(V);
                var invV = Vector256<float>.One / absV;

                var largeMask = Vector256.GreaterThan(absV, Vector256<float>.One);
                var x = Vector256.ConditionalSelect(largeMask, invV, absV);

                var sign = (V & Vector256.Create(-0.0f)) | Vector256<float>.One;

                Vector256<float> x2 = x * x;

                // using the constants from DirectXMath
                Vector256<float> p = Vector256.Create(+0.0028662257f);                      // x^17
                p = Vector256.FusedMultiplyAdd(p, x2, Vector256.Create(-0.0161657367f));    // x^15
                p = Vector256.FusedMultiplyAdd(p, x2, Vector256.Create(+0.0429096138f));    // x^13
                p = Vector256.FusedMultiplyAdd(p, x2, Vector256.Create(-0.0752896400f));    // x^11
                p = Vector256.FusedMultiplyAdd(p, x2, Vector256.Create(+0.1065626393f));    // x^9
                p = Vector256.FusedMultiplyAdd(p, x2, Vector256.Create(-0.1420889944f));    // x^7
                p = Vector256.FusedMultiplyAdd(p, x2, Vector256.Create(+0.1999355085f));    // x^5
                p = Vector256.FusedMultiplyAdd(p, x2, Vector256.Create(-0.3333314528f));    // x^3
                p = Vector256.FusedMultiplyAdd(p, x2, Vector256<float>.One);                // x^1
                p *= x;

                var halfPi = Vector256.Create(MathF.PI / 2.0f);
                var result = Vector256.ConditionalSelect(
                    condition: largeMask,
                    left: sign * (halfPi - p),
                    right: sign * p
                    );

                return Unsafe.BitCast<Vector256<float>, Vector256<T>>(result);
                }
            else {
                var ret = Vector256.Create(
                    e0: float.Atan(V[0]),
                    e1: float.Atan(V[1]),
                    e2: float.Atan(V[2]),
                    e3: float.Atan(V[3]),
                    e4: float.Atan(V[4]),
                    e5: float.Atan(V[5]),
                    e6: float.Atan(V[6]),
                    e7: float.Atan(V[7])
                    );
                return Unsafe.BitCast<Vector256<float>, Vector256<T>>(ret);
                }
            }
        else if (typeof(T) == typeof(double)) {

            var V = Unsafe.BitCast<Vector256<T>, Vector256<double>>(vector);

            if (Vector256.IsHardwareAccelerated && Vector256<double>.IsSupported) {

                var absV = Vector256.Abs(V);
                var invV = Vector256<double>.One / absV;

                var largeMask = Vector256.GreaterThan(absV, Vector256<double>.One);
                var x = Vector256.ConditionalSelect(largeMask, invV, absV);

                var sign = (V & Vector256.Create(-0.0)) | Vector256<double>.One;

                var x2 = x * x;

                /* 27-degree minimax polynomial (accuracy=~5.62e-13, this is the most accurate iteration count that we could achieve with the Remez algorithm before suffering numerical instability issues */
                Vector256<double> p = Vector256.Create(-2.3593211833982126e-04);                    // x^27
                p = Vector256.FusedMultiplyAdd(p, x2, Vector256.Create(+1.9543019071630030e-03));   // x^25
                p = Vector256.FusedMultiplyAdd(p, x2, Vector256.Create(-7.6115819997874369e-03));   // x^23
                p = Vector256.FusedMultiplyAdd(p, x2, Vector256.Create(+1.8820639329690764e-02));   // x^21
                p = Vector256.FusedMultiplyAdd(p, x2, Vector256.Create(-3.3981266187505396e-02));   // x^19
                p = Vector256.FusedMultiplyAdd(p, x2, Vector256.Create(+4.9445401817719919e-02));   // x^17
                p = Vector256.FusedMultiplyAdd(p, x2, Vector256.Create(-6.3109550003480008e-02));   // x^15
                p = Vector256.FusedMultiplyAdd(p, x2, Vector256.Create(+7.5933114542454952e-02));   // x^13
                p = Vector256.FusedMultiplyAdd(p, x2, Vector256.Create(-9.0713498883979435e-02));   // x^11
                p = Vector256.FusedMultiplyAdd(p, x2, Vector256.Create(+1.1108488465450911e-01));   // x^9
                p = Vector256.FusedMultiplyAdd(p, x2, Vector256.Create(-1.4285491043242010e-01));   // x^7
                p = Vector256.FusedMultiplyAdd(p, x2, Vector256.Create(+1.9999989165458310e-01));   // x^5
                p = Vector256.FusedMultiplyAdd(p, x2, Vector256.Create(-3.3333333086700506e-01));   // x^3
                p = Vector256.FusedMultiplyAdd(p, x2, Vector256.Create(+9.9999999998328404e-01));   // x^1
                p *= x;

                var halfPi = Vector256.Create(Math.PI / 2.0);
                var result = Vector256.ConditionalSelect(
                    condition: largeMask,
                    left: sign * (halfPi - p),
                    right: sign * p
                    );

                return Unsafe.BitCast<Vector256<double>, Vector256<T>>(result);
                }
            else {
                var ret = Vector256.Create(
                    e0: double.Atan(V[0]),
                    e1: double.Atan(V[1]),
                    e2: double.Atan(V[2]),
                    e3: double.Atan(V[3])
                    );
                return Unsafe.BitCast<Vector256<double>, Vector256<T>>(ret);
                }
            }
        else { // half-precision
            throw new NotImplementedException("Half-precision Vector256s weren't supported at time of writing...");
            }
        }

    /// <inheritdoc
    /// cref="Atan{T}(Vector128{T})"/>
    public static Vector512<T> Atan<T>(this Vector512<T> vector) where T : unmanaged, IBinaryFloatingPointIeee754<T> {

        if (typeof(T) == typeof(float)) {

            var V = Unsafe.BitCast<Vector512<T>, Vector512<float>>(vector);

            if (Vector512.IsHardwareAccelerated && Vector512<float>.IsSupported) {

                var absV = Vector512.Abs(V);
                var invV = Vector512<float>.One / absV;

                var largeMask = Vector512.GreaterThan(absV, Vector512<float>.One);
                var x = Vector512.ConditionalSelect(largeMask, invV, absV);


                var sign = (V & Vector512.Create(-0.0f)) | Vector512<float>.One;

                Vector512<float> x2 = x * x;

                // using the constants from DirectXMath
                Vector512<float> p = Vector512.Create(+0.0028662257f);                      // x^17
                p = Vector512.FusedMultiplyAdd(p, x2, Vector512.Create(-0.0161657367f));    // x^15
                p = Vector512.FusedMultiplyAdd(p, x2, Vector512.Create(+0.0429096138f));    // x^13
                p = Vector512.FusedMultiplyAdd(p, x2, Vector512.Create(-0.0752896400f));    // x^11
                p = Vector512.FusedMultiplyAdd(p, x2, Vector512.Create(+0.1065626393f));    // x^9
                p = Vector512.FusedMultiplyAdd(p, x2, Vector512.Create(-0.1420889944f));    // x^7
                p = Vector512.FusedMultiplyAdd(p, x2, Vector512.Create(+0.1999355085f));    // x^5
                p = Vector512.FusedMultiplyAdd(p, x2, Vector512.Create(-0.3333314528f));    // x^3
                p = Vector512.FusedMultiplyAdd(p, x2, Vector512<float>.One);                // x^1
                p *= x;

                var halfPi = Vector512.Create(MathF.PI / 2.0f);
                var result = Vector512.ConditionalSelect(
                    condition: largeMask,
                    left: sign * (halfPi - p),
                    right: sign * p
                    );

                return Unsafe.BitCast<Vector512<float>, Vector512<T>>(result);
                }
            else {
                var ret = Vector512.Create(
                    e0: float.Atan(V[0]),
                    e1: float.Atan(V[1]),
                    e2: float.Atan(V[2]),
                    e3: float.Atan(V[3]),
                    e4: float.Atan(V[4]),
                    e5: float.Atan(V[5]),
                    e6: float.Atan(V[6]),
                    e7: float.Atan(V[7]),
                    e8: float.Atan(V[8]),
                    e9: float.Atan(V[9]),
                    e10: float.Atan(V[10]),
                    e11: float.Atan(V[11]),
                    e12: float.Atan(V[12]),
                    e13: float.Atan(V[13]),
                    e14: float.Atan(V[14]),
                    e15: float.Atan(V[15])
                    );
                return Unsafe.BitCast<Vector512<float>, Vector512<T>>(ret);
                }
            }
        else if (typeof(T) == typeof(double)) {

            var V = Unsafe.BitCast<Vector512<T>, Vector512<double>>(vector);

            if (Vector512.IsHardwareAccelerated && Vector512<double>.IsSupported) {

                var absV = Vector512.Abs(V);
                var invV = Vector512<double>.One / absV;

                var largeMask = Vector512.GreaterThan(absV, Vector512<double>.One);
                var x = Vector512.ConditionalSelect(largeMask, invV, absV);

                var sign = (V & Vector512.Create(-0.0)) | Vector512<double>.One;

                var x2 = x * x;

                /* 27-degree minimax polynomial (accuracy=~5.62e-13, this is the most accurate iteration count that we could achieve with the Remez algorithm before suffering numerical instability issues */
                Vector512<double> p = Vector512.Create(-2.3593211833982126e-04);                    // x^27
                p = Vector512.FusedMultiplyAdd(p, x2, Vector512.Create(+1.9543019071630030e-03));   // x^25
                p = Vector512.FusedMultiplyAdd(p, x2, Vector512.Create(-7.6115819997874369e-03));   // x^23
                p = Vector512.FusedMultiplyAdd(p, x2, Vector512.Create(+1.8820639329690764e-02));   // x^21
                p = Vector512.FusedMultiplyAdd(p, x2, Vector512.Create(-3.3981266187505396e-02));   // x^19
                p = Vector512.FusedMultiplyAdd(p, x2, Vector512.Create(+4.9445401817719919e-02));   // x^17
                p = Vector512.FusedMultiplyAdd(p, x2, Vector512.Create(-6.3109550003480008e-02));   // x^15
                p = Vector512.FusedMultiplyAdd(p, x2, Vector512.Create(+7.5933114542454952e-02));   // x^13
                p = Vector512.FusedMultiplyAdd(p, x2, Vector512.Create(-9.0713498883979435e-02));   // x^11
                p = Vector512.FusedMultiplyAdd(p, x2, Vector512.Create(+1.1108488465450911e-01));   // x^9
                p = Vector512.FusedMultiplyAdd(p, x2, Vector512.Create(-1.4285491043242010e-01));   // x^7
                p = Vector512.FusedMultiplyAdd(p, x2, Vector512.Create(+1.9999989165458310e-01));   // x^5
                p = Vector512.FusedMultiplyAdd(p, x2, Vector512.Create(-3.3333333086700506e-01));   // x^3
                p = Vector512.FusedMultiplyAdd(p, x2, Vector512.Create(+9.9999999998328404e-01));   // x^1
                p *= x;

                var halfPi = Vector512.Create(Math.PI / 2.0);
                var result = Vector512.ConditionalSelect(
                    condition: largeMask,
                    left: sign * (halfPi - p),
                    right: sign * p
                    );

                return Unsafe.BitCast<Vector512<double>, Vector512<T>>(result);
                }
            else {
                var ret = Vector512.Create(
                    e0: double.Atan(V[0]),
                    e1: double.Atan(V[1]),
                    e2: double.Atan(V[2]),
                    e3: double.Atan(V[3]),
                    e4: double.Atan(V[4]),
                    e5: double.Atan(V[5]),
                    e6: double.Atan(V[6]),
                    e7: double.Atan(V[7])
                    );
                return Unsafe.BitCast<Vector512<double>, Vector512<T>>(ret);
                }
            }
        else { // half-precision
            throw new NotImplementedException("Half-precision Vector512s weren't supported at time of writing...");
            }
        }

    /// <summary> Returns the <see cref="Math.Atan2(double, double)">arctangent</see> of the quotient of each component-wise y-x pair in the given vectors.
    /// </summary>
    /// <remarks> ⚠️ <b>Note:</b> For double-precision values, the average error between this method and <see cref="Math.Atan(double)"/> is <c>~3.1394e-7</c>, which is many orders of magnitude greater than the limits of double-precision values. Do not use this for double-precision values when greater levels of precision are needed. Similarly, this function may produce very small errors for single precision values. These errors are, on average, smaller than the limits of single-precision values and are usually negligible.
    /// </remarks>
    [BunnyAttributes.Note("Regarding the double-precision accuracy issue mentioned in the documentation, it's likely not the result of numerical instability. The error was very slightly higher when using the 21st-degree polynomial constants relative to the 27th-degree constants, meaning that there may be a limitation in the atan2 implementation itself. I haven't figured this one out yet.")]
    public static Vector128<T> Atan2<T>(this Vector128<T> yt, Vector128<T> xt) where T : unmanaged, IBinaryFloatingPointIeee754<T> {

        if (typeof(T) == typeof(float)) {

            var x = Unsafe.BitCast<Vector128<T>, Vector128<float>>(xt);
            var y = Unsafe.BitCast<Vector128<T>, Vector128<float>>(yt);

            if (Vector128.IsHardwareAccelerated && Vector128<float>.IsSupported) {

                var zero = Vector128<float>.Zero;
                var ATanResultValid = Vector128<float>.AllBitsSet;

                var pi = Vector128.Create(MathF.PI);
                var piOver2 = Vector128.Create(MathF.PI / 2.0f);
                var piOver4 = Vector128.Create(MathF.PI / 4.0f);
                var threePiOver4 = Vector128.Create((3.0f * MathF.PI) / 4.0f);

                var yEqualsZero = Vector128.Equals(y, zero);
                var xEqualsZero = Vector128.Equals(x, zero);

                // perform an equality check with uint types to avoid special floating point equality check cases
                var xIsPositive = x & Vector128.Create(float.NegativeZero);
                var xIsPositiveInt = Unsafe.BitCast<Vector128<float>, Vector128<uint>>(xIsPositive);
                xIsPositiveInt = Vector128.Equals(xIsPositiveInt, Vector128<uint>.Zero);
                xIsPositive = Unsafe.BitCast<Vector128<uint>, Vector128<float>>(xIsPositiveInt);

                var yEqualsInfinity = Vector128.IsPositiveInfinity(Vector128.Abs(y));
                var xEqualsInfinity = Vector128.IsPositiveInfinity(Vector128.Abs(x));

                var ySign = y & Vector128.Create(float.NegativeZero);
                pi |= ySign;
                piOver2 |= ySign;
                piOver4 |= ySign;
                threePiOver4 |= ySign;

                var r1 = Vector128.ConditionalSelect(xIsPositive, ySign, pi);
                var r2 = Vector128.ConditionalSelect(xEqualsZero, piOver2, ATanResultValid);
                var r3 = Vector128.ConditionalSelect(yEqualsZero, r1, r2);
                var r4 = Vector128.ConditionalSelect(xIsPositive, piOver4, threePiOver4);
                var r5 = Vector128.ConditionalSelect(xEqualsInfinity, r4, piOver2);
                var results = Vector128.ConditionalSelect(yEqualsInfinity, r5, r3);

                // perform an equality check with uint types to avoid special floating point equality check cases
                var ATanResultValidInt = Vector128.Equals(Unsafe.BitCast<Vector128<float>, Vector128<uint>>(results), Unsafe.BitCast<Vector128<float>, Vector128<uint>>(ATanResultValid));
                ATanResultValid = Unsafe.BitCast<Vector128<uint>, Vector128<float>>(ATanResultValidInt);

                var v = y / x;
                var r0 = Atan(v);
                r1 = Vector128.ConditionalSelect(xIsPositive, Vector128.Create(float.NegativeZero), pi);
                r2 = r0 + r1;

                var ret = Vector128.ConditionalSelect(ATanResultValid, r2, results);
                var nanMask = Vector128.IsNaN(x) | Vector128.IsNaN(y);
                ret = Vector128.ConditionalSelect(nanMask, Vector128.Create(float.NaN), ret);
                return Unsafe.BitCast<Vector128<float>, Vector128<T>>(ret);
                }
            else {
                var ret = Vector128.Create(
                    float.Atan2(y[0], x[0]),
                    float.Atan2(y[1], x[1]),
                    float.Atan2(y[2], x[2]),
                    float.Atan2(y[3], x[3])
                    );
                return Unsafe.BitCast<Vector128<float>, Vector128<T>>(ret);
                }
            }
        else if (typeof(T) == typeof(double)) {

            var x = Unsafe.BitCast<Vector128<T>, Vector128<double>>(xt);
            var y = Unsafe.BitCast<Vector128<T>, Vector128<double>>(yt);

            if (Vector128.IsHardwareAccelerated && Vector128<double>.IsSupported) {

                var zero = Vector128<double>.Zero;
                var ATanResultValid = Vector128<double>.AllBitsSet;

                var pi = Vector128.Create(Math.PI);
                var piOver2 = Vector128.Create(Math.PI / 2.0);
                var piOver4 = Vector128.Create(Math.PI / 4.0);
                var threePiOver4 = Vector128.Create((3.0 * Math.PI) / 4.0);

                var yEqualsZero = Vector128.Equals(y, zero);
                var xEqualsZero = Vector128.Equals(x, zero);

                // perform an equality check with ulong types to avoid special doubleing point equality check cases
                var xIsPositive = x & Vector128.Create(double.NegativeZero);
                var xIsPositiveInt = Unsafe.BitCast<Vector128<double>, Vector128<ulong>>(xIsPositive);
                xIsPositiveInt = Vector128.Equals(xIsPositiveInt, Vector128<ulong>.Zero);
                xIsPositive = Unsafe.BitCast<Vector128<ulong>, Vector128<double>>(xIsPositiveInt);

                var yEqualsInfinity = Vector128.IsPositiveInfinity(Vector128.Abs(y));
                var xEqualsInfinity = Vector128.IsPositiveInfinity(Vector128.Abs(x));

                var ySign = y & Vector128.Create(double.NegativeZero);
                pi |= ySign;
                piOver2 |= ySign;
                piOver4 |= ySign;
                threePiOver4 |= ySign;

                var r1 = Vector128.ConditionalSelect(xIsPositive, ySign, pi);
                var r2 = Vector128.ConditionalSelect(xEqualsZero, piOver2, ATanResultValid);
                var r3 = Vector128.ConditionalSelect(yEqualsZero, r1, r2);
                var r4 = Vector128.ConditionalSelect(xIsPositive, piOver4, threePiOver4);
                var r5 = Vector128.ConditionalSelect(xEqualsInfinity, r4, piOver2);
                var results = Vector128.ConditionalSelect(yEqualsInfinity, r5, r3);

                // perform an equality check with ulong types to avoid special doubleing point equality check cases
                var ATanResultValidInt = Vector128.Equals(Unsafe.BitCast<Vector128<double>, Vector128<ulong>>(results), Unsafe.BitCast<Vector128<double>, Vector128<ulong>>(ATanResultValid));
                ATanResultValid = Unsafe.BitCast<Vector128<ulong>, Vector128<double>>(ATanResultValidInt);

                var v = y / x;
                var r0 = Atan(v);
                r1 = Vector128.ConditionalSelect(xIsPositive, Vector128.Create(double.NegativeZero), pi);
                r2 = r0 + r1;

                var ret = Vector128.ConditionalSelect(ATanResultValid, r2, results);
                var nanMask = Vector128.IsNaN(x) | Vector128.IsNaN(y);
                ret = Vector128.ConditionalSelect(nanMask, Vector128.Create(double.NaN), ret);
                return Unsafe.BitCast<Vector128<double>, Vector128<T>>(ret);
                }
            else {
                var ret = Vector128.Create(
                    double.Atan2(y[0], x[0]),
                    double.Atan2(y[1], x[1])
                    );
                return Unsafe.BitCast<Vector128<double>, Vector128<T>>(ret);
                }
            }
        else { // half-precision
            throw new NotImplementedException("Half-precision Vector128s weren't supported at time of writing...");
            }
        }

    /// <inheritdoc
    /// cref="Atan2{T}(Vector128{T}, Vector128{T})"/>
    public static Vector256<T> Atan2<T>(this Vector256<T> yt, Vector256<T> xt) where T : unmanaged, IBinaryFloatingPointIeee754<T> {

        if (typeof(T) == typeof(float)) {

            var x = Unsafe.BitCast<Vector256<T>, Vector256<float>>(xt);
            var y = Unsafe.BitCast<Vector256<T>, Vector256<float>>(yt);

            if (Vector256.IsHardwareAccelerated && Vector256<float>.IsSupported) {

                var zero = Vector256<float>.Zero;
                var ATanResultValid = Vector256<float>.AllBitsSet;

                var pi = Vector256.Create(MathF.PI);
                var piOver2 = Vector256.Create(MathF.PI / 2.0f);
                var piOver4 = Vector256.Create(MathF.PI / 4.0f);
                var threePiOver4 = Vector256.Create((3.0f * MathF.PI) / 4.0f);

                var yEqualsZero = Vector256.Equals(y, zero);
                var xEqualsZero = Vector256.Equals(x, zero);

                // perform an equality check with uint types to avoid special floating point equality check cases
                var xIsPositive = x & Vector256.Create(float.NegativeZero);
                var xIsPositiveInt = Unsafe.BitCast<Vector256<float>, Vector256<uint>>(xIsPositive);
                xIsPositiveInt = Vector256.Equals(xIsPositiveInt, Vector256<uint>.Zero);
                xIsPositive = Unsafe.BitCast<Vector256<uint>, Vector256<float>>(xIsPositiveInt);

                var yEqualsInfinity = Vector256.IsPositiveInfinity(Vector256.Abs(y));
                var xEqualsInfinity = Vector256.IsPositiveInfinity(Vector256.Abs(x));

                var ySign = y & Vector256.Create(float.NegativeZero);
                pi |= ySign;
                piOver2 |= ySign;
                piOver4 |= ySign;
                threePiOver4 |= ySign;

                var r1 = Vector256.ConditionalSelect(xIsPositive, ySign, pi);
                var r2 = Vector256.ConditionalSelect(xEqualsZero, piOver2, ATanResultValid);
                var r3 = Vector256.ConditionalSelect(yEqualsZero, r1, r2);
                var r4 = Vector256.ConditionalSelect(xIsPositive, piOver4, threePiOver4);
                var r5 = Vector256.ConditionalSelect(xEqualsInfinity, r4, piOver2);
                var results = Vector256.ConditionalSelect(yEqualsInfinity, r5, r3);

                // perform an equality check with uint types to avoid special floating point equality check cases
                var ATanResultValidInt = Vector256.Equals(Unsafe.BitCast<Vector256<float>, Vector256<uint>>(results), Unsafe.BitCast<Vector256<float>, Vector256<uint>>(ATanResultValid));
                ATanResultValid = Unsafe.BitCast<Vector256<uint>, Vector256<float>>(ATanResultValidInt);

                var v = y / x;
                var r0 = Atan(v);
                r1 = Vector256.ConditionalSelect(xIsPositive, Vector256.Create(float.NegativeZero), pi);
                r2 = r0 + r1;

                var ret = Vector256.ConditionalSelect(ATanResultValid, r2, results);
                var nanMask = Vector256.IsNaN(x) | Vector256.IsNaN(y);
                ret = Vector256.ConditionalSelect(nanMask, Vector256.Create(float.NaN), ret);
                return Unsafe.BitCast<Vector256<float>, Vector256<T>>(ret);
                }
            else {
                var ret = Vector256.Create(
                    float.Atan2(y[0], x[0]),
                    float.Atan2(y[1], x[1]),
                    float.Atan2(y[2], x[2]),
                    float.Atan2(y[3], x[3]),
                    float.Atan2(y[4], x[4]),
                    float.Atan2(y[5], x[5]),
                    float.Atan2(y[6], x[6]),
                    float.Atan2(y[7], x[7])
                    );
                return Unsafe.BitCast<Vector256<float>, Vector256<T>>(ret);
                }
            }
        else if (typeof(T) == typeof(double)) {

            var x = Unsafe.BitCast<Vector256<T>, Vector256<double>>(xt);
            var y = Unsafe.BitCast<Vector256<T>, Vector256<double>>(yt);

            if (Vector256.IsHardwareAccelerated && Vector256<double>.IsSupported) {

                var zero = Vector256<double>.Zero;
                var ATanResultValid = Vector256<double>.AllBitsSet;

                var pi = Vector256.Create(Math.PI);
                var piOver2 = Vector256.Create(Math.PI / 2.0);
                var piOver4 = Vector256.Create(Math.PI / 4.0);
                var threePiOver4 = Vector256.Create((3.0 * Math.PI) / 4.0);

                var yEqualsZero = Vector256.Equals(y, zero);
                var xEqualsZero = Vector256.Equals(x, zero);

                // perform an equality check with ulong types to avoid special doubleing point equality check cases
                var xIsPositive = x & Vector256.Create(double.NegativeZero);
                var xIsPositiveInt = Unsafe.BitCast<Vector256<double>, Vector256<ulong>>(xIsPositive);
                xIsPositiveInt = Vector256.Equals(xIsPositiveInt, Vector256<ulong>.Zero);
                xIsPositive = Unsafe.BitCast<Vector256<ulong>, Vector256<double>>(xIsPositiveInt);

                var yEqualsInfinity = Vector256.IsPositiveInfinity(Vector256.Abs(y));
                var xEqualsInfinity = Vector256.IsPositiveInfinity(Vector256.Abs(x));

                var ySign = y & Vector256.Create(double.NegativeZero);
                pi |= ySign;
                piOver2 |= ySign;
                piOver4 |= ySign;
                threePiOver4 |= ySign;

                var r1 = Vector256.ConditionalSelect(xIsPositive, ySign, pi);
                var r2 = Vector256.ConditionalSelect(xEqualsZero, piOver2, ATanResultValid);
                var r3 = Vector256.ConditionalSelect(yEqualsZero, r1, r2);
                var r4 = Vector256.ConditionalSelect(xIsPositive, piOver4, threePiOver4);
                var r5 = Vector256.ConditionalSelect(xEqualsInfinity, r4, piOver2);
                var results = Vector256.ConditionalSelect(yEqualsInfinity, r5, r3);

                // perform an equality check with ulong types to avoid special doubleing point equality check cases
                var ATanResultValidInt = Vector256.Equals(Unsafe.BitCast<Vector256<double>, Vector256<ulong>>(results), Unsafe.BitCast<Vector256<double>, Vector256<ulong>>(ATanResultValid));
                ATanResultValid = Unsafe.BitCast<Vector256<ulong>, Vector256<double>>(ATanResultValidInt);

                var v = y / x;
                var r0 = Atan(v);
                r1 = Vector256.ConditionalSelect(xIsPositive, Vector256.Create(double.NegativeZero), pi);
                r2 = r0 + r1;

                var ret = Vector256.ConditionalSelect(ATanResultValid, r2, results);
                var nanMask = Vector256.IsNaN(x) | Vector256.IsNaN(y);
                ret = Vector256.ConditionalSelect(nanMask, Vector256.Create(double.NaN), ret);
                return Unsafe.BitCast<Vector256<double>, Vector256<T>>(ret);
                }
            else {
                var ret = Vector256.Create(
                    double.Atan2(y[0], x[0]),
                    double.Atan2(y[1], x[1]),
                    double.Atan2(y[2], x[2]),
                    double.Atan2(y[3], x[3])
                    );
                return Unsafe.BitCast<Vector256<double>, Vector256<T>>(ret);
                }
            }
        else { // half-precision
            throw new NotImplementedException("Half-precision Vector256s weren't supported at time of writing...");
            }
        }

    /// <inheritdoc
    /// cref="Atan2{T}(Vector128{T}, Vector128{T})"/>
    public static Vector512<T> Atan2<T>(this Vector512<T> yt, Vector512<T> xt)
        where T : unmanaged, IBinaryFloatingPointIeee754<T> {

        if (typeof(T) == typeof(float)) {

            var x = Unsafe.BitCast<Vector512<T>, Vector512<float>>(xt);
            var y = Unsafe.BitCast<Vector512<T>, Vector512<float>>(yt);

            if (Vector512.IsHardwareAccelerated && Vector512<float>.IsSupported) {

                var zero = Vector512<float>.Zero;
                var ATanResultValid = Vector512<float>.AllBitsSet;

                var pi = Vector512.Create(MathF.PI);
                var piOver2 = Vector512.Create(MathF.PI / 2.0f);
                var piOver4 = Vector512.Create(MathF.PI / 4.0f);
                var threePiOver4 = Vector512.Create((3.0f * MathF.PI) / 4.0f);

                var yEqualsZero = Vector512.Equals(y, zero);
                var xEqualsZero = Vector512.Equals(x, zero);

                // perform an equality check with uint types to avoid special floating point equality check cases
                var xIsPositive = x & Vector512.Create(float.NegativeZero);
                var xIsPositiveInt = Unsafe.BitCast<Vector512<float>, Vector512<uint>>(xIsPositive);
                xIsPositiveInt = Vector512.Equals(xIsPositiveInt, Vector512<uint>.Zero);
                xIsPositive = Unsafe.BitCast<Vector512<uint>, Vector512<float>>(xIsPositiveInt);

                var yEqualsInfinity = Vector512.IsPositiveInfinity(Vector512.Abs(y));
                var xEqualsInfinity = Vector512.IsPositiveInfinity(Vector512.Abs(x));

                var ySign = y & Vector512.Create(float.NegativeZero);
                pi |= ySign;
                piOver2 |= ySign;
                piOver4 |= ySign;
                threePiOver4 |= ySign;

                var r1 = Vector512.ConditionalSelect(xIsPositive, ySign, pi);
                var r2 = Vector512.ConditionalSelect(xEqualsZero, piOver2, ATanResultValid);
                var r3 = Vector512.ConditionalSelect(yEqualsZero, r1, r2);
                var r4 = Vector512.ConditionalSelect(xIsPositive, piOver4, threePiOver4);
                var r5 = Vector512.ConditionalSelect(xEqualsInfinity, r4, piOver2);
                var results = Vector512.ConditionalSelect(yEqualsInfinity, r5, r3);

                // perform an equality check with uint types to avoid special floating point equality check cases
                var ATanResultValidInt = Vector512.Equals(Unsafe.BitCast<Vector512<float>, Vector512<uint>>(results), Unsafe.BitCast<Vector512<float>, Vector512<uint>>(ATanResultValid));
                ATanResultValid = Unsafe.BitCast<Vector512<uint>, Vector512<float>>(ATanResultValidInt);

                var v = y / x;
                var r0 = Atan(v);
                r1 = Vector512.ConditionalSelect(xIsPositive, Vector512.Create(float.NegativeZero), pi);
                r2 = r0 + r1;

                var ret = Vector512.ConditionalSelect(ATanResultValid, r2, results);
                var nanMask = Vector512.IsNaN(x) | Vector512.IsNaN(y);
                ret = Vector512.ConditionalSelect(nanMask, Vector512.Create(float.NaN), ret);
                return Unsafe.BitCast<Vector512<float>, Vector512<T>>(ret);
                }
            else {
                var ret = Vector512.Create(
                    float.Atan2(y[0], x[0]),
                    float.Atan2(y[1], x[1]),
                    float.Atan2(y[2], x[2]),
                    float.Atan2(y[3], x[3]),
                    float.Atan2(y[4], x[4]),
                    float.Atan2(y[5], x[5]),
                    float.Atan2(y[6], x[6]),
                    float.Atan2(y[7], x[7]),
                    float.Atan2(y[8], x[8]),
                    float.Atan2(y[9], x[9]),
                    float.Atan2(y[10], x[10]),
                    float.Atan2(y[11], x[11]),
                    float.Atan2(y[12], x[12]),
                    float.Atan2(y[13], x[13]),
                    float.Atan2(y[14], x[14]),
                    float.Atan2(y[15], x[15]));
                return Unsafe.BitCast<Vector512<float>, Vector512<T>>(ret);
                }
            }
        else if (typeof(T) == typeof(double)) {

            var x = Unsafe.BitCast<Vector512<T>, Vector512<double>>(xt);
            var y = Unsafe.BitCast<Vector512<T>, Vector512<double>>(yt);

            if (Vector512.IsHardwareAccelerated && Vector512<double>.IsSupported) {

                var zero = Vector512<double>.Zero;
                var ATanResultValid = Vector512<double>.AllBitsSet;

                var pi = Vector512.Create(Math.PI);
                var piOver2 = Vector512.Create(Math.PI / 2.0);
                var piOver4 = Vector512.Create(Math.PI / 4.0);
                var threePiOver4 = Vector512.Create((3.0 * Math.PI) / 4.0);

                var yEqualsZero = Vector512.Equals(y, zero);
                var xEqualsZero = Vector512.Equals(x, zero);

                // perform an equality check with ulong types to avoid special doubleing point equality check cases
                var xIsPositive = x & Vector512.Create(double.NegativeZero);
                var xIsPositiveInt = Unsafe.BitCast<Vector512<double>, Vector512<ulong>>(xIsPositive);
                xIsPositiveInt = Vector512.Equals(xIsPositiveInt, Vector512<ulong>.Zero);
                xIsPositive = Unsafe.BitCast<Vector512<ulong>, Vector512<double>>(xIsPositiveInt);

                var yEqualsInfinity = Vector512.IsPositiveInfinity(Vector512.Abs(y));
                var xEqualsInfinity = Vector512.IsPositiveInfinity(Vector512.Abs(x));

                var ySign = y & Vector512.Create(double.NegativeZero);
                pi |= ySign;
                piOver2 |= ySign;
                piOver4 |= ySign;
                threePiOver4 |= ySign;

                var r1 = Vector512.ConditionalSelect(xIsPositive, ySign, pi);
                var r2 = Vector512.ConditionalSelect(xEqualsZero, piOver2, ATanResultValid);
                var r3 = Vector512.ConditionalSelect(yEqualsZero, r1, r2);
                var r4 = Vector512.ConditionalSelect(xIsPositive, piOver4, threePiOver4);
                var r5 = Vector512.ConditionalSelect(xEqualsInfinity, r4, piOver2);
                var results = Vector512.ConditionalSelect(yEqualsInfinity, r5, r3);

                // perform an equality check with ulong types to avoid special doubleing point equality check cases
                var ATanResultValidInt = Vector512.Equals(Unsafe.BitCast<Vector512<double>, Vector512<ulong>>(results), Unsafe.BitCast<Vector512<double>, Vector512<ulong>>(ATanResultValid));
                ATanResultValid = Unsafe.BitCast<Vector512<ulong>, Vector512<double>>(ATanResultValidInt);

                var v = y / x;
                var r0 = Atan(v);
                r1 = Vector512.ConditionalSelect(xIsPositive, Vector512.Create(double.NegativeZero), pi);
                r2 = r0 + r1;

                var ret = Vector512.ConditionalSelect(ATanResultValid, r2, results);
                var nanMask = Vector512.IsNaN(x) | Vector512.IsNaN(y);
                ret = Vector512.ConditionalSelect(nanMask, Vector512.Create(double.NaN), ret);
                return Unsafe.BitCast<Vector512<double>, Vector512<T>>(ret);
                }
            else {
                var ret = Vector512.Create(
                    double.Atan2(y[0], x[0]),
                    double.Atan2(y[1], x[1]),
                    double.Atan2(y[2], x[2]),
                    double.Atan2(y[3], x[3]),
                    double.Atan2(y[4], x[4]),
                    double.Atan2(y[5], x[5]),
                    double.Atan2(y[6], x[6]),
                    double.Atan2(y[7], x[7])
                    );
                return Unsafe.BitCast<Vector512<double>, Vector512<T>>(ret);
                }
            }
        else { // half-precision
            throw new NotImplementedException("Half-precision Vector512s weren't supported at time of writing...");
            }
        }

    /* Reference: double-precision minimax polynomial calculated via the Remez algorithm

    static double[] ConstantsOf09Degrees = [-1.1102230246251565e-16, +9.9986632916719875e-01, +8.6033352174037790e-16, -3.3030479466194063e-01, -2.3111133162202357e-15, +1.8015932093491399e-01, +1.6275793077493077e-15, -8.5156364077950880e-02, +1.5662048408300112e-16, +2.0845109892654905e-02];

    static double[] ConstantsOf11Degrees = [+1.1102230246251565e-16, +9.9997722028339298e-01, +1.6217716942800151e-15, -3.3262286158818705e-01, -1.6686346175947256e-14, +1.9354060409607787e-01, +4.6686702190906183e-14, -1.1642707168235181e-01, -5.1624682237540346e-14, +5.2647992115139139e-02, +1.9885237139424151e-14, -1.1719382026568179e-02];

    static double[] ConstantsOf13Degrees = [-1.7145937747375939e-06, +9.9998153039485804e-01, +3.4570389281212736e-05, -3.3289720118933169e-01, -2.3214245576622718e-04, +1.9630128708302985e-01, +7.1330978949800926e-04, -1.2704994388717619e-01, -1.1027159831449461e-03, +7.1661301598789681e-02, +8.3507267874592683e-04, -2.7695798615185757e-02, -2.4642278408256607e-04, +5.0969880124643640e-03];

    static double[] ConstantsOf15Degrees = [+1.8953694169709934e-10, +9.9999933657461959e-01, -2.7057165477354485e-08, -3.3329863013805844e-01, +6.0491539967350563e-07, +1.9946562970710244e-01, -4.8884220989635067e-06, -1.3908437109553337e-01, +1.8330810123630210e-05, +9.6411348404880248e-02, -3.4455075219081968e-05, -5.5888452116550268e-02, +3.1385345639187063e-05, +2.1838873721631534e-02, -1.0988065712906420e-05, -4.0455716606433339e-03];

    static double[] ConstantsOf17Degrees = [+1.1102230246251565e-16, +9.9999988640270210e-01, -9.2958077540262540e-15, -3.3332597139940712e-01, +1.8165426638884955e-13, +1.9985908529729537e-01, -1.4482771711599333e-12, -1.4161241376156555e-01, +5.8388621263920376e-12, +1.0498989959767739e-01, -1.2912195387471714e-11, -7.2349459114000106e-02, +1.5846896645693965e-11, +3.9782230759860926e-02, -1.0112202363566099e-11, -1.4401960672145638e-02, +2.6147694323992068e-12, +2.4568720420006479e-03];

    static double[] ConstantsOf19Degrees = [-3.3306690738754696e-16, +9.9999998056149420e-01, +1.0840191046902107e-14, -3.3333180390659056e-01, -3.3719313276199063e-13, +1.9996437113774998e-01, +3.8005694891852484e-12, -1.4247225401363928e-01, -2.0947315475610936e-11, +1.0878022769195965e-01, +6.3973034569154487e-11, -8.2137964570884778e-02, -1.1380959612944850e-10, +5.5028661248043127e-02, +1.1738216481032352e-10, -2.8491307000436346e-02, -6.5062580669013620e-11, +9.5676133030615784e-03, +1.4990840838684179e-11, -1.5093619470641752e-03];

    static double[] ConstantsOf21Degrees = [-5.0148107888503546e-11, +9.9999999632659231e-01, -4.2514509017008687e-09, -3.3333295512484484e-01, +1.8358503960550073e-07, +1.9998954857150891e-01, -2.4626330634786639e-06, -1.4272529295946088e-01, +1.6240802117166779e-05, +1.1017702031355847e-01, -6.1162166532867420e-05, -8.6766255854322141e-02, +1.4019673044221091e-04, +6.4627119619905693e-02, -1.9908843018184803e-04, -4.1044142598182662e-02, +1.7102252870765456e-04, +1.9621301747489207e-02, -8.1401823645813311e-05, -6.0114977288570045e-03, +1.6475788615539967e-05, +8.6332108406139089e-04];

    static double[] ConstantsOf23Degrees = [-5.5155879863377777e-13, +9.9999999943687001e-01, +1.7376820121068949e-10, -3.3333327088610187e-01, -8.9643640094855517e-09, +1.9999794386539146e-01, +1.7554394715477364e-07, -1.4282555046579679e-01, -1.7187044890649787e-06, +1.1083640886138510e-01, +9.6009351171894974e-06, -8.9407301722661855e-02, -3.2795572386357150e-05, +7.1412638118242250e-02, +7.0741290815844312e-05, -5.2468344682567267e-02, -9.6586041712885709e-05, +3.2162499949021352e-02, +8.0828926759670585e-05, -1.4658276575583520e-02, -3.7790767485836496e-05, +4.2626954417039221e-03, +7.5532023707846844e-06, -5.8127794245454480e-04];

    static double[] ConstantsOf25Degrees = [+1.1102230246251565e-16, +9.9999999990252575e-01, +8.3755505660990316e-15, -3.3333332082411754e-01, -3.9704548853967720e-13, +1.9999952247693195e-01, +8.1477354172862140e-12, -1.4284861024063783e-01, -8.8137348108714628e-11, +1.1102444805962657e-01, +5.5754053060264213e-10, -9.0352572995139643e-02, -2.2075169121476420e-09, +7.4509327327095878e-02, +5.6898914555829680e-09, -5.9271697797517106e-02, -9.6954818278125848e-09, +4.2265132690788451e-02, +1.0832902704417805e-08, -2.4662752389370431e-02, -7.6301024794577577e-09, +1.0591177129961644e-02, +3.0720025450730208e-09, -2.8938374361440252e-03, -5.3885839828643564e-10, +3.7134749695428486e-04];

    static double[] ConstantsOf27Degrees = [+1.1102230246251565e-16, +9.9999999998328404e-01, -7.0018934171057514e-15, -3.3333333086700506e-01, +1.7095294275984600e-13, +1.9999989165458310e-01, -1.6967332111442944e-13, -1.4285491043242010e-01, -2.8434926809740749e-11, +1.1108488465450911e-01, +3.5810554355000287e-10, -9.0713498883979435e-02, -2.1789539334222784e-09, +7.5933114542454952e-02, +7.9938048062741047e-09, -6.3109550003480008e-02, -1.9009255423968409e-08, +4.9445401817719919e-02, +3.0018648379645652e-08, -3.3981266187505396e-02, -3.1290347390722487e-08, +1.8820639329690764e-02, +2.0700395634465195e-08, -7.6115819997874369e-03, -7.8773379745960643e-09, +1.9543019071630030e-03, +1.3133819280226206e-09, -2.3593211833982126e-04];

    static double[] ConstantsOf29Degrees = [+9.5286001311478685e-12, +9.9999999978615361e-01, -1.3621179082127845e-09, -3.3333331950024964e-01, +4.6349707990970967e-08, +1.9999963981758639e-01, -7.2104576131812561e-07, -1.4285203227011820e-01, +6.3803875540814264e-06, +1.1106585328773476e-01, -3.5958864052724230e-05, -9.0637597292106603e-02, +1.3772402031699915e-04, +7.5758345483842138e-02, -3.7226852219839844e-04, -6.2954551790541907e-02, +7.2441823720990732e-04, +4.9779048240379570e-02, -1.0208399755817568e-03, -3.5371974127307221e-02, +1.0336621892516880e-03, +2.1180346113398727e-02, -7.3371679046967770e-04, -9.9690132753439912e-03, +3.4677656650234766e-04, +3.3907302202691946e-03, -9.8054838472643157e-05, -7.3207957181395877e-04, +1.2553639271847694e-05, +7.4768276014147532e-05];

    static double[] ConstantsOf31Degrees = [-2.1094237467877974e-15, +9.9999999999998701e-01, +1.1673054327113565e-12, -3.3333333332549903e-01, -9.8929473053192716e-11, +1.9999999928295736e-01, +3.2823715613898657e-09, -1.4285711617699681e-01, -5.6288697404211767e-08, +1.1111057418880602e-01, +5.7005559430562486e-07, -9.0902385071444616e-02, -3.6540411471136159e-06, +7.6866867948869252e-02, +1.5256874469101136e-05, -6.6334757113691245e-02, -4.0948603146651853e-05, +5.7398642913357098e-02, +6.3627396036871950e-05, -4.8088555476874378e-02, -2.6203187114034170e-05, +3.6691087682240685e-02, -1.0945853331414131e-04, -2.3351227529484202e-02, +2.5836173997646333e-04, +1.1023790805728633e-02, -2.7094291211285078e-04, -3.1961501823852870e-03, +1.4656413715064865e-04, +3.1706431168277316e-04, -3.3145578868271522e-05, +5.3780704532745942e-05];
    */
    }
