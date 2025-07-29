﻿/*
 * Copyright (c) 2014-2018, Firely <info@fire.ly>
 * Copyright (c) 2021-2025, Incendi <info@incendi.no>
 *
 * SPDX-License-Identifier: BSD-3-Clause
 */

using System.Threading.Tasks;

namespace Spark.Engine.Interfaces;

public interface IFhirStoreAdministration
{
    Task CleanAsync();
}
