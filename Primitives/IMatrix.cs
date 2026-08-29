namespace Bunnarium.Maths.Primitives;

/// <summary> Defines the foundational structure and operations for all matrix types. Most concrete matrix implementations should inherit from <see cref="ISquareMatrix{Matrix}">ISquareMatrix</see> for square matrices or <see cref="ITranslationMatrix{Matrix, Vector, Direction, Rotation}">ITranslationMatrix</see> for transformation matrices, rather than implementing this interface directly. <see cref="ISquareMatrix{Matrix}">ISquareMatrix</see> and <see cref="ITranslationMatrix{Matrix, Vector, Direction, Rotation}">ITranslationMatrix</see> are the interfaces intended to be inherited directly by typical matrices (e.g, 2x3, 4x4).
/// </summary>
/// <remarks>
/// Matrices are always stored in-memory in row-major order, but <see cref="Matrix.Docs.HandednessInstructions{T}">handed coordinate systems</see> and <see cref="Matrix.Docs.MultiplicationConventionInstructions{T}">transformation style conventions</see> are selected via compiler symbols.
/// <para/>
/// <inheritdoc cref="Matrix.Docs.DisplayMatrix3"/>
/// </remarks>
/// <typeparam name="Numeric"> The numeric type of the matrice's elements (e.g., <see cref="float"/> or <see cref="double"/>).</typeparam>
public partial interface IMatrix<Numeric>
    : IPrintable
   where Numeric
    : unmanaged
    , IMinMaxValue<Numeric>
    , IBinaryFloatingPointIeee754<Numeric> {

    /// <summary> The precision that matrix values are rounded to by default when output into strings.
    /// </summary>
    public const int DefaultRounding = 4;

    /// <summary> The height (number of rows) in this matrix type.
    /// </summary>
    static abstract int MatrixHeight { get; }

    /// <summary> The width (number of columns) in this matrix type.
    /// </summary>
    static abstract int MatrixWidth { get; }

    /// <summary> The height (number of rows) in this matrix.
    /// </summary>
    byte Height { get; }

    /// <summary> The width (number of columns) in this matrix.
    /// </summary>
    byte Width { get; }

    /// <summary> Copies contents into a 2D array and returns it. Elements are copied in column-major order: for each column from left to right, all rows from top to bottom are copied sequentially.
    /// </summary>
    Numeric[,] ColumnsTo2DArray();

    /// <summary> Copies contents into a 2D jagged array and returns it. Elements are copied in column-major order: for each column from left to right, all rows from top to bottom are copied sequentially.
    /// </summary>
    Numeric[][] ColumnsToJaggedArray();

    /// <summary> Copies contents into a 2D array and returns it. Elements are copied in row-major order: for each row from top to bottom, all columns from left to right are copied sequentially.
    /// </summary>
    Numeric[,] RowsTo2DArray();

    /// <summary> Copies contents into a 2D jagged array and returns it. Elements are copied in row-major order: for each row from top to bottom, all columns from left to right are copied sequentially.
    /// </summary>
    Numeric[][] RowsToJaggedArray();

    }
