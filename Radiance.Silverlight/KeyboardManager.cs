using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Reflection;
using Lionsguard;

namespace Radiance
{
	public class KeyboardManager
	{
		private static Dictionary<Key, bool> _keys = new Dictionary<Key, bool>();

		static KeyboardManager()
		{
			try
			{
				FieldInfo[] fields = typeof(Key).GetFields(BindingFlags.Static | BindingFlags.Public);
				if (fields != null)
				{
					foreach (var field in fields)
					{
						_keys.Add((Key)field.GetValue(null), false);
					}
				}
			}
			catch (Exception ex)
			{
				Logger.LogDebug(ex.ToString());
			}
		}

		public static Key[] GetPressedKeys()
		{
			return (from e in _keys where e.Value == true select e.Key).ToArray();
		}

		public static void ResetPressedKeys()
		{
			Key[] keys = GetPressedKeys();
			if (keys != null && keys.Length > 0)
			{
				for (int i = 0; i < keys.Length; i++)
				{
					Set(keys[i], false);
				}
			}
		}

		public static bool IsKeyDown(Key key)
		{
			return _keys[key];
		}

		public static bool IsKeyUp(Key key)
		{
			return (!_keys[key]);
		}

		public static void Set(Key key, bool value)
		{
			_keys[key] = value;
		}
	}
}
