# DTL Editor Application

A modern, cross-platform desktop application for parsing, editing, and formatting and editing DTL.

---

# Domain Toggle Language (DTL)

**DTL** is a lightweight domain-specific language for defining and managing feature toggle rules.  
It provides a clear, human-readable syntax for describing conditions under which a feature should be enabled or disabled.

---

## Why DTL?

Managing feature toggles often becomes messy when rules are hidden deep in code or scattered across config files.  
**DTL** solves this by:

1. **Clarity** – A consistent, structured syntax.
2. **Simplicity** – Easy to read, write, and validate.
3. **Flexibility** – Supports multiple properties, operators, and logical clauses.
4. **Cross-platform tooling** – A lightweight desktop editor/formatter (built with Avalonia UI) helps create and validate rules.

---

## 🚀 Features

### Core Functionality

- **Load DTL Rules**: Parse and display DTL rules from input text
- **Create from Scratch**: Start with a blank rule template and build rules interactively
- **Edit Mode**: Toggle between view and edit modes for rule management
- **Copy Formatted Rules**: Copy formatted rules to clipboard (enabled only when rules exist)
- **Clear All**: Reset everything to starting state with one click
- **Dark/Light Theme**: Toggle between dark and light themes for better user experience

### Advanced Features

- **TreeView Visualization**: Hierarchical display of rules and clauses
- **Real-time Editing**: Edit rule properties, operators, and values directly in the UI
- **State Persistence**: Remembers expanded/collapsed states of TreeView items
- **Validation**: Built-in validation for DTL rule format
- **Operator Support**: Full support for all DTL operators (in, equals, gt, gte, lt, lte)

## 📋 DTL Format Support

The application supports DTL rules in the following format:

```
Rule|property|operator|value|AND|Rule|property2|operator2|value2
```

### Supported Operators

- `in` - Contains (e.g., "country in US,CA,UK")
- `equals` - Exact match (e.g., "status equals active")
- `gt` - Greater than (e.g., "version gt 1.0")
- `gte` - Greater than or equal (e.g., "version gte 1.0")
- `lt` - Less than (e.g., "version lt 2.0")
- `lte` - Less than or equal (e.g., "version lte 2.0")

### Logical Operators

- `AND` - Logical AND between clauses or rules
- `OR` - Logical OR between clauses or rules

### Namespace Organization

- `DTLFormatterApp.Models` - Data models for rules and expressions
- `DTLFormatterApp.Services` - Business logic, parsing, and UI management services
- `DTLFormatterApp.Views` - UI components and windows
- `DTLFormatterApp` - Root namespace for the application

## 🛠️ Building and Running

### Prerequisites

- .NET 9.0 SDK or later
- Any operating system supported by .NET (Windows, macOS, Linux)

### Build and Run

```bash
# Navigate to the project directory
cd DTLFormatter/DTLFormatterApp

# Build the project
dotnet build

# Run the application
dotnet run
```

### Development

```bash
# Clean build
dotnet clean
dotnet build

# Run with specific configuration
dotnet run --configuration Release

# Build for specific platform
dotnet publish -c Release -r win-x64 --self-contained
```

## 📖 Usage Guide

### Getting Started

1. **Launch the Application**

   - Run `dotnet run` from the project directory
   - The application starts in dark mode by default

2. **Load Existing DTL Rules**

   - Paste your DTL rules into the input text box
   - Click "Load DTL Rules" to parse and display them
   - Rules will appear in the TreeView on the right

3. **Create Rules from Scratch**
   - Click "Create from Scratch" to start with a blank template
   - The application automatically enables edit mode
   - Use the "+" buttons to add new rules and clauses

### Editing Rules

1. **Toggle Edit Mode**

   - Click "Edit Mode" to switch between view and edit modes
   - In edit mode, you can modify rule properties directly

2. **Adding Rules and Clauses**

   - Click the "+" button next to a rule to add a new clause
   - Click the "+" button at the bottom to add a new rule
   - Use the "-" buttons to remove rules or clauses

3. **Modifying Properties**

   - In edit mode, click on property values to edit them
   - Use the dropdown menus to select operators
   - Changes are applied in real-time

4. **Copying Formatted Rules**
   - Click "Copy Formatted Rules" to copy the current rules to clipboard
   - The formatted output follows the DTL pipe-separated format

### Theme and Preferences

- **Toggle Theme**: Click the theme button (☀/☾) to switch between dark and light modes
- **TreeView State**: The application remembers which items are expanded/collapsed
- **Clear All**: Use the "Clear" button to reset everything to the starting state

## 🔧 Technical Details

### Key Technologies

- **Avalonia UI**: Cross-platform UI framework
- **.NET 9**: Latest .NET runtime
- **C#**: Primary programming language
- **XAML**: UI markup language

## 🚀 Future Enhancements

### Planned Features

- **Validation Rules**: Custom validation for specific rule types
- **Export Formats**: Support for JSON, XML, and other formats
- **Undo/Redo**: History management for rule changes
- **Search and Filter**: Find specific rules or clauses

### Technical Improvements

- **Unit Tests**: Comprehensive test coverage
- **Performance Profiling**: Optimize for large rule sets
- **Accessibility**: Screen reader and keyboard navigation support
- **Internationalization**: Multi-language support
- **Plugin System**: Extensible architecture for custom features

## 🤝 Contributing

### Development Setup

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests for new functionality
5. Ensure all tests pass
6. Submit a pull request

### Code Style

- Follow C# coding conventions
- Use XML documentation for public APIs
- Maintain single responsibility principle
- Write self-documenting code
- Add comments for complex logic

## 🐛 Issues and Support

If you encounter any issues or have questions:

1. Check the existing issues in the repository
2. Create a new issue with detailed information
3. Include steps to reproduce the problem
4. Provide system information and error messages

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.

---

**DTL Editor Application** - Making DTL rule management simple and efficient! 🎯
