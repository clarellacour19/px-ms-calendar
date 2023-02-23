using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PG.ABBs.CalendarOrganizer.Core.DTO;

namespace PG.ABBs.Calendar.Organizer.Service.Dto
{
	public class GetUserCalendarDto
	{
		
		public string locale { get; set; }
		public string uuidHash { get; set; }
		public int? limit { get; set; }
		public string sorting { get; set; }
		public string AccessToken { get; set; }
		public string? UserId { get; set; }
        public ValidationResponse ValidateObject()
        {
            ValidationResponse result = new ValidationResponse();
            try
            {
                ArgumentNullException.ThrowIfNull(AccessToken);
                return result.GetOkResponse();
            }
            catch (System.Exception e)
            {
                result = result.GetFailedResponse();
                result.Data = e.Message;
                return result;
            }
        }

    }
}