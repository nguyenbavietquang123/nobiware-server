﻿/* 
 * Copyright (c) 2014-2018, Firely <info@fire.ly>
 * Copyright (c) 2021-2025, Incendi <info@incendi.no>
 * 
 * SPDX-License-Identifier: BSD-3-Clause
 */

using Hl7.Fhir.Model;

namespace Spark.Engine.Interfaces;

public interface IIdentityGenerator
{
    string NextResourceId(Resource resource);
    string NextVersionId(string resourceIdentifier);
    string NextVersionId(string resourceType, string resourceIdentifier);
}
