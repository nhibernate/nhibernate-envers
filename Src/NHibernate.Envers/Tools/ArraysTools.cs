using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHibernate.Envers.Tools
{
    class ArraysTools
    {
    public static bool ArrayIncludesInstanceOf<T>(T[] array, System.Type cls) {
        foreach (T obj in array) {
            if (cls.IsAssignableFrom(obj.GetType())) {
                return true;
            }
        }
        return false;
    }
    }
}
