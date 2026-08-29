using Bunnarium.Tools;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using Debug = System.Diagnostics.Debug;

namespace Bunnarium.Maths.Primitives;

#if MATRIX_PREMULTIPLIED_CONVENTION
/// <summary> A matrix with two rows and three columns.
/// </summary>
/// <remarks> This matrix does not implement <see cref="IMatrix{Numeric}.ITranslationMatrix{Matrix, Vector, Direction, Rotation}">ITranslationMatrix</see> because the <c>MATRIX_PREMULTIPLIED_CONVENTION</c> compiler symbol is set. Because this is the case, translation-related functions for this type can only be accessed via the <see cref="Matrix"/> function library. For more information, please see <see cref="Matrix.Docs.MultiplicationConventionInstructions{T}"/>.
/// <para/><inheritdoc cref="Matrix.Docs.DisplayMatrix2x3"/>
/// </remarks>
#elif MATRIX_POSTMULTIPLIED_CONVENTION
/// <summary> A matrix with two rows and three columns.
/// </summary>
/// <remarks> This matrix implements <see cref="IMatrix{Numeric}.ITranslationMatrix{Matrix, Vector, Direction, Rotation}">ITranslationMatrix</see> because the <c>MATRIX_POSTMULTIPLIED_CONVENTION</c> compiler symbol is set. For more information, please see <see cref="Matrix.Docs.MultiplicationConventionInstructions{T}"/>.
/// <para/><inheritdoc cref="Matrix.Docs.DisplayMatrix2x3"/>
/// </remarks>
#endif
[DebuggerDisplay("{ToString(), nq}")]
public struct Matrix2x3<T>
    : IMatrix<T>.I2DMatrix<Matrix2x3<T>>
    , IMatrix<T>.IMatrixOfLinearWidth<Matrix2x3<T>, Vector2<T>>
#if MATRIX_POSTMULTIPLIED_CONVENTION
    , IMatrix<T>.ITranslationMatrix<Matrix2x3<T>, Vector2<T>, Angle<T>, Angle<T>>
#endif
    where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {

    #region Data

    private T _a1, _a2, _a3, _b1, _b2, _b3;

    #endregion Data

    #region Cells

    /// <summary> The first element of the first row.
    /// </summary>
    public T A1 { readonly get => _a1; set => _a1 = value; }

    /// <summary> The second element of the first row.
    /// </summary>
    public T A2 { readonly get => _a2; set => _a2 = value; }

    /// <summary> The third element of the first row.
    /// </summary>
    public T A3 { readonly get => _a3; set => _a3 = value; }

    /// <summary> The first element of the second row.
    /// </summary>
    public T B1 { readonly get => _b1; set => _b1 = value; }

    /// <summary> The second element of the second row.
    /// </summary>
    public T B2 { readonly get => _b2; set => _b2 = value; }

    /// <summary> The third element of the second row.
    /// </summary>
    public T B3 { readonly get => _b3; set => _b3 = value; }

    #endregion Cells

    #region Constructors

    /// <inheritdoc
    /// cref="Matrix4{T}.Matrix4(T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T)"/>
    public Matrix2x3(
        T a1, T a2, T a3,
        T b1, T b2, T b3) {
        _a1 = a1; _a2 = a2; _a3 = a3;
        _b1 = b1; _b2 = b2; _b3 = b3;
        }

    /// <summary> Creates a matrix from vector-based rows.
    /// </summary>
    public Matrix2x3(Vector3<T> rowA, Vector3<T> rowB) : this(
        rowA.X, rowA.Y, a3: rowA.Z,
        rowB.X, rowB.Y, b3: rowB.Z
        ) { }

    /// <summary> Creates a new <see cref="Matrix2x3{T}"/> as a <see cref="Matrix2{T}"/> with a third column, set to <c>(0, 0)</c>, appended.
    /// </summary>
    public Matrix2x3(Matrix2<T> matrix) : this(
          matrix.A1, matrix.A2, T.Zero,
          matrix.B1, matrix.B2, T.Zero
          ) { }


    /// <summary> Creates a <see cref="Matrix2x3{T}"/> such that its first two columns originate from the <paramref name="matrix"/> and its third is the passed <paramref name="columnC"/>.
    /// </summary>
    public Matrix2x3(Matrix2<T> matrix, Vector2<T> columnC) : this(
        matrix.A1, matrix.A2, columnC.X,
        matrix.B1, matrix.B2, columnC.Y
        ) { }

    /// <summary> Creates a <see cref="Matrix2x3{T}"/> from a <see cref="Matrix3{T}"/>, with the latter's third row omitted.
    /// </summary>
    public Matrix2x3(Matrix3<T> matrix) : this(
        matrix.A1, matrix.A2, matrix.A3,
        matrix.B1, matrix.B2, matrix.B3
        ) { }

    #endregion Constructors

    #region Factories

    public static Matrix2x3<T> Identity { get; } = new(
        +T.One, T.Zero, T.Zero,
        T.Zero, +T.One, T.Zero
        );

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix2x3<T> CreateScale(Vector2<T> scale) {
        Matrix.CreatePostmultipliedScale(scale, out Matrix2x3<T> matrix);
        return matrix;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix2x3<T> CreateRotation(Angle<T> rotation) {
        Matrix.CreateRotation(rotation, out Matrix2<T> matrix);
        Matrix.UnsafeConvert(in matrix, out Matrix2x3<T> ret);
        return ret;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix2x3<T> CreateRotationScale(Angle<T> rotation, Vector2<T> scale) {
        Matrix.CreatePostmultipliedRotationScale(rotation, scale, out Matrix2x3<T> matrix);
        return matrix;
        }

#if MATRIX_POSTMULTIPLIED_CONVENTION

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix2x3<T> CreateTranslation(Vector2<T> translation) {
        Matrix.CreatePostmultipliedTranslation(translation, out Matrix2x3<T> matrix);
        return matrix;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix2x3<T> CreateScaleTranslation(Vector2<T> scale, Vector2<T> translation) {
        Matrix.CreatePostmultipliedScaleTranslation(scale: scale, translation: translation, out Matrix2x3<T> matrix);
        return matrix;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix2x3<T> CreateRotationTranslation(Angle<T> rotation, Vector2<T> translation) {
        Matrix.CreatePostmultipliedRotationTranslation(rotation, translation, out Matrix2x3<T> matrix);
        return matrix;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix2x3<T> CreateRotationScaleTranslation(Angle<T> rotation, Vector2<T> scale, Vector2<T> translation) {
        Matrix.CreatePostmultipliedRotationScaleTranslation(rotation: rotation, scale: scale, translation: translation, out Matrix2x3<T> matrix);
        return matrix;
        }

#endif

    #endregion Factories

    #region Rows & Columns


    /// <summary> Returns <see cref="Matrix2x3{T}.RowA"/> of the <paramref name="matrix"/> by <see langword="ref"/>.
    /// </summary>
    public static ref Vector3<T> GetRowA(ref Matrix2x3<T> matrix) {
        return ref Unsafe.As<T, Vector3<T>>(ref matrix._a1);
        }

    /// <summary> Returns <see cref="Matrix2x3{T}.RowB"/> of the <paramref name="matrix"/> by <see langword="ref"/>.
    /// </summary>
    public static ref Vector3<T> GetRowB(ref Matrix2x3<T> matrix) {
        return ref Unsafe.As<T, Vector3<T>>(ref matrix._b1);
        }

    public static int MatrixWidth { get; } = 3;
    public static int MatrixHeight { get; } = 2;

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
    public Vector2<T> Column3 {
        readonly get => new(A3, B3);
        set {
            A3 = value.X;
            B3 = value.Y;
            }
        }
    public unsafe Vector3<T> RowA {
        readonly get => new(A1, A2, A3);
        set => Unsafe.Copy(Unsafe.AsPointer(ref _a1), ref value);
        }
    public unsafe Vector3<T> RowB {
        readonly get => new(B1, B2, B3);
        set => Unsafe.Copy(Unsafe.AsPointer(ref _b1), ref value);
        }
    public readonly byte Width => 3;
    public readonly byte Height => 2;

    #endregion Rows & Columns

    #region Inversion

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix2x3<T> Invert(in Matrix2x3<T> matrix) {
        return matrix.Invert();
        }

    [BunnyAttributes.SIMDCandidate]
    public readonly Matrix2x3<T> Invert() {
        var det = Determinant;
        if (T.Abs(det) < T.Epsilon)
            return new(
                T.NaN, T.NaN, T.NaN,
                T.NaN, T.NaN, T.NaN
                );
        return new Matrix2x3<T>(
            +B2, -A2, a3: A2 * B3 - A3 * B2,
            -B1, +A1, b3: A3 * B1 - A1 * B3
            ) / det;
        }

    #endregion Inversion

    #region Determinants

    public readonly T Determinant {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => GetDeterminant(in this);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T GetDeterminant(in Matrix2x3<T> matrix) {
        return matrix.A1 * matrix.B2 - matrix.B1 * matrix.A2;
        }

    #endregion Determinants

    #region Transformation

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Vector2<T> Transform(Vector2<T> vector) {
        return Transform(in this, vector);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2<T> Transform(in Matrix2x3<T> matrix, Vector2<T> vector) {
        Matrix.TransformPostmultipliedColumnVector(in matrix, vector, out Vector2<T> ret);
        return ret;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Vector2<T> InvertedTransform(Vector2<T> vector) {
        return InvertedTransform(in this, vector);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2<T> InvertedTransform(in Matrix2x3<T> matrix, Vector2<T> vector) {
        return Transform(Invert(in matrix), vector);
        }

    #endregion Transformation

    #region Extraction

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Vector2<T> GetScale() {
        return GetScale(in this);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2<T> GetScale(in Matrix2x3<T> matrix) {
        Matrix.GetScaleFromPostmultiplied(matrix, out Vector2<T> scale);
        return scale;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Angle<T> GetRotation() {
        return GetRotation(in this);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Angle<T> GetRotation(in Matrix2x3<T> matrix) {
        Matrix.GetRotationFromPostmultiplied(in matrix, out Angle<T> angle);
        return angle;
        }

#if MATRIX_POSTMULTIPLIED_CONVENTION

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Vector2<T> GetTranslation() {
        return GetTranslation(in this);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2<T> GetTranslation(in Matrix2x3<T> matrix) {
        return new(matrix.A3, matrix.B3);
        }

#endif

    #endregion Extraction

    #region Component Removal

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Remove2DScale(ref Matrix2x3<T> matrix) {
        Matrix.Remove2DScale(ref matrix);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Remove2DScale() {
        Matrix.Remove2DScale(ref this);
        }

#if MATRIX_POSTMULTIPLIED_CONVENTION

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RemoveTranslation(ref Matrix2x3<T> matrix) {
        Matrix.Remove2DTranslation(ref matrix);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void RemoveTranslation() {
        Matrix.Remove2DTranslation(ref this);
        }

#endif

    #endregion Component Removal

    #region Equatability

    public bool Equals(Matrix2x3<T> other) {
        if (Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double)) {
            var v00 = Unsafe.As<T, Vector256<double>>(ref _a1);
            var v01 = Unsafe.As<T, Vector256<double>>(ref _a3);
            var v10 = Unsafe.As<T, Vector256<double>>(ref other._a1);
            var v11 = Unsafe.As<T, Vector256<double>>(ref other._a3);
            return Vector256.EqualsAll(v00, v10)
                && Vector256.EqualsAll(v01, v11);
            }
        else if (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float)) {
            var v00 = Unsafe.As<T, Vector128<float>>(ref _a1);
            var v01 = Unsafe.As<T, Vector128<float>>(ref _a3);
            var v10 = Unsafe.As<T, Vector128<float>>(ref other._a1);
            var v11 = Unsafe.As<T, Vector128<float>>(ref other._a3);
            return Vector128.EqualsAll(v00, v10)
                && Vector128.EqualsAll(v01, v11);
            }
        else {
            return A1 == other.A1 && A2 == other.A2 && A3 == other.A3
                && B1 == other.B1 && B2 == other.B2 && B3 == other.B3;
            }
        }

    public override bool Equals(object? obj) {
        return obj is Matrix2x3<T> matrix && Equals(matrix);
        }

    public override readonly int GetHashCode() {
        return HashCode.Combine(RowA.GetHashCode(), RowB.GetHashCode());
        }

    public static bool operator ==(Matrix2x3<T> left, Matrix2x3<T> right) {
        return left.Equals(right);
        }

    public static bool operator !=(Matrix2x3<T> left, Matrix2x3<T> right) {
        return left.Equals(right) == false;
        }

    #endregion Equatability

    #region Operators

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix2x3<T> operator *(T factor, in Matrix2x3<T> matrix) {
        return Scale(in matrix, factor);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Matrix2x3<T> Multiply(in Matrix2x3<T> other) {
        return Multiply(this, other);
        }

    [BunnyAttributes.SIMDCandidate]
    public static Matrix2x3<T> Multiply(in Matrix2x3<T> left, in Matrix2x3<T> right) {
        return new(
            left.A1 * right.A1 + left.A2 * right.B1,
            left.A1 * right.A2 + left.A2 * right.B2,
            left.A1 * right.A3 + left.A2 * right.B3 + left.A3,
            left.B1 * right.A1 + left.B2 * right.B1,
            left.B1 * right.A2 + left.B2 * right.B2,
            left.B1 * right.A3 + left.B2 * right.B3 + left.B3
            );
        }

    [BunnyAttributes.SIMDCandidate]
    public static Matrix2x3<T> Scale(in Matrix2x3<T> matrix, T factor) {
        return new(
            matrix.A1 * factor, matrix.A2 * factor, a3: matrix.A3 * factor,
            matrix.B1 * factor, matrix.B2 * factor, b3: matrix.B3 * factor
            );
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Matrix2x3<T> Scale(T factor) {
        return Scale(this, factor);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix2x3<T> operator /(in Matrix2x3<T> matrix, T value) {
        return Scale(matrix, T.One / value);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix2x3<T> operator +(in Matrix2x3<T> left, in Matrix2x3<T> right) {
        return Add(left, right);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Matrix2x3<T> Add(in Matrix2x3<T> other) {
        return Add(this, other);
        }

    [BunnyAttributes.SIMDCandidate]
    public static Matrix2x3<T> Add(in Matrix2x3<T> left, in Matrix2x3<T> right) {
        return new(
            left.A1 + right.A1, left.A2 + right.A2, a3: left.A3 + right.A3,
            left.B1 + right.B1, left.B2 + right.B2, b3: left.B3 + right.B3
            );
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Matrix2x3<T> Subtract(in Matrix2x3<T> other) {
        return Subtract(this, other);
        }

    [BunnyAttributes.SIMDCandidate]
    public static Matrix2x3<T> Subtract(in Matrix2x3<T> left, in Matrix2x3<T> right) {
        return new(
            left.A1 - right.A1, left.A2 - right.A2, a3: left.A3 - right.A3,
            left.B1 - right.B1, left.B2 - right.B2, b3: left.B3 - right.B3
            );
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix2x3<T> operator -(in Matrix2x3<T> left, in Matrix2x3<T> right) {
        return Subtract(left, right);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix2x3<T> operator /(in Matrix2x3<T> left, in Matrix2x3<T> right) {
        return Divide(in left, in right);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix2x3<T> Divide(in Matrix2x3<T> left, in Matrix2x3<T> right) {
        return Multiply(left, Invert(right));
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Matrix2x3<T> Divide(in Matrix2x3<T> other) {
        return Divide(in this, in other);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix2x3<T> operator *(in Matrix2x3<T> left, in Matrix2x3<T> right) {
        return Multiply(left, right);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix2x3<T> operator *(in Matrix2x3<T> matrix, T factor) {
        return Scale(matrix, factor);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2<T> operator *(in Matrix2x3<T> matrix, Vector2<T> vector) {
        return Transform(matrix, vector);
        }

    /// <inheritdoc
    /// cref="IMatrix{Numeric}.ISquareMatrix{Matrix}.Multiply(in Matrix, in Matrix)"/>
    [BunnyAttributes.SIMDCandidate]
    public static Matrix2<T> Multiply(in Matrix2x3<T> left, in Matrix3x2<T> right) {
        return new(
            left.A1 * right.A1 + left.A2 * right.B1 + left.A3 * right.C1,
            left.A1 * right.A2 + left.A2 * right.B2 + left.A3 * right.C2,
            left.B1 * right.A1 + left.B2 * right.B1 + left.B3 * right.C1,
            left.B1 * right.A2 + left.B2 * right.B2 + left.B3 * right.C2
            );
        }

    /// <inheritdoc
    /// cref="Multiply(in Matrix2x3{T}, in Matrix3x2{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Matrix2<T> Multiply(in Matrix3x2<T> other) {
        return Multiply(in this, in other);
        }

    /// <inheritdoc
    /// cref="Multiply(in Matrix2x3{T}, in Matrix3x2{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix2<T> operator *(in Matrix2x3<T> left, in Matrix3x2<T> right) {
        return Multiply(in left, in right);
        }


    #endregion Operators

    #region Strings

    public override readonly string ToString() {
        return ToString(IMatrix<T>.DefaultRounding, 0, 0);
        }

    public readonly string ToString(byte digits, int integerLength, int paddingLength) {
        var a1 = A1.Stringify(digits, integerLength, paddingLength);
        var a2 = A2.Stringify(digits, integerLength, paddingLength);
        var a3 = A3.Stringify(digits, integerLength, paddingLength);
        var b1 = B1.Stringify(digits, integerLength, paddingLength);
        var b2 = B2.Stringify(digits, integerLength, paddingLength);
        var b3 = B3.Stringify(digits, integerLength, paddingLength);

        return $"[ [{a1}, {a2}, {a3}],\n  [{b1}, {b2}, {b3}] ]";
        }

    #endregion Strings

    #region Format Data

    public readonly T[][] RowsToJaggedArray() {
        return [[A1, A2, A3], [B1, B2, B3]];
        }
    public readonly T[][] ColumnsToJaggedArray() {
        return [[A1, B1], [A2, B2], [A3, B3]];
        }
    public readonly T[,] RowsTo2DArray() {
        return new T[,] { { A1, A2, A3 }, { B1, B2, B3 } };
        }
    public readonly T[,] ColumnsTo2DArray() {
        return new T[,] { { A1, B1 }, { A2, B2 }, { A3, B3 } };
        }

    #endregion Format Data

    #region Casting

    public static TMatrix UnsafeCast<TMatrix, TNumeric>(ref Matrix2x3<T> matrix)
        where TMatrix : unmanaged, IMatrix<TNumeric>
        where TNumeric : unmanaged, IBinaryFloatingPointIeee754<TNumeric>, IMinMaxValue<TNumeric> {
        Debug.Assert((typeof(TNumeric) == typeof(T)) && (Matrix2x3<T>.MatrixWidth == TMatrix.MatrixWidth) && (Matrix2x3<T>.MatrixHeight == TMatrix.MatrixHeight));
        return Unsafe.As<Matrix2x3<T>, TMatrix>(ref matrix);
        }

    public TMatrix UnsafeCast<TMatrix, TNumeric>()
        where TMatrix : unmanaged, IMatrix<TNumeric>
        where TNumeric : unmanaged, IBinaryFloatingPointIeee754<TNumeric>, IMinMaxValue<TNumeric> {
        Debug.Assert((typeof(TNumeric) == typeof(T)) && (Matrix2x3<T>.MatrixWidth == TMatrix.MatrixWidth) && (Matrix2x3<T>.MatrixHeight == TMatrix.MatrixHeight));
        return Unsafe.As<Matrix2x3<T>, TMatrix>(ref this);
        }

    #endregion Casting
    }
