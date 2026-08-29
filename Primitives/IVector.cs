using System.Runtime.InteropServices;

namespace Bunnarium.Maths.Primitives;

/// <summary> The parent contract to all vectors, <see cref="IFloatingPointVector{TVector, T}">floating-point</see> and <see cref="IIntegralVector{TVector, T}">integral</see> alike.
/// </summary>
public interface IVector<TVector, T>
    : IAdditionOperators<TVector, TVector, TVector>
    , ISubtractionOperators<TVector, TVector, TVector>
    , IMultiplyOperators<TVector, T, TVector>
    , IDivisionOperators<TVector, T, TVector>
    , IEquatable<TVector>
    , IPrintable
    where TVector : unmanaged, IVector<TVector, T>
    where T : unmanaged, INumberBase<T>, IMinMaxValue<T> {

    #region Constants and Construction

    /// <summary> The number of components in this vector.
    /// </summary>
    static abstract int Length { get; }

    /// <summary> A vector with all components set to <see cref="IMinMaxValue{TSelf}.MaxValue"/>.
    /// </summary>
    static abstract TVector MaxValue { get; }

    /// <summary> A vector with all components set to <see cref="IMinMaxValue{TSelf}.MinValue"/>.
    /// </summary>
    static abstract TVector MinValue { get; }

    /// <summary> A vector with all components set to 1.
    /// </summary>
    static abstract TVector One { get; }

    /// <summary> A <typeparamref name="TVector"/> in which the first component is set to 1 and all components afterwards are set to 0.
    /// </summary>
    static abstract TVector Right { get; }

    /// <summary> The size of the <typeparamref name="TVector"/>&lt;<typeparamref name="T"/>&gt;, in bytes.
    ///</summary>
    static abstract int SizeOf { get; }

    /// <summary> A <typeparamref name="TVector"/> where the second component is set to 1 and all other components are set to 0.
    /// </summary>
    /// <remarks> If there aren't at least two components, then the first component is set to 1 instead.
    /// </remarks>
    static abstract TVector Up { get; }

    /// <summary> A vector with all components set to 0.
    /// </summary>
    static abstract TVector Zero { get; }

    /// <summary> Creates a vector with all components set to the input <paramref name="value"/>.
    /// </summary>
    static abstract TVector Create(T value);

    #endregion Constants and Construction

    #region Absolute Values

    /// <summary> Returns a copy of a vector in which all of the copy's components are set to the absolute value of the input's components.
    /// </summary>
    static abstract TVector Abs(in TVector vector); // considered fine for unsigned types, declared in INumberBase

    /// <inheritdoc
    /// cref="Abs(in TVector)"/>
    TVector Abs();

    #endregion Absolute Values

    #region Conversions

    /// <summary> Equivalent to <see cref="MemoryMarshal.CreateSpan{T}(ref T, int)"> MemoryMarshal.CreateSpan</see>(<see langword="ref"/> <paramref name="vector"/>)
    /// </summary>
    static abstract Span<T> ToSpan(ref TVector vector);

    #endregion Conversions

    #region Dot

    /// <summary> Returns the dot (e.g., <c><paramref name="left"/>.X * <paramref name="right"/>.X + <paramref name="left"/>.Y * <paramref name="right"/>.Y</c>) product of the input vectors.
    /// </summary>
    static abstract T Dot(TVector left, TVector right);

    /// <inheritdoc
    /// cref="Dot(TVector, TVector)"/>
    T Dot(TVector other);

    #endregion Dot

    #region Horizontal

    /// <inheritdoc
    /// cref="HorizontalAbsoluteSum(in TVector)"/>
    T AbsoluteSum { get; }

    /// <inheritdoc
    /// cref="HorizontalProduct(in TVector)"/>
    T Product { get; }

    /// <inheritdoc
    /// cref="HorizontalSum(in TVector)"/>
    T Sum { get; }

    /// <summary> Returns the absolute sum of the <paramref name="vector"/>'s components.
    /// <para/> Each component's absolute value is evaluated before their sum is calculated.
    /// </summary>
    static abstract T HorizontalAbsoluteSum(in TVector vector);

    /// <summary> Returns the multiplicative product of this <paramref name="vector"/>'s components.
    /// </summary>
    static abstract T HorizontalProduct(in TVector vector);

    /// <summary> Returns the additive sum of this <paramref name="vector"/>'s components.
    /// </summary>
    static abstract T HorizontalSum(in TVector vector);

    #endregion Horizontal

    #region Lexicographical Ordering

    /// <summary> Returns whether the <paramref name="left"/> vector is in lower lexicographical order than the <paramref name="right"/> vector (e.g., <c><paramref name="left"/>.X &lt; <paramref name="right"/>.X || ( <paramref name="left"/>.X == <paramref name="right"/>.X &amp;&amp; <paramref name="left"/>.Y &lt; <paramref name="right"/>.Y))</c>
    /// </summary>
    static abstract bool LexicographicallyPrecedes(TVector left, TVector right);

    /// <inheritdoc
    /// cref="LexicographicallyPrecedes(TVector, TVector)"/>
    bool LexicographicallyPrecedes(TVector other);

    #endregion Lexicographical Ordering

    #region Magnitude

    /// <summary> Returns the "length" of the vector, defined as the square root of this vector's dot product (aka the Pythagorean theorem).
    /// </summary>
    T Magnitude { get; }

    /// <summary> Returns the square of the "length" of the vector, defined as this vector's dot product.
    /// </summary>
    T MagnitudeSquared { get; }

    #endregion Magnitude

    #region Min / Max

    /// <summary> Returns the vector's highest-value component.
    /// </summary>
    static abstract T HorizontalMax(TVector vector);

    /// <summary> Returns the vector's lowest-value component.
    /// </summary>
    static abstract T HorizontalMin(TVector vector);

    /// <summary> Returns a vector in which each component value is the greater value between each input vectors' corresponding component.
    /// </summary>
    static abstract TVector Max(TVector left, TVector right);

    /// <summary> Returns a vector in which each component value is the lesser value between each input vectors' corresponding component.
    /// </summary>
    static abstract TVector Min(TVector left, TVector right);

    /// <inheritdoc
    /// cref="HorizontalMax(TVector)"/>
    T HorizontalMax();

    /// <inheritdoc
    /// cref="HorizontalMin(TVector)"/>
    T HorizontalMin();

    /// <inheritdoc
    /// cref="Max(TVector, TVector)"/>
    TVector Max(TVector other);

    /// <inheritdoc
    /// cref="Min(TVector, TVector)"/>
    TVector Min(TVector other);

    #endregion Min / Max

    #region Normalization

    /// <summary> Returns a vector that has the same direction (i.e., ratios between different components) as the input <paramref name="vector"/>, but a specified <see cref="Magnitude"/>.
    /// </summary>
    /// <inheritdoc cref="DocStrings.Normalize_Magnitude_Param{TData, T}(T)"/>
    static abstract TVector Normalize(TVector vector, T magnitude);

    /// <summary> Modifies / scales a <paramref name="vector"/> to have the same direction (i.e., ratios between different components), but a specified <see cref="Magnitude">Magnitude</see>.
    /// </summary>
    /// <inheritdoc cref="DocStrings.Normalize_Magnitude_Param{TData, T}(T)"/>
    static abstract void Normalize(ref TVector vector, T magnitude);

    /// <inheritdoc
    /// cref="Normalize(TVector, T)"/>
    TVector Normalize(T magnitude);

    #endregion Normalization

    #region Negation

    /// <summary> Returns a copy of the input <paramref name="vector"/> such that each component is multiplied by -1.
    /// </summary>
    /// <exception cref="ArithmeticException">On unsigned types.</exception>
    static abstract TVector Negate(TVector vector);

    /// <summary> Multiplies each component of the input <paramref name="vector"/> by -1.
    /// </summary>
    /// <exception cref="ArithmeticException">On unsigned types.</exception>
    static abstract void Negate(ref TVector vector);

    /// <inheritdoc
    /// cref="Negate(TVector)"/>
    TVector Negate();

    #endregion Negation

    #region Operators, Equality & Comparability

    /// <summary> Returns whether all components in <paramref name="left"/> are lesser than their corresponding components in <paramref name="right"/>.
    /// </summary>
    public static abstract bool operator <(in TVector left, in TVector right);

    /// <summary> Returns whether all components in <paramref name="left"/> are lesser than or equal to their corresponding components in <paramref name="right"/>.
    /// </summary>
    public static abstract bool operator <=(in TVector left, in TVector right);

    /// <summary> Returns whether all components in <paramref name="left"/> are greater than their corresponding components in <paramref name="right"/>.
    /// </summary>
    public static abstract bool operator >(in TVector left, in TVector right);

    /// <summary> Returns whether all components in <paramref name="left"/> are greater than or equal to their corresponding components in <paramref name="right"/>.
    /// </summary>
    public static abstract bool operator >=(in TVector left, in TVector right);

    /// <summary> Returns a vector in which the first component is the second input's first component added to the first input's first component, etc.
    /// </summary>
    static abstract TVector Add(TVector left, TVector right);

    /// <summary> Returns a vector in which the first component is the first input' first component divided by the second input's first component, etc.
    /// </summary>
    static abstract TVector ComponentDivide(TVector left, TVector right);

    /// <summary> Returns a vector in which the first component is the first input's first component multiplied by the second input's first component, etc.
    /// </summary>
    static abstract TVector ComponentMultiply(TVector left, TVector right);

    /// <inheritdoc
    /// cref="Subtract(TVector, TVector)"/>
    static abstract TVector operator -(TVector left, TVector right);

    /// <summary> Returns <see langword="false"/> if each component of two vectors are equal, <see langword="true"/> otherwise.
    /// </summary>
    static abstract bool operator !=(TVector left, TVector right);

    /// <inheritdoc
    /// cref="Scale(TVector, T)"/>
    static abstract TVector operator *(TVector vector, T value);

    /// <inheritdoc
    /// cref="Scale(TVector, T)"/>
    static abstract TVector operator *(T factor, TVector vector);

    /// <summary> Returns a copy of the input <paramref name="vector"/> in which each component has been divided by the input <paramref name="factor"/>.
    /// </summary>
    static abstract TVector operator /(TVector vector, T factor);

    /// <summary> Returns a copy of the input <paramref name="vector"/> in which each component is set to the input <paramref name="factor"/> divided by the original value.
    /// </summary>
    static abstract TVector operator /(T factor, TVector vector);

    /// <inheritdoc
    /// cref="Add(TVector, TVector)"/>
    static abstract TVector operator +(TVector left, TVector right);

    /// <summary> Returns whether each component of two vectors is equal.
    /// </summary>
    static abstract bool operator ==(TVector left, TVector right);

    /// <summary> Returns a copy of the input <paramref name="vector"/> in which each component has been multiplied by the input <paramref name="factor"/>.
    /// </summary>
    static abstract TVector Scale(TVector vector, T factor);

    /// <summary> Modifies the input <paramref name="vector"/> by multiplying each component by the input <paramref name="factor"/>.
    /// </summary>
    static abstract void Scale(ref TVector vector, T factor);

    /// <summary> Returns a vector in which the first component is the second input's first component subtracted from the first input's first component, etc.
    /// </summary>
    static abstract TVector Subtract(TVector left, TVector right);

    /// <inheritdoc
    /// cref="Add(TVector, TVector)"/>
    TVector Add(TVector other);

    /// <inheritdoc
    /// cref="ComponentDivide(TVector, TVector)"/>
    TVector ComponentDivide(TVector other);

    /// <inheritdoc
    /// cref="ComponentMultiply(TVector, TVector)"/>
    TVector ComponentMultiply(TVector other);

    /// <inheritdoc
    /// cref="Scale(TVector, T)"/>
    TVector Scale(T factor);

    /// <inheritdoc
    /// cref="Subtract(TVector, TVector)"/>
    TVector Subtract(TVector other);

    #endregion Operators, Equality & Comparability

    #region Orthogonality / Orthonormality

    /// <summary> Returns a vector that is orthogonal (perpendicular) to the input <paramref name="vector"/>.
    /// </summary>
    static abstract TVector Orthogonal(TVector vector);

    /// <inheritdoc
    /// cref="Orthogonal(TVector)"/>
    TVector Orthogonal();

    /// <summary> Returns whether two vectors, if representing lines stretching out to infinity, form a right-angle at their point of intersection.
    /// </summary>
    static abstract bool IsOrthogonalWith(TVector left, TVector right);

    /// <summary> Returns whether two vectors, if representing lines stretching out to infinity, form a right-angle at their point of intersection, and whether both have a <see cref="Magnitude"/> of 1.
    /// </summary>
    static abstract bool IsOrthonormalWith(TVector left, TVector right);

    /// <inheritdoc
    /// cref="IsOrthogonalWith(TVector, TVector)"/>
    bool IsOrthogonalWith(TVector other);

    /// <inheritdoc
    /// cref="IsOrthonormalWith(TVector, TVector)"/>
    bool IsOrthonormalWith(TVector other);

    #endregion Orthogonality / Orthonormality

    #region Sign

    /// <summary> Returns a vector reflecting the sign of the components of the input <paramref name="vector"/>. Positive values will be replaced with 1s, negative values by -1s, and zero values will remain the same.
    /// </summary>
    static abstract TVector Sign(TVector vector);

    /// <inheritdoc
    /// cref="Sign(TVector)"/>
    TVector Sign();

    #endregion Sign

    #region Step

    /// <returns><paramref name="left"/> + <see cref="Sign(TVector)">Sign(<paramref name="right"/> - <paramref name="left"/>)</see>
    /// </returns>
    static abstract TVector Step(TVector left, TVector right);

    /// <inheritdoc
    /// cref="Step(TVector, TVector)"/>
    TVector Step(TVector other);

    #endregion Step
    }
