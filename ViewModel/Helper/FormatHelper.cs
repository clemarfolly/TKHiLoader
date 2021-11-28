using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using TKHiLoader.Format;

namespace TKHiLoader.Helper
{
    public static class FormatHelper
    {
        private static IList<FormatBase> _formats;
        public static IList<FormatBase> Formats
        {
            get
            {
                if (_formats == null)
                {
                    _formats = new List<FormatBase>();
                    var formatTypes = GetFormats();

                    foreach (var type in formatTypes)
                    {
                        _formats.Add((FormatBase)Activator.CreateInstance(type));
                    }
                }

                return _formats;
            }
        }

        private static IEnumerable<Type> GetFormats()
        {
            Type parentType = typeof(FormatBase);
            Assembly assembly = Assembly.GetExecutingAssembly();
            Type[] types = assembly.GetTypes();

            return types.Where(t => t.IsSubclassOf(parentType));
        }
    }
}
