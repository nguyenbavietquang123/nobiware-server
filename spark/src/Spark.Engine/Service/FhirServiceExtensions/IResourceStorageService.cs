﻿/* 
 * Copyright (c) 2016-2018, Firely <info@fire.ly>
 * Copyright (c) 2021-2025, Incendi <info@incendi.no>
 * 
 * SPDX-License-Identifier: BSD-3-Clause
 */

using System.Collections.Generic;
using System.Threading.Tasks;
using Spark.Engine.Core;

namespace Spark.Engine.Service.FhirServiceExtensions;

public interface IResourceStorageService : IFhirServiceExtension
{
    Task<Entry> GetAsync(IKey key);
    Task<Entry> AddAsync(Entry entry);
    Task<IList<Entry>> GetAsync(IEnumerable<string> localIdentifiers, string sortby = null);
}
