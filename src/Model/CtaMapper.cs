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

using System.Runtime.InteropServices.JavaScript;

using Google.Protobuf;

using MMKiwi.CtaTracker.Model.BusTracker;
using MMKiwi.CtaTracker.Model.Protobuf;

using Riok.Mapperly.Abstractions;

namespace MMKiwi.CtaTracker.Model;

[Mapper]
public static partial class CtaMapper
{
    public static ErrorList ToProtobuf(this IEnumerable<BusError> errors)
    {
        var result = new ErrorList();
        result.Message.AddRange(errors.Select(x=>x.Message));
        return result;
    }
}