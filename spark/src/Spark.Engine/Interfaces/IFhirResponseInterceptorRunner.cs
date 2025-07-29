/* 
 * Copyright (c) 2015-2018, Firely <info@fire.ly>
 * 
 * SPDX-License-Identifier: BSD-3-Clause
 */

using System.Collections.Generic;
using Spark.Engine.Core;

namespace Spark.Engine.Interfaces;

public interface IFhirResponseInterceptorRunner
{
    void AddInterceptor(IFhirResponseInterceptor interceptor);
    void ClearInterceptors();
    FhirResponse RunInterceptors(Entry entry, IEnumerable<object> parameters);
}
