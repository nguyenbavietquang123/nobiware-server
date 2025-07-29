﻿/*
 * Copyright (c) 2016-2018, Firely <info@fire.ly>
 * Copyright (c) 2021-2025, Incendi <info@incendi.no>
 *
 * SPDX-License-Identifier: BSD-3-Clause
 */

namespace Spark.Engine.Extensions;

public static class StringExtensions
{
    public static string FirstUpper(this string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return input;

        return string.Concat(input.Substring(0, 1).ToUpperInvariant(), input.Remove(0, 1));
    }
}
