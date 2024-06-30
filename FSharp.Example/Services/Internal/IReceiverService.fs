// Receiver Service Interface
//
// All receivers must implement this interface
//
// Copyright (c) 2023 Arvind Devarajan
// Licensed to you under the MIT License.
// See the LICENSE file in the project root for more information.

namespace FSharp.Example.Services.Internal

open System.Threading
open System.Threading.Tasks

type IReceiverService =
  abstract member ReceiveAsync: CancellationToken -> Async<unit>
