using System;

namespace WinUI3Utilities.Attributes;

/// <summary>
/// 根据设置类属性生成ViewModel属性（可用<see cref="SettingsViewModelExclusionAttribute"/>类，在设置类里指定不生成属性的例外）
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class SettingsViewModelAttribute<T> : Attribute
{
    public SettingsViewModelAttribute(string settingName) => SettingName = settingName;

    public string SettingName { get; }
}
