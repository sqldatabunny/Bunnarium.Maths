namespace Bunnarium.Tools.Attributes;

/// <summary> This attribute typically marks skeleton code structures as planned or deferred, such that the skeleton is there but the implementation hasn't been written yet.
/// </summary>
[AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
	public class PlannedAttribute : Attribute {

    /// <summary> An arbitrary description for developers.
    /// </summary>
    public string Description;

    /// <inheritdoc cref="PlannedAttribute"/>
    /// <param name="description">An arbitrary description for developers.</param>
    public PlannedAttribute(string description) => Description = description;

    /// <inheritdoc
    /// cref="PlannedAttribute(string)"/>
    public PlannedAttribute() => Description = "";
    }
