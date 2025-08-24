using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using DTLFormatterApp.Models;
using DTLFormatterApp.Views;

namespace DTLFormatterApp.Services;

/// <summary>
/// Manages the TreeView operations including population, state management, and UI interactions.
/// </summary>
public class TreeViewManager
{
    private readonly TreeView _treeView;
    private readonly bool _isEditMode;
    private readonly MainWindow _mainWindow;
    
    private static readonly string[] SupportedOperators = { "in", "equals", "gt", "gte", "lt", "lte" };
    private static readonly string[] ClauseOperators = { "AND", "OR" };

    public TreeViewManager(TreeView treeView, bool isEditMode, MainWindow mainWindow)
    {
        _treeView = treeView ?? throw new ArgumentNullException(nameof(treeView));
        _isEditMode = isEditMode;
        _mainWindow = mainWindow ?? throw new ArgumentNullException(nameof(mainWindow));
    }

    /// <summary>
    /// Populates the TreeView with the given rule expression.
    /// </summary>
    public void PopulateTreeView(RuleExpression expression)
    {
        if (expression == null) throw new ArgumentNullException(nameof(expression));
        
        _treeView.Items.Clear();
        
        for (int i = 0; i < expression.Rules.Count; i++)
        {
            var rule = expression.Rules[i];
            var ruleNode = CreateRuleNode(rule, i);
            
            // Add clauses to the rule
            AddClausesToRule(ruleNode, rule, i);
            
            _treeView.Items.Add(ruleNode);
            
            // Add operator between rules if there is one
            if (i < expression.RuleOperators.Count)
            {
                var ruleOperatorNode = CreateRuleOperatorNode(expression.RuleOperators[i]);
                _treeView.Items.Add(ruleOperatorNode);
            }
        }
        
        // Add "Add New Rule" button at the end (only in edit mode)
        if (_isEditMode)
        {
            var newRuleNode = CreateNewRuleButton();
            _treeView.Items.Add(newRuleNode);
        }
    }

    private void AddClausesToRule(TreeViewItem ruleNode, Rule rule, int ruleIndex)
    {
        for (int j = 0; j < rule.Clauses.Count; j++)
        {
            var clause = rule.Clauses[j];
            var clauseNode = CreateClauseNode(clause, ruleIndex, j);
            ruleNode.Items.Add(clauseNode);
            
            // Add operator between clauses if there is one
            if (j < rule.ClauseOperators.Count)
            {
                var operatorNode = CreateClauseOperatorNode(rule.ClauseOperators[j]);
                ruleNode.Items.Add(operatorNode);
            }
        }
    }

    private TreeViewItem CreateRuleNode(Rule rule, int ruleIndex)
    {
        var headerText = $"Rule {ruleIndex + 1} ({rule.Clauses.Count} clause{(rule.Clauses.Count != 1 ? "s" : "")})";
        
        if (_isEditMode)
        {
            var container = CreateHorizontalContainer();
            container.Children.Add(CreateTextBlock(headerText));
            container.Children.Add(CreateActionButton("+", ruleIndex, "add-button", _mainWindow.OnCreateNewClause));
            container.Children.Add(CreateActionButton("-", ruleIndex, "remove-button", _mainWindow.OnRemoveRule));
            
            return new TreeViewItem { Header = container, Classes = { "rule-header" } };
        }
        
        return new TreeViewItem { Header = headerText, Classes = { "rule-header" } };
    }

    private TreeViewItem CreateClauseNode(RuleClause clause, int ruleIndex, int clauseIndex)
    {
        var headerText = $"Clause {clauseIndex + 1}";
        
        if (_isEditMode)
        {
            var container = CreateHorizontalContainer();
            container.Children.Add(CreateTextBlock(headerText));
            container.Children.Add(CreateActionButton("-", $"{ruleIndex},{clauseIndex}", "remove-button", _mainWindow.OnRemoveClause));
            
            var clauseNode = new TreeViewItem { Header = container, Classes = { "clause-header" } };
            AddClauseDetails(clauseNode, clause, ruleIndex, clauseIndex);
            return clauseNode;
        }
        
        var node = new TreeViewItem { Header = headerText, Classes = { "clause-header" } };
        AddClauseDetails(node, clause, ruleIndex, clauseIndex);
        return node;
    }

    private TreeViewItem CreateClauseOperatorNode(string operatorValue)
    {
        if (_isEditMode)
        {
            var comboBox = CreateComboBox(ClauseOperators, operatorValue);
            var container = CreateHorizontalContainer();
            container.Children.Add(CreateTextBlock("→ "));
            container.Children.Add(comboBox);
            
            return new TreeViewItem { Header = container };
        }
        
        return new TreeViewItem { Header = $"→ {operatorValue}" };
    }

    private TreeViewItem CreateRuleOperatorNode(string operatorValue)
    {
        return new TreeViewItem { Header = $"→ {operatorValue}" };
    }

    private TreeViewItem CreateNewRuleButton()
    {
        var button = new Button
        {
            Content = "+",
            Margin = new Avalonia.Thickness(8, 12, 8, 8),
            Padding = new Avalonia.Thickness(12, 8, 12, 8),
            MinWidth = 40,
            MinHeight = 32,
            FontSize = 18,
            FontWeight = FontWeight.Bold,
            Tag = "new-rule",
            Classes = { "add-button" }
        };
        button.Click += _mainWindow.OnCreateNewRule;
        
        return new TreeViewItem { Header = button };
    }

    private void AddClauseDetails(TreeViewItem clauseNode, RuleClause clause, int ruleIndex, int clauseIndex)
    {
        if (_isEditMode)
        {
            clauseNode.Items.Add(CreateEditableProperty(clause.Property, ruleIndex, clauseIndex));
            clauseNode.Items.Add(CreateEditableOperator(clause.Operator, ruleIndex, clauseIndex));
            clauseNode.Items.Add(CreateEditableValue(clause.Value, ruleIndex, clauseIndex));
        }
        else
        {
            clauseNode.Items.Add(new TreeViewItem { Header = $"Property: {clause.Property}" });
            clauseNode.Items.Add(new TreeViewItem { Header = $"Operator: {RuleFormatter.GetOperatorDescription(clause.Operator)}" });
            clauseNode.Items.Add(new TreeViewItem { Header = $"Value: {clause.Value}" });
        }
    }

    private TreeViewItem CreateEditableProperty(string property, int ruleIndex, int clauseIndex)
    {
        var textBox = CreateTextBox(property, 150);
        textBox.TextChanged += (sender, e) => UpdateClauseProperty(ruleIndex, clauseIndex, textBox.Text);
        
        var container = CreateHorizontalContainer();
        container.Children.Add(CreateTextBlock("Property: "));
        container.Children.Add(textBox);
        
        return new TreeViewItem { Header = container, Classes = { "property-item" } };
    }

    private TreeViewItem CreateEditableOperator(string operatorValue, int ruleIndex, int clauseIndex)
    {
        var comboBox = CreateComboBox(SupportedOperators, operatorValue);
        comboBox.SelectionChanged += (sender, e) => UpdateClauseOperator(ruleIndex, clauseIndex, comboBox.SelectedItem);
        
        var container = CreateHorizontalContainer();
        container.Children.Add(CreateTextBlock("Operator: "));
        container.Children.Add(comboBox);
        
        return new TreeViewItem { Header = container, Classes = { "property-item" } };
    }

    private TreeViewItem CreateEditableValue(string value, int ruleIndex, int clauseIndex)
    {
        var textBox = CreateTextBox(value, 200);
        textBox.TextChanged += (sender, e) => UpdateClauseValue(ruleIndex, clauseIndex, textBox.Text);
        
        var container = CreateHorizontalContainer();
        container.Children.Add(CreateTextBlock("Value: "));
        container.Children.Add(textBox);
        
        return new TreeViewItem { Header = container, Classes = { "property-item" } };
    }

    // Helper methods for UI creation
    private static StackPanel CreateHorizontalContainer() => new() { Orientation = Orientation.Horizontal };
    
    private static TextBlock CreateTextBlock(string text) => new() 
    { 
        Text = text, 
        VerticalAlignment = VerticalAlignment.Center 
    };
    
    private static TextBox CreateTextBox(string text, int minWidth) => new()
    {
        Text = text,
        Margin = new Avalonia.Thickness(5, 2, 5, 2),
        MinWidth = minWidth
    };
    
    private static ComboBox CreateComboBox(string[] items, string selectedItem)
    {
        var comboBox = new ComboBox
        {
            Margin = new Avalonia.Thickness(5, 2, 5, 2),
            MinWidth = 150
        };
        
        foreach (var item in items)
        {
            comboBox.Items.Add(item);
        }
        
        comboBox.SelectedItem = selectedItem;
        return comboBox;
    }
    
    private static Button CreateActionButton(string content, object tag, string cssClass, EventHandler<RoutedEventArgs> clickHandler)
    {
        var button = new Button
        {
            Content = content,
            Margin = new Avalonia.Thickness(8, 0, 0, 0),
            Tag = tag,
            Classes = { cssClass }
        };
        button.Click += clickHandler;
        return button;
    }

    // Update methods for clause properties
    private void UpdateClauseProperty(int ruleIndex, int clauseIndex, string? newValue)
    {
        if (_mainWindow.CurrentExpression != null && 
            ruleIndex < _mainWindow.CurrentExpression.Rules.Count && 
            clauseIndex < _mainWindow.CurrentExpression.Rules[ruleIndex].Clauses.Count)
        {
            _mainWindow.CurrentExpression.Rules[ruleIndex].Clauses[clauseIndex].Property = newValue ?? string.Empty;
            _mainWindow.UpdateStatus();
        }
    }

    private void UpdateClauseOperator(int ruleIndex, int clauseIndex, object? selectedItem)
    {
        if (_mainWindow.CurrentExpression != null && 
            ruleIndex < _mainWindow.CurrentExpression.Rules.Count && 
            clauseIndex < _mainWindow.CurrentExpression.Rules[ruleIndex].Clauses.Count &&
            selectedItem != null)
        {
            _mainWindow.CurrentExpression.Rules[ruleIndex].Clauses[clauseIndex].Operator = selectedItem.ToString() ?? string.Empty;
            _mainWindow.UpdateStatus();
        }
    }

    private void UpdateClauseValue(int ruleIndex, int clauseIndex, string? newValue)
    {
        if (_mainWindow.CurrentExpression != null && 
            ruleIndex < _mainWindow.CurrentExpression.Rules.Count && 
            clauseIndex < _mainWindow.CurrentExpression.Rules[ruleIndex].Clauses.Count)
        {
            _mainWindow.CurrentExpression.Rules[ruleIndex].Clauses[clauseIndex].Value = newValue ?? string.Empty;
            _mainWindow.UpdateStatus();
        }
    }

    /// <summary>
    /// Clears all items from the TreeView.
    /// </summary>
    public void Clear() => _treeView.Items.Clear();
}
