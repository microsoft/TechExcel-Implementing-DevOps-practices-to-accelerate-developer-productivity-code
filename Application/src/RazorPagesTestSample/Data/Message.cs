using System.ComponentModel.DataAnnotations;

namespace RazorPagesTestSample.Data
{
    #region snippet1
    public class Message
    {
        public int Id { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [StringLength(201, ErrorMessage = "There's a 201 character limit on messages. Please shorten your message.")]
        public string Text { get; set; }
    }
    #endregion
}
