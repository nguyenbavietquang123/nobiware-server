/* 
 * Copyright (c) 2016-2018, Firely <info@fire.ly>
 * Copyright (c) 2021-2025, Incendi <info@incendi.no>
 * 
 * SPDX-License-Identifier: BSD-3-Clause
 */

using System.Threading.Tasks;
using Spark.Engine.Core;

namespace Spark.Engine.Store.Interfaces;

public interface ISnapshotStore
{
    Task AddSnapshotAsync(Snapshot snapshot);
    Task<Snapshot> GetSnapshotAsync(string snapshotId);
}
