// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Constants.cs" company="The Procter & Gamble Company">
//   Copyright © The Procter & Gamble Company 2020 - All Rights Reserved.
// </copyright>
// <summary>
//   Defines the Constants for JobApi project.
// </summary>
// --------------------------------------------------------------------------------------------------------------------


namespace PG.ABBs.Calendar.Organizer.API
{
	using System;

	public static class Constants
	{
		#region Constants

		public const string Separator = ";..;";

		#endregion


		public struct ErrorCodes
		{
			#region Constants

			public const string BadParameters = "BadParameters";

			public const string Failed = "Failed";

			public const string Ko = "KO";

			public const string NoResultsFound = "NoResultsFound";

			public const string Ok = "OK";

			public const string UnexpectedError = "UnexpectedError";

			#endregion
		}


		public const string Authority = "JwtSettings:Authority";
		public const string Audience = "JwtSettings:Audience";
		public const string ClientSecret = "JwtSettings:ClientSecret";
		public const string ClientID = "JwtSettings:ClientID";
	}
}