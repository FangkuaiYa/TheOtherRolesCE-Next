using AmongUs.GameOptions;
using HarmonyLib;
using TheOtherRoles.CustomGameModes;
using TheOtherRoles.Utilities;
using TMPro;
using UnityEngine;

namespace TheOtherRoles.Patches
{
	[HarmonyPatch]
	public static class CredentialsPatch
	{
		//        public static string fullCredentialsVersion =
		//$@"<size=130%><color=#ff351f>TheOtherRolesCE</color></size> v{TheOtherRolesPlugin.Version.ToString() + (TheOtherRolesPlugin.betaDays > 0 ? "-BETA" : "")}";
		public static string ModName = $"<size=130%><color=#ff351f>TheOtherRoles Community Edition</color></size> v{TheOtherRolesPlugin.Version.ToString() + (TheOtherRolesPlugin.betaDays > 0 ? "-BETA" : "")}";
		public static string JustASysAdmin = "<color=#FCCE03FF>JustASysAdmin</color>";
		public static string FangKuai = "<color=#00FFFF>FangKuai</color>";
		public static string TOREisbison = "TheOtherRoles by <color=#FCCE03FF>Eisbison</color>";
		public static string SvettyScribbles = "<color=#FCCE03FF>SvettyScribbles</color>";
		public static string LuanMa = "<color=#9932CC>乱码</color>";

		//        public static string contributorsCredentials =
		//$@"<size=60%> <color=#FCCE03FF>Special thanks to <color=#00FFFF>FangKuai<color=#FCCE03FF> & Smeggy</color></size>";
		private static float deltaTime;
		[HarmonyPatch(typeof(PingTracker), nameof(PingTracker.Update))]
		internal static class PingTrackerPatch
		{
			static void Postfix(PingTracker __instance)
			{
				var ping = AmongUsClient.Instance.Ping;
				string PingColor = "#ff4500";
				if (ping < 50) PingColor = "#44dfcc";
				else if (ping < 100) PingColor = "#7bc690";
				else if (ping < 200) PingColor = "#f3920e";
				else if (ping < 400) PingColor = "#ff146e";

				deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
				float fps = Mathf.Ceil(1.0f / deltaTime);

				__instance.text.alignment = AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started ? TextAlignmentOptions.Top : TextAlignmentOptions.TopLeft;
				var position = __instance.GetComponent<AspectPosition>();
				position.Alignment = AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started ? AspectPosition.EdgeAlignments.Top : AspectPosition.EdgeAlignments.LeftTop;
				if (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started)
				{
					string gameModeText = $"";
					if (HideNSeek.isHideNSeekGM) gameModeText = ModTranslation.GetString("Opt-General", 105);
					else if (HandleGuesser.isGuesserGm) gameModeText = ModTranslation.GetString("Opt-General", 106);
					else if (PropHunt.isPropHuntGM) gameModeText = ModTranslation.GetString("Opt-General", 107);
					if (gameModeText != "") gameModeText = Helpers.cs(Color.yellow, gameModeText) + "\n";
					__instance.text.text = $"<size=130%><color=#ff351f>TheOtherRoles CE</color></size> v{TheOtherRolesPlugin.Version.ToString() + (TheOtherRolesPlugin.betaDays > 0 ? "-BETA" : "")}\n{gameModeText}" + $"<color={PingColor}>PING: <b>{AmongUsClient.Instance.Ping}</b> MS</color>" + $"  {(MapOptionsTor.showFPS ? $"  <color=#00a4ff>FPS: {fps}</color>" : "")}";
					position.DistanceFromEdge = new Vector3(1.5f, 0.11f, 0);
				}
				else
				{
					string gameModeText = $"";
					if (MapOptionsTor.gameMode == CustomGamemodes.HideNSeek) gameModeText = ModTranslation.GetString("Opt-General", 105);
					else if (MapOptionsTor.gameMode == CustomGamemodes.Guesser) gameModeText = ModTranslation.GetString("Opt-General", 106);
					else if (MapOptionsTor.gameMode == CustomGamemodes.PropHunt) gameModeText = ModTranslation.GetString("Opt-General", 107);
					if (gameModeText != "") gameModeText = Helpers.cs(Color.yellow, gameModeText);

					__instance.text.text = $"{ModName}<br><size=70%>{ModTranslation.GetString("PingText", 1)} {JustASysAdmin} & {FangKuai}<br>{ModTranslation.GetString("PingText", 2)} {TOREisbison}<br>{ModTranslation.GetString("PingText", 3)} {SvettyScribbles}, {JustASysAdmin}, {FangKuai} & {LuanMa}</size>\n <color={PingColor}>PING: {AmongUsClient.Instance.Ping} MS</color>" + $"  {(MapOptionsTor.showFPS ? $"  <color=#00a4ff>FPS: {fps}</color>" : "")}";
					position.DistanceFromEdge = new Vector3(0.5f, 0.11f);

					try
					{
						var GameModeText = GameObject.Find("GameModeText")?.GetComponent<TextMeshPro>();
						GameModeText.text = gameModeText == "" ? (GameOptionsManager.Instance.currentGameOptions.GameMode == GameModes.HideNSeek ? ModTranslation.GetString("LobbyText", 4) : ModTranslation.GetString("LobbyText", 5)) : gameModeText;
						var ModeLabel = GameObject.Find("ModeLabel")?.GetComponentInChildren<TextMeshPro>();
						ModeLabel.text = ModTranslation.GetString("LobbyText", 6);
					}
					catch { }
				}
				position.AdjustPosition();
			}
		}
	}
}