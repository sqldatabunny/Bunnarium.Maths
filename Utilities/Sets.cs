using System.Collections;
using System.Runtime.CompilerServices;

namespace Bunnarium.Maths.Utilities;

/// <summary> A library of functions pertaining to sets.
/// </summary>
public static class Sets {

    #region Cartesian

    /// <summary> Represents the set of all combinations of positive integers in the range [0, <paramref name="Dimensions"/>).
    /// </summary>
    public readonly struct CartesianProduct<TVector, T>(TVector Dimensions)
        : IEnumerable<TVector>
        where TVector : unmanaged, IIntegralVector<TVector, T>
        where T : unmanaged, IBinaryInteger<T>, IMinMaxValue<T> {

        /// <summary> The set's dimensions.
        /// </summary>
        public TVector Dimensions { get; } = Dimensions;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CartesianProductEnumerator GetEnumerator() {
            return new(Dimensions);
            }

        IEnumerator<TVector> IEnumerable<TVector>.GetEnumerator() {
            return GetEnumerator();
            }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
            }

        public struct CartesianProductEnumerator() : IEnumerator<TVector> {
            TVector _dimensions;
            TVector _current;
            bool _isEmpty;
            bool _nextIsFirstSet;

            public unsafe CartesianProductEnumerator(TVector dimensions) : this() {
                var len = TVector.Length;
                T* dims = (T*)Unsafe.AsPointer(ref dimensions);
                _isEmpty = false;
                for (var i = 0; i < len; i++) {
                    if (dims[i] <= T.Zero) {
                        _isEmpty = true;
                        break;
                        }
                    }
                _dimensions = dimensions;
                Reset();
                }

            public readonly TVector Current => _current;

            readonly object IEnumerator.Current => Current;

            public readonly void Dispose() {
                /* do nothing */
                }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public unsafe bool MoveNext() {
                if (_isEmpty)
                    return false;

                // T may not be a signed number, so we need to use this branch to check
                // whether this is the move into the zero vector (first in the set)
                if (_nextIsFirstSet) {
                    _nextIsFirstSet = false;
                    return true;
                    }

                var len = TVector.Length;
                T* curr = (T*)Unsafe.AsPointer(ref _current);
                T* dims = (T*)Unsafe.AsPointer(ref _dimensions);

                // advance through components and carry into proceding components when one hits the that dimension's... dimension
                for (var i = 0; i < len; i++) {
                    curr[i]++;
                    if (curr[i] != dims[i])
                        return true;
                    curr[i] = T.Zero;
                    }

                // reached the end of the set, return to the last possible combination
                for (var i = 0; i < len; i++)
                    curr[i] = dims[i] - T.One;
                return false;
                }

            public void Reset() {
                _current = default;
                _nextIsFirstSet = true;
                }
            }
        }

    #endregion Cartesian
    }
