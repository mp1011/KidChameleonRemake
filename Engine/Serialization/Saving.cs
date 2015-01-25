using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Engine
{
    public interface ISerializable
    {
        object GetSaveModel();
        Type GetSaveModelType();
        void Load(object saveModel);
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

            var plainSerializer = new JsonSerializer();
            var saveModel = plainSerializer.Deserialize(reader, item.GetSaveModelType());
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
