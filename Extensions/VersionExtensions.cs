using System;

namespace GodLesZ.Library {

	public static class VersionExtensions {

		public static int ToInt(this Version version) {
			long longVesion = version.Major * 100000000L;
			longVesion += version.Minor * 1000000L;
			longVesion += version.Build * 100L;
			longVesion += version.Revision;

			return (int)longVesion;
		}

	}

}
