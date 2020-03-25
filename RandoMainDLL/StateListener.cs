﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RandoMainDLL.Memory;
namespace RandoMainDLL
{
    public static class StateListener
    {
        public static Dictionary<long, UberState> UberStates = new Dictionary<long, UberState>();
        public static bool Update() {
            bool grantedPickup = false;
            var memory = Randomizer.Memory;
            Dictionary<long, UberState> uberStates = memory.GetUberStates();
            foreach (KeyValuePair<long, UberState> pair in uberStates) {
                long key = pair.Key;
                UberState state = pair.Value;
                if (state.GroupName == "statsUberStateGroup" || (state.GroupName == "achievementsGroup" && state.Name == "spiritLightGainedCounter"))
                    continue;
                UberState oldState = null;
                if (UberStates.TryGetValue(key, out oldState)) {
                    UberValue value = state.Value;
                    UberValue oldValue = oldState.Value;
                    if (value.Int != oldValue.Int) {
                        grantedPickup = SeedManager.OnUberState(state) || grantedPickup;
                        Randomizer.Log($"{state.GroupName}.{state.Name} ({state.GroupID},{state.ID}): {oldValue.Int}->{value.Int}", false);
                        UberStates[key].Value = state.Value;
                    }
                } else {
                    UberStates[key] = state.Clone();
                }
            }
            return grantedPickup;

        }
    }
}
