# Bunnarium.Maths

Welcome to Bunnarium, an in-development game engine powered by bunnies! 🐇

![Bunnarium Engine (logo by Mary)](Logo-Transparent.png)

What you see here is an early, partial release of Bunnarium's mathematics library. Specifically, this library contains its primitives (e.g., vectors, matrices, and so on). In-development since 2021, this is the culmination of several iterations of attempts to create a clean and efficient backbone for the rest of the engine. The initial release of this library and its primitives will be followed by the incremental release of 2D and 3D geometry primitives and other mathematic tools. Eventually, other namespaces (**Bunnarium.Tools**, **Bunnarium.Linguistics** and, eventually, **Bunnarium.Engine**) will be released as related repositories. Accordingly, members in the **Bunnarium.Tools** folder will be moved to their respective repository, which will be made a dependency of this one.

I did my best to guarantee accuracy and consistency in this library, but it is a lot to manage a solo developer. If you find any mistakes then it would be greatly appreciated if you let me know.

## Features
* Generic Math Support: Create primitives with single, double, or half precision!
* A framework for creating 2D/3D-agnostic code! (unless you need cross products)
* Matrix2, 3, 4, 2x3, 3x2, 3x4, and 4x3!
* Support for selecting a matrix handedness system (i.e., left-handed or right-handed) and a multiplication style (i.e., row-major premultiplication style or column-major postmultiplication), all via compiler symbols!
* Varying levels of implemented vectorization!
* A bundle of SIMD helpers!

⚠️ This library is subject to revisions! Breaking changes will be minimized, but changes such as member promotions up interface chains *may* be disruptive depending on how you use this library.

## Instructions

* To set matrix handedness, apply one of MATRIX_LEFTHANDED_COORDINATE_SYSTEM and MATRIX_RIGHTHANDED_COORDINATE_SYSTEM. Exactly one of these must be set!
* To set matrix multiplication convention, apply one of MATRIX_PREMULTIPLIED_CONVENTION and MATRIX_POSTMULTIPLIED_CONVENTION. Again, exactly one of these must be set!
* These symbols will apply to matrix types such as Matrix4. To access convention- or handedness-specific function, use the specified APIs in the static Matrix library (Matrix.cs).