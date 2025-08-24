using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using DTLFormatterApp.Models;
using DTLFormatterApp.Views;

namespace DTLFormatterApp.Services;

/// <summary>
/// Manages the state of UI buttons based on the current application state.
/// </summary>
public class ButtonStateManager
{
    private readonly MainWindow _mainWindow;
    private readonly Dictionary<string, Button> _buttons = new();

    public ButtonStateManager(MainWindow mainWindow)
    {
        _mainWindow = mainWindow ?? throw new ArgumentNullException(nameof(mainWindow));
        InitializeButtons();
    }

    private void InitializeButtons()
    {
        var buttonNames = new[] { "EditModeButton", "CopyFormattedButton", "ClearButton" };
        foreach (var name in buttonNames)
        {
            var button = _mainWindow.FindControl<Button>(name);
            if (button != null)
            {
                _buttons[name] = button;
            }
        }
    }

    /// <summary>
    /// Updates button states based on the current rule expression.
    /// </summary>
    public void UpdateButtonStates(RuleExpression? expression)
    {
        var hasValidRules = expression?.Rules.Any(r => r.Clauses.Count > 0) == true;
        
        SetButtonStates(hasValidRules);
    }

    /// <summary>
    /// Sets all button states at once.
    /// </summary>
    private void SetButtonStates(bool isEnabled)
    {
        foreach (var button in _buttons.Values)
        {
            button.IsEnabled = isEnabled;
        }
    }

    /// <summary>
    /// Updates the edit mode button text based on current edit mode.
    /// </summary>
    public void UpdateEditModeButtonText(bool isEditMode)
    {
        if (_buttons.TryGetValue("EditModeButton", out var editButton))
        {
            editButton.Content = isEditMode ? "View Mode" : "Edit Mode";
        }
    }

    /// <summary>
    /// Disables all action buttons.
    /// </summary>
    public void DisableAllButtons() => SetButtonStates(false);

    /// <summary>
    /// Enables all action buttons.
    /// </summary>
    public void EnableAllButtons() => SetButtonStates(true);
}
