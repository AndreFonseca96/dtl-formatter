using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DTLFormatterApp.Models;

namespace DTLFormatterApp.Services;

/// <summary>
/// Handles formatting of rule expressions into various output formats.
/// </summary>
public static class RuleFormatter
{
    private static readonly Dictionary<string, string> OperatorDescriptions = new()
    {
        ["gt"] = "gt (greater than)",
        ["gte"] = "gte (greater than or equal)",
        ["lt"] = "lt (less than)",
        ["lte"] = "lte (less than or equal)",
        ["in"] = "in (contains)",
        ["equals"] = "equals (exact match)"
    };

    /// <summary>
    /// Formats a rule expression into a single-line DTL string format.
    /// </summary>
    /// <param name="expression">The rule expression to format</param>
    /// <returns>A formatted DTL string</returns>
    public static string FormatRulesToSingleLine(RuleExpression expression)
    {
        if (expression == null) throw new ArgumentNullException(nameof(expression));
        
        var result = new StringBuilder();
        
        for (int i = 0; i < expression.Rules.Count; i++)
        {
            var rule = expression.Rules[i];
            
            // Add rule prefix and operator
            if (i == 0)
            {
                result.Append("Rule");
            }
            else
            {
                var ruleOperator = i - 1 < expression.RuleOperators.Count ? expression.RuleOperators[i - 1] : "";
                result.Append($"|{ruleOperator}|Rule");
            }
            
            // Add all clauses for this rule
            for (int j = 0; j < rule.Clauses.Count; j++)
            {
                var clause = rule.Clauses[j];
                result.Append($"|{clause.Property}|{clause.Operator}|{clause.Value}");
                
                // Add clause operator if there's another clause
                if (j < rule.ClauseOperators.Count)
                {
                    result.Append($"|{rule.ClauseOperators[j]}");
                }
            }
        }
        
        return result.ToString();
    }

    /// <summary>
    /// Gets a human-readable description of an operator.
    /// </summary>
    /// <param name="operatorValue">The operator to describe</param>
    /// <returns>A human-readable description</returns>
    public static string GetOperatorDescription(string operatorValue)
    {
        return OperatorDescriptions.TryGetValue(operatorValue.ToLower(), out var description) 
            ? description 
            : operatorValue;
    }
}
