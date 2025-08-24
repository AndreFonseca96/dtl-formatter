using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DTLFormatterApp.Services;

/// <summary>
/// Manages the expanded state of TreeView items to preserve user's view preferences.
/// </summary>
public class ExpandedStateManager
{
    private readonly TreeView _treeView;
    private readonly Dictionary<string, bool> _expandedStates = new();

    public ExpandedStateManager(TreeView treeView)
    {
        _treeView = treeView ?? throw new ArgumentNullException(nameof(treeView));
    }

    /// <summary>
    /// Saves the current expanded states of all TreeView items.
    /// </summary>
    public void SaveExpandedStates()
    {
        _expandedStates.Clear();
        ProcessTreeViewItems(_treeView.Items.Cast<object>(), "", SaveItemState);
    }

    /// <summary>
    /// Restores the previously saved expanded states.
    /// </summary>
    public void RestoreExpandedStates()
    {
        ProcessTreeViewItems(_treeView.Items.Cast<object>(), "", RestoreItemState);
    }

    /// <summary>
    /// Clears all saved expanded states.
    /// </summary>
    public void ClearExpandedStates() => _expandedStates.Clear();

    private void ProcessTreeViewItems(IEnumerable<object> items, string path, Action<TreeViewItem, string> action)
    {
        var itemList = items.ToList();
        for (int i = 0; i < itemList.Count; i++)
        {
            if (itemList[i] is TreeViewItem treeViewItem)
            {
                var headerText = ExtractHeaderText(treeViewItem);
                var currentPath = string.IsNullOrEmpty(path) ? $"{i}:{headerText}" : $"{path}.{i}:{headerText}";
                
                if (!string.IsNullOrEmpty(currentPath))
                {
                    action(treeViewItem, currentPath);
                }
                
                if (treeViewItem.Items.Count > 0)
                {
                    ProcessTreeViewItems(treeViewItem.Items.Cast<object>(), currentPath, action);
                }
            }
        }
    }

    private void SaveItemState(TreeViewItem treeViewItem, string path)
    {
        _expandedStates[path] = treeViewItem.IsExpanded;
    }

    private void RestoreItemState(TreeViewItem treeViewItem, string path)
    {
        if (_expandedStates.TryGetValue(path, out bool wasExpanded))
        {
            treeViewItem.IsExpanded = wasExpanded;
        }
    }

    private static string ExtractHeaderText(TreeViewItem treeViewItem)
    {
        return treeViewItem.Header switch
        {
            string strHeader => strHeader,
            StackPanel panel when panel.Children.Count > 0 && panel.Children[0] is TextBlock textBlock => textBlock.Text ?? "",
            _ => ""
        };
    }
}

