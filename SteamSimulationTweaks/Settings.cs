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

		[Header("All steam locomotives")]

		/*[Draw("Sander requires air pressure (default true):",
			Tooltip = "When true, the sander will work very slowly unless there is air pressure in the air reservoir")]
		public bool airPoweredSander = true;*/

		//[Draw("Enable more accurate steam math for high pressures (default false, reload game to apply):",
			//Tooltip = "More accurate for boiler pressures between 30 and 100 bar")]
		public bool enableHighPressureSteamCalcs = false;

		//[Draw("Allow a closed throttle to create a vacuum in the cylinders (default true, reload game to apply):",
		//	Tooltip = "You will need to crack the throttle open or open the cylinder cocks to get rid of the vacuum")]
		public bool enableClosedThrottleCylinderVacuum = false;

		[Draw("Enable leaky lubricator (default true, reload save to apply):",
			Tooltip = "In vanilla DV, the lubricator runs off the valve gear, but also leaks a little. This gets rid of the leaking.")]
		public bool enableLeakyLubricator = true;

		[Draw("Enable embers (default true, reload save to apply):",
			Tooltip = "If unchecked, steam engines won't burn down the countryside")]
		public bool enableEmbers = true;

		[Header("S060")]

		[Draw("Max boiler pressure (default 14.5, simulation not accurate above 30, reload save to apply):")]
		public float s060MaxBoilerPressure = 14.5f;

		[Draw("Enable volumetric efficiency changes to S060 (reload save to apply):",
			Tooltip = "Gives the S060 the top speed of the S282")]
		public bool enableS060VolumetricEfficiencyChanges = true;

		[Draw("Max throttle flow rate, kg/s (default 3, reload save to apply):",
			DrawType.Slider, Min = 1, Max = 5, Precision = 1)]
		public float s060MaxThrottleFlow = 3;

		[Header("S282")]

		[Draw("Max boiler pressure (default 14, simulation not accurate above 30, reload save to apply):")]
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

		[Draw("Superheater effectiveness (default 0.3 for realism, 1.0 for vanilla gameplay):")]
		public float superheaterEffectiveness = 0.3f;

		[Header("Debug")]

		[Draw("S282 throttle lever scrollWheelHoverScroll (default 1.5, re-enter S282 to apply):",
			Tooltip = "IDK how, but this alters how far each click of scroll wheel changes the throttle")]
		public float throttleLeverScrollWheelHoverScroll = 1.5f;

		[Draw("S282 throttle lever scrollWheelSpring (default 0, re-enter S282 to apply):")]
		public float throttleLeverScrollWheelSpring = 0f;

		[Draw("S282 throttle lever jointSpring (default 150, re-enter S282 to apply):",
			Tooltip = "How quickly the throttle moves between notches. Higher is faster, too high causes glitches")]
		public float throttleLeverJointSpring = 150f;

		[Draw("Log extra debugging information:")]
		public bool enableDebugLogging =
#if DEBUG
			true;
#else
			false;
#endif


		public override void Save(UnityModManager.ModEntry modEntry)
		{
			Save(this, modEntry);
		}

		public void OnChange()
		{
			
		}
	}
}
