using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.ComponentModel;

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
            mSettings.Converters.Add(new ISerializableConverter());
            mSettings.Converters.Add(new TileInstanceConverter());
        }

        public static JsonSerializer CreateSerializer()
        {
            var s = new JsonSerializer();

            foreach (var c in mSettings.Converters)
                s.Converters.Add(c);

            return s;
        }

        public static string ToJSON<T>(T item)
        {
            var serializable = item as ISerializable;
            if (serializable != null)
                return JsonConvert.SerializeObject(serializable.GetSaveModel(),mSettings);
            else
                return JsonConvert.SerializeObject(item, mSettings);
        }

        public static T FromJson<T>(string json) where T:  new()
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

        public static object FromJson(string json, Type type)
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

            var s = Serializer.CreateSerializer();
            var saveModel = s.Deserialize(reader, item.GetSaveModelType());

            var itemBase = item as ISerializableBaseClass;
            if (itemBase != null)
            {
                try
                {
                    item = Activator.CreateInstance(itemBase.GetTargetType()) as ISerializable;
                }
                catch (NotImplementedException ex) { }
            }

            item.Load(saveModel);

            return item;

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
            serializer.Serialize(writer, value);
        }
    }

}
