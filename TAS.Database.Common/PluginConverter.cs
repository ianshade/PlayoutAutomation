﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using TAS.Database.Common.Interfaces;

namespace TAS.Database.Common
{
    public class PluginConverter : JsonConverter
    {
        public System.Collections.Generic.IEnumerable<IPluginTypeBinder> PluginBinders { get; }

        private static readonly string FileNameSearchPattern = "TAS.Server.*.dll";

        private PluginConverter()
        {
            var pluginPath = Path.Combine(Directory.GetCurrentDirectory(), "Plugins");

            using (var catalog = new DirectoryCatalog(pluginPath, FileNameSearchPattern))
            using (var container = new CompositionContainer(catalog))
            {
                PluginBinders = container.GetExportedValues<IPluginTypeBinder>();
            }
        }

        public static PluginConverter Current { get; } = new PluginConverter();


        private object CreateInstance(JToken container, JsonSerializer serializer)
        {
            try
            {
                var jObject = JObject.Load(container.CreateReader());
                var typeMeta = jObject.GetValue("$type").ToObject<string>().Split(',');

                Type type = null;
                foreach (var binder in PluginBinders)
                    if ((type = binder.BindToType(typeMeta[1], typeMeta[0])) != null)
                        break;

                var isEnabled = jObject.GetValue("IsEnabled").ToObject<bool>();

                if (isEnabled)
                    return serializer.Deserialize(jObject.CreateReader(), type);

                return null;
            }
            catch
            {
                return null;
            }                        
        }
        

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(value);
        }
        
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.StartArray)
            {
                var jArray = JArray.Load(reader);
                var plugins = (IList)Activator.CreateInstance(objectType);
                foreach (var child in jArray.Children())
                {
                    var item = CreateInstance(child, serializer);
                    if (item != null)
                        plugins.Add(item);
                }
                return plugins;
            }
            else
            {
                return CreateInstance(JObject.Load(reader), serializer);
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return true;
        }
    }
}
