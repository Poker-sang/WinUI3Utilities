#region Copyright (c) WinUI3Utilities/WinUI3Utilities

// GPL v3 License
// 
// WinUI3Utilities/WinUI3Utilities
// Copyright (c) 2023 WinUI3Utilities/DependencyPropertyDefaultValue.cs
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

using Microsoft.UI.Xaml;

namespace WinUI3Utilities.Attributes;

/// <summary>
/// Possible value of property
/// </summary>
public enum DependencyPropertyDefaultValue
{
    /// <summary>
    /// <see cref="DependencyProperty.UnsetValue"/>
    /// </summary>
    UnsetValue,
    /// <summary>
    /// <see langword="default"/>(T)
    /// </summary>
    Default,
    /// <summary>
    /// <see langword="new"/> T()
    /// </summary>
    /// <remarks>
    /// Call non-parameter construction
    /// </remarks>
    New
}
