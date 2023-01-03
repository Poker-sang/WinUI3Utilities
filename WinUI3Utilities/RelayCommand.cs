using System;

namespace WinUI3Utilities;

/// <summary>
/// A relay command
/// </summary>
public class RelayCommand : System.Windows.Input.ICommand
{
    private readonly Func<object?, bool> _canExecuteFunc;
    private readonly Action<object?> _executeAction;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="canExecute"></param>
    /// <param name="execute"></param>
    public RelayCommand(Func<object?, bool> canExecute, Action<object?> execute)
    {
        _canExecuteFunc = canExecute;
        _executeAction = execute;
    }

    /// <summary>
    /// <see cref="CanExecute"/> is always <see langword="true"/>
    /// </summary>
    /// <param name="execute"></param>
    public RelayCommand(Action<object?> execute)
    {
        _canExecuteFunc = _ => true;
        _executeAction = execute;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="parameter"></param>
    /// <returns></returns>
    public bool CanExecute(object? parameter) => _canExecuteFunc(parameter);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="parameter"></param>
    public void Execute(object? parameter) => _executeAction(parameter);

    /// <summary>
    /// Invoked when call <see cref="OnCanExecuteChanged"/>
    /// </summary>
    public event EventHandler? CanExecuteChanged;

    /// <summary>
    /// Invoke <see cref="CanExecuteChanged"/>
    /// </summary>
    public void OnCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}
