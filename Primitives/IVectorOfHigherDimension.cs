namespace Bunnarium.Maths.Primitives;

/// <summary> Assigned to a type of <see cref="IVector{TVector, T}">IVector</see> to indicate what its next-order vector type is (e.g., that the next-dimensional vector of <see cref="Vector2{T}"/> is <see cref="Vector3{T}"/>).
/// </summary>
public interface IVectorOfHigherDimension<TVector, THyperdimensionalVector, T>
    : IVector<TVector, T>
    where TVector : unmanaged, IVector<TVector, T>, IVectorOfHigherDimension<TVector, THyperdimensionalVector, T>
    where THyperdimensionalVector : unmanaged, IVector<THyperdimensionalVector, T>
    where T : unmanaged, INumberBase<T>, IMinMaxValue<T> {

    /// <inheritdoc
    /// cref="Append(in TVector, T)"/>
    THyperdimensionalVector Append(T value);

    /// <summary> Returns a <typeparamref name="THyperdimensionalVector"/> with the non-final component values of the subject <paramref name="vector"/> and the given <paramref name="value"/> as its final component value.
    /// </summary>
    static abstract THyperdimensionalVector Append(in TVector vector, T value);

    }

