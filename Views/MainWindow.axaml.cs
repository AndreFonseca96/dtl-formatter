using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Styling;
using System;
using System.Linq;
using DTLFormatterApp.Models;
using DTLFormatterApp.Services;

namespace DTLFormatterApp.Views;

/// <summary>
/// Main window for the DTL Formatter application.
/// </summary>
public partial class MainWindow : Window
{
    private RuleExpression? _currentExpression;
    private bool _isEditMode = false;
    private bool _isDarkMode = false;
    
    // Public properties for services to access
    public RuleExpression? CurrentExpression => _currentExpression;
    public bool IsEditMode => _isEditMode;
    
    // Component managers
    private TreeViewManager? _treeViewManager;
    private ButtonStateManager? _buttonStateManager;
    private ExpandedStateManager? _expandedStateManager;

    public MainWindow()
    {
        InitializeComponent();
        _isDarkMode = true; // App starts in dark mode
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        
        // Initialize component managers
        _treeViewManager = new TreeViewManager(RulesTreeView, _isEditMode, this);
        _buttonStateManager = new ButtonStateManager(this);
        _expandedStateManager = new ExpandedStateManager(RulesTreeView);
        
        // Set initial theme
        SetThemeToggleButtonContent();
    }

    #region Event Handlers

    private void OnLoadDTL(object? sender, RoutedEventArgs e)
    {
        var input = InputBox.Text ?? string.Empty;
        
        // Reset to view mode
        _isEditMode = false;
        _buttonStateManager?.DisableAllButtons();
        
        try
        {
            // Parse the input using RuleParser
            _currentExpression = RuleParser.Parse(input);
            
            // Update UI
            RefreshUI();
            
            StatusText.Text = $"Loaded {_currentExpression.Rules.Count} rule(s) successfully";
        }
        catch (Exception ex)
        {
            StatusText.Text = $"Error: {ex.Message}";
            _currentExpression = null;
            _treeViewManager?.Clear();
        }
    }

    private void OnCreateFromScratch(object? sender, RoutedEventArgs e)
    {
        // Create a new empty rule expression
        _currentExpression = new RuleExpression();
        
        // Add a default rule with one clause
        var newRule = new Rule();
        newRule.Clauses.Add(new RuleClause
        {
            Property = "property",
            Operator = "equals",
            Value = "value"
        });
        
        _currentExpression.Rules.Add(newRule);
        
        // Enable edit mode automatically
        _isEditMode = true;
        
        // Update UI
        RefreshUI();
        
        StatusText.Text = "Created new rule from scratch - Edit mode enabled";
    }

    private void OnToggleEditMode(object? sender, RoutedEventArgs e)
    {
        if (_currentExpression == null) return;
        
        // Save current expanded states before switching modes
        _expandedStateManager?.SaveExpandedStates();
        
        _isEditMode = !_isEditMode;
        
        // Update UI
        RefreshUI();
        
        // Restore expanded states after repopulating
        _expandedStateManager?.RestoreExpandedStates();
    }

    public void OnCreateNewRule(object? sender, RoutedEventArgs e)
    {
        if (_currentExpression == null)
        {
            _currentExpression = new RuleExpression();
        }

        // Save current expanded states before adding new rule
        _expandedStateManager?.SaveExpandedStates();

        // Add a new rule with a default clause
        var newRule = new Rule();
        newRule.Clauses.Add(new RuleClause
        {
            Property = "newproperty",
            Operator = "equals",
            Value = "newvalue"
        });

        _currentExpression.Rules.Add(newRule);
        
        // Update UI
        RefreshUI();
        
        // Restore expanded states after repopulating
        _expandedStateManager?.RestoreExpandedStates();
        
        UpdateStatus();
    }

    public void OnCreateNewClause(object? sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is int ruleIndex && 
            _currentExpression != null && ruleIndex < _currentExpression.Rules.Count)
        {
            var rule = _currentExpression.Rules[ruleIndex];
            
            // Add AND operator if there are existing clauses
            if (rule.Clauses.Count > 0)
            {
                rule.ClauseOperators.Add("AND");
            }
            
            // Add new clause
            rule.Clauses.Add(new RuleClause
            {
                Property = "property",
                Operator = "equals",
                Value = "value"
            });
            
            RefreshUI();
            UpdateStatus();
        }
    }

    public void OnRemoveClause(object? sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is string tag)
        {
            var parts = tag.Split(',');
            if (parts.Length == 2 && int.TryParse(parts[0], out int ruleIndex) && 
                int.TryParse(parts[1], out int clauseIndex))
            {
                if (_currentExpression != null && ruleIndex < _currentExpression.Rules.Count)
                {
                    var rule = _currentExpression.Rules[ruleIndex];
                    if (clauseIndex < rule.Clauses.Count)
                    {
                        // Remove the clause
                        rule.Clauses.RemoveAt(clauseIndex);
                        
                        // Remove corresponding operator if it exists
                        if (clauseIndex < rule.ClauseOperators.Count)
                        {
                            rule.ClauseOperators.RemoveAt(clauseIndex);
                        }
                        else if (clauseIndex > 0 && clauseIndex - 1 < rule.ClauseOperators.Count)
                        {
                            // Remove the operator before this clause
                            rule.ClauseOperators.RemoveAt(clauseIndex - 1);
                        }
                        
                        RefreshUI();
                        UpdateStatus();
                    }
                }
            }
        }
    }

    public void OnRemoveRule(object? sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is int ruleIndex)
        {
            if (_currentExpression != null && ruleIndex < _currentExpression.Rules.Count)
            {
                // Remove the rule
                _currentExpression.Rules.RemoveAt(ruleIndex);
                
                // Remove corresponding operator if it exists
                if (ruleIndex < _currentExpression.RuleOperators.Count)
                {
                    _currentExpression.RuleOperators.RemoveAt(ruleIndex);
                }
                else if (ruleIndex > 0 && ruleIndex - 1 < _currentExpression.RuleOperators.Count)
                {
                    // Remove the operator before this rule
                    _currentExpression.RuleOperators.RemoveAt(ruleIndex - 1);
                }
                
                RefreshUI();
                UpdateStatus();
            }
        }
    }

    private void OnCopyFormattedRules(object? sender, RoutedEventArgs e)
    {
        if (_currentExpression == null || _currentExpression.Rules.Count == 0)
        {
            StatusText.Text = "No rules to copy";
            return;
        }

        try
        {
            var formattedRules = RuleFormatter.FormatRulesToSingleLine(_currentExpression);
            
            // Copy to clipboard
            var clipboard = TopLevel.GetTopLevel(this)?.Clipboard;
            if (clipboard != null)
            {
                clipboard.SetTextAsync(formattedRules);
                StatusText.Text = "Rules copied to clipboard!";
            }
            else
            {
                StatusText.Text = "Clipboard not available";
            }
        }
        catch (Exception ex)
        {
            StatusText.Text = $"Error copying to clipboard: {ex.Message}";
        }
    }
    
    private void OnClearAll(object? sender, RoutedEventArgs e)
    {
        // Clear all rules and reset to starting state
        _currentExpression = null;
        _isEditMode = false;
        
        // Clear the TreeView
        _treeViewManager?.Clear();
        
        // Disable all buttons
        _buttonStateManager?.DisableAllButtons();
        
        // Clear the input box
        InputBox.Text = string.Empty;
        
        // Update status
        StatusText.Text = "Ready";
    }

    private void OnToggleTheme(object? sender, RoutedEventArgs e)
    {
        _isDarkMode = !_isDarkMode;
        
        if (_isDarkMode)
        {
            this.RequestedThemeVariant = ThemeVariant.Dark;
        }
        else
        {
            this.RequestedThemeVariant = ThemeVariant.Light;
        }
        
        SetThemeToggleButtonContent();
    }

    #endregion

    #region Private Methods

    private void RefreshUI()
    {
        // Update TreeView manager with current edit mode
        _treeViewManager = new TreeViewManager(RulesTreeView, _isEditMode, this);
        
        // Populate TreeView
        if (_currentExpression != null)
        {
            _treeViewManager.PopulateTreeView(_currentExpression);
        }
        
        // Update button states
        _buttonStateManager?.UpdateButtonStates(_currentExpression);
        _buttonStateManager?.UpdateEditModeButtonText(_isEditMode);
    }

    public void UpdateStatus()
    {
        if (_currentExpression != null)
        {
            StatusText.Text = $"Modified: {_currentExpression.Rules.Count} rule(s), {_currentExpression.Rules.Sum(r => r.Clauses.Count)} clause(s)";
        }
    }

    private void SetThemeToggleButtonContent()
    {
        if (ThemeToggleButton != null)
        {
            ThemeToggleButton.Content = _isDarkMode ? "☀" : "☾";
        }
    }

    #endregion
}