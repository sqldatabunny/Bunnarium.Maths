using Bunnarium.Tools.Utilities;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using Debug = System.Diagnostics.Debug;
using static Bunnarium.Tools.Utilities.SIMD;

namespace Bunnarium.Maths.Primitives;

/// <summary> A matrix with four rows and four columns.
/// </summary>
/// <remarks><para/><inheritdoc cref="Matrix.Docs.DisplayMatrix4"/>
/// </remarks>
[DebuggerDisplay("{ToString(), nq}")]
public struct Matrix4<T>
    : IMatrix<T>.ISquareMatrix<Matrix4<T>>
    , IMatrix<T>.I3DProjectionMatrix<Matrix4<T>>
    , IMatrix<T>.IMatrixOfLinearWidth<Matrix4<T>, Vector4<T>>
    where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> {

    #region Data

    private T _a1, _a2, _a3, _a4, _b1, _b2, _b3, _b4, _c1, _c2, _c3, _c4, _d1, _d2, _d3, _d4; // DO NOT CHANGE ORDER

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

    /// <summary> The fourth element of the first row.
    /// </summary>
    public T A4 { readonly get => _a4; set => _a4 = value; }

    /// <summary> The first element of the second row.
    /// </summary>
    public T B1 { readonly get => _b1; set => _b1 = value; }

    /// <summary> The second element of the second row.
    /// </summary>
    public T B2 { readonly get => _b2; set => _b2 = value; }

    /// <summary> The third element of the second row.
    /// </summary>
    public T B3 { readonly get => _b3; set => _b3 = value; }

    /// <summary> The fourth element of the second row.
    /// </summary>
    public T B4 { readonly get => _b4; set => _b4 = value; }

    /// <summary> The first element of the third row.
    /// </summary>
    public T C1 { readonly get => _c1; set => _c1 = value; }

    /// <summary> The second element of the third row.
    /// </summary>
    public T C2 { readonly get => _c2; set => _c2 = value; }

    /// <summary> The third element of the third row.
    /// </summary>
    public T C3 { readonly get => _c3; set => _c3 = value; }

    /// <summary> The fourth element of the third row.
    /// </summary>
    public T C4 { readonly get => _c4; set => _c4 = value; }

    /// <summary> The first element of the fourth row.
    /// </summary>
    public T D1 { readonly get => _d1; set => _d1 = value; }

    /// <summary> The second element of the fourth row.
    /// </summary>
    public T D2 { readonly get => _d2; set => _d2 = value; }

    /// <summary> The third element of the fourth row.
    /// </summary>
    public T D3 { readonly get => _d3; set => _d3 = value; }

    /// <summary> The fourth element of the fourth row.
    /// </summary>
    public T D4 { readonly get => _d4; set => _d4 = value; }

    #endregion Cells

    #region Constructors

    /// <summary> Creates a new matrix.
    /// </summary>
    /// <param name="a1"> The first element in the first row.</param>
    /// <param name="a2"> The second element in the first row.</param>
    /// <param name="a3"> The third element in the first row.</param>
    /// <param name="a4"> The fourth element in the first row.</param>
    /// <param name="b1"> The first element in the second row.</param>
    /// <param name="b2"> The second element in the second row.</param>
    /// <param name="b3"> The third element in the second row.</param>
    /// <param name="b4"> The fourth element in the second row.</param>
    /// <param name="c1"> The first element in the third row.</param>
    /// <param name="c2"> The second element in the third row.</param>
    /// <param name="c3"> The third element in the third row.</param>
    /// <param name="c4"> The fourth element in the third row.</param>
    /// <param name="d1"> The first element in the fourth row.</param>
    /// <param name="d2"> The second element in the fourth row.</param>
    /// <param name="d3"> The third element in the fourth row.</param>
    /// <param name="d4"> The fourth element in the fourth row.</param>
    public Matrix4(T a1, T a2, T a3, T a4, T b1, T b2, T b3, T b4, T c1, T c2, T c3, T c4, T d1, T d2, T d3, T d4) {
        _a1 = a1; _a2 = a2; _a3 = a3; _a4 = a4;
        _b1 = b1; _b2 = b2; _b3 = b3; _b4 = b4;
        _c1 = c1; _c2 = c2; _c3 = c3; _c4 = c4;
        _d1 = d1; _d2 = d2; _d3 = d3; _d4 = d4;
        }

    /// <summary> Creates an affine transformation matrix from a <see cref="Matrix4x3{T}"/>, such that its first three columns are populated by the input <paramref name="matrix"/> and its fourth column is <c>[0, 0, 0, 1]</c>.
    /// </summary>
    public Matrix4(Matrix4x3<T> matrix) : this(
        matrix.A1, matrix.A2, matrix.A3, T.Zero,
        matrix.B1, matrix.B2, matrix.B3, T.Zero,
        matrix.C1, matrix.C2, matrix.C3, T.Zero,
        matrix.D1, matrix.D2, matrix.D3, T.One
        ) { }

    /// <summary> Creates an affine transformation matrix from a <see cref="Matrix3{T}"/>, such that its first three rows and columns are populated by the input <paramref name="matrix"/> and its fourth row and column column are both <c>[0, 0, 0, 1]</c>.
    /// </summary>
    public Matrix4(Matrix3<T> matrix) : this(
        matrix.A1, matrix.A2, matrix.A3, T.Zero,
        matrix.B1, matrix.B2, matrix.B3, T.Zero,
        matrix.C1, matrix.C2, matrix.C3, T.Zero,
        T.Zero, T.Zero, T.Zero, T.One
        ) { }

    /// <summary> Creates a <see cref="Matrix4{T}"/> such that its first three columns originate from the <paramref name="matrix"/> and its fourth is the passed <paramref name="columnD"/>.
    /// </summary>
    public Matrix4(Matrix4x3<T> matrix, Vector4<T> columnD) : this(
        matrix.A1, matrix.A2, matrix.A3, columnD.X,
        matrix.B1, matrix.B2, matrix.B3, columnD.Y,
        matrix.C1, matrix.C2, matrix.C3, columnD.Z,
        matrix.D1, matrix.D2, matrix.D3, columnD.W
        ) { }

    /// <summary> Creates an affine transformation matrix from a <see cref="Matrix3x4{T}"/>, such that its first three rows are populated by the input <paramref name="matrix"/> and its fourth row is <c>[0, 0, 0, 1]</c>.
    /// </summary>
    public Matrix4(Matrix3x4<T> matrix) : this(
        matrix.A1, matrix.A2, matrix.A3, matrix.A4,
        matrix.B1, matrix.B2, matrix.B3, matrix.B4,
        matrix.C1, matrix.C2, matrix.C3, matrix.C4,
        T.Zero, T.Zero, T.Zero, T.One
        ) { }

    /// <summary> Creates a <see cref="Matrix4{T}"/> such that its first three rows originate from the <paramref name="matrix"/> and its fourth is the passed <paramref name="rowD"/>.
    /// </summary>
    public Matrix4(Matrix3x4<T> matrix, Vector4<T> rowD) : this(
        matrix.A1, matrix.A2, matrix.A3, matrix.A4,
        matrix.B1, matrix.B2, matrix.B3, matrix.B4,
        matrix.C1, matrix.C2, matrix.C3, matrix.C4,
        rowD.X, rowD.Y, rowD.Z, rowD.W
        ) { }

    /// <summary> Creates a matrix from vector-based rows.
    /// </summary>
    public Matrix4(Vector4<T> rowA, Vector4<T> rowB, Vector4<T> rowC, Vector4<T> rowD) : this(
        rowA.X, rowA.Y, rowA.Z, rowA.W,
        rowB.X, rowB.Y, rowB.Z, rowB.W,
        rowC.X, rowC.Y, rowC.Z, rowC.W,
        rowD.X, rowD.Y, rowD.Z, rowD.W
        ) { }

    #endregion Constructors

    #region Factories

    public static Matrix4<T> Identity { get; } = new(
        +T.One, T.Zero, T.Zero, T.Zero,
        T.Zero, +T.One, T.Zero, T.Zero,
        T.Zero, T.Zero, +T.One, T.Zero,
        T.Zero, T.Zero, T.Zero, +T.One
        );

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix4<T> CreateScale(Vector3<T> scale) {
        Matrix.CreateScale(scale: scale, out Matrix4<T> matrix);
        return matrix;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix4<T> CreateRotation(Quaternion<T> rotation) {
        Matrix.CreateRotation(rotation, out Matrix4<T> matrix);
        return matrix;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix4<T> CreateRotationScale(Quaternion<T> rotation, Vector3<T> scale) {
        Matrix.CreateRotationScale(rotation: rotation, scale: scale, out Matrix4<T> matrix);
        return matrix;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix4<T> CreateRotationTranslation(Quaternion<T> rotation, Vector3<T> translation) {
        Matrix.CreateRotationTranslation(rotation: rotation, translation: translation, out var matrix);
        return matrix;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix4<T> CreateRotationScaleTranslation(Quaternion<T> rotation, Vector3<T> scale, Vector3<T> translation) {
        Matrix.CreateRotationScaleTranslation(rotation: rotation, scale: scale, translation: translation, out var matrix);
        return matrix;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix4<T> CreateRotationX(Angle<T> angle) {
        Matrix.CreateRotationX(angle.Radians, out Matrix4<T> matrix);
        return matrix;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix4<T> CreateRotationY(Angle<T> angle) {
        Matrix.CreateRotationY(angle.Radians, out Matrix4<T> matrix);
        return matrix;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix4<T> CreateRotationZ(Angle<T> angle) {
        Matrix.CreateRotationZ(angle.Radians, out Matrix4<T> matrix);
        return matrix;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix4<T> CreateAxisAngle(Direction<T> axis, Angle<T> angle) {
        Matrix.AxisAngle(axis, angle, out Matrix4<T> matrix);
        return matrix;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix4<T> CreateReflection(Plane3<T> plane) {
        Matrix.CreateReflection(plane, out var matrix);
        return matrix;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix4<T> CreateShadow(Vector3<T> lightSource, Vector3<T> lightTarget, Plane3<T> plane) {
        Matrix.CreateShadow(lightSource, lightTarget, plane, out var matrix);
        return matrix;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix4<T> CreateShadow(Vector3<T> lightDirection, Plane3<T> plane) {
        Matrix.CreateShadow(lightDirection, plane, out var matrix);
        return matrix;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix4<T> CreateLookAt(Vector3<T> cameraPosition, Vector3<T> cameraTarget, Vector3<T> cameraUp) {
        Matrix.CreateLookAt(cameraPosition, cameraTarget, cameraUp, out Matrix4<T> matrix);
        return matrix;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix4<T> CreateOrthographic(T width, T height, T nearPlane, T farPlane) {
        Matrix.CreateOrthographic(width, height, nearPlane, farPlane, out Matrix4<T> matrix);
        return matrix;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix4<T> CreateOrthographicOffCenter(T left, T right, T top, T bottom, T nearPlane, T farPlane) {
        Matrix.CreateOrthographicOffCenter(left, right, top, bottom, nearPlane, farPlane, out Matrix4<T> matrix);
        return matrix;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix4<T> CreatePerspective(T width, T height, T nearPlane, T farPlane) {
        Matrix.CreatePerspective(width, height, nearPlane, farPlane, out Matrix4<T> matrix);
        return matrix;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix4<T> CreatePerspectiveFOV(Angle<T> fov, T aspectRatio, T nearPlane, T farPlane) {
        Matrix.CreatePerspectiveFOV(fov, aspectRatio, nearPlane, farPlane, out Matrix4<T> matrix);
        return matrix;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix4<T> CreatePerspectiveOffCenter(T left, T right, T top, T bottom, T nearPlane, T farPlane) {
        Matrix.CreatePerspectiveOffCenter(left, right, top, bottom, nearPlane, farPlane, out Matrix4<T> matrix);
        return matrix;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix4<T> CreateScaleTranslation(Vector3<T> scale, Vector3<T> translation) {
        Matrix.CreateScaleTranslation(scale: scale, translation: translation, out var matrix);
        return matrix;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix4<T> CreateTranslation(Vector3<T> translation) {
        Matrix.CreateTranslation(translation, out var matrix);
        return matrix;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix4<T> CreateRotationXYZ(Angle<T> rotationX, Angle<T> rotationY, Angle<T> rotationZ) {
        Matrix.CreateRotationXYZ(rotationX, rotationY, rotationZ, out Matrix4<T> matrix);
        return matrix;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix4<T> CreateRotationXZY(Angle<T> rotationX, Angle<T> rotationZ, Angle<T> rotationY) {
        Matrix.CreateRotationXZY(rotationX, rotationZ, rotationY, out Matrix4<T> matrix);
        return matrix;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix4<T> CreateRotationYXZ(Angle<T> rotationY, Angle<T> rotationX, Angle<T> rotationZ) {
        Matrix.CreateRotationYXZ(rotationY, rotationX, rotationZ, out Matrix4<T> matrix);
        return matrix;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix4<T> CreateRotationYZX(Angle<T> rotationY, Angle<T> rotationZ, Angle<T> rotationX) {
        Matrix.CreateRotationYZX(rotationY, rotationZ, rotationX, out Matrix4<T> matrix);
        return matrix;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix4<T> CreateRotationZYX(Angle<T> rotationZ, Angle<T> rotationY, Angle<T> rotationX) {
        Matrix.CreateRotationZYX(rotationZ, rotationY, rotationX, out Matrix4<T> matrix);
        return matrix;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix4<T> CreateLookTo(Vector3<T> cameraPosition, Vector3<T> cameraDirection, Vector3<T> cameraUp) {
        Matrix.CreateLookTo(cameraPosition, cameraDirection, cameraUp, out Matrix4<T> matrix);
        return matrix;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix4<T> CreateLookTo(Vector3<T> cameraPosition, Direction<T> cameraDirection, Direction<T> cameraUp) {
        Matrix.CreateLookTo(cameraPosition, cameraDirection, cameraUp, out Matrix4<T> matrix);
        return matrix;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix4<T> CreateRotationZXY(Angle<T> rotationZ, Angle<T> rotationX, Angle<T> rotationY) {
        Matrix.CreateRotationZXY(rotationZ, rotationX, rotationY, out Matrix4<T> matrix);
        return matrix;
        }

    #endregion Factories

    #region Rows & Columns

    /// <summary> Returns <see cref="Matrix4{T}.RowA"/> of the <paramref name="matrix"/> by <see langword="ref"/>.
    /// </summary>
    public static ref Vector4<T> GetRowA(ref Matrix4<T> matrix) {
        return ref Unsafe.As<T, Vector4<T>>(ref matrix._a1);
        }

    /// <summary> Returns <see cref="Matrix4{T}.RowB"/> of the <paramref name="matrix"/> by <see langword="ref"/>.
    /// </summary>
    public static ref Vector4<T> GetRowB(ref Matrix4<T> matrix) {
        return ref Unsafe.As<T, Vector4<T>>(ref matrix._b1);
        }

    /// <summary> Returns <see cref="Matrix4{T}.RowC"/> of the <paramref name="matrix"/> by <see langword="ref"/>.
    /// </summary>
    public static ref Vector4<T> GetRowC(ref Matrix4<T> matrix) {
        return ref Unsafe.As<T, Vector4<T>>(ref matrix._c1);
        }

    /// <summary> Returns <see cref="Matrix4{T}.RowD"/> of the <paramref name="matrix"/> by <see langword="ref"/>.
    /// </summary>
    public static ref Vector4<T> GetRowD(ref Matrix4<T> matrix) {
        return ref Unsafe.As<T, Vector4<T>>(ref matrix._d1);
        }

    public static int MatrixWidth { get; } = 4;

    public static int MatrixHeight { get; } = 4;

    public Vector4<T> Column1 {
        readonly get => new(A1, B1, C1, D1);
        set {
            A1 = value.X;
            B1 = value.Y;
            C1 = value.Z;
            D1 = value.W;
            }
        }

    public Vector4<T> Column2 {
        readonly get => new(A2, B2, C2, D2);
        set {
            A2 = value.X;
            B2 = value.Y;
            C2 = value.Z;
            D2 = value.W;
            }
        }

    public Vector4<T> Column3 {
        readonly get => new(A3, B3, C3, D3);
        set {
            A3 = value.X;
            B3 = value.Y;
            C3 = value.Z;
            D3 = value.W;
            }
        }

    public Vector4<T> Column4 {
        readonly get => new(A4, B4, C4, D4);
        set {
            A4 = value.X;
            B4 = value.Y;
            C4 = value.Z;
            D4 = value.W;
            }
        }

    public unsafe Vector4<T> RowA {
        readonly get => new(A1, A2, A3, A4);
        set { Unsafe.Copy(Unsafe.AsPointer(ref _a1), ref value); }
        }

    public unsafe Vector4<T> RowB {
        readonly get => new(B1, B2, B3, B4);
        set { Unsafe.Copy(Unsafe.AsPointer(ref _b1), ref value); }
        }

    public unsafe Vector4<T> RowC {
        readonly get => new(C1, C2, C3, C4);
        set { Unsafe.Copy(Unsafe.AsPointer(ref _c1), ref value); }
        }

    public unsafe Vector4<T> RowD {
        readonly get => new(D1, D2, D3, D4);
        set { Unsafe.Copy(Unsafe.AsPointer(ref _d1), ref value); }
        }

    public readonly byte Width => 4;
    public readonly byte Height => 4;

    #endregion Rows & Columns

    #region Inversion

    [BunnyAttributes.Citation("DirectXMath XMMatrixInverse")]
    public static Matrix4<T> Invert(in Matrix4<T> matrix) {
        if (Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double)) {
            var (r0, r1, r2, r3) = Unsafe.BitCast<Matrix4<T>, (Vector256<double>, Vector256<double>, Vector256<double>, Vector256<double>)>(matrix);

            //invert the matrix(inlined)
            var t0 = WeaveLow(r0, r1); var t1 = WeaveHigh(r0, r1);
            var t2 = WeaveLow(r2, r3); var t3 = WeaveHigh(r2, r3);
            var rowA = ToLowLow(t0, t2); var rowB = ToHighHigh(t0, t2);
            var rowC = ToLowLow(t1, t3); var rowD = ToHighHigh(t1, t3);

            var V00 = rowC.Shuffle0011();
            var V10 = rowD.Shuffle2323();
            var V01 = rowA.Shuffle0011();
            var V11 = rowB.Shuffle2323();
            var V02 = Gather(rowC, rowA, 0, 2, 0, 2);
            var V12 = Gather(rowD, rowB, 1, 3, 1, 3);

            var D0 = V00 * V10;
            var D1 = V01 * V11;
            var D2 = V02 * V12;

            V00 = rowC.Shuffle2323();
            V10 = rowD.Shuffle0011();
            V01 = rowA.Shuffle2323();
            V11 = rowB.Shuffle0011();
            V02 = Gather(rowC, rowA, 1, 3, 1, 3);
            V12 = Gather(rowD, rowB, 0, 2, 0, 2);

            D0 = Vector256.FusedMultiplyAdd(-V00, V10, D0);
            D1 = Vector256.FusedMultiplyAdd(-V01, V11, D1);
            D2 = Vector256.FusedMultiplyAdd(-V02, V12, D2);

            V11 = Gather(D0, D2, 1, 3, 1, 1);
            V00 = Vector256.Shuffle(rowB, Vector256.Create(1, 2, 0, 1));
            V10 = Gather(V11, D0, 2, 0, 3, 0);
            V01 = Vector256.Shuffle(rowA, Vector256.Create(2, 0, 1, 0));
            V11 = Gather(V11, D0, 1, 2, 1, 2);

            Vector256<double> V13 = Gather(D1, D2, 1, 3, 3, 3);
            V02 = Vector256.Shuffle(rowD, Vector256.Create(1, 2, 0, 1));
            V12 = Gather(V13, D1, 2, 0, 3, 0);
            Vector256<double> V03 = Vector256.Shuffle(rowC, Vector256.Create(2, 0, 1, 0));
            V13 = Gather(V13, D1, 1, 2, 1, 2);

            var C0 = V00 * V10;
            var C2 = V01 * V11;
            var C4 = V02 * V12;
            var C6 = V03 * V13;

            V11 = Gather(D0, D2, 0, 1, 0, 0);
            V00 = Vector256.Shuffle(rowB, Vector256.Create(2, 3, 1, 2));
            V10 = Gather(D0, V11, 3, 0, 1, 2);
            V01 = Vector256.Shuffle(rowA, Vector256.Create(3, 2, 3, 1));
            V11 = Gather(D0, V11, 2, 1, 2, 0);

            V13 = Gather(D1, D2, 0, 1, 2, 2);
            V02 = Vector256.Shuffle(rowD, Vector256.Create(2, 3, 1, 2));
            V12 = Gather(D1, V13, 3, 0, 1, 2);
            V03 = Vector256.Shuffle(rowC, Vector256.Create(3, 2, 3, 1));
            V13 = Gather(D1, V13, 2, 1, 2, 0);

            C0 = Vector256.FusedMultiplyAdd(-V00, V10, C0);
            C2 = Vector256.FusedMultiplyAdd(-V01, V11, C2);
            C4 = Vector256.FusedMultiplyAdd(-V02, V12, C4);
            C6 = Vector256.FusedMultiplyAdd(-V03, V13, C6);

            V00 = Vector256.Shuffle(rowB, Vector256.Create(3, 0, 3, 0));
            V10 = Vector256.Shuffle(Gather(D0, D2, 2, 2, 0, 1), Vector256.Create(0, 3, 2, 0));
            V01 = Vector256.Shuffle(rowA, Vector256.Create(1, 3, 0, 2));
            V11 = Vector256.Shuffle(Gather(D0, D2, 0, 3, 0, 1), Vector256.Create(3, 0, 1, 2));
            V02 = Vector256.Shuffle(rowD, Vector256.Create(3, 0, 3, 0));
            V12 = Vector256.Shuffle(Gather(D1, D2, 2, 2, 2, 3), Vector256.Create(0, 3, 2, 0));
            V03 = Vector256.Shuffle(rowC, Vector256.Create(1, 3, 0, 2));
            V13 = Vector256.Shuffle(Gather(D1, D2, 0, 3, 2, 3), Vector256.Create(3, 0, 1, 2));

            V00 *= V10;
            V01 *= V11;
            V02 *= V12;
            V03 *= V13;

            var C1 = C0 - V00; C0 += V00;
            var C3 = C2 + V01; C2 -= V01;
            var C5 = C4 - V02; C4 += V02;
            var C7 = C6 + V03; C6 -= V03;

            C0 = Vector256.Shuffle(Gather(C0, C1, 0, 2, 1, 3), Vector256.Create(0, 2, 1, 3));
            C2 = Vector256.Shuffle(Gather(C2, C3, 0, 2, 1, 3), Vector256.Create(0, 2, 1, 3));
            C4 = Vector256.Shuffle(Gather(C4, C5, 0, 2, 1, 3), Vector256.Create(0, 2, 1, 3));
            C6 = Vector256.Shuffle(Gather(C6, C7, 0, 2, 1, 3), Vector256.Create(0, 2, 1, 3));

            var det = Vector256.Sum(C0 * rowA);
            if (double.Abs(det) < double.Epsilon) {
                var nan = Vector256.Create(double.NaN);
                return Unsafe.BitCast<(Vector256<double>, Vector256<double>, Vector256<double>, Vector256<double>), Matrix4<T>>(
                    (nan, nan, nan, nan));
                }

            var vTemp = Vector256.Create(1d / det);

            return Unsafe.BitCast<(Vector256<double>, Vector256<double>, Vector256<double>, Vector256<double>), Matrix4<T>>(
                (C0 * vTemp, C2 * vTemp, C4 * vTemp, C6 * vTemp));
            }
        else if (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float)) {
            var (r0, r1, r2, r3) = Unsafe.BitCast<Matrix4<T>, (Vector128<float>, Vector128<float>, Vector128<float>, Vector128<float>)>(matrix);

            // invert the matrix (inlined)
            var t0 = WeaveLow(r0, r1); var t1 = WeaveHigh(r0, r1);
            var t2 = WeaveLow(r2, r3); var t3 = WeaveHigh(r2, r3);
            var rowA = ToLowLow(t0, t2); var rowB = ToHighHigh(t0, t2);
            var rowC = ToLowLow(t1, t3); var rowD = ToHighHigh(t1, t3);

            var V00 = rowC.Shuffle0011();
            var V10 = rowD.Shuffle2323();
            var V01 = rowA.Shuffle0011();
            var V11 = rowB.Shuffle2323();
            var V02 = Gather(rowC, rowA, 0, 2, 0, 2);
            var V12 = Gather(rowD, rowB, 1, 3, 1, 3);

            var D0 = V00 * V10;
            var D1 = V01 * V11;
            var D2 = V02 * V12;

            V00 = rowC.Shuffle2323();
            V10 = rowD.Shuffle0011();
            V01 = rowA.Shuffle2323();
            V11 = rowB.Shuffle0011();
            V02 = Gather(rowC, rowA, 1, 3, 1, 3);
            V12 = Gather(rowD, rowB, 0, 2, 0, 2);

            D0 = Vector128.FusedMultiplyAdd(-V00, V10, D0);
            D1 = Vector128.FusedMultiplyAdd(-V01, V11, D1);
            D2 = Vector128.FusedMultiplyAdd(-V02, V12, D2);

            V11 = Gather(D0, D2, 1, 3, 1, 1);
            V00 = Vector128.Shuffle(rowB, Vector128.Create(1, 2, 0, 1));
            V10 = Gather(V11, D0, 2, 0, 3, 0);
            V01 = Vector128.Shuffle(rowA, Vector128.Create(2, 0, 1, 0));
            V11 = Gather(V11, D0, 1, 2, 1, 2);

            Vector128<float> V13 = Gather(D1, D2, 1, 3, 3, 3);
            V02 = Vector128.Shuffle(rowD, Vector128.Create(1, 2, 0, 1));
            V12 = Gather(V13, D1, 2, 0, 3, 0);
            Vector128<float> V03 = Vector128.Shuffle(rowC, Vector128.Create(2, 0, 1, 0));
            V13 = Gather(V13, D1, 1, 2, 1, 2);

            var C0 = V00 * V10;
            var C2 = V01 * V11;
            var C4 = V02 * V12;
            var C6 = V03 * V13;

            V11 = Gather(D0, D2, 0, 1, 0, 0);
            V00 = Vector128.Shuffle(rowB, Vector128.Create(2, 3, 1, 2));
            V10 = Gather(D0, V11, 3, 0, 1, 2);
            V01 = Vector128.Shuffle(rowA, Vector128.Create(3, 2, 3, 1));
            V11 = Gather(D0, V11, 2, 1, 2, 0);

            V13 = Gather(D1, D2, 0, 1, 2, 2);
            V02 = Vector128.Shuffle(rowD, Vector128.Create(2, 3, 1, 2));
            V12 = Gather(D1, V13, 3, 0, 1, 2);
            V03 = Vector128.Shuffle(rowC, Vector128.Create(3, 2, 3, 1));
            V13 = Gather(D1, V13, 2, 1, 2, 0);

            C0 = Vector128.FusedMultiplyAdd(-V00, V10, C0);
            C2 = Vector128.FusedMultiplyAdd(-V01, V11, C2);
            C4 = Vector128.FusedMultiplyAdd(-V02, V12, C4);
            C6 = Vector128.FusedMultiplyAdd(-V03, V13, C6);

            V00 = Vector128.Shuffle(rowB, Vector128.Create(3, 0, 3, 0));
            V10 = Vector128.Shuffle(Gather(D0, D2, 2, 2, 0, 1), Vector128.Create(0, 3, 2, 0));
            V01 = Vector128.Shuffle(rowA, Vector128.Create(1, 3, 0, 2));
            V11 = Vector128.Shuffle(Gather(D0, D2, 0, 3, 0, 1), Vector128.Create(3, 0, 1, 2));
            V02 = Vector128.Shuffle(rowD, Vector128.Create(3, 0, 3, 0));
            V12 = Vector128.Shuffle(Gather(D1, D2, 2, 2, 2, 3), Vector128.Create(0, 3, 2, 0));
            V03 = Vector128.Shuffle(rowC, Vector128.Create(1, 3, 0, 2));
            V13 = Vector128.Shuffle(Gather(D1, D2, 0, 3, 2, 3), Vector128.Create(3, 0, 1, 2));

            V00 *= V10;
            V01 *= V11;
            V02 *= V12;
            V03 *= V13;

            var C1 = C0 - V00; C0 += V00;
            var C3 = C2 + V01; C2 -= V01;
            var C5 = C4 - V02; C4 += V02;
            var C7 = C6 + V03; C6 -= V03;

            C0 = Vector128.Shuffle(Gather(C0, C1, 0, 2, 1, 3), Vector128.Create(0, 2, 1, 3));
            C2 = Vector128.Shuffle(Gather(C2, C3, 0, 2, 1, 3), Vector128.Create(0, 2, 1, 3));
            C4 = Vector128.Shuffle(Gather(C4, C5, 0, 2, 1, 3), Vector128.Create(0, 2, 1, 3));
            C6 = Vector128.Shuffle(Gather(C6, C7, 0, 2, 1, 3), Vector128.Create(0, 2, 1, 3));

            var det = Vector128.Sum(C0 * rowA);
            if (float.Abs(det) < float.Epsilon) {
                var nan = Vector128.Create(float.NaN);
                return Unsafe.BitCast<(Vector128<float>, Vector128<float>, Vector128<float>, Vector128<float>), Matrix4<T>>(
                    (nan, nan, nan, nan));
                }

            var vTemp = Vector128.Create(1f / det);

            return Unsafe.BitCast<(Vector128<float>, Vector128<float>, Vector128<float>, Vector128<float>), Matrix4<T>>(
                (C0 * vTemp, C2 * vTemp, C4 * vTemp, C6 * vTemp));
            }
        else {
            /* The MIT License (MIT)

            Copyright (c) .NET Foundation and Contributors

            All rights reserved.

            Permission is hereby granted, free of charge, to any person obtaining a copy
            of this software and associated documentation files (the "Software"), to deal
            in the Software without restriction, including without limitation the rights
            to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
            copies of the Software, and to permit persons to whom the Software is
            furnished to do so, subject to the following conditions:

            The above copyright notice and this permission notice shall be included in all
            copies or substantial portions of the Software.

            THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
            IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
            FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
            AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
            LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
            OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
            SOFTWARE.
            */

            T A1 = matrix.A1, A2 = matrix.A2, A3 = matrix.A3, A4 = matrix.A4;
            T B1 = matrix.B1, B2 = matrix.B2, B3 = matrix.B3, B4 = matrix.B4;
            T C1 = matrix.C1, C2 = matrix.C2, C3 = matrix.C3, C4 = matrix.C4;
            T D1 = matrix.D1, D2 = matrix.D2, D3 = matrix.D3, D4 = matrix.D4;
            T kp_lo = C3 * D4 - C4 * D3;
            T jp_ln = C2 * D4 - C4 * D2;
            T jo_kn = C2 * D3 - C3 * D2;
            T ip_lm = C1 * D4 - C4 * D1;
            T io_km = C1 * D3 - C3 * D1;
            T in_jm = C1 * D2 - C2 * D1;
            T a11 = +(B2 * kp_lo - B3 * jp_ln + B4 * jo_kn);
            T a12 = -(B1 * kp_lo - B3 * ip_lm + B4 * io_km);
            T a13 = +(B1 * jp_ln - B2 * ip_lm + B4 * in_jm);
            T a14 = -(B1 * jo_kn - B2 * io_km + B3 * in_jm);
            T det = A1 * a11 + A2 * a12 + A3 * a13 + A4 * a14;
            if (T.Abs(det) < T.Epsilon) {
                return new(
                    T.NaN, T.NaN, T.NaN, T.NaN,
                    T.NaN, T.NaN, T.NaN, T.NaN,
                    T.NaN, T.NaN, T.NaN, T.NaN,
                    T.NaN, T.NaN, T.NaN, T.NaN
                    );
                }
            T invDet = T.One / det;
            T gp_ho = B3 * D4 - B4 * D3;
            T fp_hn = B2 * D4 - B4 * D2;
            T fo_gn = B2 * D3 - B3 * D2;
            T ep_hm = B1 * D4 - B4 * D1;
            T eo_gm = B1 * D3 - B3 * D1;
            T en_fm = B1 * D2 - B2 * D1;
            T gl_hk = B3 * C4 - B4 * C3;
            T fl_hj = B2 * C4 - B4 * C2;
            T fk_gj = B2 * C3 - B3 * C2;
            T el_hi = B1 * C4 - B4 * C1;
            T ek_gi = B1 * C3 - B3 * C1;
            T ej_fi = B1 * C2 - B2 * C1;
            return new(
                a1: +(a11 * invDet),
                a2: -(A2 * kp_lo - A3 * jp_ln + A4 * jo_kn) * invDet,
                a3: +(A2 * gp_ho - A3 * fp_hn + A4 * fo_gn) * invDet,
                a4: -(A2 * gl_hk - A3 * fl_hj + A4 * fk_gj) * invDet,
                b1: +(a12 * invDet),
                b2: +(A1 * kp_lo - A3 * ip_lm + A4 * io_km) * invDet,
                b3: -(A1 * gp_ho - A3 * ep_hm + A4 * eo_gm) * invDet,
                b4: +(A1 * gl_hk - A3 * el_hi + A4 * ek_gi) * invDet,
                c1: +(a13 * invDet),
                c2: -(A1 * jp_ln - A2 * ip_lm + A4 * in_jm) * invDet,
                c3: +(A1 * fp_hn - A2 * ep_hm + A4 * en_fm) * invDet,
                c4: -(A1 * fl_hj - A2 * el_hi + A4 * ej_fi) * invDet,
                d1: +(a14 * invDet),
                d2: +(A1 * jo_kn - A2 * io_km + A3 * in_jm) * invDet,
                d3: -(A1 * fo_gn - A2 * eo_gm + A3 * en_fm) * invDet,
                d4: +(A1 * fk_gj - A2 * ek_gi + A3 * ej_fi) * invDet
            );
            }

        }

    public readonly Matrix4<T> Invert() {
        return Invert(this);
        }

    #endregion Inversion

    #region Determinants

    public readonly T Determinant {
        get {
            var num1 = C3 * D4 - C4 * D3;
            var num2 = C2 * D4 - C4 * D2;
            var num3 = C2 * D3 - C3 * D2;
            var num4 = C1 * D4 - C4 * D1;
            var num5 = C1 * D3 - C3 * D1;
            var num6 = C1 * D2 - C2 * D1;
            return
                +A1 * (B2 * num1 - B3 * num2 + B4 * num3)
                - A2 * (B1 * num1 - B3 * num4 + B4 * num5)
                + A3 * (B1 * num2 - B2 * num4 + B4 * num6)
                - A4 * (B1 * num3 - B2 * num5 + B3 * num6);
            }
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T GetDeterminant(in Matrix4<T> matrix) {
        return matrix.Determinant;
        }

    #endregion Determinants

    #region Transposition

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Matrix4<T> Transpose() {
        return Transpose(in this);
        }

    public static Matrix4<T> Transpose(in Matrix4<T> matrix) {
        if (typeof(T) == typeof(float)) {
            ref var m = ref Unsafe.As<Matrix4<T>, Matrix4<float>>(ref Unsafe.AsRef(in matrix));
            if (Vector512<float>.IsSupported && Vector512.IsHardwareAccelerated) {
                var v = Unsafe.BitCast<Matrix4<float>, Vector512<float>>(m);
                var t = Vector512.Shuffle(v, Vector512.Create(0, 4, 8, 12, 1, 5, 9, 13, 2, 6, 10, 14, 3, 7, 11, 15));
                return Unsafe.BitCast<Vector512<float>, Matrix4<T>>(t);
                }
            else if (Vector256<float>.IsSupported && Vector256.IsHardwareAccelerated) {
                var v0 = Unsafe.As<Matrix4<float>, Vector256<float>>(ref m);
                var v1 = Unsafe.Add(ref Unsafe.As<Matrix4<float>, Vector256<float>>(ref m), 1);
                var t0 = Vector256.Shuffle(v0, Vector256.Create(0, 4, 1, 5, 2, 6, 3, 7));
                var t1 = Vector256.Shuffle(v1, Vector256.Create(0, 4, 1, 5, 2, 6, 3, 7));

                var r0 = Vector256.Shuffle(
                    Vector256.Create(t0.GetLower(), t1.GetLower()),
                    Vector256.Create(0, 1, 4, 5, 2, 3, 6, 7)
                    );
                var r1 = Vector256.Shuffle(
                    Vector256.Create(t0.GetUpper(), t1.GetUpper()),
                    Vector256.Create(0, 1, 4, 5, 2, 3, 6, 7)
                    );
                Unsafe.SkipInit(out Matrix4<T> transposed);
                ref var result = ref Unsafe.As<Matrix4<T>, Vector256<float>>(ref transposed);
                result = r0;
                Unsafe.Add(ref result, 1) = r1;
                return transposed;
                }
            else if (Vector128.IsHardwareAccelerated && Vector128<float>.IsSupported) {
                var (r0, r1, r2, r3) = Unsafe.BitCast<Matrix4<T>, (Vector128<float>, Vector128<float>, Vector128<float>, Vector128<float>)>(matrix);
                var t0 = WeaveLow(r0, r1); var t1 = WeaveHigh(r0, r1);
                var t2 = WeaveLow(r2, r3); var t3 = WeaveHigh(r2, r3);
                return Unsafe.BitCast<(Vector128<float>, Vector128<float>, Vector128<float>, Vector128<float>), Matrix4<T>>((ToLowLow(t0, t2), ToHighHigh(t0, t2), ToLowLow(t1, t3), ToHighHigh(t1, t3)));
                }
            }
        else if (typeof(T) == typeof(double)) {
            ref var m = ref Unsafe.As<Matrix4<T>, Matrix4<double>>(ref Unsafe.AsRef(in matrix));
            if (Vector512<double>.IsSupported && Vector512.IsHardwareAccelerated) {
                var v0 = Unsafe.As<Matrix4<double>, Vector512<double>>(ref m);
                var v1 = Unsafe.Add(ref Unsafe.As<Matrix4<double>, Vector512<double>>(ref m), 1);
                var t0 = Vector512.Shuffle(v0, Vector512.Create(0L, 4, 1, 5, 2, 6, 3, 7));
                var t1 = Vector512.Shuffle(v1, Vector512.Create(0L, 4, 1, 5, 2, 6, 3, 7));

                var r0 = Vector512.Shuffle(
                    Vector512.Create(t0.GetLower(), t1.GetLower()),
                    Vector512.Create(0L, 1, 4, 5, 2, 3, 6, 7)
                    );
                var r1 = Vector512.Shuffle(
                    Vector512.Create(t0.GetUpper(), t1.GetUpper()),
                    Vector512.Create(0L, 1, 4, 5, 2, 3, 6, 7)
                    );

                Unsafe.SkipInit(out Matrix4<T> transposed);
                ref var result = ref Unsafe.As<Matrix4<T>, Vector512<double>>(ref transposed);
                result = r0;
                Unsafe.Add(ref result, 1) = r1;
                return transposed;
                }
            else if (Vector256.IsHardwareAccelerated && Vector256<double>.IsSupported) {
                var (r0, r1, r2, r3) = Unsafe.BitCast<Matrix4<T>, (Vector256<double>, Vector256<double>, Vector256<double>, Vector256<double>)>(matrix);
                var t0 = WeaveLow(r0, r1); var t1 = WeaveHigh(r0, r1);
                var t2 = WeaveLow(r2, r3); var t3 = WeaveHigh(r2, r3);
                return Unsafe.BitCast<(Vector256<double>, Vector256<double>, Vector256<double>, Vector256<double>), Matrix4<T>>((ToLowLow(t0, t2), ToHighHigh(t0, t2), ToLowLow(t1, t3), ToHighHigh(t1, t3)));
                }
            }
        return new(
            matrix.A1, matrix.B1, matrix.C1, matrix.D1,
            matrix.A2, matrix.B2, matrix.C2, matrix.D2,
            matrix.A3, matrix.B3, matrix.C3, matrix.D3,
            matrix.A4, matrix.B4, matrix.C4, matrix.D4
            );
        }

    #endregion Transposition

    #region Transformation

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Vector4<T> Transform(Vector4<T> vector) {
        return Transform(in this, vector);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4<T> Transform(in Matrix4<T> matrix, Vector4<T> vector) {
        Matrix.Transform(in matrix, vector, out var result);
        return result;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Vector3<T> InvertedTransform(Vector3<T> vector) {
        return Invert().Transform(vector);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3<T> Transform(in Matrix4<T> matrix, Vector3<T> vector) {
        Matrix.Transform(in matrix, vector, out var result);
        return result;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3<T> InvertedTransform(in Matrix4<T> matrix, Vector3<T> vector) {
        return Transform(Invert(in matrix), vector);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Vector3<T> Transform(Vector3<T> vector) {
        return Transform(in this, vector);
        }

    #endregion Transformation

    #region Extraction

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Vector3<T> GetTranslation() {
        return GetTranslation(in this);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Vector3<T> GetScale() {
        return GetScale(in this);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3<T> GetScale(in Matrix4<T> matrix) {
        Matrix.GetScale(in matrix, out Vector3<T> scale);
        return scale;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3<T> GetTranslation(in Matrix4<T> matrix) {
        Matrix.GetTranslation(in matrix, out Vector3<T> translation);
        return translation;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Quaternion<T> GetRotation(in Matrix4<T> matrix) {
        Matrix.GetRotation(in matrix, out Quaternion<T> rotation);
        return rotation;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Quaternion<T> GetRotation() {
        return GetRotation(in this);
        }

    #endregion Extraction

    #region Component Removal

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RemoveTranslation(ref Matrix4<T> matrix) {
        Matrix.Remove3DTranslation(ref matrix);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void RemoveTranslation() {
        Matrix.Remove3DTranslation(ref this);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Remove3DScale(ref Matrix4<T> matrix) {
        Matrix.Remove3DScale(ref matrix);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Remove3DScale() {
        Matrix.Remove3DScale(ref this);
        }

    #endregion Component Removal

    #region Equatability

    public bool Equals(Matrix4<T> other) {
        if (Vector512.IsHardwareAccelerated && Vector512<T>.IsSupported && typeof(T) == typeof(float)) {
            var v0 = Unsafe.BitCast<Matrix4<T>, Vector512<float>>(this);
            var v1 = Unsafe.BitCast<Matrix4<T>, Vector512<float>>(other);
            return Vector512.EqualsAll(v0, v1);
            }
        else if (Vector512.IsHardwareAccelerated && Vector512<T>.IsSupported && typeof(T) == typeof(double)) {
            var v00 = Unsafe.As<T, Vector512<double>>(ref _a1);
            var v01 = Unsafe.As<T, Vector512<double>>(ref _c1);
            var v10 = Unsafe.As<T, Vector512<double>>(ref other._a1);
            var v11 = Unsafe.As<T, Vector512<double>>(ref other._c1);
            return Vector512.EqualsAll(v00, v10)
                && Vector512.EqualsAll(v01, v11);
            }
        else if (Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(float)) {
            var v00 = Unsafe.As<T, Vector256<float>>(ref _a1);
            var v01 = Unsafe.As<T, Vector256<float>>(ref _c1);
            var v10 = Unsafe.As<T, Vector256<float>>(ref other._a1);
            var v11 = Unsafe.As<T, Vector256<float>>(ref other._c1);
            return Vector256.EqualsAll(v00, v10)
                && Vector256.EqualsAll(v01, v11);
            }
        else if (Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double)) {
            var v00 = Unsafe.As<T, Vector256<double>>(ref _a1);
            var v01 = Unsafe.As<T, Vector256<double>>(ref _b1);
            var v02 = Unsafe.As<T, Vector256<double>>(ref _c1);
            var v03 = Unsafe.As<T, Vector256<double>>(ref _d1);
            var v10 = Unsafe.As<T, Vector256<double>>(ref other._a1);
            var v11 = Unsafe.As<T, Vector256<double>>(ref other._b1);
            var v12 = Unsafe.As<T, Vector256<double>>(ref other._c1);
            var v13 = Unsafe.As<T, Vector256<double>>(ref other._d1);
            return Vector256.EqualsAll(v00, v10)
                && Vector256.EqualsAll(v01, v11)
                && Vector256.EqualsAll(v02, v12)
                && Vector256.EqualsAll(v03, v13);
            }
        else if (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float)) {
            var v00 = Unsafe.As<T, Vector128<float>>(ref _a1);
            var v01 = Unsafe.As<T, Vector128<float>>(ref _b1);
            var v02 = Unsafe.As<T, Vector128<float>>(ref _c1);
            var v03 = Unsafe.As<T, Vector128<float>>(ref _d1);
            var v10 = Unsafe.As<T, Vector128<float>>(ref other._a1);
            var v11 = Unsafe.As<T, Vector128<float>>(ref other._b1);
            var v12 = Unsafe.As<T, Vector128<float>>(ref other._c1);
            var v13 = Unsafe.As<T, Vector128<float>>(ref other._d1);
            return Vector128.EqualsAll(v00, v10)
                && Vector128.EqualsAll(v01, v11)
                && Vector128.EqualsAll(v02, v12)
                && Vector128.EqualsAll(v03, v13);
            }
        else {
            return A1 == other.A1 && A2 == other.A2 && A3 == other.A3 && A4 == other.A4
                && B1 == other.B1 && B2 == other.B2 && B3 == other.B3 && B4 == other.B4
                && C1 == other.C1 && C2 == other.C2 && C3 == other.C3 && C4 == other.C4
                && D1 == other.D1 && D2 == other.D2 && D3 == other.D3 && D4 == other.D4;
            }
        }

    public override bool Equals(object? obj) {
        return obj is Matrix4<T> matrix && Equals(matrix);
        }

    public override readonly int GetHashCode() {
        return HashCode.Combine(RowA.GetHashCode(), RowB.GetHashCode(), RowC.GetHashCode(), RowD.GetHashCode());
        }

    public static bool operator ==(Matrix4<T> left, Matrix4<T> right) {
        return left.Equals(right);
        }

    public static bool operator !=(Matrix4<T> left, Matrix4<T> right) {
        return left.Equals(right) == false;
        }

    #endregion Equatability

    #region Operators

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix4<T> operator *(T factor, in Matrix4<T> matrix) {
        return Scale(in matrix, factor);
        }

    public static Matrix4<T> Scale(in Matrix4<T> matrix, T factor) {
        if (Vector512.IsHardwareAccelerated) {
            if (Vector512<double>.IsSupported && typeof(T) == typeof(double)) {
                var f = Vector512.Create(factor);
                var ab = Vector512.LoadUnsafe(in matrix._a1);
                var cd = Vector512.LoadUnsafe(in matrix._c1);
                var ret = (ab * f, cd * f);
                return Unsafe.As<(Vector512<T>, Vector512<T>), Matrix4<T>>(ref ret);
                }
            else if (Vector512<float>.IsSupported && typeof(T) == typeof(float)) {
                var f = Vector512.Create(factor);
                var abcd = Vector512.LoadUnsafe(in matrix._a1);
                var ret = abcd * f;
                return Unsafe.As<Vector512<T>, Matrix4<T>>(ref ret);
                }
            }
        else if (Vector256.IsHardwareAccelerated) {
            if (Vector256<double>.IsSupported && typeof(T) == typeof(double)) {
                var f = Vector256.Create(factor);
                var a = Vector256.LoadUnsafe(in matrix._a1);
                var b = Vector256.LoadUnsafe(in matrix._b1);
                var c = Vector256.LoadUnsafe(in matrix._c1);
                var d = Vector256.LoadUnsafe(in matrix._d1);
                var ret = (a * f, b * f, c * f, d * f);
                return Unsafe.As<(Vector256<T>, Vector256<T>, Vector256<T>, Vector256<T>), Matrix4<T>>(ref ret);
                }
            else if (Vector256<float>.IsSupported && typeof(T) == typeof(float)) {
                var f = Vector256.Create(factor);
                var ab = Vector256.LoadUnsafe(in matrix._a1);
                var cd = Vector256.LoadUnsafe(in matrix._c1);
                var ret = (ab * f, cd * f);
                return Unsafe.As<(Vector256<T>, Vector256<T>), Matrix4<T>>(ref ret);
                }
            else { // Half
                var f = Vector256.Create(factor);
                var abcd = Vector256.LoadUnsafe(in matrix._a1);
                var ret = abcd * f;
                return Unsafe.As<Vector256<T>, Matrix4<T>>(ref ret);
                }
            }
        if (Vector128.IsHardwareAccelerated) {
            if (Vector128<double>.IsSupported && typeof(T) == typeof(double)) {
                var f = Vector128.Create(factor);
                var a12 = Vector128.LoadUnsafe(in matrix._a1);
                var a34 = Vector128.LoadUnsafe(in matrix._a3);
                var b12 = Vector128.LoadUnsafe(in matrix._b1);
                var b34 = Vector128.LoadUnsafe(in matrix._b3);
                var c12 = Vector128.LoadUnsafe(in matrix._c1);
                var c34 = Vector128.LoadUnsafe(in matrix._c3);
                var d12 = Vector128.LoadUnsafe(in matrix._d1);
                var d34 = Vector128.LoadUnsafe(in matrix._d3);
                var ret = (a12 * f, a34 * f, b12 * f, b34 * f, c12 * f, c34 * f, d12 * f, d34 * f);
                return Unsafe.As<(Vector128<T>, Vector128<T>, Vector128<T>, Vector128<T>, Vector128<T>, Vector128<T>, Vector128<T>, Vector128<T>), Matrix4<T>>(ref ret);
                }
            else if (Vector128<float>.IsSupported && typeof(T) == typeof(float)) {
                var f = Vector128.Create(factor);
                var a = Vector128.LoadUnsafe(in matrix._a1);
                var b = Vector128.LoadUnsafe(in matrix._b1);
                var c = Vector128.LoadUnsafe(in matrix._c1);
                var d = Vector128.LoadUnsafe(in matrix._d1);
                var ret = (a * f, b * f, c * f, d * f);
                return Unsafe.As<(Vector128<T>, Vector128<T>, Vector128<T>, Vector128<T>), Matrix4<T>>(ref ret);
                }
            else { // Half
                var f = Vector128.Create(factor);
                var ab = Vector128.LoadUnsafe(in matrix._a1);
                var cd = Vector128.LoadUnsafe(in matrix._c1);
                var ret = (ab * f, cd * f);
                return Unsafe.As<(Vector128<T>, Vector128<T>), Matrix4<T>>(ref ret);
                }
            }
        if (Vector64.IsHardwareAccelerated) {
            if (Vector64<float>.IsSupported && typeof(T) == typeof(float)) {
                var f = Vector64.Create(factor);
                var a12 = Vector64.LoadUnsafe(in matrix._a1);
                var a34 = Vector64.LoadUnsafe(in matrix._a3);
                var b12 = Vector64.LoadUnsafe(in matrix._b1);
                var b34 = Vector64.LoadUnsafe(in matrix._b3);
                var c12 = Vector64.LoadUnsafe(in matrix._c1);
                var c34 = Vector64.LoadUnsafe(in matrix._c3);
                var d12 = Vector64.LoadUnsafe(in matrix._d1);
                var d34 = Vector64.LoadUnsafe(in matrix._d3);
                var ret = (a12 * f, a34 * f, b12 * f, b34 * f, c12 * f, c34 * f, d12 * f, d34 * f);
                return Unsafe.As<(Vector64<T>, Vector64<T>, Vector64<T>, Vector64<T>, Vector64<T>, Vector64<T>, Vector64<T>, Vector64<T>), Matrix4<T>>(ref ret);
                }
            else { // Half
                var f = Vector64.Create(factor);
                var a = Vector64.LoadUnsafe(in matrix._a1);
                var b = Vector64.LoadUnsafe(in matrix._b1);
                var c = Vector64.LoadUnsafe(in matrix._c1);
                var d = Vector64.LoadUnsafe(in matrix._d1);
                var ret = (a * f, b * f, c * f, d * f);
                return Unsafe.As<(Vector64<T>, Vector64<T>, Vector64<T>, Vector64<T>), Matrix4<T>>(ref ret);
                }
            }
        else {
            return new(
                matrix.A1 * factor, matrix.A2 * factor, matrix.A3 * factor, matrix.A4 * factor,
                matrix.B1 * factor, matrix.B2 * factor, matrix.B3 * factor, matrix.B4 * factor,
                matrix.C1 * factor, matrix.C2 * factor, matrix.C3 * factor, matrix.C4 * factor,
                matrix.D1 * factor, matrix.D2 * factor, matrix.D3 * factor, matrix.D4 * factor
                );
            }
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Matrix4<T> Scale(T factor) {
        return Scale(this, factor);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix4<T> operator +(in Matrix4<T> left, in Matrix4<T> right) {
        return Add(left, right);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Matrix4<T> Add(in Matrix4<T> other) {
        return Add(this, other);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Matrix4<T> Subtract(in Matrix4<T> other) {
        return Subtract(this, other);
        }

    [BunnyAttributes.Benchmark]
    public static Matrix4<T> Subtract(in Matrix4<T> left, in Matrix4<T> right) {
        if (Vector512.IsHardwareAccelerated && Vector512<T>.IsSupported && typeof(T) == typeof(float)) {
            var a = Vector512.LoadUnsafe(in left._a1);
            var b = Vector512.LoadUnsafe(in right._a1);
            return Unsafe.BitCast<Vector512<T>, Matrix4<T>>(a - b);
            }
        else if (Vector512.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double)) {
            var a1 = Vector512.LoadUnsafe(in left._a1);
            var a2 = Vector512.LoadUnsafe(in left._c1);
            var b1 = Vector512.LoadUnsafe(in right._a1);
            var b2 = Vector512.LoadUnsafe(in right._c1);
            var r1 = a1 - b1;
            var r2 = a2 - b2;
            var ret = default(Matrix4<T>);
            Vector512.StoreUnsafe(r1, ref ret._a1);
            Vector512.StoreUnsafe(r2, ref ret._c1);
            return ret;
            }
        else if (Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(float)) {
            var a1 = Vector256.LoadUnsafe(in left._a1);
            var a2 = Vector256.LoadUnsafe(in left._c1);
            var b1 = Vector256.LoadUnsafe(in right._a1);
            var b2 = Vector256.LoadUnsafe(in right._c1);
            var r1 = a1 - b1;
            var r2 = a2 - b2;
            var ret = default(Matrix4<T>);
            Vector256.StoreUnsafe(r1, ref ret._a1);
            Vector256.StoreUnsafe(r2, ref ret._c1);
            return ret;
            }
        else if (Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double)) {
            var a1 = Vector256.LoadUnsafe(in left._a1);
            var a2 = Vector256.LoadUnsafe(in left._b1);
            var a3 = Vector256.LoadUnsafe(in left._c1);
            var a4 = Vector256.LoadUnsafe(in left._d1);
            var b1 = Vector256.LoadUnsafe(in right._a1);
            var b2 = Vector256.LoadUnsafe(in right._b1);
            var b3 = Vector256.LoadUnsafe(in right._c1);
            var b4 = Vector256.LoadUnsafe(in right._d1);
            var r1 = a1 - b1;
            var r2 = a2 - b2;
            var r3 = a3 - b3;
            var r4 = a4 - b4;
            var ret = default(Matrix4<T>);
            Vector256.StoreUnsafe(r1, ref ret._a1);
            Vector256.StoreUnsafe(r2, ref ret._b1);
            Vector256.StoreUnsafe(r3, ref ret._c1);
            Vector256.StoreUnsafe(r4, ref ret._d1);
            return ret;
            }
        else if (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float)) {
            var a1 = Vector128.LoadUnsafe(in left._a1);
            var a2 = Vector128.LoadUnsafe(in left._b1);
            var a3 = Vector128.LoadUnsafe(in left._c1);
            var a4 = Vector128.LoadUnsafe(in left._d1);
            var b1 = Vector128.LoadUnsafe(in right._a1);
            var b2 = Vector128.LoadUnsafe(in right._b1);
            var b3 = Vector128.LoadUnsafe(in right._c1);
            var b4 = Vector128.LoadUnsafe(in right._d1);
            var r1 = a1 - b1;
            var r2 = a2 - b2;
            var r3 = a3 - b3;
            var r4 = a4 - b4;
            var ret = default(Matrix4<T>);
            Vector128.StoreUnsafe(r1, ref ret._a1);
            Vector128.StoreUnsafe(r2, ref ret._b1);
            Vector128.StoreUnsafe(r3, ref ret._c1);
            Vector128.StoreUnsafe(r4, ref ret._d1);
            return ret;
            }
        else {
            return new(
                left.A1 - right.A1, left.A2 - right.A2, left.A3 - right.A3, left.A4 - right.A4,
                left.B1 - right.B1, left.B2 - right.B2, left.B3 - right.B3, left.B4 - right.B4,
                left.C1 - right.C1, left.C2 - right.C2, left.C3 - right.C3, left.C4 - right.C4,
                left.D1 - right.D1, left.D2 - right.D2, left.D3 - right.D3, left.D4 - right.D4
                );
            }
        }

    [BunnyAttributes.Benchmark]
    public static Matrix4<T> Add(in Matrix4<T> left, in Matrix4<T> right) {
        if (Vector512.IsHardwareAccelerated && Vector512<T>.IsSupported && typeof(T) == typeof(float)) {
            var a = Vector512.LoadUnsafe(in left._a1);
            var b = Vector512.LoadUnsafe(in right._a1);
            return Unsafe.BitCast<Vector512<T>, Matrix4<T>>(a + b);
            }
        else if (Vector512.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double)) {
            var a1 = Vector512.LoadUnsafe(in left._a1);
            var a2 = Vector512.LoadUnsafe(in left._c1);
            var b1 = Vector512.LoadUnsafe(in right._a1);
            var b2 = Vector512.LoadUnsafe(in right._c1);
            var r1 = a1 + b1;
            var r2 = a2 + b2;
            var ret = default(Matrix4<T>);
            Vector512.StoreUnsafe(r1, ref ret._a1);
            Vector512.StoreUnsafe(r2, ref ret._c1);
            return ret;
            }
        else if (Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(float)) {
            var a1 = Vector256.LoadUnsafe(in left._a1);
            var a2 = Vector256.LoadUnsafe(in left._c1);
            var b1 = Vector256.LoadUnsafe(in right._a1);
            var b2 = Vector256.LoadUnsafe(in right._c1);
            var r1 = a1 + b1;
            var r2 = a2 + b2;
            var ret = default(Matrix4<T>);
            Vector256.StoreUnsafe(r1, ref ret._a1);
            Vector256.StoreUnsafe(r2, ref ret._c1);
            return ret;
            }
        else if (Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(double)) {
            var a1 = Vector256.LoadUnsafe(in left._a1);
            var a2 = Vector256.LoadUnsafe(in left._b1);
            var a3 = Vector256.LoadUnsafe(in left._c1);
            var a4 = Vector256.LoadUnsafe(in left._d1);
            var b1 = Vector256.LoadUnsafe(in right._a1);
            var b2 = Vector256.LoadUnsafe(in right._b1);
            var b3 = Vector256.LoadUnsafe(in right._c1);
            var b4 = Vector256.LoadUnsafe(in right._d1);
            var r1 = a1 + b1;
            var r2 = a2 + b2;
            var r3 = a3 + b3;
            var r4 = a4 + b4;
            var ret = default(Matrix4<T>);
            Vector256.StoreUnsafe(r1, ref ret._a1);
            Vector256.StoreUnsafe(r2, ref ret._b1);
            Vector256.StoreUnsafe(r3, ref ret._c1);
            Vector256.StoreUnsafe(r4, ref ret._d1);
            return ret;
            }
        else if (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && typeof(T) == typeof(float)) {
            var a1 = Vector128.LoadUnsafe(in left._a1);
            var a2 = Vector128.LoadUnsafe(in left._b1);
            var a3 = Vector128.LoadUnsafe(in left._c1);
            var a4 = Vector128.LoadUnsafe(in left._d1);
            var b1 = Vector128.LoadUnsafe(in right._a1);
            var b2 = Vector128.LoadUnsafe(in right._b1);
            var b3 = Vector128.LoadUnsafe(in right._c1);
            var b4 = Vector128.LoadUnsafe(in right._d1);
            var r1 = a1 + b1;
            var r2 = a2 + b2;
            var r3 = a3 + b3;
            var r4 = a4 + b4;
            var ret = default(Matrix4<T>);
            Vector128.StoreUnsafe(r1, ref ret._a1);
            Vector128.StoreUnsafe(r2, ref ret._b1);
            Vector128.StoreUnsafe(r3, ref ret._c1);
            Vector128.StoreUnsafe(r4, ref ret._d1);
            return ret;
            }
        else {
            return new(
                left.A1 + right.A1, left.A2 + right.A2, left.A3 + right.A3, left.A4 + right.A4,
                left.B1 + right.B1, left.B2 + right.B2, left.B3 + right.B3, left.B4 + right.B4,
                left.C1 + right.C1, left.C2 + right.C2, left.C3 + right.C3, left.C4 + right.C4,
                left.D1 + right.D1, left.D2 + right.D2, left.D3 + right.D3, left.D4 + right.D4
                );
            }
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix4<T> operator -(in Matrix4<T> left, in Matrix4<T> right) {
        return Subtract(left, right);
        }

    [BunnyAttributes.SIMDCandidate]
    public static Vector4<T> operator *(in Matrix4<T> matrix, Vector4<T> vector) {
        return new(
            matrix.A1 * vector.X + matrix.B1 * vector.Y + matrix.C1 * vector.Z + matrix.D1 * vector.W,
            matrix.A2 * vector.X + matrix.B2 * vector.Y + matrix.C2 * vector.Z + matrix.D2 * vector.W,
            matrix.A3 * vector.X + matrix.B3 * vector.Y + matrix.C3 * vector.Z + matrix.D3 * vector.W,
            matrix.A4 * vector.X + matrix.B4 * vector.Y + matrix.C4 * vector.Z + matrix.D4 * vector.W
            );
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Matrix4<T> Multiply(in Matrix4<T> other) {
        return Multiply(this, other);
        }

    public static Matrix4<T> Multiply(in Matrix4<T> left, in Matrix4<T> right) {
        if (Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && typeof(T) == typeof(float)) {
            var m1ABt = Vector256.LoadUnsafe(in left._a1);
            var m1CDt = Vector256.LoadUnsafe(in left._c1);
            var m2ABt = Vector256.LoadUnsafe(in right._a1);
            var m2CDt = Vector256.LoadUnsafe(in right._c1);

            var m2AB = Unsafe.As<Vector256<T>, Vector256<float>>(ref m2ABt);
            var m2CD = Unsafe.As<Vector256<T>, Vector256<float>>(ref m2CDt);

            Vector256<float> v1, v2;

            v1 = Vector256.Shuffle(m2AB, Vector256.Create(0, 4, 0, 4, 1, 5, 1, 5));
            v2 = Vector256.Shuffle(m2CD, Vector256.Create(0, 4, 0, 4, 1, 5, 1, 5));
            var t = BitConverter.Int32BitsToSingle(-1); var f = 0f;
            var m2EF = Vector256.ConditionalSelect(
                condition: Vector256.Create(t, t, f, f, t, t, f, f),
                left: v1,
                right: v2
                ); // m2AB[0], m2AB[4], m2CD[0], m2CD[4], m2AB[1], m2AB[5], m2CD[1], m2CD[5]

            v1 = Vector256.Shuffle(m2AB, Vector256.Create(2, 6, 2, 6, 3, 7, 3, 7));
            v2 = Vector256.Shuffle(m2CD, Vector256.Create(2, 6, 2, 6, 3, 7, 3, 7));
            var m2GH = Vector256.ConditionalSelect(
                condition: Vector256.Create(t, t, f, f, t, t, f, f),
                left: v1,
                right: v2
                ); // m2AB[2], m2AB[6], m2CD[2], m2CD[6], m2AB[3], m2AB[7], m2CD[3], m2CD[7]

            var m2FE = Vector256.Shuffle(m2EF, Vector256.Create(4, 5, 6, 7, 0, 1, 2, 3));
            var m2HG = Vector256.Shuffle(m2GH, Vector256.Create(4, 5, 6, 7, 0, 1, 2, 3));

            var m2EFt = Unsafe.As<Vector256<float>, Vector256<T>>(ref m2EF);
            var m2FEt = Unsafe.As<Vector256<float>, Vector256<T>>(ref m2FE);
            var m2GHt = Unsafe.As<Vector256<float>, Vector256<T>>(ref m2GH);
            var m2HGt = Unsafe.As<Vector256<float>, Vector256<T>>(ref m2HG);

            var m1 = m1ABt * m2EFt; // A1 & B2
            var m2 = m1ABt * m2GHt; // A3 & B4
            var m3 = m1CDt * m2EFt; // C1 & D2
            var m4 = m1CDt * m2GHt; // C3 & D4

            var m5 = m1ABt * m2FEt; // A2 & B1
            var m6 = m1ABt * m2HGt; // A4 & B3
            var m7 = m1CDt * m2FEt; // C2 & D1
            var m8 = m1CDt * m2HGt; // C4 & D3

            return new(
                Vector128.Sum(m1.GetLower()), Vector128.Sum(m5.GetLower()), Vector128.Sum(m2.GetLower()), Vector128.Sum(m6.GetLower()),
                Vector128.Sum(m5.GetUpper()), Vector128.Sum(m1.GetUpper()), Vector128.Sum(m6.GetUpper()), Vector128.Sum(m2.GetUpper()),
                Vector128.Sum(m3.GetLower()), Vector128.Sum(m7.GetLower()), Vector128.Sum(m4.GetLower()), Vector128.Sum(m8.GetLower()),
                Vector128.Sum(m7.GetUpper()), Vector128.Sum(m3.GetUpper()), Vector128.Sum(m8.GetUpper()), Vector128.Sum(m4.GetUpper())
                );
            }
        else if (Vector512.IsHardwareAccelerated && typeof(T) == typeof(double)) {
            var m1ABt = Vector512.LoadUnsafe(in left._a1);
            var m1CDt = Vector512.LoadUnsafe(in left._c1);
            var m2ABt = Vector512.LoadUnsafe(in right._a1);
            var m2CDt = Vector512.LoadUnsafe(in right._c1);

            var m2AB = Unsafe.As<Vector512<T>, Vector512<double>>(ref m2ABt);
            var m2CD = Unsafe.As<Vector512<T>, Vector512<double>>(ref m2CDt);

            Vector512<double> v1, v2;

            v1 = Vector512.Shuffle(m2AB, Vector512.Create(0, 4, 0, 4, 1, 5, 1, 5));
            v2 = Vector512.Shuffle(m2CD, Vector512.Create(0, 4, 0, 4, 1, 5, 1, 5));
            var t = BitConverter.Int32BitsToSingle(-1); var f = 0f;
            var m2EF = Vector512.ConditionalSelect(
                condition: Vector512.Create(t, t, f, f, t, t, f, f),
                left: v1,
                right: v2
                ); // m2AB[0], m2AB[4], m2CD[0], m2CD[4], m2AB[1], m2AB[5], m2CD[1], m2CD[5]

            v1 = Vector512.Shuffle(m2AB, Vector512.Create(2, 6, 2, 6, 3, 7, 3, 7));
            v2 = Vector512.Shuffle(m2CD, Vector512.Create(2, 6, 2, 6, 3, 7, 3, 7));
            var m2GH = Vector512.ConditionalSelect(
                condition: Vector512.Create(t, t, f, f, t, t, f, f),
                left: v1,
                right: v2
                ); // m2AB[2], m2AB[6], m2CD[2], m2CD[6], m2AB[3], m2AB[7], m2CD[3], m2CD[7]

            var m2FE = Vector512.Shuffle(m2EF, Vector512.Create(4, 5, 6, 7, 0, 1, 2, 3));
            var m2HG = Vector512.Shuffle(m2GH, Vector512.Create(4, 5, 6, 7, 0, 1, 2, 3));

            var m2EFt = Unsafe.As<Vector512<double>, Vector512<T>>(ref m2EF);
            var m2FEt = Unsafe.As<Vector512<double>, Vector512<T>>(ref m2FE);
            var m2GHt = Unsafe.As<Vector512<double>, Vector512<T>>(ref m2GH);
            var m2HGt = Unsafe.As<Vector512<double>, Vector512<T>>(ref m2HG);

            var m1 = m1ABt * m2EFt; // A1 & B2
            var m2 = m1ABt * m2GHt; // A3 & B4
            var m3 = m1CDt * m2EFt; // C1 & D2
            var m4 = m1CDt * m2GHt; // C3 & D4

            var m5 = m1ABt * m2FEt; // A2 & B1
            var m6 = m1ABt * m2HGt; // A4 & B3
            var m7 = m1CDt * m2FEt; // C2 & D1
            var m8 = m1CDt * m2HGt; // C4 & D3

            return new(
                Vector256.Sum(m1.GetLower()), Vector256.Sum(m5.GetLower()), Vector256.Sum(m2.GetLower()), Vector256.Sum(m6.GetLower()),
                Vector256.Sum(m5.GetUpper()), Vector256.Sum(m1.GetUpper()), Vector256.Sum(m6.GetUpper()), Vector256.Sum(m2.GetUpper()),
                Vector256.Sum(m3.GetLower()), Vector256.Sum(m7.GetLower()), Vector256.Sum(m4.GetLower()), Vector256.Sum(m8.GetLower()),
                Vector256.Sum(m7.GetUpper()), Vector256.Sum(m3.GetUpper()), Vector256.Sum(m8.GetUpper()), Vector256.Sum(m4.GetUpper())
                );
            }
        else {
            return new(
                left.A1 * right.A1 + left.A2 * right.B1 + left.A3 * right.C1 + left.A4 * right.D1,
                left.A1 * right.A2 + left.A2 * right.B2 + left.A3 * right.C2 + left.A4 * right.D2,
                left.A1 * right.A3 + left.A2 * right.B3 + left.A3 * right.C3 + left.A4 * right.D3,
                left.A1 * right.A4 + left.A2 * right.B4 + left.A3 * right.C4 + left.A4 * right.D4,
                left.B1 * right.A1 + left.B2 * right.B1 + left.B3 * right.C1 + left.B4 * right.D1,
                left.B1 * right.A2 + left.B2 * right.B2 + left.B3 * right.C2 + left.B4 * right.D2,
                left.B1 * right.A3 + left.B2 * right.B3 + left.B3 * right.C3 + left.B4 * right.D3,
                left.B1 * right.A4 + left.B2 * right.B4 + left.B3 * right.C4 + left.B4 * right.D4,
                left.C1 * right.A1 + left.C2 * right.B1 + left.C3 * right.C1 + left.C4 * right.D1,
                left.C1 * right.A2 + left.C2 * right.B2 + left.C3 * right.C2 + left.C4 * right.D2,
                left.C1 * right.A3 + left.C2 * right.B3 + left.C3 * right.C3 + left.C4 * right.D3,
                left.C1 * right.A4 + left.C2 * right.B4 + left.C3 * right.C4 + left.C4 * right.D4,
                left.D1 * right.A1 + left.D2 * right.B1 + left.D3 * right.C1 + left.D4 * right.D1,
                left.D1 * right.A2 + left.D2 * right.B2 + left.D3 * right.C2 + left.D4 * right.D2,
                left.D1 * right.A3 + left.D2 * right.B3 + left.D3 * right.C3 + left.D4 * right.D3,
                left.D1 * right.A4 + left.D2 * right.B4 + left.D3 * right.C4 + left.D4 * right.D4
                );
            }
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix4<T> operator *(in Matrix4<T> left, in Matrix4<T> right) {
        return Multiply(left, right);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix4<T> operator *(in Matrix4<T> matrix, T factor) {
        return Scale(matrix, factor);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix4<T> operator /(in Matrix4<T> left, in Matrix4<T> right) {
        return Divide(in left, in right);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix4<T> Divide(in Matrix4<T> left, in Matrix4<T> right) {
        return Multiply(left, Invert(right));
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Matrix4<T> Divide(in Matrix4<T> other) {
        return Divide(in this, in other);
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix4<T> operator /(in Matrix4<T> matrix, T factor) {
        return Scale(matrix, T.One / factor);
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
        var a4 = A4.Stringify(digits, integerLength, paddingLength);
        var b1 = B1.Stringify(digits, integerLength, paddingLength);
        var b2 = B2.Stringify(digits, integerLength, paddingLength);
        var b3 = B3.Stringify(digits, integerLength, paddingLength);
        var b4 = B4.Stringify(digits, integerLength, paddingLength);
        var c1 = C1.Stringify(digits, integerLength, paddingLength);
        var c2 = C2.Stringify(digits, integerLength, paddingLength);
        var c3 = C3.Stringify(digits, integerLength, paddingLength);
        var c4 = C4.Stringify(digits, integerLength, paddingLength);
        var d1 = D1.Stringify(digits, integerLength, paddingLength);
        var d2 = D2.Stringify(digits, integerLength, paddingLength);
        var d3 = D3.Stringify(digits, integerLength, paddingLength);
        var d4 = D4.Stringify(digits, integerLength, paddingLength);

        return $"[ [{a1}, {a2}, {a3}, {a4}],\n  [{b1}, {b2}, {b3}, {b4}],\n  [{c1}, {c2}, {c3}, {c4}],\n  [{d1}, {d2}, {d3}, {d4}] ]";
        }

    #endregion Strings

    #region Format Data

    public readonly T[][] RowsToJaggedArray() {
        return [[A1, A2, A3, A4], [B1, B2, B3, B4], [C1, C2, C3, C4], [D1, D2, D3, D4]];
        }

    public readonly T[][] ColumnsToJaggedArray() {
        return [[A1, B1, C1, D1], [A2, B2, C2, D2], [A3, B3, C3, D3], [A4, B4, C4, D4]];
        }

    public readonly T[,] RowsTo2DArray() {
        return new T[4, 4] { { A1, A2, A3, A4 }, { B1, B2, B3, B4 }, { C1, C2, C3, C4 }, { D1, D2, D3, D4 } };
        }

    public readonly T[,] ColumnsTo2DArray() {
        return new T[4, 4] { { A1, B1, C1, D1 }, { A2, B2, C2, D2 }, { A3, B3, C3, D3 }, { A4, B4, C4, D4 } };
        }

    #endregion Format Data

    #region Casting

    public static TMatrix UnsafeCast<TMatrix, TNumeric>(ref Matrix4<T> matrix)
        where TMatrix : unmanaged, IMatrix<TNumeric>
        where TNumeric : unmanaged, IBinaryFloatingPointIeee754<TNumeric>, IMinMaxValue<TNumeric> {
        Debug.Assert((typeof(TNumeric) == typeof(T)) && (Matrix4<T>.MatrixWidth == TMatrix.MatrixWidth) && (Matrix4<T>.MatrixHeight == TMatrix.MatrixHeight));
        return Unsafe.As<Matrix4<T>, TMatrix>(ref matrix);
        }

    public TMatrix UnsafeCast<TMatrix, TNumeric>()
            where TMatrix : unmanaged, IMatrix<TNumeric>
            where TNumeric : unmanaged, IBinaryFloatingPointIeee754<TNumeric>, IMinMaxValue<TNumeric> {
        Debug.Assert((typeof(TNumeric) == typeof(T)) && (Matrix4<T>.MatrixWidth == TMatrix.MatrixWidth) && (Matrix4<T>.MatrixHeight == TMatrix.MatrixHeight));
        return Unsafe.As<Matrix4<T>, TMatrix>(ref this);
        }

    #endregion Casting
    }
