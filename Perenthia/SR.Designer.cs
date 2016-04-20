﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.296
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Perenthia {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class SR {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal SR() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Perenthia.SR", typeof(SR).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Health: {0}/{1}.
        /// </summary>
        internal static string BodyValueMaxFormat {
            get {
                return ResourceManager.GetString("BodyValueMaxFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Perenthia requires that the port {0} be open. If you are behind a firewall you may need to open the port or contact your network adminstrator to open the port..
        /// </summary>
        internal static string ConnectionFailed {
            get {
                return ResourceManager.GetString("ConnectionFailed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Perenthia is testing to see whether or not you can connect via port {0}. If you have control over opening ports you can open port {0} and refresh this page. Otherwise you will be connected via a slower HTTP connection..
        /// </summary>
        internal static string ConnectionTestFormat {
            get {
                return ResourceManager.GetString("ConnectionTestFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Willpower: {0}/{1}.
        /// </summary>
        internal static string MindValueMaxFormat {
            get {
                return ResourceManager.GetString("MindValueMaxFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} ({1}).
        /// </summary>
        internal static string NameLevelFormat {
            get {
                return ResourceManager.GetString("NameLevelFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to health:{0}/{1}, willpower:{2}/{3}, experience:{4} &gt;.
        /// </summary>
        internal static string PromptFormat {
            get {
                return ResourceManager.GetString("PromptFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to www.perenthia.com : version {0}.
        /// </summary>
        internal static string VersionFormat {
            get {
                return ResourceManager.GetString("VersionFormat", resourceCulture);
            }
        }
    }
}
