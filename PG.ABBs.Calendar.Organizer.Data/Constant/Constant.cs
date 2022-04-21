// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Constant.cs" company="The Procter & Gamble Company">
//   Copyright © The Procter & Gamble Company 2020 - All Rights Reserved.
// </copyright>
// <summary>
//   Defines the Constant type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace PG.ABBs.Calendar.Organizer.Data.Constant
{
    public static class Constant
    {
        #region Constants

        public const string ConnectionString = "DatabaseSettings:ConnectionString";

        public const string KeyVaultConnectionString = "DatabaseSettings-ConnectionString";
        public const string KeyVaultStorageConnectionString = "Azurestorage-ConnectionString";

        public const string KeyVaultDeliveryApiKey = "Contentful-DeliveryApiKey";
        public const string KeyVaultPreviewApiKey = "Contentful-PreviewApiKey";
        public const string KeyVaultManagementApiKey = "Contentful-ManagementApiKey";



        public const string DbSettings = "DatabaseSettings";

        public static class DatabaseObject
        {
            public static class StoredProcedure
            {
	            public const string GetEvents = "GetEvents";
	            public const string GetCalendars = "GetCalendars";
	            public const string GetAllCalendars = "GetAllCalendars";
	            public const string GetUserCalendars = "GetUserCalendars";
	           

	            public const string AddOrUpdateEvents = "AddOrUpdateEvents";
	            public const string AddOrUpdateCalendars = "AddOrUpdateCalendars";
	            public const string AddOrUpdateUserCalendar = "AddOrUpdateUserCalendar";

                
                public const string DeleteEvent = "DeleteEvent";
                public const string DeleteCalendar = "DeleteCalendar";

            }
        }

        #endregion
    }
}
