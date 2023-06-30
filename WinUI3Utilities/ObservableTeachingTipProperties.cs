using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.UI.Xaml.Controls;

namespace WinUI3Utilities;

/// <summary>
/// Contains the properties of <see cref="TeachingTip"/> that can be bound to
/// </summary>
public class ObservableTeachingTipProperties : INotifyPropertyChanged, INotifyPropertyChanging
{
    #region ObservableObject

    /// <inheritdoc cref="INotifyPropertyChanged.PropertyChanged"/>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <inheritdoc cref="INotifyPropertyChanging.PropertyChanging"/>
    public event PropertyChangingEventHandler? PropertyChanging;

    /// <summary>
    /// Raises the <see cref="PropertyChanged"/> event.
    /// </summary>
    /// <param name="propertyName">Caller member name</param>
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new(propertyName));
    }

    /// <summary>
    /// Raises the <see cref="PropertyChanging"/> event.
    /// </summary>
    /// <param name="propertyName">Caller member name</param>
    protected virtual void OnPropertyChanging([CallerMemberName] string propertyName = "")
    {
        PropertyChanging?.Invoke(this, new(propertyName));
    }

    #endregion

    /// <inheritdoc cref="TeachingTip.IsOpen"/>
    public string Title
    {
        get => _title;
        set
        {
            if (value != _title)
                return;
            OnPropertyChanging();
            _title = value;
            OnPropertyChanged();
        }
    }

    /// <inheritdoc cref="TeachingTip.Subtitle"/>
    public string Subtitle
    {
        get => _subtitle;
        set
        {
            if (value != _subtitle)
                return;
            OnPropertyChanging();
            _subtitle = value;
            OnPropertyChanged();
        }
    }

    /// <inheritdoc cref="TeachingTip.IconSource"/>
    public IconSource? IconSource
    {
        get => _iconSource;
        set
        {
            if (value != _iconSource)
                return;
            OnPropertyChanging();
            _iconSource = value;
            OnPropertyChanged();
        }
    }

    /// <inheritdoc cref="TeachingTip.IsLightDismissEnabled"/>
    public bool IsLightDismissEnabled
    {
        get => _isLightDismissEnabled;
        set
        {
            if (value != _isLightDismissEnabled)
                return;
            OnPropertyChanging();
            _isLightDismissEnabled = value;
            OnPropertyChanged();
        }
    }

    /// <inheritdoc cref="TeachingTip.IsOpen"/>
    public bool IsOpen
    {
        get => _isOpen;
        set
        {
            if (value != _isOpen)
                return;
            OnPropertyChanging();
            _isOpen = value;
            OnPropertyChanged();
        }
    }

    private string _title = "";
    private string _subtitle = "";
    private IconSource? _iconSource;
    private bool _isLightDismissEnabled;
    private bool _isOpen;
}
