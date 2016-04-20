using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Radiance.Contract
{
	[DataContract]
	public class MapChunk
	{
		[DataMember]
		public string MapName { get; set; }
		[DataMember]
		public int StartX { get; set; }
		[DataMember]
		public int StartY { get; set; }
		[DataMember]
		public bool IncludeActors { get; set; }
		[DataMember]
		public string Tags { get; set; }	
	}
}
