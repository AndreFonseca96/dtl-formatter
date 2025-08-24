using System.Collections.Generic;
using System.Linq;

namespace DTLFormatterApp.Models;

/// <summary>
/// Represents a complete rule expression containing multiple rules and operators between them.
/// </summary>
public class RuleExpression
{
    /// <summary>
    /// The list of rules that make up this expression
    /// </summary>
    public List<Rule> Rules { get; set; } = new();
    
    /// <summary>
    /// The operators between different rules (AND / OR)
    /// </summary>
    public List<string> RuleOperators { get; set; } = new();
    
    /// <summary>
    /// Legacy property for backward compatibility - flattens all clauses from all rules
    /// </summary>
    public List<RuleClause> Clauses => Rules.SelectMany(r => r.Clauses).ToList();
    
    /// <summary>
    /// Legacy property for backward compatibility - returns rule operators
    /// </summary>
    public List<string> ClauseOperators => RuleOperators;
}
