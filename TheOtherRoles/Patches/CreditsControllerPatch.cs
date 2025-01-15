using System.Collections.Generic;
using System.Linq;
using HarmonyLib;

namespace TheOtherRoles.Patches
{

    [HarmonyPatch(typeof(CreditsController))]
    public class CreditsControllerPatch
    {
        private static List<CreditsController.CreditStruct> GetModCredits()
        {
            var devList = new List<string>()
            {
                "The Other Roles Community Edition",
                $"By —{CredentialsPatch.FangKuai} & {CredentialsPatch.JustASysAdmin}",
                "",
                $"<color=#FCCE03FF>Imp11</color> - {ModTranslation.GetString("CreditsController", 1)}",
                $"<color=#cdfffd>XtremeWave</color> - {ModTranslation.GetString("CreditsController", 1)}",
                $"<color=#49FFA5>Yu</color> - {ModTranslation.GetString("CreditsController", 1)}",
                $"<color=#FCCE03FF>TEL</color> - {ModTranslation.GetString("CreditsController", 1)}",
                $"{CredentialsPatch.LuanMa} - {ModTranslation.GetString("CreditsController", 2)}",
                $"{CredentialsPatch.FangKuai} - {ModTranslation.GetString("CreditsController", 2)}",
                $"{CredentialsPatch.JustASysAdmin} - {ModTranslation.GetString("CreditsController", 2)}",
                $"{CredentialsPatch.SvettyScribbles} - {ModTranslation.GetString("CreditsController", 2)}",
            };
            var translatorList = new List<string>()
            {
                $"<color=#FCCE03FF>miru-y</color> - {ModTranslation.GetString("CreditsController", 3)}",
                $"<color=#00ffff>FangKuai</color> - {ModTranslation.GetString("CreditsController", 4)}",
                $"<color=#FCCE03FF>Imp11</color> - {ModTranslation.GetString("CreditsController", 4)}",
            };
            var acList = new List<string>()
            {
                //Mods
                Helpers.GradientColorText("FFD700", "FF0000", $"TheOtherRoles GM IA") + " - by <color=#FCCE03FF>Imp11</color>",
                "<color=#ff351f>The Other Us</color> - by <color=#FCCE03FF>Spex</color>",
                "<color=#ff351f>The Other Us-Edited</color> - by <color=#FFB793>mxyx</color>",
                "<color=#ff351f>The Other Roles MR</color> - by <color=#FCCE03FF>miru-y</color>",
                "<color=#FFC0CB>Town Of New Epic Xtreme</color> - by <color=#cdfffd>XtremeWave</color>",
                "<color=#ffc0cb>Town Of Next</color> - by <color=#ffc0cb>KARPED1EM</color>",
                "<color=#fffcbe>YuEzTools</color> - by <color=#49FFA5>Yu</color>",
            };

            var credits = new List<CreditsController.CreditStruct>();

            AddTitleToCredits(Helpers.cs(TheOtherRolesPlugin.ModColor32, "The Other Roles Community Edition"));
            AddPersonToCredits(devList);
            AddSpcaeToCredits();

            AddTitleToCredits(ModTranslation.GetString("CreditsController", 5));
            AddPersonToCredits(translatorList);
            AddSpcaeToCredits();

            AddTitleToCredits(ModTranslation.GetString("CreditsController", 6));
            AddPersonToCredits(acList);
            AddSpcaeToCredits();

            return credits;

            void AddSpcaeToCredits()
            {
                AddTitleToCredits(string.Empty);
            }
            void AddTitleToCredits(string title)
            {
                credits.Add(new()
                {
                    format = "title",
                    columns = new[] { title },
                });
            }
            void AddPersonToCredits(List<string> list)
            {
                foreach (var line in list)
                {
                    var cols = line.Split(" - ").ToList();
                    if (cols.Count < 2) cols.Add(string.Empty);
                    credits.Add(new()
                    {
                        format = "person",
                        columns = cols.ToArray(),
                    });
                }
            }
        }

        [HarmonyPatch(nameof(CreditsController.AddCredit)), HarmonyPrefix]
        public static void AddCreditPrefix(CreditsController __instance, [HarmonyArgument(0)] CreditsController.CreditStruct originalCredit)
        {
            if (originalCredit.columns[0] != "logoImage") return;

            foreach (var credit in GetModCredits())
            {
                __instance.AddCredit(credit);
                __instance.AddFormat(credit.format);
            }
        }
    }
}