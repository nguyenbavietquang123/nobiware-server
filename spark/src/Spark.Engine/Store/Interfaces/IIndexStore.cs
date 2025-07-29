/*
 * Copyright (c) 2016-2018, Firely <info@fire.ly>
 * Copyright (c) 2021-2025, Incendi <info@incendi.no>
 *
 * SPDX-License-Identifier: BSD-3-Clause
 */

using System.Threading.Tasks;
using Spark.Engine.Core;
using Spark.Engine.Model;

namespace Spark.Engine.Store.Interfaces;

public interface IIndexStore
{
    Task SaveAsync(IndexValue indexValue);
    Task DeleteAsync(Entry entry);
    Task CleanAsync();
}
