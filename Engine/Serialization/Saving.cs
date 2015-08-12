using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.ComponentModel;
using Newtonsoft.Json.Serialization;

namespace Engine
{
    public interface ISerializable
    {
        object GetSaveModel();
        Type GetSaveModelType();
        void Load(object saveModel);
    }

    public interface ISerializableBaseClass : ISerializable 
    {
        Type GetTargetType();
    }

    public static class Serializer
    {
        private static JsonSerializerSettings mSettings;
        static Serializer()
        {
            mSettings = new JsonSerializerSettings();
            mSettings.MissingMemberHandling = MissingMemberHandling.Ignore;
            
            mSettings.Converters.Add(new ISerializableConverter());
            mSettings.Converters.Add(new TileInstanceConverter());
            mSettings.Error = HandleError;
        }

        private static void HandleError(object sender, ErrorEventArgs e)
        {
            e.ErrorContext.Handled = true;
        }

        public static JsonSerializer CreateSerializer()
        {
            var s = new JsonSerializer();

            foreach (var c in mSettings.Converters)
                s.Converters.Add(c);

            s.Error += HandleError;
            return s;
        }

        public static T Copy<T>(T item) where T : new()
        {
            var json = ToJSON(item);
            return FromJson<T>(json);
        }

        public static string ToJSON<T>(T item)
        {
            try
            {
                var serializable = item as ISerializable;
                if (serializable != null)
                    return JsonConvert.SerializeObject(serializable.GetSaveModel(), mSettings);
                else
                    return JsonConvert.SerializeObject(item, mSettings);
            }
            catch (Exception ex)
            {
                throw ex.Flatten();
            }
        }

        public static T FromJson<T>(string json) where T:  new()
        {
            try
            {
                var item = new T();

                var serializable = item as ISerializable;
                if (serializable != null)
                {
                    var saveModel = JsonConvert.DeserializeObject(json, serializable.GetSaveModelType(),mSettings);
                    serializable.Load(saveModel);
                    return (T)serializable;
                }

                return JsonConvert.DeserializeObject<T>(json, mSettings);
            }
            catch (Exception ex)
            {
                throw ex.Flatten();
            }
        }

        public static object FromJson(string json, Type type)
        {
            try
            {
                var item = Activator.CreateInstance(type);

                var serializable = item as ISerializable;
                if (serializable != null)
                {
                    var saveModel = JsonConvert.DeserializeObject(json, serializable.GetSaveModelType(), mSettings);
                    serializable.Load(saveModel);
                    return serializable;
                }

                return JsonConvert.DeserializeObject(json, type, mSettings);
            }
            catch (Exception ex)
            {
                throw ex.Flatten();
            }
        }


    }

    class ISerializableConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(ISerializable).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {

            ISerializable item;

            try
            {
                item = existingValue as ISerializable;
                if (item == null)
                    item = Activator.CreateInstance(objectType) as ISerializable;

                if (item == null)
                    throw new ArgumentNullException();
            }
            catch (Exception ex)
            {
                throw new Exception("Deserialization failed for type: " + objectType.Name, ex);
            }

            try
            {
                var s = Serializer.CreateSerializer();
               
                var itemBase = item as ISerializableBaseClass;
                if (itemBase != null)
                {
                    try
                    {
                        item = Activator.CreateInstance(itemBase.GetTargetType()) as ISerializable;
                    }
                    catch (NotImplementedException ex) { }
                }

                var saveModel = s.Deserialize(reader, item.GetSaveModelType());
                item.Load(saveModel);

                return item;
            }
            catch (Exception e)
            {
                throw e.Flatten();
            }

        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {            
            ISerializable item = value as ISerializable;
            serializer.Serialize(writer, item.GetSaveModel());           
        }
    }


    class TileInstanceConverter : JsonConverter
    {

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(TileInstance);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var special = Core.GameBase.Current.TileInstanceCreate();
            return serializer.Deserialize(reader, special.GetType());
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            try
            {
                serializer.Serialize(writer, value);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
