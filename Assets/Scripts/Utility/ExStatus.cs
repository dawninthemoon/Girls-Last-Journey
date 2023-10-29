using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RieslingUtils {
    public static class ExStatus {
        public static int IncreaseByLevel(this int value, int level) {
            value += Mathf.FloorToInt(value * (level * 0.2f));
            return value;
        }

        public static float IncreaseByLevel(this float value, int level) {
            value += value * (level * 0.2f);
            return value;
        }

        public static float GetRatioByLevel(int level, int bonusTerm, int frequency, int minC) {
            float ratio = Mathf.Log10(Mathf.Abs(Mathf.Cos(level / bonusTerm) * frequency)) + minC;
            return ratio;
        }
    }
}
