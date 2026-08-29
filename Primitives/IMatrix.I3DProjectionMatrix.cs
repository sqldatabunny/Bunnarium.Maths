namespace Bunnarium.Maths.Primitives;

public partial interface IMatrix<Numeric>
    where Numeric : unmanaged, IBinaryFloatingPointIeee754<Numeric>, IMinMaxValue<Numeric> {

    /// <summary> Defines functionality for <see cref="Matrix4{T}">4D matrices</see> specific to 3D projection and rendering.
    /// </summary>
    /// <remarks> This interface is intended specifically to be inherited by <see cref="Matrix4{T}"/> and to include functions that call for dimensionally-specific types and parameter sets applicable to 3D simulation.
    /// </remarks>
    public interface I3DProjectionMatrix<Matrix>
        : I3DTranslationMatrix<Matrix>
        , IProjectionMatrix<Matrix, Vector3<Numeric>, Vector4<Numeric>, Direction<Numeric>, Quaternion<Numeric>>
        where Matrix : unmanaged, I3DProjectionMatrix<Matrix> {

        #region Factories - Perspective

        /// <summary> Creates a perspective projection matrix that maps 3D coordinates to 2D screen space with perspective scaling for perspective rendering.
        /// </summary>
        static abstract Matrix CreatePerspective(Numeric width, Numeric height, Numeric nearPlane, Numeric farPlane);

        /// <summary> Creates a perspective projection matrix that maps 3D coordinates to 2D screen space using a specified field-of-view (FOV) and aspect ratio for perspective rendering.
        /// </summary>
        static abstract Matrix CreatePerspectiveFOV(Angle<Numeric> fov, Numeric aspectRatio, Numeric nearPlane, Numeric farPlane);

        /// <summary> Creates a perspective projection matrix that maps 3D coordinates to 2D screen space with custom viewing boundaries for perspective rendering.
        /// </summary>
        static abstract Matrix CreatePerspectiveOffCenter(Numeric left, Numeric right, Numeric top, Numeric bottom, Numeric nearPlane, Numeric farPlane);

        #endregion Factories - Perspective

        #region Factories - Orthographic

        /// <summary> Creates an orthographic projection matrix that maps 3D coordinates to 2D screen space without perspective scaling for orthographic rendering.
        /// </summary>
        static abstract Matrix CreateOrthographic(Numeric width, Numeric height, Numeric nearPlane, Numeric farPlane);

        /// <summary> Creates an orthographic projection matrix that maps 3D coordinates to 2D screen space with custom viewing boundaries for orthographic rendering.
        /// </summary>
        static abstract Matrix CreateOrthographicOffCenter(Numeric left, Numeric right, Numeric top, Numeric bottom, Numeric nearPlane, Numeric farPlane);

        #endregion Factories - Orthographic

        #region Factories - Look To/At

        /// <summary> Creates a view matrix that converts world space coordinates to camera space. This view matrix represents the perspective of a virtual camera by its position (<paramref name="cameraPosition"/>), the position it's pointed at (<paramref name="cameraTarget"/>), and its orientation (as defined by its up direction, i.e., <paramref name="cameraUp"/>).
        /// </summary>
        static abstract Matrix CreateLookAt(Vector3<Numeric> cameraPosition, Vector3<Numeric> cameraTarget, Vector3<Numeric> cameraUp);

        /// <summary> Creates a view matrix that converts world space coordinates to camera space. This view matrix represents the perspective of a virtual camera by its position (<paramref name="cameraPosition"/>), the direction it's pointed in (<paramref name="cameraDirection"/>), and its orientation (as defined by its up direction, i.e., <paramref name="cameraUp"/>).
        /// </summary>
        static abstract Matrix CreateLookTo(Vector3<Numeric> cameraPosition, Vector3<Numeric> cameraDirection, Vector3<Numeric> cameraUp);

        /// <inheritdoc
        /// cref="CreateLookTo(Vector3{Numeric}, Vector3{Numeric}, Vector3{Numeric})"/>
        static abstract Matrix CreateLookTo(Vector3<Numeric> cameraPosition, Direction<Numeric> cameraDirection, Direction<Numeric> cameraUp);

        #endregion Factories - Look To/At

        #region Factories - Shadow / Reflection

        /// <summary> Creates a reflection matrix that mirrors coordinates across a specified <paramref name="plane"/>.
        /// </summary>
        static abstract Matrix CreateReflection(Plane3<Numeric> plane);

        /// <summary> Creates a shadow projection matrix that projects geometry onto a specified <paramref name="plane"/> based on the position of a light source and its target.
        /// </summary>
        static abstract Matrix CreateShadow(Vector3<Numeric> lightSource, Vector3<Numeric> lightTarget, Plane3<Numeric> plane);

        /// <summary> Creates a shadow projection matrix that projects geometry onto a specified <paramref name="plane"/> based on the <paramref name="lightDirection"/>.
        /// </summary>
        static abstract Matrix CreateShadow(Vector3<Numeric> lightDirection, Plane3<Numeric> plane);

        #endregion Factories - Shadow / Reflection
        }
    }
