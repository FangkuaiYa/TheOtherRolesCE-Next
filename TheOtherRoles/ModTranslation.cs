using HarmonyLib;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using TheOtherRoles.Modules;
using TheOtherRoles.Patches;
using UnityEngine;


namespace TheOtherRoles
{
	public class TranslationInfo
	{
		public TranslationInfo(string text)
			: this(text, Color.white)
		{
		}

		public TranslationInfo(string text, Color color)
		{
			this.text = text;
			this.color = color;
		}

		public TranslationInfo(string category, int id)
			: this(category, id, Color.white)
		{
		}

		public TranslationInfo(string category, int id, Color color)
		{
			this.category = category;
			this.id = id;
			this.color = color;
		}

		public TranslationInfo(RoleId roleId)
			: this(roleId, Color.white)
		{
		}

		public TranslationInfo(RoleId roleId, Color color)
		{
			this.roleId = roleId;
			this.color = color;
		}

		public void AddHeadText(string text)
		{
			headText = text;
		}
		public void AddTailText(string text)
		{
			tailText = text;
		}

		public string GetString()
		{
			if (!string.IsNullOrEmpty(text))
				return Helpers.cs(color, headText + text + tailText);
			if (roleId != RoleId.Max)
				return Helpers.cs(color, headText + ModTranslation.GetRoleName(roleId, color) + tailText);
			return Helpers.cs(color, headText + ModTranslation.GetString(category, id) + tailText);
		}

		public override string ToString()
		{
			return GetString();
		}

		string headText;
		string text;
		string tailText;
		string category;
		int id;
		Color color;
		RoleId roleId = RoleId.Max;
	}

	public class ModTranslation
	{
		// Dictionary<Category, Dictionary<Category-Id, Dictionary<Lang-Id, Str>>>
		public static Dictionary<string, Dictionary<int, Dictionary<int, string>>> stringTable;

		public static void Load()
		{
			var assembly = Assembly.GetExecutingAssembly();
			Stream stream = assembly.GetManifestResourceStream("TheOtherRoles.Resources.stringData.json");
			var byteArray = new byte[stream.Length];
			var read = stream.Read(byteArray, 0, (int)stream.Length);
			string json = System.Text.Encoding.UTF8.GetString(byteArray);
			stringTable = new();
			JObject parsed = JObject.Parse(json);

			for (int i = 0; i < parsed.Count; i++)
			{
				JProperty token = parsed.ChildrenTokens[i].TryCast<JProperty>();
				if (token == null) continue;
				var val = token.Value.TryCast<JObject>();
				if (token.HasValues)
				{
					string categoryStr = token.Name;
					int index = categoryStr.IndexOf(",");
					string categoryName = categoryStr.Substring(0, index);
					int categoryId = int.Parse(categoryStr.Substring(index + 1));

					if (!stringTable.TryGetValue(categoryName, out var t))
					{
						t = new();
						stringTable.Add(categoryName, t);
					}

					var strings = new Dictionary<int, string>();
					for (int j = 0; j < (int)SupportedLangs.Irish + 1; j++)
					{
						string key = j.ToString();
						var text = val[key]?.TryCast<JValue>().Value.ToString();

						if (text != null && text.Length > 0)
						{
							if (text == blankText) strings[j] = "";
							else strings[j] = text;
						}
					}

					t[categoryId] = strings;
				}
			}
		}

		public static string GetString(string category, int id, string def = null)
		{
			//TownOfRolesPlugin.Instance.Log.LogMessage($"category:{category}, id:{id}, def:{def}");
			if (!stringTable.TryGetValue(category, out var t))
				return def;
			if (!t.TryGetValue(id, out var t2))
				return def;
			int langId = (int)AmongUs.Data.DataManager.Settings.Language.CurrentLanguage;
			if (t2.ContainsKey(langId))
				return t2[langId];
			else if (t2.ContainsKey(defaultLangId))
				return t2[defaultLangId];

			return def;
		}

		public static TranslationInfo GetRoleName(RoleId roleId, Color? color = null)
		{
			return new TranslationInfo("Role-Name", GetRoleStringId(roleId), color.HasValue ? color.Value : Color.white);
		}

		public static TranslationInfo GetRoleIntroDesc(RoleId roleId, Color? color = null)
		{
			return new TranslationInfo("Role-IntroDesc", GetRoleStringId(roleId), color.HasValue ? color.Value : Color.white);
		}

		public static TranslationInfo GetRoleShortDesc(RoleId roleId, Color? color = null)
		{
			return new TranslationInfo("Role-ShortDesc", GetRoleStringId(roleId), color.HasValue ? color.Value : Color.white);
		}

		static int GetRoleStringId(RoleId roleId)
		{
			int id = -1;
			switch (roleId)
			{
				case RoleId.Jester: id = 1; break;
				case RoleId.Werewolf: id = 2; break;
				case RoleId.Prosecutor: id = 3; break;
				case RoleId.Swooper: id = 4; break;
				case RoleId.Mayor: id = 5; break;
				case RoleId.Portalmaker: id = 6; break;
				case RoleId.Engineer: id = 7; break;
				case RoleId.PrivateInvestigator: id = 8; break;
				case RoleId.Sheriff: id = 9; break;
				case RoleId.BodyGuard: id = 10; break;
				case RoleId.Deputy: id = 11; break;
				case RoleId.Lighter: id = 12; break;
				case RoleId.Godfather: id = 13; break;
				case RoleId.Mafioso: id = 14; break;
				case RoleId.Janitor: id = 15; break;
				case RoleId.Morphling: id = 16; break;
				case RoleId.Bomber: id = 18; break;
				case RoleId.Bomber2: id = 20; break;
				case RoleId.Yoyo: id = 17; break;
				case RoleId.Poucher: id = 19; break;
				case RoleId.Mimic: id = 21; break;
				case RoleId.Camouflager: id = 22; break;
				case RoleId.Miner: id = 23; break;
				case RoleId.Vampire: id = 24; break;
				case RoleId.Eraser: id = 25; break;
				case RoleId.Trickster: id = 26; break;
				case RoleId.Cleaner: id = 27; break;
				case RoleId.Undertaker: id = 28; break;
				case RoleId.Warlock: id = 29; break;
				case RoleId.BountyHunter: id = 30; break;
				case RoleId.Detective: id = 31; break;
				case RoleId.TimeMaster: id = 32; break;
				case RoleId.Veteran: id = 33; break;
				case RoleId.Medic: id = 34; break;
				case RoleId.Swapper: id = 35; break;
				case RoleId.Seer: id = 36; break;
				case RoleId.Hacker: id = 37; break;
				case RoleId.Tracker: id = 38; break;
				case RoleId.Snitch: id = 39; break;
				case RoleId.Jackal: id = 40; break;
				case RoleId.Sidekick: id = 41; break;
				case RoleId.Spy: id = 42; break;
				case RoleId.SecurityGuard: id = 43; break;
				case RoleId.Arsonist: id = 44; break;
				case RoleId.Amnisiac: id = 45; break;
				case RoleId.Vulture: id = 46; break;
				case RoleId.Medium: id = 47; break;
				case RoleId.Trapper: id = 48; break;
				case RoleId.Lawyer: id = 49; break;
				case RoleId.Pursuer: id = 50; break;
				case RoleId.Impostor: id = 51; break;
				case RoleId.Crewmate: id = 52; break;
				case RoleId.Witch: id = 53; break;
				case RoleId.Cultist: id = 54; break;
				case RoleId.Ninja: id = 55; break;
				case RoleId.Blackmailer: id = 56; break;
				case RoleId.Thief: id = 57; break;
				case RoleId.Doomsayer: id = 58; break;
				case RoleId.Hunter: id = 59; break;
				case RoleId.Hunted: id = 60; break;
				case RoleId.Prop: id = 61; break;
				case RoleId.Bloody: id = 62; break;
				case RoleId.AntiTeleport: id = 63; break;
				case RoleId.Tiebreaker: id = 64; break;
				case RoleId.Bait: id = 65; break;
				case RoleId.Sunglasses: id = 66; break;
				case RoleId.Lover: id = 67; break;
				case RoleId.Mini: id = 68; break;
				case RoleId.Vip: id = 69; break;
				case RoleId.Indomitable: id = 70; break;
				case RoleId.Slueth: id = 71; break;
				case RoleId.Cursed: id = 72; break;
				case RoleId.Invert: id = 73; break;
				case RoleId.Blind: id = 74; break;
				case RoleId.Tunneler: id = 75; break;
				case RoleId.NiceGuesser: id = 76; break;
				case RoleId.Paranoid: id = 77; break;
				case RoleId.Disperser: id = 78; break;
				case RoleId.EvilGuesser: id = 79; break;
				case RoleId.Chameleon: id = 80; break;
				case RoleId.Shifter: id = 81; break;
				case RoleId.Survivor: id = 82; break;
				case RoleId.Juggernaut: id = 83; break;
				case RoleId.PlagueDoctor: id = 84; break;
				case RoleId.Cupid: id = 85; break;
				case RoleId.Radar: id = 86; break;
				case RoleId.Torch: id = 87; break;
			}
			return id;
		}

		const string blankText = "[BLANK]";
		const int defaultLangId = (int)SupportedLangs.English;
	}

	[HarmonyPatch(typeof(LanguageSetter), nameof(LanguageSetter.SetLanguage))]
	class SetLanguagePatch
	{
		static void Postfix()
		{
			ClientOptionsPatch.UpdateTranslations();
		}
	}
}
