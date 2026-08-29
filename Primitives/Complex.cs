// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Base = System.Numerics.Complex;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Bunnarium.Maths.Primitives;

/// <summary> A Complex&lt;T&gt; number z is a number of the form <c>z = x + yi</c>, where x and y are real numbers, and i is the imaginary unit, with the property <c>i^2 = -1</c>.
/// </summary>
/// <remarks> This type is a generically-typed revision of the standard library's <see cref="Complex">System.Numerics.Complex</see>
/// </remarks>
[BunnyAttributes.Citation($"Based and reliant on {nameof(System.Numerics.Complex)} from the .NET Foundation, licensed under the MIT license.")]
public readonly struct Complex<T> : IEquatable<Complex<T>>
    where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T>, INumberBase<T> {
    /* The MIT License (MIT)

    Copyright (c) .NET Foundation and Contributors

    All rights reserved.

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.

    */

    /// <inheritdoc cref="Base.Complex(double, double)"/>
    public Complex(T real, T imaginary) {
        Real = real;
        Imaginary = imaginary;
        }

    /// <inheritdoc cref="Base.Imaginary"/>
    public T Imaginary { get; }

    /// <inheritdoc cref="Base.Real"/>
    public T Real { get; }

    /// <inheritdoc cref="Base.Magnitude"/>
    public T Magnitude { get { return Abs(this); } }

    /// <inheritdoc cref="Base.Phase"/>
    public T Phase { get { return T.Atan2(Imaginary, Real); } }

    #region Constants

    /// <inheritdoc cref="Base.ImaginaryOne"/>
    public static readonly Complex<T> ImaginaryOne = new(T.Zero, T.One);

    /// <inheritdoc cref="Base.Infinity"/>
    public static readonly Complex<T> Infinity = new(T.PositiveInfinity, T.PositiveInfinity);

    /// <inheritdoc cref="Base.NaN"/>
    public static readonly Complex<T> NaN = new(T.NaN, T.NaN);

    /// <inheritdoc cref="Base.One"/>
    public static readonly Complex<T> One = new(T.One, T.Zero);

    /// <inheritdoc cref="Base.Zero"/>
    public static readonly Complex<T> Zero = new(T.Zero, T.Zero);

    static readonly T InverseOfLog10 = T.CreateChecked(0.43429448190325); // 1 / Log(10)

    // This is the largest x for which 2 x^2 will not overflow. It is used for branching inside Asin and Acos.
    static readonly T s_asinOverflowThreshold = T.Sqrt(T.MaxValue) * GenericNumbers<T>.OneHalf;

    // This value is used inside Asin and Acos.
    static readonly T s_log2 = T.Log(GenericNumbers<T>.Two);

    // This is the largest x for which (Hypot(x,x) + x) will not overflow. It is used for branching inside Sqrt.
    static readonly T s_sqrtRescaleThreshold = T.MaxValue / (GenericNumbers<T>.RootTwo + T.One);

    #endregion Constants

    #region Factories

    /// <inheritdoc cref="Base.FromPolarCoordinates(double, double)"/>
    public static Complex<T> FromPolarCoordinates(T magnitude, T phase) {
        (T sin, T cos) = T.SinCos(phase);
        return new Complex<T>(magnitude * cos, magnitude * sin);
        }

    #endregion Factories

    #region Mathematical Operators (Functions)

    /// <inheritdoc cref="Base.Add(Base, Base)"/>
    public static Complex<T> Add(Complex<T> left, Complex<T> right) {
        return left + right;
        }

    /// <inheritdoc cref="Base.Add(Base, double)"/>
    public static Complex<T> Add(Complex<T> left, T right) {
        return left + right;
        }

    /// <inheritdoc cref="Base.Add(double, Base)"/>
    public static Complex<T> Add(T left, Complex<T> right) {
        return left + right;
        }

    /// <inheritdoc cref="Base.Divide(Base, Base)"/>
    public static Complex<T> Divide(Complex<T> dividend, Complex<T> divisor) {
        return dividend / divisor;
        }

    /// <inheritdoc cref="Base.Divide(Base, double)"/>
    public static Complex<T> Divide(Complex<T> dividend, T divisor) {
        return dividend / divisor;
        }

    /// <inheritdoc cref="Base.Divide(double, Base)"/>
    public static Complex<T> Divide(T dividend, Complex<T> divisor) {
        return dividend / divisor;
        }

    /// <inheritdoc cref="Base.Multiply(Base, Base)"/>
    public static Complex<T> Multiply(Complex<T> left, Complex<T> right) {
        return left * right;
        }

    /// <inheritdoc cref="Base.Multiply(Base, double)"/>
    public static Complex<T> Multiply(Complex<T> left, T right) {
        return left * right;
        }

    /// <inheritdoc cref="Base.Multiply(double, Base)"/>
    public static Complex<T> Multiply(T left, Complex<T> right) {
        return left * right;
        }

    /// <inheritdoc cref="Base.Negate(Base)"/>
    public static Complex<T> Negate(Complex<T> value) {
        return -value;
        }

    /// <inheritdoc cref="Base.Subtract(Base, Base)"/>
    public static Complex<T> Subtract(Complex<T> left, Complex<T> right) {
        return left - right;
        }

    /// <inheritdoc cref="Base.Subtract(Base, double)"/>
    public static Complex<T> Subtract(Complex<T> left, T right) {
        return left - right;
        }

    /// <inheritdoc cref="Base.Subtract(double, Base)"/>
    public static Complex<T> Subtract(T left, Complex<T> right) {
        return left - right;
        }

    static Complex<T> Scale(Complex<T> value, T factor) {
        var realResult = factor * value.Real;
        var imaginaryResult = factor * value.Imaginary;
        return new Complex<T>(realResult, imaginaryResult);
        }

    #endregion Mathematical Operators (Functions)

    #region Mathematical Operators

    /// <inheritdoc cref="Base.Negate(Base)"/>
    public static Complex<T> operator -(Complex<T> value)  /* Unary negation of a Complex<T> number */
        {
        return new Complex<T>(-value.Real, -value.Imaginary);
        }

    /// <inheritdoc cref="Base.Subtract(Base, Base)"/>
    public static Complex<T> operator -(Complex<T> left, Complex<T> right) {
        return new Complex<T>(left.Real - right.Real, left.Imaginary - right.Imaginary);
        }

    /// <inheritdoc cref="Base.Subtract(Base, double)"/>
    public static Complex<T> operator -(Complex<T> left, T right) {
        return new Complex<T>(left.Real - right, left.Imaginary);
        }

    /// <inheritdoc cref="Base.Subtract(double, Base)"/>
    public static Complex<T> operator -(T left, Complex<T> right) {
        return new Complex<T>(left - right.Real, -right.Imaginary);
        }

    /// <inheritdoc cref="Base.Multiply(Base, Base)"/>
    public static Complex<T> operator *(Complex<T> left, Complex<T> right) {
        // Multiplication:  (a + bi)(c + di) = (ac -bd) + (bc + ad)i

        var result_realpart = left.Real * right.Real - left.Imaginary * right.Imaginary;
        var result_imaginarypart = left.Imaginary * right.Real + left.Real * right.Imaginary;
        return new Complex<T>(result_realpart, result_imaginarypart);
        }

    /// <inheritdoc cref="Base.Multiply(Base, double)"/>
    public static Complex<T> operator *(Complex<T> left, T right) {
        if (!T.IsFinite(left.Real)) {
            if (!T.IsFinite(left.Imaginary)) {
                return new Complex<T>(T.NaN, T.NaN);
                }

            return new Complex<T>(left.Real * right, T.NaN);
            }

        if (!T.IsFinite(left.Imaginary)) {
            return new Complex<T>(T.NaN, left.Imaginary * right);
            }

        return new Complex<T>(left.Real * right, left.Imaginary * right);
        }

    /// <inheritdoc cref="Base.Multiply(double, Base)"/>
    public static Complex<T> operator *(T left, Complex<T> right) {
        if (!T.IsFinite(right.Real)) {
            if (!T.IsFinite(right.Imaginary)) {
                return new Complex<T>(T.NaN, T.NaN);
                }

            return new Complex<T>(left * right.Real, T.NaN);
            }

        if (!T.IsFinite(right.Imaginary)) {
            return new Complex<T>(T.NaN, left * right.Imaginary);
            }

        return new Complex<T>(left * right.Real, left * right.Imaginary);
        }

    /// <inheritdoc cref="Base.Divide(Base, Base)"/>
    public static Complex<T> operator /(Complex<T> left, Complex<T> right) {
        // Division : Smith's formula.
        var a = left.Real;
        var b = left.Imaginary;
        var c = right.Real;
        var d = right.Imaginary;

        // Computing c * c + d * d will overflow even in cases where the actual result of the division does not overflow.
        if (T.Abs(d) < T.Abs(c)) {
            var doc = d / c;
            return new Complex<T>((a + b * doc) / (c + d * doc), (b - a * doc) / (c + d * doc));
            }
        else {
            var cod = c / d;
            return new Complex<T>((b + a * cod) / (d + c * cod), (-a + b * cod) / (d + c * cod));
            }
        }

    /// <inheritdoc cref="Base.Divide(Base, double)"/>
    public static Complex<T> operator /(Complex<T> left, T right) {
        // IEEE prohibit optimizations which are value changing
        // so we make sure that behaviour for the simplified version exactly match
        // full version.
        if (right == T.Zero) {
            return new Complex<T>(T.NaN, T.NaN);
            }

        if (!T.IsFinite(left.Real)) {
            if (!T.IsFinite(left.Imaginary)) {
                return new Complex<T>(T.NaN, T.NaN);
                }

            return new Complex<T>(left.Real / right, T.NaN);
            }

        if (!T.IsFinite(left.Imaginary)) {
            return new Complex<T>(T.NaN, left.Imaginary / right);
            }

        // Here the actual optimized version of code.
        return new Complex<T>(left.Real / right, left.Imaginary / right);
        }

    /// <inheritdoc cref="Base.Divide(double, Base)"/>
    public static Complex<T> operator /(T left, Complex<T> right) {
        // Division : Smith's formula.
        var a = left;
        var c = right.Real;
        var d = right.Imaginary;

        // Computing c * c + d * d will overflow even in cases where the actual result of the division does not overflow.
        if (T.Abs(d) < T.Abs(c)) {
            var doc = d / c;
            return new Complex<T>(a / (c + d * doc), -a * doc / (c + d * doc));
            }
        else {
            var cod = c / d;
            return new Complex<T>(a * cod / (d + c * cod), -a / (d + c * cod));
            }
        }

    /// <inheritdoc cref="Base.Add(Base, Base)"/>
    public static Complex<T> operator +(Complex<T> left, Complex<T> right) {
        return new Complex<T>(left.Real + right.Real, left.Imaginary + right.Imaginary);
        }

    /// <inheritdoc cref="Base.Add(Base, double)"/>
    public static Complex<T> operator +(Complex<T> left, T right) {
        return new Complex<T>(left.Real + right, left.Imaginary);
        }

    /// <inheritdoc cref="Base.Add(double, Base)"/>
    public static Complex<T> operator +(T left, Complex<T> right) {
        return new Complex<T>(left + right.Real, right.Imaginary);
        }

    #endregion Mathematical Operators

    #region Abs, Conjugate and Reciprocal

    /// <inheritdoc cref="Base.Abs(Base)"/>
    public static T Abs(Complex<T> value) {
        return T.Hypot(value.Real, value.Imaginary);
        }

    /// <inheritdoc cref="Base.Conjugate(Base)"/>
    public static Complex<T> Conjugate(Complex<T> value) {
        // Conjugate of a Complex<T> number: the conjugate of x+i*y is x-i*y
        return new Complex<T>(value.Real, -value.Imaginary);
        }

    /// <inheritdoc cref="Base.Reciprocal(Base)"/>
    public static Complex<T> Reciprocal(Complex<T> value) {
        // Reciprocal of a Complex<T> number : the reciprocal of x+i*y is 1/(x+i*y)
        if (value.Real == T.Zero && value.Imaginary == T.Zero) {
            return Zero;
            }
        return One / value;
        }

    #endregion Abs, Conjugate and Reciprocal

    #region Equatability

    public static bool operator !=(Complex<T> left, Complex<T> right) {
        return left.Real != right.Real || left.Imaginary != right.Imaginary;
        }

    public static bool operator ==(Complex<T> left, Complex<T> right) {
        return left.Real == right.Real && left.Imaginary == right.Imaginary;
        }

    /// <inheritdoc cref="Base.Equals(object?)"/>
    public override bool Equals([NotNullWhen(true)] object? obj) {
        return obj is Complex<T> other && Equals(other);
        }

    /// <inheritdoc cref="Base.Equals(Base)"/>
    public bool Equals(Complex<T> value) {
        return Real.Equals(value.Real) && Imaginary.Equals(value.Imaginary);
        }

    /// <inheritdoc cref="Base.GetHashCode"/>
    public override int GetHashCode() {
        return HashCode.Combine(Real, Imaginary);
        }

    #endregion Equatability

    #region Strings

    /// <inheritdoc cref="Base.ToString()"/>
    public override string ToString() {
        return ToString(null, null);
        }

    /// <inheritdoc cref="Base.ToString(string?)"/>
    public string ToString([StringSyntax(StringSyntaxAttribute.NumericFormat)] string? format) {
        return ToString(format, null);
        }

    /// <inheritdoc cref="Base.ToString(IFormatProvider?)"/>
    public string ToString(IFormatProvider? provider) {
        return ToString(null, provider);
        }

    /// <inheritdoc cref="Base.ToString(string?, IFormatProvider?)"/>
    public string ToString([StringSyntax(StringSyntaxAttribute.NumericFormat)] string? format, IFormatProvider? provider) {
        // $"<{m_real.ToString(format, provider)}; {m_imaginary.ToString(format, provider)}>";
        var handler = new DefaultInterpolatedStringHandler(4, 2, provider, stackalloc char[512]);
        handler.AppendLiteral("<");
        handler.AppendFormatted(Real, format);
        handler.AppendLiteral("; ");
        handler.AppendFormatted(Imaginary, format);
        handler.AppendLiteral(">");
        return handler.ToStringAndClear();
        }

    #endregion Strings

    #region Trigonometry

    /// <inheritdoc cref="Base.Acos(Base)"/>
    public static Complex<T> Acos(Complex<T> value) {
        Asin_Internal(T.Abs(value.Real), T.Abs(value.Imaginary), out T b, out T bPrime, out T v);

        T u;
        if (bPrime < T.Zero) {
            u = T.Acos(b);
            }
        else {
            u = T.Atan(T.One / bPrime);
            }

        if (value.Real < T.Zero) u = T.Pi - u;
        if (value.Imaginary > T.Zero) v = -v;

        return new Complex<T>(u, v);
        }

    /// <inheritdoc cref="Base.Asin(Base)"/>
    public static Complex<T> Asin(Complex<T> value) {
        Asin_Internal(T.Abs(value.Real), T.Abs(value.Imaginary), out T b, out T bPrime, out T v);

        T u;
        if (bPrime < T.Zero) {
            u = T.Asin(b);
            }
        else {
            u = T.Atan(bPrime);
            }

        if (value.Real < T.Zero) u = -u;
        if (value.Imaginary < T.Zero) v = -v;

        return new Complex<T>(u, v);
        }

    /// <inheritdoc cref="Base.Atan(Base)"/>
    public static Complex<T> Atan(Complex<T> value) {
        var two = new Complex
            <T>(GenericNumbers<T>.Two, T.Zero);
        return ImaginaryOne / two * (Log(One - ImaginaryOne * value) - Log(One + ImaginaryOne * value));
        }

    /// <inheritdoc cref="Base.Cos(Base)"/>
    public static Complex<T> Cos(Complex<T> value) {
        (T sin, T cos) = T.SinCos(value.Real);
        return new Complex<T>(cos * T.Cosh(value.Imaginary), -sin * T.Sinh(value.Imaginary));
        }

    /// <inheritdoc cref="Base.Cosh(Base)"/>
    public static Complex<T> Cosh(Complex<T> value) {
        // Use cosh(z) = cos(iz) to compute via cos(z).
        return Cos(new Complex<T>(-value.Imaginary, value.Real));
        }

    /// <inheritdoc cref="Base.Sin(Base)"/>
    public static Complex<T> Sin(Complex<T> value) {
        (T sin, T cos) = T.SinCos(value.Real);
        return new Complex<T>(sin * T.Cosh(value.Imaginary), cos * T.Sinh(value.Imaginary));
        // There is a known limitation with this algorithm: inputs that cause sinh and cosh to overflow, but for
        // which sin or cos are small enough that sin * cosh or cos * sinh are still representable, nonetheless
        // produce overflow. For example, Sin((T.Zero, 711)) should produce (~3.0E306, PositiveInfinity), but
        // instead produces (PositiveInfinity, PositiveInfinity).
        }

    /// <inheritdoc cref="Base.Sinh(Base)"/>
    public static Complex<T> Sinh(Complex<T> value) {
        // Use sinh(z) = -i sin(iz) to compute via sin(z).
        Complex<T> sin = Sin(new Complex<T>(-value.Imaginary, value.Real));
        return new Complex<T>(sin.Imaginary, -sin.Real);
        }

    /// <inheritdoc cref="Base.Tan(Base)"/>
    public static Complex<T> Tan(Complex<T> value) {
        // tan z = sin z / cos z, but to avoid unnecessary repeated trig computations, use
        //   tan z = (sin(2x) + i sinh(2y)) / (cos(2x) + cosh(2y))
        // (see Abramowitz & Stegun 4.3.57 or derive by hand), and compute trig functions here.

        // This approach does not work for |y| > ~355, because sinh(2y) and cosh(2y) overflow,
        // even though their ratio does not. In that case, divide through by cosh to get:
        //   tan z = (sin(2x) / cosh(2y) + i \tanh(2y)) / (1 + cos(2x) / cosh(2y))
        // which correctly computes the (tiny) real part and the (normal-sized) imaginary part.

        var x2 = GenericNumbers<T>.Two * value.Real;
        var y2 = GenericNumbers<T>.Two * value.Imaginary;
        (T sin, T cos) = T.SinCos(x2);
        var cosh = T.Cosh(y2);
        if (T.Abs(value.Imaginary) <= GenericNumbers<T>.Four) {
            var D = cos + cosh;
            return new Complex<T>(sin / D, T.Sinh(y2) / D);
            }
        else {
            var D = T.One + cos / cosh;
            return new Complex<T>(sin / cosh / D, T.Tanh(y2) / D);
            }
        }

    /// <inheritdoc cref="Base.Tanh(Base)"/>
    public static Complex<T> Tanh(Complex<T> value) {
        // Use tanh(z) = -i tan(iz) to compute via tan(z).
        var tan = Tan(new Complex<T>(-value.Imaginary, value.Real));
        return new Complex<T>(tan.Imaginary, -tan.Real);
        }

    private static void Asin_Internal(T x, T y, out T b, out T bPrime, out T v) {
        // This method for the inverse Complex<T> sine (and cosine) is described in Hull, Fairgrieve,
        // and Tang, "Implementing the Complex<T> Arcsine and Arccosine Functions Using Exception Handling",
        // ACM Transactions on Mathematical Software (1997)
        // (https://www.researchgate.net/profile/Ping_Tang3/publication/220493330_Implementing_the_Complex_Arcsine_and_Arccosine_Functions_Using_Exception_Handling/links/55b244b208ae9289a085245d.pdf)

        // First, the basics: start with sin(w) = (e^{iw} - e^{-iw}) / (2i) = z. Here z is the input
        // and w is the output. To solve for w, define t = e^{i w} and multiply through by t to
        // get the quadratic equation t^2 - 2 i z t - 1 = 0. The solution is t = i z + sqrt(1 - z^2), so
        //   w = arcsin(z) = - i log( i z + sqrt(1 - z^2) )
        // Decompose z = x + i y, multiply out i z + sqrt(1 - z^2), use log(s) = |s| + i arg(s), and do a
        // bunch of algebra to get the components of w = arcsin(z) = u + i v
        //   u = arcsin(beta)  v = sign(y) log(alpha + sqrt(alpha^2 - 1))
        // where
        //   alpha = (rho + sigma) / 2      beta = (rho - sigma) / 2
        //   rho = sqrt((x + 1)^2 + y^2)    sigma = sqrt((x - 1)^2 + y^2)
        // These formulas appear in DLMF section 4.23. (http://dlmf.nist.gov/4.23), along with the analogous
        //   arccos(w) = arccos(beta) - i sign(y) log(alpha + sqrt(alpha^2 - 1))
        // So alpha and beta together give us arcsin(w) and arccos(w).

        // As written, alpha is not susceptible to cancelation errors, but beta is. To avoid cancelation, note
        //   beta = (rho^2 - sigma^2) / (rho + sigma) / 2 = (2 x) / (rho + sigma) = x / alpha
        // which is not subject to cancelation. Note alpha >= 1 and |beta| <= 1.

        // For alpha ~ 1, the argument of the log is near unity, so we compute (alpha - 1) instead,
        // write the argument as 1 + (alpha - 1) + sqrt((alpha - 1)(alpha + 1)), and use the log1p function
        // to compute the log without loss of accuracy.
        // For beta ~ 1, arccos does not accurately resolve small angles, so we compute the tangent of the angle
        // instead.
        // Hull, Fairgrieve, and Tang derive formulas for (alpha - 1) and beta' = tan(u) that do not suffer
        // from cancelation in these cases.

        // For simplicity, we assume all positive inputs and return all positive outputs. The caller should
        // assign signs appropriate to the desired cut conventions. We return v directly since its magnitude
        // is the same for both arcsin and arccos. Instead of u, we usually return beta and sometimes beta'.
        // If beta' is not computed, it is set to -1; if it is computed, it should be used instead of beta
        // to determine u. Compute u = arcsin(beta) or u = arctan(beta') for arcsin, u = arccos(beta)
        // or arctan(1/beta') for arccos.

        Debug.Assert(x >= T.Zero || T.IsNaN(x));
        Debug.Assert(y >= T.Zero || T.IsNaN(y));

        // For x or y large enough to overflow alpha^2, we can simplify our formulas and avoid overflow.
        if (x > s_asinOverflowThreshold || y > s_asinOverflowThreshold) {
            b = T.NegativeOne;
            bPrime = x / y;

            T small, big;
            if (x < y) {
                small = x;
                big = y;
                }
            else {
                small = y;
                big = x;
                }
            var ratio = small / big;
            v = s_log2 + T.Log(big) + GenericNumbers<T>.OneHalf * Log1P(ratio * ratio);
            }
        else {
            var r = T.Hypot(x + T.One, y);
            var s = T.Hypot(x - T.One, y);

            var a = (r + s) * GenericNumbers<T>.OneHalf;
            b = x / a;

            if (b > GenericNumbers<T>.ThreeFourths) {
                if (x <= T.One) {
                    var amx = (y * y / (r + (x + T.One)) + (s + (T.One - x))) * GenericNumbers<T>.OneHalf;
                    bPrime = x / T.Sqrt((a + x) * amx);
                    }
                else {
                    // In this case, amx ~ y^2. Since we take the square root of amx, we should
                    // pull y out from under the square root so we don't lose its contribution
                    // when y^2 underflows.
                    var t = (T.One / (r + (x + T.One)) + T.One / (s + (x - T.One))) * GenericNumbers<T>.OneHalf;
                    bPrime = x / y / T.Sqrt((a + x) * t);
                    }
                }
            else {
                bPrime = T.NegativeOne;
                }

            if (a < GenericNumbers<T>.ThreeHalves) {
                if (x < T.One) {
                    // This is another case where our expression is proportional to y^2 and
                    // we take its square root, so again we pull out a factor of y from
                    // under the square root.
                    var t = (T.One / (r + (x + T.One)) + T.One / (s + (T.One - x))) * GenericNumbers<T>.OneHalf;
                    var am1 = y * y * t;
                    v = Log1P(am1 + y * T.Sqrt(t * (a + T.One)));
                    }
                else {
                    var am1 = (y * y / (r + (x + T.One)) + (s + (x - T.One))) * GenericNumbers<T>.OneHalf;
                    v = Log1P(am1 + T.Sqrt(am1 * (a + T.One)));
                    }
                }
            else {
                // Because of the test above, we can be sure that a * a will not overflow.
                v = T.Log(a + T.Sqrt((a - T.One) * (a + T.One)));
                }
            }
        }

    #endregion Trigonometry

    #region Finiteness / IsNaN

    /// <inheritdoc cref="Base.IsFinite(Base)"/>
    public static bool IsFinite(Complex<T> value) {
        return T.IsFinite(value.Real) && T.IsFinite(value.Imaginary);
        }

    /// <inheritdoc cref="Base.IsInfinity(Base)"/>
    public static bool IsInfinity(Complex<T> value) {
        return T.IsInfinity(value.Real) || T.IsInfinity(value.Imaginary);
        }

    /// <inheritdoc cref="Base.IsNaN(Base)"/>
    public static bool IsNaN(Complex<T> value) {
        return !IsInfinity(value) && !IsFinite(value);
        }

    #endregion Finiteness / IsNaN

    #region Logrithmics, Exponents, and Roots

    /// <inheritdoc cref="Base.Exp(Base)"/>
    public static Complex<T> Exp(Complex<T> value) {
        var expReal = T.Exp(value.Real);
        return FromPolarCoordinates(expReal, value.Imaginary);
        }

    /// <inheritdoc cref="Base.Log(Base)"/>
    public static Complex<T> Log(Complex<T> value) {
        return new Complex<T>(T.Log(Abs(value)), T.Atan2(value.Imaginary, value.Real));
        }

    /// <inheritdoc cref="Base.Log(Base, double)"/>
    public static Complex<T> Log(Complex<T> value, T baseValue) {
        return Log(value) / Log(baseValue);
        }

    /// <inheritdoc cref="Base.Log10(Base)"/>
    public static Complex<T> Log10(Complex<T> value) {
        Complex<T> tempLog = Log(value);
        return Scale(tempLog, InverseOfLog10);
        }

    /// <inheritdoc cref="Base.Pow(Base, Base)"/>
    public static Complex<T> Pow(Complex<T> value, Complex<T> power) {
        if (power == Zero) {
            return One;
            }

        if (value == Zero) {
            return Zero;
            }

        var valueReal = value.Real;
        var valueImaginary = value.Imaginary;
        var powerReal = power.Real;
        var powerImaginary = power.Imaginary;

        var rho = Abs(value);
        var theta = T.Atan2(valueImaginary, valueReal);
        var newRho = powerReal * theta + powerImaginary * T.Log(rho);

        var t = T.Pow(rho, powerReal) * T.Exp(-powerImaginary * theta);

        return FromPolarCoordinates(t, newRho);
        }

    /// <inheritdoc cref="Base.Pow(Base, double)"/>
    public static Complex<T> Pow(Complex<T> value, T power) {
        return Pow(value, new Complex<T>(power, T.Zero));
        }

    /// <inheritdoc cref="Base.Sqrt(Base)"/>
    public static Complex<T> Sqrt(Complex<T> value) {
        // Handle NaN input cases according to IEEE 754
        if (T.IsNaN(value.Real)) {
            if (T.IsInfinity(value.Imaginary)) {
                return new Complex<T>(T.PositiveInfinity, value.Imaginary);
                }
            return new Complex<T>(T.NaN, T.NaN);
            }
        if (T.IsNaN(value.Imaginary)) {
            if (T.IsPositiveInfinity(value.Real)) {
                return new Complex<T>(T.NaN, T.PositiveInfinity);
                }
            if (T.IsNegativeInfinity(value.Real)) {
                return new Complex<T>(T.PositiveInfinity, T.NaN);
                }
            return new Complex<T>(T.NaN, T.NaN);
            }

        if (value.Imaginary == T.Zero) {
            // Handle the trivial case quickly.
            if (value.Real < T.Zero) {
                return new Complex<T>(T.Zero, T.Sqrt(-value.Real));
                }

            return new Complex<T>(T.Sqrt(value.Real), T.Zero);
            }

        // One way to compute Sqrt(z) is just to call Pow(z, GenericNumbers<T>.OneHalf), which coverts to polar coordinates
        // (sqrt + atan), halves the phase, and reconverts to cartesian coordinates (cos + sin).
        // Not only is this more expensive than necessary, it also fails to preserve certain expected
        // symmetries, such as that the square root of a pure negative is a pure imaginary, and that the
        // square root of a pure imaginary has exactly equal real and imaginary parts. This all goes
        // back to the fact that T.Pi is not stored with infinite precision, so taking half of T.Pi
        // does not land us on an argument with cosine exactly equal to zero.

        // To find a fast and symmetry-respecting formula for Complex<T> square root,
        // note x + i y = \sqrt{a + i b} implies x^2 + 2 i x y - y^2 = a + i b,
        // so x^2 - y^2 = a and 2 x y = b. Cross-substitute and use the quadratic formula to obtain
        //   x = \sqrt{\frac{\sqrt{a^2 + b^2} + a}{2}}  y = \pm \sqrt{\frac{\sqrt{a^2 + b^2} - a}{2}}
        // There is just one complication: depending on the sign on a, either x or y suffers from
        // cancelation when |b| << |a|. We can get around this by noting that our formulas imply
        // x^2 y^2 = b^2 / 4, so |x| |y| = |b| / 2. So after computing the one that doesn't suffer
        // from cancelation, we can compute the other with just a division. This is basically just
        // the right way to evaluate the quadratic formula without cancelation.

        // All this reduces our total cost to two sqrts and a few flops, and it respects the desired
        // symmetries. Much better than atan + cos + sin!

        // The signs are a matter of choice of branch cut, which is traditionally taken so x > 0 and sign(y) = sign(b).

        // If the components are too large, Hypot will overflow, even though the subsequent sqrt would
        // make the result representable. To avoid this, we re-scale (by exact powers of 2 for accuracy)
        // when we encounter very large components to avoid intermediate infinities.
        bool rescale = false;
        var realCopy = value.Real;
        var imaginaryCopy = value.Imaginary;
        if (T.Abs(realCopy) >= s_sqrtRescaleThreshold || T.Abs(imaginaryCopy) >= s_sqrtRescaleThreshold) {
            if (T.IsInfinity(value.Imaginary)) {
                // We need to handle infinite imaginary parts specially because otherwise
                // our formulas below produce inf/inf = NaN.
                return new Complex<T>(T.PositiveInfinity, imaginaryCopy);
                }

            realCopy *= GenericNumbers<T>.OneFourth;
            imaginaryCopy *= GenericNumbers<T>.OneFourth;
            rescale = true;
            }

        // This is the core of the algorithm. Everything else is special case handling.
        T x, y;
        if (realCopy >= T.Zero) {
            x = T.Sqrt((T.Hypot(realCopy, imaginaryCopy) + realCopy) * GenericNumbers<T>.OneHalf);
            y = imaginaryCopy / (GenericNumbers<T>.Two * x);
            }
        else {
            y = T.Sqrt((T.Hypot(realCopy, imaginaryCopy) - realCopy) * GenericNumbers<T>.OneHalf);
            if (imaginaryCopy < T.Zero) y = -y;
            x = imaginaryCopy / (GenericNumbers<T>.Two * y);
            }

        if (rescale) {
            x *= GenericNumbers<T>.Two;
            y *= GenericNumbers<T>.Two;
            }

        return new Complex<T>(x, y);
        }

    private static T Log1P(T x) {
        // Compute log(1 + x) without loss of accuracy when x is small.

        // Our only use case so far is for positive values, so this isn't coded to handle negative values.
        Debug.Assert(x >= T.Zero || T.IsNaN(x));

        var xp1 = T.One + x;
        if (xp1 == T.One) {
            return x;
            }
        else if (x < GenericNumbers<T>.ThreeFourths) {
            // This is accurate to within 5 ulp with any floating-point system that uses a guard digit,
            // as proven in Theorem 4 of "What Every Computer Scientist Should Know About Floating-Point
            // Arithmetic" (https://docs.oracle.com/cd/E19957-01/806-3568/ncg_goldberg.html)
            return x * T.Log(xp1) / (xp1 - T.One);
            }
        else {
            return T.Log(xp1);
            }
        }

    #endregion Logrithmics, Exponents, and Roots

    #region Replacement operators

    public static implicit operator Complex<T>(T value) {
        return new(value, T.Zero);
        }

    #endregion Replacement operators

    #region Bunnarium Integration

    /// <summary> Creates a new instance of <see cref="Complex{T}"/> from the specified real and imaginary parts.
    /// </summary>
    public Complex(Vector2<T> vector) : this(vector.X, vector.Y) { }

    /// <summary> Creates a new instance of <see cref="Complex{T}"/> from the specified <see cref="Angle{T}"/>.
    /// </summary>
    public Complex(Angle<T> angle) {
        (T sin, T cos) = T.SinCos(angle.Radians);
        Real = cos;
        Imaginary = sin;
        }

    /// <inheritdoc cref="ToVector(Complex{T})"/>
    public readonly Vector2<T> Vector {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => new(Real, Imaginary);
        }

    /// <summary> Returns the <see cref="Vector2{T}"/> representation of this <paramref name="complex"/> number such that its <see cref="Real">real</see> component is <see cref="Vector2{T}.X">X</see> and its <see cref="Imaginary">imaginary</see> component is <see cref="Vector2{T}.Y">Y</see>.
    /// </summary>

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2<T> ToVector(Complex<T> complex) {
        return new(complex.Real, complex.Imaginary);
        }

    /// <summary> Returns the <see cref="Complex{T}"/> representation of the given <paramref name="vector"/> such that its <see cref="Real">real</see> component is <see cref="Vector2{T}.X">X</see> and its <see cref="Imaginary">imaginary</see> component is <see cref="Vector2{T}.Y">Y</see>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Complex<T> FromVector(Vector2<T> vector) {
        return new(vector.X, vector.Y);
        }

    /// <summary> Returns the <see cref="Complex{T}"/> representation of the given <paramref name="angle"/>.
    /// </summary>
    public static Complex<T> FromAngle(Angle<T> angle) {
        return FromPolarCoordinates(T.One, angle.Radians);
        }

    #endregion Bunnarium Integration
    }
