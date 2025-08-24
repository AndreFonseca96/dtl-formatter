using System;
using System.Collections.Generic;
using System.Linq;
using DTLFormatterApp.Models;

namespace DTLFormatterApp.Services;

/// <summary>
/// Parses DTL (Data Transformation Language) rules from string format into structured objects.
/// </summary>
public static class RuleParser
{
    private static readonly HashSet<string> SupportedOperators = new(StringComparer.OrdinalIgnoreCase)
    {
        "in", "equals", "gt", "gte", "lt", "lte"
    };

    private static readonly HashSet<string> LogicalOperators = new(StringComparer.OrdinalIgnoreCase)
    {
        "AND", "OR"
    };

    /// <summary>
    /// Parses a DTL rule string into a RuleExpression object.
    /// </summary>
    /// <param name="input">The DTL rule string to parse</param>
    /// <returns>A RuleExpression containing the parsed rules</returns>
    /// <exception cref="ArgumentNullException">Thrown when input is null or empty</exception>
    /// <exception cref="FormatException">Thrown when the input format is invalid</exception>
    public static RuleExpression Parse(string input)
    {
        // Validate input
        if (string.IsNullOrWhiteSpace(input))
        {
            throw new ArgumentNullException(nameof(input), "Input cannot be null or empty");
        }

        var tokens = input.Split('|').Select(t => t.Trim()).ToArray();
        var expression = new RuleExpression();
        var currentRule = new Rule();
        var currentIndex = 0;

        while (currentIndex < tokens.Length)
        {
            var token = tokens[currentIndex];

            if (IsRuleStart(token))
            {
                // Add previous rule if it has clauses
                AddRuleIfValid(expression, currentRule);
                
                // Start new rule
                currentRule = new Rule();
                currentIndex++;
                
                // Parse the first clause of the new rule
                currentIndex = ParseClause(tokens, currentIndex, currentRule);
            }
            else if (IsLogicalOperator(token))
            {
                // Check if this operator is between rules or clauses
                if (IsBetweenRules(tokens, currentIndex))
                {
                    // Add current rule and operator between rules
                    AddRuleIfValid(expression, currentRule);
                    expression.RuleOperators.Add(token.ToUpper());
                    currentRule = new Rule();
                    currentIndex++;
                    
                    // Parse the first clause of the next rule
                    currentIndex = ParseClause(tokens, currentIndex, currentRule);
                }
                else
                {
                    // Operator between clauses within the same rule
                    currentRule.ClauseOperators.Add(token.ToUpper());
                    currentIndex++;
                    
                    // Parse the next clause
                    currentIndex = ParseClause(tokens, currentIndex, currentRule);
                }
            }
            else
            {
                // Skip unrecognized tokens
                currentIndex++;
            }
        }

        // Add the last rule if it has clauses
        AddRuleIfValid(expression, currentRule);

        return expression;
    }

    /// <summary>
    /// Checks if a token represents the start of a new rule.
    /// </summary>
    private static bool IsRuleStart(string token) => 
        token.Equals("Rule", StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Checks if a token is a logical operator (AND/OR).
    /// </summary>
    private static bool IsLogicalOperator(string token) => 
        LogicalOperators.Contains(token);

    /// <summary>
    /// Checks if the current operator is between rules (followed by "Rule").
    /// </summary>
    private static bool IsBetweenRules(string[] tokens, int currentIndex) =>
        currentIndex + 1 < tokens.Length && IsRuleStart(tokens[currentIndex + 1]);

    /// <summary>
    /// Parses a clause (property, operator, value) starting from the given index.
    /// </summary>
    /// <returns>The index after parsing the clause</returns>
    private static int ParseClause(string[] tokens, int startIndex, Rule rule)
    {
        if (startIndex + 2 >= tokens.Length)
        {
            throw new FormatException($"Incomplete clause at position {startIndex}: expected property, operator, and value");
        }

        var property = tokens[startIndex];
        var operatorValue = tokens[startIndex + 1];
        var value = tokens[startIndex + 2];

        ValidateOperator(operatorValue);

        rule.Clauses.Add(new RuleClause
        {
            Property = property,
            Operator = operatorValue,
            Value = value
        });

        return startIndex + 3;
    }

    /// <summary>
    /// Validates that the operator is supported.
    /// </summary>
    private static void ValidateOperator(string operatorValue)
    {
        if (!SupportedOperators.Contains(operatorValue))
        {
            throw new FormatException(
                $"Unsupported operator '{operatorValue}'. " +
                $"Supported operators: {string.Join(", ", SupportedOperators)}");
        }
    }

    /// <summary>
    /// Adds a rule to the expression if it contains at least one clause.
    /// </summary>
    private static void AddRuleIfValid(RuleExpression expression, Rule rule)
    {
        if (rule.Clauses.Count > 0)
        {
            expression.Rules.Add(rule);
        }
    }
}
