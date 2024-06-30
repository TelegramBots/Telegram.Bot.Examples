// Utility functions
//
// Copyright (c) 2023 Arvind Devarajan
// Licensed to you under the MIT License.
// See the LICENSE file in the project root for more information.

namespace FSharp.Example

open System.Threading
open System.Threading.Tasks
open Microsoft.Extensions.Logging

module Util =
  // Placeholder for unimplemented functions
  let undefined<'T> : 'T = failwith "Not implemented yet"

  // Log information using the passed-in logger
  let logInfo (logger: ILogger) (msg: string) = logger.LogInformation msg
