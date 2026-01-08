using System;

namespace Progress {
	public static class ProgressExtensions {
		public static void Kill(this Progressable progress) {
			progress.Decrease(progress.Current);
		}

		public static bool IsAlive(this Progressable progress) {
			return progress.Current > 0;
		}
		
		public static bool HasFullDurability(this Progressable progress, float tolerance = 0.01f) {
			return Math.Abs(progress.Total - progress.Current) < tolerance;
		}
	
		public static bool IsDead(this Progressable progress) {
			return progress.Current <= 0;
		}
	}
}