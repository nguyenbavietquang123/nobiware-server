/* 
 * Copyright (c) 2016-2018, Firely <info@fire.ly>
 * 
 * SPDX-License-Identifier: BSD-3-Clause
 */

using Spark.Engine.Core;

namespace Spark.Engine.Service.FhirServiceExtensions;

public interface ISnapshotPaginationProvider
{
    ISnapshotPagination StartPagination(Snapshot snapshot);
}