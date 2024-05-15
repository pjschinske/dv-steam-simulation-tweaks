using DV.CabControls;
using DV.CabControls.Spec;
using DV.Simulation.Controllers;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SteamSimulationTweaks.Patches
{


	//[HarmonyPatch(typeof(LeverBase), nameof(LeverBase.SetSpringTarget))]
	class IncreaseThrottleScrollSpeedPatch
	{
		/*static void Prefix(LeverBase __instance, ref float target)
		{
			if (__instance.hj.name != "C_Regulator"
			*//*|| __instance.car.carLivery.id != "LocoS282A"*//*)
			{
				return;
			}
			*//*Main.Logger.Log("Old target: " + target);
			if (__instance.hj.spring.targetPosition > target)
			{
				target = __instance.hj.limits.min;
			}
			else
			{
				target = __instance.hj.limits.max;
			}
			Main.Logger.Log("New target: " + target);
			JointSpring spring = __instance.hj.spring;
			spring.spring = __instance.spec.scrollWheelSpring * 3;
			//Main.Logger.Log("New spring strength: " + spring.spring);
			__instance.hj.spring = spring;
		}*/
	}

	[HarmonyPatch(typeof(TrainCar), nameof(TrainCar.LoadInterior))]
	class IncreaseThrottleScrollSpeedPatch2
	{
		private const int THROTTLE_NUM_OF_NOTCHES = 41;
		private const float DEFAULT_THROTTLE_SCROLL_WHEEL_HOVER_SCROLL = -3;
		private const float DEFAULT_THROTTLE_SCROLL_WHEEL_SPRING = 400;
		private const float DEFAULT_THROTTLE_JOINT_SPRING = 50;

		static void Postfix(TrainCar __instance)
		{
			if (__instance.carLivery.id != "LocoS282A")
			{
				return;
			}

			Transform c_regulator = __instance.loadedInterior.transform.Find("Center/Regulator/C_Regulator");
			if (c_regulator is null)
			{
				Main.Logger.Error("Could not find regulator transform");
				return;
			}
			Lever throttleLever = c_regulator.GetComponent<Lever>();
			if (throttleLever is null)
			{
				Main.Logger.Error("Could not find throttle lever");
				return;
			}
			/*throttleLever.rigidbodyDrag = 1;
			throttleLever.rigidbodyAngularDrag = 2000;
			throttleLever.pullingForceMultiplier = 3;*/
			throttleLever.notches = THROTTLE_NUM_OF_NOTCHES;
			throttleLever.scrollWheelHoverScroll = DEFAULT_THROTTLE_SCROLL_WHEEL_HOVER_SCROLL
				/ Main.Settings.throttleLeverScrollWheelHoverScroll;
			throttleLever.scrollWheelSpring = DEFAULT_THROTTLE_SCROLL_WHEEL_SPRING
				* Main.Settings.throttleLeverScrollWheelSpring;
			throttleLever.jointSpring = DEFAULT_THROTTLE_JOINT_SPRING
				* Main.Settings.throttleLeverJointSpring;
		}
	}
}
