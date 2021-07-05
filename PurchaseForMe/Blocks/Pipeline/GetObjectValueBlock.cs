using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using IronBlock;
using IronBlock.Blocks;
using Newtonsoft.Json.Linq;
using PurchaseForMe.Core.WebPipeline;

namespace PurchaseForMe.Blocks.Pipeline
{
    [RegisterBlock("pipeline_getObjectValue")]
    public class GetObjectValueBlock : IBlock
    {
        public override object Evaluate(Context context)
        {
            //Person.age [Person, age]
            string[] graph = Values.Evaluate("propertyName", context).ToString().Split(".");
            dynamic obj = Values.Evaluate("object", context);
            IDictionary<string, Object> objDictionary;
            if (obj is WebDataModel)
            {
                WebDataModel dataModel = obj as WebDataModel;
                objDictionary = new Dictionary<string, object>();
                objDictionary.Add(nameof(dataModel.ModelData), dataModel.ModelData);
            }
            else
            {
                objDictionary = (IDictionary<string, Object>)obj;
            }
            
            return traverseObjectGraph(objDictionary, 0, graph);
        }

        private object traverseObjectGraph<TValue>(IDictionary<string, TValue> currentObj, int position, string[] graph)
        {
            //We have reached the end of the graph.
            if (position == graph.Length - 1)
            {
                if (currentObj.ContainsKey(graph[position]))
                {
                    var obj = currentObj[graph[position]];
                    return obj;
                }
                else throw new ArgumentException($"Unable to find property, no property found for {graph[position]}");
            }
                

            if (currentObj.ContainsKey(graph[position]))
            {
                var nextObj = currentObj[graph[position]];
                var newObj = (IDictionary<string, TValue>)nextObj;
                return traverseObjectGraph(newObj, ++position, graph);
            }
            else throw new ArgumentException($"Unable to find property, no property found for {graph[position]}");
        }
    }
}