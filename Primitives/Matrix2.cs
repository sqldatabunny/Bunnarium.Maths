using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using Debug = System.Diagnostics.Debug;



namespace Bunnarium.Maths.Primitives;

/// <summary> A matrix with two rows and two columns.
/// </summary>
/// <remarks><inheritdoc cref="Matrix.Docs.DisplayMatrix2"/>
/// </remarks>
[DebuggerDisplay("{ToString(), nq}")]
public struct Matrix2<T>
    : IMatrix<T>.ISquareMatrix<Matrix2<T>>
    , IMatrix<T>.IMatrixBase<Matrix2<T>, Vector2<T>, Angle<T>, Angle<T>>
    , IMatrix<T>.IMatrixOfLinearWidth<Matrix2<T>, Vector2<T>>
    , IMatrix<T>.I2DMatrix<Matrix2<T>>
    where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {

    #region Data

    private T _a1, _a2, _b1, _b2;

    #endregion Data

    #region Cells

    /// <summary> The first element of the first row.
    /// </summary>
    public T A1 { readonly get => _a1; set => _a1 = value; }

    /// <summary> The second element of the first row.
    /// </summary>
    public T A2 { readonly get => _a2; set => _a2 = value; }

    /// <summary> The first element of the second row.
    /// </summary>
    public T B1 { readonly get => _b1; set => _b1 = value; }

    /// <summary> The second element of the second row.
    /// </summary>
    public T B2 { readonly get => _b2; set => _b2 = value; }

    #endregion Cells

    #region Constructors

    /// <inheritdoc
    /// cref="Matrix4{T}.Matrix4(T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T)"/>
    public Matrix2(
        T a1, T a2,
        T b1, T b2) {
        _a1 = a1; _a2 = a2;
        _b1 = b1; _b2 = b2;
        }

    /// <summary> Creates a matrix from vector-based rows.
    /// </summary>
    public Matrix2(Vector2<T> rowA, Vector2<T> rowB) : this(
        rowA.X, rowA.Y,
        rowB.X, rowB.Y
        ) { }

    /// <summary> Creates a <see cref="Matrix2{T}"/> from a <see cref="Matrix2x3{T}"/>, with the latter's third column omitted.
    /// </summary>
    public Matrix2(Matrix2x3<T> matrix) {
        _a1 = matrix.A1; _a2 = matrix.A2;
        _b1 = matrix.B1; _b2 = matrix.B2;
        }

    /// <summary> Creates a <see cref="Matrix2{T}"/> from a <see cref="Matrix3x2{T}"/>, with the latter's third row omitted.
    /// </summary>
    public Matrix2(Matrix3x2<T> matrix) {
        _a1 = matrix.A1; _a2 = matrix.A2;
        _b1 = matrix.B1; _b2 = matrix.B2;
        }

    /// <summary> Creates a <see cref="Matrix2{T}"/> from a <see cref="Matrix3{T}"/>, with the latter's third row and column omitted.
    /// </summary>
    public Matrix2(Matrix3<T> matrix) {
        _a1 = matrix.A1; _a2 = matrix.A2;
        _b1 = matrix.B1; _b2 = matrix.B2;
        }

    #endregion Constructors

    #region Factories

    public static Matrix2<T> Identity { get; } = new(
        +T.One, T.Zero,
        T.Zero, +T.One
        );

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix2<T> CreateScale(Vector2<T> scale) {
        Matrix.CreateScale(scale, out Matrix2<T> matrix);
        return matrix;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix2<T> CreateRotation(Angle<T> rotation) {
        Matrix.CreateRotation(rotation, out Matrix2<T> matrix);
        return matrix;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix2<T> CreateRotationScale(Angle<T> rotation, Vector2<T> scale) {
        Matrix.CreateRotationScale(rotation, scale, out Matrix2<T> matrix);
        return matrix;
        }

    #endregion Factories

    #region Rows & Columns


    /// <summary> Returns <see cref="Matrix2{T}.RowA"/> of the <paramref name="matrix"/> by <see langword="ref"/>.
    /// </summary>
    public static ref Vector2<T> GetRowA(ref Matrix2<T> matrix) {
        return ref Unsafe.As<T, Vector2<T>>(ref matrix._a1);
        }

    /// <summary> Returns <see cref="Matrix2{T}.RowB"/> of the <paramref name="matrix"/> by <see langword="ref"/>.
    /// </summary>
    public static ref Vector2<T> GetRowB(ref Matrix2<T> matrix) {
        return ref Unsafe.As<T, Vector2<T>>(ref matrix._b1);
        }

    public Vector2<T> Column1 {
        readonly get => new(A1, B1);
        set {
            A1 = value.X;
            B1 = value.Y;
            }
        }
    public Vector2<T> Column2 {
        readonly get => new(A2, B2);
        set {
            A2 = value.X;
            B2 = value.Y;
            }
        }
    public unsafe Vector2<T> RowA {
        readonly get => new(A1, A2);
        set => Unsafe.Copy(Unsafe.AsPointer(ref _a1), ref value);
        }
    public unsafe Vector2<T> RowB {
        readonly get => new(B1, B2);
        set => Unsafe.Copy(Unsafe.AsPointer(ref _b1), ref value);
        }
    public readonly byte Width => 2;
    public readonly byte Height => 2;

    public static int MatrixWidth { get; } = 2;
    public static int MatrixHeight { get; } = 2;

    #endregion Rows & Columns

    #region Inversion

    [BunnyAttributes.SIMDCandidate]
    public readonly Matrix2<T> Invert() {
        var det = Determinant;
        if (T.Abs(det) < T.Epsilon)
            return new(
                T.NaN, T.NaN,
                T.NaN, T.NaN
                );
        return new Matrix2<T>(
            +B2, -A2,
            -B1, +A1
            ) / det;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix2<T> Invert(in Matrix2<T> matrix) {
        return matrix.Invert();
        }

    #endregion Inversion

    #region Determinants

    public readonly T Determinant {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => GetDeterminant(in this);
        }

    public static T GetDeterminant(in Matrix2<T> matrix) {
        return matrix.A1 * matrix.B2 - matrix.A2 * matrix.B1;
        }

    #endregion Determinants

    #region Transposition

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Matrix2<T> Transpose() {
        return Transpose(in this);
        }

    [BunnyAttributes.SIMDCandidate]
    public static Matrix2<T> Transpose(in Matrix2<T> matrix) {
        return new(
            matrix.A1, matrix.B1,
            matrix.A2, matrix.B2
            );
        }

    #endregion Transposition

    #region Transformation

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Vector2<T> Transform(Vector2<T> vector) {
        return Transform(this, vector);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2<T> Transform(in Matrix2<T> matrix, Vector2<T> vector) {
        Matrix.Transform(matrix, vector, out Vector2<T> ret);
        return ret;
        }

    #endregion Transformation

    #region Extraction

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Vector2<T> GetScale() {
        return GetScale(in this);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Angle<T> GetRotation() {
        return GetRotation(in this);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2<T> GetScale(in Matrix2<T> matrix) {
        Matrix.GetScale(in matrix, out Vector2<T> scale);
        return scale;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Angle<T> GetRotation(in Matrix2<T> matrix) {
        Matrix.GetRotation(in matrix, out Angle<T> angle);
        return angle;
        }

    #endregion Extraction

    #region Component Removal

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Remove2DScale(ref Matrix2<T> matrix) {
        Matrix.Remove2DScale(ref matrix);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Remove2DScale() {
        Matrix.Remove2DScale(ref this);
        }

    #endregion Component Removal

    #region Operators

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix2<T> operator *(T factor, in Matrix2<T> matrix) {
        return Scale(in matrix, factor);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Matrix2<T> Multiply(in Matrix2<T> other) {
        return Multiply(this, other);
        }

    [BunnyAttributes.SIMDCandidate]
    public static Matrix2<T> Multiply(in Matrix2<T> left, in Matrix2<T> right) {
        return new(
            left.A1 * right.A1 + left.A2 * right.B1,
            left.A1 * right.A2 + left.A2 * right.B2,
            left.B1 * right.A1 + left.B2 * right.B1,
            left.B1 * right.A2 + left.B2 * right.B2
            );
        }

    [BunnyAttributes.SIMDCandidate]
    public static Matrix2<T> Scale(in Matrix2<T> matrix, T factor) {
        return new(
            matrix.A1 * factor, matrix.A2 * factor,
            matrix.B1 * factor, matrix.B2 * factor
            );
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Matrix2<T> Scale(T factor) {
        return Scale(this, factor);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix2<T> operator +(in Matrix2<T> left, in Matrix2<T> right) {
        return Add(left, right);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Matrix2<T> Add(in Matrix2<T> other) {
        return Add(this, other);
        }

    [BunnyAttributes.SIMDCandidate]
    public static Matrix2<T> Add(in Matrix2<T> left, in Matrix2<T> right) {
        return new(
            left.A1 + right.A1, left.A2 + right.A2,
            left.B1 + right.B1, left.B2 + right.B2
            );
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Matrix2<T> Subtract(in Matrix2<T> other) {
        return Subtract(this, other);
        }

    [BunnyAttributes.SIMDCandidate]
    public static Matrix2<T> Subtract(in Matrix2<T> left, in Matrix2<T> right) {
        return new(
            left.A1 - right.A1, left.A2 - right.A2,
            left.B1 - right.B1, left.B2 - right.B2
            );
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix2<T> operator -(in Matrix2<T> left, in Matrix2<T> right) {
        return Subtract(left, right);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix2<T> operator *(in Matrix2<T> left, in Matrix2<T> right) {
        return Multiply(left, right);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix2<T> operator /(in Matrix2<T> left, in Matrix2<T> right) {
        return Divide(in left, in right);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix2<T> Divide(in Matrix2<T> left, in Matrix2<T> right) {
        return Multiply(left, Invert(right));
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Matrix2<T> Divide(in Matrix2<T> other) {
        return Divide(in this, in other);
        }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix2<T> operator *(in Matrix2<T> matrix, T value) {
        return Scale(matrix, value);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix2<T> operator /(in Matrix2<T> matrix, T value) {
        return Scale(matrix, T.One / value);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2<T> operator *(in Matrix2<T> matrix, Vector2<T> vector) {
        return Transform(matrix, vector);
        }


    #endregion Operators

    #region Equatability

    public bool Equals(Matrix2<T> other) {
        if (Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double)) {
            var v0 = Unsafe.As<T, Vector256<double>>(ref _a1);
            var v1 = Unsafe.As<T, Vector256<double>>(ref other._a1);
            return Vector256.EqualsAll(v0, v1);
            }
        else if (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float)) {
            var v0 = Unsafe.As<T, Vector128<float>>(ref _a1);
            var v1 = Unsafe.As<T, Vector128<float>>(ref other._a1);
            return Vector128.EqualsAll(v0, v1);
            }
        else {
            return A1 == other.A1 && A2 == other.A2
                && B1 == other.B1 && B2 == other.B2;
            }
        }

    public override bool Equals(object? obj) {
        return obj is Matrix2<T> matrix && Equals(matrix);
        }

    public override readonly int GetHashCode() {
        return HashCode.Combine(RowA.GetHashCode(), RowB.GetHashCode());
        }

    public static bool operator ==(Matrix2<T> left, Matrix2<T> right) {
        return left.Equals(right);
        }

    public static bool operator !=(Matrix2<T> left, Matrix2<T> right) {
        return left.Equals(right) == false;
        }

    #endregion Equatability

    #region Strings

    public override readonly string ToString() {
        return ToString(IMatrix<T>.DefaultRounding, 0, 0);
        }

    public readonly string ToString(byte digits, int integerLength, int paddingLength) {
        var a1 = A1.Stringify(digits, integerLength, paddingLength);
        var a2 = A2.Stringify(digits, integerLength, paddingLength);
        var b1 = B1.Stringify(digits, integerLength, paddingLength);
        var b2 = B2.Stringify(digits, integerLength, paddingLength);

        return $"[ [{a1}, {a2}],\n  [{b1}, {b2}] ]";
        }

    #endregion Strings

    #region Format Data

    public readonly T[][] RowsToJaggedArray() {
        return [[A1, A2], [B1, B2]];
        }

    public readonly T[][] ColumnsToJaggedArray() {
        return [[A1, B1], [A2, B2]];
        }

    public readonly T[,] RowsTo2DArray() {
        return new T[,] { { A1, A2 }, { B1, B2 } };
        }
    public readonly T[,] ColumnsTo2DArray() {
        return new T[,] { { A1, B1 }, { A2, B2 } };
        }

    #endregion Format Data

    #region Casting

    public static TMatrix UnsafeCast<TMatrix, TNumeric>(ref Matrix2<T> matrix)
        where TMatrix : unmanaged, IMatrix<TNumeric>
        where TNumeric : unmanaged, IBinaryFloatingPointIeee754<TNumeric>, IMinMaxValue<TNumeric> {
        Debug.Assert((typeof(TNumeric) == typeof(T)) && (Matrix2<T>.MatrixWidth == TMatrix.MatrixWidth) && (Matrix2<T>.MatrixHeight == TMatrix.MatrixHeight));
        return Unsafe.As<Matrix2<T>, TMatrix>(ref matrix);
        }

    public TMatrix UnsafeCast<TMatrix, TNumeric>()
        where TMatrix : unmanaged, IMatrix<TNumeric>
        where TNumeric : unmanaged, IBinaryFloatingPointIeee754<TNumeric>, IMinMaxValue<TNumeric> {
        Debug.Assert((typeof(TNumeric) == typeof(T)) && (Matrix2<T>.MatrixWidth == TMatrix.MatrixWidth) && (Matrix2<T>.MatrixHeight == TMatrix.MatrixHeight));
        return Unsafe.As<Matrix2<T>, TMatrix>(ref this);
        }

    #endregion Casting

    #region Row Deconstruction

    public readonly void Deconstruct(out Vector2<T> X, out Vector2<T> Y) {
        X = RowA;
        Y = RowB;
        }

    #endregion

    }

