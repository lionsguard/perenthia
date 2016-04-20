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

using Perenthia.Screens;

namespace Perenthia.Household
{
	public interface IHouseholdScreen
	{
		IHouseholdScreenManager Manager { get; set; }
		void Show();
	}

	public interface IHouseholdScreenManager
	{
		HouseholdScreenState State { get; set; }
		void Refresh();
	}

	public enum HouseholdScreenState
	{
		SearchOrCreate,
		Create,
		Search,
		Join,
		JoinRequest,
		Details,
	}
}
