namespace Bunnarium.Maths.Primitives;


/// <summary> This interface signals that <typeparamref name="TWrapped"/> is an <see langword="unmanaged"/> type that wraps a single instance of a <typeparamref name="TVector"/>.
/// </summary>
public interface IVectorWrapper<TWrapped, TVector, T>
    where TWrapped : unmanaged, IVectorWrapper<TWrapped, TVector, T>
    where TVector : unmanaged, IVector<TVector, T>
    where T : unmanaged, INumberBase<T>, IMinMaxValue<T>
    {

    /// <summary> The number of elements in the vector that this type represents.
    /// </summary>
    static abstract int VectorLength { get; }

    /// <returns> This <typeparamref name="TWrapped"/> represented as its underlying vector type.
    /// </returns>
    TVector UnwrapVector { get; }

    /// <returns> A <typeparamref name="TWrapped"/> cast as its underlying vector type.
    /// </returns>
    static abstract ref TVector AsVector(ref TWrapped wrapped);

    /// <returns> A <typeparamref name="TVector"/> cast as a <typeparamref name="TWrapped"/>.
    /// </returns>
    static abstract ref TWrapped AsWrapper(ref TVector vector);

    /// <returns> A newly constructed <typeparamref name="TWrapped"/> created directly from a <typeparamref name="TVector"/>.
    /// </returns>
    static abstract TWrapped FromVector(TVector vector);
    }
