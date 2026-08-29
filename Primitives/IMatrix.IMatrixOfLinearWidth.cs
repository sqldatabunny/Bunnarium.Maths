namespace Bunnarium.Maths.Primitives;

public partial interface IMatrix<Numeric>
    where Numeric : unmanaged, IBinaryFloatingPointIeee754<Numeric>, IMinMaxValue<Numeric> {

    /// <summary> Defines the width of the linear part of a matrix by its corresponding <typeparamref name="Vector"/> type.
    ///  </summary>
    /// <remarks> This interface stores requirements that cannot be satisfied by the same vector type that is bound to <see cref="IMatrixBase{Matrix, Vector, Direction, Rotation}"/> for a given type. As examples, <see cref="Matrix2{T}"/> and <see cref="Matrix2x3{T}"/> have linear widths of 2, <see cref="Matrix3{T}"/> and <see cref="Matrix3x4{T}"/> have linear widths of 3, and <see cref="Matrix4{T}"/> and <see cref="Matrix4x3{T}"/> have linear widths of 4.
    /// </remarks>
    /// <inheritdoc cref="IMatrixBase{Matrix, Vector, Direction, Rotation}"/>
    public interface IMatrixOfLinearWidth<Matrix, Vector>
        : IMatrixBase<Matrix>
        where Matrix : unmanaged, IMatrixOfLinearWidth<Matrix, Vector>, IMatrixBase<Matrix>
        where Vector : unmanaged, IFloatingPointVector<Vector, Numeric> {

        #region Operators & Transformation

        /// <inheritdoc
        /// cref="IMatrixBase{Matrix, Vector, Direction, Rotation}.Transform(in Matrix, Vector)"/>
        static abstract Vector operator *(in Matrix matrix, Vector vector);

        /// <returns> <inheritdoc cref="IMatrixBase{Matrix, Vector, Direction, Rotation}.Transform(in Matrix, Vector)"/>
        /// <para/> If the <paramref name="vector"/> length is not equal to the <paramref name="matrix"/> width, then the missing vector parts will be extended to homogenous coordinates and the result will be divided back by its trailing homoginous coordinate.
        /// </returns>
        static abstract Vector Transform(in Matrix matrix, Vector vector);

        /// <inheritdoc
        /// cref="Transform(in Matrix, Vector)"/>
        Vector Transform(Vector vector);

        #endregion Operators & Transformation
        }
    }
