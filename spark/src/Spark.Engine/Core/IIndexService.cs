﻿/* 
 * Copyright (c) 2019-2025, Incendi <info@incendi.no>
 * 
 * SPDX-License-Identifier: BSD-3-Clause
 */

using System.Threading.Tasks;
using Hl7.Fhir.Model;
using Spark.Engine.Model;
using Task = System.Threading.Tasks.Task;

namespace Spark.Engine.Core;

public interface IIndexService
{
    Task ProcessAsync(Entry entry);
    Task<IndexValue> IndexResourceAsync(Resource resource, IKey key);
}
