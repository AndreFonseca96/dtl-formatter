using System.Collections.Generic;

namespace DTLFormatterApp.Models;

/// <summary>
/// Represents a rule containing multiple clauses and operators between them.
/// </summary>
public class Rule
{
    /// <summary>
    /// The list of clauses that make up this rule
    /// </summary>
    public List<RuleClause> Clauses { get; set; } = new();
    
    /// <summary>
    /// The operators between clauses within this rule (AND / OR)
    /// </summary>
    public List<string> ClauseOperators { get; set; } = new();
}
