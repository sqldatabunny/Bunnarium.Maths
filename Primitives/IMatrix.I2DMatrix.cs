namespace Bunnarium.Maths.Primitives;

public partial interface IMatrix<Numeric>
    where Numeric : unmanaged, IBinaryFloatingPointIeee754<Numeric>, IMinMaxValue<Numeric> {

    public interface I2DMatrix<Matrix>
    : IMatrixBase<Matrix, Vector2<Numeric>, Angle<Numeric>, Angle<Numeric>>
    where Matrix : unmanaged, I2DMatrix<Matrix> {

        #region Component Removal

        /// <summary> Unscales the matrix, changing its scale to <c>[1, 1]</c> while preserving its <see cref="IMatrixBase{Matrix, Vector, Direction, Rotation}.GetRotation(in Matrix)">Rotation</see> and <see cref="ITranslationMatrix{Matrix, Vector, Direction, Rotation}.GetTranslation(in Matrix)">Translation</see> (if any).
        /// </summary>
        static abstract void Remove2DScale(ref Matrix matrix);

        /// <inheritdoc
        /// cref="Remove2DScale(ref Matrix)"/>
        void Remove2DScale();

        #endregion Component Removal

        }
    }
