namespace Bunnarium.Maths.Primitives;

/// <summary> Inherited by floating-point vector types such as <see cref="Vector2{T}"/> and <see cref="Vector3{T}"/>.
/// </summary>
public interface IFloatingPointVector<TVector, T>
    : IVector<TVector, T>
    where TVector
    : unmanaged
    , IFloatingPointVector<TVector, T>
    where T
    : unmanaged
    , IFloatingPoint<T>
    , IMinMaxValue<T> {

    #region Ceiling & Floor

    /// <summary> Rounds all values in a vector up to the next higher integer.
    /// </summary>
    static abstract TVector Ceiling(TVector vector);

    /// <inheritdoc
    /// cref="Ceiling(TVector)"/>
    static abstract void Ceiling(ref TVector vector);

    /// <inheritdoc
    /// cref="Ceiling(TVector)"/>
    TVector Ceiling();

    /// <summary> Rounds all values in a vector down to the next lower integer.
    /// </summary>
    static abstract TVector Floor(TVector vector);

    /// <inheritdoc
    /// cref="Floor(TVector)"/>
    static abstract void Floor(ref TVector vector);

    /// <inheritdoc
    /// cref="Floor(TVector)"/>
    TVector Floor();

    #endregion Ceiling & Floor

    #region Constants

    /// <summary> A constant such that (<c><see cref="RootN">RootN</see> ^ 2</c>) * <see cref="IVector{TVector, T}.Length"/> = <c>1</c>
    /// <para/> A <typeparamref name="TVector"/> in which each component has this value will have a magnitude of <c>1</c>.
    /// </summary>
    static abstract TVector RootN { get; }

    /// <summary> A <typeparamref name="TVector"/> where the third component is set to <c>1</c> and all other components are set to <c>0</c>.
    /// </summary>
    /// <remarks> If there aren't at least three components, then the first component is set to <c>1</c> instead.
    /// </remarks>
    static abstract TVector Forward { get; }

    #endregion Constants

    #region Horizontal Averages

    /// <inheritdoc
    /// cref="HorizontalAverage(in TVector)"/>
    T Average { get; }

    /// <returns> The average value of the <paramref name="vector"/>'s components.
    /// </returns>
    static abstract T HorizontalAverage(in TVector vector);

    /// <inheritdoc
    /// cref="HorizontalAbsoluteAverage(in TVector)"/>
    T AbsoluteAverage { get; }

    /// <returns> The average absolute value of the <paramref name="vector"/>'s components.
    /// Each component's absolute value is evaluated before their average is calculated.
    /// </returns>
    static abstract T HorizontalAbsoluteAverage(in TVector vector);

    #endregion Horizontal Averages

    #region Lerp

    /// <summary> Linearly interpolates between 2 <typeparamref name="TVector"/>s such that the output will be
    /// the <paramref name="amount"/> % progress marker from the <paramref name="from"/> vector to the
    /// <paramref name="to"/> vector.
    /// </summary>
    /// <inheritdoc cref="DocStrings.Lerp_Amount_Param{TVector, T}(TVector, TVector, T)"/>
    static abstract TVector Lerp(TVector from, TVector to, T amount);

    /// <summary> Linearly interpolates between 2 <typeparamref name="TVector"/>s such that the output will be
    /// the <paramref name="amount"/> % progress marker from this vector to the <paramref name="to"/>
    /// vector.
    /// </summary>
    /// <inheritdoc cref="Lerp(TVector, TVector, T)"/>
    TVector Lerp(TVector to, T amount);

    #endregion Lerp

    #region Round

    /// <summary><inheritdoc cref="DocStrings.Round_Create_Static"/>
    /// <para/> <inheritdoc cref="DocStrings.Round_ZeroDefault"/>
    /// </summary>
    static abstract TVector Round(TVector vector);

    /// <inheritdoc
    /// cref="DocStrings.Round_Create_Static{TVector}"/>
    static abstract TVector Round(TVector vector, byte digits);

    /// <summary><inheritdoc cref="DocStrings.Round_Static{TVector}"/> <inheritdoc cref="DocStrings.Round_ZeroDefault"/>
    /// </summary>
    static abstract void Round(ref TVector vector);

    /// <inheritdoc
    /// cref="DocStrings.Round_Static{TVector}"/>
    static abstract void Round(ref TVector vector, byte digits);

    /// <summary><inheritdoc cref="DocStrings.Round"/> <inheritdoc cref="DocStrings.Round_ZeroDefault"/>
    /// </summary>
    TVector Round();

    /// <inheritdoc
    /// cref="DocStrings.Round"/>
    TVector Round(byte digits);

    #endregion Round

    #region Normalization

    /// <summary> Returns whether this vector is normalized (i.e., has a <see cref="IVector{TVector, T}.Magnitude"/> of <c>1</c>, making it a unit-length vector).
    /// </summary>
    bool IsNormalized { get; }

    /// <summary> Returns a vector that has the same direction (i.e., ratios between different components as the input <paramref name="vector"/>), but a <see cref="IVector{TVector, T}.Magnitude">Magnitude</see> of <c>1</c>.
    /// </summary>
    static abstract TVector Normalize(TVector vector);

    /// <summary> Modifies / scales a <paramref name="vector"/> to have the same direction (i.e., ratios between different components), but a <see cref="IVector{TVector, T}.Magnitude">Magnitude</see> of <c>1</c>.
    /// </summary>
    static abstract void Normalize(ref TVector vector);

    /// <inheritdoc
    /// cref="Normalize(TVector)"/>
    TVector Normalize();

    #endregion Normalization
    }
