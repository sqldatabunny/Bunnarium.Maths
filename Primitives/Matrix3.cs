using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using Debug = System.Diagnostics.Debug;

namespace Bunnarium.Maths.Primitives;

/// <summary> A matrix with three rows and three columns.
/// </summary>
/// <remarks><inheritdoc cref="Matrix.Docs.DisplayMatrix3"/>
/// </remarks>
[DebuggerDisplay("{ToString(), nq}")]
public struct Matrix3<T>
    : IMatrix<T>.I3DMatrix<Matrix3<T>>
    , IMatrix<T>.ISquareMatrix<Matrix3<T>>, IMatrix<T>.IMatrixOfLinearWidth<Matrix3<T>, Vector3<T>>
    , IMatrix<T>.IProjectionMatrix<Matrix3<T>, Vector2<T>, Vector3<T>, Angle<T>, Angle<T>>
    , IMatrix<T>.ITranslationMatrix<Matrix3<T>, Vector2<T>, Angle<T>, Angle<T>>
    where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {

    #region Data

    private T _a1, _a2, _a3, _b1, _b2, _b3, _c1, _c2, _c3;

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

    /// <summary> The first element of the third row.
    /// </summary>
    public T C1 { readonly get => _c1; set => _c1 = value; }

    /// <summary> The second element of the third row.
    /// </summary>
    public T C2 { readonly get => _c2; set => _c2 = value; }

    /// <summary> The third element of the third row.
    /// </summary>
    public T C3 {
        readonly get => _c3; set => _c3 = value;
        }

    #endregion Cells

    #region Constructors

    /// <inheritdoc
    /// cref="Matrix4{T}.Matrix4(T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T)"/>
    public Matrix3(
        T a1, T a2, T a3,
        T b1, T b2, T b3,
        T c1, T c2, T c3) {
        _a1 = a1; _a2 = a2; _a3 = a3;
        _b1 = b1; _b2 = b2; _b3 = b3;
        _c1 = c1; _c2 = c2; _c3 = c3;
        }

    /// <summary> Creates an affine transformation matrix from a <see cref="Matrix3x2{T}"/>, such that its first two columns are populated by the input <paramref name="matrix"/> and its third column is <c>[0, 0, 1]</c>.
    /// </summary>
    public Matrix3(Matrix3x2<T> matrix) : this(
        matrix.A1, matrix.A2, T.Zero,
        matrix.B1, matrix.B2, T.Zero,
        matrix.C1, matrix.C2, T.One
        ) { }

    /// <summary> Creates an affine transformation matrix from a <see cref="Matrix2x3{T}"/>, such that its first two rows are populated by the input <paramref name="matrix"/> and its third row is <c>[0, 0, 1]</c>.
    /// </summary>
    public Matrix3(Matrix2x3<T> matrix) : this(
        matrix.A1, matrix.A2, matrix.A3,
        matrix.B1, matrix.B2, matrix.B3,
        T.Zero, T.Zero, T.One
        ) { }


    /// <summary> Creates a <see cref="Matrix3{T}"/> such that its first two rows originate from the <paramref name="matrix"/> and its third is the passed <paramref name="rowC"/>.
    /// </summary>
    public Matrix3(Matrix2x3<T> matrix, Vector3<T> rowC) : this(
        matrix.A1, matrix.A2, matrix.A3,
        matrix.B1, matrix.B2, matrix.B3,
        rowC.X, rowC.Y, rowC.Z
        ) { }

    /// <summary> Creates an affine transformation matrix from a <see cref="Matrix2{T}"/>, such that its first two rows and columns are populated by the input <paramref name="matrix"/> and its third row and column are both <c>[0, 0, 0, 1]</c>.
    /// </summary>
    public Matrix3(Matrix2<T> matrix) : this(
        matrix.A1, matrix.A2, T.Zero,
        matrix.B1, matrix.B2, T.Zero,
        T.Zero, T.Zero, T.One
        ) { }

    /// <summary> Creates a <see cref="Matrix3{T}"/> such that its first two columns originate from the <paramref name="matrix"/> and its third is the passed <paramref name="columnC"/>.
    /// </summary>
    public Matrix3(Matrix3x2<T> matrix, Vector3<T> columnC) : this(
        matrix.A1, matrix.A2, columnC.X,
        matrix.B1, matrix.B2, columnC.Y,
        matrix.C1, matrix.C2, columnC.Z
        ) { }

    /// <summary> Creates a <see cref="Matrix3{T}"/> from a <see cref="Matrix3x4{T}"/>, with the latter's fourth column omitted.
    /// </summary>
    public Matrix3(Matrix3x4<T> matrix) {
        _a1 = matrix.A1; _a2 = matrix.A2; _a3 = matrix.A3;
        _b1 = matrix.B1; _b2 = matrix.B2; _b3 = matrix.B3;
        _c1 = matrix.C1; _c2 = matrix.C2; _c3 = matrix.C3;
        }

    /// <summary> Creates a <see cref="Matrix3{T}"/> from a <see cref="Matrix4x3{T}"/>, with the latter's fourth row omitted.
    /// </summary>
    public Matrix3(Matrix4x3<T> matrix) {
        _a1 = matrix.A1; _a2 = matrix.A2; _a3 = matrix.A3;
        _b1 = matrix.B1; _b2 = matrix.B2; _b3 = matrix.B3;
        _c1 = matrix.C1; _c2 = matrix.C2; _c3 = matrix.C3;
        }

    /// <summary> Creates a <see cref="Matrix3{T}"/> from a <see cref="Matrix4{T}"/>, with the latter's fourth row and column omitted.
    /// </summary>
    public Matrix3(Matrix4<T> matrix) {
        _a1 = matrix.A1; _a2 = matrix.A2; _a3 = matrix.A3;
        _b1 = matrix.B1; _b2 = matrix.B2; _b3 = matrix.B3;
        _c1 = matrix.C1; _c2 = matrix.C2; _c3 = matrix.C3;
        }

    /// <summary> Creates a matrix from vector-based rows.
    /// </summary>
    public Matrix3(Vector3<T> rowA, Vector3<T> rowB, Vector3<T> rowC) : this(
        rowA.X, rowA.Y, rowA.Z,
        rowB.X, rowB.Y, rowB.Z,
        rowC.X, rowC.Y, rowC.Z
        ) { }


    #endregion Constructors

    #region Factories

    public static Matrix3<T> Identity { get; } = new(
        +T.One, T.Zero, T.Zero,
        T.Zero, +T.One, T.Zero,
        T.Zero, T.Zero, +T.One
        );

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix3<T> CreateScale(Vector3<T> scale) {
        Matrix.CreateScale(scale, out Matrix3<T> matrix);
        return matrix;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix3<T> CreateRotation(Quaternion<T> rotation) {
        Matrix.CreateRotation(rotation, out Matrix4<T> matrix);
        Matrix.UnsafeConvert(in matrix, out Matrix3<T> ret);
        return ret;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix3<T> CreateRotationScale(Quaternion<T> rotation, Vector3<T> scale) {
        Matrix.CreateRotationScale(rotation, scale, out Matrix3<T> matrix);
        return matrix;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix3<T> CreateRotationX(Angle<T> angle) {
        Matrix.CreateRotationX(angle.Radians, out Matrix4<T> matrix);
        Matrix.UnsafeConvert(in matrix, out Matrix3<T> ret);
        return ret;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix3<T> CreateRotationY(Angle<T> angle) {
        Matrix.CreateRotationY(angle.Radians, out Matrix4<T> matrix);
        Matrix.UnsafeConvert(in matrix, out Matrix3<T> ret);
        return ret;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix3<T> CreateRotationZ(Angle<T> angle) {
        Matrix.CreateRotationZ(angle.Radians, out Matrix4<T> matrix);
        Matrix.UnsafeConvert(in matrix, out Matrix3<T> ret);
        return ret;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix3<T> CreateAxisAngle(Direction<T> axis, Angle<T> angle) {
        Matrix.AxisAngle(axis, angle, out Matrix4<T> matrix);
        Matrix.UnsafeConvert(in matrix, out Matrix3<T> ret);
        return ret;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix3<T> CreateRotationYXZ(Angle<T> rotationY, Angle<T> rotationX, Angle<T> rotationZ) {
        Matrix.CreateRotationYXZ(rotationY, rotationX, rotationZ, out Matrix4<T> matrix);
        Matrix.UnsafeConvert(in matrix, out Matrix3<T> ret);
        return ret;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix3<T> CreateRotationZYX(Angle<T> rotationZ, Angle<T> rotationY, Angle<T> rotationX) {
        Matrix.CreateRotationZYX(rotationZ, rotationY, rotationX, out Matrix4<T> matrix);
        Matrix.UnsafeConvert(in matrix, out Matrix3<T> ret);
        return ret;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix3<T> CreateRotationZXY(Angle<T> rotationZ, Angle<T> rotationX, Angle<T> rotationY) {
        Matrix.CreateRotationZXY(rotationZ, rotationX, rotationY, out Matrix4<T> matrix);
        Matrix.UnsafeConvert(in matrix, out Matrix3<T> ret);
        return ret;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix3<T> CreateRotationYZX(Angle<T> rotationY, Angle<T> rotationZ, Angle<T> rotationX) {
        Matrix.CreateRotationYZX(rotationY, rotationZ, rotationX, out Matrix4<T> matrix);
        Matrix.UnsafeConvert(in matrix, out Matrix3<T> ret);
        return ret;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix3<T> CreateRotationXYZ(Angle<T> rotationX, Angle<T> rotationY, Angle<T> rotationZ) {
        Matrix.CreateRotationXYZ(rotationX, rotationY, rotationZ, out Matrix4<T> matrix);
        Matrix.UnsafeConvert(in matrix, out Matrix3<T> ret);
        return ret;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix3<T> CreateRotationXZY(Angle<T> rotationX, Angle<T> rotationZ, Angle<T> rotationY) {
        Matrix.CreateRotationXZY(rotationX, rotationZ, rotationY, out Matrix4<T> matrix);
        Matrix.UnsafeConvert(in matrix, out Matrix3<T> ret);
        return ret;
        }

    #endregion Factories

    #region Rows & Columns

    /// <summary> Returns <see cref="Matrix3{T}.RowA"/> of the <paramref name="matrix"/> by <see langword="ref"/>.
    /// </summary>
    public static ref Vector3<T> GetRowA(ref Matrix3<T> matrix) {
        return ref Unsafe.As<T, Vector3<T>>(ref matrix._a1);
        }

    /// <summary> Returns <see cref="Matrix3{T}.RowB"/> of the <paramref name="matrix"/> by <see langword="ref"/>.
    /// </summary>
    public static ref Vector3<T> GetRowB(ref Matrix3<T> matrix) {
        return ref Unsafe.As<T, Vector3<T>>(ref matrix._b1);
        }

    /// <summary> Returns <see cref="Matrix3{T}.RowC"/> of the <paramref name="matrix"/> by <see langword="ref"/>.
    /// </summary>
    public static ref Vector3<T> GetRowC(ref Matrix3<T> matrix) {
        return ref Unsafe.As<T, Vector3<T>>(ref matrix._c1);
        }

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
    public Vector3<T> Column3 {
        readonly get => new(A3, B3, C3);
        set {
            A3 = value.X;
            B3 = value.Y;
            C3 = value.Z;
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
    public unsafe Vector3<T> RowC {
        readonly get => new(C1, C2, C3);
        set => Unsafe.Copy(Unsafe.AsPointer(ref _c1), ref value);
        }
    public readonly byte Width => 3;
    public readonly byte Height => 3;

    public static int MatrixWidth { get; } = 3;
    public static int MatrixHeight { get; } = 3;

    #endregion Rows & Columns

    #region Inversion

    [BunnyAttributes.SIMDCandidate]
    public readonly Matrix3<T> Invert() {
        var det = Determinant;
        if (T.Abs(det) < T.Epsilon)
            return new(
                T.NaN, T.NaN, T.NaN,
                T.NaN, T.NaN, T.NaN,
                T.NaN, T.NaN, T.NaN
                );
        return new Matrix3<T>(
            B2 * C3 - B3 * C2, A3 * C2 - A2 * C3, A2 * B3 - A3 * B2,
            B3 * C1 - B1 * C3, A1 * C3 - A3 * C1, A3 * B1 - A1 * B3,
            B1 * C2 - B2 * C1, A2 * C1 - A1 * C2, A1 * B2 - A2 * B1)
            / det;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix3<T> Invert(in Matrix3<T> matrix) {
        return matrix.Invert();
        }

    #endregion Inversion

    #region Determinants

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T GetDeterminant(in Matrix3<T> matrix) {
        return matrix.Determinant;
        }

    [BunnyAttributes.SIMDCandidate]
    public readonly T Determinant {
        get => A1 * B2 * C3 + B1 * C2 * A3 + C1 * A2 * B3 - A1 * C2 * B3 - C1 * B2 * A3 - B1 * A2 * C3;
        }

    #endregion Determinants

    #region Transposition

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Matrix3<T> Transpose() {
        return Transpose(in this);
        }

    [BunnyAttributes.SIMDCandidate]
    public static Matrix3<T> Transpose(in Matrix3<T> matrix) {
        return new(
            matrix.A1, matrix.B1, matrix.C1,
            matrix.A2, matrix.B2, matrix.C2,
            matrix.A3, matrix.B3, matrix.C3
            );
        }

    #endregion Transposition

    #region Transformation

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Vector3<T> Transform(Vector3<T> vector) {
        return Transform(this, vector);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3<T> Transform(in Matrix3<T> matrix, Vector3<T> vector) {
        Matrix.Transform(in matrix, vector, out Vector3<T> ret);
        return ret;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2<T> Transform(in Matrix3<T> matrix, Vector2<T> vector) {
        Matrix.Transform(in matrix, vector, out Vector2<T> ret);
        return ret;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Vector2<T> Transform(Vector2<T> vector) {
        return Transform(in this, vector);
        }

    #endregion Transformation

    #region Extraction

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3<T> GetScale(in Matrix3<T> matrix) {
        Matrix.GetScale(in matrix, out Vector3<T> scale);
        return scale;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Quaternion<T> GetRotation(in Matrix3<T> matrix) {
        Matrix.UnsafeConvert(in matrix, out Matrix4<T> m);
        Matrix.CreateQuaternionFromMatrix(in m, out var quaternion);
        return quaternion;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Vector3<T> GetScale() {
        return GetScale(in this);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Quaternion<T> GetRotation() {
        return GetRotation(in this);
        }

    #endregion Extraction

    #region Component Removal

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Remove2DScale(ref Matrix3<T> matrix) {
        Matrix.Remove2DScale(ref matrix);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Remove2DScale() {
        Matrix.Remove2DScale(ref this);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Remove3DScale(ref Matrix3<T> matrix) {
        Matrix.Remove3DScale(ref matrix);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Remove3DScale() {
        Matrix.Remove3DScale(ref this);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RemoveTranslation(ref Matrix3<T> matrix) {
        Matrix.Remove2DTranslation(ref matrix);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void RemoveTranslation() {
        Matrix.Remove2DTranslation(ref this);
        }

    #endregion Component Removal

    #region Equatability

    public bool Equals(Matrix3<T> other) {
        if (Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(float)) {
            var v0 = Unsafe.As<T, Vector256<float>>(ref _a1);
            var v1 = Unsafe.As<T, Vector256<float>>(ref other._a1);
            return Vector256.EqualsAll(v0, v1)
                && C3 == other.C3;
            }
        else if (Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double)) {
            var v00 = Unsafe.As<T, Vector256<double>>(ref _a1);
            var v01 = Unsafe.As<T, Vector256<double>>(ref _b2);
            var v10 = Unsafe.As<T, Vector256<double>>(ref other._a1);
            var v11 = Unsafe.As<T, Vector256<double>>(ref other._b2);
            return Vector256.EqualsAll(v00, v10)
                && Vector256.EqualsAll(v01, v11)
                && C3 == other.C3;
            }
        else if (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float)) {
            var v00 = Unsafe.As<T, Vector128<float>>(ref _a1);
            var v01 = Unsafe.As<T, Vector128<float>>(ref _b2);
            var v10 = Unsafe.As<T, Vector128<float>>(ref other._a1);
            var v11 = Unsafe.As<T, Vector128<float>>(ref other._b2);
            return Vector128.EqualsAll(v00, v10)
                && Vector128.EqualsAll(v01, v11)
                && C3 == other.C3;
            }
        else {
            return A1 == other.A1 && A2 == other.A2 && A3 == other.A3
                && B1 == other.B1 && B2 == other.B2 && B3 == other.B3
                && C1 == other.C1 && C2 == other.C2 && C3 == other.C3;
            }
        }

    public override bool Equals(object? obj) {
        return obj is Matrix3<T> matrix && Equals(matrix);
        }

    public override readonly int GetHashCode() {
        return HashCode.Combine(RowA.GetHashCode(), RowB.GetHashCode(), RowC.GetHashCode());
        }

    public static bool operator ==(Matrix3<T> left, Matrix3<T> right) {
        return left.Equals(right);
        }

    public static bool operator !=(Matrix3<T> left, Matrix3<T> right) {
        return left.Equals(right) == false;
        }

    #endregion Equatability

    #region Operators

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix3<T> operator *(T factor, in Matrix3<T> matrix) {
        return Scale(in matrix, factor);
        }

    [BunnyAttributes.SIMDCandidate]
    public static Matrix3<T> Add(in Matrix3<T> left, in Matrix3<T> right) {
        return new(
            left.A1 + right.A1, left.A2 + right.A2, left.A3 + right.A3,
            left.B1 + right.B1, left.B2 + right.B2, left.B3 + right.B3,
            left.C1 + right.C1, left.C2 + right.C2, left.C3 + right.C3
            );
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Matrix3<T> Add(in Matrix3<T> other) {
        return Add(this, other);
        }

    [BunnyAttributes.SIMDCandidate]
    public static Matrix3<T> Scale(in Matrix3<T> matrix, T factor) {
        return new(
            matrix.A1 * factor, matrix.A2 * factor, matrix.A3 * factor,
            matrix.B1 * factor, matrix.B2 * factor, matrix.B3 * factor,
            matrix.C1 * factor, matrix.C2 * factor, matrix.C3 * factor
            );
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Matrix3<T> Scale(T factor) {
        return Scale(this, factor);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Matrix3<T> Multiply(in Matrix3<T> other) {
        return Multiply(this, other);
        }

    [BunnyAttributes.SIMDCandidate]
    public static Matrix3<T> Multiply(in Matrix3<T> left, in Matrix3<T> right) {
        return new(
            left.A1 * right.A1 + left.A2 * right.B1 + left.A3 * right.C1,
            left.A1 * right.A2 + left.A2 * right.B2 + left.A3 * right.C2,
            left.A1 * right.A3 + left.A2 * right.B3 + left.A3 * right.C3,
            left.B1 * right.A1 + left.B2 * right.B1 + left.B3 * right.C1,
            left.B1 * right.A2 + left.B2 * right.B2 + left.B3 * right.C2,
            left.B1 * right.A3 + left.B2 * right.B3 + left.B3 * right.C3,
            left.C1 * right.A1 + left.C2 * right.B1 + left.C3 * right.C1,
            left.C1 * right.A2 + left.C2 * right.B2 + left.C3 * right.C2,
            left.C1 * right.A3 + left.C2 * right.B3 + left.C3 * right.C3
            );
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix3<T> operator +(in Matrix3<T> left, in Matrix3<T> right) {
        return Add(left, right);
        }

    [BunnyAttributes.SIMDCandidate]
    public static Matrix3<T> Subtract(in Matrix3<T> left, in Matrix3<T> right) {
        return new(
            left.A1 - right.A1, left.A2 - right.A2, left.A3 - right.A3,
            left.B1 - right.B1, left.B2 - right.B2, left.B3 - right.B3,
            left.C1 - right.C1, left.C2 - right.C2, left.C3 - right.C3
            );
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Matrix3<T> Subtract(in Matrix3<T> other) {
        return Subtract(this, other);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix3<T> operator -(in Matrix3<T> left, in Matrix3<T> right) {
        return Subtract(left, right);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix3<T> operator *(in Matrix3<T> left, in Matrix3<T> right) {
        return Multiply(left, right);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix3<T> operator /(in Matrix3<T> left, in Matrix3<T> right) {
        return Divide(in left, in right);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix3<T> Divide(in Matrix3<T> left, in Matrix3<T> right) {
        return Multiply(left, Invert(right));
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Matrix3<T> Divide(in Matrix3<T> other) {
        return Divide(in this, in other);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix3<T> operator *(in Matrix3<T> matrix, T value) {
        return Scale(matrix, value);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix3<T> operator /(in Matrix3<T> matrix, T value) {
        return Scale(matrix, T.One / value);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3<T> operator *(in Matrix3<T> matrix, Vector3<T> vector) {
        return Transform(matrix, vector);
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
        var c1 = C1.Stringify(digits, integerLength, paddingLength);
        var c2 = C2.Stringify(digits, integerLength, paddingLength);
        var c3 = C3.Stringify(digits, integerLength, paddingLength);

        return $"[ [{a1}, {a2}, {a3}],\n  [{b1}, {b2}, {b3}],\n  [{c1}, {c2}, {c3}] ]";
        }

    #endregion Strings

    #region Format Data

    public readonly T[][] RowsToJaggedArray() {
        return [[A1, A2, A3], [B1, B2, B3], [C1, C2, C3]];
        }
    public readonly T[][] ColumnsToJaggedArray() {
        return [[A1, B1, C1], [A2, B2, C2], [A3, B3, C3]];
        }

    public readonly T[,] RowsTo2DArray() {
        return new T[,] { { A1, A2, A3 }, { B1, B2, B3 }, { C1, C2, C3 } };
        }
    public readonly T[,] ColumnsTo2DArray() {
        return new T[,] { { A1, B1, C1 }, { A2, B2, C2 }, { A3, B3, C3 } };
        }

    #endregion Format Data

    #region Casting

    public static TMatrix UnsafeCast<TMatrix, TNumeric>(ref Matrix3<T> matrix)
        where TMatrix : unmanaged, IMatrix<TNumeric>
        where TNumeric : unmanaged, IBinaryFloatingPointIeee754<TNumeric>, IMinMaxValue<TNumeric> {
        Debug.Assert((typeof(TNumeric) == typeof(T)) && (Matrix3<T>.MatrixWidth == TMatrix.MatrixWidth) && (Matrix3<T>.MatrixHeight == TMatrix.MatrixHeight));
        return Unsafe.As<Matrix3<T>, TMatrix>(ref matrix);
        }

    public TMatrix UnsafeCast<TMatrix, TNumeric>()
            where TMatrix : unmanaged, IMatrix<TNumeric>
            where TNumeric : unmanaged, IBinaryFloatingPointIeee754<TNumeric>, IMinMaxValue<TNumeric> {
        Debug.Assert((typeof(TNumeric) == typeof(T)) && (Matrix3<T>.MatrixWidth == TMatrix.MatrixWidth) && (Matrix3<T>.MatrixHeight == TMatrix.MatrixHeight));
        return Unsafe.As<Matrix3<T>, TMatrix>(ref this);
        }

    #endregion Casting

    #region 2D Translation / Projection Matrices

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix3<T> CreateRotation(Angle<T> rotation) {
        Matrix.CreateRotation(rotation, out Matrix2<T> matrix);
        Matrix.UnsafeConvert(in matrix, out Matrix3<T> ret);
        return ret;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix3<T> CreateRotationScale(Angle<T> rotation, Vector2<T> scale) {
        Matrix.CreateRotationScale(rotation, scale, out Matrix3<T> matrix);
        return matrix;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix3<T> CreateScale(Vector2<T> scale) {
        Matrix.CreateScale(scale, out Matrix3<T> matrix);
        return matrix;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static Angle<T> IMatrix<T>.IMatrixBase<Matrix3<T>, Vector2<T>, Angle<T>, Angle<T>>.GetRotation(in Matrix3<T> matrix) {
        Matrix.GetRotation(in matrix, out Angle<T> rotation);
        return rotation;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static Vector2<T> IMatrix<T>.IMatrixBase<Matrix3<T>, Vector2<T>, Angle<T>, Angle<T>>.GetScale(in Matrix3<T> matrix) {
        Matrix.GetScale(in matrix, out Vector2<T> scale);
        return scale;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    readonly Angle<T> IMatrix<T>.IMatrixBase<Matrix3<T>, Vector2<T>, Angle<T>, Angle<T>>.GetRotation() {
        Matrix.GetRotation(in this, out Angle<T> rotation);
        return rotation;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    readonly Vector2<T> IMatrix<T>.IMatrixBase<Matrix3<T>, Vector2<T>, Angle<T>, Angle<T>>.GetScale() {
        Matrix.GetScale(in this, out Vector2<T> scale);
        return scale;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix3<T> CreateRotationScaleTranslation(Angle<T> rotation, Vector2<T> scale, Vector2<T> translation) {
        Matrix.CreateRotationScaleTranslation(rotation: rotation, scale: scale, translation: translation, out Matrix3<T> matrix);
        return matrix;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix3<T> CreateRotationTranslation(Angle<T> rotation, Vector2<T> translation) {
        Matrix.CreateRotationTranslation(rotation, translation, out Matrix3<T> matrix);
        return matrix;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix3<T> CreateScaleTranslation(Vector2<T> scale, Vector2<T> translation) {
        Matrix.CreateScaleTranslation(scale: scale, translation: translation, out Matrix3<T> matrix);
        return matrix;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix3<T> CreateTranslation(Vector2<T> translation) {
        Matrix.CreateTranslation(translation, out Matrix3<T> matrix);
        return matrix;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2<T> GetTranslation(in Matrix3<T> matrix) {
        Matrix.GetTranslation(in matrix, out Vector2<T> translation);
        return translation;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2<T> InvertedTransform(in Matrix3<T> matrix, Vector2<T> vector) {
        return Transform(Invert(in matrix), vector);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Vector2<T> GetTranslation() {
        return GetTranslation(in this);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Vector2<T> InvertedTransform(Vector2<T> vector) {
        return InvertedTransform(in this, vector);
        }

    #endregion 2D Translation / Projection Matrices
    }
