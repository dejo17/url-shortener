using System;
using System.ComponentModel.DataAnnotations;

namespace UrlShortener
{
    public class AccountRequestBody
    {
        [Required]
        [StringLength(50)]
        public string AccountId { get; set; }

        /*
         * Equals and GetHashCode su overridane metode (ctrl + . za automatsko generiranje) koje moraju
         * biti tu zbog usporedivanja objekata jednog s drugim, npr kada koristimo List.Contains
         */
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
