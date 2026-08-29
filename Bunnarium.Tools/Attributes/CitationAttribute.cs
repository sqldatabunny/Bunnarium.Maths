namespace Bunnarium.Tools.Attributes;

/// <summary> Signals to develops that the attributed symbol needs a formal citation and stores information on what and where the cited source is.
/// </summary>
[AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
public class CitationAttribute : Attribute {

    #region Data

    /// <summary> A description or a link to the source.
    /// </summary>
    public string Description;

    /// <summary> The page number of the source, if applicable (for example, if the source is in a book).
    /// </summary>
    public int PageNumber;

    /// <summary> A URL to the source, if available.
    /// </summary>
    public string? URL;

    #endregion Data

    #region Constructors

    /// <param name="description">A description or a link to the source.</param>
    /// <param name="pageNumber">The page number of the source, if applicable (for example, if the source is in a book).</param>
    /// <inheritdoc cref="CitationAttribute"/>
    public CitationAttribute(string description, string? url = "", int pageNumber = -1) {
        Description = description;
        URL = url;
        PageNumber = pageNumber;
        }

    /// <inheritdoc
    /// cref="CitationAttribute"/>
    public CitationAttribute() => Description = string.Empty;

    #endregion Constructors
    }
