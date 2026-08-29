# Bunnarium.Maths

Welcome to Bunnarium, an in-development game engine powered by bunnies! 🐇

![Bunnarium Engine (logo by Mary)](Logo-Transparent.png)

What you see here is an early, partial release of Bunnarium's mathematics library. Specifically, this library contains its primitives (e.g., vectors, matrices, and so on). This library has been in-development since 2021 and is the culmination of several iterations of attempting to create a clean and efficient backbone for the rest of the engine. The initial release of this library and its primitives will be followed by the incremental release of 2D and 3D geometry primitives and other mathematic tools. Eventually, other namespaces (**Bunnarium.Tools**, **Bunnarium.Linguistics** and, eventually, **Bunnarium.Engine**) will be released as related repositories.

I did my best to guarantee accuracy and consistency in this library, but it is a lot to manage a solo developer. If you find any mistakes then it would be greatly appreciated if you let me know.

## Features
* Generic Math Support: Create primitives with single, double, or half precision!
* A framework for creating 2D/3D-agnostic code! (unless you need cross products)
* Matrix2, 3, 4, 2x3, 3x2, 3x4, and 4x3!
* Varying levels of implemented vectorization!
* A bundle of SIMD helpers!