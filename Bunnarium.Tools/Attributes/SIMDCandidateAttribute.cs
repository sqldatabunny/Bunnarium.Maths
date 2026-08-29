namespace Bunnarium.Tools.Attributes;

/// <summary> This attribute marks opportunities to implement SIMD vectorization in hot codepaths.
/// </summary>
[AttributeUsage(AttributeTargets.All)]
public class SIMDCandidateAttribute : OptimizeAttribute {

    /// <inheritdoc cref="SIMDCandidateAttribute"/>
    /// <param name="description">An arbitrary description for developers.</param>
    public SIMDCandidateAttribute(string description) : base(description) { }

    /// <inheritdoc
    /// cref="SIMDCandidateAttribute(string)"/>
    public SIMDCandidateAttribute() => Description = "";

    }
