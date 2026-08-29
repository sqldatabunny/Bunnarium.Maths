namespace Bunnarium.Maths.Primitives;

public partial interface IMatrix<Numeric>
    where Numeric : unmanaged, IBinaryFloatingPointIeee754<Numeric>, IMinMaxValue<Numeric> {
#if MATRIX_PREMULTIPLIED_CONVENTION
    /// <summary> For direct inheritance by matrices having one more row than columns, e.g., <see cref="Matrix3x2{T}"/> or <see cref="Matrix4x3{T}"/>.
    /// </summary>
    /// <remarks> This interface is meant only for matrices that have one more row than they do columns—it is not meant for just any non-square matrix. The exceptions to this rule are <see cref="IMatrix{Matrix}.ISquareMatrix{T}">square</see> matrices such that the <typeparamref name="Vector"/>, <typeparamref name="Direction"/>, <typeparamref name="Rotation"/> type parameters are bound to n-1 lower-dimensional space types, as these matrices can be evaluated as translation matrices that happen to also have a projection row (e.g., a <see cref="Matrix4{T}"/> being evaluated for 3D translation).
    /// </remarks>
    /// <inheritdoc cref="IMatrixBase{Matrix, Vector, Direction, Rotation}"/>
#elif MATRIX_POSTMULTIPLIED_CONVENTION
    /// <summary> For direct inheritance by matrices having one more column than rows, e.g., <see cref="Matrix2x3{T}"/> or <see cref="Matrix3x4{T}"/>.
    /// </summary>
    /// <remarks> This interface is meant only for matrices that have one more column than they do rows—it is not meant for just any non-square matrix. The exceptions to this rule are <see cref="IMatrix{Matrix}.ISquareMatrix{T}">square</see> matrices such that the <typeparamref name="Vector"/>, <typeparamref name="Direction"/>, <typeparamref name="Rotation"/> type parameters are bound to n-1 lower-dimensional space types, as these matrices can be evaluated as translation matrices that happen to also have a projection row (e.g., a <see cref="Matrix4{T}"/> being evaluated for 3D translation).
    /// </remarks>
    /// <inheritdoc cref="IMatrixBase{Matrix, Vector, Direction, Rotation}"/>
#endif
    public interface ITranslationMatrix<Matrix, Vector, Direction, Rotation>
        : IMatrixBase<Matrix, Vector, Direction, Rotation>
        where Matrix : unmanaged, IMatrixBase<Matrix>, ITranslationMatrix<Matrix, Vector, Direction, Rotation>
        where Vector : unmanaged, IFloatingPointVector<Vector, Numeric>
        where Direction : unmanaged, IDirection<Direction, Rotation, Vector, Numeric>
        where Rotation : unmanaged, IRotation<Rotation, Direction, Vector, Numeric> {

        #region Factories

        /// <summary> Creates a matrix that applies rotation, scaling, and translation transformations. Rotation and scaling are applied before translation.
        /// </summary>
        static abstract Matrix CreateRotationScaleTranslation(Rotation rotation, Vector scale, Vector translation);

        /// <summary> Creates a matrix that combines rotation and translation transformations. Rotation is applied before translation.
        /// </summary>
        static abstract Matrix CreateRotationTranslation(Rotation rotation, Vector translation);

        /// <summary> Creates a matrix that applies both scaling and translation transformations. Scaling is applied before translation.
        /// </summary>
        static abstract Matrix CreateScaleTranslation(Vector scale, Vector translation);

        /// <summary> Creates a translation matrix that moves objects by the specified translation vector.
        /// </summary>
        static abstract Matrix CreateTranslation(Vector translation);

        #endregion Factories

        #region Extraction

        /// <summary> Extracts the translation component from the provided transformation matrix.
        /// </summary>
        static abstract Vector GetTranslation(in Matrix matrix);

        /// <inheritdoc
        /// cref="GetTranslation(in Matrix)"/>
        Vector GetTranslation();

        #endregion Extraction

        #region Transformation

        /// <returns> The <paramref name="vector"/> that has been transformed by the <see cref="ISquareMatrix{Matrix}.Invert(in Matrix)">inverse</see> of the matrix, effectively representing the reversal of the matrix's transformation.
        /// </returns>
        static abstract Vector InvertedTransform(in Matrix matrix, Vector vector);

        /// <inheritdoc
        /// cref="InvertedTransform(in Matrix, Vector)"/>
        Vector InvertedTransform(Vector vector);

        #endregion Transformation

        #region Inversion

        /// <inheritdoc
        /// cref="Invert(in Matrix)"/>
        Matrix Invert();

        /// <summary><inheritdoc cref="ISquareMatrix{Matrix}.Invert(in Matrix)"/>
        /// </summary>
        /// <remarks> Implementations of this method in matrices that inherit <see cref="ITranslationMatrix{Matrix, Vector, Direction, Rotation}">ITranslationMatrix</see> return the same matrix type as the output. Strictly-speaking, the inverse of a matrix of dimensions n*m has the dimensions m*n. Instead, these methods return the inverse of the matrix with implied homogenous coordinates.
        /// </remarks>
        static abstract Matrix Invert(in Matrix matrix);

        #endregion Inversion

        #region Component Removal

        /// <summary> Removes the <see cref="GetTranslation(in Matrix)">Translation</see> component from the <paramref name="matrix"/>.
        /// </summary>
        static abstract void RemoveTranslation(ref Matrix matrix);

        /// <inheritdoc
        /// cref="RemoveTranslation(ref Matrix)"/>
        void RemoveTranslation();

        #endregion Component Removal

        #region Operators

        /// <summary><inheritdoc cref="ISquareMatrix{Matrix}.Divide(in Matrix, in Matrix)"/>
        /// </summary>
        /// <remarks><inheritdoc cref="Invert(in Matrix)"/>
        /// </remarks>
        static abstract Matrix Divide(in Matrix left, in Matrix right);

        /// <inheritdoc
        /// cref="Divide(in Matrix, in Matrix)"/>
        static abstract Matrix operator /(in Matrix left, in Matrix right);

        /// <inheritdoc
        /// cref="Divide(in Matrix, in Matrix)"/>
        Matrix Divide(in Matrix other);

        /// <summary><inheritdoc cref="ISquareMatrix{Matrix}.Multiply(in Matrix, in Matrix)"/>
        /// </summary>
        static abstract Matrix Multiply(in Matrix left, in Matrix right);

        /// <inheritdoc
        /// cref="Multiply(in Matrix, in Matrix)"/>
        /// <remarks> Implementations of this method in matrices that inherit <see cref="ITranslationMatrix{Matrix, Vector, Direction, Rotation}">ITranslationMatrix</see> return the same matrix type as the output. Strictly-speaking, these matrices cannot be multiplied because the <paramref name="left"/> matrix will have a <see cref="IMatrix{T}.Width">width</see> that is unequal to the <see cref="IMatrix{T}.Height">height</see> of the <paramref name="right"/> matrix. These implementations thus work by "squaring" the matrices by assuming homogenous coordinates on both
        /// </remarks>
        static abstract Matrix operator *(in Matrix left, in Matrix right);

        /// <inheritdoc
        /// cref="Multiply(in Matrix, in Matrix)"/>
        Matrix Multiply(in Matrix other);

        #endregion Operators
        }
    }
