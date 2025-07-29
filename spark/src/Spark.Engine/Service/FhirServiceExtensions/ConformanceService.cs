﻿/* 
 * Copyright (c) 2016-2018, Firely <info@fire.ly>
 * Copyright (c) 2018-2025, Incendi <info@incendi.no>
 * 
 * SPDX-License-Identifier: BSD-3-Clause
 */

using Hl7.Fhir.Model;
using Spark.Engine.Core;

namespace Spark.Engine.Service.FhirServiceExtensions;

public class CapabilityStatementService : ICapabilityStatementService
{
    private readonly ILocalhost _localhost;

    public CapabilityStatementService(ILocalhost localhost)
    {
        _localhost = localhost;
    }

    public CapabilityStatement GetSparkCapabilityStatement(string sparkVersion)
    {
        return CapabilityStatementBuilder.GetSparkCapabilityStatement(sparkVersion, _localhost);
    }
}
