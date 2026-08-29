namespace Bunnarium.Maths.Primitives;

/// <summary> Defines behaviors common to <see cref="Angle{T}"/> and <see cref="Quaternion{T}"/> so that there is a common, limited API for operations designed to be dimension-agnostic.
/// </summary>
public interface IRotation<TRotation, TDirection, TVector, T> : IEquatable<TRotation>, IPrintable
    where TRotation : unmanaged, IRotation<TRotation, TDirection, TVector, T>
    where TDirection : unmanaged, IDirection<TDirection, TRotation, TVector, T>
    where TVector : unmanaged, IFloatingPointVector<TVector, T>
    where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {

    #region Factories

    /// <returns>The rotational movement that would rotate <typeparamref name="TVector"/> <paramref name="from"/> around the origin to <paramref name="to"/>.
    /// </returns>
    static abstract TRotation FromToRotation(TVector from, TVector to);

    /// <returns>The rotational movement that would rotate <typeparamref name="TDirection"/> <paramref name="from"/> around the origin to <paramref name="to"/>.
    /// </returns>
    static abstract TRotation FromToRotation(TDirection from, TDirection to);


    /// <summary> Creates a <typeparamref name="TRotation"/> representing the rotation needed to orient an object so that its forward axis points towards a target position.
    /// </summary>
    /// <param name="source">The position to look from.</param>
    /// <param name="target">The position to look towards.</param>
    static abstract TRotation FromLookAt(TVector source, TVector target);

    /// <summary> Returns an instance of <typeparamref name="TRotation"/> representing no rotation.
    /// </summary>
    static abstract TRotation Zero{ get; }

    #endregion Factories

    #region Concatenation

    /// <returns> The "sum" of two rotational movements such that the new rotational movement is equivalent to rotating first by <paramref name="left"/>, then by <paramref name="right"/>.
    /// <para/> Note: Rotating first by <paramref name="left"/> and then by <paramref name="right"/> is NOT the same as rotating by <paramref name="right"/> and then by <paramref name="left"/>.
    /// </returns>
    static abstract TRotation Concatenate(TRotation left, TRotation right);

    /// <returns> The "sum" of two rotational movements such that the new rotational movement is equivalent to rotating first by this, then by <paramref name="other"/>.
    /// <para/> Note: Rotating first by this and then by <paramref name="other"/> is NOT the same as rotating by <paramref name="other"/> and then by this.
    /// </returns>
    TRotation Concatenate(TRotation other);

    #endregion Concatenation

    #region Dot

    /// <returns> The dot product of two <typeparamref name="TRotation"/> instances.
    /// </returns>
    static abstract T Dot(in TRotation left, in TRotation right);

    /// <inheritdoc
    /// cref="Dot(in TRotation, in TRotation)"/>.
    T Dot(in TRotation other);

    #endregion Dot

    #region Interpolation

    /// <summary> Linearly interpolates between two rotations. This method blends the initial (<paramref name="from"/>) rotation towards the target (<paramref name="to"/>) rotation by a factor of an <paramref name="amount"/> (between <c>0</c> and <c>1</c>).
    /// </summary>
    /// <inheritdoc cref="DocStrings.Lerp_Amount_Param{TRotation, T}(TRotation, TRotation, T)"/>
    static abstract TRotation Lerp(TRotation from, TRotation to, T amount);

    /// <summary><inheritdoc cref="Lerp(TRotation, TRotation, T)"/>
    /// </summary>
    /// <remarks> This method is a more efficient implementation of <see cref="Lerp(TRotation, TRotation, T)">linear interpolation</see> that  may break down if the input <typeparamref name="TRotation"/> values haven't been <see cref="Normalize(TRotation)">normalized</see>.
    /// </remarks>
    /// <inheritdoc cref="Lerp(TRotation, TRotation, T)"/>
    static abstract TRotation LerpUnchecked(TRotation from, TRotation to, T amount);

    /// <summary> Spherically interpolates between two rotations. This method blends between the <paramref name="from"/> and <paramref name="to"/> rotations with smooth, rotational blending by following the shortest path on the unit sphere with constant angular velocity.
    /// </summary>
    /// <remarks> Depending on the type of <typeparamref name="TRotation"/>, spherical interpolation may be slower than <see cref="Lerp(TRotation, TRotation, T)">linear interpolation</see>, but that slowness will be the tradeoff for greater accuracy.
    /// </remarks>
    /// <inheritdoc cref="Lerp(TRotation, TRotation, T)"/>
    static abstract TRotation Slerp(TRotation from, TRotation to, T amount);

    /// <inheritdoc
    /// cref="Lerp(TRotation, TRotation, T)"/>
    TRotation Lerp(TRotation to, T amount);

    /// <inheritdoc
    /// cref="LerpUnchecked(TRotation, TRotation, T)"/>
    TRotation LerpUnchecked(TRotation to, T amount);

    /// <inheritdoc
    /// cref="Slerp(TRotation, TRotation, T)"/>
    TRotation Slerp(TRotation to, T amount);

    #endregion Interpolation

    #region Inversion

    /// <summary> Returns the rotational movement that represents the same amount of rotation as the input in the <b>opposite</b> direction.
    /// </summary>
    static abstract TRotation Invert(TRotation rotation);

    /// <inheritdoc
    /// cref="Invert(TRotation)"/>
    TRotation Invert();

    /// <summary> Modifies the input rotational movement such that it represents the same <em>amount</em> (i.e., magnitude) of rotation as the input, but in the <b>opposite</b> direction.
    /// </summary>
    static abstract void Invert(ref TRotation rotation);

    #endregion

    #region Negation

    /// <summary> Returns a <typeparamref name="TRotation"/> reflecting the input <paramref name="rotation"/> such that the rotation reaches <u>the same rotational endpoint</u> while moving in the <b>opposite</b> direction.
    /// </summary>
    static abstract TRotation Negate(TRotation rotation);

    /// <summary> Modifies the input rotational movement such that the <paramref name="rotation"/> reaches <u>the same rotational endpoint</u> while moving in the <b>opposite</b> direction.
    /// </summary>
    static abstract void Negate(ref TRotation rotation);

    /// <summary> Returns a rotation reflecting the input rotation such that the rotation reaches <u>the same rotational endpoint</u> while moving in the <b>opposite</b> direction.
    /// </summary>
    TRotation Negate();

    #endregion Negation

    #region Normalization

    /// <inheritdoc
    /// cref="Normalize(TRotation)"/>
    static abstract void Normalize(ref TRotation rotation);

    /// <summary> Normalizes the input <paramref name="rotation"/>, ensuring it has a unit magnitude or a magnitude that doesn't exceed a one full rotation.
    /// </summary>
    static abstract TRotation Normalize(TRotation rotation);

    /// <inheritdoc
    /// cref="Normalize(TRotation)"/>
    TRotation Normalize();

    #endregion Normalization

    #region Operators, Scale and Equality

    /// <inheritdoc
    /// cref="Concatenate(TRotation, TRotation)"/>
    static abstract TRotation Add(TRotation left, TRotation right);

    /// <summary> Equivalent to <see cref="Concatenate(TRotation, TRotation)">concatenating</see> <typeparamref name="TRotation"/> <paramref name="left"/> with the <see cref="Negate(TRotation)"> negation</see> of <paramref name="right"/>.
    ///  <para/> Note: Rotating first by <paramref name="left"/> and then by the negation of <paramref name="right"/> is <b><u>not</u></b> the same as rotating by the negation of <paramref name="right"/> and then by <paramref name="left"/>.
    /// </summary>
    static abstract TRotation Divide(TRotation left, TRotation right);

    /// <summary> Returns the <typeparamref name="TRotation"/> with the relative reciprocal <paramref name="amount"/> of rotation interpolated/extrapolated.
    /// </summary>
    static abstract TRotation Divide(TRotation rotation, T amount);

    /// <inheritdoc
    /// cref="Concatenate(TRotation, TRotation)"/>
    static abstract TRotation Multiply(TRotation left, TRotation right);

    /// <summary> Returns the <typeparamref name="TRotation"/> with the relative <paramref name="amount"/> of rotation interpolated/extrapolated.
    /// </summary>
    static abstract TRotation Multiply(TRotation rotation, T amount);

    /// <summary> Returns <see langword="true"/> when two <typeparamref name="TRotation"/> do not represent equivalent rotational movements, <see langword="false"/> if otherwise.
    /// </summary>
    static abstract bool operator !=(TRotation left, TRotation right);

    /// <inheritdoc
    /// cref="Concatenate(TRotation, TRotation)"/>
    static abstract TRotation operator *(TRotation left, TRotation right);

    /// <inheritdoc
    /// cref="Multiply(TRotation, T)"/>
    static abstract TRotation operator *(TRotation rotation, T amount);

    /// <inheritdoc
    /// cref="Divide(TRotation, TRotation)"/>
    static abstract TRotation operator /(TRotation left, TRotation right);

    /// <inheritdoc
    /// cref="Divide(TRotation, T)"/>
    static abstract TRotation operator /(TRotation rotation, T amount);

    /// <summary> Returns <see langword="true"/> when two <typeparamref name="TRotation"/> represent equivalent rotational movements, <see langword="false"/> if otherwise.
    /// </summary>
    static abstract bool operator ==(TRotation left, TRotation right);

    /// <inheritdoc
    /// cref="Divide(TRotation, TRotation)"/>
    static abstract TRotation Subtract(TRotation left, TRotation right);

    /// <inheritdoc
    /// cref="Concatenate(TRotation)"/>
    TRotation Add(TRotation other);

    /// <inheritdoc
    /// cref="Divide(TRotation, T)"/>
    TRotation Divide(T amount);

    /// <summary> Equivalent to <see cref="Concatenate(TRotation, TRotation)">concatenating</see> this <typeparamref name="TRotation"/> with the <see cref="Negate(TRotation)">negation</see> of the <paramref name="other"/> <typeparamref name="TRotation"/>.
    /// </summary>
    /// <returns> The "difference" of two rotational movements such that the new rotational movement is equivalent to rotating first by this, then by the <see cref="Negate()">negation</see> of <paramref name="other"/>.
    /// <para/> Note: Rotating first by this and then by the negation of <paramref name="other"/> is NOT the same as rotating by the negation of <paramref name="other"/> and then by this.
    /// </returns>
    TRotation Divide(TRotation other);

    /// <inheritdoc
    /// cref="Multiply(TRotation, T)"/>
    TRotation Multiply(T amount);

    /// <inheritdoc
    /// cref="Concatenate(TRotation)"/>
    TRotation Multiply(TRotation other);

    /// <inheritdoc
    /// cref="Divide(TRotation)"/>
    TRotation Subtract(TRotation other);

    #endregion Operators, Scale and Equality

    #region Rotation

    /// <summary> Returns the number of full rotations this <typeparamref name="TRotation"/> represents.
    /// </summary>
    T RoundTrips { get; }

    /// <summary> Returns the <paramref name="direction"/> that the input <paramref name="direction"/> would face after being rotated by a given <paramref name="rotation"/>al movement.
    /// </summary>
    static abstract TDirection Rotate(TDirection direction, TRotation rotation);

    /// <summary> Rotates the given <paramref name="direction"/> in-place with the given <paramref name="rotation"/>.
    /// </summary>
    static abstract void Rotate(ref TDirection direction, TRotation rotation);

    /// <summary><inheritdoc cref="Rotate(TDirection, TRotation)"/>
    /// </summary>
    /// <remarks> The <paramref name="vector"/> is rotated around the origin  (<typeparamref name="TVector"/>.<see cref="IVector{TVector, T}.Zero">Zero</see>).
    /// </remarks>
    static abstract TVector Rotate(TVector vector, TRotation rotation);

    /// <inheritdoc
    /// cref="Rotate(TDirection, TRotation)"/>
    TDirection Rotate(TDirection direction);

    /// <inheritdoc
    /// cref="Rotate(TVector, TRotation)"/>
    TVector Rotate(TVector vector);

    #endregion Rotation

    #region Vectors

    /// <summary> This <typeparamref name="TRotation"/>'s data, interpreted as a <typeparamref name="TVector"/>.
    /// </summary>
    TVector Vector { get; }

    #endregion Vectors
    }
