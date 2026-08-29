namespace Bunnarium.Maths.Primitives;

public partial interface IMatrix<Numeric>
    where Numeric : unmanaged, IBinaryFloatingPointIeee754<Numeric>, IMinMaxValue<Numeric> {

    /// <summary> For direct inheritance by square-shaped matrices, which is a matrix with equal numbers of rows and columns (e.g., <see cref="Matrix3{T}"/>).
    /// </summary>
    /// <inheritdoc cref="IMatrixBase{Matrix}"/>
    public interface ISquareMatrix<Matrix>
        : IMatrixBase<Matrix>
        where Matrix : ISquareMatrix<Matrix> {

        #region Transposition

        /// <returns> The transpose of the matrix, such that the matrix's rows and columns are swapped.
        /// </returns>
        static abstract Matrix Transpose(in Matrix matrix);

        /// <inheritdoc
        /// cref="Transpose(in Matrix)"/>
        Matrix Transpose();

        #endregion Transposition

        #region Inversion

        /// <inheritdoc
        /// cref="Invert(in Matrix)"/>
        Matrix Invert();

        /// <summary> Returns the inversion of a matrix, such that the input <paramref name="matrix"/> multiplied by its inverse would equal the <see cref="IMatrixBase{Matrix}.Identity">Identity</see> matrix.
        /// <para/> If <c>matrixC = matrixA * matrixB</c>, then <c>matrixA = matrixC * Invert(matrixB)</c>.
        /// </summary>
        static abstract Matrix Invert(in Matrix matrix);

        #endregion Inversion

        #region Operators

        /// <summary> Equivalent to multiplying <paramref name="left"/> by the <see cref="Invert(in Matrix)">inverse</see> of <paramref name="right"/>.
        /// </summary>
        static abstract Matrix Divide(in Matrix left, in Matrix right);

        /// <inheritdoc
        /// cref="Divide(in Matrix, in Matrix)"/>
        static abstract Matrix operator /(in Matrix left, in Matrix right);

        /// <inheritdoc
        /// cref="Divide(in Matrix, in Matrix)"/>
        Matrix Divide(in Matrix other);


        /// <summary> Returns the combination of two matrices, such that the linear transformation represented by the product is equal to the transformation applied by the first followed by the transformation by the second.
        /// </summary>
        static abstract Matrix Multiply(in Matrix left, in Matrix right);

        /// <inheritdoc
        /// cref="Multiply(in Matrix, in Matrix)"/>
        static abstract Matrix operator *(in Matrix left, in Matrix right);

        /// <inheritdoc
        /// cref="Multiply(in Matrix, in Matrix)"/>
        Matrix Multiply(in Matrix other);

        #endregion Operators
        }
    }
