#region Copyright (c) WinUI3Utilities/WinUI3Utilities

// GPL v3 License
// 
// WinUI3Utilities/WinUI3Utilities
// Copyright (c) 2023 WinUI3Utilities/TeachingTipSeverity.cs
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

#endregion

using Microsoft.UI.Xaml.Controls;

namespace WinUI3Utilities;

/// <summary>
/// Snack bar severity on <see cref="TeachingTip.IconSource"/> (Segoe Fluent Icons font)
/// </summary>
public enum TeachingTipSeverity
{
    /// <summary>
    /// Accept (E10B)
    /// </summary>
    Ok,

    /// <summary>
    /// Info (E946)
    /// </summary>
    Information,

    /// <summary>
    /// Important (E171)
    /// </summary>
    Important,

    /// <summary>
    /// Warning (E7BA)
    /// </summary>
    Warning,

    /// <summary>
    /// ErrorBadge (EA39)
    /// </summary>
    Error,

    /// <summary>
    /// <see langword="null"/>
    /// </summary>
    None
}
