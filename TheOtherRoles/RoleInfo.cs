using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using TheOtherRoles.Players;
using TheOtherRoles.Utilities;
using UnityEngine;
using static TheOtherRoles.TheOtherRoles;

namespace TheOtherRoles
{
    public class RoleInfo
    {
        public Color color { get; private set; }
        public RoleId roleId { get; private set; }
        public RoleTeam roleTeam { get; private set; }
        public bool isNeutral { get; private set; }
        public bool isModifier { get; private set; }
        public string name => name_ != null ? name_.GetString() : "";
        public string introDescription => introDescription_ != null ? introDescription_.GetString() : "";
        public string shortDescription => shortDescription_ != null ? shortDescription_.GetString() : "";

        public RoleInfo(Color color, RoleId roleId, RoleTeam roleTeam, bool isNeutral = false, bool isModifier = false, bool isGuessable = false, TranslationInfo name = null, TranslationInfo introDescription = null, TranslationInfo shortDescription = null)
        {
            this.color = color;
            this.name_ = name ?? ModTranslation.GetRoleName(roleId, color);
            this.introDescription_ = introDescription ?? ModTranslation.GetRoleIntroDesc(roleId, color);
            this.shortDescription_ = shortDescription ?? ModTranslation.GetRoleShortDesc(roleId, color);
            this.roleId = roleId;
            this.isNeutral = isNeutral;
            this.isModifier = isModifier;
        }

        TranslationInfo name_ = null;
        TranslationInfo introDescription_ = null;
        TranslationInfo shortDescription_ = null;

        public static RoleInfo jester = new RoleInfo(Jester.color, RoleId.Jester, RoleTeam.Neutral, true);
        public static RoleInfo werewolf = new RoleInfo(Werewolf.color, RoleId.Werewolf, RoleTeam.Neutral, true);
        public static RoleInfo prosecutor = new RoleInfo(Lawyer.color, RoleId.Prosecutor, RoleTeam.Neutral, true);
        public static RoleInfo swooper = new RoleInfo(Swooper.color, RoleId.Swooper, RoleTeam.Neutral, true);
        public static RoleInfo mayor = new RoleInfo(Mayor.color, RoleId.Mayor, RoleTeam.Crewmate);
        public static RoleInfo portalmaker = new RoleInfo(Portalmaker.color, RoleId.Portalmaker, RoleTeam.Crewmate);
        public static RoleInfo engineer = new RoleInfo(Engineer.color, RoleId.Engineer, RoleTeam.Crewmate);
        public static RoleInfo privateInvestigator = new RoleInfo(PrivateInvestigator.color, RoleId.PrivateInvestigator, RoleTeam.Crewmate);
        public static RoleInfo sheriff = new RoleInfo(Sheriff.color, RoleId.Sheriff, RoleTeam.Crewmate);
        public static RoleInfo bodyguard = new RoleInfo(BodyGuard.color, RoleId.BodyGuard, RoleTeam.Crewmate, false);
        public static RoleInfo deputy = new RoleInfo(Sheriff.color, RoleId.Deputy, RoleTeam.Crewmate);
        public static RoleInfo lighter = new RoleInfo(Lighter.color, RoleId.Lighter, RoleTeam.Crewmate);
        public static RoleInfo godfather = new RoleInfo(Godfather.color, RoleId.Godfather, RoleTeam.Impostor);
        public static RoleInfo mafioso = new RoleInfo(Mafioso.color, RoleId.Mafioso, RoleTeam.Impostor);
        public static RoleInfo janitor = new RoleInfo(Janitor.color, RoleId.Janitor, RoleTeam.Impostor);
        public static RoleInfo morphling = new RoleInfo(Morphling.color, RoleId.Morphling, RoleTeam.Impostor);
        public static RoleInfo bomber = new RoleInfo(Bomber.color, RoleId.Bomber, RoleTeam.Impostor);
        public static RoleInfo bomber2 = new RoleInfo(Bomber2.color, RoleId.Bomber2, RoleTeam.Impostor);
        public static RoleInfo yoyo = new RoleInfo(Yoyo.color, RoleId.Yoyo, RoleTeam.Impostor);
        public static RoleInfo poucher = new RoleInfo(Poucher.color, RoleId.Poucher, RoleTeam.Impostor);
        public static RoleInfo mimic = new RoleInfo(Mimic.color, RoleId.Mimic, RoleTeam.Impostor);
        public static RoleInfo camouflager = new RoleInfo(Camouflager.color, RoleId.Camouflager, RoleTeam.Impostor);
        public static RoleInfo miner = new RoleInfo(Miner.color, RoleId.Miner, RoleTeam.Impostor);
        public static RoleInfo vampire = new RoleInfo(Vampire.color, RoleId.Vampire, RoleTeam.Impostor);
        public static RoleInfo eraser = new RoleInfo(Eraser.color, RoleId.Eraser, RoleTeam.Impostor);
        public static RoleInfo trickster = new RoleInfo(Trickster.color, RoleId.Trickster, RoleTeam.Impostor);
        public static RoleInfo cleaner = new RoleInfo(Cleaner.color, RoleId.Cleaner, RoleTeam.Impostor);
        public static RoleInfo undertaker = new RoleInfo(Undertaker.color, RoleId.Undertaker, RoleTeam.Impostor);
        public static RoleInfo warlock = new RoleInfo(Warlock.color, RoleId.Warlock, RoleTeam.Impostor);
        public static RoleInfo bountyHunter = new RoleInfo(BountyHunter.color, RoleId.BountyHunter, RoleTeam.Impostor);
        public static RoleInfo detective = new RoleInfo(Detective.color, RoleId.Detective, RoleTeam.Crewmate);
        public static RoleInfo timeMaster = new RoleInfo(TimeMaster.color, RoleId.TimeMaster, RoleTeam.Crewmate);
        public static RoleInfo veteran = new RoleInfo(Veteran.color, RoleId.Veteran, RoleTeam.Crewmate);
        public static RoleInfo medic = new RoleInfo(Medic.color, RoleId.Medic, RoleTeam.Crewmate);
        public static RoleInfo swapper = new RoleInfo(Swapper.color, RoleId.Swapper, RoleTeam.Crewmate);
        public static RoleInfo seer = new RoleInfo(Seer.color, RoleId.Seer, RoleTeam.Crewmate);
        public static RoleInfo hacker = new RoleInfo(Hacker.color, RoleId.Hacker, RoleTeam.Crewmate);
        public static RoleInfo tracker = new RoleInfo(Tracker.color, RoleId.Tracker, RoleTeam.Crewmate);
        public static RoleInfo snitch = new RoleInfo(Snitch.color, RoleId.Snitch, RoleTeam.Crewmate);
        public static RoleInfo jackal = new RoleInfo(Jackal.color, RoleId.Jackal, RoleTeam.Neutral, true);
        public static RoleInfo sidekick = new RoleInfo(Sidekick.color, RoleId.Sidekick, RoleTeam.Neutral, true);
        public static RoleInfo spy = new RoleInfo(Spy.color, RoleId.Spy, RoleTeam.Crewmate);
        public static RoleInfo securityGuard = new RoleInfo(SecurityGuard.color, RoleId.SecurityGuard, RoleTeam.Crewmate);
        public static RoleInfo arsonist = new RoleInfo(Arsonist.color, RoleId.Arsonist, RoleTeam.Neutral, true);
        public static RoleInfo amnisiac = new RoleInfo(Amnisiac.color, RoleId.Amnisiac, RoleTeam.Neutral, true);
        public static RoleInfo vulture = new RoleInfo(Vulture.color, RoleId.Vulture, RoleTeam.Neutral, true);
        public static RoleInfo medium = new RoleInfo(Medium.color, RoleId.Medium, RoleTeam.Crewmate);
        public static RoleInfo trapper = new RoleInfo(Trapper.color, RoleId.Trapper, RoleTeam.Crewmate);
        public static RoleInfo lawyer = new RoleInfo(Lawyer.color, RoleId.Lawyer, RoleTeam.Neutral, true);
        // public static RoleInfo prosecutor = new RoleInfo("Prosecutor", Lawyer.color, "Vote out your target", "Vote our your target", RoleId.Prosecutor, true);
        public static RoleInfo pursuer = new RoleInfo(Pursuer.color, RoleId.Pursuer, RoleTeam.Neutral);
        public static RoleInfo impostor = new RoleInfo(Palette.ImpostorRed, RoleId.Impostor, RoleTeam.Impostor);
        public static RoleInfo crewmate = new RoleInfo(Palette.CrewmateBlue, RoleId.Crewmate, RoleTeam.Crewmate);
        public static RoleInfo witch = new RoleInfo(Witch.color, RoleId.Witch, RoleTeam.Impostor);
        public static RoleInfo cultist = new RoleInfo(Cultist.color, RoleId.Cultist, RoleTeam.Impostor);
        public static RoleInfo ninja = new RoleInfo(Ninja.color, RoleId.Ninja, RoleTeam.Impostor);
        public static RoleInfo blackmailer = new RoleInfo(Blackmailer.color, RoleId.Blackmailer, RoleTeam.Impostor);
        public static RoleInfo thief = new RoleInfo(Thief.color, RoleId.Thief, RoleTeam.Neutral, true);
        public static RoleInfo doomsayer = new RoleInfo(Doomsayer.color, RoleId.Doomsayer, RoleTeam.Neutral, true);
        public static RoleInfo survivor = new RoleInfo(Survivor.color, RoleId.Survivor, RoleTeam.Neutral, true);
        public static RoleInfo juggernaut = new RoleInfo(Juggernaut.color, RoleId.Juggernaut, RoleTeam.Neutral, true);
        public static RoleInfo plagueDoctor = new RoleInfo(PlagueDoctor.color, RoleId.PlagueDoctor, RoleTeam.Neutral, true);
        public static RoleInfo cupid = new RoleInfo(Cupid.color, RoleId.Cupid, RoleTeam.Neutral, true);

        public static RoleInfo hunter = new RoleInfo(Palette.ImpostorRed, RoleId.Impostor, RoleTeam.Impostor);
        public static RoleInfo hunted = new RoleInfo(Palette.CrewmateBlue, RoleId.Crewmate, RoleTeam.Crewmate);

        public static RoleInfo prop = new RoleInfo(Palette.CrewmateBlue, RoleId.Crewmate, RoleTeam.Crewmate);



        // Modifier
        public static RoleInfo bloody = new RoleInfo(Color.yellow, RoleId.Bloody, RoleTeam.Modifier, false, true);
        public static RoleInfo antiTeleport = new RoleInfo(Color.yellow, RoleId.AntiTeleport, RoleTeam.Modifier, false, true);
        public static RoleInfo tiebreaker = new RoleInfo(Color.yellow, RoleId.Tiebreaker, RoleTeam.Modifier, false, true);
        public static RoleInfo bait = new RoleInfo(Color.yellow, RoleId.Bait, RoleTeam.Modifier, false, true);
        public static RoleInfo sunglasses = new RoleInfo(Color.yellow, RoleId.Sunglasses, RoleTeam.Modifier, false, true);
        public static RoleInfo lover = new RoleInfo(Lovers.color, RoleId.Lover, RoleTeam.Modifier, false, true);
        public static RoleInfo mini = new RoleInfo(Color.yellow, RoleId.Mini, RoleTeam.Modifier, false, true);
        public static RoleInfo vip = new RoleInfo(Color.yellow, RoleId.Vip, RoleTeam.Modifier, false, true);
        public static RoleInfo indomitable = new RoleInfo(Color.yellow, RoleId.Indomitable, RoleTeam.Modifier, false, true);
        public static RoleInfo slueth = new RoleInfo(Color.yellow, RoleId.Slueth, RoleTeam.Modifier, false, true);
        public static RoleInfo cursed = new RoleInfo(Color.yellow, RoleId.Cursed, RoleTeam.Modifier, false, true, true);
        public static RoleInfo invert = new RoleInfo(Color.yellow, RoleId.Invert, RoleTeam.Modifier, false, true);
        public static RoleInfo blind = new RoleInfo(Color.yellow, RoleId.Blind, RoleTeam.Modifier, false, true);
        public static RoleInfo tunneler = new RoleInfo(Color.yellow, RoleId.Tunneler, RoleTeam.Modifier, false, true);
        public static RoleInfo goodGuesser = new RoleInfo(Color.yellow, RoleId.NiceGuesser, RoleTeam.Modifier, false, true);
        public static RoleInfo paranoid = new RoleInfo(Color.yellow, RoleId.Paranoid, RoleTeam.Modifier, false, true);
        public static RoleInfo disperser = new RoleInfo(Color.red, RoleId.Disperser, RoleTeam.Modifier, false, true);
        public static RoleInfo badGuesser = new RoleInfo(Color.yellow, RoleId.EvilGuesser, RoleTeam.Modifier, false, true);
        public static RoleInfo chameleon = new RoleInfo(Color.yellow, RoleId.Chameleon, RoleTeam.Modifier, false, true);
        public static RoleInfo shifter = new RoleInfo(Color.yellow, RoleId.Shifter, RoleTeam.Modifier, false, true);
        public static RoleInfo radar = new RoleInfo(Color.yellow, RoleId.Radar, RoleTeam.Modifier, false, true);
        public static RoleInfo cupidLover = new RoleInfo(Cupid.color, RoleId.Lover, RoleTeam.Modifier, false, true);
        public static RoleInfo torch = new RoleInfo(Color.yellow, RoleId.Torch, RoleTeam.Modifier, false, true);


        public static List<RoleInfo> allRoleInfos = new List<RoleInfo>() {
            impostor,
            godfather,
            mafioso,
            janitor,
            morphling,
            bomber,
            yoyo,
            camouflager,
            vampire,
            eraser,
            trickster,
            cleaner,
            undertaker,
            warlock,
            werewolf,
            cursed,
            bountyHunter,
            witch,
            ninja,
            bomber2,
            bodyguard,
            blackmailer,
            miner,
            swooper,
            goodGuesser,
            privateInvestigator,
            mimic,
            poucher,
            badGuesser,
            lover,
            jester,
            prosecutor,
            arsonist,
            jackal,
            sidekick,
            survivor,
            cupid,
            vulture,
            pursuer,
            lawyer,
            thief,
            crewmate,
            mayor,
            portalmaker,
            engineer,
            sheriff,
            deputy,
            lighter,
            detective,
            timeMaster,
            amnisiac,
            veteran,
            medic,
            swapper,
            seer,
            hacker,
            tracker,
            snitch,
            spy,
            securityGuard,
            bait,
            medium,
            trapper,
            bloody,
            antiTeleport,
            tiebreaker,
            sunglasses,
            mini,
            vip,
            indomitable,
            slueth,
            blind,
            tunneler,
            paranoid,
            invert,
            chameleon,
            shifter,
            disperser,
            juggernaut,
            plagueDoctor,
            cupidLover,
            doomsayer,
            radar,
            torch
};

        public static List<RoleInfo> getRoleInfoForPlayer(PlayerControl p, bool showModifier = true, bool onlyMods = false)
        {
            List<RoleInfo> infos = new List<RoleInfo>();
            if (p == null) return infos;

            // Modifier
            if (showModifier)
            {
                // after dead modifier
                if (!CustomOptionHolder.modifiersAreHidden.getBool() || PlayerControl.LocalPlayer.Data.IsDead || AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Ended)
                {
                    if (Bait.bait.Any(x => x.PlayerId == p.PlayerId)) infos.Add(bait);
                    if (Bloody.bloody.Any(x => x.PlayerId == p.PlayerId)) infos.Add(bloody);
                    if (Vip.vip.Any(x => x.PlayerId == p.PlayerId)) infos.Add(vip);
                    if (p == Tiebreaker.tiebreaker) infos.Add(tiebreaker);
                    if (p == Indomitable.indomitable) infos.Add(indomitable);
                }
                if (PlayerControl.LocalPlayer.Data.IsDead)
                {
                    if (p == Cursed.cursed) infos.Add(cursed);
                }
                if (p == Lovers.lover1 || p == Lovers.lover2) infos.Add(lover);
                if (AntiTeleport.antiTeleport.Any(x => x.PlayerId == p.PlayerId)) infos.Add(antiTeleport);
                if (Sunglasses.sunglasses.Any(x => x.PlayerId == p.PlayerId)) infos.Add(sunglasses);
                if (p == Mini.mini) infos.Add(mini);
                if (p == Blind.blind) infos.Add(blind);
                if (p == Tunneler.tunneler) infos.Add(tunneler);
                if (p == Paranoid.paranoid) infos.Add(paranoid);
                if (p == Slueth.slueth) infos.Add(slueth);
                if (Cupid.lovers1 != null && Cupid.lovers2 != null && (p == Cupid.lovers2 || p == Cupid.lovers1)) infos.Add(cupidLover);
                if (Invert.invert.Any(x => x.PlayerId == p.PlayerId)) infos.Add(invert);
                if (p == Guesser.niceGuesser) infos.Add(goodGuesser);
                if (p == Guesser.evilGuesser) infos.Add(badGuesser);
                if (p == Radar.radar) infos.Add(radar);
                if (Torch.torch.Any(x => x.PlayerId == p.PlayerId)) infos.Add(torch);
                if (p == Shifter.shifter) infos.Add(shifter);
            }
            if (onlyMods) return infos;

            int count = infos.Count;  // Save count after modifiers are added so that the role count can be checked

            // Special roles
            if (p == Jester.jester) infos.Add(jester);
            if (p == Werewolf.werewolf) infos.Add(werewolf);
            //if (p == Prosecutor.prosecutor) infos.Add(prosecutor);
            if (p == Swooper.swooper) infos.Add(swooper);
            if (p == Disperser.disperser) infos.Add(disperser);
            if (p == Mayor.mayor) infos.Add(mayor);
            if (p == Portalmaker.portalmaker) infos.Add(portalmaker);
            if (p == Engineer.engineer) infos.Add(engineer);
            if (p == Sheriff.sheriff || p == Sheriff.formerSheriff) infos.Add(sheriff);
            if (p == Deputy.deputy) infos.Add(deputy);
            if (p == Lighter.lighter) infos.Add(lighter);
            if (p == Godfather.godfather) infos.Add(godfather);
            if (p == Miner.miner) infos.Add(miner);
            if (p == Mafioso.mafioso) infos.Add(mafioso);
            if (p == Janitor.janitor) infos.Add(janitor);
            if (p == Morphling.morphling) infos.Add(morphling);
            if (p == Camouflager.camouflager) infos.Add(camouflager);
            if (p == Vampire.vampire) infos.Add(vampire);
            if (p == Eraser.eraser) infos.Add(eraser);
            if (p == Trickster.trickster) infos.Add(trickster);
            if (p == Cleaner.cleaner) infos.Add(cleaner);
            if (p == Undertaker.undertaker) infos.Add(undertaker);
            if (p == Bomber2.bomber) infos.Add(bomber2);
            if (p == Bomber.bomber) infos.Add(bomber);
            if (p == Yoyo.yoyo) infos.Add(yoyo);
            if (p == Mimic.mimic) infos.Add(mimic);
            if (p == Poucher.poucher) infos.Add(poucher);
            if (p == PrivateInvestigator.privateInvestigator) infos.Add(privateInvestigator);
            if (p == Warlock.warlock) infos.Add(warlock);
            if (p == Witch.witch) infos.Add(witch);
            if (p == Ninja.ninja) infos.Add(ninja);
            if (p == Cupid.cupid) infos.Add(cupid);
            if (p == Blackmailer.blackmailer) infos.Add(blackmailer);
            if (p == Detective.detective) infos.Add(detective);
            if (p == TimeMaster.timeMaster) infos.Add(timeMaster);
            if (p == Cultist.cultist) infos.Add(cultist);
            if (p == Amnisiac.amnisiac) infos.Add(amnisiac);
            if (p == Veteran.veteran) infos.Add(veteran);
            if (p == Medic.medic) infos.Add(medic);
            if (p == Swapper.swapper) infos.Add(swapper);
            if (p == BodyGuard.bodyguard) infos.Add(bodyguard);
            if (p == Seer.seer) infos.Add(seer);
            if (p == Hacker.hacker) infos.Add(hacker);
            if (p == Tracker.tracker) infos.Add(tracker);
            if (p == Snitch.snitch) infos.Add(snitch);
            if (p == Jackal.jackal || (Jackal.formerJackals != null && Jackal.formerJackals.Any(x => x.PlayerId == p.PlayerId)))
            {
                if (p == Jackal.jackal && Jackal.jackal != Swooper.swooper) infos.Add(jackal);
                else if (p != Jackal.jackal) infos.Add(jackal);
            }
            if (p == Sidekick.sidekick) infos.Add(sidekick);
            if (p == Spy.spy) infos.Add(spy);
            if (p == SecurityGuard.securityGuard) infos.Add(securityGuard);
            if (p == Arsonist.arsonist) infos.Add(arsonist);
            if (p == BountyHunter.bountyHunter) infos.Add(bountyHunter);
            if (p == Vulture.vulture) infos.Add(vulture);
            if (p == Medium.medium) infos.Add(medium);
            if (p == Lawyer.lawyer && !Lawyer.isProsecutor) infos.Add(lawyer);
            if (p == Lawyer.lawyer && Lawyer.isProsecutor) infos.Add(prosecutor);
            if (p == Trapper.trapper) infos.Add(trapper);
            if (p == Pursuer.pursuer) infos.Add(pursuer);
            if (p == Thief.thief) infos.Add(thief);
            if (p == Doomsayer.doomsayer) infos.Add(doomsayer);
            if (p == Juggernaut.juggernaut) infos.Add(juggernaut);
            if (p == PlagueDoctor.plagueDoctor) infos.Add(plagueDoctor);
            if (Survivor.survivor.Any(x => x.PlayerId == p.PlayerId)) infos.Add(survivor);

            // Default roles (just impostor, just crewmate, or hunter / hunted for hide n seek, prop hunt prop ...
            if (infos.Count == count)
            {
                if (p.Data.Role.IsImpostor)
                    infos.Add(MapOptionsTor.gameMode == CustomGamemodes.HideNSeek || MapOptionsTor.gameMode == CustomGamemodes.PropHunt ? RoleInfo.hunter : RoleInfo.impostor);
                else
                    infos.Add(MapOptionsTor.gameMode == CustomGamemodes.HideNSeek ? RoleInfo.hunted : MapOptionsTor.gameMode == CustomGamemodes.PropHunt ? RoleInfo.prop : RoleInfo.crewmate);
            }

            return infos;
        }

        public static String GetRolesString(PlayerControl p, bool useColors, bool showModifier = true, bool suppressGhostInfo = false)
        {
            string roleName;
            roleName = String.Join(" ", getRoleInfoForPlayer(p, showModifier).Select(x => useColors ? Helpers.cs(x.color, x.name) : x.name).ToArray());
            if (Lawyer.target != null && p.PlayerId == Lawyer.target.PlayerId && CachedPlayer.LocalPlayer.PlayerControl != Lawyer.target)
                roleName += (useColors ? Helpers.cs(Pursuer.color, " §") : " §");
            if (HandleGuesser.isGuesserGm && HandleGuesser.isGuesser(p.PlayerId)) roleName += $" ({ModTranslation.GetString("Opt-Guesser", 9)})";
            if (!suppressGhostInfo && p != null)
            {
                if (p == Shifter.shifter && (CachedPlayer.LocalPlayer.PlayerControl == Shifter.shifter || Helpers.shouldShowGhostInfo()) && Shifter.futureShift != null)
                    roleName += Helpers.cs(Color.yellow, " ← " + Shifter.futureShift.Data.PlayerName);
                if (p == Vulture.vulture && (CachedPlayer.LocalPlayer.PlayerControl == Vulture.vulture || Helpers.shouldShowGhostInfo()))
                    roleName = roleName + Helpers.cs(Vulture.color, string.Format($" {ModTranslation.GetString("RoleInfo", 1)}", Vulture.vultureNumberToWin - Vulture.eatenBodies));
                if (Helpers.shouldShowGhostInfo())
                {
                    if (Eraser.futureErased.Contains(p))
                        roleName = Helpers.cs(Color.gray, $"{ModTranslation.GetString("RoleInfo", 2)} ") + roleName;
                    if (Vampire.vampire != null && !Vampire.vampire.Data.IsDead && Vampire.bitten == p && !p.Data.IsDead)
                        roleName = Helpers.cs(Vampire.color, $"({ModTranslation.GetString("RoleInfo", 3)} {(int)HudManagerStartPatch.vampireKillButton.Timer + 1}) ") + roleName;
                    if (Deputy.handcuffedPlayers.Contains(p.PlayerId))
                        roleName = Helpers.cs(Color.gray, $"{ModTranslation.GetString("RoleInfo", 4)} ") + roleName;
                    if (Deputy.handcuffedKnows.ContainsKey(p.PlayerId))  // Active cuff
                        roleName = Helpers.cs(Deputy.color, $"{ModTranslation.GetString("RoleInfo", 4)} ") + roleName;
                    if (p == Warlock.curseVictim)
                        roleName = Helpers.cs(Warlock.color, $"{ModTranslation.GetString("RoleInfo", 5)} ") + roleName;
                    if (p == Ninja.ninjaMarked)
                        roleName = Helpers.cs(Ninja.color, $"{ModTranslation.GetString("RoleInfo", 6)} ") + roleName;
                    if (Pursuer.blankedList.Contains(p) && !p.Data.IsDead)
                        roleName = Helpers.cs(Pursuer.color, $"{ModTranslation.GetString("RoleInfo", 7)} ") + roleName;
                    if (Witch.futureSpelled.Contains(p) && !MeetingHud.Instance) // This is already displayed in meetings!
                        roleName = Helpers.cs(Witch.color, "☆ ") + roleName;
                    if (BountyHunter.bounty == p)
                        roleName = Helpers.cs(BountyHunter.color, $"{ModTranslation.GetString("RoleInfo", 8)} ") + roleName;
                    if (Arsonist.dousedPlayers.Contains(p))
                        roleName = Helpers.cs(Arsonist.color, "♨ ") + roleName;
                    if (p == Arsonist.arsonist)
                        roleName = roleName + Helpers.cs(Arsonist.color, string.Format($" {ModTranslation.GetString("RoleInfo", 9)}", CachedPlayer.AllPlayers.Count(x => { return x.PlayerControl != Arsonist.arsonist && !x.Data.IsDead && !x.Data.Disconnected && Arsonist.dousedPlayers.All(y => y.PlayerId != x.PlayerId); })));
                    if (p == Jackal.fakeSidekick)
                        roleName = Helpers.cs(Sidekick.color, $" {ModTranslation.GetString("RoleInfo", 10)}") + roleName;
                    // Death Reason on Ghosts
                    if (p.Data.IsDead)
                    {
                        string deathReasonString = "";
                        var deadPlayer = GameHistory.deadPlayers.FirstOrDefault(x => x.player.PlayerId == p.PlayerId);

                        Color killerColor = new();
                        if (deadPlayer != null && deadPlayer.killerIfExisting != null)
                        {
                            killerColor = RoleInfo.getRoleInfoForPlayer(deadPlayer.killerIfExisting, false).FirstOrDefault().color;
                        }

                        if (deadPlayer != null)
                        {
                            switch (deadPlayer.deathReason)
                            {
                                case DeadPlayer.CustomDeathReason.Disconnect:
                                    deathReasonString = $" - {ModTranslation.GetString("RoleInfo", 11)}";
                                    break;
                                case DeadPlayer.CustomDeathReason.Exile:
                                    deathReasonString = $" - {ModTranslation.GetString("RoleInfo", 12)}";
                                    break;
                                case DeadPlayer.CustomDeathReason.Kill:
                                    deathReasonString = string.Format($" - {ModTranslation.GetString("RoleInfo", 13)}", Helpers.cs(killerColor, deadPlayer.killerIfExisting.Data.PlayerName));
                                    break;
                                case DeadPlayer.CustomDeathReason.Guess:
                                    if (deadPlayer.killerIfExisting.Data.PlayerName == p.Data.PlayerName)
                                        deathReasonString = $" - {ModTranslation.GetString("RoleInfo", 14)}";
                                    else
                                        deathReasonString = string.Format($" - {ModTranslation.GetString("RoleInfo", 15)}", Helpers.cs(killerColor, deadPlayer.killerIfExisting.Data.PlayerName));
                                    break;
                                case DeadPlayer.CustomDeathReason.Shift:
                                    deathReasonString = $" - {string.Format("{1} {0}", Helpers.cs(killerColor, deadPlayer.killerIfExisting.Data.PlayerName), Helpers.cs(Color.yellow, ModTranslation.GetString("RoleInfo", 16)))}";
                                    break;
                                case DeadPlayer.CustomDeathReason.WitchExile:
                                    deathReasonString = $" - {Helpers.cs(Witch.color, ModTranslation.GetString("RoleInfo", 17))} by {Helpers.cs(killerColor, deadPlayer.killerIfExisting.Data.PlayerName)}";
                                    break;
                                case DeadPlayer.CustomDeathReason.LoverSuicide:
                                    deathReasonString = $" - {Helpers.cs(Lovers.color, ModTranslation.GetString("RoleInfo", 18))}";
                                    break;
                                case DeadPlayer.CustomDeathReason.LawyerSuicide:
                                    deathReasonString = $" - {Helpers.cs(Lawyer.color, ModTranslation.GetString("RoleInfo", 19))}";
                                    break;
                                case DeadPlayer.CustomDeathReason.Bomb:
                                    deathReasonString = string.Format($" - {ModTranslation.GetString("RoleInfo", 20)}", Helpers.cs(killerColor, deadPlayer.killerIfExisting.Data.PlayerName));
                                    break;
                                case DeadPlayer.CustomDeathReason.Arson:
                                    deathReasonString = string.Format($" - {ModTranslation.GetString("RoleInfo", 21)}", Helpers.cs(killerColor, deadPlayer.killerIfExisting.Data.PlayerName));
                                    break;
                                case DeadPlayer.CustomDeathReason.Disease:
                                    deathReasonString = string.Format($" - {ModTranslation.GetString("RoleInfo", 22)}", Helpers.cs(killerColor, deadPlayer.killerIfExisting.Data.PlayerName));
                                    break;
                                case DeadPlayer.CustomDeathReason.Scapegoat:
                                    deathReasonString = $" - {Helpers.cs(Cupid.color, ModTranslation.GetString("RoleInfo", 23))}";
                                    break;
                                case DeadPlayer.CustomDeathReason.LoveStolen:
                                    deathReasonString = $" - {Helpers.cs(Lovers.color, ModTranslation.GetString("RoleInfo", 24))}";
                                    break;
                                case DeadPlayer.CustomDeathReason.Suicide:
                                    deathReasonString = ModTranslation.GetString("RoleInfo", 25);
                                    break;
                            }
                            roleName = roleName + deathReasonString;
                        }
                    }
                }
            }
            return roleName;
        }


        static string ReadmePage = "";
        public static async Task loadReadme()
        {
            if (ReadmePage == "")
            {
                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.GetAsync("https://raw.githubusercontent.com/FangkuaiYa/TheOtherRolesCE-Next/main/README.md");
                response.EnsureSuccessStatusCode();
                string httpres = await response.Content.ReadAsStringAsync();
                ReadmePage = httpres;
            }
        }
        public static string GetRoleDescription(RoleInfo roleInfo)
        {
            while (ReadmePage == "")
            {
            }

            int index = ReadmePage.IndexOf($"## {roleInfo.name}");
            int endindex = ReadmePage.Substring(index).IndexOf("### Game Options");
            return ReadmePage.Substring(index, endindex);

        }
    }
}

