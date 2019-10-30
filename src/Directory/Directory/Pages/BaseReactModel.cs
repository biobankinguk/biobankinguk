using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Directory.Pages
{
    /// <summary>
    /// <para>Used to ensure PageModel properties are not returned to the View as JSON.</para>
    /// 
    /// <para>
    /// Passwords (DataType.Password) are excluded by default, but for anything else
    /// this decorator can be used.
    /// </para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class NoJsonViewModelAttribute : Attribute { }

    public abstract class BaseReactModel : PageModel
    {
        protected BaseReactModel(string route) => Route = route;

        public string Route { get; set; }

        // Memoize this in a backing field
        // because Reflection is expensive
        private string? _jsonViewModel;
        public string JsonViewModel
        {
            get
            {
                if (_jsonViewModel is null)
                {
                    // Get the actual properties on the actual derived PageModel
                    var model = GetType()
                        .GetProperties(BindingFlags.DeclaredOnly
                            | BindingFlags.Public
                            | BindingFlags.Instance)
                        .Aggregate(
                            new Dictionary<string, object?>(),
                            (a, x) =>
                            {
                                if (ExcludePropFromJson(x)) return a;

                                a[x.Name] = x.GetValue(this);
                                return a;
                            });

                    // Add ModelState Errors separately
                    // ModelState is a safe key, because derived classes can't have a ModelState property
                    // due to the base class already declaring it :)
                    model["ModelState"] = ModelState.Keys.Aggregate(new Dictionary<string, IEnumerable<string>>(),
                        (a, k) => { a[k] = ModelState[k].Errors.Select(x => x.ErrorMessage); return a; });

                    // Serialize to JSON
                    _jsonViewModel = JsonSerializer.Serialize(model);
                }

                return _jsonViewModel;
            }
        }

        private bool ExcludePropFromJson(PropertyInfo p)
            => p.GetCustomAttributes().Any(at =>
                {
                    // we check types by null casting...

                    // Our own exclusion attribute
                    NoJsonViewModelAttribute? exclude = at as NoJsonViewModelAttribute;
                    if (exclude is { }) return true;

                    // Passwords
                    DataTypeAttribute? dt = at as DataTypeAttribute;
                    if (dt is { } && dt.DataType == DataType.Password)
                    {
                        return true;
                    }

                    return false; // no attribute conditions met
                });

    }
}
