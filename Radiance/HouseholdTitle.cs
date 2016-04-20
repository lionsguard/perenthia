using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Radiance
{
    /// <summary>
    /// Gets or sets a title for a position witin a household.
    /// </summary>
    public class HouseholdTitle : GameObject
    {
        /// <summary>
        /// Gets or sets the ID of the owning household.
        /// </summary>
        public int HouseholdID { get; set; }

        /// <summary>
        /// Gets or sets the type of the current household title.
        /// </summary>
        public HouseholdTitleType Type { get; set; }

        /// <summary>
        /// Gets or sets the order of precedence for the title.
        /// </summary>
        public int Order { get; set; }  

        /// <summary>
        /// Gets or sets the ImageUri property value using the underlying Properties collection.
        /// </summary>
        public string ImageUri
        {
            get { return this.Properties.GetValue<string>(ImageUriPropertyName); }
            set { this.Properties.SetValue(ImageUriPropertyName, value); }
        }
        /// <summary>
        /// Gets the name of the ImageUri property as stored in the Properties collection.
        /// </summary>
        public const string ImageUriPropertyName = "ImageUri";

        /// <summary>   
        /// Gets or sets the Description property value using the underlying Properties collection.
        /// </summary>
        public string Description
        {
            get { return this.Properties.GetValue<string>(DescriptionPropertyName); }
            set { this.Properties.SetValue(DescriptionPropertyName, value); }
        }
        /// <summary>
        /// Gets the name of the Description property as stored in the Properties collection.
        /// </summary>
        public const string DescriptionPropertyName = "Description";

        /// <summary>
        /// Gets or sets a CanEditImage value of the Household.
        /// </summary>
        public bool CanEditImage
        {
            get { return this.Properties.GetValue<bool>(CanEditImageProperty); }
            set { this.Properties.SetValue(CanEditImageProperty, value); }
        }
        public static readonly string CanEditImageProperty = "CanEditImage";

        /// <summary>
        /// Gets or sets a CanEditMotto value of the Household.
        /// </summary>
        public bool CanEditMotto
        {
            get { return this.Properties.GetValue<bool>(CanEditMottoProperty); }
            set { this.Properties.SetValue(CanEditMottoProperty, value); }
        }
        public static readonly string CanEditMottoProperty = "CanEditMotto";

        /// <summary>
        /// Gets or sets a CanEditDescription value of the Household.
        /// </summary>
        public bool CanEditDescription
        {
            get { return this.Properties.GetValue<bool>(CanEditDescriptionProperty); }
            set { this.Properties.SetValue(CanEditDescriptionProperty, value); }
        }
        public static readonly string CanEditDescriptionProperty = "CanEditDescription";

        /// <summary>
        /// Gets or sets a CanEditTitles value of the Household.
        /// </summary>
        public bool CanEditTitles
        {
            get { return this.Properties.GetValue<bool>(CanEditTitlesProperty); }
            set { this.Properties.SetValue(CanEditTitlesProperty, value); }
        }
        public static readonly string CanEditTitlesProperty = "CanEditTitles";

        /// <summary>
        /// Gets or sets a CanEditOfficers value of the Household.
        /// </summary>
        public bool CanEditOfficers
        {
            get { return this.Properties.GetValue<bool>(CanEditOfficersProperty); }
            set { this.Properties.SetValue(CanEditOfficersProperty, value); }
        }
        public static readonly string CanEditOfficersProperty = "CanEditOfficers";

        /// <summary>
        /// Gets or sets a CanEditMembers value of the Household.
        /// </summary>
        public bool CanEditMembers
        {
            get { return this.Properties.GetValue<bool>(CanEditMembersProperty); }
            set { this.Properties.SetValue(CanEditMembersProperty, value); }
        }
        public static readonly string CanEditMembersProperty = "CanEditMembers";

        /// <summary>
        /// Gets or sets a CanEditNews value of the Household.
        /// </summary>
        public bool CanEditNews
        {
            get { return this.Properties.GetValue<bool>(CanEditNewsProperty); }
            set { this.Properties.SetValue(CanEditNewsProperty, value); }
        }
        public static readonly string CanEditNewsProperty = "CanEditNews";

        /// <summary>
        /// Gets or sets a CanAccessArmory value of the Household.
        /// </summary>
        public bool CanAccessArmory
        {
            get { return this.Properties.GetValue<bool>(CanAccessArmoryProperty); }
            set { this.Properties.SetValue(CanAccessArmoryProperty, value); }
        }
        public static readonly string CanAccessArmoryProperty = "CanAccessArmory";

        public HouseholdTitle()
        {
            this.Order = 1;
        }
    }

    public class HouseholdTitleType
    {
        public int ID { get; set; }
        public string Name { get; set; }    
    }
}
