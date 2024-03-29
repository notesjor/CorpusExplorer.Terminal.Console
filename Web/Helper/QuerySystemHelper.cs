﻿using CorpusExplorer.Sdk.Utils.Filter.Abstract;
using CorpusExplorer.Sdk.Utils.Filter.Queries;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Tfres;

namespace CorpusExplorer.Terminal.Universal
{
  public static class QuerySystemHelper
  {
    private static AbstractFilterQuery[] _queries = new AbstractFilterQuery[]
    {
      new FilterQueryDocumentGuid(),
      new FilterQueryMetaContains(),
      new FilterQueryMetaContainsCaseSensitive(),
      new FilterQueryMetaEndsWith(),
      new FilterQueryMetaExactMatch(),
      new FilterQueryMetaExactMatchCaseSensitive(),
      new FilterQueryMetaIsEmpty(),
      new FilterQueryMetaRegex(),
      new FilterQueryMetaStartsWith(),
      new FilterQueryMultiLayerAll(),
      new FilterQueryMultiLayerAny(),
      new FilterQueryMultiLayerComplex(),
      new FilterQueryMultiLayerPhrase(),
      new FilterQuerySingleLayerAFollowedByBMatch(),
      new FilterQuerySingleLayerAFollowedByBSpanMatch(),
      new FilterQuerySingleLayerAllInExactSpanWords(),
      new FilterQuerySingleLayerAllInOneDocument(),
      new FilterQuerySingleLayerAllInOneSentence(),
      new FilterQuerySingleLayerAllInSpanSentences(),
      new FilterQuerySingleLayerAllInSpanWords(),
      new FilterQuerySingleLayerAlternativePhrase(),
      new FilterQuerySingleLayerAnyMatch(),
      new FilterQuerySingleLayerExactPhrase(),
      new FilterQuerySingleLayerFirstAndAnyOtherMatch(),
      new FilterQuerySingleLayerFirstFollowedByAnyOtherMatch(),
      new FilterQuerySingleLayerMarkedPhrase(),
      new FilterQuerySingleLayerRanked(),
      new FilterQuerySingleLayerRegex(),
      new FilterQuerySingleLayerRegexFulltext(),
    };
    private static string _operators = null;

    public static void GetOperators(HttpContext obj)
    {
      if (_operators == null)
      {
        _operators = JsonConvert.SerializeObject(_queries, new JsonSerializerSettings
        {
          TypeNameHandling = TypeNameHandling.Objects,
          ContractResolver = new IgnorePropertiesResolver()
        });
      }

      obj.Response.Send(_operators);
    }

    public static AbstractFilterQuery ConvertToQuery(string postDataAsString)
    {
      return JsonConvert.DeserializeObject<AbstractFilterQuery>(postDataAsString, new JsonSerializerSettings
      {
        TypeNameHandling = TypeNameHandling.Objects
      });
    }

    private class IgnorePropertiesResolver : DefaultContractResolver
    {
      private static HashSet<string> _ignore = new HashSet<string> { "Guid", "OrFilterQueries", "Verbal" };

      protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
      {
        JsonProperty property = base.CreateProperty(member, memberSerialization);
        if (_ignore.Contains(property.PropertyName))
          property.ShouldSerialize = instance => false;

        return property;
      }
    }
  }
}
