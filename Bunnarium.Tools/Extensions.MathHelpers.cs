using System.Diagnostics;

namespace Bunnarium.Tools {

    public static partial class Extensions {

        #region Decimal (Float128) Helpers

        const decimal Ln10 = 2.3025850929940456840179914547M;
        const decimal InvSqrt10 = 0.3162277660168379331998893544M;
        const decimal Sqrt10 = 3.1622776601683793319988935444M;

        /// <summary> Estimates the logarithm (base-<see cref="double.E">E</see>) of the <see langword="decimal"/> value.
        /// </summary>
        public static decimal Log(this decimal value) {
            if (value <= 0)
                throw new ArgumentOutOfRangeException(nameof(value), "Cannot calculate the power a non-positive number!");
            var result = 0M;
            while (value >= Sqrt10) {
                result += Ln10;
                value /= 10;
                }
            while (value <= InvSqrt10) {
                result -= Ln10;
                value *= 10;
                }
            var term = (value - 1) / (value + 1);
            var termSquared = term * term;
            var termPower = term;
            var numerator = 2 * term;
            var denominator = 1M;

            for (int i = 1; i < 100; i += 2) {
                if (numerator == 0) break; // termPower underflowed to 0
                result += numerator / denominator;
                termPower *= termSquared;
                numerator = 2 * termPower;
                denominator += 2;
                }

            return result;
            }

        /// <summary> Estimates the exponent of the <see langword="decimal"/> value (<see cref="double.E">E</see> raised to the <paramref name="value"/>).
        /// </summary>
        public static decimal Exp(this decimal value) {
            if (value == 0) return 1;
            if (value < 0) return 1 / Exp(-value);

            var result = 1M;
            var term = 1M;
            var i = 1;

            while (true) {
                term *= value / i;
                if (term == 0) break;
                result += term;
                i++;
                }

            return result;
            }

        /// <summary> Estimates the value of <paramref name="x"/> raised to the <paramref name="y"/>th power.
        /// </summary>
        public static decimal Pow(this decimal x, decimal y) {
            if (x <= 0)
                throw new ArgumentOutOfRangeException(nameof(x), "Cannot calculate the power a non-positive number!");
            return Exp(y * Log(x));
            }

        /// <summary> Estimates the square root of the <paramref name="value"/> to within the given <paramref name="epsilon"/>.
        /// </summary>
        /// <param name="epsilon"> An error margin to estimate the square root within. Smaller errors require more iteratins and are thus slower.
        /// </param>
        public static decimal Sqrt(this decimal value, decimal epsilon = 0.0M) {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value), "Cannot calculate square root of a negative number!");

            var current = (decimal)Math.Sqrt((double)value); // initial guess
            var iterations = 0; // count iterations and terminate after 10 of them to avoid a potential infinite loop
            decimal previous;

            do {
                previous = current;
                if (previous == 0.0M) return 0;
                current = (previous + value / previous) / 2;
                }
            while (Math.Abs(previous - current) > epsilon && ++iterations < 10);

            return current;
            }

        #endregion Decimal (Float128) Helpers

        #region Signedness

        /// <summary> Throws if <typeparamref name="T"/> doesn't inherit from <see cref="ISignedNumber{TSelf}"/>.
        /// </summary>
        [Conditional("DEBUG")]
        public static void ThrowIfUnsigned<T>() where T : unmanaged, System.Numerics.INumberBase<T> {
            if (Signedness<T>.IsSigned == false)
                throw new NotSupportedException($"Necessarily signed operation is not supported for unsigned type {typeof(T).Name}.");
            }

        static class Signedness<T> where T : unmanaged, INumberBase<T> {
            public static readonly bool IsSigned = typeof(T).GetInterfaces().Any(static i => i.IsConstructedGenericType && (i.GetGenericTypeDefinition() == typeof(ISignedNumber<>)));

            public static readonly bool IsUnsigned = IsSigned == false;
            }

        #endregion Signedness
        }
    }
