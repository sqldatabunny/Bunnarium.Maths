namespace Bunnarium.Tools.Attributes;

/// <summary> This attribute marks opportunities for optimization later-on. Sometimes, a developer may recognize such an opportunity but they lack the time or expertise to implement the optimization - or maybe they are lazy bunnies who just want to pawn that work onto somebunny else...
/// </summary>
[AttributeUsage(AttributeTargets.All)]
	public class OptimizeAttribute : Attribute {

    /// <summary> An arbitrary description for developers.
    /// </summary>
    public string Description;

    /// <inheritdoc cref="OptimizeAttribute"/>
    /// <param name="description">An arbitrary description for developers.</param>
    public OptimizeAttribute(string description) => Description = description;

    /// <inheritdoc
    /// cref="OptimizeAttribute(string)"/>
    public OptimizeAttribute() => Description = "";
    }


