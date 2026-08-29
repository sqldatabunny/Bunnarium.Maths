namespace Bunnarium.Maths.Primitives;

public partial interface IMatrix<Numeric>
    where Numeric : unmanaged, IBinaryFloatingPointIeee754<Numeric>, IMinMaxValue<Numeric> {

    /// <summary> Represents a matrix that can be used a projection matrix in <typeparamref name="Vector"/>-dimensional space.
    /// <para/> A projection matrix has a <see cref="IMatrix{Numeric}.Height">height</see> of <typeparamref name="HyperdimensionalVector"/>.<see cref="IVector{TVector, T}.Length">Length</see>, which is equal to <c><typeparamref name="Vector"/>.<see cref="IVector{TVector, T}.Length">Length</see> + 1</c>.
    /// </summary>
    /// <typeparam name="Matrix"><inheritdoc/></typeparam>
    /// <typeparam name="Vector"> An <see cref="IFloatingPointVector{TVector, T}">IFloatingPointVector</see> with a length of <c><typeparamref name="Matrix"/>.<see cref="IMatrix{Numeric}.Height">Height</see> - 1</c>, which is equal to <c><typeparamref name="HyperdimensionalVector"/>.<see cref="IVector{TVector, T}.Length">Length</see> - 1</c>.
    /// </typeparam>
    /// <typeparam name="HyperdimensionalVector"> An <see cref="IFloatingPointVector{TVector, T}">IFloatingPointVector</see> with a length of <c><typeparamref name="Matrix"/>.<see cref="IMatrix{Numeric}.Height">Height</see></c>, which is equal to <c><typeparamref name="Vector"/>.<see cref="IVector{TVector, T}.Length">Length</see> + 1</c>.
    /// </typeparam>
    /// <inheritdoc cref="IMatrixBase{Matrix, Vector, Direction, Rotation}"/><!-- This should be at the end of this documentation block to prevent the inherited documentation from overwriting this type's own documentation. -->
    /// <typeparam name="Direction"><inheritdoc/></typeparam>
    /// <typeparam name="Rotation"><inheritdoc/></typeparam>
    public interface IProjectionMatrix<Matrix, Vector, HyperdimensionalVector, Direction, Rotation>
    : IMatrixBase<Matrix, Vector, Direction, Rotation>
    where Matrix : unmanaged, IProjectionMatrix<Matrix, Vector, HyperdimensionalVector, Direction, Rotation>
    where Vector : unmanaged, IFloatingPointVector<Vector, Numeric>, IVectorOfHigherDimension<Vector, HyperdimensionalVector, Numeric>
    where HyperdimensionalVector : unmanaged, IFloatingPointVector<HyperdimensionalVector, Numeric>
    where Direction : unmanaged, IDirection<Direction, Rotation, Vector, Numeric>
    where Rotation : unmanaged, IRotation<Rotation, Direction, Vector, Numeric> {
        }
    }
