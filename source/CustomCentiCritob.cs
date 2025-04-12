using Fisobs.Core;
using Fisobs.Creatures;
using Fisobs.Sandbox;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using DevInterface;
using System;
using static PathCost.Legality;
using Fisobs.Properties;
using System.Globalization;
using RWCustom;
using Random = UnityEngine.Random;
using System.Runtime.InteropServices;

namespace CustomCentisMod;

public class CustomCentiCritob : Critob, ISandboxHandler
{
    [StructLayout(LayoutKind.Sequential)]
    [method: MethodImpl(MethodImplOptions.AggressiveInlining)]
    public class CustomCentiProperties(CustomCentiBreedParams props) : ItemProperties
    {
        public CustomCentiBreedParams Props = props;

        public override void Grabability(Player player, ref Player.ObjectGrabability grabability)
        {
            if (Props.Flags2.Get(Grabability_HasValue))
                grabability = Props.Grabability;
        }

        public override void Throwable(Player player, ref bool throwable)
        {
            if (Props.Flags2.Get(Throwable_HasValue))
                throwable = Props.Flags2.Get(CustomCentiBreedParams.Throwable);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    [method: MethodImpl(MethodImplOptions.AggressiveInlining)]
    public class CustomCentiIcon(CustomCentiBreedParams props) : Icon
    {
        public CustomCentiBreedParams Props = props;

        public override int Data(AbstractPhysicalObject apo)
        {
            var props = Props;
            if (props.BodySizeGenerationType is not SizeGenerationType.StaticMin and not SizeGenerationType.StaticMax)
            {
                var dif = Above0(props.MaxSize) - Above0(props.MinSize);
                var num = Centipede.GenerateSize(apo as AbstractCreature);
                if (num < Above0(props.MinSize) + dif * .255f)
                    return 1;
                if (num < Above0(props.MinSize) + dif * .6f)
                    return 2;
                return 3;
            }
            return 0;
        }

        public override Color SpriteColor(int data)
        {
            var props = Props;
            if (props.BodySizeGenerationType is not SizeGenerationType.StaticMin and not SizeGenerationType.StaticMax)
            {
                if (data == 1)
                    return Clamp01(props.SmallIconColor);
                if (data == 3)
                    return Clamp01(props.BigIconColor);
            }
            return Clamp01(props.StandardIconColor);
        }

        public override string SpriteName(int data)
        {
            var props = Props;
            if (props.BodySizeGenerationType is not SizeGenerationType.StaticMin and not SizeGenerationType.StaticMax)
            {
                if (data == 1)
                    return SpriteExists(props.SmallIconSprite) ? props.SmallIconSprite! : s_Kill_Centipede1;
                if (data == 3)
                    return SpriteExists(props.BigIconSprite) ? props.BigIconSprite! : s_Kill_Centipede3;
            }
            return SpriteExists(props.StandardIconSprite) ? props.StandardIconSprite! : s_Kill_Centipede2;
        }
    }

    public CustomCentiBreedParams Props;
    public ItemProperties ItemProps;
    readonly string[] _aliases;
    internal SandboxUnlock? _bigUnlock, _standardUnlock, _smallUnlock;

    public CustomCentiCritob(CustomCentiBreedParams props) : base(props.CreatureType)
    {
        props._critob = this;
        Props = props;
        ItemProps = new CustomCentiProperties(props);
        Icon = new CustomCentiIcon(props);
        ShelterDanger = props.Flags.Get(SmallFood) ? ShelterDanger.Safe : ShelterDanger.Hostile;
        if (s_majorCreatures is HashSet<CreatureTemplate.Type> set && props.Flags.Get(MajorCreature))
            set.Add(Type);
        if (props.UnlockID is MultiplayerUnlocks.SandboxUnlockID id && MultiplayerUnlocks.CreatureUnlockList is List<MultiplayerUnlocks.SandboxUnlockID> l)
        {
            var sb = (this as ISandboxHandler).SandboxUnlocks;
            var hidden = props.Flags.Get(KillScoreHidden);
            if (props.BodySizeGenerationType is not SizeGenerationType.StaticMin and not SizeGenerationType.StaticMax && Props.Flags.Get(UsesUnlockData))
            {
                if (props.SmallUnlockID is MultiplayerUnlocks.SandboxUnlockID idSmall && !l.Contains(idSmall))
                {
                    RegisterUnlock(hidden ? KillScore.Constant(Above0(props.SmallKillScore)) : KillScore.Configurable(Above0(props.SmallKillScore)), idSmall, props.SmallParentUnlock, 1);
                    l.Add(idSmall);
                    _smallUnlock = sb[sb.Count - 1];
                }
                if (!l.Contains(id))
                {
                    RegisterUnlock(hidden ? KillScore.Constant(Above0(props.StandardKillScore)) : KillScore.Configurable(Above0(props.StandardKillScore)), id, props.ParentUnlock, 2);
                    l.Add(id);
                    _standardUnlock = sb[sb.Count - 1];
                }
                if (Props.BigUnlockID is MultiplayerUnlocks.SandboxUnlockID idBig && !l.Contains(idBig))
                {
                    RegisterUnlock(hidden ? KillScore.Constant(Above0(props.BigKillScore)) : KillScore.Configurable(Above0(props.BigKillScore)), idBig, props.BigParentUnlock, 3);
                    l.Add(idBig);
                    _bigUnlock = sb[sb.Count - 1];
                }
            }
            else if (!l.Contains(id))
            {
                RegisterUnlock(hidden ? KillScore.Constant(Above0(props.StandardKillScore)) : KillScore.Configurable(Above0(props.StandardKillScore)), id, props.ParentUnlock);
                l.Add(id);
                _standardUnlock = sb[sb.Count - 1];
            }
            SandboxPerformanceCost = new(Above0(props.LinearSandboxCost), Above0(props.ExponentialSandboxCost));
        }
        LoadedPerformanceCost = Above0(props.RoomPerformanceCost);
        _aliases = [Type.value.ToLower().Trim()];
    }

    public override IEnumerable<RoomAttractivenessPanel.Category> DevtoolsRoomAttraction() => [];

    public override void KillsMatter(ref bool killsMatter) => killsMatter = Props.Flags.Get(CountsAsAKill);

    public override int ExpeditionScore() => Above0(Props.ExpeditionScore);

    public override Color DevtoolsMapColor(AbstractCreature acrit) => Clamp01(Props.DevColor);

    public override string DevtoolsMapName(AbstractCreature acrit) => string.IsNullOrEmpty(Props.DevName) ? Type.value : Props.DevName!;

    public override IEnumerable<string> WorldFileAliases() => _aliases;

    public override CreatureTemplate CreateTemplate()
    {
        var props = Props;
        props._defaultRel._type = CreatureTemplate.Relationship.Type.Eats;
        props._defaultRel._intensity = 1f;
        var t = new CreatureFormula((props.ParentType is not CreatureTemplate.Type tp || tp.Index == -1) ? null : tp, Type, Type.ToString())
        {
            TileResistances = new()
            {
                OffScreen = new() { resistance = 1f, legality = Allowed },
                Floor = new() { resistance = 1f, legality = props.Flags.Get(WaterOnly) ? Unwanted : Allowed },
                Corridor = new() { resistance = 1f, legality = props.Flags.Get(WaterOnly) ? Unwanted : Allowed },
                Climb = props.Flags.Get(WaterOnly) ? new() { resistance = 10f, legality = Unwanted } : new() { resistance = 1f, legality = Allowed },
                Wall = props.Flags.Get(WaterOnly) ? new() { resistance = 100f, legality = Unwanted } : new() { resistance = 1f, legality = Allowed },
                Ceiling = props.Flags.Get(WaterOnly) ? new() { resistance = 100f, legality = Unwanted } : new() { resistance = 1f, legality = Allowed },
                Air = props.Flags.Get(Flying) ? new() { resistance = 1f, legality = Allowed } : new(60f, IllegalTile)
            },
            ConnectionResistances = new()
            {
                Standard = new() { resistance = 1f, legality = Allowed },
                OpenDiagonal = new() { resistance = 3f, legality = Allowed },
                ReachOverGap = new() { resistance = 3f, legality = Allowed },
                DoubleReachUp = new() { resistance = 2f, legality = Allowed },
                SemiDiagonalReach = new() { resistance = 2f, legality = Allowed },
                NPCTransportation = new() { resistance = props.Flags.Get(Flying) ? 10f : 25f, legality = Allowed },
                OffScreenMovement = new() { resistance = 1f, legality = Allowed },
                BetweenRooms = new() { resistance = 10f, legality = Allowed },
                Slope = new() { resistance = 1.5f, legality = Allowed },
                DropToFloor = new() { resistance = 5f, legality = Allowed },
                DropToWater = props.Flags.Get(Swimming) ? new() { resistance = 1f, legality = Allowed } : new() { resistance = 100f, legality = IllegalConnection },
                DropToClimb = new() { resistance = 5f, legality = Allowed },
                ShortCut = new() { resistance = 1f, legality = Allowed },
                ReachUp = new() { resistance = 1.1f, legality = Allowed },
                ReachDown = new() { resistance = 1.1f, legality = Allowed },
                CeilingSlope = new() { resistance = 2f, legality = Allowed }
            },
            DefaultRelationship = new() { type = CreatureTemplate.Relationship.Type.Eats, intensity = 1f },
            DamageResistances = new() { Base = Above0(props.BaseDamageResistance), Explosion = Above0(props.ExplosionResistance), Electric = 102f, Water = props.Flags.Get(Swimming) ? 102f : 0f },
            StunResistances = new() { Base = Above0(props.BaseStunResistance), Explosion = Above0(props.ExplosionStunResistance), Electric = 102f, Water = props.Flags.Get(Swimming) ? 102f : 0f },
            HasAI = true,
            Pathing = PreBakedPathing.Ancestral(props.Flags.Get(Flying) ? (props.Flags.Get(Swimming) ? CreatureTemplate.Type.BigEel : CreatureTemplate.Type.Fly) : (props.Flags.Get(Swimming) ? CreatureTemplate.Type.Leech : CreatureTemplate.Type.BlueLizard))
        }.IntoTemplate();
        t.quickDeath = false;
        t.offScreenSpeed = .3f;
        t.grasps = 2;
        t.abstractedLaziness = Above0(props.AbstractedLaziness);
        t.bodySize = Above0(props.BodySizeEstimate);
        t.shortcutSegments = Above1(props.ShortCutSegments);
        t.doubleReachUpConnectionParams = StaticWorld.GetCreatureTemplate(CreatureTemplate.Type.BlueLizard).doubleReachUpConnectionParams;
        t.visualRadius = Above0(props.VisualRadius);
        t.waterVision = Above0(props.WaterVision);
        t.throughSurfaceVision = Above0(props.ThroughSurfaceVision);
        t.movementBasedVision = Above0(props.MovementBasedVision);
        t.dangerousToPlayer = Above0(props.DangerousToPlayer);
        t.shortcutColor = Clamp01(props.ShortCutColor);
        t.communityInfluence = Above0(props.CommunityInfluence);
        t.meatPoints = props.Flags.Get(SmallFood) ? 0 : 1;
        t.lungCapacity = Above0(props.LungCapacity);
        t.waterRelationship = props.Flags.Get(WaterOnly) ? CreatureTemplate.WaterRelationship.WaterOnly : (props.Flags.Get(Swimming) ? CreatureTemplate.WaterRelationship.Amphibious : CreatureTemplate.WaterRelationship.AirAndSurface);
        t.canSwim = true;
        t.BlizzardWanderer = props.Flags.Get(BlizzardWanderer);
        t.BlizzardAdapted = props.Flags2.Get(HypothermiaImmune_HasValue | HypothermiaImmune);
        t.stowFoodInDen = false;
        t.usesNPCTransportation = true;
        t.jumpAction = "Swap Heads";
        t.pickupAction = "Grab/Shock";
        t.throwAction = "Release";
        t.communityID = props.Flags.Get(IgnoresCommunity) ? CreatureCommunities.CommunityID.None : CreatureCommunities.CommunityID.All;
        t.roamBetweenRoomsChance = props.Flags.Get(TriesToStayInRoom) ? -1f : .1f;
        t.instantDeathDamageLimit = props.Flags.Get(EasyKill) ? 1f : float.MaxValue;
        t.countsAsAKill = !props.Flags.Get(CountsAsAKill) ? 0 : 2;
        t.canFly = props.Flags.Get(Flying);
        t.breedParameters = props;
        t.wormgrassTilesIgnored = props.Flags.Get(WormGrassImmune);
        t.wormGrassImmune = props.Flags.Get(WormGrassImmune);
        t.forbidStandardShortcutEntry = props.Flags.Get(ForbidStandardShortcutEntry);
        t.scaryness = props.Scaryness;
        t.meatPoints = props.Flags.Get(SmallFood) ? 0 : 1; // just to be able to eat custom centi corpses (if not small food), the amount of meat points here is overriden by Centipede.ctor
        props._template = t;
        return t;
    }

    public void ResetRelationships(CreatureTemplate? ancestor)
    {
        var props = Props;
        var myRels = props._template.relationships;
        if (ancestor is null)
        {
            var def = new CreatureTemplate.Relationship() { type = props._defaultRel._type, intensity = props._defaultRel._intensity };
            for (var i = 0; i < myRels.Length; i++)
                myRels[i] = def;
        }
        else
        {
            Array.Copy(ancestor.relationships, 0, myRels, 0, ancestor.relationships.Length);
            if (Props._nonMSCAqua)
                EstablishRelationship(Type, CreatureTemplate.Type.BigEel, new() { type = CreatureTemplate.Relationship.Type.Afraid, intensity = 1f });
        }
        props._realRelationships.Clear();
    }

    //keep initial state of rels
    public override void EstablishRelationships()
    {
        if (Props._nonMSCAqua)
            EstablishRelationship(Type, CreatureTemplate.Type.BigEel, new() { type = CreatureTemplate.Relationship.Type.Afraid, intensity = 1f });
        Props._realRelationships = [];
        SetCustomRelationships();
    }

    public void SetCustomRelationships()
    {
        var tp = Type;
        var rels = Props.Relationships;
        var rrels = Props._realRelationships;
        if (rels is not null)
        {
            for (var i = 0; i < rels.Length; i++)
            {
                var rel = rels[i];
                var rela = new RelationValue() { _type = rel.Type, _intensity = Clamp01(rel.Intensity) };
                if (rel.ActivePosition)
                    rrels[new() { _a = tp, _b = rel.Target }] = rela;
                else
                    rrels[new() { _a = rel.Target, _b = tp }] = rela;
            }
        }
    }

    public static void EstablishRelationship(CreatureTemplate.Type a, CreatureTemplate.Type b, in CreatureTemplate.Relationship relationship)
    {
        CreatureTemplate creatureTemplate = StaticWorld.GetCreatureTemplate(a), creatureTemplate2 = StaticWorld.GetCreatureTemplate(b);
        if (creatureTemplate is not null && creatureTemplate2 is not null)
        {
            creatureTemplate.relationships[b.Index] = relationship;
            var array = StaticWorld.creatureTemplates;
            for (var i = 0; i < array.Length; i++)
            {
                var creatureTemplate3 = array[i];
                if (creatureTemplate3?.ancestor is CreatureTemplate ancestor)
                {
                    if (ancestor == creatureTemplate)
                        EstablishRelationship(creatureTemplate3, creatureTemplate2, relationship);
                    else if (ancestor == creatureTemplate2)
                        EstablishRelationship(creatureTemplate, creatureTemplate3, relationship);
                }
            }
        }
    }

    public static void EstablishRelationship(CreatureTemplate creatureTemplate, CreatureTemplate creatureTemplate2, in CreatureTemplate.Relationship relationship)
    {
        if (creatureTemplate is not null && creatureTemplate2 is not null)
        {
            creatureTemplate.relationships[creatureTemplate2.type.Index] = relationship;
            var array = StaticWorld.creatureTemplates;
            for (var i = 0; i < array.Length; i++)
            {
                var creatureTemplate3 = array[i];
                if (creatureTemplate3?.ancestor is CreatureTemplate ancestor)
                {
                    if (ancestor == creatureTemplate)
                        EstablishRelationship(creatureTemplate3, creatureTemplate2, relationship);
                    else if (ancestor == creatureTemplate2)
                        EstablishRelationship(creatureTemplate, creatureTemplate3, relationship);
                }
            }
        }
    }

    public override ArtificialIntelligence CreateRealizedAI(AbstractCreature acrit) => new CentipedeAI(acrit, acrit.world);

    public override Creature CreateRealizedCreature(AbstractCreature acrit) => new Centipede(acrit, acrit.world);

    public override CreatureState CreateState(AbstractCreature acrit) => new Centipede.CentipedeState(acrit); // even for small ones to avoid crashes in vanilla code

    public override void LoadResources(RainWorld rainWorld) { }

    public override CreatureTemplate.Type? ArenaFallback() => (Props.ParentType is not CreatureTemplate.Type tp || tp.Index == -1) ? CreatureTemplate.Type.Centipede : tp;

    public override ItemProperties? Properties(Creature crit) => ItemProps;

    AbstractWorldEntity ISandboxHandler.ParseFromSandbox(World world, EntitySaveData data, SandboxUnlock unlock)
    {
        var text = $"{data.CustomData}SandboxData<cC>{unlock.Data}<cB>";
        var abstractCreature = new AbstractCreature(world, Props._template, null, data.Pos, data.ID)
        {
            pos = data.Pos // needed
        };
        abstractCreature.state.LoadFromString(text.Split(s_cb, StringSplitOptions.RemoveEmptyEntries));
        abstractCreature.setCustomFlags();
        if (Props.BodySizeGenerationType is not SizeGenerationType.StaticMin and not SizeGenerationType.StaticMax && Props.Flags.Get(UsesUnlockData))
        {
            var num = 0f;
            if (unlock.Data == 2)
                num = Lerp(.265f, .55f, (float)Math.Pow(Custom.ClampedRandomVariation(.5f, .5f, .7f), 1.2f));
            else if (unlock.Data == 3)
                num = Lerp(.7f, 1f, (float)Math.Pow(Random.value, .6f));
            abstractCreature.spawnData = string.Format(CultureInfo.InvariantCulture, "{{{0}}}", num);
        }
        return abstractCreature;
    }
}