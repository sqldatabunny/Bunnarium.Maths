namespace Bunnarium.Maths.Geometry;


/// <summary> i.e., <see cref="Clockwise">Clockwise</see>, <see cref="Counterclockwise">Counterclockwise</see>, or <see cref="Colinear">Colinear</see>.
/// </summary>
public enum WindingOrder : byte {
    /* DO NOT MODIFY, THERE IS CODE THAT IS HARD-CODED TO THIS ORDER AND TO THESE DEFINITIONS */

    /// <summary> In a three-point sequence of p1, p2 and p3, <b><em>(y₂-y₁)(x₃-x₁) - (x₂-x₁)(y₃-y₁) &lt; 0</em></b>
    /// </summary>
    Counterclockwise = 0,

    /// <summary> In a three-point sequence of p1, p2 and p3, <b><em>(y₂-y₁)(x₃-x₁) - (x₂-x₁)(y₃-y₁) == 0</em></b>
    /// </summary>
    Colinear = 1,

    /// <summary> In a three-point sequence of p1, p2 and p3, <b><em>(y₂-y₁)(x₃-x₁) - (x₂-x₁)(y₃-y₁) &gt; 0</em></b>
    /// </summary>
    Clockwise = 2,

#if MATRIX_LEFTHANDED_COORDINATE_SYSTEM
    /// <inheritdoc cref="Clockwise"/>
    VerticeEnumerationDefault = Clockwise
#else
    /// <inheritdoc cref="Counterclockwise"/>
    VerticeEnumerationDefault = Counterclockwise
#endif
    }
