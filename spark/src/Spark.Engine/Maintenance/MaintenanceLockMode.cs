﻿/*
 * Copyright (c) 2020-2025, Incendi <info@incendi.no>
 *
 * SPDX-License-Identifier: BSD-3-Clause
 */

namespace Spark.Engine.Maintenance;

internal enum MaintenanceLockMode
{
    None = 0,
    Write = 1,
    Full = 2
}
