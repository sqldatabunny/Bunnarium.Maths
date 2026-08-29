namespace Bunnarium.Maths.Utilities;

public interface DocStrings {

    #region Bounding

    /// <returns> A <typeparamref name="T"/> that has a position at this shape's <see cref="IShape{TLine, TVector, T}.Centerpoint">centerpoint</see> and a radius defined by the maximum distance between that centerpoint and this shape's vertices.
    /// </returns>
    public static abstract int BoundingRound<T>();

    #endregion Bounding

    #region Buffering

    /// <summary> Buffers this shape<inheritdoc cref="Buffer_Base"/>
    /// </summary>
    public static abstract void Buffer();

    /// <summary> Buffers a <typeparamref name="TShape"/><inheritdoc cref="Buffer_Base"/>
    /// </summary>
    public static abstract void Buffer_Static<TShape>();

    /// <summary> Creates a copy of this shape, buffered<inheritdoc cref="Buffer_Base"/>
    /// </summary>
    public static abstract void Buffer_Create();

    /// <summary> Creates a copy of a <typeparamref name="TShape"/>, buffered<inheritdoc cref="Buffer_Base"/>
    /// </summary>
    public static abstract void Buffer_Create_Static<TShape>();

    /// <summary> such that all vertices are moved away from the centerpoint by a specified amount.
    /// </summary>
    public const int Buffer_Base = 0;

    #endregion Buffering

    #region Containment

    /// <returns> Whether this primitive completely contains a <typeparamref name="TOther"/>, 
    /// </returns>
    public static abstract int Contains_Other_Instance<TOther>();

    /// <returns> Whether this <typeparamref name="TPrimitive"/> completely contains another, 
    /// </returns>
    public static abstract int Contains_Base_Instance<TPrimitive>();

    /// <returns> Whether this <typeparamref name="TLeft"/> completely contains a <typeparamref name="TRight"/>, 
    /// </returns>
    public static abstract int Contains_Base_Instance<TLeft, TRight>();

    /// <returns> Whether one <typeparamref name="TPrimitive"/> completely contains another, 
    /// </returns>
    public static abstract int Contains_Base_Static<TPrimitive>();

    /// <returns> Whether one <typeparamref name="TLeft"/> completely contains a <typeparamref name="TRight"/>, 
    /// </returns>
    public static abstract int Contains_Base_Static<TLeft, TRight>();

    /// <returns>allowing for a point-edge overlap.
    /// </returns>
    public const int Contains_AllowsAdjacency_Vector = 0;

    /// <returns>prohibiting any point-edge overlap.
    /// </returns>
    public const int Contains_ProhibitsAdjacency_Vector = 0;

    /// <returns>allowing for adjacaent (touching) edges.
    /// </returns>
    public const int Contains_AllowsAdjacency_Primitive = 0;

    /// <returns>prohibiting any adjacent (touching) touching edges.
    /// </returns>
    public const int Contains_ProhibitsAdjacency_Primitive = 0;

    #endregion Containment

    #region Definitions

    /// <summary> Positive angluar values signify counterclockwise rotation.
    /// </summary>
    /// <remarks> Positive angluar values signify counterclockwise rotation.
    /// </remarks>
    public static abstract void Definition_Angles_Positive();

    #endregion Definitions

    #region Intersection

    /// <param name="output">A tuple containing, if the ray intersects the <typeparamref name="TPrimitive"/>, the intersection point and the intersection point's distance from the ray's origin.
    /// </param>
    /// <returns> Whether the <typeparamref name="TRay"/> intersects the <typeparamref name="TPrimitive"/>.
    /// </returns>
    public static abstract int Intersects_Ray_Static<TPrimitive, TRay, TVector, T>(out (TVector Intersection, T Distance) output);

    /// <param name="output">A tuple containing, if the line intersects the <typeparamref name="TPrimitive"/>, the intersection point and the intersection point's distance from the line's origin.
    /// </param>
    /// <returns> Whether the <typeparamref name="TLine"/> intersects the <typeparamref name="TPrimitive"/>.
    /// </returns>
    public static abstract int Intersects_Line_Static<TPrimitive, TLine, TVector, T>(out (TVector Intersection, T Distance) output);

    /// <inheritdoc
    /// cref="Intersects_Ray_Static{TPrimitive, TRay, TVector, T}(out ValueTuple{TVector, T})"/>
    public static abstract int Intersects_Ray_Instance<TPrimitive, TLine, TVector, T>(out (TVector Intersection, T Distance) output);

    /// <inheritdoc
    /// cref="Intersects_Line_Static{TPrimitive, TRay, TVector, T}(out ValueTuple{TVector, T})"/>
    public static abstract int Intersects_Line_Instance<TPrimitive, TLine, TVector, T>(out (TVector Intersection, T Distance) output);

    /// <returns> Whether this <typeparamref name="TLeft"/> intersects a <typeparamref name="TRight"/>, allowing for adjacent (touching) edges.
    /// </returns>
    public static abstract int Intersects_Base_Instance<TLeft, TRight>();

    /// <returns> Whether this <typeparamref name="TPrimitive"/> intersects another, allowing for adjacent (touching) edges.
    /// </returns>
    public static abstract int Intersects_Base_Instance<TPrimitive>();

    /// <returns> Whether one <typeparamref name="TLeft"/> intersects a <typeparamref name="TRight"/>, allowing for adjacent (touching) edges.
    /// </returns>
    public static abstract int Intersects_Base_Static<TLeft, TRight>();

    /// <returns> Whether one <typeparamref name="TPrimitive"/> intersects another, allowing for adjacent (touching) edges.
    /// </returns>
    public static abstract int Intersects_Base_Static<TPrimitive>();

    /// <returns> Whether the <typeparamref name="TLine"/> intersects the <typeparamref name="TPrimitive"/>, allowing for adjacent (touching) edges.
    /// </returns>
    public static abstract int Intersects_Base_Static_Line<TPrimitive, TLine>();

    /// <returns> Whether this primitive intersects a <typeparamref name="TOther"/>, allowing for adjacent (touching) edges.
    /// </returns>
    public static abstract int Intersects_Other_Instance<TOther>();

    /// <returns> Whether this <typeparamref name="TLeft"/> intersects a <typeparamref name="TRight"/>, <b>not</b> allowing for adjacent (touching) edges.
    /// </returns>
    public static abstract int Intersects_ExcludesTouching_Base_Instance<TLeft, TRight>();

    /// <returns> Whether this <typeparamref name="TPrimitive"/> intersects another, <b>not</b> allowing for adjacent (touching) edges.
    /// </returns>
    public static abstract int Intersects_ExcludesTouching_Base_Instance<TPrimitive>();

    /// <returns> Whether one <typeparamref name="TLeft"/> intersects a <typeparamref name="TRight"/>, <b>not</b> allowing for adjacent (touching) edges.
    /// </returns>
    public static abstract int Intersects_ExcludesTouching_Base_Static<TLeft, TRight>();

    /// <returns> Whether one <typeparamref name="TPrimitive"/> intersects another, <b>not</b> allowing for adjacent (touching) edges.
    /// </returns>
    public static abstract int Intersects_ExcludesTouching_Base_Static<TPrimitive>();

    /// <returns> Whether this primitive intersects a <typeparamref name="TOther"/>, <b>not</b> allowing for adjacent (touching) edges.
    /// </returns>
    public static abstract int Intersects_ExcludesTouching_Other_Instance<TOther>();

    #endregion Intersection

    #region Lerp

    /// <param name="amount"> The ratio between <paramref name="to"/> and <paramref name="from"/> to interpolate to. This value is typically within the range <c>[0, 1]</c>, such that <c>0</c> will return <paramref name="from"/>, <c>1</c> will return <paramref name="to"/>, and <c>0.5</c> will return the nearest <typeparamref name="TData"/> that is halfway between them, but values outside of the <c>[0, 1]</c> range are permitted.
    /// </param>
    /// <param name="from">The <typeparamref name="TData"/> to interpolate from.</param>
    /// <param name="to">The <typeparamref name="TData"/> to interpolate to.</param>
    public static abstract void Lerp_Amount_Param<TData, T>(TData from, TData to, T amount);

    #endregion Lerp

    #region Flip / Negation

    /// <remarks> If the underlying numeric type is unsigned, then this will throw an error in <b>debug</b> mode and it will work incorrectly in <b>release</b> mode.
    /// </remarks>
    public static abstract void Disclaimer_Negation_IntegralMayBeUnsigned();

    /// <remarks> The opposite of a direction is such that the sum of a vector of magnitude <em>M</em> pointing in the direction and a vector of magnitude <em>M</em> pointing in the flipped direction is a zero vector.
    /// </remarks>
    public static abstract void Flip_Direction_Remarks();

    #endregion Negation

    #region Normalization

    /// <summary> Normalizes the input <typeparamref name="TData"/> to a unit length, such that it maintains the same direction but its <see cref="IVector{TVector, T}.Magnitude">magnitude</see> is <c>1.0</c>.
    /// </summary>
    public static abstract void Normalize_Summary_Direction<TData>();

    /// <param name="magnitude"> The new length for the input <typeparamref name="TData"/>.
    /// </param>
    public static abstract void Normalize_Magnitude_Param<TData, T>(T magnitude);

    #endregion Normalization

    #region Operators

    /// <returns> The <typeparamref name="TPrimitive"/>, shifted by the <typeparamref name="TVector"/> <inheritdoc cref="OperatorPrimitive_Preserve"/>.
    /// </returns>
    public static abstract int OperatorPrimitive_VectorAddition<TPrimitive, TVector>();

    /// <returns> The <typeparamref name="TPrimitive"/>, shifted by the negation of <typeparamref name="TVector"/> <inheritdoc cref="OperatorPrimitive_Preserve"/>.
    /// </returns>
    public static abstract int OperatorPrimitive_VectorSubtraction<TPrimitive, TVector>();

    /// <returns>while maintaining shape, orientation, and scale
    /// </returns>
    public const int OperatorPrimitive_Preserve = 0;

    /// <returns> Whether the two <typeparamref name="TPrimitive"/> instances are exactly equal-by-value.
    /// </returns>
    public static abstract int OperatorPrimitive_ValueEquality<TPrimitive>();

    /// <returns> Whether the two <typeparamref name="TPrimitive"/> instances are not equal-by-value.
    /// </returns>
    public static abstract int OperatorPrimitive_ValueInequality<TPrimitive>();

    #endregion Operators

    #region Rounding

    /// <remarks> This function is a high-performance, SIMD-implementing function variant that may be used so long as all parameters are passed by-ref.
    /// </remarks>
    public const int Disclaimer_IBoundingPrimitiveByRef = 0;

    /// <summary> Modifies this object <inheritdoc cref="Round_Base"/>
    /// </summary>
    public static abstract void Round();

    /// <summary> Modifies a <typeparamref name="T"/> <inheritdoc cref="Round_Base"/>
    /// </summary>
    public static abstract void Round_Static<T>();

    /// <summary> Returns a copy of this object <inheritdoc cref="Round_Base"/>
    /// </summary>
    public static abstract void Round_Create();

    /// <summary>Returns a copy of a <typeparamref name="T"/> <inheritdoc cref="Round_Base"/>
    /// </summary>
    public static abstract void Round_Create_Static<T>();

    /// <summary> such that all data is rounded to a specified number of digits.
    /// </summary>
    public const int Round_Base = 0;

    /// <summary> Rounds to the nearest integer by default.
    /// </summary>
    public const int Round_ZeroDefault = 0;

    #endregion Rounding

    #region Union

    /// <returns> The result of unioning (merging) two <typeparamref name="TPrimitive"/> instances to create the smallest instance that can fully overlap both.
    /// <para/> If the two <typeparamref name="TPrimitive"/>s have different orientations, then the result will use their orientations' average.
    /// </returns>
    public static abstract int Union<TPrimitive>();

    #endregion Union

    #region Vector Components

    /// <summary> The vector's first component.
    /// </summary>
    public abstract void VectorComponentX();

    /// <summary> The vector's second component.
    /// </summary>
    public abstract void VectorComponentY();

    /// <summary> The vector's third component.
    /// </summary>
    public abstract void VectorComponentZ();

    /// <summary> The vector's fourth component.
    /// </summary>
    public abstract void VectorComponentW();

    #endregion Vector Components
    }
