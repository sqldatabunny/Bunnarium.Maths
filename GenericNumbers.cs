using System.Runtime.CompilerServices;
using System.Collections.Immutable;

namespace Bunnarium.Maths;

public static partial class GenericNumbers {

    /// <summary> Provides fast access to constant integer values.
    /// </summary>
    /// <remarks> Mirrors <see cref="GenericNumbers{T}"/> fast-access integers, but for cases where <typeparamref name="T"/> is specifically an integer (<see cref="GenericNumbers{T}"/> is constrained to <typeparamref name="T"/> : <see cref="IFloatingPoint{T}"/>).
    /// <para/> This type is provided to compensate for the slow performance of functions such as <see cref="INumberBase{TSelf}.CreateTruncating{TOther}(TOther)"/> and <see cref="INumberBase{TSelf}.CreateChecked{TOther}(TOther)"/> in DEBUG mode.
    /// </remarks>
    public static class Integers<T>
        where T : unmanaged, IBinaryInteger<T> {
#if DEBUG

        #region Fast-access Integers

        ///<summary>2</summary>
        public static readonly T Two = T.One + T.One;

        ///<summary>3</summary>
        public static readonly T Three = Two + T.One;

        ///<summary>4</summary>
        public static readonly T Four = Two + Two;

        ///<summary>5</summary>
        public static readonly T Five = Three + Two;

        ///<summary>6</summary>
        public static readonly T Six = Three + Three;

        ///<summary>8</summary>
        public static readonly T Eight = Four + Four;

        ///<summary>10</summary>
        public static readonly T Ten = Five + Five;

        ///<summary>12</summary>
        public static readonly T Twelve = Six + Six;

        ///<summary>16</summary>
        public static readonly T Sixteen = Eight + Eight;

        ///<summary>20</summary>
        public static readonly T Twenty = Ten + Ten;

        ///<summary>60</summary>
        public static readonly T Sixty = Twenty * Three;

        ///<summary>80</summary>
        public static readonly T Eighty = Twenty * Four;

        ///<summary>100</summary>
        public static readonly T OneHundred = Twenty * Five;

        ///<summary>120</summary>
        public static readonly T OneHundredAndTwenty = Twenty * Six;

        ///<summary>180</summary>
        public static readonly T OneHundredAndEighty = Sixty * Three;

        ///<summary>200</summary>
        public static readonly T TwoHundred = Twenty * Ten;

        ///<summary>240 <c>(0xF0)</c></summary>
        public static readonly T TwoHundredAndForty = OneHundredAndTwenty * Two;

        ///<summary>255</summary>
        public static readonly T TwoHundredAndFiftyFive = (Sixty * Four) + (Five * Three);

        ///<summary>256</summary>
        /// <remarks>This value is <see cref="INumberBase{T}.CreateTruncating{TOther}(TOther)">truncated</see> when numeric type <typeparamref name="T"/> is too limited to represent it.</remarks>
        public static readonly T TwoHundredAndFiftySix = TwoHundredAndFiftyFive + T.One;

        ///<summary>1000</summary>
        /// <remarks><inheritdoc cref="TwoHundredAndFiftySix"/></remarks>
        public static readonly T OneThousand = T.CreateTruncating(1000);

        ///<summary>1,000,000</summary>
        /// <remarks><inheritdoc cref="TwoHundredAndFiftySix"/></remarks>
        public static readonly T OneMillion = OneThousand * OneThousand;

        ///<summary>65280 <c>(0xFF00)</c></summary>
        /// <remarks><inheritdoc cref="TwoHundredAndFiftySix"/></remarks>
        public static readonly T xFF00 = T.CreateTruncating(0xFF00);

        ///<summary>4,294,901,760 <c>(0xFFFF0000)</c></summary>
        /// <remarks><inheritdoc cref="TwoHundredAndFiftySix"/></remarks>
        public static readonly T xFFFF0000 = T.CreateTruncating(0xFFFF0000);

        #endregion Fast-access Integers

#else

        #region Fast-access Integers

        ///<summary>2</summary>
        public static T Two { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => T.CreateTruncating(2); }

        ///<summary>3</summary>
        public static T Three { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => T.CreateTruncating(3); }

        ///<summary>4</summary>
        public static T Four { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => T.CreateTruncating(4); }

        ///<summary>5</summary>
        public static T Five { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => T.CreateTruncating(5); }

        ///<summary>6</summary>
        public static T Six { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => T.CreateTruncating(6); }

        ///<summary>8</summary>
        public static T Eight { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => T.CreateTruncating(8); }

        ///<summary>10</summary>
        public static T Ten { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => T.CreateTruncating(10); }

        ///<summary>12</summary>
        public static T Twelve { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => T.CreateTruncating(12); }

        ///<summary>16</summary>
        public static T Sixteen { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => T.CreateTruncating(16); }

        ///<summary>20</summary>
        public static T Twenty { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => T.CreateTruncating(20); }

        ///<summary>60</summary>
        public static T Sixty { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => T.CreateTruncating(60); }

        ///<summary>80</summary>
        public static T Eighty { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => T.CreateTruncating(80); }

        ///<summary>100</summary>
        public static T OneHundred { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => T.CreateTruncating(100); }

        ///<summary>120</summary>
        public static T OneHundredAndTwenty { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => T.CreateTruncating(120); }

        ///<summary>180</summary>
        public static T OneHundredAndEighty { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => T.CreateTruncating(180); }

        ///<summary>200</summary>
        public static T TwoHundred { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => T.CreateTruncating(200); }

        ///<summary>240</summary>
        public static T TwoHundredAndForty { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => T.CreateTruncating(240); }

        ///<summary>255</summary>
        public static T TwoHundredAndFiftyFive { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => T.CreateTruncating(255); }

        ///<summary>256</summary>
        /// <remarks>This value is <see cref="INumberBase{T}.CreateTruncating{TOther}(TOther)">truncated</see> when numeric type <typeparamref name="T"/> is too limited to represent it.</remarks>
        public static T TwoHundredAndFiftySix { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => T.CreateTruncating(256); }

        ///<summary>1000</summary>
        /// <remarks><inheritdoc cref="TwoHundredAndFiftySix"/></remarks>
        public static T OneThousand { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => T.CreateTruncating(1000); }

        ///<summary>1,000,000</summary>
        /// <remarks><inheritdoc cref="TwoHundredAndFiftySix"/></remarks>
        public static T OneMillion { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => T.CreateTruncating(1_000_000); }

        ///<summary>65280 <c>(0xFF00)</c></summary>
        /// <remarks><inheritdoc cref="TwoHundredAndFiftySix"/></remarks>
        public static T xFF00 { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => T.CreateTruncating(0xFF00); }

        ///<summary>4,294,901,760 <c>(0xFFFF0000)</c></summary>
        /// <remarks><inheritdoc cref="TwoHundredAndFiftySix"/></remarks>
        public static T xFFFF0000 { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => T.CreateTruncating(0xFFFF0000); }

        #endregion Fast-access Integers

#endif
        }

    #region Fetch

    /// <summary> Retrieves the reciprocal of the input <paramref name="denominator"/>, which should be between 0 and 255, as a <typeparamref name="T"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T GetReciprocal<T>(byte denominator)
        where T : unmanaged, IFloatingPoint<T> {
        return GenericNumbers<T>.ArrayOfTFractions[denominator];
        }

    #endregion Fetch

    #region Integers

    /// <summary> Converts a binary integer to an <see langword="int"/> using C#'s existing conversion rules.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ToInt32Unsafe<TIn>(this TIn value)
        where TIn : unmanaged, IBinaryInteger<TIn> {
        if (typeof(TIn) == typeof(int)) {
            return Unsafe.As<TIn, int>(ref value);
            }
        else if (typeof(TIn) == typeof(uint)) {
            return (int)Unsafe.As<TIn, uint>(ref value);
            }
        else if (typeof(TIn) == typeof(short)) {
            return Unsafe.As<TIn, short>(ref value);
            }
        else if (typeof(TIn) == typeof(ushort)) {
            return Unsafe.As<TIn, ushort>(ref value);
            }
        else if (typeof(TIn) == typeof(byte)) {
            return Unsafe.As<TIn, byte>(ref value);
            }
        else if (typeof(TIn) == typeof(sbyte)) {
            return Unsafe.As<TIn, sbyte>(ref value);
            }
        else if (typeof(TIn) == typeof(long)) {
            return (int)Unsafe.As<TIn, long>(ref value);
            }
        else if (typeof(TIn) == typeof(ulong)) {
            return (int)Unsafe.As<TIn, ulong>(ref value);
            }
        else {
            return GenericNumbers.ThrowIfUnsupported<TIn, int>();
            }
        }

    /// <summary> Convert a <typeparamref name="TIn"/> to a <typeparamref name="TOut"/>.
    /// </summary>
    [BunnyAttributes.Optimize("For debug mode")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TOut IntegerToInteger<TIn, TOut>(TIn value)
         where TIn : unmanaged, IBinaryInteger<TIn>
         where TOut : unmanaged, IBinaryInteger<TOut> {
        if (typeof(TIn) == typeof(TOut))
            return Unsafe.As<TIn, TOut>(ref value);
        return TOut.CreateTruncating(value);
        }

    /// <inheritdoc
    /// cref="IntegerToInteger{TIn, TOut}(TIn)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TTo FromBinaryInteger<TFrom, TTo>(TFrom value)
        where TFrom : unmanaged, IBinaryInteger<TFrom>
        where TTo : unmanaged, IFloatingPoint<TTo> {
        if (typeof(TFrom) == typeof(TTo))
            return Unsafe.As<TFrom, TTo>(ref value);

        if (typeof(TFrom) == typeof(int)) {
            var source = Unsafe.As<TFrom, int>(ref value);
            if (typeof(TTo) == typeof(double)) { var result = (double)source; return Unsafe.As<double, TTo>(ref result); }
            if (typeof(TTo) == typeof(float)) { var result = (float)source; return Unsafe.As<float, TTo>(ref result); }
            if (typeof(TTo) == typeof(Half)) { var result = (Half)source; return Unsafe.As<Half, TTo>(ref result); }
            }
        else if (typeof(TFrom) == typeof(long)) {
            var source = Unsafe.As<TFrom, long>(ref value);
            if (typeof(TTo) == typeof(double)) { var result = (double)source; return Unsafe.As<double, TTo>(ref result); }
            if (typeof(TTo) == typeof(float)) { var result = (float)source; return Unsafe.As<float, TTo>(ref result); }
            if (typeof(TTo) == typeof(Half)) { var result = (Half)source; return Unsafe.As<Half, TTo>(ref result); }
            }
        else if (typeof(TFrom) == typeof(uint)) {
            var source = Unsafe.As<TFrom, uint>(ref value);
            if (typeof(TTo) == typeof(double)) { var result = (double)source; return Unsafe.As<double, TTo>(ref result); }
            if (typeof(TTo) == typeof(float)) { var result = (float)source; return Unsafe.As<float, TTo>(ref result); }
            if (typeof(TTo) == typeof(Half)) { var result = (Half)source; return Unsafe.As<Half, TTo>(ref result); }
            }
        else if (typeof(TFrom) == typeof(ulong)) {
            var source = Unsafe.As<TFrom, ulong>(ref value);
            if (typeof(TTo) == typeof(double)) { var result = (double)source; return Unsafe.As<double, TTo>(ref result); }
            if (typeof(TTo) == typeof(float)) { var result = (float)source; return Unsafe.As<float, TTo>(ref result); }
            if (typeof(TTo) == typeof(Half)) { var result = (Half)source; return Unsafe.As<Half, TTo>(ref result); }
            }
        else if (typeof(TFrom) == typeof(short)) {
            var source = Unsafe.As<TFrom, short>(ref value);
            if (typeof(TTo) == typeof(double)) { var result = (double)source; return Unsafe.As<double, TTo>(ref result); }
            if (typeof(TTo) == typeof(float)) { var result = (float)source; return Unsafe.As<float, TTo>(ref result); }
            if (typeof(TTo) == typeof(Half)) { var result = (Half)source; return Unsafe.As<Half, TTo>(ref result); }
            }
        else if (typeof(TFrom) == typeof(ushort)) {
            var source = Unsafe.As<TFrom, ushort>(ref value);
            if (typeof(TTo) == typeof(double)) { var result = (double)source; return Unsafe.As<double, TTo>(ref result); }
            if (typeof(TTo) == typeof(float)) { var result = (float)source; return Unsafe.As<float, TTo>(ref result); }
            if (typeof(TTo) == typeof(Half)) { var result = (Half)source; return Unsafe.As<Half, TTo>(ref result); }
            }
        else if (typeof(TFrom) == typeof(byte)) {
            var source = Unsafe.As<TFrom, byte>(ref value);
            if (typeof(TTo) == typeof(double)) { var result = (double)source; return Unsafe.As<double, TTo>(ref result); }
            if (typeof(TTo) == typeof(float)) { var result = (float)source; return Unsafe.As<float, TTo>(ref result); }
            if (typeof(TTo) == typeof(Half)) { var result = (Half)source; return Unsafe.As<Half, TTo>(ref result); }
            }
        else if (typeof(TFrom) == typeof(sbyte)) {
            var source = Unsafe.As<TFrom, sbyte>(ref value);
            if (typeof(TTo) == typeof(double)) { var result = (double)source; return Unsafe.As<double, TTo>(ref result); }
            if (typeof(TTo) == typeof(float)) { var result = (float)source; return Unsafe.As<float, TTo>(ref result); }
            if (typeof(TTo) == typeof(Half)) { var result = (Half)source; return Unsafe.As<Half, TTo>(ref result); }
            }

        return ThrowIfUnsupported<TFrom, TTo>();
        }

    /// <inheritdoc
    /// cref="IntegerToInteger{TIn, TOut}(TIn)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TTo ToBinaryInteger<TFrom, TTo>(TFrom value)
        where TFrom : unmanaged, IFloatingPoint<TFrom>
        where TTo : unmanaged, IBinaryInteger<TTo> {
        if (typeof(TFrom) == typeof(TTo))
            return Unsafe.As<TFrom, TTo>(ref value);

        if (typeof(TFrom) == typeof(double)) {
            var source = Unsafe.As<TFrom, double>(ref value);
            if (typeof(TTo) == typeof(int)) { var result = (int)source; return Unsafe.As<int, TTo>(ref result); }
            if (typeof(TTo) == typeof(long)) { var result = (long)source; return Unsafe.As<long, TTo>(ref result); }
            if (typeof(TTo) == typeof(uint)) { var result = (uint)source; return Unsafe.As<uint, TTo>(ref result); }
            if (typeof(TTo) == typeof(ulong)) { var result = (ulong)source; return Unsafe.As<ulong, TTo>(ref result); }
            if (typeof(TTo) == typeof(short)) { var result = (short)source; return Unsafe.As<short, TTo>(ref result); }
            if (typeof(TTo) == typeof(ushort)) { var result = (ushort)source; return Unsafe.As<ushort, TTo>(ref result); }
            if (typeof(TTo) == typeof(byte)) { var result = (byte)source; return Unsafe.As<byte, TTo>(ref result); }
            if (typeof(TTo) == typeof(sbyte)) { var result = (sbyte)source; return Unsafe.As<sbyte, TTo>(ref result); }
            }
        else if (typeof(TFrom) == typeof(float)) {
            var source = Unsafe.As<TFrom, float>(ref value);
            if (typeof(TTo) == typeof(int)) { var result = (int)source; return Unsafe.As<int, TTo>(ref result); }
            if (typeof(TTo) == typeof(long)) { var result = (long)source; return Unsafe.As<long, TTo>(ref result); }
            if (typeof(TTo) == typeof(uint)) { var result = (uint)source; return Unsafe.As<uint, TTo>(ref result); }
            if (typeof(TTo) == typeof(ulong)) { var result = (ulong)source; return Unsafe.As<ulong, TTo>(ref result); }
            if (typeof(TTo) == typeof(short)) { var result = (short)source; return Unsafe.As<short, TTo>(ref result); }
            if (typeof(TTo) == typeof(ushort)) { var result = (ushort)source; return Unsafe.As<ushort, TTo>(ref result); }
            if (typeof(TTo) == typeof(byte)) { var result = (byte)source; return Unsafe.As<byte, TTo>(ref result); }
            if (typeof(TTo) == typeof(sbyte)) { var result = (sbyte)source; return Unsafe.As<sbyte, TTo>(ref result); }
            }
        else if (typeof(TFrom) == typeof(Half)) {
            var source = Unsafe.As<TFrom, Half>(ref value);
            if (typeof(TTo) == typeof(int)) { var result = (int)source; return Unsafe.As<int, TTo>(ref result); }
            if (typeof(TTo) == typeof(long)) { var result = (long)source; return Unsafe.As<long, TTo>(ref result); }
            if (typeof(TTo) == typeof(uint)) { var result = (uint)source; return Unsafe.As<uint, TTo>(ref result); }
            if (typeof(TTo) == typeof(ulong)) { var result = (ulong)source; return Unsafe.As<ulong, TTo>(ref result); }
            if (typeof(TTo) == typeof(short)) { var result = (short)source; return Unsafe.As<short, TTo>(ref result); }
            if (typeof(TTo) == typeof(ushort)) { var result = (ushort)source; return Unsafe.As<ushort, TTo>(ref result); }
            if (typeof(TTo) == typeof(byte)) { var result = (byte)source; return Unsafe.As<byte, TTo>(ref result); }
            if (typeof(TTo) == typeof(sbyte)) { var result = (sbyte)source; return Unsafe.As<sbyte, TTo>(ref result); }
            }

        return ThrowIfUnsupported<TFrom, TTo>();
        }

    /// <summary> Convert a <typeparamref name="TFrom"/> to a <typeparamref name="TTo"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TTo FromFloatingPoint<TFrom, TTo>(TFrom value)
        where TFrom : unmanaged, IFloatingPoint<TFrom>
        where TTo : unmanaged, IFloatingPoint<TTo> {
        if (typeof(TFrom) == typeof(TTo))
            return Unsafe.As<TFrom, TTo>(ref value);

        if (typeof(TFrom) == typeof(float)) {
            var source = Unsafe.As<TFrom, float>(ref value);
            if (typeof(TTo) == typeof(double)) { var result = (double)source; return Unsafe.As<double, TTo>(ref result); }
            if (typeof(TTo) == typeof(Half)) { var result = (Half)source; return Unsafe.As<Half, TTo>(ref result); }
            }
        else if (typeof(TFrom) == typeof(double)) {
            var source = Unsafe.As<TFrom, double>(ref value);
            if (typeof(TTo) == typeof(float)) { var result = (float)source; return Unsafe.As<float, TTo>(ref result); }
            if (typeof(TTo) == typeof(Half)) { var result = (Half)source; return Unsafe.As<Half, TTo>(ref result); }
            }
        else if (typeof(TFrom) == typeof(Half)) {
            var source = Unsafe.As<TFrom, Half>(ref value);
            if (typeof(TTo) == typeof(double)) { var result = (double)source; return Unsafe.As<double, TTo>(ref result); }
            if (typeof(TTo) == typeof(float)) { var result = (float)source; return Unsafe.As<float, TTo>(ref result); }
            }

        return ThrowIfUnsupported<TFrom, TTo>();
        }

    #endregion Integers

    #region Helpers

    [MethodImpl(MethodImplOptions.NoInlining)]
    internal static TTo ThrowIfUnsupported<TFrom, TTo>() {
        throw new NotSupportedException($"No conversion is defined from {typeof(TFrom)} to {typeof(TTo)}.");
        }

    #endregion Helpers
    }

public static class GenericNumbers<T> where T : unmanaged, IFloatingPoint<T> {

    #region Integers

    /// <summary> Convert a <typeparamref name="TFrom"/> to a <typeparamref name="T"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T FromBinaryInteger<TFrom>(TFrom value)
    where TFrom : unmanaged, IBinaryInteger<TFrom> {
        return GenericNumbers.FromBinaryInteger<TFrom, T>(value);
        }

    /// <summary> Convert a <typeparamref name="T"/> to a <typeparamref name="TTo"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TTo ToBinaryInteger<TTo>(T value) where TTo
        : unmanaged, IBinaryInteger<TTo> {
        return GenericNumbers.ToBinaryInteger<T, TTo>(value);
        }

    #endregion Integers

    #region From

    /// <inheritdoc
    /// cref="GenericNumbers.FromFloatingPoint{TFrom, T}(TFrom)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T FromFloatingPoint<TFrom>(TFrom value)
        where TFrom : unmanaged, IFloatingPoint<TFrom> {
        return GenericNumbers.FromFloatingPoint<TFrom, T>(value);
        }

    /// <summary> A statically-cached delegate to convert a <see cref="double"/> to a <typeparamref name="T"/>
    /// </summary>
    public static Func<double, T> FromDouble { get; }
        = typeof(T) == typeof(double) ? static (double v) => {
            return Unsafe.As<double, T>(ref v);
        }
    : typeof(T) == typeof(float) ? static (double v) => {
            var val = (float)v;
            return Unsafe.As<float, T>(ref val);
        }
    : typeof(T) == typeof(Half) ? static (double v) => {
            var val = (Half)v;
            return Unsafe.As<Half, T>(ref val);
        }
    : typeof(T) == typeof(decimal) ? static (double v) => {
            var val = (decimal)v;
            return Unsafe.As<decimal, T>(ref val);
        }
    : static (double v) => { return GenericNumbers.ThrowIfUnsupported<double, T>(); };

    /// <summary> A statically-cached delegate to convert a <see cref="decimal"/> to a <typeparamref name="T"/>
    /// </summary>
    public static Func<decimal, T> FromDecimal { get; }
        = typeof(T) == typeof(double) ? static (decimal v) => {
              var val = (double)v;
              return Unsafe.As<double, T>(ref val);
          }
    : typeof(T) == typeof(float) ? static (decimal v) => {
              var val = (float)v;
              return Unsafe.As<float, T>(ref val);
          }
    : typeof(T) == typeof(Half) ? static (decimal v) => {
              var val = (Half)v;
              return Unsafe.As<Half, T>(ref val);
          }
    : typeof(T) == typeof(decimal) ? static (decimal v) => {
              return Unsafe.As<decimal, T>(ref v);
          }
    : (static (decimal v) => { return GenericNumbers.ThrowIfUnsupported<decimal, T>(); });

    /// <summary> A statically-cached delegate to convert a <see cref="Half"/> to a <typeparamref name="T"/>
    /// </summary>
    public static Func<Half, T> FromHalf { get; }
       = typeof(T) == typeof(Half) ? static (Half v) => {
           return Unsafe.As<Half, T>(ref v);
       }
    : typeof(T) == typeof(float) ? static (Half v) => {
        var val = (float)v;
        return Unsafe.As<float, T>(ref val);
    }
    : typeof(T) == typeof(double) ? static (Half v) => {
        var val = (double)v;
        return Unsafe.As<double, T>(ref val);
    }
    : typeof(T) == typeof(decimal) ? static (Half v) => {
        var val = (decimal)v;
        return Unsafe.As<decimal, T>(ref val);
    }
    : (static (Half v) => { return GenericNumbers.ThrowIfUnsupported<Half, T>(); });

    /// <summary> A statically-cached delegate to convert an <see cref="int"/> to a <typeparamref name="T"/>
    /// </summary>
    public static Func<int, T> FromInt32 { get; }
        = typeof(T) == typeof(float) ? static (int v) => {
            var val = (float)v;
            return Unsafe.As<float, T>(ref val);
        }
    : typeof(T) == typeof(double) ? static (int v) => {
        var val = (double)v;
        return Unsafe.As<double, T>(ref val);
    }
    : typeof(T) == typeof(Half) ? static (int v) => {
        var val = (Half)v;
        return Unsafe.As<Half, T>(ref val);
    }
    : typeof(T) == typeof(decimal) ? static (int v) => {
        var val = (decimal)v;
        return Unsafe.As<decimal, T>(ref val);
    }
    : (static (int v) => { return GenericNumbers.ThrowIfUnsupported<int, T>(); });

    /// <summary> A statically-cached delegate to convert a <see cref="long"/> to a <typeparamref name="T"/>
    /// </summary>
    public static Func<long, T> FromInt64 { get; }
        = typeof(T) == typeof(float) ? static (long v) => {
            var val = (float)v;
            return Unsafe.As<float, T>(ref val);
        }
    : typeof(T) == typeof(double) ? static (long v) => {
        var val = (double)v;
        return Unsafe.As<double, T>(ref val);
    }
    : typeof(T) == typeof(Half) ? static (long v) => {
        var val = (Half)v;
        return Unsafe.As<Half, T>(ref val);
    }
    : typeof(T) == typeof(decimal) ? static (long v) => {
        var val = (decimal)v;
        return Unsafe.As<decimal, T>(ref val);
    }
    : (static (long v) => { return GenericNumbers.ThrowIfUnsupported<long, T>(); });

    /// <summary> A statically-cached delegate to convert a <see cref="float"/> to a <typeparamref name="T"/>
    /// </summary>
    public static Func<float, T> FromSingle { get; }
        = typeof(T) == typeof(float) ? static (float v) => {
            return Unsafe.As<float, T>(ref v);
        }
    : typeof(T) == typeof(double) ? static (float v) => {
        var val = (double)v;
        return Unsafe.As<double, T>(ref val);
    }
    : typeof(T) == typeof(Half) ? static (float v) => {
        var val = (Half)v;
        return Unsafe.As<Half, T>(ref val);
    }
    : typeof(T) == typeof(decimal) ? static (float v) => {
        var val = (decimal)v;
        return Unsafe.As<decimal, T>(ref val);
    }
    : (static (float v) => { return GenericNumbers.ThrowIfUnsupported<float, T>(); });

    /// <summary> A statically-cached delegate to convert a <see cref="uint"/> to a <typeparamref name="T"/>
    /// </summary>
    public static Func<uint, T> FromUInt32 { get; }
        = typeof(T) == typeof(float) ? static (uint v) => {
            var val = (float)v;
            return Unsafe.As<float, T>(ref val);
        }
    : typeof(T) == typeof(double) ? static (uint v) => {
        var val = (double)v;
        return Unsafe.As<double, T>(ref val);
    }
    : typeof(T) == typeof(Half) ? static (uint v) => {
        var val = (Half)v;
        return Unsafe.As<Half, T>(ref val);
    }
    : typeof(T) == typeof(decimal) ? static (uint v) => {
        var val = (decimal)v;
        return Unsafe.As<decimal, T>(ref val);
    }
    : (static (uint v) => { return GenericNumbers.ThrowIfUnsupported<uint, T>(); });

    /// <summary> A statically-cached delegate to convert a <see cref="ulong"/> to a <typeparamref name="T"/>
    /// </summary>
    public static Func<ulong, T> FromUInt64 { get; }
        = typeof(T) == typeof(float) ? static (ulong v) => {
            var val = (float)v;
            return Unsafe.As<float, T>(ref val);
        }
    : typeof(T) == typeof(double) ? static (ulong v) => {
        var val = (double)v;
        return Unsafe.As<double, T>(ref val);
    }
    : typeof(T) == typeof(Half) ? static (ulong v) => {
        var val = (Half)v;
        return Unsafe.As<Half, T>(ref val);
    }
    : typeof(T) == typeof(decimal) ? static (ulong v) => {
        var val = (decimal)v;
        return Unsafe.As<decimal, T>(ref val);
    }
    : (static (ulong v) => { return GenericNumbers.ThrowIfUnsupported<ulong, T>(); });

    #endregion From

    #region To

    /// <summary> A statically-cached delegate to convert a <typeparamref name="T"/> to a <see cref="double"/>
    /// </summary>
    public static Func<T, double> ToDouble { get; }
        = typeof(T) == typeof(double) ? (static (T value) => {
            return Unsafe.As<T, double>(ref value);
        })
    : typeof(T) == typeof(float) ? (static (T value) => {
        return (double)Unsafe.As<T, float>(ref value);
    })
    : typeof(T) == typeof(decimal) ? (static (T value) => {
        return (double)Unsafe.As<T, decimal>(ref value);
    })
    : typeof(T) == typeof(Half) ? (static (T value) => {
        return (double)Unsafe.As<T, Half>(ref value);
    })
    : (static (T value) => { return GenericNumbers.ThrowIfUnsupported<T, double>(); });

    /// <summary> A statically-cached delegate to convert a <typeparamref name="T"/> to a <see cref="decimal"/>
    /// </summary>
    public static Func<T, decimal> ToDecimal { get; }
        = typeof(T) == typeof(float) ? (static (T value) => {
            return (decimal)Unsafe.As<T, float>(ref value);
        })
    : typeof(T) == typeof(double) ? (static (T value) => {
        return (decimal)Unsafe.As<T, double>(ref value);
    })
    : typeof(T) == typeof(decimal) ? (static (T value) => {
        return Unsafe.As<T, decimal>(ref value);
    })
    : typeof(T) == typeof(Half) ? (static (T value) => {
        return (decimal)Unsafe.As<T, Half>(ref value);
    })
    : (static (T value) => { return GenericNumbers.ThrowIfUnsupported<T, decimal>(); });

    /// <summary> A statically-cached delegate to convert a <typeparamref name="T"/> to a <see cref="Half"/>
    /// </summary>
    public static Func<T, Half> ToHalf { get; }
        = typeof(T) == typeof(float) ? (static (T value) => {
            return (Half)Unsafe.As<T, float>(ref value);
        })
    : typeof(T) == typeof(double) ? (static (T value) => {
        return (Half)Unsafe.As<T, double>(ref value);
    })
    : typeof(T) == typeof(decimal) ? (static (T value) => {
        return (Half)Unsafe.As<T, decimal>(ref value);
    })
    : typeof(T) == typeof(Half) ? (static (T value) => {
        return Unsafe.As<T, Half>(ref value);
    })
    : (static (T value) => { return GenericNumbers.ThrowIfUnsupported<T, Half>(); });

    /// <summary> A statically-cached delegate to convert a <typeparamref name="T"/> to a <see cref="int"/>
    /// </summary>
    public static Func<T, int> ToInt32 { get; }
        = typeof(T) == typeof(float) ? static (T v) => {
            return (int)Unsafe.As<T, float>(ref v);
        }
    : typeof(T) == typeof(double) ? static (T v) => {
        return (int)Unsafe.As<T, double>(ref v);
    }
    : typeof(T) == typeof(Half) ? static (T v) => {
        return (int)Unsafe.As<T, Half>(ref v);
    }
    : typeof(T) == typeof(decimal) ? static (T v) => {
        return (int)Unsafe.As<T, decimal>(ref v);
    }
    : (static (T value) => { return GenericNumbers.ThrowIfUnsupported<T, int>(); });

    /// <summary> A statically-cached delegate to convert a <typeparamref name="T"/> to a <see cref="long"/>
    /// </summary>
    public static Func<T, long> ToInt64 { get; }
        = typeof(T) == typeof(float) ? static (T v) => {
            return (long)Unsafe.As<T, float>(ref v);
        }
    : typeof(T) == typeof(double) ? static (T v) => {
        return (long)Unsafe.As<T, double>(ref v);
    }
    : typeof(T) == typeof(Half) ? static (T v) => {
        return (long)Unsafe.As<T, Half>(ref v);
    }
    : typeof(T) == typeof(decimal) ? static (T v) => {
        return (long)Unsafe.As<T, decimal>(ref v);
    }
    : (static (T value) => { return GenericNumbers.ThrowIfUnsupported<T, long>(); });

    /// <summary> A statically-cached delegate to convert a <typeparamref name="T"/> to a <see cref="float"/>
    /// </summary>
    public static Func<T, float> ToSingle { get; }
        = typeof(T) == typeof(double) ? (static (T value) => {
            return (float)Unsafe.As<T, double>(ref value);
        })
    : typeof(T) == typeof(float) ? (static (T value) => {
        return Unsafe.As<T, float>(ref value);
    })
    : typeof(T) == typeof(decimal) ? (static (T value) => {
        return (float)Unsafe.As<T, decimal>(ref value);
    })
    : typeof(T) == typeof(Half) ? (static (T value) => {
        return (float)Unsafe.As<T, Half>(ref value);
    })
    : (static (T value) => { return GenericNumbers.ThrowIfUnsupported<T, float>(); });

    /// <summary> A statically-cached delegate to convert a <typeparamref name="T"/> to a <see cref="uint"/>
    /// </summary>
    public static Func<T, uint> ToUInt32 { get; }
        = typeof(T) == typeof(float) ? static (T v) => {
            return (uint)Unsafe.As<T, float>(ref v);
        }
    : typeof(T) == typeof(double) ? static (T v) => {
        return (uint)Unsafe.As<T, double>(ref v);
    }
    : typeof(T) == typeof(Half) ? static (T v) => {
        return (uint)Unsafe.As<T, Half>(ref v);
    }
    : typeof(T) == typeof(decimal) ? static (T v) => {
        return (uint)Unsafe.As<T, decimal>(ref v);
    }
    : (static (T value) => { return GenericNumbers.ThrowIfUnsupported<T, uint>(); });

    /// <summary> A statically-cached delegate to convert a <typeparamref name="T"/> to a <see cref="ulong"/>
    /// </summary>
    public static Func<T, ulong> ToUInt64 { get; }
        = typeof(T) == typeof(float) ? static (T v) => {
            return (ulong)Unsafe.As<T, float>(ref v);
        }
    : typeof(T) == typeof(double) ? static (T v) => {
        return (ulong)Unsafe.As<T, double>(ref v);
    }
    : typeof(T) == typeof(Half) ? static (T v) => {
        return (ulong)Unsafe.As<T, Half>(ref v);
    }
    : typeof(T) == typeof(decimal) ? static (T v) => {
        return (ulong)Unsafe.As<T, decimal>(ref v);
    }
    : (static (T value) => { return GenericNumbers.ThrowIfUnsupported<T, ulong>(); });

    #endregion To

#if DEBUG

    #region Fast-access Integers

    ///<summary>2</summary>
    public static readonly T Two = T.One + T.One;

    ///<summary>3</summary>
    public static readonly T Three = Two + T.One;

    ///<summary>4</summary>
    public static readonly T Four = Two + Two;

    ///<summary>5</summary>
    public static readonly T Five = Three + Two;

    ///<summary>6</summary>
    public static readonly T Six = Three + Three;

    ///<summary>8</summary>
    public static readonly T Eight = Four + Four;

    ///<summary>10</summary>
    public static readonly T Ten = Five + Five;

    ///<summary>12</summary>
    public static readonly T Twelve = Six + Six;

    ///<summary>16</summary>
    public static readonly T Sixteen = Eight + Eight;

    ///<summary>20</summary>
    public static readonly T Twenty = Ten + Ten;

    ///<summary>60</summary>
    public static readonly T Sixty = Twenty * Three;

    ///<summary>80</summary>
    public static readonly T Eighty = Twenty * Four;

    ///<summary>100</summary>
    public static readonly T OneHundred = Twenty * Five;

    ///<summary>120</summary>
    public static readonly T OneHundredAndTwenty = Twenty * Six;

    ///<summary>180</summary>
    public static readonly T OneHundredAndEighty = Sixty * Three;

    ///<summary>200</summary>
    public static readonly T TwoHundred = Twenty * Ten;

    ///<summary>240 <c>(0xF0)</c></summary>
    public static readonly T TwoHundredAndForty = OneHundredAndTwenty * Two;

    ///<summary>255</summary>
    public static readonly T TwoHundredAndFiftyFive = (Sixty * Four) + (Five * Three);

    ///<summary>256</summary>
    /// <remarks>This value is <see cref="INumberBase{T}.CreateTruncating{TOther}(TOther)">truncated</see> when numeric type <typeparamref name="T"/> is too limited to represent it.</remarks>
    public static readonly T TwoHundredAndFiftySix = TwoHundredAndFiftyFive + T.One;

    ///<summary>1000</summary>
    /// <remarks><inheritdoc cref="TwoHundredAndFiftySix"/></remarks>
    public static readonly T OneThousand = T.CreateTruncating(1000);

    ///<summary>1,000,000</summary>
    /// <remarks><inheritdoc cref="TwoHundredAndFiftySix"/></remarks>
    public static readonly T OneMillion = OneThousand * OneThousand;

    ///<summary>65280 <c>(0xFF00)</c></summary>
    /// <remarks><inheritdoc cref="TwoHundredAndFiftySix"/></remarks>
    public static readonly T xFF00 = T.CreateTruncating(0xFF00);

    ///<summary>4,294,901,760 <c>(0xFFFF0000)</c></summary>
    /// <remarks><inheritdoc cref="TwoHundredAndFiftySix"/></remarks>
    public static readonly T xFFFF0000 = T.CreateTruncating(0xFFFF0000);

    #endregion Fast-access Integers

    #region Fast-access Fractions

    ///<summary>1 / 2</summary>
    public static readonly T OneHalf = T.One / Two;

    ///<summary>3 / 2</summary>
    public static readonly T ThreeHalves = Three / Two;

    ///<summary>1 / 3</summary>
    public static readonly T OneThird = T.One / Three;

    ///<summary>2 / 3</summary>
    public static readonly T TwoThirds = Two / Three;

    ///<summary>4 / 3</summary>
    public static readonly T FourThirds = Four / Three;

    ///<summary>1 / 4</summary>
    public static readonly T OneFourth = T.One / Four;

    ///<summary>3 / 4</summary>
    public static readonly T ThreeFourths = Three / Four;

    ///<summary>1 / 5</summary>
    public static readonly T OneFifth = T.One / Five;

    ///<summary>1 / 6</summary>
    public static readonly T OneSixth = T.One / Six;

    ///<summary>1 / 10</summary>
    public static readonly T OneTenth = T.One / Ten;

    ///<summary>1 / 12</summary>
    public static readonly T OneTwelfth = T.One / Twelve;

    ///<summary>1 / 20</summary>
    public static readonly T OneTwentieth = T.One / Twenty;

    ///<summary>1 / 60</summary>
    public static readonly T OneSixtieth = T.One / Sixty;

    ///<summary>1 / 180</summary>
    public static readonly T OneOneHundredAndEightieth = T.One / OneHundredAndEighty;

    ///<summary>1 / 255</summary>
    public static readonly T OneTwoHundredAndFiftyFifth = T.One / TwoHundredAndFiftyFive;

    ///<summary>1 / 1000</summary>
    public static readonly T OneThousandth = T.One / OneThousand;

    ///<summary>1 / 1,000,000</summary>
    public static readonly T OneMillionth = T.One / (OneThousand * OneThousand);

    /// <summary>0.999</summary>
    public static readonly T OneMinusOneThousandth = T.One - OneThousandth;

    /// <summary>0.999999</summary>
    public static readonly T OneMinusOneMillionth = T.One - OneMillionth;

    /// <summary>0.499</summary>
    public static readonly T OneHalfMinusOneThousandth = OneHalf - OneThousandth;

    #endregion Fast-access Fractions

    #region Fast-access Roots / Fractional Roots

    ///<summary>√2</summary>
    public static readonly T RootTwo = typeof(T) == typeof(float) ? FromSingle(float.Sqrt(2f)) : typeof(T) == typeof(double) ? FromDouble(double.Sqrt(2d)) : typeof(T) == typeof(Half) ? FromHalf(Half.Sqrt((Half)2)) : typeof(T) == typeof(decimal) ? FromDecimal(2M.Sqrt()) : throw new NotImplementedException();

    ///<summary>1 / √2</summary>
    public static readonly T OneOverRootTwo = T.One / RootTwo;

    ///<summary>√3</summary>
    public static readonly T RootThree = typeof(T) == typeof(float) ? FromSingle(float.Sqrt(3f)) : typeof(T) == typeof(double) ? FromDouble(double.Sqrt(3d)) : typeof(T) == typeof(Half) ? FromHalf(Half.Sqrt((Half)3)) : typeof(T) == typeof(decimal) ? FromDecimal(3M.Sqrt()) : throw new NotImplementedException();

    ///<summary>1 / √3</summary>
    public static readonly T OneOverRootThree = T.One / RootThree;

    ///<summary>√3 / 2</summary>
    public static readonly T RootThreeOverTwo = RootThree / Two;

    ///<summary>2 / √3</summary>
    public static readonly T TwoOverRootThree = Two / RootThree;

    ///<summary>12th√2</summary>
    public static readonly T TwelfthRootTwo = typeof(T) == typeof(float) ? FromSingle(float.Pow(2f, 1f / 12f)) : typeof(T) == typeof(double) ? FromDouble(double.Pow(2d, 1d / 12d)) : typeof(T) == typeof(Half) ? FromHalf(Half.Pow((Half)2, (Half)(1f / 12f))) : typeof(T) == typeof(decimal) ? FromDecimal(2M.Pow(1M / 12M)) : throw new NotImplementedException();

    ///<summary>1 / 12th√2</summary>
    public static readonly T OneOverTwelfthRootTwo = T.One / TwelfthRootTwo;

    #endregion Fast-access Roots / Fractional Roots

    #region PI and PI Partials

    /// <summary>1 / π</summary>
    public static readonly T OneOverPi = T.One / T.Pi;

    /// <summary>1 / (2π)</summary>
    public static readonly T OneOverTwoPi = T.One / (T.Pi * Two);

    /// <summary>π / 2</summary>
    public static readonly T HalfPi = T.Pi / Two;

    /// <summary>2π</summary>
    public static readonly T TwoPi = T.Pi * Two;

    /// <summary>π / 4</summary>
    public static readonly T QuarterPi = T.Pi / Four;

    /// <summary>4π</summary>
    public static readonly T FourPi = T.Pi * Four;

    #endregion PI and PI Partials

    #region Common Tuples

    /// <summary>
    /// (2, 1/2)
    /// </summary>
    public static readonly (T Two, T OneHalf) Two_OneHalf = (Two, OneHalf);

    #endregion Common Tuples

#else

    #region Fast-access Integers

    ///<summary>2</summary>
    public static T Two { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => T.CreateTruncating(2); }

    ///<summary>3</summary>
    public static T Three { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => T.CreateTruncating(3); }

    ///<summary>4</summary>
    public static T Four { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => T.CreateTruncating(4); }

    ///<summary>5</summary>
    public static T Five { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => T.CreateTruncating(5); }

    ///<summary>6</summary>
    public static T Six { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => T.CreateTruncating(6); }

    ///<summary>8</summary>
    public static T Eight { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => T.CreateTruncating(8); }

    ///<summary>10</summary>
    public static T Ten { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => T.CreateTruncating(10); }

    ///<summary>12</summary>
    public static T Twelve { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => T.CreateTruncating(12); }

    ///<summary>16</summary>
    public static T Sixteen { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => T.CreateTruncating(16); }

    ///<summary>20</summary>
    public static T Twenty { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => T.CreateTruncating(20); }

    ///<summary>60</summary>
    public static T Sixty { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => T.CreateTruncating(60); }

    ///<summary>80</summary>
    public static T Eighty { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => T.CreateTruncating(80); }

    ///<summary>100</summary>
    public static T OneHundred { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => T.CreateTruncating(100); }

    ///<summary>120</summary>
    public static T OneHundredAndTwenty { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => T.CreateTruncating(120); }

    ///<summary>180</summary>
    public static T OneHundredAndEighty { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => T.CreateTruncating(180); }

    ///<summary>200</summary>
    public static T TwoHundred { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => T.CreateTruncating(200); }

    ///<summary>240</summary>
    public static T TwoHundredAndForty { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => T.CreateTruncating(240); }

    ///<summary>255</summary>
    public static T TwoHundredAndFiftyFive { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => T.CreateTruncating(255); }

    ///<summary>256</summary>
    /// <remarks>This value is <see cref="INumberBase{T}.CreateTruncating{TOther}(TOther)">truncated</see> when numeric type <typeparamref name="T"/> is too limited to represent it.</remarks>
    public static T TwoHundredAndFiftySix { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => T.CreateTruncating(256); }

    ///<summary>1000</summary>
    /// <remarks><inheritdoc cref="TwoHundredAndFiftySix"/></remarks>
    public static T OneThousand { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => T.CreateTruncating(1000); }

    ///<summary>1,000,000</summary>
    /// <remarks><inheritdoc cref="TwoHundredAndFiftySix"/></remarks>
    public static T OneMillion { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => T.CreateTruncating(1_000_000); }

    ///<summary>65280 <c>(0xFF00)</c></summary>
    /// <remarks><inheritdoc cref="TwoHundredAndFiftySix"/></remarks>
    public static T xFF00 { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => T.CreateTruncating(0xFF00); }

    ///<summary>4,294,901,760 <c>(0xFFFF0000)</c></summary>
    /// <remarks><inheritdoc cref="TwoHundredAndFiftySix"/></remarks>
    public static T xFFFF0000 { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => T.CreateTruncating(0xFFFF0000); }

    #endregion Fast-access Integers

    #region Fast-access Fractions

    ///<summary>1 / 2</summary>
    public static T OneHalf { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => T.CreateChecked(0.5); }

    ///<summary>3 / 2</summary>
    public static T ThreeHalves { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => T.CreateChecked(1.5); }

    ///<summary>1 / 3</summary>
    public static T OneThird { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => T.One / Three; }

    ///<summary>2 / 3</summary>
    public static T TwoThirds { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => Two / Three; }

    ///<summary>4 / 3</summary>
    public static T FourThirds { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => Four / Three; }

    ///<summary>1 / 4</summary>
    public static T OneFourth { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => T.CreateChecked(0.25); }

    ///<summary>3 / 4</summary>
    public static T ThreeFourths { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => T.CreateChecked(0.75); }

    ///<summary>1 / 5</summary>
    public static T OneFifth { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => T.CreateChecked(0.2); }

    ///<summary>1 / 6</summary>
    public static T OneSixth { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => T.One / Six; }

    ///<summary>1 / 10</summary>
    public static T OneTenth { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => T.CreateChecked(0.1); }

    ///<summary>1 / 12</summary>
    public static T OneTwelfth { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => T.One / Twelve; }

    ///<summary>1 / 20</summary>
    public static T OneTwentieth { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => T.CreateChecked(0.05); }

    ///<summary>1 / 60</summary>
    public static T OneSixtieth { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => T.One / Sixty; }

    ///<summary>1 / 180</summary>
    public static T OneOneHundredAndEightieth { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => T.One / OneHundredAndEighty; }

    ///<summary>1 / 255</summary>
    public static T OneTwoHundredAndFiftyFifth { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => T.One / TwoHundredAndFiftyFive; }

    ///<summary>1 / 1000</summary>
    public static T OneThousandth { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => T.CreateChecked(0.001); }

    ///<summary>1 / 1,000,000</summary>
    public static T OneMillionth { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => T.CreateChecked(0.000001); }

    /// <summary>0.999</summary>
    public static T OneMinusOneThousandth { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => T.One - OneThousandth; }

    /// <summary>0.999999</summary>
    public static T OneMinusOneMillionth { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => T.One - OneMillionth; }

    /// <summary>0.499</summary>
    public static T OneHalfMinusOneThousandth { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => OneHalf - OneThousandth; }

    #endregion Fast-access Fractions

    #region Fast-access Roots / Fractional Roots

    ///<summary>√2</summary>
    public static T RootTwo { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => typeof(T) == typeof(float) ? FromSingle(float.Sqrt(2f)) : typeof(T) == typeof(double) ? FromDouble(double.Sqrt(2d)) : typeof(T) == typeof(Half) ? FromHalf(Half.Sqrt((Half)2)) : typeof(T) == typeof(decimal) ? FromDecimal(2M.Sqrt()) : throw new NotImplementedException(); }

    ///<summary>1 / √2</summary>
    public static T OneOverRootTwo { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => T.One / RootTwo; }

    ///<summary>√3</summary>
    public static T RootThree { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => typeof(T) == typeof(float) ? FromSingle(float.Sqrt(3f)) : typeof(T) == typeof(double) ? FromDouble(double.Sqrt(3d)) : typeof(T) == typeof(Half) ? FromHalf(Half.Sqrt((Half)3)) : typeof(T) == typeof(decimal) ? FromDecimal(3M.Sqrt()) : throw new NotImplementedException(); }

    ///<summary>1 / √3</summary>
    public static T OneOverRootThree { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => T.One / RootThree; }

    ///<summary>√3 / 2</summary>
    public static T RootThreeOverTwo { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => RootThree / Two; }

    ///<summary>2 / √3</summary>
    public static T TwoOverRootThree { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => Two / RootThree; }

    ///<summary>12th√2</summary>
    public static T TwelfthRootTwo { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => typeof(T) == typeof(float) ? FromSingle(float.Pow(2f, 1f / 12f)) : typeof(T) == typeof(double) ? FromDouble(double.Pow(2d, 1d / 12d)) : typeof(T) == typeof(Half) ? FromHalf(Half.Pow((Half)2, (Half)(1f / 12f))) : typeof(T) == typeof(decimal) ? FromDecimal(2M.Pow(1M / 12M)) : throw new NotImplementedException(); }

    ///<summary>1 / 12th√2</summary>
    public static T OneOverTwelfthRootTwo { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => T.One / TwelfthRootTwo; }

    #endregion Fast-access Roots / Fractional Roots

    #region PI and PI Partials

    /// <summary>1 / π</summary>
    public static T OneOverPi { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => T.One / T.Pi; }

    /// <summary>1 / (2π)</summary>
    public static T OneOverTwoPi { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => OneHalf / T.Pi; }

    /// <summary>π / 2</summary>
    public static T HalfPi { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => OneHalf * T.Pi; }

    /// <summary>2π</summary>
    public static T TwoPi { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => T.Pi * Two; }

    /// <summary>π / 4</summary>
    public static T QuarterPi { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => T.Pi / Four; }

    /// <summary>4π</summary>
    public static T FourPi { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => T.Pi * Four; }

    #endregion PI and PI Partials

    #region Common Tuples

    /// <summary>
    /// (2, 1/2)
    /// </summary>
    public static (T Two, T OneHalf) Two_OneHalf { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => (T.CreateChecked(2), T.CreateChecked(0.5)); }

    #endregion Common Tuples

#endif

    #region Arrays

    internal static readonly ImmutableArray<T> ArrayOfT;

    internal static readonly ImmutableArray<T> ArrayOfTFractions;

    #endregion Arrays

    #region Initialization

    static GenericNumbers() {
        // instantiate the arrays
        var _ArrayOfT = new T[256];
        var _ArrayOfTFractions = new T[256];

        // initialize
        var one = T.One;
        var val = one;

        // assign 0-index values manually to avoid a division by zero
        _ArrayOfT[0] = T.Zero;
        _ArrayOfTFractions[0] = T.CreateTruncating(double.PositiveInfinity);

        // assign the rest of the values
        for (int i = 1; i < 256; i++) {
            _ArrayOfT[i] = val;
            _ArrayOfTFractions[i] = T.One / val;
            val += one;
            }

        ArrayOfT = ImmutableArray.Create(_ArrayOfT);
        ArrayOfTFractions = ImmutableArray.Create(_ArrayOfTFractions);
        }

    #endregion Initialization

    #region Fetch

    /// <inheritdoc
    /// cref="GenericNumbers.GetReciprocal{T}(byte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T GetReciprocal(byte denominator) {
        return ArrayOfTFractions[denominator];
        }

    #endregion Fetch

    #region Epsilon

    /// <summary> The smallest positive value that can be represented by the numeric type.
    /// </summary>
    public static T Epsilon { get; } = GetEpsilon();

    static T GetEpsilon() {
        if (typeof(T) == typeof(float)) {
            return FromSingle(float.Pow(2f, -24f));
            }
        else if (typeof(T) == typeof(double)) {
            return FromDouble(double.Pow(2d, -53d));
            }
        else if (typeof(T) == typeof(Half)) {
            return FromHalf(Half.Pow((Half)2, -11));
            }
        else if (typeof(T) == typeof(decimal)) {
            return FromDecimal(0.0000000000000000000000000001M);
            }
        else throw new NotImplementedException();
        }

    #endregion Epsilon
    }
