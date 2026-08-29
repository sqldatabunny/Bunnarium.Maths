namespace Bunnarium.Maths.Primitives;

/// <summary> Represents a vector where the components' value types are integers.
/// </summary>
public interface IIntegralVector<TVector, T> : IVector<TVector, T>
    where TVector : unmanaged, IIntegralVector<TVector, T>
    where T : unmanaged, IBinaryInteger<T>, IMinMaxValue<T> {

    /// <summary> Interpreting a <typeparamref name="TVector"/> coordinate in a grid with a given set of <paramref name="dimensions"/>, returns whether the input <paramref name="position"/> sits at a corner of that grid.
    /// <para/><b>Technical definition:</b> all vector components are at <c>(0 | Max)</c>
    /// </summary>
    static abstract bool IsCornerOf(TVector position, TVector dimensions);

    /// <inheritdoc
    /// cref="IsCornerOf(TVector, TVector)"/>
    bool IsCornerOf(TVector dimensions);

    /// <summary> Interpreting a <typeparamref name="TVector"/> coordinate in a grid with a given set of <paramref name="dimensions"/>, returns whether the input <paramref name="position"/> sits at an edge (including corners) of that grid.
    /// <para/><b>Technical definition:</b> <c>(n >= dimensions-1)</c> vector components are at <c>(0 | Max)</c>
    /// </summary>
    static abstract bool IsOnEdgeOf(TVector position, TVector dimensions);

    /// <inheritdoc
    /// cref="IsOnEdgeOf(TVector, TVector)"/>
    bool IsOnEdgeOf(TVector dimensions);

    /// <summary> Interpreting a <typeparamref name="TVector"/> as a coordinate in a grid with a given set of <paramref name="dimensions"/>, returns whether the input <paramref name="position"/> sits at an edge (excluding corners) of that grid.
    /// <para/><b>Technical definition:</b> <c>exactly (dimensions-1)</c> vector components are at <c>(0 | Max)</c>
    /// </summary>
    static abstract bool IsOnEdgeButNotCornerOf(TVector position, TVector dimensions);

    /// <inheritdoc
    /// cref="IsOnEdgeButNotCornerOf(TVector, TVector)"/>
    bool IsOnEdgeButNotCornerOf(TVector dimensions);

    /// <summary> Interpreting a <typeparamref name="TVector"/> as a coordinate in a grid with a given set of <paramref name="dimensions"/>, returns whether the input <paramref name="position"/> sits at the surface / hull of that grid.
    /// <para/><b>Technical definition:</b> <c>n >= (dimensions-2)</c> vector components are at <c>(0 | Max)</c>
    /// </summary>
    static abstract bool IsOnSurfaceOf(TVector position, TVector dimensions);

    /// <inheritdoc
    /// cref="IsOnSurfaceOf(TVector, TVector)"/>
    bool IsOnSurfaceOf(TVector dimensions);

    /// <summary> Interpreting a <typeparamref name="TVector"/> as a coordinate in a grid with a given set of <paramref name="dimensions"/>, returns whether the input <paramref name="position"/> sits at the surface / hull of that grid, excluding edges.
    /// <para/><b>Technical definition:</b> exactly <c>(dimensions-2)</c> vector components are at <c>(0 | Max)</c>
    /// </summary>
    static abstract bool IsOnSurfaceButNotEdgeOf(TVector position, TVector dimensions);

    /// <inheritdoc
    /// cref="IsOnSurfaceButNotEdgeOf(TVector, TVector)"/>
    bool IsOnSurfaceButNotEdgeOf(TVector dimensions);

    /// <summary> Interpreting a <typeparamref name="TVector"/> as a coordinate in a grid with a given set of <paramref name="dimensions"/>, returns whether the input <paramref name="position"/> sits at the surface / hull of that grid, including corners, faces, and edges.
    /// <para/><b>Technical definition:</b> at least one vector component is at <c>(0 | Max)</c>
    /// </summary>
    static abstract bool IsOnOutskirtsOf(TVector position, TVector dimensions);

    /// <inheritdoc
    /// cref="IsOnOutskirtsOf(TVector, TVector)"/>
    bool IsOnOutskirtsOf(TVector dimensions);

    /// <summary> Returns all combinations of vectors representing coordinate positions in a grid of size <paramref name="dimensions"/>.
    /// </summary>
    static abstract IEnumerable<TVector> GetCartesianProduct(TVector dimensions);

    /// <summary> Returns all combinations of vectors representing coordinate positions in a grid with dimension lengths equal to this vector's respective component values.
    /// </summary>
    IEnumerable<TVector> GetCartesianProduct();

    }
