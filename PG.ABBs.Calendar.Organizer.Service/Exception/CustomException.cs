// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CustomException.cs" company="The Procter & Gamble Company">
//   Copyright © The Procter & Gamble Company 2020 - All Rights Reserved.
// </copyright>
// <summary>
//   Define a custom exception format.
// </summary>
// -------------------------------------------------------------------------------------------------------------------- 

namespace PG.ABBs.Calendar.Organizer.Service.Exception
{
	using System;
	using System.Collections.Generic;

	public class CustomException : Exception
	{
		public CustomException(string message, IList<Exception> exceptionList) : base(message)
		{
			this.ExceptionList = exceptionList;
		}

		public IList<Exception> ExceptionList { get; set; }
	}
}