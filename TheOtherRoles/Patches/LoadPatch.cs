using BepInEx.Unity.IL2CPP.Utils.Collections;
using HarmonyLib;
using System.Collections;
using TheOtherRoles.Modules;
using TheOtherRoles.Modules.CustomHats;
using UnityEngine;
using static TheOtherRoles.Helpers;

namespace TheOtherRoles.Patches;


[HarmonyPatch(typeof(SplashManager), nameof(SplashManager.Update))]
public class LoadPatch
{
	static Sprite logoSprite = loadSpriteFromResources("TheOtherRoles.Resources.MainMenu.TheOtherRolesLoadLogo.png", 100f);
	static Sprite logoGlowSprite = loadSpriteFromResources("TheOtherRoles.Resources.MainMenu.TheOtherRolesLoadLogoGlow.png", 100f);
	public static TMPro.TextMeshPro loadText = null!;
	public static string LoadingText { set { loadText.text = value; } }
	static IEnumerator CoLoadTheOtherRoles(SplashManager __instance)
	{
		var logo = Helpers.CreateObject<SpriteRenderer>("TheOtherRolesLogo", null, new Vector3(0, 0.2f, -5f));
		var logoGlow = Helpers.CreateObject<SpriteRenderer>("TheOtherRolesLogoGlow", null, new Vector3(0, 0.2f, -5f));
		logo.sprite = logoSprite;
		logoGlow.sprite = logoGlowSprite;

		float p = 1f;
		while (p > 0f)
		{
			p -= Time.deltaTime * 2.8f;
			float alpha = 1 - p;
			logo.color = Color.white.AlphaMultiplied(alpha);
			logoGlow.color = Color.white.AlphaMultiplied(Mathf.Min(1f, alpha * (p * 2)));
			logo.transform.localScale = Vector3.one * (p * p * 0.012f + 1f);
			logoGlow.transform.localScale = Vector3.one * (p * p * 0.012f + 1f);
			yield return null;
		}
		logo.color = Color.white;
		logoGlow.gameObject.SetActive(false);
		logo.transform.localScale = Vector3.one;
		logoGlow.color = Color.white;
		logoGlow.transform.localScale = Vector3.one;

		loadText = GameObject.Instantiate(__instance.errorPopup.InfoText, null);
		loadText.transform.localPosition = new(0f, -0.28f, -10f);
		loadText.fontStyle = TMPro.FontStyles.Bold;
		loadText.text = "Loading...";
		loadText.color = Color.white.AlphaMultiplied(0.3f);

		loadText.text = "Loading Assets";
		ZipsLoad.Load();
		yield return new WaitForSeconds(0.5f);

		loadText.text = "Loading Language";
		ModTranslation.Load();
		yield return new WaitForSeconds(0.5f);

		loadText.text = "Loading Settings";
		CustomOptionHolder.Load();
		yield return new WaitForSeconds(0.5f);

		loadText.text = "Loading Custom Color";
		CustomColors.Load();
		yield return new WaitForSeconds(0.5f);

		loadText.text = "Loading Custom Hats";
		CustomHatManager.LoadHats();
		yield return new WaitForSeconds(0.5f);

		while (HatsLoader.isRunning)
		{
			loadText.text = "Downloading Custom Hats";
			yield return false;
		}

		loadText.text = "Loading Custom Options";
		AddToKillDistanceSetting.addKillDistance();
		yield return new WaitForSeconds(0.5f);

		loadText.text = "Loading Main Menu";
		MainMenuPatch.addSceneChangeCallbacks();
		yield return new WaitForSeconds(0.5f);

		loadText.text = "Try To Load Submerged";
		SubmergedCompatibility.Initialize();
		yield return new WaitForSeconds(0.5f);
		if (SubmergedCompatibility.LoadedExternally) loadText.text = "Loading Submerged";

        loadText.text = "Loading Completed";
		for (int i = 0; i < 3; i++)
		{
			loadText.gameObject.SetActive(false);
			yield return new WaitForSeconds(0.03f);
			loadText.gameObject.SetActive(true);
			yield return new WaitForSeconds(0.03f);
		}

		GameObject.Destroy(loadText.gameObject);

		p = 1f;
		while (p > 0f)
		{
			logo.gameObject.SetActive(false);
			logoGlow.gameObject.SetActive(true);
			p -= Time.deltaTime * 1.2f;
			logoGlow.color = Color.white.AlphaMultiplied(p); 
			yield return null;
		}
		logoGlow.color = Color.clear;


		__instance.sceneChanger.AllowFinishLoadingScene();
		__instance.startedSceneLoad = true;
	}

	static bool loadedTheOtherRoles = false;
	public static bool Prefix(SplashManager __instance)
	{
		if (__instance.doneLoadingRefdata && !__instance.startedSceneLoad && Time.time - __instance.startTime > __instance.minimumSecondsBeforeSceneChange && !loadedTheOtherRoles)
		{
			loadedTheOtherRoles = true;
			__instance.StartCoroutine(CoLoadTheOtherRoles(__instance).WrapToIl2Cpp());
		}

		return false;
	}
}
