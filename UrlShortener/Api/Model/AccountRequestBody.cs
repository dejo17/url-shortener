using System;
using System.ComponentModel.DataAnnotations;

namespace UrlShortener
{
    public class AccountRequestBody
    {
        [Required]
        [StringLength(50)]
        [MinLength(5)]
        public string AccountId { get; set; }

        public override bool Equals(object obj)
        {
            return obj is AccountRequestBody account &&
                   AccountId == account.AccountId;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(AccountId);
        }
    }
}
