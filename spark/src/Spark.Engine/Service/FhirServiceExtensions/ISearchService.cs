/* 
 * Copyright (c) 2016-2018, Firely <info@fire.ly>
 * Copyright (c) 2021-2025, Incendi <info@incendi.no>
 * 
 * SPDX-License-Identifier: BSD-3-Clause
 */

using System.Threading.Tasks;
using Hl7.Fhir.Rest;
using Spark.Engine.Core;

namespace Spark.Engine.Service.FhirServiceExtensions;

public interface ISearchService : IFhirServiceExtension
{
    Task<Snapshot> GetSnapshotAsync(string type, SearchParams searchCommand);
    Task<Snapshot> GetSnapshotForEverythingAsync(IKey key);
    Task<IKey> FindSingleAsync(string type, SearchParams searchCommand);
    Task<IKey> FindSingleOrDefaultAsync(string type, SearchParams searchCommand);
    Task<SearchResults> GetSearchResultsAsync(string type, SearchParams searchCommand);
}
