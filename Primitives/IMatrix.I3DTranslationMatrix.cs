namespace Bunnarium.Maths.Primitives;

public partial interface IMatrix<Numeric>
    where Numeric : unmanaged, IBinaryFloatingPointIeee754<Numeric>, IMinMaxValue<Numeric> {

#if MATRIX_PREMULTIPLIED_CONVENTION
    /// <summary> Defines functionality for matrices relevant to 3D simulation and rendering that have a fourth row applicable to 3D translation and rendering.
    /// </summary>
    /// <remarks> This interface is intended specifically to be inherited by <see cref="Matrix4x3{T}"/> and <see cref="Matrix4{T}"/> and to include functions that call for dimensionally-specific types and parameter sets applicable to 3D simulation.
    /// </remarks>
    /// <inheritdoc cref="IMatrixBase{Matrix, Vector, Direction, Rotation}"/>
#elif MATRIX_POSTMULTIPLIED_CONVENTION
    /// <summary> Defines functionality for matrices relevant to 3D simulation and rendering that have a fourth column applicable to 3D translation and rendering.
    /// </summary>
    /// <remarks> This interface is intended specifically to be inherited by <see cref="Matrix3x4{T}"/> and <see cref="Matrix4{T}"/> and to include functions that call for dimensionally-specific types and parameter sets applicable to 3D simulation.
    /// </remarks>
    /// <inheritdoc cref="IMatrixBase{Matrix, Vector, Direction, Rotation}"/>
#endif
    public interface I3DTranslationMatrix<Matrix>
        : ITranslationMatrix<Matrix, Vector3<Numeric>, Direction<Numeric>, Quaternion<Numeric>>
        , I3DMatrix<Matrix>
        where Matrix : unmanaged, I3DTranslationMatrix<Matrix> {
        }
    }
