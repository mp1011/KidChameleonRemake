using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Engine
{
    public static class ReflectionHelper
    {

        public static Assembly MainAssembly
        {
            get { return Assembly.GetExecutingAssembly(); }
        }

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
       
    }


}
