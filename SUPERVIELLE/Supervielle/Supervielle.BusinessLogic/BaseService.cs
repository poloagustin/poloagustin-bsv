using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Supervielle.BusinessLogic
{
    public abstract class BaseService
    {
        protected static IEnumerable<FilterExpression> GetFilters<T>(IEnumerable<T> collection, Func<T, FilterExpression> predicate)
        {
            var filters = new List<FilterExpression>();

            foreach (var element in collection)
            {
                filters.Add(predicate(element));
            }

            return filters;
        }

    }
}
