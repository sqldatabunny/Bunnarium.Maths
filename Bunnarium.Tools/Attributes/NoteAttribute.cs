namespace Bunnarium.Tools.Attributes;

/// <summary> Provides a home for important developer notes. By including these notes in an <see cref="Attribute"/>, developers can find these notes by searching for instances of this attribute.
/// </summary>
[AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
	public class NoteAttribute : Attribute {

    /// <summary> An arbitrary description for developers.
    /// </summary>
    public string Description;

    /// <inheritdoc cref="NoteAttribute"/>
    /// <param name="description">An arbitrary description for developers.</param>
    public NoteAttribute(string description) => Description = description;

    /// <inheritdoc
    /// cref="NoteAttribute(string)"/>
    public NoteAttribute() => Description = "";
    }
