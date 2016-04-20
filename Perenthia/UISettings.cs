using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Radiance.Serialization;
using System.Collections.Generic;
using Perenthia.Windows;
using System.Xml.Serialization;
using System.Xml.Linq;

namespace Perenthia
{
	public class UISettings : IXmlSerializable
	{
		public const string PlayerWindowID = "Player";
		public const string TargetWindowID = "Target";
		public const string ChatWindowID = "Chat";
		public const string GeneralWindowID = "General";
		public const string MapWindowID = "Map";
		public const string CharacterSheetWindowID = "CharacterSheet";
		public const string GodModeWindowID = "GodMode";

		private Dictionary<string, WindowInfoSetting> _windows = new Dictionary<string, WindowInfoSetting>(StringComparer.InvariantCultureIgnoreCase);

		public void SetWindowValues(IWindow window)
		{
			if (window == null)
				return;

			WindowInfoSetting setting;
			if (_windows.TryGetValue(window.WindowID, out setting))
			{
				window.Position = setting.Position;
				window.Size = setting.Size;
			}
		}

		public void AddWindow(IWindow window)
		{
			if (window == null)
				return;

			WindowInfoSetting setting;
			if (!_windows.TryGetValue(window.WindowID, out setting))
				setting = new WindowInfoSetting();

			setting.Position = window.Position;
			setting.Size = window.Size;

			lock (_windows)
			{
				_windows[window.WindowID] = setting;
			}
		}

		#region IXmlSerializable Members

		public System.Xml.Schema.XmlSchema GetSchema()
		{
			return null;
		}

		public void ReadXml(System.Xml.XmlReader reader)
		{
			var settingsElement = XElement.Load(reader);
			if (settingsElement != null)
			{
				var windowsElement = settingsElement.Element("windows");
				if (windowsElement != null)
				{
					_windows.Clear();
					foreach (var windowElement in windowsElement.Elements())
					{
						_windows.Add(windowElement.GetStringValue("id"), 
							new WindowInfoSetting
							{
								Position = new Point(windowElement.GetDoubleValue("x"), windowElement.GetDoubleValue("y")),
								Size = new Size(windowElement.GetDoubleValue("width"), windowElement.GetDoubleValue("height"))
							});
					}
				}
			}
		}

		public void WriteXml(System.Xml.XmlWriter writer)
		{
			var settingsElement = new XElement("settings");

			var windowsElement = new XElement("windows");
			settingsElement.Add(windowsElement);

			lock (_windows)
			{
				foreach (var kvp in _windows)
				{
					var windowElement = new XElement("window");
					windowsElement.Add(windowElement);

					windowElement.Add(new XAttribute("id", kvp.Key));
					windowElement.Add(new XAttribute("x", kvp.Value.Position.X));
					windowElement.Add(new XAttribute("y", kvp.Value.Position.Y));
					windowElement.Add(new XAttribute("width", kvp.Value.Size.Width));
					windowElement.Add(new XAttribute("height", kvp.Value.Size.Height));
				}
			}

			settingsElement.WriteTo(writer);
		}

		#endregion

		private class WindowInfoSetting
		{
			public Point Position { get; set; }
			public Size Size { get; set; }
		}
	}
}
