using System.Collections.Generic;
using System.Dynamic;
using IronBlock;
using IronBlock.Blocks;
using Newtonsoft.Json.Linq;

namespace PurchaseForMeService.Blocks.ObjectBlocks
{
    [RegisterBlock("object_jsonToObject")]
    public class JsonToObjectBlock : IBlock
    {
        public override object Evaluate(Context context)
        {
            string jsonText = Values.Evaluate("jsonText", context).ToString();
            ExpandoObject baseObj = new ExpandoObject();
            convertJObjectToObject(baseObj, JObject.Parse(jsonText));
            return baseObj;
        }

        private void convertJObjectToObject(IDictionary<string, object> baseObj, JToken currentToken, string propertyName = "", bool inArray = false, List<object> array = null)
        {
            foreach (var childToken in currentToken.Children())
            {
                if (childToken.Type == JTokenType.Property)
                {
                    JProperty variableProperty = childToken.ToObject<JProperty>();
                    convertJObjectToObject(baseObj, variableProperty, variableProperty.Name);
                }
                else if (childToken.Type == JTokenType.Object)
                {
                    JObject variableObject = childToken.ToObject<JObject>();
                    ExpandoObject newObj = new ExpandoObject();
                    if (inArray)
                    {
                        array?.Add(newObj);
                    }
                    else
                    {
                        baseObj.Add(propertyName, newObj);
                    }
                    convertJObjectToObject(newObj, variableObject);
                }
                else if (childToken.Type == JTokenType.Array)
                {
                    JArray jArray = childToken.ToObject<JArray>();
                    List<object> newArray = new List<object>(jArray.Count);
                    baseObj.Add(propertyName, newArray);
                    convertJObjectToObject(baseObj, jArray, inArray: true, array: newArray);
                }
                else
                {
                    if (inArray)
                    {
                        array?.Add(childToken.ToObject<object>());
                    }
                    else
                    {
                        baseObj.Add(propertyName, childToken.ToObject<object>());
                    }
                }
            }
        }
    }
}