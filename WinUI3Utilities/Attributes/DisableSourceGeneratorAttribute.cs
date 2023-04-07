#region Copyright (c) WinUI3Utilities/WinUI3Utilities
// GPL v3 License
// 
// WinUI3Utilities/WinUI3Utilities
// Copyright (c) 2023 WinUI3Utilities/DisableSourceGeneratorAttribute.cs
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

using System;

namespace WinUI3Utilities.Attributes;

/// <summary>
/// Indicates that the source generator is disabled. This is usually used for debug purpose
/// </summary>
[AttributeUsage(AttributeTargets.Assembly)]
public class DisableSourceGeneratorAttribute : Attribute
{

}
