using System;
using System.Reflection;
using HarmonyLib;
using UnityModManagerNet;

namespace SteamSimulationTweaks
{
	public static class Main
	{
		public static UnityModManager.ModEntry.ModLogger Logger
		{  get; private set; }
		public static Settings Settings { get; private set; }

		public static bool IsRearrangedS282Installed { get; private set; }

		// Unity Mod Manage Wiki: https://wiki.nexusmods.com/index.php/Category:Unity_Mod_Manager
		private static bool Load(UnityModManager.ModEntry modEntry)
		{
			Logger = modEntry.Logger;
			Harmony harmony = null;

			Settings = Settings.Load<Settings>(modEntry);
			modEntry.OnGUI = OnGUI;
			modEntry.OnSaveGUI = OnSaveGUI;

			IsRearrangedS282Installed = UnityModManager.FindMod("RearrangedS282") is not null;

			try
			{
				harmony = new Harmony(modEntry.Info.Id);
				harmony.PatchAll(Assembly.GetExecutingAssembly());

				// Other plugin startup logic
			}
			catch (Exception ex)
			{
				modEntry.Logger.LogException($"Failed to load {modEntry.Info.DisplayName}:", ex);
				harmony.UnpatchAll(modEntry.Info.Id);
				return false;
			}

			return true;
		}

		static void OnGUI(UnityModManager.ModEntry modEntry)
		{
			Settings.Draw(modEntry);
		}

		static void OnSaveGUI(UnityModManager.ModEntry modEntry)
		{
			Settings.Save(modEntry);
		}
	}
}
