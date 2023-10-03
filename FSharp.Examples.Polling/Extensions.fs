// Extension functions needed to extract bot configuration
//
// Copyright (c) 2023 Arvind Devarajan
// Licensed to you under the MIT License.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Extensions.DependencyInjection

open System
open Microsoft.Extensions.Options
open System.Runtime.CompilerServices

[<Extension>]
type PollingExtensions() =
  [<Extension>]
  static member GetConfiguration<'T when 'T: not struct>(sp: IServiceProvider) =
    let o = sp.GetService<IOptions<'T>>()

    if isNull o then
      raise <| ArgumentNullException nameof<'T>

    o.Value
