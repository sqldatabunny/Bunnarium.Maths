namespace Bunnarium.Tools.Attributes;

/// <summary> This attribute marks symbols that notably require benchmarking.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class BenchmarkAttribute : Attribute {

    /// <summary> An arbitrary description for developers.
    /// </summary>
    public string Description;

    /// <param name="description"> An arbitrary description for developers.
    /// </param>
    /// <inheritdoc cref="BenchmarkAttribute"/>
    public BenchmarkAttribute(string description) => Description = description;

    /// <inheritdoc
    /// cref="BenchmarkAttribute"/>
    public BenchmarkAttribute() => Description = "";
    }
