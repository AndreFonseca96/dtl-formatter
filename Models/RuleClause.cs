namespace DTLFormatterApp.Models;

/// <summary>
/// Represents a single clause within a rule, containing a property, operator, and value.
/// </summary>
public class RuleClause
{
    /// <summary>
    /// The property name to check (e.g., "country", "appversion")
    /// </summary>
    public string Property { get; set; } = string.Empty;
    
    /// <summary>
    /// The operator to use for comparison (e.g., "in", "equals", "gt", "gte", "lt", "lte")
    /// </summary>
    public string Operator { get; set; } = string.Empty;
    
    /// <summary>
    /// The value to compare against
    /// </summary>
    public string Value { get; set; } = string.Empty;
}
