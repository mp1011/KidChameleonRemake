using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Engine
{
    public static class ReflectionHelper
    {

        public static Assembly EngineAssembly { get; set; }
        public static Assembly GameAssembly { get; set; }

        public static Assembly GetAssembly(object o)
        {
            var type = o.GetType();
            return type.Assembly;
        }

        public static  IEnumerable<Type> GetTypesByAttribute<TAttr>(this Assembly assembly) where TAttr : Attribute
        {
            return assembly.GetTypes().Where(p => p.GetCustomAttributes(typeof(TAttr), false).Any());
        }

        public static IEnumerable<PropertyInfo> GetPropertiesByAttribute<TAttr>(this Type type) where TAttr : Attribute
        {
            return type.GetProperties().Where(p => p.GetCustomAttributes(typeof(TAttr), false).Any());
        }

        /// <summary>
        /// Creates an instance of the class decorated by the given attribute
        /// </summary>
        /// <typeparam name="TReturn"></typeparam>
        /// <typeparam name="TAttr"></typeparam>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static TReturn CreateObjectByAttribute<TReturn, TAttr>(object parent, Func<TAttr, bool> condition, params object[] args) where TAttr : Attribute
        {
            var assembly = GetAssembly(parent);
            foreach (var type in GetTypesByAttribute<TAttr>(assembly))
            {
                foreach (var attr in type.GetCustomAttributes(false).OfType<TAttr>())
                {
                    if (condition(attr))
                    {
                        var obj = Activator.CreateInstance(type,args);
                        return (TReturn)obj;
                    }
                }
            }

            throw new Exception("Unable to create an object");
        }
    }


}
