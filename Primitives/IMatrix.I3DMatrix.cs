namespace Bunnarium.Maths.Primitives;

public partial interface IMatrix<Numeric>
    where Numeric : unmanaged, IBinaryFloatingPointIeee754<Numeric>, IMinMaxValue<Numeric> {
#if MATRIX_PREMULTIPLIED_CONVENTION
    /// <summary> Defines functionality for matrices relevant to 3D simulation and rendering.
    /// </summary>
    /// <remarks> This interface is intended specifically to be inherited by <see cref="Matrix3{T}"/>, <see cref="Matrix4x3{T}"/>, and <see cref="Matrix4{T}"/> and to include functions that call for dimensionally-specific types and parameter sets applicable to 3D simulation.
    /// </remarks>
#elif MATRIX_POSTMULTIPLIED_CONVENTION
    /// <summary> Defines functionality for matrices relevant to 3D simulation and rendering.
    /// </summary>
    /// <remarks> This interface is intended specifically to be inherited by <see cref="Matrix3{T}"/>, <see cref="Matrix3x4{T}"/>, and <see cref="Matrix4{T}"/> and to include functions that call for dimensionally-specific types and parameter sets applicable to 3D simulation.
    /// </remarks>
#endif
    public interface I3DMatrix<Matrix>
        : IMatrixBase<Matrix, Vector3<Numeric>, Direction<Numeric>, Quaternion<Numeric>>
        where Matrix : unmanaged, I3DMatrix<Matrix> {

        #region Factories

        /// <summary> Creates a <typeparamref name="Matrix"/> representing a rotational movement around the specified <paramref name="axis"/> by the given <paramref name="angle"/>.
        /// </summary>
        static abstract Matrix CreateAxisAngle(Direction<Numeric> axis, Angle<Numeric> angle);

        /// <summary> Creates a 3D transformation matrix representing rotation around the X axis by a given <paramref name="angle"/>.
        /// </summary>
        static abstract Matrix CreateRotationX(Angle<Numeric> angle);

        /// <summary> Creates a matrix representing extrinsic rotation around the world's X axis, then around the world's Y axis, then around the world's Z axis.
        /// </summary>
        static abstract Matrix CreateRotationXYZ(Angle<Numeric> rotationX, Angle<Numeric> rotationY, Angle<Numeric> rotationZ);

        /// <summary> Creates a matrix representing extrinsic rotation around the world's X axis, then around the world's Z axis, then around the world's Y axis.
        /// </summary>
        static abstract Matrix CreateRotationXZY(Angle<Numeric> rotationX, Angle<Numeric> rotationZ, Angle<Numeric> rotationY);

        /// <summary> Creates a 3D transformation matrix representing rotation around the Y axis by a given <paramref name="angle"/>.
        /// </summary>
        static abstract Matrix CreateRotationY(Angle<Numeric> angle);

        /// <summary> Creates a matrix representing extrinsic rotation around the world's Y axis, then around the world's X axis, then around the world's Z axis.
        /// </summary>
        static abstract Matrix CreateRotationYXZ(Angle<Numeric> rotationY, Angle<Numeric> rotationX, Angle<Numeric> rotationZ);

        /// <summary> Creates a matrix representing extrinsic rotation around the world's Y axis, then around the world's Z axis, then around the world's X axis.
        /// </summary>
        static abstract Matrix CreateRotationYZX(Angle<Numeric> rotationY, Angle<Numeric> rotationZ, Angle<Numeric> rotationX);

        /// <summary> Creates a 3D transformation matrix representing rotation around the Z axis by a given <paramref name="angle"/>.
        /// </summary>
        static abstract Matrix CreateRotationZ(Angle<Numeric> angle);

        /// <summary> Creates a matrix representing extrinsic rotation around the world's Z axis, then around the world's X axis, then around the world's Y axis.
        /// </summary>
        static abstract Matrix CreateRotationZXY(Angle<Numeric> rotationZ, Angle<Numeric> rotationX, Angle<Numeric> rotationY);

        /// <summary> Creates a matrix representing extrinsic rotation around the world's Z axis, then around the world's Y axis, then around the world's X axis.
        /// </summary>
        static abstract Matrix CreateRotationZYX(Angle<Numeric> rotationZ, Angle<Numeric> rotationY, Angle<Numeric> rotationX);

        #endregion Factories

        #region Component Removal

        /// <summary> Unscales the matrix, changing its scale to <c>[1, 1, 1]</c> while preserving its <see cref="IMatrixBase{Matrix, Vector, Direction, Rotation}.GetRotation(in Matrix)">Rotation</see> and <see cref="ITranslationMatrix{Matrix, Vector, Direction, Rotation}.GetTranslation(in Matrix)">Translation</see> (if any).
        /// </summary>
        static abstract void Remove3DScale(ref Matrix matrix);

        /// <inheritdoc
        /// cref="Remove3DScale(ref Matrix)"/>
        void Remove3DScale();

        #endregion Component Removal
        }
    }
