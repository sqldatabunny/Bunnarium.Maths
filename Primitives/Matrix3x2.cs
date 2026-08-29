using Bunnarium.Tools;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using Debug = System.Diagnostics.Debug;

namespace Bunnarium.Maths.Primitives;


#if MATRIX_PREMULTIPLIED_CONVENTION
/// <summary> A matrix with three rows and two columns.
/// </summary>
/// <remarks> This matrix implements <see cref="IMatrix{Numeric}.ITranslationMatrix{Matrix, Vector, Direction, Rotation}">ITranslationMatrix</see> because the <c>MATRIX_PREMULTIPLIED_CONVENTION</c> compiler symbol is set. For more information, please see <see cref="Matrix.Docs.MultiplicationConventionInstructions{T}"/>.
/// <para/><inheritdoc cref="Matrix.Docs.DisplayMatrix3x2"/>
/// </remarks>
#elif MATRIX_POSTMULTIPLIED_CONVENTION
/// <summary> A matrix with three rows and two columns.
/// </summary>
/// <remarks> This matrix does not implement <see cref="IMatrix{Numeric}.ITranslationMatrix{Matrix, Vector, Direction, Rotation}">ITranslationMatrix</see> because the <c>MATRIX_POSTMULTIPLIED_CONVENTION</c> compiler symbol is set. Because this is the case, translation-related functions for this type can only be accessed via the <see cref="Matrix"/> function library. For more information, please see <see cref="Matrix.Docs.MultiplicationConventionInstructions{T}"/>.
/// <para/><inheritdoc cref="Matrix.Docs.DisplayMatrix3x2"/>
/// </remarks>
#endif
[DebuggerDisplay("{ToString(), nq}")]
public struct Matrix3x2<T>
    : IMatrix<T>.I2DMatrix<Matrix3x2<T>>
    , IMatrix<T>.IMatrixOfLinearWidth<Matrix3x2<T>, Vector2<T>>
#if MATRIX_PREMULTIPLIED_CONVENTION
    , IMatrix<T>.ITranslationMatrix<Matrix3x2<T>, Vector2<T>, Angle<T>, Angle<T>>
#endif
    where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {

    #region Data

    private T _a1, _a2, _b1, _b2, _c1, _c2;

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

    /// <summary> The first element of the third row.
    /// </summary>
    public T C1 { readonly get => _c1; set => _c1 = value; }

    /// <summary> The second element of the third row.
    /// </summary>
    public T C2 { readonly get => _c2; set => _c2 = value; }

    #endregion Cells

    #region Constructors

    /// <inheritdoc
    /// cref="Matrix4{T}.Matrix4(T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T)"/>
    public Matrix3x2(T a1, T a2, T b1, T b2, T c1, T c2) {
        _a1 = a1; _a2 = a2;
        _b1 = b1; _b2 = b2;
        _c1 = c1; _c2 = c2;
        }

    /// <summary> Creates a matrix from vector-based rows.
    /// </summary>
    public Matrix3x2(Vector2<T> rowA, Vector2<T> rowB, Vector2<T> rowC) : this(
        rowA.X, rowA.Y,
        rowB.X, rowB.Y,
        rowC.X, rowC.Y
        ) { }


    /// <summary> Creates a <see cref="Matrix3x2{T}"/> such that its first two rows originate from the <paramref name="matrix"/> and its third is the passed <paramref name="rowC"/>.
    /// </summary>
    public Matrix3x2(Matrix2<T> matrix, Vector2<T> rowC) : this(
        matrix.A1, matrix.A2,
        matrix.B1, matrix.B2,
        rowC.X, rowC.Y
        ) { }

    /// <summary> Creates a new <see cref="Matrix3x2{T}"/> as a <see cref="Matrix2{T}"/> with a third row, set to <c>(0, 0)</c>, appended.
    /// </summary>
    public Matrix3x2(Matrix2<T> matrix) : this(
        matrix.A1, matrix.A2,
        matrix.B1, matrix.B2,
        T.Zero, T.Zero
        ) { }


    /// <summary> Creates a <see cref="Matrix3x2{T}"/> from a <see cref="Matrix3{T}"/>, with the latter's third column omitted.
    /// </summary>
    public Matrix3x2(Matrix3<T> matrix) : this(
        matrix.A1, matrix.A2,
        matrix.B1, matrix.B2,
        matrix.C1, matrix.C2
        ) { }

    #endregion Constructors

    #region Factories

    public static Matrix3x2<T> Identity { get; } = new(
        +T.One, T.Zero,
        T.Zero, +T.One,
        T.Zero, T.Zero
        );

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix3x2<T> CreateScale(Vector2<T> scale) {
        Matrix.CreatePremultipliedScale(scale, out Matrix3x2<T> matrix);
        return matrix;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix3x2<T> CreateRotation(Angle<T> rotation) {
        Matrix.CreateRotation(rotation, out Matrix2<T> matrix);
        Matrix.UnsafeConvert(in matrix, out Matrix3x2<T> ret);
        return ret;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix3x2<T> CreateRotationScale(Angle<T> rotation, Vector2<T> scale) {
        Matrix.CreatePremultipliedRotationScale(rotation: rotation, scale: scale, out Matrix3x2<T> matrix);
        return matrix;
        }

#if MATRIX_PREMULTIPLIED_CONVENTION

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix3x2<T> CreateTranslation(Vector2<T> translation) {
        Matrix.CreatePremultipliedTranslation(translation: translation, out Matrix3x2<T> matrix);
        return matrix;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix3x2<T> CreateRotationTranslation(Angle<T> rotation, Vector2<T> translation) {
        Matrix.CreatePremultipliedRotationTranslation(rotation: rotation, translation: translation, out Matrix3x2<T> matrix);
        return matrix;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix3x2<T> CreateRotationScaleTranslation(Angle<T> rotation, Vector2<T> scale, Vector2<T> translation) {
        Matrix.CreatePremultipliedRotationScaleTranslation(rotation: rotation, scale: scale, translation: translation, out Matrix3x2<T> matrix);
        return matrix;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix3x2<T> CreateScaleTranslation(Vector2<T> scale, Vector2<T> translation) {
        Matrix.CreatePremultipliedScaleTranslation(scale: scale, translation: translation, out Matrix3x2<T> matrix);
        return matrix;
        }

#endif

    #endregion Factories

    #region Rows & Columns


    /// <summary> Returns <see cref="Matrix3x2{T}.RowA"/> of the <paramref name="matrix"/> by <see langword="ref"/>.
    /// </summary>
    public static ref Vector2<T> GetRowA(ref Matrix3x2<T> matrix) {
        return ref Unsafe.As<T, Vector2<T>>(ref matrix._a1);
        }

    /// <summary> Returns <see cref="Matrix3x2{T}.RowB"/> of the <paramref name="matrix"/> by <see langword="ref"/>.
    /// </summary>
    public static ref Vector2<T> GetRowB(ref Matrix3x2<T> matrix) {
        return ref Unsafe.As<T, Vector2<T>>(ref matrix._b1);
        }

    /// <summary> Returns <see cref="Matrix3x2{T}.RowC"/> of the <paramref name="matrix"/> by <see langword="ref"/>.
    /// </summary>
    public static ref Vector2<T> GetRowC(ref Matrix3x2<T> matrix) {
        return ref Unsafe.As<T, Vector2<T>>(ref matrix._c1);
        }

    public static int MatrixWidth { get; } = 2;
    public static int MatrixHeight { get; } = 3;

    public Vector3<T> Column1 {
        readonly get => new(A1, B1, C1);
        set {
            A1 = value.X;
            B1 = value.Y;
            C1 = value.Z;
            }
        }
    public Vector3<T> Column2 {
        readonly get => new(A2, B2, C2);
        set {
            A2 = value.X;
            B2 = value.Y;
            C2 = value.Z;
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
    public unsafe Vector2<T> RowC {
        readonly get => new(C1, C2);
        set => Unsafe.Copy(Unsafe.AsPointer(ref _c1), ref value);
        }
    public readonly byte Width => 2;
    public readonly byte Height => 3;

    #endregion Rows & Columns

    #region Inversion

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix3x2<T> Invert(in Matrix3x2<T> matrix) {
        return matrix.Invert();
        }

    [BunnyAttributes.SIMDCandidate]
    public readonly Matrix3x2<T> Invert() {
        var det = Determinant;
        if (T.Abs(det) < T.Epsilon)
            return new(
                T.NaN, T.NaN,
                T.NaN, T.NaN,
                T.NaN, T.NaN
                );
        return new Matrix3x2<T>(
            +B2, -A2,
            -B1, +A1,
            B1 * C2 - B2 * C1,
            A2 * C1 - A1 * C2
            )
            / det;
        }

    #endregion Inversion

    #region Determinants

    public readonly T Determinant {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => GetDeterminant(in this);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T GetDeterminant(in Matrix3x2<T> matrix) {
        return matrix.A1 * matrix.B2 - matrix.B1 * matrix.A2;
        }

    #endregion Determinants

    #region Transformation

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Vector2<T> Transform(Vector2<T> vector) {
        return Transform(in this, vector);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2<T> Transform(in Matrix3x2<T> matrix, Vector2<T> vector) {
        Matrix.TransformPremultipliedRowVector(vector, in matrix, out var ret);
        return ret;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Vector2<T> InvertedTransform(Vector2<T> vector) {
        return InvertedTransform(in this, vector);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2<T> InvertedTransform(in Matrix3x2<T> matrix, Vector2<T> vector) {
        return Transform(Invert(in matrix), vector);
        }

    #endregion Transformation

    #region Extraction

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Vector2<T> GetScale() {
        return GetScale(in this);
        }

    [BunnyAttributes.SIMDCandidate]
    public static Vector2<T> GetScale(in Matrix3x2<T> matrix) {
        Matrix.GetScaleFromPremultiplied(in matrix, out Vector2<T> scale);
        return scale;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Angle<T> GetRotation() {
        return GetRotation(in this);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Angle<T> GetRotation(in Matrix3x2<T> matrix) {
        Matrix.GetRotationFromPremultiplied(in matrix, out Angle<T> angle);
        return angle;
        }

#if MATRIX_PREMULTIPLIED_CONVENTION

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Vector2<T> GetTranslation() {
        return GetTranslation(in this);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2<T> GetTranslation(in Matrix3x2<T> matrix) {
        Matrix.GetTranslationFromPremultiplied(matrix, out Vector2<T> translation);
        return translation;
        }

#endif

    #endregion Extraction

    #region Component Removal

#if MATRIX_PREMULTIPLIED_CONVENTION

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RemoveTranslation(ref Matrix3x2<T> matrix) {
        Matrix.Remove2DTranslation(ref matrix);
        }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void RemoveTranslation() {
        Matrix.Remove2DTranslation(ref this);
        }

#endif

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Remove2DScale(ref Matrix3x2<T> matrix) {
        Matrix.Remove2DScale(ref matrix);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Remove2DScale() {
        Matrix.Remove2DScale(ref this);
        }

    #endregion Component Removal

    #region Equatability

    public bool Equals(Matrix3x2<T> other) {
        if (Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double)) {
            var v00 = Unsafe.As<T, Vector256<double>>(ref _a1);
            var v01 = Unsafe.As<T, Vector256<double>>(ref _b1);
            var v10 = Unsafe.As<T, Vector256<double>>(ref other._a1);
            var v11 = Unsafe.As<T, Vector256<double>>(ref other._b1);
            return Vector256.EqualsAll(v00, v10)
                && Vector256.EqualsAll(v01, v11);
            }
        else if (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float)) {
            var v00 = Unsafe.As<T, Vector128<float>>(ref _a1);
            var v01 = Unsafe.As<T, Vector128<float>>(ref _b1);
            var v10 = Unsafe.As<T, Vector128<float>>(ref other._a1);
            var v11 = Unsafe.As<T, Vector128<float>>(ref other._b1);
            return Vector128.EqualsAll(v00, v10)
                && Vector128.EqualsAll(v01, v11);
            }
        else {
            return A1 == other.A1 && A2 == other.A2
                && B1 == other.B1 && B2 == other.B2
                && C1 == other.C1 && C2 == other.C2;
            }
        }

    public override bool Equals(object? obj) {
        return obj is Matrix3x2<T> matrix && Equals(matrix);
        }

    public override readonly int GetHashCode() {
        return HashCode.Combine(RowA.GetHashCode(), RowB.GetHashCode(), RowC.GetHashCode());
        }

    public static bool operator ==(Matrix3x2<T> left, Matrix3x2<T> right) {
        return left.Equals(right);
        }

    public static bool operator !=(Matrix3x2<T> left, Matrix3x2<T> right) {
        return left.Equals(right) == false;
        }

    #endregion Equatability

    #region Operators

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix3x2<T> operator *(T factor, in Matrix3x2<T> matrix) {
        return Scale(in matrix, factor);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Matrix3x2<T> Multiply(in Matrix3x2<T> other) {
        return Multiply(this, other);
        }

    [BunnyAttributes.SIMDCandidate]
    public static Matrix3x2<T> Multiply(in Matrix3x2<T> left, in Matrix3x2<T> right) {
        return new(
            left.A1 * right.A1 + left.A2 * right.B1,
            left.A1 * right.A2 + left.A2 * right.B2,
            left.B1 * right.A1 + left.B2 * right.B1,
            left.B1 * right.A2 + left.B2 * right.B2,
            left.C1 * right.A1 + left.C2 * right.B1 + right.C1,
            left.C1 * right.A2 + left.C2 * right.B2 + right.C2
            );
        }

    [BunnyAttributes.SIMDCandidate]
    public static Matrix3x2<T> Scale(in Matrix3x2<T> matrix, T factor) {
        return new(
            matrix.A1 * factor, matrix.A2 * factor,
            matrix.B1 * factor, matrix.B2 * factor,
            matrix.C1 * factor, matrix.C2 * factor
            );
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Matrix3x2<T> Scale(T factor) {
        return Scale(this, factor);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix3x2<T> operator /(in Matrix3x2<T> matrix, T value) {
        return Scale(matrix, T.One / value);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix3x2<T> operator +(in Matrix3x2<T> left, in Matrix3x2<T> right) {
        return Add(left, right);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Matrix3x2<T> Add(in Matrix3x2<T> other) {
        return Add(this, other);
        }

    [BunnyAttributes.SIMDCandidate]
    public static Matrix3x2<T> Add(in Matrix3x2<T> left, in Matrix3x2<T> right) {
        return new(
            left.A1 + right.A1, left.A2 + right.A2,
            left.B1 + right.B1, left.B2 + right.B2,
            left.C1 + right.C1, left.C2 + right.C2
            );
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Matrix3x2<T> Subtract(in Matrix3x2<T> other) {
        return Subtract(this, other);
        }

    [BunnyAttributes.SIMDCandidate]
    public static Matrix3x2<T> Subtract(in Matrix3x2<T> left, in Matrix3x2<T> right) {
        return new(
            left.A1 - right.A1, left.A2 - right.A2,
            left.B1 - right.B1, left.B2 - right.B2,
            left.C1 - right.C1, left.C2 - right.C2
            );
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix3x2<T> operator -(in Matrix3x2<T> left, in Matrix3x2<T> right) {
        return Subtract(left, right);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix3x2<T> operator /(in Matrix3x2<T> left, in Matrix3x2<T> right) {
        return Divide(in left, in right);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix3x2<T> Divide(in Matrix3x2<T> left, in Matrix3x2<T> right) {
        return Multiply(left, Invert(right));
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Matrix3x2<T> Divide(in Matrix3x2<T> other) {
        return Divide(in this, in other);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix3x2<T> operator *(in Matrix3x2<T> left, in Matrix3x2<T> right) {
        return Multiply(left, right);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix3x2<T> operator *(in Matrix3x2<T> matrix, T factor) {
        return Scale(matrix, factor);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2<T> operator *(in Matrix3x2<T> matrix, Vector2<T> vector) {
        return Transform(matrix, vector);
        }

    /// <inheritdoc
    /// cref="IMatrix{Numeric}.ISquareMatrix{Matrix}.Multiply(in Matrix, in Matrix)"/>
    [BunnyAttributes.SIMDCandidate]
    public static Matrix3<T> Multiply(in Matrix3x2<T> left, in Matrix2x3<T> right) {
        return new(
            left.A1 * right.A1 + left.A2 * right.B1,
            left.A1 * right.A2 + left.A2 * right.B2,
            left.A1 * right.A3 + left.A2 * right.B3,
            left.B1 * right.A1 + left.B2 * right.B1,
            left.B1 * right.A2 + left.B2 * right.B2,
            left.B1 * right.A3 + left.B2 * right.B3,
            left.C1 * right.A1 + left.C2 * right.B1,
            left.C1 * right.A2 + left.C2 * right.B2,
            left.C1 * right.A3 + left.C2 * right.B3
            );
        }

    /// <inheritdoc
    /// cref="Multiply(in Matrix3x2{T}, in Matrix2x3{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Matrix3<T> Multiply(in Matrix2x3<T> other) {
        return Multiply(in this, in other);
        }

    /// <inheritdoc
    /// cref="Multiply(in Matrix3x2{T}, in Matrix2x3{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix3<T> operator *(in Matrix3x2<T> left, in Matrix2x3<T> right) {
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
        var b1 = B1.Stringify(digits, integerLength, paddingLength);
        var b2 = B2.Stringify(digits, integerLength, paddingLength);
        var c1 = C1.Stringify(digits, integerLength, paddingLength);
        var c2 = C2.Stringify(digits, integerLength, paddingLength);

        return $"[ [{a1}, {a2}],\n  [{b1}, {b2}],\n  [{c1}, {c2}] ]";
        }

    #endregion Strings

    #region Format Data

    public readonly T[][] RowsToJaggedArray() {
        return [[A1, A2], [B1, B2], [C1, C2]];
        }
    public readonly T[][] ColumnsToJaggedArray() {
        return [[A1, B1, C1], [A2, B2, C2]];
        }
    public readonly T[,] RowsTo2DArray() {
        return new T[,] { { A1, A2 }, { B1, B2 }, { C1, C2 } };
        }
    public readonly T[,] ColumnsTo2DArray() {
        return new T[,] { { A1, B1, C1 }, { A2, B2, C2 } };
        }

    #endregion Format Data

    #region Casting

    public static TMatrix UnsafeCast<TMatrix, TNumeric>(ref Matrix3x2<T> matrix)
        where TMatrix : unmanaged, IMatrix<TNumeric>
        where TNumeric : unmanaged, IBinaryFloatingPointIeee754<TNumeric>, IMinMaxValue<TNumeric> {
        Debug.Assert((typeof(TNumeric) == typeof(T)) && (Matrix3x2<T>.MatrixWidth == TMatrix.MatrixWidth) && (Matrix3x2<T>.MatrixHeight == TMatrix.MatrixHeight));
        return Unsafe.As<Matrix3x2<T>, TMatrix>(ref matrix);
        }

    public TMatrix UnsafeCast<TMatrix, TNumeric>()
        where TMatrix : unmanaged, IMatrix<TNumeric>
        where TNumeric : unmanaged, IBinaryFloatingPointIeee754<TNumeric>, IMinMaxValue<TNumeric> {
        Debug.Assert((typeof(TNumeric) == typeof(T)) && (Matrix3x2<T>.MatrixWidth == TMatrix.MatrixWidth) && (Matrix3x2<T>.MatrixHeight == TMatrix.MatrixHeight));
        return Unsafe.As<Matrix3x2<T>, TMatrix>(ref this);
        }

    #endregion Casting
    }
