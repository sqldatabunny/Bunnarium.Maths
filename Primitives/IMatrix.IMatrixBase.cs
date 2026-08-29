namespace Bunnarium.Maths.Primitives;

public partial interface IMatrix<Numeric>
    where Numeric : unmanaged, IBinaryFloatingPointIeee754<Numeric>, IMinMaxValue<Numeric> {

    /// <summary> Establishes core mathematical functionality for all matrix types.
    ///  </summary>
    ///  <typeparam name="Matrix"> The implemented <see cref="IMatrix{Numeric}">matrix</see> type.</typeparam>
    public interface IMatrixBase<Matrix>
        : IMatrix<Numeric>, IEquatable<Matrix>
        where Matrix : IMatrix<Numeric>, IMatrixBase<Matrix> {

        #region Factories & Constants

        /// <summary> A matrix in which the top-left-to-bottom-right cells are set to 1 and the others to 0.
        /// </summary>
        /// <remarks> Any matrix multiplied by the identity matrix remains unchanged.
        /// </remarks>
        static abstract Matrix Identity { get; }

        #endregion Factories & Constants

        #region Determinants

        /// <summary> A scalar value that indicates whether the matrix is invertible and represents the scale factor of the linear transformation defined by the matrix. A determinant of zero indicates that the matrix is singular and cannot be inverted.
        /// </summary>
        /// <remarks> 📝 On non-square matrices, this value is the determinant of the largest <see cref="ISquareMatrix{Matrix}">square matrix</see> with a width and height equal to the matrix type's smaller dimension.
        /// </remarks>
        Numeric Determinant { get; }

        /// <inheritdoc
        /// cref="Determinant"/>
        static abstract Numeric GetDeterminant(in Matrix matrix);

        #endregion Determinants

        #region Operators & Transformation

        /// <returns> A matrix in which each cell is the difference between the two input matrices.
        /// </returns>
        static abstract Matrix operator -(in Matrix left, in Matrix right);

        /// <inheritdoc
        /// cref="Scale(in Matrix, Numeric)"/>
        static abstract Matrix operator *(in Matrix matrix, Numeric value);

        /// <inheritdoc
        /// cref="Scale(in Matrix, Numeric)"/>
        static abstract Matrix operator *(Numeric value, in Matrix matrix);

        /// <returns> A matrix in which each cell is the sum of the two input matrices.
        /// </returns>
        static abstract Matrix operator +(in Matrix left, in Matrix right);

        /// <returns> A <typeparamref name="Matrix"/> in which each cell is the product of the <paramref name="matrix"/>'s cell and a scalar <paramref name="factor"/>.
        /// </returns>
        static abstract Matrix Scale(in Matrix matrix, Numeric factor);

        /// <inheritdoc
        /// cref="Scale(in Matrix, Numeric)"/>
        Matrix Scale(Numeric factor);

        /// <summary> Equivalent to <see cref="Scale(in Matrix, Numeric)">scale</see> a matrix by 1/<paramref name="value"/>.
        /// </summary>
        static abstract Matrix operator /(in Matrix matrix, Numeric value);

        #endregion Operators & Transformation

        #region Casting

        /// <summary> Unsafely casts a generic matrix to a matrix of a specific numeric without validation. Type coherence will be checked via assertions, if in debug mode.
        /// </summary>
        /// <typeparam name="TMatrix">The type of <see cref="IMatrix{Numeric}"/> to cast to.</typeparam>
        /// <typeparam name="TNumeric">Type of numeric to cast to.</typeparam>
        /// <param name="matrix">The matrix to cast.</param>
        static abstract TMatrix UnsafeCast<TMatrix, TNumeric>(ref Matrix matrix)
            where TMatrix : unmanaged, IMatrix<TNumeric>
            where TNumeric : unmanaged, IBinaryFloatingPointIeee754<TNumeric>, IMinMaxValue<TNumeric>;

        /// <inheritdoc
        /// cref="UnsafeCast{TMatrix, TNumeric}(ref Matrix)"/>
        TMatrix UnsafeCast<TMatrix, TNumeric>()
           where TMatrix : unmanaged, IMatrix<TNumeric>
           where TNumeric : unmanaged, IBinaryFloatingPointIeee754<TNumeric>, IMinMaxValue<TNumeric>;

        #endregion Casting
        }

    /// <summary> Dictates functionality common to all matrices that pertains to their corresponding <see cref="IFloatingPointVector{TVector, T}">vector</see> and <see cref="IRotation{TRotation, TDirection, TVector, T}">rotation</see>.
    ///  </summary>
    ///  <inheritdoc cref="IMatrixBase{Matrix}"/>
    ///  <typeparam name="Matrix"><inheritdoc/></typeparam>
    ///  <typeparam name="Vector"> The matrix's fundamental <see cref="IFloatingPointVector{TVector, T}">vector</see> type whose <see cref="IVector{TVector, T}.Length">Length</see> equals <typeparamref name="Matrix"/>.<see cref="IMatrix{Numeric}.Height">Height</see>.</typeparam>
    ///  <typeparam name="Direction"> The type of the <see cref="IDirection{TDirection, TRotation, TVector, T}">direction</see> representation that is appropriate to its respective <typeparamref name="Vector"/> type.</typeparam>
    ///  <typeparam name="Rotation"> The type of the <see cref="IRotation{TRotation, TDirection, TVector, T}">rotation</see> representation that is appropriate to its respective <typeparamref name="Vector"/> type.</typeparam>
    public interface IMatrixBase<Matrix, Vector, Direction, Rotation>
        : IMatrixBase<Matrix>
        where Matrix : unmanaged, IMatrixBase<Matrix, Vector, Direction, Rotation>, IMatrixBase<Matrix>
        where Vector : unmanaged, IFloatingPointVector<Vector, Numeric>
        where Rotation : unmanaged, IRotation<Rotation, Direction, Vector, Numeric>
        where Direction : unmanaged, IDirection<Direction, Rotation, Vector, Numeric> {

        #region Factories

        /// <summary> Creates a matrix representing the linear transformation that rotates a matrix by a <paramref name="rotation"/>.
        /// </summary>
        static abstract Matrix CreateRotation(Rotation rotation);

        /// <summary> Creates a matrix representing the linear transformation that rotates and scales a matrix by a <paramref name="rotation"/> and <paramref name="scale"/> factor.
        /// </summary>
        static abstract Matrix CreateRotationScale(Rotation rotation, Vector scale);

        /// <summary> Creates a matrix representing the linear transformation that scales a matrix by a <paramref name="scale"/> factor.
        /// </summary>
        static abstract Matrix CreateScale(Vector scale);

        #endregion Factories

        #region Extraction

        /// <summary> Extracts the rotation component from the provided <paramref name="matrix"/>.
        /// </summary>
        static abstract Rotation GetRotation(in Matrix matrix);

        /// <summary> Extracts the scale component from the provided transformation matrix.
        /// </summary>
        static abstract Vector GetScale(in Matrix matrix);

        /// <inheritdoc
        /// cref="GetRotation(in Matrix)"/>
        Rotation GetRotation();

        /// <inheritdoc
        /// cref="GetScale(in Matrix)"/>
        Vector GetScale();

        #endregion Extraction

        #region Operators & Transformation

        /// <summary> Returns the <paramref name="vector"/> that has been linearly-transformed to the input <paramref name="matrix"/>'s coordinate space.
        /// </summary>
        static abstract Vector Transform(in Matrix matrix, Vector vector);

        /// <inheritdoc
        /// cref="Transform(in Matrix, Vector)"/>
        Vector Transform(Vector vector);

        #endregion Operators & Transformation
        }
    }
