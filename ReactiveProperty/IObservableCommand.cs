// ------------------------------------------------------
// Copyright (C) Microsoft. All rights reserved.
// ------------------------------------------------------

using System.Threading.Tasks;
using System.Windows.Input;

namespace ReactiveProperty
{
    public interface IObservableCommand<in TInput, TOutput> : IProperty<TOutput>, ICommand
    {
        IReadOnlyProperty<bool> IsEnabled { get; }

        IReadOnlyProperty<bool> IsExecuting { get; }

        new IReadOnlyProperty<bool> CanExecute { get; }

        Task<TOutput> ExecuteAsync(TInput parameter);
    }
}