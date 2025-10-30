// Copyright (C) 2025 Micah Makaiwi

// This program is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General
// Public License as published by the Free Software Foundation, version 3.
//
//This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied
//warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more
//details.
//
//You should have received a copy of the GNU Affero General Public License along with this program. If not, see
//<https://www.gnu.org/licenses/>.

using System.Diagnostics.CodeAnalysis;

namespace MMKiwi.CtaTracker.Model.BusTracker;

public readonly record struct Response<T>
    where T : class
{
    public Response(T result)
    {
        ArgumentNullException.ThrowIfNull(result);
        Value = result;
    }

    public Response(IReadOnlyList<BusError> error)
    {
        ArgumentNullException.ThrowIfNull(error);
        Value = error;
    }

    public T? Result => Value as T;
    
    public IReadOnlyList<BusError>? Errors => Value as IReadOnlyList<BusError>;

    private object Value { get; }

    public bool TryGetResult([NotNullWhen(true)] out T? result)
    {
        result = Result;
        return Result is not null;
    }
    
    public bool TryGetErrors([NotNullWhen(true)] out IReadOnlyList<BusError>? errors)
    {
        errors = Errors;
        return Errors is not null;
    }

    public T GetResultOrThrow() => Result ?? throw new InvalidOperationException($"Error from CTA Server:\n{string.Join('\n',Errors ?? [])}");
}
