using Microsoft.AspNet.Identity;
using RecipeBox.Model.Enumerators;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeBox.Model.Models
{
    /// <summary>
    /// User account class.
    /// </summary>
    public class Account : IUser<string>
    {
        /// <summary>
        /// Account description.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.UserName ?? String.Empty;
        }


        /// <summary>
        /// The name of the account authentication/authorization provider, e.g. google.
        /// </summary>
        [Required]
        public string AuthProvider { get; set; }

        /// <summary>
        /// Account email address.
        /// </summary>
        [Required]
        [EmailAddress]
        public string EmailAddress { get; set; }

        /// <summary>
        /// Requried by IUser.
        /// Unique Id given by the authentication/authorization provider.
        /// </summary>
        [MaxLength(128)]
        [Key]
        public string Id { get; set; }

        /// <summary>
        /// Encrypted password.
        /// </summary>
        [Required]
        public string Password { get; set; }

        /// <summary>
        /// A row version property for concurency.
        /// </summary>
        [Timestamp]
        public byte[] RowVersion { get; set; }

        /// <summary>
        /// Salt value for password encryption.
        /// </summary>
        public string Salt { get; set; }

        /// <summary>
        /// AccountStatus enumerator value implemented as a string for cross platform compatibility.
        /// </summary>
        public string Status 
        {
            get { return _status; }
            set
            {
                AccountStatus status;
                if (Enum.TryParse<AccountStatus>(value, out status)) { _status = status.ToString(); }
                else { _status = AccountStatus.Unconfirmed.ToString(); }
            }
        }
        private string _status = AccountStatus.Unconfirmed.ToString();

        /// <summary>
        /// Requried by IUser.
        /// Unique name.
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string UserName { get; set; }
    }
}
