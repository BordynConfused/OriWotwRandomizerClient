﻿using System;
using System.Collections.Generic;
using System.IO;
using RandoMainDLL.Memory;
namespace RandoMainDLL {
    public static class SeedController {
        public enum FakeUberGroups : int {
            TREE = 0,
            OPHER_WEAPON = 1,
            TWILLEN_SHARD = 2
        }
        public static Dictionary<UberId, Pickup> pickupMap = new Dictionary<UberId, Pickup>();
        public static void ReadSeed() {
            var seedName = File.ReadAllText(Randomizer.SeedNameFile);
            if (seedName.Trim() != "") {
                pickupMap.Clear();
                foreach (var line in File.ReadLines(Randomizer.SeedFile)) {
                    try {
                        var frags = line.Split('|');
                        var uberId = new UberId(int.Parse(frags[0]), int.Parse(frags[1]));
                        var pickupType = (PickupType)byte.Parse(frags[2]);
                        //                    Randomizer.Log($"uberId {uberId} -> {pickupType} {frags[3]}");
                        pickupMap[uberId] = BuildPickup(pickupType, frags[3]);
                    } catch (Exception e) {
                        Randomizer.Log($"Error parsing line: '{line}'\nError: {e.Message} \nStacktrace: {e.StackTrace}", false);
                    }
                }
                AHK.Print($"Seed {seedName} loaded", 300);
            } else
                AHK.Print($"No seed loaded; Download a .wotwr file and double-click it to load one", 360);
        }
        public static Sellable OpherWeapon(AbilityType ability) {
            UberId fakeId = new UberId((int)FakeUberGroups.OPHER_WEAPON, (int)ability);
            if (pickupMap.TryGetValue(fakeId, out Pickup p) && p is Sellable) {
                return p as Sellable;
            }
            Randomizer.Log($"Couldn't find a valid Sellable for {ability}...");
            return new Resource(ResourceType.Energy);
        }
        public static Sellable TwillenShard(ShardType shard) {
            UberId fakeId = new UberId((int)FakeUberGroups.TWILLEN_SHARD, (int)shard);
            if (pickupMap.TryGetValue(fakeId, out Pickup p) && p is Sellable) {
                return p as Sellable;
            }
            Randomizer.Log($"Couldn't find a valid Sellable for {shard}...");
            return new Resource(ResourceType.Energy);
        }
        public static void OnTree(AbilityType ability) {
            UberId fakeId = new UberId((int)FakeUberGroups.TREE, (int)ability);
            if (pickupMap.TryGetValue(fakeId, out Pickup p)) {
                AHK.Print(p.ToString());
                p.Grant();
                Randomizer.PleaseSave = true;
            } else {
                Randomizer.Log($"Tree {ability} not found in seed. Get a seed from seedpack 10 or later.");
            }
        }
        public static void OnUberState(UberState state) {
            var id = state.GetUberId();
            if (pickupMap.TryGetValue(id, out Pickup p)) {
                AHK.Print(p.ToString());
                p.Grant();
                Randomizer.PleaseSave = true;
            }
        }
        public static Pickup BuildPickup(PickupType type, String pickupData) {
            switch (type) {
                case PickupType.Ability:
                    return new Ability((AbilityType)byte.Parse(pickupData));
                case PickupType.Shard:
                    return new Shard((ShardType)byte.Parse(pickupData));
                case PickupType.SpiritLight:
                    return new Cash(int.Parse(pickupData));
                case PickupType.Resource:
                    return new Resource((ResourceType)byte.Parse(pickupData));
                case PickupType.Teleporter:
                    return new Teleporter((TeleporterType)byte.Parse(pickupData));
                default:
                    throw new NotImplementedException("Unknown Pickup");
            }
        }
    }
}
