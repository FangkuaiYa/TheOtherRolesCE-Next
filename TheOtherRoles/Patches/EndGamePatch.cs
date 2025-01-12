using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
using TheOtherRoles.CustomGameModes;
using TheOtherRoles.Players;
using TheOtherRoles.Utilities;
using TMPro;
using UnityEngine;
using static TheOtherRoles.TheOtherRoles;

namespace TheOtherRoles.Patches
{
    enum CustomGameOverReason
    {
        LoversWin = 10,
        TeamJackalWin = 11,
        MiniLose = 12,
        JesterWin = 13,
        ArsonistWin = 14,
        VultureWin = 15,
        LawyerSoloWin = 17,
        ProsecutorWin = 10,
        SwooperWin = 18,
        WerewolfWin = 19,
        DoomsayerWin = 20,
        JuggernautWin = 21,
        PlagueDoctorWin = 22,
        CupidLoversWin = 23
    }

    enum WinCondition
    {
        Default,
        LoversTeamWin,
        LoversSoloWin,
        JesterWin,
        JackalWin,
        MiniLose,
        ArsonistWin,
        VultureWin,
        LawyerSoloWin,
        AdditionalLawyerStolenWin,
        AdditionalLawyerBonusWin,
        AdditionalAlivePursuerWin,
        AdditionalAliveProsecutorWin,
        AdditionalAliveSurvivorWin,
        ProsecutorWin,
        SwooperWin,
        WerewolfWin,
        JuggernautWin,
        DoomsayerWin,
        EveryoneDied,
        PlagueDoctorWin,
        CupidLoversWin,
        CrewmateWin,
        ImpostorWin
    }

    static class AdditionalTempData
    {
        // Should be implemented using a proper GameOverReason in the future
        public static WinCondition winCondition = WinCondition.Default;
        public static List<WinCondition> additionalWinConditions = new List<WinCondition>();
        public static List<PlayerRoleInfo> playerRoles = new List<PlayerRoleInfo>();
        public static float timer = 0;
        public static Dictionary<int, PlayerControl> plagueDoctorInfected = new Dictionary<int, PlayerControl>();
        public static Dictionary<int, float> plagueDoctorProgress = new Dictionary<int, float>();

        public static void clear()
        {
            playerRoles.Clear();
            additionalWinConditions.Clear();
            winCondition = WinCondition.Default;
            timer = 0;
        }

        internal class PlayerRoleInfo
        {
            public string PlayerName { get; set; }
            public List<RoleInfo> Roles { get; set; }
            public string RoleNames { get; set; }
            public int TasksCompleted { get; set; }
            public int TasksTotal { get; set; }
            public bool IsGuesser { get; set; }
            public int? Kills { get; set; }
            public bool IsAlive { get; set; }
            public byte PlayerId { get; set; }
        }
    }


    [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnGameEnd))]
    public class OnGameEndPatch
    {
        private static GameOverReason gameOverReason;
        public static void Prefix(AmongUsClient __instance, [HarmonyArgument(0)] ref EndGameResult endGameResult)
        {
            gameOverReason = endGameResult.GameOverReason;
            if ((int)endGameResult.GameOverReason >= 10) endGameResult.GameOverReason = GameOverReason.ImpostorByKill;

            // Reset zoomed out ghosts
            Helpers.toggleZoom(reset: true);
        }

        public static void Postfix(AmongUsClient __instance, [HarmonyArgument(0)] ref EndGameResult endGameResult)
        {
            AdditionalTempData.clear();

            foreach (var playerControl in CachedPlayer.AllPlayers)
            {
                var roles = RoleInfo.getRoleInfoForPlayer(playerControl);
                var (tasksCompleted, tasksTotal) = TasksHandler.taskInfo(playerControl.Data);
                bool isGuesser = HandleGuesser.isGuesserGm && HandleGuesser.isGuesser(playerControl.PlayerId);
                int? killCount = GameHistory.deadPlayers.FindAll(x => x.killerIfExisting != null && x.killerIfExisting.PlayerId == playerControl.PlayerId).Count;
                if (killCount == 0 && !(new List<RoleInfo>() { RoleInfo.sheriff, RoleInfo.jackal, RoleInfo.sidekick, RoleInfo.thief }.Contains(RoleInfo.getRoleInfoForPlayer(playerControl, false).FirstOrDefault()) || playerControl.Data.Role.IsImpostor))
                {
                    killCount = null;
                }
                byte playerId = playerControl.PlayerId;
                string roleString = RoleInfo.GetRolesString(playerControl, true, true, false);
                AdditionalTempData.playerRoles.Add(new AdditionalTempData.PlayerRoleInfo() { PlayerName = playerControl.Data.PlayerName, Roles = roles, PlayerId = playerId, RoleNames = roleString, TasksTotal = tasksTotal, TasksCompleted = tasksCompleted, IsGuesser = isGuesser, Kills = killCount, IsAlive = !playerControl.Data.IsDead });
            }
            AdditionalTempData.plagueDoctorInfected = PlagueDoctor.infected;
            AdditionalTempData.plagueDoctorProgress = PlagueDoctor.progress;

            // Remove Jester, Swooper, Amnesiac, Arsonist, Vulture, Jackal, former Jackals and Sidekick from winners (if they win, they'll be readded)
            List<PlayerControl> notWinners = new List<PlayerControl>();
            if (Jester.jester != null) notWinners.Add(Jester.jester);
            if (Swooper.swooper != null) notWinners.Add(Swooper.swooper);
            if (Sidekick.sidekick != null) notWinners.Add(Sidekick.sidekick);
            if (Amnisiac.amnisiac != null) notWinners.Add(Amnisiac.amnisiac);
            if (Jackal.jackal != null) notWinners.Add(Jackal.jackal);
            if (Arsonist.arsonist != null) notWinners.Add(Arsonist.arsonist);
            if (Vulture.vulture != null) notWinners.Add(Vulture.vulture);
            if (Cupid.cupid != null) notWinners.Add(Cupid.cupid);
            if (Werewolf.werewolf != null) notWinners.Add(Werewolf.werewolf);
            if (Lawyer.lawyer != null) notWinners.Add(Lawyer.lawyer);
            if (Pursuer.pursuer != null) notWinners.Add(Pursuer.pursuer);
            if (Thief.thief != null) notWinners.Add(Thief.thief);
            if (Survivor.survivor != null) notWinners.AddRange(Survivor.survivor);
            if (Doomsayer.doomsayer != null) notWinners.Add(Doomsayer.doomsayer);
            if (Juggernaut.juggernaut != null) notWinners.Add(Juggernaut.juggernaut);
            if (PlagueDoctor.plagueDoctor != null) notWinners.Add(PlagueDoctor.plagueDoctor);

            notWinners.AddRange(Jackal.formerJackals);

            List<CachedPlayerData> winnersToRemove = new List<CachedPlayerData>();
            foreach (CachedPlayerData winner in EndGameResult.CachedWinners.GetFastEnumerator())
            {
                if (notWinners.Any(x => x.Data.PlayerName == winner.PlayerName)) winnersToRemove.Add(winner);
            }
            foreach (var winner in winnersToRemove) EndGameResult.CachedWinners.Remove(winner);

            bool saboWin = gameOverReason == GameOverReason.ImpostorBySabotage;
            bool impostorkillWin = gameOverReason == GameOverReason.ImpostorByKill;
            bool impostorvoteWin = gameOverReason == GameOverReason.ImpostorByVote;
            bool impostorWin = saboWin || impostorkillWin || impostorvoteWin;

            bool taskWin = gameOverReason == GameOverReason.HumansByTask;
            bool crewmatevoteWin = gameOverReason == GameOverReason.HumansByVote;
            bool crewmateWin = taskWin || crewmatevoteWin;

            bool doomsayerWin = Doomsayer.doomsayer != null && gameOverReason == (GameOverReason)CustomGameOverReason.DoomsayerWin;
            bool jesterWin = Jester.jester != null && gameOverReason == (GameOverReason)CustomGameOverReason.JesterWin;
            bool swooperWin = gameOverReason == (GameOverReason)CustomGameOverReason.SwooperWin && ((Swooper.swooper != null && !Swooper.swooper.Data.IsDead));
            bool werewolfWin = gameOverReason == (GameOverReason)CustomGameOverReason.WerewolfWin && ((Werewolf.werewolf != null && !Werewolf.werewolf.Data.IsDead));
            bool juggernautWin = gameOverReason == (GameOverReason)CustomGameOverReason.JuggernautWin && ((Juggernaut.juggernaut != null && !Juggernaut.juggernaut.Data.IsDead));
            bool arsonistWin = Arsonist.arsonist != null && gameOverReason == (GameOverReason)CustomGameOverReason.ArsonistWin;
            bool miniLose = Mini.mini != null && gameOverReason == (GameOverReason)CustomGameOverReason.MiniLose;
            bool loversWin = Lovers.existingAndAlive() && (gameOverReason == (GameOverReason)CustomGameOverReason.LoversWin || (GameManager.Instance.DidHumansWin(gameOverReason) && !Lovers.existingWithKiller())); // Either they win if they are among the last 3 players, or they win if they are both Crewmates and both alive and the Crew wins (Team Imp/Jackal Lovers can only win solo wins)
            bool teamJackalWin = gameOverReason == (GameOverReason)CustomGameOverReason.TeamJackalWin && ((Jackal.jackal != null && !Jackal.jackal.Data.IsDead) || (Sidekick.sidekick != null && !Sidekick.sidekick.Data.IsDead));
            bool vultureWin = Vulture.vulture != null && gameOverReason == (GameOverReason)CustomGameOverReason.VultureWin;
            bool lawyerSoloWin = Lawyer.lawyer != null && gameOverReason == (GameOverReason)CustomGameOverReason.LawyerSoloWin;
            bool everyoneDead = AdditionalTempData.playerRoles.All(x => !x.IsAlive);
            bool plagueDoctorWin = PlagueDoctor.plagueDoctor != null && gameOverReason == (GameOverReason)CustomGameOverReason.PlagueDoctorWin;
            bool cupidLoversWin = Cupid.lovers1 != null && Cupid.lovers2 != null && !Cupid.lovers1.Data.IsDead && !Cupid.lovers2.Data.IsDead && gameOverReason == (GameOverReason)CustomGameOverReason.CupidLoversWin;
            bool prosecutorWin = Lawyer.lawyer != null && gameOverReason == (GameOverReason)CustomGameOverReason.ProsecutorWin;

            bool isPursurerLose = arsonistWin || miniLose;

            // Crewmates Win
            if (crewmateWin)
            {
                EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
                foreach (PlayerControl p in CachedPlayer.AllPlayers)
                {
                    if (!p.Data.Role.IsImpostor && !Helpers.isNeutral(p))
                    {
                        CachedPlayerData wpd = new(p.Data);
                        EndGameResult.CachedWinners.Add(wpd);
                    }
                }
                AdditionalTempData.winCondition = WinCondition.CrewmateWin;
            }

            // Impostors Win
            if (impostorWin)
            {
                EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
                foreach (PlayerControl p in CachedPlayer.AllPlayers)
                {
                    if (p.Data.Role.IsImpostor)
                    {
                        CachedPlayerData wpd = new(p.Data);
                        EndGameResult.CachedWinners.Add(wpd);
                    }
                }
                AdditionalTempData.winCondition = WinCondition.ImpostorWin;
            }

            // Prosecutor win
            if (prosecutorWin)
            {
                EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
                CachedPlayerData wpd = new CachedPlayerData(Lawyer.lawyer.Data);
                EndGameResult.CachedWinners.Add(wpd);
                AdditionalTempData.winCondition = WinCondition.ProsecutorWin;
            }

            // Mini lose
            if (miniLose)
            {
                EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
                CachedPlayerData wpd = new CachedPlayerData(Mini.mini.Data);
                wpd.IsYou = false; // If "no one is the Mini", it will display the Mini, but also show defeat to everyone
                EndGameResult.CachedWinners.Add(wpd);
                AdditionalTempData.winCondition = WinCondition.MiniLose;
            }

            else if (doomsayerWin)
            {
                // DoomsayerWin wins if nobody except jackal is alive
                EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
                CachedPlayerData wpd = new CachedPlayerData(Doomsayer.doomsayer.Data);
                EndGameResult.CachedWinners.Add(wpd);
                AdditionalTempData.winCondition = WinCondition.DoomsayerWin;
            }

            // Jester win
            else if (jesterWin)
            {
                EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
                CachedPlayerData wpd = new CachedPlayerData(Jester.jester.Data);
                EndGameResult.CachedWinners.Add(wpd);
                AdditionalTempData.winCondition = WinCondition.JesterWin;
            }

            // Arsonist win
            else if (arsonistWin)
            {
                EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
                CachedPlayerData wpd = new CachedPlayerData(Arsonist.arsonist.Data);
                EndGameResult.CachedWinners.Add(wpd);
                AdditionalTempData.winCondition = WinCondition.ArsonistWin;
            }

            // Vulture win
            else if (vultureWin)
            {
                EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
                CachedPlayerData wpd = new CachedPlayerData(Vulture.vulture.Data);
                EndGameResult.CachedWinners.Add(wpd);
                AdditionalTempData.winCondition = WinCondition.VultureWin;
            }

            else if (plagueDoctorWin)
            {
                EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
                CachedPlayerData wpd = new CachedPlayerData(PlagueDoctor.plagueDoctor.Data);
                EndGameResult.CachedWinners.Add(wpd);
                AdditionalTempData.winCondition = WinCondition.PlagueDoctorWin;
            }

            // Lovers win conditions
            else if (loversWin)
            {
                // Double win for lovers, crewmates also win
                if (!Lovers.existingWithKiller())
                {
                    AdditionalTempData.winCondition = WinCondition.LoversTeamWin;
                    EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
                    foreach (PlayerControl p in CachedPlayer.AllPlayers)
                    {
                        if (p == null) continue;
                        if (p == Lovers.lover1 || p == Lovers.lover2)
                            EndGameResult.CachedWinners.Add(new CachedPlayerData(p.Data));
                        else if (p == Pursuer.pursuer && !Pursuer.pursuer.Data.IsDead)
                            EndGameResult.CachedWinners.Add(new CachedPlayerData(p.Data));
                        else if (Survivor.survivor.Any(pc => pc == p) && !Survivor.survivor.Any(pc => pc.Data.IsDead))
                            EndGameResult.CachedWinners.Add(new CachedPlayerData(p.Data));
                        else if (p == Cupid.lovers1 || p == Cupid.lovers2)
                            EndGameResult.CachedWinners.Add(new CachedPlayerData(p.Data));
                        else if (p != Jester.jester && p != Jackal.jackal && p != Doomsayer.doomsayer && p != PlagueDoctor.plagueDoctor && p != Swooper.swooper && p != Juggernaut.juggernaut && p != Werewolf.werewolf && p != Sidekick.sidekick && p != Arsonist.arsonist && p != Vulture.vulture && !Jackal.formerJackals.Contains(p) && !p.Data.Role.IsImpostor)
                            EndGameResult.CachedWinners.Add(new CachedPlayerData(p.Data));
                    }
                }
                // Lovers solo win
                else
                {
                    AdditionalTempData.winCondition = WinCondition.LoversSoloWin;
                    EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
                    EndGameResult.CachedWinners.Add(new CachedPlayerData(Lovers.lover1.Data));
                    EndGameResult.CachedWinners.Add(new CachedPlayerData(Lovers.lover2.Data));
                }
            }

            // Jackal win condition (should be implemented using a proper GameOverReason in the future)
            else if (teamJackalWin)
            {
                // Jackal wins if nobody except jackal is alive
                AdditionalTempData.winCondition = WinCondition.JackalWin;
                EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
                CachedPlayerData wpd = new CachedPlayerData(Jackal.jackal.Data);
                wpd.IsImpostor = false;
                EndGameResult.CachedWinners.Add(wpd);
                // If there is a sidekick. The sidekick also wins
                if (Sidekick.sidekick != null)
                {
                    CachedPlayerData wpdSidekick = new CachedPlayerData(Sidekick.sidekick.Data);
                    wpdSidekick.IsImpostor = false;
                    EndGameResult.CachedWinners.Add(wpdSidekick);
                }
                foreach (var player in Jackal.formerJackals)
                {
                    CachedPlayerData wpdFormerJackal = new CachedPlayerData(player.Data);
                    wpdFormerJackal.IsImpostor = false;
                    EndGameResult.CachedWinners.Add(wpdFormerJackal);
                }
            }

            else if (swooperWin)
            {
                // Swooper wins if nobody except jackal is alive
                AdditionalTempData.winCondition = WinCondition.SwooperWin;
                EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
                CachedPlayerData wpd = new CachedPlayerData(Swooper.swooper.Data);
                wpd.IsImpostor = false;
                EndGameResult.CachedWinners.Add(wpd);
            }

            else if (werewolfWin)
            {
                // Werewolf wins if nobody except jackal is alive
                AdditionalTempData.winCondition = WinCondition.WerewolfWin;
                EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
                CachedPlayerData wpd = new CachedPlayerData(Werewolf.werewolf.Data);
                wpd.IsImpostor = false;
                EndGameResult.CachedWinners.Add(wpd);
            }

            else if (cupidLoversWin)
            {
                AdditionalTempData.winCondition = WinCondition.CupidLoversWin;
                EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
                EndGameResult.CachedWinners.Add(new CachedPlayerData(Cupid.lovers1.Data));
                EndGameResult.CachedWinners.Add(new CachedPlayerData(Cupid.lovers2.Data));
            }

            else if (juggernautWin)
            {
                AdditionalTempData.winCondition = WinCondition.JuggernautWin;
                EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
                CachedPlayerData wpd = new CachedPlayerData(Juggernaut.juggernaut.Data);
                wpd.IsImpostor = false;
                EndGameResult.CachedWinners.Add(wpd);
            }

            // Everyone Died
            else if (everyoneDead)
            {
                EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
                AdditionalTempData.winCondition = WinCondition.EveryoneDied;
            }

            // Lawyer solo win 
            else if (lawyerSoloWin)
            {
                EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
                CachedPlayerData wpd = new(Lawyer.lawyer.Data);
                EndGameResult.CachedWinners.Add(wpd);
                AdditionalTempData.winCondition = WinCondition.LawyerSoloWin;
            }

            bool pursuerWin = false;
            // Possible Additional winner: Pursuer
            if (Pursuer.pursuer != null && !Pursuer.pursuer.Data.IsDead && !Pursuer.notAckedExiled && !isPursurerLose && !EndGameResult.CachedWinners.ToArray().Any(x => x.IsImpostor))
            {
                if (!EndGameResult.CachedWinners.ToArray().Any(x => x.PlayerName == Pursuer.pursuer.Data.PlayerName))
                    EndGameResult.CachedWinners.Add(new CachedPlayerData(Pursuer.pursuer.Data));
                AdditionalTempData.additionalWinConditions.Add(WinCondition.AdditionalAlivePursuerWin);
                pursuerWin = true;
            }

            if (Pursuer.pursuer != null && !Pursuer.pursuer.Data.IsDead && Lawyer.isProsecutor && !pursuerWin)
            {
                EndGameResult.CachedWinners.Add(new CachedPlayerData(Pursuer.pursuer.Data));
                AdditionalTempData.additionalWinConditions.Add(WinCondition.AdditionalAliveProsecutorWin);
                pursuerWin = true;
            }

            // Possible Additional winner: Lawyer
            if (!lawyerSoloWin && Lawyer.lawyer != null && Lawyer.target != null &&
                (!Lawyer.target.Data.IsDead || Lawyer.target == Jester.jester) && !Lawyer.notAckedExiled && !Lawyer.isProsecutor)
            {
                CachedPlayerData winningClient = null;
                foreach (var winner in EndGameResult.CachedWinners.GetFastEnumerator())
                    if (winner.PlayerName == Lawyer.target.Data.PlayerName)
                        winningClient = winner;
                if (winningClient != null)
                {
                    if (!EndGameResult.CachedWinners.ToArray().Any(x => x.PlayerName == Lawyer.lawyer.Data.PlayerName))
                    {
                        if (!Lawyer.lawyer.Data.IsDead && Lawyer.stolenWin)
                        {
                            EndGameResult.CachedWinners.Remove(winningClient);
                            EndGameResult.CachedWinners.Add(new CachedPlayerData(Lawyer.lawyer.Data));
                            AdditionalTempData.additionalWinConditions.Add(WinCondition.AdditionalLawyerStolenWin); // The Lawyer replaces the client's victory
                        }
                        else
                        {
                            EndGameResult.CachedWinners.Add(new CachedPlayerData(Lawyer.lawyer.Data));
                            AdditionalTempData.additionalWinConditions.Add(WinCondition.AdditionalLawyerBonusWin); // The Lawyer wins with the client
                        }
                    }
                }
            }

            // Cupid wins with both cupid Loverse
            if (Cupid.cupid != null && Cupid.lovers1 != null && Cupid.lovers2 != null)
            {
                if (EndGameResult.CachedWinners.ToArray().Any(x => x.PlayerName == Cupid.lovers1.Data.PlayerName) &&
                    EndGameResult.CachedWinners.ToArray().Any(x => x.PlayerName == Cupid.lovers2.Data.PlayerName) &&
                    !EndGameResult.CachedWinners.ToArray().Any(x => x.PlayerName == Cupid.cupid.Data.PlayerName))
                {
                    EndGameResult.CachedWinners.Add(new CachedPlayerData(Cupid.cupid.Data));
                }
            }

            // Possible Additional winner: Survivor
            if (Survivor.survivor != null && Survivor.survivor.Any(p => !p.Data.IsDead) && !isPursurerLose)
            {
                foreach (var player in Survivor.survivor.Where(p => !p.Data.IsDead))
                {
                    if (!EndGameResult.CachedWinners.ToArray().Any(x => x.PlayerName == player.Data.PlayerName))
                        EndGameResult.CachedWinners.Add(new CachedPlayerData(player.Data));
                }
                AdditionalTempData.additionalWinConditions.Add(WinCondition.AdditionalAliveSurvivorWin);
            }

            AdditionalTempData.timer = ((float)(DateTime.UtcNow - (HideNSeek.isHideNSeekGM ? HideNSeek.startTime : PropHunt.startTime)).TotalMilliseconds) / 1000;

            // Reset Settings
            if (MapOptionsTor.gameMode == CustomGamemodes.HideNSeek) ShipStatusPatch.resetVanillaSettings();
            RPCProcedure.resetVariables();
            EventUtility.gameEndsUpdate();
        }
    }

    [HarmonyPatch(typeof(EndGameManager), nameof(EndGameManager.SetEverythingUp))]
    public class EndGameManagerSetUpPatch
    {
        public static void Postfix(EndGameManager __instance)
        {
            // Delete and readd PoolablePlayers always showing the name and role of the player
            foreach (PoolablePlayer pb in __instance.transform.GetComponentsInChildren<PoolablePlayer>())
            {
                UnityEngine.Object.Destroy(pb.gameObject);
            }
            int num = Mathf.CeilToInt(7.5f);
            List<CachedPlayerData> list = EndGameResult.CachedWinners.ToArray().ToList().OrderBy(delegate (CachedPlayerData b)
            {
                if (!b.IsYou)
                {
                    return 0;
                }
                return -1;
            }).ToList<CachedPlayerData>();
            for (int i = 0; i < list.Count; i++)
            {
                CachedPlayerData CachedPlayerData2 = list[i];
                int num2 = (i % 2 == 0) ? -1 : 1;
                int num3 = (i + 1) / 2;
                float num4 = (float)num3 / (float)num;
                float num5 = Mathf.Lerp(1f, 0.75f, num4);
                float num6 = (float)((i == 0) ? -8 : -1);
                PoolablePlayer poolablePlayer = UnityEngine.Object.Instantiate<PoolablePlayer>(__instance.PlayerPrefab, __instance.transform);
                poolablePlayer.transform.localPosition = new Vector3(1f * (float)num2 * (float)num3 * num5, FloatRange.SpreadToEdges(-1.125f, 0f, num3, num), num6 + (float)num3 * 0.01f) * 0.9f;
                float num7 = Mathf.Lerp(1f, 0.65f, num4) * 0.9f;
                Vector3 vector = new Vector3(num7, num7, 1f);
                poolablePlayer.transform.localScale = vector;
                if (CachedPlayerData2.IsDead)
                {
                    poolablePlayer.SetBodyAsGhost();
                    poolablePlayer.SetDeadFlipX(i % 2 == 0);
                }
                else
                {
                    poolablePlayer.SetFlipX(i % 2 == 0);
                }
                poolablePlayer.UpdateFromPlayerOutfit(CachedPlayerData2.Outfit, PlayerMaterial.MaskType.None, CachedPlayerData2.IsDead, true);

                poolablePlayer.cosmetics.nameText.color = Color.white;
                poolablePlayer.cosmetics.nameText.transform.localScale = new Vector3(1f / vector.x, 1f / vector.y, 1f / vector.z);
                poolablePlayer.cosmetics.nameText.transform.localPosition = new Vector3(poolablePlayer.cosmetics.nameText.transform.localPosition.x, poolablePlayer.cosmetics.nameText.transform.localPosition.y, -15f);
                poolablePlayer.cosmetics.nameText.text = CachedPlayerData2.PlayerName;

                foreach (var data in AdditionalTempData.playerRoles)
                {
                    if (data.PlayerName != CachedPlayerData2.PlayerName) continue;
                    var roles =
                    poolablePlayer.cosmetics.nameText.text += $"\n{string.Join("\n", data.Roles.Select(x => Helpers.cs(x.color, x.name)))}";
                }
            }

            // Additional code
            var bonusText = UnityEngine.Object.Instantiate(__instance.WinText.gameObject);
            var position1 = __instance.WinText.transform.position;
            bonusText.transform.position = new Vector3(position1.x,
                position1.y - 0.5f, position1.z);
            bonusText.transform.localScale = new Vector3(0.7f, 0.7f, 1f);
            var textRenderer = bonusText.GetComponent<TMP_Text>();
            textRenderer.text = "";

            switch (AdditionalTempData.winCondition)
            {
                case WinCondition.JesterWin:
                    textRenderer.text = ModTranslation.GetString("EndGame", 1);
                    textRenderer.color = Jester.color;
                    __instance.BackgroundBar.material.SetColor("_Color", Jester.color);
                    break;
                case WinCondition.DoomsayerWin:
                    textRenderer.text = ModTranslation.GetString("EndGame", 2);
                    textRenderer.color = Doomsayer.color;
                    __instance.BackgroundBar.material.SetColor("_Color", Doomsayer.color);
                    break;
                case WinCondition.ProsecutorWin:
                    textRenderer.text = ModTranslation.GetString("EndGame", 3);
                    textRenderer.color = Lawyer.color;
                    __instance.BackgroundBar.material.SetColor("_Color", Lawyer.color);
                    break;
                case WinCondition.SwooperWin:
                    textRenderer.text = ModTranslation.GetString("EndGame", 4);
                    textRenderer.color = Swooper.color;
                    __instance.BackgroundBar.material.SetColor("_Color", Swooper.color);
                    break;
                case WinCondition.ArsonistWin:
                    textRenderer.text = ModTranslation.GetString("EndGame", 5);
                    textRenderer.color = Arsonist.color;
                    __instance.BackgroundBar.material.SetColor("_Color", Arsonist.color);
                    break;
                case WinCondition.VultureWin:
                    textRenderer.text = ModTranslation.GetString("EndGame", 6);
                    textRenderer.color = Vulture.color;
                    __instance.BackgroundBar.material.SetColor("_Color", Vulture.color);
                    break;
                case WinCondition.LawyerSoloWin:
                    textRenderer.text = ModTranslation.GetString("EndGame", 7);
                    textRenderer.color = Lawyer.color;
                    __instance.BackgroundBar.material.SetColor("_Color", Lawyer.color);
                    break;
                case WinCondition.WerewolfWin:
                    textRenderer.text = ModTranslation.GetString("EndGame", 8);
                    textRenderer.color = Werewolf.color;
                    __instance.BackgroundBar.material.SetColor("_Color", Werewolf.color);
                    break;
                case WinCondition.PlagueDoctorWin:
                    textRenderer.text = ModTranslation.GetString("EndGame", 24);
                    textRenderer.color = PlagueDoctor.color;
                    __instance.BackgroundBar.material.SetColor("_Color", PlagueDoctor.color);
                    break;
                case WinCondition.LoversTeamWin:
                    textRenderer.text = ModTranslation.GetString("EndGame", 9);
                    textRenderer.color = Lovers.color;
                    __instance.BackgroundBar.material.SetColor("_Color", Lovers.color);
                    break;
                case WinCondition.LoversSoloWin:
                    textRenderer.text = ModTranslation.GetString("EndGame", 10);
                    textRenderer.color = Lovers.color;
                    __instance.BackgroundBar.material.SetColor("_Color", Lovers.color);
                    break;
                case WinCondition.JackalWin:
                    textRenderer.text = ModTranslation.GetString("EndGame", 11);
                    textRenderer.color = Jackal.color;
                    __instance.BackgroundBar.material.SetColor("_Color", Jackal.color);
                    break;
                case WinCondition.MiniLose:
                    textRenderer.text = ModTranslation.GetString("EndGame", 12);
                    textRenderer.color = Mini.color;
                    __instance.BackgroundBar.material.SetColor("_Color", Mini.color);
                    break;
                case WinCondition.JuggernautWin:
                    textRenderer.text = ModTranslation.GetString("EndGame", 20);
                    textRenderer.color = Juggernaut.color;
                    __instance.BackgroundBar.material.SetColor("_Color", Juggernaut.color);
                    break;
                case WinCondition.EveryoneDied:
                    textRenderer.text = ModTranslation.GetString("EndGame", 21);
                    textRenderer.color = Palette.DisabledGrey;
                    __instance.BackgroundBar.material.SetColor("_Color", Palette.DisabledGrey);
                    break;
                case WinCondition.ImpostorWin:
                    textRenderer.text = ModTranslation.GetString("EndGame", 22);
                    textRenderer.color = Palette.ImpostorRed;
                    __instance.BackgroundBar.material.SetColor("_Color", Palette.ImpostorRed);
                    break;
                case WinCondition.CrewmateWin:
                    textRenderer.text = ModTranslation.GetString("EndGame", 23);
                    textRenderer.color = Palette.CrewmateBlue;
                    __instance.BackgroundBar.material.SetColor("_Color", Palette.CrewmateBlue);
                    break;
                case WinCondition.CupidLoversWin:
                    textRenderer.text = ModTranslation.GetString("EndGame", 10);
                    textRenderer.color = Lovers.color;
                    __instance.BackgroundBar.material.SetColor("_Color", Lovers.color);
                    break;
            }

            var winConditionsTexts = new List<string>();
            var pursuerAlive = false;
            var survivorAlive = false;
            foreach (WinCondition cond in AdditionalTempData.additionalWinConditions)
            {
                switch (cond)
                {
                    case WinCondition.AdditionalLawyerStolenWin:
                        winConditionsTexts.Add(Helpers.cs(Lawyer.color, ModTranslation.GetString("EndGame", 13)));
                        break;
                    case WinCondition.AdditionalLawyerBonusWin:
                        textRenderer.text += $"\n{Helpers.cs(Lawyer.color, ModTranslation.GetString("EndGame", 14))}";
                        break;
                    case WinCondition.AdditionalAliveProsecutorWin:
                        textRenderer.text += $"\n{Helpers.cs(Lawyer.color, ModTranslation.GetString("EndGame", 25))}";
                        break;
                    case WinCondition.AdditionalAlivePursuerWin:
                        pursuerAlive = true;
                        break;
                    case WinCondition.AdditionalAliveSurvivorWin:
                        survivorAlive = true;
                        break;
                }
            }
            if (pursuerAlive && survivorAlive)
            {
                winConditionsTexts.Add($"{Helpers.cs(Pursuer.color, ModTranslation.GetRoleName(RoleId.Prosecutor).GetString())} & {Helpers.cs(Survivor.color, ModTranslation.GetString("EndGame", 15))}");
            }
            else
            {
                if (pursuerAlive) winConditionsTexts.Add(Helpers.cs(Pursuer.color, ModTranslation.GetString("EndGame", 16)));
                if (survivorAlive) winConditionsTexts.Add(Helpers.cs(Survivor.color, ModTranslation.GetString("EndGame", 15)));
            }

            if (winConditionsTexts.Count == 1)
            {
                textRenderer.text += $"\n{winConditionsTexts[0]}";
            }
            else if (winConditionsTexts.Count > 1)
            {
                var combinedText = string.Join(" & ", winConditionsTexts);
                textRenderer.text += $"\n{combinedText}";
            }

            if (MapOptionsTor.showRoleSummary || HideNSeek.isHideNSeekGM || PropHunt.isPropHuntGM)
            {
                var position = Camera.main.ViewportToWorldPoint(new Vector3(0f, 1f, Camera.main.nearClipPlane));
                GameObject roleSummary = UnityEngine.Object.Instantiate(__instance.WinText.gameObject);
                roleSummary.transform.position = new Vector3(__instance.Navigation.ExitButton.transform.position.x + 0.1f, position.y - 0.1f, -214f);
                roleSummary.transform.localScale = new Vector3(1f, 1f, 1f);

                var roleSummaryText = new StringBuilder();
                if (HideNSeek.isHideNSeekGM || PropHunt.isPropHuntGM)
                {
                    int minutes = (int)AdditionalTempData.timer / 60;
                    int seconds = (int)AdditionalTempData.timer % 60;
                    roleSummaryText.AppendLine($"<color=#FAD934FF>{ModTranslation.GetString("EndGame", 17)} {minutes:00}:{seconds:00}</color> \n");
                }
                roleSummaryText.AppendLine(ModTranslation.GetString("EndGame", 18));
                bool plagueExists = AdditionalTempData.playerRoles.Any(x => x.Roles.Contains(RoleInfo.plagueDoctor));
                foreach (var data in AdditionalTempData.playerRoles)
                {
                    //var roles = string.Join(" ", data.Roles.Select(x => Helpers.cs(x.color, x.name)));
                    string roles = data.RoleNames;
                    //if (data.IsGuesser) roles += " (Guesser)";
                    if (data.IsGuesser) roles += " (Guesser)";
                    var taskInfo = data.TasksTotal > 0 ? $" - <color=#FAD934FF>({data.TasksCompleted}/{data.TasksTotal})</color>" : "";
                    if (data.Kills != null) taskInfo += $" - <color=#FF0000FF>({ModTranslation.GetString("EndGame", 19)} {data.Kills})</color>";
                    string infectionInfo = "";
                    if (plagueExists && !data.Roles.Contains(RoleInfo.plagueDoctor))
                    {
                        if (AdditionalTempData.plagueDoctorInfected.ContainsKey(data.PlayerId))
                        {
                            infectionInfo += " - " + Helpers.cs(Color.red, ModTranslation.GetString("Game-PlagueDoctor", 1));
                        }
                        else
                        {
                            float progress = AdditionalTempData.plagueDoctorProgress.ContainsKey(data.PlayerId) ? AdditionalTempData.plagueDoctorProgress[data.PlayerId] : 0f;
                            infectionInfo += " - " + PlagueDoctor.getProgressString(progress);
                        }
                    }
                    roleSummaryText.AppendLine($"{Helpers.cs(data.IsAlive ? Color.white : new Color(.7f, .7f, .7f), data.PlayerName)} - {roles}{taskInfo}");
                }
                TMPro.TMP_Text roleSummaryTextMesh = roleSummary.GetComponent<TMPro.TMP_Text>();
                roleSummaryTextMesh.alignment = TMPro.TextAlignmentOptions.TopLeft;
                roleSummaryTextMesh.color = Color.white;
                roleSummaryTextMesh.fontSizeMin = 1.5f;
                roleSummaryTextMesh.fontSizeMax = 1.5f;
                roleSummaryTextMesh.fontSize = 1.5f;

                var roleSummaryTextMeshRectTransform = roleSummaryTextMesh.GetComponent<RectTransform>();
                roleSummaryTextMeshRectTransform.anchoredPosition = new Vector2(position.x + 3.5f, position.y - 0.1f);
                roleSummaryTextMesh.text = roleSummaryText.ToString();
            }
            AdditionalTempData.clear();
        }
    }

    [HarmonyPatch(typeof(LogicGameFlowNormal), nameof(LogicGameFlowNormal.CheckEndCriteria))]
    class CheckEndCriteriaPatch
    {
        public static bool Prefix(ShipStatus __instance)
        {
            if (!GameData.Instance) return false;
            if (DestroyableSingleton<TutorialManager>.InstanceExists) // InstanceExists | Don't check Custom Criteria when in Tutorial
                return true;
            var statistics = new PlayerStatistics(__instance);
            if (CheckAndEndGameForDoomsayerWin(__instance)) return false;
            if (CheckAndEndGameForMiniLose(__instance)) return false;
            if (CheckAndEndGameForJesterWin(__instance)) return false;
            if (CheckAndEndGameForProsecutorWin(__instance)) return false;
            if (CheckAndEndGameForJuggernautWin(__instance, statistics)) return false;
            if (CheckAndEndGameForPlagueDoctorWin(__instance)) return false;
            if (CheckAndEndGameForWerewolfWin(__instance, statistics)) return false;
            if (CheckAndEndGameForArsonistWin(__instance)) return false;
            if (CheckAndEndGameForVultureWin(__instance)) return false;
            if (CheckAndEndGameForSabotageWin(__instance)) return false;
            if (CheckAndEndGameForTaskWin(__instance)) return false;
            if (CheckAndEndGameForLoverWin(__instance, statistics)) return false;
            if (CheckAndEndGameForJackalWin(__instance, statistics)) return false;
            if (CheckAndEndGameForSwooperWin(__instance, statistics)) return false;
            if (CheckAndEndGameForImpostorWin(__instance, statistics)) return false;
            if (CheckAndEndGameForCrewmateWin(__instance, statistics)) return false;
            if (CheckAndEndGameForCupidLoversWin(__instance, statistics)) return false;
            return false;
        }
        private static bool CheckAndEndGameForDoomsayerWin(ShipStatus __instance)
        {
            if (Doomsayer.triggerDoomsayerrWin)
            {
                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.DoomsayerWin, false);
                return true;
            }
            return false;
        }

        private static bool CheckAndEndGameForMiniLose(ShipStatus __instance)
        {
            if (Mini.triggerMiniLose)
            {
                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.MiniLose, false);
                return true;
            }
            return false;
        }

        private static bool CheckAndEndGameForJesterWin(ShipStatus __instance)
        {
            if (Jester.triggerJesterWin)
            {
                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.JesterWin, false);
                return true;
            }
            return false;
        }

        private static bool CheckAndEndGameForProsecutorWin(ShipStatus __instance)
        {
            if (Lawyer.triggerProsecutorWin)
            {
                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.ProsecutorWin, false);
                return true;
            }
            return false;
        }

        private static bool CheckAndEndGameForCupidLoversWin(ShipStatus __instance, PlayerStatistics statistics)
        {
            if (statistics.TeamCupidLoversAlive == 2 && statistics.TotalAlive <= 3)
            {
                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.CupidLoversWin, false);
                return true;
            }
            return false;
        }

        private static bool CheckAndEndGameForArsonistWin(ShipStatus __instance)
        {
            if (Arsonist.triggerArsonistWin)
            {
                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.ArsonistWin, false);
                return true;
            }
            return false;
        }

        private static bool CheckAndEndGameForPlagueDoctorWin(ShipStatus __instance)
        {
            if (PlagueDoctor.triggerPlagueDoctorWin)
            {
                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.PlagueDoctorWin, false);
                return true;
            }
            return false;
        }

        private static bool CheckAndEndGameForVultureWin(ShipStatus __instance)
        {
            if (Vulture.triggerVultureWin)
            {
                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.VultureWin, false);
                return true;
            }
            return false;
        }

        private static bool CheckAndEndGameForSabotageWin(ShipStatus __instance)
        {
            if (MapUtilities.Systems == null) return false;
            var systemType = MapUtilities.Systems.ContainsKey(SystemTypes.LifeSupp) ? MapUtilities.Systems[SystemTypes.LifeSupp] : null;
            if (systemType != null)
            {
                LifeSuppSystemType lifeSuppSystemType = systemType.TryCast<LifeSuppSystemType>();
                if (lifeSuppSystemType != null && lifeSuppSystemType.Countdown < 0f)
                {
                    EndGameForSabotage(__instance);
                    lifeSuppSystemType.Countdown = 10000f;
                    return true;
                }
            }
            var systemType2 = MapUtilities.Systems.ContainsKey(SystemTypes.Reactor) ? MapUtilities.Systems[SystemTypes.Reactor] : null;
            if (systemType2 == null)
            {
                systemType2 = MapUtilities.Systems.ContainsKey(SystemTypes.Laboratory) ? MapUtilities.Systems[SystemTypes.Laboratory] : null;
            }
            if (systemType2 != null)
            {
                ICriticalSabotage criticalSystem = systemType2.TryCast<ICriticalSabotage>();
                if (criticalSystem != null && criticalSystem.Countdown < 0f)
                {
                    EndGameForSabotage(__instance);
                    criticalSystem.ClearSabotage();
                    return true;
                }
            }
            return false;
        }

        private static bool CheckAndEndGameForTaskWin(ShipStatus __instance)
        {
            if (HideNSeek.isHideNSeekGM && !HideNSeek.taskWinPossible || PropHunt.isPropHuntGM) return false;
            if (GameData.Instance.TotalTasks > 0 && GameData.Instance.TotalTasks <= GameData.Instance.CompletedTasks)
            {
                GameManager.Instance.RpcEndGame(GameOverReason.HumansByTask, false);
                return true;
            }
            return false;
        }


        private static bool CheckAndEndGameForLoverWin(ShipStatus __instance, PlayerStatistics statistics)
        {
            if (statistics.TeamLoversAlive == 2 && statistics.TotalAlive <= 3)
            {
                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.LoversWin, false);
                return true;
            }
            return false;
        }

        private static bool CheckAndEndGameForJackalWin(ShipStatus __instance, PlayerStatistics statistics)
        {
            if (statistics.TeamJackalAlive >= statistics.TotalAlive - statistics.TeamJackalAlive && statistics.TeamImpostorsAlive == 0 && statistics.TeamJuggernautAlive == 0 && (statistics.TeamSwooperAlive == 0 || statistics.TeamCupidLoversAlive == 2 || Swooper.swooper == Jackal.jackal) && statistics.TeamWerewolfAlive == 0 && !(statistics.TeamJackalHasAliveLover && statistics.TeamLoversAlive == 2) && !Helpers.killingCrewAlive())
            {
                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.TeamJackalWin, false);
                return true;
            }
            return false;
        }

        private static bool CheckAndEndGameForSwooperWin(ShipStatus __instance, PlayerStatistics statistics)
        {
            if (Swooper.swooper == Jackal.jackal) return false;
            if (statistics.TeamSwooperAlive >= statistics.TotalAlive - statistics.TeamSwooperAlive && statistics.TeamImpostorsAlive == 0 && statistics.TeamJackalAlive == 0 && statistics.TeamWerewolfAlive == 0 && !(statistics.TeamSwooperHasAliveLover && statistics.TeamLoversAlive == 2) && !Helpers.killingCrewAlive())
            {
                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.SwooperWin, false);
                return true;
            }
            return false;
        }

        private static bool CheckAndEndGameForWerewolfWin(ShipStatus __instance, PlayerStatistics statistics)
        {
            if (
                statistics.TeamWerewolfAlive >= statistics.TotalAlive - statistics.TeamWerewolfAlive &&
                statistics.TeamImpostorsAlive == 0 &&
                statistics.TeamJackalAlive == 0 &&
                statistics.TeamSwooperAlive == 0 &&
                statistics.TeamJuggernautAlive == 0 &&
                !(statistics.TeamWerewolfHasAliveLover && statistics.TeamLoversAlive == 2) &&
                !Helpers.killingCrewAlive()
            )
            {
                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.WerewolfWin, false);
                return true;
            }
            return false;
        }

        private static bool CheckAndEndGameForJuggernautWin(ShipStatus __instance, PlayerStatistics statistics)
        {
            if (
                statistics.TeamJuggernautAlive >= statistics.TotalAlive - statistics.TeamJuggernautAlive &&
                statistics.TeamImpostorsAlive == 0 &&
                statistics.TeamJackalAlive == 0 &&
                statistics.TeamWerewolfAlive == 0 &&
                //statistics.TeamSerialKillerAlive == 0 &&
                !(statistics.TeamJuggernautHasAliveLover && statistics.TeamLoversAlive == 2)
            )
            {
                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.JuggernautWin, false);
                return true;
            }
            return false;
        }

        private static bool CheckAndEndGameForImpostorWin(ShipStatus __instance, PlayerStatistics statistics)
        {
            if (HideNSeek.isHideNSeekGM || PropHunt.isPropHuntGM)
                if ((0 != statistics.TotalAlive - statistics.TeamImpostorsAlive)) return false;
            if (statistics.TeamImpostorsAlive >= statistics.TotalAlive - statistics.TeamImpostorsAlive && statistics.TeamJackalAlive == 0 && statistics.TeamJuggernautAlive == 0 && statistics.TeamSwooperAlive == 0 && statistics.TeamWerewolfAlive == 0 && !(statistics.TeamImpostorHasAliveLover && statistics.TeamLoversAlive == 2) && !Helpers.killingCrewAlive())
            {
                GameOverReason endReason;
                switch (GameData.LastDeathReason)
                {
                    case DeathReason.Exile:
                        endReason = GameOverReason.ImpostorByVote;
                        break;
                    case DeathReason.Kill:
                        endReason = GameOverReason.ImpostorByKill;
                        break;
                    default:
                        endReason = GameOverReason.ImpostorByVote;
                        break;
                }
                GameManager.Instance.RpcEndGame(endReason, false);
                return true;
            }
            return false;
        }

        private static bool CheckAndEndGameForCrewmateWin(ShipStatus __instance, PlayerStatistics statistics)
        {
            if (HideNSeek.isHideNSeekGM && HideNSeek.timer <= 0 && !HideNSeek.isWaitingTimer)
            {
                GameManager.Instance.RpcEndGame(GameOverReason.HumansByVote, false);
                return true;
            }
            if (PropHunt.isPropHuntGM && PropHunt.timer <= 0 && PropHunt.timerRunning)
            {
                GameManager.Instance.RpcEndGame(GameOverReason.HumansByVote, false);
                return true;
            }
            if (statistics.TeamImpostorsAlive == 0 && statistics.TeamJackalAlive == 0 && statistics.TeamSwooperAlive == 0 && statistics.TeamJuggernautAlive == 0 && statistics.TeamWerewolfAlive == 0)
            {
                GameManager.Instance.RpcEndGame(GameOverReason.HumansByVote, false);
                return true;
            }
            return false;
        }

        private static void EndGameForSabotage(ShipStatus __instance)
        {
            GameManager.Instance.RpcEndGame(GameOverReason.ImpostorBySabotage, false);
            return;
        }

    }

    internal class PlayerStatistics
    {
        public int TeamImpostorsAlive { get; set; }
        public int TeamJackalAlive { get; set; }
        public int TeamLoversAlive { get; set; }
        public int TotalAlive { get; set; }
        public bool TeamImpostorHasAliveLover { get; set; }
        public bool TeamJackalHasAliveLover { get; set; }

        public int TeamSwooperAlive { get; set; }
        public bool TeamSwooperHasAliveLover { get; set; }

        public int TeamWerewolfAlive { get; set; }
        public bool TeamWerewolfHasAliveLover { get; set; }

        public int TeamJuggernautAlive { get; set; }
        public bool TeamJuggernautHasAliveLover { get; set; }

        public int TeamCupidLoversAlive { get; set; }

        public PlayerStatistics(ShipStatus __instance)
        {
            GetPlayerCounts();
        }

        private bool isLover(NetworkedPlayerInfo p)
        {
            return (Lovers.lover1 != null && Lovers.lover1.PlayerId == p.PlayerId) || (Lovers.lover2 != null && Lovers.lover2.PlayerId == p.PlayerId);
        }

        private bool isCupidLover(NetworkedPlayerInfo p)
        {
            return (Cupid.lovers1 != null && Cupid.lovers1.PlayerId == p.PlayerId) || (Cupid.lovers2 != null && Cupid.lovers2.PlayerId == p.PlayerId);
        }

        private void GetPlayerCounts()
        {
            int numJackalAlive = 0;
            int numImpostorsAlive = 0;
            int numLoversAlive = 0;
            int numTotalAlive = 0;
            bool impLover = false;
            bool jackalLover = false;

            int numSwooperAlive = 0;
            bool swooperLover = false;

            int numWerewolfAlive = 0;
            bool werewolfLover = false;

            int numJuggernautAlive = 0;
            bool juggernautLover = false;

            int numCupidLoversAlive = 0;


            foreach (var playerInfo in GameData.Instance.AllPlayers.GetFastEnumerator())
            {
                if (!playerInfo.Disconnected)
                {
                    if (!playerInfo.IsDead)
                    {
                        numTotalAlive++;

                        bool lover = isLover(playerInfo);
                        bool cupidLover = isCupidLover(playerInfo);
                        if (lover) numLoversAlive++;
                        if (cupidLover) numCupidLoversAlive++;

                        if (playerInfo.Role.IsImpostor)
                        {
                            numImpostorsAlive++;
                            if (lover || cupidLover) impLover = true;
                        }
                        if (Jackal.jackal != null && Jackal.jackal.PlayerId == playerInfo.PlayerId)
                        {
                            numJackalAlive++;
                            if (lover || cupidLover) jackalLover = true;
                        }
                        if (Sidekick.sidekick != null && Sidekick.sidekick.PlayerId == playerInfo.PlayerId)
                        {
                            numJackalAlive++;
                            if (lover || cupidLover) jackalLover = true;
                        }

                        if (Swooper.swooper != null && Swooper.swooper.PlayerId == playerInfo.PlayerId)
                        {
                            numSwooperAlive++;
                            if (lover) swooperLover = true;
                        }

                        if (Werewolf.werewolf != null && Werewolf.werewolf.PlayerId == playerInfo.PlayerId)
                        {
                            numWerewolfAlive++;
                            if (lover) werewolfLover = true;
                        }

                        if (Juggernaut.juggernaut != null && Juggernaut.juggernaut.PlayerId == playerInfo.PlayerId)
                        {
                            numJuggernautAlive++;
                            if (lover) juggernautLover = true;
                        }
                    }
                }
            }

            TeamJackalAlive = numJackalAlive;
            TeamImpostorsAlive = numImpostorsAlive;
            TeamLoversAlive = numLoversAlive;
            TotalAlive = numTotalAlive;
            TeamImpostorHasAliveLover = impLover;
            TeamJackalHasAliveLover = jackalLover;

            TeamSwooperHasAliveLover = swooperLover;
            TeamSwooperAlive = numSwooperAlive;

            TeamWerewolfHasAliveLover = werewolfLover;
            TeamWerewolfAlive = numWerewolfAlive;

            TeamJuggernautAlive = numJuggernautAlive;
            TeamJuggernautHasAliveLover = juggernautLover;

            TeamCupidLoversAlive = numCupidLoversAlive;
        }
    }
}
