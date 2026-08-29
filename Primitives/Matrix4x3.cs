using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.Runtime.Intrinsics;
using Debug = System.Diagnostics.Debug;

namespace Bunnarium.Maths.Primitives;

#if MATRIX_PREMULTIPLIED_CONVENTION
/// <summary> A matrix with four rows and three columns.
/// </summary>
/// <remarks> This matrix implements <see cref="IMatrix{Numeric}.ITranslationMatrix{Matrix, Vector, Direction, Rotation}">ITranslationMatrix</see> because the <c>MATRIX_PREMULTIPLIED_CONVENTION</c> compiler symbol is set. For more information, please see <see cref="Matrix.Docs.MultiplicationConventionInstructions{T}"/>.
/// <para/><inheritdoc cref="Matrix.Docs.DisplayMatrix4x3"/>
/// </remarks>
#elif MATRIX_POSTMULTIPLIED_CONVENTION
/// <summary> A matrix with four rows and three columns.
/// </summary>
/// <remarks> This matrix does not implement <see cref="IMatrix{Numeric}.ITranslationMatrix{Matrix, Vector, Direction, Rotation}">ITranslationMatrix</see> because the <c>MATRIX_POSTMULTIPLIED_CONVENTION</c> compiler symbol is set. Because this is the case, translation-related functions for this type can only be accessed via the <see cref="Matrix"/> function library. For more information, please see <see cref="Matrix.Docs.MultiplicationConventionInstructions{T}"/>.
/// <para/><inheritdoc cref="Matrix.Docs.DisplayMatrix4x3"/>
/// </remarks>
#endif
[DebuggerDisplay("{ToString(), nq}")]
public struct Matrix4x3<T>
    : IMatrix<T>.IMatrixOfLinearWidth<Matrix4x3<T>, Vector3<T>>
#if MATRIX_PREMULTIPLIED_CONVENTION
    , IMatrix<T>.I3DTranslationMatrix<Matrix4x3<T>>
#endif
    where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {

    #region Data

    private T _a1, _a2, _a3, _b1, _b2, _b3, _c1, _c2, _c3, _d1, _d2, _d3;

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
    public T C3 { readonly get => _c3; set => _c3 = value; }

    /// <summary> The first element of the fourth row.
    /// </summary>
    public T D1 { readonly get => _d1; set => _d1 = value; }

    /// <summary> The second element of the fourth row.
    /// </summary>
    public T D2 { readonly get => _d2; set => _d2 = value; }

    /// <summary> The third element of the fourth row.
    /// </summary>
    public T D3 { readonly get => _d3; set => _d3 = value; }

    #endregion Cells

    #region Constructors

    /// <inheritdoc
    /// cref="Matrix4{T}.Matrix4(T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T)"/>
    public Matrix4x3(T a1, T a2, T a3, T b1, T b2, T b3, T c1, T c2, T c3, T d1, T d2, T d3) {
        _a1 = a1; _a2 = a2; _a3 = a3;
        _b1 = b1; _b2 = b2; _b3 = b3;
        _c1 = c1; _c2 = c2; _c3 = c3;
        _d1 = d1; _d2 = d2; _d3 = d3;
        }

    /// <summary> Creates a new <see cref="Matrix4x3{T}"/> as a <see cref="Matrix3{T}"/> with a fourth row, set to <c>(0, 0, 0)</c>, appended.
    /// </summary>
    public Matrix4x3(Matrix3<T> matrix) : this(
        matrix.A1, matrix.A2, matrix.A3,
        matrix.B1, matrix.B2, matrix.B3,
        matrix.C1, matrix.C2, matrix.C3,
        T.Zero, T.Zero, T.Zero
        ) { }



    /// <summary> Creates a <see cref="Matrix4x3{T}"/> such that its first three rows originate from the <paramref name="matrix"/> and its fourth is the passed <paramref name="rowD"/>.
    /// </summary>
    public Matrix4x3(Matrix3<T> matrix, Vector3<T> rowD) : this(
        matrix.A1, matrix.A2, matrix.A3,
        matrix.B1, matrix.B2, matrix.B3,
        matrix.C1, matrix.C2, matrix.C3,
        rowD.X, rowD.Y, rowD.Z
        ) { }


    /// <summary> Creates a <see cref="Matrix4x3{T}"/> from a <see cref="Matrix4{T}"/>, with the latter's fourth column omitted.
    /// </summary>
    public Matrix4x3(Matrix4<T> matrix) : this(
        matrix.A1, matrix.A2, matrix.A3,
        matrix.B1, matrix.B2, matrix.B3,
        matrix.C1, matrix.C2, matrix.C3,
        matrix.D1, matrix.D2, matrix.D3
        ) { }


    /// <summary> Creates a matrix from vector-based rows.
    /// </summary>
    public Matrix4x3(Vector3<T> rowA, Vector3<T> rowB, Vector3<T> rowC, Vector3<T> rowD) : this(
        rowA.X, rowA.Y, rowA.Z,
        rowB.X, rowB.Y, rowB.Z,
        rowC.X, rowC.Y, rowC.Z,
        rowD.X, rowD.Y, rowD.Z
        ) { }

    #endregion Constructors

    #region Factories

#if MATRIX_PREMULTIPLIED_CONVENTION

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix4x3<T> CreateScaleTranslation(Vector3<T> scale, Vector3<T> translation) {
        Matrix.CreatePremultipliedScaleTranslation(scale: scale, translation: translation, out Matrix4x3<T> matrix);
        return matrix;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix4x3<T> CreateRotationTranslation(Quaternion<T> rotation, Vector3<T> translation) {
        Matrix.CreatePremultipliedRotationTranslation(rotation, translation, out Matrix4x3<T> matrix);
        return matrix;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix4x3<T> CreateRotationScaleTranslation(Quaternion<T> rotation, Vector3<T> scale, Vector3<T> translation) {
        Matrix.CreatePremultipliedRotationScaleTranslation(rotation: rotation, scale: scale, translation: translation, out Matrix4x3<T> matrix);
        return matrix;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix4x3<T> CreateTranslation(Vector3<T> translation) {
        Matrix.CreatePremultipliedTranslation(translation, out Matrix4x3<T> matrix);
        return matrix;
        }

#endif

    public static Matrix4x3<T> Identity { get; } = new(
        +T.One, T.Zero, T.Zero,
        T.Zero, +T.One, T.Zero,
        T.Zero, T.Zero, +T.One,
        T.Zero, T.Zero, T.Zero
        );

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix4x3<T> CreateRotation(Quaternion<T> rotation) {
        Matrix.CreateRotation(rotation, out Matrix4<T> matrix);
        Matrix.UnsafeConvert(in matrix, out Matrix4x3<T> ret);
        return ret;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix4x3<T> CreateScale(Vector3<T> scale) {
        Matrix.CreatePremultipliedScale(scale, out Matrix4x3<T> matrix);
        return matrix;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix4x3<T> CreateRotationScale(Quaternion<T> rotation, Vector3<T> scale) {
        Matrix.CreatePremultipliedRotationScale(rotation, scale, out Matrix4x3<T> matrix);
        return matrix;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix4x3<T> CreateRotationX(Angle<T> angle) {
        Matrix.CreateRotationX(angle.Radians, out Matrix4<T> matrix);
        Matrix.UnsafeConvert(in matrix, out Matrix4x3<T> ret);
        return ret;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix4x3<T> CreateRotationY(Angle<T> angle) {
        Matrix.CreateRotationY(angle.Radians, out Matrix4<T> matrix);
        Matrix.UnsafeConvert(in matrix, out Matrix4x3<T> ret);
        return ret;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix4x3<T> CreateRotationZ(Angle<T> angle) {
        Matrix.CreateRotationZ(angle.Radians, out Matrix4<T> matrix);
        Matrix.UnsafeConvert(in matrix, out Matrix4x3<T> ret);
        return ret;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix4x3<T> CreateAxisAngle(Direction<T> axis, Angle<T> angle) {
        Matrix.AxisAngle(axis, angle, out Matrix4<T> matrix);
        Matrix.UnsafeConvert(in matrix, out Matrix4x3<T> ret);
        return ret;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix4x3<T> CreateRotationXYZ(Angle<T> rotationX, Angle<T> rotationY, Angle<T> rotationZ) {
        Matrix.CreateRotationXYZ(rotationX, rotationY, rotationZ, out Matrix4<T> matrix);
        Matrix.UnsafeConvert(in matrix, out Matrix4x3<T> ret);
        return ret;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix4x3<T> CreateRotationXZY(Angle<T> rotationX, Angle<T> rotationZ, Angle<T> rotationY) {
        Matrix.CreateRotationXZY(rotationX, rotationZ, rotationY, out Matrix4<T> matrix);
        Matrix.UnsafeConvert(in matrix, out Matrix4x3<T> ret);
        return ret;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix4x3<T> CreateRotationYXZ(Angle<T> rotationY, Angle<T> rotationX, Angle<T> rotationZ) {
        Matrix.CreateRotationYXZ(rotationY, rotationX, rotationZ, out Matrix4<T> matrix);
        Matrix.UnsafeConvert(in matrix, out Matrix4x3<T> ret);
        return ret;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix4x3<T> CreateRotationYZX(Angle<T> rotationY, Angle<T> rotationZ, Angle<T> rotationX) {
        Matrix.CreateRotationYZX(rotationY, rotationZ, rotationX, out Matrix4<T> matrix);
        Matrix.UnsafeConvert(in matrix, out Matrix4x3<T> ret);
        return ret;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix4x3<T> CreateRotationZYX(Angle<T> rotationZ, Angle<T> rotationY, Angle<T> rotationX) {
        Matrix.CreateRotationZYX(rotationZ, rotationY, rotationX, out Matrix4<T> matrix);
        Matrix.UnsafeConvert(in matrix, out Matrix4x3<T> ret);
        return ret;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix4x3<T> CreateRotationZXY(Angle<T> rotationZ, Angle<T> rotationX, Angle<T> rotationY) {
        Matrix.CreateRotationZXY(rotationZ, rotationX, rotationY, out Matrix4<T> matrix);
        Matrix.UnsafeConvert(in matrix, out Matrix4x3<T> ret);
        return ret;
        }

    #endregion Factories

    #region Rows & Columns

    /// <summary> Returns <see cref="Matrix4x3{T}.RowA"/> of the <paramref name="matrix"/> by <see langword="ref"/>.
    /// </summary>
    public static ref Vector3<T> GetRowA(ref Matrix4x3<T> matrix) {
        return ref Unsafe.As<T, Vector3<T>>(ref matrix._a1);
        }

    /// <summary> Returns <see cref="Matrix4x3{T}.RowB"/> of the <paramref name="matrix"/> by <see langword="ref"/>.
    /// </summary>
    public static ref Vector3<T> GetRowB(ref Matrix4x3<T> matrix) {
        return ref Unsafe.As<T, Vector3<T>>(ref matrix._b1);
        }

    /// <summary> Returns <see cref="Matrix4x3{T}.RowC"/> of the <paramref name="matrix"/> by <see langword="ref"/>.
    /// </summary>
    public static ref Vector3<T> GetRowC(ref Matrix4x3<T> matrix) {
        return ref Unsafe.As<T, Vector3<T>>(ref matrix._c1);
        }

    /// <summary> Returns <see cref="Matrix4x3{T}.RowD"/> of the <paramref name="matrix"/> by <see langword="ref"/>.
    /// </summary>
    public static ref Vector3<T> GetRowD(ref Matrix4x3<T> matrix) {
        return ref Unsafe.As<T, Vector3<T>>(ref matrix._d1);
        }

    public static int MatrixWidth { get; } = 3;

    public static int MatrixHeight { get; } = 4;

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
    public unsafe Vector3<T> RowD {
        readonly get => new(D1, D2, D3);
        set => Unsafe.Copy(Unsafe.AsPointer(ref _c1), ref value);
        }
    public readonly byte Width => 3;
    public readonly byte Height => 4;

    #endregion Rows & Columns

    #region Inversion

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix4x3<T> Invert(in Matrix4x3<T> matrix) {
        return matrix.Invert();
        }

    [BunnyAttributes.SIMDCandidate]
    public readonly Matrix4x3<T> Invert() {
        var det = Determinant;
        if (T.Abs(det) < T.Epsilon)
            return new(
                T.NaN, T.NaN, T.NaN,
                T.NaN, T.NaN, T.NaN,
                T.NaN, T.NaN, T.NaN,
                T.NaN, T.NaN, T.NaN
                );
        det = T.One / det;
        return new(
            a1: (-C2 * B3 + B2 * C3) * det,
            a2: (+C2 * A3 - A2 * C3) * det,
            a3: (-B2 * A3 + A2 * B3) * det,
            b1: (+C1 * B3 - B1 * C3) * det,
            b2: (-C1 * A3 + A1 * C3) * det,
            b3: (+B1 * A3 - A1 * B3) * det,
            c1: (-C1 * B2 + B1 * C2) * det,
            c2: (+C1 * A2 - A1 * C2) * det,
            c3: (-B1 * A2 + A1 * B2) * det,
            d1: (-B1 * C2 * D3 + B1 * C3 * D2 + C1 * B2 * D3 - C1 * B3 * D2 - D1 * B2 * C3 + D1 * B3 * C2) * det,
            d2: (+A1 * C2 * D3 - A1 * C3 * D2 - C1 * A2 * D3 + C1 * A3 * D2 + D1 * A2 * C3 - D1 * A3 * C2) * det,
            d3: (-A1 * B2 * D3 + A1 * B3 * D2 + B1 * A2 * D3 - B1 * A3 * D2 - D1 * A2 * B3 + D1 * A3 * B2) * det
            );
        }

    #endregion Inversion

    #region Determinants

    public readonly T Determinant {
        get => A1 * (B2 * C3 - C2 * B3) + B1 * (C2 * A3 - A2 * C3) + C1 * (A2 * B3 - B2 * A3);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T GetDeterminant(in Matrix4x3<T> matrix) {
        return matrix.Determinant;
        }

    #endregion Determinants

    #region Transformation

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Vector3<T> Transform(Vector3<T> vector) {
        return Transform(this, vector);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3<T> Transform(in Matrix4x3<T> matrix, Vector3<T> vector) {
        Matrix.TransformPremultipliedRowVector(vector, in matrix, out Vector3<T> ret);
        return ret;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Vector3<T> InvertedTransform(Vector3<T> vector) {
        return InvertedTransform(in this, vector);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3<T> InvertedTransform(in Matrix4x3<T> matrix, Vector3<T> vector) {
        return Transform(Invert(in matrix), vector);
        }

    #endregion Transformation

    #region Extraction

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Vector3<T> GetScale() {
        return GetScale(in this);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3<T> GetScale(in Matrix4x3<T> matrix) {
        Matrix.GetScaleFromPremultiplied(in matrix, out Vector3<T> scale);
        return scale;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Quaternion<T> GetRotation() {
        return GetRotation(in this);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Quaternion<T> GetRotation(in Matrix4x3<T> matrix) {
        Matrix.GetRotationFromPremultiplied(in matrix, out Quaternion<T> quaternion);
        return quaternion;
        }

#if MATRIX_PREMULTIPLIED_CONVENTION

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Vector3<T> GetTranslation() {
        return GetTranslation(in this);
        }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3<T> GetTranslation(in Matrix4x3<T> matrix) {
        Matrix.GetTranslationFromPremultiplied(in matrix, out Vector3<T> translation);
        return translation;
        }

#endif

    #endregion Extraction

    #region Component Removal

#if MATRIX_PREMULTIPLIED_CONVENTION

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RemoveTranslation(ref Matrix4x3<T> matrix) {
        Matrix.Remove3DTranslation(ref matrix);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void RemoveTranslation() {
        Matrix.Remove3DTranslation(ref this);
        }

#endif

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Remove3DScale(ref Matrix4x3<T> matrix) {
        Matrix.Remove3DScale(ref matrix);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Remove3DScale() {
        Matrix.Remove3DScale(ref this);
        }

    #endregion Component Removal

    #region Equatability

    public bool Equals(Matrix4x3<T> other) {
        if (Vector512.IsHardwareAccelerated && Vector512<T>.IsSupported && typeof(T) == typeof(double)) {
            var v00 = Unsafe.As<T, Vector512<double>>(ref _a1);
            var v01 = Unsafe.As<T, Vector512<double>>(ref _b2);
            var v10 = Unsafe.As<T, Vector512<double>>(ref other._a1);
            var v11 = Unsafe.As<T, Vector512<double>>(ref other._b2);
            return Vector512.EqualsAll(v00, v10)
                && Vector512.EqualsAll(v01, v11);
            }
        else if (Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(float)) {
            var v00 = Unsafe.As<T, Vector256<float>>(ref _a1);
            var v01 = Unsafe.As<T, Vector256<float>>(ref _b2);
            var v10 = Unsafe.As<T, Vector256<float>>(ref other._a1);
            var v11 = Unsafe.As<T, Vector256<float>>(ref other._b2);
            return Vector256.EqualsAll(v00, v10)
                && Vector256.EqualsAll(v01, v11);
            }
        else if (Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double)) {
            var v00 = Unsafe.As<T, Vector256<double>>(ref _a1);
            var v01 = Unsafe.As<T, Vector256<double>>(ref _b2);
            var v02 = Unsafe.As<T, Vector256<double>>(ref _c3);
            var v10 = Unsafe.As<T, Vector256<double>>(ref other._a1);
            var v11 = Unsafe.As<T, Vector256<double>>(ref other._b2);
            var v12 = Unsafe.As<T, Vector256<double>>(ref other._c3);
            return Vector256.EqualsAll(v00, v10)
                && Vector256.EqualsAll(v01, v11)
                && Vector256.EqualsAll(v02, v12);
            }
        else if (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float)) {
            var v00 = Unsafe.As<T, Vector128<float>>(ref _a1);
            var v01 = Unsafe.As<T, Vector128<float>>(ref _b2);
            var v02 = Unsafe.As<T, Vector128<float>>(ref _c3);
            var v10 = Unsafe.As<T, Vector128<float>>(ref other._a1);
            var v11 = Unsafe.As<T, Vector128<float>>(ref other._b2);
            var v12 = Unsafe.As<T, Vector128<float>>(ref other._c3);
            return Vector128.EqualsAll(v00, v10)
                && Vector128.EqualsAll(v01, v11)
                && Vector128.EqualsAll(v02, v12);
            }
        else {
            return A1 == other.A1 && A2 == other.A2 && A3 == other.A3
                && B1 == other.B1 && B2 == other.B2 && B3 == other.B3
                && C1 == other.C1 && C2 == other.C2 && C3 == other.C3
                && D1 == other.D1 && D2 == other.D2 && D3 == other.D3;
            }
        }

    public override bool Equals(object? obj) {
        return obj is Matrix4x3<T> matrix && Equals(matrix);
        }

    public override readonly int GetHashCode() {
        return HashCode.Combine(RowA.GetHashCode(), RowB.GetHashCode(), RowC.GetHashCode(), RowD.GetHashCode());
        }

    public static bool operator ==(Matrix4x3<T> left, Matrix4x3<T> right) {
        return left.Equals(right);
        }

    public static bool operator !=(Matrix4x3<T> left, Matrix4x3<T> right) {
        return left.Equals(right) == false;
        }

    #endregion Equatability

    #region Operators

    [BunnyAttributes.SIMDCandidate]
    public static Matrix4x3<T> Scale(in Matrix4x3<T> matrix, T factor) {
        return new(
            matrix.A1 * factor, matrix.A2 * factor, matrix.A3 * factor,
            matrix.B1 * factor, matrix.B2 * factor, matrix.B3 * factor,
            matrix.C1 * factor, matrix.C2 * factor, matrix.C3 * factor,
            matrix.D1 * factor, matrix.D2 * factor, matrix.D3 * factor
            );
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix4x3<T> operator *(T factor, in Matrix4x3<T> matrix) {
        return Scale(in matrix, factor);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Matrix4x3<T> Scale(T factor) {
        return Scale(this, factor);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix4x3<T> operator +(in Matrix4x3<T> left, in Matrix4x3<T> right) {
        return Add(left, right);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix4x3<T> operator -(in Matrix4x3<T> left, in Matrix4x3<T> right) {
        return Subtract(left, right);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3<T> operator *(in Matrix4x3<T> matrix, Vector3<T> vector) {
        return Transform(matrix, vector);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix4x3<T> operator *(in Matrix4x3<T> left, in Matrix4x3<T> right) {
        return Multiply(left, right);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Matrix4x3<T> Multiply(in Matrix4x3<T> other) {
        return Multiply(this, other);
        }

    [BunnyAttributes.SIMDCandidate]
    public static Matrix4x3<T> Multiply(in Matrix4x3<T> left, in Matrix4x3<T> right) {
        return new(
            left.A1 * right.A1 + left.A2 * right.B1 + left.A3 * right.C1,
            left.A1 * right.A2 + left.A2 * right.B2 + left.A3 * right.C2,
            left.A1 * right.A3 + left.A2 * right.B3 + left.A3 * right.C3,
            left.B1 * right.A1 + left.B2 * right.B1 + left.B3 * right.C1,
            left.B1 * right.A2 + left.B2 * right.B2 + left.B3 * right.C2,
            left.B1 * right.A3 + left.B2 * right.B3 + left.B3 * right.C3,
            left.C1 * right.A1 + left.C2 * right.B1 + left.C3 * right.C1,
            left.C1 * right.A2 + left.C2 * right.B2 + left.C3 * right.C2,
            left.C1 * right.A3 + left.C2 * right.B3 + left.C3 * right.C3,
            left.D1 * right.A1 + left.D2 * right.B1 + left.D3 * right.C1 + right.D1,
            left.D1 * right.A2 + left.D2 * right.B2 + left.D3 * right.C2 + right.D2,
            left.D1 * right.A3 + left.D2 * right.B3 + left.D3 * right.C3 + right.D3
            );
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix4x3<T> operator *(in Matrix4x3<T> matrix, T value) {
        return Scale(matrix, value);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix4x3<T> operator /(in Matrix4x3<T> left, in Matrix4x3<T> right) {
        return Divide(in left, in right);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix4x3<T> Divide(in Matrix4x3<T> left, in Matrix4x3<T> right) {
        return Multiply(left, Invert(right));
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Matrix4x3<T> Divide(in Matrix4x3<T> other) {
        return Divide(in this, in other);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix4x3<T> operator /(in Matrix4x3<T> matrix, T value) {
        return Scale(matrix, T.One / value);
        }

    [BunnyAttributes.SIMDCandidate]
    public static Matrix4x3<T> Add(in Matrix4x3<T> left, in Matrix4x3<T> right) {
        return new(
            left.A1 + right.A1, left.A2 + right.A2, left.A3 + right.A3,
            left.B1 + right.B1, left.B2 + right.B2, left.B3 + right.B3,
            left.C1 + right.C1, left.C2 + right.C2, left.C3 + right.C3,
            left.D1 + right.D1, left.D2 + right.D2, left.D3 + right.D3
            );
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Matrix4x3<T> Add(in Matrix4x3<T> other) {
        return Add(this, other);
        }

    [BunnyAttributes.SIMDCandidate]
    public static Matrix4x3<T> Subtract(in Matrix4x3<T> left, in Matrix4x3<T> right) {
        return new(
            left.A1 - right.A1, left.A2 - right.A2, left.A3 - right.A3,
            left.B1 - right.B1, left.B2 - right.B2, left.B3 - right.B3,
            left.C1 - right.C1, left.C2 - right.C2, left.C3 - right.C3,
            left.D1 - right.D1, left.D2 - right.D2, left.D3 - right.D3
            );
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Matrix4x3<T> Subtract(in Matrix4x3<T> other) {
        return Subtract(this, other);
        }

    /// <inheritdoc
    /// cref="IMatrix{Numeric}.ISquareMatrix{Matrix}.Multiply(in Matrix, in Matrix)"/>
    [BunnyAttributes.SIMDCandidate]
    public static Matrix4<T> Multiply(in Matrix4x3<T> left, in Matrix3x4<T> right) {
        return new(
            left.A1 * right.A1 + left.A2 * right.B1 + left.A3 * right.C1,
            left.A1 * right.A2 + left.A2 * right.B2 + left.A3 * right.C2,
            left.A1 * right.A3 + left.A2 * right.B3 + left.A3 * right.C3,
            left.A1 * right.A4 + left.A2 * right.B4 + left.A3 * right.C4,
            left.B1 * right.A1 + left.B2 * right.B1 + left.B3 * right.C1,
            left.B1 * right.A2 + left.B2 * right.B2 + left.B3 * right.C2,
            left.B1 * right.A3 + left.B2 * right.B3 + left.B3 * right.C3,
            left.B1 * right.A4 + left.B2 * right.B4 + left.B3 * right.C4,
            left.C1 * right.A1 + left.C2 * right.B1 + left.C3 * right.C1,
            left.C1 * right.A2 + left.C2 * right.B2 + left.C3 * right.C2,
            left.C1 * right.A3 + left.C2 * right.B3 + left.C3 * right.C3,
            left.C1 * right.A4 + left.C2 * right.B4 + left.C3 * right.C4,
            left.D1 * right.A1 + left.D2 * right.B1 + left.D3 * right.C1,
            left.D1 * right.A2 + left.D2 * right.B2 + left.D3 * right.C2,
            left.D1 * right.A3 + left.D2 * right.B3 + left.D3 * right.C3,
            left.D1 * right.A4 + left.D2 * right.B4 + left.D3 * right.C4
            );
        }

    /// <inheritdoc
    /// cref="Multiply(in Matrix4x3{T}, in Matrix3x4{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Matrix4<T> Multiply(in Matrix3x4<T> other) {
        return Multiply(in this, in other);
        }

    /// <inheritdoc
    /// cref="Multiply(in Matrix4x3{T}, in Matrix3x4{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix4<T> operator *(in Matrix4x3<T> left, in Matrix3x4<T> right) {
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
        var c1 = C1.Stringify(digits, integerLength, paddingLength);
        var c2 = C2.Stringify(digits, integerLength, paddingLength);
        var c3 = C3.Stringify(digits, integerLength, paddingLength);
        var d1 = D1.Stringify(digits, integerLength, paddingLength);
        var d2 = D2.Stringify(digits, integerLength, paddingLength);
        var d3 = D3.Stringify(digits, integerLength, paddingLength);

        return $"[ [{a1}, {a2}, {a3}],\n  [{b1}, {b2}, {b3}],\n  [{c1}, {c2}, {c3}],\n  [{d1}, {d2}, {d3}] ]";
        }

    #endregion Strings

    #region Format Data

    public readonly T[][] RowsToJaggedArray() {
        return [[A1, A2, A3], [B1, B2, B3], [C1, C2, C3], [D1, D2, D3]];
        }

    public readonly T[][] ColumnsToJaggedArray() {
        return [[A1, B1, C1, D1], [A2, B2, C2, D2], [A3, B3, C3, D3]];
        }
    public readonly T[,] RowsTo2DArray() {
        return new T[,] { { A1, A2, A3 }, { B1, B2, B3 }, { C1, C2, C3 }, { D1, D2, D3 } };
        }
    public readonly T[,] ColumnsTo2DArray() {
        return new T[,] { { A1, B1, C1, D1 }, { A2, B2, C2, D2 }, { A3, B3, C3, D3 } };
        }

    #endregion Format Data

    #region Casting

    public static TMatrix UnsafeCast<TMatrix, TNumeric>(ref Matrix4x3<T> matrix)
        where TMatrix : unmanaged, IMatrix<TNumeric>
        where TNumeric : unmanaged, IBinaryFloatingPointIeee754<TNumeric>, IMinMaxValue<TNumeric> {
        Debug.Assert((typeof(TNumeric) == typeof(T)) && (Matrix4x3<T>.MatrixWidth == TMatrix.MatrixWidth) && (Matrix4x3<T>.MatrixHeight == TMatrix.MatrixHeight));
        return Unsafe.As<Matrix4x3<T>, TMatrix>(ref matrix);
        }

    public TMatrix UnsafeCast<TMatrix, TNumeric>()
        where TMatrix : unmanaged, IMatrix<TNumeric>
        where TNumeric : unmanaged, IBinaryFloatingPointIeee754<TNumeric>, IMinMaxValue<TNumeric> {
        Debug.Assert((typeof(TNumeric) == typeof(T)) && (Matrix4x3<T>.MatrixWidth == TMatrix.MatrixWidth) && (Matrix4x3<T>.MatrixHeight == TMatrix.MatrixHeight));
        return Unsafe.As<Matrix4x3<T>, TMatrix>(ref this);
        }

    #endregion Casting
    }
