using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityModManagerNet;

namespace SteamSimulationTweaks
{
	[Serializable]
	public class Settings : UnityModManager.ModSettings, IDrawable
	{

		[Header("S060")]

		[Draw("Max boiler pressure (default 14.5, reload save to apply):")]
		public float s060MaxBoilerPressure = 14.5f;

		[Draw("Enable volumetric efficiency changes to S060 (reload save to apply):",
			Tooltip = "Gives the S060 the top speed of the S282")]
		public bool enableS060VolumetricEfficiencyChanges = true;

		[Draw("Max throttle flow rate, kg/s (default 3, reload save to apply):",
			DrawType.Slider, Min = 1, Max = 5, Precision = 1)]
		public float s060MaxThrottleFlow = 3;

		[Header("S282")]

		[Draw("Max boiler pressure (default 14, reload save to apply):")]
		public float s282MaxBoilerPressure = 14f;

		[Draw("Max throttle flow rate, kg/s (default 16, reload save to apply):",
			DrawType.Slider, Min = 1, Max = 20, Precision = 1)]
		public float s282MaxThrottleFlow = 16;

		[Draw("Enable feedwater heater changes (reload save to apply):",
			Tooltip = "Feedwater heater will be less efficient when there is no exhaust pressure")]
		public bool enableFeedwaterHeaterChanges = true;

		[Draw("Feedwater temp heat up time, seconds (default 3):",
			DrawType.Slider, Min = 1, Max = 10, Precision = 0,
			Tooltip = "Time it takes for the feedwater temperature to heat up when steam enters the exhaust")]
		public float feedwaterTempSmoothTimeUp = 3;

		[Draw("Feedwater temp cool down time, seconds (default 3):",
			DrawType.Slider, Min = 1, Max = 10, Precision = 0,
			Tooltip = "Time it takes for the feedwater temperature to cool down after the exhaust empties")]
		public float feedwaterTempSmoothTimeDown = 3;

		public override void Save(UnityModManager.ModEntry modEntry)
		{
			Save(this, modEntry);
		}

		public void OnChange()
		{
			
		}
	}
}
