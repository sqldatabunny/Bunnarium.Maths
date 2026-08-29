namespace Bunnarium.Maths.Primitives;

/// <summary> An interface defining mathematical and geometric properties of types representing a direction in n-dimensional space.
/// </summary>
public interface IDirection<TDirection, TRotation, TVector, T>
    : IEquatable<TDirection>
    , IPrintable
    where TDirection : unmanaged, IDirection<TDirection, TRotation, TVector, T>
    where TRotation : unmanaged, IRotation<TRotation, TDirection, TVector, T>
    where TVector : unmanaged, IFloatingPointVector<TVector, T>
    where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {

    #region Constants

    /// <inheritdoc
    /// cref="IFloatingPointVector{TVector, T}.Forward"/>
    static abstract TDirection Forward { get; }

    /// <inheritdoc
    /// cref="IVector{TVector, T}.Right"/>
    static abstract TDirection Right { get; }

    /// <inheritdoc
    /// cref="IVector{TVector, T}.Up"/>
    static abstract TDirection Up { get; }

    #endregion Constants

    #region Factories

    /// <returns> A newly-constructed <typeparamref name="TDirection"/> pointing in the same direction as the input <paramref name="vector"/>.
    /// </returns>
    static abstract TDirection FromVector(TVector vector);

    #endregion Factories

    #region Flip

    /// <summary> Returns the <typeparamref name="TDirection"/> opposite from the input <paramref name="direction"/>.
    /// </summary>
    /// <inheritdoc cref="DocStrings.Flip_Direction_Remarks"/>
    static abstract TDirection Flip(TDirection direction);

    /// <summary> Flips the input <paramref name="direction"/> in-place so that it faces the opposite way.
    /// </summary>
    /// <inheritdoc cref="DocStrings.Flip_Direction_Remarks"/>
    static abstract void Flip(ref TDirection direction);

    /// <inheritdoc
    /// cref="Flip(TDirection)"/>
    TDirection Flip();

    #endregion Flip

    #region Normalization

    /// <inheritdoc
    /// cref="DocStrings.Normalize_Summary_Direction{TData}"/>
    static abstract TDirection Normalize(TDirection direction);

    /// <inheritdoc
    /// cref="Normalize(TDirection)"/>
    static abstract void Normalize(ref TDirection direction);

    /// <inheritdoc
    /// cref="Normalize(TDirection)"/>
    TDirection Normalize();

    #endregion Normalization

    #region Operators and Equality

    /// <returns> The input <paramref name="direction"/>, rotated by the <see cref="IRotation{TRotation, TDirection, TVector, T}.Invert(TRotation)">inversion</see> of a given <paramref name="rotation"/> movement.
    /// </returns>
    static abstract TDirection operator -(TDirection direction, TRotation rotation);

    /// <returns> <see langword="false"/> if the input directions face the same direction, <see langword="true"/> if otherwise.
    /// </returns>
    static abstract bool operator !=(TDirection first, TDirection second);

    /// <returns> The input <paramref name="direction"/>, rotated by a given <paramref name="rotation"/> movement.
    /// </returns>
    static abstract TDirection operator +(TDirection direction, TRotation rotation);

    /// <returns> <see langword="true"/> if the input directions face the same direction, <see langword="false"/> if otherwise.
    /// </returns>
    static abstract bool operator ==(TDirection first, TDirection second);

    #endregion Operators and Equality

    #region Rotation

    /// <inheritdoc
    /// cref="IRotation{TRotation, TDirection, TVector, T}.Rotate(TDirection, TRotation)"/>
    static abstract TDirection Rotate(TRotation rotation, TDirection direction);

    /// <summary> Rotates a <paramref name="point"/> around an <paramref name="axis"/> by a given <paramref name="angle"/>.
    /// </summary>
    /// <param name="point"> The point to rotate around the <paramref name="axis"/>.
    /// </param>
    /// <param name="axis"> The axis to rotate the <paramref name="point"/> around.
    /// </param>
    /// <param name="angle"> The angle/magnitude to rotate the <paramref name="point"/> around the <paramref name="axis"/> by.
    /// <para/> <b>Note:</b> The angle will be <see cref="Angle{T}.Normalize(Angle{T})">normalized</see> before the rotation is applied.
    /// </param>
    static abstract TVector RotateAboutAxis(TVector point, TDirection axis, Angle<T> angle);

    /// <inheritdoc
    /// cref="IRotation{TRotation, TDirection, TVector, T}.Rotate(TDirection)"/>
    TDirection Rotate(TRotation rotation);

    /// <summary> Rotates a <paramref name="point"/> around an axis represented by this <typeparamref name="TDirection"/>.
    /// </summary>
    /// <remarks> <b><u>Important:</u></b> This instance-based function treats the instance as the axis of rotation—NOT the point being rotated. The <paramref name="point"/> to be rotated is instead an argument of this function.
    /// </remarks>
    /// <param name="point"> The point to rotate around the axis represented by this <typeparamref name="TDirection"/>.</param>
    /// <param name="angle"><inheritdoc cref="RotateAboutAxis(TVector, TDirection, Angle{T})"/></param>
    /// <inheritdoc cref="RotateAboutAxis(TVector, TDirection, Angle{T})"/>
    TVector RotateAboutAxis(TVector point, Angle<T> angle);

    #endregion Rotation

    #region Vectors

    /// <summary> This <typeparamref name="TDirection"/> expressed as a <typeparamref name="TVector"/>.
    /// </summary>
    TVector Vector { get; set; }

    #endregion Vectors

    #region Dot

    /// <returns> The dot product of two <typeparamref name="TDirection"/> instances.
    /// </returns>
    static abstract T Dot(in TDirection left, in TDirection right);

    /// <inheritdoc
    /// cref="Dot(in TDirection, in TDirection)"/>.
    T Dot(in TDirection other);

    #endregion Dot
    }
