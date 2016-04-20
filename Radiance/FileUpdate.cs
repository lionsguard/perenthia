using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;
using System.Text;

namespace Radiance
{
#if !SILVERLIGHT
    [DataContract]
#endif
    public class FileUpdate
    {
        #if !SILVERLIGHT
        [DataMember]
#endif
        public string FileName { get; set; }
        #if !SILVERLIGHT
        [DataMember]
#endif
        public DateTime LastUpdateDate { get; set; }    
    }
}
