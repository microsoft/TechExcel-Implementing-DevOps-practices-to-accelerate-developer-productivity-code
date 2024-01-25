using System.ComponentModel.DataAnnotations;

namespace RazorPagesTestSample.Data
{
    #region snippet1
    public class Message
    {
        // a new comment for issue 9
        public int Id { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [StringLength(250, ErrorMessage = "There's a 250 character limit on messages!. Please shorten your message to enable this to work.")]
        public string Text { get; set; }
    }
    #endregion
}
