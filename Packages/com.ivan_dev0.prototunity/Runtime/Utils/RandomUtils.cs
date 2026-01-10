using UnityEngine;

namespace PrototUnity.Utils {
	public static class RandomUtils {
		public static Vector3 Range(Vector3 min, Vector3 max) {
			var x = Random.Range(min.x, max.x);
			var y = Random.Range(min.y, max.y);
			var z = Random.Range(min.z, max.z);
			return new Vector3(x, y, z);
		}
		
		public static T GetRandom<T>(this T[] array) {
			return array[Random.Range(0, array.Length)];
		}
	}
}